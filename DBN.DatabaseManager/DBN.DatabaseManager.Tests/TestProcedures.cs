using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestProcedures : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestProcedures(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(Procedure_ShouldReturnDataTestData))]
        public async Task Procedure_ShouldReturnData(FactoryEnumType provider, string procedureName, int id, string expected)
        {
            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteStoredProcedureAsync(procedureName, 
                _fixture.GetParameter(provider, "p_id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBString("StringValue");

            await reader.CloseAsync();

            Assert.Equal(expected, value);
        }

        [Theory]
        [MemberData(nameof(Package_ShouldReturnDataTestData))]
        public async Task Package_ShouldReturnData(FactoryEnumType provider, int id, string expected)
        {
            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteStoredProcedureAsync("TESTDBEXTENSIONSPK.TESTPROCEDUREDATA",
                _fixture.GetParameter(provider, "p_id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBString("StringValue");

            await reader.CloseAsync();

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> Procedure_ShouldReturnDataTestData
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
                    if (provider == FactoryEnumType.Sqlite)
                    {
                        continue;
                    }

                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                        provider,
                        provider == FactoryEnumType.Iris ? "Test.TestDbExtensions_TestProcedureData" : "TESTPROCEDUREDATA",
                        test.Id,
                        test.Expected
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> Package_ShouldReturnDataTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Expected = "Hello World" },
                    new { Id = 2, Expected = "Test String" },
                    new { Id = 4, Expected = "Another Value" }
                };

                foreach (var test in testCases)
                {
                    yield return new object[]
                    {
                        FactoryEnumType.Oracle,
                        test.Id,
                        test.Expected
                    };
                }
            }
        }
    }
}