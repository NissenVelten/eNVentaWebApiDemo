namespace NVShop.Data.eNVenta.NVRepository
{
    using AutoMapper;

    using FrameworkSystems.FrameworkBase;

    using NVShop.Data.FS;

    using NVShop.Data.NV.Model;
    using NVShop.Data.NV.Repository;

    using System;
    using System.Threading.Tasks;

    public class NVCrudRepository<TNVEntity, TFSEntity> : NVReadRepository<TNVEntity, TFSEntity>, INVCrudRepository<TNVEntity>
        where TNVEntity : NVEntity, new()
        where TFSEntity : class, IDevFrameworkDataObject
    {
        protected NVCrudRepository(IFSRepository<TFSEntity> rep, IMapper mapper, FSUtil util) : base(rep, mapper, util)
        {
        }

        public object Insert(object entity) => Insert((TNVEntity)entity);

        public virtual TNVEntity Insert(TNVEntity entity)
        {
            var data = Repository.Create();
            data = Mapper.Map(entity, data);
            data.State = FrameworkComponentState.New;

            data = Repository.Save(data);

            return Mapper.Map<TNVEntity>(data);
        }

        public virtual async Task<TNVEntity> InsertAsync(
            TNVEntity entity)
        {
            var data = Repository.Create();
            data = Mapper.Map(entity, data);
            data.State = FrameworkComponentState.New;

            data = await Repository.SaveAsync(data);

            return Mapper.Map<TNVEntity>(data);
        }

        public object Update(object entity) => Update((TNVEntity)entity);

        public virtual TNVEntity Update(TNVEntity entity)
        {
            var data = Repository.Find(entity.RowId);
            if (data == null)
            {
                throw new ApplicationException("record not found");
            }

            data = Mapper.Map(entity, data);
            data = Repository.Save(data);

            return Mapper.Map(data, entity);
        }

        public virtual async Task<TNVEntity> UpdateAsync(
            TNVEntity entity)
        {
            var data = await Repository.FindAsync(entity.RowId);

            if (data == null)
            {
                throw new ApplicationException("record not found");
            }

            data = Mapper.Map(entity, data);
            data = await Repository.SaveAsync(data);

            return Mapper.Map(data, entity);
        }

        public void Delete(object entity) => Delete((TNVEntity)entity);

        public virtual void Delete(
            TNVEntity entity)
        {
            var data = Repository.Create();
            data = Mapper.Map(entity, data);
            data.State = FrameworkComponentState.UnchangedButDeleted;

            if (data == null)
            {
                throw new ApplicationException("record not found");
            }

            Repository.Delete(data);
        }

        public virtual async Task DeleteAsync(
            TNVEntity entity)
        {
            var data = Repository.Create();
            data = Mapper.Map(entity, data);
            data.State = FrameworkComponentState.UnchangedButDeleted;

            if (data == null)
            {
                throw new ApplicationException("record not found");
            }

            await Repository.DeleteAsync(data);
        }
    }
}