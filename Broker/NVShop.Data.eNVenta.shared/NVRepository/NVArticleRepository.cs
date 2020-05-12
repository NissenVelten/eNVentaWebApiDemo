using AutoMapper;
using NV.ERP.Base.Article;
using NV.ERP.MM.ECommerce.ECComponents;

namespace NVShop.Data.eNVenta.NVRepository
{

    using FS;

    using NV.Model;
    using NV.QueryBuilder;
    using NV.Repository;

    using QueryBuilder;

    public partial class NVArticleRepository : NVQueryReadRepository<NVArticle, IcdArticle, INVArticleQueryBuilder>, INVArticleRepository
    {
        public NVArticleRepository(IFSRepository<IcdArticle> rep, IMapper mapper, FSUtil util)
            : base(rep, mapper, util)
        {
        }

        public override INVArticleQueryBuilder Query() => new NVArticleQueryBuilder(this, Mapper, FSUtil);
    }
}