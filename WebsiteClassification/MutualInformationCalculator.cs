using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsiMl.WebsiteClasification
{
    public class MutualInformationCalculator
    {
        private Dictionary<Target, int> targetCount;

        private int EntityCount
        {
            get
            {
                return targetCount.Values.Sum();
            }
        }

        public MutualInformationCalculator(Dictionary<Target, int> targetCount)
        {
            this.targetCount = targetCount;
        }

        public double Calculate(Dictionary<Target, int> featureFrequency, Target target)
        {
            float[,] M = new float[2, 2];
            int totalFeatureCount = featureFrequency.Values.Sum();

            M[0, 0] = (1f / EntityCount) * (EntityCount - totalFeatureCount - targetCount[target] + featureFrequency[target]);
            M[0, 1] = (1f / EntityCount) * (totalFeatureCount - featureFrequency[target]);
            M[1, 0] = (1f / EntityCount) * (targetCount[target] - featureFrequency[target]);
            M[1, 1] = (1f / EntityCount) * (featureFrequency[target]);

            float pci = totalFeatureCount / (float)EntityCount;
            float pfj = totalFeatureCount / (float)EntityCount;

            float[,] A = new float[2, 2];
            A[0, 0] = (M[0, 0] == 0) ? 0 : (float)(M[0, 0] * Math.Log(M[0, 0] / (1 - pci) * (1 - pfj)));
            A[0, 1] = (M[0, 1] == 0) ? 0 : (float)(M[0, 1] * Math.Log(M[0, 1] / (1 - pci) * pfj));
            A[1, 0] = (M[1, 0] == 0) ? 0 : (float)(M[1, 0] * Math.Log(M[1, 0] / pci * (1 - pfj)));
            A[1, 1] = (M[1, 1] == 0) ? 0 : (float)(M[1, 1] * Math.Log(M[1, 1] / pci * pfj));

            var score = A.Cast<float>().Sum();
            var probF = -(pfj * Math.Log(pfj) + (1 - pfj) * Math.Log(1 - pfj));
            var probC = -(pci * Math.Log(pci) + (1 - pci) * Math.Log(1 - pci));

            var normalizedScore = score / Math.Min(probC, probF);
            return normalizedScore;
        }

        public double ChiSquared(Dictionary<Target, int> featureFrequency, Target target, long totalEntities)
        {
            float[,] M = new float[2, 2];
            int totalFeatureCount = featureFrequency.Values.Sum();

            M[0, 0] = (1f / EntityCount) * (EntityCount - totalFeatureCount - targetCount[target] + featureFrequency[target]);
            M[0, 1] = (1f / EntityCount) * (totalFeatureCount - featureFrequency[target]);
            M[1, 0] = (1f / EntityCount) * (targetCount[target] - featureFrequency[target]);
            M[1, 1] = (1f / EntityCount) * (featureFrequency[target]);

            float pci = totalFeatureCount / (float)EntityCount;
            float pfj = totalFeatureCount / (float)EntityCount;

            double denominator = (M[1, 1] + M[0, 1]) * (M[1, 1] + M[1, 0]) * (M[1, 0] + M[0, 0]) * (M[0, 1] + M[0, 0]);
            double chiSquaredScore = 0;
            if (denominator > 0)
            {
                double numerator = Math.Pow(((double)M[1, 1] * M[0, 0]) - ((double)M[1, 0] * M[0, 1]), 2) * totalEntities;
                return chiSquaredScore = numerator / denominator;
            }
            return double.MinValue;
        }
    }
}
