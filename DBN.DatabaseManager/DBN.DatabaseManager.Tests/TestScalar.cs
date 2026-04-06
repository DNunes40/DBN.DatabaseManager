using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestScalar : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestScalar(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(ScalarIntValueTestData))]
        public async Task ScalarIntValue(FactoryEnumType provider, int id, int expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "IntValue");

            var _database = _fixture.GetProvider(provider);

            var value = await _database.ExecuteScalarAsync<int>(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.Equal(expected, value);
        }

        [Theory]
        [MemberData(nameof(ScalarLongValueTestData))]
        public async Task ScalarLongValue(FactoryEnumType provider, int id, long expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "LongValue");

            var _database = _fixture.GetProvider(provider);

            var value = await _database.ExecuteScalarAsync<long>(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.Equal(expected, value);
        }

        [Theory]
        [MemberData(nameof(ScalarStringValueTestData))]
        public async Task ScalarStringValue(FactoryEnumType provider, int id, string expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "StringValue");

            var _database = _fixture.GetProvider(provider);

            var value = await _database.ExecuteScalarAsync<string>(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> ScalarIntValueTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Expected = 42 },
                    new { Id = 2, Expected = 123 },
                    new { Id = 4, Expected = -99 }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
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

        public static IEnumerable<object[]> ScalarLongValueTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Expected = 9999999999L },
                    new { Id = 2, Expected = 1234567890123L },
                    new { Id = 4, Expected = -123456789L }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
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

        public static IEnumerable<object[]> ScalarStringValueTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Expected = "Hello World" },
                    new { Id = 2, Expected = "Test String" },
                    new { Id = 4, Expected = "Another Value" }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
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