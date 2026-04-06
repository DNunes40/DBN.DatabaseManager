using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.SQLite
{
    /// <summary>
    /// Interface for SQLite-specific database manager functionality.
    /// Extends <see cref="IDbManager"/> with SQLite-specific operations.
    /// </summary>
    public interface ISQLiteDbManager : IDbManager { }
}