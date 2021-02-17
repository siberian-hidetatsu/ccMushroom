namespace ccMushroom
{
	partial class ExitDlg
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
			if ( disposing && (components != null) )
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExitDlg));
			this.buttonExit = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.buttonDummy = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// buttonExit
			// 
			this.buttonExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.buttonExit.Image = ((System.Drawing.Image)(resources.GetObject("buttonExit.Image")));
			this.buttonExit.Location = new System.Drawing.Point(0, 0);
			this.buttonExit.Name = "buttonExit";
			this.buttonExit.Size = new System.Drawing.Size(23, 23);
			this.buttonExit.TabIndex = 1;
			this.buttonExit.TabStop = false;
			this.toolTip.SetToolTip(this.buttonExit, "プログラムを終了する");
			this.buttonExit.UseVisualStyleBackColor = false;
			this.buttonExit.MouseLeave += new System.EventHandler(this.buttonExit_MouseLeave);
			this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
			this.buttonExit.MouseEnter += new System.EventHandler(this.buttonExit_MouseEnter);
			// 
			// timer
			// 
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// buttonDummy
			// 
			this.buttonDummy.Location = new System.Drawing.Point(24, 0);
			this.buttonDummy.Name = "buttonDummy";
			this.buttonDummy.Size = new System.Drawing.Size(8, 16);
			this.buttonDummy.TabIndex = 0;
			this.buttonDummy.Text = "Dummy";
			this.buttonDummy.UseVisualStyleBackColor = true;
			// 
			// ExitDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(30, 23);
			this.ControlBox = false;
			this.Controls.Add(this.buttonDummy);
			this.Controls.Add(this.buttonExit);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ExitDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "ExitDlg";
			this.Load += new System.EventHandler(this.ExitDlg_Load);
			this.Click += new System.EventHandler(this.ExitDlg_Click);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonExit;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Button buttonDummy;
		private System.Windows.Forms.ToolTip toolTip;
	}
}