using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestDates : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestDates(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(DBDateTestData))]
        public async Task GetDBDateValue(int id, FactoryEnumType provider, DateTime? expected)
        {
            var sql = _fixture.GetSqlProvider(provider, "DateValue");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBDate("DateValue");

            await reader.DisposeAsync();

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> DBDateTestData
        {
            get
            {
                DateTime?[] testValues = new DateTime?[] { DateTime.Parse("2024-12-25 15:30:00"), DateTime.Parse("2024-12-25 00:00:00"), null, DateTime.Parse("2024-01-01 00:00:00") };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    for (int i = 0; i < testValues.Length; i++)
                    {
                        yield return new object[] { i + 1, provider, testValues[i] };
                    }
                }
            }
        }




        [Theory]
        [MemberData(nameof(DBOtherDatesTestData))]
        public async Task GetDBDateOtherValues(FactoryEnumType provider, string column, string format, DateTime expected)
        {
            var sql = GetDateSqlProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql);

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBDate(column, format);

            await reader.DisposeAsync();

            Assert.Equal(expected, value);
        }

        public static IEnumerable<object[]> DBOtherDatesTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Column = "Date1", Format = "dd-MM-yyyy HH:mm:ss",  Expected = new DateTime(1988, 5, 28, 12, 41, 12) },
                    new { Column = "Date2", Format = "dd-MM-yyyy HH:mm",     Expected = new DateTime(1988, 5, 28, 12, 41, 0) },
                    new { Column = "Date3", Format = "dd-MM-yyyy HH",        Expected = new DateTime(1988, 5, 28, 12, 0, 0) },
                    new { Column = "Date4", Format = "dd-MM-yyyy",           Expected = new DateTime(1988, 5, 28) },
                    new { Column = "Date5", Format = "dd-MMM-yyyy",          Expected = new DateTime(1988, 5, 28) },
                    new { Column = "Date6", Format = "yyyy-MM-dd",           Expected = new DateTime(1988, 5, 28) },
                    new { Column = "Date7", Format = "yyyy/MM/dd",           Expected = new DateTime(1988, 5, 28) },
                    new { Column = "Date8", Format = "ddMMyyyy",             Expected = new DateTime(1988, 5, 28) },
                    new { Column = "Date9", Format = "yyyyMMdd",             Expected = new DateTime(1988, 5, 28) }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Column,
                            test.Format,
                            test.Expected
                        };
                    }
                }
            }
        }

        public string GetDateSqlProvider(FactoryEnumType provider)
        {
            var dates = @"
                '28-05-1988 12:41:12' As Date1, 
                '28-05-1988 12:41' As Date2, 
                '28-05-1988 12' As Date3, 
                '28-05-1988' As Date4, 
                '28-MAY-1988' As Date5,
                '1988-05-28' As Date6,
                '1988/05/28' As Date7,
                '28051988' As Date8,
                '19880528' As Date9
            ".Trim();

            if (provider == FactoryEnumType.Oracle)
            {
                return $"SELECT {dates} FROM dual";
            }

            return $"SELECT {dates}";
        }
    }
}