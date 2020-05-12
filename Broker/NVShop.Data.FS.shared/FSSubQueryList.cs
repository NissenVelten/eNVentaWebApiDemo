namespace NVShop.Data.FS
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FrameworkSystems.FrameworkBase;
    using FrameworkSystems.FrameworkDataProvider;

    public class FSSubQueryList<TFSSubEntity, TFSEntity> : List<string>
        where TFSSubEntity : class, IDevFrameworkDataObject
        where TFSEntity : class, IDevFrameworkDataObject
    {
        public FSSubQueryList(FSUtil util)
        {
            FSUtil = util;

            Entity = util.Create<TFSEntity>();
            SubEntity = util.Create<TFSSubEntity>();
        }

        protected FSUtil FSUtil { get; }
        protected TFSEntity Entity { get; }
        protected TFSSubEntity SubEntity { get; }

        public FSSubQueryList<TFSSubEntity, TFSEntity> Rel(
            Expression<Func<TFSSubEntity, object>> prop,
            Expression<Func<TFSEntity, object>> prop2)
        {
            Add($" {SubEntity.Prop(prop)} = {Entity.Prop(prop2)} ");

            return this;
        }

        public FSSubQueryList<TFSSubEntity, TFSEntity> Rel(
            Expression<Func<TFSSubEntity, object>> prop,
            dynamic value)
        {
            Add($" {SubEntity.Prop(prop)} = {DB.SqlString(value)} ");

            return this;
        }
    }
}