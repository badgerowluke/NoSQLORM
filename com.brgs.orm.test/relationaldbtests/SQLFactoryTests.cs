using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using com.brgs.orm.RelationalDB;
using System.Data;

namespace com.brgs.orm.test
{
    public class SqlFactoryTests
    {
        [Fact]
        public void WeDoGetASqlFactory()
        {
            var test = new SQLStorageFactory(null);
            Assert.NotNull(test);

        }
        [Fact]
        public void SqlFactory_DoesGet()
        {
            var facMock = new Mock<IDbFactory>();
            var connMock = new Mock<IDbConnection>();
            var commandMock = new Mock<IDbCommand>();
            var readerMock = new Mock<IDataReader>();
            commandMock.Setup(cmd => cmd.ExecuteReader()).Returns(readerMock.Object);
            connMock.Setup(c =>c.CreateCommand()).Returns(commandMock.Object);
            facMock.Setup(f => f.CreateConnection()).Returns(connMock.Object);
            var tester = new SQLStorageFactory(facMock.Object);
            var stuff = tester.Get<List<River>>("select * from Rivers");
            Assert.Empty(stuff);
        }
        [Fact]
        public void SqlFactory_DoesGet_DoesRead()
        {
            var facMock = new Mock<IDbFactory>();
            var connMock = new Mock<IDbConnection>();
            var commandMock = new Mock<IDbCommand>();
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            commandMock.Setup(cmd => cmd.ExecuteReader()).Returns(readerMock.Object);
            connMock.Setup(c =>c.CreateCommand()).Returns(commandMock.Object);
            facMock.Setup(f => f.CreateConnection()).Returns(connMock.Object);
            var tester = new SQLStorageFactory(facMock.Object);
            var stuff = tester.Get<List<River>>("select * from Rivers");
            Assert.Empty(stuff);
        }
        [Fact]
        public void SqlFactory_DoesGet_DoesReadAndDoesHaveValues()
        {
            var facMock = new Mock<IDbFactory>();
            var connMock = new Mock<IDbConnection>();
            var commandMock = new Mock<IDbCommand>();
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            readerMock.Setup(r => r.FieldCount).Returns(2);
            readerMock.Setup(r => r.GetName(0)).Returns("RiverName");
            readerMock.Setup(r => r.GetName(1)).Returns("RiverCode");
            commandMock.Setup(cmd => cmd.ExecuteReader()).Returns(readerMock.Object);
            connMock.Setup(c =>c.CreateCommand()).Returns(commandMock.Object);
            facMock.Setup(f => f.CreateConnection()).Returns(connMock.Object);
            var tester = new SQLStorageFactory(facMock.Object);       
            var stuff = tester.Get<List<River>>("select * from Rivers");
            Assert.Empty(stuff);     
        }
    }
}