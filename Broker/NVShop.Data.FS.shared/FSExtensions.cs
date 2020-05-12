using NV.ERP.Navigator;
using NVShop.Data.NV.Model;

namespace NVShop.Data.FS
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using AutoMapper;

    using FrameworkSystems.FrameworkBase;
    using FrameworkSystems.FrameworkBase.Metadatatype;

    using FSGeneral.GlobalObjects;

    public static class FSExtensions
    {
        public static FSQueryHelper<TFSEntity> QueryHelper<TFSEntity>(this FSUtil util)
            where TFSEntity : class, IDevFrameworkDataObject
        {
            return new FSQueryHelper<TFSEntity>(util);
        }

        public static FSQueryList<TFSEntity> QueryList<TFSEntity>(this FSUtil util)
            where TFSEntity : class, IDevFrameworkDataObject
        {
            return new FSQueryList<TFSEntity>(util);
        }

        public static TFSEntity Create<TFSEntity>(this IFSGlobalObjects global)
            where TFSEntity : class, IDevFrameworkObject
        {
            var type = typeof(TFSEntity);
            var componentName = type.FullName;

            if (type.IsInterface)
            {
                componentName = $"{type.Namespace}.{type.Name.Substring(1)}";
            }

            return global.CreateComponent(componentName) as TFSEntity;
        }

        public static object Create(this IFSGlobalObjects global, Type type)
        {
            var componentName = type.FullName;

            if (type.IsInterface)
            {
                componentName = $"{type.Namespace}.{type.Name.Substring(1)}";
            }

            return global.CreateComponent(componentName);
        }

        public static bool Authenticate(NVIdentity identity)
        {
            try
            {
                var global = FSGlobalFactory.Get(identity);
                if (global == null)
                    return false;

                identity.IsAuthenticated = true;
                FSGlobalFactory.Put(global);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IFSGlobalObjects Authenticate(this IFSGlobalObjects global, NVIdentity identity)
        {
            cLoginFactory.Create(global).LoginServiceHost(identity.Name, identity.Password, identity.BusinessUnit);

            return global;
        }

        public static IFSGlobalObjects EnableCaching(this IFSGlobalObjects global)
        {
            global.ocGlobal.oCache.bEnabled = true;

            return global;
        }

        public static FSSystemGuid FSRowId(this IDevFrameworkDataObject fsEntity)
        {
            return (FSSystemGuid) fsEntity.GetProperty("ROWID");
        }

        public static string RowIdAsString(this IDevFrameworkDataObject fsEntity)
        {
            return fsEntity.FSRowId()
                .ToString();
        }

        public static long RowVersionAsLong(this IDevFrameworkDataObject fsEntity)
        {
            return (FSlong) fsEntity.GetProperty("ROWVERSION");
        }

        public static string PK(this IDevFrameworkDataObject fsEntity)
        {
            var pkProperties = new string[fsEntity.GetPKProperties()
                .Count];
            fsEntity.GetPKProperties()
                .CopyTo(pkProperties, 0);

            return string.Join("|", pkProperties.Select(pk => fsEntity.GetProperty(pk)
                .ToString()));
        }

        public static string IdentCommand(this IDevFrameworkDataObject fsEntity)
        {
			var selectClause = string.Format("SELECT {0} AS ROWIDCOL, {1} AS FSROWVERSION ",
				fsEntity.GetVirtualColumnNameOnPropertyName("ROWID"),
				fsEntity.GetVirtualColumnNameOnPropertyName("ROWVERSION"));

			return selectClause + fsEntity.QueryInfo.FromClause + fsEntity.QueryInfo.WhereClause + fsEntity.QueryInfo.OrderByClause;
        }


        public static TFSEntity MapTo<TFSEntity>(this object source, IMapper mapper, FSScope scope)
            where TFSEntity : class, IDevFrameworkObject
        {
            return mapper.Map(source, scope.Create<TFSEntity>());
        }

        public static TFSEntity MapTo<TSource, TFSEntity>(this TSource source, IMapper mapper, FSScope scope, Action<IMappingOperationOptions<TSource, TFSEntity>> opts)
            where TFSEntity : class, IDevFrameworkObject
        {
            var target = scope.Create<TFSEntity>();
            return mapper.Map(source, target, opts);
        }

        //public static IEnumerable<TFSEntity> MapTo<TFSEntity>(this IEnumerable<object> source, FSScope scope) 
        //    where TFSEntity : class, IDevFrameworkObject
        //{
        //    return Mapper.Map<IEnumerable<TFSEntity>>(source, opt => opt.ConstructServicesUsing(scope.Create));
        //}

        public static string Text<TTextColl>(this FSScope scope, int id, string defaultText = "")
        {
            return Text(scope, typeof(TTextColl), id, defaultText);
        }

        public static string Text(this FSScope scope, Type type, int id, string defaultText = "")
        {
            var global = scope.FSGlobalContext;

            var typeName = type.Name;
            var factoryTypeName = type.AssemblyQualifiedName.Replace(typeName, typeName.TrimStart('I') + "Factory");
            var mi = Type.GetType(factoryTypeName).GetMethod("GetText", new[] { global.FSGlobal.GetType(), id.GetType(), defaultText.GetType() });

            return mi.Invoke(null, new object[] { global.FSGlobal, id, defaultText }) as string;
        }

        private static string GetPropertyName<TFSEntity, TProp>(Expression<Func<TFSEntity, TProp>> property)
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