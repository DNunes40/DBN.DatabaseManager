using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;
using System.Globalization;

namespace DBN.DatabaseManager.Tests
{
    public class TestMapTo : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public TestMapTo(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(MapToObjectTestData))]
        public async Task MapToObject(FactoryEnumType provider, int id)
        {
            var sql = _fixture.GetSqlProvider(provider, "*");

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql,
                _fixture.GetParameter(provider, "id", id));

            Assert.True(reader.HasRows);

            TestDbExtensionsModel? obj = null;

            if (provider == FactoryEnumType.Iris)
            {
                obj = await reader.MapTo<TestDbExtensionsIrisModel>();
            }
            else
            {
                obj = await reader.MapTo<TestDbExtensionsModel>();
            }

            Assert.NotNull(obj);

            if (id == 1)
            {
                Assert.Equal(42, obj.IntValue);
                Assert.Equal(3.14159, obj.DoubleValue, 5);  // compare 5 decimal places
                Assert.Equal(2.5f, obj.FloatValue, 1);      // compare 1 decimal place
                Assert.Equal(9999999999, obj.LongValue);
                Assert.True(obj.BoolValue);
                Assert.Equal("Hello World", obj.StringValue);
                Assert.Equal(new DateTime(2024, 12, 25, 15, 30, 00), obj.DateValue);
                Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), obj.GuidValue);               
            }
            else if (id == 2)
            {
                Assert.Equal(123, obj.IntValue);
                Assert.Equal(12.34, obj.DoubleValue, 2);    // 2 decimal places
                Assert.Equal(8.8f, obj.FloatValue, 1);      // 1 decimal place
                Assert.Equal(1234567890123, obj.LongValue);
                Assert.True(obj.BoolValue);
                Assert.Equal("Test String", obj.StringValue);
                Assert.Equal(new DateTime(2024, 12, 25, 00, 00, 00), obj.DateValue);
                Assert.Equal(Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), obj.GuidValue);              
            }
            else
            {
                Assert.Equal(-99, obj.IntValue);
                Assert.Equal(0.0001, obj.DoubleValue, 4);   // 4 decimal places
                Assert.Equal(15.75f, obj.FloatValue, 2);    // 2 decimal places
                Assert.Equal(-123456789, obj.LongValue);
                Assert.True(obj.BoolValue);
                Assert.Equal("Another Value", obj.StringValue);
                Assert.Equal(new DateTime(2024, 1, 1, 00, 00, 00), obj.DateValue);
                Assert.Equal(Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b"), obj.GuidValue);                
            }
        }

        [Theory]
        [MemberData(nameof(MapToObjectTestData))]
        public async Task MapToObject2(FactoryEnumType provider, int id)
        {
            var sql = _fixture.GetSqlProvider(provider, "*");

            var _database = _fixture.GetProvider(provider);

            TestDbExtensionsModel? obj = null;

            if (provider == FactoryEnumType.Iris)
            {
                obj = await _database.MapToAsync<TestDbExtensionsIrisModel>(sql,
                    _fixture.GetParameter(provider, "id", id));
            }
            else
            {
                obj = await _database.MapToAsync<TestDbExtensionsModel>(sql,
                    _fixture.GetParameter(provider, "id", id));
            }

            Assert.NotNull(obj);

            if (id == 1)
            {
                Assert.Equal(42, obj.IntValue);
                Assert.Equal(3.14159, obj.DoubleValue, 5);  // compare 5 decimal places
                Assert.Equal(2.5f, obj.FloatValue, 1);      // compare 1 decimal place
                Assert.Equal(9999999999, obj.LongValue);
                Assert.True(obj.BoolValue);
                Assert.Equal("Hello World", obj.StringValue);
                Assert.Equal(new DateTime(2024, 12, 25, 15, 30, 00), obj.DateValue);
                Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), obj.GuidValue);
            }
            else if (id == 2)
            {
                Assert.Equal(123, obj.IntValue);
                Assert.Equal(12.34, obj.DoubleValue, 2);    // 2 decimal places
                Assert.Equal(8.8f, obj.FloatValue, 1);      // 1 decimal place
                Assert.Equal(1234567890123, obj.LongValue);
                Assert.True(obj.BoolValue);
                Assert.Equal("Test String", obj.StringValue);
                Assert.Equal(new DateTime(2024, 12, 25, 00, 00, 00), obj.DateValue);
                Assert.Equal(Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), obj.GuidValue);
            }
            else
            {
                Assert.Equal(-99, obj.IntValue);
                Assert.Equal(0.0001, obj.DoubleValue, 4);   // 4 decimal places
                Assert.Equal(15.75f, obj.FloatValue, 2);    // 2 decimal places
                Assert.Equal(-123456789, obj.LongValue);
                Assert.True(obj.BoolValue);
                Assert.Equal("Another Value", obj.StringValue);
                Assert.Equal(new DateTime(2024, 1, 1, 00, 00, 00), obj.DateValue);
                Assert.Equal(Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b"), obj.GuidValue);
            }
        }


        [Theory]
        [MemberData(nameof(MapToListObjectTestData))]
        public async Task MapToListObject(FactoryEnumType provider)
        {
            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql);

            Assert.True(reader.HasRows);

            var objs = await reader.MapTo<List<TestDbExtensionsModel>>();

            Assert.NotNull(objs);

            Assert.True(objs.Count == 4);

            var obj1 = objs.FirstOrDefault(o => o.Id == 1);
            Assert.NotNull(obj1);

            Assert.Equal(42, obj1.IntValue);
            Assert.Equal(3.14159, obj1.DoubleValue, 5);  // compare 5 decimal places
            Assert.Equal(2.5f, obj1.FloatValue, 1);      // compare 1 decimal place
            Assert.Equal(9999999999, obj1.LongValue);
            Assert.True(obj1.BoolValue);
            Assert.Equal("Hello World", obj1.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 15, 30, 00), obj1.DateValue);
            Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), obj1.GuidValue);

            var obj2 = objs.FirstOrDefault(o => o.Id == 2);
            Assert.NotNull(obj2);

            Assert.Equal(123, obj2.IntValue);
            Assert.Equal(12.34, obj2.DoubleValue, 2);    // 2 decimal places
            Assert.Equal(8.8f, obj2.FloatValue, 1);      // 1 decimal place
            Assert.Equal(1234567890123, obj2.LongValue);
            Assert.True(obj2.BoolValue);
            Assert.Equal("Test String", obj2.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 00, 00, 00), obj2.DateValue);
            Assert.Equal(Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), obj2.GuidValue);

            var obj4 = objs.FirstOrDefault(o => o.Id == 4);
            Assert.NotNull(obj4);

            Assert.Equal(-99, obj4.IntValue);
            Assert.Equal(0.0001, obj4.DoubleValue, 4);   // 4 decimal places
            Assert.Equal(15.75f, obj4.FloatValue, 2);    // 2 decimal places
            Assert.Equal(-123456789, obj4.LongValue);
            Assert.True(obj4.BoolValue);
            Assert.Equal("Another Value", obj4.StringValue);
            Assert.Equal(new DateTime(2024, 1, 1, 00, 00, 00), obj4.DateValue);
            Assert.Equal(Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b"), obj4.GuidValue);
        }


        [Fact]
        public async Task MapToListObjectIRIS()
        {
            var provider = FactoryEnumType.Iris;

            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var reader = await _database.ExecuteReaderAsync(sql);

            Assert.True(reader.HasRows);

            var objs = await reader.MapTo<List<TestDbExtensionsModel>>();

            Assert.NotNull(objs);

            Assert.True(objs.Count == 4);

            var obj1 = objs.FirstOrDefault(o => o.Id == 1);
            Assert.NotNull(obj1);

            Assert.Equal(42, obj1.IntValue);
            Assert.Equal(3.14159, obj1.DoubleValue, 5);  // compare 5 decimal places
            Assert.Equal(2.5f, obj1.FloatValue, 1);      // compare 1 decimal place
            Assert.Equal(9999999999, obj1.LongValue);
            Assert.True(obj1.BoolValue);
            Assert.Equal("Hello World", obj1.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 15, 30, 00), obj1.DateValue);
            Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), obj1.GuidValue);

            var obj2 = objs.FirstOrDefault(o => o.Id == 2);
            Assert.NotNull(obj2);

            Assert.Equal(123, obj2.IntValue);
            Assert.Equal(12.34, obj2.DoubleValue, 2);    // 2 decimal places
            Assert.Equal(8.8f, obj2.FloatValue, 1);      // 1 decimal place
            Assert.Equal(1234567890123, obj2.LongValue);
            Assert.True(obj2.BoolValue);
            Assert.Equal("Test String", obj2.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 00, 00, 00), obj2.DateValue);
            Assert.Equal(Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), obj2.GuidValue);

            var obj4 = objs.FirstOrDefault(o => o.Id == 4);
            Assert.NotNull(obj4);

            Assert.Equal(-99, obj4.IntValue);
            Assert.Equal(0.0001, obj4.DoubleValue, 4);   // 4 decimal places
            Assert.Equal(15.75f, obj4.FloatValue, 2);    // 2 decimal places
            Assert.Equal(-123456789, obj4.LongValue);
            Assert.True(obj4.BoolValue);
            Assert.Equal("Another Value", obj4.StringValue);
            Assert.Equal(new DateTime(2024, 1, 1, 00, 00, 00), obj4.DateValue);
            Assert.Equal(Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b"), obj4.GuidValue);
        }


        [Theory]
        [MemberData(nameof(MapToListObjectTestData))]
        public async Task MapToListObject2(FactoryEnumType provider)
        {
            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var objs = await _database.MapToAsync<List<TestDbExtensionsModel>>(sql);

            Assert.NotNull(objs);

            Assert.True(objs.Count == 4);

            var obj1 = objs.FirstOrDefault(o => o.Id == 1);
            Assert.NotNull(obj1);

            Assert.Equal(42, obj1.IntValue);
            Assert.Equal(3.14159, obj1.DoubleValue, 5);  // compare 5 decimal places
            Assert.Equal(2.5f, obj1.FloatValue, 1);      // compare 1 decimal place
            Assert.Equal(9999999999, obj1.LongValue);
            Assert.True(obj1.BoolValue);
            Assert.Equal("Hello World", obj1.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 15, 30, 00), obj1.DateValue);
            Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), obj1.GuidValue);

            var obj2 = objs.FirstOrDefault(o => o.Id == 2);
            Assert.NotNull(obj2);

            Assert.Equal(123, obj2.IntValue);
            Assert.Equal(12.34, obj2.DoubleValue, 2);    // 2 decimal places
            Assert.Equal(8.8f, obj2.FloatValue, 1);      // 1 decimal place
            Assert.Equal(1234567890123, obj2.LongValue);
            Assert.True(obj2.BoolValue);
            Assert.Equal("Test String", obj2.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 00, 00, 00), obj2.DateValue);
            Assert.Equal(Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), obj2.GuidValue);

            var obj4 = objs.FirstOrDefault(o => o.Id == 4);
            Assert.NotNull(obj4);

            Assert.Equal(-99, obj4.IntValue);
            Assert.Equal(0.0001, obj4.DoubleValue, 4);   // 4 decimal places
            Assert.Equal(15.75f, obj4.FloatValue, 2);    // 2 decimal places
            Assert.Equal(-123456789, obj4.LongValue);
            Assert.True(obj4.BoolValue);
            Assert.Equal("Another Value", obj4.StringValue);
            Assert.Equal(new DateTime(2024, 1, 1, 00, 00, 00), obj4.DateValue);
            Assert.Equal(Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b"), obj4.GuidValue);
        }


        [Fact]
        public async Task MapToListObjectIRIS2()
        {
            var provider = FactoryEnumType.Iris;

            var sql = _fixture.GetSqlAllProvider(provider);

            var _database = _fixture.GetProvider(provider);

            var objs = await _database.MapToAsync<List<TestDbExtensionsModel>>(sql);

            Assert.NotNull(objs);

            Assert.True(objs.Count == 4);

            var obj1 = objs.FirstOrDefault(o => o.Id == 1);
            Assert.NotNull(obj1);

            Assert.Equal(42, obj1.IntValue);
            Assert.Equal(3.14159, obj1.DoubleValue, 5);  // compare 5 decimal places
            Assert.Equal(2.5f, obj1.FloatValue, 1);      // compare 1 decimal place
            Assert.Equal(9999999999, obj1.LongValue);
            Assert.True(obj1.BoolValue);
            Assert.Equal("Hello World", obj1.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 15, 30, 00), obj1.DateValue);
            Assert.Equal(Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), obj1.GuidValue);

            var obj2 = objs.FirstOrDefault(o => o.Id == 2);
            Assert.NotNull(obj2);

            Assert.Equal(123, obj2.IntValue);
            Assert.Equal(12.34, obj2.DoubleValue, 2);    // 2 decimal places
            Assert.Equal(8.8f, obj2.FloatValue, 1);      // 1 decimal place
            Assert.Equal(1234567890123, obj2.LongValue);
            Assert.True(obj2.BoolValue);
            Assert.Equal("Test String", obj2.StringValue);
            Assert.Equal(new DateTime(2024, 12, 25, 00, 00, 00), obj2.DateValue);
            Assert.Equal(Guid.Parse("d94fca1e-771d-4b2a-9ee7-7fd7d049bf16"), obj2.GuidValue);

            var obj4 = objs.FirstOrDefault(o => o.Id == 4);
            Assert.NotNull(obj4);

            Assert.Equal(-99, obj4.IntValue);
            Assert.Equal(0.0001, obj4.DoubleValue, 4);   // 4 decimal places
            Assert.Equal(15.75f, obj4.FloatValue, 2);    // 2 decimal places
            Assert.Equal(-123456789, obj4.LongValue);
            Assert.True(obj4.BoolValue);
            Assert.Equal("Another Value", obj4.StringValue);
            Assert.Equal(new DateTime(2024, 1, 1, 00, 00, 00), obj4.DateValue);
            Assert.Equal(Guid.Parse("5e2f83fe-fb4f-4e23-8fb2-fb631fd44a0b"), obj4.GuidValue);
        }




        [Theory]
        [MemberData(nameof(MapToObjectCustomPropsTestData))]
        public async Task MapToObjectCustomProps(FactoryEnumType provider)
        {
            var date1 = "28-MAY-1988";

            var sql = $"SELECT '{date1}' AS DATE_VALUE_1 FROM DUAL";

            if (provider != FactoryEnumType.Oracle)
            {
                sql = $"SELECT '{date1}' AS DATE_VALUE_1";
            }

            var _database = _fixture.GetProvider(provider);

            var data = await _database.MapToAsync<TestDbExtensionsCustomModel>(sql);

            Assert.NotNull(data);

            Assert.NotNull(data.DateValue1);
            Assert.Equal(DateTime.ParseExact(date1, "dd-MMM-yyyy", CultureInfo.InvariantCulture), data.DateValue1);
        }

        public static IEnumerable<object[]> MapToObjectTestData
        {
            get
            {
                var testCases = new[]
                {
                    new { Id = 1 },
                    new { Id = 2 },
                    new { Id = 4 }
                };

                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    foreach (var test in testCases)
                    {
                        yield return new object[]
                        {
                            provider,
                            test.Id
                        };
                    }
                }
            }
        }

        public static IEnumerable<object[]> MapToListObjectTestData
        {
            get
            {
                foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
                {
                    if (provider == FactoryEnumType.Iris)
                    {
                        continue;
                    }

                    yield return new object[]
                    {
                        provider
                    };
                }
            }
        }

        public static IEnumerable<object[]> MapToObjectCustomPropsTestData
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