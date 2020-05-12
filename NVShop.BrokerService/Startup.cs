using Microsoft.Web.Http.Versioning;
using Owin;
using System;
using System.Globalization;
using System.Threading;
using System.Web.Http;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(NVShop.BrokerService.Startup))]

namespace NVShop.BrokerService
{
	public partial class Startup
	{

		/// <summary>
		/// The HTTP configuration
		/// </summary>
		public static HttpConfiguration HttpConfig = new HttpConfiguration();

        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
		{
			app.Use((context, next) =>
			{
				var lang = context.Request.Query.Get("lang");
				var currentLang = "en";
				if (!string.IsNullOrEmpty(lang))
				{
					currentLang = lang;
				}

                Thread.CurrentThread.CurrentUICulture = new CultureInfo(currentLang);

                return next.Invoke();
			});

			// Enable Api Versioning
			HttpConfig.AddApiVersioning(cfg =>
			{
				cfg.ReportApiVersions = true;
				cfg.AssumeDefaultVersionWhenUnspecified = true;
				cfg.ApiVersionReader = ApiVersionReader.Combine(
					new QueryStringApiVersionReader(),
					new HeaderApiVersionReader("api-version", "x-ms-version")
				);

				cfg.DefaultApiVersion = ApiVersions.V1;
			});

			// Enable WebApi Compression
			HttpConfig.MessageHandlers.Insert(0, new ServerCompressionHandler(4096, new GZipCompressor(), new DeflateCompressor()));

            ConfigureUnity(app);
            ConfigureAutoMapper(app);
            ConfigureValidation(app);
            ConfigureOData(app);
			ConfigureWebApi(app);
            ConfigureSwagger(app);
        }
	}
}