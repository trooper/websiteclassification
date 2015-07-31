using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace PsiMl.WebsiteClasification
{
    public class HTMLStorage
    {
        public Dictionary<string, HtmlDocument> Cache
        {
            get;
            set;
        }

        public HTMLStorage()
        {
            Cache = new Dictionary<string, HtmlDocument>();
        }

        public void Add(WebPage webPage)
        {
            var document = new HtmlDocument();
            document.LoadHtml(webPage.Content.ToLower());
            Cache.Add(webPage.Url, document);
        }

        public HtmlDocument GetDocument(WebPage webPage)
        {
            return Cache[webPage.Url];
        }
    }
}
