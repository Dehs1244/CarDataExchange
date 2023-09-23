using CarDataExchange.Core;
using System.Net;
using System.Net.Sockets;

namespace CarDataExchange.Client
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Добро пожаловать в интерфейс клиента!");
            IPAddress address = IPAddress.Parse("127.0.0.1");

            int port;
            do
            {
                Console.WriteLine("Введите порт:");
            } while (!int.TryParse(Console.ReadLine(), out port));

            TcpClient client = new();
            client.Connect(address, port);

            ServerHandler handler = new(client, new CarDataFoundation(), new MessageDataFoundation());
            CancellationTokenSource tokenSource = new();

            await handler.LoopAsync(tokenSource.Token);
        }
    }
}