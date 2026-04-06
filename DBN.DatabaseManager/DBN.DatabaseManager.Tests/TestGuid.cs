using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestGuid : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestGuid(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(DBGuidTestData))]
        public async Task GetDBGuidValue(int id, FactoryEnumType provider, Guid? expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "GuidValue");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBGuid("GuidValue");

            await reader.DisposeAsync();

            Assert.Equal(value, expected);
        }

        public static IEnumerable<object[]> DBGuidTestData
        {
            get
            {
                Guid?[] testValues = { Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), null, Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b") };

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