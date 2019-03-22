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
    public partial class SQLStorage : IStorageFactory
    {
        private readonly IDbFactory _connection;
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }
        public SQLStorage(IDbFactory factory)
        {
            _connection = factory;
        }


        public T Get<T>(TableQuery query) {throw new NotImplementedException("coming soon");}
        public T Get<T>(string val)
        { 
            var outVal = _connection.Query<T>(val);
            return outVal;
        }           

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
