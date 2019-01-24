using System.Collections.Generic;
using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
namespace com.brgs.orm.helpers
{
    public class TableEntityFactory
    {
        public readonly Dictionary<string, Delegate> mapper = new Dictionary<string, Delegate>(){
            { "System.String", new Action<string, dynamic>(AddStringProperty) },
            { "System.Boolean", new Action<string, dynamic>(AddBooleanProperty) },
            { "System.Double", new Action<string, dynamic>(AddDoubleProperty) },
            { "System.Int32", new Action<string, dynamic>(AddInt32Property) },
            { "System.Int64", new Action<string, dynamic>(AddInt64Property) }
        };
        private static Dictionary<string, EntityProperty> properties;
        public Dictionary<string, EntityProperty> Properties {
            get { return TableEntityFactory.properties; }
            set { TableEntityFactory.properties = value; }
        }
        private static void AddInt32Property(string name, dynamic value)
        {
            properties.Add(name, new EntityProperty((Int32?)value));
        }
        private static void AddInt64Property(string name, dynamic value)
        {
            properties.Add(name, new EntityProperty((Int64?)value));
        }
        private static void AddDoubleProperty(string name, dynamic value)
        {
            properties.Add(name, new EntityProperty((double?)value));
        }
        private static void AddBooleanProperty(string name, dynamic value)
        {
            properties.Add(name, new EntityProperty((bool?)value));
        }
        private static void AddStringProperty(string name, dynamic value)
        {
            properties.Add(name, new EntityProperty(value.ToString()));
        }
    }
}