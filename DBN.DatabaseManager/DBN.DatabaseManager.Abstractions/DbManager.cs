using DBN.DatabaseManager.Abstractions.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;

namespace DBN.DatabaseManager.Abstractions
{
    public abstract class DbManager : IDbManager, IAsyncDisposable
    {
        private readonly string _identifier;
        string IDbManager.Identifier { get => _identifier; }

        public readonly IServiceProvider? _serviceProvider;

        private readonly DbProviderFactory _factory;

        private readonly FactoryEnumType _factoryType;

        private readonly string _connectionString;
        private bool _disposed;

        private DbConnection? _connection;
        private DbTransaction? _transaction;

        public DbManager(string connectionString, DbProviderFactory factory, IServiceProvider? serviceProvider, string? identifier)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            _connectionString = connectionString;

            _factory = factory ?? throw new ArgumentNullException(nameof(factory), "DbProviderFactory cannot be null.");

            _serviceProvider = serviceProvider;

            _identifier = identifier ?? "DEFAULT_CONNECTION";
           
            _factoryType = GetCurrentFactoryType();
          
            if (_factoryType == FactoryEnumType.Sqlite && !_connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
            {
                _connectionString = $"Data Source={_connectionString}";
            }
        }

        public T ResolveDbManager<T>(string identifier) where T : IDbManager
        {
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException(
                    "No IServiceProvider available. Pass an IServiceProvider to the constructor " +
                    "if you want to retrieve a manager by identifier."
                );
            }

            var allManagers = _serviceProvider.GetServices<T>();

            return allManagers.FirstOrDefault(m => m.Identifier == identifier) ?? throw new KeyNotFoundException($"No {typeof(T).Name} registered with identifier '{identifier}'.");
        }

        protected async Task<DbConnection> GetConnection()
        {
            if (_connection == null)
            {
                try
                {
                    _connection = _factory.CreateConnection();

                    if (_connection == null)
                    {
                        throw new DatabaseManagerException($"Failed to create a connection using connection string '{_connectionString}'.");
                    }
                }
                catch (Exception exc)
                {
                    throw new DatabaseManagerException("Failed to create connection.", exc);
                }

                _connection.ConnectionString = _connectionString;
                await _connection.OpenAsync();
            }
            else if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }

