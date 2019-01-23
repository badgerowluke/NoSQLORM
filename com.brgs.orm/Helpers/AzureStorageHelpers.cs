using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.helpers
{
    public class AzureFormatHelpers
    {
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
        public static DynamicTableEntity BuildTableEntity<T>(PropertyInfo[] recordProps, T record)
        {
            var props = new Dictionary<string, EntityProperty>();
            var partitionKey = "1";
            foreach (var prop in recordProps)
            {
                // if(prop.Name.ToUpper().Equals("ID"))
                // {
                //     partitionKey = prop.GetValue(record).ToString();
                // }
                switch (prop.PropertyType.ToString())
                {
                    case ("System.String"):
                        props.Add(prop.Name, new EntityProperty(prop.GetValue(record).ToString()));
                        break;
                    case ("System.Boolean"):
                        props.Add(prop.Name, new EntityProperty((bool?)prop.GetValue(record)));
                        break;
                    case ("System.Double"):
                        props.Add(prop.Name, new EntityProperty((double?)prop.GetValue(record)));
                        break;
                    case ("System.Int32"):
                        props.Add(prop.Name, new EntityProperty((Int32?)prop.GetValue(record)));
                        break;
                    case ("System.Int64"):
                        props.Add(prop.Name, new EntityProperty((Int64?)prop.GetValue(record)));
                        break;
                }
            }
            return new DynamicTableEntity(partitionKey, record.GetType().Name, "*", props);
        }
    }
}