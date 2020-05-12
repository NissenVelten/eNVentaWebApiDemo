using System;
using System.Linq;

namespace NVShop.Data.FS.Linq
{
    using System.Linq.Expressions;
	using System.Threading;
	using Visitors;

    internal class FSRepositoryQueryContext : IFSQueryContext
    {
        private readonly IFSRepository _fsRep;
        internal FSRepositoryQueryContext(IFSRepository fsRep)
        {
            _fsRep = fsRep;
        }

		/// <summary>
		///     Gets or sets the cancellation token.
		/// </summary>
		/// <value>
		///     The cancellation token.
		/// </value>
		public virtual CancellationToken CancellationToken { get; set; }

		// Executes the expression tree that is passed to it.
		public object Execute(Expression expression, bool isEnumerable)
        {
            // The expression must represent a query over the data source.
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

			string whereClause = "";

            // Find the call to Where() and get the lambda expression predicate.
            var whereExpression = new InnermostWhereFinder().GetInnermostWhere(expression);
			if (whereExpression != null)
			{
				var lambdaExpression = (LambdaExpression)((UnaryExpression)whereExpression.Arguments[1]).Operand;

				// Send the lambda expression through the partial evaluator.
				lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

				var builder = new FSWhereBuilder(lambdaExpression.Body);

				whereClause = builder.WhereClause;
			}

            var queryableResult = _fsRep.Get(whereClause).AsQueryable();

            return isEnumerable 
                ? queryableResult.Provider.CreateQuery(expression) 
                : queryableResult.Provider.Execute(expression);
        }

        private bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance,
            // expression is of type ConstantExpression, not MethodCallExpression.
            return (expression is MethodCallExpression);
        }
    }
}
