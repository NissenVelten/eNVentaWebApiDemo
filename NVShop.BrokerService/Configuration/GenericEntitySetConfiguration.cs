namespace NVShop.BrokerService.Configuration
{
    using Microsoft.AspNet.OData.Builder;

    using System;
    using System.Linq.Expressions;

    public class EntitySetConfiguration<TEntity, TKey> : EntitySetConfiguration<TEntity>
        where TEntity : class
	{
        private readonly Expression<Func<TEntity, TKey>> _key;
        private readonly Action<EntityTypeConfiguration<TEntity>> _config;

        public EntitySetConfiguration(Action<EntityTypeConfiguration<TEntity>> config) : base()
        {
            _config = config;
        }

        public EntitySetConfiguration(Expression<Func<TEntity,TKey>> key) : base()
        {
            _key = key;
        }

        protected override EntityTypeConfiguration<TEntity> ConfigureCurrent(ODataModelBuilder builder)
        {
            var config = base.ConfigureCurrent(builder);

            if (_key != null) { config.HasKey(_key); }

            _config?.Invoke(config);

            return config;
        }
    }
}