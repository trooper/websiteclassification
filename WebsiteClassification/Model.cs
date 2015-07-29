namespace PsiMl.WebsiteClasification
{
    using Accord.Statistics.Models.Regression;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class Model
    {
        private MultinomialLogisticRegression regression;
        private const string SuffixModel = ".weights.txt";
        private const string SuffixFeatures = ".features.txt";

        static public Dictionary<Target, int> GetTargetToInt(HashSet<Target> targets)
        {
            var mapping = new Dictionary<Target, int>();
            int count = 0;
            foreach (var target in targets.OrderBy(t => (int)t))
            {
                mapping[target] = count;
                ++count;
            }

            return mapping;
        }

        public Model(string path)
        {
            this.Load(path);
        }

        public Model(MultinomialLogisticRegression regression, Featurizer featurizer, FeatureSpace featureSpace, HashSet<Target> targets)
        {
            this.regression = regression;
            this.FeatureSpace = featureSpace;
            this.Featurizer = featurizer;
            this.Targets = targets;
        }

        public FeatureSpace FeatureSpace { get; private set; }

        public Featurizer Featurizer { get; private set; }

        public HashSet<Target> Targets { get; private set; }

        public Prediction Classify(WebSite webSite)
        {
            var featureVector = this.Featurizer.CreateFeatureVector(webSite, this.FeatureSpace);
            var confidences = this.regression.Compute(featureVector);

            var prediction = new Prediction();
            prediction.Confidences = new Dictionary<Target, double>();
            var mapping = Model.GetTargetToInt(this.Targets);
            foreach (var target in this.Targets)
            {
                prediction.Confidences.Add(target, confidences[mapping[target]]);
            }

            return prediction;
        }

        public void Save(string path)
        {
            string modelFile = path + SuffixModel;
            string featuresFile = path + SuffixFeatures;

            var vectors = this.regression.Coefficients;
            using (var writer = new StreamWriter(modelFile))
            {
                writer.WriteLine(string.Join("\t", this.Targets));
                foreach (var vector in vectors)
                {
                    writer.WriteLine(string.Join("\t", vector));
                }
            }

            this.FeatureSpace.SaveToFile(featuresFile);
        }

        private void Load(string path)
        {
            string modelFile = path + SuffixModel;
            string featuresFile = path + SuffixFeatures;
            double[][] weights = null;

            int targetCount = 0;
            using (var reader = new StreamReader(modelFile))
            {
                this.Targets = new HashSet<Target>(reader.ReadLine().Split('\t').Select(t => (Target)Enum.Parse(typeof(Target), t)));
                targetCount = this.Targets.Count;
                weights = new double[targetCount][];

                int count = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    weights[count] = line.Split('\t').Select(t => double.Parse(t)).ToArray();
                    ++count;
                }
            }

            this.regression = new MultinomialLogisticRegression(weights[0].Length, targetCount);
            this.regression.Coefficients = weights;
            this.FeatureSpace = WebsiteClasification.FeatureSpace.LoadFromFile(featuresFile);
            this.Featurizer = new Featurizer();
        }
    }
}