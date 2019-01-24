using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.helpers
{
    public class AzureFormatHelpers
    {
        public static string PartitionKey { get; set; }
        public static object RecastEntity(DynamicTableEntity entity, Type type)
        {
            var val = Activator.CreateInstance(type);
            foreach (var property in entity.Properties)
            {
                var propInfo = val.GetType().GetProperty(property.Key);
                if (propInfo != null)
                {
                    switch (propInfo.PropertyType.ToString())
                    {
                        case ("System.String"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.StringValue);
                            break;
                        case ("System.DateTime"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.DateTime);
                            break;
                        case ("System.DateTimeOffset"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.DateTimeOffsetValue);
                            break;
                        case ("System.Int32"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.Int32Value);
                            break;
                        case ("System.Int64"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.Int64Value);
                            break;
                        case ("Systme.Double"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.DoubleValue);
                            break;
                        case ("Systme.Boolean"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.BooleanValue);
                            break;
                        case ("Systme.Binary"):
                            val.GetType().GetProperty(property.Key).SetValue(val, property.Value.BinaryValue);
                            break;
                    }
                }
            }
            return val;
        }
        public static DynamicTableEntity BuildTableEntity<T>(T record)
        {
            var props = new Dictionary<string, EntityProperty>();
            var fac = new TableEntityBuilder(props);
            string rowKey = string.Empty;
            string delimiter = "||";

            foreach (var prop in record.GetType().GetProperties())
            {
                if(prop.Name.ToUpper().Equals("ID"))
                {
                    rowKey = prop.GetValue(record).ToString();
                }
                if(fac.Mapper.ContainsKey(prop.PropertyType.ToString()))
                {
                    var func = fac.Mapper[prop.PropertyType.ToString()];
                    func.DynamicInvoke(prop.Name, prop.GetValue(record));
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