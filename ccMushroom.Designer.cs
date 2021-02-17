namespace ccMushroom
{
    partial class ccMushroom
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
			if ( rectMarkForeColor != null )
			{
				rectMarkForeColor.Dispose();
				rectMarkForeColor = null;
			}

			if ( rectMarkBackColor != null )
			{
				rectMarkBackColor.Dispose();
				rectMarkBackColor = null;
			}

			if ( dragCursor != null )
			{
				dragCursor.Dispose();
				dragCursor = null;
			}

			if ( newAppIcon != null )
			{
				CommonFunctions.api.DestroyIcon(newAppIcon.Handle);
				newAppIcon.Dispose();
			}

			if ( timerAutoWindowClose != null )
			{
				timerAutoWindowClose.Dispose();
				timerAutoWindowClose = null;
			}

			if ( disposing && (components != null) )
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ccMushroom));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.contextMenuTab = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuEditTab = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuImageLayout = new System.Windows.Forms.ToolStripMenuItem();
			this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.centerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stretchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuTransButton = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuResumeFomeSize = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuForJumpListTabPage = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuButton = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuEditShortcut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuOpenAppFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.menuRunAsAdministrator = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripScanned = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripVersion = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripMousePosition = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.notifyIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuRestoreWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
			this.timerOpacity = new System.Windows.Forms.Timer(this.components);
			this.contextMenuTab.SuspendLayout();
			this.contextMenuButton.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.notifyIconMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.AllowDrop = true;
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.ContextMenuStrip = this.contextMenuTab;
			this.tabControl.Location = new System.Drawing.Point(8, 8);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(616, 435);
			this.tabControl.TabIndex = 0;
			this.tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl_Selecting);
			this.tabControl.Click += new System.EventHandler(this.ccMushroom_Click);
			this.tabControl.DragDrop += new System.Windows.Forms.DragEventHandler(this.tabControl_DragDrop);
			this.tabControl.DragEnter += new System.Windows.Forms.DragEventHandler(this.tabControl_DragEnter);
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// contextMenuTab
			// 
			this.contextMenuTab.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuEditTab,
            this.toolStripSeparator4,
            this.toolStripMenuImageLayout,
            this.toolStripMenuTransButton,
            this.toolStripMenuResumeFomeSize,
            this.toolStripSeparator6,
            this.toolStripMenuForJumpListTabPage});
			this.contextMenuTab.Name = "contextMenuTab";
			this.contextMenuTab.Size = new System.Drawing.Size(227, 126);
			this.contextMenuTab.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuTab_Opening);
			// 
			// toolStripMenuEditTab
			// 
			this.toolStripMenuEditTab.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuEditTab.Image")));
			this.toolStripMenuEditTab.Name = "toolStripMenuEditTab";
			this.toolStripMenuEditTab.Size = new System.Drawing.Size(226, 22);
			this.toolStripMenuEditTab.Text = "タブのプロパティ(&R)...";
			this.toolStripMenuEditTab.Click += new System.EventHandler(this.toolStripMenuEditTab_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(223, 6);
			// 
			// toolStripMenuImageLayout
			// 
			this.toolStripMenuImageLayout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tileToolStripMenuItem,
            this.centerToolStripMenuItem,
            this.stretchToolStripMenuItem,
            this.zoomToolStripMenuItem});
			this.toolStripMenuImageLayout.Name = "toolStripMenuImageLayout";
			this.toolStripMenuImageLayout.Size = new System.Drawing.Size(226, 22);
			this.toolStripMenuImageLayout.Text = "背景画像のレイアウト(&L)";
			// 
			// tileToolStripMenuItem
			// 
			this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
			this.tileToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.tileToolStripMenuItem.Tag = "Tile";
			this.tileToolStripMenuItem.Text = "&Tile";
			this.tileToolStripMenuItem.ToolTipText = "クライアント領域全体に並べて表示する";
			// 
			// centerToolStripMenuItem
			// 
			this.centerToolStripMenuItem.Name = "centerToolStripMenuItem";
			this.centerToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.centerToolStripMenuItem.Tag = "Center";
			this.centerToolStripMenuItem.Text = "&Center";
			this.centerToolStripMenuItem.ToolTipText = "クライアント領域の中央に配置する";
			// 
			// stretchToolStripMenuItem
			// 
			this.stretchToolStripMenuItem.Name = "stretchToolStripMenuItem";
			this.stretchToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.stretchToolStripMenuItem.Tag = "Stretch";
			this.stretchToolStripMenuItem.Text = "&Stretch";
			this.stretchToolStripMenuItem.ToolTipText = "クライアント領域全体に伸縮する";
			// 
			// zoomToolStripMenuItem
			// 
			this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
			this.zoomToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.zoomToolStripMenuItem.Tag = "Zoom";
			this.zoomToolStripMenuItem.Text = "&Zoom";
			this.zoomToolStripMenuItem.ToolTipText = "クライアント領域に縦横比固定で拡大する";
			// 
			// toolStripMenuTransButton
			// 
			this.toolStripMenuTransButton.Name = "toolStripMenuTransButton";
			this.toolStripMenuTransButton.Size = new System.Drawing.Size(226, 22);
			this.toolStripMenuTransButton.Text = "ボタンの背景を透過する(&T)";
			// 
			// toolStripMenuResumeFomeSize
			// 
			this.toolStripMenuResumeFomeSize.Name = "toolStripMenuResumeFomeSize";
			this.toolStripMenuResumeFomeSize.Size = new System.Drawing.Size(226, 22);
			this.toolStripMenuResumeFomeSize.Text = "このフォーム サイズを保存する(&F)";
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(223, 6);
			// 
			// toolStripMenuForJumpListTabPage
			// 
			this.toolStripMenuForJumpListTabPage.Name = "toolStripMenuForJumpListTabPage";
			this.toolStripMenuForJumpListTabPage.Size = new System.Drawing.Size(226, 22);
			this.toolStripMenuForJumpListTabPage.Text = "ジャンプリスト用のタブページ";
			this.toolStripMenuForJumpListTabPage.ToolTipText = "ジャンプリストをこのタブページで固定する";
			// 
			// contextMenuButton
			// 
			this.contextMenuButton.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuEditShortcut,
            this.toolStripSeparator1,
            this.menuOpenAppFolder,
            this.toolStripSeparator5,
            this.menuRunAsAdministrator});
			this.contextMenuButton.Name = "contextMenuButton";
			this.contextMenuButton.Size = new System.Drawing.Size(225, 82);
			this.contextMenuButton.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuButton_Opening);
			// 
			// menuEditShortcut
			// 
			this.menuEditShortcut.Image = ((System.Drawing.Image)(resources.GetObject("menuEditShortcut.Image")));
			this.menuEditShortcut.Name = "menuEditShortcut";
			this.menuEditShortcut.Size = new System.Drawing.Size(224, 22);
			this.menuEditShortcut.Text = "ショートカットのプロパティ(&R)...";
			this.menuEditShortcut.Click += new System.EventHandler(this.menuEditShortcut_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(221, 6);
			// 
			// menuOpenAppFolder
			// 
			this.menuOpenAppFolder.Image = ((System.Drawing.Image)(resources.GetObject("menuOpenAppFolder.Image")));
			this.menuOpenAppFolder.Name = "menuOpenAppFolder";
			this.menuOpenAppFolder.Size = new System.Drawing.Size(224, 22);
			this.menuOpenAppFolder.Text = "アプリケーション フォルダを開く(&O)";
			this.menuOpenAppFolder.Click += new System.EventHandler(this.menuOpenAppFolder_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(221, 6);
			// 
			// menuRunAsAdministrator
			// 
			this.menuRunAsAdministrator.Image = global::ccMushroom.Properties.Resources.runAs;
			this.menuRunAsAdministrator.Name = "menuRunAsAdministrator";
			this.menuRunAsAdministrator.Size = new System.Drawing.Size(224, 22);
			this.menuRunAsAdministrator.Text = "管理者として実行(A)...";
			this.menuRunAsAdministrator.Click += new System.EventHandler(this.menuRunAsAdministrator_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.BackColor = System.Drawing.SystemColors.ControlLight;
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripScanned,
            this.toolStripVersion,
            this.toolStripMousePosition,
            this.toolStripProgressBar});
			this.statusStrip.Location = new System.Drawing.Point(0, 451);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.statusStrip.ShowItemToolTips = true;
			this.statusStrip.Size = new System.Drawing.Size(632, 22);
			this.statusStrip.TabIndex = 12;
			this.statusStrip.Text = "statusStrip1";
			// 
			// toolStripScanned
			// 
			this.toolStripScanned.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripScanned.DoubleClickEnabled = true;
			this.toolStripScanned.Name = "toolStripScanned";
			this.toolStripScanned.Size = new System.Drawing.Size(52, 17);
			this.toolStripScanned.Text = "Scanned";
			this.toolStripScanned.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripScanned.DoubleClick += new System.EventHandler(this.toolStripScanned_DoubleClick);
			this.toolStripScanned.Click += new System.EventHandler(this.toolStripScanned_Click);
			// 
			// toolStripVersion
			// 
			this.toolStripVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripVersion.DoubleClickEnabled = true;
			this.toolStripVersion.Name = "toolStripVersion";
			this.toolStripVersion.Size = new System.Drawing.Size(463, 17);
			this.toolStripVersion.Spring = true;
			this.toolStripVersion.Text = "Assembly Version";
			this.toolStripVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// toolStripMousePosition
			// 
			this.toolStripMousePosition.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripMousePosition.Name = "toolStripMousePosition";
			this.toolStripMousePosition.Size = new System.Drawing.Size(27, 17);
			this.toolStripMousePosition.Text = "0,0";
			this.toolStripMousePosition.Visible = false;
			// 
			// toolStripProgressBar
			// 
			this.toolStripProgressBar.Name = "toolStripProgressBar";
			this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.notifyIconMenu;
			this.notifyIcon.Text = "ccMushroom";
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
			// 
			// notifyIconMenu
			// 
			this.notifyIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRestoreWindow,
            this.toolStripSeparator2,
            this.toolStripSeparator3,
            this.menuExit});
			this.notifyIconMenu.Name = "contextMenuNotify";
			this.notifyIconMenu.Size = new System.Drawing.Size(172, 60);
			this.notifyIconMenu.Text = "ccMushroom";
			this.notifyIconMenu.Opened += new System.EventHandler(this.notifyIconMenu_Opened);
			this.notifyIconMenu.Opening += new System.ComponentModel.CancelEventHandler(this.notifyIconMenu_Opening);
			this.notifyIconMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.notifyIconMenu_Closing);
			this.notifyIconMenu.Move += new System.EventHandler(this.notifyIconMenu_Move);
			// 
			// menuRestoreWindow
			// 
			this.menuRestoreWindow.Image = ((System.Drawing.Image)(resources.GetObject("menuRestoreWindow.Image")));
			this.menuRestoreWindow.Name = "menuRestoreWindow";
			this.menuRestoreWindow.Size = new System.Drawing.Size(171, 22);
			this.menuRestoreWindow.Text = "元のサイズに戻す(&R)";
			this.menuRestoreWindow.Click += new System.EventHandler(this.menuRestoreWindow_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(168, 6);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(168, 6);
			// 
			// menuExit
			// 
			this.menuExit.Image = ((System.Drawing.Image)(resources.GetObject("menuExit.Image")));
			this.menuExit.Name = "menuExit";
			this.menuExit.Size = new System.Drawing.Size(171, 22);
			this.menuExit.Text = "終了(&X)";
			this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
			// 
			// timerOpacity
			// 
			this.timerOpacity.Tick += new System.EventHandler(this.timerOpacity_Tick);
			// 
			// ccMushroom
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(632, 473);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.tabControl);
			this.Font = new System.Drawing.Font("MS UI Gothic", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximumSize = new System.Drawing.Size(1024, 837);
			this.Name = "ccMushroom";
			this.Text = "ccMushroom";
			this.Load += new System.EventHandler(this.ccMushroom_Load);
			this.SizeChanged += new System.EventHandler(this.ccMushroom_SizeChanged);
			this.Shown += new System.EventHandler(this.ccMushroom_Shown);
			this.Activated += new System.EventHandler(this.ccMushroom_Activated);
			this.Click += new System.EventHandler(this.ccMushroom_Click);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ccMushroom_KeyUp);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ccMushroom_FormClosing);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ccMushroom_KeyDown);
			this.contextMenuTab.ResumeLayout(false);
			this.contextMenuButton.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.notifyIconMenu.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.ContextMenuStrip contextMenuButton;
		private System.Windows.Forms.ToolStripMenuItem menuOpenAppFolder;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripVersion;
		private System.Windows.Forms.ToolStripStatusLabel toolStripScanned;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuEditShortcut;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenuStrip notifyIconMenu;
		private System.Windows.Forms.ToolStripMenuItem menuRestoreWindow;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem menuExit;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.Timer timerOpacity;
		private System.Windows.Forms.ContextMenuStrip contextMenuTab;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuEditTab;
		private System.Windows.Forms.ToolStripStatusLabel toolStripMousePosition;
		public System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuImageLayout;
		private System.Windows.Forms.ToolStripMenuItem tileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem centerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stretchToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuTransButton;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuResumeFomeSize;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem menuRunAsAdministrator;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuForJumpListTabPage;				// 更新されたアプリケーション用のインジケータ
	}
}

