using System;
using FrameworkSystems.FrameworkBase;
using FrameworkSystems.FrameworkDataClient;
using FSGeneral.GlobalObjects;
using NVShop.Data.NV.Model;

namespace NVShop.Data.FS
{
    public class FSGlobalContext
    {
        public FSGlobalContext(NVIdentity identity, IFSGlobalObjects global)
        {
            Identity = identity;
            FSGlobal = global;
        }

        public NVIdentity Identity { get; }
        internal IFSGlobalObjects FSGlobal { get; }

        internal TFSEntity Create<TFSEntity>()
            where TFSEntity : class, IDevFrameworkObject
        {
            return FSGlobal.Create<TFSEntity>();
        }

        internal object Create(Type type) => FSGlobal.Create(type);
        internal void CloseConnection() => FSGlobal.CloseConnection();

        internal FrameworkDataConnection CreateConnection(FrameworkDataConnection existingConnection)
        {
            var conn = new FrameworkDataConnection(FSGlobal, existingConnection);
            conn.Open();

            return conn;
        }
    }
}
