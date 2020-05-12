using NVShop.Data.NV.Model;

namespace NVShop.Data.FS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FrameworkSystems.FrameworkBase;
    using FrameworkSystems.FrameworkBase.Metadatatype;
    using FrameworkSystems.FrameworkDataClient;
    using FrameworkSystems.FrameworkDataProvider;
    using FrameworkSystems.FrameworkExceptions;

    public partial class FSDataContext<T> : IDisposable
        where T : class, IDevFrameworkDataObject
    {
        private FSGlobalContext _global;
        //private readonly Func<FSGlobal, T> _factory;
        public FSDataContext(INVIdentityProvider provider)
        {
            _global = FSGlobalFactory.Get(provider.Get);
        }

        /// <summary>
        ///     Returns all record matching to the condition
        /// </summary>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        internal IEnumerable<T> GetFetchNext(string where = "", string orderBy = "", int? offset = null, int? limit = null)
        {
            var options = new FrameworkQueryOptions
            {
                Condition = where,
                OrderBy = orderBy,
                Offset = offset,
                Count = limit
            };

            return Create().GetFetchNext(options).Cast<T>();
        }

        internal IEnumerable<Tuple<FSSystemGuid, long>> GetFetchNextPK(string where = "", string orderBy = "")
        {
            var o = Create();
            var dev = o as DevFrameworkObject;

            var havingClause = string.Empty;
            _global.FSGlobal.ocGlobal.GlobalOnBeforeLoad(dev, ref where, ref havingClause, ref orderBy);
            using (var conn = _global.CreateConnection(o.Connection))
            using (var cmd = CreatePKCommand(conn, o, where, orderBy))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var rowId = FSSystemGuid.Null;
                    rowId.SetFromDB(reader["ROWIDCOL"]);
                    var rowVersion = FSlong.Null;
                    rowVersion.SetFromDB(reader["FSROWVERSION"]);

                    yield return new Tuple<FSSystemGuid, long>(rowId, rowVersion);
                }
            }
        }

        private FrameworkDataCommand CreatePKCommand(FrameworkDataConnection conn, T o, string where = null, string orderBy = null)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = o.IdentCommand();
            cmd.AdditionalWhereClause = where != null
                ? DevFrameworkBaseObject.ConvertLoadCondition(o.GetVirtualColumnNameOnPropertyName, where, false)
                : null;
            cmd.ReplacingOrderByClause = where != null
                ? DevFrameworkBaseObject.ConvertLoadCondition(o.GetVirtualColumnNameOnPropertyName, orderBy, false)
                : null;
            return cmd;
        }

        internal T GetByRowId(string rowId)
        {
            if (!Guid.TryParse(rowId, out var guid))
            {
                return null;
            }

            var o = Create();

            var entityIdColumn = o.GetVirtualColumnNameOnPropertyName("ROWID");
            var where = entityIdColumn + DB.SqlStringRowID("=", FSSystemGuid.FromObject(guid));

            if (o.Load(where) != 1)
            {
                return null;
            }

            return o;
        }

        internal IEnumerable<T> GetByRowId(IEnumerable<string> rowIds)
        {
            var o = Create();

            var entityIdColumn = o.GetVirtualColumnNameOnPropertyName("ROWID");

            var where = rowIds.Batch(1000)
                .Select(block => $"{entityIdColumn} IN ({block.Join(",", x => $"'{DB.SqlStringRowID(FSSystemGuid.FromObject(x))}'")})")
                .Join(" OR ");

            return o.GetFetchNext(where).Cast<T>();
        }

        internal T GetByRowId(FSSystemGuid rowId)
        {
            var o = Create();

            var entityIdColumn = o.GetVirtualColumnNameOnPropertyName("ROWID");
            var where = entityIdColumn + DB.SqlStringRowID("=", rowId);

            if (o.Load(where) != 1)
            {
                return null;
            }

            return o;
        }

        internal async Task<T> GetByRowIdAsync(Guid rowId)
        {
            var o = Create();

            var entityIdColumn = o.GetVirtualColumnNameOnPropertyName("ROWID");
            var where = entityIdColumn + DB.SqlStringRowID("=", FSSystemGuid.FromObject(rowId));

            var loadResult = await Task.Run(() => o.Load(where));
            if (loadResult != 1)
            {
                return null;
            }

            return o;
        }


        /// <summary>
        /// Gets the by row identifier asynchronous.
        /// </summary>
        /// <param name="rowIds">The row ids.</param>
        /// <returns></returns>
        internal async Task<IEnumerable<T>> GetByRowIdAsync(IEnumerable<string> rowIds)
        {
            var o = Create();

            var entityIdColumn = o.GetVirtualColumnNameOnPropertyName("ROWID");

            var where = rowIds.Batch(1000)
                .Select(block => $"{entityIdColumn} IN ({block.Join(",", x => $"'{DB.SqlStringRowID(FSSystemGuid.FromObject(x))}'")})")
                .Join(" OR ");

            return await Task.Run(() => o.GetFetchNext(where)
                .Cast<T>()
            );
        }

        /// <summary>
        ///     Creates a new instance. Instance must be saved with SaveChanges
        /// </summary>
        /// <returns>new instance</returns>
        internal T Create()
        {
            return _global.Create<T>();
        }

        /// <summary>
        ///     Returns the count for a condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        internal int Count(string where = "")
        {
            return Create().LoadCount(where);
        }

        /// <summary>
        ///     Returns the count for a condition
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        internal async Task<int> CountAsync(string where = "")
        {
            var o = Create();
            return await Task.Run(() => o.LoadCount(where));
        }

        /// <summary>
        ///     Checks if a record for the condition exists
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        internal bool Exists(string where)
        {
            return Create()
                .Exists(where);
        }

        /// <summary>
        ///     Checks if a record for the condition exists
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        internal async Task<bool> ExistsAsync(string where)
        {
            var o = Create();
            return await Task.Run(() => o.Exists(where));
        }

        /// <summary>
        ///     Saves the entity to the erp database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal T SaveChanges(T entity)
        {
            if (entity.QueryInfo.IsReadonly)
            {
                throw new ApplicationException(typeof(T).Name + " is readonly");
            }

            var o = GetDbEntity(entity);

            // set all values to the loaded entity;
            o.AdoptValues(entity, AdoptMode.Flat);

            o.Save(SaveEntryPoints.CallAllways);

            return o;
        }

        /// <summary>
        ///     Saves the entity to the erp database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal async Task<T> SaveChangesAsync(T entity)
        {
            if (entity.QueryInfo.IsReadonly)
            {
                throw new ApplicationException(typeof(T).Name + " is readonly");
            }

            var o = await Task.Run(() => GetDbEntity(entity));

            // set all values to the loaded entity;
            o.AdoptValues(entity, AdoptMode.Flat);

            await Task.Run(() => o.Save(SaveEntryPoints.CallAllways));

            return o;
        }

        internal bool HasEntityChanged(T entity)
        {
            var pkWhere = GetDbEntityPKWhere(entity);
            var versionWhere = GetDbEntityVersionWhere(entity);

            var where = pkWhere + " AND " + versionWhere;

            var o = Create();
            return !o.Exists(where);
        }

        internal T ReloadEntity(T entity)
        {
            return GetDbEntity(entity);
        }

        #region helper methods

        /// <summary>
        ///     Loads the entity from the database by rowid or pk
        /// </summary>
        /// <param name="entity">entity to load</param>
        /// <returns>loaded entity</returns>
        private T GetDbEntity(T entity)
        {
            var o = Create();

            if (entity.State != FrameworkComponentState.New)
            {
                if (o.Load(GetDbEntityPKWhere(entity)) == 0)
                {
                    throw new ApplicationException(typeof(T).Name + " not found");
                }
            }

            return o;
        }

        /// <summary>
        ///     Generates a condition by rowid or logical pk
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetDbEntityPKWhere(T entity)
        {
            var entityIdColumn = entity.GetVirtualColumnNameOnPropertyName("ROWID");
            var entityId = FSSystemGuid.Null;
            try
            {
                entityId = (FSSystemGuid) entity.GetProperty("ROWID");
            }
            catch (PropertyNotFoundException) {}

            if (!string.IsNullOrEmpty(entityIdColumn) && entityId.HasValue)
            {
                return entityIdColumn + DB.SqlString("=", entityId);
            }
            return entity.GetPKConditionLogical();
        }

        /// <summary>
        ///     Generates a condition by rowversion
        /// </summary>
        /// <param name="entity">Entitiy the condition is generated for</param>
        /// <returns></returns>
        private string GetDbEntityVersionWhere(T entity)
        {
            var entityVersionColumn = entity.GetVirtualColumnNameOnPropertyName("ROWVERSION");
            var entityVersion = FSlong.Null;
            try
            {
                entityVersion = (FSlong) entity.GetProperty("ROWVERSION");
            }
            catch (PropertyNotFoundException) {}

            if (!string.IsNullOrEmpty(entityVersionColumn) && entityVersion.HasValue)
            {
                return entityVersionColumn + DB.SqlString("=", entityVersion);
            }

            return string.Empty;
        }

        #endregion

        public void Dispose()
        {
            if (_global != null)
            {
                _global.CloseConnection();
                FSGlobalFactory.Put(_global);
                _global = null;
            }
        }
    }
}