namespace PsiMl.WebsiteRuntime
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Forms;
    using PsiMl.WebsiteClasification;

    public partial class MainForm : Form
    {
        private Target prediction;

        public MainForm()
        {
            InitializeComponent();
            Classifier.InitializeClassifier();

            this.webBrowser.ScriptErrorsSuppressed = true;
        }
        
        private void buttonExecute_Click(object sender, EventArgs e)
        {
            this.buttonExecute.Enabled = false;
            this.crawlDepth.Enabled = false;

            var url = this.url.Text;
            this.webBrowser.Navigate(url);

            var worker = new BackgroundWorker();
            worker.DoWork += FetchAndClassify;
            worker.RunWorkerCompleted += FetchCompleted;
            worker.RunWorkerAsync();
        }

        private void FetchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.labelPrediction.Text = this.prediction.ToString();
            this.buttonExecute.Enabled = true;
            this.crawlDepth.Enabled = true;
        }

        private void FetchAndClassify(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            var url = this.url.Text;
            int depth = (int)this.crawlDepth.Value;
            this.prediction = Classifier.FetchAndClassify(url, depth);
        }
    }
}
