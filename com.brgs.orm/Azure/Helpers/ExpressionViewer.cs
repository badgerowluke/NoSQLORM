using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.WindowsAzure.Storage.Table;

namespace com.brgs.orm.Azure.helpers
{
    internal class Interegator : ExpressionVisitor
    {
        public Expression<Func<DynamicTableEntity, bool>> Stuff<T>(Expression<Func<T, bool>> predicate)
        {

            var input = predicate.Parameters[0]
            var dummy = (MethodCallExpression)predicate.Body;
            var methodName = dummy.Method.Name;
            var methodInfo = dummy.Method;
            var allParameters = from element in dummy.Arguments
                        select processArgument(element);

            int three = 1 + 1;

            return null;
        }
        private Tuple<Type, object> processArgument(Expression
            element)
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