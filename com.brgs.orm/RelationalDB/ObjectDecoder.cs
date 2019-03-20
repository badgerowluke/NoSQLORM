using System;
using System.Data;
using System.Linq;

namespace com.brgs.orm.RelationalDB
{
    internal class ObjectDecoder
    {
        public static object DecodeData(IDataRecord record, Type type)
        {
            var val = Activator.CreateInstance(type);
            var properties = val.GetType().GetProperties();
            
            for(var f=0; f < record.FieldCount; f++)
            {
                var name = record.GetName(f);
                var value = record.GetValue(f);
                var property = properties.FirstOrDefault(p => p.Name.ToLower().Contains(name.ToLower()));
                if (property != null)
                {
                    val.GetType().GetProperty(property.Name)
                            .SetValue(val, value, null);
                }
            }
            return val;
        }        
    }
}