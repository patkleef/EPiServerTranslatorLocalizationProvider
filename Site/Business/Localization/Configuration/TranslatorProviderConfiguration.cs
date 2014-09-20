using System.Configuration;

namespace Site.Business.Localization.Configuration
{
    /// <summary>
    /// Translator provider configuration
    /// </summary>
    public class TranslatorProviderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection) base["providers"]; }
        }

        [ConfigurationProperty("default", DefaultValue = "microsoftTranslator")]
        public string Default
        {
            get { return (string) base["default"]; }
            set { base["default"] = value; }
        }

        [ConfigurationProperty("cacheFilePath", DefaultValue = "/Resources/")]
        public string CacheFilePath
        {
            get { return (string) base["cacheFilePath"]; }
            set { base["cacheFilePath"] = value; }
        }
    }
}