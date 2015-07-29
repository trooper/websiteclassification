namespace PsiMl.WebsiteClasification
{
    using Accord.MachineLearning.Bayes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Logger.Initialize();

            // var model = Training();
            Evaluation(); //model);
            Console.ReadLine();
        }

        static void Evaluation(Model m = null)
        {
            var reader = new Reader();
            var model = m == null ? new Model(@"d:\Data\model") : m;
            var evaluator = new Evaluation(model);

            var positive = reader.ReadAll(@"d:\Data\DataSets\Restaurant", Target.Restaurant);
            var negative = reader.ReadAll(@"d:\Data\DataSets\Other", Target.Other);

            var entities = positive.Union(negative);
            Logger.Log("Sets loaded");

            var results = evaluator.Evaluate(entities);
            Logger.Log("Evaluation done");

            foreach (var result in results)
            {
                Console.WriteLine("{0,15}: {1}", result.Key, result.Value.ToString());
            }
        }

        static Model Training()
        {
            var reader = new Reader();
            var featurizer = new Featurizer();
            featurizer.Whitelist = new HashSet<string>(System.IO.File.ReadLines(@"d:\Data\Features\RestaurantWhitelist.txt").Select(l => l.Split('\t').First()));

            var positive = reader.ReadAll(@"d:\Data\DataSets\Restaurant", Target.Restaurant);
            var negative = reader.ReadAll(@"d:\Data\DataSets\Other", Target.Other);

            var entities = positive.Union(negative).ToArray();
            Logger.Log("Sets loaded");

            var featureSpace = featurizer.CreateFeatureSpace(entities);
            Logger.Log("Feature space created");

            var learner = new Learner(featurizer);

            var targets = new HashSet<Target>() { Target.Restaurant, Target.Other };
            var model = learner.Learn(entities, entities.Length, featureSpace, targets);
            Logger.Log("Model learned");

            model.Save(@"d:\Data\model");
            Logger.Log("Model serialized to file");

            return model;
        }
    }
}
