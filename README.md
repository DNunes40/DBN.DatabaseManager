# DBN.DatabaseManager

A lightweight, production‑ready, async‑first database management library for .NET — with a unified API across multiple database providers.

✅ Clean & consistent API  
✅ Fully async  
✅ Strongly‑typed mapping  
✅ Expression‑based querying  
✅ Helpers for strings, numbers, dates, booleans, JSON, streams, GUIDs, packages/procedures and functions

✅ Extensive test coverage across all database providers

---

# ✨ Supported Providers

| Database          | Interface           |
| ----------------- | ------------------- |
| Oracle            | IOracleDbManager    |
| SQL Server        | ISqlServerDbManager |
| InterSystems IRIS | IIrisDbManager      |
| SQLite            | ISQLiteDbManager    |
| MySQL             | IMySqlDbManager     |
| MariaDB           | IMariaDbManager     |

---

## Created by

Daniel Nunes  
📧 dbnunesg40@hotmail.com

---

# 📦 Installation

```
dotnet add package DBN.DatabaseManager
```

---

# 🚀 Quickstart

### Model

```csharp
using System.ComponentModel.DataAnnotations.Schema;

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
```

### Example

```csharp
using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Oracle;
using Oracle.ManagedDataAccess.Client;

var builder = WebApplication.CreateBuilder(args);

Environment.SetEnvironmentVariable("TNS_ADMIN", @"c:\oraconfig");

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("ORACLE") ?? "";

builder.Services.AddScoped<IOracleDbManager>(sp => new OracleDbManager(connectionString));

var app = builder.Build();

app.MapGet("/example1", async (IOracleDbManager dm) =>
{
    var data = await dm.MapToAsync<List<TestDbExtensionsModel>>("SELECT * from TestDbExtensions");
    return Results.Ok(data);
});

app.MapGet("/example2", async (IOracleDbManager dm) =>
{
    var reader = await dm.ExecuteReaderAsync("SELECT * from TestDbExtensions WHERE ID = :id", new OracleParameter("id", 1));

    if (!reader.HasRows)
    {
        return Results.NotFound();
    }

    await reader.ReadAsync();

    var value1 = reader.GetDBInt("IntValue");
    var value2 = reader.GetDBString("StringValue");

    await reader.DisposeAsync();

    return Results.Ok(new { value1, value2 });
});

app.Run();
```

---

# 🗃️ Model Mapping System

## [Table("TABLE_NAME")]

Declares the database table for the model.

Used to:

- identify the table when performing mapped operations
- document the origin of the model

Example:

```csharp
[Table("USERS")]
class UserModel { }
```

## [MapsTo("COLUMN_NAME")]

Binds a property to a database column.

✅Supports:

- different naming conventions
- aliases (`AS`)
- inheritance
- any primitive or convertible type

Example:

```csharp
[MapsTo("FIRST_NAME")]
public string FirstName { get; set; }
```

## [DBFormat("FORMAT")]

Specifies how a DateTime should be parsed when the database returns **string-based date formats**.

Supports:

- custom patterns like `yyyyMMdd`, `yyyyMMddHHmmss`, `dd-MMM-yyyy` and `dd/MM/yyyy`
- nullable and non-nullable DateTime properties

Example:

```csharp
[DBFormat("dd-MMM-yyyy")]
public DateTime BirthDate { get; set; }
```

## [IgnoreMissingColumns]

Used at the **class level**.  
When enabled, missing columns in the database **do not throw exceptions**.

Example:

```csharp
[IgnoreMissingColumns]
class UserModel { ... }
```

✅If a column is not returned → property stays null  
✅No exception thrown  
❌ Without → throws `DatabaseManagerException`

---

## `[IgnoreIfMissing]`

Used at the **property level**.  
Ignores missing columns only for that specific property.

Example:

```csharp
[IgnoreIfMissing]
public string OptionalField { get; set; }
```

---

## Ignored Types (Automatic Rules)

The mapper automatically **ignores** the following properties:

- complex objects
- collections
- unsupported or complex structures

---

# Key Features

### 🔹 Execute SQL

```csharp
reader = await _databaseManager.ExecuteReaderAsync(sql, params...);
user   = await _databaseManager.QuerySingleAsync<UserModel>(u => u.Id == 20);
users  = await _databaseManager.QueryAsync<UserModel>(u => u.Id == 20);
data   = await _databaseManager.ExecuteDataSetAsync(sql, params...);
rows   = await _databaseManager.ExecuteNonQueryAsync(sql, params...);
value  = await _databaseManager.GetNextSequenceValueAsync("SEQ_NAME");
count  = await _databaseManager.ExecuteScalarAsync<T>(sql, params...);
reader = await _databaseManager.ExecuteStoredProcedureAsync(procedureName, params...);
value  = await _databaseManager.ExecuteFunctionAsync<T>(functionName, params...);
user   = await _databaseManager.MapToAsync<UserModel>(sql, params...)

```

---

# 🔧 Mapping Examples

Single record:

```csharp
var reader = await _databaseManager.ExecuteReaderAsync(sql);
var user   = await reader.MapTo<UserModel>();
```

List mapping:

```csharp
var users = await reader.MapTo<List<UserModel>>();
```

Direct mapping:

```csharp
var model = await _databaseManager.MapToAsync<UserModel>(sql, params...);
```

---

# 📚 Helper Methods

### 🔹 LINQ Expression Queries

