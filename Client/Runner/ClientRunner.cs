using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Client.Runner
{
    public interface IClientRunner {
        Task Run(string serverUrl, int port);
    }

    public class ClientRunner : IClientRunner
    {
        private readonly ILogger<ClientRunner> _logger;

        public ClientRunner(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ClientRunner>();
        }

        public async Task Run(string serverUrl, int port)
        {
            var tcpClient = new TcpClient();
            tcpClient.Connect(serverUrl, port);
            var stream = tcpClient.GetStream();
            string returndata = "";
            while(string.IsNullOrWhiteSpace(returndata)) {
                byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                stream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);
                returndata = Encoding.UTF8.GetString(bytes);
                _logger.LogInformation(returndata);
                await WriteAsync(returndata);
            }
            var clientNumber = await GetInputAsync();
            await SendDataAsync(tcpClient, clientNumber);
            var nextResponse = "";
            while(string.IsNullOrWhiteSpace(nextResponse)) {
                byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                stream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);
                nextResponse = Encoding.UTF8.GetString(bytes);
                _logger.LogInformation(nextResponse);
                await WriteAsync(nextResponse);
            }
            await WriteAsync("Type something to close the client...");
            await GetInputAsync();
            tcpClient.Close();
        }

        public Task SendDataAsync(TcpClient tcp, string command)
        {
            var socketAsyncEventArgs = new SocketAsyncEventArgs();
            var buffer = Encoding.UTF8.GetBytes(command);
            socketAsyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
            var tcs = new TaskCompletionSource<object>();
            if(!tcp.Client.SendAsync(socketAsyncEventArgs))
            {
                return Task.CompletedTask;
            }
            else
            {
                return tcs.Task;
            }
        }

        private async Task<string> GetInputAsync() {
            return await Task.Run(() => Console.ReadLine());
        }

        private async Task WriteAsync(string message) {
            await Task.Run(() => Console.WriteLine(message));
        }
    }
}