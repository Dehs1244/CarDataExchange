using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Exceptions
{
    public class UnknownByteTypeException : Exception
    {
        public UnknownByteTypeException(byte type) : base("Unknown type: 0x" + type.ToString("X2"))
        {

        }
    }
}
