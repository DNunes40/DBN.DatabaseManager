using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;

namespace DBN.DatabaseManager.Tests
{
    public class TestHelpers : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestHelpers(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(DBIsNullTestData))]
        public async Task IsDBValueNullValue(FactoryEnumType provider, int id, string column, bool expected)
        {
            var sql = _fixture.GetSqlProvider(provider, column);

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var value = reader.IsDBValueNull(column);

            await reader.DisposeAsync();

            Assert.Equal(expected, value);
        }


        [Theory]
        [MemberData(nameof(DBGetDBRowTestData))]
        public async Task GetDBRowValue(FactoryEnumType provider, int index, string column, string? expected)
        {
            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var dataSet = await _database.ExecuteDataSetAsync(sql);

            Assert.NotNull(dataSet);

            var row = dataSet.GetDBRow(index);

            Assert.NotNull(row);

            var value = row.GetDBString(column);

            dataSet.Dispose();

            Assert.Equal(expected, value);
        }


        [Theory]
        [MemberData(nameof(DBHasColumnTestData))]
        public async Task HasColumnValue(FactoryEnumType provider, bool useReader, int id, string column, string columnCheck, bool expected)
        {
            var sql = _fixture.GetSqlProvider(provider, column);

            var _database = _fixture.GetProvider(provider);

            var value = false;

            if (useReader)
            {
                var reader = await _database.ExecuteReaderAsync(sql,
                    _fixture.GetParameter(provider, "id", id));

                Assert.True(reader.HasRows);

                await reader.ReadAsync(TestContext.Current.CancellationToken);

                value = reader.HasColumn(columnCheck);

                await reader.DisposeAsync();
            }
            else
            {
                var dataSet = await _database.ExecuteDataSetAsync(sql,
                    _fixture.GetParameter(provider, "id", id));

                Assert.NotNull(dataSet);

                var dataRow = dataSet.GetDBRow(0);

                value = dataRow.HasColumn(columnCheck);

                dataSet.Dispose();
            }
           
            Assert.Equal(expected, value);
        }


        [Theory]
        [MemberData(nameof(DBGetDBRowsCountTestData))]
        public async Task GetDBRowsCountValue(FactoryEnumType provider, int expected)
        {
            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var dataSet = await _database.ExecuteDataSetAsync(sql);

            Assert.NotNull(dataSet);

            var value = dataSet.GetDBRowsCount();

            dataSet.Dispose();

            Assert.Equal(expected, value);
        }


        [Theory]
        [MemberData(nameof(DBToDataTableTestData))]
        public async Task ToDataTableValue(FactoryEnumType provider, int index, string column, string? expected)
        {
            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql);

            Assert.True(reader.HasRows);

            var table = await reader.ToDataTable();

            await reader.DisposeAsync();

            var row = table.Rows[index];

            var value = row[column] == DBNull.Value ? null : row[column]?.ToString();

            Assert.Equal(expected, value);
        }


        [Theory]
        [MemberData(nameof(DBGetDBColumnsTestData))]
        public async Task GetDBColumnsValue(FactoryEnumType provider, bool useReader, int id, string column, int expected)
        {
            var sql = _fixture.GetSqlProvider(provider, column);

            var _database = _fixture.GetProvider(provider);

            List<string> value = [];

            if (useReader)
            {
                var reader = await _database.ExecuteReaderAsync(sql,
                    _fixture.GetParameter(provider, "id", id));

                Assert.True(reader.HasRows);

                await reader.ReadAsync(TestContext.Current.CancellationToken);

                value = reader.GetDBColumns();

                await reader.DisposeAsync();
            }
            else
            {
                var dataSet = await _database.ExecuteDataSetAsync(sql,
                    _fixture.GetParameter(provider, "id", id));

                Assert.NotNull(dataSet);

                value = dataSet.GetDBColumns();

                dataSet.Dispose();
            }

            if (provider == FactoryEnumType.Iris)
            {
                expected++; //Auto ID1 column
            }

            Assert.Equal(expected, value.Count);
        }


        [Theory]
        [MemberData(nameof(DBGetDBValueAsTestData))]
        public async Task GetDBValueAsValue(FactoryEnumType provider)
        {
            var sql = _fixture.GetSqlProvider(provider, "*");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                    _fixture.GetParameter(provider, "id", 1));

            Assert.True(reader.HasRows);

            await reader.ReadAsync(TestContext.Current.CancellationToken);

            var intValue = reader.GetDBValueAs<int>("IntValue");

            var doubleValue = reader.GetDBValueAs<double>("DoubleValue");

            var floatValue = reader.GetDBValueAs<float>("FloatValue");

            var longValue = reader.GetDBValueAs<long>("LongValue");

            var boolValue = reader.GetDBValueAs<bool>("LongValue");

            var stringValue = reader.GetDBValueAs<string>("StringValue");

            await reader.DisposeAsync();

            Assert.Equal(42, intValue);

            Assert.Equal(3.14159, doubleValue);

            Assert.Equal(2.5, floatValue);

            Assert.Equal(9999999999, longValue);

            Assert.True(boolValue);

            Assert.Equal("Hello World", stringValue);
        }




        public static IEnumerable<object[]> DBIsNullTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Column = "StringValue", Id = 1,  Expected = false },
                    new { Column = "StringValue", Id = 3,  Expected = true }
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

        public static IEnumerable<object[]> DBGetDBRowTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Column = "StringValue", Index = 0,  Expected = "Hello World" },
                    new { Column = "StringValue", Index = 1,  Expected = "Test String" },
                    new { Column = "StringValue", Index = 2,  Expected = (string?)null! },
                    new { Column = "StringValue", Index = 3,  Expected = "Another Value" }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Index,
                            test.Column,
                            test.Expected
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> DBHasColumnTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { UseReader = true, Id = 1, Column = "*", ColumnCheck = "StringValue", Expected = true },
                    new { UseReader = true, Id = 1, Column = "*", ColumnCheck = "RandomColumnName", Expected = false },
                    new { UseReader = false, Id = 1, Column = "*", ColumnCheck = "StringValue", Expected = true },
                    new { UseReader = false, Id = 1, Column = "*", ColumnCheck = "RandomColumnName", Expected = false }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.UseReader,
                            test.Id,
                            test.Column,
                            test.ColumnCheck,
                            test.Expected
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> DBGetDBRowsCountTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Expected = 4 },
                    new { Expected = 4 },
                    new { Expected = 4 },
                    new { Expected = 4 }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Expected
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> DBToDataTableTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Index = 0, Column = "StringValue", Expected = "Hello World" },
                    new { Index = 1, Column = "StringValue", Expected = "Test String" },
                    new { Index = 2, Column = "StringValue", Expected = (string?)null! },
                    new { Index = 3, Column = "StringValue", Expected = "Another Value" }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Index,
                            test.Column,
                            test.Expected
                        };
                    }
                }
            }
        }
        public static IEnumerable<object[]> DBGetDBColumnsTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { UseReader = true, Id = 1, Column = "*",  Expected = 12 },
                    new { UseReader = false, Id = 1, Column = "*", Expected = 12 }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.UseReader,
                            test.Id,
                            test.Column,
                            test.Expected
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> DBGetDBValueAsTestData
        {
            get
            {
                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    yield return new object[] { provider };
                }
            }
        }
    }
}