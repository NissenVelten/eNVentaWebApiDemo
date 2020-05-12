using NVShop.Data.NV.Model;

namespace NVShop.Data.FS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FrameworkSystems.FrameworkBase;
    using FrameworkSystems.FrameworkBase.Metadatatype;

    using NVShop.Data.FS.Linq;

    public class FSRepository<TFSEntity> : IFSRepository<TFSEntity>, IFSRepository
        where TFSEntity : class, IDevFrameworkDataObject
    {
        private readonly INVIdentityProvider _provider;

        public FSRepository(INVIdentityProvider provider)
        {
            _provider = provider;
        }

        public virtual IQueryable<TFSEntity> Queryable()
        {
			return new FSQueryable<TFSEntity>(this);
        }

		public virtual IQueryable<TFSEntity> Queryable(string where, string orderby)
		{
			return new FSQueryable<TFSEntity>(this, where, orderby);
		}

		public virtual IEnumerable<TFSEntity> Get(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                var query = dataContext.GetFetchNext(where, orderBy, offset, limit);

                if (loadSize.HasValue)
                {
                    query = query.TakeWhile((t, index) => index <= loadSize);
                }

                foreach (var o in query)
                {
                    yield return o;
                }
            }
        }

        public virtual Task<IEnumerable<TFSEntity>> GetAsync(
            string where = "",
            string orderBy = "",
            int? offset = null,
            int? limit = null,
            int? loadSize = null)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                var query = dataContext.GetFetchNext(where, orderBy, offset, limit);

                if (loadSize.HasValue)
                {
                    query = query.TakeWhile((t, index) => index < loadSize);
                }

                return Task.FromResult(query.ToList().AsEnumerable());
            }
        }

        public IEnumerable<FSIdent> Ident(
            string where = "",
            string orderBy = "")
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                var query = dataContext.GetFetchNextPK(where, orderBy);

                foreach (var (item1, item2) in query)
                {
                    yield return new FSIdent(item1, item2);
                }
            }
        }

        public TFSEntity FirstOrDefault(string where)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.GetFetchNext(where)
                    .FirstOrDefault();
            }
        }

        public async Task<TFSEntity> FirstOrDefaultAsync(string where)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return await Task.FromResult(dataContext.GetFetchNext(where)
                    .FirstOrDefault());
            }
        }

        public TFSEntity Find(string rowId)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.GetByRowId(rowId);
            }
        }

        public TFSEntity Find(FSSystemGuid rowId)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.GetByRowId(rowId);
            }
        }

        public async Task<TFSEntity> FindAsync(string rowId)
        {
            if (!Guid.TryParse(rowId, out var id))
                return default;

            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return await dataContext.GetByRowIdAsync(id);
            }
        }

        public virtual int Count(string where = "")
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.Count(where);
            }
        }

        public virtual async Task<int> CountAsync(string where = "")
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return await dataContext.CountAsync(where);
            }
        }

        public TFSEntity Create()
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.Create();
            }
        }

        public bool Exists(string where = "")
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.Exists(where);
            }
        }

        public async Task<bool> ExistsAsync(string where = "")
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return await dataContext.ExistsAsync(where);
            }
        }

        public void Delete(TFSEntity entityToDelete)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                var entity = dataContext.ReloadEntity(entityToDelete);
                entity.Delete();
                dataContext.SaveChanges(entity);
            }
        }

        public async Task DeleteAsync(TFSEntity entityToDelete)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                var entity = dataContext.ReloadEntity(entityToDelete);
                entity.Delete();

                await dataContext.SaveChangesAsync(entity);
            }
        }

        public TFSEntity Save(TFSEntity entity)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.SaveChanges(entity);
            }
        }

        public async Task<TFSEntity> SaveAsync(TFSEntity entity)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return await dataContext.SaveChangesAsync(entity);
            }
        }

        public bool HasEntityChanged(TFSEntity entity)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.HasEntityChanged(entity);
            }
        }

        public TFSEntity ReloadEntity(TFSEntity entity)
        {
            using (var dataContext = new FSDataContext<TFSEntity>(_provider))
            {
                return dataContext.ReloadEntity(entity);
            }
        }

		public bool IsReadOnly => Create()
            .QueryInfo.IsReadonly;

		IQueryable IFSRepository.Queryable()
		{
			return Queryable();
		}

		IEnumerable IFSRepository.Get(string where, string orderby, int? offset, int? limit, int? loadSize)
		{
			return Get(where, orderby, offset, limit, loadSize);
		}
    }
}