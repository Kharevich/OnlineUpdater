using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OnlineUpdater
{
    /// <summary>
    /// Основной класс библиотеки, который включает настройки обновления и основную логику
    /// </summary>
    public static class Updater
    {
        /// <summary>
        /// Url xml-файла содержащего информацию о последней версии
        /// </summary>
        public static string UpdateXmlUrl;

        /// <summary>
        /// Ссылка на страницу с журналом изменений
        /// </summary>
        private static string ChangeLogURL;

        /// <summary>
        /// Ссылка на файл обновления
        /// РЕАЛИЗАЦИЯ ДЛЯ ОБНОВЛЕНИЯ ОДНИМ ФАЙЛОМ
        /// </summary>
        private static string DownloadURL;

        private static Version LatestVersion;

        private static Version InstalledVersion;

        /// <summary>
        /// Проверка наличия новой версии, при инициализированном UpdateXmlURL
        /// </summary>
        public static void StartUpdate()
        {
            StartUpdate(UpdateXmlUrl);
        }

        private static string UpdateDialogTitle;

        /// <summary>
        /// Проверка наличия новой версии
        /// </summary>
        /// <param name="xmlUrl">Url xml-файла содержащего информацию о последней версии</param>
        public static void StartUpdate(string xmlUrl)
        {
            UpdateXmlUrl = xmlUrl;
            BackgroundWorker bkgWorker = new BackgroundWorker();
            bkgWorker.DoWork += bkgWorker_DoWork;
            bkgWorker.RunWorkerAsync();
        }

        private static void bkgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ReadXML(UpdateXmlUrl);
            if(CheckArgs())
            {

            }
        }

        private static void ReadXML(string xmlUrl)
        {
            WebRequest webRequest = WebRequest.Create(xmlUrl);
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            WebResponse webResponse;

            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch
            {
                //TODO
                return;
            }

            Stream xmlStream = webResponse.GetResponseStream();

            var xmlDocument = new XmlDocument();

            if (xmlStream != null)
            {
                xmlDocument.Load(xmlStream);
            }
            else
            {
                //TODO
                return;
            }

            XmlNodeList xmlItems = xmlDocument.SelectNodes("item");

            if (xmlItems != null)
            {
                foreach (XmlNode item in xmlItems)
                {
                    XmlNode appXmlVersion = item.SelectSingleNode("version");
                    if (appXmlVersion != null)
                    {
                        string appVersion = appXmlVersion.InnerText;
                        LatestVersion = new Version(appVersion);
                    }
                    else
                        continue;

                    XmlNode appXmlTitle = item.SelectSingleNode("title");
                    UpdateDialogTitle = appXmlTitle != null ? appXmlTitle.InnerText : "";

                    XmlNode appXmlChangeLog = item.SelectSingleNode("changelog");
                    ChangeLogURL = GetURL(webResponse.ResponseUri, appXmlChangeLog);

                    XmlNode appXmlUrl = item.SelectSingleNode("url");
                    DownloadURL = GetURL(webResponse.ResponseUri, appXmlUrl);

                    if (IntPtr.Size.Equals(8))
                    {
                        XmlNode appXmlUrl64 = item.SelectSingleNode("url64");
                        string downloadURL64 = GetURL(webResponse.ResponseUri, appXmlUrl64);
                        if (!string.IsNullOrEmpty(downloadURL64))
                        {
                            DownloadURL = downloadURL64;
                        }
                    }
                }
            }
        }

        private static bool CheckArgs()
        {
            throw new NotImplementedException();
        }

        private static string GetURL(Uri respondUri, XmlNode xmlNode)
        {
            var temp = xmlNode != null ? xmlNode.InnerText : "";
            if (!string.IsNullOrEmpty(temp) && Uri.IsWellFormedUriString(temp, UriKind.Relative))
            {
                Uri uri = new Uri(respondUri, temp);
                if (uri.IsAbsoluteUri)
                {
                    temp = uri.AbsoluteUri;
                }
            }
            return temp;
        }
    }
}
