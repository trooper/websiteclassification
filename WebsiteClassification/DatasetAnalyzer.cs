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

            foreach(var entity in entities)
            {
                var webSite = entity.WebSite;
                long count = 0;
                foreach(var page in webSite.Pages)
                {
                    var document = new HtmlDocument();
                    document.LoadHtml(page.Content);
                    var nodes = document.DocumentNode.SelectNodes("/html/head/meta[@name=\"description\"]/@content");

                    /*
                    if (nodes == null)
                        nodes = document.DocumentNode.SelectNodes("/html/head/meta[@name=\"keywords\"]/@content");
                    else
                        nodes = (HtmlNodeCollection)nodes.Union(document.DocumentNode.SelectNodes("/html/head/meta[@name=\"keywords\"]/@content"));
                    */

                    if(nodes != null)
                    {
                        foreach (var node in nodes)
                        {
                            if (!String.IsNullOrEmpty(node.Attributes["content"].Value)) count++;
                        }
                    } 
                }
                int oldCount = 0;
                totalCount.TryGetValue(entity.Label, out oldCount);
                totalCount[entity.Label] = oldCount + 1;

                if(count > 0)
                {
                    int oldCount2 = 0;
                   labeledCount.TryGetValue(entity.Label, out oldCount2);
                    labeledCount[entity.Label] = oldCount2 + 1;

                    //Console.WriteLine("{0} : {1}", entity.Label.ToString(), webSite.Domain);
                }
            }
            foreach( Target t in Enum.GetValues(typeof ( Target)))
            {
                Logger.Log("{0} : {1} / {2} = {3}", t.ToString(), labeledCount[t], totalCount[t], (double)labeledCount[t] / totalCount[t]);
            }
        }
    }
}
