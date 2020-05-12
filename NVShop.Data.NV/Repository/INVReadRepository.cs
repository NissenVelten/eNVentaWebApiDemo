namespace NVShop.Data.NV.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NVShop.Data.NV.Model;

    public partial interface INVReadRepository<TNVEntity> : INVReadRepository<TNVEntity, string>
        where TNVEntity : NVEntity
    {
    }

    public partial interface INVReadRepository<TNVEntity, in TKey>
    {
        IQueryable<TNVEntity> Queryable();

        TNVEntity Find(TKey key);
        Task<TNVEntity> FindAsync(TKey key);

        IEnumerable<TNVEntity> Find(IEnumerable<TKey> keys);
        Task<IEnumerable<TNVEntity>> FindAsync(IEnumerable<TKey> keys);

        IEnumerable<TNVEntity> GetAll();
        Task<IEnumerable<TNVEntity>> GetAllAsync();

        int Count();
		Task<int> CountAsync();
    }
}