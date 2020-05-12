namespace NVShop.BrokerService
{
    using Owin;
    using System.Web.Http;
    using Unity.AspNet.WebApi;

    public partial class Startup
    {
        public static void ConfigureUnity(IAppBuilder app)
        {
            // Register Unity as MVC Default Resolver
            // Register Unity as WepApi Default Resolver
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(UnityConfig.Container);
        }
    }
}