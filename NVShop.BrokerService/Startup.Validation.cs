using FluentValidation.WebApi;
using Owin;

namespace NVShop.BrokerService
{
	public partial class Startup
	{
        public static void ConfigureValidation(IAppBuilder app)
        {
            //HttpConfig.Filters.Add(new ValidateModelStateFilter());

            FluentValidationModelValidatorProvider.Configure(HttpConfig);
        }
	}
}