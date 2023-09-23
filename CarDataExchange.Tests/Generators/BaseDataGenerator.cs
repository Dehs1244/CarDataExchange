using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Tests.Generators
{
    public abstract class BaseDataGenerator : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator() => Generate();

        public abstract IEnumerator<object[]> Generate();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
