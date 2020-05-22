using System.Collections.Generic;
using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
namespace com.brgs.orm.Azure.helpers
{
    public class TableEntityBuilder
    {
        public Dictionary<string, EntityProperty> Properties {get;set;}

        private readonly Dictionary<string, Delegate> Mapper;


        public TableEntityBuilder()
        {
            Mapper = BuildMapper();
        }

        public TableEntityBuilder(Dictionary<string, EntityProperty> props)
        {
            Properties = props;
            Mapper = BuildMapper();          
        }

        private Dictionary<string, Delegate> BuildMapper()
        {
            return new Dictionary<string, Delegate>
            {
                { "System.String", new Action<string, dynamic>(AddStringProperty) },
                { "System.Boolean", new Action<string, dynamic>(AddBooleanProperty) },
                { "System.Double", new Action<string, dynamic>(AddDoubleProperty) },
                { "System.Int32", new Action<string, dynamic>(AddInt32Property) },
                { "System.Int64", new Action<string, dynamic>(AddInt64Property) },
                { "System.DateTime", new Action<string, dynamic>(AddDateTimeProperty) }
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
            if(name != null && value != null && Properties != null && !Properties.ContainsKey(name) )
            {
                Properties.Add(name, new EntityProperty((Int32?)value));
            } 
        }
        private void AddInt64Property(string name, dynamic value)
        {
            if(name != null && value != null && Properties != null && !Properties.ContainsKey(name) )
            {
                Properties.Add(name, new EntityProperty((Int64?)value));
            } 
        }
        private void AddDoubleProperty(string name, dynamic value)
        {
            if(name != null && value != null && Properties != null && !Properties.ContainsKey(name) )
            {
                Properties.Add(name, new EntityProperty((double?)value));
            } 
        }
        private void AddBooleanProperty(string name, dynamic value)
        {
            if(name != null && value != null && Properties != null && !Properties.ContainsKey(name) )
            {
                Properties.Add(name, new EntityProperty((bool?)value));
            } 
        }
        private void AddStringProperty(string name, dynamic value)
        {
            
            if(name != null && value != null && Properties != null 
                && !Properties.ContainsKey(name) )
            {
                Properties.Add(name, new EntityProperty((string) value));
            } 
        }
        private void AddDateTimeProperty(string name, dynamic value)
        {
            if(name != null && value != null && Properties != null
                && !Properties.ContainsKey(name))
            {
                var date = (DateTime) value;
                var utcDate = date.ToUniversalTime();
                Properties.Add(name, new EntityProperty(utcDate));
            }

        }
    }
}