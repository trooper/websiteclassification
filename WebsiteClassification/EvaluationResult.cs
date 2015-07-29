namespace PsiMl.WebsiteClasification
{
    public class EvaluationResult
    {
        public EvaluationResult()
        {
            this.TruePositive = this.TrueNegative = this.FalsePositive = this.FalseNegative = 0;
        }

        public int TruePositive { get; set; }

        public int TrueNegative{ get; set; }

        public int FalsePositive { get; set; }

        public int FalseNegative { get; set; }

        public double Precision
        {
            get
            {
                int outputs = this.TruePositive + this.FalsePositive;
                return outputs != 0 ? (double)this.TruePositive / outputs : 0.0;
            }
        }

        public double Recall
        {
            get
            {
                int total = this.TruePositive + this.FalseNegative;
                return total != 0 ? (double)this.TruePositive / total : 1.0;
            }
        }

        public override string ToString()
        {
            return string.Format("P: {0}\tR: {1}", this.Precision, this.Recall);
        }
    }
}
