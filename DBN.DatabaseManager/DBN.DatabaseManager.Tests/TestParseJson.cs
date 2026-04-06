using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;
using System.Text.Json;

namespace DBN.DatabaseManager.Tests
{
    public class TestParseJson : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestParseJson(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(DBParseJsonTestData))]
        public async Task GetDBParseJsonValue(int id, FactoryEnumType provider)
        {
            var sql = _fixture.GetSqlProvider(provider, "JsonValue");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            if (id == 1)
            {
                var value = reader.GetDBParseJson<JsonPerson>("JsonValue");

                await reader.DisposeAsync();

                Assert.Equal(
                    JsonSerializer.Serialize(new JsonPerson { Name = "Alice", Age = 30 }),
                    JsonSerializer.Serialize(value)
                );
            }
            else if (id == 2)
            {
                var value = reader.GetDBParseJson<JsonParameters>("JsonValue");

                await reader.DisposeAsync();

                Assert.Equal(
                    JsonSerializer.Serialize(new JsonParameters { Active = true }),
                    JsonSerializer.Serialize(value)
                );
            }
            else if (id == 3)
            {
                var value = reader.GetDBParseJson<JsonParameters>("JsonValue");

                await reader.DisposeAsync();

                Assert.Null(value);
            }
            else
            {
                var value = reader.GetDBParseJson<JsonList>("JsonValue");

                await reader.DisposeAsync();

                Assert.Equal(
                    JsonSerializer.Serialize(new JsonList { Items = [1, 2, 3] }),
                    JsonSerializer.Serialize(value)
                );
            }            
        }

        public static IEnumerable<object[]> DBParseJsonTestData
        {
            get
            {
                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        yield return new object[] { i + 1, provider };
                    }
                }
            }
        }
    }
}