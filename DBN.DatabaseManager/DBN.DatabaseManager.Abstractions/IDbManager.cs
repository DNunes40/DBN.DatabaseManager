using System.Data;
using System.Data.Common;
using System.Linq.Expressions;

namespace DBN.DatabaseManager.Abstractions
{
    /// <summary>
    /// Defines a contract for asynchronous database management operations, including
    /// executing queries, commands, transactions, and mapping results to objects.
    /// </summary>
    public interface IDbManager : IAsyncDisposable
    {
        /// <summary>
        /// Gets the unique identifier for this database connection or manager instance.
        /// </summary>
        /// <remarks>
        /// This identifier can be used to distinguish between multiple database connections
        /// or configurations in a multi-database application.
        /// </remarks>
        string Identifier { get; }

        /// <summary>
        /// Retrieves a registered database manager of type <typeparamref name="T"/> by identifier
        /// from the provided <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The specific DB manager interface, e.g., ISqlServerDbManager.</typeparam>
        /// <param name="identifier">The unique identifier for the database connection.</param>
        /// <returns>An instance of <typeparamref name="T"/> registered with the service provider.</returns>
        /// <exception cref="InvalidOperationException">If no IServiceProvider is available or GetServices method not found.</exception>
        /// <exception cref="KeyNotFoundException">If no manager with the given identifier is registered.</exception>
        public T ResolveDbManager<T>(string identifier) where T : IDbManager;

        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current transaction, making all changes permanent.
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current transaction, undoing all changes made within it.
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// Executes the provided SQL query asynchronously and returns an <see cref="DbDataReader"/> 
        /// to read the results. The caller is responsible for disposing the reader.
        /// </summary>
        /// <param name="sql">The SQL query string to execute.</param>
        /// <param name="parameters">Optional parameters for parameterized queries.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="DbDataReader"/> with the query results.</returns>
        Task<DbDataReader> ExecuteReaderAsync(string sql, params DbParameter[] parameters);

        /// <summary>
        /// Executes a stored procedure asynchronously and returns a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="procedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameters">An optional array of <see cref="DbParameter"/> for the procedure.</param>
        /// <returns>A <see cref="DbDataReader"/> representing the result set of the procedure.</returns>
        /// <exception cref="DatabaseManagerException">
        /// Thrown if execution fails. Includes the SQL command text and parameters for debugging.
        /// </exception>
        Task<DbDataReader> ExecuteStoredProcedureAsync(string procedureName, params DbParameter[] parameters);

        /// <summary>
        /// Executes a scalar database function asynchronously and returns the result as the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The expected return type of the function. Can be any type that can be converted from the database result, including nullable types.
        /// </typeparam>
        /// <param name="functionName">
        /// The name of the database function to execute. For SQL Server, the schema (e.g., "dbo.") should be included if needed.
        /// </param>
        /// <param name="parameters">
        /// An array of <see cref="DbParameter"/> representing the parameters for the database function. 
        /// Parameter names and values must match the function signature in the database. 
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the function return value converted to type <typeparamref name="T"/>.
        /// If the function returns NULL or no rows are found, the result is default(<typeparamref name="T"/>).
        /// </returns>
        /// <remarks>
        /// - Numeric conversions and enum handling are supported automatically.
        /// </remarks>
        Task<T?> ExecuteFunctionAsync<T>(string functionName, params DbParameter[] parameters);

        /// <summary>
        /// Asynchronously executes a LINQ expression against the database and returns the first matching row 
        /// mapped to an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object to return. The result row will be mapped to an instance of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="expr">
        /// A LINQ expression representing the filter conditions for the query.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains the first matching object of type <typeparamref name="T"/>, 
        /// or <c>null</c> if no rows match the expression.
        /// </returns>
        /// <remarks>
        /// This method translates the provided LINQ expression into a SQL WHERE clause,
        /// executes the query asynchronously, and maps the first result row to an instance of <typeparamref name="T"/>.
        /// If multiple rows match, only the first row is returned.
        /// </remarks>
        Task<T?> QuerySingleAsync<T>(Expression<Func<T, bool>> expr);


        /// <summary>
        /// Asynchronously executes a LINQ expression against the database and returns all matching rows 
        /// mapped to a list of objects of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the objects to return. Each result row will be mapped to an instance of <typeparamref name="T"/>.
        /// </typeparam>
        /// <param name="expr">
        /// A LINQ expression representing the filter conditions for the query.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a list of objects of type <typeparamref name="T"/>.
        /// Returns an empty list if no rows match the expression.
        /// </returns>
        /// <remarks>
        /// This method translates the provided LINQ expression into a SQL WHERE clause,
        /// executes the query asynchronously, and maps all matching rows to instances of <typeparamref name="T"/>.
        /// </remarks>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> expr);

        /// <summary>
        /// Executes the provided SQL query asynchronously and fills a <see cref="DataSet"/> 
        /// with the results.
        /// </summary>
        /// <param name="sql">The SQL query string to execute.</param>
        /// <param name="parameters">Optional parameters for parameterized queries.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains a <see cref="DataSet"/> with the query results.</returns>
        Task<DataSet> ExecuteDataSetAsync(string sql, params DbParameter[] parameters);

        /// <summary>
        /// Executes a SQL command asynchronously that does not return rows, such as INSERT, UPDATE, or DELETE.
        /// </summary>
        /// <param name="sql">The SQL command string to execute.</param>
        /// <param name="parameters">Optional parameters for parameterized commands.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the number of rows affected by the command.</returns>
        Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] parameters);

        /// <summary>
        /// Retrieves the next value of a specified sequence asynchronously.
        /// </summary>
        /// <param name="sequenceName">The name of the sequence.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the next value of the sequence as a <see cref="long"/>.</returns>
        Task<long> GetNextSequenceValueAsync(string sequenceName);

        /// <summary>
        /// Executes the provided SQL query asynchronously and returns the first column of the first row 
        /// cast to type <typeparamref name="T"/>. Returns <c>default(T)</c> if the result is null or <c>DBNull</c>.
        /// </summary>
        /// <typeparam name="T">The type to convert the scalar value to.</typeparam>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Optional parameters for the query.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the scalar value converted to <typeparamref name="T"/> or <c>default(T)</c> if null.</returns>
        Task<T?> ExecuteScalarAsync<T>(string sql, params DbParameter[] parameters);

        /// <summary> 
        /// Asynchronously executes the specified SQL query with optional parameters and maps the first result 
        /// (or results, if <typeparamref name="T"/> is a collection type) to an object of type <typeparamref name="T"/>.
        /// </summary>       
        /// <typeparam name="T">The type to map the result to. Can be a single object or a collection type.</typeparam>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="parameters">Optional database parameters to pass to the query.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an instance of 
        /// <typeparamref name="T"/> populated from the first row of the result set, or <c>null</c> if no rows are returned.
        /// For collection types, returns an empty collection if no rows exist.
        /// </returns>
        /// <remarks>
        /// Returns <c>null</c> for reference types if no rows are returned.
        /// For value types or collections, returns a default instance or empty collection.
        /// </remarks>
        Task<T?> MapToAsync<T>(string sql, params DbParameter[] parameters);
    }
}
