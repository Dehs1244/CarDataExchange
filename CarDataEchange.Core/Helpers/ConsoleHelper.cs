﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Helpers
{
    public static class ConsoleHelper
    {
        private static SemaphoreSlim _sync = new(1);
        private static Task<string?>? _readTask;

        public static async ValueTask<string?> ReadLineAsync(CancellationToken token = default)
        {
            _readTask ??= Task.Run(() => Console.ReadLine());

            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (_readTask.IsCompleted) break;

                //await Task.Delay(1000, token);
                await Task.Yield();
            }
            //await Task.WhenAny(_readTask, Task.Delay(-1, token));
            token.ThrowIfCancellationRequested();

            string? result = await _readTask;
            _readTask = null;

            return result;
        }

        public static async Task WriteLineAsync(string? value, CancellationToken token = default)
        {
            await _sync.WaitAsync(token);

            await Console.Out.WriteLineAsync(value);

            _sync.Release();
        }

        /// <summary>
        /// Позволяет запустить последовательность методов для синхронной работы с консолью
        /// </summary>
        /// <param name="sequence">Последовательность методов</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task SequenceAsync(Func<CancellationToken, Task> sequence, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                await _sync.WaitAsync(token);
                await sequence(token);
            }
            finally
            {
                _sync.Release();
            }
        }

        public static async Task WriteAsync(string? value, CancellationToken token = default)
        {
            await _sync.WaitAsync(token);

            await Console.Out.WriteAsync(value);

            _sync.Release();
        }

        public static Task WriteLineAsync(CancellationToken token = default)
            => WriteAsync("\n", token);

        public static async Task WriteColoredLineAsync(string? value, ConsoleColor color, CancellationToken token = default)
        {
            await _sync.WaitAsync(token);

            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();

            _sync.Release();
        }

        public static Task WriteLineAsync(object value, CancellationToken token = default)
            => WriteLineAsync(value.ToString(), token);

        public static Task WriteColoredLineAsync(object value, ConsoleColor color, CancellationToken token = default)
            => WriteColoredLineAsync(value.ToString(), color, token);

        public static void WriteColoredLine(string? value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        public static void WriteColoredLine(object value, ConsoleColor color)
            => WriteColoredLine(value.ToString(), color);
    }
}
