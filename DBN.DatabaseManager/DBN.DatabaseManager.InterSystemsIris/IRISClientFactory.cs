using InterSystems.Data.IRISClient;
using System.Data.Common;

namespace DBN.DatabaseManager.InterSystemsIris
{
    public sealed class IRISClientFactory : DbProviderFactory
    {
        public static readonly IRISClientFactory Instance = new IRISClientFactory();

        private IRISClientFactory() { } 

        public override DbCommand CreateCommand()
        {
            return new IRISCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new IRISConnection();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new IRISDataAdapter();
        }
    }
}