using CarDataExchange.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CarDataExchange.Client
{
    /// <summary>
    /// Реализует сохранение полученных записей объектов в XML файловый формат.
    /// </summary>
    internal class XMLFileLogger : ILogHandler
    {
        /// <summary>
        /// Папка, в которой хранятся файлы.
        /// </summary>
        public readonly string LogDirectory;

        public XMLFileLogger(string directory)
        {
            LogDirectory = directory;
            EnsureDirectoryCreated();
        }

        public bool EnsureDirectoryCreated()
        {
            if (Directory.Exists(LogDirectory)) return true;

            Directory.CreateDirectory(LogDirectory);
            return false;
        }

        public void Write<T>(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            var path = Path.Combine(LogDirectory, $"response_{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}.xml");
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, value);
                ConsoleHelper.WriteColoredLine($"Ответ от сервера был сохранён в виде XML по пути: {path}", ConsoleColor.Yellow);
            }
        }
    }
}
