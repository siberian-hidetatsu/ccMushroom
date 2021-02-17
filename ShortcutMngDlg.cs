#define	_USE_TOOLTIP_AS_ICON_FILENAME
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using CommonFunctions;

namespace ccMushroom
{
	public partial class ShortcutMngDlg : Form
	{
		private ccMushroom _ccMushroom = null;

		public XmlDocument xmlCcConfigurationImport;
		public string tabText;
		public string buttonText;
		public string appName;
		public string commandLine = string.Empty;
		public string workingDirectory = string.Empty;
		public string comment = string.Empty;
		public string iconFile = null;
		public bool autoExec = false;
		public string buttonBackColor = null;
		public bool newApp = false;

		private usage _usage;
		private string currentButtonText = null;
		private Icon appIcon = null;

		public enum usage { import, edit };

		private static string iconsFolderPath = Application.StartupPath + "\\" + ccMushroom.iconsFolder;
		private string[] tempFaviconFiles = { iconsFolderPath + "\\~favicon.ico", iconsFolderPath + "\\~~favicon.ico" };
		private string[] tempAppiconFiles = { iconsFolderPath + "\\~appicon.ico", iconsFolderPath + "\\~~appicon.ico" };
		private enum ic { temp, saved };

		private readonly string pictureIconToolTip = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShortcutMngDlg(usage _usage, string currentButtonText)
		{
			InitializeComponent();

			//checkAutoExec.Location = new Point(labelCountDown.Location.X, labelCountDown.Location.Y + labelCountDown.Height - checkAutoExec.Height);
			checkAutoExec.Location = new Point(pictureIcon.Location.X, textTabText.Location.Y + textTabText.Height - checkAutoExec.Height);

			this._usage = _usage;	// 新規|編集

			if ( _usage == usage.import )
			{
				comboTabName.Visible = false;
			}
			else if ( _usage == usage.edit )
			{
				this.currentButtonText = currentButtonText;
				textTabText.Visible = false;
				labelCountDown.Visible = false;
				checkAutoExec.Visible = !labelCountDown.Visible;
			}

			pictureIcon.AllowDrop = true;

#if USE_TOOLTIP_AS_ICON_FILENAME
			toolTip.SetToolTip(pictureIcon, null);
#else
			pictureIconToolTip = toolTip.GetToolTip(pictureIcon);
#endif
		}

