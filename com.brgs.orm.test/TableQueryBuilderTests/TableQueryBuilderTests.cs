using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.test
{
    public class TableQueryBuilderTests
    {
        [Fact]
        public void DoesBuildQueryFilter ()
        {
            var builder = new Interegator();
            var query = builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV"));
            Assert.NotNull(query);
        }
        [Fact]
        public void DoesEncodeOperandCorrectly()
        {
            var builder = new Interegator();
            
            var query = builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV"));
            Assert.Equal("StateCode eq 'WV'", query);  
        }
        [Fact]
        public void DoesEncodeNotOperandCorrectly()
        {
            var builder = new Interegator();
            
            var query = builder.BuildQueryFilter<River>(r => !r.StateCode.Equals("WV"));
            Assert.Equal("StateCode ne 'WV'", query);  

        }
        [Fact]
        public void DoesEncodeGreaterThanCorrectly()
        {
            var builder = new Interegator();
            var query = builder.BuildQueryFilter<River>(r => r.Latitude > 36);
            Assert.Equal("Latitude gt 36", query);
        }
        [Fact]
        public void DoesEncodeGreatherThanOrEqualCorrectly()
        {
            var builder = new Interegator();
            var query = builder.BuildQueryFilter<River>(r => r.Latitude >= 36);
            Assert.Equal("Latitude ge 36", query);

        }
        [Fact]
        public void DoesEncodeLessThanCorrectly()
        {
            var builder = new Interegator();
            var query = builder.BuildQueryFilter<River>(r => r.Latitude < 36);
            Assert.Equal("Latitude lt 36", query);
        }
        [Fact]
        public void DoesEncodeLessThanOrEqualCorrectly()
        {
            var builder = new Interegator();
            var query = builder.BuildQueryFilter<River>(r => r.Latitude <= 36);
            Assert.Equal("Latitude le 36", query);

        }
        // [Fact]
        // public void DoesEncodeMultipleOperands()
        // {
        //     var builder = new Interegator();
        //     var query = builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV") 
        //     && r.Name.Equals("Gauley") && r.Latitude > 36);
        //     Assert.Equal("StateCode eq 'WV' and Name eq 'Gauley'", query);

        // }
        // [Fact]
        // public void DoesEncodeMultipleOrOperands()//OrElse
        // {
        //     var builder = new Interegator();
        //     var query = builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV") 
        //     || r.Name.Equals("Ohio"));
        //     Assert.Equal("1", query);
        // }
        

    }

}