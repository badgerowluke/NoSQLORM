
using System.Linq.Expressions;

namespace com.brgs.orm.AzureHelpers.ExpressionHelpers
{
    internal class ExpressionTypeEqualHelper : ExpressionHelper
    {
        private readonly bool _notEqual;
        public BinaryExpression Body { get; private set; }
        public MemberExpression Left { get { return (MemberExpression) Body.Left; } }
        public ConstantExpression Right { get { return (ConstantExpression) Body.Right; } }
        public string PropertyName { get { return Left.Member.Name; } }
        public object Value { get { return Right.Value; } }

        public ExpressionTypeEqualHelper(Expression predicate, bool notEqual =false)
        {
            Body = (BinaryExpression) predicate;
            _notEqual = notEqual;
            
        }

        public override string ToString()
        {
            if(_notEqual)
            {
                return $"{PropertyName} ne '{Value}'";
            }
            return $"{PropertyName} eq '{Value}'";
        }

    }
}