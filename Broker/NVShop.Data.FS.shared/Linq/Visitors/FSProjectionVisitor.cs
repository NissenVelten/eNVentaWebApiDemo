using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using System;
using System.Linq.Expressions;

namespace NVShop.Data.FS.Linq.Visitors
{
	// Builds an in-memory projector for a given select expression. Uses ResultObjectMapping to resolve references to query sources. Does not support 
	// sub-queries.
	public class FSProjectionVisitor<TSource, TTarget> : RelinqExpressionVisitor
	{
		// Call this method to get the projector. T is the type of the result (after the projection).
		public static Func<TSource, TTarget> BuildProjector(Expression selectExpression)
		{
			// This is the parameter of the delegat we're building. It's the ResultObjectMapping, which holds all the input data needed for the projection.
			var parameter = Expression.Parameter(typeof(TSource), "x");

			// The visitor gives us the projector's body. It simply replaces all QuerySourceReferenceExpressions with calls to ResultObjectMapping.GetObject<T>().
			var visitor = new FSProjectionVisitor<TSource, TTarget>(parameter);
			var body = visitor.Visit(selectExpression);

			// Construct a LambdaExpression from parameter and body and compile it into a delegate.
			return Expression.Lambda<Func<TSource, TTarget>>(body, parameter)
                .Compile();
		}

		private readonly ParameterExpression _resultItemParameter;

		private FSProjectionVisitor(ParameterExpression resultItemParameter)
		{
			_resultItemParameter = resultItemParameter;
		}

		protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
		{
			return _resultItemParameter;
		}
	}
}
