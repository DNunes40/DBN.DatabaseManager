using DBN.DatabaseManager.Abstractions;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace DBN.DatabaseManager.Oracle
{
    public class OracleDbManager : DbManager, IOracleDbManager
    {
        public OracleDbManager(string connectionString) : base(connectionString, OracleClientFactory.Instance, null, null) { }

        public OracleDbManager(string connectionString, IServiceProvider serviceProvider, string identifier) : base(connectionString, OracleClientFactory.Instance, serviceProvider, identifier) { }

        protected override DbParameter[] ProcessProcedureParameters(DbParameter[] parameters)
        {
            var list = parameters.ToList();

            list.Add(new OracleParameter
            {
                ParameterName = "rc",
                OracleDbType = OracleDbType.RefCursor,
                Direction = ParameterDirection.Output
            });

            return list.ToArray();
        }
    }
}