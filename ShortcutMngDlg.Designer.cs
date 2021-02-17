namespace ccMushroom
{
	partial class ShortcutMngDlg
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortcutMngDlg));
			this.textAppName = new System.Windows.Forms.TextBox();
			this.labelWorkingDirectory = new System.Windows.Forms.Label();
			this.textTabText = new System.Windows.Forms.TextBox();
			this.labelCommandLine = new System.Windows.Forms.Label();
			this.pictureIcon = new System.Windows.Forms.PictureBox();
			this.contextMenuPictIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuRemoveIcon = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuReloadFavicon = new System.Windows.Forms.ToolStripMenuItem();
			this.labelTabText = new System.Windows.Forms.Label();
			this.labelButtonText = new System.Windows.Forms.Label();
			this.textButtonText = new System.Windows.Forms.TextBox();
			this.textCommandLine = new System.Windows.Forms.TextBox();
			this.labelAppName = new System.Windows.Forms.Label();
			this.textWorkingDirectory = new System.Windows.Forms.TextBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.checkDelete = new System.Windows.Forms.CheckBox();
			this.checkNewApp = new System.Windows.Forms.CheckBox();
			this.timerAutoApply = new System.Windows.Forms.Timer(this.components);
			this.labelCountDown = new System.Windows.Forms.Label();
			this.textComment = new System.Windows.Forms.TextBox();
			this.labelComment = new System.Windows.Forms.Label();
			this.comboTabName = new System.Windows.Forms.ComboBox();
			this.checkMoveToLast = new System.Windows.Forms.CheckBox();
			this.checkParseShortcut = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.buttonSelectApp = new System.Windows.Forms.Button();
			this.buttonSelectBackColor = new System.Windows.Forms.Button();
			this.comboBackColor = new System.Windows.Forms.ComboBox();
			this.checkAutoExec = new System.Windows.Forms.CheckBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.textFaviconUrl = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).BeginInit();
			this.contextMenuPictIcon.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// textAppName
			// 
			this.textAppName.Location = new System.Drawing.Point(88, 56);
			this.textAppName.Name = "textAppName";
			this.textAppName.Size = new System.Drawing.Size(195, 19);
			this.textAppName.TabIndex = 7;
			this.toolTip.SetToolTip(this.textAppName, "アプリケーション名");
			this.textAppName.Validated += new System.EventHandler(this.textBox_Validated);
			this.textAppName.Leave += new System.EventHandler(this.textAppName_Leave);
			this.textAppName.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
			// 
			// labelWorkingDirectory
			// 
			this.labelWorkingDirectory.AutoSize = true;
			this.labelWorkingDirectory.Location = new System.Drawing.Point(10, 107);
			this.labelWorkingDirectory.Name = "labelWorkingDirectory";
			this.labelWorkingDirectory.Size = new System.Drawing.Size(78, 12);
			this.labelWorkingDirectory.TabIndex = 11;
			this.labelWorkingDirectory.Text = "作業ディレクトリ";
			this.labelWorkingDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTabText
			// 
			this.textTabText.Location = new System.Drawing.Point(88, 8);
			this.textTabText.Name = "textTabText";
			this.textTabText.Size = new System.Drawing.Size(166, 19);
			this.textTabText.TabIndex = 1;
			this.toolTip.SetToolTip(this.textTabText, "ccMushroom のタブ名");
			this.textTabText.Validated += new System.EventHandler(this.textBox_Validated);
			this.textTabText.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
			// 
			// labelCommandLine
			// 
			this.labelCommandLine.AutoSize = true;
			this.labelCommandLine.Location = new System.Drawing.Point(18, 83);
			this.labelCommandLine.Name = "labelCommandLine";
			this.labelCommandLine.Size = new System.Drawing.Size(70, 12);
			this.labelCommandLine.TabIndex = 9;
			this.labelCommandLine.Text = "コマンド ライン";
			this.labelCommandLine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pictureIcon
			// 
			this.pictureIcon.BackColor = System.Drawing.Color.White;
			this.pictureIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureIcon.ContextMenuStrip = this.contextMenuPictIcon;
			this.pictureIcon.Location = new System.Drawing.Point(259, 32);
			this.pictureIcon.Name = "pictureIcon";
			this.pictureIcon.Size = new System.Drawing.Size(24, 20);
			this.pictureIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureIcon.TabIndex = 42;
			this.pictureIcon.TabStop = false;
			this.toolTip.SetToolTip(this.pictureIcon, "登録するアイコンをドラッグ＆ドロップするか、ダブルクリックで選択する");
			this.pictureIcon.DoubleClick += new System.EventHandler(this.pictureIcon_DoubleClick);
			this.pictureIcon.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureIcon_DragDrop);
			this.pictureIcon.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureIcon_Paint);
			this.pictureIcon.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureIcon_DragEnter);
			// 
			// contextMenuPictIcon
			// 
			this.contextMenuPictIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuRemoveIcon,
            this.toolStripMenuReloadFavicon});
			this.contextMenuPictIcon.Name = "contextMenuPictIcon";
			this.contextMenuPictIcon.ShowImageMargin = false;
			this.contextMenuPictIcon.Size = new System.Drawing.Size(186, 48);
			this.contextMenuPictIcon.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuPictIcon_Opening);
			// 
			// toolStripMenuRemoveIcon
			// 
			this.toolStripMenuRemoveIcon.Name = "toolStripMenuRemoveIcon";
			this.toolStripMenuRemoveIcon.Size = new System.Drawing.Size(185, 22);
			this.toolStripMenuRemoveIcon.Text = "アイコンの登録を解除する(&R)";
			this.toolStripMenuRemoveIcon.Click += new System.EventHandler(this.toolStripMenuRemoveIcon_Click);
			// 
			// toolStripMenuReloadFavicon
			// 
			this.toolStripMenuReloadFavicon.Name = "toolStripMenuReloadFavicon";
			this.toolStripMenuReloadFavicon.Size = new System.Drawing.Size(185, 22);
			this.toolStripMenuReloadFavicon.Text = "favicon.ico を再取得する(&F)";
			this.toolStripMenuReloadFavicon.Click += new System.EventHandler(this.toolStripMenuReloadFavicon_Click);
			// 
			// labelTabText
			// 
			this.labelTabText.AutoSize = true;
			this.labelTabText.Location = new System.Drawing.Point(54, 11);
			this.labelTabText.Name = "labelTabText";
			this.labelTabText.Size = new System.Drawing.Size(34, 12);
			this.labelTabText.TabIndex = 0;
			this.labelTabText.Text = "タブ名";
			this.labelTabText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelButtonText
			// 
			this.labelButtonText.AutoSize = true;
			this.labelButtonText.Location = new System.Drawing.Point(44, 35);
			this.labelButtonText.Name = "labelButtonText";
			this.labelButtonText.Size = new System.Drawing.Size(44, 12);
			this.labelButtonText.TabIndex = 3;
			this.labelButtonText.Text = "ボタン名";
			this.labelButtonText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textButtonText
			// 
			this.textButtonText.Location = new System.Drawing.Point(88, 32);
			this.textButtonText.Name = "textButtonText";
			this.textButtonText.Size = new System.Drawing.Size(166, 19);
			this.textButtonText.TabIndex = 4;
			this.toolTip.SetToolTip(this.textButtonText, "アプリケーションのボタン名");
			this.textButtonText.Validated += new System.EventHandler(this.textBox_Validated);
			this.textButtonText.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
			// 
			// textCommandLine
			// 
			this.textCommandLine.Location = new System.Drawing.Point(88, 80);
			this.textCommandLine.Name = "textCommandLine";
			this.textCommandLine.Size = new System.Drawing.Size(224, 19);
			this.textCommandLine.TabIndex = 10;
			this.toolTip.SetToolTip(this.textCommandLine, "コマンド ライン引数");
			// 
			// labelAppName
			// 
			this.labelAppName.AutoSize = true;
			this.labelAppName.Location = new System.Drawing.Point(14, 59);
			this.labelAppName.Name = "labelAppName";
			this.labelAppName.Size = new System.Drawing.Size(74, 12);
			this.labelAppName.TabIndex = 6;
			this.labelAppName.Text = "アプリケーション";
			this.labelAppName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWorkingDirectory
			// 
			this.textWorkingDirectory.Location = new System.Drawing.Point(88, 104);
			this.textWorkingDirectory.Name = "textWorkingDirectory";
			this.textWorkingDirectory.Size = new System.Drawing.Size(224, 19);
			this.textWorkingDirectory.TabIndex = 12;
			this.toolTip.SetToolTip(this.textWorkingDirectory, "作業ディレクトリ名");
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Location = new System.Drawing.Point(160, 200);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 25);
			this.buttonOk.TabIndex = 21;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.CausesValidation = false;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(240, 200);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 25);
			this.buttonCancel.TabIndex = 22;
			this.buttonCancel.Text = "キャンセル";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// checkDelete
			// 
			this.checkDelete.AutoSize = true;
			this.checkDelete.Location = new System.Drawing.Point(56, 168);
			this.checkDelete.Name = "checkDelete";
			this.checkDelete.Size = new System.Drawing.Size(48, 16);
			this.checkDelete.TabIndex = 17;
			this.checkDelete.Text = "削除";
			this.toolTip.SetToolTip(this.checkDelete, "ボタンを削除する");
			this.checkDelete.UseVisualStyleBackColor = true;
			this.checkDelete.CheckedChanged += new System.EventHandler(this.checkDelete_CheckedChanged);
			// 
			// checkNewApp
			// 
			this.checkNewApp.AutoSize = true;
			this.checkNewApp.Location = new System.Drawing.Point(168, 168);
			this.checkNewApp.Name = "checkNewApp";
			this.checkNewApp.Size = new System.Drawing.Size(36, 16);
			this.checkNewApp.TabIndex = 19;
			this.checkNewApp.Text = "新";
			this.toolTip.SetToolTip(this.checkNewApp, "新しいアプリケーション");
			this.checkNewApp.UseVisualStyleBackColor = true;
			// 
			// timerAutoApply
			// 
			this.timerAutoApply.Tick += new System.EventHandler(this.timerAutoApply_Tick);
			// 
			// labelCountDown
			// 
			this.labelCountDown.AutoSize = true;
			this.labelCountDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelCountDown.Location = new System.Drawing.Point(299, 8);
			this.labelCountDown.Name = "labelCountDown";
			this.labelCountDown.Size = new System.Drawing.Size(13, 14);
			this.labelCountDown.TabIndex = 2;
			this.labelCountDown.Text = "0";
			this.labelCountDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textComment
			// 
			this.textComment.Location = new System.Drawing.Point(88, 128);
			this.textComment.Name = "textComment";
			this.textComment.Size = new System.Drawing.Size(224, 19);
			this.textComment.TabIndex = 14;
			this.toolTip.SetToolTip(this.textComment, "注釈");
			// 
			// labelComment
			// 
			this.labelComment.AutoSize = true;
			this.labelComment.Location = new System.Drawing.Point(50, 131);
			this.labelComment.Name = "labelComment";
			this.labelComment.Size = new System.Drawing.Size(38, 12);
			this.labelComment.TabIndex = 13;
			this.labelComment.Text = "コメント";
			this.labelComment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTabName
			// 
			this.comboTabName.FormattingEnabled = true;
			this.comboTabName.Location = new System.Drawing.Point(8, 8);
			this.comboTabName.Name = "comboTabName";
			this.comboTabName.Size = new System.Drawing.Size(32, 20);
			this.comboTabName.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboTabName, "ccMushroom のタブ名");
			this.comboTabName.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Validating);
			this.comboTabName.Validated += new System.EventHandler(this.textBox_Validated);
			this.comboTabName.TextChanged += new System.EventHandler(this.comboTabName_TextChanged);
			// 
			// checkMoveToLast
			// 
			this.checkMoveToLast.AutoSize = true;
			this.checkMoveToLast.Location = new System.Drawing.Point(104, 168);
			this.checkMoveToLast.Name = "checkMoveToLast";
			this.checkMoveToLast.Size = new System.Drawing.Size(60, 16);
			this.checkMoveToLast.TabIndex = 18;
			this.checkMoveToLast.Text = "最後尾";
			this.toolTip.SetToolTip(this.checkMoveToLast, "ショートカットの順番を最後尾に移動する");
			this.checkMoveToLast.UseVisualStyleBackColor = true;
			// 
			// checkParseShortcut
			// 
			this.checkParseShortcut.AutoSize = true;
			this.checkParseShortcut.Location = new System.Drawing.Point(8, 168);
			this.checkParseShortcut.Name = "checkParseShortcut";
			this.checkParseShortcut.Size = new System.Drawing.Size(48, 16);
			this.checkParseShortcut.TabIndex = 16;
			this.checkParseShortcut.Text = "解析";
			this.toolTip.SetToolTip(this.checkParseShortcut, "ショートカット ファイルの実体を解析する");
			this.checkParseShortcut.UseVisualStyleBackColor = true;
			this.checkParseShortcut.CheckedChanged += new System.EventHandler(this.checkParseShortcut_CheckedChanged);
			// 
			// buttonSelectApp
			// 
			this.buttonSelectApp.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttonSelectApp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSelectApp.Image = ((System.Drawing.Image)(resources.GetObject("buttonSelectApp.Image")));
			this.buttonSelectApp.Location = new System.Drawing.Point(288, 56);
			this.buttonSelectApp.Name = "buttonSelectApp";
			this.buttonSelectApp.Size = new System.Drawing.Size(24, 19);
			this.buttonSelectApp.TabIndex = 8;
			this.toolTip.SetToolTip(this.buttonSelectApp, "アプリケーションを選択する");
			this.buttonSelectApp.Click += new System.EventHandler(this.buttonSelectApp_Click);
			// 
			// buttonSelectBackColor
			// 
			this.buttonSelectBackColor.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttonSelectBackColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSelectBackColor.Image = ((System.Drawing.Image)(resources.GetObject("buttonSelectBackColor.Image")));
			this.buttonSelectBackColor.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.buttonSelectBackColor.Location = new System.Drawing.Point(288, 32);
			this.buttonSelectBackColor.Name = "buttonSelectBackColor";
			this.buttonSelectBackColor.Size = new System.Drawing.Size(24, 19);
			this.buttonSelectBackColor.TabIndex = 5;
			this.toolTip.SetToolTip(this.buttonSelectBackColor, "ボタンの背景色を選択する");
			this.buttonSelectBackColor.Click += new System.EventHandler(this.buttonSelectBackColor_Click);
			// 
			// comboBackColor
			// 
			this.comboBackColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBackColor.DropDownHeight = 117;
			this.comboBackColor.FormattingEnabled = true;
			this.comboBackColor.IntegralHeight = false;
			this.comboBackColor.Location = new System.Drawing.Point(8, 208);
			this.comboBackColor.Name = "comboBackColor";
			this.comboBackColor.Size = new System.Drawing.Size(76, 20);
			this.comboBackColor.TabIndex = 5;
			this.toolTip.SetToolTip(this.comboBackColor, "ボタンの背景色");
			this.comboBackColor.Visible = false;
			this.comboBackColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBackColor_DrawItem);
			this.comboBackColor.SelectedIndexChanged += new System.EventHandler(this.comboBackColor_SelectedIndexChanged);
			this.comboBackColor.Leave += new System.EventHandler(this.comboBackColor_Leave);
			this.comboBackColor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBackColor_KeyUp);
			// 
			// checkAutoExec
			// 
			this.checkAutoExec.AutoSize = true;
			this.checkAutoExec.Location = new System.Drawing.Point(96, 200);
			this.checkAutoExec.Name = "checkAutoExec";
			this.checkAutoExec.Size = new System.Drawing.Size(63, 16);
			this.checkAutoExec.TabIndex = 2;
			this.checkAutoExec.Text = "StartUp";
			this.toolTip.SetToolTip(this.checkAutoExec, "ccMushroom 起動時に自動実行する");
			this.checkAutoExec.UseVisualStyleBackColor = true;
			this.checkAutoExec.Visible = false;
			// 
			// textFaviconUrl
			// 
			this.textFaviconUrl.Location = new System.Drawing.Point(212, 165);
			this.textFaviconUrl.Name = "textFaviconUrl";
			this.textFaviconUrl.Size = new System.Drawing.Size(100, 19);
			this.textFaviconUrl.TabIndex = 12;
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(8, 192);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(304, 2);
			this.label2.TabIndex = 20;
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(8, 156);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(304, 2);
			this.label1.TabIndex = 15;
			// 
			// ShortcutMngDlg
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(326, 233);
			this.Controls.Add(this.buttonSelectBackColor);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textFaviconUrl);
			this.Controls.Add(this.comboBackColor);
			this.Controls.Add(this.buttonSelectApp);
			this.Controls.Add(this.checkParseShortcut);
			this.Controls.Add(this.checkMoveToLast);
			this.Controls.Add(this.checkDelete);
			this.Controls.Add(this.checkAutoExec);
			this.Controls.Add(this.comboTabName);
			this.Controls.Add(this.labelCountDown);
			this.Controls.Add(this.textAppName);
			this.Controls.Add(this.checkNewApp);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.labelComment);
			this.Controls.Add(this.labelWorkingDirectory);
			this.Controls.Add(this.textTabText);
			this.Controls.Add(this.labelCommandLine);
			this.Controls.Add(this.pictureIcon);
			this.Controls.Add(this.labelTabText);
			this.Controls.Add(this.labelButtonText);
			this.Controls.Add(this.textButtonText);
			this.Controls.Add(this.textCommandLine);
			this.Controls.Add(this.labelAppName);
			this.Controls.Add(this.textWorkingDirectory);
			this.Controls.Add(this.textComment);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ShortcutMngDlg";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ショートカット管理ダイアログ";
			this.Load += new System.EventHandler(this.ShortcutMngDlg_Load);
			this.Shown += new System.EventHandler(this.ShortcutMngDlg_Shown);
			this.Click += new System.EventHandler(this.ShortcutMngDlg_Click);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ShortcutMngDlg_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.pictureIcon)).EndInit();
			this.contextMenuPictIcon.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textAppName;
		private System.Windows.Forms.Label labelWorkingDirectory;
		private System.Windows.Forms.TextBox textTabText;
		private System.Windows.Forms.Label labelCommandLine;
		private System.Windows.Forms.PictureBox pictureIcon;
		private System.Windows.Forms.Label labelTabText;
		private System.Windows.Forms.Label labelButtonText;
		private System.Windows.Forms.TextBox textButtonText;
		private System.Windows.Forms.TextBox textCommandLine;
		private System.Windows.Forms.Label labelAppName;
		private System.Windows.Forms.TextBox textWorkingDirectory;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ErrorProvider errorProvider;
		public System.Windows.Forms.CheckBox checkDelete;
		private System.Windows.Forms.CheckBox checkNewApp;
		private System.Windows.Forms.Timer timerAutoApply;
		private System.Windows.Forms.Label labelCountDown;
		private System.Windows.Forms.Label labelComment;
		private System.Windows.Forms.TextBox textComment;
		private System.Windows.Forms.ComboBox comboTabName;
		public System.Windows.Forms.CheckBox checkMoveToLast;
		private System.Windows.Forms.CheckBox checkParseShortcut;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ContextMenuStrip contextMenuPictIcon;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuRemoveIcon;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuReloadFavicon;
		private System.Windows.Forms.Button buttonSelectApp;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TextBox textFaviconUrl;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonSelectBackColor;
		private System.Windows.Forms.ComboBox comboBackColor;
		private System.Windows.Forms.CheckBox checkAutoExec;

	}
}