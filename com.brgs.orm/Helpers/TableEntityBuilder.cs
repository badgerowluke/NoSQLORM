using System.Collections.Generic;
using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
namespace com.brgs.orm.helpers
{
    public class TableEntityBuilder
    {
        private  Dictionary<string, EntityProperty> properties;

        private readonly Dictionary<string, Delegate> Mapper;
        public TableEntityBuilder()
        {
            Mapper = new Dictionary<string, Delegate>()
            {
                { "System.String", new Action<string, dynamic>(AddStringProperty) },
                { "System.Boolean", new Action<string, dynamic>(AddBooleanProperty) },
                { "System.Double", new Action<string, dynamic>(AddDoubleProperty) },
                { "System.Int32", new Action<string, dynamic>(AddInt32Property) },
                { "System.Int64", new Action<string, dynamic>(AddInt64Property) }
            };
        }
        public Dictionary<string, EntityProperty> Properties 
        {
            get 
            { 
                return properties; 
            }
            set 
            { 
                properties = value; 
            }
        }
        public TableEntityBuilder(Dictionary<string, EntityProperty> props)
        {
            Properties = props;
            Mapper = new Dictionary<string, Delegate>()
            {
                { "System.String", new Action<string, dynamic>(AddStringProperty) },
                { "System.Boolean", new Action<string, dynamic>(AddBooleanProperty) },
                { "System.Double", new Action<string, dynamic>(AddDoubleProperty) },
                { "System.Int32", new Action<string, dynamic>(AddInt32Property) },
                { "System.Int64", new Action<string, dynamic>(AddInt64Property) }
            };            
        }
        public void EncodeProperty<T>(PropertyInfo prop, T record)
        {
            if(Mapper.ContainsKey(prop.PropertyType.ToString()))
            {
                Mapper[prop.PropertyType.ToString()].DynamicInvoke(prop.Name, prop.GetValue(record));
            }            
        }


        private void AddInt32Property(string name, dynamic value)
        {
            if(name != null && value != null && properties != null && !properties.ContainsKey(name) )
            {
                properties.Add(name, new EntityProperty((Int32?)value));
            } 
        }
        private void AddInt64Property(string name, dynamic value)
        {
            if(name != null && value != null && properties != null && !properties.ContainsKey(name) )
            {
                properties.Add(name, new EntityProperty((Int64?)value));
            } 
        }
        private void AddDoubleProperty(string name, dynamic value)
        {
            if(name != null && value != null && properties != null && !properties.ContainsKey(name) )
            {
                properties.Add(name, new EntityProperty((double?)value));
            } 
        }
        private void AddBooleanProperty(string name, dynamic value)
        {
            if(name != null && value != null && properties != null && !properties.ContainsKey(name) )
            {
                properties.Add(name, new EntityProperty((bool?)value));
            } 
        }
        private void AddStringProperty(string name, dynamic value)
        {
            
            if(name != null && value != null && properties != null 
                && !properties.ContainsKey(name) )
            {
                properties.Add(name, new EntityProperty((string) value));
            } 
        }
    }
}