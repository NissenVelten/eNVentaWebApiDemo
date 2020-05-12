namespace NVShop.Data.NV.Repository
{
    public interface INVQueryRepository<TNVEntity, TKey, out TNVQueryBuilder>
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity, TKey>
    {
        TNVQueryBuilder Query();
    }

    public interface INVQueryRepository<TNVEntity, out TNVQueryBuilder> : INVQueryRepository<TNVEntity>
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity>
    {
        new TNVQueryBuilder Query();
    }

    public interface INVQueryRepository<TNVEntity>
    {
        INVQueryBuilder<TNVEntity> Query();
    }
}