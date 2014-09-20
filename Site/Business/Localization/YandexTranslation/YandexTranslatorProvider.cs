using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;
using log4net;

namespace Site.Business.Localization.YandexTranslation
{
    /// <summary>
    /// Yandex translator provider
    /// </summary>
    public class YandexTranslatorProvider : TranslatorProvider
    {
        private string _apiKey;
        private readonly ILog _logger;

        /// <summary>
        /// Public constructor
        /// </summary>
        public YandexTranslatorProvider()
        {
            _logger = LogManager.GetLogger(typeof (YandexTranslatorProvider));
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _apiKey = config["apiKey"];
        }

        /// <summary>
        /// Translate method
        /// </summary>
        /// <param name="text"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public override string Translate(string text, string from, string to)
        {
            var uri = string.Format("https://translate.yandex.net/api/v1.5/tr/translate?key={0}&lang={1}-{2}&text={3}", _apiKey, from, to, text);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());

                 // Read the whole contents and return as a string  
                var value = reader.ReadToEnd();

                var xDoc = XDocument.Parse(value);
                if (xDoc.XPathSelectElement("Translation")
                    .Attribute("code")
                    .Value.Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                    return xDoc.XPathSelectElement("Translation/text").Value;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return string.Empty;
        }
    }
}