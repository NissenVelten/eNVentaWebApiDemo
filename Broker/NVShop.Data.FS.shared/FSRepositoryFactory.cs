using NVShop.Data.NV.Model;

namespace NVShop.Data.FS
{
    using FrameworkSystems.FrameworkBase;

    public static class FSRepositoryFactory
    {
        public static IFSRepository<TFSEntity> Create<TFSEntity>(INVIdentityProvider provider) where TFSEntity 
            : class, IDevFrameworkDataObject
        {
            return new FSRepository<TFSEntity>(provider);
        }
    }
}