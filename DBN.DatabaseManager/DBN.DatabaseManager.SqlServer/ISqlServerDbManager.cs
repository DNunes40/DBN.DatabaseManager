using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.SqlServer
{
    /// <summary>
    /// Interface for SQL Server-specific database manager functionality.
    /// Extends <see cref="IDbManager"/> with SQL Server-specific operations.
    /// </summary>
    public interface ISqlServerDbManager : IDbManager { }
}