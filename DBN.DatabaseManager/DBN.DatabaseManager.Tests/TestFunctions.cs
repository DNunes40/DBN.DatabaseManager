using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestFunctions : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestFunctions(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(Procedure_ShouldReturnDataTestData))]
        public async Task Function_ShouldReturnData(FactoryEnumType provider, string functionName, int id, string expected)
        {
            var _database = _fixture.GetProvider(provider);

            var value = await _database.ExecuteFunctionAsync<string>(functionName,
                _fixture.GetParameter(provider, "p_id", id));

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
                    if (provider == FactoryEnumType.Iris || provider == FactoryEnumType.Sqlite)
                    {
                        continue;
                    }

                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            provider == FactoryEnumType.SqlServer ? "dbo.TESTFUNCTIONDATASTRING" : "TESTFUNCTIONDATASTRING",
                            test.Id,
                            test.Expected
                        };
                    }
                }
            }
        }
    }
}