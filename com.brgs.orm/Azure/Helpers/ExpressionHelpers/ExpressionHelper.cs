using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace com.brgs.orm.AzureHelpers.ExpressionHelpers
{
        internal abstract class ExpressionHelper
    {
        protected Tuple<Type, object> processArgument(Expression element)
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