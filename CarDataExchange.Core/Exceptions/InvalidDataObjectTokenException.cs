using CarDataExchange.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Exceptions
{
    /// <summary>
    /// Исключение, которое возникает при неправильном описании битовой последовательности.
    /// </summary>
    public class InvalidDataObjectTokenException : Exception
    {
        public InvalidDataObjectTokenException(DataTokenType expected, DataTokenType received) : base($"Incorrect token received {received}, expected {expected}")
        {

        }

        /// <summary>
        /// Проверяет ожидаемый токен. Исключает токен, если тот является <see cref="DataTokenType.Nothing"/>.
        /// </summary>
        /// <param name="expected">Ожидаемый токен</param>
        /// <param name="received">Полученный</param>
        /// <exception cref="InvalidDataObjectTokenException">Если ожидаемый и полученный токен не совпадает, выкидывает исключение</exception>
        public static void ThrowIfInvalidToken(DataTokenType expected, DataTokenType received)
        {
            if (expected != received && received != DataTokenType.Nothing) throw new InvalidDataObjectTokenException(expected, received);
        }
    }
}
