using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using CommonFunctions;

namespace ccMushroom
{
	public partial class TabMngDlg : Form
	{
		private ccMushroom _ccMushroom = null;

		public bool hasImportedApplication;
		public string tabText;
		public string iconFileName;

		private string textTabText;
		private Icon tabIcon = null;

		public Image tabBackground = null;

		private bool nowLoading = true;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TabMngDlg()
		{
			InitializeComponent();

			pictureIcon.AllowDrop = true;
			pictureBackgroundImage.AllowDrop = true;
			checkMoveToLast.Enabled = false;
		}

		/// <summary>
		/// TabMngDlg_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabMngDlg_Load(object sender, EventArgs e)
		{
			try
			{
				_ccMushroom = (ccMushroom)this.Owner;

				IntPtr sysMenuHandle = api.GetSystemMenu(this.Handle, false);
				api.EnableMenuItem(sysMenuHandle, api.SC_CLOSE, api.MF_GRAYED);

				this.Text = "タブのプロパティ"/*"タブ編集ダイアログ"*/;

				string[] tabNames = tabText.Split('\t');
				textTabText = tabNames[0];	// 現在のタブ名の保存用として使う
				comboTabName.Text = tabNames[0];
				for ( int i = 1; i < tabNames.Length; i++ )
				{
					comboTabName.Items.Add(tabNames[i]);
				}

				if ( checkShowIcon.Enabled = (iconFileName != null) )
				{
					tabIcon = ReadTabIconFile(iconFileName);
					pictureIcon.Invalidate();
					checkShowIcon.Enabled = (comboTabName.Text != _ccMushroom.DefaultTabText/*"お気に入り"*/);
					checkShowIcon.Checked = true;
				}

				if ( !hasImportedApplication )
				{
					labelTabText.Enabled = false;
					comboTabName.Enabled = false;
					//labelIcon.Enabled = false;	// リモート側でアイコンが設定されているとは限らないので、一応設定出来るようにしておく
					//pictureIcon.Enabled = false;
					//checkShowIcon.Enabled = false;
				}

				checkEnabledBackground.Enabled = false;
#if ENABLE_TAB_BACKGROUND
				//pictureBackgroundImage.BackgroundImageLayout = ImageLayout.Zoom;
				pictureBackgroundImage.BackColor = Color.Gray;

				string tabPageSettingsIniFileName = Application.StartupPath + "\\" + ccMushroom.TAB_PAGE_SETTINGS_INI_FILE_NAME;
				StringBuilder returnedString = new StringBuilder(1024);

				if ( api.GetPrivateProfileString(textTabText, ccMushroom.KEY_BACKGROUND_IMAGE, "", returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName) != 0 )
				{
					string tabBackgroundFileName = returnedString.ToString();
					if ( File.Exists(tabBackgroundFileName) )
					{
						if ( SetTabBackground(tabBackgroundFileName) )
						{
							checkEnabledBackground.Enabled = true;
							api.GetPrivateProfileString(textTabText, ccMushroom.KEY_ENABLED_BACKGROUND, false.ToString(), returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName);
							checkEnabledBackground.Checked = bool.Parse(returnedString.ToString());

							pictureBackgroundImage.ContextMenuStrip = contextMenuTabBackground;
						}
					}
				}
#else
				groupBox1.Enabled = false;
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				nowLoading = false;
			}
		}

		/// <summary>
		/// TabMngDlg_Shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabMngDlg_Shown(object sender, EventArgs e)
		{
			comboTabName.Select();	// フォーカス失った時に内容を検証させるため
		}

