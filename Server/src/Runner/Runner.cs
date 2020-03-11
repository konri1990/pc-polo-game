using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Server.Runner
{
    public interface IServerRunner
    {
        Task Run();
    }

    internal class ServerRunner : IServerRunner
    {
        private readonly ILogger<ServerRunner> _logger;
        private readonly ObservableCollection<TcpClient> _clients;

        public ServerRunner(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServerRunner>();
            _clients = new ObservableCollection<TcpClient>();
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
                    _clients.Add(client);
                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                }
            }
            catch (SocketException e)
            {
                _logger.LogError("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string imei = String.Empty;
            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    _logger.LogInformation("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);
                    string str = "Hey Device!";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                    _logger.LogInformation("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
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