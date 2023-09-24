using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Client
{
    public interface ILogHandler
    {
        void Write<T>(T value);
    }
}
