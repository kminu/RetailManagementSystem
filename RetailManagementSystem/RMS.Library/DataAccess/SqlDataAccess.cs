using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace RMS.Library.DataAccess;

public class SqlDataAccess : IDisposable, IDataAccess
{
    private readonly IConfiguration _config;

    public SqlDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName)
    {
        var connectionString = _config.GetConnectionString(connectionStringName);

        using IDbConnection connection = new SqlConnection(connectionString);
        var rows = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<int> SaveData<T>(string storedProcedure, T parameters, string connectionStringName)
    {
        var connectionString = _config.GetConnectionString(connectionStringName);

        using IDbConnection connection = new SqlConnection(connectionString);
        return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    }

    // The code below is used when transaction is required

    private IDbConnection _connection;
    private IDbTransaction _transaction;
    private bool isClosed = false;

    public void StartTransaction(string connectionStringName)
    {
        var connectionString = _config.GetConnectionString(connectionStringName);

        _connection = new SqlConnection(connectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();

        isClosed = false;
    }

    public async Task<List<T>> LoadDataInTransaction<T, U>(string storedProcedure, U parameters)
    {
        var rows = await _connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure,
                                                    transaction: _transaction);

        return rows.ToList();
    }

    public async Task<int> SaveDataInTransaction<T>(string storedProcedure, T parameters)
    {
        return await _connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure,
                                                transaction: _transaction);
    }

    public void CommitTransaction()
    {
        _transaction?.Commit();
        _connection?.Close();

        isClosed = true;
    }

    public void RollbackTransaction()
    {
        _transaction?.Rollback();
        _connection.Close();

        isClosed = true;
    }

    public void Dispose()
    {
        if (isClosed == false)
        {
            try
            {
                CommitTransaction();
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Commit transaction failed in the dispose method.");
            }
        }

        _transaction = null;
        _connection = null;
    }
}