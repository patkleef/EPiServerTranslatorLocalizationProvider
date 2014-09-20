using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Site.Business.Localization.Cache
{
    /// <summary>
    /// Translator cach provider
    /// </summary>
    public class TranslatorCacheProvider : ITranslatorCacheProvider
    {
        private readonly string _xmlPath;
        private readonly IDictionary<string, XDocument> _xDocuments;
        private const string _xmlFilename = "temp_{0}.xml";
        private const string _languageSelectorPreset = "/languages/language/";

        /// <summary>
        /// Return xml filename + path
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public string XmlFilenamePath(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentException("culture");
            }
            return string.Format("{0}{1}", _xmlPath, string.Format(_xmlFilename, culture.TwoLetterISOLanguageName));
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="cacheFilePath"></param>
        public TranslatorCacheProvider(string cacheFilePath)
        {
            if (string.IsNullOrEmpty(cacheFilePath))
            {
                throw new ArgumentException("cacheFilePath");
            }
            if (!cacheFilePath.Last().Equals('/')) // check if the cache file path end with a slash
            {
                cacheFilePath = cacheFilePath + "/";
            }

            _xmlPath = AppDomain.CurrentDomain.BaseDirectory + cacheFilePath;

            _xDocuments = new Dictionary<string, XDocument>();
            foreach (var file in Directory.GetFiles(_xmlPath))
            {
                if(!string.IsNullOrEmpty(file) && Path.GetExtension(file).Equals("xml", StringComparison.InvariantCultureIgnoreCase))
                {
                    var xDoc = XDocument.Load(file);
                    var id = xDoc.XPathSelectElement("/languages/language").Attribute("id");
                    if(Path.GetFileName(file).Equals(string.Format(_xmlFilename, id))) // only add xml files with the cache xml files
                    {
                        _xDocuments.Add(xDoc.XPathSelectElement("/languages/language").Attribute("id").Value, xDoc);
                    }
                }
            }
        }

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="path"></param>
        /// <param name="value"></param>
        public void Insert(CultureInfo culture, string[] path, string value)
        {
            var xDoc = GetByCulture(culture);

            var xElement = xDoc.XPathSelectElement("/languages/language");
            var builder = new StringBuilder();
            for (var i = 0; i < (path.Length - 1); i++)
            {
                if (i > 0)
                {
                    builder.Append(path[i]);
                }
                builder.Append(path[i]);

                if (xDoc.XPathSelectElement(_languageSelectorPreset + builder) == null)
                {
                    xElement.Add(new XElement(path[i]));
                }
                xElement = xDoc.XPathSelectElement(_languageSelectorPreset + builder);
            }
            var element = xDoc.XPathSelectElement(_languageSelectorPreset + builder);
            element.Add(new XElement(path[path.Length - 1], value));

            xDoc.Save(XmlFilenamePath(culture));
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Get(CultureInfo culture, string path)
        {
            var xDoc = GetByCulture(culture);

            var element = xDoc.XPathSelectElement(_languageSelectorPreset + path);
            if (element != null)
            {
                return element.Value;
            }
            return string.Empty;
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Contains(CultureInfo culture, string path)
        {
            var xDoc = GetByCulture(culture);

            return xDoc.XPathSelectElement(_languageSelectorPreset + path) != null;
        }

        /// <summary>
        /// Get by culture
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        private XDocument GetByCulture(CultureInfo culture)
        {
            if (!_xDocuments.ContainsKey(culture.TwoLetterISOLanguageName)) // does not exists so create new xml file
            {
                var xDoc = new XDocument(
                    new XElement("languages",
                        new XElement("language", new XAttribute("name", culture.EnglishName),
                            new XAttribute("id", culture.TwoLetterISOLanguageName))));

                xDoc.Save(XmlFilenamePath(culture));
                _xDocuments.Add(culture.TwoLetterISOLanguageName, xDoc);

                return xDoc;
            }
            return _xDocuments[culture.TwoLetterISOLanguageName];
        }
    }
}