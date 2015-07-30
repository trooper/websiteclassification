namespace PsiMl.WebsiteRuntime
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.crawlDepth = new System.Windows.Forms.NumericUpDown();
            this.labelPrediction = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.url = new System.Windows.Forms.TextBox();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crawlDepth)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.crawlDepth);
            this.splitContainer1.Panel1.Controls.Add(this.labelPrediction);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.url);
            this.splitContainer1.Panel1.Controls.Add(this.buttonExecute);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.webBrowser);
            this.splitContainer1.Size = new System.Drawing.Size(1074, 651);
            this.splitContainer1.SplitterDistance = 44;
            this.splitContainer1.TabIndex = 0;
            // 
            // crawlDepth
            // 
            this.crawlDepth.Location = new System.Drawing.Point(49, 14);
            this.crawlDepth.Name = "crawlDepth";
            this.crawlDepth.Size = new System.Drawing.Size(64, 22);
            this.crawlDepth.TabIndex = 5;
            this.crawlDepth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelPrediction
            // 
            this.labelPrediction.AutoSize = true;
            this.labelPrediction.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPrediction.Location = new System.Drawing.Point(774, 15);
            this.labelPrediction.Name = "labelPrediction";
            this.labelPrediction.Size = new System.Drawing.Size(100, 25);
            this.labelPrediction.TabIndex = 4;
            this.labelPrediction.Text = "________";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Depth:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(692, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Prediction:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(130, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL:";
            // 
            // url
            // 
            this.url.Location = new System.Drawing.Point(176, 12);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(406, 22);
            this.url.TabIndex = 1;
            this.url.Text = "http://www.hyatt.com";
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(588, 12);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(75, 23);
            this.buttonExecute.TabIndex = 0;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(1074, 603);
            this.webBrowser.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1074, 651);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Classifier";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.crawlDepth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.Label labelPrediction;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown crawlDepth;
        private System.Windows.Forms.Label label3;
    }
}

