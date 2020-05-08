using System;
using System.Collections.Generic;

using System.Linq.Expressions;
using System.Threading.Tasks;

namespace com.brgs.orm.RelationalDB
{
    public class SqlStorageFactory : IStorageFactory
    {
        private readonly IDbFactory _connection;

        public SqlStorageFactory(IDbFactory factory)
        {
            _connection = factory;
        }


        public T Get<T>()
        {
            var outVal = (T)Activator.CreateInstance(typeof(T));

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
                        }
                    }
                }
            }
            return outVal;
        }

        public Task<T> GetAsync<T>(string filename){ throw new NotImplementedException("coming soon");}
        public Task<IEnumerable<T>> GetAsync<T>(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }        
        public Task<int> PostAsync<T>(T record) {throw new NotImplementedException("coming soon");}
        public T Put<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
        public Task DeleteAsync<T>(T record)
        {
            throw new NotImplementedException("coming soon");
        }
    }
}
