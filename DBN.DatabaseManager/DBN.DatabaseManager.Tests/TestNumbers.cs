using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;
using System.Data.Common;

namespace DBN.DatabaseManager.Tests
{
    public class TestNumbers : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestNumbers(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(GetTestDataInt))]
        public Task GetDBIntValue(int id, FactoryEnumType provider, int? expected)
        {
            return TestValueAsync("IntValue", id, provider, expected, (r, c) => r.GetDBInt(c));
        }
            

        [Theory]
        [MemberData(nameof(GetTestDataDouble))]
        public Task GetDBDoubleValue(int id, FactoryEnumType provider, double? expected)
        {
            return TestValueAsync("DoubleValue", id, provider, expected, (r, c) => r.GetDBDouble(c));
        }
            

        [Theory]
        [MemberData(nameof(GetTestDataFloat))]
        public Task GetDBFloatValue(int id, FactoryEnumType provider, float? expected)
        {
            return TestValueAsync("FloatValue", id, provider, expected, (r, c) => r.GetDBFloat(c));
        }
            

        [Theory]
        [MemberData(nameof(GetTestDataLong))]
        public Task GetDBLongValue(int id, FactoryEnumType provider, long? expected)
        {
            return TestValueAsync("LongValue", id, provider, expected, (r, c) => r.GetDBLong(c));
        }

        [Theory]
        [MemberData(nameof(GetTestDataDecimal))]
        public Task GetDBDecimalValue(int id, FactoryEnumType provider, decimal? expected)
        {
            return TestValueAsync("DoubleValue", id, provider, expected, (r, c) => r.GetDBDecimal(c));
        }


        private async Task TestValueAsync<T>(string column, int id, FactoryEnumType provider, T expected, Func<DbDataReader, string, T> getFunc)
        {
            var _database = _fixture.GetProvider(provider);

            var sql = _fixture.GetSqlProvider(provider, column);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = getFunc(reader, column);

            await reader.DisposeAsync();

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> GetTestDataInt
        {
            get
            {
                int?[] testValues = { 42, 123, null, -99 };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetTestDataDouble
        {
            get
            {
                double?[] testValues = { 3.14159, 12.34, null, 0.0001 };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetTestDataFloat
        {
            get
            {
                float?[] testValues = { 2.5f, 8.8f, null, 15.75f };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetTestDataLong
        {
            get
            {
                long?[] testValues = { 9999999999L, 1234567890123L, null, -123456789L };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetTestDataDecimal
        {
            get
            {
                decimal?[] testValues = { 3.14159m, 12.34m, null, 0.0001m };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }
    }
}