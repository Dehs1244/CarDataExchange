using CarDataExchange.Core;
using CarDataExchange.Core.Models;
using CarDataExchange.Tests.Generators;

namespace CarDataExchange.Tests
{
    public class DecodingTests
    {
        [Theory]
        [ClassData(typeof(DecodeDataGenerator))]
        public void DecodeTest(byte[] encoded)
        {
            CarDataFoundation dataFoundation = new();
            DecodedInfo<Car> decoded = dataFoundation.Decode(encoded, encoded.Length);

            Assert.True(decoded.IsStructStarted);
            if(decoded.ElementsCount == 2)
            {
                Assert.True(string.IsNullOrEmpty(decoded.Value.Brand));
                return;
            }
            Assert.Equal(2008, decoded.Value.Year);
            Assert.Equal("Nissan", decoded.Value.Brand);
        }
    }
}
