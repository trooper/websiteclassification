namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FeatureSpace
    {
        // Keep a mapping from each unique feature name to a unique index.
        private Dictionary<string, int> features = new Dictionary<string, int>();
        public Dictionary<string, int> featureTypeCount
        {
            get; set;
        }

        private int numEntities;

        public FeatureSpace()
        {
            featureTypeCount = new Dictionary<string, int>();
            foreach(var type in typeof(Feature.Type).GetFields())
            {
                featureTypeCount.Add((string)type.GetRawConstantValue(), 0);
            }
        }

        public int NumEntities
        {
            get
            {
                return numEntities;
            }
            set
            {
                numEntities = value;
            }
        }

        public Dictionary<string, int> Features
        {
            get
            {
                return features;
            }
        }


        public static FeatureSpace LoadFromFile(string path)
        {
            int count = 0;
            var featureSpace = new FeatureSpace();
            foreach (var line in System.IO.File.ReadLines(path))
            {
                featureSpace.features.Add(line, count);
                ++count;
            }

            return featureSpace;
        }

        public int Size
        {
            get
            {
                return this.features.Count;
            }
        }

        public void SaveToFile(string path)
        {
            System.IO.File.WriteAllLines(path, this.features.OrderBy(kv => kv.Value).Select(kv => kv.Key));
        }

        public int? GetFeatureIndex(Feature feature)
        {
            int index;
            if (this.features.TryGetValue(feature.Name, out index))
            {
                return index;
            }
            else
            {
                return null;
            }
        }

        public int AddFeature(Feature feature)
        {
            int count = this.features.Count;
            if (!this.features.ContainsKey(feature.Name))
                this.features.Add(feature.Name, count);
            return count;
        }
    }
}