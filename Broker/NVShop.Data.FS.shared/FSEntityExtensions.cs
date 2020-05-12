using System;
using System.Linq.Expressions;

using FrameworkSystems.FrameworkBase;

namespace NVShop.Data.FS
{
    public static class FSEntityExtensions
    {
        public static string Table<TFSEntity>(this TFSEntity entity) 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            return entity.GetTableName();
        }

        public static string Table<TFSEntity, TFSProperty>(this TFSEntity entity, Expression<Func<TFSEntity, TFSProperty>> property)
            where TFSEntity : class, IDevFrameworkDataObject
        {
            if (property == null)
            {
                return entity.GetTableName();
            }

            return entity.GetTableName(entity.GetPropertyName(property));
        }

        public static string Prop<TFSEntity, TProp>(this TFSEntity entity, Expression<Func<TFSEntity, TProp>> property) 
            where TFSEntity : IDevFrameworkDataObject
        {
            var name = entity.GetPropertyName(property);

            return entity.GetVirtualColumnNameOnPropertyName(name);
        }

        internal static string GetPropertyName<TFSEntity>(this TFSEntity entity, Expression<Func<TFSEntity, object>> property)
        {
            var member = property.Body as MemberExpression;
            if (member == null)
            {
                if (property.Body is UnaryExpression unary)
                {
                    member = unary.Operand as MemberExpression;
                }
            }

            return member?.Member.Name;
        }

        internal static string GetPropertyName<TFSEntity, TProp>(this TFSEntity entity, Expression<Func<TFSEntity, TProp>> property)
        {
            var member = property.Body as MemberExpression;
            if (member == null)
            {
                if (property.Body is UnaryExpression unary)
                {
                    member = unary.Operand as MemberExpression;
                }
            }

            return member?.Member.Name;
        }
    }
}
