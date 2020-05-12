namespace NVShop.Data.NV.Repository
{
    public interface INVQueryRepository<TNVEntity, TKey, out TNVQueryBuilder>
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity, TKey>
    {
        TNVQueryBuilder Query();
    }

    public interface INVQueryRepository<TNVEntity, out TNVQueryBuilder>
        where TNVQueryBuilder : INVQueryBuilder<TNVEntity>
    {
        TNVQueryBuilder Query();
    }

    public interface INVQueryRepository<TNVEntity>
    {
        INVQueryBuilder<TNVEntity> Query();
    }
}