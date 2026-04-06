using DBN.DatabaseManager.Abstractions;
using Microsoft.Data.SqlClient;

namespace DBN.DatabaseManager.SqlServer
{
    public class SqlServerDbManager : DbManager, ISqlServerDbManager
    {
        public SqlServerDbManager(string connectionString) : base(connectionString, SqlClientFactory.Instance, null, null) { }

        public SqlServerDbManager(string connectionString, IServiceProvider serviceProvider, string identifier) : base(connectionString, SqlClientFactory.Instance, serviceProvider, identifier) { }
    }
}