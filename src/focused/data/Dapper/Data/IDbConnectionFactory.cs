using System.Data;

namespace DapperDemo.Data;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}
