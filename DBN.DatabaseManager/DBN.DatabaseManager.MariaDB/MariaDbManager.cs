using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.MariaDB
{
    public class MariaDbManager : DbManager, IMariaDbManager
    {
        public MariaDbManager(string connectionString) : base(connectionString, MariaDbClientFactory.Instance, null, null) { }

        public MariaDbManager(string connectionString, IServiceProvider serviceProvider, string identifier) : base(connectionString, MariaDbClientFactory.Instance, serviceProvider, identifier) { }
    }
}