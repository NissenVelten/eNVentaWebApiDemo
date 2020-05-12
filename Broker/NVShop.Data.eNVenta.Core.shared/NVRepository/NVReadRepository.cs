using AutoMapper;

namespace NVShop.Data.eNVenta.NVRepository
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FrameworkSystems.FrameworkBase;
    using FS;

    using NVShop.Data.NV.Model;
    using NVShop.Data.NV.Repository;

    public abstract class NVReadRepository<TNVEntity, TFSEntity> : NVReadRepository<TNVEntity, TFSEntity, string>, INVReadRepository<TNVEntity>
        where TNVEntity : NVEntity, new()
        where TFSEntity : class, IDevFrameworkObject
    {
        protected NVReadRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil util) : base(rep, mapper, util)
        {
        }

        public override TNVEntity Find(string key)
        {
            var result = Repository.Find(key);
            return ToEntity(result);
        }

        public override async Task<TNVEntity> FindAsync(string key)
        {
            var result = await Repository.FindAsync(key);
            return Mapper.Map<TNVEntity>(result);
        }
    }

    public abstract class NVReadRepository<TNVEntity, TFSEntity, TKey> : 
        NVBaseRepository<TNVEntity, TFSEntity>, INVReadRepository<TNVEntity, TKey>
        where TNVEntity : class, new()
        where TFSEntity : class, IDevFrameworkObject
    {
        protected NVReadRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil util)
            : base(rep, mapper, util)
        {
        }
        
        protected string EntityKey => typeof(TNVEntity).Name;

        public IQueryable<TNVEntity> Queryable() => base.Queryable("", "");

        public abstract TNVEntity Find(TKey key);
        
        public IEnumerable<TNVEntity> Find(IEnumerable<TKey> keys)
        {
            if (keys == null) return Enumerable.Empty<TNVEntity>();

            return keys.Select(Find).Select(Mapper.Map<TNVEntity>);
        }

        public abstract Task<TNVEntity> FindAsync(TKey key);
        

        public async Task<IEnumerable<TNVEntity>> FindAsync(IEnumerable<TKey> keys)
        {
            var entities = new List<TNVEntity>();

            foreach (var key in keys)
            {
                var result = await FindAsync(key);
                if (result != null)
                {
                    entities.Add(Mapper.Map<TNVEntity>(result));
                }
            }

            return entities;
        }

        public virtual IEnumerable<TNVEntity> GetAll()
        {
            return Repository.Get().Select(Mapper.Map<TNVEntity>);
        }

        public virtual async Task<IEnumerable<TNVEntity>> GetAllAsync()
        {
            return (await Repository.GetAsync()).Select(Mapper.Map<TNVEntity>);
        }

        public virtual int Count() => Repository.Count();

        public virtual async Task<int> CountAsync() => await Repository.CountAsync();
    }
}