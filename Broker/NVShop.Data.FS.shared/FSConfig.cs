using NVShop.Core.Configuration;

namespace NVShop.Data.FS
{
    public static class FSConfig
    {
        static FSConfig()
        {
            var broker = BrokerSettings.Default;

            BrokerPath = broker.BrokerPath;
            GlobalPoolSize = broker.GlobalPoolSize;
        }

        public static string BrokerPath { get; set; }
        public static int GlobalPoolSize { get; set; }
    }
}