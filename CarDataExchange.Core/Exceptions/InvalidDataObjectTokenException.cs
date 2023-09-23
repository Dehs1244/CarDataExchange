using CarDataExchange.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Exceptions
{
    public class InvalidDataObjectTokenException : Exception
    {
        public InvalidDataObjectTokenException(DataTokenType expected, DataTokenType received) : base($"Incorrect token received {received}, expected {expected}")
        {

        }

        public static void ThrowIfInvalidToken(DataTokenType expected, DataTokenType received)
        {
            if (expected != received && received != DataTokenType.Nothing) throw new InvalidDataObjectTokenException(expected, received);
        }
    }
}
