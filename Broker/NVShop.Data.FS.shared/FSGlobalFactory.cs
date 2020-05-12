
namespace NVShop.Data.FS
{
    using FrameworkSystems.FrameworkBase.Configuration.Application;
    using FrameworkSystems.FrameworkBroker;
    using FrameworkSystems.FrameworkDataProvider.BaseObjects;

    using FSGeneral.GlobalObjects;

    using NVShop.Core.Configuration;
    using NVShop.Data.NV.Model;

    using System;
    using System.Collections.Concurrent;


    internal static class FSGlobalFactory
    {
        private static readonly ConcurrentDictionary<string, Lazy<FSGlobalPool>> _globalPools = new ConcurrentDictionary<string, Lazy<FSGlobalPool>>();

        static FSGlobalFactory()
        {
            ApplicationConfig.ApplicationPath = FSConfig.BrokerPath;
            ApplicationConfig.ApplicationLicensePath = FSConfig.BrokerPath;
        }

        /// <summary>
        ///     Gets this instance.
        /// </summary>
        /// <returns></returns>
        internal static FSGlobalContext Get(NVIdentity identity)
        {
            //identity = identity ?? NVUserContext.Identity;

            var pool = _globalPools.GetOrAdd(identity.GetToken(), new Lazy<FSGlobalPool>(() => 
                new FSGlobalPool(() => GenerateGlobal(identity), BrokerSettings.Default.GlobalPoolSize)
            ));

            var global = pool.Value.GetGlobal() as IFSGlobalObjects;

            return new FSGlobalContext(identity, global);
        }

        /// <summary>
        ///     Puts the specified global.
        /// </summary>
        /// <param name="global">The global.</param>
        internal static void Put(FSGlobalContext global)
        {
            //identity = identity ?? NVUserContext.Identity;

            var pool = _globalPools.GetOrAdd(global.Identity.GetToken(), new Lazy<FSGlobalPool>(() =>
                new FSGlobalPool(() => GenerateGlobal(global.Identity), BrokerSettings.Default.GlobalPoolSize)
            ));

            pool.Value.PutGlobal(global.FSGlobal);
        }

        internal static IFSGlobalObjects GenerateGlobal(NVIdentity identity)
        {
            return ( (IFSGlobalObjects)GlobalObjectManager.CreateGlobalObject(guid.NewGuid().Value) )
                .Authenticate(identity)
                .EnableCaching();
        }
    }
}