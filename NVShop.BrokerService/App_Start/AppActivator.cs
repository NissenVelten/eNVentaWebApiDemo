using NVShop.BrokerService;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(AppActivator), "PreStart")]
[assembly: ApplicationShutdownMethod(typeof(AppActivator), "Shutdown")]

namespace NVShop.BrokerService
{
    public static partial class AppActivator
    {
        public static void PreStart()
        {
            Broker.PreStart();
            //IoC.PreStart();
        }

        public static void Shutdown()
        {
            //IoC.Shutdown();
            Broker.Shutdown();
        }
    }
}