		/// <summary>
		/// ShortcutMngDlg_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShortcutMngDlg_Load(object sender, EventArgs e)
		{
			try
			{
				_ccMushroom = (ccMushroom)this.Owner;

				IntPtr sysMenuHandle = api.GetSystemMenu(this.Handle, false);
				api.EnableMenuItem(sysMenuHandle, api.SC_CLOSE, api.MF_GRAYED);

				if ( _usage == usage.import )
				{
					this.Text = "新規ショートカットのプロパティ"/*"ショートカット取り込みダイアログ"*/;
					textTabText.Text = tabText;
					checkMoveToLast.Checked = true;
					if ( ccMushroom.shortcutMngApplyTime != "0" )
					{
						foreach ( Control control in this.Controls )
						{
							if ( !(control is TextBox) )
								continue;
							control.Click += new System.EventHandler(this.ShortcutMngDlg_Click);
						}
						labelCountDown.Visible = true;
						labelCountDown.Text = ccMushroom.shortcutMngApplyTime;
						timerAutoApply.Interval = 1000;
						timerAutoApply.Start();
					}
				}
				else if ( _usage == usage.edit )
				{
					this.Text = "ショートカットのプロパティ"/*"ショートカット編集ダイアログ"*/;
					string[] tabNames = tabText.Split('\t');
					textTabText.Text = tabNames[0];	// 現在のタブ名の保存用として使う
					comboTabName.Text = tabNames[0];
					for ( int i = 1; i < tabNames.Length; i++ )
					{
						comboTabName.Items.Add(tabNames[i]);
					}
					comboTabName.Location = textTabText.Location;
					comboTabName.Size = textTabText.Size;
					comboTabName.TabIndex = textTabText.TabIndex;
					checkDelete.Enabled = true;
				}

				textButtonText.Text = buttonText;
				textAppName.Text = appName;
				textCommandLine.Text = commandLine;
				textWorkingDirectory.Text = workingDirectory;
				textComment.Text = comment;

				string appIconFile = appName;
				if ( iconFile != null )		// 個別のアイコン指定あり？
				{
					if ( File.Exists(iconFile) )
					{
						appIconFile = iconFile;
						SetAppIconName(appIconFile, null, false);
					}
					else
					{
						iconFile = null;	// 個別に設定されたアイコン ファイルが存在しない場合、後の処理でこのノードを削除する為に null にしておく
					}
				}
				UpdateAppIcon(appIconFile);

#if !USE_TOOLTIP_AS_ICON_FILENAME
				toolTip.SetToolTip(pictureIcon, pictureIconToolTip + ((Program.debMode && (iconFile != null)) ? "\r\n" + iconFile : ""));
#endif

				checkAutoExec.Checked = autoExec;

				string ext = Path.GetExtension(textAppName.Text);
				checkParseShortcut.Enabled = ((string.Compare(ext, ".lnk", true) == 0) || (string.Compare(ext, ".url", true) == 0));
				checkDelete.Enabled = (_usage == usage.edit);
				checkMoveToLast.Enabled = false;
				checkNewApp.Checked = newApp;
				if ( !Program.debMode )
				{
					checkNewApp.Visible = false;
				}

				textFaviconUrl.Visible = false;
				toolTip.SetToolTip(textFaviconUrl, "\"http://ホスト名/\" 指定されたホスト名でアイコンを設定する\n\"http://ホスト名/favicon.ico\" 実際のホスト名でアイコンを設定する");
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

#if USE_TOOLTIP_AS_ICON_FILENAME
		/// <summary>
		/// アイコンファイル名を pictureIcon にセットする
		/// </summary>
		/// <param name="appIconFile"></param>
		/// <param name="actualAppIconFile"></param>
		/// <param name="withUpdate"></param>
		private void SetAppIconName(string appIconFile, string actualAppIconFile, bool withUpdate)
		{
			toolTip.SetToolTip(pictureIcon, appIconFile);	// ToolTip がある == 個別にアイコンが指定されている
			pictureIcon.Tag = actualAppIconFile;			// 実際のファイル名

			if ( withUpdate )
			{
				UpdateAppIcon((actualAppIconFile == null) ? appIconFile : actualAppIconFile);
			}
		}
#else
		/// <summary>
		/// アイコンファイル名を pictureIcon にセットする
		/// </summary>
		/// <param name="appIconFile">null 以外 == 個別にアイコンが指定されている</param>
		/// <param name="actualAppIconFile">実際のファイル名</param>
		/// <param name="withUpdate"></param>
		private void SetAppIconName(string appIconFile, string actualAppIconFile, bool withUpdate)
		{
			pictureIcon.Tag = new string[] { appIconFile, actualAppIconFile };

			if ( withUpdate )
			{
				UpdateAppIcon((actualAppIconFile == null) ? appIconFile : actualAppIconFile);
			}

			if ( Program.debMode )
			{
				toolTip.SetToolTip(pictureIcon, pictureIconToolTip + "\r\n" + appIconFile);
			}
		}
#endif

#if USE_TOOLTIP_AS_ICON_FILENAME
		/// <summary>
		/// アイコンファイル名を pictureIcon から取得する
		/// </summary>
		/// <returns></returns>
		private string GetAppIconName()
		{
			string appIconFile = toolTip.GetToolTip(pictureIcon);
			return (appIconFile.Length == 0) ? null : appIconFile;
		}
#else
		/// <summary>
		/// アイコンファイル名を pictureIcon から取得する
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private string GetAppIconName(int type)
		{
			if ( pictureIcon.Tag == null )
				return null;

			string[] appIconFileNames = (string[])pictureIcon.Tag;
			return string.IsNullOrEmpty(appIconFileNames[type]) ? null : appIconFileNames[type];
		}
		private string GetAppIconName()
		{
			return GetAppIconName(0);
		}
#endif

		/// <summary>
		/// appIcon にアイコンをセットして pictureIcon を更新する
		/// </summary>
		/// <param name="appIconFile"></param>
		private void UpdateAppIcon(string appIconFile)
		{
			DestroyAppIcon();

			appIcon = ccf.GetIcon(common.CheckEnvironmentVariable(appIconFile), true);
			pictureIcon.Invalidate();
		}

		/// <summary>
		/// appIcon を解放する
		/// </summary>
		private void DestroyAppIcon()
		{
			try
			{
				if ( appIcon == null )
					return;

				api.DestroyIcon(appIcon.Handle);
				appIcon.Dispose();
				appIcon = null;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ShortcutMngDlg_Shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShortcutMngDlg_Shown(object sender, EventArgs e)
		{
			textButtonText.Select();	// フォーカス失った時に内容を検証させるため
			//textButtonText.Select(textButtonText.Text.Length, 0);
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
				if ( _usage == usage.edit )
				{
					checkMoveToLast.Enabled = (textTabText.Text/*tabText*/ != comboTabName.Text);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

		}

		/// <summary>
		/// [アプリケーション] がフォーカスを失った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textAppName_Leave(object sender, EventArgs e)
		{
			try
			{
				if ( GetAppIconName() != null )	// アイコンは個別に指定されている？
					return;

				UpdateAppIcon(common.CheckEnvironmentVariable(textAppName.Text));
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
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
				if ( !pictureIcon.Enabled )
				{
					SolidBrush solidBrush = new SolidBrush(SystemColors.Control);
					e.Graphics.FillRectangle(solidBrush, 0, 0, pictureIcon.Width, pictureIcon.Height);
					solidBrush.Dispose();
					return;
				}

				if ( appIcon == null )
					return;

				int iconX = 3;
				int iconY = 1;

				e.Graphics.DrawIcon(appIcon, iconX, iconY);
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
				if ( textButtonText.Text.Length == 0 )
					return;

				object obj = e.Data.GetData(DataFormats.FileDrop);
				string fileName = ((string[])obj)[0];

				AppIconFileSelected(fileName);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
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
				if ( textAppName.Text.Length == 0 )
					openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				else if ( Path.GetExtension(textAppName.Text).EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase) )
					openFileDialog.InitialDirectory = Path.GetDirectoryName(common.CheckEnvironmentVariable(textAppName.Text));
				else
					openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				openFileDialog.Filter = "リソース ファイル (*.exe;*.dll;*.ico)|*.exe;*.dll;*.ico|すべてのファイル (*.*)|*.*";
				openFileDialog.Multiselect = false;

				if ( openFileDialog.ShowDialog(this) != DialogResult.OK )
					return;

				AppIconFileSelected(openFileDialog.FileName);
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
		private void AppIconFileSelected(string fileName)
		{
			try
			{
#if true
				if ( appIcon != null )
				{
					DestroyAppIcon();

					if ( StartsWithHttp() )
					{
						DeleteFaviconFile(GetAppIconName(), false);
					}
					else
					{
						DeleteAppIconFile(GetAppIconName());
					}
				}
#endif

				string fileExt = Path.GetExtension(fileName).ToLower();
				if ( (fileExt == ".exe") || (fileExt == ".dll") )
				{
					SaveTempIconFile(tempAppiconFiles);

					if ( !ccf.ExtractResourceIcon(this.Handle, fileName, tempAppiconFiles[(int)ic.temp]) )
						return;
					string appIconFile = iconsFolderPath + "\\" + (StartsWithHttp() ? "favicon@" : "appicon@") + GetCorrectFileName(textButtonText.Text) + ".ico";

					SetAppIconName(appIconFile, tempAppiconFiles[(int)ic.temp], true);
				}
				else
				{
					appIcon = new Icon(fileName, 16, 16);
					pictureIcon.Invalidate();

					SetAppIconName(fileName, null, false);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 一時アイコンファイルを保存して待避しておく
		/// </summary>
		/// <param name="tempIconFiles"></param>
		private void SaveTempIconFile(string []tempIconFiles)
		{
#if true
			try
			{
				if ( !File.Exists(tempIconFiles[(int)ic.temp]) )	// 既に現在のアイコン（[アイコン指定を解除する] が実行された場合）が一時ファイルになっていない？
					return;

				// 現在のアイコンを待避しておく
				File.Copy(tempIconFiles[(int)ic.temp], tempIconFiles[(int)ic.saved], false);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif
		}

		/// <summary>
		/// ファイル名に使えない文字を全角にする
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private string GetCorrectFileName(string fileName)
		{
			char[] oldChar = { '\\', '/', ':', '*', '?', '\"', /*'<', '>',*/ '|' };
			char[] newChar = { '￥', '／', '：', '＊', '？', '”', /*'＜', '＞',*/ '｜' };

			for ( int i = 0; i < oldChar.Length; i++ )
			{
				fileName = fileName.Replace(oldChar[i], newChar[i]);
			}

			return fileName;
		}

		/// <summary>
		/// [背景色の選択] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelectBackColor_Click(object sender, EventArgs e)
		{
			try
			{
				//this.AcceptButton = null;	// null にすると、comboBackColor の入力が Enter キーで確定するようになる

				int right = buttonSelectBackColor.Location.X + buttonSelectBackColor.Width;
				int x = right - comboBackColor.Width;
				//comboBackColor.Location = pictureIcon.Location;
				comboBackColor.Location = new Point(x, buttonSelectBackColor.Location.Y);
				comboBackColor.Visible = true;			// comboBackColor コントロールを表示する
				comboBackColor.BringToFront();

				int offset = pictureIcon.Location.X - x;
				textButtonText.Width -= offset;			// ボタン名の入力領域を縮める
				textButtonText.Tag = offset;

				if ( comboBackColor.Items.Count == 0 )
				{
					Cursor.Current = Cursors.WaitCursor;

					//comboBackColor.ItemHeight =	comboBackColor.Font.Height + 3/*MARGIN_ITEM_TO_STRING*/ * 2;	// 項目の高さ
					//comboBackColor.Size = new Size(150, comboBackColor.Height);	// 高さはItemHeightに応じて自動調整される

					// 列挙型はGetNamesメソッドで定数名を列挙できる
					// KnownColorの色名を列挙
					/*foreach ( string knownColorName in System.Enum.GetNames(typeof(KnownColor)) )
					{
						comboBackColor.Items.Add(knownColorName);
					}*/
					foreach ( KnownColor knownColor in Enum.GetValues(typeof(KnownColor)) )
					{
						Color color = Color.FromKnownColor(knownColor);
						if ( color.IsSystemColor )
							continue;
						int index = comboBackColor.Items.Add(knownColor);

						if ( knownColor.ToString() == buttonBackColor )
						{
							comboBackColor.SelectedIndex = index;
						}
					}

					Cursor.Current = Cursors.Default;
				}

				comboBackColor.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ボタンの背景色を comboBackColor に描画する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBackColor_DrawItem(object sender, DrawItemEventArgs e)
		{
			if ( e.Index == -1 )
				return;	// 未選択時は処理せず抜ける

			try
			{
				//　★定数が表すマージンの意味
				// ┌─────────┐←コンボボックスの1項目分の範囲の矩形(ITEM)
				// │┌───────┐←─コンボボックス内で該当する色で塗りつぶす分の矩形(COLORBOX)
				// ││　DarkBlue←────色名の文字列(STRING)
				// │└───────┘│
				// └─────────┘
				int MARGIN_ITEM_TO_COLORBOX = 1/*2*/;
				int MARGIN_COLORBOX_TO_STRING = 1;
				int MARGIN_ITEM_TO_STRING = MARGIN_ITEM_TO_COLORBOX + MARGIN_COLORBOX_TO_STRING;

				ComboBox combo = (ComboBox)sender;
				string colorName = combo.Items[e.Index].ToString();
				Color color = /*(colorName == Color.Transparent.Name) ? SystemColors.Window : */Color.FromName(colorName);
				Rectangle rect = new Rectangle(MARGIN_ITEM_TO_COLORBOX, MARGIN_ITEM_TO_COLORBOX, e.Bounds.Width - MARGIN_ITEM_TO_COLORBOX * 2, e.Bounds.Height - MARGIN_ITEM_TO_COLORBOX * 2);
				PointF colorNameLocation = new PointF(rect.Left + MARGIN_COLORBOX_TO_STRING, rect.Top + MARGIN_COLORBOX_TO_STRING);

				using ( Bitmap bmp = new Bitmap(e.Bounds.Width, e.Bounds.Height) )
				using ( Graphics g = Graphics.FromImage(bmp) )
				using ( Brush foreBrush = new SolidBrush((color.GetBrightness() >= 0.4) ? Color.Black : Color.White) )
				using ( Brush backBrush = new SolidBrush(color) )
				using ( Pen framePen = new Pen(Color.Black) )
				{
					// ビットマップオブジェクト(裏画面)に描画
					g.FillRectangle(backBrush, rect);
					g.DrawRectangle(framePen, rect);

					//g.DrawString(itemColor.Name, combo.Font, foreBrush, colorNameLocation);
					if ( colorName == Color.Transparent.Name )
					{
						g.DrawString("default", combo.Font, foreBrush, colorNameLocation);
					}

					e.Graphics.DrawImage(bmp, e.Bounds);	// ビットマップに描画した内容を実際の画面に反映
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ボタンの背景色が選択された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBackColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			ChangeButtonBackColor(((ComboBox)sender).SelectedItem.ToString());
		}

		/// <summary>
		/// comboBackColor でキーが上がった
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBackColor_KeyUp(object sender, KeyEventArgs e)
		{
			Debug.Write(e.KeyData);

			// this.AcceptButton の設定により、[Enter] キー入力後に確定するか否かが決まる
			if ( (e.KeyData == Keys.Enter) && (this.AcceptButton == null) )
			{
				comboBackColor.Visible = false;
			}
		}

		/// <summary>
		/// comboBackColor がフォーカスを失った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBackColor_Leave(object sender, EventArgs e)
		{
			if ( comboBackColor.Tag != null )
				return;

			try
			{
				comboBackColor.Tag = "nowLeaving";
				Debug.WriteLine(" " + comboBackColor.Tag);

				if ( this.AcceptButton == null )
				{
					this.AcceptButton = buttonOk;
				}

				Point point = Cursor.Position;

				if ( buttonCancel.ClientRectangle.Contains(buttonCancel.PointToClient(point)) )	// [キャンセル] ボタンが押された？
					return;

				if ( comboBackColor.Text.Length != 0 )	// 色名称の入力がある？
				{
					int index;
					for ( index = 0; (index < comboBackColor.Items.Count) && (string.Compare(comboBackColor.Items[index].ToString(), comboBackColor.Text, true) != 0); index++ ) ;
					if ( index == comboBackColor.Items.Count )	// 無効な色名称？
					{
						if ( buttonBackColor != null )
						{
							comboBackColor.Text = buttonBackColor;
						}
					}
					else
					{
						if ( comboBackColor.SelectedItem == null )	// 現在選択されている色と違う色名称が入力されている？
						{
							comboBackColor.Text = comboBackColor.Items[index].ToString();
							ChangeButtonBackColor(comboBackColor.Text);
						}
					}
				}

				comboBackColor.Visible = false;

				textButtonText.Width += (int)textButtonText.Tag;	// ボタン名の入力領域を元に戻す
				textButtonText.Tag = null;

				if ( buttonOk.ClientRectangle.Contains(buttonOk.PointToClient(point)) )	// [Ok] ボタンが押された？
					return;

				/*textButtonText.Select();	// この直後にも Leave イベントが発生するので、Tag で制御している
				textButtonText.Select(textButtonText.Text.Length, 0);
				//textButtonText.SelectionLength = 0;*/
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
			finally
			{
				comboBackColor.Tag = null;
			}
		}

		/// <summary>
		/// ボタンの背景色を変更する
		/// </summary>
		/// <param name="colorName"></param>
		private void ChangeButtonBackColor(string colorName)
		{
			try
			{
				buttonBackColor = colorName;

				if ( buttonBackColor == Color.Transparent.Name )
				{
					buttonBackColor = null;
					textButtonText.BackColor = ccMushroom.buttonBackColor/*SystemColors.Window*/;
					//toolTip.SetToolTip(comboBackColor, "ボタンの背景色");
				}
				else
				{
					textButtonText.BackColor = Color.FromName(buttonBackColor);
					//toolTip.SetToolTip(comboBackColor, buttonBackColor);
				}

				float backBrightness = textButtonText.BackColor.GetBrightness();
				textButtonText.ForeColor = (backBrightness < 0.4) ? Color.White : Color.Black;
				Debug.WriteLine(textButtonText.BackColor + " " + backBrightness);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ShortcutMngDlg がクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShortcutMngDlg_Click(object sender, EventArgs e)
		{
			StopTimerAutoApply();

			if ( comboBackColor.Visible )
			{
				comboBackColor.Visible = false;	// この直後に comboBackColor_Leave イベントが発生する

				textButtonText.Select();		// comboBackColor_Leave 処理後に実行される
				textButtonText.Select(textButtonText.Text.Length, 0);
			}
		}

		/// <summary>
		/// timerAutoApply を停止する
		/// </summary>
		private void StopTimerAutoApply()
		{
			try
			{
				if ( timerAutoApply.Enabled )
				{
					labelCountDown.Visible = false;
					timerAutoApply.Stop();

					checkAutoExec.Visible = !labelCountDown.Visible;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 自動的に適用の処理を行う
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerAutoApply_Tick(object sender, EventArgs e)
		{
			try
			{
				labelCountDown.Text = (int.Parse(labelCountDown.Text) - 1).ToString();
				if ( labelCountDown.Text != "0" )
					return;

				timerAutoApply.Stop();
				labelCountDown.Visible = false;

				buttonOk.Select();
				Application.DoEvents();

				bool isValidating = false;
				foreach ( Control control in this.Controls )
				{
					if ( !(control is TextBox) )
						continue;

					if ( !string.IsNullOrEmpty(errorProvider.GetError(control)) )	// テキストボックスの検証中？
					{
						isValidating = true;
						break;
					}
				}

				if ( isValidating )
					return;

				OkProcess();

				DialogResult = DialogResult.OK;
				this.Close();
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
					if ( control.Name == textButtonText.Name )
					{
						buttonText = textButtonText.Text;
						buttonText = buttonText.Replace('&', '＆');
						buttonText = buttonText.Replace('<', '＜');
						buttonText = buttonText.Replace('>', '＞');
						buttonText = buttonText.Replace('"', '”');
						buttonText = buttonText.Replace('\'', '’');
						/*buttonText = buttonText.Replace('\\', '￥');
						buttonText = buttonText.Replace('/', '／');
						buttonText = buttonText.Replace(':', '：');
						buttonText = buttonText.Replace('*', '＊');
						buttonText = buttonText.Replace('?', '？');
						buttonText = buttonText.Replace('\"', '”');
						buttonText = buttonText.Replace('|', '｜');*/
						textButtonText.Text = buttonText;

						if ( (currentButtonText == null) || (currentButtonText != control.Text) )
						{
							string xpath = ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_BUTTON_TEXT + "='" + control.Text + "']";
							XmlNode applicationImport = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);
							if ( applicationImport != null )
							{
								errorMessage = "そのボタン名は既に登録されています";
								validate = false;
							}
						}
					}
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
		/// [アプリケーション選択] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelectApp_Click(object sender, EventArgs e)
		{
			try
			{
				openFileDialog.Reset();	// こうしないと前回選択したディレクトリが有効になってしまう？
				openFileDialog.RestoreDirectory = true;
				openFileDialog.Title = "アプリケーション選択ダイアログ";
				if ( textAppName.Text.Length == 0 )
					openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
				else
					openFileDialog.InitialDirectory = Path.GetDirectoryName(common.CheckEnvironmentVariable(textAppName.Text));
				openFileDialog.Filter = "実行可能ファイル (*.exe)|*.exe|バッチ ファイル (*.bat)|*.bat|すべてのファイル (*.*)|*.*";
				openFileDialog.Multiselect = false;

				if ( openFileDialog.ShowDialog(this) != DialogResult.OK )
					return;

				if ( !textAppName.Text.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) &&
					 !textAppName.Text.EndsWith(".com", StringComparison.OrdinalIgnoreCase) &&
					 !textAppName.Text.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) )
				{
					if ( MessageBox.Show("現在のアプリケーション設定をコマンドラインとして使用しますか？", "アプリケーション選択", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes )
					{
						bool withSpacePath = false;
						string commandLine = textAppName.Text;
						if ( (commandLine.IndexOf('\\') != -1) && commandLine.IndexOf(' ') != -1 )
						{
							withSpacePath = (commandLine.IndexOf('/', 1) == -1);
						}
						textCommandLine.Text = (withSpacePath ? "\"" : "") + commandLine + (withSpacePath ? "\"" : "");

						if ( GetAppIconName() == null )
						{
							SetAppIconName(textAppName.Text, null, false);
						}
					}
				}

				string fileName = openFileDialog.FileName;
				//textButtonText.Text = Path.GetFileNameWithoutExtension(fileName);
				textAppName.Text = fileName;
				textAppName.Select(textAppName.Text.Length, 0);

				if ( GetAppIconName() == null )
				{
					UpdateAppIcon(fileName);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				textButtonText.Select();
				textButtonText.Select(textButtonText.Text.Length, 0);
			}
		}

		/// <summary>
		/// [解析] がチェックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkParseShortcut_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				StopTimerAutoApply();

				if ( checkParseShortcut.Checked )
				{
					string ext = Path.GetExtension(textAppName.Text);
					if ( string.Compare(ext, ".lnk", true) == 0 )
					{
						ShellLink shortcut = new ShellLink(textAppName.Text);
						textAppName.Text = shortcut.TargetPath;
						textCommandLine.Text = shortcut.Arguments;
						if ( Path.GetDirectoryName(textAppName.Text) != shortcut.WorkingDirectory )
						{
							textWorkingDirectory.Text = shortcut.WorkingDirectory;
						}
						textComment.Text = shortcut.Description;
					}
					else if ( string.Compare(ext, ".url", true) == 0 )
					{
						string iniFileName = textAppName.Text;
						StringBuilder returnedString = new StringBuilder(1024);

						if ( api.GetPrivateProfileString("InternetShortcut", "URL", "\0", returnedString, (uint)returnedString.Capacity, iniFileName) == 0 )
							return;
						string ieUrl = returnedString.ToString();

						//string iexplore = @"%ProgramFiles%\Internet Explorer\iexplore.exe";
						//textAppName.Text = iexplore;
						//textCommandLine.Text = ieUrl;
						textAppName.Text = ieUrl;
						textCommandLine.Text = "";

						string faviconFile = null;
						// favicon.ico あり？
						if ( api.GetPrivateProfileString("InternetShortcut", "IconFile", "\0", returnedString, (uint)returnedString.Capacity, iniFileName) != 0 )
						{
							if ( (faviconFile = DownloadFavicon(new Uri(returnedString.ToString()), null)) != null )
							{
								SetAppIconName(faviconFile, tempFaviconFiles[(int)ic.temp], true);
								return;
							}
						}

						if ( (faviconFile = GetUrlDllIconFile()) != null )
						{
							SetAppIconName(faviconFile, null, true);
						}

						return;
					}
				}
				else
				{
					textAppName.Text = appName;
					textCommandLine.Text = commandLine/*string.Empty*/;
					textWorkingDirectory.Text = workingDirectory/*string.Empty*/;
					textComment.Text = comment/*string.Empty*/;

					SetAppIconName(null, null, false);
				}

				UpdateAppIcon(common.CheckEnvironmentVariable(textAppName.Text));
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// favicon.ico をダウンロードしてファイルに保存する
		/// </summary>
		/// <param name="iconUri"></param>
		/// <param name="hostName"></param>
		/// <returns></returns>
		private string DownloadFavicon(Uri iconUri, string hostName)
		{
			string faviconFile = null;

			try
			{
				Cursor.Current = Cursors.WaitCursor;

				try
				{
					if ( File.Exists(faviconFile) )
					{
						File.Delete(faviconFile);
					}

#if false
					CookieContainer aCookieContainer = new CookieContainer();

					// ここにログインのPOST処理を挿入する。
					// 説明のため便宜上クエリ文字列にユーザー名をパスを与えるようにします。
					// 本当はフォームのPOSTのコードにします。（＾＾；
					string aURL = "http://ccmushroom.com/wmredirect.nsf?username=" + "user" + "&password=" + "pass";

					HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(aURL);
					webreq.CookieContainer = new CookieContainer();
					webreq.CookieContainer.Add(aCookieContainer.GetCookies(webreq.RequestUri));
					HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
					CookieCollection cookies = webreq.CookieContainer.GetCookies(webreq.RequestUri);
					aCookieContainer.Add(cookies);
					Stream st = webres.GetResponseStream();
					StreamReader sr = new StreamReader(st, Encoding.GetEncoding("shift_jis"));
					string aspx = sr.ReadToEnd();
					sr.Close();
#endif

					SaveTempIconFile(tempFaviconFiles);

					WebClient webClient = new WebClient();
					webClient.DownloadFile(iconUri.AbsoluteUri, tempFaviconFiles[(int)ic.temp]);

					faviconFile = iconsFolderPath + "\\" + "favicon@" + (hostName == null ? iconUri.Host : hostName) + ".ico";
				}
				catch ( Exception exp )
				{
					MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					faviconFile = null;
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}

			return faviconFile;
		}

		/// <summary>
		/// url.dll のアイコンファイル名を取得する
		/// </summary>
		/// <returns></returns>
		private string GetUrlDllIconFile()
		{
			string urlDllIconFile = iconsFolderPath + "\\" + ccMushroom.faviconUrlDllFileName;

			return (File.Exists(urlDllIconFile)) ? urlDllIconFile : null;
		}

		/// <summary>
		/// [削除] がチェックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkDelete_CheckedChanged(object sender, EventArgs e)
		{
			try
			{
				bool enable = !checkDelete.Checked;

				labelTabText.Enabled = enable;
				textTabText.Enabled = enable;
				comboTabName.Enabled = enable;
				checkAutoExec.Enabled = enable;
				labelButtonText.Enabled = enable;
				textButtonText.Enabled = enable;
				pictureIcon.Enabled = enable;
				buttonSelectBackColor.Enabled = enable;
				labelAppName.Enabled = enable;
				textAppName.Enabled = enable;
				buttonSelectApp.Enabled = enable;
				labelCommandLine.Enabled = enable;
				textCommandLine.Enabled = enable;
				labelWorkingDirectory.Enabled = enable;
				textWorkingDirectory.Enabled = enable;
				labelComment.Enabled = enable;
				textComment.Enabled = enable;
				textFaviconUrl.Enabled = enable;
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
			if ( comboBackColor.Visible )
			{
				comboBackColor.Visible = false;
			}

			OkProcess();
		}

		/// <summary>
		/// OK の処理
		/// </summary>
		private void OkProcess()
		{
			try
			{
				tabText = (_usage == usage.import) ? textTabText.Text : comboTabName.Text;
				if ( tabText.EndsWith(ccMushroom.newAppIndicator) )
				{
					tabText = tabText.Substring(0, tabText.Length - 1);
				}

				buttonText = textButtonText.Text;
				/*buttonText = buttonText.Replace('&', '＆');
				buttonText = buttonText.Replace('<', '＜');
				buttonText = buttonText.Replace('>', '＞');
				buttonText = buttonText.Replace('"', '”');
				buttonText = buttonText.Replace('\'', '’');*/

				appName = textAppName.Text;

				commandLine = textCommandLine.Text;

				workingDirectory = textWorkingDirectory.Text.Trim().TrimEnd('\\');

				comment = textComment.Text;

				iconFile = GetAppIconName();

				autoExec = checkAutoExec.Checked;

				newApp = checkNewApp.Checked;

				if ( StartsWithHttp() )
				{
					if ( checkDelete.Checked )
					{
						DeleteFaviconFile(iconFile, true);
					}
					else
					{
						// 実際の favicon.ico ファイルが存在する？
#if USE_TOOLTIP_AS_ICON_FILENAME
						string actualAppIconFile = (string)pictureIcon.Tag;
#else
						string actualAppIconFile = GetAppIconName(1);
#endif
						if ( (actualAppIconFile != null) && File.Exists(actualAppIconFile) )
						{
							File.Copy(actualAppIconFile, iconFile, true);	// ダウンロードした favicon.ico を正式ファイル名にコピーする
						}
					}
				}
				else
				{
					if ( (iconFile != null) && (Directory.GetParent(iconFile).Name == ccMushroom.iconsFolder) )
					{
						if ( checkDelete.Checked )
						{
							DeleteAppIconFile(iconFile);
						}
						else
						{
							// 実際の appicon.ico ファイルが存在する？
#if USE_TOOLTIP_AS_ICON_FILENAME
							string actualAppIconFile = (string)pictureIcon.Tag;
#else
							string actualAppIconFile = GetAppIconName(1);
#endif
							if ( (actualAppIconFile != null) && File.Exists(actualAppIconFile) )
							{
								iconFile = Path.GetDirectoryName(iconFile) + "\\" + "appicon@" + GetCorrectFileName(buttonText) + ".ico";
								File.Copy(actualAppIconFile, iconFile, true);	// ダウンロードした appicon.ico を正式ファイル名にコピーする
							}
							else
							{
								if ( currentButtonText != null )
								{
									string _currentButtonText = GetCorrectFileName(currentButtonText);
									if ( _currentButtonText != GetCorrectFileName(buttonText) )	// ボタン名が変更されただけ？
									{
										string _iconFile = Path.GetDirectoryName(iconFile) + "\\" + "appicon@" + GetCorrectFileName(buttonText) + ".ico";
										File.Move(iconFile, _iconFile);
										iconFile = _iconFile;
									}
								}
							}
						}
					}
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// [キャンセル] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonCancel_Click(object sender, EventArgs e)
		{
			if ( iconFile == null )		// 現在、個別のアイコン指定が無い？
				return;

			if ( Path.GetFileName(iconFile).StartsWith("favicon@") )	// それは favicon.ico である？
			{
				RestoreIconFromTempFile(tempFaviconFiles);
			}
			else if ( Path.GetFileName(iconFile).StartsWith("appicon@") )	// それは appicon.ico である？
			{
				RestoreIconFromTempFile(tempAppiconFiles);
			}
		}

		/// <summary>
		/// 待避していた一時ファイルを正式ファイルに書き戻す
		/// </summary>
		/// <param name="tempIconFileName"></param>
		private void RestoreIconFromTempFile(string[] tempIconFileName)
		{
			try
			{	
				// [アイコン指定を解除する] が実行された？
				if ( !File.Exists(iconFile) && File.Exists(tempIconFileName[(int)ic.temp]) )
				{
#if true
					if ( File.Exists(tempIconFileName[(int)ic.saved]) )
					{
						// 待避したアイコンを一時ファイルとする
						File.Copy(tempIconFileName[(int)ic.saved], tempIconFileName[(int)ic.temp], true);
					}
#endif
					// 一時ファイルを正式ファイルに書き戻す
					File.Copy(tempIconFileName[(int)ic.temp], iconFile, true);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 一時ファイルを削除する
		/// </summary>
		/// <param name="tabText"></param>
		private void CleanupTempFile(string[] tempIconFileName)
		{
			try
			{
				for ( int i = 0; i < tempIconFileName.Length; i++ )
				{
					if ( File.Exists(tempIconFileName[i]) )
					{
						File.Delete(tempIconFileName[i]);
					}
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
		private void ShortcutMngDlg_FormClosing(object sender, FormClosingEventArgs e)
		{
			CleanupTempFile(tempFaviconFiles);

			CleanupTempFile(tempAppiconFiles);

			DestroyAppIcon();
		}

		/// <summary>
		/// contextMenuPictIcon が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuPictIcon_Opening(object sender, CancelEventArgs e)
		{
			try
			{
				toolStripMenuRemoveIcon.Enabled = (GetAppIconName() != null);
				toolStripMenuReloadFavicon.Enabled = StartsWithHttp();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// http で始まっているか否か
		/// </summary>
		/// <returns></returns>
		private bool StartsWithHttp()
		{
			return (textAppName.Text.StartsWith("http") || textCommandLine.Text.StartsWith("http"));
		}

		/// <summary>
		/// [アイコン登録を解除する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuRemoveIcon_Click(object sender, EventArgs e)
		{
			try
			{
				string appIconFile = GetAppIconName();

				SetAppIconName(null, null, false);

				DestroyAppIcon();
				pictureIcon.Invalidate();

				if ( StartsWithHttp() )
				{
					DeleteFaviconFile(appIconFile, false);

					if ( !appIconFile.EndsWith(ccMushroom.faviconUrlDllFileName) )
					{
						string urlDllIconFile = GetUrlDllIconFile();
						if ( urlDllIconFile != null )
						{
							SetAppIconName(urlDllIconFile, null, true);
						}
						return;
					}
				}
				else
				{
					DeleteAppIconFile(appIconFile);
				}

				UpdateAppIcon(common.CheckEnvironmentVariable(textAppName.Text));
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// favicon.ico ファイルを削除する
		/// </summary>
		/// <param name="faviconFile"></param>
		/// <param name="rightAway"></param>
		private void DeleteFaviconFile(string faviconFile, bool rightAway)
		{
			try
			{
				if ( (_ccMushroom == null) || !isDownloadedIconFile(faviconFile)
					/*string.IsNullOrEmpty(faviconFile) ||
					(faviconFile.IndexOf(ccMushroom.iconsFolder + "\\favicon@") == -1) ||
					faviconFile.EndsWith(faviconUrlDllFileName)*/
																   )
					return;

				string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_ICON_FILE + "='" + faviconFile + "']";
				XmlNodeList applications = _ccMushroom.XmlCcConfiguration.DocumentElement.SelectNodes(xpath);
				if ( applications.Count != 0 )
				{
					// このアイコンファイルは他のアプリでも使用されているか、現在編集中の物ではない？
					if ( (1 < applications.Count) || (applications[0][ccMushroom.TAG_BUTTON_TEXT].InnerText != buttonText)/*(applications[0][ccMushroom.TAG_APP_NAME].InnerText != appName)*/ )
						return;
				}

				if ( rightAway )
				{
					File.Delete(faviconFile);
				}
				else
				{
					/*if ( File.Exists(tempFaviconFile) )
					{
						File.Delete(tempFaviconFile);
					}
					File.Move(faviconFile, tempFaviconFile);*/
					if ( File.Exists(faviconFile) )
					{
						File.Delete(tempFaviconFiles[(int)ic.temp]);
						File.Move(faviconFile, tempFaviconFiles[(int)ic.temp]);	// アイコンファイルを一時ファイルとしておく
					}
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// appicon.ico ファイルを削除する
		/// </summary>
		/// <param name="appIconFile"></param>
		private void DeleteAppIconFile(string appIconFile)
		{
			try
			{
				if ( Directory.GetParent(appIconFile).Name != ccMushroom.iconsFolder )
					return;

				if ( File.Exists(appIconFile) )
				{
					File.Delete(tempAppiconFiles[(int)ic.temp]);
					File.Move(appIconFile, tempAppiconFiles[(int)ic.temp]);	// アイコンファイルを一時ファイルとしておく
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ダウンロードされたアイコンか否か
		/// </summary>
		/// <param name="iconFile"></param>
		/// <returns></returns>
		private bool isDownloadedIconFile(string iconFile)
		{
			if ( string.IsNullOrEmpty(iconFile) )
				return false;

			if ( iconFile.IndexOf(ccMushroom.iconsFolder + "\\" + "favicon@") == -1 )
				return false;

			if ( iconFile.EndsWith(ccMushroom.faviconUrlDllFileName) )
				return false;

			return true;
		}

		/// <summary>
		/// [favicon.ico を再取得する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuReloadFavicon_Click(object sender, EventArgs e)
		{
			try
			{
				Uri uri;
				string hostName = null;

				if ( textFaviconUrl.Visible )
				{
					bool hostNameTerminate = textFaviconUrl.Text.EndsWith("/");
					uri = new Uri(textFaviconUrl.Text + (hostNameTerminate ? "favicon.ico" : ""));
					hostName = (hostNameTerminate) ? null : ((Uri)textFaviconUrl.Tag).Host;	// ホスト名止まりの場合、ファイルは指定されたホスト名にする。それ以外は通常通り、実際の url のホスト名にする
				}
				else
				{
					string url = textAppName.Text.StartsWith("http") ? textAppName.Text : (textCommandLine.Text.StartsWith("http") ? textCommandLine.Text : "");
					uri = new Uri(url);
					uri = new Uri(uri.Scheme + "://" + uri.Host + "/favicon.ico");

#if true
					if ( isDownloadedIconFile(iconFile) )
					{
						string iconHostName = Path.GetFileNameWithoutExtension(iconFile).Substring(8);	// 8:favicon@
						if ( uri.Host != iconHostName )	// 現在のアイコンは別ホストのアイコンが設定されている？
						{
							uri = new Uri(uri.Scheme + "://" + iconHostName + "/favicon.ico");
						}
					}
#endif

					if ( ccMushroom.textFaviconUrlVisible )
					{
						// favicon.ico の URL をテキストボックスで設定できるようにして、作業ディレクトリを非表示にする
						textFaviconUrl.Location = textWorkingDirectory.Location;
						textFaviconUrl.Size = textWorkingDirectory.Size;
						textFaviconUrl.TabIndex = textWorkingDirectory.TabIndex;
						textFaviconUrl.Visible = true;
						textFaviconUrl.Text = uri.ToString();
						textFaviconUrl.Tag = uri;

						labelWorkingDirectory.Text = "favicon.ico";
						textWorkingDirectory.Visible = false;
					}
				}

				string faviconFile = DownloadFavicon(uri, hostName);

				if ( faviconFile != null )	// ダウンロードは成功した？
				{
					SetAppIconName(faviconFile, tempFaviconFiles[(int)ic.temp], true);
					return;
				}

				if ( (faviconFile = GetUrlDllIconFile()) != null )
				{
					SetAppIconName(faviconFile, null, true);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void comboBackColor_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void comboBackColor_SelectionChangeCommitted(object sender, EventArgs e)
		{
			Debug.WriteLine(comboBackColor.SelectedItem);
		}
	}
}
