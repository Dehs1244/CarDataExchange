using CarDataExchange.Core.Abstractions;
using CarDataExchange.Core.Exceptions;
using CarDataExchange.Core.Models;
using CarDataExchange.Core.Enums;
using System.Text;

namespace CarDataExchange.Core
{
    public class CarDataFoundation : BaseDataFoundation<Car>
    {
        public override Car ConvertToData(IReadOnlyCollection<DataObject> body)
        {
            var brand = body.ElementAt(0);
            InvalidDataObjectTokenException.ThrowIfInvalidToken(DataTokenType.String, brand.Token);

            var year = body.ElementAt(1);
            InvalidDataObjectTokenException.ThrowIfInvalidToken(DataTokenType.UInt16, year.Token);

            var engineSize = body.ElementAt(2);
            InvalidDataObjectTokenException.ThrowIfInvalidToken(DataTokenType.FloatPoint, engineSize.Token);

            return new Car()
            {
                Brand = brand.ToObject<string>() ?? string.Empty,
                Year = year.ToObject<ushort>(),
                EngineSize = engineSize.ToObject<float>()
            };
        }
    }
}
