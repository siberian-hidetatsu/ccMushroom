using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Configuration;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using CommonFunctions;
using ProgUpdateClass;

namespace ccMushroom
{
	static class Program
	{
		private const string CMDPARAM_OFFLINE = "/offline";
		public const string CMDPARAM_APP_TITLE = "/AppTitle:";
		private const string CMDPARAM_DEBMODE = "/DebMode";
		private const string CMDPARAM_EXPERT_MODE = "/ExpertMode";
		public const string CMDPARAM_DELAY_TIME = "/DelayTime:";
		public const string CMDPARAM_BUTTON_TEXT = "/ButtonText:";

		private static bool showProgUpdateMessage = false;
		public static bool onlineEnable = true;
		public static bool debMode = false;
		public static bool expertMode = false;

		// <App.config>
		public const string CONSET_LATEST_PROGRAM_FOLDER = "LatestProgramFolder";			// 最新のプログラム フォルダを置いているフォルダ名
		private const string CONSET_URL_MAIL_TO_DEVELOPER = "UrlMailToDeveloper";
		public const string CONSET_REMOTE_PROGRAMS_FOLDER = "RemoteProgramsFolder";			// リモート側のプログラムを置いているフォルダ名
		public const string CONSET_LOCAL_PROGRAMS_FOLDER = "LocalProgramsFolder";			// ローカル側のプログラムをコピーするフォルダ名
		public const string CONSET_ENABLED_PROGRAMS_FOLDER = "EnabledProgramsFolder";		// 有効/無効なプログラムフォルダの設定
		public const string CONSET_SCAN_REMOTE_PROGRAMS_FOLDER = "ScanRemoteProgramsFolder";	// リモート側のプログラム フォルダをスキャンするか否か
		public const string CONSET_REMOVE_NOUSE_LOCAL_PROGRAMS_FOLDER = "RemoveNoUseLocalProgramsFolder";	// 未使用のローカル プログラムのサブディレクトリを検索して削除する
		public const string CONSET_ALWAYS_IN_TASKTRAY = "AlwaysInTasktray";					// 常にタスクトレイにアイコンを表示するか否か
		public const string CONSET_MULTI_LINE_TAB = "MultiLineTab";							// タブを複数の行で表示する
		public const string CONSET_IMPORT_TAB_APPEAR_FIRST = "ImportTabAppearFirst";		// インポートされたタブを最初に表示する
		public const string CONSET_HIDE_TAB_PAGE_TEXT = "HideTabPageText";					// 非表示にするタブ
		public const string CONSET_AUTO_WINDOW_CLOSE_TIME = "AutoWindowCloseTime";			// 自動的にウィンドウを閉じるまでの時間（分）
		public const string CONSET_SHORTCUT_MNG_APPLY_TIME = "ShortcutMngApplyTime";		// ショートカット管理で自動的に適用されるまでの時間(秒)
		public const string CONSET_AUTO_OPACITY_TIME_PERCENT = "AutoOpacityTimePercent";	// 自動的に半透明にする時間(秒)と透過率
		public const string CONSET_TRANSPARENT_WITH_CLICKTHROUGH = "TransparentWithClickThrough";	// 半透明の時はマウスのクリックを素通りさせる
		public const string CONSET_TEXT_EDITOR = "TextEditor";								// テキスト エディタ
		public const string CONSET_MOVABLE_BUTTON = "MovableButton";						// ボタンの移動を有効にする
		public const string CONSET_INTEGRATE_EXPLORER_CONTEXT = "IntegrateExplorerContext";	// エクスプローラのコンテキストメニューに統合する
		public const string CONSET_SMALL_APPLICATION_ICON = "SmallApplicationIcon";
		public const string CONSET_BUTTON_SIZE = "ButtonSize";
		public const string CONSET_BUTTON_FONT = "ButtonFont";
		public const string CONSET_BUTTON_BACK_COLOR = "ButtonBackColor";
		public const string CONSET_HOT_KEY = "HotKey";
		// </App.config>

		private static string messageApplicationError = null;

		public enum bfont									// フォント情報
		{
			family, size, style, color
		}

