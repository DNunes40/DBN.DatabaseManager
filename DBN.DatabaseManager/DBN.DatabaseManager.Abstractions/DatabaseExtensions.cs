using DBN.DatabaseManager.Abstractions.Helpers;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DBN.DatabaseManager.Abstractions
{
    public static class DatabaseExtensions
    {
        private static readonly ConditionalWeakTable<DbDataReader, Dictionary<string, int>> OrdinalCache = new();

        /// <summary>
        /// Retrieves a list of all column names from the specified <see cref="DbDataReader"/>.
        /// </summary>
        public static List<string> GetDBColumns(this DbDataReader reader)
        {
            return GetDBColumnsImplementation(reader);
        }

        /// <summary>
        /// Retrieves a list of all column names from the first table in the specified <see cref="DataSet"/>.
        /// </summary>
        public static List<string> GetDBColumns(this DataSet dataSet)
        {
            return GetDBColumnsImplementation(dataSet);
        }

        private static List<string> GetDBColumnsImplementation<T>(T source)
        {
            var columns = new List<string>();

            if (source == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: source is null.");
            }

            if (source is DbDataReader reader)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);

                    columns.Add(columnName);
                }
            }
            else if (source is DataSet dataSet)
            {
                if (dataSet.Tables.Count > 0)
                {
                    foreach (DataColumn column in dataSet.Tables[0].Columns)
                    {
                        columns.Add(column.ColumnName);
                    }
                }
            }

            return columns;
        }



        /// <summary>
        /// Retrieves the specified column value as a string from a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to retrieve.</param>
        /// <returns>The string value, or <c>null</c> if empty or not found.</returns>
        public static string? GetDBString(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<string>(data.Value);
        }

        /// <summary>
        /// Retrieves the specified column value as a string from a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to retrieve.</param>
        /// <returns>The string value, or <c>null</c> if empty or not found.</returns>
        public static string? GetDBString(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<string>(data.Value);
        }



        /// <summary>
        /// Returns the first non-null string value found among the specified columns in a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The data reader source.</param>
        /// <param name="columns">The columns to check in order.</param>
        /// <returns>The first non-null string value, or <c>null</c> if none found.</returns>
        public static string? GetFirstDBString(this DbDataReader reader, params string[] columns)
        {
            return GetFirstDBStringImplementation(reader, columns);
        }

        /// <summary>
        /// Returns the first non-null string value found among the specified columns in a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="dataRow">The dataRow source.</param>
        /// <param name="columns">The columns to check in order.</param>
        /// <returns>The first non-null string value, or <c>null</c> if none found.</returns>
        public static string? GetFirstDBString(this DataRow dataRow, params string[] columns)
        {
            return GetFirstDBStringImplementation(dataRow, columns);
        }

        private static string? GetFirstDBStringImplementation<T>(T source, params string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                throw new DatabaseManagerException($"Invalid arguments passed: no columns specified.");
            }

            foreach (var column in columns)
            {
                if (string.IsNullOrWhiteSpace(column))
                {
                    continue;
                }

                string? value = null;

                try
                {
                    if (source is DbDataReader reader)
                    {
                        value = reader.GetDBString(column);
                    }
                    else if (source is DataRow dataRow)
                    {
                        value = dataRow.GetDBString(column);
                    }
                }
                catch (DatabaseManagerException)
                {
                    continue;
                }
                catch (Exception)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }



        /// <summary>
        /// Splits the string value of a column by the given separator and returns the element at the specified index.
        /// </summary>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to read from.</param>
        /// <param name="index">The zero-based index of the split segment.</param>
        /// <param name="separator">The string separator. Defaults to comma.</param>
        /// <returns>The split value at the given index, or <c>null</c> if not found.</returns>
        public static string? GetSplitDBStringAt(this DbDataReader reader, string column, int index, string separator = ",")
        {
            return GetSplitDBStringAtImplementation(reader, column, index, separator);
        }

        /// <summary>
        /// Splits the string value of a column by the given separator and returns the element at the specified index.
        /// </summary>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to read from.</param>
        /// <param name="index">The zero-based index of the split segment.</param>
        /// <param name="separator">The string separator. Defaults to comma.</param>
        /// <returns>The split value at the given index, or <c>null</c> if not found.</returns>
        public static string? GetSplitDBStringAt(this DataRow dataRow, string column, int index, string separator = ",")
        {
            return GetSplitDBStringAtImplementation(dataRow, column, index, separator);
        }

        private static string? GetSplitDBStringAtImplementation<T>(T source, string column, int index, string separator = ",")
        {
            if (index < 0)
            {
                throw new DatabaseManagerException($"GetSplitDBStringAt: Invalid arguments passed: index is negative.");
            }

            if (string.IsNullOrWhiteSpace(separator))
            {
                throw new DatabaseManagerException($"GetSplitDBStringAt: Invalid arguments passed: separator is null or empty.");
            }

            string? value = null;

            try
            {
                if (source is DbDataReader reader)
                {
                    value = reader.GetDBString(column);
                }
                else if (source is DataRow dataRow)
                {
                    value = dataRow.GetDBString(column);
                }
            }
            catch (DatabaseManagerException exc)
            {
                throw new DatabaseManagerException(exc.Message, exc);
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc.Message, exc);
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var parts = value.Split([separator], StringSplitOptions.None);

            if (index >= parts.Length)
            {
                return null;
            }

            return parts[index];
        }



        /// <summary>
        /// Retrieves and parses a date/time value from a column as a nullable <see cref="DateTime"/>.
        /// Supports multiple input formats including:
        /// <list type="bullet">
        /// <item>Standard <c>DateTime</c> objects from the database</item>
        /// <item>ISO-formatted date strings (e.g., "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss")</item>
        /// <item>Numeric date strings (e.g., "yyyyMMdd", "yyyyMMddHHmmss")</item>
        /// <item>Delimited date strings (e.g., "dd-MMM-yyyy", "dd-MMM-yyyy HH:mm:ss")</item>
        /// </list>
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> source.</param>
        /// <param name="column">The column name to read.</param>
        /// <param name="format">
        /// Optional. A custom date format string to use for parsing string values. 
        /// If empty, multiple default formats are attempted automatically.
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> value if parsing succeeds; otherwise, <c>null</c>.
        /// </returns>
        public static DateTime? GetDBDate(this DbDataReader reader, string column, string? format = null)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<DateTime>(data.Value, format);
        }

        /// <summary>
        /// Retrieves and parses a date/time value from a column as a nullable <see cref="DateTime"/>.
        /// Supports multiple input formats including:
        /// <list type="bullet">
        /// <item>Standard <c>DateTime</c> objects from the database</item>
        /// <item>ISO-formatted date strings (e.g., "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss")</item>
        /// <item>Numeric date strings (e.g., "yyyyMMdd", "yyyyMMddHHmmss")</item>
        /// <item>Delimited date strings (e.g., "dd-MMM-yyyy", "dd-MMM-yyyy HH:mm:ss")</item>
        /// </list>
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> source.</param>
        /// <param name="column">The column name to read.</param>
        /// <param name="format">
        /// Optional. A custom date format string to use for parsing string values. 
        /// If empty, multiple default formats are attempted automatically.
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> value if parsing succeeds; otherwise, <c>null</c>.
        /// </returns>
        public static DateTime? GetDBDate(this DataRow dataRow, string column, string? format = null)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<DateTime>(data.Value, format);
        }



        /// <summary>
        /// Retrieves and parses a column value as an integer.
        /// </summary>
        /// <returns>
        /// The integer value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static int? GetDBInt(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<int>(data.Value);
        }

        /// <summary>
        /// Retrieves and parses a column value as an integer.
        /// </summary>
        /// <returns>
        /// The integer value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static int? GetDBInt(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<int>(data.Value);
        }



        /// <summary>
        /// Retrieves and parses a column value as a double.
        /// </summary>
        /// <returns>
        /// The double-precision value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static double? GetDBDouble(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<double>(data.Value);
        }

        /// <summary>
        /// Retrieves and parses a column value as a double.
        /// </summary>
        /// <returns>
        /// The double-precision value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static double? GetDBDouble(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<double>(data.Value);
        }


        /// <summary>
        /// Retrieves and parses a column value as a single-precision float.
        /// </summary>
        /// <returns>
        /// The float value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static float? GetDBFloat(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<float>(data.Value);
        }

        /// <summary>
        /// Retrieves and parses a column value as a single-precision float.
        /// </summary>
        /// <returns>
        /// The float value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static float? GetDBFloat(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<float>(data.Value);
        }



        /// <summary>
        /// Retrieves and parses a column value as a 64-bit integer.
        /// </summary>
        /// <returns>
        /// The long value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static long? GetDBLong(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<long>(data.Value);
        }

        /// <summary>
        /// Retrieves and parses a column value as a 64-bit integer.
        /// </summary>
        /// <returns>
        /// The long value of the specified column, or <c>null</c> if the value is null or cannot be converted.
        /// </returns>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static long? GetDBLong(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<long>(data.Value);
        }



        /// <summary>
        /// Retrieves and parses the value of a specified column as a nullable boolean.
        /// <para>
        /// Recognizes the following string values (case-insensitive) as <c>true</c>: 
        /// "TRUE", "1", "Y", "YES", "ON", "ENABLED", "ACTIVE", "T", "OK", "PASS", "SUCCESS", "UP", "OPEN", "VALID", "CONFIRM", "AGREE", "ALLOW".
        /// </para>
        /// <para>
        /// Recognizes the following string values (case-insensitive) as <c>false</c>: 
        /// "FALSE", "0", "N", "NO", "OFF", "DISABLED", "INACTIVE", "F", "FAIL", "ERROR", "DOWN", "CLOSED", "INVALID", "REJECT", "DISALLOW".
        /// </para>
        /// <para>
        /// Returns <c>null</c> if the column is missing, the value is <c>null</c> / <c>DBNull</c>, or cannot be interpreted as a boolean.
        /// </para>
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to read from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A nullable boolean representing the parsed value of the column, or <c>null</c> if parsing fails or the value is missing.
        /// </returns>
        public static bool? GetDBBool(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<bool>(data.Value);
        }

        /// <summary>
        /// Retrieves and parses the value of a specified column from a <see cref="DataRow"/> as a nullable boolean.
        /// <para>
        /// Recognizes the following string values (case-insensitive) as <c>true</c>: 
        /// "TRUE", "1", "Y", "YES", "ON", "ENABLED", "ACTIVE", "T", "OK", "PASS", "SUCCESS", "UP", "OPEN", "VALID", "CONFIRM", "AGREE", "ALLOW".
        /// </para>
        /// <para>
        /// Recognizes the following string values (case-insensitive) as <c>false</c>: 
        /// "FALSE", "0", "N", "NO", "OFF", "DISABLED", "INACTIVE", "F", "FAIL", "ERROR", "DOWN", "CLOSED", "INVALID", "REJECT", "DISALLOW".
        /// </para>
        /// <para>
        /// Returns <c>null</c> if the column is missing, the value is <c>null</c> / <c>DBNull</c>, or cannot be interpreted as a boolean.
        /// </para>
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> to read from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A nullable boolean representing the parsed value of the column, or <c>null</c> if parsing fails or the value is missing.
        /// </returns>
        public static bool? GetDBBool(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<bool>(data.Value);
        }


        /// <summary>
        /// Retrieves and parses the value of a specified column as a nullable decimal.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to read from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A nullable decimal representing the parsed value of the column, 
        /// or <c>null</c> if the column is missing, the value is <c>null</c>/<c>DBNull</c>, 
        /// or cannot be converted to a decimal.
        /// </returns>
        public static decimal? GetDBDecimal(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<decimal>(data.Value);
        }

        /// <summary>
        /// Retrieves and parses the value of a specified column from a <see cref="DataRow"/> as a nullable decimal.
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> to read from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A nullable decimal representing the parsed value of the column, 
        /// or <c>null</c> if the column is missing, the value is <c>null</c>/<c>DBNull</c>, 
        /// or cannot be converted to a decimal.
        /// </returns>
        public static decimal? GetDBDecimal(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<decimal>(data.Value);
        }


        /// <summary>
        /// Deserializes the JSON value from a column into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize into.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c> if parsing fails.</returns>
        public static T? GetDBParseJson<T>(this DbDataReader reader, string column)
        {
            return GetDBParseJsonImplementation<DbDataReader, T>(reader, column);
        }

        /// <summary>
        /// Deserializes the JSON value from a column into an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize into.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/>, or <c>null</c> if parsing fails.</returns>
        public static T? GetDBParseJson<T>(this DataRow dataRow, string column)
        {
            return GetDBParseJsonImplementation<DataRow, T>(dataRow, column);
        }

        private static TResult? GetDBParseJsonImplementation<TSource, TResult>(TSource source, string column)
        {
            var value = GetColumnObject(source, column);

            if (value == null || value.Value == null)
            {
                return default;
            }

            var json = value.Value.ToString()?.Trim();

            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<TResult>(json, _jsonCaseJsonOptions);
            }
            catch
            {
                return default;
            }
        }



        /// <summary>
        /// Retrieves a column value as a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The parsed <see cref="Guid"/>, or <c>null</c> if parsing fails.</returns>
        /// <param name="reader">The data reader source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static Guid? GetDBGuid(this DbDataReader reader, string column)
        {
            var data = GetColumnObject(reader, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<Guid>(data.Value);
        }

        /// <summary>
        /// Retrieves a column value as a <see cref="Guid"/>.
        /// </summary>
        /// <returns>The parsed <see cref="Guid"/>, or <c>null</c> if parsing fails.</returns>
        /// <param name="dataRow">The data source.</param>
        /// <param name="column">The column name to retrieve.</param>
        public static Guid? GetDBGuid(this DataRow dataRow, string column)
        {
            var data = GetColumnObject(dataRow, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            return ConvertValueAuto<Guid>(data.Value);
        }


        /// <summary>
        /// Retrieves a binary or text column as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to read the column from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A <see cref="Stream"/> representing the column data, or <c>null</c> if the column value is <c>null</c> or unavailable.
        /// </returns>
        /// <remarks>
        /// The returned <see cref="Stream"/> is internally a <see cref="MemoryStream"/>. 
        /// Use <see cref="GetDBMemoryStream(DbDataReader, string)"/> if you require a memory-resident stream with <c>Position</c> set to 0.
        /// </remarks>
        public static Stream? GetDBStream(this DbDataReader reader, string column)
        {
            return GetDBStreamImplementation(reader, column);
        }

        /// <summary>
        /// Retrieves a binary or text column as a <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to read the column from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A <see cref="MemoryStream"/> representing the column data, or <c>null</c> if the column value is <c>null</c> or unavailable.
        /// </returns>
        /// <remarks>
        /// The returned <see cref="MemoryStream"/> has its <c>Position</c> set to 0, making it ready for reading.
        /// </remarks>
        public static MemoryStream? GetDBMemoryStream(this DbDataReader reader, string column)
        {
            var memoryStream = GetDBStreamImplementation(reader, column);

            if (memoryStream == null)
            {
                return null;
            }

            memoryStream.Position = 0; // reset to start
            return memoryStream;
        }

        /// <summary>
        /// Retrieves a binary or text column as a <see cref="Stream"/>.
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> to read the column from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A <see cref="Stream"/> representing the column data, or <c>null</c> if the column value is <c>null</c> or unavailable.
        /// </returns>
        public static Stream? GetDBStream(this DataRow dataRow, string column)
        {
            return GetDBStreamImplementation(dataRow, column);
        }

        /// <summary>
        /// Retrieves a binary or text column as a <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> to read the column from.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A <see cref="MemoryStream"/> representing the column data, or <c>null</c> if the column value is <c>null</c> or unavailable.
        /// </returns>
        /// <remarks>
        /// The returned <see cref="MemoryStream"/> has its <c>Position</c> set to 0, making it ready for reading.
        /// </remarks>
        public static MemoryStream? GetDBMemoryStream(this DataRow dataRow, string column)
        {
            var memoryStream = GetDBStreamImplementation(dataRow, column);

            if (memoryStream == null)
            {
                return null;
            }

            memoryStream.Position = 0; // reset to start
            return memoryStream;
        }

        private static MemoryStream? GetDBStreamImplementation<T>(T source, string column)
        {
            var data = GetColumnObject(source, column);

            if (data == null || data.Value == null)
            {
                return null;
            }

            if (data.Value is byte[] bytes)
            {
                return new MemoryStream(bytes);
            }

            if (data.Value is IConvertible)
            {
                try
                {
                    var bytesFromString = System.Text.Encoding.UTF8.GetBytes(data.Value.ToString()!);
                    return new MemoryStream(bytesFromString);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }



        /// <summary>
        /// Retrieves the value of a specified column from a <see cref="DbDataReader"/> 
        /// and converts it to the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to convert the value to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> instance to read from.</param>
        /// <param name="column">The name of the column whose value to retrieve.</param>
        /// <returns>
        /// The converted value of type <typeparamref name="T"/> if the column exists and is not null; 
        /// otherwise, <c>default(T)</c>.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="reader"/> is null, the column does not exist, 
        /// or the value cannot be converted to the specified type.
        /// </exception>
        public static T? GetDBValueAs<T>(this DbDataReader reader, string column)
        {
            return GetDBValueAsImplementation<DbDataReader, T>(reader, column);
        }

        /// <summary>
        /// Retrieves the value of a specified column from a <see cref="DataRow"/> 
        /// and converts it to the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target type to convert the value to.</typeparam>
        /// <param name="dataRow">The <see cref="DataRow"/> instance to read from.</param>
        /// <param name="column">The name of the column whose value to retrieve.</param>
        /// <returns>
        /// The converted value of type <typeparamref name="T"/> if the column exists and is not null; 
        /// otherwise, <c>default(T)</c>.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="dataRow"/> is null, the column does not exist, 
        /// or the value cannot be converted to the specified type.
        /// </exception>
        public static T? GetDBValueAs<T>(this DataRow dataRow, string column)
        {
            return GetDBValueAsImplementation<DataRow, T>(dataRow, column);
        }

        private static TResult? GetDBValueAsImplementation<TSource, TResult>(TSource source, string column)
        {
            var data = GetColumnObject(source, column);

            if (data == null)
            {
                return default;
            }

            return ConvertValueAuto<TResult>(data.Value);
        }


        internal static TResult? ConvertValueAuto<TResult>(object? value, string? format = null)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            if (value is string)  // string-based (robust)
            {
                return ConvertValue<TResult>(value, format);       
            }

            // type-based (fast)
            return ConvertValueStrict<TResult>(value);      
        }

        // STRING-based
        private static TResult? ConvertValue<TResult>(object? value, string? format = null)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            Type targetType = typeof(TResult);

            Type? underlyingType = Nullable.GetUnderlyingType(targetType);

            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            var strValue = value.ToString()?.Trim();

            if (string.IsNullOrEmpty(strValue))
            {
                return default;
            }

            try
            {
                // STRING
                if (targetType == typeof(string))
                {
                    return (TResult)(object)strValue;
                }

                // INT
                if (targetType == typeof(int))
                {
                    return int.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var i) ? (TResult)(object)i : default;
                }

                // LONG
                if (targetType == typeof(long))
                {
                    return long.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var l) ? (TResult)(object)l : default;
                }

                // DOUBLE
                if (targetType == typeof(double))
                {
                    return double.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? (TResult)(object)d : default;
                }

                // FLOAT
                if (targetType == typeof(float))
                {
                    return float.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var f) ? (TResult)(object)f : default;
                }

                // DECIMAL
                if (targetType == typeof(decimal))
                {
                    return decimal.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec) ? (TResult)(object)dec : default;
                }

                // BOOL
                if (targetType == typeof(bool))
                {
                    if (TrueValues.Contains(strValue))
                    {
                        return (TResult)(object)true;
                    }

                    if (FalseValues.Contains(strValue))
                    {
                        return (TResult)(object)false;
                    }

                    return default;
                }

                // GUID
                if (targetType == typeof(Guid))
                {
                    return Guid.TryParse(strValue, out var guid) ? (TResult)(object)guid : default;
                }

                // DATETIME
                if (targetType == typeof(DateTime))
                {
                    if (string.IsNullOrWhiteSpace(format))
                    {
                        return DateTime.TryParse(strValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? (TResult)(object)dt : default;
                    }

                    if (DateTime.TryParseExact(strValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExact))
                    {
                        return (TResult)(object)dtExact;
                    }

                    return default;
                }

                // ENUM
                if (targetType.IsEnum)
                {
                    return Enum.TryParse(targetType, strValue, true, out var enumVal) ? (TResult)enumVal : default;
                }

                // FALLBACK
                return (TResult)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return default;
            }
        }

        // TYPE-based
        private static TResult? ConvertValueStrict<TResult>(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            Type targetType = typeof(TResult);
            Type? underlyingType = Nullable.GetUnderlyingType(targetType);

            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            try
            {
                // Direct match (FAST PATH)
                if (value.GetType() == targetType)
                {
                    return (TResult)value;
                }

                // STRING
                if (targetType == typeof(string))
                {
                    return (TResult)(object)value.ToString()!;
                }

                // NUMERIC (no string conversion)
                if (targetType == typeof(int))
                {
                    return (TResult)(object)Convert.ToInt32(value);
                }

                if (targetType == typeof(long))
                {
                    return (TResult)(object)Convert.ToInt64(value);
                }

                if (targetType == typeof(double))
                {
                    return (TResult)(object)Convert.ToDouble(value);
                }

                if (targetType == typeof(float))
                {
                    return (TResult)(object)Convert.ToSingle(value);
                }

                if (targetType == typeof(decimal))
                {
                    return (TResult)(object)Convert.ToDecimal(value);
                }

                // BOOL
                if (targetType == typeof(bool))
                {
                    if (value is bool b)
                    {
                        return (TResult)(object)b;
                    }

                    if (value is IConvertible)
                    {
                        return (TResult)(object)(Convert.ToDecimal(value) != 0);
                    }

                    return default;
                }

                // GUID (your DB = string → handled safely)
                if (targetType == typeof(Guid))
                {
                    if (value is Guid g)
                    {
                        return (TResult)(object)g;
                    }

                    return default;
                }

                // DATETIME
                if (targetType == typeof(DateTime))
                {
                    if (value is DateTime dt)
                    {
                        return (TResult)(object)dt;
                    }

                    return default;
                }

                // ENUM
                if (targetType.IsEnum)
                {
                    return (TResult)Enum.ToObject(targetType, value);
                }

                // FALLBACK
                return (TResult)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return default;
            }
        }


        /// <summary>
        /// Retrieves the number of rows in the first table of the specified <see cref="DataSet"/>.
        /// </summary>
        /// <returns>The number of rows.</returns>
        public static int GetDBRowsCount(this DataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: dataSet is null.");
            }

            if (dataSet.Tables.Count == 0)
            {
                return 0;
            }

            return dataSet.Tables[0].Rows.Count;
        }



        /// <summary>
        /// Retrieves a <see cref="DataRow"/> at the specified zero-based index 
        /// from the first table in the provided <see cref="DataSet"/>.
        /// </summary>
        /// <param name="dataSet">The <see cref="DataSet"/> containing the data.</param>
        /// <param name="index">The zero-based index of the row to retrieve.</param>
        /// <returns>
        /// The <see cref="DataRow"/> at the specified index in the first table.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description><paramref name="dataSet"/> is null.</description></item>
        /// <item><description>The <see cref="DataSet"/> contains no tables.</description></item>
        /// <item><description>The first table contains no rows.</description></item>
        /// <item><description>The specified <paramref name="index"/> is out of range.</description></item>
        /// </list>
        /// </exception>
        public static DataRow GetDBRow(this DataSet dataSet, int index)
        {
            if (dataSet == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: dataSet is null.");
            }

            if (dataSet.Tables.Count == 0)
            {
                throw new DatabaseManagerException($"Invalid arguments passed: dataSet has no Tables.");
            }

            if (dataSet.Tables[0].Rows.Count == 0)
            {
                throw new DatabaseManagerException($"Invalid arguments passed: dataset table has no rows.");
            }

            var table = dataSet.Tables[0];

            if (index < 0 || index >= table.Rows.Count)
            {
                throw new DatabaseManagerException($"Invalid arguments passed: row index {index} is out of range.");
            }

            return table.Rows[index];
        }



        /// <summary>
        /// Converts the current result set of a <see cref="DbDataReader"/> into a <see cref="DataTable"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to convert.</param>
        /// <returns>
        /// A <see cref="DataTable"/> containing all rows and columns from the current reader result set.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="reader"/> is null.
        /// </exception>
        public static async Task<DataTable> ToDataTable(this DbDataReader reader)
        {
            if (reader == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: reader is null.");
            }

            var table = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var colName = reader.GetName(i);
                var colType = reader.GetFieldType(i);

                var column = new DataColumn(colName, colType);
                table.Columns.Add(column);
            }

            while (reader.Read())
            {
                var row = table.NewRow();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                    row[i] = value;
                }

                table.Rows.Add(row);
            }

            await reader.DisposeAsync();

            return table;
        }



        /// <summary>
        /// Determines whether the specified column exists within a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance to check.</param>
        /// <param name="column">The name of the column to look for.</param>
        /// <returns>
        /// <c>true</c> if the column exists in the reader; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="reader"/> is null.
        /// </exception>
        public static bool HasColumn(this DbDataReader reader, string column)
        {
            if (reader == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: reader is null.");
            }

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(column, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }



        /// <summary>
        /// Determines whether the specified column exists within the <see cref="DataTable"/> 
        /// associated with the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> whose table is to be checked.</param>
        /// <param name="column">The name of the column to look for.</param>
        /// <returns>
        /// <c>true</c> if the column exists in the table; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="dataRow"/> is null.
        /// </exception>
        public static bool HasColumn(this DataRow dataRow, string column)
        {
            if (dataRow == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: dataRow is null.");
            }

            return dataRow.Table.Columns.Contains(column);
        }



        /// <summary>
        /// Determines whether the value of the specified column in a <see cref="DbDataReader"/> 
        /// is <see cref="DBNull.Value"/>.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> instance to inspect.</param>
        /// <param name="column">The name of the column to check.</param>
        /// <returns>
        /// <c>true</c> if the column value is <see cref="DBNull.Value"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="reader"/> is null.
        /// </exception>
        public static bool IsDBValueNull(this DbDataReader reader, string column)
        {
            if (reader == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: reader is null.");
            }

            return reader[column] == DBNull.Value;
        }



        /// <summary>
        /// Determines whether the value of the specified column in a <see cref="DataRow"/> 
        /// is <see cref="DBNull.Value"/>.
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> instance to inspect.</param>
        /// <param name="column">The name of the column to check.</param>
        /// <returns>
        /// <c>true</c> if the column value is <see cref="DBNull.Value"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when <paramref name="dataRow"/> is null.
        /// </exception>
        public static bool IsDBValueNull(this DataRow dataRow, string column)
        {
            if (dataRow == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: dataRow is null.");
            }

            return dataRow[column] == DBNull.Value;
        }



        /// <summary>
        /// Converts the current <see cref="DataRow"/> into a dictionary of column names and their corresponding values.
        /// </summary>
        /// <param name="dataRow">
        /// The <see cref="DataRow"/> instance to convert.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> where each key represents a column name and each value represents the corresponding cell value.
        /// Returns <see langword="null"/> if the conversion fails.
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown when the provided <paramref name="dataRow"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// This method is useful for serializing a row’s data or performing dynamic column-based lookups.
        /// Database <see cref="DBNull"/> values are preserved as-is in the resulting dictionary.
        /// </remarks>
        public static Dictionary<string, object?>? GetDBRowAsDictionary(this DataRow dataRow)
        {
            if (dataRow == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: dataRow is null.");
            }

            try
            {
                return dataRow.Table.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName, c => (object?)dataRow[c]);
            }
            catch
            {
                return default;
            }
        }



        /// <summary>
        /// Maps the current <see cref="DbDataReader"/> row(s) to an instance of <typeparamref name="T"/>.
        /// Supports mapping to a single object or a list of objects (e.g., <c>List&lt;PatientModel&gt;</c>).
        /// </summary>
        /// <typeparam name="T">The type to map the data to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> containing the data.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> populated with data from the reader, 
        /// or <c>null</c> if no rows are present. For list types, an empty list is returned if no rows exist.
        /// </returns>
        /// <remarks>
        /// This method uses a high-performance reflection-based mapper internally. Supports:
        /// <list type="bullet">
        /// <item>Case-insensitive property matching</item>
        /// <item>Optional <c>[MapsTo("ColumnName")]</c> attribute for custom column mapping</item>
        /// <item>Automatic mapping of nullable types</item>
        /// </list>
        /// </remarks>
        public static async Task<T?> MapTo<T>(this DbDataReader reader)
        {
            var result = FastMapper.MapReader(reader, typeof(T));
            await reader.DisposeAsync();
            return (T?)result;
        }

        /// <summary>
        /// Maps the first <see cref="DataTable"/> of a <see cref="DataSet"/> to an instance of <typeparamref name="T"/>.
        /// Supports mapping to a single object or a list of objects (e.g., <c>List&lt;PatientModel&gt;</c>).
        /// </summary>
        /// <typeparam name="T">The type to map the data to.</typeparam>
        /// <param name="dataSet">The <see cref="DataSet"/> containing one or more tables.</param>
        /// <param name="index">The index of the table to map (default is 0).</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> populated with data from the specified table, 
        /// or <c>null</c> if no rows are present. For list types, an empty list is returned if no rows exist.
        /// </returns>
        /// <remarks>
        /// Internally creates a <see cref="DbDataReader"/> from the specified table and uses a high-performance mapper. Supports:
        /// <list type="bullet">
        /// <item>Case-insensitive property matching</item>
        /// <item>Optional <c>[MapsTo("ColumnName")]</c> attribute for custom column mapping</item>
        /// <item>Automatic mapping of nullable types</item>
        /// </list>
        /// </remarks>
        public static T? MapTo<T>(this DataSet dataSet, int index = 0)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
            {
                return default;
            }

            var table = dataSet.Tables[index];

            using var reader = table.CreateDataReader();

            var result = FastMapper.MapReader(reader, typeof(T));

            dataSet.Dispose();

            return (T?)result;
        }


        /// <summary>
        /// Converts an object to its JSON string representation, with optional pretty-printing.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object instance to serialize to JSON.</param>
        /// <param name="prettyPrint">
        /// If <c>true</c>, the JSON output will be indented for readability;
        /// otherwise, it will be compact (no extra whitespace).
        /// </param>
        /// <returns>
        /// A JSON string representation of the object if serialization succeeds; 
        /// otherwise, an empty string (<see cref="string.Empty"/>).
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="System.Text.Json.JsonSerializer"/> for serialization.
        /// If <paramref name="obj"/> is <c>null</c> or serialization fails, 
        /// it returns <see cref="string.Empty"/> instead of throwing an exception.
        /// </remarks>
        /// <example>
        /// The following example demonstrates both compact and pretty-printed output:
        /// <code>
        /// var person = new { Name = "Alice", Age = 30 };
        ///
        /// string compact = person.ToJson(); // {"Name":"Alice","Age":30}
        /// string pretty = person.ToJson(prettyPrint: true);
        /// // {
        /// //   "Name": "Alice",
        /// //   "Age": 30
        /// // }
        /// </code>
        /// </example>
        public static string ToJson<T>(this T obj, bool prettyPrint = false)
        {
            try
            {
                if (obj == null)
                {
                    return string.Empty;
                }

                var options = prettyPrint ? _prettyJsonOptions : _defaultJsonOptions;

                return JsonSerializer.Serialize(obj, options);
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Deserializes a JSON string into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize into.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> if successful; 
        /// otherwise, <c>default</c>.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="JsonSerializer"/>.
        /// Returns <c>default</c> if <paramref name="json"/> is null, empty,
        /// or if deserialization fails.
        /// </remarks>
        public static T? FromJson<T>(this string? json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(json, _jsonCaseJsonOptions);
            }
            catch
            {
                return default;
            }
        }


        

        /// <summary>
        /// Retrieves the value and the type of a specified column from a data source.
        /// Supports <see cref="DbDataReader"/> and <see cref="DataRow"/> sources.
        /// </summary>
        /// <typeparam name="T">The type of the data source, either <see cref="DbDataReader"/> or <see cref="DataRow"/>.</typeparam>
        /// <param name="source">The data source containing the column.</param>
        /// <param name="column">The name of the column to retrieve.</param>
        /// <returns>
        /// A <see cref="DBValueType"/> containing:
        /// <list type="bullet">
        /// <item><description><see cref="DBValueType.Value"/> — the value of the column, or <c>null</c> if the column contains <see cref="DBNull"/> or <c>null</c>.</description></item>
        /// <item><description><see cref="DBValueType.PropertyType"/> — the .NET type of the column.</description></item>
        /// </list>
        /// </returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown if <paramref name="source"/> is <c>null</c>, <paramref name="column"/> is null or whitespace, 
        /// the column does not exist, or the <paramref name="source"/> type is unsupported.
        /// </exception>
        private static DBValueType? GetColumnObject<T>(T source, string column)
        {
            if (source == null)
            {
                throw new DatabaseManagerException("Invalid arguments passed: source is null");
            }

            if (string.IsNullOrWhiteSpace(column))
            {
                throw new DatabaseManagerException("Invalid arguments passed: column name is null or empty");
            }

            if (source is DbDataReader reader)
            {
                try
                {
                    int ordinal = GetOrdinalCached(reader, column);

                    var value = reader.GetValue(ordinal);

                    var valueInfo = new DBValueType
                    {
                        Value = value is DBNull ? null : value,
                        PropertyType = reader.GetFieldType(ordinal)
                    };

                    return valueInfo;
                }
                catch (IndexOutOfRangeException exc)
                {
                    throw new DatabaseManagerException(
                        $"Column '{column}' does not exist. Available columns: " +
                        string.Join(", ", Enumerable.Range(0, reader.FieldCount).Select(reader.GetName)), exc);
                }
                catch (Exception exc)
                {
                    throw new DatabaseManagerException($"Error accessing column '{column}': {exc.Message}", exc);
                }
            }

            if (source is DataRow row)
            {
                if (row.Table == null || row.Table.Rows.Count == 0)
                {
                    throw new DatabaseManagerException($"Invalid arguments passed: the dataRow is not associated with a valid table or the table has no rows.");
                }

                if (!row.Table.Columns.Contains(column))
                {
                    throw new DatabaseManagerException(
                        $"Column '{column}' does not exist. Available columns: " +
                        string.Join(", ", row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                }

                var columnObj = row.Table.Columns[column];

                if (columnObj == null)
                {
                    throw new DatabaseManagerException($"Column '{column}' does not exist. Available columns: " + string.Join(", ", row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                }

                var value = row[columnObj];

                return new DBValueType
                {
                    Value = value is DBNull ? null : value,
                    PropertyType = columnObj.DataType
                };
            }

            throw new DatabaseManagerException($"Invalid arguments passed: source of type '{source?.GetType().Name ?? "null"}' is not supported.");
        }

        private static int GetOrdinalCached(DbDataReader reader, string column)
        {
            var map = OrdinalCache.GetValue(reader, _ => new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

            // Load cache once
            if (map.Count == 0)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    map[reader.GetName(i)] = i;
                }
            }

            // Try get
            if (!map.TryGetValue(column, out var ordinal))
            {
                throw new IndexOutOfRangeException($"Column '{column}' not found.");
            }

            return ordinal;
        }

        private static readonly JsonSerializerOptions _defaultJsonOptions = new()
        {
            WriteIndented = false
        };

        private static readonly JsonSerializerOptions _prettyJsonOptions = new()
        {
            WriteIndented = true
        };

        private static readonly JsonSerializerOptions _jsonCaseJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly HashSet<string> TrueValues = new(StringComparer.OrdinalIgnoreCase)
        {
            "TRUE", "1", "Y", "YES", "ON", "ENABLED", "ACTIVE", "T", "OK", "PASS", "SUCCESS", "UP", "OPEN", "VALID", "CONFIRM", "AGREE", "ALLOW"
        };

        private static readonly HashSet<string> FalseValues = new(StringComparer.OrdinalIgnoreCase)
        {
            "FALSE", "0", "N", "NO", "OFF", "DISABLED", "INACTIVE", "F", "FAIL", "ERROR", "DOWN", "CLOSED", "INVALID", "REJECT", "DISALLOW"
        };
    }
}
