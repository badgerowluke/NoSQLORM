using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.brgs.orm.Azure;
using Microsoft.WindowsAzure.Storage.Auth;

using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using com.brgs.orm.Azure.helpers;


namespace com.brgs.orm.test.Azure
{
    public abstract class BaseAzureTableStorageTester : AzureFormatHelper
    {
        public Mock<ICloudStorageAccount> AccountMock { get; private set; }
        public Mock<CloudTableClient> TableClientMock { get; private set; }
        public Mock<CloudTable> TableMock { get; private set; }
        public RiverEntity Entity { get; private set; }
        public River ARiver { get; private set; }
        public AzureStorageFactory Fac { get; private set; }

        public BaseAzureTableStorageTester()
        {

            Entity = Mock.Of<RiverEntity>();
            ARiver = Mock.Of<River>(r => r.Name == GetRandomRiverNameFroMock() );
            AccountMock = new Mock<ICloudStorageAccount>();
            TableClientMock = new Mock<CloudTableClient>(new Uri("https://www.google.com"), new StorageCredentials() );
            TableMock = new Mock<CloudTable>(new Uri("https://www.google.com"));

            TableClientMock.Setup(tc => tc.GetTableReference(It.IsAny<string>())).Returns(TableMock.Object);

            AccountMock.Setup(c => c.CreateCloudTableClient()).Returns(TableClientMock.Object);
            Fac = new AzureStorageFactory(AccountMock.Object)
            {
                PartitionKey = "TACOS",
                CollectionName = "Pizza"                
            };
        }

        private string GetRandomRiverNameFroMock()
        {
            var ran = new Random();
            string[] riverNames = new string[10] {
                "Aroostook River at Washburn, Maine", "river2", "river3", "river4", "river5", "river6", "river7",
                "river8", "river9", "river10"
            };
            return riverNames[ran.Next(riverNames.Count())];

        }

        public virtual TableQuerySegment GetTableQuerySegments(int count = 1)
        {
            var ctor = typeof(TableQuerySegment).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(c => c.GetParameters().Count() == count);
            return ctor.Invoke(new object[]{ new List<DynamicTableEntity>(){
                new DynamicTableEntity()
            }}) as TableQuerySegment;
        }

        public virtual IEnumerable<River> BuildRiverEnumerable(int num)
        {
            var list = new List<River>();
            for(var x =0; x < num; x++)
            {
                var r = new River();
                list.Add(r);
            }
            return list;
        }

        public virtual IEnumerable<DynamicTableEntity> BuildRiverTableEntities(int num, string partitionKey = "TACOS", bool addPartitionKey = true)
        {
            var list = new List<DynamicTableEntity>();
            for(var x=0; x <num; x++)
            {
                var r = new DynamicTableEntity();
                if(addPartitionKey)
                {
                    r.PartitionKey = partitionKey;
                }
                r.RowKey = x.ToString();
            
                list.Add(r);
            }
            return list;
        }

        public virtual IEnumerable<DynamicTableEntity> BuildRiverTableEntitiesForDelete(int num, string partitionKey = "TACOS", bool addPartitionKey = true)
        {
            var list = new List<DynamicTableEntity>();
            for(var x=0; x <num; x++)
            {
                var r = new DynamicTableEntity();
                if(addPartitionKey)
                {
                    r.PartitionKey = partitionKey;
                }
                r.ETag = "*";
                r.RowKey = x.ToString();
            
                list.Add(r);
            }
            return list;            
        }
    
    }
}