using Microsoft.Data.SqlClient;
using System.Data;

namespace GMS.Services.DBContext
{
    public class DapperDBContext : IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        public DapperDBContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("GMSConnectionDB");
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
        public IDbTransaction Transaction => _transaction;

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = new SqlConnection(_connectionString);

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                return _connection;
            }
        }
        public void BeginTransaction()
        {
            if (_transaction == null)
                _transaction = Connection.BeginTransaction();
        }
        public void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }
        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }
        public void Dispose()
        {
            _transaction?.Dispose();

            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                    _connection.Close();

                _connection.Dispose();
            }
        }
    }
}
