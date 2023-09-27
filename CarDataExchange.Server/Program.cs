using CarDataExchange.Backend;
using CarDataExchange.Core;
using CarDataExchange.Core.Abstractions;
using CarDataExchange.Core.Models;
using System.Net;
using System.Net.Sockets;

internal class Program
{
    public static IEnumerable<Car> AllCars = new List<Car>()
    {
        new Car()
        {
            Brand = "Nissan",
            Year = 2008,
            EngineSize = 1.6f
        }
    };

    public static void Main()
    {
        Console.WriteLine("Настройка сервера...");
        IPAddress address = IPAddress.Parse("127.0.0.1");
        int port;
        do
        {
            Console.WriteLine("Введите порт:");
        } while (!int.TryParse(Console.ReadLine(), out port));

        TcpListener socketListener = new TcpListener(address, port);
        CarDataFoundation carDataFoundation = new CarDataFoundation();
        MessageDataFoundation messageDataFoundation = new();
        CancellationTokenSource token = new CancellationTokenSource();
        ClientHandler? handler = null;

        try
        {
            socketListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient connectedClient = socketListener.AcceptTcpClient();
                Console.WriteLine("Подключён клиент");
                handler = new(connectedClient, messageDataFoundation, carDataFoundation);
                Task.Run(() => handler.HandleAsync(token.Token), token.Token);
            }
        }catch(Exception e)
        {
            Console.WriteLine($"Произошла ошибка: {e.Message}");
        }
        finally
        {
            handler?.Dispose();
            token.Dispose();
            socketListener.Stop();
        }
    }
}