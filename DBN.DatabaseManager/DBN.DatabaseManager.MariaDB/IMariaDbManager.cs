using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.MariaDB
{
    /// <summary>
    /// Interface for MariaDB-specific database manager functionality.
    /// Extends <see cref="IDbManager"/> with MariaDB-specific operations.
    /// </summary>
    public interface IMariaDbManager : IDbManager { }
}