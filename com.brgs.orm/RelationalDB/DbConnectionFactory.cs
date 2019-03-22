using System.Data;
using System.Data.SqlClient;

namespace com.brgs.orm.RelationalDB
{

    ///
    ///<summary>MSSQLServer Connection Factory </summary>
    ///
    public class MSSQLConnection : RdbmsFactoryCreator, IDbFactory
    {


        public MSSQLConnection(string connection)
        {
            connectionString = connection;
        }
        public override IDbConnection CreateConnection()
        {
            //TODO once we solve for how to spin up docker from a yaml build, write up some integration tests.
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}