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
            return null;
        }
        public static object BuildTableEntity<T>(PropertyInfo[] properties, T record)
        {
            return null;
        }
    }
}