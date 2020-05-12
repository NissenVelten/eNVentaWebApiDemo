namespace NVShop.Data.FS.Linq.Query
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using Remotion.Linq;
	using Remotion.Linq.Clauses;
	using Remotion.Linq.Clauses.Expressions;
	using Remotion.Linq.Clauses.ResultOperators;

	public class FSQueryModelVisitor : QueryModelVisitorBase
	{
		public static readonly ParameterExpression QueryContextParameter = Expression.Parameter(typeof(IFSQueryContext), "queryContext");

		private readonly FSCommandQueryModelVisitor _fsCommandQueryModelVisitor;

		public FSQueryModelVisitor()
		{
			_fsCommandQueryModelVisitor = new FSCommandQueryModelVisitor(this);
		}

		public static FSCommand GenerateFSQuery(QueryModel queryModel)
        {
			var visitor = new FSQueryModelVisitor();

			visitor.VisitQueryModel(queryModel);

			return visitor.GetFSCommand();
        }


		/// <summary>
		///     Gets the expression that represents this query.
		/// </summary>
		public virtual Expression Expression { get; set; }


		public FSQueryPartsAggregator QueryParts { get; } = new FSQueryPartsAggregator();
        public int? Offset { get; private set; }
        public int? Limit { get; private set; }
		public bool IsCountQuery { get; private set; }
        public bool IsLongCountQuery { get; private set; }

        ///// <summary>
        /////     Gets the <see cref="ILinqOperatorProvider" /> being used for this query.
        ///// </summary>
        //public virtual ILinqOperatorProvider LinqOperatorProvider { get; private set; }

        public FSCommand GetFSCommand()
        {
            return new FSCommand(QueryParts.BuildFSWhere(), QueryParts.BuildFSOrderBy(), null, Offset, Limit)
            {
                IsCountQuery = IsCountQuery,
                IsLongCountQuery = IsLongCountQuery
            };
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
			Guard.ArgumentNotNull(() => queryModel);

			base.VisitQueryModel(queryModel);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            if (resultOperator is CountResultOperator)
            {
                IsCountQuery = true;
            }
            else if (resultOperator is LongCountResultOperator)
            {
                IsLongCountQuery = true;
            }
            else if (resultOperator is TakeResultOperator)
			{
				var count = ((TakeResultOperator)resultOperator).GetConstantCount();

				if (Limit == null || Limit > count)
				{
					Limit = count;
				}
			}
			else if (resultOperator is SkipResultOperator)
			{
				var count = ((SkipResultOperator)resultOperator).GetConstantCount();

				Offset = Offset != null
					? Offset + count
					: count;
			}
            else
            {
                throw new NotSupportedException(
                    "Only Count() result operator is showcased in this sample. Adding Sum, Min, Max is left to the reader.");
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
			Guard.ArgumentNotNull(() => fromClause);
			Guard.ArgumentNotNull(() => queryModel);

			var visitor = new FSQueryableExpressionVisitor(this, fromClause);

			Expression = visitor.Visit(fromClause.FromExpression);

			QueryParts.Merge(visitor.QueryParts);
			Offset = visitor.Offset;
			Limit = visitor.Limit;

            IsCountQuery = visitor.IsCountQuery;
            IsLongCountQuery = visitor.IsLongCountQuery;
        }
	
		public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
			Guard.ArgumentNotNull(() => selectClause);
			Guard.ArgumentNotNull(() => queryModel);

			if (selectClause.Selector.Type == Expression.Type.GetSequenceType() 
				&& selectClause.Selector is QuerySourceReferenceExpression)
			{
				return;
			}
			
			//_queryParts.SelectPart = GetFSExpression(selectClause.Selector);

			base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            QueryParts.AddWherePart(GetFSExpression(whereClause.Predicate));

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            QueryParts.AddOrderByPart(orderByClause.Orderings.Select(o => GetFSExpression(o.Expression)));

            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

		private string GetFSExpression(Expression expression)
        {
            return FSExpressionVisitor.GetFSExpression(expression);
        }
    }
}