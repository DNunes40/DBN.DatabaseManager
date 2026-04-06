using DBN.DatabaseManager.Abstractions;
using Microsoft.Data.Sqlite;

namespace DBN.DatabaseManager.SQLite
{
    public class SQLiteDbManager : DbManager, ISQLiteDbManager
    {
        public SQLiteDbManager(string connectionString) : base(connectionString, SqliteFactory.Instance, null, null) { }

        public SQLiteDbManager(string connectionString, IServiceProvider serviceProvider, string identifier) : base(connectionString, SqliteFactory.Instance, serviceProvider, identifier) { }
    }
}