using CarDataExchange.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Abstractions
{
    public interface IDecoder<T>
    {
        bool TryDecode(Stream stream, out T? decoded);
        T Decode(byte[] data, int length);
    }
}
