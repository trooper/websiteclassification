namespace PsiMl.WebsiteClasification
{
    using Accord.Statistics.Models.Regression;
    using Accord.Statistics.Models.Regression.Fitting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Learner
    {
        private Featurizer featurizer;

        public Learner(Featurizer featurizer)
        {
            this.featurizer = featurizer;
        }

        public Model Learn(IEnumerable<MLEntity> entities, int numberOfEntities, FeatureSpace featureSpace, HashSet<Target> targets)
        {
            var featureMatrix = new double[numberOfEntities][];
            var labels = new int[numberOfEntities];

            int counter = 0;
            var targetToInt = Model.GetTargetToInt(targets);

            foreach (var entity in entities)
            {
                featureMatrix[counter] = this.featurizer.CreateFeatureVector(entity.WebSite, featureSpace);
                labels[counter] = targetToInt[entity.Label];
                ++counter;
            }

            Logger.Log("Features extracted");

            var regression = new MultinomialLogisticRegression(inputs: featureSpace.Size, categories: targets.Count);
            LowerBoundNewtonRaphson lbnr = new LowerBoundNewtonRaphson(regression);

            double delta;
            int iteration = 0;
            do
            {
                Logger.Log("Iteration: {0}", iteration);
                delta = lbnr.Run(featureMatrix, labels);
                iteration++;
            } while (iteration < 20 && delta > 1e-6);

            return new Model(regression, this.featurizer, featureSpace, targets);
        }
    }
}
