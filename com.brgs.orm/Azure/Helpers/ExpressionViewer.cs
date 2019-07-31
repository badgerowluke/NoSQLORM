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
    internal class Interegator : ExpressionVisitor
    {
        public string BuildQueryFilter<T>(Expression<Func<T, bool>> predicate)
        {
            var body = predicate.Body;
            return this.BuildQueryFilter(body);


        }
        private string BuildQueryFilter(Expression e)
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
                var body = (BinaryExpression)e;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} gt {valName}";
            }
            if(e.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                var body = (BinaryExpression)e;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} ge {valName}";

            }
            if(e.NodeType == ExpressionType.LessThan)
            {
                var body = (BinaryExpression)e;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                BuildQueryFilter(body.Left);
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} lt {valName}";

            }
            if(e.NodeType == ExpressionType.LessThanOrEqual)
            {
                var body = (BinaryExpression)e;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} le {valName}";

            }            
            if(e.NodeType == ExpressionType.AndAlso)
            {
                var body = (BinaryExpression)e;
                // var left = BuildQueryFilter(body.Left);
                // var right = BuildQueryFilter(body.Right);
                // return $"{left} and {right}";

            }
            if(e.NodeType == ExpressionType.OrElse)
            {
                var body = (BinaryExpression)e;
                var left = body.Left;
                var right = body.Right;
                // var val = BuildQueryFilter<T>(predicate);
                int three = 1;
            }

            return string.Empty;
        }
        private static Tuple<Type, object> processArgument(Expression element)
        {
            object argument = default(object);
            LambdaExpression l = Expression.Lambda(
                Expression.Convert(element, element.Type));
            Type parmType = l.ReturnType;
            argument = l.Compile().DynamicInvoke();
            return Tuple.Create(parmType, argument);
        }   
    

    }
}