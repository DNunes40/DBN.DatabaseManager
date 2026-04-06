using System.Data.Common;

namespace DBN.DatabaseManager.Abstractions.Helpers
{
    internal class DbCommandHelper
    {
        public static DbCommand CreateCommand(DbProviderFactory factory, DbConnection connection, string sql, DbTransaction? transaction, DbParameter[]? parameters = null)
        {
            var command = factory.CreateCommand() ?? throw new DatabaseManagerException("Failed to create command.");

            command.Connection = connection;
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;

            if (parameters?.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            return command;
        }
    }
}
