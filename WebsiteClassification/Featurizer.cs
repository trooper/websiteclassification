namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

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

            // Frequency matrix, featureMatrix[k][t] indicates the number of appearances of feature K in entities of target T
            // var featureMatrix = new Dictionary<int, int[]>();

            foreach (var entity in entities)
            {
                foreach (var feature in this.ExtractFeatures(entity))
                {
                    var featureIndex = featureSpace.GetFeatureIndex(feature);
                    if (featureIndex == null)
                    {
                        featureSpace.AddFeature(feature);
                    }

                    //int[] matrix;
                    //if (!featureMatrix.TryGetValue(featureIndex, out matrix))
                    //{
                    //    matrix = this.InitializeFeatureMatrix(numberOfTargets);
                    //    featureMatrix[featureIndex] = matrix;
                    //}

                    //++matrix[(int)entity.Label];
                }
            }

            // int numberOfFeatures = featureToIndex.Count;
            // var disabledFeatures = new bool[numberOfFeatures];
            // this.RemoveInfrequentFeatures(disabledFeatures, featureMatrix
            // pack the features to remove unused ones

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
