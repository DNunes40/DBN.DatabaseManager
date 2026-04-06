using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestStream : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestStream(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(GetDBStreamValueTestData))]
        public async Task GetDBStreamValue(FactoryEnumType provider, int id, string column, string expected)
        {
            var sql = _fixture.GetSqlProvider(provider, column);

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.GetDBStream(column);

            await reader.DisposeAsync();

            var text = await GetTextFromStream(value);

            Assert.Equal(expected, text);
        }

        public static IEnumerable<object[]> GetDBStreamValueTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1, Column = "BlobValue", Expected = "HelloBlob" },
                    new { Id = 3, Column = "BlobValue", Expected = (string)null! }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Id,
                            test.Column,
                            test.Expected
                        };
                    }
                }
            }
        }

        private static async Task<string?> GetTextFromStream(Stream? stream)
        {
            if (stream == null)
            {
                return null;
            }

            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            var reader = new StreamReader(stream);

            var value = await reader.ReadToEndAsync();

            reader.Dispose();

            return value;
        }
    }
}