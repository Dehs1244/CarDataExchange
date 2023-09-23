using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core
{
    public struct DecodedInfo<T>
    {
        private T? _value;
        public T? Value { readonly get => _value; set => 
                _value = !IsStructStarted ? throw new Exception("Struct not started to write") : value; }
        public readonly bool IsStructStarted => ElementsCount > 0;
        public int ElementsCount;

        public override string ToString()
        {
            StringBuilder decodedData = new StringBuilder();

            ArgumentNullException.ThrowIfNull(_value, nameof(Value));

            decodedData.AppendLine($"Начало структуры (число элементов: {ElementsCount}):");
            decodedData.AppendLine(Value!.ToString());

            return decodedData.ToString();
        }
    }
}
