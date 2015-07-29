namespace PsiMl.WebsiteRuntime
{
    using HtmlAgilityPack;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using PsiMl.WebsiteClasification;
    using System.Collections.Concurrent;
    using System.Threading;
    using System;

    class Fetcher
    {
        private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
        private const int MaxThreads = 4;
        
        private class FetchItem
        {
            public string Url { get; set; }

            public int Depth { get; set; }
        }

        public WebSite Fetch(string url, int depth = 0)
        {
            var webSite = new WebSite();
            webSite.Domain = WebTools.DomainHelper.GetDomain(url);
            var pages = new List<WebPage>();
            webSite.Pages = pages;

            var queue = new ConcurrentQueue<FetchItem>();
            var visited = new HashSet<string>();

            queue.Enqueue(new FetchItem { Depth = 0, Url = url });

            Semaphore semaphore = new Semaphore(1, MaxThreads);
            FetchItem item;
            var mutex = new object();
            while (true)
            {
                lock(mutex)
                {
                    semaphore.WaitOne();
                    if (!queue.TryDequeue(out item))
                    {
                        break;
                    }

                    new Thread(() => FetchAndAdd(semaphore, item, depth, webSite.Domain, visited, queue, webSite.Pages)).Start();
                }
            }

            return webSite;
        }
        
        private void FetchAndAdd(Semaphore semaphore, FetchItem item, int depth, string domain, HashSet<string> visited, ConcurrentQueue<FetchItem> queue, List<WebPage> pages)
        {
            lock(visited)
            {
                visited.Add(item.Url.ToLower());
            }

            var page = this.FetchPage(item.Url);

            lock(pages)
            {
                pages.Add(page);
            }

            if (item.Depth < depth)
            {
                foreach (var childUrl in this.ExtractUrls(page))
                {
                    string url;
                    if (!childUrl.StartsWith("http"))
                    {
                        url = new Uri(new Uri(page.Url), childUrl).ToString();
                    }
                    else
                    {
                        url = childUrl;
                    }

                    // We don't cross domain boundary
                    if (WebTools.DomainHelper.GetDomain(url) == domain)
                    {
                        lock (visited)
                        {
                            // Make sure we don't enter a cycle
                            if (!visited.Contains(url.ToLower()))
                            {
                                queue.Enqueue(new FetchItem { Depth = item.Depth + 1, Url = url });
                            }
                        }
                    }
                }
            }

            semaphore.Release();
        }

        private IEnumerable<string> ExtractUrls(WebPage page)
        {
            // Overhead, we are essentially parsing a web page twice (once for links, second time for content)
            HtmlWeb hw = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page.Content);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                yield return hrefValue;
            }
        }

        private WebPage FetchPage(string url)
        {
            var page = new WebPage();
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = DefaultUserAgent;
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var html = reader.ReadToEnd();
                    page.Content = html;
                    page.Url = url;
                }
            }

            return page;
        }
    }
}
