using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestQuery : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestQuery(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(QueryAsyncTestData))]
        public async Task QueryAsync(FactoryEnumType provider, int id, int intValue, string expectedString)
        {
            var _database = _fixture.GetProvider(provider);

            var data = await _database.QuerySingleAsync<TestDbExtensionsModel>(t => t.Id == id && t.IntValue == intValue);

            Assert.NotNull(data);

            Assert.Equal(expectedString, data.StringValue);
        }

        [Theory]
        [MemberData(nameof(QueryAsyncListTestData))]
        public async Task QueryAsyncList(FactoryEnumType provider, int id, int expected)
        {
            var _database = _fixture.GetProvider(provider);

            var data = await _database.QueryAsync<TestDbExtensionsModel>(t => t.Id < id);

            Assert.NotNull(data);

            Assert.True(data.Count == expected);
        }

        public static IEnumerable<object[]> QueryAsyncTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Val = 42, ExpectedString = "Hello World" },
                    new { Id = 2, Val = 123,  ExpectedString = "Test String" }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    if (provider == FactoryEnumType.Iris)
                    {
                        continue;
                    }

                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Id,
                            test.Val,
                            test.ExpectedString
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> QueryAsyncListTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 2, Expected = 1 },
                    new { Id = 3, Expected = 2 },
                    new { Id = 4, Expected = 3 },
                    new { Id = 5, Expected = 4 }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    if (provider == FactoryEnumType.Iris)
                    {
                        continue;
                    }

                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Id,
                            test.Expected
                        };
                    }
                }
            }
        }
    }
}