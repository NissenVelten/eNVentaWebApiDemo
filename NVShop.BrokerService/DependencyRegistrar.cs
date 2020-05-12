using NVShop.Core.Infrastructure.DependencyManagement;
using NVShop.Data.NV.Model;
using Unity;

namespace NVShop.BrokerService
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(IUnityContainer container)
        {
            container.RegisterType<ITypeFinder, AppDomainTypeFinder>();
            container.RegisterType<INVIdentityProvider, AuthNVIdentityProvider>();
        }

        public int Order => 0;
    }
}