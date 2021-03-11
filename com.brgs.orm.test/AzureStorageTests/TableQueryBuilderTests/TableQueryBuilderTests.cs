using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using com.brgs.orm.Azure.helpers;
using Microsoft.WindowsAzure.Storage.Table;
using FluentAssertions;
using com.brgs.orm.Azure;

namespace com.brgs.orm.test.Azure.LamdaExpressionParsingTests
{
    public class LamdaExpressionParsingShould
    {
        private readonly AzureStorageFactory _builder;
        public LamdaExpressionParsingShould()
        {
            var mock = new Mock<ICloudStorageAccount>();
            _builder = new AzureStorageFactory(mock.Object);
        }
        [Fact]
        public void BuildQueryFilter()
        {
            var query =  _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV"));
  
            query.Should().NotBeNull();
        }
        [Fact]
        public void EncodeOperandCorrectly()
        {           
            var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV"));
            query.Should().BeEquivalentTo("StateCode eq 'WV'");
        }
        [Fact]
        public void ThrowWhenContainsIsCalled()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Name.Contains("Gauley"));
            query.Should().BeEmpty();

        }
        [Fact]
        public void EncodeNotOperandCorrectly()
        {
            var query =_builder.BuildQueryFilter<River>(r => !r.StateCode.Equals("WV"));
            query.Should().BeEquivalentTo("StateCode ne 'WV'");

        }
        [Fact]
        public void EncodeNullOrEmptyAndEquals()
        {
            var query = _builder.BuildQueryFilter<River>(r => !r.StateCode.Equals("") 
                                                            && r.StateCode == "WV");
            query.Should().BeEquivalentTo("StateCode ne '' and StateCode eq 'WV'");
        }

        [Fact]
        public void EncodeNullOrEmptyWithoutMethodCall()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.StateCode != ""  
                                                            && r.StateCode == "WV");
            query.Should().BeEquivalentTo("StateCode ne '' and StateCode eq 'WV'");            
        }

        [Fact]
        public void EncodeNullOrEmptyUsingStringNullOrEmpty() 
        {
            var query = _builder.BuildQueryFilter<River>(r => (r.StateCode != null || r.StateCode != "")
                                                            && r.StateCode == "WV");
            query.Should().BeEquivalentTo("StateCode ne '' or StateCode ne '' and StateCode eq 'WV'");   
        }

        [Fact]
        public void EncodeGreaterThanCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude > 36);
            query.Should().BeEquivalentTo("Latitude gt 36");
        }
        [Fact]
        public void EncodeGreatherThanOrEqualCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude >= 36);
            query.Should().BeEquivalentTo("Latitude ge 36");
        }

        [Fact]
        public void EncodeLessThanCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude < 36);
            query.Should().BeEquivalentTo("Latitude lt 36");
        }

        [Fact]
        public void EncodeLessThanOrEqualCorrectly()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.Latitude <= 36);
            query.Should().BeEquivalentTo("Latitude le 36");

        }
        [Fact]
        public void EncodeMultipleOperands()
        {
            var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV") 
                                                            && r.Name.Equals("Gauley") && r.Latitude > 36);
            Assert.Equal("StateCode eq 'WV' and Name eq 'Gauley' and Latitude gt 36", query);

        }
        [Fact]
        public void EncodeMultipleOrOperands()//OrElse
        {
            var query = _builder.BuildQueryFilter<River>(r => r.StateCode.Equals("WV") 
                                            || r.Name.Equals("Ohio"));
            Assert.Equal("StateCode eq 'WV' or Name eq 'Ohio'", query);
        }
        [Fact]
        public void ChainOrAndAndOperands()
        {
            var query = _builder.BuildQueryFilter<River>(
                r => r.Name.Equals("Gauley") || r.Name.Equals("Meadow") && r.StateCode.Equals("WV")
            );
            Assert.Equal("Name eq 'Gauley' or Name eq 'Meadow' and StateCode eq 'WV'", query);
        }
        [Fact]
        public void ReturnEmptyString()
        {
            var query = _builder.BuildQueryFilter<River>(null);
            Assert.Equal(string.Empty, query);
        }
        

    }

}