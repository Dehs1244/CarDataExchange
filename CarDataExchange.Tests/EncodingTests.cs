using CarDataExchange.Core;
using CarDataExchange.Core.Models;
using CarDataExchange.Tests.Generators;

namespace CarDataExchange.Tests
{
    public class EncodingTests
    {
        [Theory]
        [ClassData(typeof(EncodeDataGenerator))]
        public void EncodeTest(Car carTest)
        {
            var dataFoundation = new CarDataFoundation();
            byte[] response = dataFoundation.Encode(carTest);

            Assert.Equal(0x02, response[0]);
        }

        [Theory]
        [InlineData("TEST_DATA", null)]
        [InlineData("TEST_DATA", (ushort)3)]
        public void EncodeMessageTest(string command, ushort? index)
        {
            var messageFoundation = new MessageDataFoundation();
            byte[] response = messageFoundation.Encode(new Message() { Command = command, Index = index});

            Assert.Equal(0x02, response[0]);
        }

        [Theory]
        [InlineData("TEST", null)]
        [InlineData("TEST_DATA", null)]
        [InlineData("TEST_DATA", (ushort)3)]
        public void EncodeWithDecodeMessageTest(string command, ushort? index)
        {
            var messageFoundation = new MessageDataFoundation();
            byte[] response = messageFoundation.Encode(new Message() { Command = command, Index = index });

            Assert.Equal(0x02, response[0]);

            DecodedInfo<Message> decoded = messageFoundation.Decode(response, response.Length);

            Assert.True(decoded.IsStructStarted);
            Assert.StartsWith(decoded.Value.Command, command);
        }

        [Theory]
        [ClassData(typeof(EncodeDataGenerator))]
        public void EncodeWithDecodeTest(Car carTest)
        {
            var dataFoundation = new CarDataFoundation();
            byte[] response = dataFoundation.Encode(carTest);

            Assert.Equal(0x02, response[0]);
            //Assert.Equal(0x03, response[1]);

            DecodedInfo<Car> decoded = dataFoundation.Decode(response, response.Length);

            Assert.True(decoded.Value == carTest);
        }
    }
}