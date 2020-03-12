using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // new Thread(() =>
            // {
            //     Thread.CurrentThread.IsBackground = true;
            //     Connect("127.0.0.1", "Hello I'm Device 1...");
            // }).Start();
            // new Thread(() =>
            // {
            //     Thread.CurrentThread.IsBackground = true;
            //     Connect("127.0.0.1", "Hello I'm Device 2...");
            // }).Start();
            await Connect("127.0.0.1", "Hello I'm Device 1...");
            Console.ReadLine();
        }

        private static Task Connect(string server, string message)
        {
            try
            {
                Int32 serverPort = 8990;
                TcpClient client = new TcpClient();
                client.Connect(server, serverPort);
                NetworkStream stream = client.GetStream();
                int count = 0;
                while (count++ < 3)
                {
                    // Translate the Message into ASCII.
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                    // Send the message to the connected TcpServer. 
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent: {0}", message);
                    // Bytes Array to receive Server Response.
                    data = new Byte[256];
                    String response = String.Empty;
                    // Read the Tcp Server Response Bytes.
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Console.WriteLine("Received: {0}", response);
                    Thread.Sleep(2000);
                }
                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
            return Task.CompletedTask;
        }
    }
}
