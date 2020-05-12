namespace NVShop.Data.NV.Repository
{
    using System.Threading.Tasks;
    using NVShop.Data.NV.Model;

    public partial interface INVCrudRepository<TNVEntity> : INVReadRepository<TNVEntity>
        where TNVEntity : NVEntity
    {
        TNVEntity Update(TNVEntity entity);
        Task<TNVEntity> UpdateAsync(TNVEntity entity);

        TNVEntity Insert(TNVEntity entity);
        Task<TNVEntity> InsertAsync(TNVEntity entity);

        void Delete(TNVEntity entity);
        Task DeleteAsync(TNVEntity entity);
    }

    public partial interface INVCrudRepository
    {
        object Insert(object entity);
        object Update(object entity);
        void Delete(object entity);
    }
}