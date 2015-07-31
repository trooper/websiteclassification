﻿namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Accord.Statistics.Visualizations;

    public class Featurizer
    {
        private const int MinimumFeatureCount = 5;
        private Regex regex;

        public Featurizer()
        {
            // ignore everything in pattern regex 
            //var pattern = @"[^\s \t \r\n,;.!`>@$*\[\]()_\-?{}""'/:|#&]+"; // za splitovanje
            var pattern = @"[\w]+";
            this.regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
        }

        public HashSet<string> Whitelist { get; set; }

        public HashSet<string> Blacklist { get; set; }

        public FeatureSpace CreateFeatureSpace(IEnumerable<MLEntity> entities)
        {
            var featureSpace = new FeatureSpace();
            int numberOfTargets = Enum.GetValues(typeof(Target)).Length;

            var targetCount = new Dictionary<Target, int>();
            foreach(Target t in Enum.GetValues(typeof(Target)))
            {
                targetCount.Add(t, 0);
            }
           
            var freqTable = new Dictionary<string, Dictionary<Target, int>>();

            foreach (var entity in entities)
            {
                foreach (var feature in this.ExtractFeatures(entity))
                {
                    var featureIndex = featureSpace.GetFeatureIndex(feature);
                    if (featureIndex == null)
                    {
                        if (!freqTable.ContainsKey(feature.Name))
                        {
                            freqTable[feature.Name][entity.Label] = 0;
                        }
                        if (!freqTable[feature.Name].ContainsKey(entity.Label))
                        {
                            freqTable[feature.Name][entity.Label] = 0;
                        }
                        freqTable[feature.Name][entity.Label]++;
                    }
                    targetCount[entity.Label]++;
                }
            }
            using (var writer = new System.IO.StreamWriter(@"histogram_data.csv"))
            {
                long totalEntities = entities.Count<MLEntity>();
                foreach (var featureFreq in freqTable)
                {
                    var totalFreq = featureFreq.Value.Values.Sum();
                    foreach (Target target in Enum.GetValues(typeof(Target)))
                    {
                        float[,] M = new float[2, 2];

                        M[0, 0] = (1 / totalFreq) * (totalEntities - totalFreq - targetCount[target] + featureFreq.Value[target]);
                        M[0, 1] = (1 / totalFreq) * (totalFreq - featureFreq.Value[target]);
                        M[1, 0] = (1 / totalFreq) * (targetCount[target] - featureFreq.Value[target]);
                        M[1, 1] = (1 / totalFreq) * (featureFreq.Value[target]);

                        float pci = totalFreq / (float)totalEntities;
                        float pfj = totalFreq / (float)totalEntities;

                        float[,] A = new float[2, 2];
                        A[0, 0] = (float)(M[0, 0] * Math.Log(M[0, 0] / (1 - pci) * (1 - pfj)));
                        A[0, 1] = (float)(M[0, 1] * Math.Log(M[0, 1] / (1 - pci) * pfj));
                        A[1, 0] = (float)(M[1, 0] * Math.Log(M[1, 0] / pci * (1 - pfj)));
                        A[1, 1] = (float)(M[1, 1] * Math.Log(M[1, 1] / pci * pfj));

                        var score = A.Cast<float>().Sum();
                        var probF = -(pfj * Math.Log(pfj) + (1 - pfj) * Math.Log(1 - pfj));
                        var probC = -(pci * Math.Log(pci) + (1 - pci) * Math.Log(1 - pci));

                        var normalizedScore = score / Math.Min(probC, probF);
                        writer.WriteLine("{0},{1},{2}", target.ToString(), featureFreq.Key, normalizedScore);
                    }
                }
            }

            return featureSpace;
        }

        public double[] CreateFeatureVector(WebSite webSite, FeatureSpace featureSpace)
        {
            var vector = new double[featureSpace.Size];
            foreach (var feature in this.ExtractFeatures(webSite))
            {
                var index = featureSpace.GetFeatureIndex(feature);
                if (index.HasValue)
                {
                    vector[index.Value] = feature.Value;
                }
            }

            return vector;
        }

        public IEnumerable<Feature> ExtractFeatures(MLEntity entity)
        {
            return this.ExtractFeatures(entity.WebSite);
        }

        public IEnumerable<Feature> ExtractFeatures(WebSite webSite)
        {
            foreach (var page in webSite.Pages)
            {
                var ngrams = new HashSet<string>();

                foreach (var ngram in this.ExtractNGrams(page))
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
        }

        public IEnumerable<string> ExtractNGrams(WebPage page)
        {
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(page.Content.ToLower());

            if (String.IsNullOrEmpty(page.Content))
            {
                yield return "empty";
            }

            else
            {
                var nodes = document.DocumentNode.SelectNodes(
                    "//meta[@name=\"description\" or @name=\"keywords\"]/@content");
                    //"or //h1/text() or //h2/text() or //h3/text() or //title/text()");
                //var nodes = document.DocumentNode.SelectNodes("//title/text()");

                if (nodes != null)
                {
                    if (!nodes.Any()) throw new Exception("nece da moze");
                    foreach (var node in nodes)
                    {
                        foreach (Match token in regex.Matches(node.OuterHtml))
                        {
                            if(token.Value.Length > 3)
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
    }
}
