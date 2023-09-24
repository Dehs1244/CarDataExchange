using CarDataExchange.Core;
using CarDataExchange.Core.Abstractions;
using CarDataExchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Backend
{
    /// <summary>
    /// Реализует клиента на стороне сервера для обработки сообщений.
    /// </summary>
    internal class ClientHandler : IDisposable
    {
        public readonly IDecoder<DecodedInfo<Message>> MessageDecoder;
        public readonly IEncoder<Car> CarEncoder;
        private readonly TcpClient _client;
        private bool _disposed;

        public ClientHandler(TcpClient client, IDecoder<DecodedInfo<Message>> decoder, IEncoder<Car> encoder)
        {
            _client = client;
            MessageDecoder = decoder;
            CarEncoder = encoder;
        }

        public void Dispose()
        {
            _disposed = true;
            _client.Close();
            _client.Dispose();
        }

        public void SendAllCarsToClient()
        {
            foreach(var car in Program.AllCars)
            {
                SendCarToClient(car);
            }
        }

        public void SendCarToClient(Car car)
        {
            byte[] package = CarEncoder.Encode(car);
            _client.GetStream().Write(package);
        }

        public async Task HandleAsync(CancellationToken token = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(_client));

            try
            {
                while (_client.Connected)
                {
                    token.ThrowIfCancellationRequested();

                    if (MessageDecoder.TryDecode(_client.GetStream(), out DecodedInfo<Message> message))
                    {
                        switch (message.Value.Command)
                        {
                            case "G_ALL":
                                SendAllCarsToClient();
                                break;
                            case "G_BY":
                                SendCarToClient(Program.AllCars.ElementAt(message.Value.Index!.Value));
                                break;
                        }
                    }

                    await Task.Yield();
                }

                Console.WriteLine("Клиент отключился");
            }
            catch (OperationCanceledException)
            {
            }catch(Exception ex)
            {
                Console.WriteLine($"Произошла необработанная ошибка: {ex.Message}");
            }
            finally
            {
                Dispose();
            }
        }
    }
}
