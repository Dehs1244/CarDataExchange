using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Abstractions
{
    /// <summary>
    /// Интерфейс для описания сериализации данных в битовую последовательность.
    /// </summary>
    /// <typeparam name="T">Объект для сериализации</typeparam>
    public interface IEncoder<T>
    {
        byte[] Encode(T data);
    }
}
