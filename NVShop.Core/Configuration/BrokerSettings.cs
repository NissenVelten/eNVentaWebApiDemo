namespace NVShop.Core.Configuration
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Security.Principal;

    public static class BrokerSettings
    {
        private static readonly Lazy<BrokerElement> broker = new Lazy<BrokerElement>(GetBroker);

        public static BrokerElement Default => broker.Value;

        public static bool IsValid()
        {
            if (Default == null)
                return false;

            var brokerPath = Default.BrokerPath;

            if (string.IsNullOrEmpty(brokerPath))
                return false;

            if (!Directory.Exists(brokerPath))
                return false;

            var acl = Directory.GetAccessControl(brokerPath);

            var rules = acl?.GetAccessRules(true, true, typeof(SecurityIdentifier));
            return rules != null && rules.Cast<FileSystemAccessRule>().Any(x => ( x.FileSystemRights & FileSystemRights.Read ) != FileSystemRights.Read);

            // Read permissions to target path required
        }

       

        public static bool IsConnected()
        {
            if (!IsValid())
                return false;
    
            var broker = GetBroker();

            if (string.IsNullOrEmpty(broker.BrokerPath))
                return false;

            if (!Directory.Exists(broker.BrokerPath))
                return false;

            if (AppDomain.CurrentDomain.GetAssemblies().All(x => x.GetName().Name != "NVShop.Data.eNVenta"))
                return false;

            return true;
        }

        private static BrokerElement GetBroker()
        {
            var brokers = AppConfig.NVShop.Brokers.Cast<BrokerElement>();

            var broker = !string.IsNullOrEmpty(AppConfig.NVShop.Brokers.Default)
                ? brokers.FirstOrDefault(x => x.Name == AppConfig.NVShop.Brokers.Default)
                : brokers.FirstOrDefault();

            return broker;
        }
    }
}
