namespace PsiMl.WebsiteClasification
{
    using System;
    using System.Collections.Generic;

    public class Evaluation
    {
        private Model model;

        public Evaluation(Model model)
        {
            this.model = model;
        }

        public Dictionary<Target, EvaluationResult> Evaluate(IEnumerable<MLEntity> entities)
        {
            var results = new Dictionary<Target, EvaluationResult>();
            foreach (var target in  Enum.GetValues(typeof(Target)))
            {
                results[(Target)target] = new EvaluationResult();
            }

            foreach (var entity in entities)
            {
                var result = this.model.Classify(entity.WebSite);
                var label = result.Label;
                if (label == entity.Label)
                {
                    ++results[label].TruePositive;
                }
                else 
                {
                    ++results[label].FalsePositive;
                    ++results[entity.Label].FalseNegative;
                }
            }

            return results;
        }
    }
}
