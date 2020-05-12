using Microsoft.AspNet.OData.Builder;
using Microsoft.Web.Http;

namespace NVShop.BrokerService.Configuration
{
    public class EntitySetConfiguration<TEntity> : IModelConfiguration
        where TEntity : class
	{
        /// <summary>
        /// Gets the name of the entity set.
        /// </summary>
        /// <value>
        /// The name of the entity set.
        /// </value>
        protected virtual string EntitySetName => typeof(TEntity).Name;

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        protected virtual string Namespace => ServiceConstants.DefaultNamespace;

        /// <summary>
        /// Configures the v1.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected virtual void ConfigureV1(ODataModelBuilder builder)
		{
			ConfigureCurrent(builder);
		}


		protected virtual EntityTypeConfiguration<TEntity> ConfigureCurrent(ODataModelBuilder builder)
		{
			var config = builder.EntitySet<TEntity>(EntitySetName).EntityType;

			return config;
		}

		public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
		{
            builder.Namespace = Namespace;

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