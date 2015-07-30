using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsiMl.WebsiteClasification
{
    class DatasetAnalyzer
    {
        private IEnumerable<MLEntity> entities;

        public DatasetAnalyzer(IEnumerable<MLEntity> entities)
        {
            this.entities = entities;
        }

        public void Analyze()
        {
            var labeledCount = new Dictionary<Target, int>();
            var totalCount = new Dictionary<Target, int>();
            var keywordCount = new Dictionary<Target, Dictionary<string, int>>();

            foreach (Target t in Enum.GetValues(typeof(Target)))
            {
                labeledCount.Add(t, 0);
                totalCount.Add(t, 0);
                keywordCount.Add(t, new Dictionary<string, int>());
            }

            foreach(var entity in entities)
            {
                var webSite = entity.WebSite;
                long count = 0;
                foreach(var page in webSite.Pages)
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(page.Content);
                    var nodes = document.DocumentNode.SelectNodes("/html/head/meta[@name=\"description\" or @name=\"keywords\"]/@content");

                    if(nodes != null)
                    {
                        foreach (var node in nodes)
                        {
                            if (!String.IsNullOrEmpty(node.Attributes["content"].Value))
                            {
                                count++;
                                IEnumerable<string> words = node.Attributes["content"].Value.Split(new char[] { ' ', ',' });
                                foreach(var word in words)
                                {
                                    int val = 0;
                                    keywordCount[entity.Label].TryGetValue(word, out val);
                                    keywordCount[entity.Label][word] = val + 1;
                                }
                            }

                        }
                    } 
                }

                totalCount[entity.Label] ++;
                if(count > 0)
                {
                    labeledCount[entity.Label]++;
                }
            }
            foreach( Target t in Enum.GetValues(typeof ( Target)))
            {
                Logger.Log("{0} : {1} / {2} = {3}", t.ToString(), labeledCount[t], totalCount[t], (double)labeledCount[t] / totalCount[t]);
            }
            foreach (var dict in keywordCount)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"Data\Other\KeywordCounts\"+dict.Key.ToString()+".txt"))
                {
                    foreach(var pair in dict.Value)
                    {
                        sw.WriteLine("{0, 8} {1}", pair.Value, pair.Key);
                    }
                }
            }
        }
    }
}
