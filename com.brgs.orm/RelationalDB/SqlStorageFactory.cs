using System;
using System.Linq;
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
        public T Get<T>(string val){ throw new NotImplementedException("coming soon");}
        public T Get<T>(TableQuery query) {throw new NotImplementedException("coming soon");}
        public T Get<T>()
        {
            var outVal = (T)Activator.CreateInstance(typeof(T));
            var properties = outVal.GetType().GetProperties();
            // var assembly = typeof(T).Assembly;
            // var types = assembly.GetTypes();
            // var methods = from type in assembly.GetTypes()
            //                 where type.IsSealed && !type.IsGenericType
            //                                     && !type.IsNested
            //                 from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            //                 where method.IsDefined(typeof(ExtensionAttribute), false)
            //                 where method.GetParameters()[0].ParameterType == typeof(T)
            //                 select method;
            using (var conn = _connection.CreateConnection())
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        for (var f = 0; f < reader.FieldCount; f++)
                        {
                            var resultName = reader.GetName(f);
                            //This makes the assumption that the column name is 1:1 match with the 
                            var property = properties.FirstOrDefault(p => p.Name.ToLower().Contains(resultName.ToLower()));
                            var resultValue = reader.GetValue(f);
                            if (property != null)
                            {
                                outVal.GetType().GetProperty(property.Name)
                                        .SetValue(outVal, resultValue.ToString(), null);
                            }
                        }
                    }
                }
            }
            return outVal;
        }
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
