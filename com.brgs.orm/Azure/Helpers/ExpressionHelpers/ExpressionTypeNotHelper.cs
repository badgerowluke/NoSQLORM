using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace com.brgs.orm.AzureHelpers.ExpressionHelpers
{
    internal class ExpressionTypeNotHelper : ExpressionHelper
    {

        public ExpressionTypeNotHelper(Expression predicate)
        {
            Body = (UnaryExpression)predicate;
        }

        public UnaryExpression Body { get; private set; }
        public MethodCallExpression Operand { get { return (MethodCallExpression)Body.Operand; } }
        public MemberExpression Member { get { return (MemberExpression)Operand.Object; } }
        public IEnumerable<Tuple<Type, object>> Parameters 
        { 
            get 
            {
                return from element in Operand.Arguments select processArgument(element);
            }
        }        
        public override string ToString()
        {
            return  $"{Member.Member.Name} ne '{Parameters.ToList()[0].Item2}'";
        }
    }

}