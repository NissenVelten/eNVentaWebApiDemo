using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace NVShop.Data.FS.Linq.Query
{
	internal class FSCommandQueryModelVisitor : QueryModelVisitorBase
	{
		private readonly FSQueryModelVisitor _queryModelVisitor;

		public FSCommandQueryModelVisitor(FSQueryModelVisitor queryModelVisitor)
		{
			_queryModelVisitor = queryModelVisitor;
		}

		public FSCommand Command => new FSCommand("", "");

		public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
		{
			base.VisitMainFromClause(fromClause, queryModel);
		}

	}
}
