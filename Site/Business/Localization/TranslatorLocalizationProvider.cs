using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using EPiServer.ServiceLocation;
using Site.Business.Localization.Cache;
using Site.Business.Localization.Configuration;

namespace Site.Business.Localization
{
    /// <summary>
    /// Translator localization provider
    /// </summary>
    public class TranslatorLocalizationProvider : FileXmlLocalizationProvider
    {
        private TranslatorProvider _translatorProvider;
        private ITranslatorCacheProvider _cacheProvider;

        public TranslatorProvider TranslatorProvider
        {
            get { return _translatorProvider ?? (_translatorProvider = TranslatorProviderManager.Provider); }
        }

        public ITranslatorCacheProvider CacheProvider
        {
            get { return (_cacheProvider ?? (_cacheProvider = new TranslatorCacheProvider(TranslatorProvider.CacheFilePath))); }
        }

        /// <summary>
        /// Get string override
        /// </summary>
        /// <param name="originalKey"></param>
        /// <param name="normalizedKey"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override string GetString(string originalKey, string[] normalizedKey, System.Globalization.CultureInfo culture)
        {
            var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>();

            var value = base.GetString(originalKey, normalizedKey, culture);
            if (string.IsNullOrEmpty(value) && !culture.TwoLetterISOLanguageName.Equals(localizationService.FallbackCulture.TwoLetterISOLanguageName)) // Do online translation
            {
                if (CacheProvider.Contains(culture, originalKey)) // Check if the online translation is already cached in de temp xml file
                {
                    return CacheProvider.Get(culture, originalKey);
                }
                var toBeTranslateText = base.GetString(originalKey, normalizedKey, localizationService.FallbackCulture); // Get the fallback translated text so it can be translated via the online tool
                value = TranslatorProvider.Translate(toBeTranslateText, localizationService.FallbackCulture.TwoLetterISOLanguageName, culture.TwoLetterISOLanguageName); // Do translation
                CacheProvider.Insert(culture, normalizedKey, value); // Insert in the cache
            }
            return value;
        }
    }
}