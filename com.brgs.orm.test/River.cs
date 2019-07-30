using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.test
{
    internal class River
    {
        public string Id { get; set; }
		public string Name { get; set; }
		public string RiverId { get; set; }
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public string Srs { get; set; }
        public RiverData[] Levels { get; set; }
        public RiverData[] Flow { get; set; }
        public RiverData[] RiverData { get; set; }
        public string State { get; set; }
        public string StateCode { get; set; }
		public River() {
		}
    }
    internal class RiverData
    {
        public DateTime DateTime { get; set; }
        public object Value { get; set; }
        public string Flow { get; set; }
        public string Level { get; set; }        
    }
    internal class RiverEntity : River, ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            throw new NotImplementedException();
        }
    }
}