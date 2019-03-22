using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using com.brgs.orm.RelationalDB;
using System.Data;

namespace com.brgs.orm.test
{
    public class SqlFactoryGetTests
    {
        [Fact]
        public void WeDoGetASqlFactory()
        {
            var test = new SQLStorage(null);
            Assert.NotNull(test);

        }
        [Fact]
        public void SqlFactory_DoesGet()
        {
            var facMock = new Mock<IDbFactory>();
            facMock.Setup(f => f.Query<List<River>>(It.IsAny<string>())).Returns(new List<River>());
            var tester = new SQLStorage(facMock.Object);
            var stuff = tester.Get<List<River>>("select * from Rivers");
            Assert.Empty(stuff);
        }
    }
}