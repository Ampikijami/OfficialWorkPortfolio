using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using System.Threading;
using System.IO;

namespace GmailLoginScript
{
    class CustomCefDownloadHandler : IDownloadHandler, IDisposable
    {
        public bool downloadComplete = false;
        public event EventHandler<DownloadItem> OnBeforeDownloadFired;
        public event EventHandler<DownloadItem> OnDownloadUpdatedFired;
        private string tempPathForDownloads = @"Path for the Downloaded file.";
        public CustomCefDownloadHandler()
        {
            tempPathForDownloads = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        }
        public bool CanDownload(IWebBrowser webBrowser, IBrowser browser, string x, string y)
        {
            return true;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release managed
            }
            //release unmanaged
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            OnBeforeDownloadFired?.Invoke(this, downloadItem);
            if (downloadItem.IsValid)
            {
                Console.WriteLine("== File information ========================");
                Console.WriteLine(" File URL: {0}", downloadItem.Url);
                Console.WriteLine(" Suggested FileName: {0}", downloadItem.SuggestedFileName);
                Console.WriteLine(" MimeType: {0}", downloadItem.MimeType);
                Console.WriteLine(" Content Disposition: {0}", downloadItem.ContentDisposition);
                Console.WriteLine(" Total Size: {0}", downloadItem.TotalBytes);
                Console.WriteLine("============================================");
            }
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(Path.Combine(
                            tempPathForDownloads,
                            downloadItem.SuggestedFileName
                        ),
                        showDialog: false
                    );
                }
            }
        }
        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            OnDownloadUpdatedFired?.Invoke(this, downloadItem);
            if (downloadItem.IsValid)
            {
                if (downloadItem.IsInProgress && (downloadItem.PercentComplete != 0))
                {
                    Console.WriteLine(
                        "Current Download Speed: {0} bytes ({1}%)",
                        downloadItem.CurrentSpeed,
                        downloadItem.PercentComplete
                    );
                }
                if (downloadItem.IsComplete)
                {
                    Console.WriteLine("The download has been finished !");
                    downloadComplete = true;
                }
            }
        }
    }
}