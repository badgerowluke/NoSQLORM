using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.helpers
{
    public class AzureFormatHelper
    {
        public string PartitionKey { get; set; }
        public AzureFormatHelper()
        {
            PartitionKey = string.Empty;
        }
        public AzureFormatHelper(string key)
        {
            PartitionKey = key;
        }
        public static object RecastEntity(DynamicTableEntity entity, Type type)
        {
            var val = Activator.CreateInstance(type);
            var decoder = new TableEntityDecoder(type);
            foreach (var property in entity.Properties)
            {
                var propInfo = val.GetType().GetProperty(property.Key);
                if (propInfo != null && decoder.Mapper.ContainsKey(propInfo.PropertyType.ToString()))
                {
                    decoder.Mapper[propInfo.PropertyType.ToString()].DynamicInvoke(property.Key, property.Value);
                }
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
                if(fac.Mapper.ContainsKey(prop.PropertyType.ToString()))
                {
                    fac.Mapper[prop.PropertyType.ToString()].DynamicInvoke(prop.Name, prop.GetValue(record));
                }
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