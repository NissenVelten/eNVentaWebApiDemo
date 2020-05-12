namespace NVShop.Data.NV
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Model;

    public interface INVQueryBuilder<TNVEntity> : INVQueryBuilder<TNVEntity, string>
    {
        
    }

    public interface INVQueryBuilder<TNVEntity, in TKey>
    {
        INVQueryBuilder<TNVEntity, TKey> OrderBy(params string[] orderBy);
        INVQueryBuilder<TNVEntity, TKey> Offset(int offset);
		INVQueryBuilder<TNVEntity, TKey> Limit(int limit);
        INVQueryBuilder<TNVEntity, TKey> LoadMax(int loadSize);

        string BuildWhere(NVQueryOperator op = NVQueryOperator.And);

        TNVEntity Find(TKey key);
        Task<TNVEntity> FindAsync(TKey key);

        IQueryable<TNVEntity> Queryable();
        
        IEnumerable<TNVEntity> Select();
        IEnumerable<TNVEntity> Select(IProgress<int> progress, CancellationToken ct);

        Task<IEnumerable<TNVEntity>> SelectAsync();
        Task<IEnumerable<TNVEntity>> SelectAsync(IProgress<int> progress, CancellationToken ct);
        
        List<TNVEntity> ToList();
        Task<List<TNVEntity>> ToListAsync();

        IEnumerable<TDestination> MapTo<TDestination>();
        Task<IEnumerable<TDestination>> MapToAsync<TDestination>();


        TNVEntity FirstOrDefault();
        Task<TNVEntity> FirstOrDefaultAsync();

        int Count();
        Task<int> CountAsync();
    }
}