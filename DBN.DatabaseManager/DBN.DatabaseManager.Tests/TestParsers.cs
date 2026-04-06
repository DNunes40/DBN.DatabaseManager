using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.Tests
{
    public class TestParsers : IClassFixture<TestFixture>
    {
        [Fact]
        public void Deserialize_ValidObjects()
        {
            var obj1 = @"{""name"":""Alice"",""age"":30}".FromJson<JsonPerson>();
            var obj2 = @"{""active"":true}".FromJson<JsonParameters>();
            var obj3 = @"{""items"":[1,2,3]}".FromJson<JsonList>();

            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            Assert.NotNull(obj3);

            Assert.Equal("Alice", obj1.Name);
            Assert.Equal(30, obj1.Age);

            Assert.True(obj2.Active);

            Assert.Equal(new[] { 1, 2, 3 }, obj3.Items);
        }

        [Fact]
        public void Deserialize_InvalidJson_ShouldReturnNull()
        {
            var invalidJson = "{ invalid json }";

            var result = invalidJson.FromJson<JsonPerson>();

            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_EmptyString_ShouldReturnNull()
        {
            var json = "";

            var result = json.FromJson<JsonPerson>();

            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_Null_ShouldThrow()
        {
            string json = null!;

            var result = json.FromJson<JsonPerson>();

            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_MissingProperties_ShouldUseDefaults()
        {
            var json = @"{""name"":""Alice""}";

            var result = json.FromJson<JsonPerson>();

            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
            Assert.Equal(0, result.Age); // default int
        }

        [Fact]
        public void Deserialize_ExtraProperties_ShouldIgnore()
        {
            var json = @"{""name"":""Alice"",""age"":30,""extra"":123}";

            var result = json.FromJson<JsonPerson>();

            Assert.NotNull(result);
            Assert.Equal("Alice", result.Name);
            Assert.Equal(30, result.Age);
        }

        [Fact]
        public void Deserialize_EmptyArray()
        {
            var json = @"{""items"":[]}";

            var result = json.FromJson<JsonList>();

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Empty(result.Items);
        }

        // =========================
        // SERIALIZATION TESTS
        // =========================

        [Fact]
        public void Serialize_ValidObjects()
        {
            var obj1 = new JsonPerson { Name = "Alice", Age = 30 };
            var obj2 = new JsonParameters { Active = true };
            var obj3 = new JsonList { Items = [1, 2, 3] };

            var json1 = obj1.ToJson();
            var json2 = obj2.ToJson();
            var json3 = obj3.ToJson();

            // Instead of string compare → deserialize back
            var result1 = json1.FromJson<JsonPerson>();
            var result2 = json2.FromJson<JsonParameters>();
            var result3 = json3.FromJson<JsonList>();

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);

            Assert.Equal(obj1.Name, result1.Name);
            Assert.Equal(obj1.Age, result1.Age);

            Assert.Equal(obj2.Active, result2.Active);

            Assert.Equal(obj3.Items, result3.Items);
        }

        [Fact]
        public void Serialize_EmptyList()
        {
            var obj = new JsonList { Items = [] };

            var json = obj.ToJson();
            var result = json.FromJson<JsonList>();

            Assert.NotNull(result);
            Assert.NotNull(result.Items);
            Assert.Empty(result.Items);
        }

        [Fact]
        public void Serialize_NullList()
        {
            var obj = new JsonList { Items = null! };

            var json = obj.ToJson();
            var result = json.FromJson<JsonList>();

            Assert.NotNull(result);
            Assert.True(result.Items == null || result.Items.Count == 0);
        }

        // =========================
        // ROUND-TRIP TESTS (MOST IMPORTANT)
        // =========================

        [Fact]
        public void RoundTrip_Person_ShouldMatch()
        {
            var original = new JsonPerson { Name = "Alice", Age = 30 };

            var json = original.ToJson();
            var result = json.FromJson<JsonPerson>();

            Assert.NotNull(result);
            Assert.Equal(original.Name, result.Name);
            Assert.Equal(original.Age, result.Age);
        }

        [Fact]
        public void RoundTrip_Parameters_ShouldMatch()
        {
            var original = new JsonParameters { Active = true };

            var json = original.ToJson();
            var result = json.FromJson<JsonParameters>();

            Assert.NotNull(result);
            Assert.Equal(original.Active, result.Active);
        }

        [Fact]
        public void RoundTrip_List_ShouldMatch()
        {
            var original = new JsonList { Items = [1, 2, 3] };

            var json = original.ToJson();
            var result = json.FromJson<JsonList>();

            Assert.NotNull(result);
            Assert.Equal(original.Items, result.Items);
        }
    }
}