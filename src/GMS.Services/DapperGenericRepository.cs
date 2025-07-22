using Dapper.Contrib.Extensions;
using Dapper;
using GMS.Core.Repository;
using GMS.Services.DBContext;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Z.Dapper.Plus;

namespace GMS.Services;

public class DapperGenericRepository<T> : IDapperRepository<T> where T : class, new()
{
    protected IDbConnection DbConnection { get; private set; }
    private readonly DapperDBContext _dapperDBContext;
    //private readonly SqlConnection _connection;
    //private readonly string _connectionString;
    public DapperGenericRepository(DapperDBContext dapperDBContext)
    {
        ////string strConnection = "Data Source=182.18.138.110;Initial Catalog=MedicalReports_BK;User ID=sa;Password=CG$sBK9%!8P4c$;Encrypt=False;";
        _dapperDBContext = dapperDBContext;
        //DbConnection = dapperDBContext.CreateConnection();
        ////_connection = new SqlConnection(strConnection);
        //_connectionString = DbConnection.ConnectionString;
    }

    public object UpdateFields<TS>(T param)
    {
        var names = new List<string>();
        string tableID = string.Empty;
        object id = null;

        // Get the table key (primary key column name)
        T t = new();
        tableID = GetKeyOfEntity(t);

        // Fallback to checking properties if key not found (edge case)
        if (string.IsNullOrWhiteSpace(tableID))
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(param))
            {
                if (!property.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                    names.Add(property.Name);
                else
                    id = property.GetValue(param);
            }

            if (id != null && names.Count > 0)
            {
                string tableName = typeof(T).Name;
                string setClause = string.Join(",", names.Select(name => $"{name}=@{name}"));
                string sql = $"UPDATE {tableName} SET {setClause} WHERE Id=@Id";

                var affectedRows = _dapperDBContext.Connection.Execute(sql, param, _dapperDBContext.Transaction);
                return affectedRows > 0 ? id : null;
            }
        }

