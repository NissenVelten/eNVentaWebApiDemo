namespace NVShop.Data.eNVenta
{

    using NVRepository;

    using NVShop.Core.Infrastructure.DependencyManagement;
    using NVShop.Data.FS;
    using NVShop.Data.NV;
    using NVShop.Data.NV.Repository;
    using NVShop.Data.NV.Services;
    using NVShop.Data.eNVenta.Services;

    using Unity;

    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType(typeof(IFSRepository<>), typeof(FSRepository<>));

            container.RegisterType<INVArticleRepository, NVArticleRepository>();
            container.RegisterType<IECAuthService, ECAuthService>();

            container.RegisterType<INVContext, NVContext>();
        }

        public int Order => -150;
    }
}