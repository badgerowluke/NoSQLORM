using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("com.brgs.orm.test")]

namespace com.brgs.orm.Azure.helpers
{
    internal class Interegator : ExpressionVisitor
    {
        public string BuildQueryFilter<T>(Expression<Func<T, bool>> predicate)
        {

            if(predicate.Body.NodeType == ExpressionType.Call)
            {
                var body = (MethodCallExpression)predicate.Body;
                var operand = (MemberExpression)body.Object;
                var propName = operand.Member.Name;
                var allParameters = from element in body.Arguments
                            select processArgument(element);

                var val = $"{propName} eq '{allParameters.ToList()[0].Item2}'";
                return val;

            }
            if(predicate.Body.NodeType == ExpressionType.Not)
            {
                var body = (UnaryExpression)predicate.Body;
                var operand = (MethodCallExpression)body.Operand;
                var allParameters = from element in operand.Arguments
                            select processArgument(element);
                var member = (MemberExpression) operand.Object;
                var name = member.Member.Name;
                return  $"{name} ne '{allParameters.ToList()[0].Item2}'";
            }
            if(predicate.Body.NodeType == ExpressionType.GreaterThan)
            {
                var body = (BinaryExpression)predicate.Body;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} gt {valName}";
            }
            if(predicate.Body.NodeType == ExpressionType.GreaterThanOrEqual)
            {
                var body = (BinaryExpression)predicate.Body;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} ge {valName}";

            }
            if(predicate.Body.NodeType == ExpressionType.LessThan)
            {
                var body = (BinaryExpression)predicate.Body;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} lt {valName}";

            }
            if(predicate.Body.NodeType == ExpressionType.LessThanOrEqual)
            {
                var body = (BinaryExpression)predicate.Body;
                var leftName = (MemberExpression)body.Left;
                var rightName =(ConstantExpression) body.Right;
                var propName = leftName.Member.Name; 
                var valName= rightName.Value;

                return $"{propName} le {valName}";

            }            
            if(predicate.Body.NodeType == ExpressionType.AndAlso)
            {
                var body = (BinaryExpression)predicate.Body;
                var left = BuildQueryFilter(body.Left);
                var right = BuildQueryFilter(body.Right);
                return $"{left} and {right}";

            }
            if(predicate.Body.NodeType == ExpressionType.OrElse)
            {
                var body = (BinaryExpression)predicate.Body;
                var left = body.Left;
                var right = body.Right;
                var val = BuildQueryFilter<T>(predicate);
                int three = 1;
            }

            return string.Empty;
        }
        private string BuildQueryFilter(Expression e)
        {
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