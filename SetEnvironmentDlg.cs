using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using CommonFunctions;

namespace ccMushroom
{
	public partial class SetEnvironmentDlg : Form
	{
		private ccMushroom _ccMushroom = null;

		private string THIS_FORM_TEXT = "環境設定 ダイアログ";

		private string TAB_PAGE_EXE_CONFIG = "動作設定";
		private string TAB_PAGE_IMPORT = "ショートカット";
		private string TAB_PAGE_SCAN = "リモート走査";
		private string TAB_PAGE_APP_ENVIRON = "アプリ環境";

		// ccMushroom.exe.config
		private bool modifiedCcMushroomExeConfig = false;
		private Color buttonTextColor;
		private Color buttonBackColor;

		// ccMushroom.ini
		private string ccMushroomIniFileName = ccMushroom.ccMushroomIniFileName;
		private int currentProgramsFolder;			// 現在のリモート/ローカル プログラム フォルダ数
		private string[] remoteProgramsNames;		// 環境設定用のリモート側のプログラム名称
		private string[] remoteProgramsFolders;		// 環境設定用のリモート側のプログラムを置いているフォルダ名
		private string[] localProgramsFolders;		// 環境設定用のローカル側のプログラムをコピーするフォルダ名
		private bool[] enabledProgramsFolders;		// 環境設定用の有効/無効なプログラムフォルダの設定
		private bool modifiedCcMushroomIni = false;

		private enum fold
		{
			name, remote, local
		}

		// ccConfiguration.import.xml
		private string fileCcConfigurationImport = Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_IMPORT_FILE_NAME;
		private XmlDocument xmlCcConfigurationImport = null;
		private bool modifiedCcConfigurationImportXml = false;
		private Icon appIcon = null;
		private string defTabText;

		private enum imp
		{
			tab, button, app, arg, dir
		}

		// ccAppEnvironmentSetting.xml
		private string fileCcAppEnvironmentSetting = Application.StartupPath + "\\" + ccMushroom.CC_APP_ENVIRONMENT_SETTING_FILE_NAME;
		private XmlDocument xmlCcAppEnvironmentSetting = null;
		private string comboAppEnvironText = null;
		private bool modifiedCcAppEnvironmentSettingXml = false;
		private ToolStripMenuItem[] contextMenuTabPages = null;
		private ToolStripMenuItem[] contextMenuButtons = null;
		private bool nowSettingAeText = false;
		private string editCell = null;

		private enum appset
		{
			fname, environ, cfile
		}

		private bool nowLoading = true;

		private string sampleButtonText;
		private int maxSampleButtonWidth;

		[System.Runtime.InteropServices.DllImport("ccMushShellExt.dll")]
		public static extern int DllRegisterServer();

		[System.Runtime.InteropServices.DllImport("ccMushShellExt.dll")]
		public static extern int DllUnregisterServer();

		/// <summary>
		/// SetEnvironmentDlg コンストラクタ
		/// </summary>
		public SetEnvironmentDlg()
		{
			InitializeComponent();

			// ccMushroom.ini
			currentProgramsFolder = ccMushroom.remoteProgramsFolders.Length;
			//remoteProgramsNames = ccMushroom.remoteProgramsNames;
			remoteProgramsNames = new string[currentProgramsFolder];
			Array.Copy(ccMushroom.remoteProgramsNames, remoteProgramsNames, currentProgramsFolder);
			//remoteProgramsFolders = ccMushroom.remoteProgramsFolders;
			remoteProgramsFolders = new string[currentProgramsFolder];
			Array.Copy(ccMushroom.remoteProgramsFolders, remoteProgramsFolders, currentProgramsFolder);
			//localProgramsFolders = ccMushroom.localProgramsFolders;
			localProgramsFolders = new string[currentProgramsFolder];
			Array.Copy(ccMushroom.localProgramsFolders, localProgramsFolders, currentProgramsFolder);
			//enabledProgramsFolders = ccMushroom.enabledProgramsFolders;	// こうすると参照（アドレス渡し）になる？
			enabledProgramsFolders = new bool[currentProgramsFolder];
			Array.Copy(ccMushroom.enabledProgramsFolders, enabledProgramsFolders, currentProgramsFolder);

			if ( !Program.expertMode )
			{
				tabControl.TabPages.Remove(tabPageExpertSettings);
			}

#if !FOR_WINDOWS7
			checkEnabledTaskbarThumbnail.Visible = false;
#endif
		}

