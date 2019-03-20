using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Moq;
using com.brgs.orm.RelationalDB;
using System.Data;

namespace com.brgs.orm.test
{
    public class SqlFactoryPostTests
    {
        [Fact]
        public void SqlFactory_Post_EchoesBackTheValue()
        {
            var river = new River(){RiverId= "03189600", Name="GAULEY RIVER BELOW SUMMERSVILLE DAM, WV"};
            var tester = new SQLStorageFactory(null);
            var stuff = tester.Post(river);
            
        }

    }
}
