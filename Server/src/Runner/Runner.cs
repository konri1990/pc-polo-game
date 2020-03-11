using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

    internal class ServerRunner : IServerRunner
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
                while (true)
                {
                    _logger.LogInformation("Waiting for a connection...");
                    TcpClient client = await server.AcceptTcpClientAsync();
                    _logger.LogInformation("Connected!");
                    var clientId = new Guid().ToString();
                    _clients.TryAdd(client, clientId);
                    _game.AddPlayer(clientId);
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDevice));
                }
            }
            catch (SocketException e)
            {
                _logger.LogError("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDevice(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string imei = String.Empty;
            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                if(_game.IsGameReadyToStart)
                {
                    var clientId = _clients[client];
                    string str = $"Your id is{_clients}: Welcome in PoloGame! Send your number to the server.";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                }
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    _logger.LogInformation("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);
                    string str = "Hey Device!";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                    _logger.LogInformation("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                    //todo somewhere here client engine
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Exception: {0}", e.ToString(), e);
                client.Close();
            }
        }
    }
}