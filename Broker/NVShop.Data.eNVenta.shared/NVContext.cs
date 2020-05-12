using System.Web.Mvc;

namespace NVShop.Data.eNVenta
{
    using NVShop.Data.eNVenta.NVRepository;
    using NVShop.Data.NV;
    using NVShop.Data.NV.Model;
    using NVShop.Data.NV.Repository;

    using System;

    public partial class NVContext : INVContext
    {
        private readonly Lazy<NVArticleRepository> _articleRep;

        public NVContext(
            Lazy<NVArticleRepository> articleRep
        )
        {
            _articleRep = articleRep;
        }

        public INVArticleRepository Article => _articleRep.Value;

        public INVReadRepository<TNVEntity> Set<TNVEntity>() where TNVEntity : NVEntity
        {
            return DependencyResolver.Current.GetService<INVReadRepository<TNVEntity>>();
        }
    }
}