		/// <summary>
		/// [タブ名] が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboTabName_TextChanged(object sender, EventArgs e)
		{
			try
			{
				if ( nowLoading )
					return;

#if true
				bool newTabName = (comboTabName.Items.IndexOf(comboTabName.Text) == -1);

				if ( !newTabName )
				{
					SetTabIcon(null);

					if ( checkShowIcon.Enabled = (textTabText == comboTabName.Text) && (iconFileName != null) )
					{
						checkShowIcon.Enabled = (comboTabName.Text != _ccMushroom.DefaultTabText/*"お気に入り"*/);
					}
				}

				if ( tabBackground != null )
				{
					bool enable = (textTabText == comboTabName.Text);
					checkEnabledBackground.Enabled = enable;
				}

				checkMoveToLast.Enabled = newTabName;
#else
				SetTabIcon(null);

				if ( checkShowIcon.Enabled = ((textTabText == comboTabName.Text) || (comboTabName.Items.IndexOf(comboTabName.Text) == -1)) && (iconFileName != null) )
				{
					checkShowIcon.Enabled = (comboTabName.Text != _ccMushroom.DefaultTabText/*"お気に入り"*/);
				}
				checkMoveToLast.Enabled = (comboTabName.Items.IndexOf(comboTabName.Text) == -1)/*(textTabText != comboTabName.Text)*/;
#endif
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [タブ名] の選択が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboTabName_SelectedIndexChanged(object sender, EventArgs e)
		{
			/*SetTabIcon(null);

			checkShowIcon.Enabled = (textTabText == comboTabName.Text);*/
			checkMoveToLast.Enabled = false;
		}

		/// <summary>
		/// [タブ名] のフォーカスが失われた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboTabName_Leave(object sender, EventArgs e)
		{
			/*if ( textTabText != comboTabName.Text )
			{
				SetTabIcon(null);
			}*/
		}

		/// <summary>
		/// タブアイコンを設定する
		/// </summary>
		/// <param name="tabName"></param>
		private void SetTabIcon(string fileName)
		{
			try
			{
				string _iconFileName = fileName;
				if ( _iconFileName == null )
				{
					_iconFileName = Application.StartupPath + "\\" + ccMushroom.iconsFolder + "\\" + "tab" + comboTabName.Text + ".ico";
					_iconFileName = File.Exists(_iconFileName) ? _iconFileName : null/*iconFileName*/;
				}

				DestroyTabIcon();

				if ( _iconFileName != null )
				{
					if ( fileName != null )
					{
						string fileExt = Path.GetExtension(fileName).ToLower();
						if ( (fileExt == ".exe") || (fileExt == ".dll") )
						{
							string saveIconFileName = Application.StartupPath + "\\" + ccMushroom.iconsFolder + "\\" + "~tab" + comboTabName.Text + ".ico";
							if ( !ccf.ExtractResourceIcon(this.Handle, fileName, saveIconFileName) )
								return;
							_iconFileName = saveIconFileName;
						}
					}

					tabIcon = ReadTabIconFile(_iconFileName);
					//checkShowIcon.Enabled = true;
					checkShowIcon.Checked = true;
					iconFileName = _iconFileName;
				}
				else
				{
					tabIcon = null;
					//checkShowIcon.Enabled = false;
					checkShowIcon.Checked = false;
					iconFileName = null;
				}

				pictureIcon.Invalidate();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// タブアイコン ファイルを読み込む
		/// </summary>
		/// <param name="iconfileName"></param>
		private Icon ReadTabIconFile(string _iconFileName)
		{
			Icon _icon = null;
			//_icon = new Icon(iconFileName, 16, 16);	// これだとアイコン ファイルがロックされて、[OK]後のファイル コピーが出来ない時がある

			using ( FileStream fsIcon = new FileStream(_iconFileName, FileMode.Open, FileAccess.Read) )
			{
				//Bitmap bmp = (Bitmap)Bitmap.FromStream(fsIcon);
				_icon = new Icon(fsIcon, new Size(16, 16));
				fsIcon.Close();
			}

			return _icon;
		}

		/// <summary>
		/// pictureIcon_Paint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureIcon_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				if ( tabIcon == null )
				{
					SolidBrush solidBrush = new SolidBrush(SystemColors.Control);
					e.Graphics.FillRectangle(solidBrush, 0, 0, pictureIcon.Width, pictureIcon.Height);
					solidBrush.Dispose();
					return;
				}

				Rectangle rect = new Rectangle(3, 1, 16, 16);
				e.Graphics.DrawIcon(tabIcon, rect);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// アイコンのファイルのドラッグが開始された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureIcon_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
				{
					e.Effect = DragDropEffects.Copy;
					return;
				}

				e.Effect = DragDropEffects.None;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}

		/// <summary>
		/// アイコンのファイルがドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureIcon_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				object obj = e.Data.GetData(DataFormats.FileDrop);
				string fileName = ((string[])obj)[0];

				TabIconFileSelected(fileName);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// pictureIcon がダブルクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureIcon_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				openFileDialog.Reset();	// こうしないと前回選択したディレクトリが有効になってしまう？
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Title = "アイコン選択ダイアログ";
				openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				openFileDialog.Filter = "リソース ファイル (*.exe;*.dll;*.ico)|*.exe;*.dll;*.ico|全てのファイル (*.*)|*.*";
				openFileDialog.Multiselect = false;

				if ( openFileDialog.ShowDialog(this) != DialogResult.OK )
					return;

				TabIconFileSelected(openFileDialog.FileName);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// アイコンファイルが選択された
		/// </summary>
		/// <param name="fileName"></param>
		private void TabIconFileSelected(string fileName)
		{
			try
			{
				SetTabIcon(fileName);

				if ( checkShowIcon.Enabled = ((textTabText == comboTabName.Text) || (comboTabName.Items.IndexOf(comboTabName.Text) == -1)) )
				{
					checkShowIcon.Enabled = (comboTabName.Text != _ccMushroom.DefaultTabText/*"お気に入り"*/);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// タブの背景画像の有効|無効が切り替わった
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkEnabledBackground_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				if ( nowLoading )
					return;

				if ( checkEnabledBackground.Checked )
				{
					if ( pictureBackgroundImage.ContextMenuStrip == null )
					{
						pictureBackgroundImage.ContextMenuStrip = contextMenuTabBackground;
					}
				}
				else
				{
					pictureBackgroundImage.ContextMenuStrip = null;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// タブの背景画像を設定する
		/// </summary>
		/// <param name="tabName"></param>
		private bool SetTabBackground(string fileName)
		{
			try
			{
				Image _tabBackground = new Bitmap(fileName);

				if ( tabBackground != null )	// 正常に画像が読み込めた時だけ現在の画像を破棄する
				{
					tabBackground.Dispose();
					tabBackground = null;
				}

				tabBackground = _tabBackground;

				toolTip.SetToolTip(pictureBackgroundImage, fileName);

				pictureBackgroundImage.Refresh();

				return true;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
				return false;
			}
		}

		/// <summary>
		/// pictureBackgroundImage_Paint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBackgroundImage_Paint(object sender, PaintEventArgs e)
		{
			try
			{
				if ( tabBackground == null )
				{
					SolidBrush solidBrush = new SolidBrush(SystemColors.Control);
					e.Graphics.FillRectangle(solidBrush, 0, 0, pictureBackgroundImage.Width, pictureBackgroundImage.Height);
					solidBrush.Dispose();
					return;
				}

				/*Rectangle rect = new Rectangle(0, 0, pictureBackgroundImage.Width, pictureBackgroundImage.Height);
				e.Graphics.DrawImage(tabBackground, rect);*/
				Rectangle rect = new Rectangle(0, 0, pictureBackgroundImage.Width - (1 + 1), pictureBackgroundImage.Height - (1 + 1));	// 1:BorderStyle:FixedSingle
				var left = rect.Left;
				var top = rect.Top;
				var width = (float)tabBackground.Width;
				var height = (float)tabBackground.Height;
				if ( true/*isKeepAspectRatio*/ )
				{
					var ratio = height / width;
					width = rect.Height / ratio;
					height = rect.Height;
					if ( width > rect.Width )
					{
						width = rect.Width;
						height = rect.Width * ratio;
					}
					/*var blankA = (rect.Width - (int)width) / 2;		// 左余白
					var blankB = rect.Width - blankA - (int)width;	// 右余白
					left += Math.Max(0, Math.Min(blankA, blankB));
					blankA = (rect.Height - (int)height) / 2;		// 上余白
					blankB = rect.Height - blankA - (int)height;	// 下余白
					top += Math.Max(0, Math.Min(blankA, blankB));*/
					width = (float)Math.Round(width, 0);
					height = (float)Math.Round(height, 0);
					left += (int)(rect.Width - width) / 2;
					top += (int)(rect.Height - height) / 2;
				}
				e.Graphics.DrawImage(tabBackground, left, top, width, height);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// タブの背景画像ファイルのドラッグが開始された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBackgroundImage_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
				{
					e.Effect = DragDropEffects.Copy;
					return;
				}

				e.Effect = DragDropEffects.None;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}

		/// <summary>
		/// タブの背景画像ファイルがドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBackgroundImage_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				object obj = e.Data.GetData(DataFormats.FileDrop);
				string fileName = ((string[])obj)[0];

				TabBackgroundFileSelected(fileName);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// pictureBackgroundImage がダブルクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBackgroundImage_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				openFileDialog.Reset();	// こうしないと前回選択したディレクトリが有効になってしまう？
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Title = "背景画像選択ダイアログ";
				openFileDialog.InitialDirectory = (tabBackground != null) ? Path.GetDirectoryName(toolTip.GetToolTip(pictureBackgroundImage)) : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				openFileDialog.Filter = "画像ファイル (*.jpeg;*.jpg;*.gif;*.bmp)|*.jpeg;*.jpg;*.gif;*.bmp|全てのファイル (*.*)|*.*";
				openFileDialog.Multiselect = false;

				if ( openFileDialog.ShowDialog(this) != DialogResult.OK )
					return;

				TabBackgroundFileSelected(openFileDialog.FileName);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// タブの背景画像ファイルが選択された
		/// </summary>
		/// <param name="fileName"></param>
		private void TabBackgroundFileSelected(string fileName)
		{
			try
			{
				if ( SetTabBackground(fileName) )
				{
					checkEnabledBackground.Enabled = true;
					checkEnabledBackground.Checked = true;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// contextMenuTabBackground が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuTabBackground_Opening(object sender, CancelEventArgs e)
		{
			try
			{
				toolStripMenuRemoveTabBackground.Enabled = (checkEnabledBackground.Enabled && (tabBackground != null));
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [背景画像登録を解除する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuRemoveTabBackground_Click(object sender, EventArgs e)
		{
			try
			{
				checkEnabledBackground.Enabled = false;
				checkEnabledBackground.Checked = false;

				tabBackground.Dispose();
				tabBackground = null;

				toolTip.SetToolTip(pictureBackgroundImage, null);
				pictureBackgroundImage.Refresh();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 入力されたテキストの有効性を検証する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				Control control = (Control)sender;
				bool validate = true;
				string errorMessage = "input error...";

				if ( control.Text.Length == 0 )
				{
					errorMessage = "文字列が空です";
					validate = false;
				}
				else
				{
				}

				if ( !validate )
				{
					errorProvider.SetError(control, errorMessage);
					// e.Cancel = true　でCancel を true にすると正しく入力しないと次に行けない。
					e.Cancel = true;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// テキストの検証が終了した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox_Validated(object sender, EventArgs e)
		{
			try
			{
				this.errorProvider.SetError((Control)sender, null);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [OK] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonOk_Click(object sender, EventArgs e)
		{
			try
			{
				tabText = comboTabName.Text;

				if ( checkShowIcon.Checked )
				{
					SetTabIcon(iconFileName);
				}

#if ENABLE_TAB_BACKGROUND
				string tabPageSettingsIniFileName = Application.StartupPath + "\\" + ccMushroom.TAB_PAGE_SETTINGS_INI_FILE_NAME;

				api.WritePrivateProfileString(comboTabName.Text, ccMushroom.KEY_ENABLED_BACKGROUND, checkEnabledBackground.Checked.ToString().ToLower(), tabPageSettingsIniFileName);

				if ( tabBackground != null )
				{
					string tabBackgroundFileName = toolTip.GetToolTip(pictureBackgroundImage);

					api.WritePrivateProfileString(comboTabName.Text, ccMushroom.KEY_BACKGROUND_IMAGE, tabBackgroundFileName, tabPageSettingsIniFileName);

					tabBackground.Tag = tabBackgroundFileName;
				}
				else
				{
					DeleteTabPageSection(comboTabName.Text);
				}
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

#if ENABLE_TAB_BACKGROUND
		/// <summary>
		/// タブ名のセクションを削除する
		/// </summary>
		/// <param name="tabName"></param>
		public static void DeleteTabPageSection(string tabName)
		{
			try
			{
				string tabPageSettingsIniFileName = Application.StartupPath + "\\" + ccMushroom.TAB_PAGE_SETTINGS_INI_FILE_NAME;

				api.WritePrivateProfileString(tabName, null, null, tabPageSettingsIniFileName);

				byte[] returnedByte = new byte[0xffff];
				int count = (int)api.GetPrivateProfileSectionNames(returnedByte, (uint)returnedByte.Length, tabPageSettingsIniFileName);
				if ( count == 0 )
				{
					File.Delete(tabPageSettingsIniFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#endif

		/// <summary>
		/// [キャンセル] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			CleanupTempFile(comboTabName.Text);

			if ( tabBackground != null )
			{
				tabBackground.Dispose();
				tabBackground = null;
			}
		}

		/// <summary>
		/// 一時ファイルを削除する
		/// </summary>
		/// <param name="tabText"></param>
		public void CleanupTempFile(string tabText)
		{
			try
			{
				string _tabIconFileName = Application.StartupPath + "\\" + ccMushroom.iconsFolder + "\\" + "~tab" + tabText + ".ico";
				if ( File.Exists(_tabIconFileName) )
				{
					File.Delete(_tabIconFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// フォームが閉じられようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabMngDlg_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				DestroyTabIcon();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// tabIcon を解放する
		/// </summary>
		private void DestroyTabIcon()
		{
			try
			{
				if ( tabIcon == null )
					return;

				api.DestroyIcon(tabIcon.Handle);
				tabIcon.Dispose();
				tabIcon = null;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

#if false
		/// <summary>
		/// フォームへのドラッグが開始された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabMngDlg_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				e.Effect = DragDropEffects.None;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}

		/// <summary>
		/// フォームでのドラッグ中
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabMngDlg_DragOver(object sender, DragEventArgs e)
		{
			try
			{
				Point m = PointToClient(new Point(e.X, e.Y));

				if ( (pictureIcon.Location.X <= m.X) && (m.X <= pictureIcon.Location.X + pictureIcon.Width) &&
					 (pictureIcon.Location.Y <= m.Y) && (m.Y <= pictureIcon.Location.Y + pictureIcon.Height) )
				{
					if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
					{
						e.Effect = DragDropEffects.Copy;
						return;
					}
				}

				e.Effect = DragDropEffects.None;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}

		/// <summary>
		/// フォームへドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TabMngDlg_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				Point m = PointToClient(new Point(e.X, e.Y));

				// pictureIcon にドロップされた？
				if ( (pictureIcon.Location.X <= m.X) && (m.X <= pictureIcon.Location.X + pictureIcon.Width) &&
					 (pictureIcon.Location.Y <= m.Y) && (m.Y <= pictureIcon.Location.Y + pictureIcon.Height) )
				{
					object obj = e.Data.GetData(DataFormats.FileDrop);
					string fileName = ((string[])obj)[0];

					tabIcon = new Icon(fileName);
					pictureIcon.Invalidate();

					checkShowIcon.Enabled = true;
					checkShowIcon.Checked = true;
					iconFileName = fileName;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
#endif
	}
}
