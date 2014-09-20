using System.Globalization;

namespace Site.Business.Localization.Cache
{
    /// <summary>
    /// Translator cache provider interface
    /// </summary>
    public interface ITranslatorCacheProvider
    {
        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        void Insert(CultureInfo culture, string[] path, string value);

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        string Get(CultureInfo culture, string path);

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool Contains(CultureInfo culture, string path);
    }
}