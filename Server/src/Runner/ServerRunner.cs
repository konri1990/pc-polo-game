using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Server.Game;

namespace Server.Runner
{
    public interface IServerRunner
    {
        Task Run();
    }

    internal class ServerRunner : IServerRunner, IDisposable
    {
        private readonly ILogger<ServerRunner> _logger;
        private readonly IPoloGameEngine _game;
        private readonly ConcurrentDictionary<TcpClient, string> _clients;

        public ServerRunner(ILoggerFactory loggerFactory, IPoloGameEngine game)
        {
            _logger = loggerFactory.CreateLogger<ServerRunner>();
            _clients = new ConcurrentDictionary<TcpClient, string>();
            _game = game;
        }

        public Task Run()
        {
            var port = 8990;
            var ip = "127.0.0.1";
            IPAddress localAddr = IPAddress.Parse(ip);
            var server = new TcpListener(localAddr, port);
            server.Start();
            _logger.LogInformation($"Server started: {ip}:{port}");
            return StartListener(server);
        }

        private async Task StartListener(TcpListener server)
        {
            try
            {
                var gameEnded = false;
                while (!gameEnded)
                {
                    if (!_game.IsGameReadyToStart)
                    {
                        _logger.LogInformation("Waiting for a connection...");
                        TcpClient client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                        var clientId = Guid.NewGuid().ToString();
                        _logger.LogInformation($"Client: {clientId} connected!");
                        _clients.TryAdd(client, clientId);
                        _game.AddPlayer(clientId);
                        HandleDevice(client, clientId);
                    }
                    else
                    {
                        var tasksHandleGameForPlayers = new List<Task<bool>>();
                        foreach (var client in _clients)
                        {
                            tasksHandleGameForPlayers.Add(HandleGameForPlayer(client.Key, client.Value));
                        }
                        var results = await Task.WhenAll(tasksHandleGameForPlayers);
                        if(results.All(x => x))
                        {
                            foreach (var client in _clients)
                            {
                                await NotifyPlayers(client.Key, client.Value).ConfigureAwait(false);
                            }
                            gameEnded = true;
                        }
                    }
                }
                _logger.LogInformation("Game ended!");
            }
            catch (SocketException e)
            {
                _logger.LogError("SocketException: {0}", e);
                server.Stop();
            }
        }

        private Task NotifyPlayers(TcpClient client, string clientId)
        {
            var stream = client.GetStream();
            Byte[] bytes = new Byte[40];
            try
            {
                var result = "";
                switch(_game.GameResultForPlayer(clientId)) {
                    case GameResulsForPlayer.Won:
                        result = $"Congratulation {clientId} you won!";
                        break;
                    case GameResulsForPlayer.Lost:
                        result = $"Unfortunately {clientId} you lost :(";
                        break;
                    default:
                        result = "Game crashed...";
                        break;
                }
                string str = $"{result} Game ended! Reconnect to start new one...";
                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                stream.Write(reply, 0, reply.Length);
                _logger.LogInformation("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError("Exception: {0}", e.ToString(), e);
                return Task.FromException(e);
            }
        }

        private Task<bool> HandleGameForPlayer(TcpClient client, string clientId)
        {
            var stream = client.GetStream();
            string imei = String.Empty;
            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                string str = $"Welcome in PoloGame! Send your number to the server...!";
                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                stream.Write(reply, 0, reply.Length);
                _logger.LogInformation("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    _logger.LogInformation("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);
                    return Task.FromResult(_game.SetPlayerNumber(clientId, Convert.ToInt32(data)));
                }
                return Task.FromResult(false);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception: {0}", e.ToString(), e);
                return Task.FromResult(false);
            }
        }

        public void HandleDevice(TcpClient client, string clientId)
        {
            var stream = client.GetStream();
            try
            {
                string str = $"Hey player, your id is: {clientId}!";
                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                stream.Write(reply, 0, reply.Length);
                _logger.LogInformation("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception e)
            {
                _logger.LogError("Exception: {0}", e.ToString(), e);
                client.Close();
            }
        }

        public void Dispose()
        {
            foreach (var tcpClient in _clients.Keys)
            {
                tcpClient.Dispose();
            }
        }
    }
}