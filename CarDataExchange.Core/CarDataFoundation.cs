using CarDataExchange.Core.Abstractions;
using CarDataExchange.Core.Exceptions;
using CarDataExchange.Core.Models;
using CarDataExchange.Core.Enums;
using System.Text;

namespace CarDataExchange.Core
{
    /// <summary>
    /// Реализует конвертацию обработанных данных в структуру <see cref="Car"/>.
    /// </summary>
    public class CarDataFoundation : BaseDataFoundation<Car>
    {
        public override Car ConvertToData(IReadOnlyCollection<DataObject> body)
        {
            string brand = string.Empty;
            ushort? year = null;
            float? engineSize = null;
            ushort? doors = null;
            foreach(var element in body)
            {
                switch (element.Token)
                {
                    case DataTokenType.String:
                        if (!string.IsNullOrEmpty(brand)) break;

                        brand = element.ToObject<string>() ?? string.Empty;
                        break;
                    case DataTokenType.UInt16:
                        if (!year.HasValue) year = element.ToObject<ushort?>();
                        else if(!doors.HasValue) doors = element.ToObject<ushort?>();
                        break;
                    case DataTokenType.FloatPoint:
                        if (engineSize.HasValue) break;

                        engineSize = element.ToObject<float?>();
                        break;
                }
            }

            return new Car()
            {
                Brand = brand,
                Year = year,
                EngineSize = engineSize,
                Doors = doors
            };
        }
    }
}
