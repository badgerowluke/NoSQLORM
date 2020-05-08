using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using com.brgs.orm.RelationalDB;

namespace com.brgs.orm.test
{
    public class SqlServerTests
    {
        [Fact]
        public void WeDoGetASqlFactory()
        {
            var test = new SqlStorageFactory(null);
            Assert.NotNull(test);

        }
    }
}