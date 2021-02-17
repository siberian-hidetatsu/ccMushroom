namespace ccMushroom
{
	partial class TabMngDlg
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
			if ( tabIcon != null )
			{
				tabIcon.Dispose();
				tabIcon = null;
			}

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
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.pictureIcon = new System.Windows.Forms.PictureBox();
			this.comboTabName = new System.Windows.Forms.ComboBox();
			this.checkShowIcon = new System.Windows.Forms.CheckBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.labelTabText = new System.Windows.Forms.Label();
			this.checkMoveToLast = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkEnabledBackground = new System.Windows.Forms.CheckBox();
			this.pictureBackgroundImage = new System.Windows.Forms.PictureBox();
			this.labelIcon = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.contextMenuTabBackground = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuRemoveTabBackground = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBackgroundImage)).BeginInit();
			this.contextMenuTabBackground.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Location = new System.Drawing.Point(88, 120);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 25);
			this.buttonOk.TabIndex = 10;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.CausesValidation = false;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(168, 120);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 25);
			this.buttonCancel.TabIndex = 0;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// pictureIcon
			// 
			this.pictureIcon.BackColor = System.Drawing.Color.White;
			this.pictureIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureIcon.Location = new System.Drawing.Point(48, 32);
			this.pictureIcon.Name = "pictureIcon";
			this.pictureIcon.Size = new System.Drawing.Size(24, 20);
			this.pictureIcon.TabIndex = 43;
			this.pictureIcon.TabStop = false;
			this.toolTip.SetToolTip(this.pictureIcon, "登録するアイコンをドラッグ＆ドロップするか、ダブルクリックで選択する");
			this.pictureIcon.DoubleClick += new System.EventHandler(this.pictureIcon_DoubleClick);
			this.pictureIcon.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureIcon_DragDrop);
			this.pictureIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureIcon_Paint);
			this.pictureIcon.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureIcon_DragEnter);
			// 
			// comboTabName
			// 
			this.comboTabName.FormattingEnabled = true;
			this.comboTabName.Location = new System.Drawing.Point(48, 8);
			this.comboTabName.Name = "comboTabName";
			this.comboTabName.Size = new System.Drawing.Size(114, 20);
			this.comboTabName.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboTabName, "ccMushroom のタブ名");
			this.comboTabName.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
			this.comboTabName.SelectedIndexChanged += new System.EventHandler(this.comboTabName_SelectedIndexChanged);
			this.comboTabName.Leave += new System.EventHandler(this.comboTabName_Leave);
			this.comboTabName.Validated += new System.EventHandler(this.textBox_Validated);
			this.comboTabName.TextChanged += new System.EventHandler(this.comboTabName_TextChanged);
			// 
			// checkShowIcon
			// 
			this.checkShowIcon.AutoSize = true;
			this.checkShowIcon.Location = new System.Drawing.Point(8, 90);
			this.checkShowIcon.Name = "checkShowIcon";
			this.checkShowIcon.Size = new System.Drawing.Size(83, 16);
			this.checkShowIcon.TabIndex = 6;
			this.checkShowIcon.Text = "アイコン表示";
			this.toolTip.SetToolTip(this.checkShowIcon, "タブページのアイコンを登録する/しない");
			this.checkShowIcon.UseVisualStyleBackColor = true;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// labelTabText
			// 
			this.labelTabText.AutoSize = true;
			this.labelTabText.Location = new System.Drawing.Point(14, 12);
			this.labelTabText.Name = "labelTabText";
			this.labelTabText.Size = new System.Drawing.Size(34, 12);
			this.labelTabText.TabIndex = 0;
			this.labelTabText.Text = "タブ名";
			this.labelTabText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkMoveToLast
			// 
			this.checkMoveToLast.AutoSize = true;
			this.checkMoveToLast.Location = new System.Drawing.Point(96, 90);
			this.checkMoveToLast.Name = "checkMoveToLast";
			this.checkMoveToLast.Size = new System.Drawing.Size(60, 16);
			this.checkMoveToLast.TabIndex = 7;
			this.checkMoveToLast.Text = "最後尾";
			this.toolTip.SetToolTip(this.checkMoveToLast, "ショートカットの順番を最後尾に移動する");
			this.checkMoveToLast.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkEnabledBackground);
			this.groupBox1.Controls.Add(this.pictureBackgroundImage);
			this.groupBox1.Location = new System.Drawing.Point(169, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(72, 68);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "　　背景";
			this.toolTip.SetToolTip(this.groupBox1, "背景画像をドラッグ＆ドロップするか、ダブルクリックで選択する");
			// 
			// checkEnabledBackground
			// 
			this.checkEnabledBackground.AutoSize = true;
			this.checkEnabledBackground.Location = new System.Drawing.Point(8, 0);
			this.checkEnabledBackground.Name = "checkEnabledBackground";
			this.checkEnabledBackground.Size = new System.Drawing.Size(15, 14);
			this.checkEnabledBackground.TabIndex = 3;
			this.toolTip.SetToolTip(this.checkEnabledBackground, "タブの背景画像を有効にする");
			this.checkEnabledBackground.UseVisualStyleBackColor = true;
			this.checkEnabledBackground.CheckedChanged += new System.EventHandler(this.checkEnabledBackground_CheckedChanged);
			// 
			// pictureBackgroundImage
			// 
			this.pictureBackgroundImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBackgroundImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBackgroundImage.Location = new System.Drawing.Point(3, 15);
			this.pictureBackgroundImage.Name = "pictureBackgroundImage";
			this.pictureBackgroundImage.Size = new System.Drawing.Size(66, 50);
			this.pictureBackgroundImage.TabIndex = 45;
			this.pictureBackgroundImage.TabStop = false;
			this.pictureBackgroundImage.DoubleClick += new System.EventHandler(this.pictureBackgroundImage_DoubleClick);
			this.pictureBackgroundImage.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBackgroundImage_DragDrop);
			this.pictureBackgroundImage.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBackgroundImage_Paint);
			this.pictureBackgroundImage.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBackgroundImage_DragEnter);
			// 
			// labelIcon
			// 
			this.labelIcon.AutoSize = true;
			this.labelIcon.Location = new System.Drawing.Point(8, 36);
			this.labelIcon.Name = "labelIcon";
			this.labelIcon.Size = new System.Drawing.Size(40, 12);
			this.labelIcon.TabIndex = 2;
			this.labelIcon.Text = "アイコン";
			this.labelIcon.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(8, 112);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(233, 2);
			this.label2.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Location = new System.Drawing.Point(8, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(233, 2);
			this.label3.TabIndex = 5;
			// 
			// contextMenuTabBackground
			// 
			this.contextMenuTabBackground.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuRemoveTabBackground});
			this.contextMenuTabBackground.Name = "contextMenuTabBackground";
			this.contextMenuTabBackground.ShowImageMargin = false;
			this.contextMenuTabBackground.Size = new System.Drawing.Size(199, 26);
			this.contextMenuTabBackground.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuTabBackground_Opening);
			// 
			// toolStripMenuRemoveTabBackground
			// 
			this.toolStripMenuRemoveTabBackground.Name = "toolStripMenuRemoveTabBackground";
			this.toolStripMenuRemoveTabBackground.Size = new System.Drawing.Size(198, 22);
			this.toolStripMenuRemoveTabBackground.Text = "背景画像の登録を解除する(&R)";
			this.toolStripMenuRemoveTabBackground.Click += new System.EventHandler(this.toolStripMenuRemoveTabBackground_Click);
			// 
			// TabMngDlg
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(249, 154);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkMoveToLast);
			this.Controls.Add(this.checkShowIcon);
			this.Controls.Add(this.pictureIcon);
			this.Controls.Add(this.comboTabName);
			this.Controls.Add(this.labelIcon);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelTabText);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TabMngDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "タブ管理ダイアログ";
			this.Load += new System.EventHandler(this.TabMngDlg_Load);
			this.Shown += new System.EventHandler(this.TabMngDlg_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TabMngDlg_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBackgroundImage)).EndInit();
			this.contextMenuTabBackground.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.PictureBox pictureIcon;
		private System.Windows.Forms.ComboBox comboTabName;
		public System.Windows.Forms.CheckBox checkShowIcon;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.Label labelTabText;
		public System.Windows.Forms.CheckBox checkMoveToLast;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label labelIcon;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.PictureBox pictureBackgroundImage;
		private System.Windows.Forms.GroupBox groupBox1;
		public System.Windows.Forms.CheckBox checkEnabledBackground;
		private System.Windows.Forms.ContextMenuStrip contextMenuTabBackground;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuRemoveTabBackground;
	}
}