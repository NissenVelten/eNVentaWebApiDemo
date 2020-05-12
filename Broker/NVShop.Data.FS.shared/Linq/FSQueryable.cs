namespace NVShop.Data.FS.Linq
{
	using NVShop.Data.FS.Linq.Query;
	using Remotion.Linq;
	using Remotion.Linq.Parsing.Structure;
	using System.Linq;
	using System.Linq.Expressions;

	internal class FSQueryable<TFSEntity> : QueryableBase<TFSEntity>
    {
		private static IQueryExecutor CreateExecutor(IFSRepository<TFSEntity> rep)
		{
			return new FSQueryExecutor<TFSEntity>(rep);
		}

		private static IQueryExecutor CreateExecutor(IFSRepository<TFSEntity> rep, FSCommand command)
		{
			return new FSQueryExecutor<TFSEntity>(rep, command);
		}

		public FSQueryable(IFSRepository<TFSEntity> rep, string whereBase, string orderByBase) : base(QueryParser.CreateDefault(), CreateExecutor(rep, new FSCommand(whereBase, orderByBase)))
		{
		}

		public FSQueryable(IFSRepository<TFSEntity> rep) : base(QueryParser.CreateDefault(), CreateExecutor(rep))
        {
		}

		public FSQueryable(IQueryParser parser, IQueryExecutor executor) : base(parser, executor)
		{

		}

		public FSQueryable(IQueryProvider provider, Expression expression) : base(provider, expression) { }
	}
}