        return null;
    }


    public string GetKeyOfEntity(T entity)
    {
        string tableKey = string.Empty;
        PropertyInfo[] properties = typeof(T).GetProperties();
        //Find the tableID
        foreach (PropertyInfo propertyInfo in properties)
        {
            bool isIdentity = propertyInfo.GetCustomAttributes(inherit: true).Any((object a) => a is KeyAttribute);
            if (isIdentity)
            {
                tableKey = propertyInfo.Name;
                break;
            }
        }
        return tableKey;
    }

    public async Task<int> ExecuteAddAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null)
    {
        try
        {
            var inserted = (await _dapperDBContext.Connection.InsertAsync<T>(entity, transaction, timeOut));
            return inserted;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<bool> ExecuteUpdateAsync(T entity, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null)
    {
        try
        {
            return await _dapperDBContext.Connection.UpdateAsync<T>(entity, transaction, timeOut);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> ExecuteDeleteAsync(int id, IDbConnection dbConnection, IDbTransaction transaction = null, int? timeOut = null)
    {
        try
        {
            var entity = await _dapperDBContext.Connection.GetAsync<T>(id);
            if (entity == null)
                return false;

            return await _dapperDBContext.Connection.DeleteAsync<T>(entity, transaction, timeOut);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<DapperPlusActionSet<T>> AddRangeAsync(T entity)
    {
        try
        {
            var inserted = (_dapperDBContext.Connection.BulkInsert<T>(entity));
            return inserted;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally { }
    }
    public async Task<bool> AddAsync(List<T> entities)
    {
        try
        {
            foreach (var entity in entities)
            {
                await _dapperDBContext.Connection.InsertAsync(entity, _dapperDBContext.Transaction);
            }

            return true;
        }
        catch (Exception)
        {
            // Optional: log the error if needed
            throw; // rethrow so UnitOfWork can handle rollback
        }
    }

    public async Task<int> AddAsync(T entity)
    {
        try
        {
            var inserted = (await _dapperDBContext.Connection.InsertAsync<T>(entity, _dapperDBContext.Transaction));

            return inserted;
        }
        catch (Exception ex)
        {
            return 0;
        }
        finally { }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await _dapperDBContext.Connection.GetAsync<T>(id);
            if (entity == null)
                return false;

            return await _dapperDBContext.Connection.DeleteAsync<T>(entity, _dapperDBContext.Transaction);
        }
        finally { }
    }

    public async Task<List<T>> FindAllAsync()
    {
        try
        {
            var results = await _dapperDBContext.Connection.GetAllAsync<T>(_dapperDBContext.Transaction);

            return results.ToList();
        }
        finally { }
    }

    public async Task<T> FindByIdAsync(int id)
    {
        try
        {
            return await _dapperDBContext.Connection.GetAsync<T>(id, _dapperDBContext.Transaction);
        }
        finally { }
    }

    public async Task<bool> UpdateAsync(T entity)
    {
        try
        {
            return await _dapperDBContext.Connection.UpdateAsync<T>(entity, _dapperDBContext.Transaction);
        }
        catch (Exception ex)
        {
            return false;
        }
        finally { }
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> filter)
    {
        try
        {
            var data = await _dapperDBContext.Connection.GetAllAsync<T>(_dapperDBContext.Transaction);
            var results = data.AsQueryable().SingleOrDefault(filter);
            if (results != null)
                return true;
            return false;
        }

        finally { }
    }
    public async Task<T> GetFilter(Expression<Func<T, bool>> filter)
    {
        try
        {
            var data = await _dapperDBContext.Connection.GetAllAsync<T>(_dapperDBContext.Transaction);
            var results = data.AsQueryable().SingleOrDefault(filter);
            return results;
        }

        finally { }
    }
    public async Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter, Expression<Func<T, bool>> orderBy)
    {
        try
        {
            var data = await _dapperDBContext.Connection.GetAllAsync<T>(_dapperDBContext.Transaction);
            var results = data.AsQueryable().Where(filter).OrderBy(orderBy).ToList();
            return results;
        }
        finally { }
    }
    public async Task<List<T>> GetFilterAll(Expression<Func<T, bool>> filter)
    {
        try
        {

            var data = await _dapperDBContext.Connection.GetAllAsync<T>(_dapperDBContext.Transaction);
            var results = data.AsQueryable().Where(filter).ToList();
            return results;
        }
        catch (Exception ex)
        {
            var x = 0;
            //var data = await _dapperDBContext.Connection.GetAllAsync<T>();
            //var results = data.AsQueryable().Where(filter).ToList();
            return null;

        }
        finally { }
    }

    public async Task<List<T>> GetDynamicQuery(string query, object dynamicParameters)
    {
        List<T> result = new List<T>();
        try
        {
            var results = (await _dapperDBContext.Connection.QueryAsync<T>(query, dynamicParameters, transaction: _dapperDBContext.Transaction, commandType: CommandType.Text)).ToList();
            return results;
        }
        catch (Exception ex)
        {
            return result;
        }
        finally { }
    }

    public async Task<bool> IsExists(string query, object dynamicParameters)
    {
        try
        {
            List<T> results = (await _dapperDBContext.Connection.QueryAsync<T>(query, dynamicParameters, transaction: _dapperDBContext.Transaction, commandType: CommandType.Text)).ToList();
            return (results.Count == 0 ? false : true);
        }
        catch (Exception ex)
        {
            return false;
        }
        finally { }
    }


    public async Task<bool> ExecuteQuery(string query, object dynamicParameters)
    {
        try
        {
            await _dapperDBContext.Connection.QueryAsync<T>(query, dynamicParameters, transaction: _dapperDBContext.Transaction, commandType: CommandType.Text);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
        finally { }
    }

    public async Task<List<TS>> ExecuteQuery<TS>(string query, object dynamicParameters)
    {

        List<TS> t = new();
        try
        {
            t = (await _dapperDBContext.Connection.QueryAsync<TS>(query, dynamicParameters, transaction: _dapperDBContext.Transaction, commandType: CommandType.Text)).ToList();
            return t;
        }
        catch (Exception ex)
        {
            return t;
        }
        finally { }
    }

    public async Task<int> GetStoredProcedure(string storedProcedure, DynamicParameters dynamicParameters)
    {

        try
        {
            var results = await _dapperDBContext.Connection.ExecuteAsync(storedProcedure, dynamicParameters, transaction: _dapperDBContext.Transaction, commandType: CommandType.StoredProcedure);
            return results;
        }
        catch (Exception ex)
        {
            return 0;
        }
        finally { }
    }
    public async Task<List<T>> GetQueryAll(string query)
    {

        try
        {
            var data = await _dapperDBContext.Connection.QueryAsync<T>(query, _dapperDBContext.Transaction);
            return data.ToList();
        }
        finally { }
    }

    public async Task<List<T>> GetQueryAll<T>(string query, IDbConnection IDBConn, IDbTransaction trans)
    {
        //if (connection.State != ConnectionState.Open)
        //    connection.Open();
        //else
        if (IDBConn.State != ConnectionState.Open)
            IDBConn.Open();
        try
        {
            var data = await IDBConn.QueryAsync<T>(query, null, trans);
            return data.ToList();
        }
        catch (Exception ex)
        {
            return null;
        }
        finally { IDBConn.Close(); }
    }
    public async Task<List<T>> GetTableData<T>(string sQuery, object parameters = null)
    {

        var tableName = typeof(T).Name;

        var query = sQuery;


        try
        {
            var data = await _dapperDBContext.Connection.QueryAsync<T>(query, parameters, _dapperDBContext.Transaction);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { }
    }


    public async Task<List<T>> GetTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "", string sOrderBy = "")
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        string sQryOrderBy = (sOrderBy != "" ? " ORDER BY " + sOrderBy : "");
        var query = $"SELECT * FROM {tableName} {sQryWhere} {sQryOrderBy}";

        try
        {
            var data = await connection.QueryAsync<T>(query, null, trans);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { }
    }

    public async Task<List<T>> GetTableData<T>(string sQuery, IDbConnection connection, IDbTransaction trans = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        var query = sQuery;
        if (_dapperDBContext.Connection.State != ConnectionState.Open)
            _dapperDBContext.Connection.Open();
        try
        {
            var data = await _dapperDBContext.Connection.QueryAsync<T>(query, null, trans);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
    }

    public async Task<List<T>> GetTableData<T>(string sQuery, object dbParam, IDbConnection connection, IDbTransaction trans = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        var query = sQuery;
        if (connection.State != ConnectionState.Open)
            connection.Open();
        try
        {
            var data = await connection.QueryAsync<T>(query, dbParam, trans);
            return data.ToList();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<List<T>> GetTableData<T>(string sQuery)
    {
        var tableName = typeof(T).Name;

        var query = sQuery;
        if (_dapperDBContext.Connection.State != ConnectionState.Open)
            _dapperDBContext.Connection.Open();

        try
        {
            var data = await _dapperDBContext.Connection.QueryAsync<T>(query, _dapperDBContext.Transaction);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { _dapperDBContext.Connection.Close(); }


    }
    //public async Task<List<T>> GetTableData<T>(string sQuery)
    //{
    //    //var DbConnection = trans?.Connection ?? connection;
    //    var tableName = typeof(T).Name;

    //    var query = sQuery;
    //    if (_dapperDBContext.Connection.State != ConnectionState.Open)
    //        _dapperDBContext.Connection.Open();

    //    try
    //    {
    //        var data = await _dapperDBContext.Connection.QueryAsync<T>(query);
    //        return data.ToList();
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally { _dapperDBContext.Connection.Close(); }
    //}
    public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null)
    {
        if (_dapperDBContext.Connection.State != ConnectionState.Open)
            _dapperDBContext.Connection.Open();

        try
        {
            var result = await _dapperDBContext.Connection.QueryAsync<T>(
                procedureName,
                parameters,
                transaction: _dapperDBContext.Transaction,
                commandType: CommandType.StoredProcedure
            );
            return result; // Returns an IEnumerable<TestTable>
        }
        catch (Exception ex)
        {
            // Log or handle the exception as needed
            throw new Exception($"Error executing stored procedure '{procedureName}'", ex);
        }
        finally
        {
            if (_dapperDBContext.Connection.State == ConnectionState.Open)
                _dapperDBContext.Connection.Close();
        }
    }

    public async Task<int> ExecuteStoredProcedureAsync(string procedureName, object parameters = null)
    {
        if (_dapperDBContext.Connection.State != ConnectionState.Open)
            _dapperDBContext.Connection.Open();
        try
        {
            var result = await _dapperDBContext.Connection.ExecuteAsync(
                procedureName,
                parameters,
                transaction: _dapperDBContext.Transaction,
                commandType: CommandType.StoredProcedure
            );
            return result; // Returns the number of affected rows
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing stored procedure '{procedureName}'", ex);
        }
        finally
        {
            if (_dapperDBContext.Connection.State == ConnectionState.Open)
                _dapperDBContext.Connection.Close();
        }
    }
    public async Task<int> GetEntityCount(string sQuery, object parameters = null)
    {

        try
        {
            var count = await _dapperDBContext.Connection.ExecuteScalarAsync<int>(sQuery, parameters, _dapperDBContext.Transaction);
            return count;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
        }
    }
    public async Task<T> GetEntityData<T>(string sQuery, object parameters = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;



        try
        {
            var data = await _dapperDBContext.Connection.QueryFirstOrDefaultAsync<T>(sQuery, parameters, _dapperDBContext.Transaction);
            return data;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }
    public async Task<List<T>> GetTableDataExec<T>(string sQuery, object dbParam = null)
    {
        var tableName = typeof(T).Name;

        var query = sQuery;
        if (_dapperDBContext.Connection.State != ConnectionState.Open)
            _dapperDBContext.Connection.Open();

        try
        {
            var data = await _dapperDBContext.Connection.QueryAsync<T>(query, dbParam, _dapperDBContext.Transaction);
            return data.ToList();
        }
        catch (Exception ex) { throw ex; }
        finally { _dapperDBContext.Connection.Close(); }

        ////var DbConnection = trans?.Connection ?? connection;
        //var tableName = typeof(T).Name;
        //var query = sQuery;
        //if (_dapperDBContext.Connection.State != ConnectionState.Open)
        //    _dapperDBContext.Connection.Open();
        //try
        //{
        //    var data = await _dapperDBContext.Connection.QueryAsync<T>(query, null, null);
        //    return data.ToList();
        //}
        //catch (Exception ex) { throw ex; }
        //finally { _dapperDBContext.Connection.Close(); }
    }

    public async Task<bool> DeleteTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "")
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        var query = $"DELETE FROM {tableName} {sQryWhere}";
        if (_dapperDBContext.Connection.State != ConnectionState.Open)
            _dapperDBContext.Connection.Open();

        try
        {
            await _dapperDBContext.Connection.QueryAsync<T>(query, null, trans);
            return true;
        }
        catch (Exception ex) { return false; }
    }

    public async Task<bool> DeleteTableData<T>(IDbConnection connection, IDbTransaction trans = null, string sWhere = "", object paramsObject = null)
    {
        //var DbConnection = trans?.Connection ?? connection;
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        var query = $"DELETE FROM {tableName} {sQryWhere}";
        if (connection.State != ConnectionState.Open)
            connection.Open();

        try
        {
            await connection.QueryAsync<T>(query, paramsObject, trans);
            return true;
        }
        catch (Exception ex) { return false; }
    }

    public async Task<bool> RunSQLCommand(string sQuery)
    {

        try
        {
            await _dapperDBContext.Connection.QueryAsync(sQuery, _dapperDBContext.Transaction);
            return true;
            //return true;
        }
        catch (Exception ex) { return false; }
        finally { }
    }

    public async Task<bool> RunSQLCommand(string sQuery, object parameters = null)
    {

        try
        {
            await _dapperDBContext.Connection.ExecuteAsync(sQuery, parameters, _dapperDBContext.Transaction);
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception here if needed
            Console.WriteLine(ex.Message);
            return false;
        }
        finally
        {
        }
    }

    public async Task<List<T>> GetAllPagedAsync(int limit, int offset, string sWhere = "", string sOrderBy = "")
    {
        var tableName = typeof(T).Name;
        string sQryWhere = (sWhere != "" ? " Where " + sWhere : "");
        string sQryOrderBy = (sOrderBy != "" ? " ORDER BY " + sOrderBy : "");
        var query = $"SELECT * FROM {tableName} {sQryWhere} {sQryOrderBy} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
        _dapperDBContext.Connection.Open();

        try
        {
            var data = await _dapperDBContext.Connection.QueryAsync<T>(query, _dapperDBContext.Transaction);
            return data.ToList();
        }
        finally { _dapperDBContext.Connection.Close(); }
    }

    //our function can accept an array of property or field expressions for the output object, which you can then map to the DynamicParameters object.

    private DynamicParameters GetDynamicParameters<TInput, TOutput>(
        TInput inputObject,
        TOutput outputObject,
        params Expression<Func<TOutput, object>>[] outputExpressions
        )
    {
        var dp = new DynamicParameters(inputObject);
        foreach (var expr in outputExpressions)
            dp.Output(outputObject, expr);

        return dp;
    }

    public void CallProcedure<TInput, TOutput>(
        string storedProcedure,
        TInput inputObject,
        TOutput outputObject,
        string connectionId,
        params Expression<Func<TOutput, object>>[] outputExpressions
        )
    {
        using var connection = _dapperDBContext.Connection;

        var dynamicParameters = GetDynamicParameters<TInput, TOutput>(inputObject, outputObject, outputExpressions);

        connection.Execute(storedProcedure, dynamicParameters, transaction: _dapperDBContext.Transaction,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<T>> GetSPData<T>(IDbConnection connection, IDbTransaction trans = null, string spName = "", DynamicParameters spInput = null)
    {
        var data = await SqlMapper.QueryAsync<T>(connection, spName, spInput, transaction: trans, commandType: CommandType.StoredProcedure);
        return data.ToList();
    }
    public async Task<List<T1>> GetSPData<T1>(string spName = "", DynamicParameters spInput = null)
    {
        var data = await SqlMapper.QueryAsync<T1>(_dapperDBContext.Connection, spName, spInput, commandType: CommandType.StoredProcedure);
        return data.ToList();
    }

    public async Task<List<T1>> GetSPData<T1>(string spName = "", object spInput = null)
    {
        var data = await SqlMapper.QueryAsync<T1>(_dapperDBContext.Connection, spName, spInput, commandType: CommandType.StoredProcedure);
        return data.ToList();
    }

    public async Task<List<T>> GetSPData(string spName = "", DynamicParameters spInput = null)
    {
        try
        {
            var data = await SqlMapper.QueryAsync<T>(_dapperDBContext.Connection, spName, spInput, commandType: CommandType.StoredProcedure);
            return data.ToList();
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task CallProcedureAsync<TInput, TOutput>(
        string storedProcedure,
        TInput inputObject,
        TOutput outputObject,
        string connectionId = "Default",
        params Expression<Func<TOutput, object>>[] outputExpressions
        )
    {
        using var connection = _dapperDBContext.Connection;

        var dynamicParameters = GetDynamicParameters<TInput, TOutput>(inputObject, outputObject, outputExpressions);

        await connection.ExecuteAsync(storedProcedure, dynamicParameters,
            commandType: CommandType.StoredProcedure);
    }



    public async Task<bool> ExecuteListData<T>(List<T> listData, string sQuery, bool isCommitRollback = true)
    {
        IDbConnection IDBConn = _dapperDBContext.Connection;
        string sWhere = string.Empty, sReturnMsg = "Success", status;
        if (string.IsNullOrEmpty(IDBConn.ConnectionString))
            IDBConn.ConnectionString = _dapperDBContext.Connection.ConnectionString;
        if (IDBConn.State == ConnectionState.Closed)
        { IDBConn.Open(); }
        IDbTransaction trans = IDBConn.BeginTransaction();
        try
        {
            using (IDBConn)
            {

                if (listData.Count > 0)
                {
                    IDBConn.Execute(sQuery, listData, trans);
                    isCommitRollback = true;
                }
                if (isCommitRollback)
                    if (listData.Count == 0)
                        trans.Rollback();
                    else
                        trans.Commit();
                return true;

            }

            //ProcessAttendance
        }
        catch (Exception ex)
        {
            if (isCommitRollback)
                trans.Rollback();
            return false;
        }
        finally { IDBConn.Close(); }
    }


}
