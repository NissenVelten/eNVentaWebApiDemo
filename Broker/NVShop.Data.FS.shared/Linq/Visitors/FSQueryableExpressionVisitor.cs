namespace NVShop.Data.FS.Linq.Visitors
{
	using System.Linq.Expressions;
	using NVShop.Data.FS.Linq.Query;
	using Remotion.Linq.Clauses;

	internal class FSQueryableExpressionVisitor : ExpressionVisitorBase
	{
		private FSQueryModelVisitor _fSQueryModelVisitor;
		private IQuerySource _querySource;

		public FSQueryableExpressionVisitor(FSQueryModelVisitor fSQueryModelVisitor, IQuerySource querySource)
		{
			_fSQueryModelVisitor = fSQueryModelVisitor;
			_querySource = querySource;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			return base.VisitMethodCall(node);
		}
	}
}
