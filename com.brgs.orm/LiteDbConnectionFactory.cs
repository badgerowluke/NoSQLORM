using System.Data;
using System.Data.SQLite;


namespace com.brgs.orm
{
    ///<summary>SQLite Connection Factory</summary>
    public class LiteDBConnection: IDbFactory
    {
        private readonly string connectionString;
        public LiteDBConnection(string connection)
        {
            connectionString = connection;
        }
        public IDbConnection CreateConnection()
        {
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

    }
}