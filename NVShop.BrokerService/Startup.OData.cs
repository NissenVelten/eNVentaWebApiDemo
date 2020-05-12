using System.Web.Http;

using Microsoft.AspNet.OData.Batch;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.Web.Http;
using NVShop.BrokerService.Configuration;
using NVShop.Data.NV.Model;

using Owin;

namespace NVShop.BrokerService
{
    using Microsoft.AspNet.OData.Formatter;
    using Microsoft.AspNet.OData.Formatter.Deserialization;
    using Microsoft.OData.Edm;

    public partial class Startup
	{
		/// <summary>
		/// Configures the o data.
		/// </summary>
		/// <param name="app">The application.</param>
		public static void ConfigureOData(IAppBuilder app)
		{
			//HttpConfig.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			// Web-API-Konfiguration und -Dienste
			HttpConfig.Select()
                .Filter()
                .OrderBy()
				.MaxTop(null)
				.Count();

            var httpServer = new HttpServer(HttpConfig);

            var modelBuilder = new VersionedODataModelBuilder(HttpConfig)
			{
				ModelBuilderFactory = () => new ODataConventionModelBuilder(),
				ModelConfigurations =
				{
                    new NVEntitySetConfiguration<NVArticle>(),
                    new NVConfiguration()
                }
			};

            HttpConfig.MapVersionedODataRoutes("odata", "", modelBuilder.GetEdmModels(), new DefaultODataBatchHandler(httpServer));
		}
		
		private static void ConfigureODataServices(IContainerBuilder builder)
		{
			builder.AddService(ServiceLifetime.Singleton, sp => new ODataUriResolver());
		}
	}
}