using System.ComponentModel;

namespace DBN.DatabaseManager.Abstractions.Helpers
{
    public enum FactoryEnumType
    {
        [Description("OracleClientFactory")]
        Oracle,

        [Description("SqlClientFactory")]
        SqlServer,

        [Description("IRISClientFactory")]
        Iris,

        [Description("SqliteFactory")]
        Sqlite,

        [Description("MariaDbClientFactory")]
        MariaDb,

        [Description("MySqlClientFactory")]
        MySQL
    }
}
