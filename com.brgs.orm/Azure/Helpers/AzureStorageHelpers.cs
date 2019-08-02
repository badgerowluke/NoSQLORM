using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure.helpers
{
    public abstract class AzureFormatHelper
    {
        public string PartitionKey { get; set; }
        public string CollectionName { get; set; }

        public object RecastEntity(DynamicTableEntity entity, Type type)
        {
            var decoder = new TableEntityDecoder(type);
            foreach (var property in entity.Properties)
            {
                var propInfo = type.GetProperty(property.Key);
                decoder.DecodeProperty(propInfo, property);
            }
            return decoder.Value;
        }
        public DynamicTableEntity BuildTableEntity<T>(T record)
        {
            var props = new Dictionary<string, EntityProperty>();
            var fac = new TableEntityBuilder(props);
            string rowKey = string.Empty;
            string delimiter = "||";

            foreach (var prop in record.GetType().GetProperties())
            {
                if(prop.Name.ToUpper().Equals("ID") && prop.GetValue(record) != null)
                {
                    rowKey = prop.GetValue(record).ToString();
                }
                fac.EncodeProperty<T>(prop, record);

            }
            /* 
                TODO lift the delimiter between the partition and entity id (row id).
                this needs to be lifted up somewhere much closer to the ultimate
                domain context deveveloper.
            */
            return new DynamicTableEntity(PartitionKey, $"{PartitionKey}{delimiter}{rowKey}", "*", fac.Properties);
        }
    }
}