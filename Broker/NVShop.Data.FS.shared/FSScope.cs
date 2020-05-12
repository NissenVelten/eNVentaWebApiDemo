using NV.ERP.General.ServerFile;
using NVShop.Data.NV.Model;

namespace NVShop.Data.FS
{
    using System;
    using FrameworkSystems.FrameworkBase;

    public class FSScope : IDisposable
    {
        
        public FSScope(FSGlobalContext global)
        {
            if (global == null) throw new ArgumentNullException(nameof(global));

            FSGlobalContext = FSGlobalFactory.Get(global.Identity);
        }

        public FSScope(NVIdentity identity)
        {
            FSGlobalContext = FSGlobalFactory.Get(identity);
        }

        public TFSEntity Create<TFSEntity>()
            where TFSEntity : class, IDevFrameworkObject
        {
            return FSGlobalContext.Create<TFSEntity>();
        }

        public object Create(Type type)
        {
            return FSGlobalContext.Create(type);
        }

        public IcFileManager FileManager => FSGlobalContext.FSGlobal.ocGlobal.oFileManager;

        internal FSGlobalContext FSGlobalContext { get; }

        public void Dispose()
        {
            if (FSGlobalContext != null)
            {
                FSGlobalContext.CloseConnection();
                FSGlobalFactory.Put(FSGlobalContext);
            }
        }
    }
}