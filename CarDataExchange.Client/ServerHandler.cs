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
    /// <summary>
    /// Реализует ассинхронную связь с сервером. Один поток обрабатывает входящие сообщения, другой исходящие.
    /// </summary>
    internal class ServerHandler : IDisposable
    {
        private readonly TcpClient _client;
        public readonly IDecoder<DecodedInfo<Car>> CarDecoder;
        public readonly IEncoder<Message> MessageEncoder;
        private bool _disposed;
        private CancellationTokenSource _consoleToken;

        private readonly ILogHandler _logHandler;
        
        public ServerHandler(TcpClient client, IDecoder<DecodedInfo<Car>> carDecoder, IEncoder<Message> messageEncoder) 
        {
            _client = client;
            MessageEncoder = messageEncoder;
            CarDecoder = carDecoder;
            _consoleToken = new CancellationTokenSource();

            _logHandler = new XMLFileLogger("logs");
        }

        public bool IsConnected => _client.Connected;
        public Stream ClientStream => _client.GetStream();

        public async Task SendMessageAsync(Message message)
        {
            byte[] encodedMessage = MessageEncoder.Encode(message);
            await ClientStream.WriteAsync(encodedMessage, 0, encodedMessage.Length);
            ConsoleHelper.WriteColoredLine("Запрос отправлен", ConsoleColor.Green);
        }

        /// <summary>
        /// Ожидание для получения сообщения
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task ListenForResponseAsync(CancellationToken token = default)
        {
            while (IsConnected && !token.IsCancellationRequested)
            {
                if (CarDecoder.TryDecode(ClientStream, out DecodedInfo<Car> decoded))
                {
                    Console.WriteLine();
                    if(decoded.ElementsCount == 0)
                    {
                        ConsoleHelper.WriteColoredLine("Данные не найдены", ConsoleColor.DarkRed);
                        break;
                    }
                    ConsoleHelper.WriteColoredLine("Получены данные:", ConsoleColor.Magenta);
                    ConsoleHelper.WriteColoredLine(decoded, ConsoleColor.Green);
                    Console.WriteLine("Сохранение информации...");
                    _logHandler.Write(decoded.Value);
                    break;
                }
                await Task.Yield();
            }
        }

        /// <summary>
        /// Main Loop для связи с севрером. Запускает два асинхронных потока.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Выбрасывается в случае, если объект был освобождён.</exception>
        public async Task LoopAsync(CancellationToken token = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(TcpClient));

            try
            {
                await _LoopInternalAsync(token);
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

        /// <summary>
        /// Прослушивает команды для исходящих сообщений.
        /// </summary>
        private async Task _LoopInternalAsync(CancellationToken token = default)
        {
            while (IsConnected && !token.IsCancellationRequested)
            {
                try
                {
                    string? command = string.Empty;
                    Console.Write("Введите комманду или напишите help: ");
                    command = await ConsoleHelper.ReadLineAsync(token);
                    Console.WriteLine();

                    switch (command)
                    {
                        case "G_ALL":
                            await ConsoleHelper.WriteLineAsync("Запрос на все записи...", _consoleToken.Token);
                            await SendMessageAsync(new Message() { Command = command });
                            await ListenForResponseAsync(token);
                            break;
                        case "G_BY":
                            await ConsoleHelper.WriteLineAsync("Запрос на запись...", _consoleToken.Token);
                            ushort recordIndex = 0;

                            while (recordIndex == 0)
                            {
                                await ConsoleHelper.WriteAsync("Введите номер записи: ", _consoleToken.Token);
                                if (!ushort.TryParse(await ConsoleHelper.ReadLineAsync(_consoleToken.Token), out recordIndex))
                                {
                                    await ConsoleHelper.WriteColoredLineAsync("Неправильно, введите НОМЕР в числовом виде", ConsoleColor.Red, _consoleToken.Token);
                                }
                            }

                            await SendMessageAsync(new Message() { Command = command, Index = recordIndex });
                            await ListenForResponseAsync(token);
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
