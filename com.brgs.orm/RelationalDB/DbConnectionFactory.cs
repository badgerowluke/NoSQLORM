using System.Data;
using System.Data.SqlClient;

namespace com.brgs.orm.RelationalDB
{
    public interface IDbFactory
    {
        IDbConnection CreateConnection();
    }
    ///
    ///<summary>MS SqlServer Connection Factory </summary>
    ///
    public class DbConnection : IDbFactory
    {
        private readonly string connectionString;

        public DbConnection(string connection)
        {
            connectionString = connection;
        }
        public IDbConnection CreateConnection()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}