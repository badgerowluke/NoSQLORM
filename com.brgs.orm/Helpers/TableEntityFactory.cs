using System.Collections.Generic;
using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
namespace com.brgs.orm.helpers
{
    public static class TableEntityFactory
    {
        private static readonly string[] types = {
            "System.String", "System.Boolean", "System.Double", "System.Int32", "System.Int64"
        };
        private static readonly Dictionary<string, Delegate> mapper = new Dictionary<string, Delegate>(){
            {"System.String", new Action<string, dynamic>(AddStringProperty)},
            {"System.Boolean", new Action<string, dynamic>(AddBooleanProperty)}
        };
            
        private static Dictionary<string, EntityProperty> properties;
        private static string partitionKey;

        private static void AddInt32Propeerty(string name, dynamic value)
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