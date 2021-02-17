namespace ccMushroom
{
	partial class LogonDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogonDlg));
			this.label1 = new System.Windows.Forms.Label();
			this.textUserName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textDomain = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 11);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "ユーザー名(&U):";
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(87, 8);
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(127, 19);
			this.textUserName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "パスワード(&P):";
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(87, 32);
			this.textPassword.MaxLength = 64;
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(127, 19);
			this.textPassword.TabIndex = 3;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(81, 91);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(67, 23);
			this.buttonOK.TabIndex = 6;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(149, 91);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(67, 23);
			this.buttonCancel.TabIndex = 7;
			this.buttonCancel.TabStop = false;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 12);
			this.label3.TabIndex = 4;
			this.label3.Text = "ログオン先(&L):";
			// 
			// textDomain
			// 
			this.textDomain.BackColor = System.Drawing.SystemColors.Window;
			this.textDomain.Location = new System.Drawing.Point(87, 57);
			this.textDomain.Name = "textDomain";
			this.textDomain.ReadOnly = true;
			this.textDomain.Size = new System.Drawing.Size(127, 19);
			this.textDomain.TabIndex = 5;
			// 
			// LogonDlg
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(228, 121);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDomain);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textUserName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LogonDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ログオン";
			this.Load += new System.EventHandler(this.LogonDlg_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.TextBox textUserName;
		public System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDomain;
	}
}