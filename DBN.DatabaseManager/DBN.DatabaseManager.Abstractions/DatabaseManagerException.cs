using System.Data.Common;

namespace DBN.DatabaseManager.Abstractions
{
    /// <summary>
    /// Custom exception for database-related errors in UHS.DatabaseManager.
    /// </summary>
    public class DatabaseManagerException : Exception
    {
        /// <summary>
        /// The SQL statement that caused the error.
        /// </summary>
        public string Sql { get; }

        /// <summary>
        /// A string representation of the parameters passed to the command.
        /// </summary>
        public string Parameters { get; }

        /// <summary>
        /// Constructor for wrapping an inner exception.
        /// </summary>
        public DatabaseManagerException(Exception innerException, string? sql = null, IEnumerable<DbParameter>? parameters = null) : base(innerException.Message, innerException)
        {
            Sql = sql ?? string.Empty;
            Parameters = parameters == null || !parameters.Any() ? "" : string.Join("\n", parameters.Select(p => $"{p.ParameterName}={p.Value ?? ""}"));
        }

        /// <summary>
        /// Constructor for a custom error message (no inner exception).
        /// </summary>
        public DatabaseManagerException(string message, string? sql = null, IEnumerable<DbParameter>? parameters = null) : base(message)
        {
            Sql = sql ?? string.Empty;
            Parameters = parameters == null || !parameters.Any() ? "" : string.Join("\n", parameters.Select(p => $"{p.ParameterName}={p.Value ?? ""}"));
        }

        /// <summary>
        /// Constructor for a custom message with an inner exception.
        /// </summary>
        public DatabaseManagerException(string message, Exception? innerException, string? sql = null, IEnumerable<DbParameter>? parameters = null) : base(message, innerException)
        {
            Sql = sql ?? string.Empty;
            Parameters = parameters == null || !parameters.Any() ? "" : string.Join("\n", parameters.Select(p => $"{p.ParameterName}={p.Value ?? ""}"));
        }

        public override string ToString()
        {
            var name = InnerException == null ? "Exception" : InnerException!.GetType().Name;
            return $"{name}: {Message}\nSQL: {Sql}\nParameters: {Parameters}";
        }
    }
}
