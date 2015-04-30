using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Windows;

namespace OnlineUpdater
{
    /// <summary>
    /// Логика взаимодействия для DownloadProgressWindow.xaml
    /// </summary>
    public partial class DownloadProgressWindow : Window
    {
        private string downloadUrl;
        private string tempPath;
        private WebClient webClient;

        public DownloadProgressWindow(string url)
        {
            InitializeComponent();

            downloadUrl = url;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webClient = new WebClient();
            Uri uri = new Uri(downloadUrl);
            tempPath = Path.Combine(Path.GetTempPath(), GetFileName(downloadUrl));
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

            webClient.DownloadFileAsync(uri, tempPath);
        }

        void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo { FileName = tempPath, UseShellExecute = true };
                Process.Start(processStartInfo);

                Environment.Exit(0);
            }
        }

        void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private static string GetFileName(string url)
        {
            string fileName = string.Empty;
            Uri uri = new Uri(url);
            if (uri.Scheme.Equals(Uri.UriSchemeHttp) || uri.Scheme.Equals(Uri.UriSchemeHttps))
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                httpWebRequest.Method = "HEAD";
                httpWebRequest.AllowAutoRedirect = false;

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Redirect) ||
                    httpWebResponse.StatusCode.Equals(HttpStatusCode.Moved) ||
                    httpWebResponse.StatusCode.Equals(HttpStatusCode.MovedPermanently))
                {
                    if (httpWebResponse.Headers["Location"] != null)
                    {
                        var location = httpWebResponse.Headers["Location"];
                        fileName = GetFileName(location);
                        return fileName;
                    }
                }


                var contentDisposition = httpWebResponse.Headers["content-disposition"];
                if (!string.IsNullOrEmpty(contentDisposition))
                {
                    const string lookForFileName = "filename=";
                    var index = contentDisposition.IndexOf(lookForFileName, StringComparison.CurrentCultureIgnoreCase);
                    if (index >= 0)
                        fileName = contentDisposition.Substring(index + lookForFileName.Length);
                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Substring(1, fileName.Length - 2);
                    }
                }
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = Path.GetFileName(uri.LocalPath);
            }
            return fileName;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            webClient.CancelAsync();
        }
    }
}
