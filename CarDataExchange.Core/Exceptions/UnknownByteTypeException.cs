using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Exceptions
{
    /// <summary>
    /// Возникает в случае получения неизвестного байта для обработки последовательности
    /// </summary>
    public class UnknownByteTypeException : Exception
    {
        public UnknownByteTypeException(byte type) : base("Unknown type: 0x" + type.ToString("X2"))
        {

        }
    }
}
