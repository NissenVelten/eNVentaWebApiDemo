namespace NVShop.Data.FS
{
    using System;
    using System.Linq.Expressions;
    using FrameworkSystems.FrameworkBase;

    public class FSSubQueryHelper<TFSSubEntity, TFSEntity> : FSQueryHelper<TFSEntity>
        where TFSSubEntity : class, IDevFrameworkDataObject
        where TFSEntity : class, IDevFrameworkDataObject
    {
        public FSSubQueryHelper(FSUtil util)
            : base(util)
        {
            SubEntity = util.Create<TFSSubEntity>();
        }

        protected TFSSubEntity SubEntity { get; }

        public string Rel(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSSubEntity, object>> prop2)
        {
            return $" {Entity.Prop(prop)} = {SubEntity.Prop(prop2)}"; 
        }

        public string NotRel(Expression<Func<TFSEntity, object>> prop, Expression<Func<TFSSubEntity, object>> prop2)
        {
            return $" {Entity.Prop(prop)} != {SubEntity.Prop(prop2)}"; 
        }
    }
}