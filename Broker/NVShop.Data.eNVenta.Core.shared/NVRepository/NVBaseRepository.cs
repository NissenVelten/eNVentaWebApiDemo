using AutoMapper;

namespace NVShop.Data.eNVenta.NVRepository
{
    using AutoMapper.QueryableExtensions;

    using FS;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract partial class NVBaseRepository<TNVEntity, TFSEntity> : INVSelectRepository<TNVEntity>
        where TNVEntity : class, new()
        where TFSEntity : class
    {
        protected IFSRepository<TFSEntity> Repository { get; }
        public IMapper Mapper { get; }
        protected FSUtil FSUtil { get; }

        protected NVBaseRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil fsUtil)
        {
            Repository = rep;
            Mapper = mapper;
            FSUtil = fsUtil;
        }

        public virtual IQueryable<TNVEntity> Queryable(
            string where = "",
            string orderBy = ""
        )
        {
            return Repository.Queryable(where, orderBy)
                .UseAsDataSource(Mapper)
                .For<TNVEntity>();
        }

        public virtual IEnumerable<TNVEntity> Select(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null
        )
        {
            var result = Repository.Get(where, orderBy, offset, limit, loadSize);

            return result.Select(Mapper.Map<TFSEntity, TNVEntity>);
        }

        public virtual async Task<IEnumerable<TNVEntity>> SelectAsync(
            string where = "",
            string orderBy = "",
		    int? offset = null,
			int? limit = null,
			int? loadSize = null)
        {
            var result = await Repository.GetAsync(where, orderBy, offset, limit, loadSize);

            return result.Select(Mapper.Map<TFSEntity, TNVEntity>);
        }

        public object Create()
        {
            return new TNVEntity();
        }

        public TNVEntity Find(string key)
        {
            var result = Repository.Find(key);

            return ToEntity(result);
        }

        public bool Any(string where = "")
        {
            return Repository.Exists(where);
        }

        public Task<bool> AnyAsync(string where = "")
        {
            return Repository.ExistsAsync(where);
        }

        public TNVEntity FirstOrDefault(string where = "")
        {
            var result = Repository.FirstOrDefault(where);

            return ToEntity(result);
        }

        public async Task<TNVEntity> FirstOrDefaultAsync(string where = "")
        {
            var result = await Repository.FirstOrDefaultAsync(where);

            return ToEntity(result);
        }

        public virtual int Count(string where = "")
        {
            return Repository.Count(where);
        }

        public virtual async Task<int> CountAsync(string where = "")
        {
            return await Repository.CountAsync(where);
        }

        protected virtual TNVEntity ToEntity(TFSEntity source) => Mapper.Map<TNVEntity>(source);
    }

    
}