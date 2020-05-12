using AutoMapper;

namespace NVShop.Data.eNVenta.NVRepository
{
    using FrameworkSystems.FrameworkBase;

    using FS;

    using NV.Model;
    using NVShop.Data.NV;
    using NVShop.Data.NV.Repository;

    public abstract class NVQueryCrudRepository<TNVEntity, TFSEntity, TNVQueryBuilder> : 
        NVCrudRepository<TNVEntity, TFSEntity>,
        INVQueryRepository<TNVEntity, TNVQueryBuilder>
        where TNVEntity : NVEntity, new()
        where TFSEntity : class, IDevFrameworkDataObject
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity>
    {
        protected NVQueryCrudRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil util)
            : base(rep, mapper, util) { }

        public abstract TNVQueryBuilder Query();
        INVQueryBuilder<TNVEntity> INVQueryRepository<TNVEntity>.Query() => Query();
    }
}