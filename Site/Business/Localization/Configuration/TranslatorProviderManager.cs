using System;
using System.Configuration;
using System.Web.Configuration;

namespace Site.Business.Localization.Configuration
{
    /// <summary>
    /// Translator provider manager
    /// </summary>
    public class TranslatorProviderManager
    {
        private static TranslatorProvider _defaultProvider;
        private static TranslatorProviderCollection _providers;

        /// <summary>
        /// Static constructor
        /// </summary>
        static TranslatorProviderManager()
        {
           Initialize(); 
        }

        /// <summary>
        /// Initialize
        /// </summary>
        private static void Initialize()
        {
            var configuration = (TranslatorProviderConfiguration)ConfigurationManager.GetSection("translatorProvider");

            if (configuration == null)
                throw new ConfigurationErrorsException
                    ("SampleProvider configuration section is not set correctly.");
           
            _providers = new TranslatorProviderCollection();
            ProvidersHelper.InstantiateProviders(configuration.Providers, _providers, typeof (TranslatorProvider));
            _providers.SetReadOnly();

            foreach (TranslatorProvider p in _providers)
            {
                p.CacheFilePath = configuration.CacheFilePath;
            }

            _defaultProvider = _providers[configuration.Default];
            if (_defaultProvider == null)
                throw new Exception("defaultProvider");
        }

        /// <summary>
        /// Get default provider
        /// </summary>
        public static TranslatorProvider Provider
        {
            get { return _defaultProvider; }
        }

        /// <summary>
        /// Get collection of configured providers
        /// </summary>
        public static TranslatorProviderCollection Providers
        {
            get { return _providers; }
        }
    }
}