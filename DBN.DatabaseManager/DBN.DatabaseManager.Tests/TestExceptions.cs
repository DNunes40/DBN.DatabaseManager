using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestExceptions : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestExceptions(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(TestExceptionsTestData))]
        public async Task TestExceptionsData(FactoryEnumType provider)
        {
            var _database = _fixture.GetProvider(provider);

            var sql = _fixture.GetSqlProvider(provider, ",,,,");

            try
            {
                var reader = await _database.ExecuteReaderAsync(sql,
                    _fixture.GetParameter(provider, "id", 1));

                await reader.DisposeAsync();

                Assert.True(1 == 2, $"Expected an exception to be thrown for '{provider.ToString()}', but it was not.");
            }
            catch (DatabaseManagerException exc)
            {
                var sqlDetail = exc.Sql;
                var parametersDetail = exc.Parameters;

                Assert.Equal("id=1", parametersDetail);
                Assert.Contains("SELECT ,,,, FROM Test", sql);
            }
            catch (Exception exc)
            {
                Assert.True(1 == 2, $"Expected a DatabaseManagerException for '{provider.ToString()}', but got {exc.GetType().Name} instead.");
            }
        }

        public static IEnumerable<object[]> TestExceptionsTestData
        {
            get
            {
                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    yield return new object[]
                    {
                        provider
                    };
                }
            }
        }
    }
}