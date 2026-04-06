using DBN.DatabaseManager.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBN.DatabaseManager.Tests
{
    public class JsonPerson
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    public class JsonParameters
    {
        public bool Active { get; set; }
    }

    public class JsonList
    {
        public List<int> Items { get; set; } = [];
    }

    [Table("TestDbExtensions")]
    public class TestDbExtensionsModel
    {
        public int Id { get; set; } 

        public int IntValue { get; set; }

        public double DoubleValue { get; set; }

        public float FloatValue { get; set; }

        public long LongValue { get; set; }

        public bool BoolValue { get; set; }

        public string? StringValue { get; set; }

        public DateTime DateValue { get; set; }

        public Guid GuidValue { get; set; }
    }

    [Table("Test.TestDbExtensions")]
    public class TestDbExtensionsIrisModel : TestDbExtensionsModel
    {
    }


    [Table("TestDbExtensions")]
    public class TestDbExtensionsCustomModel
    {
        [DBFormat("dd-MMM-yyyy")]
        [MapsTo("DATE_VALUE_1")]
        public DateTime? DateValue1 { get; set; }
    }
}