using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Abstractions
{
    public interface IEncoder<T>
    {
        byte[] Encode(T data);
    }
}
