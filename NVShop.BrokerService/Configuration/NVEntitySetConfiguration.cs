using Microsoft.AspNet.OData.Builder;

using NVShop.Data.NV.Model;

namespace NVShop.BrokerService.Configuration
{
    public class NVEntitySetConfiguration<TNVEntity> : EntitySetConfiguration<TNVEntity>
		where TNVEntity : NVEntity
	{
        protected override EntityTypeConfiguration<TNVEntity> ConfigureCurrent(ODataModelBuilder builder)
		{
            var config = base.ConfigureCurrent(builder);

            config.HasKey(p => p.RowId);
            config.Property(p => p.RowId).IsOptional();
            config.Property(p => p.RowVersion).IsOptional();

            return config;
		}
	}
}