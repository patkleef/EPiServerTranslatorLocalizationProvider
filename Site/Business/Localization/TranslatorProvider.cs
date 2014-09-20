using System.Configuration.Provider;

namespace Site.Business.Localization
{
    /// <summary>
    /// Translator provider
    /// </summary>
    public abstract class TranslatorProvider : ProviderBase
    {
        public string CacheFilePath { get; set; }

        /// <summary>
        /// Translate method
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public abstract string Translate(string text, string from, string to);
    }
}