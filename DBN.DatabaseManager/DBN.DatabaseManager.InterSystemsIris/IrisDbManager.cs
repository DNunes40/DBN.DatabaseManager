using DBN.DatabaseManager.Abstractions;

namespace DBN.DatabaseManager.InterSystemsIris
{
    public class IrisDbManager : DbManager, IIrisDbManager
    {
        public IrisDbManager(string connectionString) : base(connectionString, IRISClientFactory.Instance, null, null) { }

        public IrisDbManager(string connectionString, IServiceProvider serviceProvider, string identifier) : base(connectionString, IRISClientFactory.Instance, serviceProvider, identifier) { }
    }
}