namespace NVShop.Data.eNVenta.NVRepository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public interface INVSelectRepository<TNVEntity>
    {
        IQueryable<TNVEntity> Queryable(string where = "", string orderBy = "");

        TNVEntity Find(string key);

        IEnumerable<TNVEntity> Select(
            string where = "",
            string orderBy = "",
            int? pageIndex = null,
            int? pageSize = null,
            int? loadSize = null);

        Task<IEnumerable<TNVEntity>> SelectAsync(
            string where = "",
            string orderBy = "",
            int? pageIndex = null,
            int? pageSize = null,
            int? loadSize = null);

        bool Any(string where = "");

        Task<bool> AnyAsync(string where = "");

        TNVEntity FirstOrDefault(string where = "");

        Task<TNVEntity> FirstOrDefaultAsync(string where = "");

        int Count(string where = "");

        Task<int> CountAsync(string where = ""); 
    }
}