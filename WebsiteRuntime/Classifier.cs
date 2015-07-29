namespace PsiMl.WebsiteRuntime
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;

    using PsiMl.WebsiteClasification;

    public class Classifier
    {
        private static PsiMl.WebsiteClasification.Model model;
        private static Fetcher fetcher = new Fetcher();

        public static Target FetchAndClassify(string url, int depth)
        {
            WebSite website = fetcher.Fetch(url, depth);
            return model.Classify(website).Label;
        }

        internal static void InitializeClassifier()
        {
            model = new PsiMl.WebsiteClasification.Model(null);

            // classifier.Initialize(model)
        }

        
    }
}
