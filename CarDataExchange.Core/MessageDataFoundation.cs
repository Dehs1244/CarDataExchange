using CarDataExchange.Core.Abstractions;
using CarDataExchange.Core.Exceptions;
using CarDataExchange.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core
{
    /// <summary>
    /// Реализует конвертацию обработанных данных в структуру <see cref="Message"/>.
    /// </summary>
    public class MessageDataFoundation : BaseDataFoundation<Message>
    {
        public override Message ConvertToData(IReadOnlyCollection<DataObject> body)
        {
            ushort index = 0;
            string message = string.Empty;

            foreach(var element in body)
            {
                switch (element.Token)
                {
                    case Enums.DataTokenType.String:
                        message = element.ToObject<string>() ?? string.Empty;
                        break;
                    case Enums.DataTokenType.UInt16:
                        index = element.ToObject<ushort>();
                        break;
                }
            }

            return new Message()
            {
                Command = message,
                Index = index
            };
        }
    }
}
