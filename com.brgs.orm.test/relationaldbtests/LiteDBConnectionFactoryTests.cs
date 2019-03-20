using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using com.brgs.orm.RelationalDB;
using System.Data.SQLite;

namespace com.brgs.orm.test
{
    public class SqliteDBTests
    {
        [Fact]
        public void WeDoGetAFactory()
        {
            var tester = new LiteDBConnection("Data Source=test.db");
            Assert.NotNull(tester);
        }
        [Fact]
        public void LiteDBConnection_IsIDbFactory()
        {
            var tester = new LiteDBConnection("Data Source=test.db");
            var fac = new SQLStorageFactory(tester);  
            Assert.NotNull(fac);   
        }
        [Fact]
        public void LiteDBConnection_CreateConnection_DoesReturnConnection(){
            var tester = new LiteDBConnection("Data Source=test.db");
            var conn = tester.CreateConnection();
            Assert.NotNull(conn);
        }
    }
}