            return _connection;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("An async transaction is already active.");
            }

            var connection = await GetConnection();
            _transaction = await connection.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                return;
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }


        public async Task<DbDataReader> ExecuteReaderAsync(string sql, params DbParameter[] parameters)
        {
            try
            {
                var connection = await GetConnection();
                var command = DbCommandHelper.CreateCommand(_factory, connection, sql, _transaction, parameters);
                var behavior = _transaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default;
                var reader = await command.ExecuteReaderAsync(behavior);
                return reader;
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }


        /// <summary>
        /// Processes procedure parameters before execution. 
        /// Can be overridden in derived classes to add provider-specific parameters (e.g., Oracle RefCursor).
        /// </summary>
        /// <param name="parameters">The input parameters for the stored procedure.</param>
        /// <returns>The processed array of <see cref="DbParameter"/>.</returns>
        protected virtual DbParameter[] ProcessProcedureParameters(DbParameter[] parameters)
        {
            return parameters;
        }

        public async Task<DbDataReader> ExecuteStoredProcedureAsync(string procedureName, params DbParameter[] parameters)
        {
            var sql = "";

            try
            {
                if (_factoryType == FactoryEnumType.Oracle)
                {
                    parameters = ProcessProcedureParameters(parameters);
                }
                
                if (_factoryType == FactoryEnumType.Oracle)
                {
                    var paramList = string.Join(", ", parameters.Select(p => p.ParameterName.StartsWith(":") ? p.ParameterName : $":{p.ParameterName}"));
                    sql = $"BEGIN {procedureName}({paramList}); END;";
                }
                else if (_factoryType == FactoryEnumType.SqlServer)
                {
                    var paramList = string.Join(", ", parameters.Select(p => p.ParameterName.StartsWith("@") ? p.ParameterName : $"@{p.ParameterName}"));
                    sql = $"EXEC {procedureName} {paramList};";
                }
                else
                {
                    var paramList = string.Join(", ", parameters.Select(_ => "?"));
                    sql = $"CALL {procedureName}({paramList});";
                }

                var connection = await GetConnection();
                var command = DbCommandHelper.CreateCommand(_factory, connection, sql, _transaction, parameters);
                var behavior = _transaction == null ? CommandBehavior.CloseConnection : CommandBehavior.Default;
                var reader = await command.ExecuteReaderAsync(behavior);
                return reader;
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }


        public async Task<T?> ExecuteFunctionAsync<T>(string functionName, params DbParameter[] parameters)
        {
            var sql = "";

            try
            {
                if (_factoryType == FactoryEnumType.Oracle)
                {
                    var paramList = string.Join(", ", parameters.Select(p => p.ParameterName.StartsWith(":") ? p.ParameterName : $":{p.ParameterName}"));

                    sql = $"SELECT {functionName}({paramList}) FROM DUAL";
                }
                else
                {
                    var paramList = string.Join(", ", parameters.Select(p => p.ParameterName.StartsWith("@") ? p.ParameterName : $"@{p.ParameterName}"));

                    sql = $"SELECT {functionName}({paramList})";
                }

                return await ExecuteScalarAsync<T>(sql, parameters);
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }


        public async Task<T?> QuerySingleAsync<T>(Expression<Func<T, bool>> expr)
        {
            var sql = "";

            try
            {
                sql = SqlBuilderExpression.ExecuteWhere(expr);
                var reader = await ExecuteReaderAsync(sql);
                var result = FastMapper.MapReader(reader, typeof(T));
                await reader.DisposeAsync();
                return (T)(result ?? Activator.CreateInstance(typeof(T))!);
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql);
            }
        }

        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> expr)
        {
            var sql = "";

            try
            {
                sql = SqlBuilderExpression.ExecuteWhere(expr);
                var reader = await ExecuteReaderAsync(sql);
                var result = FastMapper.MapReader(reader, typeof(List<T>));
                await reader.DisposeAsync();
                return (List<T>)(result ?? new List<T>());
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql);
            }
        }



        public async Task<DataSet> ExecuteDataSetAsync(string sql, params DbParameter[] parameters)
        {
            try
            {
                if (_factory.GetType().Name.Contains("Sqlite"))
                {
                    return await ExecuteDataSetSqLiteAsync(sql, parameters);
                }

                var reader = await ExecuteReaderAsync(sql, parameters);
                return await LoadDataSetAsync(reader);
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }

        private async Task<DataSet> ExecuteDataSetSqLiteAsync(string sql, params DbParameter[] parameters)
        {
            try
            {
                var reader = await ExecuteReaderAsync(sql, parameters);
                return await LoadDataSetAsync(reader);
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }



        public async Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] parameters)
        {
            try
            {
                var connection = await GetConnection();
                var command = DbCommandHelper.CreateCommand(_factory, connection, sql, _transaction, parameters);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }



        public async Task<long> GetNextSequenceValueAsync(string sequenceName)
        {
            var sql = $"SELECT NEXT VALUE FOR {sequenceName}";

            if (_factoryType == FactoryEnumType.Oracle)
            {
                sql = $"SELECT {sequenceName}.NEXTVAL FROM DUAL";
            }

            try
            {
                return await ExecuteScalarAsync<long>(sql);
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, null);
            }
        }



        public async Task<T?> ExecuteScalarAsync<T>(string sql, params DbParameter[] parameters)
        {
            try
            {
                var connection = await GetConnection();
                var command = DbCommandHelper.CreateCommand(_factory, connection, sql, _transaction, parameters);

                var result = await command.ExecuteScalarAsync();

                if (result is DBNull || result == null)
                {
                    return default;
                }

                try
                {
                    // Special handling for numeric types
                    var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                    if (targetType.IsEnum)
                    {
                        return (T)Enum.ToObject(targetType, result);
                    }

                    if (targetType == typeof(int))
                    {
                        return (T)(object)Convert.ToInt32(result, CultureInfo.InvariantCulture);
                    }

                    if (targetType == typeof(long))
                    {
                        return (T)(object)Convert.ToInt64(result, CultureInfo.InvariantCulture);
                    }

                    if (targetType == typeof(decimal))
                    {
                        return (T)(object)Convert.ToDecimal(result, CultureInfo.InvariantCulture);
                    }

                    if (targetType == typeof(double))
                    {
                        return (T)(object)Convert.ToDouble(result, CultureInfo.InvariantCulture);
                    }

                    if (targetType == typeof(float))
                    {
                        return (T)(object)Convert.ToSingle(result, CultureInfo.InvariantCulture);
                    }

                    return (T?)Convert.ChangeType(result, targetType, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    return default;
                }
            }
            catch (Exception exc)
            {
                throw new DatabaseManagerException(exc, sql, parameters);
            }
        }



        public async Task<T?> MapToAsync<T>(string sql, params DbParameter[] parameters)
        {
            try
            {
                var reader = await ExecuteReaderAsync(sql, parameters);

                return await reader.MapTo<T>();
            }
            catch (DatabaseManagerException exc)
            {
                throw new DatabaseManagerException(exc.Message, exc, sql, parameters);
            }
        }


        private static async Task<DataSet> LoadDataSetAsync(DbDataReader reader)
        {
            var table = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            while (await reader.ReadAsync())
            {
                var row = table.NewRow();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                }

                table.Rows.Add(row);
            }

            await reader.DisposeAsync();

            var dataSet = new DataSet();
            dataSet.Tables.Add(table);
            return dataSet;
        }


        private FactoryEnumType GetCurrentFactoryType()
        {
            var name = _factory.GetType().Name;

            return name switch
            {
                "OracleClientFactory" => FactoryEnumType.Oracle,
                "SqlClientFactory" => FactoryEnumType.SqlServer,
                "IRISClientFactory" => FactoryEnumType.Iris,
                "SqliteFactory" => FactoryEnumType.Sqlite,
                "MySqlClientFactory" => FactoryEnumType.MySQL,
                "MariaDbClientFactory" => FactoryEnumType.MariaDb,
                _ => throw new InvalidOperationException($"Unknown factory type: {name}")
            };
        }


        // Dispose pattern
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            if (_transaction != null)
            {
                try { await _transaction.RollbackAsync(); } catch { }
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