Translate a LINQ expression directly into SQL:

```csharp
var user = await _databaseManager.QuerySingleAsync<UserModel>(u => u.Id == "1");

// OR

var users = await _databaseManager.QueryAsync<UserModel>(u => u.Id > "1");
```

## 🔹 Helpers

```csharp
var reader = await _databaseManager.ExecuteReaderAsync(sql, params...);

// OR

var dataSet = await _databaseManager.ExecuteDataSetAsync(sql, params...);
```

## 🔤 String Helpers

```csharp
.GetDBString(string column)
.GetFirstDBString(params string[] columns)
.GetSplitDBStringAt(string column, int index, string separator = ",")

//Examples:

var name = reader.GetDBString("FIRST_NAME");
var val = reader.GetFirstDBString("COL_1", "COL_2"); //"Return the first value that is neither null nor empty.
var lastName = reader.GetSplitDBStringAt("FULL_NAME", 2, ",");
```

## 🔢 Numeric Helpers

```csharp
.GetDBInt(string column)
.GetDBLong(string column)
.GetDBFloat(string column)
.GetDBDouble(string column)

//Examples:

var num1 = reader.GetDBInt("INT_COL");
var num2 = reader.GetDBLong("LONG_COL");
var num3 = reader.GetDBFloat("FLOAT_COL");
var num4 = reader.GetDBDouble("DOUBLE_COL");
```

## 📅 Date/Time Helper

```csharp
.GetDBDate(string column, string format = "")

//Examples:

var date1 = reader.GetDBDate("DOB");
var date2 = reader.GetDBDate("COL_DATE_STRING", "dd/MM/yyyy");
var date3 = reader.GetDBDate("COL_DATE_STRING", "dd/MM/yyyy HH:mm:ss");
```

## 🧬 Guid Helper

```csharp
.GetDBGuid(string column)

//Example:

var guid = reader.GetDBGuid("GUID_COL");
```

## 🟩 Boolean Helper

```csharp
/*
  Supports all variants, case-insensitive:
    - Y / N
    - YES / NO
    - TRUE / FALSE
    - 1 / 0
*/
.GetDBBool(string column)

//Example:

var val = reader.GetDBBool("BOOL_COL");
```

## 🟩 JSON Helper

```csharp
.GetDBParseJson(string column)

//Example:

var obj = reader.GetDBParseJson<ModelClass>("COL_JSON");

if(obj != null){
  Console.WriteLine(obj.Prop1);
}
```

## 🟩 Generic Helper

```csharp
.GetDBValueAs<T>(string column)

//Examples:

int value1 = reader.GetDBValueAs<int>("COL_1");

double value2 = reader.GetDBValueAs<double>("COL_2");

string value3 = reader.GetDBValueAs<string>("COL_3");

DateTime value4 = reader.GetDBValueAs<DateTime>("COL_4");
```

## 🟩 Others

```csharp
.HasColumn(string column)
.GetDBColumns()

//Example:

var isPresent = reader.HasColumn("COL_1");

Console.WriteLine(isPresent);

//

var columns = reader.GetDBColumns();

foreach (var colum in membcolumnsers)
{
    Console.WriteLine(colum);
}
```

## 🟩 Exceptions

```csharp
try
{
    var reader = await _database.ExecuteReaderAsync(sql, params..);
    //...
    await reader.DisposeAsync();
}
catch (DatabaseManagerException exc)
{
    var sqlDetail = exc.Sql;
    var parametersDetail = exc.Parameters;
    var message = exc.Message;
    //Log details
}
catch (Exception exc)
{
    var message = exc.Message;
    //Log details
}
```

# 🔁 Transactions

```csharp
await _databaseManager.BeginTransactionAsync();
try {
    await _databaseManager.ExecuteNonQueryAsync(sql);
    await _databaseManager.CommitTransactionAsync();
} catch {
    await _databaseManager.RollbackTransactionAsync();
}
```

- Rollback restores state
- Commit persists
- Fully tested across all providers

---

# 🔢 Sequences

```csharp
var next = await _databaseManager.GetNextSequenceValueAsync("SEQ_NAME");
```

Some providers throw (SQLite, IRIS, MySQL, MariaDB).

---

# 🔁 Package/Procedures

```csharp
var reader = await _databaseManager.ExecuteStoredProcedureAsync("PROC_NAME", params...);
```

This call always returns a data reader as the result.

---

# ▶️ Functions

```csharp
var value = await _databaseManager.ExecuteFunctionAsync<T>("FUNC_NAME", params...);
```

This call always returns a value of type T.

---

# 🧪 Test Coverage Summary

Tests included:

- Boolean parsing
- Date parsing (all formats)
- GUID parsing
- Numbers (int, float, long, double, decimal)
- JSON parse + serialize
- Streams (blob)
- Strings + splitting
- Mapping (single & lists)
- Missing column handling
- Expression queries
- Transactions
- Sequences
- DataTables & DataSets
- Packages/Procedures
- Functions
- All providers

---

# 🌟 Example Model

```csharp
[Table("TestDbExtensions")]
public class TestDbExtensionsModel
{
    public int Id { get; set; }
    public int IntValue { get; set; }

    [MapsTo("DoubleValue")]
    public double DoubleValue { get; set; }

    [DBFormat("yyyy-MM-dd")]
    public DateTime DateValue { get; set; }
}
```

---

# 📄 License

MIT
