using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Tests.Generators
{
    public sealed class DecodeDataGenerator : BaseDataGenerator
    {
        public override IEnumerator<object[]> Generate()
        {
            //С пустым полем
            yield return new object[] { new byte[] { 0x02, 0x02, 0x12, 0x07, 0xD8, 0x13, 0x3F, 0xCC, 0xCC, 0xCD } };
            //Из ТЗ
            yield return new object[] { new byte[] { 0x02, 0x03, 0x09, 0x06, 0x4E, 0x69, 0x73, 0x73, 0x61, 0x6E, 0x12, 0x07, 0xD8, 0x13, 0x3F, 0xCC, 0xCC, 0xCD } };
            //Пустая структура
            yield return new object[] { new byte[] { 0x02, 0x00 } };
        }
    }
}
