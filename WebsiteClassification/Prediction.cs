namespace PsiMl.WebsiteClasification
{
    using System.Collections.Generic;
    using System.Linq;

    public class Prediction
    {
        public Target Label
        {
            get
            {
                var max = this.Confidences.OrderByDescending(kv => kv.Value).First();
                return max.Key;
            }
        }

        public Dictionary<Target, double> Confidences { get; set; }
    }
}
