using System.Web.Http;

using Owin;

using Unity.AspNet.WebApi;

namespace NVShop.BrokerService
{
    public partial class Startup
	{
		/// <summary>
		/// Configures the web API.
		/// </summary>
		/// <param name="app">The application.</param>
		public void ConfigureWebApi(IAppBuilder app)
		{
			HttpConfig.DependencyResolver = new UnityDependencyResolver(UnityConfig.Container);

			// Web API routes
			HttpConfig.MapHttpAttributeRoutes();

			HttpConfig.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);

			app.UseWebApi(HttpConfig);
		}
	}
}