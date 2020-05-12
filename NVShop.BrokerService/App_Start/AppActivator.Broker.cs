using NVShop.Core.Configuration;

namespace NVShop.BrokerService
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web.Hosting;

    public static partial class AppActivator
    {
        private static class Broker
        {
            public static void PreStart()
            {
                if (!BrokerSettings.IsValid()) return;

                AppDomain.CurrentDomain.AssemblyResolve += Broker_AssemblyResolve;

                Assembly.LoadFrom(Path.Combine(HostingEnvironment.ApplicationPhysicalPath,
                    $"broker/{BrokerSettings.Default.Version}/NVShop.Data.eNVenta.dll"));
            }

            public static void Shutdown() {}

            private static Assembly Broker_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                var fsPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "broker", BrokerSettings.Default.Version, args.Name);
                if (File.Exists(fsPath))
                {
                    return Assembly.LoadFrom(fsPath);
                }

                var brokerPath = BrokerSettings.Default.BrokerPath;

                Debug.Print("DataSourceConfig.CurrentDomain_AssemblyResolve:" + args.Name);

                var name = new AssemblyName(args.Name).Name + ".dll";

                if (!string.IsNullOrEmpty(brokerPath))
                {
                    if (File.Exists(Path.Combine(brokerPath, "assembly", name)))
                    {
                        return Assembly.LoadFrom(Path.Combine(brokerPath, "assembly", name));
                    }
                    if (File.Exists(Path.Combine(brokerPath, "bin", name)))
                    {
                        return Assembly.LoadFrom(Path.Combine(brokerPath, "bin", name));
                    }
                }

                return null;
            }
        }
    }
}