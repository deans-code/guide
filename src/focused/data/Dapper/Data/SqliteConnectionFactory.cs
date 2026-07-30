using System.Data;
using Microsoft.Data.Sqlite;

namespace DapperDemo.Data;

public class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection Create()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
