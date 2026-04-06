namespace DBN.DatabaseManager.Abstractions
{
    /// <summary>
    /// Specifies that a property maps to a different column name in the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MapsToAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the column in the database that this property maps to.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapsToAttribute"/> class.
        /// </summary>
        /// <param name="columnName">The name of the database column to map to.</param>
        public MapsToAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// Indicates that missing columns in the database should be ignored for the entire class.
    /// Useful when mapping a DataReader or DataSet to a class that might not have all columns present.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class IgnoreMissingColumnsAttribute : Attribute
    {
    }

    /// <summary>
    /// Indicates that a single property should be ignored if the corresponding database column is missing.
    /// Can be used in combination with <see cref="IgnoreMissingColumnsAttribute"/> or independently.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreIfMissingAttribute : Attribute
    {
    }

    /// <summary>
    /// Specifies a custom database format for parsing or formatting date/time values
    /// when the database column stores the value as a string rather than a native date/time type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DBFormatAttribute : Attribute
    {
        /// <summary>
        /// Gets the format string used to parse or format the database date/time value.
        /// For example: <c>"yyyyMMdd"</c>, <c>"dd-MM-yyyy HH:mm:ss"</c>, etc.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DBFormatAttribute"/> class.
        /// </summary>
        /// <param name="format">The database date/time format string.</param>
        /// <remarks>
        /// This format will only be applied when the source column value is a string.
        /// If the column type is a native <see cref="DateTime"/> (e.g., SQL Server <c>datetime</c> or Oracle <c>DATE</c>),
        /// the specified format will be ignored.
        /// </remarks>
        public DBFormatAttribute(string format)
        {
            Format = format;
        }
    }
}
