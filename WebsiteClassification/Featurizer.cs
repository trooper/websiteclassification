namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.IO;
    using Accord.Statistics.Visualizations;

    public class Featurizer
    {
        private const int MinimumFeatureCount = 5;
        private Regex regex;
        private HTMLStorage storage;

        public Featurizer()
        {
            var pattern = @"[\w]+";
            this.regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
            //storage = new HTMLStorage();
        }

        public Blacklist Blacklist { get; set; }

        public FeatureSpace CreateFeatureSpace(IEnumerable<MLEntity> entities)
        {
            Random rnd = new Random(42);
            float byClassThreshold = 0.04f;
            var featureSpace = new FeatureSpace();
            int numberOfTargets = Enum.GetValues(typeof(Target)).Length;

            var targetCount = new Dictionary<Target, int>();
            foreach (Target t in Enum.GetValues(typeof(Target)))
            {
                targetCount.Add(t, 0);
            }

            var freqTable = new Dictionary<string, Dictionary<Target, int>>();

            long entitiesxfeatures = 0;

            featureSpace.NumEntities = 0;
            foreach (var entity in entities)
            {
                //LoadEntityPagesHTML(entity);
                ++featureSpace.NumEntities;
                foreach (var feature in this.ExtractMetaFeatures(entity).Concat(this.ExtractFeaturesFromUnigrams(entity.WebSite)))
                {
                    if (!freqTable.ContainsKey(feature.Name))
                    {
                        freqTable.Add(feature.Name, new Dictionary<Target, int>());
                        foreach (Target target in Enum.GetValues(typeof(Target)))
                        {
                            freqTable[feature.Name].Add(target, 0);
                        }
                    }
                    freqTable[feature.Name][entity.Label]++;
                    entitiesxfeatures++;
                }
                targetCount[entity.Label]++;
            }

            MutualInformationCalculator mutualCalc = new MutualInformationCalculator(targetCount);
            long totalEntities = featureSpace.NumEntities;

            SortedSet<KeyValuePair<Feature, double>> priority = new SortedSet<KeyValuePair<Feature, double>>(new FeatureComparer());
            
            const int howManyRandomFeatures = 200;
            float randomThreshold = (float)howManyRandomFeatures / (freqTable.Count * Enum.GetValues(typeof(Target)).Length);

            foreach (var featureFreq in freqTable)
            {
                var totalFreq = featureFreq.Value.Values.Sum();
                bool useFeature = false;

                foreach (Target t in Enum.GetValues(typeof(Target)))
                {
                    if ((float)featureFreq.Value[t] / targetCount[t] > byClassThreshold)
                    {
                        useFeature = true;
                        break;
                    }
                }
                if (!useFeature) continue;

                List<double> normalizedScores = new List<double>();

                foreach (Target target in Enum.GetValues(typeof(Target)))
                {
                    normalizedScores.Add(mutualCalc.Calculate(featureFreq.Value, target));
                }

                var normalizedScoreThreshold = 0.0;
                var normalizedScoreCeil = 300;
                var featureTag = featureFreq.Key.Split(':').First();

                if (normalizedScores.Any<double>(x => x > normalizedScoreThreshold && x < normalizedScoreCeil))
                {
                    /*featureSpace.featureTypeCount[featureTag]++;*/
                    /*featureSpace.AddFeature(new Feature()
                    {
                        Name = featureFreq.Key,
                        Value = 1.0
                    });*/
                    priority.Add(new KeyValuePair<Feature, double>(new Feature()
                    {
                        Name = featureFreq.Key,
                        Value = 1.0
                    }, normalizedScores.Max()));
                }
                else if (featureFreq.Key.StartsWith("r"))
                {
                    Blacklist.Add(featureFreq.Key);
                }
            }

            int howMuchIsLeft = 250;
            while(howMuchIsLeft-- > 0)
            {
                featureSpace.AddFeature(priority.Max.Key);
                featureSpace.featureTypeCount[priority.Max.Key.Name.Split(':').First()]++;
                priority.Remove(priority.Max);
            }

            return featureSpace;
        }

        public IEnumerable<Feature> ExtractFeaturesFromUnigrams(WebSite webSite)
        {
            const string unigramsXPath = "/html/body//text()";
            var ngrams = new HashSet<string>();
            foreach (var page in webSite.Pages)
            {
                foreach (var ngram in this.ExtractXPath(page, unigramsXPath))
                {
                    if (!ngrams.Contains(ngram))
                    {
                        ngrams.Add(ngram);

                        var value = Feature.Type.RawText + ":" + ngram;
                        bool use = false;

                        if (this.Blacklist != null)
                        {
                            use = !this.Blacklist.Contains(value);
                        }
                        else
                        {
                            use = true;
                        }

                        if (use)
                        {
                            yield return new Feature
                            {
                                Name = value,
                                Value = 1.0 // we use n-grams as binary features
                            };
                        }
                    }
                }
            }
            this.Blacklist.Save();
        }
        
        public double[] CreateFeatureVector(WebSite webSite, FeatureSpace featureSpace)
        {
            var vector = new double[featureSpace.Size];
            foreach (var feature in this.ExtractMetaFeatures(webSite))
            {
                var index = featureSpace.GetFeatureIndex(feature);
                if (index.HasValue)
                {
                    vector[index.Value] = feature.Value;
                }
            }

            return vector;
        }

        public void LoadEntityPagesHTML(MLEntity entity)
        {
            foreach(var page in entity.WebSite.Pages)
            {
                storage.Add(page);
            }
        }

        public IEnumerable<Feature> ExtractMetaFeatures(MLEntity entity)
        {
            return this.ExtractMetaFeatures(entity.WebSite);
        }

        public IEnumerable<Feature> ExtractMetaFeatures(WebSite webSite)
        {
            const string metaTagXPath= "/html/head/meta[@name=\"description\" or @name=\"keywords\"]/@content | /html/head/title/@content | //h1/@content | //h2/@content";

            var ngrams = new HashSet<string>();
            foreach (var page in webSite.Pages)
            {
                foreach (var ngram in this.ExtractXPath(page, metaTagXPath))
                {
                    if (!ngrams.Contains(ngram))
                    {
                        ngrams.Add(ngram);

                        var value = Feature.Type.MetaTag + ":" + ngram;
                        bool use = false;

                        if (this.Blacklist != null)
                        {
                            use = !this.Blacklist.Contains(value);
                        }
                        else
                        {
                            use = true;
                        }

                        if (use)
                        {
                            yield return new Feature
                            {
                                Name = value,
                                Value = 1.0 // we use n-grams as binary features
                            };
                        }
                    }
                }
            }
        }

        public IEnumerable<string> ExtractXPath(WebPage page, string xPath)
        {
            //var document = storage.GetDocument(page);
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(page.Content);

            if (String.IsNullOrEmpty(page.Content))
            {
                yield return "empty";
            }
            else
            {
                var nodes = document.DocumentNode.SelectNodes(xPath);

                if (nodes != null)
                {
                    if (!nodes.Any()) throw new Exception("nece da moze");
                    foreach (var node in nodes)
                    {
                        foreach (Match token in regex.Matches(node.OuterHtml))
                        {
                            if (token.Value.Length > 3)
                                yield return token.ToString();
                        }
                    }
                }
            }
        }

        private void RemoveInfrequentFeatures(bool[] disabledFeatures, Dictionary<int, int[]> featureMatrix)
        {
            foreach (var feature in featureMatrix)
            {
                int totalAppearance = feature.Value.Sum();
                if (!disabledFeatures[feature.Key])
                {
                    disabledFeatures[feature.Key] = totalAppearance < MinimumFeatureCount;
                }
            }
        }

        private int[] InitializeFeatureMatrix(int numberOfTargets)
        {
            var matrix = new int[numberOfTargets];
            for (int i = 0; i < numberOfTargets; ++i)
            {
                matrix[i] = 0;
            }

            return matrix;
        }

        class FeatureComparer : IComparer<KeyValuePair<Feature, double>>
        {
            public int Compare(KeyValuePair<Feature, double> x, KeyValuePair<Feature, double> y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }
    }
}
