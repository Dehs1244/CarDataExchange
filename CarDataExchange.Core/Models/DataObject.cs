using CarDataExchange.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Models
{
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

        public static DataObject Empty => new(0, DataTokenType.Nothing);
    }
}
