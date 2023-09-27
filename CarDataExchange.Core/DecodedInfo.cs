using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core
{
    /// <summary>
    /// Описание обработанного значения в последовательности битов.
    /// </summary>
    /// <typeparam name="T">Обработанное значение</typeparam>
    public struct DecodedInfo<T>
    {
        private T? _value;
        /// <summary>
        /// Обработанное значение.
        /// </summary>
        public T? Value { readonly get => _value; set => 
                _value = !IsStructStarted ? throw new Exception("Struct not started to write") : value; }
        /// <summary>
        /// Был ли токен начала записи структуры.
        /// </summary>
        public readonly bool IsStructStarted => ElementsCount > -1;
        /// <summary>
        /// Кол-во записей.
        /// </summary>
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
