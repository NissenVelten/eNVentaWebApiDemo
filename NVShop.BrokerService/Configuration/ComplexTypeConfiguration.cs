using Microsoft.AspNet.OData.Builder;

using NVShop.Data.NV.Model;

namespace NVShop.BrokerService.Configuration
{
    public class CustomNVEntitySetConfiguration<TEntity> : NVEntitySetConfiguration<TEntity>
        where TEntity : NVEntity, INVCustomEntity
    {
        protected override EntityTypeConfiguration<TEntity> ConfigureCurrent(ODataModelBuilder builder)
        {
            var cfg = base.ConfigureCurrent(builder);

            cfg.HasDynamicProperties(x => x.CustomAttributes);

            return cfg;
        }
    }
}