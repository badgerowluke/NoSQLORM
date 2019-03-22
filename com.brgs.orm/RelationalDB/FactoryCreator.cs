using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace com.brgs.orm.RelationalDB
{
    public interface IDbFactory
    {
        IDbConnection CreateConnection();
        T Query<T>(string val);
    }
    public abstract class RdbmsFactoryCreator: IDbFactory
    {
        protected virtual string connectionString { get; set; }
        public abstract IDbConnection CreateConnection();
        public virtual T Query<T>(string val)
        {
            var outVal = (T)Activator.CreateInstance(typeof(T));
            var properties = outVal.GetType().GetProperties();
            using(var conn = CreateConnection())
            {
                using (var command = conn.CreateCommand())
                {
                                      
                    command.CommandText = val;
                    var reader = command.ExecuteReader();
                    //TODO
                    // var reader = command.ExecuteNonQuery();
                    // var reader = command.ExecuteScalar();
  
                    //TODO sql parameterization
                    var enumerable = typeof(T).GetTypeInfo().ImplementedInterfaces
                                                            .Contains(typeof(System.Collections.IEnumerable));
                    
                    while (reader.Read()) 
                    {
                        if(enumerable)
                        {
                            var type = outVal.GetType().GetGenericArguments()[0];
                            var record = ObjectDecoder.DecodeData(reader, type);
                            typeof(T).GetMethod("Add").Invoke(outVal, new object[]{record});
                        } else 
                        {
                            outVal = (T)ObjectDecoder.DecodeData(reader, typeof(T));
                        }
                    }


                }
            }
            return outVal;
        }

    }    
}