namespace PsiMl.WebsiteClasification
{
    using HtmlAgilityPack;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class WebTools
    {
        private const string DomainSuffixes = @"d:\Data\DomainSufixes.txt";
        private const string DomainPrefixes = @"d:\Data\DomainPrefixes.txt";
        private static HashSet<string> NonTextTags = new HashSet<string> { "script", "style" };

        public static string UnescapeTsv(string rawHtml)
        {
            return rawHtml.Replace("#N#", "\n").Replace("#R#", "\r").Replace("#TAB#", "\t");
        }

        public static string GetRawTextFromPage(WebPage page)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page.Content);
            return GetRawFromHTML(doc);
        }

        public static string GetRawFromHTML(HtmlDocument document)
        {
            if (document == null || document.DocumentNode == null)
            {
                return "";
            }

            var buffer = new StringBuilder();
            var nodes = document.DocumentNode.SelectNodes("//text()");
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    if (node.ParentNode != null && NonTextTags.Contains(node.ParentNode.Name))
                    {
                        continue;
                    }

                    buffer.Append(node.InnerText);
                }
            }

            return buffer.ToString();
        }

        public class DomainHelper
        {
            private static DomainHandler domainHandler = new DomainHandler(DomainSuffixes);
            public static string[] prefixes = File.ReadAllLines(DomainPrefixes);

            public static string GetDomain(string uri)
            {
                return domainHandler.normalizeDomain(uri);
            }
        }

/////////////////////////////////////////////////////////
// Here be dragons
/////////////////////////////////////////////////////////
        class Trie
        {
            class Node
            {
                Dictionary<char, Node> nxt;
                bool isTerminate;

                public Node()
                {
                    nxt = new Dictionary<char, Node>();
                    isTerminate = false;
                }

                public void add(string s, int i)
                {
                    if (i == s.Length)
                    {
                        isTerminate = true;
                        return;
                    }
                    if (nxt.ContainsKey(s[i]) == false)
                    {
                        nxt.Add(s[i], new Node());
                    }
                    nxt[s[i]].add(s, i + 1);
                }

                public int getLongestMatchLength(string s, int i)
                {
                    int res = -1;
                    if (i == s.Length) return res;
                    if (isTerminate) res = i;

                    if (nxt.ContainsKey(s[i]))
                    {
                        res = Math.Max(res, nxt[s[i]].getLongestMatchLength(s, i + 1));
                    }

                    return res;
                }
            }

            Node root;

            public Trie()
            {
                root = new Node();
                root.add("", 0);
            }

            public void add(string s)
            {
                root.add(s, 0);
            }

            public int getLongestMatchLength(string s)
            {
                return root.getLongestMatchLength(s, 0);
            }
        }

        class DomainHandler
        {
            private Trie trie;

            private static string reverse(string s)
            {
                char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
            }

            public DomainHandler(string path)
            {
                trie = new Trie();
                int id = 0;
                foreach (var line in File.ReadLines((path)))
                {
                    string item = reverse(line);
                    trie.add(item);
                    id++;
                }
            }

            private int getLongestSuffixLength(string s)
            {
                string rev = reverse(s);
                return trie.getLongestMatchLength(rev);
            }

            public string normalizeDomain(string s)
            {
                try
                {
                    Uri uri;

                    try
                    {
                        uri = new Uri(s);
                    }
                    catch
                    {
                        uri = new Uri("http://" + s);
                    }

                    var domain = uri.Host;

                    foreach (string prefix in DomainHelper.prefixes)
                        if (domain.StartsWith(prefix))
                            domain = domain.Substring(prefix.Length);


                    var longestSufixLength = getLongestSuffixLength(domain);
                    if (longestSufixLength > 0)
                    {
                        domain = domain.Substring(0, domain.Length - longestSufixLength);
                    }
                    return domain;
                }
                catch
                {
                    return string.Empty;
                }
            }

        }
    }
}
