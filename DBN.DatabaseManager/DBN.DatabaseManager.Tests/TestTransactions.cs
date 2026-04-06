using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestTransactions : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestTransactions(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(Transaction_ShoudNotPersistTestData))]
        public async Task Transaction_ShoudNotPersist(FactoryEnumType provider, string table)
        {
            var id = GetUniqueId();

            var sql = @$"
                    INSERT INTO {table} (Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue)
                    VALUES ({id}, 42, 3.14159, 2.5, 9999999999, 'TRUE', 'Hello World', 'A,B,C,D')".Trim();

            var _database = _fixture.GetProvider(provider);

            await _database.BeginTransactionAsync();

            try
            {
                var affected = await _database.ExecuteNonQueryAsync(sql);

                Assert.Equal(1, affected);

                var obj = await GetRow(_database, provider, id);

                Assert.NotNull(obj);

                Assert.Equal(42, obj.IntValue);

                var p = int.Parse("zxvfzscgvzsdxg");

                await _database.CommitTransactionAsync();
            }
            catch 
            {
                await _database.RollbackTransactionAsync();
            }

            var obj2 = await GetRow(_database, provider, id);

            Assert.Null(obj2);
        }

        [Theory]
        [MemberData(nameof(Transaction_ShoudNotPersistTestData))]
        public async Task Transaction_ShouldPersist(FactoryEnumType provider, string table)
        {
            var id = GetUniqueId();

            var sql = @$"
                INSERT INTO {table} (Id, IntValue, DoubleValue, FloatValue, LongValue, BoolValue, StringValue, SplitValue)
                VALUES ({id}, 42, 3.14159, 2.5, 9999999999, 'TRUE', 'Hello World', 'A,B,C,D')".Trim();

            var _database = _fixture.GetProvider(provider);

            await _database.BeginTransactionAsync();

            var affected = await _database.ExecuteNonQueryAsync(sql);
            Assert.Equal(1, affected);

            var objInsideTx = await GetRow(_database, provider, id);

            Assert.NotNull(objInsideTx);
            Assert.Equal(42, objInsideTx.IntValue);

            await _database.CommitTransactionAsync();

            // Assert AFTER commit
            var objAfterCommit = await GetRow(_database, provider, id);

            Assert.NotNull(objAfterCommit);
            Assert.Equal(42, objAfterCommit.IntValue);

            TestFixture.NeedTableCleanup = true;
        }

        public static IEnumerable<object[]> Transaction_ShoudNotPersistTestData
        {
            get
            {
                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    yield return new object[]
                    {
                        provider,
                        provider == FactoryEnumType.Iris ? "Test.TestDbExtensions" : "TestDbExtensions"
                    };
                }
            }
        }

        public async Task<TestDbExtensionsModel?> GetRow(IDbManager dm, FactoryEnumType provider, int id)
        {
            var sql = _fixture.GetSqlProvider(provider, "*");
            var parameter = _fixture.GetParameter(provider, "id", id);
            var reader = await dm.ExecuteReaderAsync(sql, parameter);
            var data = await reader.MapTo<TestDbExtensionsModel>();            
            return data;
        }

        private int GetUniqueId()
        {
            return new Random().Next(10002, 20000);
        }
    }
}