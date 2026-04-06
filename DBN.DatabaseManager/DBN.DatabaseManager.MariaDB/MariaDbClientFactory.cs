using MySql.Data.MySqlClient;
using System.Data.Common;

namespace DBN.DatabaseManager.MariaDB
{
    public sealed class MariaDbClientFactory : DbProviderFactory
    {
        public static readonly MariaDbClientFactory Instance = new MariaDbClientFactory();

        private MariaDbClientFactory() { } 

        public override DbConnection CreateConnection()
        {
            return new MySqlConnection(); 
        }

        public override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }
    }
}