		/// <summary>
		/// SetEnvironmentDlg_Load
		/// </summary>
		private void SetEnvironmentDlg_Load(object sender, System.EventArgs e)
		{
			try
			{
				_ccMushroom = (ccMushroom)this.Owner;

				this.Text = THIS_FORM_TEXT;

				// タブ名
				tabPageExeConfig.Text = TAB_PAGE_EXE_CONFIG;
				tabPageImport.Text = TAB_PAGE_IMPORT;
				tabPageScan.Text = TAB_PAGE_SCAN;
				tabPageAppEnviron.Text = TAB_PAGE_APP_ENVIRON;

				tabPageExeConfig.ToolTipText = Application.ProductName + ".exe.config";
				tabPageScan.ToolTipText = ccMushroom.CC_MUSHROOM_INI_FILE_NAME;
				tabPageImport.ToolTipText = ccMushroom.CC_CONFIGURATION_IMPORT_FILE_NAME;
				tabPageAppEnviron.ToolTipText = ccMushroom.CC_APP_ENVIRONMENT_SETTING_FILE_NAME;
				tabPageExpertSettings.ToolTipText = ccMushroom.SETTINGS_SECTION + "@" + ccMushroom.CC_MUSHROOM_INI_FILE_NAME;

				/*// Windows98 ではタブの tooltip が文字化けする
				if ( System.Environment.OSVersion.Platform == PlatformID.Win32Windows )
				{
					this.tabPageCommon.ToolTipText = "ccMushroom.exe.config";
					this.tabPagePrivate.ToolTipText = "ccMushroom.ini";
					this.tabPageImport.ToolTipText = "ccConfiguration.import.xml";
				}*/

				// リストビューの縦幅を調節する
				if ( (common.GetOsPlatform() & common.platform.winxp) != 0 )
				{
					//listViewProgramsFolder.Height = 123;
					//listViewApplication.Height = 125;
				}
				else
				{
					listViewProgramsFolder.Height -= 4;
					listViewApplication.Height -= 4;
				}

				// NumericUpDown 内のすべてのコントロールに ToolTip を設定する
				foreach ( Control c in numericButtonWidth.Controls )
					toolTip.SetToolTip(c, toolTip.GetToolTip(numericButtonWidth));
				foreach ( Control c in numericButtonHeight.Controls )
					toolTip.SetToolTip(c, toolTip.GetToolTip(numericButtonHeight));

				// ccMushroom.exe.config
				textLatestProgramFolder.Text = ccMushroom.latestProgramFolder;
				textRemoteProgramsFolder.Text = remoteProgramsFolders[0];
				textLocalProgramsFolder.Text = localProgramsFolders[0];
				checkScanRemoteProgramsFolder.Checked = ccMushroom.scanRemoteProgramsFolder;
				checkRemoveNoUseLocalProgramsFolder.Checked = ccMushroom.removeNoUseLocalProgramsFolder;
				checkAlwaysInTasktray.Checked = ccMushroom.alwaysInTasktray;
				checkMultiLineTab.Checked = ccMushroom.multiLineTab;
				checkImportTabAppearFirst.Checked = ccMushroom.importTabAppearFirst;
				textHideTabPage.Text = (ccMushroom.hideTabPageText == null) ? "" : string.Join(",", ccMushroom.hideTabPageText);
				textAutoWindowCloseTime.Text = (ccMushroom.autoWindowCloseTime / 60 / 1000).ToString();
				textShortcutMngApplyTime.Text = ccMushroom.shortcutMngApplyTime.ToString();
				textAutoOpacityTime.Text = ccMushroom.autoOpacityTimePercent.Split(',')[0];
				textAutoOpacityPercent.Text = ccMushroom.autoOpacityTimePercent.Split(',')[1];
				checkTransparentWithClickThrough.Checked = ccMushroom.transparentWithClickThrough;
				textTextEditor.Text = (ccMushroom.textEditor == null) ? "" : ccMushroom.textEditor;
				checkMovableButton.Checked = ccMushroom.movableButton;
				checkIntegrateExplorerContext.Checked = ccMushroom.integrateExplorerContext;
				radioAppIconLarge.Checked = !ccMushroom.smallApplicationIcon;
				radioAppIconSmall.Checked = ccMushroom.smallApplicationIcon;
				numericButtonWidth.Value = ccMushroom.buttonSize.Width;
				numericButtonHeight.Value = ccMushroom.buttonSize.Height;
				buttonTextColor = ccMushroom.buttonTextColor;
				buttonBackColor = ccMushroom.buttonBackColor;

#if ENABLE_TAB_BACKGROUND
				if ( checkForSelectedTab.Enabled )
				{
					toolTip.SetToolTip(checkForSelectedTab, "「" + ccMushroom.selectedTabPageText + "」タブ用の設定");
				}
#else
				checkForSelectedTab.Enabled = false;
#endif

				sampleButtonText = labelSampleButton.Text;
				maxSampleButtonWidth = labelSampleButton.Width;

				labelSampleButton.Text = string.Empty;
				labelSampleButton.Font = ccMushroom.buttonFont;
				labelSampleButton.Width = Math.Min(Decimal.ToInt32(numericButtonWidth.Value), labelSampleButton.Width);
				labelSampleButton.Height = Decimal.ToInt32(numericButtonHeight.Value);
				labelSampleButton.BackColor = buttonBackColor;
				toolTip.SetToolTip(labelSampleButton, Program.GetFontInfo(ccMushroom.buttonFont, ccMushroom.buttonTextColor));

				textLatestProgramFolder.ReadOnly = true;
				textRemoteProgramsFolder.ReadOnly = true;
				textLocalProgramsFolder.ReadOnly = true;
				checkRemoveNoUseLocalProgramsFolder.Enabled = ccMushroom.enableRemoveNoUseLocalProgramsFolder;

				string caption = Program.CONSET_LATEST_PROGRAM_FOLDER + ": " + ccMushroom.latestProgramFolder + "\n" +
								 Program.CONSET_REMOTE_PROGRAMS_FOLDER + ": " + remoteProgramsFolders[0] + "\n" +
								 Program.CONSET_LOCAL_PROGRAMS_FOLDER + ": " + localProgramsFolders[0] + "\n" +
								 Program.CONSET_ENABLED_PROGRAMS_FOLDER + ": " + enabledProgramsFolders[0].ToString().ToLower();
								 
				toolTip.SetToolTip(labelAppConfig, caption);

				ccf.hotKeys hotKeys;
				ccf.ParseHotKey(ccMushroom.hotKey, out hotKeys);
				checkHkAlt.Checked = hotKeys.alt;
				checkHkCtrl.Checked = hotKeys.ctrl;
				checkHkShift.Checked = hotKeys.shift;
				checkHkWin.Checked = hotKeys.win;
				textHkCode.Text = hotKeys.GetKeyChar();

				string[] ccMushShellExtDll = {Application.StartupPath + "\\" + Program._ccMushShellExtFile.Substring(1), Application.StartupPath + "\\" + Program._ccMushShellExtFile};
				for ( int i = 0; i < ccMushShellExtDll.Length; i++ )
				{
					if ( !File.Exists(ccMushShellExtDll[i]) )
						continue;
					Version verCcMushShellExt = new Version(FileVersionInfo.GetVersionInfo(ccMushShellExtDll[i]).FileVersion);
					toolTip.SetToolTip(checkIntegrateExplorerContext, toolTip.GetToolTip(checkIntegrateExplorerContext) + "\r\n" + Path.GetFileName(ccMushShellExtDll[i]) + "  ver: " + verCcMushShellExt);
					break;
				}

				// ccConfiguration.import.xml
				radioAppUp.Enabled = false;
				radioAppDown.Enabled = false;
				buttonAppMove.Enabled = false;

				xmlCcConfigurationImport = new XmlDocument();
				xmlCcConfigurationImport.Load(fileCcConfigurationImport);

				XmlNode nodeDefTabText = xmlCcConfigurationImport.DocumentElement[ccMushroom.TAG_DEFAULT_TAB_TEXT];
				toolTip.SetToolTip(labelImportXml, ccMushroom.TAG_DEFAULT_TAB_TEXT + ": " + ((nodeDefTabText == null) ? "" : nodeDefTabText.InnerText));
				defTabText = _ccMushroom.DefaultTabText/*(nodeDefTabText == null) ? "お気に入り" : nodeDefTabText.InnerText*/;

				ShowApplicationList();

				// ccMushroom.ini
				radioFoldUp.Enabled = false;
				radioFoldDown.Enabled = false;
				buttonFoldMove.Enabled = false;

				ShowProgramsFolderList();

				//listViewProgramsFolder.Enabled = false;

				// ccAppEnvironmentSetting.xml
				xmlCcAppEnvironmentSetting = new XmlDocument();
				xmlCcAppEnvironmentSetting.Load(fileCcAppEnvironmentSetting);

				ShowAppEnvironmentSetting();

				radioAeEdit.Enabled = false;
				radioAeCopy.Enabled = false;

				string fileCcLatestAppEnvironment = Application.StartupPath + "\\" + ccMushroom.CC_LATEST_APP_ENVIRONMENT_FILE_NAME;
				checkAeDelCcLatestAppEnviron.Enabled = File.Exists(fileCcLatestAppEnvironment);

				// Settings@ccMushroom.ini
				ReadExpertSettings();

				// バージョン情報
				Version ccVer = Assembly.GetExecutingAssembly().GetName().Version;
				DateTime buildDateTime = new DateTime(2000, 1, 1);
				TimeSpan verSpan = new TimeSpan(ccVer.Build * TimeSpan.TicksPerDay + ccVer.Revision * 2 * TimeSpan.TicksPerSecond);
				buildDateTime += verSpan;
				labelAppTitle.Text = Application.ProductName;
				toolTip.SetToolTip(labelAppTitle, ccMushroom.appTitle);
				labelVersion.Text = "version " + ccVer.Major + "." + ccVer.Minor.ToString("D2") + "." + ccVer.Build;
				toolTip.SetToolTip(labelVersion, "Builded: " + buildDateTime.ToString("yyyy/MM/dd HH:mm"));

				// その他の初期化を行う
				tabControl.SelectedTab = tabPageImport;
				tabControl.SelectedTab = tabPageScan;	// ここで一旦タブを切り替えておかないと、フォームのロード後に listViewProgramsFolder_ItemCheck イベントが発生してしまう
				//Application.DoEvents ();
				tabControl.SelectedTab = tabPageExeConfig;

				nowLoading = false;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// SetEnvironmentDlg_Shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SetEnvironmentDlg_Shown(object sender, EventArgs e)
		{
			try
			{
				//textLatestProgramFolder.Select();
				//textLatestProgramFolder.Select(0, 0);
				tabControl.SelectedTab.Select();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// tabControl_SelectedIndexChanged
		/// </summary>
		private void tabControl_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				TabControl tabControl = (TabControl)sender;
				if ( tabControl.SelectedTab.Text == TAB_PAGE_EXE_CONFIG )
					this.AcceptButton = buttonOk;
				else if ( tabControl.SelectedTab.Text == TAB_PAGE_IMPORT )
					this.AcceptButton = buttonOk/*buttonAppRegist*/;
				else if ( tabControl.SelectedTab.Text == TAB_PAGE_SCAN )
					this.AcceptButton = buttonOk/*buttonFoldRegist*/;
				else if ( tabControl.SelectedTab.Text == TAB_PAGE_APP_ENVIRON )
					this.AcceptButton = buttonOk;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// [Ok] ボタンが押された
		/// </summary>
		private void buttonOk_Click(object sender, System.EventArgs e)
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;
				int i;

				// ccMushroom.exe.config
				if ( modifiedCcMushroomExeConfig )
				{
					File.Copy(Application.StartupPath + "\\" + Application.ProductName + ".exe.config", Application.StartupPath + "\\" + Application.ProductName + ".exe.config.bak", true);

					ccMushroom.scanRemoteProgramsFolder = checkScanRemoteProgramsFolder.Checked;
					ccMushroom.removeNoUseLocalProgramsFolder = checkRemoveNoUseLocalProgramsFolder.Checked;
					ccMushroom.alwaysInTasktray = checkAlwaysInTasktray.Checked;
					ccMushroom.multiLineTab = checkMultiLineTab.Checked;
					ccMushroom.importTabAppearFirst = checkImportTabAppearFirst.Checked;
					ccMushroom.hideTabPageText = (textHideTabPage.Text.Length == 0) ? null : textHideTabPage.Text.Split(',');
					ccMushroom.autoWindowCloseTime = Int32.Parse(textAutoWindowCloseTime.Text) * 60 * 1000;
					ccMushroom.shortcutMngApplyTime = textShortcutMngApplyTime.Text;
					ccMushroom.autoOpacityTimePercent = textAutoOpacityTime.Text + "," + textAutoOpacityPercent.Text;
					ccMushroom.transparentWithClickThrough = checkTransparentWithClickThrough.Checked;
					ccMushroom.textEditor = (textTextEditor.Text.Length == 0) ? null : textTextEditor.Text;
					ccMushroom.movableButton = checkMovableButton.Checked;
					//ccMushroom.integrateExplorerContext = checkIntegrateExplorerContext.Checked;
					if ( !checkForSelectedTab.Checked )	// 全てのタブ用の設定？
					{
						ccMushroom.smallApplicationIcon = radioAppIconSmall.Checked;
						ccMushroom.buttonSize = new Size(Decimal.ToInt32(numericButtonWidth.Value), Decimal.ToInt32(numericButtonHeight.Value));
						ccMushroom.buttonFont = Program.CreateFont(toolTip.GetToolTip(labelSampleButton), out ccMushroom.buttonTextColor);
						ccMushroom.buttonBackColor = buttonBackColor;
					}
					ccMushroom.hotKey = (checkHkAlt.Checked ? "Alt+" : "") + (checkHkCtrl.Checked ? "Ctrl+" : "") + (checkHkShift.Checked ? "Shift+" : "") + (checkHkWin.Checked ? "Win+" : "") + textHkCode.Text;

					string appName = Process.GetCurrentProcess().ProcessName;
					AppConfig appConfig = new AppConfig(appName);
					appConfig.SetValue(Program.CONSET_SCAN_REMOTE_PROGRAMS_FOLDER, ccMushroom.scanRemoteProgramsFolder.ToString().ToLower());
					appConfig.SetValue(Program.CONSET_REMOVE_NOUSE_LOCAL_PROGRAMS_FOLDER, ccMushroom.removeNoUseLocalProgramsFolder.ToString().ToLower());
					appConfig.SetValue(Program.CONSET_ALWAYS_IN_TASKTRAY, ccMushroom.alwaysInTasktray.ToString().ToLower());
					appConfig.SetValue(Program.CONSET_MULTI_LINE_TAB, ccMushroom.multiLineTab.ToString().ToLower());
					appConfig.SetValue(Program.CONSET_IMPORT_TAB_APPEAR_FIRST, ccMushroom.importTabAppearFirst.ToString().ToLower());
					appConfig.SetValue(Program.CONSET_HIDE_TAB_PAGE_TEXT, textHideTabPage.Text);
					appConfig.SetValue(Program.CONSET_AUTO_WINDOW_CLOSE_TIME, textAutoWindowCloseTime.Text);
					appConfig.SetValue(Program.CONSET_SHORTCUT_MNG_APPLY_TIME, ccMushroom.shortcutMngApplyTime);
					appConfig.SetValue(Program.CONSET_AUTO_OPACITY_TIME_PERCENT, ccMushroom.autoOpacityTimePercent);
					appConfig.SetValue(Program.CONSET_TRANSPARENT_WITH_CLICKTHROUGH, ccMushroom.transparentWithClickThrough.ToString().ToLower());
					appConfig.SetValue(Program.CONSET_TEXT_EDITOR, textTextEditor.Text);
					appConfig.SetValue(Program.CONSET_MOVABLE_BUTTON, ccMushroom.movableButton.ToString().ToLower());
					//appConfig.SetValue(Program.CONSET_INTEGRATE_EXPLORER_CONTEXT, ccMushroom.integrateExplorerContext.ToString().ToLower());
					if ( !checkForSelectedTab.Checked )	// 全てのタブ用の設定？
					{
						appConfig.SetValue(Program.CONSET_SMALL_APPLICATION_ICON, ccMushroom.smallApplicationIcon.ToString().ToLower());
						appConfig.SetValue(Program.CONSET_BUTTON_SIZE, ccMushroom.buttonSize.Width + "," + ccMushroom.buttonSize.Height);
						appConfig.SetValue(Program.CONSET_BUTTON_FONT, Program.GetFontInfo(ccMushroom.buttonFont, ccMushroom.buttonTextColor));
						appConfig.SetValue(Program.CONSET_BUTTON_BACK_COLOR, ccMushroom.buttonBackColor.ToArgb().ToString("x"));
					}
					appConfig.SetValue(Program.CONSET_HOT_KEY, ccMushroom.hotKey);

#if ENABLE_TAB_BACKGROUND
					if ( checkForSelectedTab.Checked )	// 現在のタブ用の設定？
					{
						Size _buttonSize = new Size(Decimal.ToInt32(numericButtonWidth.Value), Decimal.ToInt32(numericButtonHeight.Value));

						string lpString = null;
						if ( _buttonSize.Width != ccMushroom.buttonSize.Width || _buttonSize.Height != ccMushroom.buttonSize.Height )
						{
							lpString = _buttonSize.Width + "," + _buttonSize.Height;
						}
						api.WritePrivateProfileString(ccMushroom.selectedTabPageText, ccMushroom.KEY_BUTTON_SIZE, lpString, ccMushroom.tabPageSettingsIniFileName);

						lpString = null;
						if ( buttonTextColor != ccMushroom.buttonTextColor )
						{
							lpString = buttonTextColor.ToArgb().ToString("x");
						}
						api.WritePrivateProfileString(ccMushroom.selectedTabPageText, ccMushroom.KEY_BUTTON_TEXT_COLOR, lpString, ccMushroom.tabPageSettingsIniFileName);
					}
#endif

					RegCcMushShellExt();
				}

				// ccConfiguration.import.xml
				if ( modifiedCcConfigurationImportXml )
				{
					File.Copy(fileCcConfigurationImport, fileCcConfigurationImport + ".bak", true);

					xmlCcConfigurationImport.Save(fileCcConfigurationImport);
				}

				// ccMushroom.ini
				if ( modifiedCcMushroomIni )
				{
					File.Copy(ccMushroomIniFileName, ccMushroomIniFileName + ".bak", true);

					//ccMushroom.remoteProgramsNames = remoteProgramsNames;
					ccMushroom.remoteProgramsNames = new string[remoteProgramsFolders.Length];
					Array.Copy(remoteProgramsNames, ccMushroom.remoteProgramsNames, ccMushroom.remoteProgramsNames.Length);
					//ccMushroom.remoteProgramsFolders = remoteProgramsFolders;
					ccMushroom.remoteProgramsFolders = new string[remoteProgramsFolders.Length];
					Array.Copy(remoteProgramsFolders, ccMushroom.remoteProgramsFolders, ccMushroom.remoteProgramsFolders.Length);
					//CcMushroom.localProgramsFolders = localProgramsFolders;
					ccMushroom.localProgramsFolders = new string[localProgramsFolders.Length];
					Array.Copy(localProgramsFolders, ccMushroom.localProgramsFolders, ccMushroom.localProgramsFolders.Length);
					//ccMushroom.enabledProgramsFolders = enabledProgramsFolders;
					ccMushroom.enabledProgramsFolders = new bool[enabledProgramsFolders.Length];
					Array.Copy(enabledProgramsFolders, ccMushroom.enabledProgramsFolders, ccMushroom.enabledProgramsFolders.Length);

					for ( i = 1; i < ccMushroom.remoteProgramsFolders.Length; i++ )
					{
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_REMOTE_PROGRAMS_NAME + i, ccMushroom.remoteProgramsNames[i], ccMushroomIniFileName);
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_REMOTE_PROGRAMS_FOLDER + i, ccMushroom.remoteProgramsFolders[i], ccMushroomIniFileName);
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_LOCAL_PROGRAMS_FOLDER + i, ccMushroom.localProgramsFolders[i], ccMushroomIniFileName);
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_ENABLED_PROGRAMS_FOLDER + i, ccMushroom.enabledProgramsFolders[i].ToString(), ccMushroomIniFileName);
					}
					for ( ; i < currentProgramsFolder; i++ )	// 余った設定を削除する
					{
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_REMOTE_PROGRAMS_NAME + i, null, ccMushroomIniFileName);
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_REMOTE_PROGRAMS_FOLDER + i, null, ccMushroomIniFileName);
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_LOCAL_PROGRAMS_FOLDER + i, null, ccMushroomIniFileName);
						api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_ENABLED_PROGRAMS_FOLDER + i, null, ccMushroomIniFileName);
					}

					for ( i = 1; i < ccMushroom.localProgramsFolders.Length; i++ )
					{
						ccMushroom.localProgramsFolders[i] = ccf.ReplaceCcMushroomLocalFolder(localProgramsFolders[i], true);
					}
				}

				// ccAppEnvironmentSetting.xml
				if ( modifiedCcAppEnvironmentSettingXml )
				{
					File.Copy(fileCcAppEnvironmentSetting, fileCcAppEnvironmentSetting + ".bak", true);

					// comboAppEnviron の内容を反映させる
					XmlNode appEnviron = xmlCcAppEnvironmentSetting.DocumentElement[ccMushroom.TAG_APP_ENVIRON];
					XmlNode _appEnviron = null;	// 現在の appEnviron のクローンを作成しておく
					if ( appEnviron == null )
					{
						appEnviron = xmlCcAppEnvironmentSetting.CreateNode(XmlNodeType.Element, ccMushroom.TAG_APP_ENVIRON, null);
						xmlCcAppEnvironmentSetting.DocumentElement.AppendChild(appEnviron);
					}
					else
					{
						_appEnviron = appEnviron.CloneNode(true);
					}
					appEnviron.RemoveAll();

					foreach ( string text in comboAppEnviron.Items )
					{
						if ( text.Length == 0 )
							continue;

						XmlElement elem = xmlCcAppEnvironmentSetting.CreateElement(ccMushroom.TAG_ITEM);
						elem.InnerText = text;
						appEnviron.AppendChild(elem);

						if ( _appEnviron != null )
						{
							string xpath = ccMushroom.TAG_ITEM + "[.='" + text + "']";
							XmlNode _item = _appEnviron.SelectSingleNode(xpath);
							if ( _item != null && _item.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR] != null )
							{
								elem.Attributes.Append(_item.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR]);
							}
						}
					}

					xmlCcAppEnvironmentSetting.Save(fileCcAppEnvironmentSetting);

					if ( checkAeDelCcLatestAppEnviron.Checked )
					{
						try
						{
							string fileCcLatestAppEnvironment = Application.StartupPath + "\\" + ccMushroom.CC_LATEST_APP_ENVIRONMENT_FILE_NAME;
							File.Delete(fileCcLatestAppEnvironment);
						}
						catch ( Exception exp )
						{
							Debug.WriteLine(exp.Message);
						}
					}
				}

				// Settings@ccMushroom.ini
				SaveExpertSettings();

				Cursor.Current = Cursors.Default;
				this.Close();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushShellExt.dll を登録/解除する
		/// </summary>
		private void RegCcMushShellExt()
		{
			StringBuilder shellExtLog = new StringBuilder(1024);
			shellExtLog.Append("\r\n\r\n" + "==== " + MethodBase.GetCurrentMethod().Name + " ====\r\n");

			try
			{
				bool sameStatus;
				if ( (sameStatus = (ccMushroom.integrateExplorerContext == checkIntegrateExplorerContext.Checked)) )
				{
					shellExtLog.Append("(ccMushroom.integrateExplorerContext == checkIntegrateExplorerContext.Checked)" + "\r\n");
				}

				string ccMushShellExtFile = Program._ccMushShellExtFile.Substring(1);
				string curCcMushShellExt = Application.StartupPath + "\\" + ccMushShellExtFile;
				shellExtLog.Append("curCcMushShellExt: " + curCcMushShellExt + "\r\n");
				string tmpCcMushShellExt = Application.StartupPath + "\\" + "~" + ccMushShellExtFile;
				shellExtLog.Append("tmpCcMushShellExt: " + tmpCcMushShellExt + "\r\n");

				string appID = null;
				int hresult = -1;

				//ccf.UpdateFile(null, null, tmpCcMushShellExt, ref shellExtLog);

				string subKeyName = @"AppID\" + ccMushShellExtFile;
				try
				{
					Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(subKeyName);
					appID = (string)regKey.GetValue("AppID");
					regKey.Close();
					shellExtLog.Append("ClassesRoot.OpenSubKey(" + subKeyName + ")\r\n");
					shellExtLog.Append("AppID: " + appID + "\r\n");
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message + " (" + subKeyName + ")");
					shellExtLog.Append(exp.Message + " (" + subKeyName + ")" + "\r\n");
				}

				string errmes = null;

				// ccMushShellExt.dll を登録する
				if ( checkIntegrateExplorerContext.Checked )
				{
					shellExtLog.Append("checkIntegrateExplorerContext.Checked\r\n");

					// C:\Documents and Settings\(UserName)\Local Settings\Application Data\ICSG\ccMushroom\ccMushroom.ini
					string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					string localAppCcMushPath = localAppDataPath + Program.CCMUSHROOM_APP_DATA;
					if ( !Directory.Exists(localAppCcMushPath) )
					{
						Directory.CreateDirectory(localAppCcMushPath);
					}
					string ccMushroomIniFileName = localAppCcMushPath + "\\" + ccMushroom.CC_MUSHROOM_INI_FILE_NAME;
					shellExtLog.Append(ccMushroomIniFileName + "\r\n");
					// CcMushStartupPath
					api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, Program.KEY_CCMUSH_STARTUP_PATH, Application.StartupPath, ccMushroomIniFileName);
					shellExtLog.Append("[" + ccMushroom.SETTINGS_SECTION + "] " + Program.KEY_CCMUSH_STARTUP_PATH + " = " + Application.StartupPath + "\r\n");
					// CcMushAppTitle
					api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, Program.KEY_CCMUSH_APP_TITLE, ccMushroom.appTitle, ccMushroomIniFileName);
					shellExtLog.Append("[" + ccMushroom.SETTINGS_SECTION + "] " + Program.KEY_CCMUSH_APP_TITLE + " = " + ccMushroom.appTitle + "\r\n");

					string ccMushCmdLineArgs = null;
					string[] cmdLineArgs = Environment.GetCommandLineArgs();
					for ( int i = 1; i < cmdLineArgs.Length; i++ )
					{
						if ( cmdLineArgs[i].StartsWith(Program.CMDPARAM_APP_TITLE) || cmdLineArgs[i].StartsWith(Program.CMDPARAM_DELAY_TIME) || cmdLineArgs[i].StartsWith(ProgUpdateClass.update.CMDPARAM_SHOW_PROG_UPDATE_MESSAGE) )
							continue;
						ccMushCmdLineArgs += (" " + cmdLineArgs[i]);
					}
					// CcMushCmdLineArgs
					api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, Program.KEY_CCMUSH_CMDLINE_ARGS, ccMushCmdLineArgs ?? null, ccMushroomIniFileName);
					shellExtLog.Append("[" + ccMushroom.SETTINGS_SECTION + "] " + Program.KEY_CCMUSH_CMDLINE_ARGS + " =" + (ccMushCmdLineArgs ?? null) + "\r\n");

					if ( sameStatus )
						return;

					string orgCcMushShellExt = Application.StartupPath + "\\" + Program._ccMushShellExtFile;
					shellExtLog.Append("orgCcMushShellExt: " + orgCcMushShellExt + "\r\n");

					Version verOrgCcMushShellExt = new Version(FileVersionInfo.GetVersionInfo(orgCcMushShellExt).FileVersion);
					Version verCurCcMushShellExt = new Version("0.0.0.0");
					if ( File.Exists(curCcMushShellExt) )
					{
						verCurCcMushShellExt = new Version(FileVersionInfo.GetVersionInfo(curCcMushShellExt).FileVersion);
					}
					shellExtLog.Append("verOrgCcMushShellExt: " + verOrgCcMushShellExt + "\r\n");
					shellExtLog.Append("verCurCcMushShellExt: " + verCurCcMushShellExt + "\r\n");

					if ( (verCurCcMushShellExt < verOrgCcMushShellExt) || Program.debMode )	// 新しい _ccMushShellExt.dll ？
					{
						ccf.UpdateFile(orgCcMushShellExt, curCcMushShellExt, tmpCcMushShellExt, ref shellExtLog);
					}

					if ( (hresult = DllRegisterServer()) != 0 )
					{
						errmes = "DllRegisterServer@ccMushShellExt.dll が失敗しました";
					}
				}
				// ccMushShellExt.dll の登録を解除する
				else
				{
					shellExtLog.Append("!checkIntegrateExplorerContext.Checked\r\n");

					if ( sameStatus )
						return;

					if ( File.Exists(curCcMushShellExt) )
					{
						shellExtLog.Append("File.Exists(curCcMushShellExt)\r\n");
						if ( appID != null )
						{
							if ( (hresult = DllUnregisterServer()) != 0 )
							{
								errmes = "DllUnregisterServer@ccMushShellExt.dll が失敗しました";
							}
						}

						ccf.UpdateFile(null, curCcMushShellExt, tmpCcMushShellExt, ref shellExtLog);
					}
				}

				string _HRESULT = "HRESULT:" + "0x" + hresult.ToString("X08");

				Debug.WriteLine(_HRESULT);
				shellExtLog.Append(_HRESULT + "\r\n");

				if ( errmes != null )
				{
					MessageBox.Show(errmes + "\r\n" + _HRESULT, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				ccMushroom.integrateExplorerContext = checkIntegrateExplorerContext.Checked;

				string appName = Process.GetCurrentProcess().ProcessName;
				AppConfig appConfig = new AppConfig(appName);
				appConfig.SetValue(Program.CONSET_INTEGRATE_EXPLORER_CONTEXT, ccMushroom.integrateExplorerContext.ToString().ToLower());
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				shellExtLog.Append(exp.Message + "\r\n");
			}
			finally
			{
				try
				{
					string fileCcConfigurationLog = Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_LOG_FILE_NAME;

					using ( StreamWriter sw = new StreamWriter(fileCcConfigurationLog, true) )
					{
						sw.Write(shellExtLog.ToString());
						sw.Close();
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
			}
		}

		/// <summary>
		/// フォームが閉じられようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SetEnvironmentDlg_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if ( appIcon != null )
				{
					api.DestroyIcon(appIcon.Handle);
					appIcon = null;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		#region ccMushroom.exe.config
		/// <summary>
		/// ccMushroom.exe.config
		/// textExeConfig_TextChanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExeConfig_TextChanged(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			modifiedCcMushroomExeConfig = true;

			TextBox textBox = (TextBox)sender;
			if ( (textBox.Name != textAutoWindowCloseTime.Name) && (textBox.Name != textShortcutMngApplyTime.Name) &&
				 (!textBox.Name.StartsWith("textAutoOpacity")) && (textBox.Name != textTextEditor.Name) &&
				 (textBox.Name != textHkCode.Name) )
			{
				checkReload.Checked = true;
			}
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// checkExeConfig_CheckedChanged
		/// </summary>
		private void checkExeConfig_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			modifiedCcMushroomExeConfig = true;

			CheckBox checkBox = (CheckBox)sender;
			if ( (checkBox.Name != checkRemoveNoUseLocalProgramsFolder.Name) && (checkBox.Name != checkAlwaysInTasktray.Name) &&
				 (checkBox.Name != checkMovableButton.Name) && (!checkBox.Name.StartsWith("checkHk")) &&
				 (checkBox.Name != checkTransparentWithClickThrough.Name) )
			{
				checkReload.Checked = true;
			}
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// checkIntegrateExplorerContext_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkIntegrateExplorerContext_Click(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			try
			{
				string vcredist_x86 = "{402ED4A1-8F5B-387A-8688-997ABF58B8F2}";	// Microsoft Visual C++ 2008 Redistributable Setup
				api.INSTALLSTATE installState = api.MsiQueryProductState(vcredist_x86);

				string tooltip = toolTip.GetToolTip(checkIntegrateExplorerContext);
				int index;
				if ( (index = tooltip.IndexOf("vcredist_x86:")) == -1 )
				{
					tooltip += "\r\nvcredist_x86: ";
				}
				else
				{
					tooltip = tooltip.Substring(0, index + 14);	// 14:"vcredist_x86: "
				}
				toolTip.SetToolTip(checkIntegrateExplorerContext, tooltip + installState);

				if ( !Program.debMode && (installState < api.INSTALLSTATE.INSTALLSTATE_ABSENT) )
				{
					string text = "マイクロソフトのサイトから下記のランタイムをダウンロードしてインストールして下さい. (" + installState + ")\r\n" +
								  "[Microsoft Visual C++ 2008 SP1 再頒布可能パッケージ (x86)]\r\n" +
								  "http://www.microsoft.com/Downloads/details.aspx?displaylang=ja&FamilyID=a5c84275-3b97-4ab7-a40d-3802b2af5fc2\r\n" +
								  "※このメッセージは [Ctrl]+[C]/[V] でコピペできます.";
					MessageBox.Show(text, "Microsoft Visual C++ 2008 SP1 Redistributable", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
				return;
			}

			checkIntegrateExplorerContext.Checked = !checkIntegrateExplorerContext.Checked;
			modifiedCcMushroomExeConfig = true;
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// radioExeConfig_CheckedChanged
		/// </summary>
		private void radioExeConfig_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			labelSampleButton.Invalidate();

			modifiedCcMushroomExeConfig = true;
			checkReload.Checked = true;
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// numericButtonSize_ValueChanged
		/// </summary>
		private void numericButtonSize_ValueChanged(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			ResizeLabelSampleButton(((NumericUpDown)sender).Name);

			modifiedCcMushroomExeConfig = true;
			checkReload.Checked = true;
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// numericButtonSize_Leave
		/// </summary>
		private void numericButtonSize_Leave(object sender, System.EventArgs e)
		{
			NumericUpDown numericButtonSize = (NumericUpDown)sender;
			if ( numericButtonSize.Text == numericButtonSize.Value.ToString() )
				return;

			ResizeLabelSampleButton(numericButtonSize.Name);

			modifiedCcMushroomExeConfig = true;
			checkReload.Checked = true;
		}

		/// <summary>
		/// ResizeLabelSampleButton
		/// </summary>
		/// <param name="controlName"></param>
		private void ResizeLabelSampleButton(string controlName)
		{
			if ( controlName == numericButtonWidth.Name )
			{
				int width = Decimal.ToInt32(numericButtonWidth.Value);
				if ( width <= maxSampleButtonWidth )
				{
					labelSampleButton.Width = width;
				}
				else
				{
					labelSampleButton.Width = maxSampleButtonWidth;
				}
			}
			else if ( controlName == numericButtonHeight.Name )
			{
				labelSampleButton.Height = Decimal.ToInt32(numericButtonHeight.Value);
			}
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// [フォントの選択] ボタンが押された
		/// </summary>
		private void buttonSelectFont_Click(object sender, System.EventArgs e)
		{
			try
			{
#if ENABLE_TAB_BACKGROUND
				if ( checkForSelectedTab.Checked )	// 現在のタブ用の設定
				{
					colorDialog.Color = buttonTextColor;
					colorDialog.FullOpen = true;
					colorDialog.CustomColors = new[] { ColorTranslator.ToWin32(buttonTextColor) };

					if ( colorDialog.ShowDialog(this) == DialogResult.OK )
					{
						buttonTextColor = colorDialog.Color;
						labelSampleButton.Refresh();

						modifiedCcMushroomExeConfig = true;
						checkReload.Checked = true;
					}

					labelSampleButton.Select();
					return;
				}
#endif

				fontDialog.Font = Program.CreateFont(toolTip.GetToolTip(labelSampleButton), out buttonTextColor);
				fontDialog.Color = buttonTextColor;
				fontDialog.AllowVectorFonts = false;
				fontDialog.AllowVerticalFonts = false;
				fontDialog.MaxSize = Decimal.ToInt32(numericButtonHeight.Value);
				fontDialog.ShowColor = true;

				if ( fontDialog.ShowDialog(this) == DialogResult.OK )
				{
					labelSampleButton.Font = fontDialog.Font;
					buttonTextColor = fontDialog.Color;
					toolTip.SetToolTip(labelSampleButton, Program.GetFontInfo(fontDialog.Font, fontDialog.Color));

					modifiedCcMushroomExeConfig = true;
					checkReload.Checked = true;
				}

				labelSampleButton.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// [背景色の選択] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelectBackColor_Click(object sender, EventArgs e)
		{
			try
			{
				colorDialog.Color = buttonBackColor;
				colorDialog.FullOpen = true;
				colorDialog.CustomColors = new[] { ColorTranslator.ToWin32(buttonBackColor) };

				if ( colorDialog.ShowDialog(this) == DialogResult.OK )
				{
					buttonBackColor = colorDialog.Color;
					labelSampleButton.BackColor = buttonBackColor;

					modifiedCcMushroomExeConfig = true;
					checkReload.Checked = true;
				}

				labelSampleButton.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// [現在のタブ用] がチェックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkForSelectedTab_CheckedChanged(object sender, EventArgs e)
		{
#if ENABLE_TAB_BACKGROUND
			try
			{
				bool enabled = checkForSelectedTab.Checked;

				labelAppIconSize.Enabled = !enabled;
				radioAppIconLarge.Enabled = !enabled;
				radioAppIconSmall.Enabled = !enabled;

				Size _buttonSize = (enabled) ? ccMushroom.GetTabButtonSize(ccMushroom.selectedTabPageText) : ccMushroom.buttonSize;
				numericButtonWidth.Value = _buttonSize.Width;
				numericButtonHeight.Value = _buttonSize.Height;

				//labelButtonFont.Enabled = !enabled;
				//buttonSelectFont.Enabled = !enabled;
				labelButtonFont.Text = (enabled) ? "ボタンの文字色" : " ボタンのフォント";
				buttonTextColor = (enabled) ? ccMushroom.GetTabButtonTextColor(ccMushroom.selectedTabPageText) : ccMushroom.buttonTextColor;
				labelSampleButton.Refresh();

				labelButtonBackColor.Enabled = !enabled;
				buttonSelectBackColor.Enabled = !enabled;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// labelSampleButton_Paint
		/// </summary>
		private void labelSampleButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			try
			{
				Control control = (Control)sender;
				string iconFile = Application.StartupPath + "\\" + Application.ProductName + ".exe";
				Icon appIcon = ccf.GetIcon(iconFile, radioAppIconSmall.Checked);
				ccf.PaintButton(control, e.Graphics, radioAppIconSmall.Checked, appIcon, -1, true, null, sampleButtonText, -1.0F, labelSampleButton.Font, buttonTextColor, buttonBackColor, false);
				if ( appIcon != null )
				{
					api.DestroyIcon(appIcon.Handle);
					appIcon.Dispose();
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// 入力されたテキストの有効性を検証する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExeConfig_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				TextBox textBox = (TextBox)sender;

				if ( ActiveControl == textBox )	// フォーム上で ESC キーが押された？（フォーカスが次に移動してないので）
					return;

				bool validate = true;
				Regex regex;

				switch ( textBox.Name )
				{
					case "textAutoWindowCloseTime":
					case "textShortcutMngApplyTime":
					case "textAutoOpacityTime":
					case "textAutoOpacityPercent":
						regex = new Regex(@"^[0-9]+$");
						validate = regex.IsMatch(textBox.Text);
						break;
					case "textHkCode":
						if ( textBox.Text.Length != 0 )
						{
							regex = new Regex(@"^[a-zA-Z]+$");
							validate = regex.IsMatch(textBox.Text);
						}
						break;
				}

				if ( !validate )
				{
					errorProvider.SetError(textBox, "input error...");
					// e.Cancel = true　でCancel を true にすると正しく入力しないと次に行けない。
					e.Cancel = true;
				}
				else
				{
					if ( textBox.Name == textAutoOpacityPercent.Name )
					{
						int percent = int.Parse(textBox.Text);
						if ( (percent < 5) || (100 < percent) )
						{
							errorProvider.SetError(textBox, "limit error...");
							e.Cancel = true;
						}
						else
						{
							if ( textAutoOpacityTime.Text != "0" )
							{
								this.Opacity = (double)(percent) / 100.0;
							}
						}
					}
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		/// <summary>
		/// ccMushroom.exe.config
		/// テキストの検証が終了した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExeConfig_Validated(object sender, EventArgs e)
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
		/// tabPageExeConfig_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageExeConfig_Click(object sender, EventArgs e)
		{
			try
			{
				if ( this.Opacity != 1.0 )
				{
					this.Opacity = 1.0;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region ccConfiguration.import.xml
		/// <summary>
		/// ccConfiguration.import.xml
		/// ShowApplicationList
		/// </summary>
		private void ShowApplicationList()
		{
			try
			{
				listViewApplication.Items.Clear();

				foreach ( XmlNode application in xmlCcConfigurationImport.DocumentElement.ChildNodes )
				{
					if ( application.Name != ccMushroom.TAG_APPLICATION )
						continue;

					SetListApplicationItem(-1, application);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// textImport_Enter
		/// </summary>
		private void textImport_Enter(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			this.AcceptButton = buttonAppRegist;
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// textImport_Leave
		/// </summary>
		private void textImport_Leave(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			this.AcceptButton = buttonOk;
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// 入力されたテキストの有効性を検証する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textImport_Validating(object sender, CancelEventArgs e)
		{

		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// テキストの検証が終了した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textImport_Validated(object sender, EventArgs e)
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
		/// ccConfiguration.import.xml
		/// [登録] ボタンが押された
		/// </summary>
		private void buttonAppRegist_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.AcceptButton = buttonOk;

				if ( textTabText.Text.Length == 0 )
				{
					/*MessageBox.Show("タブ名を入力して下さい.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textTabText.Select();
					return;*/
					textTabText.Text = defTabText;
				}
				if ( textButtonText.Text.Length == 0 )
				{
					//MessageBox.Show("ボタン名を入力して下さい.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textButtonText.Select();
					return;
				}
				if ( textAppName.Text.Length == 0 )
				{
					//MessageBox.Show("アプリケーションを入力して下さい.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textAppName.Select();
					return;
				}

				//string xpath = ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_APP_NAME + "='" + textAppName.Text + "']";
				string xpath = ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_BUTTON_TEXT + "='" + textButtonText.Text + "']";
				XmlNode applicationImport = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);

				if ( radioAppNew.Checked )	// 新規
				{
					if ( applicationImport != null )
					{
						MessageBox.Show("そのボタン名は既に登録されています.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					string autoExec, buttonBackColor, comment, iconFile;
					GetOtherParams(listViewApplication.SelectedIndices == null ? -1 : listViewApplication.SelectedIndices[0], out autoExec, out buttonBackColor, out comment, out iconFile);

					if ( (applicationImport = ccMushroom.CreateAppImportNode(ref xmlCcConfigurationImport, textTabText.Text, textButtonText.Text, buttonBackColor, textAppName.Text, textCommandLine.Text, textWorkingDirectory.Text, comment, iconFile, true, bool.Parse(autoExec), false)) == null )
						return;

					// <application> ノードを追加する
					AppendAppImport(applicationImport);

					listViewApplication.EnsureVisible(listViewApplication.Items.Count - 1);

					// 入力領域をクリアする
					ClearAppInputs();
				}
				else if ( radioAppEdit.Checked || radioAppDelete.Checked )	// 変更 or 削除
				{
					if ( applicationImport == null )
					{
						if ( radioAppDelete.Checked || listViewApplication.SelectedItems.Count == 0 )
						{
							MessageBox.Show("そのアプリケーションは登録されていません.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						else
						{
							int selected = listViewApplication.SelectedIndices[0];
							applicationImport = xmlCcConfigurationImport.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION)[selected];
							//// アプリケーションが変更された
							//applicationImport[ccMushroom.TAG_APP_NAME].InnerText = textAppName.Text;
							//listViewApplication.Items[selected].SubItems[(int)imp.app].Text = textAppName.Text ;
							// ボタン名が変更された
							applicationImport[ccMushroom.TAG_BUTTON_TEXT].InnerText = textButtonText.Text;
							listViewApplication.Items[selected].SubItems[(int)imp.button].Text = textButtonText.Text;
						}
					}

					// リストビュー内のボタン名を検索する
					int index = lvAppIndexOfButton(textButtonText.Text);
					if ( index == -1 )
					{
						MessageBox.Show("プログラム内部のデータが不整合です.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					if ( radioAppEdit.Checked )
					{
						string autoExec, buttonBackColor, comment, iconFile;
						GetOtherParams(index, out autoExec, out buttonBackColor, out comment, out iconFile);

						// <application> ノードを編集する
						XmlNode _application;
						if ( (_application = ccMushroom.CreateAppImportNode(ref xmlCcConfigurationImport, textTabText.Text, textButtonText.Text, buttonBackColor, textAppName.Text, textCommandLine.Text, textWorkingDirectory.Text, comment, iconFile, listViewApplication.Items[index].Checked, bool.Parse(autoExec), false)) == null )
							return;

						EditAppImport(_application, applicationImport, index);
					}
					else if ( radioAppDelete.Checked )
					{
						// <application> ノードを削除する
						DeleteAppImport(applicationImport, index);
					}
				}

				listViewApplication.Select();

				modifiedCcConfigurationImportXml = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// リストビュー内のボタン名を検索する
		/// </summary>
		/// <param name="buttonName"></param>
		/// <returns></returns>
		private int lvAppIndexOfButton(string buttonText)
		{
			int index;

			for ( index = 0; index < listViewApplication.Items.Count; index++ )
			{
				//if ( listViewApplication.Items[index].SubItems[(int)imp.app].Text == textAppName.Text )
				if ( listViewApplication.Items[index].SubItems[(int)imp.button].Text == buttonText )
					break;
			}

			if ( index == listViewApplication.Items.Count )
			{
				index = -1;
			}

			return index;
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// application ノードを追加する
		/// </summary>
		/// <param name="_application"></param>
		private void AppendAppImport(XmlNode _application)
		{
			xmlCcConfigurationImport.DocumentElement.AppendChild(_application);

			// <application> ノードをリストビューへ追加する
			SetListApplicationItem(-1, _application);
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// application ノードを編集して listViewApplication を更新する
		/// </summary>
		/// <param name="_application"></param>
		/// <param name="applicationImport"></param>
		/// <param name="index"></param>
		private void EditAppImport(XmlNode _application, XmlNode applicationImport, int index)
		{
			xmlCcConfigurationImport.DocumentElement.ReplaceChild(_application, applicationImport);

			// リストビューを編集する
			SetListApplicationItem(index, _application);
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// application ノードを削除して listViewApplication を更新する
		/// </summary>
		/// <param name="applicationImport"></param>
		/// <param name="index"></param>
		private void DeleteAppImport(XmlNode applicationImport, int index)
		{
			xmlCcConfigurationImport.DocumentElement.RemoveChild(applicationImport);

			listViewApplication.Items[index].Remove();

			int itemsCount = listViewApplication.Items.Count;
			if ( itemsCount != 0 )
			{
				listViewApplication.Items[Math.Min(index, itemsCount - 1)].Selected = true;
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// ClearAppInputs
		/// </summary>
		private void ClearAppInputs()
		{
			textTabText.Text = string.Empty;
			textButtonText.Text = string.Empty;
			textAppName.Text = string.Empty;
			textCommandLine.Text = string.Empty;
			textWorkingDirectory.Text = string.Empty;
			if ( appIcon != null )
			{
				api.DestroyIcon(appIcon.Handle);
				appIcon.Dispose();
				appIcon = null;
			}
			pictureIcon.Image = null;

			radioAppNew.Checked = true;
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// [移動] ボタンが押された
		/// </summary>
		private void buttonAppMove_Click(object sender, System.EventArgs e)
		{
			try
			{
				int selected = listViewApplication.SelectedIndices[0];
				int reference = (radioAppUp.Checked) ? selected - 1 : selected + 1;
				XmlNode newChild = xmlCcConfigurationImport.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION)[selected];
				XmlNode refChild = xmlCcConfigurationImport.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION)[reference];
				if ( radioAppUp.Checked )
					xmlCcConfigurationImport.DocumentElement.InsertBefore(newChild, refChild);
				else
					xmlCcConfigurationImport.DocumentElement.InsertAfter(newChild, refChild);

				SetListApplicationItem(selected, refChild);
				SetListApplicationItem(reference, newChild);

				listViewApplication.Items[reference].Selected = true;
				listViewApplication.EnsureVisible(reference);
				buttonAppMove.Select();

				modifiedCcConfigurationImportXml = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// SetListApplicationItem
		/// </summary>
		private void SetListApplicationItem(int index, XmlNode application)
		{
			if ( index == -1 )
			{
				// <application> ノードをリストビューへ追加する
				ListViewItem item = new ListViewItem(application[ccMushroom.TAG_TAB_TEXT].InnerText);
#if true
				XmlAttribute attr = application.Attributes[ccMushroom.ATTRIB_ENABLED];
				if ( attr == null )
				{
					attr = xmlCcConfigurationImport.CreateAttribute(ccMushroom.ATTRIB_ENABLED);
					attr.Value = true.ToString().ToLower();
					application.Attributes.Append(attr);
				}
				item.Checked = bool.Parse(attr.Value);
#endif
				item.SubItems.Add(application[ccMushroom.TAG_BUTTON_TEXT].InnerText);
				item.SubItems.Add(application[ccMushroom.TAG_APP_NAME].InnerText);
				item.SubItems.Add((application[ccMushroom.TAG_COMMAND_LINE] == null) ? string.Empty : application[ccMushroom.TAG_COMMAND_LINE].InnerText);
				item.SubItems.Add((application[ccMushroom.TAG_WORKING_DIRECTORY] == null) ? string.Empty : application[ccMushroom.TAG_WORKING_DIRECTORY].InnerText);
				SetOtherParams(item, application);
				listViewApplication.Items.Add(item);
				//listViewApplication.EnsureVisible(listViewApplication.Items.Count - 1);
			}
			else
			{
				ListViewItem item = listViewApplication.Items[index];
				item.Checked = bool.Parse(application.Attributes[ccMushroom.ATTRIB_ENABLED].Value);
				item.SubItems[(int)imp.tab].Text = application[ccMushroom.TAG_TAB_TEXT].InnerText;
				item.SubItems[(int)imp.button].Text = application[ccMushroom.TAG_BUTTON_TEXT].InnerText;
				item.SubItems[(int)imp.app].Text = application[ccMushroom.TAG_APP_NAME].InnerText;
				item.SubItems[(int)imp.arg].Text = (application[ccMushroom.TAG_COMMAND_LINE] == null) ? string.Empty : application[ccMushroom.TAG_COMMAND_LINE].InnerText;
				item.SubItems[(int)imp.dir].Text = (application[ccMushroom.TAG_WORKING_DIRECTORY] == null) ? string.Empty : application[ccMushroom.TAG_WORKING_DIRECTORY].InnerText;
				SetOtherParams(item, application);
			}
		}

		/// <summary>
		/// 編集可能なパラメータ以外を Tag にセットする
		/// </summary>
		/// <param name="item"></param>
		/// <param name="application"></param>
		private void SetOtherParams(ListViewItem item, XmlNode application)
		{
			try
			{
				string autoExec = (application.Attributes[ccMushroom.ATTRIB_AUTO_EXEC] == null) ? false.ToString() : application.Attributes[ccMushroom.ATTRIB_AUTO_EXEC].Value;
				string buttonBackColor = (application[ccMushroom.TAG_BUTTON_TEXT].Attributes[ccMushroom.ATTRIB_BACK_COLOR] == null) ? string.Empty : application[ccMushroom.TAG_BUTTON_TEXT].Attributes[ccMushroom.ATTRIB_BACK_COLOR].Value;
				/*string comment = (application[ccMushroom.TAG_COMMENT] == null) ? string.Empty : application[ccMushroom.TAG_COMMENT].InnerText;
				string iconFile = (application[ccMushroom.TAG_ICON_FILE] == null) ? string.Empty : application[ccMushroom.TAG_ICON_FILE].InnerText;

				item.Tag = autoExec + "\t" + buttonBackColor + "\t" + comment + "\t" + iconFile;*/
				string comment = (application[ccMushroom.TAG_COMMENT] == null) ? null : application[ccMushroom.TAG_COMMENT].InnerText;
				string iconFile = (application[ccMushroom.TAG_ICON_FILE] == null) ? null : application[ccMushroom.TAG_ICON_FILE].InnerText;

				item.Tag = new string[] { autoExec, buttonBackColor, comment, iconFile };
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 編集可能なパラメータ以外を Tag から取得する
		/// </summary>
		/// <param name="index"></param>
		/// <param name="autoExec"></param>
		/// <param name="buttonBackColor"></param>
		/// <param name="comment"></param>
		/// <param name="iconFile"></param>
		private void GetOtherParams(int index, out string autoExec, out string buttonBackColor, out string comment, out string iconFile)
		{
			autoExec = buttonBackColor = comment = iconFile = null;

			try
			{
				if ( index == -1 )
					return;

				ListViewItem item = listViewApplication.Items[index];
				if ( item.Tag == null )
					return;

				/*string[] otherParams = ((string)item.Tag).Split('\t');
				if ( otherParams.Length != 4 )
					return;

				autoExec = otherParams[0];
				buttonBackColor = string.IsNullOrEmpty(otherParams[1]) ? null : otherParams[1];
				comment = string.IsNullOrEmpty(otherParams[2]) ? null : otherParams[2];
				iconFile = string.IsNullOrEmpty(otherParams[3]) ? null : otherParams[3];*/
				string[] otherParams = (string[])item.Tag;
				if ( otherParams.Length != 4 )
					return;

				autoExec = otherParams[0];
				buttonBackColor = otherParams[1];
				comment = otherParams[2];
				iconFile = otherParams[3];
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// listViewApplication_ItemCheck
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listViewApplication_ItemCheck(object sender, ItemCheckEventArgs e)
		{
#if true
			try
			{
				if ( nowLoading )
					return;

				xmlCcConfigurationImport.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION)[e.Index].Attributes[ccMushroom.ATTRIB_ENABLED].Value = (e.NewValue == CheckState.Checked).ToString().ToLower();

				modifiedCcConfigurationImportXml = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
#endif
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// [アプリケーションの選択] ボタンが押された
		/// </summary>
		private void buttonSelectApp_Click(object sender, System.EventArgs e)
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

				if ( openFileDialog.ShowDialog(this) == DialogResult.OK )
				{
					string fileName = openFileDialog.FileName;
					if ( textTabText.Text.Length == 0 )
						textTabText.Text = defTabText;
					textButtonText.Text = Path.GetFileNameWithoutExtension(fileName);
					textAppName.Text = fileName;
					textAppName.Select(textAppName.Text.Length, 0);
					textCommandLine.Text = string.Empty;
					textWorkingDirectory.Text = string.Empty;
					try
					{
						//pictureIcon.Image = common.GetIcon (fileName, true).ToBitmap ();
						appIcon = ccf.GetIcon(fileName, true);
						pictureIcon.Invalidate();
					}
					catch ( Exception ) { }
				}

				textButtonText.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// listViewApplication_SelectedIndexChanged
		/// </summary>
		private void listViewApplication_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				radioAppUp.Enabled = false;
				radioAppDown.Enabled = false;
				buttonAppMove.Enabled = false;

				if ( listViewApplication.SelectedItems.Count == 0 )
				{
					ClearAppInputs();
				}
				else
				{
					textTabText.Text = listViewApplication.SelectedItems[0].SubItems[(int)imp.tab].Text;
					textButtonText.Text = listViewApplication.SelectedItems[0].SubItems[(int)imp.button].Text;
					textAppName.Text = listViewApplication.SelectedItems[0].SubItems[(int)imp.app].Text;
					textAppName.Select(textAppName.Text.Length, 0);
					textCommandLine.Text = listViewApplication.SelectedItems[0].SubItems[(int)imp.arg].Text;
					textWorkingDirectory.Text = listViewApplication.SelectedItems[0].SubItems[(int)imp.dir].Text;
					try
					{
						string appName = common.CheckEnvironmentVariable(textAppName.Text);
						//pictureIcon.Image = common.GetIcon (appName, true).ToBitmap ();
						appIcon = ccf.GetIcon(appName, true);
						pictureIcon.Invalidate();
					}
					catch ( Exception ) { }

					radioAppEdit.Checked = true;
					if ( listViewApplication.SelectedIndices[0] != 0 )
						radioAppUp.Enabled = true;
					if ( listViewApplication.SelectedIndices[0] != listViewApplication.Items.Count - 1 )
						radioAppDown.Enabled = true;
					if ( !radioAppDown.Enabled )
						radioAppUp.Checked = true;
					else if ( !radioAppUp.Enabled )
						radioAppDown.Checked = true;
					buttonAppMove.Enabled = (radioAppUp.Enabled || radioAppDown.Enabled)/*true*/;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// pictureIcon_Paint
		/// </summary>
		private void pictureIcon_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			try
			{
				if ( appIcon == null )
					return;

				int iconX = 3;
				int iconY = 1;

				e.Graphics.DrawIcon(appIcon, iconX, iconY);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// listViewApplication でキーが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listViewApplication_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if ( e.KeyCode == Keys.Delete )
				{
					if ( listViewApplication.SelectedItems.Count == 0 )
						return;

					int index = listViewApplication.SelectedItems[0].Index;
					string buttonText = listViewApplication.Items[index].SubItems[(int)imp.button].Text;

					string xpath = ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_BUTTON_TEXT + "='" + buttonText/*textButtonText.Text*/ + "']";
					XmlNode applicationImport = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);
					if ( applicationImport == null )
						return;

					DeleteAppImport(applicationImport, index);

					modifiedCcConfigurationImportXml = true;
					checkReload.Checked = true;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// インポート用 ccMushroom 構成ファイルのドラッグが開始された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageImport_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
				{
					string[] sourceFileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
					if ( Path.GetExtension(sourceFileNames[0]) == ".xml" )
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
		/// インポート用 ccMushroom 構成ファイルがドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageImport_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				object obj = e.Data.GetData(DataFormats.FileDrop);
				string[] configImportFileNames = (string[])obj;

				string message = "インポート用 ccMushroom 構成ファイルを取り込みます.\n" +
								 Path.GetFileName(configImportFileNames[0]) + "\n" +
								 "[はい(Y)] 既存の設定名は上書きする\n" +
								 "[いいえ(N)] 既存の設定名はスキップする\n" +
								 "[キャンセル] 中止する";
				DialogResult res = MessageBox.Show(message, THIS_FORM_TEXT, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if ( res == DialogResult.Cancel )
					return;

				Cursor.Current = Cursors.WaitCursor;

				bool overwrite = (res == DialogResult.Yes);
				XmlDocument xmlConfigImport = new XmlDocument();
				xmlConfigImport.Load(configImportFileNames[0]);	// ドロップされたファイル

				// <defaultTabText>
				XmlNode nodeDefTabText = xmlConfigImport.DocumentElement[ccMushroom.TAG_DEFAULT_TAB_TEXT];
				if ( nodeDefTabText != null )
				{
					XmlNode _nodeDefTabText = xmlCcConfigurationImport.DocumentElement[ccMushroom.TAG_DEFAULT_TAB_TEXT];
					if ( _nodeDefTabText == null )
					{
						_nodeDefTabText = xmlCcConfigurationImport.CreateNode(XmlNodeType.Element, ccMushroom.TAG_DEFAULT_TAB_TEXT, null);
						xmlCcConfigurationImport.DocumentElement.InsertAfter(_nodeDefTabText, null);
					}

					if ( nodeDefTabText.InnerText.Length == 0 )	// <defaultTabText> を削除する？
					{
						xmlCcConfigurationImport.DocumentElement.RemoveChild(_nodeDefTabText);
						toolTip.SetToolTip(labelImportXml, ccMushroom.TAG_DEFAULT_TAB_TEXT + ": ");
						defTabText = "お気に入り";
					}
					else
					{
						_nodeDefTabText.InnerText = nodeDefTabText.InnerText;
						toolTip.SetToolTip(labelImportXml, ccMushroom.TAG_DEFAULT_TAB_TEXT + ": " + _nodeDefTabText.InnerText);
						defTabText = _nodeDefTabText.InnerText;
					}

					modifiedCcConfigurationImportXml = true;
				}

				// <application>
				foreach ( XmlNode application in xmlConfigImport.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION) )
				{
					if ( (application[ccMushroom.TAG_TAB_TEXT] == null) || (application[ccMushroom.TAG_BUTTON_TEXT] == null) || application[ccMushroom.TAG_APP_NAME] == null )
						continue;
					if ( /*(application[ccMushroom.TAG_TAB_TEXT].InnerText.Length == 0) || */(application[ccMushroom.TAG_BUTTON_TEXT].InnerText.Length == 0) || application[ccMushroom.TAG_APP_NAME].InnerText.Length == 0 )
						continue;

					if ( application[ccMushroom.TAG_TAB_TEXT].InnerText.Length == 0 )
					{
						application[ccMushroom.TAG_TAB_TEXT].InnerText = defTabText;
					}

					int index = lvAppIndexOfButton(application[ccMushroom.TAG_BUTTON_TEXT].InnerText);
					if ( index == -1 )	// 新規？
					{
						XmlNode _application = xmlCcConfigurationImport.ImportNode(application, true);

						AppendAppImport(_application);
					}
					else
					{
						if ( !overwrite )
							continue;

						string xpath = ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_BUTTON_TEXT + "='" + application[ccMushroom.TAG_BUTTON_TEXT].InnerText + "']";
						XmlNode oldChild = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);
						XmlNode _application = xmlCcConfigurationImport.ImportNode(application, true);

						EditAppImport(_application, oldChild, index);
					}

					modifiedCcConfigurationImportXml = true;
				}

				if ( listViewApplication.SelectedItems.Count != 0 )
				{
					listViewApplication.SelectedItems[0].Selected = false;
				}

				// 入力領域をクリアする
				ClearAppInputs();

				if ( modifiedCcConfigurationImportXml )
				{
					checkReload.Checked = true;
				}

				Cursor.Current = Cursors.Default;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region ccMushroom.ini
		/// <summary>
		/// ccMushroom.ini
		/// ShowProgramsFolderList
		/// </summary>
		private void ShowProgramsFolderList()
		{
			try
			{
				listViewProgramsFolder.Items.Clear();

				for ( int i = 1; i < remoteProgramsFolders.Length; i++ )
				{
					localProgramsFolders[i] = ccf.ReplaceCcMushroomLocalFolder(localProgramsFolders[i], false);

					SetListProgramsFolderItem(-1, remoteProgramsNames[i], remoteProgramsFolders[i], localProgramsFolders[i], enabledProgramsFolders[i]);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// textProgramsFolder_Enter
		/// </summary>
		private void textProgramsFolder_Enter(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			this.AcceptButton = buttonFoldRegist;
		}

		/// <summary>
		/// ccMushroom.ini
		/// textProgramsFolder_Leave
		/// </summary>
		private void textProgramsFolder_Leave(object sender, System.EventArgs e)
		{
			if ( nowLoading )
				return;

			this.AcceptButton = buttonOk;
		}

		/// <summary>
		/// ccMushroom.ini
		/// 入力されたテキストの有効性を検証する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textProgramsFolder_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				TextBox textBox = (TextBox)sender;

				if ( ActiveControl == textBox )	// フォーム上で ESC キーが押された？（フォーカスが次に移動してないので）
					return;

				if ( textBox.Text.Length == 0 )
					return;

				bool validate = true;
				string message = string.Empty;

				if ( textBox.Name == textScanRemoteProgramsFolder.Name )
				{
					string privateRemoteProgramsFolder = textScanRemoteProgramsFolder.Text;
					if ( !ccf.AdjustFolderFormat(ref privateRemoteProgramsFolder) )
					{
						message = "リモート側のプログラム フォルダが不正です.";
						validate = false;
					}
					textScanRemoteProgramsFolder.Text = privateRemoteProgramsFolder;
				}
				else if ( textBox.Name == textScanLocalProgramsFolder.Name )
				{
					string privateLocalProgramsFolder = ccf.ReplaceCcMushroomLocalFolder(textScanLocalProgramsFolder.Text, true);
					if ( !ccf.AdjustFolderFormat(ref privateLocalProgramsFolder) )
					{
						message = "ローカル側のプログラム フォルダが不正です.";
						validate = false;
					}
					else
					{
						if ( ccf.IsSystemFolder(privateLocalProgramsFolder) )
						{
							message = "このフォルダはローカル側のプログラム フォルダにできません.";
							validate = false;
						}
					}
					textScanLocalProgramsFolder.Text = ccf.ReplaceCcMushroomLocalFolder(privateLocalProgramsFolder, false);
				}

				if ( !validate )
				{
					errorProvider.SetError(textBox, message);
					// e.Cancel = true　でCancel を true にすると正しく入力しないと次に行けない。
					e.Cancel = true;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// テキストの検証が終了した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textProgramsFolder_Validated(object sender, EventArgs e)
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
		/// ccMushroom.ini
		/// [リモート側|ローカル側] ラベルがダブルクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void labelScanAnyProgramsFolder_DoubleClick(object sender, EventArgs e)
		{
			try
			{
				string directoryName = null;

				if ( ((Label)sender).Name == labelScanRemoteProgramsFolder.Name )
				{
					directoryName = textScanRemoteProgramsFolder.Text;
					if ( directoryName.EndsWith(".xml") )
					{
						directoryName = Path.GetDirectoryName(directoryName);
					}
				}
				else if ( ((Label)sender).Name == labelScanLocalProgramsFolder.Name )
				{
					directoryName = textScanLocalProgramsFolder.Text;
					directoryName = ccf.ReplaceCcMushroomLocalFolder(directoryName, true);
				}

				if ( string.IsNullOrEmpty(directoryName) )
					return;

				Process.Start("explorer", directoryName);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// [リモート側|ローカル側のプログラム フォルダ] ボタンが押された
		/// </summary>
		private void buttonSelectProgramsFolder_Click(object sender, System.EventArgs e)
		{
			try
			{
				bool remoteProgramsFolder = (((Button)sender).Name == buttonSelectRemoteFolder.Name);
				string scanRemoteProgramsFileName = null;

				folderBrowserDialog.Reset();	// こうしないと前回選択したディレクトリが有効になってしまう？

				if ( remoteProgramsFolder )
				{
					folderBrowserDialog.Description = "リモート側のプログラム フォルダを選択して下さい.";
					if ( textScanRemoteProgramsFolder.Text.Length != 0 )
					{
						folderBrowserDialog.SelectedPath = textScanRemoteProgramsFolder.Text;
						if ( folderBrowserDialog.SelectedPath.EndsWith(".xml") )
						{
							folderBrowserDialog.SelectedPath = Path.GetDirectoryName(textScanRemoteProgramsFolder.Text);
							scanRemoteProgramsFileName = Path.GetFileName(textScanRemoteProgramsFolder.Text);
						}
					}
					folderBrowserDialog.ShowNewFolderButton = false;
				}
				else
				{
					folderBrowserDialog.Description = "ローカル側のプログラム フォルダを選択して下さい.";
					folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
					if ( textScanLocalProgramsFolder.Text.Length != 0 )
					{
						folderBrowserDialog.SelectedPath = ccf.ReplaceCcMushroomLocalFolder(textScanLocalProgramsFolder.Text, true);
					}
					else
					{
						string windir = Environment.GetEnvironmentVariable("windir");
						string systemDriveRoot = (3 < windir.Length) ? windir.Substring(0, 3) : string.Empty;
						folderBrowserDialog.SelectedPath = systemDriveRoot;
					}
					folderBrowserDialog.ShowNewFolderButton = true;
				}

				if ( folderBrowserDialog.ShowDialog(this) == DialogResult.OK )
				{
					if ( remoteProgramsFolder )
					{
						textScanRemoteProgramsFolder.Text = folderBrowserDialog.SelectedPath + ((scanRemoteProgramsFileName == null) ? "" : "\\" + scanRemoteProgramsFileName);
						textScanRemoteProgramsFolder.Select(textScanRemoteProgramsFolder.Text.Length, 0);
					}
					else
					{
						textScanLocalProgramsFolder.Text = ccf.ReplaceCcMushroomLocalFolder(folderBrowserDialog.SelectedPath, false);
						textScanLocalProgramsFolder.Select(textScanLocalProgramsFolder.Text.Length, 0);
					}
				}

				if ( remoteProgramsFolder )
					textScanRemoteProgramsFolder.Select();
				else
					textScanLocalProgramsFolder.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// [登録] ボタンが押された
		/// </summary>
		private void buttonFoldRegist_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.AcceptButton = buttonOk;

				if ( textScanRemoteProgramsName.Text.Length == 0 )
				{
					textScanRemoteProgramsName.Select();
					return;
				}
				if ( textScanRemoteProgramsFolder.Text.Length == 0 )
				{
					//MessageBox.Show("リモート側のプログラム フォルダを入力して下さい.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textScanRemoteProgramsFolder.Select();
					return;
				}
				if ( textScanLocalProgramsFolder.Text.Length == 0 )
				{
					//MessageBox.Show("ローカル側のプログラム フォルダを入力して下さい.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textScanLocalProgramsFolder.Select();
					return;
				}

				string scanRemoteProgramsName = textScanRemoteProgramsName.Text;
				string scanRemoteProgramsFolder = textScanRemoteProgramsFolder.Text;
				string scanLocalProgramsFolder = textScanLocalProgramsFolder.Text;

				/*if ( !ccf.AdjustFolderFormat(ref scanRemoteProgramsFolder) )
				{
					MessageBox.Show("リモート側のプログラム フォルダが不正です.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textScanRemoteProgramsFolder.Select();
					return;
				}

				if ( !ccf.AdjustFolderFormat(ref scanLocalProgramsFolder) )
				{
					MessageBox.Show("ローカル側のプログラム フォルダが不正です.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textScanLocalProgramsFolder.Select();
					return;
				}
				if ( ccf.IsSystemFolder(scanLocalProgramsFolder) )
				{
					MessageBox.Show(scanLocalProgramsFolder + " フォルダはローカル側のプログラム フォルダにできません.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					textScanLocalProgramsFolder.Select();
					return;
				}*/

				int exist = -1;
				for ( int i = 0; i < remoteProgramsFolders.Length; i++ )
				{
					if ( scanRemoteProgramsFolder == remoteProgramsFolders[i] )
					{
						exist = i;
						break;
					}
				}

				if ( radioFoldNew.Checked )	// 新規
				{
					if ( exist != -1 )
					{
						MessageBox.Show("そのリモート側のプログラム フォルダは既に登録されています.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					AppendProgramsFolder(scanRemoteProgramsName, scanRemoteProgramsFolder, scanLocalProgramsFolder, true);

					listViewProgramsFolder.EnsureVisible(listViewProgramsFolder.Items.Count - 1);

					// 入力領域をクリアする
					ClearProgFolderInputs();
				}
				else if ( radioFoldEdit.Checked || radioFoldDelete.Checked )	// 変更 or 削除
				{
					if ( exist == -1 )
					{
						if ( radioFoldDelete.Checked || listViewProgramsFolder.SelectedItems.Count == 0 )
						{
							MessageBox.Show("そのリモート側のプログラム フォルダは登録されていません.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						else
						{
							// リモート側のプログラム フォルダが変更された
							int selected = listViewProgramsFolder.SelectedIndices[0];
							listViewProgramsFolder.Items[selected].SubItems[(int)fold.remote].Text = scanRemoteProgramsFolder;
						}
					}

					// リストビュー内のリモート側のプログラム フォルダを検索する
					int index = lvProgFolderIndexOfItem(fold.remote, scanRemoteProgramsFolder);
					if ( index == -1 )
					{
						MessageBox.Show("プログラム内部のデータが不整合です.", THIS_FORM_TEXT, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}

					if ( radioFoldEdit.Checked )
					{
						EditProgramsFolder(scanRemoteProgramsName, scanRemoteProgramsFolder, scanLocalProgramsFolder, enabledProgramsFolders[index + 1], index);
					}
					else if ( radioFoldDelete.Checked )
					{
						DeleteProgramsFolder(index);
					}
				}

				listViewProgramsFolder.Select();

				modifiedCcMushroomIni = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// リストビュー内のアイテムを検索する
		/// </summary>
		/// <param name="subItemIndex"></param>
		/// <param name="itemValue"></param>
		/// <returns></returns>
		private int lvProgFolderIndexOfItem(fold subItemIndex, string itemValue)
		{
			int index;

			for ( index = 0; index < listViewProgramsFolder.Items.Count; index++ )
			{
				if ( listViewProgramsFolder.Items[index].SubItems[(int)subItemIndex].Text == itemValue )
					break;
			}

			if ( index == listViewProgramsFolder.Items.Count )
			{
				index = -1;
			}

			return index;
		}

		/// <summary>
		/// ccMushroom.ini
		/// リモート/ローカルのプログラム フォルダ設定を追加する
		/// </summary>
		/// <param name="remoteProgramsName"></param>
		/// <param name="remoteProgramsFolder"></param>
		/// <param name="localProgramsFolder"></param>
		/// <param name="enabledProgramsFolder"></param>
		private void AppendProgramsFolder(string remoteProgramsName, string remoteProgramsFolder, string localProgramsFolder, bool enabledProgramsFolder)
		{
			int count = remoteProgramsFolders.Length;

			localProgramsFolder = ccf.ReplaceCcMushroomLocalFolder(localProgramsFolder, false);

			Array.Resize(ref remoteProgramsNames, count + 1);
			remoteProgramsNames[count] = remoteProgramsName;

			Array.Resize(ref remoteProgramsFolders, count + 1);
			remoteProgramsFolders[count] = remoteProgramsFolder;

			Array.Resize(ref localProgramsFolders, count + 1);
			localProgramsFolders[count] = localProgramsFolder;

			Array.Resize(ref enabledProgramsFolders, count + 1);
			enabledProgramsFolders[count] = enabledProgramsFolder;

			// プログラム フォルダをリストビューへ追加する
			SetListProgramsFolderItem(-1, remoteProgramsNames[count], remoteProgramsFolders[count], localProgramsFolders[count], enabledProgramsFolders[count]);
		}

		/// <summary>
		/// ccMushroom.ini
		/// リモート/ローカルのプログラム フォルダ設定を編集して、listViewProgramsFolder を更新する
		/// </summary>
		/// <param name="remoteProgramsName"></param>
		/// <param name="remoteProgramsFolder"></param>
		/// <param name="localProgramsFolder"></param>
		/// <param name="enabledProgramsFolder"></param>
		/// <param name="index"></param>
		private void EditProgramsFolder(string remoteProgramsName, string remoteProgramsFolder, string localProgramsFolder, bool enabledProgramsFolder, int index)
		{
			localProgramsFolder = ccf.ReplaceCcMushroomLocalFolder(localProgramsFolder, false);

			// プログラム フォルダを編集する
			remoteProgramsNames[index + 1] = remoteProgramsName;
			remoteProgramsFolders[index + 1] = remoteProgramsFolder;
			localProgramsFolders[index + 1] = localProgramsFolder;
			enabledProgramsFolders[index + 1] = enabledProgramsFolder;

			// リストビューを編集する
			SetListProgramsFolderItem(index, remoteProgramsNames[index + 1], remoteProgramsFolders[index + 1], localProgramsFolders[index + 1], enabledProgramsFolders[index + 1]);
		}

		/// <summary>
		/// ccMushroom.ini
		/// リモート/ローカルのプログラム フォルダ設定を削除して、listViewProgramsFolder を更新する
		/// </summary>
		/// <param name="index"></param>
		private void DeleteProgramsFolder(int index)
		{
			for ( int i = index + 1; i < remoteProgramsFolders.Length - 1; i++ )
			{
				remoteProgramsNames[i] = remoteProgramsNames[i + 1];
				remoteProgramsFolders[i] = remoteProgramsFolders[i + 1];
				localProgramsFolders[i] = localProgramsFolders[i + 1];
				enabledProgramsFolders[i] = enabledProgramsFolders[i + 1];
			}

			Array.Resize(ref remoteProgramsNames, remoteProgramsNames.Length - 1);
			Array.Resize(ref remoteProgramsFolders, remoteProgramsFolders.Length - 1);
			Array.Resize(ref localProgramsFolders, localProgramsFolders.Length - 1);
			Array.Resize(ref enabledProgramsFolders, enabledProgramsFolders.Length - 1);

			listViewProgramsFolder.Items[index].Remove();

			int itemsCount = listViewProgramsFolder.Items.Count;
			if ( itemsCount != 0 )
			{
				listViewProgramsFolder.Items[Math.Min(index, itemsCount - 1)].Selected = true;
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// ClearProgFolderInputs
		/// </summary>
		private void ClearProgFolderInputs()
		{
			textScanRemoteProgramsName.Text = string.Empty;
			textScanRemoteProgramsFolder.Text = string.Empty;
			textScanLocalProgramsFolder.Text = string.Empty;
			radioFoldNew.Checked = true;
		}

		/// <summary>
		/// ccMushroom.ini
		/// listViewProgramsFolder でキーが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listViewProgramsFolder_KeyDown(object sender, KeyEventArgs e)
		{
			try
			{
				if ( e.KeyCode == Keys.Delete )
				{
					if ( listViewProgramsFolder.SelectedItems.Count == 0 )
						return;

					int index = listViewProgramsFolder.SelectedItems[0].Index;

					DeleteProgramsFolder(index);

					modifiedCcMushroomIni = true;
					checkReload.Checked = true;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// [移動] ボタンが押された
		/// </summary>
		private void buttonFoldMove_Click(object sender, System.EventArgs e)
		{
			try
			{
				int selected = listViewProgramsFolder.SelectedIndices[0] + 1;
				int reference = (radioFoldUp.Checked) ? selected - 1 : selected + 1;

				string refRemoteProgramsName = remoteProgramsNames[reference];
				string refRemoteProgramsFolder = remoteProgramsFolders[reference];
				string refLocalProgramsFolder = localProgramsFolders[reference];
				bool refEnabledProgramsFolder = enabledProgramsFolders[reference];

				remoteProgramsNames[reference] = remoteProgramsNames[selected];
				remoteProgramsFolders[reference] = remoteProgramsFolders[selected];
				localProgramsFolders[reference] = localProgramsFolders[selected];
				enabledProgramsFolders[reference] = enabledProgramsFolders[selected];

				remoteProgramsNames[selected] = refRemoteProgramsName;
				remoteProgramsFolders[selected] = refRemoteProgramsFolder;
				localProgramsFolders[selected] = refLocalProgramsFolder;
				enabledProgramsFolders[selected] = refEnabledProgramsFolder;

				selected--;
				reference--;

				SetListProgramsFolderItem(selected, refRemoteProgramsName, refRemoteProgramsFolder, refLocalProgramsFolder, refEnabledProgramsFolder);
				SetListProgramsFolderItem(reference, remoteProgramsNames[reference + 1], remoteProgramsFolders[reference + 1], localProgramsFolders[reference + 1], enabledProgramsFolders[reference + 1]);

				listViewProgramsFolder.Items[reference].Selected = true;
				listViewProgramsFolder.EnsureVisible(reference);
				buttonFoldMove.Select();

				modifiedCcMushroomIni = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// SetListProgramsFolderItem
		/// </summary>
		private void SetListProgramsFolderItem(int index, string remoteProgramsName, string remoteProgramsFolder, string localProgramsFolder, bool enabledProgramsFolder)
		{
			if ( index == -1 )
			{
				ListViewItem item = new ListViewItem(remoteProgramsName);
				item.SubItems.Add(remoteProgramsFolder);
				item.SubItems.Add(localProgramsFolder);
				item.Checked = enabledProgramsFolder;
				listViewProgramsFolder.Items.Add(item);
				//listViewProgramsFolder.EnsureVisible(listViewProgramsFolder.Items.Count - 1);
			}
			else
			{
				listViewProgramsFolder.Items[index].SubItems[(int)fold.name].Text = remoteProgramsName;
				listViewProgramsFolder.Items[index].SubItems[(int)fold.remote].Text = remoteProgramsFolder;
				listViewProgramsFolder.Items[index].SubItems[(int)fold.local].Text = localProgramsFolder;
				listViewProgramsFolder.Items[index].Checked = enabledProgramsFolder;
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// listViewProgramsFolder_ItemCheck
		/// </summary>
		private void listViewProgramsFolder_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			try
			{
				if ( nowLoading )
					return;

				enabledProgramsFolders[e.Index + 1] = (e.NewValue == CheckState.Checked);

				modifiedCcMushroomIni = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom.ini
		/// listViewProgramsFolder_SelectedIndexChanged
		/// </summary>
		private void listViewProgramsFolder_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				radioFoldUp.Enabled = false;
				radioFoldDown.Enabled = false;
				buttonFoldMove.Enabled = false;

				if ( listViewProgramsFolder.SelectedItems.Count == 0 )
				{
					ClearProgFolderInputs();
				}
				else
				{
					textScanRemoteProgramsName.Text = listViewProgramsFolder.SelectedItems[0].SubItems[(int)fold.name].Text;
					textScanRemoteProgramsName.Select(textScanRemoteProgramsName.Text.Length, 0);
					textScanRemoteProgramsFolder.Text = listViewProgramsFolder.SelectedItems[0].SubItems[(int)fold.remote].Text;
					textScanRemoteProgramsFolder.Select(textScanRemoteProgramsFolder.Text.Length, 0);
					textScanLocalProgramsFolder.Text = listViewProgramsFolder.SelectedItems[0].SubItems[(int)fold.local].Text;
					textScanLocalProgramsFolder.Select(textScanLocalProgramsFolder.Text.Length, 0);

					radioFoldEdit.Checked = true;
					if ( listViewProgramsFolder.SelectedIndices[0] != 0 )
						radioFoldUp.Enabled = true;
					if ( listViewProgramsFolder.SelectedIndices[0] != listViewProgramsFolder.Items.Count - 1 )
						radioFoldDown.Enabled = true;
					if ( !radioFoldDown.Enabled )
						radioFoldUp.Checked = true;
					else if ( !radioFoldUp.Enabled )
						radioFoldDown.Checked = true;
					buttonFoldMove.Enabled = (radioFoldUp.Enabled || radioFoldDown.Enabled)/*true*/;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// ccMushroom 設定ファイルのドラッグが開始した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageScan_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
				{
					string[] sourceFileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
					if ( Path.GetExtension(sourceFileNames[0]) == ".ini" )
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
		/// ccMushroom 設定ファイルがドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageScan_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				object obj = e.Data.GetData(DataFormats.FileDrop);
				string[] ccMushroomFileNames = (string[])obj;

				string message = "ccMushroom 設定ファイルを取り込みます.\n" +
								 Path.GetFileName(ccMushroomFileNames[0]) + "\n" +
								 "[はい(Y)] 既存の設定名は上書きする\n" +
								 "[いいえ(N)] 既存の設定名はスキップする\n" +
								 "[キャンセル] 中止する";
				DialogResult res = MessageBox.Show(message, THIS_FORM_TEXT, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if ( res == DialogResult.Cancel )
					return;

				bool overwrite = (res == DialogResult.Yes);
				string ccMushroomIniFileName = ccMushroomFileNames[0];	// ドロップされたファイル

				byte[] returnedBytes = new byte[1024];
				int length = (int)api.GetPrivateProfileString/*ByByteArray*/(ccMushroom.SETTINGS_SECTION, null, "", returnedBytes, (uint)returnedBytes.Length, ccMushroomIniFileName);
				if ( length == 0 )
					return;

				Cursor.Current = Cursors.WaitCursor;

				string returnedKey = Encoding.Default.GetString(returnedBytes, 0, length - 1);
				List<string> settingsKeys = new List<string>(returnedKey.Split('\0'));

				foreach ( string settingsKey in settingsKeys )
				{
					if ( !settingsKey.StartsWith(ccMushroom.KEY_REMOTE_PROGRAMS_FOLDER) )
						continue;
					string number = settingsKey.Substring(ccMushroom.KEY_REMOTE_PROGRAMS_FOLDER.Length);
					if ( settingsKeys.IndexOf(ccMushroom.KEY_REMOTE_PROGRAMS_NAME + number) == -1 )
						continue;
					if ( settingsKeys.IndexOf(ccMushroom.KEY_LOCAL_PROGRAMS_FOLDER + number) == -1 )
						continue;
					if ( settingsKeys.IndexOf(ccMushroom.KEY_ENABLED_PROGRAMS_FOLDER + number) == -1 )
						continue;

					StringBuilder returnedString = new StringBuilder(1024);

					// リモート側のプログラム名称
					if ( api.GetPrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_REMOTE_PROGRAMS_NAME + number, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) == 0 )
						continue;
					string remoteProgramsName = returnedString.ToString();

					// リモート側のプログラムを置いているフォルダ名
					if ( api.GetPrivateProfileString(ccMushroom.SETTINGS_SECTION, settingsKey, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) == 0 )
						continue;
					string remoteProgramsFolder = returnedString.ToString();
					if ( !ccf.AdjustFolderFormat(ref remoteProgramsFolder) )
						continue;

					// ローカル側のプログラムをコピーするフォルダ名
					if ( api.GetPrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_LOCAL_PROGRAMS_FOLDER + number, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) == 0 )
						continue;
					string localProgramsFolder = returnedString.ToString();
					localProgramsFolder = ccf.ReplaceCcMushroomLocalFolder(localProgramsFolder, true);
					if ( !ccf.AdjustFolderFormat(ref localProgramsFolder) )
						continue;
					if ( ccf.IsSystemFolder(localProgramsFolder) )
						continue;

					// 有効/無効なプログラムフォルダの設定
					if ( api.GetPrivateProfileString(ccMushroom.SETTINGS_SECTION, ccMushroom.KEY_ENABLED_PROGRAMS_FOLDER + number, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) == 0 )
						continue;
					bool enabledProgramsFolder = bool.Parse(returnedString.ToString());

					int index = lvProgFolderIndexOfItem(fold.name, remoteProgramsName);
					if ( index == -1 )	// 新規？
					{
						AppendProgramsFolder(remoteProgramsName, remoteProgramsFolder, localProgramsFolder, enabledProgramsFolder);
					}
					else
					{
						if ( !overwrite )
							continue;

						EditProgramsFolder(remoteProgramsName, remoteProgramsFolder, localProgramsFolder, enabledProgramsFolder, index);
					}
				
					modifiedCcMushroomIni = true;
				}

				if ( listViewProgramsFolder.SelectedItems.Count != 0 )
				{
					listViewProgramsFolder.SelectedItems[0].Selected = false;
				}

				// 入力領域をクリアする
				ClearProgFolderInputs();

				if ( modifiedCcMushroomIni )
				{
					checkReload.Checked = true;
				}

				Cursor.Current = Cursors.Default;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
		#endregion

		#region ccAppEnvironmentSetting.xml
		/// <summary>
		/// ccAppEnvironmentSetting.xml
		/// アプリケーション環境設定ファイル
		/// </summary>
		private void ShowAppEnvironmentSetting()
		{
			try
			{
				//treeAppEnviron.Nodes.Clear();

				foreach ( XmlNode application in xmlCcAppEnvironmentSetting.DocumentElement.ChildNodes )
				{
					if ( application.Name == ccMushroom.TAG_APP_ENVIRON )	// <appEnviron> ?
					{
						foreach ( XmlNode nodeItem in application.ChildNodes )
						{
							comboAppEnviron.Items.Add(nodeItem.InnerText);
						}
						if ( comboAppEnviron.Items.Count != 0 )
						{
							comboAppEnviron.SelectedIndex = 0;
						}
						continue;
					}

					if ( application.Name != ccMushroom.TAG_APPLICATION )
						continue;

					// <application>
					TreeNode applicationTree = treeAppEnviron.Nodes.Add(application.Attributes[ccMushroom.ATTRIB_NAME].Value);
					applicationTree.Name = application.Attributes[ccMushroom.ATTRIB_NAME].Value;

					AppendApplicationTreeNodes(applicationTree, application);
				}

				//textAppNameO.ReadOnly = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// アプリケーションのツリーに application ノードを追加する
		/// </summary>
		/// <param name="applicationTree"></param>
		/// <param name="application"></param>
		private void AppendApplicationTreeNodes(TreeNode applicationTree, XmlNode application)
		{
			foreach ( XmlNode node in application )
			{
				if ( node.Name == ccMushroom.TAG_APP_NAME )
				{
					//string appName = application[ccMushroom.TAG_APP_NAME].InnerText;
					//applicationTree.Nodes.Add(appName);
				}
				else if ( node.Name == ccMushroom.TAG_APP_SETTING_CONFIG )	// <appSettingConfig> ?
				{
					string appSettingConfig = node.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value + "@" + node.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value;
					TreeNode appSettingConfigTree = applicationTree.Nodes.Add(appSettingConfig);
					appSettingConfigTree.Name = ccMushroom.TAG_APP_SETTING_CONFIG;

					/*if ( node.Attributes[ccMushroom.ATTRIB_COPY_FROM] != null )
					{
						appSettingConfigTree.Nodes.Add("コピー元:" + node.Attributes[ccMushroom.ATTRIB_COPY_FROM].Value);
					}*/

					/*foreach ( XmlNode key in node.SelectNodes(ccMushroom.TAG_KEY) )
					{
						TreeNode keyTree = appSettingConfigTree.Nodes.Add(key.Attributes[ccMushroom.ATTRIB_NAME].Value);
						keyTree.Nodes.Add(key.Attributes[ccMushroom.ATTRIB_VALUE].Value);
					}*/
				}
				else if ( node.Name == ccMushroom.TAG_APP_SETTING_INI )		// <appSettingIni> ?
				{
					string appSettingIni = node.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value + "@" + node.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value;
					TreeNode appSettingIniTree = applicationTree.Nodes.Add(appSettingIni);
					//appSettingIniTree.Name = ccMushroom.TAG_APP_SETTING_INI;

					/*if ( node.Attributes[ccMushroom.ATTRIB_COPY_FROM] != null )
					{
						appSettingIniTree.Nodes.Add("コピー元:" + node.Attributes[ccMushroom.ATTRIB_COPY_FROM].Value);
					}*/

					foreach ( XmlNode section in node.SelectNodes(ccMushroom.TAG_SECTION) )	// <section>
					{
						string sectionName = section.Attributes[ccMushroom.ATTRIB_NAME].InnerText;
						TreeNode sectionTree = appSettingIniTree.Nodes.Add("[" + sectionName + "]");
						sectionTree.Name = ccMushroom.TAG_APP_SETTING_INI;

						/*foreach ( XmlNode key in section.SelectNodes(ccMushroom.TAG_KEY) )
						{
							TreeNode keyTree = sectionTree.Nodes.Add(key.Attributes[ccMushroom.ATTRIB_NAME].Value);
							keyTree.Nodes.Add(key.Attributes[ccMushroom.ATTRIB_VALUE].Value);
						}*/
					}
				}
				else if ( node.Name == ccMushroom.TAG_APP_SETTING_XML )	// <appSettingXml> ?
				{
					string appSettingXml = node.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value + "@" + node.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value;
					TreeNode appSettingConfigTree = applicationTree.Nodes.Add(appSettingXml);
					appSettingConfigTree.Name = ccMushroom.TAG_APP_SETTING_XML;
				}
			}
		}

		/// <summary>
		/// treeAppEnviron でキーが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeAppEnviron_KeyDown(object sender, KeyEventArgs e)
		{
			if ( e.KeyCode == Keys.Delete )
			{
				DeleteApplicationNode();
			}
		}

		/// <summary>
		/// treeAppEnviron のノードが選択された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void treeAppEnviron_AfterSelect(object sender, TreeViewEventArgs e)
		{
			try
			{
				dataGridAppEnviron.Columns.Clear();

				ChangeAeApplyColor(false);

				TreeNode selNode = treeAppEnviron.SelectedNode;

				if ( selNode.Level == 0 )	// アプリケーションのルート？
				{
					string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + selNode.Text + "']";
					XmlNode application = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
					if ( application == null )
						return;
					SetApplicationText(application);

					DataGridViewTextBoxColumn colFileName = new DataGridViewTextBoxColumn();
					colFileName.HeaderText = "ファイル名";
					colFileName.Name = "fileName";
					colFileName.SortMode = DataGridViewColumnSortMode.NotSortable;
#if false
					DataGridViewTextBoxColumn colEnvironment = new DataGridViewTextBoxColumn();
#else
					DataGridViewColumn colEnvironment;
					if ( comboAppEnviron.Items.Count == 0 )
					{
						colEnvironment = new DataGridViewTextBoxColumn();
					}
					else
					{
						colEnvironment = new DataGridViewComboBoxColumn();
						//colEnvironment.Items.AddRange(comboAppEnviron.Items);
						((DataGridViewComboBoxColumn)colEnvironment).DataSource = comboAppEnviron.Items;
						((DataGridViewComboBoxColumn)colEnvironment).DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
						((DataGridViewComboBoxColumn)colEnvironment).DisplayStyleForCurrentCellOnly = true;
					}
#endif
					colEnvironment.HeaderText = "環境";
					colEnvironment.Name = "environment";
					colEnvironment.Width = 70;
					colEnvironment.SortMode = DataGridViewColumnSortMode.NotSortable;
					DataGridViewTextBoxColumn colCopyFrom = new DataGridViewTextBoxColumn();
					colCopyFrom.HeaderText = "コピー元";
					colCopyFrom.Name = "copyFrom";
					colCopyFrom.SortMode = DataGridViewColumnSortMode.NotSortable;
					DataGridViewCheckBoxColumn colDel = new DataGridViewCheckBoxColumn();
					colDel.HeaderText = "削除";
					colDel.Name = "del";
					colDel.Width = 40;
					colDel.SortMode = DataGridViewColumnSortMode.NotSortable;
					dataGridAppEnviron.Columns.AddRange(new DataGridViewColumn[] { colFileName, colEnvironment, colCopyFrom, colDel });

					foreach ( XmlNode appSetting in application.ChildNodes )
					{
						if ( !appSetting.Name.StartsWith("appSetting") )
							continue;

						string fileName = appSetting.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value;
						string environment = appSetting.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value;
						string copyFrom = string.Empty;
						if ( appSetting.Attributes[ccMushroom.ATTRIB_COPY_FROM] != null )
						{
							copyFrom = appSetting.Attributes[ccMushroom.ATTRIB_COPY_FROM].Value;
						}

						if ( comboAppEnviron.Items.IndexOf(environment) == -1 )
						{
							comboAppEnviron.Items.Add(environment);	// comboBox にない値を入れると、DataError イベントが発生するので、その対策
							if ( comboAppEnviron.SelectedItem == null )
							{
								comboAppEnviron.SelectedItem = environment;
							}
						}

						//dataGridViewO.Rows.Add(new Object[] { fileName, environment, copyFrom*/ });
						int i = dataGridAppEnviron.Rows.Add();
						dataGridAppEnviron.Rows[i].Cells[(int)appset.fname].Value = fileName;
						dataGridAppEnviron.Rows[i].Cells[(int)appset.environ].Value = environment;
						dataGridAppEnviron.Rows[i].Cells[(int)appset.cfile].Value = copyFrom;
						dataGridAppEnviron.Rows[i].Cells[(int)appset.cfile + 1].Tag = "*";	// 既存の印
					}

					radioAeCopy.Enabled = true;
					treeAppEnviron.Select();
					return;
				}

				radioAeCopy.Enabled = false;

				if ( selNode.Level == 1 )
				{
					string[] appSettingAttrib = selNode.Text.Split('@');
					if ( string.Compare(Path.GetExtension(appSettingAttrib[(int)appset.fname]), ".ini", true) == 0 )	// <section> ?
					{
						string appName = selNode.Parent.Text;
						/*string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + appName + "']" +
									   "/" + ccMushroom.TAG_APP_SETTING_INI + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + appSettingConfigAttrib[(int)appset.fname] + "' and @" + ccMushroom.ATTRIB_ENVIRONMENT + "='" + appSettingConfigAttrib[(int)appset.environ] + "']";
						XmlNode appSettingIni = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);*/
						string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + appName + "']";
						XmlNode application = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
						if ( application == null )
							return;
						SetApplicationText(application);
						xpath = ccMushroom.TAG_APP_SETTING_INI + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + appSettingAttrib[(int)appset.fname] + "' and @" + ccMushroom.ATTRIB_ENVIRONMENT + "='" + appSettingAttrib[(int)appset.environ] + "']";
						XmlNode appSettingIni = application.SelectSingleNode(xpath);
						if ( appSettingIni == null )
							return;

						DataGridViewTextBoxColumn colSection = new DataGridViewTextBoxColumn();
						colSection.HeaderText = "セクション";
						colSection.Name = "section";
						colSection.SortMode = DataGridViewColumnSortMode.NotSortable;
						DataGridViewCheckBoxColumn colDel = new DataGridViewCheckBoxColumn();
						colDel.HeaderText = "削除";
						colDel.Name = "del";
						colDel.Width = 40;
						colDel.SortMode = DataGridViewColumnSortMode.NotSortable;
						dataGridAppEnviron.Columns.AddRange(new DataGridViewColumn[] { colSection, colDel });

						foreach ( XmlNode section in appSettingIni.SelectNodes(ccMushroom.TAG_SECTION) )
						{
							int i = dataGridAppEnviron.Rows.Add(new Object[] { section.Attributes[ccMushroom.ATTRIB_NAME].Value });
							dataGridAppEnviron.Rows[i].Cells[1].Tag = "*";	// 既存の印
						}
						return;
					}
				}

				if ( selNode.Name.StartsWith("appSetting") && (selNode.Name != ccMushroom.TAG_APP_SETTING_XML) )
				{
					DataGridViewTextBoxColumn colKey = new DataGridViewTextBoxColumn();
					colKey.HeaderText = "キー";
					colKey.Name = "key";
					colKey.SortMode = DataGridViewColumnSortMode.NotSortable;
					DataGridViewTextBoxColumn colValue = new DataGridViewTextBoxColumn();
					colValue.HeaderText = "値";
					colValue.Name = "value";
					colValue.SortMode = DataGridViewColumnSortMode.NotSortable;
					DataGridViewCheckBoxColumn colDel = new DataGridViewCheckBoxColumn();
					colDel.HeaderText = "削除";
					colDel.Name = "del";
					colDel.Width = 40;
					colDel.SortMode = DataGridViewColumnSortMode.NotSortable;
					dataGridAppEnviron.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colKey, colValue, colDel });

					if ( selNode.Name == ccMushroom.TAG_APP_SETTING_CONFIG )
					{
						ShowKeyNode(selNode.Parent.Text, ccMushroom.TAG_APP_SETTING_CONFIG, selNode.Text.Split('@'), null);
					}
					else if ( selNode.Name == ccMushroom.TAG_APP_SETTING_INI )
					{
						ShowKeyNode(selNode.Parent.Parent.Text, ccMushroom.TAG_APP_SETTING_INI, selNode.Parent.Text.Split('@'), selNode.Text.Substring(1, selNode.Text.Length - 2));
					}
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				radioAeNew.Checked = false;
				radioAeEdit.Enabled = true;
				radioAeEdit.Checked = true;
			}
		}

		/// <summary>
		/// key ノードを表示する
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="appSettingName"></param>
		/// <param name="appSettingAttrib"></param>
		/// <param name="sectionName"></param>
		private void ShowKeyNode(string appName, string appSettingName, string[] appSettingAttrib, string sectionName)
		{
			string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + appName + "']";
			XmlNode application = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
			if ( application == null )
				return;

			SetApplicationText(application);

			xpath = appSettingName + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + appSettingAttrib[0] + "' and @" + ccMushroom.ATTRIB_ENVIRONMENT + "='" + appSettingAttrib[1] + "']" +
					((sectionName == null) ? "" : "/" + ccMushroom.TAG_SECTION + "[@" + ccMushroom.ATTRIB_NAME + "='" + sectionName + "']");
			XmlNode appSetting = application.SelectSingleNode(xpath);

			foreach ( XmlNode key in appSetting.SelectNodes(ccMushroom.TAG_KEY) )
			{
				string name = key.Attributes[ccMushroom.ATTRIB_NAME].Value;
				string value = key.Attributes[ccMushroom.ATTRIB_VALUE].Value;
				int i = dataGridAppEnviron.Rows.Add(new Object[] { name, value });
				dataGridAppEnviron.Rows[i].Cells[2].Tag = "*";	// 既存の印
			}
		}

		/// <summary>
		/// application ノードをテキストボックスに入れる
		/// </summary>
		/// <param name="application"></param>
		private void SetApplicationText(XmlNode application)
		{
			nowSettingAeText = true;

			checkAeEnabled.Checked = bool.Parse(application.Attributes[ccMushroom.ATTRIB_ENABLED].Value);

			textAeAppName.Text = application.Attributes[ccMushroom.ATTRIB_NAME].Value;
			textAeAppName.Select(textAeAppName.Text.Length, 0);

			textAeAppFileName.Text = application[ccMushroom.TAG_APP_NAME].InnerText;
			textAeAppFileName.Select(textAeAppFileName.Text.Length, 0);

			textAeAppComment.Text = string.Empty;
			if ( application.FirstChild.NodeType == XmlNodeType.Comment )
			{
				textAeAppComment.Text = application.FirstChild.InnerText;
				textAeAppComment.Select(textAeAppComment.Text.Length, 0);
			}

			nowSettingAeText = false;
		}

		/// <summary>
		/// [環境名] コンボボックスにフォーカスが移った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAppEnviron_Enter(object sender, EventArgs e)
		{
			comboAppEnvironText = comboAppEnviron.Text;
			this.AcceptButton = buttonAeApply;
		}

		/// <summary>
		/// [環境名] コンボボックスのフォーカスが失われた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAppEnviron_Leave(object sender, EventArgs e)
		{
			try
			{
				if ( comboAppEnviron.Text.Length == 0 )	// アイテムを削除する？
				{
					if ( comboAppEnvironText != null )
					{
						if ( comboAppEnviron.Items.IndexOf(comboAppEnvironText) != -1 )
						{
							comboAppEnviron.Items.Remove(comboAppEnvironText);
							if ( comboAppEnviron.Items.Count != 0 )
							{
								comboAppEnviron.SelectedIndex = 0;
							}
						}
					}
				}
				else
				{
					if ( comboAppEnviron.Items.IndexOf(comboAppEnviron.Text) == -1 )
					{
						comboAppEnviron.Items.Add(comboAppEnviron.Text);	// 新しい環境を追加する
					}
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// [環境名] コンボボックスの選択が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAppEnviron_SelectedIndexChanged(object sender, EventArgs e)
		{
			comboAppEnvironText = comboAppEnviron.Text;
		}

		/// <summary>
		/// [アプリケーション] がチェックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkAeEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if ( nowSettingAeText || treeAppEnviron.SelectedNode == null )
				return;

			ChangeAeApplyColor(true);
		}

		/// <summary>
		/// テキストボックスがフォーカスを得た
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textAppEnviron_Enter(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			this.AcceptButton = buttonAeApply;
		}

		/// <summary>
		/// テキストボックスがフォーカスを失った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textAppEnviron_leave(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			if ( ((TextBox)sender).Modified )
			{
				ChangeAeApplyColor(true);
			}

			this.AcceptButton = buttonOk;
		}

		/// <summary>
		/// [新規] ラジオボタンの状態が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioAeNew_CheckedChanged(object sender, EventArgs e)
		{
			if ( radioAeNew.Checked )
			{
				//treeAppEnviron.SelectedNode = null;
				//textAeAppName.Text = string.Empty;
				//textAeAppFileName.Text = string.Empty;
				//textAeAppComment.Text = string.Empty;
				checkAeEnabled.Checked = true;
				dataGridAppEnviron.Columns.Clear();
				ChangeAeApplyColor(false);

			}

			//textAeAppName.ReadOnly = !radioAeNew.Checked;
		}

		/// <summary>
		/// [複写] ラジオボタンの状態が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioAeCopy_CheckedChanged(object sender, EventArgs e)
		{
			if ( radioAeCopy.Checked )
			{
				if ( !textAeAppName.Text.EndsWith(" ～コピー") )
				{
					textAeAppName.Text += " ～コピー";
				}

				ChangeAeApplyColor(true);
			}
			else
			{
				if ( textAeAppName.Text.EndsWith(" ～コピー") )
				{
					textAeAppName.Text = textAeAppName.Text.Substring(0, textAeAppName.Text.Length - 5);
				}
			}
		}
		
		/// <summary>
		/// dataGridAppEnviron がフォーカスを得た
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridAppEnviron_Enter(object sender, EventArgs e)
		{
			this.AcceptButton = buttonAeApply;
			this.CancelButton = null;
		}

		/// <summary>
		/// dataGridAppEnviron フォーカスを失った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridAppEnviron_Leave(object sender, EventArgs e)
		{
			this.AcceptButton = buttonOk;
			this.CancelButton = buttonCancel;
		}

		/// <summary>
		/// dataGridAppEnviron セルの内容がクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridAppEnviron_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				if ( e.RowIndex == -1 )
					return;

				if ( e.ColumnIndex == dataGridAppEnviron.ColumnCount - 1 )	// 削除セルがクリックされた？
				{
					DataGridViewCell cell = dataGridAppEnviron.Rows[e.RowIndex].Cells[e.ColumnIndex];
					if ( cell.Tag == null )	// ユーザーが追加した行？
					{
						if ( (cell.Value == null) || (bool)cell.Value )
						{
							dataGridAppEnviron.Rows.RemoveAt(e.RowIndex);
							dataGridAppEnviron.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
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
		/// dataGridAppEnviron セルの編集が開始した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridAppEnviron_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			try
			{
				editCell = (dataGridAppEnviron[e.ColumnIndex, e.RowIndex].Value ?? string.Empty).ToString();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// dataGridAppEnviron セルの編集が終了した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dataGridAppEnviron_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			try
			{
				DataGridViewCell cell = dataGridAppEnviron.Rows[e.RowIndex].Cells[e.ColumnIndex];
				if ( cell.Value == null )
					return;

				if ( dataGridAppEnviron.Columns[e.ColumnIndex].Name == "fileName" )
				{
					string fileName = (string)cell.Value;
					if ( fileName.Length != 0 )
					{
						string ext = Path.GetExtension(fileName);
						if ( (string.Compare(ext, ".config", true) != 0) && (string.Compare(ext, ".ini", true) != 0) && (string.Compare(ext, ".xml", true) != 0) )
						{
							cell.Value = fileName + ".ini";
						}
					}
				}

				if ( cell.Value.ToString() != editCell )
				{
					ChangeAeApplyColor(true);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// checkAppEnviron_CheckedChanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkAppEnviron_CheckedChanged(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			modifiedCcAppEnvironmentSettingXml = true;
			checkReload.Checked = true;
		}

		/// <summary>
		/// [適用] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAeApply_Click(object sender, EventArgs e)
		{
			try
			{
				this.AcceptButton = buttonOk;

				if ( radioAeNew.Checked )
				{
					AppendApplicationNode();
				}
				else if ( radioAeEdit.Checked )
				{
					EditAppEnvironment();
				}
				else if ( radioAeCopy.Checked )
				{
					CopyApplicationNode();
				}

				treeAppEnviron.Select();

				modifiedCcAppEnvironmentSettingXml = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				radioAeNew.Checked = false;
				radioAeCopy.Checked = false;
			}
		}

		/// <summary>
		/// [適用] ボタンの色を変更する
		/// </summary>
		/// <param name="init"></param>
		private void ChangeAeApplyColor(bool modified)
		{
			buttonAeApply.ForeColor = (!modified) ? Color.Black : Color.White/*Color.Crimson*/;
			buttonAeApply.BackColor = (!modified) ? SystemColors.ControlLight : Color.Red/*Color.LavenderBlush*/;
		}

		/// <summary>
		/// application ノードを新規に追加する
		/// </summary>
		private void AppendApplicationNode()
		{
			if ( (textAeAppName.Text.Length == 0) || (textAeAppFileName.Text.Length == 0) )
				return;

			/*string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + textAeAppName.Text + "']";
			if ( xmlCcAppEnvironmentSetting.SelectSingleNode(xpath) != null )	// 既に登録済み？
				return;*/
			int _index = treeAppEnviron.Nodes.IndexOfKey(textAeAppName.Text);
			if ( _index != -1 )	// 既に登録済み？
			{
				treeAppEnviron.SelectedNode = treeAppEnviron.Nodes[_index];
				return;
			}

			// application ノードを追加する
			XmlNode application = xmlCcAppEnvironmentSetting.CreateNode(XmlNodeType.Element, ccMushroom.TAG_APPLICATION, null);	// <application>
			XmlAttribute attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_NAME);							// @name
			attrib.Value = textAeAppName.Text;
			application.Attributes.Append(attrib);
			attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_ENABLED);										// @enabled
			attrib.Value = checkAeEnabled.Checked.ToString().ToLower();
			application.Attributes.Append(attrib);

			XmlComment comment = xmlCcAppEnvironmentSetting.CreateComment(textAeAppComment.Text);	// <!--コメント-->
			application.AppendChild(comment);

			XmlElement elem = xmlCcAppEnvironmentSetting.CreateElement(ccMushroom.TAG_APP_NAME);	// <appName>
			elem.InnerText = textAeAppFileName.Text;
			application.AppendChild(elem);

			xmlCcAppEnvironmentSetting.DocumentElement.AppendChild(application);

			// treeAppEnviron に追加する
			TreeNode applicationTree = treeAppEnviron.Nodes.Add(application.Attributes[ccMushroom.ATTRIB_NAME].Value);
			applicationTree.Name = application.Attributes[ccMushroom.ATTRIB_NAME].Value;
			treeAppEnviron.SelectedNode = applicationTree;
		}

		/// <summary>
		/// 各環境ノードを編集する
		/// </summary>
		private void EditAppEnvironment()
		{
			if ( textAeAppName.Text.Length == 0 || textAeAppFileName.Text.Length == 0 )
				return;
			
			if ( treeAppEnviron.SelectedNode.Level == 0 )	// アプリケーションのルート？
			{
				int _index = treeAppEnviron.Nodes.IndexOfKey(textAeAppName.Text);
				if ( (_index != -1) && (_index != treeAppEnviron.SelectedNode.Index) )	// 既に登録済み？
				{
					treeAppEnviron.SelectedNode = treeAppEnviron.Nodes[_index];
					return;
				}

				string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + treeAppEnviron.SelectedNode.Text + "']";
				XmlNode application = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);		// <application>
				if ( application == null )
					return;

				application.Attributes[ccMushroom.ATTRIB_ENABLED].Value = checkAeEnabled.Checked.ToString().ToLower();	// @enabled
				application.Attributes[ccMushroom.ATTRIB_NAME].Value = textAeAppName.Text;	// @name
				application.FirstChild.InnerText = textAeAppComment.Text;					// <!--コメント-->
				application[ccMushroom.TAG_APP_NAME].InnerText = textAeAppFileName.Text;	// <appName>

				TreeNode applicationTree = treeAppEnviron.SelectedNode;
				applicationTree.Text = application.Attributes[ccMushroom.ATTRIB_NAME].Value;
				applicationTree.Name = application.Attributes[ccMushroom.ATTRIB_NAME].Value;

				// appSettingConfig|appSettingIni
				XmlNodeList appSettingList = application.SelectNodes(ccMushroom.TAG_APP_SETTING_CONFIG + "|" + ccMushroom.TAG_APP_SETTING_INI + "|" + ccMushroom.TAG_APP_SETTING_XML);
				int iAppSetting = 0;

				List<int> delRow = new List<int>();

				foreach ( DataGridViewRow row in dataGridAppEnviron.Rows )
				{
					if ( row.IsNewRow )
						break;

					string fileName = (string)row.Cells[(int)appset.fname].Value;
					string environment = (row.Cells[(int)appset.environ].Value == null) ? comboAppEnviron.Text : (string)row.Cells[(int)appset.environ].Value;
					string copyFile = (string)row.Cells[(int)appset.cfile].Value;
					bool del = (row.Cells[(int)appset.cfile + 1].Value == null) ? false : (bool)row.Cells[(int)appset.cfile + 1].Value;
					string appSettingName = null;
					if ( string.Compare(Path.GetExtension(fileName), ".config", true) == 0 )
					{
						appSettingName = ccMushroom.TAG_APP_SETTING_CONFIG;
					}
					else if ( string.Compare(Path.GetExtension(fileName), ".ini", true) == 0 )
					{
						appSettingName = ccMushroom.TAG_APP_SETTING_INI;
					}
					else if ( string.Compare(Path.GetExtension(fileName), ".xml", true) == 0 )
					{
						appSettingName = ccMushroom.TAG_APP_SETTING_XML;
					}
					/*else
					{
						MessageBox.Show("サポートされていない拡張子です", MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}*/

					if ( appSettingName != null )
					{
						EditAppSettingNode(ref application, ref appSettingList, iAppSetting, appSettingName, fileName, environment, copyFile, del, ref delRow);
					}

					iAppSetting++;
				}

				// 行を削除する
				foreach ( int index in delRow )
				{
					dataGridAppEnviron.Rows.RemoveAt(index);
					treeAppEnviron.SelectedNode.Nodes.RemoveAt(index);
					application.RemoveChild(appSettingList[index]);
				}
			}
			else if ( treeAppEnviron.SelectedNode.Level == 1 )
			{
				string[] appSettingAttrib = treeAppEnviron.SelectedNode.Text.Split('@');
				if ( string.Compare(Path.GetExtension(appSettingAttrib[(int)appset.fname]), ".ini", true) == 0 )	// <section> ?
				{
					string appName = treeAppEnviron.SelectedNode.Parent.Text;
					EditSectionNode(appName, appSettingAttrib);
				}
				else
				{
					string appName = treeAppEnviron.SelectedNode.Parent.Text;
					EditKeyNode(appName, ccMushroom.TAG_APP_SETTING_CONFIG, appSettingAttrib, null);
				}
			}
			else if ( treeAppEnviron.SelectedNode.Level == 2 )
			{
				string appName = treeAppEnviron.SelectedNode.Parent.Parent.Text;
				string[] appSettingAttrib = treeAppEnviron.SelectedNode.Parent.Text.Split('@');
				string sectionName = treeAppEnviron.SelectedNode.Text.Substring(1, treeAppEnviron.SelectedNode.Text.Length - 2);
				EditKeyNode(appName, ccMushroom.TAG_APP_SETTING_INI, appSettingAttrib, sectionName);
			}

			try
			{
				// dataGridAppEnviron をリフレッシュする
				TreeNode selNode = treeAppEnviron.SelectedNode;
				int rowIndex = dataGridAppEnviron.CurrentCell.RowIndex;
				int columnIndex = dataGridAppEnviron.CurrentCell.ColumnIndex;

				treeAppEnviron.SelectedNode = null;
				treeAppEnviron.SelectedNode = selNode;
				Application.DoEvents();

				rowIndex = Math.Min(rowIndex, dataGridAppEnviron.Rows.Count - 1);
				dataGridAppEnviron.Rows[rowIndex].Cells[columnIndex].Selected = true;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 現在選択されている application ノードをコピーする
		/// </summary>
		private void CopyApplicationNode()
		{
			if ( (textAeAppName.Text.Length == 0) || (textAeAppFileName.Text.Length == 0) )
				return;
			
			/*string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + textAeAppName.Text + "']";
			if ( xmlCcAppEnvironmentSetting.SelectSingleNode(xpath) != null )	// 既に登録済み？
				return;*/
			int _index = treeAppEnviron.Nodes.IndexOfKey(textAeAppName.Text);
			if ( _index != -1 )	// 既に登録済み？
			{
				treeAppEnviron.SelectedNode = treeAppEnviron.Nodes[_index];
				return;
			}

			string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + treeAppEnviron.SelectedNode.Text + "']";
			XmlNode application = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath).Clone();	// <application> のクローン

			application.Attributes[ccMushroom.ATTRIB_NAME].Value = textAeAppName.Text;	// @name
			application.FirstChild.InnerText = textAeAppComment.Text;					// <!--コメント-->
			application[ccMushroom.TAG_APP_NAME].InnerText = textAeAppFileName.Text;	// <appName>

			xmlCcAppEnvironmentSetting.DocumentElement.AppendChild(application);

			// treeAppEnviron に追加する
			TreeNode applicationTree = treeAppEnviron.Nodes.Add(application.Attributes[ccMushroom.ATTRIB_NAME].Value);
			applicationTree.Name = application.Attributes[ccMushroom.ATTRIB_NAME].Value;
			foreach ( TreeNode node in treeAppEnviron.SelectedNode.Nodes )
			{
				TreeNode clone = (TreeNode)node.Clone();
				applicationTree.Nodes.Add(clone);
			}

			treeAppEnviron.SelectedNode = applicationTree;
		}

		/// <summary>
		/// 現在選択されている application ノードを削除する
		/// </summary>
		private void DeleteApplicationNode()
		{
			try
			{
				if ( (treeAppEnviron.SelectedNode == null) || (treeAppEnviron.SelectedNode.Level != 0) )
					return;

				string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + treeAppEnviron.SelectedNode.Text + "']";
				XmlNode application = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);	// <application>

				xmlCcAppEnvironmentSetting.DocumentElement.RemoveChild(application);

				treeAppEnviron.SelectedNode.Remove();

				InitAppEnvironForm();

				TreeNode selectedNode = treeAppEnviron.SelectedNode;
				if ( selectedNode != null )
				{
					treeAppEnviron.SelectedNode = null;
					treeAppEnviron.Update();
					// treeAppEnviron_AfterSelect 状態にする
					treeAppEnviron.SelectedNode = selectedNode;
				}

				modifiedCcAppEnvironmentSettingXml = true;
				checkReload.Checked = true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// アプリ環境フォームを初期化する
		/// </summary>
		private void InitAppEnvironForm()
		{
			checkAeEnabled.Checked = false;
			textAeAppName.Text = string.Empty;
			textAeAppFileName.Text = string.Empty;
			textAeAppComment.Text = string.Empty;
			dataGridAppEnviron.Columns.Clear();

			radioAeNew.Checked = false;
			radioAeEdit.Checked = false;
			radioAeEdit.Enabled = false;
			radioAeCopy.Checked = false;
			radioAeCopy.Enabled = false;
		}

		/// <summary>
		/// appSettingConfig|appSettingIni ノードを編集する
		/// </summary>
		/// <param name="application"></param>
		/// <param name="appSettingList"></param>
		/// <param name="iAppSetting"></param>
		/// <param name="appSettingName"></param>
		/// <param name="fileName"></param>
		/// <param name="environment"></param>
		/// <param name="copyFile"></param>
		/// <param name="del"></param>
		/// <param name="delRow"></param>
		private void EditAppSettingNode(ref XmlNode application, ref XmlNodeList appSettingList, int iAppSetting, string appSettingName, string fileName, string environment, string copyFile, bool del, ref List<int> delRow)
		{
			if ( string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(environment) )
				return;

			if ( (appSettingList == null) || (appSettingList.Count <= iAppSetting) )	// 新規の appSettingConfig|appSettingIni ?
			{
				string xpath = appSettingName + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + fileName + "' and @" + ccMushroom.ATTRIB_ENVIRONMENT + "='" + environment + "']";
				if ( application.SelectSingleNode(xpath) != null )	// 既に登録済み？
					return;

				XmlNode appSetting = xmlCcAppEnvironmentSetting.CreateElement(appSettingName);					// <appSettingConfig>|<appSettingIni>

				XmlAttribute attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_FILE_NAME);	// @fileName
				attrib.Value = fileName;
				appSetting.Attributes.Append(attrib);

				attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_ENVIRONMENT);				// @environment
				attrib.Value = environment;
				appSetting.Attributes.Append(attrib);

				if ( !string.IsNullOrEmpty(copyFile) )
				{
					attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_COPY_FROM);			// @copyFrom
					attrib.Value = copyFile;
					appSetting.Attributes.Append(attrib);
				}
				application.AppendChild(appSetting);

				TreeNode appSettingTree = treeAppEnviron.SelectedNode.Nodes.Add(appSetting.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value + "@" + appSetting.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value);
				appSettingTree.Name = appSettingName;

				appSettingList = application.SelectNodes(appSettingName);
			}
			else
			{
				if ( del )
				{
					delRow.Insert(0, iAppSetting);
				}
				else
				{
					/*string[] appSettingConfigAttrib = treeAppEnviron.SelectedNode.Nodes[iRow].Text.Split('@');
					xpath = ccMushroom.TAG_APP_SETTING_CONFIG + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + appSettingConfigAttrib[(int)appset.fname] + "' and @" + ccMushroom.ATTRIB_OENVIRONMENT + "='" + appSettingConfigAttrib[(int)appset.environ] + "']";
					XmlNode appSettingConfig = application.SelectSingleNode(xpath);
					if ( appSettingConfig != null ) { }*/
					//XmlNode appSettingConfig = appSettingConfigList[iConfig];
					XmlNode appSetting = appSettingList[iAppSetting];

					appSetting.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value = fileName;				// @fileName

					appSetting.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value = environment;			// @environment

					XmlAttribute attrib;																// @copyFile
					if ( string.IsNullOrEmpty(copyFile) )
					{
						if ( (attrib = appSetting.Attributes[ccMushroom.ATTRIB_COPY_FROM]) != null )
						{
							appSetting.Attributes.Remove(attrib);
						}
					}
					else
					{
						if ( (attrib = appSetting.Attributes[ccMushroom.ATTRIB_COPY_FROM]) == null )
						{
							attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_COPY_FROM);
						}
						attrib.Value = copyFile;
						appSetting.Attributes.Append(attrib);
					}

					treeAppEnviron.SelectedNode.Nodes[iAppSetting].Text = appSetting.Attributes[ccMushroom.ATTRIB_FILE_NAME].Value + "@" + appSetting.Attributes[ccMushroom.ATTRIB_ENVIRONMENT].Value;
				}
			}
		}

		/// <summary>
		/// section ノードを編集する
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="appSettingAttrib"></param>
		private void EditSectionNode(string appName, string[] appSettingAttrib)
		{
			string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + appName + "']" +
						   "/" + ccMushroom.TAG_APP_SETTING_INI + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + appSettingAttrib[(int)appset.fname] + "' and @" + ccMushroom.ATTRIB_ENVIRONMENT + "='" + appSettingAttrib[(int)appset.environ] + "']";
			XmlNode appSettingIni = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
			if ( appSettingIni == null )
				return;

			// section
			XmlNodeList sectionList = appSettingIni.SelectNodes(ccMushroom.TAG_SECTION);
			int iSection = 0;

			List<int> delRow = new List<int>();	// 削除する行番号

			foreach ( DataGridViewRow row in dataGridAppEnviron.Rows )
			{
				if ( row.IsNewRow )
					break;

				string sectionName = (string)row.Cells[0].Value;
				bool del = (row.Cells[1].Value == null) ? false : (bool)row.Cells[1].Value;
				if ( string.IsNullOrEmpty(sectionName) )
					continue;

				if ( (sectionList == null) || (sectionList.Count <= iSection) )	// 新規の section ?
				{
					xpath = ccMushroom.TAG_SECTION + "[@" + ccMushroom.ATTRIB_NAME + "='" + sectionName + "']";
					if ( appSettingIni.SelectSingleNode(xpath) == null )
					{
						// section ノードを作成する
						XmlNode section = xmlCcAppEnvironmentSetting.CreateElement(ccMushroom.TAG_SECTION);			// <section>
						XmlAttribute attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_NAME);	// @name
						attrib.Value = sectionName;
						section.Attributes.Append(attrib);
						appSettingIni.AppendChild(section);

						sectionList = appSettingIni.SelectNodes(ccMushroom.TAG_SECTION);

						TreeNode sectionTree = treeAppEnviron.SelectedNode.Nodes.Add("[" + sectionName + "]");
						sectionTree.Name = ccMushroom.TAG_APP_SETTING_INI;
					}
				}
				else
				{
					if ( del )
					{
						delRow.Insert(0, iSection);
					}
					else
					{
						XmlNode section = sectionList[iSection];

						section.Attributes[ccMushroom.ATTRIB_NAME].Value = sectionName;									// @name

						treeAppEnviron.SelectedNode.Nodes[iSection].Text = "[" + sectionName + "]";
					}
				}

				iSection++;
			}

			// 行を削除する
			foreach ( int index in delRow )
			{
				dataGridAppEnviron.Rows.RemoveAt(index);
				treeAppEnviron.SelectedNode.Nodes.RemoveAt(index);
				appSettingIni.RemoveChild(sectionList[index]);
			}
		}

		/// <summary>
		/// key ノードを編集する
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="appSettingName"></param>
		/// <param name="appSettingAttrib"></param>
		/// <param name="section"></param>
		private void EditKeyNode(string appName, string appSettingName, string [] appSettingAttrib, string sectionName)
		{
			string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + appName + "']" +
						   "/" + appSettingName + "[@" + ccMushroom.ATTRIB_FILE_NAME + "='" + appSettingAttrib[(int)appset.fname] + "' and @" + ccMushroom.ATTRIB_ENVIRONMENT + "='" + appSettingAttrib[(int)appset.environ] + "']" +
						   ((sectionName == null) ? "" : "/" + ccMushroom.TAG_SECTION + "[@" + ccMushroom.ATTRIB_NAME + "='" + sectionName + "']");
			XmlNode appSetting = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
			if ( appSetting == null )
				return;

			// key
			XmlNodeList keyList = appSetting.SelectNodes(ccMushroom.TAG_KEY);
			int iKey = 0;

			List<int> delRow = new List<int>();	// 削除する行番号

			foreach ( DataGridViewRow row in dataGridAppEnviron.Rows )
			{
				if ( row.IsNewRow )
					break;

				string keyName = (string)row.Cells[0].Value;
				string value = (string)row.Cells[1].Value;
				bool del = (row.Cells[2].Value == null) ? false : (bool)row.Cells[2].Value;
				if ( string.IsNullOrEmpty(keyName) || string.IsNullOrEmpty(value) )
					continue;

				if ( (keyList == null) || (keyList.Count <= iKey) )	// 新規の key ?
				{
					xpath = ccMushroom.TAG_KEY + "[@" + ccMushroom.ATTRIB_NAME + "='" + keyName + "']";
					if ( appSetting.SelectSingleNode(xpath) == null )
					{
						// key ノードを作成する
						XmlNode key = xmlCcAppEnvironmentSetting.CreateElement(ccMushroom.TAG_KEY);					// <key>
						XmlAttribute attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_NAME);	// @name
						attrib.Value = keyName;
						key.Attributes.Append(attrib);
						attrib = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_VALUE);				// @value
						attrib.Value = value;
						key.Attributes.Append(attrib);
						appSetting.AppendChild(key);

						keyList = appSetting.SelectNodes(ccMushroom.TAG_KEY);
					}
				}
				else
				{
					if ( del )
					{
						delRow.Insert(0, iKey);
					}
					else
					{
						XmlNode key = keyList[iKey];

						key.Attributes[ccMushroom.ATTRIB_NAME].Value = keyName;											// @name
						key.Attributes[ccMushroom.ATTRIB_VALUE].Value = value;											// @value
					}
				}

				iKey++;
			}

			// 行を削除する
			foreach ( int index in delRow )
			{
				dataGridAppEnviron.Rows.RemoveAt(index);
				appSetting.RemoveChild(keyList[index]);
			}
		}

		/// <summary>
		/// [アプリケーションの一覧] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonSelApp_Click(object sender, EventArgs e)
		{
			contextMenuApps.Show(Cursor.Position);
		}

		/// <summary>
		/// contextMenuStrip が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			try
			{
				if ( (contextMenuApps.Items.Count == 1) && (contextMenuApps.Items[0] == toolStripSeparator) )
				{
					contextMenuApps.Items.RemoveAt(0);	// ダミーのセパレータを削除する。ダミーを入れておかないと、最初のクリックでメニューが表示されない
				}

				TabPage[] appTabPages = ((ccMushroom)this.Owner).AppTabPages;
				ImageList appTabIcons = ((ccMushroom)this.Owner).AppTabIcons;

				ClearContextMenuApps();

				// タブ名をサブメニューとして追加する
				contextMenuTabPages = new ToolStripMenuItem[appTabPages.Length];
				for ( int i = 0; i < appTabPages.Length; i++ )
				{
					contextMenuTabPages[i] = new ToolStripMenuItem();
					contextMenuTabPages[i].Name = appTabPages[i].Text;
					contextMenuTabPages[i].Text = appTabPages[i].Text;
					contextMenuTabPages[i].Tag = i;
					int index = appTabIcons.Images.IndexOfKey(appTabPages[i].ImageKey/*tabControl.TabPages[i].Name*/);
					if ( index != -1 )
					{
						contextMenuTabPages[i].Image = appTabIcons.Images[index];
					}
					contextMenuTabPages[i].DropDownItems.Add(new ToolStripMenuItem());	// ドロップダウンの▲マークを出す為に、ダミー メニューを追加しておく
					contextMenuTabPages[i].DropDownOpening += new EventHandler(contextMenuTabPage_Opening);
					contextMenuApps.Items.Insert(0, contextMenuTabPages[i]);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// contextMenuButtons のサブメニューをクリアする
		/// </summary>
		private void ClearContextMenuApps()
		{
			if ( contextMenuButtons != null )
			{
				for ( int i = 0; i < contextMenuButtons.Length; i++ )
				{
					if ( contextMenuButtons[i] != null )
					{
						contextMenuButtons[i].Dispose();
					}
				}

				contextMenuButtons = null;
			}

			if ( contextMenuTabPages != null )
			{
				for ( int i = 0; i < contextMenuTabPages.Length; i++ )
				{
					contextMenuTabPages[i].DropDownItems.Clear();
					contextMenuTabPages[i].Dispose();
					//contextMenuTabPages.Items.RemoveAt(2 + i);
				}

				contextMenuTabPages = null;
			}
		}

		/// <summary>
		/// notifyMenuTabPage が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuTabPage_Opening(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				ccMushroom ccMushroom = (ccMushroom)this.Owner;
				TabPage[] appTabPages = ccMushroom.AppTabPages;
				Icon[] appIcons = ccMushroom.AppIcons;
				XmlDocument xmlCcConfiguration = ccMushroom.XmlCcConfiguration;

				ToolStripMenuItem contextMenuTabPage = (ToolStripMenuItem)sender;

				contextMenuTabPage.DropDownItems.Clear();

				int iTabPage = (int)contextMenuTabPage.Tag;
				contextMenuButtons = new ToolStripMenuItem[appTabPages[iTabPage].Controls.Count];

				// タブ内のボタンをサブメニューとして追加する
				int n = 0;
				foreach ( Control control in appTabPages[iTabPage].Controls )
				{
					if ( !(control is Button) )
						continue;

					int iButton = (int)control.Tag;
					string appToolTip = ccMushroom.toolTip.GetToolTip(control);
					bool newApp;
					string[] appToolTips = ccMushroom.GetButtonTextFromToolTip(appToolTip, out newApp, true).Split('\t');

					contextMenuButtons[n] = new ToolStripMenuItem();
					contextMenuButtons[n].Name = appToolTips[(int)ccMushroom.tt.buttonText];
					contextMenuButtons[n].Text = appToolTips[(int)ccMushroom.tt.buttonText];
					contextMenuButtons[n].Tag = iButton;

					//contextMenuButtons[n].ToolTipText = appToolTip;
					StringBuilder toolTipText = new StringBuilder();
					if ( appToolTips[(int)ccMushroom.tt.comment].Length != 0 )
					{
						toolTipText.Append(appToolTips[(int)ccMushroom.tt.comment]);
					}
					if ( appToolTips[(int)ccMushroom.tt.appEnviron].Length != 0 )
					{
						toolTipText.Append(toolTipText.Length == 0 ? "" : "\r\n");
						toolTipText.Append(appToolTips[(int)ccMushroom.tt.appEnviron]);
					}
					contextMenuButtons[n].ToolTipText = toolTipText.ToString();

					if ( appIcons[iButton] != null )
					{
						if ( ccMushroom.smallApplicationIcon )
						{
							contextMenuButtons[n].Image = appIcons[iButton].ToBitmap();
						}
						else
						{
							//notifyMenuButtons[n].Image = ccf.GetIcon(xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton][TAG_APP_NAME].InnerText, true).ToBitmap();
							XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION)[iButton];
							string appIconFile = (application[ccMushroom.TAG_ICON_FILE] == null) ? application[ccMushroom.TAG_APP_NAME].InnerText : application[ccMushroom.TAG_ICON_FILE].InnerText;
							Icon icon = ccf.GetIcon(appIconFile, true);
							if ( icon != null )
							{
								contextMenuButtons[n].Image = icon.ToBitmap();
								api.DestroyIcon(icon.Handle);
							}
						}
					}

					string appName = appToolTips[(int)ccMushroom.tt.appName].ToLower();
					if ( appName.EndsWith(".exe") || appName.EndsWith(".xls") )
					{
						contextMenuButtons[n].Click += new EventHandler(this.contextMenuButton_Click);
					}
					else
					{
						contextMenuButtons[n].Enabled = false;
					}
					n++;
				}

				contextMenuTabPage.DropDownItems.AddRange(contextMenuButtons);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		/// <summary>
		/// [アプリケーションメニュー] がクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuButton_Click(object sender, System.EventArgs e)
		{
			try
			{
				ccMushroom ccMushroom = (ccMushroom)this.Owner;
				TabPage[] appTabPages = ccMushroom.AppTabPages;
				Icon[] appIcons = ccMushroom.AppIcons;
				XmlDocument xmlCcConfiguration = ccMushroom.XmlCcConfiguration;

				int iButton = (int)((ToolStripMenuItem)sender).Tag;
				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION)[iButton];

				checkAeEnabled.Checked = true;

				textAeAppName.Text = application[ccMushroom.TAG_BUTTON_TEXT].InnerText;
				textAeAppName.Select(textAeAppName.Text.Length, 0);

				textAeAppFileName.Text = application[ccMushroom.TAG_APP_NAME].InnerText;
				textAeAppFileName.Text = ccf.ReplaceCcMushroomLocalFolder(textAeAppFileName.Text, false);
				textAeAppFileName.Select(textAeAppFileName.Text.Length, 0);

				textAeAppComment.Text = string.Empty;
				if ( application[ccMushroom.TAG_COMMENT] != null )
				{
					textAeAppComment.Text = application[ccMushroom.TAG_COMMENT].InnerText;
				}

				if ( !radioAeEdit.Checked && !radioAeCopy.Checked )
				{
					radioAeNew.Checked = true;
				}

				ChangeAeApplyColor(true);
				this.AcceptButton = buttonAeApply;
				textAeAppName.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// アプリケーション環境設定ファイルのドラッグが開始された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageAppEnviron_DragEnter(object sender, DragEventArgs e)
		{
			try
			{
				if ( e.Data.GetDataPresent(DataFormats.FileDrop) )
				{
					string[] sourceFileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
					if ( Path.GetExtension(sourceFileNames[0]) == ".xml" )
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
		/// アプリケーション環境設定ファイルがドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageAppEnviron_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				object obj = e.Data.GetData(DataFormats.FileDrop);
				string[] appEnvironSettingFileNames = (string[])obj;

				string message = "アプリケーション環境設定ファイルを取り込みます.\n" +
								 Path.GetFileName(appEnvironSettingFileNames[0]) + "\n" +
								 "[はい(Y)] 既存の設定名は上書きする\n" +
								 "[いいえ(N)] 既存の設定名はスキップする\n" +
								 "[キャンセル] 中止する";
				DialogResult res = MessageBox.Show(message, THIS_FORM_TEXT, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if ( res == DialogResult.Cancel )
					return;

				Cursor.Current = Cursors.WaitCursor;

				bool overwrite = (res == DialogResult.Yes);
				XmlDocument xmlAppEnvironSetting = new XmlDocument();
				xmlAppEnvironSetting.Load(appEnvironSettingFileNames[0]);	// ドロップされたファイル

				// <appEnviron>
				XmlNode appEnviron = xmlAppEnvironSetting.DocumentElement[ccMushroom.TAG_APP_ENVIRON];
				if ( appEnviron != null )
				{
					foreach ( XmlNode nodeItem in appEnviron.ChildNodes )
					{
						if ( comboAppEnviron.Items.IndexOf(nodeItem.InnerText) != -1 )
							continue;
						comboAppEnviron.Items.Add(nodeItem.InnerText);
						if ( comboAppEnviron.Items.Count == 1 )
						{
							comboAppEnviron.SelectedIndex = 0;
						}
					}

					modifiedCcAppEnvironmentSettingXml = true;
				}
	
				// <application>
				foreach ( XmlNode application in xmlAppEnvironSetting.DocumentElement.SelectNodes(ccMushroom.TAG_APPLICATION) )
				{
					if ( (application.Attributes[ccMushroom.ATTRIB_NAME] == null) || (application[ccMushroom.TAG_APP_NAME] == null) )
						continue;
					if ( (application.Attributes[ccMushroom.ATTRIB_NAME].Value.Length == 0) || (application[ccMushroom.TAG_APP_NAME].InnerText.Length == 0) )
						continue;

					XmlNode _application;
					TreeNode applicationTree;

					int index = treeAppEnviron.Nodes.IndexOfKey(application.Attributes[ccMushroom.ATTRIB_NAME].Value);
					if ( index == -1 )	// 新規？
					{
						_application = xmlCcAppEnvironmentSetting.ImportNode(application, true);
						xmlCcAppEnvironmentSetting.DocumentElement.AppendChild(_application);

						applicationTree = treeAppEnviron.Nodes.Add(_application.Attributes[ccMushroom.ATTRIB_NAME].Value);
						applicationTree.Name = _application.Attributes[ccMushroom.ATTRIB_NAME].Value;
					}
					else
					{
						if ( !overwrite )
							continue;

						string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APPLICATION + "[@" + ccMushroom.ATTRIB_NAME + "='" + application.Attributes[ccMushroom.ATTRIB_NAME].Value + "']";
						XmlNode oldChild = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
						_application = xmlCcAppEnvironmentSetting.ImportNode(application, true);
						xmlCcAppEnvironmentSetting.DocumentElement.ReplaceChild(_application, oldChild);

						applicationTree = treeAppEnviron.Nodes[index];
						applicationTree.Nodes.Clear();
					}

					AppendApplicationTreeNodes(applicationTree, _application);
					modifiedCcAppEnvironmentSettingXml = true;
				}

				treeAppEnviron.SelectedNode = null;
				InitAppEnvironForm();

				if ( modifiedCcAppEnvironmentSettingXml )
				{
					checkReload.Checked = true;
				}

				Cursor.Current = Cursors.Default;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// tabPageAppEnviron_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageAppEnviron_Click(object sender, EventArgs e)
		{
			if ( comboAeFormColor.Visible )
			{
				comboAeFormColor.Visible = false;
			}
		}

		/// <summary>
		/// [フォームの背景色の選択] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAeSelFormColor_Click(object sender, EventArgs e)
		{
			try
			{
				XmlNode itemNode = null;
				if ( (comboAppEnviron.Items.Count == 0) || ((itemNode = GetAppEnvironItemNode(comboAppEnviron.Text)) == null) )
				{
					comboAppEnviron.Select();
					return;
				}

				//this.AcceptButton = null;	// null にすると、comboAeFormColor の入力が Enter キーで確定するようになる

				int right = buttonAeSelFormColor.Location.X + buttonAeSelFormColor.Width;
				int x = right - comboAeFormColor.Width;
				comboAeFormColor.Location = new Point(x, buttonAeSelFormColor.Location.Y);
				comboAeFormColor.Visible = true;			// comboAeFormColor コントロールを表示する
				comboAeFormColor.BringToFront();

				comboAppEnviron.Tag = comboAppEnviron.Location.X + "," + comboAppEnviron.Width;
				comboAppEnviron.Location = new Point(labelAeName.Location.X, comboAppEnviron.Location.Y);	// 環境名の入力領域を移動する
				comboAppEnviron.Width = comboAeFormColor.Location.X - labelAeName.Location.X - 5;
				//comboAppEnviron.Select(comboAppEnviron.Text.Length, 0);
				comboAppEnviron.SelectionLength = 0;

				string formBackColor = (itemNode.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR] == null) ? null : itemNode.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR].Value;

				if ( comboAeFormColor.Items.Count == 0 )
				{
					Cursor.Current = Cursors.WaitCursor;

					//comboAeFormColor.ItemHeight =	comboAeFormColor.Font.Height + 3/*MARGIN_ITEM_TO_STRING*/ * 2;	// 項目の高さ
					//comboAeFormColor.Size = new Size(150, comboAeFormColor.Height);	// 高さはItemHeightに応じて自動調整される

					// 列挙型はGetNamesメソッドで定数名を列挙できる
					// KnownColorの色名を列挙
					/*foreach ( string knownColorName in System.Enum.GetNames(typeof(KnownColor)) )
					{
						comboAeFormColor.Items.Add(knownColorName);
					}*/
					foreach ( KnownColor knownColor in Enum.GetValues(typeof(KnownColor)) )
					{
						Color color = Color.FromKnownColor(knownColor);
						if ( color.IsSystemColor )
							continue;
						comboAeFormColor.Items.Add(knownColor);
						/*int index = comboAeFormColor.Items.Add(knownColor);

						if ( knownColor.ToString() == formBackColor )
						{
							comboAeFormColor.SelectedIndex = index;
						}*/
					}

					Cursor.Current = Cursors.Default;
				}

				int index;
				for ( index = 0; (index < comboAeFormColor.Items.Count) && (string.Compare(comboAeFormColor.Items[index].ToString(), formBackColor, true) != 0); index++ ) ;
				if ( index != comboAeFormColor.Items.Count )	// 有効な色名称？
				{
					comboAeFormColor.SelectedIndex = index;
					this.BackColor = Color.FromName(formBackColor);
				}
				else
				{
					comboAeFormColor.Text = string.Empty;	// comboAeFormColor.SelectedIndex = -1 の代わり
				}

				comboAeFormColor.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				comboAeFormColor.Tag = null;
			}
		}

		/// <summary>
		/// GetAppEnvironItemNode
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private XmlNode GetAppEnvironItemNode(string text)
		{
			string xpath = "/" + ccMushroom.TAG_CONFIGURATION + "/" + ccMushroom.TAG_APP_ENVIRON + "/" + ccMushroom.TAG_ITEM + "[.='" + text + "']";
			XmlNode itemNode = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
			return itemNode;
		}

		/// <summary>
		/// フォームの背景色を comboAeFormColor に描画する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAeFormColor_DrawItem(object sender, DrawItemEventArgs e)
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
		/// フォームの背景色が選択された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAeFormColor_SelectedIndexChanged(object sender, EventArgs e)
		{
			ChangeFormBackColor(((ComboBox)sender).SelectedItem.ToString());
		}

		/// <summary>
		/// comboAeFormColor でキーが上がった
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAeFormColor_KeyUp(object sender, KeyEventArgs e)
		{
			Debug.Write(e.KeyData);

			// this.AcceptButton の設定により、[Enter] キー入力後に確定するか否かが決まる
			if ( (e.KeyData == Keys.Enter) && (this.AcceptButton == null) )
			{
				comboAeFormColor.Visible = false;
			}
		}

		/// <summary>
		/// comboAeFormColor がフォーカスを失った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboAeFormColor_Leave(object sender, EventArgs e)
		{
			if ( comboAeFormColor.Tag != null )
				return;

			try
			{
				comboAeFormColor.Tag = "nowLeaving";
				Debug.WriteLine(" " + comboAeFormColor.Tag);

				if ( this.AcceptButton == null )
				{
					this.AcceptButton = buttonOk;
				}

				Point point = Cursor.Position;

				if ( buttonCancel.ClientRectangle.Contains(buttonCancel.PointToClient(point)) )	// [キャンセル] ボタンが押された？
					return;

				if ( comboAeFormColor.Text.Length != 0 )	// 色名称の入力がある？
				{
					int index;
					for ( index = 0; (index < comboAeFormColor.Items.Count) && (string.Compare(comboAeFormColor.Items[index].ToString(), comboAeFormColor.Text, true) != 0); index++ ) ;
					if ( index == comboAeFormColor.Items.Count )	// 無効な色名称？
					{
						XmlNode itemNode = GetAppEnvironItemNode(comboAppEnviron.Text);
						string formBackColor = (itemNode.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR] == null) ? null : itemNode.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR].Value;
						if ( formBackColor != null )
						{
							comboAeFormColor.Text = formBackColor;
						}
					}
					else
					{
						if ( comboAeFormColor.SelectedItem == null )	// 現在選択されている色と違う色名称が入力されている？
						{
							comboAeFormColor.Text = comboAeFormColor.Items[index].ToString();
							ChangeFormBackColor(comboAeFormColor.Text);
						}
					}
				}

				comboAeFormColor.Visible = false;

				string[] xWidth = ((string)comboAppEnviron.Tag).Split(',');
				comboAppEnviron.Location = new Point(int.Parse(xWidth[0]), comboAppEnviron.Location.Y);	// 環境名の入力領域を元に戻す
				comboAppEnviron.Width = int.Parse(xWidth[1]);
				comboAppEnviron.Tag = null;

				this.BackColor = SystemColors.Control;

				if ( buttonOk.ClientRectangle.Contains(buttonOk.PointToClient(point)) )	// [Ok] ボタンが押された？
					return;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
			finally
			{
				comboAeFormColor.Tag = null;
			}
		}

		/// <summary>
		/// フォームの背景色を変更する
		/// </summary>
		/// <param name="colorName"></param>
		private void ChangeFormBackColor(string colorName)
		{
			try
			{
				string formBackColor = colorName;

				if ( formBackColor == Color.Transparent.Name )
				{
					formBackColor = null;
					this.BackColor = SystemColors.Control;
				}
				else
				{
					this.BackColor = Color.FromName(formBackColor);
				}

				bool modified = false;
				XmlNode itemNode = GetAppEnvironItemNode(comboAppEnviron.Text);	// <item>

				if ( formBackColor == null )	// 背景色の指定なし？
				{
					XmlAttribute attr = itemNode.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR];
					if ( attr != null )
					{
						itemNode.Attributes.Remove(attr);	// 既存の背景色属性があれば削除する
						modified = true;
					}
				}
				else
				{
					XmlAttribute attr = itemNode.Attributes[ccMushroom.ATTRIB_FORM_BACK_COLOR];
					if ( attr == null )
					{
						attr = xmlCcAppEnvironmentSetting.CreateAttribute(ccMushroom.ATTRIB_FORM_BACK_COLOR);
						itemNode.Attributes.Append(attr);
					}
					modified = (attr.Value != formBackColor);
					attr.Value = formBackColor;				// @formBackColor
				}

				if ( modified )
				{
					modifiedCcAppEnvironmentSettingXml = true;
					checkReload.Checked = true;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region Settings@ccMushroom.ini
		/// <summary>
		/// 拡張設定を読み込む
		/// </summary>
		private void ReadExpertSettings()
		{
			try
			{
				if ( !Program.expertMode )
					return;

				// removeNoUseLocalProgramsFolder の環境設定を有効にする
				//SetExpertSettingsTextBox(ccMushroom.KEY_ENABLE_NONEED_LOCAL_FOLDER_REMOVE, "true | false");
				SetExpertSettingsCheckBox(ccMushroom.KEY_ENABLE_REMOVE_NO_USE_LOCAL_FOLDER);

				// プログラム終了ダイアログのカウントダウン(秒)
				SetExpertSettingsTextBox(ccMushroom.KEY_EXIT_DIALOG_COUNT_DOWN, " " + "3");

				// favicon.ico の URL をテキストボックスで設定できるか否か
				//SetExpertSettingsTextBox(ccMushroom.KEY_TEXT_FAVICON_URL_VISIBLE, "true | false");
				SetExpertSettingsCheckBox(ccMushroom.KEY_TEXT_FAVICON_URL_VISIBLE);

				// フォームの背景色
				SetExpertSettingsTextBox(ccMushroom.KEY_FORM_BACK_COLOR, "SystemColors.Control");

				// タブページの背景色
				SetExpertSettingsTextBox(ccMushroom.KEY_TAB_PAGE_BACK_COLOR, "SystemColors.ControlLight");

				// ステータスバーのアプリ環境名の背景色
				SetExpertSettingsTextBox(ccMushroom.KEY_COMBO_APP_ENVIRON_COLOR, "SystemColors.ControlLight");

				// □マークの前景色
				SetExpertSettingsTextBox(ccMushroom.KEY_RECT_MARK_FORE_COLOR, "Color.White");

				// □マークの背景色
				SetExpertSettingsTextBox(ccMushroom.KEY_RECT_MARK_BACK_COLOR, "Color.Black");

				// SDI のテキストエディタ
				SetExpertSettingsTextBox(ccMushroom.KEY_SDI_TEXT_EDITORS, "EditorName");

				// ignoreRemoteInfo の機能を有効にする
				//SetExpertSettingsTextBox(ccMushroom.KEY_ENABLE_IGNORE_REMOTE_INFO, "true | false");
				SetExpertSettingsCheckBox(ccMushroom.KEY_ENABLE_IGNORE_REMOTE_INFO);

				// フォームの最大サイズ
				SetExpertSettingsTextBox(ccMushroom.KEY_FORM_MAXIMUM_SIZE, "0,0 ");

				// 最後に選択された notifyIconMenu を記憶する
				//SetExpertSettignsTextBox(ccMushroom.KEY_RESUME_LATEST_NOTIFY_MENU_ITEM, "true | false");
				SetExpertSettingsCheckBox(ccMushroom.KEY_RESUME_LATEST_NOTIFY_MENU_ITEM);

				// リモート側のボタンに設定された背景色を無視する
				//SetExpertSettingsTextBox(ccMushroom.KEY_IGNORE_REMOTE_BUTTON_BACK_COLOR, "true | false");
				SetExpertSettingsCheckBox(ccMushroom.KEY_IGNORE_REMOTE_BUTTON_BACK_COLOR);

				// Click-Through を切り替えるホットキー
				SetExpertSettingsTextBox(ccMushroom.KEY_TOGGLE_CLICK_THROUGH_HOTKEY, "Shift+Win+C" + " ");
	
				// ボタンの移動中はタブの背景画像を隠す
				SetExpertSettingsCheckBox(ccMushroom.KEY_HIDE_TAB_IMAGE_DURING_BUTTON_DRAG);

				// ボタンのツールチップの横幅
				SetExpertSettingsTextBox(ccMushroom.KEY_BUTTON_TOOLTIP_WIDTH, "");

				// TopMost を解除する時間(秒)
				SetExpertSettingsTextBox(ccMushroom.KEY_TOP_MOST_RELEASE_TIME, " " + "1");

				// エキスパート用で起動するか否か
				//SetExpertSettingsTextBox(ccMushroom.KEY_EXPERT_MODE, "true | false");
				SetExpertSettingsCheckBox(ccMushroom.KEY_EXPERT_MODE);

#if FOR_WINDOWS7
				// タスクバーのライブサムネイルを有効にする
				SetExpertSettingsCheckBox(ccMushroom.KEY_ENABLED_TASKBAR_THUMBNAIL);
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 拡張設定を保存する
		/// </summary>
		private void SaveExpertSettings()
		{
			try
			{
				if ( !Program.expertMode )
					return;

				string text;
				bool? check;

				// removeNoUseLocalProgramsFolder の環境設定を有効にする
				//PutExpertSettingsTextBox(ccMushroom.KEY_ENABLE_NONEED_LOCAL_FOLDER_REMOVE);
				PutExpertSettingsCheckBox(ccMushroom.KEY_ENABLE_REMOVE_NO_USE_LOCAL_FOLDER);

				// プログラム終了ダイアログのカウントダウン(秒)
				PutExpertSettingsTextBox(ccMushroom.KEY_EXIT_DIALOG_COUNT_DOWN);

				// favicon.ico の URL をテキストボックスで設定できるか否か
				//PutExpertSettingsTextBox(ccMushroom.KEY_TEXT_FAVICON_URL_VISIBLE);
				PutExpertSettingsCheckBox(ccMushroom.KEY_TEXT_FAVICON_URL_VISIBLE);

				// フォームの背景色
				PutExpertSettingsTextBox(ccMushroom.KEY_FORM_BACK_COLOR);

				// タブページの背景色
				PutExpertSettingsTextBox(ccMushroom.KEY_TAB_PAGE_BACK_COLOR);

				// ステータスバーのアプリ環境名の背景色
				PutExpertSettingsTextBox(ccMushroom.KEY_COMBO_APP_ENVIRON_COLOR);

				// □マークの前景色
				PutExpertSettingsTextBox(ccMushroom.KEY_RECT_MARK_FORE_COLOR);

				// □マークの背景色
				PutExpertSettingsTextBox(ccMushroom.KEY_RECT_MARK_BACK_COLOR);

				// SDI のテキストエディタ
				PutExpertSettingsTextBox(ccMushroom.KEY_SDI_TEXT_EDITORS);

				// ignoreRemoteInfo の機能を有効にする
				//PutExpertSettingsTextBox(ccMushroom.KEY_ENABLE_IGNORE_REMOTE_INFO);
				PutExpertSettingsCheckBox(ccMushroom.KEY_ENABLE_IGNORE_REMOTE_INFO);

				// フォームの最大サイズ
				PutExpertSettingsTextBox(ccMushroom.KEY_FORM_MAXIMUM_SIZE);

				// 最後に選択された notifyIconMenu を記憶する
				//PutExpertSettingsTextBox(ccMushroom.KEY_RESUME_LATEST_NOTIFY_MENU_ITEM);
				PutExpertSettingsCheckBox(ccMushroom.KEY_RESUME_LATEST_NOTIFY_MENU_ITEM);

				// リモート側のボタンに設定された背景色を無視する
				//PutExpertSettingsTextBox(ccMushroom.KEY_IGNORE_REMOTE_BUTTON_BACK_COLOR);
				PutExpertSettingsCheckBox(ccMushroom.KEY_IGNORE_REMOTE_BUTTON_BACK_COLOR);

				// Click-Through を切り替えるホットキー
				ccMushroom.toggleClickThroughHotKey = PutExpertSettingsTextBox(ccMushroom.KEY_TOGGLE_CLICK_THROUGH_HOTKEY);

				// ボタンの移動中はタブの背景画像を隠す
				if ( (check = PutExpertSettingsCheckBox(ccMushroom.KEY_HIDE_TAB_IMAGE_DURING_BUTTON_DRAG)) == null )
				{
#if FOR_WINDOWS7
					ccMushroom.hideTabImageDuringButtonDrag = true;
#else
					ccMushroom.hideTabImageDuringButtonDrag = false;
#endif
				}
				else
				{
					ccMushroom.hideTabImageDuringButtonDrag = (bool)check;
				}

				// ボタンのツールチップの横幅
				if ( (text = PutExpertSettingsTextBox(ccMushroom.KEY_BUTTON_TOOLTIP_WIDTH)) == null )
					ccMushroom.buttonToolTipWidth = null;
				else
					ccMushroom.buttonToolTipWidth = text;

				// TopMost を解除する時間(秒)
				if ( (text = PutExpertSettingsTextBox(ccMushroom.KEY_TOP_MOST_RELEASE_TIME)) == null )
					ccMushroom.topMostReleaseTime = 1;
				else
					ccMushroom.topMostReleaseTime = int.Parse(text);

				// エキスパート用で起動するか否か
				//PutExpertSettingsTextBox(ccMushroom.KEY_EXPERT_MODE);
				PutExpertSettingsCheckBox(ccMushroom.KEY_EXPERT_MODE);

#if FOR_WINDOWS7
				// タスクバーのライブサムネイルを有効にする
				if ( (check = PutExpertSettingsCheckBox(ccMushroom.KEY_ENABLED_TASKBAR_THUMBNAIL)) == null )
					ccMushroom.enabledTaskbarThumbnail = true;
				else
					ccMushroom.enabledTaskbarThumbnail = (bool)check;
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 拡張設定をテキストボックスにセットする
		/// </summary>
		/// <param name="keyName"></param>
		/// <param name="example"></param>
		private void SetExpertSettingsTextBox(string keyName, string example)
		{
			TextBox textBox = (TextBox)tabPageExpertSettings.Controls["text" + keyName];
			if ( textBox == null )
				return;

			textBox.Tag = example;

			StringBuilder returnedString = new StringBuilder(1024);

			if ( api.GetPrivateProfileString(ccMushroom.SETTINGS_SECTION, keyName, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) == 0 )
			{
				textBox.Text = example;
				textBox.BackColor = Color.WhiteSmoke;
				//textBox.ForeColor = Color.Gray;
			}
			else
			{
				textBox.Text = returnedString.ToString();
			}
		}

		/// <summary>
		/// 拡張設定のテキストボックス値を保存する
		/// </summary>
		/// <param name="keyName"></param>
		private string PutExpertSettingsTextBox(string keyName)
		{
			TextBox textBox = (TextBox)tabPageExpertSettings.Controls["text" + keyName];
			if ( textBox == null )
				return null;

			string lpString = null;

			if ( (textBox.Text.Length != 0) && (textBox.Text != (string)textBox.Tag) )	// テキストは設定例ではない？
			{
				lpString = textBox.Text;
			}

			api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, keyName, lpString, ccMushroomIniFileName);

			return lpString;
		}

		/// <summary>
		/// 拡張設定をチェックボックスにセットする
		/// </summary>
		/// <param name="keyName"></param>
		private void SetExpertSettingsCheckBox(string keyName)
		{
			CheckBox checkBox = (CheckBox)tabPageExpertSettings.Controls["check" + keyName];
			if ( checkBox == null )
				return;

			StringBuilder returnedString = new StringBuilder(1024);
			if ( api.GetPrivateProfileString(ccMushroom.SETTINGS_SECTION, keyName, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) == 0 )
			{
				checkBox.CheckState = CheckState.Indeterminate;
			}
			else
			{
				checkBox.Checked = bool.Parse(returnedString.ToString());
			}
		}

		/// <summary>
		/// 拡張設定のチェックボックス値を保存する
		/// </summary>
		/// <param name="keyName"></param>
		/// <returns></returns>
		private bool? PutExpertSettingsCheckBox(string keyName)
		{
			CheckBox checkBox = (CheckBox)tabPageExpertSettings.Controls["check" + keyName];
			if ( checkBox == null )
				return (bool?)null;

			string lpString = null;

			if ( checkBox.CheckState != CheckState.Indeterminate )
			{
				lpString = checkBox.Checked.ToString().ToLower();
			}

			api.WritePrivateProfileString(ccMushroom.SETTINGS_SECTION, keyName, lpString, ccMushroomIniFileName);

			return lpString == null ? (bool?)null : checkBox.Checked;
		}

		/// <summary>
		/// テキストボックスの値が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExpSet_TextChanged(object sender, EventArgs e)
		{
			try
			{
				TextBox textBox = (TextBox)sender;

				if ( textBox.BackColor == Color.WhiteSmoke )
				{
					textBox.BackColor = SystemColors.Window;
					textBox.ForeColor = SystemColors.WindowText;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// テキストボックスがフォーカスを失った
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExpSet_Leave(object sender, EventArgs e)
		{
			try
			{
				TextBox textBox = (TextBox)sender;

				if ( textBox.Text.Length == 0 )
				{
					textBox.Text = (string)textBox.Tag;
					textBox.BackColor = Color.WhiteSmoke;
					//textBox.ForeColor = Color.Gray;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// チェックボックスのステータスが変わった
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkExpSet_CheckStateChanged(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			checkReload.Checked = true;
		}

		/// <summary>
		/// 入力されたテキストの有効性を検証する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExpSet_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				TextBox textBox = (TextBox)sender;

				if ( ActiveControl == textBox )	// フォーム上で ESC キーが押された？（フォーカスが次に移動してないので）
					return;

				if ( textBox.Text.Length == 0 )
					return;

				bool validate = true;
				Regex regex;

				switch ( textBox.Name )
				{
					case "textExitDialogCountDown":
					case "textButtonToolTipWidth":
					case "textTopMostReleaseTime":
						regex = new Regex(@"^[ 0-9]+$");
						validate = regex.IsMatch(textBox.Text);
						break;
				}

				if ( !validate )
				{
					errorProvider.SetError(textBox, "input error...");
					// e.Cancel = true　でCancel を true にすると正しく入力しないと次に行けない。
					e.Cancel = true;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		/// <summary>
		/// ccConfiguration.import.xml
		/// テキストの検証が終了した
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textExpSet_Validated(object sender, EventArgs e)
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
		#endregion

		#region バージョン情報
		/// <summary>
		/// [問い合わせ先] リンクがクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string url = null;

			try
			{
				tabControl.Select();

				int index;
				if ( (index = ccMushroom.urlMailToDeveloper.IndexOf("subject=")) == -1 )
				{
					url = ccMushroom.urlMailToDeveloper + "?subject=" + System.Web.HttpUtility.UrlEncode(ccMushroom.appTitle);
				}
				else
				{
					int iSubject = index + 8;	// 8:"subject="
					url = ccMushroom.urlMailToDeveloper.Substring(0, iSubject) + System.Web.HttpUtility.UrlEncode(ccMushroom.urlMailToDeveloper.Substring(iSubject));
				}

				Process.Start(url);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message + "\r\n" + url, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		#endregion

		/// <summary>
		/// buttonViewCcFiles_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonViewCcFiles_Click(object sender, EventArgs e)
		{
			try
			{
				string _textEditor = (ccMushroom.textEditor == null) ? common.CheckEnvironmentVariable("%WINDIR%\\notepad.exe") : ccMushroom.textEditor;
				string _textEditorName = Path.GetFileName(_textEditor).ToLower();
				bool _sdiTextEditor = (ccMushroom.sdiTextEditors.IndexOf(_textEditorName) != -1);

				string ccMushroomConfig = "\"" + Application.StartupPath + "\\" + Application.ProductName + ".exe.config" + "\"";
				string ccMushroomIni = "\"" + Application.StartupPath + "\\" + ccMushroom.CC_MUSHROOM_INI_FILE_NAME + "\"";
				string ccConfigurationImport = "\"" + Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_IMPORT_FILE_NAME + "\"";
				string ccAppEnvironmentSetting = Application.StartupPath + "\\" + ccMushroom.CC_APP_ENVIRONMENT_SETTING_FILE_NAME;
				ccAppEnvironmentSetting = File.Exists(ccAppEnvironmentSetting) ? "\"" + ccAppEnvironmentSetting + "\"" : "";
				string ccConfiguration = "\"" + Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_FILE_NAME + "\"";
				string ccConfigurationLog = "\"" + Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_LOG_FILE_NAME + "\"";
				string appDataCcMushroomIni = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\ICSG\\ccMushroom" + "\\" + ccMushroom.CC_MUSHROOM_INI_FILE_NAME;
				appDataCcMushroomIni = File.Exists(appDataCcMushroomIni) ? "\"" + appDataCcMushroomIni + "\"" : "";
				string tabPageSettingsIni = "";
#if ENABLE_TAB_BACKGROUND
				tabPageSettingsIni = Application.StartupPath + "\\" + ccMushroom.TAB_PAGE_SETTINGS_INI_FILE_NAME;
				tabPageSettingsIni = File.Exists(tabPageSettingsIni) ? "\"" + tabPageSettingsIni + "\"" : "";
#endif
				string[] paramFileNames = { ccMushroomConfig, ccMushroomIni, ccConfigurationImport, ccAppEnvironmentSetting, ccConfiguration, ccConfigurationLog, appDataCcMushroomIni, tabPageSettingsIni };

				if ( _sdiTextEditor )
				{
					for ( int i = 0; i < paramFileNames.Length; i++ )
					{
						if ( paramFileNames[i].Length == 0 )
							continue;
						Process.Start(_textEditor, paramFileNames[i]);
					}
				}
				else
				{
					Process.Start(_textEditor, string.Join(" ", paramFileNames));
				}

				tabControl.Select();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}