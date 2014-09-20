using System.Configuration.Provider;

namespace Site.Business.Localization.Configuration
{
    /// <summary>
    /// Translator provider collection
    /// </summary>
    public class TranslatorProviderCollection : ProviderCollection
    {
        new public TranslatorProvider this[string name]
        {
            get { return (TranslatorProvider) base[name]; }
        }
    }
}