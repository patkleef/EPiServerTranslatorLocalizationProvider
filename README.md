EPiServerTranslatorLocalizationProvider
=======================================

Before running the website make sure that you obtain a API key for the Yandex translation tool (http://api.yandex.com/translate/) 
or a client id and client secret for the Microsoft translation tool (https://datamarket.azure.com/dataset/bing/microsofttranslator).

Configure the web.config:

<translatorProvider default="yandexTranslator" cacheFilePath="/Resources/">
  <providers>
    <add name="microsoftTranslator" type="Site.Business.Localization.MicrosoftTranslation.MicrosoftTranslatorProvider, Site" clientId="" clientSecret="" />
    <add name="yandexTranslator" type="Site.Business.Localization.YandexTranslation.YandexTranslatorProvider, Site" apiKey="" />
  </providers>
</translatorProvider>
