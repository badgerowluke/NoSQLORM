using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using com.brgs.orm.AzureHelpers.ExpressionHelpers;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure.helpers
{
    public abstract class AzureFormatHelper
    {
        public string PartitionKey { get; set; }
        public string CollectionName { get; set; }

        public object RecastEntity(DynamicTableEntity entity, Type type)
        {
            var decoder = new TableEntityDecoder(type);
            foreach (var property in entity.Properties)
            {
                var propInfo = type.GetProperty(property.Key);
                decoder.DecodeProperty(propInfo, property);
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
                fac.EncodeProperty<T>(prop, record);

            }
            /* 
                TODO lift the delimiter between the partition and entity id (row id).
                this needs to be lifted up somewhere much closer to the ultimate
                domain context deveveloper.
            */
            return new DynamicTableEntity(PartitionKey, $"{PartitionKey}{delimiter}{rowKey}", "*", fac.Properties);
        }
        public string BuildQueryFilter<T>(Expression<Func<T, bool>> predicate)
        {
            if(predicate != null)
            {
                var body = predicate.Body;
                return BuildQueryFilter(body);
            }
            return string.Empty;
        }
        private  string BuildQueryFilter(Expression e)
        {
            if(e.NodeType == ExpressionType.Call)
            {
                return new ExpressionTypeCallHelper(e).ToString();
            }
            if(e.NodeType == ExpressionType.Not)
            {
                return  new ExpressionTypeNotHelper(e).ToString();
            }
            if(e.NodeType == ExpressionType.GreaterThan)
            {
                return new ExpressionTypeGreaterThanHelper(e).ToString();
            }
            if(e.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                return new ExpressionTypeGreaterThanHelper(e, true).ToString();
            }
            if(e.NodeType == ExpressionType.LessThan)
            {
                return new ExpressionTypeLessThanHelper(e).ToString();
            }
            if(e.NodeType == ExpressionType.LessThanOrEqual)
            {
                return new ExpressionTypeLessThanHelper(e, true).ToString();
            }            
            if(e.NodeType == ExpressionType.AndAlso)
            {
                var body = (BinaryExpression)e;
                var left = BuildQueryFilter(body.Left);
                var right = BuildQueryFilter(body.Right);
                return $"{left} and {right}";
            }
            if(e.NodeType == ExpressionType.OrElse)
            {
                var body = (BinaryExpression)e;
                var left = BuildQueryFilter(body.Left);
                var right = BuildQueryFilter(body.Right);
                return $"{left} or {right}";

            }

            return string.Empty;
        }        
    }
}