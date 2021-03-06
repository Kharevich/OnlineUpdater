﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Threading;
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
        internal static string ChangeLogURL;

        /// <summary>
        /// Ссылка на файл обновления
        /// РЕАЛИЗАЦИЯ ДЛЯ ОБНОВЛЕНИЯ ОДНИМ ФАЙЛОМ
        /// </summary>
        private static string DownloadURL;

        internal static string UpdateDialogTitle;

        internal static string AppTitle;

        internal static Version LatestVersion;

        internal static Version InstalledVersion;

        /// <summary>
        /// Проверка наличия новой версии, при инициализированном UpdateXmlURL
        /// </summary>
        public static void StartUpdate()
        {
            StartUpdate(UpdateXmlUrl);
        }

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
                Thread thread = new Thread(ShowUI);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
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
                }
            }
        }

        private static bool CheckArgs()
        {
            Assembly mainAssembly = Assembly.GetEntryAssembly();
            var titleAttribute = (AssemblyTitleAttribute)GetAttribute(mainAssembly, typeof(AssemblyTitleAttribute));
            AppTitle = titleAttribute != null ? titleAttribute.Title : mainAssembly.GetName().Name;

            InstalledVersion = mainAssembly.GetName().Version;

            if (LatestVersion == null || DownloadURL == null || InstalledVersion == null)
                return false;

            if (LatestVersion > InstalledVersion)
            {
                return true;
            }
            return false;
        }

        private static void ShowUI(object obj)
        {
            UpdateWindow updateForm = new UpdateWindow();
            updateForm.ShowDialog();
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

        internal static void DownloadUpdate()
        {
            DownloadProgressWindow downloadWindow = new DownloadProgressWindow(DownloadURL);
            downloadWindow.ShowDialog();
        }

        private static Attribute GetAttribute(Assembly assembly, Type attributeType)
        {
            object[] attributes = assembly.GetCustomAttributes(attributeType, false);
            if (attributes.Length == 0)
            {
                return null;
            }
            return (Attribute)attributes[0];
        }
    }
}
