namespace NVShop.Data.NV.Repository
{
    using Model;

    using QueryBuilder;

    public partial interface INVArticleRepository : 
        INVQueryRepository<NVArticle, INVArticleQueryBuilder>, 
        INVReadRepository<NVArticle>
    {
	}
}