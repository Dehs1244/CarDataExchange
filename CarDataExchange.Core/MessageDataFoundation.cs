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
    public class MessageDataFoundation : BaseDataFoundation<Message>
    {
        public override Message ConvertToData(IReadOnlyCollection<DataObject> body)
        {
            var message = body.ElementAt(0);
            InvalidDataObjectTokenException.ThrowIfInvalidToken(Enums.DataTokenType.String, message.Token);

            var index = body.ElementAt(1);
            InvalidDataObjectTokenException.ThrowIfInvalidToken(Enums.DataTokenType.UInt16, index.Token);

            return new Message()
            {
                Command = message.ToObject<string>() ?? string.Empty,
                Index = index.ToObject<ushort>()
            };
        }
    }
}
