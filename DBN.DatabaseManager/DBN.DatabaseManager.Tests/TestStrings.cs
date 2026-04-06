using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestStrings : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestStrings(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(DbStringTestData))]
        public async Task GetDBStringValue(int id, FactoryEnumType provider, string? expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "StringValue");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBString("StringValue");

            await reader.DisposeAsync();

            Assert.Equal(value, expected);
        }

        [Theory]
        [MemberData(nameof(DbSplitStringTestData))]
        public async Task SplitStringAtValue(int id, FactoryEnumType provider, int index, string separator, string? expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "SplitValue");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetSplitDBStringAt("SplitValue", index, separator);

            await reader.DisposeAsync();

            Assert.Equal(value, expected);
        }

        [Theory]
        [MemberData(nameof(DbFirstStringTestData))]
        public async Task GetFirstDBStringValue(int id, FactoryEnumType provider, string? expected)
        {
            var sql = $"SELECT StringValue, '' AS StringValue2 FROM TestDbExtensions WHERE ID = @id";

            if (provider == FactoryEnumType.Oracle)
            {
                sql = $"SELECT StringValue, '' AS StringValue2 FROM TestDbExtensions WHERE ID = :id";
            }
            else if (provider == FactoryEnumType.Iris)
            {
                sql = $"SELECT StringValue, '' AS StringValue2 FROM Test.TestDbExtensions WHERE ID = @id";
            }

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetFirstDBString("StringValue2", "StringValue");

            await reader.DisposeAsync();

            Assert.Equal(value, expected);
        }

        public static IEnumerable<object[]> DbStringTestData
        {
            get
            {
                string?[] testValues = { "Hello World", "Test String", null, "Another Value" };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }

        public static IEnumerable<object[]> DbSplitStringTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Separator = ",", Values = new string?[] { "A", "B", "C" } },
                    new { Id = 2, Separator = "|", Values = new string?[] { "One", "Two", "Three" } },
                    new { Id = 3, Separator = ",", Values = new string?[] { null, null, null } },
                    new { Id = 4, Separator = ";", Values = new string?[] { "10", "20", "30" } },
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        for (int i = 0; i < test.Values.Length; i++)
                        {
                            yield return new object[]
                            {
                                test.Id,
                                provider,
                                i,
                                test.Separator,
                                test.Values[i]
                            };
                        }
                    }
                }
            }
        }

        public static IEnumerable<object[]> DbFirstStringTestData
        {
            get
            {
                string?[] testValues = { "Hello World", "Test String", null, "Another Value" };

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