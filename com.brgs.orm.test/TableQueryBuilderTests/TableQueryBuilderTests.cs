using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.test
{
    public class InteregatorShould
    {
        private readonly Interegator _builder;
        public InteregatorShould()
        {
            _builder = new Interegator();
        }
        [Fact]
        public void BuildQueryFilter()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV"));
            Assert.NotNull(query);
        }
        [Fact]
        public void EncodeOperandCorrectly()
        {           
            var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV"));
            Assert.Equal("StateCode eq 'WV'", query);  
        }
        [Fact]
        public void EncodeNotOperandCorrectly()
        {
            var query =_builder.BuildQueryFilter<River>(r => !r.StateCode.Equals("WV"));
            Assert.Equal("StateCode ne 'WV'", query);  

        }
        [Fact]
        public void EncodeGreaterThanCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude > 36);
            Assert.Equal("Latitude gt 36", query);
        }
        [Fact]
        public void EncodeGreatherThanOrEqualCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude >= 36);
            Assert.Equal("Latitude ge 36", query);

        }
        [Fact]
        public void EncodeLessThanCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude < 36);
            Assert.Equal("Latitude lt 36", query);
        }
        [Fact]
        public void EncodeLessThanOrEqualCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude <= 36);
            Assert.Equal("Latitude le 36", query);

        }
        // [Fact]
        // public void EncodeMultipleOperands()
        // {
        //     var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV") 
        //     && r.Name.Equals("Gauley") && r.Latitude > 36);
        //     Assert.Equal("StateCode eq 'WV' and Name eq 'Gauley'", query);

        // }
        // [Fact]
        // public void EncodeMultipleOrOperands()//OrElse
        // {
        //     var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV") 
        //     || r.Name.Equals("Ohio"));
        //     Assert.Equal("1", query);
        // }
        

    }

}