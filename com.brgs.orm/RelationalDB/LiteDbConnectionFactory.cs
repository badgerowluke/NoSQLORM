using System.Data;
using System.Data.SQLite;


namespace com.brgs.orm.RelationalDB
{
    ///<summary>SQLite Connection Factory</summary>
    public class LiteDBConnection: RdbmsFactoryCreator, IDbFactory
    {
        public LiteDBConnection(string connection)
        {
            connectionString = connection;
        }
        public override IDbConnection CreateConnection()
        {
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

    }
}