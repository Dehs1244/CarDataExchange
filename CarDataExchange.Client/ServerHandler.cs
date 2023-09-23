using CarDataExchange.Core;
using CarDataExchange.Core.Abstractions;
using CarDataExchange.Core.Helpers;
using CarDataExchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Client
{
    internal class ServerHandler : IDisposable
    {
        private readonly TcpClient _client;
        public readonly IDecoder<DecodedInfo<Car>> CarDecoder;
        public readonly IEncoder<Message> MessageEncoder;
        private bool _disposed;
        private CancellationTokenSource _consoleToken;
        
        public ServerHandler(TcpClient client, IDecoder<DecodedInfo<Car>> carDecoder, IEncoder<Message> messageEncoder) 
        {
            _client = client;
            MessageEncoder = messageEncoder;
            CarDecoder = carDecoder;
            _consoleToken = new CancellationTokenSource();
        }

        public bool IsConnected => _client.Connected;
        public Stream ClientStream => _client.GetStream();

        public async Task SendMessageAsync(Message message)
        {
            byte[] encodedMessage = MessageEncoder.Encode(message);
            ClientStream.Write(encodedMessage, 0, encodedMessage.Length);
            await ConsoleHelper.WriteColoredLineAsync("Запрос отправлен", ConsoleColor.Green, _consoleToken.Token);
        }

        public async Task ListenForResponse(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested && IsConnected)
            {
                if(CarDecoder.TryDecode(ClientStream, out DecodedInfo<Car> decoded))
                {
                    _consoleToken.Cancel();
                    //_consoleToken.Dispose();
                    await ConsoleHelper.SequenceAsync(async (token) =>
                    {
                        await Console.Out.WriteLineAsync();
                        ConsoleHelper.WriteColoredLine("Получены данные:", ConsoleColor.Magenta);
                        ConsoleHelper.WriteColoredLine(decoded, ConsoleColor.Green);
                    });

                    _consoleToken = new CancellationTokenSource();
                }

                await Task.Yield();
            }
        }

        public async Task LoopAsync(CancellationToken token = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(TcpClient));

            try
            {
                await Task.WhenAll(_LoopInternalAsync(token), ListenForResponse(token));
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
                Console.WriteLine($"Закрытие соединения...");
                Console.ResetColor();
            }
            finally
            {
                Dispose();
            }
        }

        private async Task _LoopInternalAsync(CancellationToken token = default)
        {
            while (IsConnected)
            {
                try
                {
                    string? command = string.Empty;
                    await ConsoleHelper.SequenceAsync(async (token) =>
                    {
                        Console.Write("Введите комманду или напишите help: ");
                        command = await ConsoleHelper.ReadLineAsync(token);
                        Console.WriteLine();
                        await Task.Yield();
                    }, _consoleToken.Token);

                    switch (command)
                    {
                        case "G_ALL":
                            await ConsoleHelper.WriteLineAsync("Запрос на все записи...", _consoleToken.Token);
                            await SendMessageAsync(new Message() { Command = command });
                            break;
                        case "G_BY":
                            await ConsoleHelper.WriteLineAsync("Запрос на запись...", _consoleToken.Token);
                            int? recordIndex = null;

                            while (recordIndex == null)
                            {
                                await ConsoleHelper.WriteAsync("Введите номер записи: ", _consoleToken.Token);
                                if (int.TryParse(await ConsoleHelper.ReadLineAsync(_consoleToken.Token), out int parsedIndex))
                                {
                                    recordIndex = parsedIndex;
                                }
                                else
                                {
                                    await ConsoleHelper.WriteColoredLineAsync("Неправильно, введите НОМЕР в числовом виде", ConsoleColor.Red, _consoleToken.Token);
                                }
                            }

                            await SendMessageAsync(new Message() { Command = command });
                            break;
                        case "help":
                            await ConsoleHelper.WriteLineAsync("Список команд: \n1. G_ALL - запрос на все записи \n2. G_BY - запрос по определённой записи", _consoleToken.Token);
                            break;
                        default:
                            await ConsoleHelper.WriteColoredLineAsync("Неизвестная команда", ConsoleColor.Red, _consoleToken.Token);
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await Task.Yield();
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _client.Close();
            _client.Dispose();
            _consoleToken.Dispose();
        }
    }
}
