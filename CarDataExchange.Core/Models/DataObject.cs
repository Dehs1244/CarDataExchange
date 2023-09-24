using CarDataExchange.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Models
{
    /// <summary>
    /// Описывает класс обработанного значения в последовательности битов.
    /// </summary>
    public class DataObject
    {
        public DataTokenType Token { get; init; }
        public object Value { get; init; }

        public DataObject(object value, DataTokenType token)
        {
            Value = value;
            Token = token;
        }

        public T? ToObject<T>() => Token == DataTokenType.Nothing ? default : (T)Value;

        /// <summary>
        /// Возвращает пустой токен или же токен вида <see cref="Nullable"/>.
        /// </summary>
        public static DataObject Empty => new(0, DataTokenType.Nothing);
    }
}
