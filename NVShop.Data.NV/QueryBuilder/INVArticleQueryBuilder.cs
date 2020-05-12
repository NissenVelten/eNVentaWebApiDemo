namespace NVShop.Data.NV.QueryBuilder
{
    using Model;

    public partial interface INVArticleQueryBuilder : INVQueryBuilder<NVArticle>, INVNestedQueryBuilder<INVArticleQueryBuilder>
    {
        INVArticleQueryBuilder ByArticleId(string articleId);
        INVArticleQueryBuilder ByEan(string ean);
        INVArticleQueryBuilder ByGtin(string gtin);
        INVArticleQueryBuilder LikeName(string query);
    }
}