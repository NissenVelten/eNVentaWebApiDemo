using System;
using FrameworkSystems.FrameworkBase;
using FrameworkSystems.FrameworkDataClient;
using FSGeneral.GlobalObjects;
using NVShop.Core.Model;

namespace NVShop.Data.FS
{
    public class FSGlobal
    {
        public FSGlobal(NVIdentity identity, IFSGlobalObjects global)
        {
            Identity = identity;
            Global = global;
        }

        internal NVIdentity Identity { get; }
        internal IFSGlobalObjects Global { get; }

        internal TFSEntity Create<TFSEntity>()
            where TFSEntity : class, IDevFrameworkObject
        {
            return Global.Create<TFSEntity>();
        }

        internal object Create(Type type)
        {
            return Global.Create(type);
        }

        internal void CloseConnection() => Global.CloseConnection();

        internal FrameworkDataConnection CreateConnection(FrameworkDataConnection existingConnection)
        {
            var conn = new FrameworkDataConnection(Global, existingConnection);
            conn.Open();

            return conn;
        }
    }
}
