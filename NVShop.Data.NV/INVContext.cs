namespace NVShop.Data.NV
{
    using Model;

    using Repository;

    public partial interface INVContext
    {
        INVArticleRepository Article { get; }
        INVReadRepository<TNVEntity> Set<TNVEntity>() where TNVEntity : NVEntity;
    }
}