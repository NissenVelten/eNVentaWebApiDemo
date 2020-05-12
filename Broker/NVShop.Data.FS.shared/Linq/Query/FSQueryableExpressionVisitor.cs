using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace NVShop.Data.FS.Linq.Query
{
    public class FSQueryableExpressionVisitor : ExpressionVisitorBase
    {
        public FSQueryableExpressionVisitor(FSQueryModelVisitor queryModelVisitor, IQuerySource querySource)
        {
            Guard.ArgumentNotNull(() => queryModelVisitor);

            QueryModelVisitor = queryModelVisitor;
            QuerySource = querySource;
        }

        public FSQueryModelVisitor QueryModelVisitor { get; }
        public IQuerySource QuerySource { get; }

        public FSQueryPartsAggregator QueryParts { get; } = new FSQueryPartsAggregator();
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public bool IsCountQuery { get; set; }
        public bool IsLongCountQuery { get; set; }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
		{
			var visitor = new FSQueryModelVisitor();

			visitor.VisitQueryModel(expression.QueryModel);
			
			QueryParts.Merge(visitor.QueryParts);

			Offset = visitor.Offset;
			Limit = visitor.Limit;

            IsCountQuery = visitor.IsCountQuery;

			return visitor.Expression;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			return base.VisitParameter(node);
		}

		protected override Expression VisitExtension(Expression node)
		{
			return base.VisitExtension(node);
		}
	}
}
