using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestBooleans : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestBooleans(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(GetDatabaseBoolTestData))]
        public async Task GetDBBoolValue(int id, FactoryEnumType provider, bool? expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "BoolValue");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBBool("BoolValue");

            await reader.DisposeAsync();

            Assert.Equal(value, expected);
        }



        [Theory]
        [MemberData(nameof(BoolTestData))]
        public async Task GetDBBoolValue2(FactoryEnumType provider, object dbValue, bool expected)
        {
            var _database = _fixture.GetProvider(provider);

            var sql = _fixture.GetSqlDataProvider(provider, "BoolValue", dbValue);

            var reader = await _database.ExecuteReaderAsync(sql);

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBBool("BoolValue");

            await reader.DisposeAsync();

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> BoolTestData()
        {
            var values = new (object value, bool expected)[]
            {
                ("Y", true), ("y", true), 
                ("N", false), ("n", false),
                ("TRUE", true), ("true", true),
                ("FALSE", false), ("false", false),
                ("1", true), ("0", false), 
                (1, true), (0, false),
                ("yes", true), ("YES", true), 
                ("no", false), ("NO", false),
                ("ON", true), ("on", true),
                ("ENABLED", true), ("enabled", true),
                ("ACTIVE", true), ("active", true),
                ("T", true), ("t", true),
                ("OK", true), ("ok", true),
                ("PASS", true), ("pass", true),
                ("SUCCESS", true), ("success", true),
                ("UP", true), ("up", true),
                ("OPEN", true), ("open", true),
                ("VALID", true), ("valid", true),
                ("CONFIRM", true), ("confirm", true),
                ("AGREE", true), ("agree", true),
                ("ALLOW", true), ("allow", true),
                ("OFF", false), ("off", false),
                ("DISABLED", false), ("disabled", false),
                ("INACTIVE", false), ("inactive", false),
                ("F", false), ("f", false),
                ("FAIL", false), ("fail", false),
                ("ERROR", false), ("error", false),
                ("DOWN", false), ("down", false),
                ("CLOSED", false), ("closed", false),
                ("INVALID", false), ("invalid", false),
                ("REJECT", false), ("reject", false),
                ("DISALLOW", false), ("disallow", false)
            };

            foreach (FactoryEnumType provider in Enum.GetValues(typeof(FactoryEnumType)))
            {
                foreach (var (val, expected) in values)
                {
                    yield return new object[] { provider, val, expected };
                }
            }
        }

        public static IEnumerable<object[]> GetDatabaseBoolTestData
        {
            get
            {
                bool?[] testValues = { true, true, null, true };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i]! };
                    }
                }
            }
        }
    }
}