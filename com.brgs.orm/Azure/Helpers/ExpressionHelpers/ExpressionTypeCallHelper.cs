using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace com.brgs.orm.AzureHelpers.ExpressionHelpers
{

    internal class ExpressionTypeCallHelper:  ExpressionHelper
    {
         /* TODO:  Contains is likely a Method Call */
        
        public ExpressionTypeCallHelper(Expression predicate)
        {
            Body = (MethodCallExpression) predicate;
        }
        public MethodCallExpression Body { get; private set; }
        public MemberExpression Object { get { return (MemberExpression) Body.Object; } }
        public string PropertyName { get { return Object.Member.Name; } }
        public IEnumerable<Tuple<Type, object>> Parameters 
        { 
            get 
            {
                return from element in this.Body.Arguments select processArgument(element);
            }
        }
        public override string ToString()
        {
            if(Body.Method.Name.Equals("Contains"))
            {
                return string.Empty;
            }
            return $"{PropertyName} eq '{Parameters.ToList()[0].Item2}'";
        }
       
    }
}