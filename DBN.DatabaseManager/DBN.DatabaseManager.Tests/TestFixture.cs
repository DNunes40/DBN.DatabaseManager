using DBN.DatabaseManager.Abstractions;
using DBN.DatabaseManager.Abstractions.Helpers;
using DBN.DatabaseManager.InterSystemsIris;
using DBN.DatabaseManager.MariaDB;
using DBN.DatabaseManager.MySQL;
using DBN.DatabaseManager.Oracle;
using DBN.DatabaseManager.SQLite;
using DBN.DatabaseManager.SqlServer;
using InterSystems.Data.IRISClient;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Reflection;

namespace DBN.DatabaseManager.Tests
{
    public class TestFixture : IAsyncLifetime
    {
        public ServiceProvider ServiceProvider { get; }

        public static bool NeedTableCleanup = false;

        public TestFixture()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            var configPath = Path.Combine(basePath, "appsettings.json");

            var configuration = new ConfigurationBuilder().SetBasePath(basePath).AddJsonFile(configPath, optional: false, reloadOnChange: true).Build();

            Environment.SetEnvironmentVariable("TNS_ADMIN", configuration.GetValue<string>("OracleTnsPath"));

            var services = new ServiceCollection();

            services.AddScoped<IOracleDbManager>(provider => new OracleDbManager(configuration.GetConnectionString("ORACLE")!));

            services.AddScoped<IOracleDbManager>(provider => new OracleDbManager(configuration.GetConnectionString("ORACLE")!, provider, $"{FactoryEnumType.Oracle.ToString()}1"));

            services.AddScoped<IOracleDbManager>(provider => new OracleDbManager(configuration.GetConnectionString("ORACLE")!, provider, $"{FactoryEnumType.Oracle.ToString()}2"));

            services.AddScoped<ISqlServerDbManager>(provider => new SqlServerDbManager(configuration.GetConnectionString("SQLSERVER")!));

            services.AddScoped<ISqlServerDbManager>(provider => new SqlServerDbManager(configuration.GetConnectionString("SQLSERVER")!, provider, $"{FactoryEnumType.SqlServer.ToString()}1"));

            services.AddScoped<ISqlServerDbManager>(provider => new SqlServerDbManager(configuration.GetConnectionString("SQLSERVER")!, provider, $"{FactoryEnumType.SqlServer.ToString()}2"));

            services.AddScoped<IIrisDbManager>(provider => new IrisDbManager(configuration.GetConnectionString("IRIS")!));

            services.AddScoped<IIrisDbManager>(provider => new IrisDbManager(configuration.GetConnectionString("IRIS")!, provider, $"{FactoryEnumType.Iris.ToString()}1"));

            services.AddScoped<IIrisDbManager>(provider => new IrisDbManager(configuration.GetConnectionString("IRIS")!, provider, $"{FactoryEnumType.Iris.ToString()}2"));

            services.AddScoped<ISQLiteDbManager>(provider => new SQLiteDbManager(Path.Combine(basePath, "SQLite.db")));

            services.AddScoped<ISQLiteDbManager>(provider => new SQLiteDbManager(Path.Combine(basePath, "SQLite.db"), provider, $"{FactoryEnumType.Sqlite.ToString()}1"));

            services.AddScoped<ISQLiteDbManager>(provider => new SQLiteDbManager(Path.Combine(basePath, "SQLite.db"), provider, $"{FactoryEnumType.Sqlite.ToString()}2"));

            services.AddScoped<IMariaDbManager>(provider => new MariaDbManager(configuration.GetConnectionString("MARIADB")!));

            services.AddScoped<IMariaDbManager>(provider => new MariaDbManager(configuration.GetConnectionString("MARIADB")!, provider, $"{FactoryEnumType.MariaDb.ToString()}1"));

            services.AddScoped<IMariaDbManager>(provider => new MariaDbManager(configuration.GetConnectionString("MARIADB")!, provider, $"{FactoryEnumType.MariaDb.ToString()}2"));

            services.AddScoped<IMySqlDbManager>(provider => new MySqlDbManager(configuration.GetConnectionString("MYSQL")!));

            services.AddScoped<IMySqlDbManager>(provider => new MySqlDbManager(configuration.GetConnectionString("MYSQL")!, provider, $"{FactoryEnumType.MySQL.ToString()}1"));

            services.AddScoped<IMySqlDbManager>(provider => new MySqlDbManager(configuration.GetConnectionString("MYSQL")!, provider, $"{FactoryEnumType.MySQL.ToString()}2"));

            ServiceProvider = services.BuildServiceProvider();
        }

        public IDbManager GetProvider(FactoryEnumType provider)
        {
            return provider switch
            {
                FactoryEnumType.SqlServer => ServiceProvider.GetRequiredService<ISqlServerDbManager>(),
                FactoryEnumType.Sqlite => ServiceProvider.GetRequiredService<ISQLiteDbManager>(),
                FactoryEnumType.MySQL => ServiceProvider.GetRequiredService<IMySqlDbManager>(),
                FactoryEnumType.MariaDb => ServiceProvider.GetRequiredService<IMariaDbManager>(),
                FactoryEnumType.Oracle => ServiceProvider.GetRequiredService<IOracleDbManager>(),
                FactoryEnumType.Iris => ServiceProvider.GetRequiredService<IIrisDbManager>(),
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }

        public DbParameter GetParameter(FactoryEnumType provider, string parameterName, object value)
        {
            return provider switch
            {
                FactoryEnumType.SqlServer => new SqlParameter(parameterName, value),
                FactoryEnumType.Sqlite => new SqliteParameter(parameterName, value),
                FactoryEnumType.MySQL => new MySqlParameter(parameterName, value),
                FactoryEnumType.MariaDb => new MySqlParameter(parameterName, value),
                FactoryEnumType.Oracle => new OracleParameter(parameterName, value),
                FactoryEnumType.Iris => new IRISParameter(parameterName, value),
                _ => throw new ArgumentOutOfRangeException(nameof(provider), provider, null)
            };
        }

        public string GetSqlProvider(FactoryEnumType provider, string column)
        {
            if (provider == FactoryEnumType.Iris)
            {
                return $"SELECT {column} FROM Test.TestDbExtensions WHERE ID = @id";
            }

            if (provider == FactoryEnumType.Oracle)
            {
                return $"SELECT {column} FROM TestDbExtensions WHERE ID = :id";
            }

            return $"SELECT {column} FROM TestDbExtensions WHERE ID = @id";
        }

        public string GetSqlDataProvider(FactoryEnumType provider, string column, object value)
        {
            if (provider == FactoryEnumType.Oracle)
            {
                return $"SELECT '{value}' AS {column} FROM dual";
            }

            return $"SELECT '{value}' AS {column}";
        }

        public string GetSqlAllProvider(FactoryEnumType provider)
        {
            if (provider == FactoryEnumType.Iris)
            {
                return "SELECT * FROM Test.TestDbExtensions WHERE ID < 5 ORDER BY ID ASC";
            }

            return "SELECT * FROM TestDbExtensions WHERE ID < 5 ORDER BY ID ASC";
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (!NeedTableCleanup)
            {
                return;
            }

            foreach (FactoryEnumType provider in Enum.GetValues<FactoryEnumType>())
            {
                var dm = GetProvider(provider);

                var table = provider == FactoryEnumType.Iris ? "Test.TestDbExtensions" : "TestDbExtensions";

                await dm.ExecuteNonQueryAsync($"DELETE FROM {table} WHERE Id > 4");
            }

            NeedTableCleanup = false;
        }
    }
}