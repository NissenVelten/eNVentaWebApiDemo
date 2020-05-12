using AutoMapper;

namespace NVShop.Data.eNVenta.NVRepository
{
    using FrameworkSystems.FrameworkBase;
    using NVShop.Data.FS;
    using NVShop.Data.NV;
    using NVShop.Data.NV.Model;
    using NVShop.Data.NV.Repository;

    public abstract class NVQueryReadRepository<TNVEntity, TFSEntity, TNVQueryBuilder> : NVReadRepository<TNVEntity, TFSEntity>, 
        INVQueryRepository<TNVEntity, TNVQueryBuilder>
        where TNVEntity : NVEntity, new()
        where TFSEntity : class, IDevFrameworkObject
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity>
    {
        protected NVQueryReadRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil util) : base(rep, mapper, util)
        {
        }

        public abstract TNVQueryBuilder Query();
    }

    public abstract class NVQueryReadRepository<TNVEntity, TFSEntity, TKey, TNVQueryBuilder> : 
        NVReadRepository<TNVEntity, TFSEntity, TKey>, 
        INVQueryRepository<TNVEntity, TKey, TNVQueryBuilder>
        where TNVEntity : class, new()
        where TFSEntity : class, IDevFrameworkObject
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity, TKey>
    {
        protected NVQueryReadRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil util)
            : base(rep, mapper, util)
        {
        }

        public abstract TNVQueryBuilder Query();
    }
}