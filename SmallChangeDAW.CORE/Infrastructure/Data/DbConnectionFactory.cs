using Microsoft.Data.SqlClient;
using System.Data;

namespace SmallChangeDAW.Infrastructure.Data;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
