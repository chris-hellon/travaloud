using System.Data;
using Dapper;

namespace Travaloud.Application.Common.Persistence;

public interface IDapperRepository : ITransientService
{
    Task ExecuteAsync(string sql, object? param = null,
        IDbTransaction? transaction = null, CommandType? commandType = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get an <see cref="IReadOnlyList{T}"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="sql">The sql string.</param>
    /// <param name="param">The paramters in the sql string.</param>
    /// <param name="transaction">The transaction to be performed.</param>
    /// <param name="commandType"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>Returns <see cref="Task"/> of <see cref="IReadOnlyCollection{T}"/>.</returns>
    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    where T : class;

    /// <summary>
    /// Get a <typeparamref name="T"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="sql">The sql string.</param>
    /// <param name="param">The paramters in the sql string.</param>
    /// <param name="transaction">The transaction to be performed.</param>
    /// <param name="commandType"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T"/>.</returns>
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    where T : class, IEntity;

    /// <summary>
    /// Get a <typeparamref name="T"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="sql">The sql string.</param>
    /// <param name="param">The paramters in the sql string.</param>
    /// <param name="transaction">The transaction to be performed.</param>
    /// <param name="commandType"></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T"/>.</returns>
    Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    where T : class, IEntity;

    /// <summary>
    /// Get a <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/><typeparamref name="T4"/><typeparamref name="T5"/><typeparamref name="T6"/><typeparamref name="T7"/><typeparamref name="T8"/><typeparamref name="T9"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the entity 1.</typeparam>
    /// <typeparam name="T2">The type of the entity 2.</typeparam>
    /// <typeparam name="T3">The type of the entity 3.</typeparam>
    /// <typeparam name="T4">The type of the entity 4.</typeparam>
    /// <typeparam name="T5">The type of the entity 5.</typeparam>
    /// <typeparam name="T6">The type of the entity 6.</typeparam>
    /// <typeparam name="T7">The type of the entity 7.</typeparam>
    /// <typeparam name="T8">The type of the entity 8.</typeparam>
    /// <typeparam name="T9">The type of the entity 9.</typeparam>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <param name="func4"></param>
    /// <param name="func5"></param>
    /// <param name="func6"></param>
    /// <param name="func7"></param>
    /// <param name="func8"></param>
    /// <param name="func9"></param>
    /// <param name="commandType"></param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/><typeparamref name="T4"/><typeparamref name="T5"/><typeparamref name="T6"/><typeparamref name="T7"/><typeparamref name="T8"/><typeparamref name="T9"/>.</returns>
    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>, Tuple<IEnumerable<T8>, IEnumerable<T9>>>> GetMultipleAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4, Func<SqlMapper.GridReader, IEnumerable<T5>> func5, Func<SqlMapper.GridReader, IEnumerable<T6>> func6, Func<SqlMapper.GridReader, IEnumerable<T7>> func7, Func<SqlMapper.GridReader, IEnumerable<T8>> func8, Func<SqlMapper.GridReader, IEnumerable<T9>> func9, CommandType commandType = CommandType.StoredProcedure);

    /// <summary>
    /// Get a <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/><typeparamref name="T4"/><typeparamref name="T5"/><typeparamref name="T6"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the entity 1.</typeparam>
    /// <typeparam name="T2">The type of the entity 2.</typeparam>
    /// <typeparam name="T3">The type of the entity 3.</typeparam>
    /// <typeparam name="T4">The type of the entity 4.</typeparam>
    /// <typeparam name="T5">The type of the entity 5.</typeparam>
    /// <typeparam name="T6">The type of the entity 6.</typeparam>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <param name="func4"></param>
    /// <param name="func5"></param>
    /// <param name="func6"></param>
    /// <param name="commandType"></param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/><typeparamref name="T4"/><typeparamref name="T5"/><typeparamref name="T6"/>.</returns>
    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>> GetMultipleAsync<T1, T2, T3, T4, T5, T6>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4, Func<SqlMapper.GridReader, IEnumerable<T5>> func5, Func<SqlMapper.GridReader, IEnumerable<T6>> func6, CommandType commandType = CommandType.StoredProcedure);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <param name="func4"></param>
    /// <param name="func5"></param>
    /// <param name="func6"></param>
    /// <param name="func7"></param>
    /// <param name="commandType"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <returns></returns>
    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>> GetMultipleAsync<T1, T2, T3, T4, T5, T6, T7>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4, Func<SqlMapper.GridReader, IEnumerable<T5>> func5, Func<SqlMapper.GridReader, IEnumerable<T6>> func6, Func<SqlMapper.GridReader, IEnumerable<T7>> func7, CommandType commandType = CommandType.StoredProcedure);
    
    /// <summary>
    /// Get a <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/><typeparamref name="T4"/> * <typeparamref name="T5"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the entity 1.</typeparam>
    /// <typeparam name="T2">The type of the entity 2.</typeparam>
    /// <typeparam name="T3">The type of the entity 3.</typeparam>
    /// <typeparam name="T4">The type of the entity 4.</typeparam>
    /// <typeparam name="T5">The type of the entity 5.</typeparam>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <param name="func4"></param>
    /// <param name="func5"></param>
    /// <param name="commandType"></param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/><typeparamref name="T4"/> * <typeparamref name="T5"/>.</returns>
    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>>> GetMultipleAsync<T1, T2, T3, T4, T5>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3, Func<SqlMapper.GridReader, IEnumerable<T4>> func4, Func<SqlMapper.GridReader, IEnumerable<T5>> func5, CommandType commandType = CommandType.StoredProcedure);

    /// <summary>
    /// Get a <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the entity 1.</typeparam>
    /// <typeparam name="T2">The type of the entity 2.</typeparam>
    /// <typeparam name="T3">The type of the entity 3.</typeparam>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="func3"></param>
    /// <param name="commandType"></param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T1"/><typeparamref name="T2"/><typeparamref name="T3"/>.</returns>
    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> GetMultipleAsync<T1, T2, T3>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, Func<SqlMapper.GridReader, IEnumerable<T3>> func3, CommandType commandType = CommandType.StoredProcedure);

    /// <summary>
    /// Get a <typeparamref name="T1"/><typeparamref name="T2"/> using raw sql string with parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the entity 1.</typeparam>
    /// <typeparam name="T2">The type of the entity 2.</typeparam>
    /// <param name="query"></param>
    /// <param name="request"></param>
    /// <param name="func1"></param>
    /// <param name="func2"></param>
    /// <param name="commandType"></param>
    /// <returns>Returns <see cref="Task"/> of <typeparamref name="T1"/><typeparamref name="T2"/>.</returns>
    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> GetMultipleAsync<T1, T2>(string query, object request, Func<SqlMapper.GridReader, IEnumerable<T1>> func1, Func<SqlMapper.GridReader, IEnumerable<T2>> func2, CommandType commandType = CommandType.StoredProcedure);
}
   