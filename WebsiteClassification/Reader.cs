namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class Reader
    {
        public MLEntity ReadFile(string file, string domain, Target label, double sampleRate = 1.0)
        {
            var random = new Random(42);
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

        public IEnumerable<MLEntity> EnumerateTarget(string directory, Target label, double sampleRate = 1.0)
        {
            var random = new Random(42); // reci NE nedeterminizmu
            var files = System.IO.Directory.GetFiles(directory);
            int count = 0;

            foreach (var file in files)
            {
                long size = new System.IO.FileInfo(file).Length;
                if (random.NextDouble() <= sampleRate && size < 1024 * 1024)
                {
                    string domain = System.IO.Path.GetFileName(file);
                    ++count;
                    yield return this.ReadFile(file, domain, label);
                }
            }
            Logger.Log("{0} entities with label {1}", count, label.ToString());
        }

        public IEnumerable<MLEntity> EnumerateAllTargets(string directory, double sampleRate = 1.0)
        {
            Logger.Log("Enumerating all targets");
            foreach (Target t in Enum.GetValues(typeof(Target)))
            {
                foreach(var entity in EnumerateTarget(Path.Combine(directory, t.ToString()), t, sampleRate))
                {
                    yield return entity;
                }
            }
        }

        private WebPage CreatePage(string[] tokens)
        {
            return new WebPage
            {
                Content = WebTools.UnescapeTsv(tokens[2]),
                Url = tokens[0]
            };
        }
    }
}
