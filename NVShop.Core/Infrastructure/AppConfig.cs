namespace NVShop
{
    using System;
    using System.Web.Configuration;

    public static class AppConfig
    {
        private static readonly Lazy<AppConfiguration> s_app = new Lazy<AppConfiguration>(AppConfiguration.GetConfiguration);

        #region Properties

        public static bool DynamicDiscovery => NVShop.Settings.DynamicDiscovery.Value;

        #endregion

        /// <summary>
        /// Gets the nv shop.
        /// </summary>
        /// <value>
        /// The nv shop.
        /// </value>
        public static AppConfiguration NVShop => s_app.Value;
    }
}