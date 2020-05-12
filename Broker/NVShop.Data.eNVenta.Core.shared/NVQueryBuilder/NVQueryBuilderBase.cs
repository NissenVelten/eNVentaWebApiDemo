namespace NVShop.Data.eNVenta.QueryBuilder
{
    using AutoMapper;

    using FrameworkSystems.FrameworkBase;

    using NVRepository;

    using NVShop.Data.FS;
    using NVShop.Data.NV;
    using NVShop.Data.NV.Model;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class NVQueryBuilderBase<TNVEntity, TFSEntity> : NVQueryBuilderBase<TNVEntity, string, TFSEntity>
        where TNVEntity : class
        where TFSEntity : class, IDevFrameworkDataObject
    {
        protected NVQueryBuilderBase(INVSelectRepository<TNVEntity> rep, IMapper mapper, FSUtil fsUtil)
            : base(rep, mapper, fsUtil) { }
    }

    public abstract class NVQueryBuilderBase<TNVEntity, TKey, TFSEntity> : INVQueryBuilder<TNVEntity, TKey>
        where TNVEntity : class
        where TFSEntity : class, IDevFrameworkDataObject
    {
        #region Private Fields

        private int? _offset;
        private int? _limit;
        private int? _loadSize;

        #endregion

        #region Constructors

        protected NVQueryBuilderBase(INVSelectRepository<TNVEntity> rep, IMapper mapper, FSUtil fsUtil)
        {
            Repository = rep;
            Mapper = mapper;
            FSUtil = fsUtil;
        }

        #endregion

        #region Properties

        protected INVSelectRepository<TNVEntity> Repository { get; }
        protected IMapper Mapper { get; }
        protected FSUtil FSUtil { get; }

        protected List<string> WhereConditions { get; } = new List<string>();
        protected List<string> OrderByFields { get; } = new List<string>();
        protected abstract IEnumerable<string> DefaultOrder();

        #endregion

        public virtual INVQueryBuilder<TNVEntity, TKey> Offset(int offset)
        {
            Guard.ArgumentNotNegative(() => offset);

            _offset = offset;

            return this;
        }

        public virtual INVQueryBuilder<TNVEntity, TKey> Limit(int limit)
        {

            Guard.ArgumentNotNegative(() => limit);

            _limit = limit;

            return this;
        }

        public virtual INVQueryBuilder<TNVEntity, TKey> Where(params string[] where)
        {
            WhereConditions.AddRange(where);
            return this;
        }

        protected string GetPropertyName<TProperty>(Expression<Func<TFSEntity, TProperty>> expr)
        {
            if (!( expr.Body is MemberExpression memberExpr ))
            {
                throw new ArgumentException($"Expression '{expr}' refers to a method, not a property.");
            }

            return $"[{memberExpr.Member.Name}]";
        }

        protected string OrderByExpr<TProperty>(Expression<Func<TFSEntity, TProperty>> orderByExpr) => GetPropertyName(orderByExpr);

        protected string OrderByDescendingExpr<TProperty>(Expression<Func<TFSEntity, TProperty>> orderByExpr) => GetPropertyName(orderByExpr) + " DESC";

        protected NVQueryBuilderBase<TNVEntity, TKey, TFSEntity> OrderBy<TProperty>(Expression<Func<TFSEntity, TProperty>> orderByExpr)
        {
            OrderByFields.Add(OrderByExpr(orderByExpr));

            return this;
        }

        protected NVQueryBuilderBase<TNVEntity, TKey, TFSEntity> OrderByDescending<TProperty>(Expression<Func<TFSEntity, TProperty>> orderByExpr)
        {
            OrderByFields.Add(OrderByDescendingExpr(orderByExpr));

            return this;
        }

        public virtual INVQueryBuilder<TNVEntity, TKey> OrderBy(params string[] orderBy)
        {
            OrderByFields.AddRange(orderBy);
            return this;
        }

        public INVQueryBuilder<TNVEntity, TKey> LoadMax(int loadSize)
        {
            _loadSize = loadSize;

            return this;
        }

        public virtual IQueryable<TNVEntity> Queryable()
        {
            return Repository.Queryable(BuildWhere(), BuildOrderBy());
        }

        public TNVEntity Find(TKey key)
        {
            return Repository.Find(key.ToString());
        }

        public Task<TNVEntity> FindAsync(TKey key)
        {
            return Task.FromResult(Repository.Find(key.ToString()));
        }

        public virtual IEnumerable<TNVEntity> Select()
        {
            return Repository.Select(BuildWhere(),
                BuildOrderBy(),
                _offset,
                _limit,
                _loadSize);
        }
        
        public virtual IEnumerable<TNVEntity> Select(IProgress<int> progress, CancellationToken ct)
        {
            return Repository.Select(BuildWhere(),
                BuildOrderBy(),
                _offset,
                _limit,
                _loadSize);
        }

        public virtual Task<IEnumerable<TNVEntity>> SelectAsync()
        {
            return SelectAsync(false);
        }

        public virtual Task<IEnumerable<TNVEntity>> SelectAsync(IProgress<int> progress, CancellationToken ct)
        {
            return SelectAsync(false, progress, ct);
        }

        public virtual Task<IEnumerable<TNVEntity>> SelectAsync(bool noCache, IProgress<int> progress = null, CancellationToken ct = default)
        {
            return Repository.SelectAsync(BuildWhere(),
                BuildOrderBy(),
                _offset,
                _limit,
                _loadSize);
        }

        public virtual TNVEntity FirstOrDefault()
        {
            return Repository.FirstOrDefault(BuildWhere());
        }

        public virtual Task<TNVEntity> FirstOrDefaultAsync()
        {
            return Repository.FirstOrDefaultAsync(BuildWhere());
        }

        public virtual int Count()
        {
            return Repository.Count(BuildWhere());
        }

        public virtual async Task<int> CountAsync()
        {
            return await Repository.CountAsync(BuildWhere());
        }

        public List<TNVEntity> ToList()
        {
            return Select()
                .ToList();
        }

        public async Task<List<TNVEntity>> ToListAsync()
        {
            var result = await Repository.SelectAsync(BuildWhere(),
                BuildOrderBy(),
                _offset,
                _limit,
                _loadSize);

            return result.ToList();
        }

        public IEnumerable<TDestination> MapTo<TDestination>()
        {
            return Select().Select(Mapper.Map<TDestination>);
        }

        public async Task<IEnumerable<TDestination>> MapToAsync<TDestination>()
        {
            var result = await Repository.SelectAsync(BuildWhere(),
                BuildOrderBy(),
                _offset,
                _limit,
                _loadSize);

            return result.Select(Mapper.Map<TDestination>);
        }

        public virtual FSQueryList<TFSEntity> BuildScopeConditions(FSQueryList<TFSEntity> conditions) => conditions;

        public virtual FSQueryList<TFSEntity> BuildConditions(FSQueryList<TFSEntity> conditions) => conditions;

        public string BuildWhere(NVQueryOperator op = NVQueryOperator.And)
        {
            var conditions = BuildScopeConditions(new FSQueryList<TFSEntity>(FSUtil));
            var filterConditions = BuildConditions(new FSQueryList<TFSEntity>(FSUtil));

            filterConditions.AddRange(WhereConditions);

            if (op == NVQueryOperator.And)
            {
                conditions.AddRange(filterConditions);
            }
            else
            {
                conditions.Block(x => filterConditions);
            }

            return string.Join(" AND ", conditions);
        }

        protected virtual string BuildOrderBy() {
            var fields = OrderByFields.Any() ? OrderByFields : DefaultOrder();

            return string.Join(",", fields);
        }

        public override string ToString()
        {
            return string.Format("nv.{0}.query-Where:{1}-OrderBy:{2}", typeof(TNVEntity).Name.ToLower(),
                BuildWhere(),
                BuildOrderBy());
        }
    }
}