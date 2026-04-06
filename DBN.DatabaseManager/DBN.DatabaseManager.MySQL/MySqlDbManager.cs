using DBN.DatabaseManager.Abstractions;
using MySql.Data.MySqlClient;

namespace DBN.DatabaseManager.MySQL
{
    public class MySqlDbManager : DbManager, IMySqlDbManager
    {
        public MySqlDbManager(string connectionString) : base(connectionString, MySqlClientFactory.Instance, null, null) { }

        public MySqlDbManager(string connectionString, IServiceProvider serviceProvider, string identifier) : base(connectionString, MySqlClientFactory.Instance, serviceProvider, identifier) { }
    }
}