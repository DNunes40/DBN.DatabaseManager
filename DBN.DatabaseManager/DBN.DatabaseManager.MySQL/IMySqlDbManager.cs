using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.MySQL
{
    /// <summary>
    /// Interface for MySQL-specific database manager functionality.
    /// Extends <see cref="IDbManager"/> with MySQL-specific operations.
    /// </summary>
    public interface IMySqlDbManager : IDbManager { }
}