using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using System.Runtime.CompilerServices;
using com.brgs.orm.AzureHelpers.ExpressionHelpers;

[assembly:InternalsVisibleTo("com.brgs.orm.test")]

namespace com.brgs.orm.Azure.helpers
{
    internal class Interegator
    {
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

            throw new ArgumentException();
        }
    }
}