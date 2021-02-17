namespace ccMushroom
{
    partial class SplashForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashForm));
			this.label = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelMessage2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.labelMultiThreadIndicator = new System.Windows.Forms.Label();
			this.labelMessage1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label.Font = new System.Drawing.Font("Comic Sans MS", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label.ForeColor = System.Drawing.Color.Black;
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(309, 48);
			this.label.TabIndex = 0;
			this.label.Text = "ccMushroom is starting...";
			// 
			// pictureBox
			// 
			this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
			this.pictureBox.Location = new System.Drawing.Point(8, 64);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(160, 60);
			this.pictureBox.TabIndex = 1;
			this.pictureBox.TabStop = false;
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(176, 101);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(142, 23);
			this.progressBar.Step = 2;
			this.progressBar.TabIndex = 3;
			// 
			// labelMessage2
			// 
			this.labelMessage2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelMessage2.Location = new System.Drawing.Point(176, 80);
			this.labelMessage2.Name = "labelMessage2";
			this.labelMessage2.Size = new System.Drawing.Size(142, 16);
			this.labelMessage2.TabIndex = 2;
			this.labelMessage2.Text = "message2";
			this.labelMessage2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelMessage2.Paint += new System.Windows.Forms.PaintEventHandler(this.labelMessage_Paint);
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(326, 32);
			this.panel1.TabIndex = 5;
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Controls.Add(this.labelMultiThreadIndicator);
			this.panel2.Location = new System.Drawing.Point(1, 1);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(319, 39);
			this.panel2.TabIndex = 4;
			// 
			// labelMultiThreadIndicator
			// 
			this.labelMultiThreadIndicator.Location = new System.Drawing.Point(0, 0);
			this.labelMultiThreadIndicator.Name = "labelMultiThreadIndicator";
			this.labelMultiThreadIndicator.Size = new System.Drawing.Size(8, 8);
			this.labelMultiThreadIndicator.TabIndex = 6;
			this.labelMultiThreadIndicator.Text = "*";
			// 
			// labelMessage1
			// 
			this.labelMessage1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelMessage1.Location = new System.Drawing.Point(176, 64);
			this.labelMessage1.Name = "labelMessage1";
			this.labelMessage1.Size = new System.Drawing.Size(142, 16);
			this.labelMessage1.TabIndex = 1;
			this.labelMessage1.Text = "---------+---------+-";
			this.labelMessage1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelMessage1.Paint += new System.Windows.Forms.PaintEventHandler(this.labelMessage_Paint);
			// 
			// SplashForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(326, 132);
			this.Controls.Add(this.labelMessage1);
			this.Controls.Add(this.labelMessage2);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.label);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "SplashForm";
			this.ShowInTaskbar = false;
			this.Text = "SplashForm";
			this.Load += new System.EventHandler(this.SplashForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label labelMessage2;
        private System.Windows.Forms.Label labelMessage1;
        private System.Windows.Forms.Label labelMultiThreadIndicator;
    }
}