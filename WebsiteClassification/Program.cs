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
            //DatasetCleanupHelper.CleanupDirectory(@"Data\DataSets", @"Data\CleanDataSets", @"Data\Other\CommonPrefixes\");
            Model m = null;
            m = Training();
            Evaluation(m);
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        static void Analysis()
        {
            var reader = new Reader();
            var categories = new Tuple<String, Target> [] {
                Tuple.Create("Accommodation", Target.Accommodation),
                Tuple.Create("Restaurant", Target.Restaurant),
                Tuple.Create("Retail", Target.Retail),
                Tuple.Create("Other", Target.Other) };
            var labeledEntities = new List<IEnumerable<MLEntity>>();
            
            foreach (var category in categories)
            {
                labeledEntities.Add(reader.EnumerateTarget(@"Data\DataSets\" + category.Item1, category.Item2));
            }

            var allEntities = labeledEntities[0].Union(labeledEntities[1]).Union(labeledEntities[2]).Union(labeledEntities[3]);

            var analyzer = new DatasetAnalyzer(allEntities);
            analyzer.Analyze();

        }
        static void Evaluation(Model m = null)
        {
            var reader = new Reader();
            var model = m == null ? new Model(@"Data\model") : m;
            var evaluator = new Evaluation(model);

            var results = evaluator.Evaluate(Entities());
            Logger.Log("Evaluation done");

            foreach (var result in results)
            {
                Console.WriteLine("{0,15}: {1}", result.Key, result.Value.ToString());
            }
        }

        static private List<MLEntity> entityList = null;

        static IEnumerable<MLEntity> Entities(string path = @"Data\DataSets", double entitiesSampleRate = 1)
        {
            if(entityList == null)
            {
                var reader = new Reader();
                entityList = reader.EnumerateAllTargets(path, entitiesSampleRate).ToList();
            }

            return entityList;
        }

        static Model Training()
        {   
            Logger.Log("Begin training");
            var reader = new Reader();
            var featurizer = new Featurizer();
         
            featurizer.Blacklist = new Blacklist(@"Data\Features\Blacklist.txt");

            var targets = new HashSet<Target>();
            foreach (Target t in Enum.GetValues(typeof(Target)))
            {
                targets.Add(t);
            }

            var featureSpace = featurizer.CreateFeatureSpace(Entities());
            Logger.Log("Feature space created, {0} features", featureSpace.Size);
            Logger.Log("Operating with {0} entities", featureSpace.NumEntities);

            var learner = new Learner(featurizer);
            var model = learner.Learn(Entities(), featureSpace.NumEntities, featureSpace, targets);
            Logger.Log("Model learned");

            model.Save(@"Data\model");
            Logger.Log("Model serialized to file");

            return model;
        }
    }
}
