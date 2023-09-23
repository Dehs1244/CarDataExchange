using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Enums
{
    public enum DataTokenType : byte
    {
        Nothing = 0x00,
        StartStructure = 0x02,
        String = 0x09,
        UInt16 = 0x12,
        FloatPoint = 0x13,
        Int16
    }
}
