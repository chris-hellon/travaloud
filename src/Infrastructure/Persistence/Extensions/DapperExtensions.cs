using System.Data;
using Dapper;

namespace Travaloud.Infrastructure.Persistence.Extensions;

public static class DapperExtensions
{
    public static async Task<List<object>> GetMultipleAsync(this IDbConnection connection, string query, object? request = null, CommandType commandType = CommandType.StoredProcedure, params Func<SqlMapper.GridReader, object>[] readerFuncs)
    {
        var returnResults = new List<object>();

        try
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Close();
                connection.Open();
            }

            if (request != null)
            {
                var gridReader = await connection.QueryMultipleAsync(query, request, commandType: commandType);

                foreach (var readerFunc in readerFuncs)
                {
                    var obj = readerFunc(gridReader);
                    returnResults.Add(obj);
                }
            }
            else
            {
                var gridReader = await connection.QueryMultipleAsync(query, commandType: commandType);

                foreach (var readerFunc in readerFuncs)
                {
                    var obj = readerFunc(gridReader);
                    returnResults.Add(obj);
                }
            }

            return returnResults;
        }
        catch (Exception)
        {
            connection.Close();
            throw;
        }
        finally
        {
            connection.Close();
        }
    }
}