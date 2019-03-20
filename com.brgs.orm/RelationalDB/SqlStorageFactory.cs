using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.WindowsAzure.Storage.Table;
namespace com.brgs.orm.RelationalDB
{
    public class SQLStorageFactory : IStorageFactory
    {
        private readonly IDbFactory _connection;
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }
        public SQLStorageFactory(IDbFactory factory)
        {
            _connection = factory;
        }
        public T Get<T>(string val)
        { 
            var outVal = (T)Activator.CreateInstance(typeof(T));
            var properties = outVal.GetType().GetProperties();

            using (var conn = _connection.CreateConnection())
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = val;
                    var reader = command.ExecuteReader();
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

        public T Get<T>(TableQuery query) {throw new NotImplementedException("coming soon");}

        public T Post<T>(T record) 
        {
            //todo: handle converting record to sql, parameterizing
             return record;
        }
        public T Put<T>()
        {
            throw new NotImplementedException("coming soon");
        }
        public int Delete<T>()
        {
            throw new NotImplementedException("coming soon");
        }
    }
}
