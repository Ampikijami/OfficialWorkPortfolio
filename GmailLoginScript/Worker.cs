using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack; //need this if i decide to do some logic based on the html nodes on the webpgage
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace GmailLoginScript
{
    public class Worker
    {
        private string CachePath; //If left empty, the cache path becomes %user%\\Downloads\\CefSharp\\Cache
        private string Login = "testbotuserruler@gmail.com"; //testbotuserruler@gmail.com
        private string Pass = "Testbotuserruler1."; //Testbotuserruler1.
        private string Url = "https://accounts.google.com/v3/signin/identifier?dsh=S-922693188%3A1687696738993615&continue=https%3A%2F%2Fwww.google.com%2F&ec=GAZAmgQ&flowEntry=ServiceLogin&flowName=GlifWebSignIn&hl=pl&ifkv=Af_xneEUq8CAeZshMYEjntnqav66EJMl4L4jrfFgDPHInQ0089xYR3Y4A60Ed790yckxVm36zbWtiw&passive=true"; //https://accounts.google.com/v3/signin/identifier?dsh=S-922693188%3A1687696738993615&continue=https%3A%2F%2Fwww.google.com%2F&ec=GAZAmgQ&flowEntry=ServiceLogin&flowName=GlifWebSignIn&hl=pl&ifkv=Af_xneEUq8CAeZshMYEjntnqav66EJMl4L4jrfFgDPHInQ0089xYR3Y4A60Ed790yckxVm36zbWtiw&passive=true//
        private List<URIItem> websiteResources = new List<URIItem>()
        {
            new URIItem()
            {
                UrlPart = ""
            }
        };
        public string UserAgent { get; set; }
        private ChromiumWebBrowser browser = null; //in depth guide to using Cefsharp chromiumwebbrowser https://github.com/cefsharp/CefSharp/blob/master/CefSharp.Example/CefExample.cs
        private readonly Regex exFileFromUrl = new Regex(@"(?<=/)(?<file>[^/]+)\.[^\.]+$", RegexOptions.Compiled);
        //the <file> component of the above regex matches to wlwmanifest.xml from the following href value from an example node... <<link rel="wlwmanifest" type="application/wlwmanifest+xml" href="https://exploreinformatica.com/wp-includes/wlwmanifest.xml">>
        //https://exploreinformatica.com/wp-includes/wlwmanifest.xml
        // (?<=/) means positive lookbehind --- Looks for a string when the following string preceeds it.
        //so it looks for '/', then it matches to a series of characters where none of them are '/'. Followed by a '.', and then followed by NOT '.' once or more.
        public void ScriptExecutor() //this is the entry point of this class
        {
            InitializeDefaultValues();
            CefInit();
            LogIn();
            PotwierdzżeToTy();
            /*Verify it’s you
            To help keep your account safe, Google wants to make sure it’s really you trying to sign in Learn more
            testbotuserruler @gmail.com
            Choose how you want to sign in:
            Get a verification code at ru••••••@gmail.com
            Use another phone or computer to finish signing in
            Confirm your recovery email
            Get help
            */
            NavigateToGmail();
            Cef.Shutdown();
        }
        private void ClearOldCache(string path)
        {
            Task timeout = Task.Run(() => Thread.Sleep(5000));
            while (Directory.Exists(path) && path.Contains("Downloads\\CefSharp") && timeout.IsCompleted == false)
            {
                Directory.Delete(path, true);
                Thread.Sleep(900);
            };
            if (timeout.IsCompleted)
                throw new Exception("ClearOldCache(string path) method timed out.");
        }
        private void CefInit()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            ClearOldCache(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\CefSharp\\Cache").Replace("\\Cache", ""));
            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data //CachePath
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\CefSharp\\Cache"),
                UserAgent = UserAgent,
                IgnoreCertificateErrors = true,
            };
            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            browser = new ChromiumWebBrowser($"{Url}");
            Task timeout = Task.Run(() => Thread.Sleep(10000));
            do
                Thread.Sleep(50);
            while (browser.IsBrowserInitialized == false && timeout.IsCompleted == false);
            browser.LoadUrl(browser.Address);
            do
                Thread.Sleep(50);
            while (browser.GetBrowser().IsLoading && timeout.IsCompleted == false);
            string html = browser.GetSourceAsync().GetAwaiter().GetResult();
            do
                Thread.Sleep(50);
            while (browser.GetBrowser().IsLoading && timeout.IsCompleted == false);
            if (timeout.IsCompleted == true)
                throw new Exception("CefInit() timed out.");
            
        }
        private void NavigateToGmail()
        {
            string gmailAddress = "https://mail.google.com/mail/u/0/#inbox";
            
            Task timeout = Task.Run(() => Thread.Sleep(10000));
            do
                Thread.Sleep(50);
            while (browser.IsBrowserInitialized == false && timeout.IsCompleted == false);
            browser.LoadUrl(gmailAddress);
            do
                Thread.Sleep(50);
            while (browser.GetBrowser().IsLoading && timeout.IsCompleted == false);
            if (timeout.IsCompleted == true)
                throw new Exception("CefInit() timed out.");
            TakeScreenshot(browser); //Asks to allow intelligent functioning [3]
            AllowIntelligentFunctioning();
        }
        private void LogIn()
        {
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"input[type='email']\").focus(); })();").GetAwaiter();//put focus on email prompt
            SendKeys(browser, Login);
            //click "dalej"
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"button[class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 qIypjc TrZEUc lw1w4b']\").click(); })();").GetAwaiter();
            Thread.Sleep(3000); //wait for the screen to kinda change
            //< button class="VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 qIypjc TrZEUc lw1w4b" jscontroller="soHxf" jsaction="click:cOuCgd; mousedown:UX7yZ; mouseup:lbsD7e; mouseenter:tfO1Yc; mouseleave:JywGue; touchstart:p6p2H; touchmove:FwuNnf; touchend:yfqBxc; touchcancel:JMtRjd; focus:AHmuwe; blur:O22p3e; contextmenu:mg9Pef;mlnRJb:fLiPzd;" data-idom-class="nCP5yc AjY5Oe DuMIQc LQeN7 qIypjc TrZEUc lw1w4b" jsname="LgbsSe" style="--mdc-ripple-fg-size: 48px; --mdc-ripple-fg-scale: 2.05975481471895; --mdc-ripple-fg-translate-start: 34.75px, -29.484375px; --mdc-ripple-fg-translate-end: 16.625px, -6px;">
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"input[type='password']\").focus(); })();").GetAwaiter();
            SendKeys(browser, Pass);
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"button[class='VfPpkd-LgbsSe VfPpkd-LgbsSe-OWXEXe-k8QpJ VfPpkd-LgbsSe-OWXEXe-dgl2Hf nCP5yc AjY5Oe DuMIQc LQeN7 qIypjc TrZEUc lw1w4b']\").click(); })();").GetAwaiter();
            Thread.Sleep(3000); //wait for the screen to load
            TakeScreenshot(browser);
            Thread.Sleep(3000);
        }

        private void PotwierdzżeToTy()
        {
            //<div class="lCoei YZVTmd SmR8" role="link" tabindex="0" jsname="EBHGs" data-challengeid="5" data-action="selectchallenge" data-accountrecovery="false" data-challengetype="12"><div class="wupBIe" aria-hidden="true"><svg aria-hidden="true" class="stUf5b" fill="currentColor" focusable="false" width="24px" height="24px" viewBox="0 0 24 24" xmlns="https://www.w3.org/2000/svg"><path d="M20 4H4c-1.1 0-1.99.9-1.99 2L2 18c0 1.1.9 2 2 2h16c1.1 0 2-.9 2-2V6c0-1.1-.9-2-2-2zm0 14H4V8l8 5 8-5v10zm-8-7L4 6h16l-8 5z"></path></svg></div><div class="vxx8jf">Confirm your recovery email</div></div>
            //click the above element on the webpage...
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"div[data-challengetype='12']\").click(); })();").GetAwaiter();
            Thread.Sleep(1000);
            TakeScreenshot(browser);
        }
        private void AllowIntelligentFunctioning()
        {
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"input[value='false']\").click(); })();").GetAwaiter();
            Thread.Sleep(1000);
            //< button name = "data_consent_dialog_next" class="J-at1-auR">Dalej</button>
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"button[class='J-at1-auR']\").click(); })();").GetAwaiter();
            Thread.Sleep(1000); //wait for the screen to kinda change
            //<button name="turn_off_in_product" class="J-at1-auR" jslog="111113; u014N:cOuCgd">Wyłącz funkcje</button>
            browser.EvaluateScriptAsync("(function(){ document.querySelector(\"button[class='J-at1-auR']\").click(); })();").GetAwaiter();
            Thread.Sleep(3000);
        }
        private void TakeScreenshot(ChromiumWebBrowser browser)
        {
            var task = browser.CaptureScreenshotAsync();
            task.ContinueWith(x =>
            {
                // File path to save our screenshot e.g. C:\Users\{username}\Desktop\CefSharp screenshot.png
                var screenshotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CefSharp screenshot.png");

                Console.WriteLine();
                Console.WriteLine("Screenshot ready. Saving to {0}", screenshotPath);

                var bitmapAsByteArray = x.Result;

                // Save the Bitmap to the path.
                File.WriteAllBytes(screenshotPath, bitmapAsByteArray);

                Console.WriteLine($"Screenshot saved.  Launching your default image viewer...\n{browser.Address}");

                // Tell Windows to launch the saved image.
                Process.Start(new ProcessStartInfo(screenshotPath)
                {
                    // UseShellExecute is false by default on .NET Core.
                    UseShellExecute = true
                });

                Console.WriteLine("Image viewer launched.  Press any key to exit.");
            }, TaskScheduler.Default);

        }
        private static void SendKeys(ChromiumWebBrowser browser, string text)
        {
            Thread.Sleep(3000);
            foreach (char c in text)
            {
                KeyEvent k = new KeyEvent();
                k.WindowsKeyCode = c;
                k.FocusOnEditableField = true;
                k.IsSystemKey = false;
                k.Type = KeyEventType.Char;
                browser.GetBrowser().GetHost().SendKeyEvent(k);
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);
        }
        private void InitializeDefaultValues()
        {
            List<URIItem> uriMap = new List<URIItem>
            {
                new URIItem{UrlPart=""},
                new URIItem{UrlPart=""},
                new URIItem{UrlPart="test.html"},
                new URIItem{UrlPart=""}
            };
            CachePath = @"Cefsharp\Cache\";
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36";
            Login = "testbotuserruler@gmail.com";
            Pass = "Testbotuserruler1.";
            Url = "https://accounts.google.com/v3/signin/identifier?dsh=S-922693188%3A1687696738993615&continue=https%3A%2F%2Fwww.google.com%2F&ec=GAZAmgQ&flowEntry=ServiceLogin&flowName=GlifWebSignIn&hl=pl&ifkv=Af_xneEUq8CAeZshMYEjntnqav66EJMl4L4jrfFgDPHInQ0089xYR3Y4A60Ed790yckxVm36zbWtiw&passive=true";
        }
    }
}
