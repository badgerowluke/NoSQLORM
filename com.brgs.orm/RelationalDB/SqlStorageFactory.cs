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
                            var record = DecodeData(reader, type);
                            typeof(T).GetMethod("Add").Invoke(outVal, new object[]{record});
                        } else 
                        {
                            outVal = (T)DecodeData(reader, typeof(T));
                        }
                    }
                }
            }
            return outVal;
        }
        private object DecodeData(IDataRecord record, Type type)
        {
            var val = Activator.CreateInstance(type);
            var properties = val.GetType().GetProperties();
            for(var f=0; f < record.FieldCount; f++)
            {
                var name = record.GetName(f);
                var value = record.GetValue(f);
                var property = properties.FirstOrDefault(p => p.Name.ToLower().Contains(name.ToLower()));
                if (property != null)
                {
                    val.GetType().GetProperty(property.Name)
                            .SetValue(val, value, null);
                }
            }
            return val;
        }
        public T Get<T>(TableQuery query) {throw new NotImplementedException("coming soon");}

        public T Post<T>(T record) {throw new NotImplementedException("coming soon");}
        public T Post<T>()
        {
            throw new NotImplementedException("coming soon");
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
