using System.Collections.Generic;
using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.helpers
{
    public class TableEntityDecoder
    {
        public TableEntityDecoder(Type t)
        {
            Value = Activator.CreateInstance(t);
        }
        private static object entity;
        public object Value 
        { 
            get 
            {
                return TableEntityDecoder.entity;
            } 
            set 
            { 
                TableEntityDecoder.entity = value;
            } 
            
        }
        public void DecodeProperty(PropertyInfo propInfo, KeyValuePair<string, EntityProperty> property)
        {
            if (propInfo != null && Mapper.ContainsKey(propInfo.PropertyType.ToString()))
            {
                Mapper[propInfo.PropertyType.ToString()].DynamicInvoke(property.Key, property.Value);
            }
        }
        private readonly Dictionary<string, Delegate> Mapper = new Dictionary<string, Delegate>()
        {
            {"System.String", new Action<string, dynamic>(DecodeStringValue)},
            {"System.DateTime", new Action<string, dynamic>(DecodeDateTimeValue)},
            {"System.DateTimeOffset", new Action<string, dynamic>(DecodeDateTimeOffsetValue)},
            {"System.Int32", new Action<string, dynamic>(DecodeInt32Value)},
            {"System.Int64", new Action<string, dynamic>(DecodeInt64Value)},
            {"System.Double", new Action<string, dynamic>(DecodeDoubleValue)},
            {"System.Boolean", new Action<string, dynamic>(DecodeBooleanValue)},
            {"System.Binary", new Action<string, dynamic>(DecodeBinaryValue)}
        };
        private static void DecodeStringValue(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.StringValue);
        }
        private static void DecodeDateTimeValue(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.DateTime);
        }
        private static void DecodeDateTimeOffsetValue(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.DateTimeOffsetValue);
        }
        private static void DecodeInt32Value(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.Int32Value);
        }
        private static void DecodeInt64Value(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.Int64Value);
        }
        private static void DecodeDoubleValue(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.DoubleValue);
        }
        private static void DecodeBooleanValue(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.BooleanValue);
        }
        private static void DecodeBinaryValue(string key, dynamic value)
        {
            entity.GetType().GetProperty(key).SetValue(entity, value.BinaryValue);
        }
    }
}