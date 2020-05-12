using Microsoft.AspNet.OData.Builder;
using Microsoft.Web.Http;

namespace NVShop.BrokerService.Configuration
{
    public class NVConfiguration : IModelConfiguration
    {
        protected virtual void ConfigureV1(ODataModelBuilder builder)
        {
            ConfigureCurrent(builder);
        }

        protected virtual ODataModelBuilder ConfigureCurrent(ODataModelBuilder builder)
        {
            var conventionModelBuilder = builder as ODataConventionModelBuilder;

            return builder;
        }

        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
        {
            //builder.Namespace = ServiceConstants.DefaultNamespace;

            switch (apiVersion.MajorVersion)
            {
                case 1:
                    ConfigureV1(builder);
                    break;
                default:
                    ConfigureCurrent(builder);
                    break;
            }
        }
    }
}