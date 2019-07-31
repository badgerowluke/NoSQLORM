using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace com.brgs.orm.AzureHelpers.ExpressionHelpers
{
    internal class ExpressionTypeGreaterThanHelper
    {
        public ExpressionTypeGreaterThanHelper(Expression predicate)
        {
            Body = (BinaryExpression) predicate;
        }
        public BinaryExpression Body { get; private set; }
        public MemberExpression Left { get { return (MemberExpression) Body.Left; } }
        public ConstantExpression Right { get { return (ConstantExpression) Body.Right; } }
        public string PropertyName { get { return Left.Member.Name; } }
        public object Value { get { return Right.Value; } }
        public override string ToString()
        {
            return $"{PropertyName} gt {Value}";
        }
    }

}