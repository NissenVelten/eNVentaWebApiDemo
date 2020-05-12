using Remotion.Linq.Parsing;
using System.Linq.Expressions;

namespace NVShop.Data.FS.Linq.Visitors
{
	public class FSPagingVisitor : RelinqExpressionVisitor
	{
		public FSPagingVisitor()
		{
		}

		public int? Offset { get; set; }
		public int? Limit { get; set; }

		protected override Expression VisitBinary(BinaryExpression node)
		{
			return base.VisitBinary(node);
		}
	}
}
