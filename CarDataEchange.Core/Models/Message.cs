using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Core.Models
{
    [Serializable]
    public readonly record struct Message
    {
        public string Command { get; init; }
        public ushort? Index { get; init; }
    }
}
