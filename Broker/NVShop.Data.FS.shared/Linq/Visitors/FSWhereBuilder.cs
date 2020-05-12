namespace NVShop.Data.FS.Linq.Visitors
{
	using Remotion.Linq.Clauses.Expressions;
	using Remotion.Linq.Parsing;
	using System.Collections.Generic;
	using System.Linq.Expressions;

	internal class FSWhereBuilder : RelinqExpressionVisitor
	{
        private readonly Expression _expression;
        private List<string> _whereExpressions;

        public string WhereClause
        {
            get
            {
                if (_whereExpressions == null)
				{
					_whereExpressions = new List<string>();

                    Visit(_expression);
                }
                return _whereExpressions.Join(" AND ");
            }
        } 
        public FSWhereBuilder(Expression expression)
        {
            _expression = expression;
        }

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			return base.VisitQuerySourceReference(expression);
		}

		protected override Expression VisitBinary(BinaryExpression expr)
        {
            if (expr.NodeType == ExpressionType.Equal)
            {
                if (expr.Left.NodeType == ExpressionType.MemberAccess)
                {
                    var leftMember = expr.Left as MemberExpression;
                    if (leftMember != null)
                    {
                        var propName = leftMember.Member.Name;
                        object value = null;

                        if (expr.Right.NodeType == ExpressionType.Constant)
                        {
                            var rightContant = expr.Right as ConstantExpression;
                            if (rightContant != null)
                            {
                                value = rightContant.Value;
                            }
                        }

                        //_whereExpressions.Add(".Eq(propName, value);
                    }
                }
                return base.VisitBinary(expr);
            }

            return base.VisitBinary(expr);
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            return u;
        }
    }
}
