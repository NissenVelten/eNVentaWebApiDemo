using Owin;
using Swashbuckle.Application;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;

namespace NVShop.BrokerService
{
	public partial class Startup
	{
		public static void ConfigureSwagger(IAppBuilder app)
		{
			//var apiExplorer = HttpConfig.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
			var apiExplorer = HttpConfig.AddODataApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
            });

			HttpConfig.EnableSwagger(
				"{apiVersion}/swagger",
				swagger =>
				{
					swagger.MultipleApiVersions(
						(apiDescription, version) => apiDescription.GetGroupName() == version,
						info =>
						{
							foreach (var group in apiExplorer.ApiDescriptions)
							{
								var description = "eGate DataServices to Connect to eNVenta ERP.";

								if (group.IsDeprecated)
								{
									description += " (Deprecated)";
								}

								info.Version(group.Name, $"eNVenta eGate DataServices API {group.ApiVersion}")
								   .Contact(c => c.Name("Nissen & Velten Software GmbH")
								   .Email("support@nissen-velten.de"))
                                   .Description(description);
							}
						});

					swagger.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

					swagger.OperationFilter<SwaggerDefaultValues>();

					//swagger.IncludeXmlComments(XmlCommentsFilePath);
				})
				.EnableSwaggerUi(swagger => swagger.EnableDiscoveryUrlSelector());
		}

		static string XmlCommentsFilePath
		{
			get
			{
				var basePath = AppDomain.CurrentDomain.RelativeSearchPath;
				var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
				return Path.Combine(basePath, fileName);
			}
		}
	}
}