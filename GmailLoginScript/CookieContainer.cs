using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmailLoginScript
{
    class CookieVisitor : ICookieVisitor
    {
        readonly List<(string Name, string Value, string Path, string Domain)> cookies = new List<(string Name, string Value, string Path, string Domain)>();

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            cookies.Add((cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            return true;
        }

        public void Dispose()
        {

        }

        public IEnumerable<(string Name, string Value, string Path, string Domain)> Cookies
        {
            get { return cookies; }
        }
    }
}
