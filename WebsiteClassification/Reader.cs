namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Collections.Generic;

    public class Reader
    {
        public MLEntity Read(string file, string domain, Target label, double sampleRate = 1.0)
        {
            var random = new Random();
            MLEntity entity = new MLEntity();
            entity.Label = label;
            entity.WebSite = new WebSite()
            { 
                Domain = domain,
                Pages = new List<WebPage>()
            };

            foreach (var line in System.IO.File.ReadLines(file))
            {
                if (random.NextDouble() <= sampleRate)
                {
                    var tokens = line.Split('\t');
                    var page = this.CreatePage(tokens);
                    entity.WebSite.Pages.Add(page);
                }
            }

            return entity;
        }

        public IEnumerable<MLEntity> ReadAll(string directory, Target label, double sampleRate = 1.0)
        {
            var random = new Random();
            var files = System.IO.Directory.GetFiles(directory);
            foreach (var file in files)
            {
                long size = new System.IO.FileInfo(file).Length;
                if (random.NextDouble() <= sampleRate && size < 1024*1024)
                {
                    string domain = System.IO.Path.GetFileName(file);
                    yield return this.Read(file, domain, label);
                }
            }
        }

        private WebPage CreatePage(string[] tokens)
        {
            return new WebPage
            {
                Content = tokens[2],
                Url = tokens[3]
            };
        }
    }
}
