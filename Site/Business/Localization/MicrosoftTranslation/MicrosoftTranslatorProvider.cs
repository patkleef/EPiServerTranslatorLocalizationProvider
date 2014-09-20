using System;
using System.IO;
using System.Net;
using log4net;

namespace Site.Business.Localization.MicrosoftTranslation
{
    /// <summary>
    /// Microsoft translator provider
    /// </summary>
    public class MicrosoftTranslatorProvider : TranslatorProvider
    {
        private string _authToken;
        private readonly ILog _logger;
        private string _clientId;
        private string _clientSecret;

        public string AuthToken
        {
            get
            {
                if (string.IsNullOrEmpty(_authToken))
                {
                    _authToken = GetAuthToken();
                }
                return _authToken;
            }
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        public MicrosoftTranslatorProvider()
        {
            _logger = LogManager.GetLogger(typeof(MicrosoftTranslatorProvider));
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _clientId = config["clientId"];
            _clientSecret = config["clientSecret"];
        }

        /// <summary>
        /// Get auth token
        /// </summary>
        /// <returns></returns>
        private string GetAuthToken()
        {
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            var admAuth = new AdmAuthentication(_clientId, _clientSecret);
            try
            {
                var admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                return "Bearer " + admToken.access_token;
            }
            catch (WebException ex)
            {
                _logger.Error(ex.Message);
                ProcessWebException(ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return string.Empty;
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
            var uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", AuthToken);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    var dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));

                    if (stream != null)
                    {
                        var value = (string)dcs.ReadObject(stream);
                        return value;
                    }
                }
            }
            catch(Exception ex)
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

        /// <summary>
        /// Process web exception
        /// </summary>
        /// <param name="e"></param>
        private void ProcessWebException(WebException e)
        {
            Console.WriteLine("{0}", e.ToString());
            // Obtain detailed error information
            var strResponse = string.Empty;
            using (var response = (HttpWebResponse)e.Response)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(responseStream, System.Text.Encoding.ASCII))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }
            }
            Console.WriteLine("Http status code={0}, error message={1}", e.Status, strResponse);
        }
    }
}