namespace NVShop.Data.FS
{
    using FrameworkSystems.FrameworkBase;

    using NVShop.Data.NV.Model;

    using System;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    public partial class FSUtil
    {
        private readonly INVIdentityProvider _provider;

        public FSUtil(INVIdentityProvider provider)
        {
            _provider = provider;
        }

        public static bool Authenticate(NVIdentity identity)
        {
            var global = FSGlobalFactory.Get(identity);
            if (global != null)
            {
                identity.IsAuthenticated = true;

                FSGlobalFactory.Put(global);

                return true;
            }
            
            return false;
        }

        public string GetWhereBranch<TFSEntity>(TFSEntity entity, string prefix = "AND", string alias = null, bool native = false)
            where TFSEntity : class, IDevFrameworkDataObject
        {
            alias = alias ?? entity.GetTableName();
            using (var scope = Scope())
            {
                var condition = scope.FSGlobalContext.FSGlobal.ocGlobal.GetWhereBranch(prefix, entity, alias);

                if (native)
                {
                    condition = Regex.Replace(condition, "«str:(.*)»", "$1");
                }

                return condition;
            }
        }

        public string GetWhereBranch(string tableName, string prefix = "AND", string alias = null, bool native = false)
        {
            using (var scope = Scope())
            {
                var condition = scope.FSGlobalContext.FSGlobal.ocGlobal.GetBranchConditionForTable(tableName, alias ?? tableName);

                if (native)
                {
                    condition = Regex.Replace(condition, "«str:(.*)»", "'$1'");
                }

                if (!string.IsNullOrEmpty(condition))
                {
                    return $" {prefix} {condition}";
                }
            }
            return string.Empty;
        }

        public string GetBranchKey(string tableName, string alias = null)
        {
            using (var scope = Scope())
            {
                return scope.FSGlobalContext.FSGlobal.ocGlobal.GetBranchConditionForTable(tableName, alias ?? tableName);
            }
        }

        public bool IsServerFile(string path)
        {
            using (var scope = Scope())
            {
                return scope.FSGlobalContext.FSGlobal.ocGlobal.oFileManager.IsServerPath(path);
            }
        }

        public byte[] GetServerFileBytes(string path)
        {
            using (var scope = Scope())
            {
                return scope.FSGlobalContext.FSGlobal.ocGlobal.oFileManager.GetFileAsByteArray(path);
            }
        }

        public Stream GetServerFileStream(string path)
        {
            using (var scope = Scope())
            {
                return scope.FSGlobalContext.FSGlobal.ocGlobal.oFileManager.GetFile(path)?.GetStream();
            }
        }

        public string GetFileName(string serverFile)
        {
            using (var scope = Scope())
            {
                return scope.FSGlobalContext.FSGlobal.ocGlobal.oFileManager.GetFileName(serverFile);
            }
        }

        public string GetMimeType(string path)
        {
            using (var scope = Scope())
            {
                var fileManager = scope.FSGlobalContext.FSGlobal.ocGlobal.oFileManager;
                return fileManager.GetMimeTypeByFileName(path);
            }
        }

        public FSScope Scope()
        {
            return new FSScope(_provider.Get);
        }

        public FSQueryHelper<TFSEntity> QueryHelper<TFSEntity>() 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            return new FSQueryHelper<TFSEntity>(this);
        }

        public  FSSubQuery<TFSEntity, TFSSubEntity> SubQuery<TFSEntity, TFSSubEntity>() 
            where TFSEntity : class, IDevFrameworkDataObject 
            where TFSSubEntity : class, IDevFrameworkDataObject
        {
            return new FSSubQuery<TFSEntity, TFSSubEntity>(this);
        }

        public string Build<TFSEntity>(string where, params Func<FSQueryHelper<TFSEntity>, object>[] args) 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            var query = new FSQueryHelper<TFSEntity>(this);

            var paramList = args.Select(arg => arg(query))
                .ToArray();

            return string.Format(where, paramList);
        }
        
        public string Build<TFSEntity>(Func<FSQueryHelper<TFSEntity>, string> builder) 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            return builder(new FSQueryHelper<TFSEntity>(this));
        }

        public TFSEntity Create<TFSEntity>() 
            where TFSEntity : class, IDevFrameworkObject
        {
            using (var scope = Scope())
            {
                return Create<TFSEntity>(scope);
            }
        }

        public TFSEntity Create<TFSEntity>(FSScope scope)
            where TFSEntity : class, IDevFrameworkObject => scope.Create<TFSEntity>();

        public object Create(Type type)
        {
            using (var scope = Scope())
            {
                return scope.FSGlobalContext.Create(type);
            }
        }

        public string Prop<TFSEntity>(Expression<Func<TFSEntity, object>> property) 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            var entity = Create<TFSEntity>();
            var name = entity.GetPropertyName(property);
         
            return entity.GetVirtualColumnNameOnPropertyName(name);
        }

        public string Prop<TFSEntity, TProp>(Expression<Func<TFSEntity, TProp>> property) 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            var entity = Create<TFSEntity>();
            var name = entity.GetPropertyName(property);
            
            return entity.GetVirtualColumnNameOnPropertyName(name);
        }
       
        public string Table<TFSEntity>() where TFSEntity : class, IDevFrameworkDataObject
        {
            var entity = Create<TFSEntity>();

            return entity.Table();
        }

        public string BranchCondition<TFSEntity>(TFSEntity entity, bool native = false) 
            where TFSEntity : class, IDevFrameworkDataObject
        {
            return GetWhereBranch(entity, native: native);
        }

        public string BranchCondition<TFSEntity, TProp>(TFSEntity entity, Expression<Func<TFSEntity, TProp>> property, string alias = null, bool native = false) 
            where TFSEntity : IDevFrameworkDataObject
        {
            var tableName = entity.GetTableName(entity.GetPropertyName(property));
            return GetWhereBranch(tableName, alias: alias, native: native);
        }

        public string BranchCondition(string tableName, string alias = null, bool native = false)
        {
            return GetWhereBranch(tableName, alias: alias, native: native);
        }

        public string ExtendBranch<TFSEntity>(TFSEntity entity) where TFSEntity : class, IDevFrameworkDataObject
        {
            using (var scope = Scope())
            {
                var whereBranch = string.Empty;

                scope.FSGlobalContext.FSGlobal.ocGlobal.oBusinessUnit.ExtendLoadCondition(ref whereBranch, entity);

                return whereBranch;
            }
        }
    }
}