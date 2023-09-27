using CarDataExchange.Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDataExchange.Tests.Generators
{
    public sealed class EncodeDataGenerator : BaseDataGenerator
    {
        //Рандомная информация
        public override IEnumerator<object[]> Generate()
        {
            yield return new object[] { new Car() {
                    Brand = "Lada",
                    EngineSize = 2.6f,
                    Year = 2001
                }
            };
            yield return new object[] { new Car() {
                    Brand = "Mazda",
                    EngineSize = 2.2f,
                    Year = 2005
                }
            };
            yield return new object[] { new Car() {
                    Brand = "UAZ",
                    EngineSize = 2.7f,
                    Year = 2020
                }
            };
            yield return new object[] { new Car()
                {
                    Brand = string.Empty,
                    EngineSize = 3.4f,
                    Year = 4
                } 
            };
            yield return new object[] { new Car()
                {
                    Brand = string.Empty,
                    Year = 4
                }
            };
            yield return new object[]
            {
                new Car()
            };
        }
    }
}
