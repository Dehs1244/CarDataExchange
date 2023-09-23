using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Models
{
    [Serializable]
    public readonly record struct Car
    {
        public string Brand { get; init; }
        public ushort Year { get; init; }
        public float EngineSize { get; init; }
        public ushort? Doors { get; init; }

        public override string ToString()
        {
            StringBuilder builder = new();

            builder.AppendLine($"Марка (строка): {Brand}");
            builder.AppendLine($"Год выпуска (целое без знака): {Year}");
            builder.AppendLine($"Объём двигателя (с плавающей точкой): {EngineSize}");

            return builder.ToString();
        }
    }
}
