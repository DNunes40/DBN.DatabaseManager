using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;
using DBN.DatabaseManager.InterSystemsIris;
using DBN.DatabaseManager.MariaDB;
using DBN.DatabaseManager.MySQL;
using DBN.DatabaseManager.Oracle;
using DBN.DatabaseManager.SQLite;
using DBN.DatabaseManager.SqlServer;

namespace DBN.DatabaseManager.Tests
{
    public class TestMultipleConnections : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestMultipleConnections(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(Connection_ShoudNotPersistTestData))]
        public async Task Connection_ShoudNotPersist(FactoryEnumType provider, string Identifier, int id, string expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "*");

            var _database = _fixture.GetProvider(provider);

            IDbManager? dbManager = null;

            if (provider == FactoryEnumType.Oracle)
            {
                dbManager = _database.ResolveDbManager<IOracleDbManager>(Identifier);
            }
            else if (provider == FactoryEnumType.Sqlite)
            {
                dbManager = _database.ResolveDbManager<ISQLiteDbManager>(Identifier);
            }
            else if (provider == FactoryEnumType.Iris)
            {
                dbManager = _database.ResolveDbManager<IIrisDbManager>(Identifier);
            }
            else if (provider == FactoryEnumType.SqlServer)
            {
                dbManager = _database.ResolveDbManager<ISqlServerDbManager>(Identifier);
            }
            else if (provider == FactoryEnumType.MySQL)
            {
                dbManager = _database.ResolveDbManager<IMySqlDbManager>(Identifier);
            }
            else 
            {
                dbManager = _database.ResolveDbManager<IMariaDbManager>(Identifier);
            }

            Assert.NotNull(dbManager);

            var reader = await dbManager.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBString("StringValue");

            await reader.CloseAsync();

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> Connection_ShoudNotPersistTestData
        {
            get
            {
                var rounds = new List<int> { 1, 2 };

                var testCases = new[]
                {
                    new { Id = 1, Expected = "Hello World" },
                    new { Id = 2, Expected = "Test String" },
                    new { Id = 4, Expected = "Another Value" }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var round in rounds)
                    {
                        foreach (var test in testCases)
                        {
                            yield return new object[]
                            {
                                provider,
                                GetDbManagerIdentifier(provider, round),
                                test.Id,
                                test.Expected
                            };
                        }
                    }
                }
            }
        }

        public static string GetDbManagerIdentifier(FactoryEnumType provider, int round)
        {
            return $"{provider.ToString()}{round}";
        }
    }
}