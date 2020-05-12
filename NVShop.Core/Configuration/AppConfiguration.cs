namespace NVShop
{
    using System.Configuration;

    public class AppConfiguration : ConfigurationSection
    {
        #region Static Fields and Constants

        private const string SETTINGS = "settings";
        private const string BROKERS = "brokers";

        #endregion

        public static AppConfiguration GetConfiguration()
        {
            var config = ConfigurationManager.GetSection("egate/app") as AppConfiguration;

            return config ?? new AppConfiguration();
        }

        #region Constructors

        public AppConfiguration()
        {
            Settings = new SettingsElement();
            Brokers = new BrokerElementCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets/sets the settings.
        /// </summary>
        [ConfigurationProperty(SETTINGS, IsRequired = false)]
        public SettingsElement Settings
        {
            get => (SettingsElement)this[SETTINGS];
            set => this[SETTINGS] = value;
        }

        [ConfigurationProperty(BROKERS, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(BrokerElementCollection))]
        public BrokerElementCollection Brokers
        {
            get => (BrokerElementCollection)base[BROKERS];
            set => this[BROKERS] = value;
        }

        #endregion

        public override bool IsReadOnly() { return false; }
    }

    /// <summary>
    ///     The settings element of the configuration section.
    /// </summary>
    public class SettingsElement : ConfigurationElement
    {
        #region Static Fields and Constants

        private const string DYNAMIC_DISCOVERY = "dynamicDiscovery";
        private const string ENABLE_LOCALIZED_VIEWS = "enabledLocalizedViews";
        private const string ENABLE_MOBILE_VIEWS = "enabledMobileViews";
        private const string ENABLE_PLUGIN_VIEWS = "enabledPluginViews";

        #endregion

        #region Properties

        /// <summary>
        ///     In addition to configured assemblies examine and load assemblies in the bin directory.
        /// </summary>
        [ConfigurationProperty(DYNAMIC_DISCOVERY, IsRequired = false)]
        public BooleanElement DynamicDiscovery
        {
            get => (BooleanElement)this[DYNAMIC_DISCOVERY];
            set => this[DYNAMIC_DISCOVERY] = value;
        }

        [ConfigurationProperty(ENABLE_LOCALIZED_VIEWS, IsRequired = false)]
        public BooleanElement EnableLocalizedViews
        {
            get => (BooleanElement)this[ENABLE_LOCALIZED_VIEWS];
            set => this[ENABLE_LOCALIZED_VIEWS] = value;
        }

        [ConfigurationProperty(ENABLE_MOBILE_VIEWS, IsRequired = false)]
        public BooleanElement EnableMobileViews
        {
            get => (BooleanElement)this[ENABLE_MOBILE_VIEWS];
            set => this[ENABLE_MOBILE_VIEWS] = value;
        }

        [ConfigurationProperty(ENABLE_PLUGIN_VIEWS, IsRequired = false)]
        public BooleanElement EnablePluginViews
        {
            get => (BooleanElement)this[ENABLE_PLUGIN_VIEWS];
            set => this[ENABLE_PLUGIN_VIEWS] = value;
        }

        #endregion
    }

    public class BrokerElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new BrokerElement();

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ( (BrokerElement)element).Name;
        }

        public void Clear()
        {
            BaseClear();
        }

        public void Add(BrokerElement element)
        {
            BaseAdd(element);
        }

        [ConfigurationProperty("Default")]
        public string Default { get; set; }
    }

    public class BrokerElement : ConfigurationElement
    {
        #region Static Fields and Constants

        private const string NAME = "name";
        private const string BROKERURL = "url";
        private const string BROKERPATH = "path";
        private const string VERSION = "version";
        private const string GLOBALPOOLSIZE = "poolsize";
        private const string GLOBALLANGUAGE = "lang";

        #endregion

        #region Properties

        [ConfigurationProperty(NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get => (string)this[NAME];
            set => this[NAME] = value;
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [ConfigurationProperty(BROKERURL, IsRequired = false)]
        public string ServiceUrl
        {
            get => (string)this[BROKERURL];
            set => this[BROKERURL] = value;
        }

        /// <summary>
        /// Gets or sets the size of the global pool.
        /// </summary>
        /// <value>
        /// The size of the global pool.
        /// </value>
        [ConfigurationProperty(GLOBALPOOLSIZE, IsRequired = false, DefaultValue = 10)]
        public int GlobalPoolSize
        {
            get => (int)this[GLOBALPOOLSIZE];
            set => this[GLOBALPOOLSIZE] = value;
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [ConfigurationProperty(GLOBALLANGUAGE, IsRequired = false, DefaultValue = "de")]
        public string Language
        {
            get => (string)this[GLOBALLANGUAGE];
            set => this[GLOBALLANGUAGE] = value;
        }

        /// <summary>
        /// Gets or sets the broker path.
        /// </summary>
        /// <value>
        /// The broker path.
        /// </value>
        [ConfigurationProperty(BROKERPATH, IsRequired = false)]
        public string BrokerPath
        {
            get => (string)this[BROKERPATH];
            set => this[BROKERPATH] = value;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [ConfigurationProperty(VERSION, IsRequired = false)]
        public string Version
        {
            get => (string)this[VERSION];
            set => this[VERSION] = value;
        }

        #endregion
    }
    
    public class WebshopElement : ConfigurationElement
    {
        #region Static Fields and Constants

        private const string ID = "id";

        #endregion

        #region Properties

        [ConfigurationProperty(ID)]
        public int Id
        {
            get => (int) this[ID];
            set => this[ID] = value;
        }

        #endregion
    }

    /// <summary>
    ///     A configuration element with a boolean value.
    /// </summary>
    public class BooleanElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        ///     Gets/sets the element value.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public bool Value
        {
            get => (bool) this["value"];
            set => this["value"] = value;
        }

        #endregion
    }

    /// <summary>
    ///     A configuration element with a string value.
    /// </summary>
    public class StringElement : ConfigurationElement
    {
        #region Properties

        /// <summary>
        ///     Gets/sets the element value.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get => (string) this["value"];
            set => this["value"] = value;
        }

        #endregion
    }
}