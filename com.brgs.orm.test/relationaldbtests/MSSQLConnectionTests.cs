using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using com.brgs.orm.RelationalDB;
using System.Data;

namespace com.brgs.orm.test
{
    public class MSSQLConnectionTests
    {
        [Fact]
        public void WeDoGetAMSSQLConnectionObject()
        {
            var tester = new MSSQLConnection(string.Empty);
            Assert.NotNull(tester);
        }
        [Fact]
        public void DoICallBaseCreateConnection()
        {
            var conn = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            conn.Setup(c => c.CreateCommand() ).Returns(command.Object);
            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(r => r.Read()).Returns(false);
            command.Setup(c => c.ExecuteReader()).Returns(readerMock.Object);
            var mockFac =new Mock<MSSQLConnection>(string.Empty).As<IDbFactory>();
            mockFac.Setup(m => m.CreateConnection()).Returns(conn.Object);
            mockFac.CallBase = true;
            var stuff = mockFac.Object.Query<List<River>>(It.IsAny<string>());
            Assert.Empty(stuff);
        }
        [Fact]
        public void MSSQLConnection_DoesRetrieveValues_FromCommon()
        {
            var conn = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            conn.Setup(c => c.CreateCommand() ).Returns(command.Object);

            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            readerMock.Setup(r => r.FieldCount).Returns(2);
            readerMock.Setup(r => r.GetName(0)).Returns("RiverName");
            readerMock.Setup(r => r.GetName(1)).Returns("RiverId");
            readerMock.Setup(r => r.GetValue(0)).Returns("GAULEY RIVER BELOW SUMMERSVILLE DAM, WV");
            readerMock.Setup(r => r.GetValue(1)).Returns("03189600");

            command.Setup(c => c.ExecuteReader()).Returns(readerMock.Object);
            var mockFac =new Mock<MSSQLConnection>(string.Empty).As<IDbFactory>();
            mockFac.Setup(m => m.CreateConnection()).Returns(conn.Object);
            mockFac.CallBase = true;
            var stuff = mockFac.Object.Query<List<River>>(It.IsAny<string>());
            Assert.NotEmpty(stuff);            
        }
        [Fact]
        public void MSSQLConnection_DoesRetrieveSingleEntity()
        {
            var conn = new Mock<IDbConnection>();
            var command = new Mock<IDbCommand>();
            conn.Setup(c => c.CreateCommand() ).Returns(command.Object);    

            var readerMock = new Mock<IDataReader>();
            readerMock.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            readerMock.Setup(r => r.FieldCount).Returns(2);
            readerMock.Setup(r => r.GetName(0)).Returns("RiverName");
            readerMock.Setup(r => r.GetName(1)).Returns("RiverId");
            readerMock.Setup(r => r.GetValue(0)).Returns("GAULEY RIVER BELOW SUMMERSVILLE DAM, WV");
            readerMock.Setup(r => r.GetValue(1)).Returns("03189600");  

            command.Setup(c => c.ExecuteReader()).Returns(readerMock.Object);
            var mockFac =new Mock<MSSQLConnection>(string.Empty).As<IDbFactory>();
            mockFac.Setup(m => m.CreateConnection()).Returns(conn.Object);
            mockFac.CallBase = true;  
            var stuff = mockFac.Object.Query<River>(It.IsAny<string>());
            Assert.NotNull(stuff);
            Assert.Equal("03189600", stuff.RiverId);
        }

    }
}