		public const string ccHotCakeFile = "ccHotCake.exe";
		public const string _ccMushShellExtFile = "_ccMushShellExt.dll";

		public const string CCMUSHROOM_APP_DATA = "\\ICSG\\ccMushroom";		// ccMushShellExt プロジェクトと合わせる
		public const string KEY_CCMUSH_STARTUP_PATH = "CcMushStartupPath";	// ccMushShellExt プロジェクトと合わせる
		public const string KEY_CCMUSH_APP_TITLE = "CcMushAppTitle";		// ccMushShellExt プロジェクトと合わせる
		public const string KEY_CCMUSH_CMDLINE_ARGS = "CcMushCmdLineArgs";	// ccMushShellExt プロジェクトと合わせる

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new ccMushroom());

			try
			{
				ccMushroom.appTitle = Application.ProductName;

				// コマンド ライン パラメータ
				string[] cmdParam = Environment.GetCommandLineArgs();
				for ( int i = 0; i < cmdParam.Length; i++ )
				{
					if ( cmdParam[i] == update.CMDPARAM_SHOW_PROG_UPDATE_MESSAGE )
					{
						showProgUpdateMessage = true;
					}
					else if ( cmdParam[i] == CMDPARAM_OFFLINE )
					{
						onlineEnable = false;
					}
					else if ( cmdParam[i].StartsWith(CMDPARAM_APP_TITLE) )
					{
						ccMushroom.appTitle = cmdParam[i].Substring(CMDPARAM_APP_TITLE.Length);
					}
					else if ( cmdParam[i] == CMDPARAM_DEBMODE )
					{
						debMode = true;
					}
					else if ( cmdParam[i] == CMDPARAM_EXPERT_MODE )
					{
						expertMode = true;
					}
					else if ( cmdParam[i].StartsWith(CMDPARAM_DELAY_TIME) )
					{
						int delayTime = int.Parse(cmdParam[i].Substring(CMDPARAM_DELAY_TIME.Length));
						if ( delayTime != 0 )
						{
							Thread.Sleep(delayTime * 1000);
						}
					}
					else if ( cmdParam[i].StartsWith(CMDPARAM_BUTTON_TEXT) )
					{
						try
						{
							string buttonText = cmdParam[i].Substring(CMDPARAM_BUTTON_TEXT.Length);
							XmlDocument xmlCcConfiguration = new XmlDocument();
							xmlCcConfiguration.Load(Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_FILE_NAME);
							string xpath = ccMushroom.TAG_APPLICATION + "[" + ccMushroom.TAG_BUTTON_TEXT + "='" + buttonText + "']";
							XmlNode application = xmlCcConfiguration.DocumentElement.SelectSingleNode(xpath);
							if ( application != null )
							{
								ProcessStart(application, null);
							}
						}
						catch ( Exception exp )
						{
							MessageBox.Show(exp.Message, ccMushroom.appTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						return;
					}
				}

/*#if (DEBUG)
				debMode = true;
#endif*/

				if ( !GetAppConfig() )
				{
					MessageBox.Show(messageApplicationError, "APPLICATION CONFIG ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// ウィンドウの位置とサイズ
				ccMushroom.GetWindowRectangle();

				// ミューテックスを確認して、アプリケーションを起動する
				Mutex m = new Mutex(false, ccMushroom.appTitle);

				if ( m.WaitOne(0, false) )	// 最初の起動？
				{
					if ( ProgramUpdateRelaunch() )
					{
						m.ReleaseMutex();
						m.Close();
						return;
					}

					ccMushroom formCcMushroom = new ccMushroom();

#if (SHOW_STARTING_SPLASH_FORM)
					////スプラッシュウィンドウを表示
#if (USE_MULTI_THRED_SPLASH_FORM)
					SplashForm.ShowSplash(formCcMushroom);
#else
					SplashForm splash = new SplashForm(formCcMushroom);
					splash.Show();
					Application.DoEvents();
#endif
#endif

					Application.Run(formCcMushroom/*new ccMushroom()*/);
					m.ReleaseMutex();
				}
				else
				{
					// bring old instance to the front
					IntPtr hWnd = api.FindWindow(null, ccMushroom.appTitle);
					if ( hWnd != IntPtr.Zero )
					{
						if ( api.IsIconic(hWnd) || !api.IsWindowVisible(hWnd) )
						{
							api.ShowWindow(hWnd, api.SW_SHOWNOACTIVATE);
						}
						api.SetForegroundWindow(hWnd);
					}
				}

				// アプリケーションが終わるまでmへの参照を維持するようにする
				GC.KeepAlive(m);
				m.Close();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// プログラムの更新版があれば、更新して再起動する
		/// </summary>
		/// <returns></returns>
		private static bool ProgramUpdateRelaunch()
		{
			if ( !onlineEnable && ccMushroom.latestProgramFolder.StartsWith(@"\\") )
				return false;

#if false
			try
			{
				/* ここのロジックは暫くしたら無効にする */
				if ( ccMushroom.latestProgramFolder.Length != 0 )
				{
					// ccHotCakeFile がローカルに無ければ、リモート側からコピーしておく
					string remoteFileName = ccMushroom.latestProgramFolder + ccHotCakeFile;
					string localFileName = Application.StartupPath + "\\" + ccHotCakeFile;
					if ( !File.Exists(localFileName) && File.Exists(remoteFileName) )
						File.Copy(remoteFileName, localFileName);
					// ccMushShellExtFile がローカルに無ければ、リモート側からコピーしておく
					remoteFileName = ccMushroom.latestProgramFolder + _ccMushShellExtFile;
					localFileName = Application.StartupPath + "\\" + _ccMushShellExtFile;
					if ( !File.Exists(localFileName) && File.Exists(remoteFileName) )
						File.Copy(remoteFileName, localFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif

			string[] appProductNames = { Application.ProductName + ".exe", ccHotCakeFile, _ccMushShellExtFile, Application.ProductName + ".chm" };
			// overWriteConfigKeys オプション有り
			string[,] configKeys = {{CONSET_LATEST_PROGRAM_FOLDER, string.Empty},
								    {CONSET_URL_MAIL_TO_DEVELOPER, string.Empty},
								    {CONSET_REMOTE_PROGRAMS_FOLDER, string.Empty},
									{CONSET_LOCAL_PROGRAMS_FOLDER, string.Empty}};
			/*// overWriteConfigKeys オプション無し
			string[,] configKeys = {{CONSET_SCAN_REMOTE_PROGRAMS_FOLDER, ccMushroom.scanRemoteProgramsFolder.ToString().ToLower()},
									{CONSET_ALWAYS_IN_TASKTRAY, ccMushroom.alwaysInTasktray.ToString().ToLower()},
									{CONSET_MULTI_LINE_TAB, ccMushroom.multiLineTab.ToString().ToLower()},
									{CONSET_SMALL_APPLICATION_ICON, ccMushroom.smallApplicationIcon.ToString().ToLower()},
									{CONSET_BUTTON_SIZE, ccMushroom.buttonSize.Width + "," + ccMushroom.buttonSize.Height},
									{CONSET_BUTTON_FONT, GetFontInfo(ccMushroom.buttonFont, ccMushroom.buttonTextColor)}};*/

			/*uint option = 0;*/
			uint option = (uint)update.options.overWriteConfigKeys/*0*/;
			option |= (showProgUpdateMessage) ? (uint)update.options.showProgUpdateMessage : 0;

			bool completedProgramUpdate = update.CheckProgramUpdate(ccMushroom.latestProgramFolder, appProductNames, configKeys, ccMushroom.windowRectangle, option);
			if ( completedProgramUpdate )
			{
#if true
				try
				{
					string progUpdateLogFile = Application.StartupPath + "\\" + "~progUpdate.log";
					try { if ( File.Exists(progUpdateLogFile) ) File.Delete(progUpdateLogFile); }
					catch ( Exception ) { }

					// ccMushShellExt.dll ファイルを最新版で更新しておく
					string ccMushShellExtFile = Program._ccMushShellExtFile.Substring(1);
					string curCcMushShellExt = Application.StartupPath + "\\" + ccMushShellExtFile;
					if ( ccMushroom.integrateExplorerContext && File.Exists(curCcMushShellExt) )
					{
						string orgCcMushShellExt = Application.StartupPath + "\\" + _ccMushShellExtFile;
						Version verOrgCcMushShellExt = new Version(FileVersionInfo.GetVersionInfo(orgCcMushShellExt).FileVersion);
						Version verCurCcMushShellExt = new Version(FileVersionInfo.GetVersionInfo(curCcMushShellExt).FileVersion);

						if ( verCurCcMushShellExt < verOrgCcMushShellExt )	// 新しい dll ファイル？
						{
							StringBuilder shellExtLog = new StringBuilder();
							shellExtLog.Append("verOrgCcMushShellExt: " + verOrgCcMushShellExt + "\r\n");
							shellExtLog.Append("verCurCcMushShellExt: " + verCurCcMushShellExt + "\r\n");

							string tmpCcMushShellExt = Application.StartupPath + "\\" + "~" + ccMushShellExtFile;
							ccf.UpdateFile(orgCcMushShellExt, curCcMushShellExt, tmpCcMushShellExt, ref shellExtLog);

							using ( StreamWriter sw = new StreamWriter(progUpdateLogFile) )
							{
								sw.Write(shellExtLog.ToString());
								sw.Close();
							}
						}
					}

					string faviconUrlDllFile = Application.StartupPath + "\\" + ccMushroom.iconsFolder + "\\" + ccMushroom.faviconUrlDllFileName;
					try { if ( File.Exists(faviconUrlDllFile) ) File.Delete(faviconUrlDllFile); }
					catch ( Exception ) { }
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
#endif

				if ( update.RelaunchExe(appProductNames, ccMushroom.appTitle, null, null, option) )
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// アプリケーション設定ファイルを読み込む
		/// </summary>
		private static bool GetAppConfig()
		{
			try
			{
				string[] returnedStrings;
				string returnedString;

				// 最新のプログラム フォルダを置いているフォルダ名
				ccMushroom.latestProgramFolder = ConfigurationManager.AppSettings[CONSET_LATEST_PROGRAM_FOLDER];
				if ( ccMushroom.latestProgramFolder.Length != 0 && !ccMushroom.latestProgramFolder.EndsWith("\\") )
					ccMushroom.latestProgramFolder += "\\";

				// 問い合わせ先の url
				ccMushroom.urlMailToDeveloper = ConfigurationManager.AppSettings[CONSET_URL_MAIL_TO_DEVELOPER];

				// リモート側のプログラム名称
				ccMushroom.remoteProgramsNames = new string[1];
				ccMushroom.remoteProgramsNames[0] = "ccMushroom Default";

				// リモート側のプログラムを置いているフォルダ名
				ccMushroom.remoteProgramsFolders = new string[1];
				ccMushroom.remoteProgramsFolders[0] = ConfigurationManager.AppSettings[CONSET_REMOTE_PROGRAMS_FOLDER];
				if ( !ccf.AdjustFolderFormat(ref ccMushroom.remoteProgramsFolders[0]) )
					throw new Exception(CONSET_REMOTE_PROGRAMS_FOLDER + " 設定が不正です.");

				// ローカル側のプログラムをコピーするフォルダ名
				ccMushroom.localProgramsFolders = new string[1];
				ccMushroom.localProgramsFolders[0] = ConfigurationManager.AppSettings[CONSET_LOCAL_PROGRAMS_FOLDER];
				if ( !ccf.AdjustFolderFormat(ref ccMushroom.localProgramsFolders[0]) )
					throw new Exception(CONSET_LOCAL_PROGRAMS_FOLDER + " 設定が不正です.");
				if ( ccf.IsSystemFolder(ccMushroom.localProgramsFolders[0]) )
					throw new Exception(ccMushroom.localProgramsFolders[0] + " フォルダはローカル側のプログラム フォルダにできません.");
#if true
				try
				{
					ccMushroom.ccMushroomLocalFolder = Directory.GetParent(ccMushroom.localProgramsFolders[0]).FullName;
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
#endif

				// 有効/無効なプログラムフォルダの設定
				ccMushroom.enabledProgramsFolders = new bool[1];
				//ccMushroom.enabledProgramsFolders[0] = true;
				ccMushroom.enabledProgramsFolders[0] = bool.Parse(ConfigurationManager.AppSettings[CONSET_ENABLED_PROGRAMS_FOLDER]);

				// リモート側のプログラム フォルダをスキャンするか否か
				ccMushroom.scanRemoteProgramsFolder = bool.Parse(ConfigurationManager.AppSettings[CONSET_SCAN_REMOTE_PROGRAMS_FOLDER]);

				// 未使用のローカル プログラムのサブディレクトリを検索して削除する
				ccMushroom.removeNoUseLocalProgramsFolder = bool.Parse(ConfigurationManager.AppSettings[CONSET_REMOVE_NOUSE_LOCAL_PROGRAMS_FOLDER]);

				// 常にタスクトレイにアイコンを表示するか否か
				ccMushroom.alwaysInTasktray = bool.Parse(ConfigurationManager.AppSettings[CONSET_ALWAYS_IN_TASKTRAY]);

				// タブを複数の行で表示する
				ccMushroom.multiLineTab = bool.Parse(ConfigurationManager.AppSettings[CONSET_MULTI_LINE_TAB]);

				// インポートされたタブを最初に表示する
				ccMushroom.importTabAppearFirst = bool.Parse(ConfigurationManager.AppSettings[CONSET_IMPORT_TAB_APPEAR_FIRST]);

				// 非表示にするタブ
				returnedString = ConfigurationManager.AppSettings[CONSET_HIDE_TAB_PAGE_TEXT];
				if ( returnedString.Length != 0 )
				{
					ccMushroom.hideTabPageText = returnedString.Split(',');
				}

				// 自動的にウィンドウを閉じるまでの時間（分）
				ccMushroom.autoWindowCloseTime = Int32.Parse(ConfigurationManager.AppSettings[CONSET_AUTO_WINDOW_CLOSE_TIME]) * 60 * 1000;

				// ショートカット管理で自動的に適用されるまでの時間(秒)
				ccMushroom.shortcutMngApplyTime = ConfigurationManager.AppSettings[CONSET_SHORTCUT_MNG_APPLY_TIME];

				// 自動的に半透明にする時間(秒)と透過率
				ccMushroom.autoOpacityTimePercent = ConfigurationManager.AppSettings[CONSET_AUTO_OPACITY_TIME_PERCENT];

				// 半透明の時はマウスのクリックを素通りさせる
				ccMushroom.transparentWithClickThrough = bool.Parse(ConfigurationManager.AppSettings[CONSET_TRANSPARENT_WITH_CLICKTHROUGH]);

				// テキスト エディタ
				returnedString = ConfigurationManager.AppSettings[CONSET_TEXT_EDITOR];
				if ( returnedString.Length != 0 )
				{
					ccMushroom.textEditor = returnedString;
				}

				// ボタンの移動を有効にする
				ccMushroom.movableButton = bool.Parse(ConfigurationManager.AppSettings[CONSET_MOVABLE_BUTTON]);

				// エクスプローラのコンテキストメニューに統合する
				ccMushroom.integrateExplorerContext = bool.Parse(ConfigurationManager.AppSettings[CONSET_INTEGRATE_EXPLORER_CONTEXT]);

				// スモール サイズでアプリケーションのアイコンを表示する
				ccMushroom.smallApplicationIcon = bool.Parse(ConfigurationManager.AppSettings[CONSET_SMALL_APPLICATION_ICON]);

				// ボタンのサイズ
				returnedStrings = ConfigurationManager.AppSettings[CONSET_BUTTON_SIZE].Split(',');
				ccMushroom.buttonSize = new Size(Int32.Parse(returnedStrings[0]), Int32.Parse(returnedStrings[1]));

				// ボタンのフォント
				returnedString = ConfigurationManager.AppSettings[CONSET_BUTTON_FONT];
				ccMushroom.buttonFont = CreateFont(returnedString, out ccMushroom.buttonTextColor);

				// ボタンの背景色
				returnedString = ConfigurationManager.AppSettings[CONSET_BUTTON_BACK_COLOR];
				ccMushroom.buttonBackColor = Color.FromArgb(int.Parse(returnedString, System.Globalization.NumberStyles.HexNumber));

				// ホットキー
				ccMushroom.hotKey = ConfigurationManager.AppSettings[CONSET_HOT_KEY];
			}
			catch ( Exception exp )
			{
				messageApplicationError = exp.Message;
				return false;
			}

			return true;
		}

		/// <summary>
		/// フォント情報文字列からフォントを作成する
		/// </summary>
		public static Font CreateFont(string fontInfo, out Color color)
		{
			Font newFont;
			color = Color.Black;

			try
			{
				string[] fonts = fontInfo.Split(',');
				FontStyle style = FontStyle.Regular;

				for ( int i = 0; i < fonts.Length; i++ )
				{
					int index = fonts[i].IndexOf(":") + 1;
					fonts[i] = fonts[i].Substring(index).Trim();
					if ( i == (int)bfont.style )
					{
						string[] styles = fonts[i].Split('|');
						for ( int j = 0; j < styles.Length; j++ )
						{
							if ( styles[j] == "Regular" )
								style |= FontStyle.Regular;
							else if ( styles[j] == "Bold" )
								style |= FontStyle.Bold;
							else if ( styles[j] == "Italic" )
								style |= FontStyle.Italic;
							else if ( styles[j] == "Underline" )
								style |= FontStyle.Underline;
							else if ( styles[j] == "Strikeout" )
								style |= FontStyle.Strikeout;
						}
					}
					else if ( i == (int)bfont.color )
					{
						color = Color.FromName(fonts[(int)bfont.color]);
					}
				}

				newFont = new Font(fonts[(int)bfont.family], float.Parse(fonts[(int)bfont.size]), style);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				newFont = null;
			}

			return newFont;
		}

		/// <summary>
		/// フォントの情報を文字列にする
		/// </summary>
		public static string GetFontInfo(Font font, Color color)
		{
			string fontInfo;

			try
			{
				StringBuilder finfo = new StringBuilder();

				finfo.Append("Name:" + font.FontFamily.Name);

				finfo.Append(",Size:" + font.Size);

				finfo.Append(",Style:");
				FontStyle style = font.Style;
				if ( style == FontStyle.Regular )
				{
					finfo.Append(FontStyle.Regular.ToString() + "|");
				}
				else
				{
					for ( byte bit = 1; bit <= 8; bit <<= 1 )
					{
						if ( ((byte)style & bit) == 0 )
							continue;

						if ( bit == (byte)FontStyle.Bold )
							finfo.Append(FontStyle.Bold.ToString() + "|");
						else if ( bit == (byte)FontStyle.Italic )
							finfo.Append(FontStyle.Italic.ToString() + "|");
						else if ( bit == (byte)FontStyle.Underline )
							finfo.Append(FontStyle.Underline.ToString() + "|");
						else if ( bit == (byte)FontStyle.Strikeout )
							finfo.Append(FontStyle.Strikeout.ToString() + "|");
					}
				}
				finfo.Remove(finfo.Length - 1, 1);

				finfo.Append(",Color:" + color.Name);

				fontInfo = finfo.ToString();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				fontInfo = null;
			}

			return fontInfo;
		}

		/// <summary>
		/// ProcessStart
		/// </summary>
		/// <param name="application"></param>
		/// <param name="verb"></param>
		private static void ProcessStart(XmlNode application, string verb)
		{
			try
			{
				string appName = application[ccMushroom.TAG_APP_NAME].InnerText;
				string arguments = "";

				if ( application[ccMushroom.TAG_COMMAND_LINE] != null )
				{
					arguments = application[ccMushroom.TAG_COMMAND_LINE].InnerText;
				}

				ProcessStartInfo startInfo = new ProcessStartInfo(appName, arguments);

				if ( application[ccMushroom.TAG_WORKING_DIRECTORY] == null )
				{
					startInfo.WorkingDirectory = Path.GetDirectoryName(appName);
				}
				else
				{
					startInfo.WorkingDirectory = application[ccMushroom.TAG_WORKING_DIRECTORY].InnerText;
				}

				if ( verb != null )
				{
					startInfo.Verb = verb;
				}

				Process.Start(startInfo);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
