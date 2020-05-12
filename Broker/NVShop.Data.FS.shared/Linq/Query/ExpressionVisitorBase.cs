namespace NVShop.Data.FS.Linq.Query
{
	using System.Linq.Expressions;
	using Remotion.Linq.Clauses.Expressions;
	using Remotion.Linq.Parsing;

	public abstract class ExpressionVisitorBase : RelinqExpressionVisitor
	{
		protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			expression.QueryModel.TransformExpressions(Visit);

			return base.VisitSubQuery(expression);
		}
	}
}
