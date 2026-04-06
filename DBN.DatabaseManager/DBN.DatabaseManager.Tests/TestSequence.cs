using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestSequence : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        private readonly string _sequenceName;

        public TestSequence(TestFixture fixture)
        {
            _fixture = fixture;
            _sequenceName = "SEQUENCE_TEST";
        }

        [Theory]
        [InlineData(FactoryEnumType.Sqlite, true)]
        [InlineData(FactoryEnumType.Iris, true)]
        [InlineData(FactoryEnumType.SqlServer, false)]
        [InlineData(FactoryEnumType.MySQL, true)]
        [InlineData(FactoryEnumType.MariaDb, false)]
        [InlineData(FactoryEnumType.Oracle, false)]       
        public async Task GetNextSequenceValue_ShouldThrowIfExpected(FactoryEnumType provider, bool shouldThrow)
        {
            var _database = _fixture.GetProvider(provider);

            if (shouldThrow)
            {
                // Just check that it throws, no message check
                await Assert.ThrowsAsync<DatabaseManagerException>(
                    () => _database.GetNextSequenceValueAsync(_sequenceName)
                );
            }
            else
            {
                var value = await _database.GetNextSequenceValueAsync(_sequenceName);
                Assert.True(value > 0); // optional check
            }
        }
    }
}