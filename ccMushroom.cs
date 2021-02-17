//#define	SHOW_STARTING_SPLASH_FORM			// 起動時にスプラッシュ フォームを表示する（実際にはプロジェクト プロパティの[ビルド][条件付きコンパイル定数]で設定する）
//#define	SHOW_REFRESHING_INTERNAL_PROGRESS_MESSAGE	// リフレッシュ時に内部の進捗メッセージを表示する
//#define	USE_MULTI_THRED_SPLASH_FORM		// スプラッシュ フォームをマルチ スレッドにする（実際にはプロジェクト プロパティの[ビルド][条件付きコンパイル定数]で設定する）
#define	RESUME_APP_ENVIRONMENT			// プログラム起動時に、アプリケーションの環境を再表示する
#define	MOVABLE_BUTTON_BY_DRAGDROP		// ドラッグ＆ドロップでボタンの移動ができる
//#define	ENABLE_TAB_BACKGROUND			// タブの背景画像を有効にする（実際にはプロジェクト プロパティの[ビルド][条件付きコンパイル定数]で設定する）
#define	ENABLE_LOGON_REQUIRED			// ログオンの要求を有効にする
#define	PROGRAM_SUBDIR_NAME_IN_LAUNCHER	// Launcher.xml で、実ディレクトリのプログラム用サブ ディレクトリを明示的に指定する
//#define	FOR_WINDOWS7					// Windows 7 用のロジックを有効にする（実際にはプロジェクト プロパティの[ビルド][条件付きコンパイル定数]で設定する）
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
#if FOR_WINDOWS7
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Windows.Shell;			// PresentationFramework.dll
#endif
using CommonFunctions;

namespace ccMushroom
{
	public partial class ccMushroom : Form
	{
		// ccMushroom.ini
		public const string CC_MUSHROOM_INI_FILE_NAME = "ccMushroom.ini";				// ccMshroom.ini ファイル名
		public const string SETTINGS_SECTION = "Settings";								// [Settings] セクション
		private const string RESUME_SECTION = "Resume";									// [Resume] セクション
		public const string KEY_REMOTE_PROGRAMS_NAME = "RemoteProgramsName";					// リモート側のプログラム名称
		public const string KEY_REMOTE_PROGRAMS_FOLDER = Program.CONSET_REMOTE_PROGRAMS_FOLDER;	// リモート側のプログラムを置いているフォルダ名
		public const string KEY_LOCAL_PROGRAMS_FOLDER = Program.CONSET_LOCAL_PROGRAMS_FOLDER;	// ローカル側のプログラムをコピーするフォルダ名
		public const string KEY_ENABLED_PROGRAMS_FOLDER = "EnabledProgramsFolder";				// 有効/無効なプログラムフォルダの設定
		public const string KEY_ENABLE_REMOVE_NO_USE_LOCAL_FOLDER = "EnableRemoveNoUseLocalFolder";	// removeNoUseLocalProgramsFolder の環境設定を有効にする（拡張設定）
		public const string KEY_EXIT_DIALOG_COUNT_DOWN = "ExitDialogCountDown";			// プログラム終了ダイアログのカウントダウン(秒)（拡張設定）
		public const string KEY_TEXT_FAVICON_URL_VISIBLE = "TextFaviconUrlVisible";		// favicon.ico の URL をテキストボックスで設定できるか否か（拡張設定）
		public const string KEY_FORM_BACK_COLOR = "FormBackColor";						// フォームの背景色（拡張設定）
		public const string KEY_TAB_PAGE_BACK_COLOR = "TabPageBackColor";				// タブページの背景色（拡張設定）
		public const string KEY_COMBO_APP_ENVIRON_COLOR = "ComboAppEnvironColor";		// ステータスバーのアプリ環境名の背景色（拡張設定）
		public const string KEY_RECT_MARK_FORE_COLOR = "RectMarkForeColor";				// □マークの前景色（拡張設定）
		public const string KEY_RECT_MARK_BACK_COLOR = "RectMarkBackColor";				// □マークの背景色（拡張設定）
		public const string KEY_SDI_TEXT_EDITORS = "SdiTextEditors";					// SDI のテキストエディタ（拡張設定）
		public const string KEY_ENABLE_IGNORE_REMOTE_INFO = "EnableIgnoreRemoteInfo";	// ignoreRemoteInfo の機能を有効にする（拡張設定）
		public const string KEY_FORM_MAXIMUM_SIZE = "FormMaximumSize";					// フォームの最大サイズ（拡張設定）
		public const string KEY_RESUME_LATEST_NOTIFY_MENU_ITEM = "ResumeLatestNotifyMenuItem";	// 最後に選択された notifyIconMenu を記憶する（拡張設定）
		public const string KEY_IGNORE_REMOTE_BUTTON_BACK_COLOR = "IgnoreRemoteButtonBackColor";// リモート側のボタンに設定された背景色を無視する（拡張設定）
		public const string KEY_TOGGLE_CLICK_THROUGH_HOTKEY = "ToggleClickThroughHotKey";	// Click-Through を切り替えるホットキー（拡張設定）
		public const string KEY_HIDE_TAB_IMAGE_DURING_BUTTON_DRAG = "HideTabImageDuringButtonDrag";	// ボタンの移動中はタブの背景画像を隠す（拡張設定）
		public const string KEY_BUTTON_TOOLTIP_WIDTH = "ButtonToolTipWidth";			// ボタンのツールチップの横幅（拡張設定）
		public const string KEY_TOP_MOST_RELEASE_TIME = "TopMostReleaseTime";			// TopMost を解除する時間(秒)（拡張設定）
		public const string KEY_EXPERT_MODE = "ExpertMode";								// エキスパート用で起動するか否か（拡張設定）
		private const string KEY_WINDOW_RECTANGLE = "WindowRectangle";					// ウィンドウの位置とサイズ
		private const string KEY_SELECTED_TAB_PAGE_TEXT = "SelectedTabPageText";		// 最後に選択されていたタブ名
		private const string KEY_APP_ENVIRON = "AppEnviron";							// 最後に選択されていたアプリ環境
		private const string KEY_LATEST_NOTIFY_MENU_ITEM = "LatestNotifyMenuItem";		// 最後に選択された notifyIconMenu のアイテム名
		private const string KEY_LAST_HELP_FILE_CHECK = "LastHelpFileCheck";			// 前回のヘルプファイルの更新をチェックした日時
		// ccMushroom.ini
#if ENABLE_LOGON_REQUIRED
		private const string LOGON_EXPIRES_SECTION = "LogonExpires";					// [LogonExpires] セクション
		private enum logexp { dateTime, uid, pwd };										// ログオン履歴
#endif

		// このファイルがあるフォルダのサブディレクトリをスキャンする
		private const string CC_MUSHROOM_REMOTE_FILE_NAME = @"\ccMushroom.remote.xml";

		// <ccMushroom.remote.xml>@RemoteProgramsFolder
		public const string TAG_CONFIGURATION = "configuration";
		private const string TAG_ENTRY_DIR_ORDER = "entryDirOrder";
		private const string TAG_TAB_ICON_FILE = "tabIconFile";
		private const string TAG_NAME = "name";
		private const string ATTRIB_DELETE = "delete";
		// </ccMushroom.remote.xml>

		// このファイルがあるフォルダが ccMushroom 構成ファイルに取り込まれる
		private const string CC_TAKE_ME_FILE_NAME = @"\cctakeme.xml";

		// <cctakeme.xml>@RemoteProgramsFolderのサブディレクトリ
		public const string TAG_APPLICATION = "application";
		public const string TAG_TAB_TEXT = "tabText";
		public const string TAG_BUTTON_TEXT = "buttonText";
		public const string TAG_APP_NAME = "appName";
		public const string TAG_COMMAND_LINE = "commandLine";
		public const string TAG_WORKING_DIRECTORY = "workingDirectory";
		public const string TAG_COMMENT = "comment";
		private const string TAG_ONCE_COPY = "onceCopy";			// 廃止した。今後は <copyMode> で設定する
		private const string TAG_COPY_MODE = "copyMode";
		private const string TAG_NOT_OVERWRITE_CONFIG = "notOverwriteConfig";
		private const string TAG_NOT_OVERWRITE_XML = "notOverwriteXml";
		private const string TAG_NOT_OVERWRITE_INI = "notOverwriteIni";
		public/*private*/ const string TAG_SECTION = "section";
		public/*private*/ const string TAG_KEY = "key";

		private const string ATTRIB_NEW = "new";
		public const string ATTRIB_FILE_NAME = "fileName";
		public const string ATTRIB_NAME = "name";
		public const string ATTRIB_ENABLED = "enabled";
		public const string ATTRIB_AUTO_EXEC = "autoExec";
		public const string ATTRIB_BACK_COLOR = "backColor";
		private const string ATTRIB_DEL_LOCAL_FILES = "delLocalFiles";
		// </cctakeme.xml>

		private enum cm { normal, onceCopy, shortCut, existCopy };		// コピーモード normal:通常のコピー onceCopy:一度だけコピーする shortCut:既存のアプリに対するショートカット existCopy:既にコピーされて存在しているアプリ

		public const string CC_CONFIGURATION_FILE_NAME = "~ccConfiguration.xml";

		// <~ccConfiguration.xml>						// ccMushroom 構成ファイル
		//private const string TAG_CONFIGURATION = "configuration";
		//private const string TAG_APP_ENVIRON = "appEnviron";

		private const string ATTRIB_REMOTE_PROGRAMS_FOLDER = "remoteProgramsFolder";
		// </~ccConfiguration.xml>

		public const string CC_CONFIGURATION_IMPORT_FILE_NAME = "ccConfiguration.import.xml";

		// <ccConfiguration.import.xml>					// ccMushroom インポート用の構成ファイル
		public const string TAG_DEFAULT_TAB_TEXT = "defaultTabText";	// 環境設定時のデフォルト タブ名
		private string defaultTabText = "お気に入り";					// デフォルトのタブ名

		public const string ATTRIB_IMPORTED = "imported";
		public const string TAG_ICON_FILE = "iconFile";
		// </ccConfiguration.import.xml>

		// ccConfiguration.log
		public const string CC_CONFIGURATION_LOG_FILE_NAME = "~ccConfiguration.log";

		private const string J1_LAUNCHER_FILE_NAME = @"\Launcher.xml";

		// <Jauncher.xml>								// J1Launcher 用の設定ファイル
		private const string TAG_J1_LAUCHER_INFO = "LAUCHER_INFO";
		private const string TAG_J1_PATH = "Path";						// 実ディレクトリを設定する（未設定の場合は remoteProgramsName と同じフォルダ）
		private const string TAG_J1_PRIORITY = "Priority";
		private const string TAG_J1_STRING = "string";
#if ENABLE_LOGON_REQUIRED
		private const string TAG_J1_LOGON_REQUIRED = "logonRequired";	// ログオンを要求する
		private const string ATTR_J1_DIRPATH = "dirPath";				// アクティブ ディレクトリのパス
		private const string ATTR_J1_DOMAIN = "domain";					// ログオン先のドメイン名
		private const string ATTR_J1_EXPIRES = "expires";				// ログオンの有効（末尾が D:日 H:時間 M:分 S:秒）
#endif
#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
		private const string TAG_J1_PROGRAM_SUB_DIR_NAME = "programSubDirName";	// リモート側のプログラム フォルダの実際にプログラムを置いているサブ ディレクトリ名
#else
		private const string ATTR_J1_PATH_CURRENT = "current";			// lnc*.xml ファイルのパスは、現在のリモート ディレクトリを使う（LAUCHER_INFO/Path@Launcher.xml は使わない）
#endif
		// </Launcher.xml>

		private const string J1_LNC_TAB_PREFIX_FILE_NAME = @"\lnc";

		// <lncTAB.xml>									// J1Launcher 用のタブ設定ファイル
		private const string TAG_J1_TAB_INFO = "TAB_INFO";
		private const string TAG_J1_TABCAPTION = "TabCaption";
		private const string TAG_J1_SUBDIR_NAME = "subDirName";
		private const string TAG_J1_EXEINFO = "ExeInfo";
		private const string TAG_J1_EXE_INFO = "EXE_INFO";
		private const string TAG_J1_CMDEXE = "CmdExe";
		private const string TAG_J1_CMDOPTION = "CmdOption";
		private const string TAG_J1_CMDCAPTION = "CmdCaption";
		private const string TAG_J1_COPYMODE = "CopyMode";
		private const string ATTR_J1_IMPORT_CCTAKEME_NOT_OVERWRITE_SETTING = "ImportCcTakeMeNotOverwriteSetting";
		// </lncTAB.xml>

		public const string CC_APP_ENVIRONMENT_SETTING_FILE_NAME = "ccAppEnvironmentSetting.xml";
#if RESUME_APP_ENVIRONMENT
		public const string CC_LATEST_APP_ENVIRONMENT_FILE_NAME = "~ccLatestAppEnvironment.xml";
#endif

		// <ccAppEnvironmentSetting.xml>
		//private const string TAG_CONFIGURATION = "configuration";
		public const string TAG_APP_ENVIRON = "appEnviron";
		public const string TAG_ITEM = "item";
		public const string ATTRIB_FORM_BACK_COLOR = "formBackColor";
		//public const string TAG_APPLICATION = "application";
		public const string TAG_APP_SETTING_CONFIG = "appSettingConfig";
		public const string TAG_APP_SETTING_INI = "appSettingIni";
		public const string TAG_APP_SETTING_XML = "appSettingXml";
		//private const string TAG_SECTION = "section";
		//private const string TAG_KEY = "key";

		//public const string ATTRIB_ENABLED = "enabled";
		//public const string ATTRIB_FILE_NAME = "fileName";
		public const string ATTRIB_ENVIRONMENT = "environment";
		public const string ATTRIB_COPY_FROM = "copyFrom";
		//public const string ATTRIB_NAME = "name";
		public const string ATTRIB_VALUE = "value";
		/*private const string ATTRIB_CHANGED = "changed";		// for CC_CONFIGURATION_FILE_NAME*/
		// </ccAppEnvironmentSetting.xml>

		private bool firstRunning = true;
		private bool nowLoading = true;

		// ccMushroom.exe.config
		public static string latestProgramFolder;		// 最新のプログラム フォルダを置いているフォルダ名
		public static string urlMailToDeveloper;		// 問い合わせ先の url
		public static string[] remoteProgramsNames;		// リモート側のプログラム名称
		public static string[] remoteProgramsFolders;	// リモート側のプログラムを置いているフォルダ名
		public static string[] localProgramsFolders;	// ローカル側のプログラムをコピーするフォルダ名
		public static bool[] enabledProgramsFolders;	// 有効/無効なプログラムフォルダの設定
		public static bool scanRemoteProgramsFolder;	// リモート側のプログラム フォルダをスキャンするかどうかの設定
		public static bool removeNoUseLocalProgramsFolder; // 未使用のローカル プログラムのサブディレクトリを検索して削除する
		public static bool alwaysInTasktray;			// 常にタスクトレイに常駐するか否か
		public static bool multiLineTab;				// タブを複数の行で表示する
		public static bool importTabAppearFirst;		// インポートされたタブを最初に表示する
		public static string[] hideTabPageText = null;	// 非表示にするタブ
		public static int autoWindowCloseTime;			// 自動的にウィンドウを閉じるまでの時間（分）
		public static string shortcutMngApplyTime;		// ショートカット管理で自動的に適用されるまでの時間(秒)
		public static string autoOpacityTimePercent;	// 自動的に半透明にする時間(秒)と透過率
		public static bool transparentWithClickThrough;	// 半透明の時はマウスのクリックを素通りさせる
		public static string textEditor = null;			// テキストエディタ
		public static bool movableButton;				// ボタンの移動を有効にする
		public static bool integrateExplorerContext;	// エクスプローラのコンテキストメニューに統合する
		public static bool smallApplicationIcon;		// スモール サイズでアプリケーションのアイコンを表示する
		public static Size buttonSize;					// ボタンのサイズ
		public static Font buttonFont;					// ボタンのフォント
		public static Color buttonTextColor;			// ボタンのテキストの色
		public static Color buttonBackColor;			// ボタンの背景色
		public static string hotKey;					// ホットキー

		public static string ccMushroomLocalFolder = null;	// ローカル側のプログラムをコピーするフォルダ名のルート

		// ccMushroom.ini
		public static string ccMushroomIniFileName = Application.StartupPath + "\\" + CC_MUSHROOM_INI_FILE_NAME;
		public static bool enableRemoveNoUseLocalProgramsFolder = false;
		private int exitDialogCountDown = 3;
		public static bool textFaviconUrlVisible = false;
		private Color formBackColor;
		private Color tabPageBackColor;
		public static Brush rectMarkForeColor = null;
		public static Brush rectMarkBackColor = null;
		public static List<string> sdiTextEditors;
		public static Rectangle windowRectangle;
		public static string selectedTabPageText = null;	// 現在選択されているタブ名を確保しておく
		public static bool enableIgnoreRemoteInfo = false;
		private bool resumeLatestNotifyMenuItem = true;
		private bool ignoreRemoteButtonBackColor = false;
		public static string toggleClickThroughHotKey = null;
		public static bool hideTabImageDuringButtonDrag = false;
		public static string buttonToolTipWidth = null;
		public static int topMostReleaseTime = 1;

		public static/*const*/ string appTitle = "ccMushroom";

		private System.Threading.Timer timerAutoWindowClose = null;	// 自動的にウィンドウを閉じるタイマ

		private XmlDocument xmlCcConfiguration = null;		// ccMushroom 構成ファイル

		private string[] copiedRemoteProgramsFolder;		// コピー済みのリモート プログラム フォルダ

		private StringBuilder ccConfigurationLog;			// ログ

		private List<string> netErrorRemote = new List<string>();
		private const string netErrorBalloonText = "最新版が取得できませんでした";

		private Dictionary<string, int> newAppTab = new Dictionary<string, int>();
		private const string newAppBalloonText = "アプリが更新されました";

		private TabPage[] appTabPages;						// タブページの配列
		private ImageList appTabIcons;						// タブページに表示するアイコンの配列
#if ENABLE_TAB_BACKGROUND
		private enum TabBtn { size, textColor, count };		// タブページの Tag に埋め込む ojbect
#endif

		private Button[] appButtons;						// アプリケーション起動用ボタン コントロールの配列
		private Icon[] appIcons;							// アプリケーション アイコンの配列

		public const string newAppIndicator = "*";			// 更新されたアプリケーション用のインジケータ
		private Icon newAppIcon = null;						// 更新されたアプリケーション用のアイコン

		public static bool ignoreRemoteInfo = false;		// リモート側の情報が取得できなかった時、それ以降はリモート側の情報を取得しない

		private string[] toolTipNames = { "buttonText", "リンク先: ", "コマンドライン: ", "作業フォルダ: ", "コメント: ", "バージョン: ", "更新日時: ", "アプリ環境: ", "TabIndex: ", "Tag: " };
		public enum tt { buttonText, appName, commandLine, workingDirectory, comment, version, lastWriteTime, appEnviron, tabIndex, tag, count };

#if MOVABLE_BUTTON_BY_DRAGDROP
		private Rectangle dragButtonPoint;					// ボタンのドラッグを開始したマウスのポイントとその時のボタンのポイント
		private int dragButtonTag = -1;						// ドラッグされているボタンのタグ
		private const int dragButtonTagWait = 0x3fff;		// ボタンのタグの重み
#endif
		private Cursor dragCursor = null;					// ドラッグ中のカーソル

		public const string iconsFolder = "~icons";
		public const string faviconUrlDllFileName = "favicon@url.dll.ico";

		private ToolStripComboBox toolStripComboAppEnviron = null;

		private const Int32 IDM_SET_ENVIRONMENT = 1000;
		//private const Int32 IDM_REFRESH_CCMUSHROOM_FORM = 1001;
		//private const Int32 IDM_EXIT_PROGRAM = 1002;
		//private const Int32 IDM_ABOUT_CCMUSHROOM = 1003;
		private const Int32 IDM_SHOW_HELPFILE = 1004;

		private Point notifyIconMenuLocation = Point.Empty;
		private ToolStripMenuItem[] notifyMenuTabPages = null;
		private ToolStripMenuItem[] notifyMenuButtons = null;

		private FormWindowState runWindowState;
		private KeyEventArgs formKeyDownArgs = null;		// フォーム上で押されたキーの状態
		private System.Windows.Forms.Timer timerTopMost = null;

		private const int HOTKEY_SHOW_CONTEXT_MENU = 0x0001;	// 0x0000～0xbfff 内の適当な値でよい。0xc000～0xffff は DLL 用なので 使用不可！
		private const int HOTKEY_TOGGLE_CLICK_THROUGH = 0x0002;

#if ENABLE_TAB_BACKGROUND
		public const string TAB_PAGE_SETTINGS_INI_FILE_NAME = "tabPageSettings.ini";	// tabPageSettings.ini ファイル名
		public const string KEY_ENABLED_BACKGROUND = "EnabledBackground";				// タブの背景画像を有効にする
		public const string KEY_BACKGROUND_IMAGE = "BackgroundImage";					// 背景画像のファイル名
		public const string KEY_BUTTON_SIZE = Program.CONSET_BUTTON_SIZE;				// ボタンのサイズ
		public const string KEY_BUTTON_TEXT_COLOR = "ButtonTextColor";					// ボタンのテキストの色
		private const string KEY_IMAGE_LAYOUT = "ImageLayout";							// イメージ レイアウト
		private const string KEY_TRANSPARENT_BUTTON = "TransparentButton";				// ボタンの背景を透過するか否か
		private const string KEY_RESUME_FORM_SIZE = "ResumeFormSize";					// フォーム サイズ保存して復元する

		public static readonly string tabPageSettingsIniFileName = Application.StartupPath + "\\" + TAB_PAGE_SETTINGS_INI_FILE_NAME;
#endif

#if FOR_WINDOWS7
		private TabPage previousSelectedPage = null;
		private System.Windows.Forms.Timer timerRecoverThumbnail = null;
		private const string KEY_FOR_JUMPLIST_TABPAGE = "ForJumpListTabPage";
		public const string KEY_ENABLED_TASKBAR_THUMBNAIL = "EnabledTaskbarThumbnail";
		public const string KEY_SET_FOREGROUND_WINDOW_DELAYTIME = "SetForegroundWindowDelayTime";
		private string forJumpListTabPage = null;
		public static bool enabledTaskbarThumbnail = true;
		public static int setForegroundWindowDelayTime = 200;
		List<string> runAsButtons = null;

		private const int TIMERID_TABPAGE_CLOSED = 1;
		private const int TIMERID_ARRANGEMENT_THUMBNAIL = 2;
		private const int TIMERID_SET_FOREGROUND_WINDOW = 3;
		private const int TIMERID_REMOVE_FOREGROUND_WINDOW_INDICATOR = 4;

		#region InteropServices
		[DllImport("user32.dll")]
		static extern short GetKeyState(int nVirtKey);

		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		extern static bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("user32.dll", SetLastError = true)]
		static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

		private const int SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
		private const int SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
		#endregion
#endif

		#region アクセサ(Accessor)
		public XmlDocument XmlCcConfiguration
		{
			get { return xmlCcConfiguration; }
		}

		public TabPage[] AppTabPages
		{
			get { return appTabPages; }
		}

		public ImageList AppTabIcons
		{
			get { return appTabIcons; }
		}

		public Button[] AppButtons
		{
			get { return appButtons; }
		}

		public Icon[] AppIcons
		{
			get { return appIcons; }
		}

		public string DefaultTabText
		{
			get { return defaultTabText; }
			//set { defaultTabText = value; }
		}
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ccMushroom()
		{
			InitializeComponent();

			try
			{
				toolStripComboAppEnviron = new System.Windows.Forms.ToolStripComboBox();
				toolStripComboAppEnviron.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
				toolStripComboAppEnviron.Name = "toolStripComboAppEnviron";
				toolStripComboAppEnviron.Size = new System.Drawing.Size(100 - 20, 20);
				toolStripComboAppEnviron.BackColor = /*Color.GhostWhite*/SystemColors.ControlLight;
				//toolStripComboAppEnviron.FlatStyle = FlatStyle.Flat;
				toolStripComboAppEnviron.ToolTipText = "現在のアプリケーションの環境";
				toolStripComboAppEnviron.SelectedIndexChanged += new System.EventHandler(this.toolStripComboAppEnviron_SelectedIndexChanged);

				toolStripProgressBar.Style = ProgressBarStyle.Marquee;

				appTabIcons = new System.Windows.Forms.ImageList(this.components);
				appTabIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
				appTabIcons.ImageSize = new System.Drawing.Size(16, 16);
				appTabIcons.TransparentColor = System.Drawing.Color.Transparent;
				tabControl.ImageList = appTabIcons;

				notifyIcon.Visible = false;
				notifyIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ccMushroom.App.ico"), 16, 16);

				notifyIconMenu.Tag = 0;

#if (!SHOW_STARTING_SPLASH_FORM)
				this.Activated += new System.EventHandler(this.ccMushroom_Activated);
#endif

#if ENABLE_TAB_BACKGROUND
				this.toolStripMenuImageLayout.DropDownOpening += new System.EventHandler(this.toolStripMenuImageLayout_DropDownOpening);
				this.tileToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuImageLayout_Click);
				this.centerToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuImageLayout_Click);
				this.stretchToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuImageLayout_Click);
				this.zoomToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuImageLayout_Click);
				this.toolStripMenuTransButton.Click += new System.EventHandler(this.toolStripMenuTransButton_Click);
				this.toolStripMenuResumeFomeSize.Click += new System.EventHandler(this.toolStripMenuResumeFormSize_Click);
#endif

#if FOR_WINDOWS7
				hideTabImageDuringButtonDrag = true;

				this.tabControl.Anchor -= AnchorStyles.Bottom;
				this.Height += 4;
				this.tabControl.Anchor |= AnchorStyles.Bottom;

				this.toolStripMenuForJumpListTabPage.Click += new System.EventHandler(this.toolStripMenuForJumpListTabPage_Click);
#else
				// contextMenuButton
				contextMenuButton.Items.RemoveAt(3);	// セパレータ
				contextMenuButton.Items.RemoveAt(3);	// menuRunAsAdministrator

				// contextMenuTab
				contextMenuTab.Items.RemoveAt(5);		// セパレータ
				contextMenuTab.Items.RemoveAt(5);		// toolStripMenuForJumpListTabPage
#endif

				notifyIconMenu.Text = appTitle + "NotifyIconMenu";	// 本体のタイトルとダブるので変えておく
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		#region ccMushroom イベント
		/// <summary>
		/// ccMushroom_Load
		/// </summary>
		private void ccMushroom_Load(object sender, System.EventArgs e)
		{
			try
			{
				if ( (runWindowState = this.WindowState) == FormWindowState.Minimized )
				{
					this.WindowState = FormWindowState.Normal;
				}

				this.MinimumSize = new Size(640 / 2, 480 / 2)/*this.Size*/;	// WM_SIZING を捕まえて処理する代わり

				// ccMushroom フォームを初期化する
				if ( !InitCcMushroomForm() )
				{
					this.Close();
					return;
				}

				// CCMUSHROOM.INI を読み込む
				GetPrivateProfile();

				this.BackColor = formBackColor;

				Assembly ccAssembly = Assembly.GetExecutingAssembly();	// 自分自身の Assembly を取得
				Version ccVer = ccAssembly.GetName().Version;			// バージョンの取得

				DateTime buildDateTime = new DateTime(2000, 1, 1);
				TimeSpan verSpan = new TimeSpan(ccVer.Build * TimeSpan.TicksPerDay + ccVer.Revision * 2 * TimeSpan.TicksPerSecond);
				buildDateTime += verSpan;

				//toolStripVersion.Text = "AssemblyVersion: " + ccVer + "  Builded: " + buildDateTime.ToString("yyyy/MM/dd HH:mm") + "   @Win" + System.Environment.OSVersion.Version.Major + "." + System.Environment.OSVersion.Version.Minor;
				string version = "Version " + ccVer.Major + "." + ccVer.Minor.ToString("D2") + (Program.debMode ? "." + buildDateTime.ToString("yyMMdd.HHmm") : "") + (Program.expertMode ? " ex" : "");
				toolStripVersion.Text = version + (Program.debMode ? "  " + "@Win" + Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor : "");

				// toolTip のイベントをセットする
				SetButtonToolTipEvent();

#if (SHOW_STARTING_SPLASH_FORM)
				// リモート側のプログラム フォルダをスキャンする
				ScanRemotePrograms();

				// ccMushroom 構成ファイルを読み込む
				ReadCcConfiguration();

				// ccAppEnvironmentSetting ファイルを読み込んで各アプリケーションの設定を編集する（インポートしたアプリにも環境を反映させる為、ReadCcConfiguration()前だったのを後から呼ぶようにした）
				ReadCcAppEnvironmentSetting(GetCurrentEnvironment());

				// コントロールを作成＆配置する
				CreateControlsDeployment();

				// 自動的にフォームを透過するタイマ
				StartTimerAutoOpacityForm();

				// スプラッシュウィンドウを消す
				//this.Activate();
				SplashForm.CloseSplash();
				nowLoading = false;
#endif

#if ENABLE_TAB_BACKGROUND
				RefreshSelectedTabPage();
#endif

				// ホットキーを登録する
				RegisterHotKey(hotKey);

				// 自動実行するアプリケーションを起動する
				AutoExecProcess();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
			}
		}

		/// <summary>
		/// ccMushroom_Shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccMushroom_Shown(object sender, EventArgs e)
		{
			try
			{
				if ( (selectedTabPageText == null) && (tabControl.SelectedTab != null) )
				{
					SetSelectedTabPageText(GetPlainTabPageText(tabControl.SelectedTab.Text));
				}

				notifyIcon.Visible = alwaysInTasktray;

				if ( alwaysInTasktray )
				{
					this.Visible = (runWindowState != FormWindowState.Minimized);
				}
				else
				{
					this.WindowState = runWindowState;
				}

				ShowNewAppBalloonTip();

				SetJumpList();

				AddThumbnailAllTabPages(true);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// WndProc
		/// </summary>
		protected override void WndProc(ref Message m)
		{
			if ( m.Msg == api.WM_SYSCOMMAND )
			{
				try
				{
					switch ( m.WParam.ToInt32() )
					{
						case IDM_SET_ENVIRONMENT:
							if ( nowLoading )
								return;

							StopAutoWindowCloseTimer();

							if ( timerOpacity.Enabled )
							{
								timerOpacity.Stop();
							}

							bool tempAlwaysInTaskTray = alwaysInTasktray;

							SetEnvironmentDlg dlg = new SetEnvironmentDlg();
							dlg.checkForSelectedTab.Enabled = ((tabControl.SelectedTab != null) && (tabControl.SelectedTab.BackgroundImage != null));
							if ( dlg.ShowDialog(this) == DialogResult.OK )
							{
#if ENABLE_TAB_BACKGROUND
								RemoveAllTabResumeFormSize();
#endif

								if ( tempAlwaysInTaskTray != alwaysInTasktray )
								{
									SystemMenuForExitProgram(alwaysInTasktray);
									notifyIcon.Visible = alwaysInTasktray;
								}

								if ( dlg.checkReload.Checked )
								{
									RefreshCcMushroomForm();
								}

								SetButtonToolTipEvent();

								UnregisterHotKey();
								RegisterHotKey(hotKey);
							}
							dlg.Dispose();

							StartTimerAutoOpacityForm();

							StartTimerAutoWindowClose();
							return;

						/*case IDM_REFRESH_CCMUSHROOM_FORM:
							RefreshCcMushroomForm();
							return;*/

						/*case IDM_EXIT_PROGRAM:
							this.Close();
							return;*/

						/*case IDM_ABOUT_CCMUSHROOM:
							AboutDlg aboutDlg = new AboutDlg();
							aboutDlg.ShowDialog(this);
							return;*/

						case IDM_SHOW_HELPFILE:
							ShowHelpFile();
							return;

						case (int)api.SC_CLOSE:                // [X] ボタン、コントロールメニューの「閉じる」、コントロールボックスのダブルクリック、Atl+F4 などにより閉じられようとしている？
							if ( alwaysInTasktray )
							{
								this.Visible = false;
								if ( exitDialogCountDown != 0 )
								{
									ExitDlg exitDlg = new ExitDlg(appTitle, exitDialogCountDown);
									exitDlg.Show(this);
								}
								return;
							}
							break;

						default:
							break;
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
			}
			else if ( m.Msg == api.WM_HOTKEY )
			{
				try
				{
					if ( (int)m.WParam == HOTKEY_SHOW_CONTEXT_MENU )
					{
						notifyIconMenu.Tag = HOTKEY_SHOW_CONTEXT_MENU;

						/*IntPtr hWnd = api.GetForegroundWindow();*/

						if ( !this.Visible )
						{
							notifyIconMenuLocation = Point.Empty;
							this.TopMost = true;	// タスクバーに表示させない為のおまじない
							notifyIconMenu.Show(new Point(Cursor.Position.X, Cursor.Position.Y));
							this.TopMost = false;
						}
						else
						{
							notifyIconMenuLocation = new Point(Cursor.Position.X, Cursor.Position.Y);
							Type handle = notifyIcon.GetType();
							notifyIconMenu.Visible = false;	// カーソルの左上に一瞬メニューが表示されてしまう対策
							handle.InvokeMember("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, notifyIcon, null);
						}
						
						/*if ( hWnd != null )
						{
							//api.SetWindowPos(hWnd, (IntPtr)api.HWND_TOPMOST, 0, 0, 0, 0, api.SWP_NOSIZE | api.SWP_NOMOVE);
							api.SetForegroundWindow(hWnd);
						}*/

						/*notifyIconMenu.GetType().InvokeMember("ShowInTaskbar", System.Reflection.BindingFlags.NonPublic |	System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Instance, null, notifyIconMenu, new Object[] { Cursor.Position.X, Cursor.Position.Y });*/
						/*int index = api.GWL_EXSTYLE;
						index = -16;	// STYLE
						uint exStyle = (uint)api.GetWindowLong32(notifyIconMenu.Handle, index);
						//exStyle = (uint)(exStyle & ~api.WS_EX_APPWINDOW);
						//exStyle |= api.WS_EX_TOOLWINDOW;
						//int WS_EX_NOACTIVATE = 0x08000000;
						//exStyle |= (uint)WS_EX_NOACTIVATE;
						exStyle = 0xffffffff;
						api.SetWindowLong(notifyIconMenu.Handle, index, exStyle);
						uint NOMOVE = 0x0002;
						uint FRAMECHANGED = 0x0020;
						api.SetWindowPos(notifyIconMenu.Handle, IntPtr.Zero, 0, 0, 0, 0, NOMOVE | api.SWP_NOZORDER | api.SWP_NOSIZE | FRAMECHANGED);*/
					}
					else if ( (int)m.WParam == HOTKEY_TOGGLE_CLICK_THROUGH )
					{
						ToggleClickThrough();
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
			}

			base.WndProc(ref m);
		}

		/// <summary>
		/// ccMushroom_KeyDown
		/// </summary>
		private void ccMushroom_KeyDown(object sender, KeyEventArgs e)
		{
			formKeyDownArgs = e;
		}

		/// <summary>
		/// ccMushroom_KeyUp
		/// this.KeyPreview = true; としておかないとこのイベントは発生しない
		/// </summary>
		private void ccMushroom_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if ( nowLoading )
				return;

			if ( e.KeyCode == Keys.F1 )
			{
				ShowHelpFile();
			}
			else if ( e.KeyCode == Keys.F5 )
			{
				if ( e.Control )
				{
					RefreshCcMushroomForm();
				}
				else
				{
					RefreshSelectedTabPage(true);
				}
			}
			/*else if ( e.KeyCode == Keys.F4 )
			{
				if ( e.Control && alwaysInTasktray )
				{
					this.Close();
				}
			}*/
			else if ( e.KeyCode == Keys.Enter )
			{
				AppButtonPushedWithEnter();
			}

			formKeyDownArgs = null;
		}

		/// <summary>
		/// ccMushroom_Activated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccMushroom_Activated(object sender, EventArgs e)
		{
			/*try
			{
				if ( nowLoading )
					return;

				DisTransparentWithClickThrough(true);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}*/
		}

#if (!SHOW_STARTING_SPLASH_FORM)
		/// <summary>
		/// ccMushroom_Activated
		/// </summary>
		private void ccMushroom_Activated(object sender, System.EventArgs e)
		{
			this.Activated -= new System.EventHandler(this.ccMushroom_Activated);

			try
			{
				RefreshCcMushroomForm();
			}
			catch ( Exception exp )
			{
				MessageBox.Show (exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close ();
			}
		}
#endif

		/// <summary>
		/// フォームのサイズが変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccMushroom_SizeChanged(object sender, EventArgs e)
		{
			try
			{
				if ( nowLoading )
					return;

				if ( this.WindowState == FormWindowState.Minimized )
				{
					if ( !alwaysInTasktray )
					{
						//this.Hide();
						this.Visible = false;
						if ( !notifyIcon.Visible )
						{
							notifyIcon.Visible = true;
						}
					}
				}
				else
				{
#if ENABLE_TAB_BACKGROUND
					if ( (tabControl.SelectedTab != null) && (tabControl.SelectedTab.BackgroundImage == null) )	// タブの背景画像は指定無し？
					{
						windowRectangle.Size = this.Size;
					}
#endif

					DisTransparentWithClickThrough(true);

					RefreshSelectedTabPage(false);

#if _FOR_WINDOWS7
					if ( enabledTaskbarThumbnail )
					{
						Application.DoEvents();
						if ( this.WindowState == FormWindowState.Maximized )
						{
							foreach ( TabPage tabPage in AppTabPages )
							{
								UpdatePreviewBitmap(tabPage);
							}
						}
					}
#endif
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ccMushroom のフォームがクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccMushroom_Click(object sender, EventArgs e)
		{
			this.TopMost = false;

			ResetOpaqueForm();

#if MOVABLE_BUTTON_BY_DRAGDROP
			if ( movableButton )
			{
				try
				{
					if ( tabControl.ContextMenuStrip == null )
					{
						toolTip.Active = true;
						tabControl.ContextMenuStrip = this.contextMenuTab;
					}

					if ( (0 <= dragButtonTag) && (dragButtonTag < dragButtonTagWait) )
					{
						appButtons[dragButtonTag].ContextMenuStrip = this.contextMenuButton;
						appButtons[dragButtonTag].Invalidate();
						dragButtonTag = -1;
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}

				RefreshSelectedTabPage(false);
			}
#endif
		}

		/// <summary>
		/// ccMushroom_FormClosing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ccMushroom_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				// アプリケーションの状態を保存する
				//string iniFileName = Application.StartupPath + CCMUSHROOM_INI_FILE_NAME;

				// KEY_SELECTED_TAB_PAGE_TEXT
				string _selectedTabPageText = (tabControl.SelectedTab != null) ? GetPlainTabPageText(tabControl.SelectedTab.Text) : null;
				api.WritePrivateProfileString(RESUME_SECTION, KEY_SELECTED_TAB_PAGE_TEXT, _selectedTabPageText/*selectedTabPageText*/, ccMushroomIniFileName);

				// KEY_APP_ENVIRON
				api.WritePrivateProfileString(RESUME_SECTION, KEY_APP_ENVIRON, (toolStripComboAppEnviron.Enabled) ? toolStripComboAppEnviron.SelectedItem.ToString() : null, ccMushroomIniFileName);

				// KEY_WINDOW_RECTANGLE
				Point formPoint = this.Location;
				if ( (this.WindowState == FormWindowState.Normal) && (0 <= formPoint.X && 0 <= formPoint.Y) )
				{
					string formSize = this.Size.Width + "," + this.Size.Height;					// 現在のフォーム サイズ
#if ENABLE_TAB_BACKGROUND
					if ( (tabControl.SelectedTab != null) && (tabControl.SelectedTab.BackgroundImage != null) )	// 背景画像の指定あり？
					{
						if ( GetTabResumeFormSize(tabControl.SelectedTab.Name) != Size.Empty )	// タブのフォーム サイズが保存されている？
						{
							formSize = windowRectangle.Width + "," + windowRectangle.Height;	// 背景画像の指定が無いタブのフォーム サイズを保存する
						}
					}
#endif
					api.WritePrivateProfileString(RESUME_SECTION, KEY_WINDOW_RECTANGLE, formPoint.X + "," + formPoint.Y + "," + formSize, ccMushroomIniFileName);
				}

				UnregisterHotKey();

				ClearNotifyIconMenu();

				notifyIcon.Visible = false;

#if ENABLE_LOGON_REQUIRED
				try
				{
					// 期限切れのログオン有効期限を削除する
					byte[] returnedBytes = new byte[0xffff];
					int length = (int)api.GetPrivateProfileSection(LOGON_EXPIRES_SECTION, returnedBytes, (uint)returnedBytes.Length, ccMushroomIniFileName);
					if ( length != 0 )
					{
						string returnedKey = Encoding.Default.GetString(returnedBytes, 0, length - 1);
						List<string> logonExpiresKeyValues = new List<string>(returnedKey.Split('\0'));

						foreach ( string logonExpiresKeyValue in logonExpiresKeyValues )
						{
							string[] keyValue = logonExpiresKeyValue.Split('=');
							string[] _logexp = keyValue[1].ToString().Split('｜');
							DateTime expiresDateTime = DateTime.Parse(_logexp[(int)logexp.dateTime]).AddDays(14);	// 最後のログオンから１４日経過させる
							if ( expiresDateTime < DateTime.Now )
							{
								api.WritePrivateProfileString(LOGON_EXPIRES_SECTION, keyValue[0], null, ccMushroomIniFileName);
							}
						}
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
#endif

				IntPtr hWnd;
				// ヘルプが開かれていれば閉じる
				if ( (hWnd = api.FindWindow(null, Application.ProductName + " document")) != IntPtr.Zero )
				{
					api.PostMessage(hWnd, api.WM_CLOSE, 0, 0);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region ccMushroom 関数
		#region フォームの表示関連
		/// <summary>
		/// ccMushroom フォームを最新の情報に更新する
		/// </summary>
		private void RefreshCcMushroomForm()
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;

				RemoveAllThumbnail();

				RemoveNewAppBalloonTip();

				ResetOpaqueForm();
				ignoreRemoteInfo = false;

				this.BackColor = formBackColor;

				if ( tabControl.TabCount != 0 )
				{
					/*selectedTabPageText = tabControl.SelectedTab.Text;
					if ( selectedTabPageText.EndsWith(newAppIndicator) )
					{
						selectedTabPageText = selectedTabPageText.Substring(0, selectedTabPageText.Length - 1);
					}*/

					for ( int i = 0; i < appButtons.Length; i++ )
					{
						if ( appButtons[i] == null )
							continue;
						appButtons[i].Dispose();
					}
					appButtons = null;

					for ( int i = 0; i < appIcons.Length; i++ )
					{
						if ( appIcons[i] == null )
							continue;
						api.DestroyIcon(appIcons[i].Handle);
						appIcons[i].Dispose();
					}
					appIcons = null;

					tabControl.Controls.Clear();
					tabControl.Multiline = multiLineTab;

					for ( int i = 0; i < appTabPages.Length; i++ )
					{
						appTabPages[i].Dispose();
					}
					appTabPages = null;

					appTabIcons.Images.Clear();

					GC.Collect();

					Application.DoEvents();
				}

#if (SHOW_REFRESHING_INTERNAL_PROGRESS_MESSAGE)
				// 進捗メッセージを表示する
				VisibleProgressMessageControls(true);
#else
				// スプラッシュウィンドウを表示する
#if (USE_MULTI_THRED_SPLASH_FORM)
				SplashForm.ShowSplash(this);
#else
				SplashForm splash = new SplashForm(this);
				splash.Show();
				Application.DoEvents();
#endif
				nowLoading = true;
#endif
	
				// アプリの環境名をセットする
				string _comboAppEnvironText = (toolStripComboAppEnviron.SelectedItem != null) ? toolStripComboAppEnviron.SelectedItem.ToString() : null;
				SetComboAppEnviron(_comboAppEnvironText);

				bool _toolStripComboAppEnvironEnabled = toolStripComboAppEnviron.Enabled;
				toolStripComboAppEnviron.Enabled = false;
				statusStrip.Update();

				// リモート側のプログラム フォルダをスキャンする
				//bool saveScanRemoteProgramsFolder = scanRemoteProgramsFolder;
				//scanRemoteProgramsFolder = true;
				ScanRemotePrograms();
				//scanRemoteProgramsFolder = saveScanRemoteProgramsFolder;

				// ccMushroom 構成ファイルを読み込む
				ReadCcConfiguration();

				// ccAppEnvironmentSetting ファイルを読み込んで各アプリケーションの設定を編集する（インポートしたアプリにも環境を反映させる為、ReadCcConfiguration()前だったのを後から呼ぶようにした）
				ReadCcAppEnvironmentSetting(GetCurrentEnvironment());

				// コントロールを作成＆配置する
				CreateControlsDeployment();

#if (SHOW_REFRESHING_INTERNAL_PROGRESS_MESSAGE)
				// 進捗メッセージを消す
				VisibleProgressMessageControls(false);
#else
				// スプラッシュウィンドウを消す
				//this.Activate();
				SplashForm.CloseSplash();
				nowLoading = false;
#endif

#if ENABLE_TAB_BACKGROUND
				RefreshSelectedTabPage();
#endif

				ShowNewAppBalloonTip();

				AddThumbnailAllTabPages(false);

				toolStripComboAppEnviron.Enabled = _toolStripComboAppEnvironEnabled;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		/// <summary>
		/// ccMushroom フォームを初期化する
		/// </summary>
		private bool InitCcMushroomForm()
		{
			try
			{
				IntPtr sysMenuHandle = api.GetSystemMenu(this.Handle, false);

				//It would be better to find the position at run time of the 'Close' item, but...
				api.InsertMenu(sysMenuHandle, 5, api.MF_BYPOSITION | api.MF_SEPARATOR, 0, string.Empty);
				api.InsertMenu(sysMenuHandle, 6, api.MF_BYPOSITION, IDM_SET_ENVIRONMENT, "環境設定(&O)...");
				//api.InsertMenu(sysMenuHandle, 7, api.MF_BYPOSITION, IDM_ABOUT_CCMUSHROOM, "バージョン情報(&A)...");
				//api.InsertMenu(sysMenuHandle, 7, api.MF_BYPOSITION, IDM_REFRESH_CCMUSHROOM_FORM, "最新の情報に更新(&R)");
				api.InsertMenu(sysMenuHandle, 7, api.MF_BYPOSITION, IDM_SHOW_HELPFILE, "ヘルプ(&H)");

				Bitmap sysOptionIcon = Properties.Resources.sysOption.ToBitmap();
				//Color backColor = sysOptionIcon.GetPixel(1, 1);
				//sysOptionIcon.MakeTransparent(backColor);
				IntPtr hIcon = sysOptionIcon.GetHbitmap();
				bool res = api.SetMenuItemBitmaps(sysMenuHandle, IDM_SET_ENVIRONMENT, api.MF_BYCOMMAND, hIcon, hIcon);
				//int errorNo = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

				Bitmap sysHelpIcon = Properties.Resources.sysHelp.ToBitmap();
				hIcon = sysHelpIcon.GetHbitmap();
				res = api.SetMenuItemBitmaps(sysMenuHandle, IDM_SHOW_HELPFILE, api.MF_BYCOMMAND, hIcon, hIcon);

				if ( alwaysInTasktray )
				{
					//api.RemoveMenu(sysMenuHandle, 8, api.MF_BYPOSITION);
					//api.InsertMenu(sysMenuHandle, 8, api.MF_BYPOSITION, IDM_EXIT_PROGRAM, "プログラムを終了する(&X)");
					SystemMenuForExitProgram(true);
				}

				this.Text = appTitle;
				notifyIcon.Text = appTitle;

				tabControl.Multiline = multiLineTab;

				statusStrip.Items.Insert(0, toolStripComboAppEnviron);
				//Padding padding = statusStrip.Items[0].Padding;
				//statusStrip.Items[0].Padding = new Padding(padding.Left, 4/*padding.Top*/, padding.Right, padding.Bottom);
				//toolStripComboAppEnviron.ComboBox.Location = new Point(2, 2);

				// アプリの環境名をセットする
				SetComboAppEnviron(null);

				toolStripProgressBar.Visible = false;

				// 自分自身の Assembly を取得
				Assembly ccAssembly = Assembly.GetExecutingAssembly();

				// 更新されたアプリケーション用のアイコン
				newAppIcon = new Icon(ccAssembly.GetManifestResourceStream("ccMushroom.images.newApp.ico"));

#if MOVABLE_BUTTON_BY_DRAGDROP
				// ドラッグ中のカーソル
				dragCursor = new Cursor(ccAssembly.GetManifestResourceStream("ccMushroom.Resources.drag.cur"));
#endif

				try
				{
					// アイコン フォルダ
					DirectoryInfo diIconFilesFolder = new DirectoryInfo(Application.StartupPath + "\\" + iconsFolder);

					if ( !diIconFilesFolder.Exists )
					{
						diIconFilesFolder.Create();
						diIconFilesFolder.Attributes = FileAttributes.Hidden;
					}

					string tabFavoriteFile = Application.StartupPath + "\\" + iconsFolder + "\\" + "tabお気に入り.ico";
					if ( !File.Exists(tabFavoriteFile) )
					{
						Icon tabFavorite = Properties.Resources.tabお気に入り;
						using ( FileStream fileStream = new FileStream(tabFavoriteFile, FileMode.Create) )
						{
							tabFavorite.Save(fileStream);
						}
					}

					string faviconUrlDllFile = Application.StartupPath + "\\" + iconsFolder + "\\" + faviconUrlDllFileName;
					if ( !File.Exists(faviconUrlDllFile) )
					{
						Icon faviconUrlDll = Properties.Resources.faviconUrlDll;
						using ( FileStream fs = new FileStream(faviconUrlDllFile, FileMode.Create) )
						{
							faviconUrlDll.Save(fs);
						}
					}
				}
				catch ( Exception exp )
				{
					MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}

				if ( Program.debMode )
				{
					toolStripMousePosition.Visible = true;
					this.tabControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseMove);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			return true;
		}

		/// <summary>
		/// システムの「終了」メニューを挿入|削除する
		/// </summary>
		/// <param name="insert"></param>
		private void SystemMenuForExitProgram(bool insert)
		{
#if false
			IntPtr sysMenuHandle = api.GetSystemMenu(this.Handle, false);

			if ( insert )
			{
				api.MENUITEMINFO mii = new api.MENUITEMINFO();
				mii.cbSize = api.MENUITEMINFO.sizeOf;
				mii.fMask = /*api.MIIM_FTYPE | api.MIIM_STATE | */api.MIIM_ID | api.MIIM_STRING;
				//mii.fType = api.MF_STRING;
				//mii.fState = api.MFS_DEFAULT;
				mii.wID = IDM_EXIT_PROGRAM;
				mii.dwTypeData = "終了(&X)	Ctrl+F4";
				mii.cch = (uint)mii.dwTypeData.Length;
				api.InsertMenuItem(sysMenuHandle, 7, true, ref mii);

				Bitmap sysExitIcon = Properties.Resources.sysExit.ToBitmap();
				IntPtr hIcon = sysExitIcon.GetHbitmap();
				bool res = api.SetMenuItemBitmaps(sysMenuHandle, IDM_EXIT_PROGRAM, api.MF_BYCOMMAND, hIcon, hIcon);
			}
			else
			{
				api.RemoveMenu(sysMenuHandle, IDM_EXIT_PROGRAM, api.MF_BYCOMMAND);
			}
#endif
		}
		#endregion

		#region CCMUSHROOM_INI_FILE_NAME を読み込む
		/// <summary>
		/// CCMUSHROOM_INI_FILE_NAME を読み込む
		/// </summary>
		private void GetPrivateProfile()
		{
			try
			{
				if ( !File.Exists(ccMushroomIniFileName) )
				{
					using ( FileStream fs = File.Create(ccMushroomIniFileName) )
					{
						fs.Close();
					}
				}

				string remoteProgramsName, remoteProgramsFolder, localProgramsFolder, enabledProgramsFolder;
				StringBuilder returnedString = new StringBuilder(1024);

				for ( int i = 1; ; i++ )
				{
					// リモート側のプログラム名称
					api.GetPrivateProfileString(SETTINGS_SECTION, KEY_REMOTE_PROGRAMS_NAME + i, "プログラム名称" + i, returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
					remoteProgramsName = returnedString.ToString();

					// リモート側のプログラムを置いているフォルダ名
					if ( !GetPrivateProfileProgramsFolder(KEY_REMOTE_PROGRAMS_FOLDER + i, ccMushroomIniFileName, out remoteProgramsFolder, ref remoteProgramsFolders) )
						continue;
					if ( remoteProgramsFolder == null )
						break;

					// ローカル側のプログラムをコピーするフォルダ名
					if ( !GetPrivateProfileProgramsFolder(KEY_LOCAL_PROGRAMS_FOLDER + i, ccMushroomIniFileName, out localProgramsFolder, ref localProgramsFolders) )
						continue;
					localProgramsFolder = ccf.ReplaceCcMushroomLocalFolder(localProgramsFolder, true);

					// 有効/無効なプログラムフォルダの設定
					api.GetPrivateProfileString(SETTINGS_SECTION, KEY_ENABLED_PROGRAMS_FOLDER + i, false.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
					enabledProgramsFolder = returnedString.ToString();

					int count = remoteProgramsFolders.Length;

					// リモート側のプログラム名称のリアロケーション
					Array.Resize(ref remoteProgramsNames, count + 1);
					remoteProgramsNames[count] = remoteProgramsName;

					// リモート側のプログラムを置いているフォルダのリアロケーション
					Array.Resize(ref remoteProgramsFolders, count + 1);
					remoteProgramsFolders[count] = remoteProgramsFolder;
					if ( !ccf.AdjustFolderFormat(ref remoteProgramsFolders[count]) )
					{
						Array.Resize(ref remoteProgramsNames, count);
						Array.Resize(ref remoteProgramsFolders, count);
						break;
					}

					// ローカル側のプログラムをコピーするフォルダのリアロケーション
					Array.Resize(ref localProgramsFolders, count + 1);
					localProgramsFolders[count] = localProgramsFolder;
					if ( !ccf.AdjustFolderFormat(ref localProgramsFolders[count]) ||
						 ccf.IsSystemFolder(localProgramsFolders[count]) )
					{
						Array.Resize(ref remoteProgramsNames, count);
						Array.Resize(ref remoteProgramsFolders, count);
						Array.Resize(ref localProgramsFolders, count);
						break;
					}

					// 有効/無効なプログラムフォルダの設定のリアロケーション
					Array.Resize(ref enabledProgramsFolders, count + 1);
					enabledProgramsFolders[count] = bool.Parse(enabledProgramsFolder);
				}

				// removeNoUseLocalProgramsFolder の環境設定を有効にする（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_ENABLE_REMOVE_NO_USE_LOCAL_FOLDER, enableRemoveNoUseLocalProgramsFolder.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				enableRemoveNoUseLocalProgramsFolder = bool.Parse(returnedString.ToString());

				// プログラム終了ダイアログのカウントダウン(秒)（拡張設定）
				exitDialogCountDown = (int)api.GetPrivateProfileInt(SETTINGS_SECTION, KEY_EXIT_DIALOG_COUNT_DOWN, exitDialogCountDown, ccMushroomIniFileName);

				// favicon.ico の URL をテキストボックスで設定できるか否か（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_TEXT_FAVICON_URL_VISIBLE, textFaviconUrlVisible.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				textFaviconUrlVisible = bool.Parse(returnedString.ToString());

				// フォームの背景色（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_FORM_BACK_COLOR, this.BackColor.Name.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				formBackColor = Color.FromName(returnedString.ToString());

				// タブページの背景色（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_TAB_PAGE_BACK_COLOR, SystemColors.ControlLight/*Color.Transparent*/.Name.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				tabPageBackColor = Color.FromName(returnedString.ToString());

				// ステータスバーのアプリ環境名の背景色（拡張設定）
				if ( toolStripComboAppEnviron != null )
				{
					api.GetPrivateProfileString(SETTINGS_SECTION, KEY_COMBO_APP_ENVIRON_COLOR, toolStripComboAppEnviron.BackColor.Name.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
					toolStripComboAppEnviron.BackColor = Color.FromName(returnedString.ToString());
				}

				// □マークの前景色（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_RECT_MARK_FORE_COLOR, Color.White.Name.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				rectMarkForeColor = new SolidBrush(Color.FromName(returnedString.ToString()));

				// □マークの背景色（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_RECT_MARK_BACK_COLOR, Color.Black.Name.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				rectMarkBackColor = new SolidBrush(Color.FromName(returnedString.ToString()));

				// SDI のテキストエディタ（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_SDI_TEXT_EDITORS, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				string[] _sdiTextEditor = returnedString.ToString().Split(',');
				sdiTextEditors = new List<string>(_sdiTextEditor);
				sdiTextEditors.Add("notepad.exe");
				for ( int j = sdiTextEditors.Count - 1; 0 <= j; j-- )
				{
					if ( sdiTextEditors[j].Length == 0 )
						sdiTextEditors.RemoveAt(j);
					else
						sdiTextEditors[j] = sdiTextEditors[j].ToLower();
				}

				// ignoreRemoteInfo の機能を有効にする（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_ENABLE_IGNORE_REMOTE_INFO, enableIgnoreRemoteInfo.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				enableIgnoreRemoteInfo = bool.Parse(returnedString.ToString());

				// フォームの最大サイズ（拡張設定）
				if ( api.GetPrivateProfileString(SETTINGS_SECTION, KEY_FORM_MAXIMUM_SIZE, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) != 0 )
				{
					try
					{
						string[] size = returnedString.ToString().Split(',');
						Size maxSize = new Size(int.Parse(size[0]), int.Parse(size[1]));
						if ( (maxSize.Width == 0 && maxSize.Height == 0) || (this.Size.Width < maxSize.Width && this.Size.Height < maxSize.Height) )
						{
							this.MaximumSize = maxSize;
						}
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
					}
				}

				// 最後に選択された notifyIconMenu を記憶する（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_RESUME_LATEST_NOTIFY_MENU_ITEM, resumeLatestNotifyMenuItem.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				resumeLatestNotifyMenuItem = bool.Parse(returnedString.ToString());

				// リモート側のボタンに設定された背景色を無視する（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_IGNORE_REMOTE_BUTTON_BACK_COLOR, ignoreRemoteButtonBackColor.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				ignoreRemoteButtonBackColor = bool.Parse(returnedString.ToString());

				// Click-Through を切り替えるホットキー（拡張設定）
				if ( api.GetPrivateProfileString(SETTINGS_SECTION, KEY_TOGGLE_CLICK_THROUGH_HOTKEY, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) != 0 )
				{
					toggleClickThroughHotKey = returnedString.ToString();
				}

				// ボタンの移動中はタブの背景画像を隠す（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_HIDE_TAB_IMAGE_DURING_BUTTON_DRAG, hideTabImageDuringButtonDrag.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				hideTabImageDuringButtonDrag = bool.Parse(returnedString.ToString());

				// ボタンのツールチップの横幅（拡張設定）
				if ( api.GetPrivateProfileString(SETTINGS_SECTION, KEY_BUTTON_TOOLTIP_WIDTH, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) != 0 )
				{
					buttonToolTipWidth = returnedString.ToString();
				}

				// TopMost を解除する時間(秒)（拡張設定）
				topMostReleaseTime = (int)api.GetPrivateProfileInt(SETTINGS_SECTION, KEY_TOP_MOST_RELEASE_TIME, topMostReleaseTime, ccMushroomIniFileName);

				// エキスパート用で起動するか否か（拡張設定）
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_EXPERT_MODE, Program.expertMode.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				Program.expertMode = bool.Parse(returnedString.ToString());

#if FOR_WINDOWS7
				// ジャンプリスト用のタブページ
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_FOR_JUMPLIST_TABPAGE, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				forJumpListTabPage = returnedString.ToString();

				// タスクバーのサムネイルを有効にするか否か
				api.GetPrivateProfileString(SETTINGS_SECTION, KEY_ENABLED_TASKBAR_THUMBNAIL, enabledTaskbarThumbnail.ToString(), returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				enabledTaskbarThumbnail = bool.Parse(returnedString.ToString());

				// タブページがアクティブになった後で最前面する遅延時間(ms)
				setForegroundWindowDelayTime = (int)api.GetPrivateProfileInt(SETTINGS_SECTION, KEY_SET_FOREGROUND_WINDOW_DELAYTIME, setForegroundWindowDelayTime, ccMushroomIniFileName);
#endif

				// 最後に選択されていたタブ名
				api.GetPrivateProfileString(RESUME_SECTION, KEY_SELECTED_TAB_PAGE_TEXT, "\0", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				if ( returnedString.Length != 0 )
				{
					SetSelectedTabPageText(returnedString.ToString());
					firstRunning = false;
				}

				// 最後に選択されていたアプリ環境
				api.GetPrivateProfileString(RESUME_SECTION, KEY_APP_ENVIRON, "\0", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				if ( returnedString.Length != 0 && toolStripComboAppEnviron.Enabled )
				{
					toolStripComboAppEnviron.SelectedItem = returnedString.ToString();
					AdjustComboAppEnviron(toolStripComboAppEnviron.SelectedItem.ToString());
				}

				// ウィンドウの位置とサイズ
				if ( windowRectangle.IsEmpty )
				{
					windowRectangle = new Rectangle(this.Location, this.Size);
				}
				else
				{
					this.Location = windowRectangle.Location;
					this.Size = windowRectangle.Size;
				}

#if ENABLE_TAB_BACKGROUND
				try
				{
					if ( selectedTabPageText != null )
					{
						Size tabPageSize = GetTabResumeFormSize(selectedTabPageText);
						if ( tabPageSize != Size.Empty )	// 前回終了したタブのフォーム サイズを復元する？
						{
							this.Size = tabPageSize;
						}
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(ccMushroomIniFileName + "\r\nINI ファイルの読み込みが失敗しました.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// GetPrivateProfileProgramsFolder
		/// </summary>
		private bool GetPrivateProfileProgramsFolder(string keyName, string iniFileName, out string programsFolder, ref string[] programsFolders)
		{
			programsFolder = null;
			StringBuilder returnedString = new StringBuilder(1024);

			// リモート/ローカル側のプログラムを置いているフォルダ名
			api.GetPrivateProfileString(SETTINGS_SECTION, keyName, "\0", returnedString, (uint)returnedString.Capacity, iniFileName);
			if ( returnedString.Length == 0 )
			{
				return keyName.StartsWith(KEY_REMOTE_PROGRAMS_FOLDER);
			}

			programsFolder = returnedString.ToString();

			int i;
			for ( i = 0; i < programsFolders.Length && (programsFolders[i] != programsFolder); i++ ) ;
			if ( i != programsFolders.Length )	// 既に登録済み？
			{
#if (DEBUG)
				MessageBox.Show(keyName + "=" + programsFolder + "\r\nは既に登録されています.", MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
				if ( keyName.StartsWith(KEY_REMOTE_PROGRAMS_FOLDER) )
					return false;
			}

			return true;
		}
		#endregion

		#region application ノード関連
		/// <summary>
		/// CreateApplicationNode
		/// xmlCcTakeMe の application ノードを ccMushroom 構成ファイル用にインポートして編集する
		/// </summary>
		private XmlNode CreateApplicationNode(XmlDocument xmlCcTakeMe, DirectoryInfo subdirRemoteProgram, int remoteProgramsFolderRootLength, DirectoryInfo dirLocalProgram)
		{
			XmlNode application = xmlCcConfiguration.ImportNode(xmlCcTakeMe.DocumentElement, true);

			SetApplicationAttribute(ref application, ATTRIB_REMOTE_PROGRAMS_FOLDER, subdirRemoteProgram.FullName.Substring(0, remoteProgramsFolderRootLength));

			if ( application[TAG_TAB_TEXT] == null )
			{
				XmlElement elemTabText = xmlCcConfiguration.CreateElement(TAG_TAB_TEXT);
				application.AppendChild(elemTabText);
			}

			if ( application[TAG_TAB_TEXT].InnerText.Length == 0 )
			{
				application[TAG_TAB_TEXT].InnerText = subdirRemoteProgram.Parent.ToString();
			}

			if ( application[TAG_APP_NAME].InnerText.IndexOf("\\") == -1 )
			{
				if ( !application[TAG_APP_NAME].InnerText.StartsWith("http://") )
				{
					application[TAG_APP_NAME].InnerText = dirLocalProgram.FullName + "\\" + application[TAG_APP_NAME].InnerText;
				}
			}

			return application;
		}

		/// <summary>
		/// application ノードに属性をセットする
		/// </summary>
		private void SetApplicationAttribute(ref XmlNode application, string attribName, string attribValue)
		{
			XmlNode attrib = xmlCcConfiguration.CreateNode(XmlNodeType.Attribute, attribName, "");
			attrib.Value = attribValue;
			application.Attributes.SetNamedItem(attrib);

			if ( attribName == ATTRIB_NEW )
			{
				int count;
				string tabText = application[TAG_TAB_TEXT].InnerText;
				if ( newAppTab.TryGetValue(tabText, out count) )
				{
					newAppTab[tabText] = ++count;
				}
				else
				{
					newAppTab[tabText] = 1;
				}
			}
		}
		#endregion

		#region 進捗メッセージ関連
#if (SHOW_REFRESHING_INTERNAL_PROGRESS_MESSAGE)
        /// <summary>
        /// 進捗メッセージ コントロールを表示する/消す
        /// </summary>
        private void VisibleProgressMessageControls(bool visible)
        {
            try
            {
				toolStripProgressBar.Visible = visible;

                if (visible)
                {
                    this.Cursor = Cursors.WaitCursor;
					InitProgressBar(0, 100, 2, 0);
                    nowLoading = true;
                }
                else
                {
					// 自分自身の Assembly を取得
					Assembly ccAssembly = Assembly.GetExecutingAssembly();
					// バージョンの取得
					Version ccVer = ccAssembly.GetName().Version;

					DateTime buildDateTime = new DateTime(2000, 1, 1);
					TimeSpan verSpan = new TimeSpan(ccVer.Build * TimeSpan.TicksPerDay + ccVer.Revision * 2 * TimeSpan.TicksPerSecond);
					buildDateTime += verSpan;

					toolStripVersion.Text = "version: " + ccVer.Major + "." + ccVer.Minor + "." + buildDateTime.ToString("yyMMdd.HHmm") + "   @Win" + System.Environment.OSVersion.Version.Major + "." + System.Environment.OSVersion.Version.Minor;
                    nowLoading = false;
                    this.Cursor = Cursors.Default;
                }
            }
            catch (Exception)
            {
            }
        }
#endif

		/// <summary>
		/// 進捗メッセージを表示する
		/// </summary>
		private void ShowProgressMessage(string message1, string message2)
		{
			try
			{
				if ( SplashForm.Form != null )
				{
					SplashForm.ProgressStepIt(message1, message2);
				}
				else
				{
					toolStripVersion.Text = message1 + "　" + message2;
					Application.DoEvents();

					// ProgressBarStyle.Marquee にしたので不要
					/*toolStripProgressBar.PerformStep();
					if ( toolStripProgressBar.Value == 100 )
					{
						toolStripProgressBar.Value = 0;
					}*/
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// プログレス バーを初期化する
		/// </summary>
		private void InitProgressBar(int min, int max, int step, int value)
		{
			try
			{
				if ( SplashForm.Form != null )
				{
					SplashForm.InitProgressBar(min, max, step, value);
				}
				else
				{
					// ProgressBarStyle.Marquee にしたので不要
					/*toolStripProgressBar.Minimum = min;
					toolStripProgressBar.Maximum = max;
					toolStripProgressBar.Step = step;
					toolStripProgressBar.Value = value;*/
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region タブ コントロール関連
		/// <summary>
		/// SetSelectedTabPageText
		/// </summary>
		/// <param name="tabPageText"></param>
		private void SetSelectedTabPageText(string tabPageText)
		{
			try
			{
				selectedTabPageText = tabPageText;

				if ( Program.debMode )
				{
					toolStripMousePosition.ToolTipText = (selectedTabPageText == null) ? "null" : selectedTabPageText;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// プレーンなタブ名を取得する
		/// </summary>
		/// <param name="tabPageText"></param>
		/// <returns></returns>
		private string GetPlainTabPageText(string tabPageText)
		{
			if ( tabPageText.EndsWith(newAppIndicator) )
			{
				tabPageText = tabPageText.Substring(0, tabPageText.Length - 1);
			}

			return tabPageText;
		}

		/// <summary>
		/// 選択されているタブページを最新の状態にする
		/// </summary>
		private void RefreshSelectedTabPage(bool orderByTag)
		{
			try
			{
				if ( nowLoading )
					return;
				if ( tabControl.TabPages.Count == 0 )
					return;

				api.SendMessage(tabControl.SelectedTab.Handle, api.WM_HSCROLL, api.SB_LEFT, 0);

				TabPage selectedTab = tabControl.SelectedTab;
				int countButtonInTabPage = selectedTab.Controls.Count;

				Point[] pointButtonInTabPage, pointButtonInTabPageHScroll = new Point[countButtonInTabPage];
				int hscrollButtonCount = ComputeButtonPoint(selectedTab.Name, new Rectangle(selectedTab.Top, selectedTab.Bottom, selectedTab.Width, selectedTab.Height), countButtonInTabPage, out pointButtonInTabPage, ref pointButtonInTabPageHScroll);

#if true//MOVABLE_BUTTON_BY_DRAGDROP
				List<int> ctrlIndexes = new List<int>();
				for ( int i = 0; i < countButtonInTabPage; i++ )
				{
					if ( !(selectedTab.Controls[i] is Button) )
						continue;
					ctrlIndexes.Add(orderByTag ? (int)selectedTab.Controls[i].Tag : (int)selectedTab.Controls[i].TabIndex);
				}

				if ( orderByTag )
				{
					ctrlIndexes.Sort();
				}

				for ( int i = 0; i < ctrlIndexes.Count; i++ )
				{
					int j;
					if ( orderByTag )
					{
						// Tag の小さい方からボタンを配置する
						for ( j = 0; (j < countButtonInTabPage) && ((int)selectedTab.Controls[j].Tag != ctrlIndexes[i]); j++ ) ;
						if ( j == countButtonInTabPage )
							continue;
					}
					else
					{
						j = ctrlIndexes.IndexOf(i);
					}

					Control control = selectedTab.Controls[j];

					int tabIndex = i;
					control.TabIndex = tabIndex;
#else
				foreach ( Control control in selectedTab.Controls )
				{
					if ( !(control is Button) )
						continue;

					int tabIndex = control.TabIndex;
#endif
					control.Location = (countButtonInTabPage < hscrollButtonCount) ? pointButtonInTabPage[tabIndex] : pointButtonInTabPageHScroll[tabIndex];

					if ( Program.debMode )
					{
						// ToolTip を設定する
						string[] appToolTip = toolTip.GetToolTip(control).Split('\r');
						appToolTip[appToolTip.Length - 2] = "\n" + toolTipNames[(int)tt.tabIndex] + tabIndex;
						appToolTip[appToolTip.Length - 1] = "\n" + toolTipNames[(int)tt.tag] + control.Tag;
						toolTip.SetToolTip(control, string.Join("\r", appToolTip));
					}

					((Button)control).FlatStyle = FlatStyle.Flat;
				}

#if ENABLE_TAB_BACKGROUND
				if ( hideTabImageDuringButtonDrag )
				{
					if ( tabControl.SelectedTab.BackgroundImage == null )
					{
						string tabBackgroundFileName = GetEnableTabBackgroundFileName(tabControl.SelectedTab.Name);
						if ( tabBackgroundFileName != null )
						{
							// タブの背景画像を設定する
							tabControl.SelectedTab.BackgroundImage = new Bitmap(tabBackgroundFileName);
						}
					}
				}
#endif

				tabControl.Select();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

#if ENABLE_TAB_BACKGROUND
		private void RefreshSelectedTabPage()
		{
			try
			{
				if ( tabControl.SelectedTab == null )
					return;

				Size _sizeButton = GetTabButtonSize(tabControl.SelectedTab.Tag/*Name*/);
				if ( buttonSize != _sizeButton )
				{
					RefreshSelectedTabPage(false);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#endif
		#endregion

		#region アプリケーション起動関連
		/// <summary>
		/// [アプリケーション ボタン] がクリックされた
		/// </summary>
		/// <param name="iButton"></param>
		private void AppButtonClicked(int iButton)
		{
			try
			{
				string verb = null;

#if FOR_WINDOWS7
				const int KEY_PRESSED = 0x8000;
				bool bRightShiftKey = Convert.ToBoolean(GetKeyState(0xA1/*VK_RSHIFT*/) & KEY_PRESSED);

				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton];

				if ( (formKeyDownArgs != null) && formKeyDownArgs.Control && formKeyDownArgs.Shift )
				{
					string appName = application[TAG_APP_NAME].InnerText;
					if ( appName.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase) || appName.EndsWith(".com", StringComparison.CurrentCultureIgnoreCase) || appName.EndsWith(".bat", StringComparison.CurrentCultureIgnoreCase) )
					{
						verb = "RunAs";
					}
				}

				// 管理者として実行？
				if ( runAsButtons.IndexOf(application[TAG_BUTTON_TEXT].InnerText) != -1 )
				{
					StringBuilder returnedString = new StringBuilder(256);
					api.GetPrivateProfileString("RunAs", application[TAG_BUTTON_TEXT].InnerText, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
					switch ( returnedString.ToString() )
					{
						case "true":
							verb = "RunAs"; break;
						case "false":
							verb = null; break;
						case "toggle":
							verb = (verb == null) ? "RunAs" : null; break;
					}
				}
#endif

				ProcessStart(iButton, verb);

				tabControl.Select();

#if FOR_WINDOWS7
				// [Shift] キーが単体で押されたか、[Ctrl] + 右[Shift] キーが押された？
				if ( (formKeyDownArgs != null) && ((formKeyDownArgs.Modifiers == Keys.Shift) || (formKeyDownArgs.Control && bRightShiftKey)) )
#else
				if ( (formKeyDownArgs != null) && formKeyDownArgs.Shift )
#endif
				{
					//api.SetForegroundWindow(this.Handle);
					this.TopMost = true;
					try
					{
						if ( topMostReleaseTime != 0 )
						{
							if ( timerTopMost == null )
							{
								timerTopMost = new System.Windows.Forms.Timer(this.components);
								timerTopMost.Tick += new System.EventHandler(this.timerTopMost_Tick);
							}
							timerTopMost.Interval = topMostReleaseTime * 1000;
							timerTopMost.Start();
						}
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
					}

					if ( Program.debMode )
					{
						toolStripMousePosition.Text += "+";
						statusStrip.Refresh();
						System.Threading.Thread.Sleep(200);
					}
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				formKeyDownArgs = null;
			}
		}

		/// <summary>
		/// フォームの TopMost を解除する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerTopMost_Tick(object sender, EventArgs e)
		{
			try
			{
				this.TopMost = false;
				timerTopMost.Stop();

				if ( Program.debMode )
				{
					toolStripMousePosition.Text += "-";
					statusStrip.Refresh();
					System.Threading.Thread.Sleep(200);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [アプリケーション ボタン] が [Enter] キーで押された
		/// </summary>
		private void AppButtonPushedWithEnter()
		{
#if MOVABLE_BUTTON_BY_DRAGDROP
			if ( (0 <= dragButtonTag) && (dragButtonTag < dragButtonTagWait) )
				return;
#endif

			Point point = tabControl.SelectedTab.PointToClient(Cursor.Position);
			Button appButton = GetButtonContains(point, null);
			if ( appButton == null )
				return;

#if (DEBUG)
			XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)appButton.Tag];
			bool newApp;
			string destButtonText = GetButtonTextFromToolTip(toolTip.GetToolTip(appButton), out newApp, false);
			Debug.WriteLine(destButtonText);
#endif

			AppButtonClicked((int)appButton.Tag);
		}

		/// <summary>
		/// 自動実行するアプリケーションを起動する
		/// </summary>
		private void AutoExecProcess()
		{
			try
			{
				string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[@" + ATTRIB_AUTO_EXEC + "='" + true.ToString().ToLower() + "']";
				foreach ( XmlNode application in xmlCcConfiguration.SelectNodes(xpath) )
				{
					ProcessStart(application, null);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// アプリケーションを起動する
		/// </summary>
		/// <param name="iButton"></param>
		private void ProcessStart(int iButton, string verb)
		{
			try
			{
				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton];

#if ENABLE_LOGON_REQUIRED
				if ( application[TAG_J1_LOGON_REQUIRED] != null )
				{
					StringBuilder returnedString = new StringBuilder(1024);
					XmlNode logonRequired = application[TAG_J1_LOGON_REQUIRED];
					bool overExpires = true;
					string remoteProgramsName = logonRequired.Attributes[ATTRIB_NAME].Value;
					string userID = string.Empty, password = string.Empty;

					// ログオン履歴がある？
					if ( api.GetPrivateProfileString(LOGON_EXPIRES_SECTION, remoteProgramsName, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) != 0 )
					{
						string[] _logexp = returnedString.ToString().Split('｜');
						DateTime expiresDateTime = DateTime.Parse(_logexp[(int)logexp.dateTime]);
						if ( overExpires = (expiresDateTime < DateTime.Now) )	// 有効期限切れ？
						{
							userID = _logexp[(int)logexp.uid];
							password = common.DecodePassword(_logexp[(int)logexp.pwd]);
							//api.WritePrivateProfileString(LOGON_EXPIRES_SECTION, remoteProgramsName, null, ccMushroomIniFileName);
						}
					}

					// ログオンの有効期限を過ぎている？
					if ( overExpires )
					{
						string dirPath = logonRequired.Attributes[ATTR_J1_DIRPATH] != null ? logonRequired.Attributes[ATTR_J1_DIRPATH].Value : null;
						string domain = logonRequired.Attributes[ATTR_J1_DOMAIN] != null ? logonRequired.Attributes[ATTR_J1_DOMAIN].Value : null;
						LogonDlg logonDlg = new LogonDlg(dirPath, domain, userID, password);
						if ( logonDlg.ShowDialog(this) != DialogResult.OK )
							return;

						string expires = logonRequired.Attributes[ATTR_J1_EXPIRES].Value;
						int digit = int.Parse(expires.Substring(0, expires.Length - 1));
						DateTime expiresDateTime;
						if ( expires.EndsWith("D") )
							expiresDateTime = DateTime.Now.AddDays(digit);
						else if ( expires.EndsWith("H") )
							expiresDateTime = DateTime.Now.AddHours(digit);
						else if ( expires.EndsWith("M") )
							expiresDateTime = DateTime.Now.AddMinutes(digit);
						else if ( expires.EndsWith("S") )
							expiresDateTime = DateTime.Now.AddSeconds(digit);
						else
							throw new Exception(application[TAG_APP_NAME].InnerText + " " + ATTR_J1_EXPIRES + "ERROR");

						string[] _logexp = new string[3];
						_logexp[(int)logexp.dateTime] = expiresDateTime.ToString("yyyy/MM/dd HH:mm:ss");
						_logexp[(int)logexp.uid] = logonDlg.textUserName.Text;
						_logexp[(int)logexp.pwd] = common.EncodePassword(logonDlg.textPassword.Text);
						api.WritePrivateProfileString(LOGON_EXPIRES_SECTION, remoteProgramsName, string.Join("｜", _logexp), ccMushroomIniFileName);

#if true
						// 他の同一ユーザー名のパスワードを更新しておく
						byte[] returnedBytes = new byte[0xffff];
						int length = (int)api.GetPrivateProfileSection(LOGON_EXPIRES_SECTION, returnedBytes, (uint)returnedBytes.Length, ccMushroomIniFileName);
						if ( length != 0 )
						{
							string returnedKey = Encoding.Default.GetString(returnedBytes, 0, length - 1);
							List<string> logonExpiresKeyValues = new List<string>(returnedKey.Split('\0'));

							foreach ( string logonExpiresKeyValue in logonExpiresKeyValues )
							{
								string[] keyValue = logonExpiresKeyValue.Split('=');
								if ( keyValue[0] == remoteProgramsName )
									continue;
								_logexp = keyValue[1].ToString().Split('｜');
								if ( _logexp[(int)logexp.uid] != logonDlg.textUserName.Text )
									continue;
								_logexp[(int)logexp.pwd] = common.EncodePassword(logonDlg.textPassword.Text);
								api.WritePrivateProfileString(LOGON_EXPIRES_SECTION, keyValue[0], string.Join("｜", _logexp), ccMushroomIniFileName);
							}
						}
#endif
					}
				}
#endif

				ProcessStart(application, verb);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ProcessStart(XmlNode application, string verb)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				string appName = application[TAG_APP_NAME].InnerText;
				string arguments = "";

				if ( application[TAG_COMMAND_LINE] != null )
				{
					arguments = application[TAG_COMMAND_LINE].InnerText;
				}

				ProcessStartInfo startInfo = new ProcessStartInfo(appName, arguments);

				if ( application[TAG_WORKING_DIRECTORY] == null )
				{
					startInfo.WorkingDirectory = Path.GetDirectoryName(appName);
				}
				else
				{
					startInfo.WorkingDirectory = application[TAG_WORKING_DIRECTORY].InnerText;
				}

				if ( verb != null )
				{
					startInfo.Verb = verb;
				}

				Process.Start(startInfo);

				if ( timerAutoWindowClose != null )
				{
					timerAutoWindowClose.Change(autoWindowCloseTime, System.Threading.Timeout.Infinite);	// SetTimer（周期的なシグナル通知は無効）
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			Cursor.Current = Cursors.Default;
		}

		private void ProcessStart(int iButton)
		{
			ProcessStart(iButton, null);
		}
		#endregion

		#region その他の関数
		/// <summary>
		/// ウィンドウの位置とサイズ
		/// </summary>
		public static void GetWindowRectangle()
		{
			StringBuilder returnedString = new StringBuilder(1024);
			Rectangle windowRectangle = Rectangle.Empty;

			api.GetPrivateProfileString(RESUME_SECTION, KEY_WINDOW_RECTANGLE, "\0", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);

			if ( returnedString.Length != 0 )
			{
				string[] rect = returnedString.ToString().Split(',');
				windowRectangle = new Rectangle(Int32.Parse(rect[0]), Int32.Parse(rect[1]), Int32.Parse(rect[2]), Int32.Parse(rect[3]));
			}

			ccMushroom.windowRectangle = windowRectangle;
		}

		/// <summary>
		/// ccMushroom 構成ファイルを作成する
		/// </summary>
		private bool CreateCcConfiguration()
		{
			try
			{
#if false
				// ローカル プログラムのディレクトリ
				DirectoryInfo dirLocalPrograms = new DirectoryInfo (localProgramsFolder);
				if ( !dirLocalPrograms.Exists )
				{
					dirLocalPrograms.Create ();
				}
#endif

				xmlCcConfiguration = new XmlDocument();

				XmlDeclaration decl = xmlCcConfiguration.CreateXmlDeclaration("1.0", "utf-8", null);
				xmlCcConfiguration.AppendChild(decl);

				XmlElement elemConfig = xmlCcConfiguration.CreateElement(TAG_CONFIGURATION);
				xmlCcConfiguration.AppendChild(elemConfig);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				xmlCcConfiguration = null;
				return false;
			}

			return true;
		}

		/// <summary>
		/// ログファイルを保存する
		/// </summary>
		private bool SaveCcConfigurationLog(bool append)
		{
			try
			{
				string fileCcConfigurationLog = Application.StartupPath + "\\" + CC_CONFIGURATION_LOG_FILE_NAME;

				using ( StreamWriter sw = new StreamWriter(fileCcConfigurationLog, append) )
				{
					sw.Write(ccConfigurationLog.ToString());
					sw.Close();
				}
			}
			catch ( Exception )
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 渡された座標位置にあるボタンを取得する
		/// </summary>
		/// <param name="point"></param>
		/// <param name="myButtonTab"></param>
		/// <returns></returns>
		private Button GetButtonContains(Point point, object myButtonTab)
		{
			try
			{
				Debug.WriteLine(point.X + " " + point.Y);

				foreach ( Control control in tabControl.SelectedTab.Controls )
				{
					if ( !(control is Button) )
						continue;

					Button appButton = (Button)control;

					if ( (point.X < appButton.Location.X) || (point.Y < appButton.Location.Y) ||
						 (appButton.Location.X + appButton.Width < point.X) || (appButton.Location.Y + appButton.Height < point.Y) )
						continue;

					if ( appButton.Tag == myButtonTab )
						continue;

					return appButton;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

			return null;
		}

		/// <summary>
		/// ヘルプファイルを表示する
		/// </summary>
		private void ShowHelpFile()
		{
			try
			{
				string localCcMushroomChmFile = Application.StartupPath + @"\" + Application.ProductName + ".chm";
#if true
				System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
				string now = DateTime.Now.ToString("yyyy/MM/dd tt", cultureInfo);
				StringBuilder returnedString = new StringBuilder(1024);

				try
				{
					api.GetPrivateProfileString(RESUME_SECTION, KEY_LAST_HELP_FILE_CHECK, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
					if ( (returnedString.Length == 0) || (returnedString.ToString() != now) )
					{
						string remoteCcMushroomChmFile = latestProgramFolder + Application.ProductName + ".chm";
						DateTime remoteCcMushroomChmWriteTime = File.GetLastWriteTime(remoteCcMushroomChmFile);
						DateTime localCcMushroomChmWriteTime = (File.Exists(localCcMushroomChmFile)) ? File.GetLastWriteTime(localCcMushroomChmFile) : DateTime.Parse("2007/11/01");
						if ( localCcMushroomChmWriteTime < remoteCcMushroomChmWriteTime )
						{
							File.Copy(remoteCcMushroomChmFile, localCcMushroomChmFile, true);
						}
						api.WritePrivateProfileString(RESUME_SECTION, KEY_LAST_HELP_FILE_CHECK, now, ccMushroomIniFileName);
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
#if (DEBUG)
					MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
				}
#endif

#if (DEBUG)
				localCcMushroomChmFile = @"C:\Documents and Settings\Hidetatsu\My Documents\RoboHTML\" + Application.ProductName + "\\" + Application.ProductName + ".chm";
#endif
				Process.Start(localCcMushroomChmFile);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// ジャンプリストを登録する
		/// </summary>
		private void SetJumpList()
		{
#if FOR_WINDOWS7
#if true
			try
			{
				if ( nowLoading )
					return;

				System.Windows.Shell.JumpList jumpList = new System.Windows.Shell.JumpList();

				if ( tabControl.TabCount == 0 )
				{
					jumpList.Apply();
					return;
				}

				if ( !string.IsNullOrEmpty(forJumpListTabPage) && (forJumpListTabPage != tabControl.SelectedTab.Text) )
					return;

				JumpItem[] jumpItems = new JumpItem[0];
				int i = 0;

				string selfPath = Application.StartupPath + "\\" + Application.ProductName + ".exe";

				List<int> iButtons = new List<int>();
				foreach ( Control button in tabControl.SelectedTab.Controls )
				{
					if ( !(button is Button) )
						continue;
					iButtons.Add((int)button.Tag);
				}

				iButtons.Sort();

				foreach ( int iButton in iButtons )
				{
					XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton];

					JumpTask jumpTask = new JumpTask();
					//jumpTask.ApplicationPath = application[TAG_APP_NAME].InnerText;
					jumpTask.ApplicationPath = selfPath;
					jumpTask.Title = application[TAG_BUTTON_TEXT].InnerText;
					jumpTask.Description = (application[TAG_COMMENT] != null ? application[TAG_COMMENT].InnerText : "");
					jumpTask.IconResourcePath = (application[TAG_ICON_FILE] == null ? application[TAG_APP_NAME].InnerText : application[TAG_ICON_FILE].InnerText);
					jumpTask.IconResourceIndex = 0;
					jumpTask.Arguments = "\"" + Program.CMDPARAM_BUTTON_TEXT + application[TAG_BUTTON_TEXT].InnerText + "\"";

					Array.Resize(ref jumpItems, i + 1);
					jumpItems[i++] = jumpTask;
				}

				jumpList.JumpItems.AddRange(jumpItems);

				jumpList.Apply();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
#else
			try
			{
				JumpList list = JumpList.CreateJumpList();
				JumpListLink [] jumpListLink = new JumpListLink [0];
				int i = 0;

				//string selfPath = System.Environment.CommandLine;
				// the value comes with quotation marks, strip them out
				//selfPath = selfPath.Substring(1, selfPath.Length - 3);
				//string selfPath = Environment.GetCommandLineArgs()[0];
				string selfPath = Application.StartupPath + "\\" + Application.ProductName + ".exe";

				foreach ( Control button in tabControl.SelectedTab.Controls )
				{
					if ( !(button is Button) )
						continue;

					Array.Resize(ref jumpListLink, i + 1);

					int iButton = (int)button.Tag;
					XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton];

					string pathValue = application[TAG_APP_NAME].InnerText;
					pathValue = selfPath;

					string titleValue = application[TAG_BUTTON_TEXT].InnerText;

					jumpListLink[i] = new JumpListLink(pathValue, titleValue);

					try
					{
						if ( application[TAG_ICON_FILE] == null )
						{
							jumpListLink[i].IconReference = new IconReference(application[TAG_APP_NAME].InnerText, 0);
						}
						else
						{
							jumpListLink[i].IconReference = new IconReference(application[TAG_ICON_FILE].InnerText, 0);
						}
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
					}

					jumpListLink[i].Arguments = Program.CMDPARAM_BUTTON_TEXT + iButton;

					i++;
					
					/*string systemFolder = Environment.GetFolderPath(
						Environment.SpecialFolder.System);

					string notepadPath = System.IO.Path.Combine(
						systemFolder, "notepad.exe");
					JumpListLink notepadTask = new JumpListLink(
						notepadPath, "Open Notepad");
					notepadTask.IconReference = new IconReference(
						notepadPath, 0);

					string calculatorPath = System.IO.Path.Combine(
						systemFolder, "calc.exe");
					JumpListLink calculatorTask = new JumpListLink(
						calculatorPath, "Open Calculator");
					calculatorTask.IconReference = new IconReference(
						calculatorPath, 0);


					string selfPath = System.Environment.CommandLine;
					// the value comes with quotation marks, strip them out
					selfPath = selfPath.Substring(1, selfPath.Length - 3);

					JumpListLink selfCommandATask = new JumpListLink(
						  selfPath, "Command A");
					selfCommandATask.Arguments = "Command-A";
					selfCommandATask.IconReference = new IconReference(
						  selfPath, 0);

					JumpListLink selfCommandBTask = new JumpListLink(
						  selfPath, "Command B");
					selfCommandBTask.Arguments = "Command-B";
					selfCommandBTask.IconReference = new IconReference(
						  selfPath, 0);*/
				}

				JumpListSeparator separator = new JumpListSeparator();
				list.AddUserTasks(jumpListLink );

				list.Refresh();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
#endif
#endif
		}
		#endregion
		#endregion

		#region リモート側のプログラム フォルダをスキャンする処理
		/// <summary>
		/// リモート側のプログラム フォルダをスキャンする
		/// </summary>
		private void ScanRemotePrograms()
		{
			StopAutoWindowCloseTimer();

			try
			{
				//toolStripScanned.Enabled = scanRemoteProgramsFolder;
				//toolStripScanned.Text = (scanRemoteProgramsFolder && Program.onlineEnable) ? "scanned" : "offline";
				toolStripScanned.Text = (!Program.onlineEnable) ? "offline" : (scanRemoteProgramsFolder ? "scanned" : "local");
				toolStripScanned.ToolTipText = /*"クリック(F5)で表示を更新する\n" + */"ダブルクリック(Ctrl+F5)で最新の状態にする"/*(scanRemoteProgramsFolder && Program.onlineEnable) ? "ダブルクリック(Ctrl+F5)で再スキャンする" : ""*/;

				ccConfigurationLog = new StringBuilder();
				xmlCcConfiguration = null;
				netErrorRemote.Clear();
				newAppTab.Clear();

				if ( !scanRemoteProgramsFolder || !Program.onlineEnable )	// スキャンしない、又はオフライン？
					return;

				// ccMushroom 構成ファイルを作成する
				if ( !CreateCcConfiguration() )
					return;

				for ( int i = 0; i < remoteProgramsFolders.Length; i++ )
				{
					if ( !enabledProgramsFolders[i] )
						continue;

					copiedRemoteProgramsFolder = null;
					ccConfigurationLog.Append(((i == 0) ? "" : "\r\n\r\n") + "==== " + remoteProgramsNames[i] + ": " + remoteProgramsFolders[i] + " -> " + localProgramsFolders[i] + " ====\r\n");

					bool withRemoteFileName = remoteProgramsFolders[i].EndsWith(".xml");
					bool isRemoteOnline = (withRemoteFileName && File.Exists(remoteProgramsFolders[i])) || Directory.Exists(remoteProgramsFolders[i]);

					// リモート プログラムのフォルダが見つからない？（ネットーワーク不具合？）
					if ( ignoreRemoteInfo || !isRemoteOnline/*!Directory.Exists(remoteProgramsFolders[i])*/ )
					{
						netErrorRemote.Add(remoteProgramsNames[i]);
						string remoteProgramFolder = (withRemoteFileName) ? Path.GetDirectoryName(remoteProgramsFolders[i]) : remoteProgramsFolders[i];
#if true
						for ( int j = i - 1; 0 <= j; j-- )
						{
							if ( !enabledProgramsFolders[j] )
								continue;
							bool _withRemoteFileName = remoteProgramsFolders[j].EndsWith(".xml");
							string _remoteProgramFolder = (_withRemoteFileName) ? Path.GetDirectoryName(remoteProgramsFolders[j]) : remoteProgramsFolders[j];
							// １つのリモート側に複数の設定がある場合で、既に読み込み済み？	
							if ( _remoteProgramFolder == remoteProgramFolder )
							{
								remoteProgramFolder = null;
							}
						}
						if ( remoteProgramFolder == null )
							continue;
#endif
						ReadPrevCcConfiguration(remoteProgramFolder/*remoteProgramsFolders[i]*/);
						if ( enableIgnoreRemoteInfo )
						{
							ignoreRemoteInfo = true;
						}
						continue;
					}

					if ( File.Exists(remoteProgramsFolders[i] + J1_LAUNCHER_FILE_NAME) || (withRemoteFileName && Path.GetFileName(remoteProgramsFolders[i]).IndexOf("Launcher", StringComparison.CurrentCultureIgnoreCase) != -1) )
					{
						string remoteProgramFolder = (withRemoteFileName) ? Path.GetDirectoryName(remoteProgramsFolders[i]) : remoteProgramsFolders[i];
						string j1LauncherFileName = (withRemoteFileName) ? @"\" + Path.GetFileName(remoteProgramsFolders[i]) : J1_LAUNCHER_FILE_NAME;
						// Ｊ１ランチャ プログラムのフォルダをスキャンしてローカルにコピーする
						ScanJ1LauncherPrograms(remoteProgramsNames[i], remoteProgramFolder/*remoteProgramsFolders[i]*/, remoteProgramFolder.Length/*remoteProgramsFolders[i].Length*/, j1LauncherFileName, localProgramsFolders[i]);
					}
					else
					{
						string remoteProgramFolder = (withRemoteFileName) ? Path.GetDirectoryName(remoteProgramsFolders[i]) : remoteProgramsFolders[i];
						string ccMushroomRemoteFileName = (withRemoteFileName) ? @"\" + Path.GetFileName(remoteProgramsFolders[i]) : CC_MUSHROOM_REMOTE_FILE_NAME;
						// リモート プログラムのフォルダをスキャンしてローカルにコピーする
						ScanRemotePrograms(remoteProgramFolder/*remoteProgramsFolders[i]*/, remoteProgramFolder.Length/*remoteProgramsFolders[i].Length*/, ccMushroomRemoteFileName, localProgramsFolders[i]);
					}

					if ( copiedRemoteProgramsFolder != null )
					{
						ccConfigurationLog.Append("\r\n---- RemoveNoUseLocalProgramsFolder ----\r\n");

						// 未使用のローカル プログラムのサブディレクトリを検索して削除する
						RemoveNoUseLocalProgramsFolder(localProgramsFolders[i], localProgramsFolders[i].Length);
					}
				}

#if true
				try
				{
					// 不要な "リモート プログラムのフォルダ = 実際のリモート プログラムのディレクトリ" を削除する
					byte[] returnedBytes = new byte[1024];
					int length = (int)api.GetPrivateProfileString/*ByByteArray*/(RESUME_SECTION, null, "", returnedBytes, (uint)returnedBytes.Length, ccMushroomIniFileName);
					if ( length != 0 )
					{
						string returnedKey = Encoding.Default.GetString(returnedBytes, 0, length - 1);
						List<string> settingsKeys = new List<string>(returnedKey.Split('\0'));

						foreach ( string settingsKey in settingsKeys )
						{
							if ( !settingsKey.StartsWith(@"\\") )
								continue;

							int index = Array.IndexOf(remoteProgramsFolders, settingsKey);
							if ( (index != -1) && enabledProgramsFolders[index] )	// remoteProgramsFolders に存在するキーで、有効になっている？
								continue;

							api.WritePrivateProfileString(RESUME_SECTION, settingsKey, null, ccMushroomIniFileName);
						}
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show("リモート側のプログラム ファイルのスキャンが失敗しました.\r\n以前の環境で起動します.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				xmlCcConfiguration = null;
			}
			finally
			{
				try
				{
					// ログファイルを保存する
					SaveCcConfigurationLog(false);

					// ccMushroom 構成ファイルを保存する
					if ( xmlCcConfiguration != null )
					{
						xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);
					}
				}
				catch ( Exception exp )
				{
					MessageBox.Show("ファイルの保存が失敗しました.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

			// 自動ウィンドウ クローズ用のタイマ
			StartTimerAutoWindowClose();
		}

		/// <summary>
		/// 新着アプリの数をバルーン ヘルプで表示する
		/// </summary>
		private void ShowNewAppBalloonTip()
		{
			try
			{
				StringBuilder message = new StringBuilder();
				string indent = (netErrorRemote.Count == 0 || newAppTab.Count == 0) ? "" : " ";

				if ( netErrorRemote.Count != 0 )
				{
					message.Append(netErrorBalloonText + "\r\n");
					foreach ( string remoteName in netErrorRemote )
					{
						message.Append(indent + remoteName + "\r\n");
					}
				}

				if ( newAppTab.Count != 0 )
				{
					message.Append(newAppBalloonText + "\r\n");
					foreach ( string tabText in newAppTab.Keys )
					{
						message.Append(indent + tabText + "(" + newAppTab[tabText] + ")\r\n");
					}
				}

				if ( message.Length == 0 )
					return;

				int timeout = 10;
				notifyIcon.ShowBalloonTip(timeout * 1000, appTitle, message.ToString(), ToolTipIcon.Info);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 新着アプリのバルーン ヘルプを消す
		/// </summary>
		private void RemoveNewAppBalloonTip()
		{
			try
			{
				Debug.WriteLine("\r\nEnumWindow");

				api.EnumWindows(
				new EnumWindowCallBack(delegate(IntPtr hWnd, uint lParam)
				{
					int result = api.GetWindowLong32(hWnd, api.GWL_STYLE);
					bool wsVisible = (result & api.WS_VISIBLE) != 0;
					if ( !wsVisible )
						return 1;

					StringBuilder className = new StringBuilder(512);
					api.GetClassName(hWnd, className, className.Capacity);

					StringBuilder windowText = new StringBuilder(1024);
					api.SendMessage(hWnd, api.WM_GETTEXT, windowText.Capacity, windowText);
					Debug.WriteLine(wsVisible + " - " + className + " - " + windowText);

					if ( className.ToString() != "tooltips_class32" )
						return 1;

					if ( !windowText.ToString().StartsWith(netErrorBalloonText) && !windowText.ToString().StartsWith(newAppBalloonText) )
						return 1;

					api.PostMessage(hWnd, api.TTM_POP, 0, 0);
					return 0;
				}
				), 0);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 前回起動した ccMushroom 構成ファイルの application ノードを読み込む
		/// </summary>
		private void ReadPrevCcConfiguration(string remoteProgramsFolder)
		{
			XmlDocument xmlCcConfigurationPrev = null;

			try
			{
				ccConfigurationLog.Append("!Directory.Exists (" + remoteProgramsFolder + ")\r\n");

				// 実際のリモート プログラムのディレクトリがあれば読み込む
				//string iniFileName = Application.StartupPath + CCMUSHROOM_INI_FILE_NAME;
				StringBuilder returnedString = new StringBuilder(1024);
				api.GetPrivateProfileString(RESUME_SECTION, remoteProgramsFolder, "\0", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
				if ( returnedString.Length != 0 )
				{
					ccConfigurationLog.Append(remoteProgramsFolder + " = " + returnedString.ToString() + "\r\n");
					remoteProgramsFolder = returnedString.ToString();
				}

				// 現在作成中の ccMushroom 構成ファイルの <configuration>
				XmlElement elemConfig = xmlCcConfiguration[TAG_CONFIGURATION];

				// 前回起動した ccMushroom 構成ファイル
				xmlCcConfigurationPrev = new XmlDocument();
				xmlCcConfigurationPrev.Load(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);

				// 見つからなかったリモート プログラム フォルダ属性を持っている <application> ノードを抽出する
				string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[@" + ATTRIB_REMOTE_PROGRAMS_FOLDER + "='" + remoteProgramsFolder + "']";
				XmlNodeList listApplication = xmlCcConfigurationPrev.SelectNodes(xpath);
				foreach ( XmlNode application in listApplication )
				{
					string appRemoteProgramsFolder = application.Attributes[ATTRIB_REMOTE_PROGRAMS_FOLDER].InnerText;
					string appName = Path.GetFileName(application[TAG_APP_NAME].InnerText);
					ShowProgressMessage(appRemoteProgramsFolder, appName);

					// 前回の <application> ノードを現在の <configuration> の子ノードとして取り込む
					elemConfig.AppendChild(xmlCcConfiguration.ImportNode(application, true));

					ccConfigurationLog.Append("[" + application[TAG_BUTTON_TEXT].InnerText + "] " + application[TAG_APP_NAME].InnerText + "\r\n");
				}
			}
			catch ( Exception exp )
			{
				string message = xmlCcConfigurationPrev + " 読込中に障害が発生しました.\r\n原因:" + exp.Message;
				MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
			}
		}

		/// <summary>
		/// ScanRemotePrograms
		/// </summary>
		private bool ScanRemotePrograms(string remoteProgramsFolder, int remoteProgramsFolderRootLength, string ccMushroomRemoteFileName, string localProgramsFolder)
		{
			try
			{
				ccConfigurationLog.Append("\r\nbool " + MethodBase.GetCurrentMethod().Name + "(" + remoteProgramsFolder + ", " + remoteProgramsFolderRootLength + ", " + localProgramsFolder + ")\r\n");

				string fileCcMushroomRemote = remoteProgramsFolder + ccMushroomRemoteFileName/*CC_MUSHROOM_REMOTE_FILE_NAME*/;
				// ccMushroom 登録用のルート ディレクトリではない？
				if ( !File.Exists(fileCcMushroomRemote) )
					return false;
				ccConfigurationLog.Append("[ROOT] " + fileCcMushroomRemote + "\r\n");

				// リモート プログラムのディレクトリ
				DirectoryInfo dirRemotePrograms = new DirectoryInfo(remoteProgramsFolder);
				string remoteSubDir = remoteProgramsFolder.Substring(remoteProgramsFolderRootLength);

				// ccMushroom 構成ファイルの <configuration>
				XmlElement elemConfig = xmlCcConfiguration[TAG_CONFIGURATION];
				XmlNode application;

				try
				{
					XmlDocument xmlCcMushroomRemote = new XmlDocument();
					xmlCcMushroomRemote.Load(fileCcMushroomRemote);

					// 登録フォルダの順番指定があれば優先的に登録する
					string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_ENTRY_DIR_ORDER + "/" + TAG_NAME;
					XmlNodeList listFolderName = xmlCcMushroomRemote.SelectNodes(xpath);
					foreach ( XmlNode nodeFolderName in listFolderName )
					{
						DirectoryInfo subdirRemoteProgram = new DirectoryInfo(dirRemotePrograms + "\\" + nodeFolderName.InnerText);

						// サブ ディレクトリは存在しない？
						if ( !subdirRemoteProgram.Exists )
							continue;

						// ccMushroom 登録用のルート（CC_MUSHROOM_REMOTE_FILE_NAME が存在する）ディレクトリだった？
						if ( ScanRemotePrograms(subdirRemoteProgram.FullName, remoteProgramsFolderRootLength, CC_MUSHROOM_REMOTE_FILE_NAME, localProgramsFolder) )
							continue;

						if ( (application = ReadCcTakeMeCopyRemoteProgramFiles(subdirRemoteProgram, remoteProgramsFolderRootLength, localProgramsFolder)) != null )
						{
							elemConfig.AppendChild(application);	// ccMushroom 構成ファイルに登録する
						}
					}

					// タブページのアイコン指定があればローカルにコピーしておく
					xpath =  "/" + TAG_CONFIGURATION + "/" + TAG_TAB_ICON_FILE + "/" + TAG_NAME;
					XmlNodeList listTabIconFileName = xmlCcMushroomRemote.SelectNodes(xpath);
					foreach ( XmlNode nodeFileName in listTabIconFileName )
					{
						string remoteIconFileName = dirRemotePrograms + "\\" + nodeFileName.InnerText;
						string localIconFileName = localProgramsFolder + remoteSubDir + "\\" + nodeFileName.InnerText;
						string tabName = Path.GetFileNameWithoutExtension(nodeFileName.InnerText);
						CopyTabIconFile(remoteIconFileName, localIconFileName, tabName, nodeFileName);
					}
				}
				catch ( Exception exp )
				{
					string message = fileCcMushroomRemote + " 読込中に障害が発生しました.\r\n原因:" + exp.Message;
					MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
				}

				// リモート プログラムのサブディレクトリ
				DirectoryInfo[] subdirRemotePrograms = dirRemotePrograms.GetDirectories();

				// サブディレクトリの cctakeme.xml を読み込み、ccMushroom 構成ファイルの <application> ノードを作成する
				for ( int i = 0; i < subdirRemotePrograms.Length; i++ )
				{
					// ccMushroom 登録用のルート（CC_MUSHROOM_REMOTE_FILE_NAME が存在する）ディレクトリだった？
					if ( ScanRemotePrograms(subdirRemotePrograms[i].FullName, remoteProgramsFolderRootLength, CC_MUSHROOM_REMOTE_FILE_NAME, localProgramsFolder) )
						continue;

					if ( (application = ReadCcTakeMeCopyRemoteProgramFiles(subdirRemotePrograms[i], remoteProgramsFolderRootLength, localProgramsFolder)) != null )
					{
						elemConfig.AppendChild(application);	// ccMushroom 構成ファイルに登録する
					}
				}

				// CC_MUSHROOM_REMOTE_FILE_NAME ファイルをローカル側に保存しておく
				string fileCcMushroomLocal = localProgramsFolder + remoteSubDir + CC_MUSHROOM_REMOTE_FILE_NAME;
				File.Copy(fileCcMushroomRemote, fileCcMushroomLocal, true);
			}
			catch ( Exception exp )
			{
				string message = remoteProgramsFolder + "\r\nリモート プログラムのスキャンが失敗しました.このフォルダはスキップします.\r\n原因:" + exp.Message;
				MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
			}

			return true;
		}

		/// <summary>
		/// ReadCcTakeMeCopyRemoteProgramFiles
		/// </summary>
		private XmlNode ReadCcTakeMeCopyRemoteProgramFiles(DirectoryInfo subdirRemoteProgram, int remoteProgramsFolderRootLength, string localProgramsFolder)
		{
			string fileCcTakeMe = string.Empty;
			XmlNode application = null;

			try
			{
				ccConfigurationLog.Append("XmlNode " + MethodBase.GetCurrentMethod().Name + "(" + subdirRemoteProgram + ", " + remoteProgramsFolderRootLength + ", " + localProgramsFolder + ")\r\n");
				ShowProgressMessage(subdirRemoteProgram.Parent.FullName, subdirRemoteProgram.Name);

				fileCcTakeMe = subdirRemoteProgram.FullName + CC_TAKE_ME_FILE_NAME;
				// ccMushroom 登録用のプログラム ディレクトリではない？
				if ( !File.Exists(fileCcTakeMe) )
					return null;
				ccConfigurationLog.Append("[SUB] " + fileCcTakeMe + "\r\n");

				// リモート プログラム フォルダをコピー済みとして登録する
				string remoteSubDir = subdirRemoteProgram.FullName.Substring(remoteProgramsFolderRootLength);
				int countScanned = (copiedRemoteProgramsFolder == null) ? 0 : copiedRemoteProgramsFolder.Length;
				for ( int i = 0; i < countScanned; i++ )
				{
					if ( copiedRemoteProgramsFolder[i] == remoteSubDir )
					{
						ccConfigurationLog.Append("[already]\r\n");
						return null;
					}
				}
				if ( copiedRemoteProgramsFolder == null )
					copiedRemoteProgramsFolder = new string[0];
				Array.Resize(ref copiedRemoteProgramsFolder, countScanned + 1);
				copiedRemoteProgramsFolder[countScanned] = remoteSubDir;

				// リモートのアプリケーション構成ファイルを読み込む
				XmlDocument xmlCcTakeMe = new XmlDocument();
				xmlCcTakeMe.Load(fileCcTakeMe);

				if ( (xmlCcTakeMe.DocumentElement[TAG_COPY_MODE] == null) && (xmlCcTakeMe.DocumentElement[TAG_ONCE_COPY] != null) )	// <copyMode> が無く、<onceCopy> がある？（互換性を保つため）
				{
					bool onceCopy = bool.Parse(xmlCcTakeMe.DocumentElement[TAG_ONCE_COPY].InnerText);
					string copyMode = (onceCopy ? cm.onceCopy : cm.normal).ToString();
					XmlElement elem = xmlCcTakeMe.DocumentElement[TAG_COPY_MODE];
					if ( elem == null )
					{
						elem = xmlCcTakeMe.CreateElement(TAG_COPY_MODE);
						xmlCcTakeMe.DocumentElement.AppendChild(elem);
					}
					elem.InnerText = copyMode;
				}

				DirectoryInfo dirLocalProgram = new DirectoryInfo(localProgramsFolder + copiedRemoteProgramsFolder[countScanned]/*"\\" + subdirRemoteProgram.Name*/);

				// <application> ノードを作成する
				application = CreateApplicationNode(xmlCcTakeMe, subdirRemoteProgram, remoteProgramsFolderRootLength, dirLocalProgram);

				CopyRemoteProgramFiles(subdirRemoteProgram, dirLocalProgram, xmlCcTakeMe, ref application);
			}
			catch ( Exception exp )
			{
				string message = fileCcTakeMe + "\r\nリモート ファイルのコピーが失敗しました.このプログラムはスキップします.\r\n原因:" + exp.Message;
				MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
				application = null;
			}

			return application;
		}

		/// <summary>
		/// CopyRemoteProgramFiles
		/// </summary>
		private void CopyRemoteProgramFiles(DirectoryInfo subdirRemoteProgram, DirectoryInfo dirLocalProgram, XmlDocument xmlCcTakeMe, ref XmlNode application)
		{
			try
			{
				ccConfigurationLog.Append("void " + MethodBase.GetCurrentMethod().Name + "(" + subdirRemoteProgram.FullName + "," + dirLocalProgram.FullName + ", " + xmlCcTakeMe.OuterXml.Replace("\r\n", "") + ")\r\n");

				cm copyMode = GetCopyMode(xmlCcTakeMe.DocumentElement[TAG_COPY_MODE].InnerText);

#if true		// 2011/10/04 */
				// アイコンの指定がある？
				if ( xmlCcTakeMe.DocumentElement[TAG_ICON_FILE] != null )
				{
					string iconFile = xmlCcTakeMe.DocumentElement[TAG_ICON_FILE].InnerText;
					string remoteIconFile = subdirRemoteProgram.FullName + "\\" + iconFile;
					if ( File.Exists(remoteIconFile) )
					{
						string localIconFile = dirLocalProgram.FullName + "\\" + iconFile;
						if ( dirLocalProgram.Exists )
						{
							File.Copy(remoteIconFile, localIconFile, true);
						}
						application[TAG_ICON_FILE].InnerText = localIconFile;
					}
					else
					{
						application.RemoveChild(application[TAG_ICON_FILE]);
					}
				}
#endif

				// ローカルのプログラム ディレクトリは存在しない？
				if ( !dirLocalProgram.Exists )
				{
					CopyDirectory(subdirRemoteProgram.FullName, dirLocalProgram.FullName);

					if ( !firstRunning )
					{
						// 更新されたアプリケーションの属性をセットする
						SetApplicationAttribute(ref application, ATTRIB_NEW, true.ToString().ToLower());
					}

					if ( copyMode != cm.shortCut )
						return;
				}

				// ショートカットの登録？
				if ( copyMode == cm.shortCut )
				{
#if true
					if ( application[TAG_APP_NAME].InnerText.StartsWith("http://") )
					{
						XmlElement elem = xmlCcConfiguration.CreateElement(TAG_ICON_FILE);
						string faviconUrlDllFile = Application.StartupPath + "\\" + iconsFolder + "\\" + faviconUrlDllFileName;
						elem.InnerText = faviconUrlDllFile;
						application.AppendChild(elem);
					}

					if ( application[TAG_COMMAND_LINE] != null )
					{
						string[] arguments = application[TAG_COMMAND_LINE].InnerText.Split(' ');
						if ( arguments.Length != 0 )		// コマンドライン引数あり？
						{
							for ( int i = 0; i < arguments.Length; i++ )
							{
								string argument = arguments[i];
								if ( string.IsNullOrEmpty(argument) )
									continue;
								if ( argument[0] == '"' )	// 引数が " で始まっている？
								{
									argument = argument.Substring(1);
									for ( i = i + 1; i < arguments.Length; i++ )	// 終端の " まで復元する
									{
										argument += (' ' + arguments[i]);
										if ( arguments[i].EndsWith("\"") )
										{
											argument = argument.Substring(0, argument.Length - 1);
											break;
										}
									}
								}
								if ( argument[0] == '/' )	// コマンド オプション？
									continue;
								if ( string.IsNullOrEmpty(Path.GetExtension(argument)) )	// 拡張子は無い？
									continue;

								string remoteArgName = (argument[0] == '\\' ? "" : subdirRemoteProgram.FullName + "\\") + argument;
								if ( File.Exists(remoteArgName) )	// リモート側に存在するコマンド ファイル名？
								{
#if false
									string localArgName = dirLocalProgram + "\\" + Path.GetFileName(argument);
									DateTime remoteArgDateTime = File.GetLastWriteTime(remoteArgName);
									DateTime localArgDateTime = File.Exists(localArgName) ? File.GetLastWriteTime(localArgName) : DateTime.MinValue;
									bool newArgument = (localArgDateTime < remoteArgDateTime);
									if ( newArgument )				// 新しいコマンド ファイル名？
									{
										File.Copy(remoteArgName, localArgName, true);
										SetApplicationAttribute(ref application, ATTRIB_NEW, true.ToString().ToLower());
									}
#else
									// リモート側に新しいファイルがあれば、全てローカル側にコピーする
									FileInfo[] _remoteFiles = subdirRemoteProgram.GetFiles();
									for ( int j = 0; j < _remoteFiles.Length; j++ )
									{
										string localFileName = dirLocalProgram + "\\" + _remoteFiles[j].Name;
										DateTime remoteArgDateTime = File.GetLastWriteTime(_remoteFiles[j].FullName);
										DateTime localArgDateTime = File.Exists(localFileName) ? File.GetLastWriteTime(localFileName) : DateTime.MinValue;
										if ( localArgDateTime < remoteArgDateTime )
										{
											_remoteFiles[j].CopyTo(localFileName, true);
											ccConfigurationLog.Append(" " + _remoteFiles[j] + ".CopyTo(" + localFileName + ", true);\r\n");
											if ( application.Attributes[ATTRIB_NEW] == null )
											{
												SetApplicationAttribute(ref application, ATTRIB_NEW, true.ToString().ToLower());
											}
										}
									}
#endif
								}
							}
						}
					}
#endif

					return;
				}

				// アプリケーションの更新日付をチェックする
				string appName = xmlCcTakeMe.DocumentElement[TAG_APP_NAME].InnerText;
				string remoteAppName = subdirRemoteProgram.FullName + "\\" + appName;
				string localAppName = dirLocalProgram + "\\" + appName;
				DateTime remoteDateTime = File.GetLastWriteTime(remoteAppName);
				DateTime localDateTime = File.Exists(localAppName) ? File.GetLastWriteTime(localAppName) : DateTime.MinValue;

				bool newApplication = (localDateTime < remoteDateTime);

				ccConfigurationLog.Append(" remoteAppName: " + remoteAppName + " " + remoteDateTime + "\r\n");
				ccConfigurationLog.Append(" localAppName: " + localAppName + " " + localDateTime + "\r\n");

				// 一度だけコピーする？（アプリケーション自身が自動更新機能を持っている）
				// 又は、既にコピーされたフォルダにアプリケーションが存在している？
				//if ( bool.Parse(xmlCcTakeMe.DocumentElement[TAG_ONCE_COPY].InnerText) )
				if ( (copyMode == cm.onceCopy) || (copyMode == cm.existCopy) )
				{
					if ( newApplication )
					{
						// 更新されたアプリケーションの属性をセットする
						SetApplicationAttribute(ref application, ATTRIB_NEW, true.ToString().ToLower());
					}
					return;
				}

				// リモート側のプログラムは更新されていない？
				if ( !newApplication )
					return;

				try
				{
					// exe を削除する（アプリケーションが使用中かどうかチェックするため）
					File.Delete(localAppName);
				}
				catch ( Exception exp )
				{
					ccConfigurationLog.Append(" [EXCEPTION] " + exp.Message.Replace("\r\n", "\t") + "\r\n");
				}

				// exe の削除が失敗した？（使用中だった）
				if ( File.Exists(localAppName) )
					return;

#if true
				try
				{
					XmlAttribute attrDelLocalFiles = xmlCcTakeMe.DocumentElement[TAG_COPY_MODE].Attributes[ATTRIB_DEL_LOCAL_FILES];
					// コピー前にローカル側のファイルを削除する？
					if ( (attrDelLocalFiles != null) && bool.Parse(attrDelLocalFiles.Value) )
					{
						FileInfo[] localFiles = dirLocalProgram.GetFiles();
						foreach ( FileInfo localFile in localFiles )
						{
							File.Delete(localFile.FullName);
						}
						DirectoryInfo[] localDirectries = dirLocalProgram.GetDirectories();
						foreach ( DirectoryInfo localDirectory in localDirectries )
						{
							Directory.Delete(localDirectory.FullName, true);
						}
					}
				}
				catch ( Exception exp )
				{
					ccConfigurationLog.Append(" [EXCEPTION] " + exp.Message.Replace("\r\n", "\t") + "\r\n");
				}
#endif

				// リモートのプログラム ファイルをローカルにコピーする
				FileInfo[] remoteFiles = subdirRemoteProgram.GetFiles();
				for ( int i = 0; i < remoteFiles.Length; i++ )
				{
					string localFileName = dirLocalProgram.FullName + "\\" + remoteFiles[i].Name;

					bool configFile = BackUpConfigFile(remoteFiles[i], localFileName);

					remoteFiles[i].CopyTo(localFileName, true);
					ccConfigurationLog.Append(" " + remoteFiles[i] + ".CopyTo(" + localFileName + ", true);\r\n");

					if ( configFile )
					{
						NotOverwriteConfigFile(remoteFiles[i], localFileName, ref xmlCcTakeMe);
					}
				}

				// コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
				string[] dirs = Directory.GetDirectories(subdirRemoteProgram.FullName);
				foreach ( string dir in dirs )
				{
					CopyDirectory(dir, dirLocalProgram.FullName + "\\" + Path.GetFileName(dir));
				}

				// 更新されたアプリケーションの属性をセットする
				SetApplicationAttribute(ref application, ATTRIB_NEW, true.ToString().ToLower());

#if RESUME_APP_ENVIRONMENT
				RemoveLatestAppEnvironment(application[TAG_APP_NAME].InnerText);
#endif
			}
			catch ( Exception exp )
			{
				ccConfigurationLog.Append(" [EXCEPTION] " + MethodBase.GetCurrentMethod().Name + " " + exp.Message.Replace("\r\n", "\t") + "\r\n");
				// 呼び出し側で例外をキャッチする
				throw exp;
			}
		}

		/// <summary>
		/// GetCopyMode
		/// </summary>
		/// <param name="copyMode"></param>
		/// <returns></returns>
		private cm GetCopyMode(string copyMode)
		{
			if ( copyMode == cm.onceCopy.ToString() )
				return cm.onceCopy;
			else if ( copyMode == cm.shortCut.ToString() )
				return cm.shortCut;
			else if ( copyMode == cm.existCopy.ToString() )
				return cm.existCopy;
			return cm.normal;
		}

		/// <summary>
		/// アプリケーション構成ファイルであれば、バックアップする
		/// </summary>
		private bool BackUpConfigFile(FileInfo remoteFile, string localFileName)
		{
			bool configFile = false;

			try
			{
				if ( remoteFile.Extension.ToLower() == ".config" ||
					remoteFile.Extension.ToUpper() == ".INI" ||
					remoteFile.Extension.ToLower() == ".xml" )
				{
					if ( File.Exists(localFileName) )
					{
						File.Copy(localFileName, localFileName + ".bak", true);
						ccConfigurationLog.Append(" File.Copy(" + localFileName + ", " + localFileName + ".bak" + ", true);\r\n");
					}

					configFile = true;
				}
			}
			catch ( Exception exp )
			{
				ccConfigurationLog.Append(" [EXCEPTION] " + MethodBase.GetCurrentMethod().Name + " " + exp.Message.Replace("\r\n", "\t") + "\r\n");
			}

			return configFile;
		}

		/// <summary>
		/// アプリケーション構成ファイルの上書き禁止キーをバックアップ ファイルから復元する
		/// </summary>
		private void NotOverwriteConfigFile(FileInfo remoteFile, string localFileName, ref XmlDocument xmlCcTakeMe)
		{
			try
			{
				if ( !File.Exists(localFileName + ".bak") )
					return;

				// .config ファイル
				if ( remoteFile.Extension.ToLower() == ".config" )
				{
					string xpath = "/" + TAG_APPLICATION + "/" + TAG_NOT_OVERWRITE_CONFIG + "[@" + ATTRIB_FILE_NAME + "='" + remoteFile.Name + "']";
					XmlNode nodeNotOverwriteConfig = xmlCcTakeMe.SelectSingleNode(xpath);
					if ( nodeNotOverwriteConfig == null )
						return;

					int childNodesCount = nodeNotOverwriteConfig.ChildNodes.Count;
					if ( (childNodesCount == 0) || (childNodesCount == 1 && nodeNotOverwriteConfig.FirstChild.NodeType == XmlNodeType.Comment/*nodeNotOverwriteConfig.FirstChild.Name == Program.NODE_COMMENT*/) )
					{
						File.Copy(localFileName + ".bak", localFileName, true);
						ccConfigurationLog.Append(" File.Copy(" + localFileName + ".bak" + ", " + localFileName + ", true);\r\n");
						return;
					}

					AppConfig oldAppConfig = new AppConfig(localFileName + ".bak");
					AppConfig newAppConfig = new AppConfig(localFileName);
					//XmlNodeList listKey = nodeNotOverwriteConfig.SelectNodes (TAG_KEY);
					foreach ( XmlNode nodeKey in /*listKey*/nodeNotOverwriteConfig.SelectNodes(TAG_KEY) )
					{
						string cfgValue = oldAppConfig.GetValue(nodeKey.InnerText);
						newAppConfig.SetValue(nodeKey.InnerText, cfgValue);
						ccConfigurationLog.Append(" newAppConfig.SetValue(" + nodeKey.InnerText + ", " + cfgValue + ");\r\n");
					}
				}
				// .xml ファイル
				else if ( remoteFile.Extension.ToLower() == ".xml" )
				{
					string xpath = "/" + TAG_APPLICATION + "/" + TAG_NOT_OVERWRITE_XML + "[@" + ATTRIB_FILE_NAME + "='" + remoteFile.Name + "']";
					XmlNode nodeNotOverwriteXml = xmlCcTakeMe.SelectSingleNode(xpath);
					if ( nodeNotOverwriteXml == null )
						return;

					int childNodesCount = nodeNotOverwriteXml.ChildNodes.Count;
					if ( (childNodesCount == 0) || (childNodesCount == 1 && nodeNotOverwriteXml.FirstChild.NodeType == XmlNodeType.Comment/*nodeNotOverwriteXml.FirstChild.Name == Program.NODE_COMMENT*/) )
					{
						File.Copy(localFileName + ".bak", localFileName, true);
						ccConfigurationLog.Append(" File.Copy(" + localFileName + ".bak" + ", " + localFileName + ", true);\r\n");
					}
				}
				// .INI ファイル
				else if ( remoteFile.Extension.ToUpper() == ".INI" )
				{
					string xpath = "/" + TAG_APPLICATION + "/" + TAG_NOT_OVERWRITE_INI + "[@" + ATTRIB_FILE_NAME + "='" + remoteFile.Name.ToUpper() + "']";
					XmlNode nodeNotOverwriteIni = xmlCcTakeMe.SelectSingleNode(xpath);
					if ( nodeNotOverwriteIni == null )
						return;

					int childNodesCount = nodeNotOverwriteIni.ChildNodes.Count;
					if ( (childNodesCount == 0) || (childNodesCount == 1 && nodeNotOverwriteIni.FirstChild.NodeType == XmlNodeType.Comment/*nodeNotOverwriteIni.FirstChild.Name == Program.NODE_COMMENT*/) )
					{
						File.Copy(localFileName + ".bak", localFileName, true);
						ccConfigurationLog.Append(" File.Copy(" + localFileName + ".bak" + ", " + localFileName + ", true);\r\n");
						return;
					}

					foreach ( XmlNode nodeSection in nodeNotOverwriteIni.SelectNodes(TAG_SECTION) )
					{
						string sectionName = nodeSection.Attributes[ATTRIB_NAME].InnerText;
						foreach ( XmlNode nodeKey in nodeSection.SelectNodes(TAG_KEY) )
						{
							StringBuilder cfgValue = new StringBuilder(1024);
							api.GetPrivateProfileString(sectionName, nodeKey.InnerText, "", cfgValue, (uint)cfgValue.Capacity, localFileName + ".bak");
							api.WritePrivateProfileString(sectionName, nodeKey.InnerText, cfgValue.ToString(), localFileName);
							ccConfigurationLog.Append(" api.WritePrivateProfileString(" + sectionName + ", " + nodeKey.InnerText + ", " + cfgValue.ToString() + ", " + localFileName + ");\r\n");
						}
					}
				}
			}
			catch ( Exception exp )
			{
				ccConfigurationLog.Append(" [EXCEPTION] " + MethodBase.GetCurrentMethod().Name + " " + exp.Message.Replace("\r\n", "\t") + "\r\n");
			}
		}

		/// <summary>
		/// ディレクトリをコピーする
		/// </summary>
		/// <param name="sourceDirName">コピーするディレクトリ</param>
		/// <param name="destDirName">コピー先のディレクトリ</param>
		private void CopyDirectory(string sourceDirName, string destDirName)
		{
			try
			{
				ccConfigurationLog.Append(MethodBase.GetCurrentMethod().Name + "(" + sourceDirName + ", " + destDirName + ");\r\n");

				// コピー先のディレクトリがないときは作る
				if ( !Directory.Exists(destDirName) )
				{
					Directory.CreateDirectory(destDirName);
					////属性もコピー
					//File.SetAttributes(destDirName, File.GetAttributes(sourceDirName));
				}

				// コピー先のディレクトリ名の末尾に"\"をつける
				if ( destDirName[destDirName.Length - 1] != Path.DirectorySeparatorChar )
					destDirName = destDirName + Path.DirectorySeparatorChar;

				// コピー元のディレクトリにあるファイルをコピー
				string[] files = Directory.GetFiles(sourceDirName);
				foreach ( string file in files )
				{
					string destFileName = destDirName + Path.GetFileName(file);
					DateTime sourceDateTime = File.GetLastWriteTime(file);
					DateTime destDateTime = File.Exists(destFileName) ? File.GetLastWriteTime(destFileName) : DateTime.MinValue;
					if ( destDateTime < sourceDateTime )
					{
						File.Copy(file, destFileName, true);
						ccConfigurationLog.Append(" File.Copy(" + file + ", " + destFileName + ", true);\r\n");
					}
				}

				// コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
				string[] dirs = Directory.GetDirectories(sourceDirName);
				foreach ( string dir in dirs )
				{
					CopyDirectory(dir, destDirName + Path.GetFileName(dir));
				}
			}
			catch ( Exception exp )
			{
				ccConfigurationLog.Append(" [EXCEPTION] " + MethodBase.GetCurrentMethod().Name + " " + exp.Message.Replace("\r\n", "\t") + "\r\n");
			}
		}

		/// <summary>
		/// タブページのアイコン ファイルをコピーする
		/// </summary>
		/// <param name="remoteIconFileName"></param>
		/// <param name="localIconFileName"></param>
		/// <param name="tabName"></param>
		/// <param name="node"></param>
		private void CopyTabIconFile(string remoteIconFileName, string localIconFileName, string tabName, XmlNode node)
		{
			if ( !File.Exists(remoteIconFileName) )
				return;

			try
			{
				File.Copy(remoteIconFileName, localIconFileName, true);

				string tabIconFileName = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + tabName + ".ico";
				XmlAttribute attr = node.Attributes[ATTRIB_DELETE];
				if ( (attr == null) || !bool.Parse(attr.Value) )
				{
					//File.Copy(localIconFileName, Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + (TabCaption + ".ico")/*nodeTabIconFile.InnerText*/, true);
					File.Copy(localIconFileName, tabIconFileName, true);
				}
				else
				{
					File.Delete(tabIconFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 未使用のローカル プログラムのサブディレクトリを検索して削除する
		/// </summary>
		private bool RemoveNoUseLocalProgramsFolder(string localProgramsFolder, int localProgramsFolderRootLength)
		{
			string fileCcMushroomLocal = string.Empty;

			try
			{
				fileCcMushroomLocal = localProgramsFolder + CC_MUSHROOM_REMOTE_FILE_NAME;
				// ccMushroom 登録用のルート ディレクトリではない？
				if ( !File.Exists(fileCcMushroomLocal) )
					return false;

				ccConfigurationLog.Append("\r\n" + localProgramsFolder + " [ROOT]\r\n");

				DirectoryInfo dirLocalPrograms = new DirectoryInfo(localProgramsFolder);
				DirectoryInfo[] subdirLocalPrograms = dirLocalPrograms.GetDirectories();
				bool prevFolderWasRoot = false;

				for ( int i = 0, j = 0; i < subdirLocalPrograms.Length; i++ )
				{
					// ccMushroom 登録用のルート ディレクトリだった？
					if ( RemoveNoUseLocalProgramsFolder(subdirLocalPrograms[i].FullName, localProgramsFolderRootLength) )
					{
						prevFolderWasRoot = true;
						continue;
					}

					if ( prevFolderWasRoot )
					{
						ccConfigurationLog.Append("\r\n");
						prevFolderWasRoot = false;
					}

					ccConfigurationLog.Append(subdirLocalPrograms[i].FullName);

					string localProgramFolder = subdirLocalPrograms[i].FullName.Substring(localProgramsFolderRootLength);
					for ( j = 0; j < copiedRemoteProgramsFolder.Length; j++ )
					{
						if ( localProgramFolder == copiedRemoteProgramsFolder[j] )
							break;
					}
					// コピー済みのフォルダではない？
					if ( j == copiedRemoteProgramsFolder.Length )
					{
						if ( removeNoUseLocalProgramsFolder )
							subdirLocalPrograms[i].Delete(true);
						ccConfigurationLog.Append(" [removed]");
					}

					ccConfigurationLog.Append("\r\n");
				}

				// サブ ディレクトリが空になった？
				if ( dirLocalPrograms.GetDirectories().Length == 0 )
				{
					if ( removeNoUseLocalProgramsFolder )
						dirLocalPrograms.Delete(true);
					ccConfigurationLog.Append("\r\n" + dirLocalPrograms.FullName + " [ROOT][removed]\r\n");
				}
			}
			catch ( Exception exp )
			{
				string message = fileCcMushroomLocal + "\r\n未使用のローカル側のプログラム ディレクトリの削除が失敗しました.\r\n原因：" + exp.Message;
				MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
			}

			return true;
		}

		/// <summary>
		/// ScanJ1LauncherPrograms
		/// </summary>
		private bool ScanJ1LauncherPrograms(string remoteProgramsName, string remoteProgramsFolder, int remoteProgramsFolderRootLength, string j1LauncherFileName, string localProgramsFolder)
		{
			string TabCaption = string.Empty;
			string CmdExe = string.Empty;

			try
			{
				ccConfigurationLog.Append("\r\nbool " + MethodBase.GetCurrentMethod().Name + "(" + remoteProgramsFolder + ", " + remoteProgramsFolderRootLength + ", " + localProgramsFolder + ")\r\n");

				// J1Launcher 用の設定ファイル
				string fileJ1Launcher = remoteProgramsFolder + j1LauncherFileName/*J1_LAUNCHER_FILE_NAME*/;

				XmlDocument xmlJ1Launcher = new XmlDocument();
				xmlJ1Launcher.Load(fileJ1Launcher);

#if ENABLE_LOGON_REQUIRED
				XmlNode nodeLogonRequired = xmlJ1Launcher.DocumentElement[TAG_J1_LOGON_REQUIRED];
#endif

				string actualRemoteProgramsFolder = remoteProgramsFolder;
				int actualRemoteProgramsFolderRootLength = remoteProgramsFolderRootLength;
				XmlNode nodePath = xmlJ1Launcher.DocumentElement[TAG_J1_PATH];
				if ( nodePath != null )		// 実ディレクトリ指定がある？
				{
#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
					if ( (nodePath.Attributes["ignore"] == null) || !bool.Parse(nodePath.Attributes["ignore"].Value) )
					{
#endif
						ccConfigurationLog.Append(nodePath.OuterXml + "\r\n");
						actualRemoteProgramsFolder = nodePath.InnerText;
						actualRemoteProgramsFolderRootLength = actualRemoteProgramsFolder.Length;
#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
					}
#endif
				}

				// リモート プログラムのディレクトリ
#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
				string _remoteProgramDirName = actualRemoteProgramsFolder;
				XmlNode _nodeRemoteProgramDirName = xmlJ1Launcher.DocumentElement[TAG_J1_PROGRAM_SUB_DIR_NAME];
				if ( _nodeRemoteProgramDirName != null )
				{
					// リモート側のプログラム フォルダからプログラムを置いているディレクトリを抜き出す
					int index = actualRemoteProgramsFolder.IndexOf(_nodeRemoteProgramDirName.InnerText);
					if ( index != -1 )
					{
						_remoteProgramDirName = actualRemoteProgramsFolder.Substring(0, index + _nodeRemoteProgramDirName.InnerText.Length);
					}
				}
				DirectoryInfo dirRemotePrograms = new DirectoryInfo(_remoteProgramDirName);
#else
				DirectoryInfo dirRemotePrograms = new DirectoryInfo(actualRemoteProgramsFolder);
#endif
				string remoteSubDir = actualRemoteProgramsFolder.Substring(actualRemoteProgramsFolderRootLength);

				// ccMushroom 構成ファイルの <configuration>
				XmlElement elemConfig = xmlCcConfiguration[TAG_CONFIGURATION];
				XmlNode application;

				const string TAB_J1_POP = "POP-";
				const string TAB_J1_3RD = "3rd-";
				//const string launcher = @"\launcher\";
				//bool first3rdPUBLIC = true;

				foreach ( XmlNode nodeString in xmlJ1Launcher.DocumentElement.SelectSingleNode(TAG_J1_PRIORITY).ChildNodes )
				{
					if ( nodeString.Name != TAG_J1_STRING )
						continue;
					if ( nodeString.InnerText.Length == 0 )
						continue;

					// J1Launcher 用のタブ毎の設定ファイル
#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
					string fileLncTab = actualRemoteProgramsFolder + J1_LNC_TAB_PREFIX_FILE_NAME + nodeString.InnerText + ".xml";
#else
					string fileLncTabPath = (nodeString.Attributes[TAG_J1_PATH] == null) ? null : nodeString.Attributes[TAG_J1_PATH].Value;
					string fileLncTab = (fileLncTabPath == ATTR_J1_PATH_CURRENT ? remoteProgramsFolder : actualRemoteProgramsFolder) + J1_LNC_TAB_PREFIX_FILE_NAME + nodeString.InnerText + ".xml";
#endif

					XmlDocument xmlLncTab = new XmlDocument();
					xmlLncTab.Load(fileLncTab);

					/*string */TabCaption = xmlLncTab.DocumentElement[TAG_J1_TABCAPTION].InnerText;
					Debug.WriteLine(TabCaption);

					bool tabPOP = TabCaption.StartsWith(TAB_J1_POP);
					bool tab3rd = TabCaption.StartsWith(TAB_J1_3RD);

					string subDirName = @"\Launcher\";
					if ( xmlLncTab.DocumentElement[TAG_J1_SUBDIR_NAME] != null )
					{
						//subDirName = @"\" + xmlLncTab.DocumentElement[TAG_J1_SUBDIR_NAME].InnerText + @"\";
						subDirName = xmlLncTab.DocumentElement[TAG_J1_SUBDIR_NAME].InnerText;
						subDirName = ((subDirName[0] != '\\') && (subDirName[0] != '/')) ? (@"\" + subDirName + @"\") : subDirName;
					}

					XmlNode nodeExeInfo = xmlLncTab.DocumentElement[TAG_J1_EXEINFO];
					if ( nodeExeInfo == null )
						continue;

					// <ExeInfo backColor="GhostWhite">

					string _exeButtonBackColor = null;
					if ( nodeExeInfo.Attributes[ATTRIB_BACK_COLOR] != null )
					{
						_exeButtonBackColor = nodeExeInfo.Attributes[ATTRIB_BACK_COLOR].Value;
					}

					foreach ( XmlNode nodeEXE_INFO in nodeExeInfo.ChildNodes )
					{
						if ( nodeEXE_INFO.Name != TAG_J1_EXE_INFO || nodeEXE_INFO.ChildNodes.Count == 0 )
							continue;

						// <EXE_INFO autoExec=true">
						//   <CmdExe>C:\Program Files\Launcher\iwate_cps\QTR002\QTR002.exe</CmdExe>
						//   <CmdOption />
						//   <CmdCaption backColor="White">ライン投入</CmdCaption>
						// </EXE_INFO>

						/*string */CmdExe = nodeEXE_INFO[TAG_J1_CMDEXE].InnerText;
						Debug.WriteLine(" " + CmdExe);
						int iLauncher = CmdExe.IndexOf(subDirName/*launcher*/, StringComparison.OrdinalIgnoreCase);
						if ( iLauncher == -1 )
							continue;

#if DEBUG
						if ( CmdExe == @"C:\Program Files\Launcher\iwate_cps_mst\PMR005\PMR005.exe" ) { }
						{
						}
#endif

						string CmdOption = (nodeEXE_INFO[TAG_J1_CMDOPTION] != null) ? nodeEXE_INFO[TAG_J1_CMDOPTION].InnerText : string.Empty;

						//string CmdCaption = (nodeEXE_INFO[TAG_J1_CMDCAPTION] != null) ? nodeEXE_INFO[TAG_J1_CMDCAPTION].InnerText : string.Empty;
						string CmdCaption = string.Empty;
						string _buttonBackColor = _exeButtonBackColor;
						XmlNode cmdCaptionNode= nodeEXE_INFO[TAG_J1_CMDCAPTION];
						if ( cmdCaptionNode != null )
						{
							CmdCaption = cmdCaptionNode.InnerText;
							if ( cmdCaptionNode.Attributes[ATTRIB_BACK_COLOR] != null )
							{
								_buttonBackColor = cmdCaptionNode.Attributes[ATTRIB_BACK_COLOR].Value;
							}
						}

						string subDirectory = CmdExe.Substring(iLauncher + subDirName/*launcher*/.Length - 1);
						subDirectory = Path.GetDirectoryName(subDirectory);
#if true				// 2011/10/04 */
						if ( CmdExe.StartsWith("http://") )
						{
							// リモート側にボタン名と同じフォルダがあれば、以降それをサブ ディレクトリとして取り扱う
							if ( Directory.Exists(dirRemotePrograms.FullName + "\\" + CmdCaption) )
							{
								subDirectory = "\\" + CmdCaption;
							}
						}
#endif

#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
						DirectoryInfo subDirRemoteProgram = new DirectoryInfo(dirRemotePrograms.FullName + subDirectory);
						remoteProgramsFolderRootLength = dirRemotePrograms.FullName.Length;
#else
						DirectoryInfo subDirRemoteProgram = new DirectoryInfo(actualRemoteProgramsFolder + subDirectory);
						remoteProgramsFolderRootLength = actualRemoteProgramsFolderRootLength;
#endif

						if ( tabPOP )		// POP タブ専用処理
						{
							TabCaption = TAB_J1_POP.Substring(0, TAB_J1_POP.Length - 1);
						}
#if false
						else if ( tab3rd )	// 3rd タブ専用処理
						{
							if ( (CmdOption.IndexOf(@"\PUBLIC") != -1) && ((iLauncher = CmdOption.ToLower().IndexOf(launcher)) != -1) )
							{
								string subDirPUBLIC = CmdOption.Substring(iLauncher + launcher.Length - 1);
								CmdOption = localProgramsFolder + subDirPUBLIC;

								if ( first3rdPUBLIC )
								{
									CopyDirectory(actualRemoteProgramsFolder + subDirPUBLIC, localProgramsFolder + subDirPUBLIC);

									// PUBLIC フォルダをコピー済みとして登録する
									int countScanned = (copiedRemoteProgramsFolder == null) ? 0 : copiedRemoteProgramsFolder.Length;
									if ( copiedRemoteProgramsFolder == null )
										copiedRemoteProgramsFolder = new string[0];
									Array.Resize(ref copiedRemoteProgramsFolder, countScanned + 1);
									copiedRemoteProgramsFolder[countScanned] = actualRemoteProgramsFolder.Substring(actualRemoteProgramsFolderRootLength) + subDirPUBLIC;

									// ダミーの CC_TAKE_ME_FILE_NAME ファイルを PUBLIC フォルダに保存しておく
									string fileCcTakeMe = localProgramsFolder + subDirPUBLIC + CC_TAKE_ME_FILE_NAME;
									FileStream fs = File.Create(fileCcTakeMe);
									fs.Close();

									first3rdPUBLIC = false;
								}
							}
						}
#else
						if ( CmdOption.Length != 0 )	// コマンド パラメータで指定されているフォルダをコピーする
						{
							CmdOption = CmdOption.Replace("Program Files", "Program_Files");
							string[] CmdOptions = CmdOption.Split(' ');

							for ( int i = 0; i < CmdOptions.Length; i++ )
							{
								CmdOptions[i] = CmdOptions[i].Replace("Program_Files", "Program Files");
								// フォルダ関連のパラメータではない？
								if ( CmdOptions[i].IndexOf("\\") == -1 ||
									 (iLauncher = CmdOptions[i].IndexOf(subDirName/*launcher*/, StringComparison.OrdinalIgnoreCase)) == -1 )
									continue;

								string dirCmdOption = Path.GetDirectoryName(CmdOptions[i]);
								string fileCmdOption = CmdOptions[i].Substring(dirCmdOption.Length);
								if ( fileCmdOption.Length != 0 && Path.GetExtension(fileCmdOption).Length == 0 )
								{
									dirCmdOption += fileCmdOption;
									fileCmdOption = string.Empty;
								}
								string subDirCmdOption = dirCmdOption.Substring(iLauncher + subDirName/*launcher*/.Length - 1);

								// パラメータ フォルダを ccMushroom 用に書き換える
								CmdOptions[i] = localProgramsFolder + subDirCmdOption;
								if ( fileCmdOption.Length != 0 )
									CmdOptions[i] += fileCmdOption;

								int countScanned = (copiedRemoteProgramsFolder == null) ? 0 : copiedRemoteProgramsFolder.Length, j;
								for ( j = 0; j < countScanned && (copiedRemoteProgramsFolder[j] != subDirCmdOption); j++ ) { }
								if ( j == countScanned )
								{
									// パラメータ フォルダをコピー済みとして登録する
									if ( copiedRemoteProgramsFolder == null )
										copiedRemoteProgramsFolder = new string[0];
									Array.Resize(ref copiedRemoteProgramsFolder, countScanned + 1);
									copiedRemoteProgramsFolder[countScanned] = subDirCmdOption;

									// パラメータ フォルダをコピーする
#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
									CopyDirectory(dirRemotePrograms.FullName + subDirCmdOption, localProgramsFolder + subDirCmdOption);
#else
									CopyDirectory(actualRemoteProgramsFolder + subDirCmdOption, localProgramsFolder + subDirCmdOption);
#endif

									// ダミーの CC_TAKE_ME_FILE_NAME ファイルをパラメータ フォルダに保存しておく
									string fileCcTakeMe = localProgramsFolder + subDirCmdOption + CC_TAKE_ME_FILE_NAME;
									FileStream fs = File.Create(fileCcTakeMe);
									fs.Close();
								}
							}

							CmdOption = string.Join(" ", CmdOptions);
						}
#endif

						string workingDirectory = (nodeEXE_INFO[TAG_WORKING_DIRECTORY] == null) ? null : nodeEXE_INFO[TAG_WORKING_DIRECTORY].InnerText;

						if ( nodeEXE_INFO[TAG_J1_COPYMODE] == null )
						{
							nodeEXE_INFO.AppendChild(xmlLncTab.CreateElement(TAG_J1_COPYMODE));
						}
						cm copyMode = GetCopyMode(nodeEXE_INFO[TAG_J1_COPYMODE].InnerText);

#if true				// 2011/10/04 */
						string iconFile = (nodeEXE_INFO[TAG_ICON_FILE] != null) ? nodeEXE_INFO[TAG_ICON_FILE].InnerText : null;
#endif
						bool autoExec = (nodeEXE_INFO.Attributes[ATTRIB_AUTO_EXEC] != null) ? bool.Parse(nodeEXE_INFO.Attributes[ATTRIB_AUTO_EXEC].Value) : false;

#if true				// 2011/10/04 */
						string importCcTakeMeNotOverwriteSetting = (nodeEXE_INFO.Attributes[ATTR_J1_IMPORT_CCTAKEME_NOT_OVERWRITE_SETTING] != null) ? nodeEXE_INFO.Attributes[ATTR_J1_IMPORT_CCTAKEME_NOT_OVERWRITE_SETTING].Value : null;
#endif
						if ( (application = CopyJ1LauncherProgramFiles(subDirRemoteProgram, remoteProgramsFolderRootLength, localProgramsFolder, TabCaption, CmdCaption, _buttonBackColor, CmdExe/*Path.GetFileName(CmdExe)*/, CmdOption, workingDirectory, copyMode, iconFile, autoExec, importCcTakeMeNotOverwriteSetting)) != null )
						{
#if ENABLE_LOGON_REQUIRED
							if ( nodeLogonRequired != null )
							{
								XmlNode logonRequired = xmlCcConfiguration.ImportNode(nodeLogonRequired, true);	// <logonRequired>
								XmlAttribute attrib = xmlCcConfiguration.CreateAttribute(ATTRIB_NAME);			// @name
								attrib.Value = remoteProgramsName;
								logonRequired.Attributes.Append(attrib);
								attrib = logonRequired.Attributes[ATTR_J1_EXPIRES];								// @expires
								if ( attrib == null )
								{
									attrib = xmlCcConfiguration.CreateAttribute(ATTR_J1_EXPIRES);
									attrib.Value = "4H";												// 有効期限のデフォルト
									logonRequired.Attributes.Append(attrib);
								}
								else
								{
									if ( !Char.IsLetter(attrib.Value[attrib.Value.Length - 1]) )
									{
										attrib.Value += "H";											// 有効期限の単位のデフォルト
									}
								}
								application.AppendChild(logonRequired);
							}
#endif

#if PROGRAM_SUBDIR_NAME_IN_LAUNCHER
							application.Attributes[ATTRIB_REMOTE_PROGRAMS_FOLDER].Value = actualRemoteProgramsFolder;
#endif

							elemConfig.AppendChild(application);	// ccMushroom 構成ファイルに登録する
						}
					}

#if true
					// タブページのアイコン指定があればローカルにコピーしておく
					string xpath =  "/" + TAG_J1_TAB_INFO + "/" + TAG_TAB_ICON_FILE;
					XmlNode nodeTabIconFile = xmlLncTab.SelectSingleNode(xpath);
					if ( nodeTabIconFile != null )
					{
						string remoteIconFileName = actualRemoteProgramsFolder/*dirRemotePrograms*/ + "\\" + nodeTabIconFile.InnerText;
						string localIconFileName = localProgramsFolder + remoteSubDir + "\\" + nodeTabIconFile.InnerText;
						CopyTabIconFile(remoteIconFileName, localIconFileName, TabCaption, nodeTabIconFile);
					}
#endif

					// J1Launcher 用のタブ毎の設定ファイルをローカル側に保存しておく
					string fileLncTabLocal = localProgramsFolder + J1_LNC_TAB_PREFIX_FILE_NAME + nodeString.InnerText + ".xml";
					File.Copy(fileLncTab, fileLncTabLocal, true);
				}

				// ダミーの CC_MUSHROOM_REMOTE_FILE_NAME ファイルをローカル側に保存しておく
				string fileCcMushroomLocal = localProgramsFolder + remoteSubDir + CC_MUSHROOM_REMOTE_FILE_NAME;
				File.Copy(fileJ1Launcher, fileCcMushroomLocal, true);

				// 実際のリモート プログラムのディレクトリを保存する
				//string iniFileName = Application.StartupPath + CCMUSHROOM_INI_FILE_NAME;
				api.WritePrivateProfileString(RESUME_SECTION, remoteProgramsFolder, actualRemoteProgramsFolder, ccMushroomIniFileName);
			}
			catch ( Exception exp )
			{
				string message = remoteProgramsFolder + "\r\n" + "[" + TabCaption + "] " + CmdExe + "\r\nＪ１ランチャ プログラムのスキャンが失敗しました.このフォルダはスキップします.\r\n原因:" + exp.Message;
				MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
			}

			return true;
		}

		/// <summary>
		/// CopyJ1LauncherProgramFiles
		/// </summary>
		private XmlNode CopyJ1LauncherProgramFiles(DirectoryInfo subdirRemoteProgram, int remoteProgramsFolderRootLength, string localProgramsFolder, string TabCaption, string CmdCaption, string _buttonBackColor, string CmdExe, string CmdOption, string workingDirectory, cm copyMode, string iconFile, bool autoExec, string importCcTakeMeNotOverwriteSetting)
		{
			XmlNode application = null;
			string appName = Path.GetFileName(CmdExe);

			try
			{
				ccConfigurationLog.Append("\r\nXmlNode " + MethodBase.GetCurrentMethod().Name + "(" + subdirRemoteProgram + ", " + remoteProgramsFolderRootLength + ", " + localProgramsFolder + ", " + TabCaption + ", " + CmdCaption + ", " + appName + ", " + CmdOption + ")\r\n");
				string message1, message2;
				if ( subdirRemoteProgram.Parent != null )
				{
					message1 = subdirRemoteProgram.Parent.FullName;
					message2 = subdirRemoteProgram.Name;
				}
				else
				{
					int index = subdirRemoteProgram.FullName.IndexOf(subdirRemoteProgram.Name);
					if ( (index != -1) && (0 <= subdirRemoteProgram.FullName.Length - (subdirRemoteProgram.Name.Length + 1)) )
					{
						message1 = subdirRemoteProgram.FullName.Substring(0, index - 1);
						message2 = subdirRemoteProgram.Name;
					}
					else
					{
						message1 = subdirRemoteProgram.FullName;
						message2 = "";
					}
				}
				ShowProgressMessage(message1/*subdirRemoteProgram.Parent.FullName*/, message2/*subdirRemoteProgram.Name*/);

				// リモート プログラム フォルダをコピー済みとして登録する
				string remoteSubDir = subdirRemoteProgram.FullName.Substring(remoteProgramsFolderRootLength);
				int countScanned = (copiedRemoteProgramsFolder == null) ? 0 : copiedRemoteProgramsFolder.Length;
				/*for ( int i = 0; i < countScanned; i++ )
				{
					if ( copiedRemoteProgramsFolder[i] == remoteSubDir )
					{
						ccConfigurationLog.Append ("[already]\r\n");
						return null;
					}
				}*/
				if ( copiedRemoteProgramsFolder == null )
					copiedRemoteProgramsFolder = new string[0];
				Array.Resize(ref copiedRemoteProgramsFolder, countScanned + 1);
				copiedRemoteProgramsFolder[countScanned] = remoteSubDir;

				// リモートのアプリケーション構成ファイルを作成する
				XmlDocument xmlCcTakeMe = new XmlDocument();

				XmlDeclaration decl = xmlCcTakeMe.CreateXmlDeclaration("1.0", "utf-8", null);
				xmlCcTakeMe.AppendChild(decl);

				// <application>
				XmlElement elemApplication = xmlCcTakeMe.CreateElement(TAG_APPLICATION);
				xmlCcTakeMe.AppendChild(elemApplication);

				// @autoExec
				if ( autoExec )
				{
					XmlAttribute attr = xmlCcTakeMe.CreateAttribute(ATTRIB_AUTO_EXEC);
					attr.Value = autoExec.ToString().ToLower();
					elemApplication.Attributes.Append(attr);
				}

				// <tabText>
				XmlElement elemConfig = xmlCcTakeMe.CreateElement(TAG_TAB_TEXT);
				elemConfig.InnerText = TabCaption;
				elemApplication.AppendChild(elemConfig);

				// <buttonText>
				while ( CmdCaption.IndexOf("  ") != -1 )
					CmdCaption = CmdCaption.Replace("  ", " ");
				elemConfig = xmlCcTakeMe.CreateElement(TAG_BUTTON_TEXT);
				elemConfig.InnerText = CmdCaption;
				elemApplication.AppendChild(elemConfig);

				if ( !string.IsNullOrEmpty(_buttonBackColor) )
				{
					// @backColor
					XmlAttribute attr = xmlCcTakeMe.CreateAttribute(ATTRIB_BACK_COLOR);
					attr.Value = _buttonBackColor;
					elemConfig.Attributes.Append(attr);
				}

				// <appName>
				elemConfig = xmlCcTakeMe.CreateElement(TAG_APP_NAME);
				elemConfig.InnerText = appName;
				elemApplication.AppendChild(elemConfig);

				// <commandLine>
				if ( !string.IsNullOrEmpty(CmdOption) )
				{
					elemConfig = xmlCcTakeMe.CreateElement(TAG_COMMAND_LINE);
					elemConfig.InnerText = CmdOption;
					elemApplication.AppendChild(elemConfig);
				}

				// <workingDirectory>
				if ( !string.IsNullOrEmpty(workingDirectory) )
				{
					elemConfig = xmlCcTakeMe.CreateElement(TAG_WORKING_DIRECTORY);
					elemConfig.InnerText = workingDirectory;
					elemApplication.AppendChild(elemConfig);
				}

				/*// <onceCopy>
				elemConfig = xmlCcTakeMe.CreateElement(TAG_ONCE_COPY);
				elemConfig.InnerText = false.ToString().ToLower();
				elemApplication.AppendChild(elemConfig);*/
				// <copyMode>
				elemConfig = xmlCcTakeMe.CreateElement(TAG_COPY_MODE);
				elemConfig.InnerText = copyMode.ToString();
				elemApplication.AppendChild(elemConfig);

#if true		// 2011/10/04 */
				// <iconFile>
				if ( !string.IsNullOrEmpty(iconFile) )
				{
					elemConfig = xmlCcTakeMe.CreateElement(TAG_ICON_FILE);
					elemConfig.InnerText = iconFile;
					elemApplication.AppendChild(elemConfig);
				}
#endif

#if false
				// <notOverwriteIni>
				foreach ( FileInfo iniFile in subdirRemoteProgram.GetFiles("*.INI") )
				{
					elemConfig = xmlCcTakeMe.CreateElement(TAG_NOT_OVERWRITE_INI);
					elemConfig.SetAttribute(ATTRIB_FILE_NAME, iniFile.Name.ToUpper());
					elemApplication.AppendChild(elemConfig);
				}
#else			// 2011/10/04 */
				// 設定を上書きしないインポート属性があり、リモート側に cctakeme.xml ファイルが存在する？
				if ( !string.IsNullOrEmpty(importCcTakeMeNotOverwriteSetting) && File.Exists(subdirRemoteProgram.FullName + CC_TAKE_ME_FILE_NAME) )
				{
					XmlDocument remoteCcTakeMe = new XmlDocument();
					remoteCcTakeMe.Load(subdirRemoteProgram.FullName + CC_TAKE_ME_FILE_NAME);
					StringBuilder xpath = new StringBuilder();
					xpath.Append("/" + TAG_APPLICATION + "/" + "*[");
					string[] settings = importCcTakeMeNotOverwriteSetting.Split(',');
					foreach ( string setting in settings )
					{
						xpath.Append("local-name()='notOverwrite" + setting + "'" + " or ");
					}
					xpath.Length -= 4;	// 4: or
					xpath.Append("]");
					// <notOverwriteConfig|Xml|Ini>
					XmlNodeList settingList = remoteCcTakeMe.SelectNodes(xpath.ToString());
					foreach ( XmlNode settingNode in settingList )
					{
						XmlNode _settingNode = xmlCcTakeMe.ImportNode(settingNode, true);
						elemApplication.AppendChild(_settingNode);
					}
				}
#endif

				DirectoryInfo dirLocalProgram = new DirectoryInfo(localProgramsFolder + copiedRemoteProgramsFolder[countScanned]/*"\\" + subdirRemoteProgram.Name*/);

				if ( copyMode == cm.shortCut )
				{
					// <appName>
					elemApplication[TAG_APP_NAME].InnerText = CmdExe;

					// <workingDirectory>
					if ( (elemApplication[TAG_WORKING_DIRECTORY] == null) && !CmdExe.StartsWith("http://") )
					{
						elemConfig = xmlCcTakeMe.CreateElement(TAG_WORKING_DIRECTORY);
						elemConfig.InnerText = dirLocalProgram.FullName;
						elemApplication.AppendChild(elemConfig);
					}
				}

				// <application> ノードを作成する
				application = CreateApplicationNode(xmlCcTakeMe, subdirRemoteProgram, remoteProgramsFolderRootLength, dirLocalProgram);

				CopyRemoteProgramFiles(subdirRemoteProgram, dirLocalProgram, xmlCcTakeMe, ref application);

				//if ( copyMode != cm.shortCut )
				{
					// ダミーの CC_TAKE_ME_FILE_NAME ファイルをローカル側に保存しておく
					string fileCcTakeMe = dirLocalProgram.FullName + CC_TAKE_ME_FILE_NAME;
					xmlCcTakeMe.Save(fileCcTakeMe);

					// ダミーの CC_MUSHROOM_REMOTE_FILE_NAME ファイルをローカル側の親ディレクトリに保存しておく
					string fileCcMushroomLocal = dirLocalProgram.Parent.FullName + CC_MUSHROOM_REMOTE_FILE_NAME;
					if ( !File.Exists(fileCcMushroomLocal) )
					{
						FileStream fs = File.Create(fileCcMushroomLocal);
						fs.Close();
					}
				}
			}
			catch ( Exception exp )
			{
				string message = "tabText:" + TabCaption + " buttonText:" + CmdCaption + " appName:" + appName + "\r\nＪ１ランチャ ファイルのコピーが失敗しました.このプログラムはスキップします.\r\n原因:" + exp.Message;
				//MessageBox.Show (message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
				application = null;
			}

			return application;
		}
		#endregion

		#region ccMushroom 構成ファイルを読み込む処理
		/// <summary>
		/// ccMushroom 構成ファイルを読み込む
		/// </summary>
		private void ReadCcConfiguration()
		{
			try
			{
				string fileCcConfiguration = Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME;
				/*if ( !File.Exists(fileCcConfiguration) )
				{
					SplashForm.CloseSplash();
					MessageBox.Show(fileCcConfiguration + " ファイルがありません.", MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.Close();
					return;
				}*/
				if ( File.Exists(fileCcConfiguration) )
				{
					xmlCcConfiguration = new XmlDocument();
					xmlCcConfiguration.Load(fileCcConfiguration);
				}
				else
				{
					CreateCcConfiguration();
				}

				string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[@" + ATTRIB_IMPORTED + "]";
				foreach ( XmlNode application in xmlCcConfiguration.SelectNodes(xpath) )
				{
					// インポートされたアプリケーション削除しておく
					xmlCcConfiguration.DocumentElement.RemoveChild(application);
				}

				foreach ( XmlNode application in xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION) )
				{
					if ( application[TAG_APP_ENVIRON] != null )
					{
						// ここで一旦削除しておかないと、オフラインで toolStripComboAppEnviron.Enabled が false の時に前回の設定が残ったままになる
						// その後、RESUME_APP_ENVIRONMENT で前回の環境設定を復元するようにしたので、削除する必要は無くなった？が、このままにしておく
						application.RemoveChild(application[TAG_APP_ENVIRON]);
					}
				}

				// インポート用 ccMushroom 構成ファイルを読み込む
				ReadCcConfigurationImport();

#if RESUME_APP_ENVIRONMENT
				RemoveLatestAppEnvironmentWithoutConfiguration();
#endif

				// ccMushroom 構成ファイルを保存する（一応保存しておく）
				xmlCcConfiguration.Save(fileCcConfiguration);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// インポート用 ccMushroom 構成ファイルを読み込む
		/// </summary>
		private void ReadCcConfigurationImport()
		{
			string fileCcConfigurationImport = Application.StartupPath + "\\" + CC_CONFIGURATION_IMPORT_FILE_NAME;

			try
			{
				XmlDocument xmlCcConfigurationImport = new XmlDocument();

				if ( !File.Exists(fileCcConfigurationImport) )	// ファイルが存在しなければ作成する
				{
					XmlDeclaration decl = xmlCcConfigurationImport.CreateXmlDeclaration("1.0", "utf-8", null);
					xmlCcConfigurationImport.AppendChild(decl);

					XmlElement elemConfig = xmlCcConfigurationImport.CreateElement(TAG_CONFIGURATION);
					xmlCcConfigurationImport.AppendChild(elemConfig);

					XmlNode elemDefaultTabText = xmlCcConfigurationImport.CreateNode(XmlNodeType.Comment, TAG_DEFAULT_TAB_TEXT, null);
					elemDefaultTabText.InnerText = "<" + TAG_DEFAULT_TAB_TEXT + ">" + "お気に入り" + "<" + TAG_DEFAULT_TAB_TEXT + "/>";
					elemConfig.AppendChild(elemDefaultTabText);

					xmlCcConfigurationImport.Save(fileCcConfigurationImport);
					return;
				}

				xmlCcConfigurationImport.Load(fileCcConfigurationImport);
				int imported = 0;

				XmlNode nodeDefTabText = xmlCcConfigurationImport.DocumentElement[ccMushroom.TAG_DEFAULT_TAB_TEXT];	// <defaultTabText>
				defaultTabText = (nodeDefTabText == null) ? "お気に入り" : nodeDefTabText.InnerText;

				try
				{
					string tabDefaultTabTextFile = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + defaultTabText + ".ico";
					if ( !File.Exists(tabDefaultTabTextFile) )
					{
						string tabFavoriteFile = Application.StartupPath + "\\" + iconsFolder + "\\" + "tabお気に入り.ico";
						File.Copy(tabFavoriteFile, tabDefaultTabTextFile, true);
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}

				foreach ( XmlNode application in xmlCcConfigurationImport.DocumentElement.ChildNodes )
				{
					if ( application.Name != TAG_APPLICATION )
						continue;
#if true
					XmlAttribute attr = application.Attributes[ATTRIB_ENABLED];
					if ( (attr != null) && !bool.Parse(attr.Value) )
						continue;
#endif

					// appName
					string appName = ccf.CheckEnvironmentVariable(application[TAG_APP_NAME].InnerText);

					string message1, message2;
					if ( appName.StartsWith("http") )
					{
						message1 = "";
						message2 = appName;
					}
					else
					{
						FileInfo fi = new FileInfo(appName);
						message1 = (fi.DirectoryName != null) ? fi.Directory.FullName : "";
						message2 = /*fi.Directory.Name + "\\" + */fi.Name;
					}
					ShowProgressMessage(message1, message2);

					application[TAG_APP_NAME].InnerText = appName;

					// workingDirectory
					if ( application[TAG_WORKING_DIRECTORY] != null )
					{
						string workingDirectory = application[TAG_WORKING_DIRECTORY].InnerText;
						application[TAG_WORKING_DIRECTORY].InnerText = ccf.CheckEnvironmentVariable(workingDirectory);
					}

					XmlNode existApplication = null;
					//string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[" + TAG_TAB_TEXT + "='" + application[TAG_TAB_TEXT].InnerText + "' and " + TAG_BUTTON_TEXT + "='" + application[TAG_BUTTON_TEXT].InnerText + "' and " + TAG_APP_NAME + "='" + application[TAG_APP_NAME].InnerText + "']";
					//existApplication = xmlCcConfiguration.SelectSingleNode (xpath);
					if ( existApplication == null )
					{
						XmlNode importedApplication;
						if ( importTabAppearFirst )
						{
							if ( imported == 0 )
								importedApplication = xmlCcConfiguration.DocumentElement.PrependChild(xmlCcConfiguration.ImportNode(application, true));
							else
								importedApplication = xmlCcConfiguration.DocumentElement.InsertAfter(xmlCcConfiguration.ImportNode(application, true), xmlCcConfiguration.DocumentElement.ChildNodes[imported - 1]);
						}
						else
						{
							importedApplication = xmlCcConfiguration.DocumentElement.AppendChild(xmlCcConfiguration.ImportNode(application, true));
						}
						SetApplicationAttribute(ref importedApplication, ATTRIB_IMPORTED, true.ToString().ToLower());
					}
					else
					{
						if ( application[TAG_COMMAND_LINE] != null )
						{
							XmlNode commandLine = existApplication[TAG_COMMAND_LINE];
							if ( commandLine == null )
							{
								commandLine = xmlCcConfiguration.CreateNode(XmlNodeType.Element, TAG_COMMAND_LINE, null);
								existApplication.AppendChild(commandLine);
							}
							commandLine.InnerText = application[TAG_COMMAND_LINE].InnerText;
						}

						if ( application[TAG_WORKING_DIRECTORY] != null )
						{
							XmlNode workingDirectory = existApplication[TAG_WORKING_DIRECTORY];
							if ( workingDirectory == null )
							{
								workingDirectory = xmlCcConfiguration.CreateNode(XmlNodeType.Element, TAG_WORKING_DIRECTORY, null);
								existApplication.AppendChild(workingDirectory);
							}
							workingDirectory.InnerText = application[TAG_WORKING_DIRECTORY].InnerText;
						}
					}

					imported++;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(fileCcConfigurationImport + " 読込中に障害が発生しました.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		#endregion

		#region 各アプリケーションの設定を編集する処理
		/// <summary>
		/// ccAppEnvironmentSetting ファイルを読み込んで各アプリケーションの設定を編集する
		/// </summary>
		private bool ReadCcAppEnvironmentSetting(string appEnviron)
		{
			//Cursor.Current = Cursors.WaitCursor;

			string fileCcAppEnvironmentSetting = Application.StartupPath + "\\" + CC_APP_ENVIRONMENT_SETTING_FILE_NAME;
#if RESUME_APP_ENVIRONMENT
			string fileCcLatestAppEnvironment = Application.StartupPath + "\\" + CC_LATEST_APP_ENVIRONMENT_FILE_NAME;
#endif
			ccConfigurationLog = new StringBuilder();

			try
			{
				if ( (appEnviron == null) || (xmlCcConfiguration == null) || !File.Exists(fileCcAppEnvironmentSetting) )
				{
#if RESUME_APP_ENVIRONMENT
					try
					{
						if ( File.Exists(fileCcLatestAppEnvironment) )
						{
							File.Delete(fileCcLatestAppEnvironment);
						}
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
					}
#endif
					return false;
				}

				ccConfigurationLog.Append("\r\n\r\n==== void " + MethodBase.GetCurrentMethod().Name + "(" + appEnviron + ") ====\r\n");

				// アプリケーション環境設定ファイル
				XmlDocument xmlCcAppEnvironmentSetting = new XmlDocument();
				xmlCcAppEnvironmentSetting.Load(fileCcAppEnvironmentSetting);

#if RESUME_APP_ENVIRONMENT
				// 前回設定したアプリケーション環境設定
				XmlDocument xmlCcLatestAppEnvironment = new XmlDocument();
				if ( !File.Exists(fileCcLatestAppEnvironment) )
				{
					XmlDeclaration decl = xmlCcLatestAppEnvironment.CreateXmlDeclaration("1.0", "utf-8", null);
					xmlCcLatestAppEnvironment.AppendChild(decl);
					XmlElement elemConfig = xmlCcLatestAppEnvironment.CreateElement(TAG_CONFIGURATION);		// <configuration>
					xmlCcLatestAppEnvironment.AppendChild(elemConfig);
					xmlCcLatestAppEnvironment.Save(fileCcLatestAppEnvironment);
				}
				xmlCcLatestAppEnvironment.Load(fileCcLatestAppEnvironment);
#endif

				foreach ( XmlNode application in xmlCcAppEnvironmentSetting.DocumentElement.SelectNodes(TAG_APPLICATION) )
				{
					try
					{
						ccConfigurationLog.Append("\r\n" + application.Attributes[ATTRIB_NAME].Value + "\r\n");
						if ( !bool.Parse(application.Attributes[ATTRIB_ENABLED].Value) )	// 無効設定？
						{
							ccConfigurationLog.Append(" □enabled" + "\r\n");
							continue;
						}

						string appName = application[TAG_APP_NAME].InnerText;
						appName = ccf.ReplaceCcMushroomLocalFolder(appName, true);
						ccf.AdjustFolderFormat(ref appName);

						string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[" + TAG_APP_NAME + "='" + appName + "']";
						XmlNode ccApplication = xmlCcConfiguration.SelectSingleNode(xpath);
						if ( ccApplication == null )
						{
							ccConfigurationLog.Append(" " + xpath + " NOT FOUND. " + "\r\n");
							continue;
						}

						StringBuilder ccAppEnvironment = new StringBuilder();
						string appDirectory = Path.GetDirectoryName(appName);
						string appWorkingDirectory = (ccApplication[TAG_WORKING_DIRECTORY] == null) ? null : ccApplication[TAG_WORKING_DIRECTORY].InnerText;

						// <appSettingConfig> config ファイル
						xpath = TAG_APP_SETTING_CONFIG + "[@" + ATTRIB_ENVIRONMENT + "='" + appEnviron + "']";
						foreach ( XmlNode appSettingConfig in application.SelectNodes(xpath) )
						{
							string configFileName = appDirectory + "\\" + appSettingConfig.Attributes[ATTRIB_FILE_NAME].InnerText;

							if ( !AppEnvironmentSettingFile(configFileName, appSettingConfig.Attributes[ATTRIB_COPY_FROM], ref ccConfigurationLog) )
								continue;

							AppConfig appConfig = new AppConfig(configFileName);
							foreach ( XmlNode key in appSettingConfig.SelectNodes(TAG_KEY) )
							{
								string cfgKey = key.Attributes[ATTRIB_NAME].InnerText;
								string cfgValue = key.Attributes[ATTRIB_VALUE].InnerText;
								appConfig.SetValue(cfgKey, cfgValue);
								ccConfigurationLog.Append(" appConfig.SetValue(" + cfgKey + ", " + cfgValue + ");\r\n");
							}

							ccAppEnvironment.Append(Path.GetFileName(configFileName) + ",");
						}

						// <appSettingIni> ini ファイル
						xpath = TAG_APP_SETTING_INI + "[@" + ATTRIB_ENVIRONMENT + "='" + appEnviron + "']";
						foreach ( XmlNode appSettingIni in application.SelectNodes(xpath) )
						{
							/*string attribFileName = appSettingIni.Attributes[ATTRIB_FILE_NAME].InnerText;
							attribFileName = ccf.ReplaceCcMushroomLocalFolder(attribFileName, true);
							if ( attribFileName[0] == '\\' )	// ルートからディレクトリ指定されている？
							{
								attribFileName = Directory.GetCurrentDirectory().Substring(0, 2) + attribFileName;	// ドライブ名を付加する
							}
							//string iniFileName = (attribFileName.IndexOf("\\") == -1) ? (appDirectory + "\\" + attribFileName) : attribFileName;
							string iniFileName = ((attribFileName.IndexOf("\\") == -1) ? ((appWorkingDirectory ?? appDirectory) + "\\") : "") + attribFileName;*/
							string attribFileName = appSettingIni.Attributes[ATTRIB_FILE_NAME].InnerText;
							string iniFileName = GetEnvironmentFileName(ref attribFileName, appWorkingDirectory, appDirectory);

							if ( !AppEnvironmentSettingFile(iniFileName, appSettingIni.Attributes[ATTRIB_COPY_FROM], ref ccConfigurationLog) )
								continue;

							foreach ( XmlNode section in appSettingIni.SelectNodes(TAG_SECTION) )
							{
								string sectionName = section.Attributes[ATTRIB_NAME].InnerText;
								foreach ( XmlNode key in section.SelectNodes(TAG_KEY) )
								{
									string cfgKey = key.Attributes[ATTRIB_NAME].InnerText;
									string cfgValue = key.Attributes[ATTRIB_VALUE].InnerText;
									api.WritePrivateProfileString(sectionName, cfgKey, cfgValue, iniFileName);
									ccConfigurationLog.Append(" api.WritePrivateProfileString(" + sectionName + ", " + cfgKey + ", " + cfgValue + ", " + iniFileName + ");\r\n");
								}
							}

							api.WritePrivateProfileString(null, null, null, iniFileName);	// ini ファイルをフラッシュする
							ccAppEnvironment.Append(attribFileName + ",");
						}

						// <appSettingXml> xml ファイル
						xpath = TAG_APP_SETTING_XML + "[@" + ATTRIB_ENVIRONMENT + "='" + appEnviron + "']";
						foreach ( XmlNode appSettingXml in application.SelectNodes(xpath) )
						{
							//string xmlFileName = appDirectory + "\\" + appSettingXml.Attributes[ATTRIB_FILE_NAME].InnerText;
							string attribFileName = appSettingXml.Attributes[ATTRIB_FILE_NAME].InnerText;
							string xmlFileName = GetEnvironmentFileName(ref attribFileName, appWorkingDirectory, appDirectory);

							if ( !AppEnvironmentSettingFile(xmlFileName, appSettingXml.Attributes[ATTRIB_COPY_FROM], ref ccConfigurationLog) )
								continue;

							//ccAppEnvironment.Append(Path.GetFileName(xmlFileName) + ",");
							ccAppEnvironment.Append(attribFileName + ",");
						}

						if ( ccAppEnvironment.Length != 0 )	// 環境が変更された？
						{
							string textAppEnviron = appEnviron + " (" + ccAppEnvironment.ToString().Substring(0, ccAppEnvironment.Length - 1) + ")";
							XmlElement elemCcAppEnviron = ccApplication[TAG_APP_ENVIRON];			// <appEnviron>
							if ( elemCcAppEnviron == null )
							{
								elemCcAppEnviron = xmlCcConfiguration.CreateElement(TAG_APP_ENVIRON);
								ccApplication.AppendChild(elemCcAppEnviron);
							}
							elemCcAppEnviron.InnerText = textAppEnviron;							// 今回の環境設定を appEnviron ノードに書き込む
							/*XmlAttribute changed = elemCcAppEnviron.Attributes[ATTRIB_CHANGED];		// @changed
							if ( changed == null )
							{
								changed = xmlCcConfiguration.CreateAttribute(ATTRIB_CHANGED);
								elemCcAppEnviron.Attributes.Append(changed);
							}
							changed.Value = "*";*/

#if RESUME_APP_ENVIRONMENT
							SaveAppEnvironment(appName, ref xmlCcLatestAppEnvironment, textAppEnviron);		// 今回の環境設定を保存しておく
						}
						else
						{
							ResumeAppEnvironment(ccApplication, appName, ref xmlCcLatestAppEnvironment);	// 前回の環境設定を appEnviron ノードに復元する
						}
#else
						}
#endif
					}
					catch ( Exception exp )
					{
						string message = xmlCcAppEnvironmentSetting.BaseURI + " の <application> 読込中に障害が発生しました.\r\n" + application.OuterXml + "\r\nこの設定の編集はスキップします.\r\n原因：" + exp.Message;
						MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
					}
				}

#if RESUME_APP_ENVIRONMENT
				try
				{
					// アプリケーション環境設定には無いが、前回の環境設定が残っていれば復元する
					string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[count(" + TAG_APP_ENVIRON + ")=0]";
					foreach ( XmlNode ccApplication in xmlCcConfiguration.SelectNodes(xpath) )
					{
						string appName = ccApplication[TAG_APP_NAME].InnerText;
						ccf.AdjustFolderFormat(ref appName);

						ResumeAppEnvironment(ccApplication, appName, ref xmlCcLatestAppEnvironment);	// 前回の環境設定を appEnviron ノードに復元する
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}

				// 今回設定されたアプリケーション環境設定ファイルを保存する
				xmlCcLatestAppEnvironment.Save(fileCcLatestAppEnvironment);
#endif

				// ccMushroom 構成ファイルを保存する
				xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);

				if ( toolStripComboAppEnviron.SelectedItem != null )
				{
					SetFormBackColorByAppEnviron(toolStripComboAppEnviron.SelectedItem.ToString());
				}
			}
			catch ( Exception exp )
			{
				string message = fileCcAppEnvironmentSetting + " 読込中に障害が発生しました.\r\n原因：" + exp.Message;
				MessageBox.Show(message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				ccConfigurationLog.Append(" [EXCEPTION] " + message.Replace("\r\n", "\t") + "\r\n");
				return false;
			}

			// ログファイルを保存する
			SaveCcConfigurationLog(true);

			//Cursor.Current = Cursors.Default;
			return true;
		}

		/// <summary>
		/// アプリケーションの環境ファイルをフルパスで取得する
		/// </summary>
		/// <param name="attribFileName"></param>
		/// <param name="appWorkingDirectory"></param>
		/// <param name="appDirectory"></param>
		/// <returns></returns>
		private string GetEnvironmentFileName(ref string attribFileName, string appWorkingDirectory, string appDirectory)
		{
			attribFileName = ccf.ReplaceCcMushroomLocalFolder(attribFileName, true);
			attribFileName = ccf.CheckEnvironmentVariable(attribFileName);

			if ( attribFileName[0] == '\\' )	// ルートからディレクトリ指定されている？
			{
				attribFileName = Directory.GetCurrentDirectory().Substring(0, 2) + attribFileName;	// ドライブ名を付加する
			}

			string directoryName = (attribFileName.IndexOf("\\") == -1) ? ((appWorkingDirectory ?? appDirectory) + "\\") : "";

			return directoryName + attribFileName;
		}

		/// <summary>
		/// GetCurrentEnvironment
		/// </summary>
		private string GetCurrentEnvironment()
		{
			string appEnviron = null;

			//if ( toolStripComboAppEnviron.Enabled && toolStripComboAppEnviron.Items.Count != 0 )
			if ( toolStripComboAppEnviron.SelectedItem != null )
			{
				appEnviron = toolStripComboAppEnviron.SelectedItem.ToString();
			}

			return appEnviron;
		}

		/// <summary>
		/// 環境設定のファイルを処理する
		/// </summary>
		/// <param name="appSettingFileName"></param>
		/// <param name="copyFrom"></param>
		/// <param name="ccConfigurationLog"></param>
		/// <returns></returns>
		private bool AppEnvironmentSettingFile(string appSettingFileName, XmlAttribute copyFrom, ref StringBuilder ccConfigurationLog)
		{
			ccConfigurationLog.Append(/*"\r\n" + */appSettingFileName + "\r\n");

			if ( copyFrom != null )
			{
				try
				{
					string copyFromConfigFileName = ccf.ReplaceCcMushroomLocalFolder(copyFrom.Value, true);
					File.Copy(copyFromConfigFileName, appSettingFileName, true);
				}
				catch ( Exception exp )
				{
					ccConfigurationLog.Append(" " + exp.Message + "\r\n");
					return false;
				}

				ccConfigurationLog.Append(" " + ATTRIB_COPY_FROM + ": " + copyFrom.Value + "\r\n");
			}

			if ( !File.Exists(appSettingFileName) )
			{
				ccConfigurationLog.Append(" not found.\r\n");
				return false;
			}

			return true;
		}

#if RESUME_APP_ENVIRONMENT
		/// <summary>
		/// 今回の環境設定を保存しておく
		/// </summary>
		/// <param name="appName"></param>
		/// <param name="xmlCcLatestAppEnvironment"></param>
		/// <param name="textAppEnviron"></param>
		private void SaveAppEnvironment(string appName, ref XmlDocument xmlCcLatestAppEnvironment, string textAppEnviron)
		{
			string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[" + TAG_APP_NAME + "='" + appName + "']";
			XmlNode _application = xmlCcLatestAppEnvironment.SelectSingleNode(xpath);		// <application>
			if ( _application == null )
			{
				_application = xmlCcLatestAppEnvironment.CreateNode(XmlNodeType.Element, TAG_APPLICATION, null);
				XmlElement _appName = xmlCcLatestAppEnvironment.CreateElement(TAG_APP_NAME);// <appName>
				_appName.InnerText = appName;
				_application.AppendChild(_appName);
				xmlCcLatestAppEnvironment.DocumentElement.AppendChild(_application);
			}

			XmlElement _appEnviron = _application[TAG_APP_ENVIRON];							// <appEnviron>
			if ( _appEnviron == null )
			{
				_appEnviron = xmlCcLatestAppEnvironment.CreateElement(TAG_APP_ENVIRON);
				_application.AppendChild(_appEnviron);
			}

			_appEnviron.InnerText = textAppEnviron;											// 今回の環境設定を保存しておく
		}

		/// <summary>
		/// 前回の環境設定を appEnviron ノードに復元する
		/// </summary>
		/// <param name="ccApplication"></param>
		/// <param name="appName"></param>
		/// <param name="xmlCcLatestAppEnvironment"></param>
		private void ResumeAppEnvironment(XmlNode ccApplication, string appName, ref XmlDocument xmlCcLatestAppEnvironment)
		{
			string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[" + TAG_APP_NAME + "='" + appName + "']";
			XmlNode _application = xmlCcLatestAppEnvironment.SelectSingleNode(xpath);	// 前回の <application>
			if ( _application == null )
				return;

			XmlElement elemCcAppEnviron = ccApplication[TAG_APP_ENVIRON];				// <appEnviron>
			if ( elemCcAppEnviron == null )
			{
				elemCcAppEnviron = xmlCcConfiguration.CreateElement(TAG_APP_ENVIRON);
				ccApplication.AppendChild(elemCcAppEnviron);
			}

			elemCcAppEnviron.InnerText = _application[TAG_APP_ENVIRON].InnerText;		// 前回の環境設定を復元する
		}
#endif

#if RESUME_APP_ENVIRONMENT
		/// <summary>
		/// 前回設定したアプリケーション環境設定から、指定されたアプリ設定を削除する
		/// </summary>
		/// <param name="appName"></param>
		private void RemoveLatestAppEnvironment(string appName)
		{
			try
			{
				string fileCcLatestAppEnvironment = Application.StartupPath + "\\" + CC_LATEST_APP_ENVIRONMENT_FILE_NAME;
				if ( !File.Exists(fileCcLatestAppEnvironment) )
					return;

				XmlDocument xmlCcLatestAppEnvironment = new XmlDocument();
				xmlCcLatestAppEnvironment.Load(fileCcLatestAppEnvironment);

				ccf.AdjustFolderFormat(ref appName);
				string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[" + TAG_APP_NAME + "='" + appName + "']";
				XmlNode _application = xmlCcLatestAppEnvironment.SelectSingleNode(xpath);
				if ( _application == null )	// 前回の設定は無い？
					return;

				xmlCcLatestAppEnvironment.DocumentElement.RemoveChild(_application);

				xmlCcLatestAppEnvironment.Save(fileCcLatestAppEnvironment);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 前回設定したアプリケーション環境設定から、構成ファイルに無いアプリは削除しておく
		/// </summary>
		private void RemoveLatestAppEnvironmentWithoutConfiguration()
		{
			try
			{
				string fileCcLatestAppEnvironment = Application.StartupPath + "\\" + CC_LATEST_APP_ENVIRONMENT_FILE_NAME;
				if ( !File.Exists(fileCcLatestAppEnvironment) )
					return;

				XmlDocument xmlCcLatestAppEnvironment = new XmlDocument();
				xmlCcLatestAppEnvironment.Load(fileCcLatestAppEnvironment);
				XmlNodeList applications = xmlCcLatestAppEnvironment.DocumentElement.ChildNodes;

				for ( int i = applications.Count - 1; 0 <= i; i-- )
				{
					string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[" + TAG_APP_NAME + "='" + applications[i][TAG_APP_NAME].InnerText + "']";
					if ( xmlCcConfiguration.SelectSingleNode(xpath) == null )
					{
						xmlCcLatestAppEnvironment.DocumentElement.RemoveChild(applications[i]);
					}
				}

				xmlCcLatestAppEnvironment.Save(fileCcLatestAppEnvironment);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#endif

		/// <summary>
		/// アプリの環境名を toolStripComboAppEnviron にセットする
		/// </summary>
		/// <param name="selectedText"></param>
		private void SetComboAppEnviron(string selectedText)
		{
			Graphics g = null;

			try
			{
				// アプリケーション環境設定ファイル
				XmlDocument xmlCcAppEnvironmentSetting = new XmlDocument();

				string fileCcAppEnvironmentSetting = Application.StartupPath + "\\" + CC_APP_ENVIRONMENT_SETTING_FILE_NAME;
				if ( !File.Exists(fileCcAppEnvironmentSetting) )	// ファイルが存在しなければ作成する
				{
					XmlDeclaration decl = xmlCcAppEnvironmentSetting.CreateXmlDeclaration("1.0", "utf-8", null);
					xmlCcAppEnvironmentSetting.AppendChild(decl);

					XmlElement elemConfig = xmlCcAppEnvironmentSetting.CreateElement(TAG_CONFIGURATION);			// <configuration>
					xmlCcAppEnvironmentSetting.AppendChild(elemConfig);

					XmlElement appEnviron = xmlCcAppEnvironmentSetting.CreateElement(ccMushroom.TAG_APP_ENVIRON);	// <appEnviron>
					elemConfig.AppendChild(appEnviron);

					xmlCcAppEnvironmentSetting.Save(fileCcAppEnvironmentSetting);
				}

				toolStripComboAppEnviron.Items.Clear();

				//GDI+ 描画面を作成して、文字列の幅を測定します
				g = this.CreateGraphics();

				//最大幅の項目の値を保持する Float 変数
				float maxWidth = 0;

				xmlCcAppEnvironmentSetting.Load(fileCcAppEnvironmentSetting);
				XmlNode nodeAppEnviron = xmlCcAppEnvironmentSetting.DocumentElement[TAG_APP_ENVIRON];
				if ( nodeAppEnviron != null )
				{
					foreach ( XmlNode nodeItem in nodeAppEnviron.ChildNodes )
					{
						string item = nodeItem.InnerText;
						toolStripComboAppEnviron.Items.Add(item);
						maxWidth = Math.Max(maxWidth, g.MeasureString(item, toolStripComboAppEnviron.Font).Width);
					}
				}

				if ( toolStripComboAppEnviron.Enabled = (toolStripComboAppEnviron.Items.Count != 0) )
				{
					//空白文字用のバッファを
					//テキストに追加します
					//maxWidth += 24/*30*/;

					//maxWidth を四捨五入して int にキャストします
					int newWidth = (int)Decimal.Round((decimal)maxWidth, 0);

					//新しく計算した幅よりも小さい場合にのみ
					//既定の幅を変更します
					if ( newWidth > toolStripComboAppEnviron.DropDownWidth )
					{
						//toolStripComboAppEnviron.Width = newWidth;
						//toolStripComboAppEnviron.Size = new Size(newWidth, toolStripComboAppEnviron.Height);
						toolStripComboAppEnviron.DropDownWidth = newWidth;
					}

					int selectedIndex = 0;
					if ( selectedText != null )
					{
						int index = toolStripComboAppEnviron.Items.IndexOf(selectedText);
						selectedIndex = (index == -1) ? 0 : index;
					}
					toolStripComboAppEnviron.SelectedIndex = selectedIndex;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				//描画面をクリーンアップします
				if ( g != null )
				{
					g.Dispose();
				}
			}
		}

		/// <summary>
		/// toolStripComboAppEnviron の幅を調節する
		/// </summary>
		/// <param name="text"></param>
		private void AdjustComboAppEnviron(string text)
		{
			Graphics g = null;

			try
			{
				g = this.CreateGraphics();

				float width = g.MeasureString(text, toolStripComboAppEnviron.Font).Width;
				int newWidth = (int)Decimal.Round((decimal)width, 0);

				if ( newWidth != toolStripComboAppEnviron.Width )
				{
					newWidth = Math.Max(100 - 20, newWidth + 18);
					toolStripComboAppEnviron.Size = new Size(newWidth, toolStripComboAppEnviron.Height);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
			finally
			{
				if ( g == null )
				{
					g.Dispose();
				}
			}
		}

		/// <summary>
		/// アプリケーションの環境に合わせてフォームの背景色を変える
		/// </summary>
		/// <param name="text"></param>
		private void SetFormBackColorByAppEnviron(string text)
		{
#if true
			try
			{
				// アプリケーション環境設定ファイル
				string fileCcAppEnvironmentSetting = Application.StartupPath + "\\" + CC_APP_ENVIRONMENT_SETTING_FILE_NAME;
				XmlDocument xmlCcAppEnvironmentSetting = new XmlDocument();
				xmlCcAppEnvironmentSetting.Load(fileCcAppEnvironmentSetting);

				string xpath = "/" + TAG_CONFIGURATION + "/" + TAG_APP_ENVIRON + "/" + TAG_ITEM + "[.='" + text + "']";
				XmlNode itemNode = xmlCcAppEnvironmentSetting.SelectSingleNode(xpath);
				XmlAttribute attr = itemNode.Attributes[ATTRIB_FORM_BACK_COLOR];

				this.BackColor = (attr == null) ? formBackColor : Color.FromName(attr.Value);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif
		}
		#endregion

		#region コントロールの作成と配置処理
		/// <summary>
		/// コントロールの作成と配置
		/// </summary>
		private void CreateControlsDeployment()
		{
			try
			{
				int countTabPages = 0;
				int countButtons = 0;
				int minSizeTabPage = 0;
				int maxButtonTabPage = 0;
				int[] countButtonInTabPage = new int[0]/*null*/;
				bool[] tabPageHasNewApp = null;
				bool[] tabPageHasHScroll = null;
#if FOR_WINDOWS7
				runAsButtons = new List<string>();
#endif

				// コントロール テーブルを初期化する
				appTabPages = null;
				appButtons = null;
				appIcons = null;

				XmlNodeList applications = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION);

				InitProgressBar(0, applications.Count, 1, 0);

				// タブページの処理
				foreach ( XmlNode application in applications )
				{
					try
					{
						string tabText = application[TAG_TAB_TEXT].InnerText;

						ShowProgressMessage("タブページ作成中...", tabText);

						if ( IsHideTabPage(tabText, ref hideTabPageText) )
							continue;

						int iTabPage;
						if ( (iTabPage = FindTabPage(tabText/*, ref appTabPages, false*/)) != -1 )	// 既存のタブページ？
						{
							countButtonInTabPage[iTabPage]++;
							if ( countButtonInTabPage[maxButtonTabPage] < countButtonInTabPage[iTabPage] )
							{
								maxButtonTabPage = iTabPage;
							}
							continue;
						}

						iTabPage = countTabPages++;

						AppendNewTabPage(iTabPage, tabText/*application*/, false);	// タブページを作成して追加する

						if ( appTabPages[iTabPage].Height < appTabPages[minSizeTabPage].Height )
						{
							minSizeTabPage = iTabPage;
						}

						if ( (selectedTabPageText != null) && (tabText == selectedTabPageText) )
						{
							this.tabControl.SelectedIndex = iTabPage;
						}

						// countButtonInTabPage のリアロケーション
						Array.Resize(ref countButtonInTabPage, countTabPages);

						countButtonInTabPage[iTabPage] = 1;
					}
					catch ( Exception exp )
					{
						//MessageBox.Show(xmlCcConfiguration.BaseURI + " <application> タブページの作成中に障害が発生しました.\r\n" + application.OuterXml + "\r\nこの環境はスキップします.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						string message = xmlCcConfiguration.BaseURI + " <application> タブページの作成中に障害が発生しました.\r\n" + application.OuterXml + "\r\nプログラムを再起動して下さい.\r\n原因：" + exp.Message;
						throw new Exception(message);
					}
				}

				if ( countTabPages == 0 )
				{
					SetSelectedTabPageText(null);
					return;
				}

				tabPageHasNewApp = new bool[countTabPages];
				tabPageHasHScroll = new bool[countTabPages];

				// ボタンの位置を計算する
				Point[] pointButtonInTabPage, pointButtonInTabPageHScroll = new Point[countButtonInTabPage[maxButtonTabPage]];
				int hscrollButtonCount = ComputeButtonPoint(appTabPages[minSizeTabPage].Name, new Rectangle(appTabPages[minSizeTabPage].Location, appTabPages[minSizeTabPage].Size), countButtonInTabPage[maxButtonTabPage], out pointButtonInTabPage, ref pointButtonInTabPageHScroll);

				for ( int i = 0; i < countTabPages; i++ )
				{
					tabPageHasNewApp[i] = false;
					tabPageHasHScroll[i] = (hscrollButtonCount <= countButtonInTabPage[i]);
					countButtonInTabPage[i] = 0;
				}

				InitProgressBar(0, applications.Count, 1, 0);

				// アプリケーション起動用ボタンを作成して配置する
				foreach ( XmlNode application in applications )
				{
					try
					{
						string buttonText = application[TAG_BUTTON_TEXT].InnerText;

						ShowProgressMessage("ボタン作成中...", buttonText);

						int iTabPage, iButton = countButtons++;

						if ( (iTabPage = FindTabPage(application[TAG_TAB_TEXT].InnerText/*, ref appTabPages, false*/)) == -1 )	// タブページは作成されていない？
						{
							if ( appButtons == null )
								appButtons = new Button[0];
							Array.Resize(ref appButtons, countButtons);
							if ( appIcons == null )
								appIcons = new Icon[0];
							Array.Resize(ref appIcons, countButtons);
							continue;
						}

						Point location = (!tabPageHasHScroll[iTabPage]) ? pointButtonInTabPage[countButtonInTabPage[iTabPage]] : pointButtonInTabPageHScroll[countButtonInTabPage[iTabPage]];
						int tabIndex = countButtonInTabPage[iTabPage];
						Size _sizeButton = buttonSize;
						//Color _buttonTextColor = buttonTextColor;
						bool _transButton = false;
#if ENABLE_TAB_BACKGROUND
						if ( appTabPages[iTabPage].BackgroundImage != null )
						{
							_sizeButton = GetTabButtonSize(appTabPages[iTabPage].Tag/*Name*/);
							//_buttonTextColor = GetTabButtonTextColor(appTabPages[iTabPage].Name);
							_transButton = IsTabTransparentButton(appTabPages[iTabPage].Name);
						}
#endif
						AppendNewButton(iTabPage, iButton, location, tabIndex, application, _sizeButton, /*_buttonTextColor, */_transButton);	// ボタンを作成して追加する

						// 更新されたアプリケーション？
						if ( application.Attributes[ATTRIB_NEW] != null )
						{
							tabPageHasNewApp[iTabPage] = true;
						}

						countButtonInTabPage[iTabPage]++;
					}
					catch ( Exception exp )
					{
						MessageBox.Show(xmlCcConfiguration.BaseURI + " <application> ボタンの作成中に障害が発生しました.\r\n" + application.OuterXml + "\r\nこの環境はスキップします.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}

				for ( int i = 0; i < tabControl.TabCount; i++ )
				{
					if ( tabPageHasNewApp[i] )
					{
						tabControl.TabPages[i].Text += newAppIndicator;
					}
				}

				if ( selectedTabPageText == null || (tabControl.TabPages.IndexOfKey(selectedTabPageText) == -1) )
				{
					SetSelectedTabPageText(GetPlainTabPageText(tabControl.SelectedTab.Text));
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 非表示にするタブかどうか配列から検索する
		/// </summary>
		private bool IsHideTabPage(string tabText, ref string[] hideTabPageText)
		{
			if ( hideTabPageText == null )
				return false;

			int iTabPage;
			for ( iTabPage = 0; (iTabPage < hideTabPageText.Length) && (tabText != hideTabPageText[iTabPage]); iTabPage++ ) { }

			return !(iTabPage == hideTabPageText.Length);
		}

		/// <summary>
		/// appTabPages 配列からタブを検索する
		/// </summary>
		private int FindTabPage(string tabText/*, ref TabPage[] appTabPages, bool plain*/)
		{
#if true
			return tabControl.TabPages.IndexOfKey(tabText);
#else
			int iTabPage;
			int countTabPages = (appTabPages == null) ? 0 : appTabPages.Length;

			if ( plain )
			{
				//for ( iTabPage = 0; (iTabPage < countTabPages) && (GetPlainTabPageText(appTabPages[iTabPage].Text) != tabText); iTabPage++ ) { }
				for ( iTabPage = 0; (iTabPage < countTabPages) && (appTabPages[iTabPage].Name != tabText); iTabPage++ ) { }
			}
			else
			{
				for ( iTabPage = 0; (iTabPage < countTabPages) && (appTabPages[iTabPage].Text != tabText); iTabPage++ ) { }
			}

			return (iTabPage == countTabPages) ? -1 : iTabPage;
#endif
		}

		/// <summary>
		/// 新しいタブページを追加する
		/// </summary>
		/// <param name="iTabPage"></param>
		/// <param name="tabText"></param>
		private void AppendNewTabPage(int iTabPage, string tabText/*XmlNode application*/, bool addThumbnail)
		{
			// appTabPages のリアロケーション
			if ( appTabPages == null )
			{
				appTabPages = new TabPage[0];
			}
			Array.Resize(ref appTabPages, iTabPage + 1);

			//string tabText = application[TAG_TAB_TEXT].InnerText;

			// タブページを作成する
			appTabPages[iTabPage] = new TabPage(tabText);
			appTabPages[iTabPage].Name = /*"tabPage" + */tabText;
			appTabPages[iTabPage].AutoScroll = true;
			appTabPages[iTabPage].BackColor = tabPageBackColor/*SystemColors.ControlLight*/;
			appTabPages[iTabPage].Click += new EventHandler(ccMushroom_Click);
			if ( Program.debMode || movableButton )
			{
				appTabPages[iTabPage].MouseMove += new System.Windows.Forms.MouseEventHandler(this.tabControl_MouseMove);
			}

			this.tabControl.Controls.Add(appTabPages[iTabPage]);

			SetTabPageIcon(iTabPage);

#if ENABLE_TAB_BACKGROUND
			try
			{
				// 有効なタブの背景画像ファイルを取得する
				string tabBackgroundFileName = GetEnableTabBackgroundFileName(appTabPages[iTabPage].Name);
				if ( tabBackgroundFileName != null )
				{
					// タブの背景画像を設定する
					appTabPages[iTabPage].BackgroundImage = new Bitmap(tabBackgroundFileName);
					appTabPages[iTabPage].BackgroundImageLayout = GetTabBackgroundImageLayout(appTabPages[iTabPage].Name);

					// ボタンのサイズ
					Size _buttonSize = GetTabButtonSize(appTabPages[iTabPage].Name);

					// ボタンのテキストの色
					Color _buttonTextColor = GetTabButtonTextColor(appTabPages[iTabPage].Name);

					// タブページの Tag へ object 配列として埋め込む
					object[] tabBtn = new object[(int)TabBtn.count];
					tabBtn[(int)TabBtn.size] = _buttonSize;
					tabBtn[(int)TabBtn.textColor] = _buttonTextColor;
					appTabPages[iTabPage].Tag = tabBtn;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif

#if FOR_WINDOWS7
			try
			{
				if ( addThumbnail )
				{
					AddThumbnailTabPage(appTabPages[iTabPage]);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
#endif
		}

#if ENABLE_TAB_BACKGROUND
		/// <summary>
		/// 有効なタブの背景画像ファイルを取得する
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		private string GetEnableTabBackgroundFileName(string tabName)
		{
			StringBuilder returnedString = new StringBuilder(1024);

			// タブの背景画像が有効か否か
			api.GetPrivateProfileString(tabName, KEY_ENABLED_BACKGROUND, false.ToString(), returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName);
			bool enabledBackground = bool.Parse(returnedString.ToString());
			if ( !enabledBackground )
				return null;

			// 背景画像の指定がない？
			if ( api.GetPrivateProfileString(tabName, KEY_BACKGROUND_IMAGE, "", returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName) == 0 )
				return null;

			string tabBackgroundFileName = returnedString.ToString();
			return File.Exists(tabBackgroundFileName) ? tabBackgroundFileName : null;
		}

		/// <summary>
		/// ボタンのサイズを取得する
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		public static Size GetTabButtonSize(string tabName)
		{
			Size _buttonSize = buttonSize;

			try
			{
				StringBuilder returnedString = new StringBuilder(1024);

				if ( api.GetPrivateProfileString(tabName, KEY_BUTTON_SIZE, "", returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName) != 0 )
				{
					string[] size = returnedString.ToString().Split(',');
					_buttonSize = new Size(Int32.Parse(size[0]), Int32.Parse(size[1]));
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

			return _buttonSize;
		}
		public static Size GetTabButtonSize(object tag)
		{
			if ( tag == null )
				return buttonSize;
			else
				return (Size)((object[])tag)[(int)TabBtn.size];
		}

		/// <summary>
		/// ボタンのテキストの色を取得する
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		public static Color GetTabButtonTextColor(string tabName)
		{
			Color _buttonTextColor = buttonTextColor;

			try
			{
				StringBuilder returnedString = new StringBuilder(1024);

				if ( api.GetPrivateProfileString(tabName, KEY_BUTTON_TEXT_COLOR, "", returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName) != 0 )
				{
					//_buttonTextColor = Color.FromName(returnedString.ToString());
					_buttonTextColor = Color.FromArgb(int.Parse(returnedString.ToString(), System.Globalization.NumberStyles.HexNumber));
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

			return _buttonTextColor;
		}
		public static Color GetTabButtonColor(object tag)
		{
			if ( tag == null )
				return buttonTextColor;
			else
				return (Color)((object[])tag)[(int)TabBtn.textColor];
		}

		/// <summary>
		/// タブの背景画像のイメージレイアウトを取得する
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		private ImageLayout GetTabBackgroundImageLayout(string tabName)
		{
			try
			{
				StringBuilder returnedString = new StringBuilder(1024);

				api.GetPrivateProfileString(tabName, KEY_IMAGE_LAYOUT, ImageLayout.Zoom.ToString(), returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName);

				return ConvertImageLayout(returnedString.ToString());
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
				return ImageLayout.None;
			}
		}

		/// <summary>
		/// イメージレイアウトを文字列から取得する
		/// </summary>
		/// <param name="imageLayout"></param>
		/// <returns></returns>
		private ImageLayout ConvertImageLayout(string imageLayout)
		{
			if ( imageLayout == ImageLayout.Tile.ToString() )
				return ImageLayout.Tile;

			if ( imageLayout == ImageLayout.Center.ToString() )
				return ImageLayout.Center;

			if ( imageLayout == ImageLayout.Stretch.ToString() )
				return ImageLayout.Stretch;

			return ImageLayout.Zoom;
		}

		/// <summary>
		/// ボタンの背景色は透過である？
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		private bool IsTabTransparentButton(string tabName)
		{
			try
			{
				StringBuilder returnedString = new StringBuilder(1024);

				api.GetPrivateProfileString(tabName, KEY_TRANSPARENT_BUTTON, false.ToString(), returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName);

				return bool.Parse(returnedString.ToString());
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
				return false;
			}
		}

		/// <summary>
		/// 保存されたフォーム サイズを取得する
		/// </summary>
		/// <param name="tabName"></param>
		/// <returns></returns>
		private Size GetTabResumeFormSize(string tabName)
		{
			Size tabPageSize = Size.Empty;

			try
			{
				StringBuilder returnedString = new StringBuilder(1024);

				if ( api.GetPrivateProfileString(tabName, KEY_RESUME_FORM_SIZE, "", returnedString, (uint)returnedString.Capacity, tabPageSettingsIniFileName) != 0 )
				{
					string[] size = returnedString.ToString().Split(',');
					tabPageSize = new Size(int.Parse(size[0]), int.Parse(size[1]));
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

			return tabPageSize;
		}

		/// <summary>
		/// 全てのタブの保存されたフォーム サイズを削除する
		/// </summary>
		private void RemoveAllTabResumeFormSize()
		{
#if FOR_WINDOWS7
			try
			{
				if ( !enabledTaskbarThumbnail )
					return;

				byte[] returnedBytes = new byte[0xffff];
				int length = (int)api.GetPrivateProfileSectionNames(returnedBytes, (uint)returnedBytes.Length, tabPageSettingsIniFileName);
				if ( length == 0 )
					return;

				string returnedSection = Encoding.Default.GetString(returnedBytes, 0, length - 1);
				foreach ( string section in returnedSection.Split('\0') )
				{
					api.WritePrivateProfileString(section, KEY_RESUME_FORM_SIZE, null, tabPageSettingsIniFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif
		}
#endif

		/// <summary>
		/// ボタンの位置を計算する
		/// </summary>
		/// <param name="minTabPage"></param>
		/// <param name="maxCountButtonInTabPage"></param>
		/// <param name="pointButtonInTabPage"></param>
		/// <param name="pointButtonInTabPageHScroll"></param>
		/// <returns></returns>
		private int ComputeButtonPoint(string tabName, Rectangle minTabPage, int maxCountButtonInTabPage, out Point[] pointButtonInTabPage, ref Point[] pointButtonInTabPageHScroll)
		{
			const int buttonMargine = 3;
			Point startButton = new Point(5, 5);
			Point curPoint = startButton;
			int hscrollButtonCount = int.MaxValue;

			pointButtonInTabPage = new Point[maxCountButtonInTabPage];

#if ENABLE_TAB_BACKGROUND
			Size buttonSize = GetTabButtonSize(tabName);
#endif

			for ( int i = 0; i < pointButtonInTabPage.Length; i++ )
			{
				// タブページの高さを超える？
				if ( minTabPage.Height < (curPoint.Y + buttonSize.Height + buttonMargine) )
				{
					curPoint.X += (buttonMargine + buttonSize.Width);
					curPoint.Y = startButton.Y;

					if ( hscrollButtonCount == int.MaxValue )
					{
						// タブページの横幅を超える？
						if ( minTabPage.Width < (curPoint.X + buttonSize.Width) )
						{
							hscrollButtonCount = i + 1;		// 横幅を超えるボタンの数
						}
					}
				}

				pointButtonInTabPage[i].X = curPoint.X;
				pointButtonInTabPage[i].Y = curPoint.Y;
				curPoint.Y += (buttonMargine + buttonSize.Height);
			}

			if ( (pointButtonInTabPageHScroll != null) && (hscrollButtonCount != int.MaxValue) )
			{
				Point[] dummy = null;
				ComputeButtonPoint(tabName, new Rectangle(minTabPage.Location, new Size(minTabPage.Size.Width, minTabPage.Size.Height - SystemInformation.HorizontalScrollBarHeight)), maxCountButtonInTabPage, out pointButtonInTabPageHScroll, ref dummy);
			}

			return hscrollButtonCount;
		}

		/// <summary>
		/// 新しいボタンを追加する
		/// </summary>
		/// <param name="iTabPage"></param>
		/// <param name="iButton"></param>
		/// <param name="location"></param>
		/// <param name="tabIndex"></param>
		/// <param name="application"></param>
		private void AppendNewButton(int iTabPage, int iButton, Point location, int tabIndex, XmlNode application, Size _sizeButton, /*Color _buttonTextColor, */bool _transButton)
		{
			string logicStamp = string.Empty;

			try
			{
				// appButtons のリアロケーション
				if ( appButtons == null )
				{
					appButtons = new Button[0];
				}
				Array.Resize(ref appButtons, iButton + 1);
				logicStamp = "appButtons resize done";

				// appIcons のリアロケーション
				if ( appIcons == null )
				{
					appIcons = new Icon[0];
				}
				Array.Resize(ref appIcons, iButton + 1);
				logicStamp = "appIcons resize done";

				// ボタンを作成する
				appButtons[iButton] = new Button();
				appButtons[iButton].Paint += new PaintEventHandler(this.appButton_Paint);
				//appButtons[iButton].BackColor = buttonBackColor/*Color.White/*System.Drawing.SystemColors.ControlLightLight*/;
				appButtons[iButton].FlatStyle = FlatStyle.Flat/*FlatStyle.System*/;
				appButtons[iButton].Font = buttonFont;
				appButtons[iButton].ContextMenuStrip = this.contextMenuButton;
				appButtons[iButton].Location = location;
				appButtons[iButton].TabIndex = tabIndex;	// TabIndex をタブページ内の順番として使用する
				appButtons[iButton].Name = "button" + application[TAG_BUTTON_TEXT].InnerText;
				appButtons[iButton].Size = _sizeButton/*buttonSize*/;
				//appButtons[iButton].ForeColor = _buttonTextColor;
				appButtons[iButton].Tag = iButton;			// Tag を xmlCcConfiguration の ChildNodes インデックスとして使用する
				appButtons[iButton].Click += new System.EventHandler(this.appButton_Click);
#if MOVABLE_BUTTON_BY_DRAGDROP
				appButtons[iButton].MouseDown += new System.Windows.Forms.MouseEventHandler(this.appButton_MouseDown);
				//if ( Program.debMode )
				//{
				appButtons[iButton].MouseMove += new System.Windows.Forms.MouseEventHandler(this.appButton_MouseMove);
				//}
				//appButtons[iButton].MouseUp += new System.Windows.Forms.MouseEventHandler(this.appButton_MouseUp);
#endif
#if (_DEBUG)
				appButtons[iButton].Text = application[TAG_BUTTON_TEXT].InnerText;
				appButtons[iButton].ForeColor = Color.LightGray;
#endif
				logicStamp = "appButtons[iButton] done";

				if ( _transButton )
				{
					appButtons[iButton].BackColor = Color.Transparent;
				}
				else
				{
					SetEachButtonBackColor(iButton, application);
				}
				logicStamp = "_transButton done";

				// アイコンを設定する
				string appIconFile = (application[TAG_ICON_FILE] == null) ? application[TAG_APP_NAME].InnerText : application[TAG_ICON_FILE].InnerText;
				appIcons[iButton] = ccf.GetIcon(appIconFile, smallApplicationIcon);
				logicStamp = "appIcons[iButton] done";

				// ボタンをタブページに追加する
				appTabPages[iTabPage].Controls.Add(appButtons[iButton]);
				logicStamp = "appTabPages[iTabPage] done";

				// ToolTip を設定する
				string appToolTip = MakeButtonToolTip(application, iButton);
				toolTip.SetToolTip(appButtons[iButton], appToolTip);
				logicStamp = "SetToolTip done";

#if FOR_WINDOWS7
				StringBuilder returnedString = new StringBuilder(256);
				if ( api.GetPrivateProfileString("RunAs", application[TAG_BUTTON_TEXT].InnerText, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName) != 0 )
				{
					runAsButtons.Add(application[TAG_BUTTON_TEXT].InnerText);
				}
#endif
			}
			catch ( Exception exp )
			{
				//throw exp;
				string message = exp.Message + "\r\n" +
								 "appTabPages:" + appTabPages.Length + " " + "iTabPage:" + iTabPage + "\r\n" +
								 "appButtons:" + appButtons.Length + " " + "appIcons:" + appIcons.Length + " " + "iButton:" + iButton + "\r\n" +
								 logicStamp + "\r\n";
				throw new Exception(message);
			}
		}

		/// <summary>
		/// ボタン背景色の指定があれば設定する
		/// </summary>
		/// <param name="iButton"></param>
		/// <param name="application"></param>
		private void SetEachButtonBackColor(int iButton, XmlNode application)
		{
			try
			{
				if ( application[TAG_BUTTON_TEXT].Attributes[ATTRIB_BACK_COLOR] == null )
				{
					appButtons[iButton].BackColor = buttonBackColor;
					return;
				}

				// インポートされたアプリか、リモート側の色設定を無視しない？
				if ( (application.Attributes[ATTRIB_IMPORTED] != null) || !ignoreRemoteButtonBackColor )
				{
					appButtons[iButton].BackColor = Color.FromName(application[TAG_BUTTON_TEXT].Attributes[ATTRIB_BACK_COLOR].Value);
				}
			}
			catch ( Exception exp )
			{
				if ( Program.debMode )
				{
					MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
		}

		/// <summary>
		/// ボタンのツールチップを作る
		/// </summary>
		/// <param name="application"></param>
		/// <returns></returns>
		private string MakeButtonToolTip(XmlNode application, int iButton)
		{
			try
			{
				StringBuilder appToolTip = new StringBuilder(application[TAG_BUTTON_TEXT].InnerText + ((application.Attributes[ATTRIB_NEW] != null) ? newAppIndicator : ""));

				string appFullPath = application[TAG_APP_NAME].InnerText;
#if true
				if ( appFullPath.StartsWith("http://") )
				{
					appFullPath = System.Web.HttpUtility.UrlDecode(appFullPath);
				}
#endif

				appToolTip.Append("\r\n" + toolTipNames[(int)tt.appName] + appFullPath);

				if ( application[TAG_COMMAND_LINE] != null )
				{
					appToolTip.Append("\r\n" + toolTipNames[(int)tt.commandLine] + application[TAG_COMMAND_LINE].InnerText);
				}

				if ( application[TAG_WORKING_DIRECTORY] != null )
				{
					appToolTip.Append("\r\n" + toolTipNames[(int)tt.workingDirectory] + application[TAG_WORKING_DIRECTORY].InnerText);
				}

				if ( application[TAG_COMMENT] != null )
				{
					appToolTip.Append("\r\n" + toolTipNames[(int)tt.comment] + application[TAG_COMMENT].InnerText);
				}

				//if ( Program.onlineEnable || (!Program.onlineEnable && !appFullPath.StartsWith(@"\\")) )
				if ( !appFullPath.StartsWith(@"\\") || (appFullPath.StartsWith(@"\\") && (Program.onlineEnable && !ccMushroom.ignoreRemoteInfo)) )
				{
					if ( File.Exists(appFullPath) )
					{
						string fileVersion = FileVersionInfo.GetVersionInfo(appFullPath).FileVersion;
						if ( !string.IsNullOrEmpty(fileVersion) )
						{
							appToolTip.Append("\r\n" + toolTipNames[(int)tt.version] + fileVersion);
						}
						appToolTip.Append("\r\n" + toolTipNames[(int)tt.lastWriteTime] + File.GetLastWriteTime(appFullPath));
					}
				}
				else
				{
					Debug.WriteLine(appFullPath);
				}

				if ( application[TAG_APP_ENVIRON] != null )
				{
					appToolTip.Append("\r\n" + toolTipNames[(int)tt.appEnviron] + application[TAG_APP_ENVIRON].InnerText);
				}

				if ( Program.debMode )
				{
					appToolTip.Append("\r\n" + toolTipNames[(int)tt.tabIndex] + appButtons[iButton].TabIndex);
					appToolTip.Append("\r\n" + toolTipNames[(int)tt.tag] + appButtons[iButton].Tag);
				}

				return appToolTip.ToString();
			}
			catch ( Exception exp )
			{
				return exp.Message;
			}
		}

		/// <summary>
		/// toolTip から実際のボタン テキストを抜き出す
		/// </summary>
		/// <param name="appToolTip"></param>
		/// <param name="newApp"></param>
		/// <param name="allNeed"></param>
		/// <returns></returns>
		public string GetButtonTextFromToolTip(string appToolTip, out bool newApp, bool allNeed)
		{
			newApp = false;
			int index;

			string buttonText = appToolTip;

			if ( (index = buttonText.IndexOf("\r\n")) != -1 )
			{
				buttonText = buttonText.Substring(0, index);
			}

			if ( buttonText.EndsWith(newAppIndicator) )
			{
				buttonText = buttonText.Substring(0, buttonText.Length - 1);
				newApp = true;
			}

			if ( !allNeed )
				return buttonText;

			string[] appToolTips = appToolTip.Split('\r');
			string[] eachToolTips = new string[(int)tt.count];

			eachToolTips[(int)tt.buttonText] = buttonText;

			for ( int i = 0; i < appToolTips.Length; i++ )
			{
				for ( int j = 1; j < toolTipNames.Length; j++ )
				{
					if ( appToolTips[i].StartsWith("\n" + toolTipNames[j]) )
					{
						eachToolTips[j] = appToolTips[i].Substring(1 + toolTipNames[j].Length);
						break;
					}
				}
			}

			return string.Join("\t", eachToolTips);
		}
		#endregion

		#region フォームの半透明処理
		/// <summary>
		/// 自動的にフォームを透過するタイマを開始する
		/// </summary>
		private void StartTimerAutoOpacityForm()
		{
			try
			{
				if ( autoOpacityTimePercent[0] == '0' )
					return;

				int interval = int.Parse(autoOpacityTimePercent.Split(',')[0]) * 1000;
				if ( 1000 <= interval )
				{
					if ( timerOpacity.Enabled )
					{
						timerOpacity.Stop();
					}

					timerOpacity.Interval = interval;
					timerOpacity.Start();
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// フォームを半透明にする
		/// </summary>
		private void SetOpaqueForm(bool clickThrough)
		{
			try
			{
				timerOpacity.Stop();

				int percent = int.Parse(autoOpacityTimePercent.Split(',')[1]);
				this.Opacity = (double)(percent) / 100.0;

				if ( (this.Opacity != 1.0) && clickThrough )
				{
					SetWsExTransparent(true);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 半透明のフォームをリセットする
		/// </summary>
		private void ResetOpaqueForm()
		{
			try
			{
				if ( this.Opacity != 1.0 )
				{
					DisTransparentWithClickThrough(false);

					this.Opacity = 1.0;
				}

				StartTimerAutoOpacityForm();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// Click-Through をトグルする
		/// </summary>
		private void ToggleClickThrough()
		{
			try
			{
				/*if ( !transparentWithClickThrough )
					return;*/

				if ( SetWsExTransparent(null) )	// 現在 Click-Through 状態？
				{
					SetWsExTransparent(false);
				}
				else
				{
					if ( autoOpacityTimePercent[0] != '0' )
					{
						SetOpaqueForm(true);
					}
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// マウスのクリック スルーを解除する
		/// </summary>
		/// <param name="resetOpacityTimer"></param>
		private void DisTransparentWithClickThrough(bool resetOpacityTimer)
		{
			try
			{
				SetWsExTransparent(false);

				if ( !transparentWithClickThrough )
					return;

				if ( resetOpacityTimer )
				{
					StartTimerAutoOpacityForm();
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// WS_EX_TRANSPARENT をセット|解除する
		/// </summary>
		/// <param name="set"></param>
		/// <returns></returns>
		private bool SetWsExTransparent(bool? set)
		{
			int gwlExStyle = api.GetWindowLong32(this.Handle, api.GWL_EXSTYLE);

			try
			{
				if ( set != null )
				{
					if ( (bool)set )
					{
						// WS_EX_LAYERED(Opacity != 1.0 の時) と WS_EX_TRANSPARENT の組み合わせでマウスイベントが無視されるようになる？
						gwlExStyle |= api.WS_EX_TRANSPARENT;
						this.Icon.Dispose();
						this.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ccMushroom.images.Click-through.ico"));
					}
					else
					{
						//gwlExStyle ^= api.WS_EX_TRANSPARENT;
						gwlExStyle &= (~api.WS_EX_TRANSPARENT);
						this.Icon.Dispose();
						this.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ccMushroom.App.ico"));
					}
					api.SetWindowLong(this.Handle, api.GWL_EXSTYLE, (uint)gwlExStyle);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

			return ((gwlExStyle & api.WS_EX_TRANSPARENT) != 0);
		}
		#endregion

		#region ホットキー関連
		/// <summary>
		/// ホットキーを登録する
		/// </summary>
		private void RegisterHotKey(string hotKey)
		{
			try
			{
				ccf.hotKeys hotKeys;

				if ( ccf.ParseHotKey(hotKey, out hotKeys) )
				{
					api.RegisterHotKey(this.Handle, HOTKEY_SHOW_CONTEXT_MENU, hotKeys.GetModifiers(), hotKeys.keyCode);
				}

				if ( toggleClickThroughHotKey != null )
				{
					if ( ccf.ParseHotKey(toggleClickThroughHotKey, out hotKeys) )
					{
						api.RegisterHotKey(this.Handle, HOTKEY_TOGGLE_CLICK_THROUGH, hotKeys.GetModifiers(), hotKeys.keyCode);
					}
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ホットキーの登録を解除する
		/// </summary>
		private void UnregisterHotKey()
		{
			api.UnregisterHotKey(this.Handle, HOTKEY_SHOW_CONTEXT_MENU);

			api.UnregisterHotKey(this.Handle, HOTKEY_TOGGLE_CLICK_THROUGH);
		}
		#endregion

		#region appButton イベント
		/// <summary>
		/// アプリケーション ボタンを Paint イベントで描画する
		/// </summary>
		private void appButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			try
			{
				Control control = (Control)sender;
				int iButton = (int)control.Tag;

				Icon appIcon = appIcons[iButton];
				bool newApp;
				string buttonText = GetButtonTextFromToolTip(toolTip.GetToolTip(control), out newApp, false);
				Icon newAppIcon = newApp ? this.newAppIcon : null;
#if ENABLE_TAB_BACKGROUND
				Color _buttonTextColor = (tabControl.SelectedTab.Tag == null) ? buttonTextColor : (Color)((object[])tabControl.SelectedTab.Tag)[(int)TabBtn.textColor];
#else
				Color _buttonTextColor = buttonTextColor;
#endif

				bool dragButton = false;
#if MOVABLE_BUTTON_BY_DRAGDROP
				dragButton = (iButton == dragButtonTag);
#endif

				ccf.PaintButton(control, e.Graphics, smallApplicationIcon, appIcon, 0, false, newAppIcon, buttonText, 0.0F, buttonFont, _buttonTextColor/*appButtons[iButton].ForeColor/*buttonTextColor*/, buttonBackColor, dragButton);

#if FOR_WINDOWS7
				try
				{
					// 管理者として実行？
					if ( runAsButtons.IndexOf(buttonText) != -1 )
					{
						int iconSize = (smallApplicationIcon) ? 16 : 32;
						int iconX = (smallApplicationIcon) ? 6 : 4;
						Icon runAsIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("ccMushroom.images.runAs.ico"), smallApplicationIcon ? 8 : 16, smallApplicationIcon ? 8 : 16);
						int runAsIconX = iconX;
						int runAsIconY = ((control.Size.Height - iconSize) / 2 + 0) + (iconSize - runAsIcon.Height);
						e.Graphics.DrawIcon(runAsIcon, runAsIconX, runAsIconY);
						runAsIcon.Dispose();
						runAsIcon = null;
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// [アプリケーション ボタン] がクリックされた
		/// </summary>
		private void appButton_Click(object sender, System.EventArgs e)
		{
			try
			{
				this.TopMost = false;

				Button appButton = (Button)sender;

#if MOVABLE_BUTTON_BY_DRAGDROP
				Point mdownPoint = new Point(dragButtonPoint.Width, dragButtonPoint.Height);
				if ( appButton.Location != mdownPoint )	// ボタンが移動された？
					return;
#endif

				//ResetFormOpacity();

				AppButtonClicked((int)appButton.Tag);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

#if MOVABLE_BUTTON_BY_DRAGDROP
		/// <summary>
		/// [アプリケーション ボタン] ドラッグの開始 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void appButton_MouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				if ( !movableButton )
					return;
				if ( e.Button != MouseButtons.Left )
					return;

				Button appButton = (Button)sender;
				dragButtonPoint = new Rectangle(e.X, e.Y, appButton.Location.X, appButton.Location.Y);
				Debug.WriteLine("MouseDown " + dragButtonPoint);

				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)appButton.Tag];
				bool imported = (application.Attributes[ATTRIB_IMPORTED] != null);
				if ( !imported )
				{
					dragButtonPoint.X = dragButtonPoint.Y = -1;
					return;
				}

				// 移動開始位置の記録
				//dragButtonPoint = new Rectangle(e.X, e.Y, appButton.Location.X, appButton.Location.Y);
				//StartRect.Location = appButton.PointToScreen(new Point(e.X, e.Y));
				//StartRect.Location = this.tabControl.SelectedTab.PointToClient(StartRect.Location);

				appButton.BringToFront();
				/*if ( !Program.debMode )
				{
					appButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.appButton_MouseMove);
				}*/
				appButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.appButton_MouseUp);
				appButton.ContextMenuStrip = null;
				toolTip.Active = false;
				tabControl.ContextMenuStrip = null;

				if ( (0 <= dragButtonTag) && (dragButtonTag < dragButtonTagWait) )
				{
					appButtons[dragButtonTag].ContextMenuStrip = this.contextMenuButton;
					appButtons[dragButtonTag].Invalidate();
				}

				dragButtonTag = (int)appButton.Tag + dragButtonTagWait;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// [アプリケーション ボタン] ドラッグ中の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void appButton_MouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if ( !movableButton )
					return;

				if ( Program.debMode )
				{
					Point cursorPosition = tabControl.SelectedTab.PointToClient(Cursor.Position);
					toolStripMousePosition.Text = cursorPosition.X + "," + cursorPosition.Y;
				}

				if ( dragButtonPoint.X == -1 )	// インポートされたボタンではない？
					return;
				if ( dragButtonTag == -1 )		// 右クリックでキャンセルされた後のゴミ イベント？
					return;
				if ( e.Button != MouseButtons.Left )
					return;

				Button appButton = (Button)sender;

				if ( dragButtonTagWait <= dragButtonTag )	// 重み付きのタグ？
				{
					if ( (e.X != dragButtonPoint.X) || (e.Y != dragButtonPoint.Y) )	// 最初の移動？
					{
						tabControl.Select();
						dragButtonTag -= dragButtonTagWait;	// 重みを取る
						appButtons[dragButtonTag].Invalidate();
						appButtons[dragButtonTag].Cursor = dragCursor;
						appButtons[dragButtonTag].FlatStyle = FlatStyle.Popup;

						if ( hideTabImageDuringButtonDrag )
						{
							if ( tabControl.SelectedTab.BackgroundImage != null )
							{
								tabControl.SelectedTab.BackgroundImage.Dispose();
								tabControl.SelectedTab.BackgroundImage = null;
							}
						}
					}
				}
				else
				{
					if ( (int)appButton.Tag == dragButtonTag )
					{
						if ( appButtons[dragButtonTag].Cursor == Cursors.Default )
							appButtons[dragButtonTag].Cursor = dragCursor;
					}
					else
					{
						return;
					}
				}

				// 移動後予測表示位置の描画
				//this.Refresh();
				//Point point = new Point(e.X, e.Y);
				//point = appButton.PointToScreen(new Point(e.X, e.Y));
				//point = this.tabControl.SelectedTab.PointToClient(point);
				//Debug.WriteLine(e.X + " " + e.Y );
				//Pen BorderPen = new Pen(Color.Gray);
				//System.Drawing.Graphics Graphics;
				Rectangle rect = new Rectangle();
				rect.X = appButton.Left + (e.X - dragButtonPoint.X);
				rect.Y = appButton.Top + (e.Y - dragButtonPoint.Y);
				rect.Width = appButton.Width;
				rect.Height = appButton.Height;
				//Graphics = this.CreateGraphics();
				//Graphics.DrawRectangle(BorderPen, rect);
				//BorderPen.Dispose();
				//Graphics.Dispose();

				TabPage selectedTab = tabControl.SelectedTab;

				if ( (rect.X < 0) || (rect.Y < 0) ||
					 (selectedTab.Width < rect.X + (rect.Width * .4F)) || (selectedTab.Height < rect.Y + rect.Height) )	// タブページの範囲外？
					return;

				appButton.Location = rect.Location;
				Debug.WriteLine("MouseMove " + appButton.Location);
				//selectedTab.Refresh();

				/*Point point = appButton.Location;
				Graphics g = selectedTab.CreateGraphics();
				g.DrawRectangle(Pens.DarkBlue , new Rectangle(point.X - 2, point.Y - 2, 15, 15));
				g.Dispose();
				selectedTab.Refresh();*/
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
#if (DEBUG)
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
			}
		}

		/// <summary>
		/// [アプリケーション ボタン] ドロップ後の処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void appButton_MouseUp(object sender, MouseEventArgs e)
		{
			bool appButtonClicked = false;

			try
			{
				Button appButton = (Button)sender;

				appButton.Cursor = Cursors.Default;
				/*if ( !Program.debMode )
				{
					appButton.MouseMove -= this.appButton_MouseMove;
				}*/
				appButton.FlatStyle = FlatStyle.Flat;
				appButton.MouseUp -= this.appButton_MouseUp;
				appButton.ContextMenuStrip = this.contextMenuButton;
				toolTip.Active = true;
				tabControl.ContextMenuStrip = this.contextMenuTab;

				if ( hideTabImageDuringButtonDrag )
				{
#if ENABLE_TAB_BACKGROUND
					if ( tabControl.SelectedTab.BackgroundImage == null )
					{
						string tabBackgroundFileName = GetEnableTabBackgroundFileName(tabControl.SelectedTab.Name);
						if ( tabBackgroundFileName != null )
						{
							// タブの背景画像を設定する
							tabControl.SelectedTab.BackgroundImage = new Bitmap(tabBackgroundFileName);
						}
					}
#endif
				}

				Point mdownPoint = new Point(dragButtonPoint.Width, dragButtonPoint.Height);

				if ( e.Button != MouseButtons.Left )
				{
					//appButton.Location = mdownPoint;
					Debug.WriteLine("MouseUP " + mdownPoint + " !MouseButtons.Left");
					return;
				}
				Debug.WriteLine("MouseUP {X=" + e.X + ",Y=" + e.Y + "}");

				if ( appButtonClicked = (mdownPoint == appButton.Location) )	// ボタンがクリックされた？
				{
					Debug.WriteLine("MouseUP appButtonClicked");
					return;
				}

				//Point point = appButton.PointToScreen(new Point(e.X, e.Y));
				//point = this.tabControl.SelectedTab.PointToClient(point);
				//appButton.Left = appButton.Left + (point.X - StartRect.X);
				//appButton.Top = appButton.Top + (point.Y - StartRect.Y);
				//this.Refresh();
				//this.tabControl.SelectedTab.Refresh();

				// ドロップされたボタンの位置に移動する
				TabPage selectedTab = tabControl.SelectedTab;
#if true
				Button destButton = GetButtonContains(appButton.Location, appButton.Tag);
				if ( destButton == null )
				{
					Debug.WriteLine("MouseUP !dropOnButton");
					return;
				}
#else
				Button destButton = null;
				bool dropOnButton = false;

				foreach ( Control control in selectedTab.Controls )
				{
					if ( !(control is Button) )
						continue;

					destButton = (Button)control;

					//if ( (destButton.Location.X <= appButton.Location.X) && (destButton.Location.Y <= appButton.Location.Y) &&
					//	 (appButton.Location.X <= destButton.Location.X + destButton.Width) && (appButton.Location.Y <= destButton.Location.Y + destButton.Height) )
					if ( (appButton.Location.X < destButton.Location.X) || (appButton.Location.Y < destButton.Location.Y) ||
						 (destButton.Location.X + destButton.Width < appButton.Location.X) || (destButton.Location.Y + destButton.Height < appButton.Location.Y) )
						continue;

					if ( destButton.Tag == appButton.Tag )
						continue;

					dropOnButton = true;
					break;
				}

				if ( !dropOnButton )
				{
					Debug.WriteLine("MouseUP !dropOnButton");
					return;
				}
#endif

				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)destButton.Tag];
				bool imported = (application.Attributes[ATTRIB_IMPORTED] != null);
				if ( !imported )
					return;

				Icon appIcon = appIcons[(int)appButton.Tag];

				bool newApp;
				string sorcButtonText = GetButtonTextFromToolTip(toolTip.GetToolTip(appButton), out newApp, false);
				string destButtonText = GetButtonTextFromToolTip(toolTip.GetToolTip(destButton), out newApp, false);

				uint sorcButtonOrder = ccf.MAKELONG((ushort)(int)appButton.Tag, (ushort)appButton.TabIndex);
				uint destButtonOrder = ccf.MAKELONG((ushort)(int)destButton.Tag, (ushort)destButton.TabIndex);

				int move = destButton.TabIndex - appButton.TabIndex;

				if ( move < 0 )			// 前に移動？
				{
					//if ( (appButton.TabIndex - Math.Abs((int)move) <= _appButton.TabIndex) && (_appButton.TabIndex < appButton.TabIndex) )
					//	_appButton.TabIndex++;
					int shiftStart = (int)destButton.Tag;
					int shiftEnd = (int)appButton.Tag - 1;
					//appButton.TabIndex = destButton.TabIndex;
					//appButton.Tag = destButton.Tag;
					//int destTabIndex = destButton.TabIndex;

					// ドロップ先のボタンからドラッグ元の一つ前までのボタンを後ろにずらす
					for ( int i = shiftEnd; shiftStart <= i; i-- )
					{
						int j = i + 1;
						appButtons[j] = appButtons[i];
						appButtons[j].TabIndex += (selectedTab.Controls[appButtons[j].Name] != null ? 1 : 0);
						appButtons[j].Tag = (int)j;
						appIcons[j] = appIcons[i];
					}

					/*// ドロップ先にドロップ元のボタンを移動する
					appButtons[shiftStart] = appButton;
					appButtons[shiftStart].TabIndex = destTabIndex;
					appButtons[shiftStart].Tag = (int)shiftStart;
					appIcons[shiftStart] = appIcon;*/
				}
				else if ( 0 < move )	// 後ろに移動？
				{
					//if ( (appButton.TabIndex < _appButton.TabIndex) && (_appButton.TabIndex <= appButton.TabIndex + move) )
					//	_appButton.TabIndex--;
					int shiftStart = (int)appButton.Tag + 1;
					int shiftEnd = (int)destButton.Tag;
					//appButton.TabIndex = destButton.TabIndex;
					//appButton.Tag = destButton.Tag;
					//int destTabIndex = destButton.TabIndex;

					// ドラッグ元の一つ後ろからドロップ先までのボタンを前にずらす
					for ( int i = shiftStart; i <= shiftEnd; i++ )
					{
						int j = i - 1;
						appButtons[j] = appButtons[i];
						appButtons[j].TabIndex -= (selectedTab.Controls[appButtons[j].Name] != null ? 1 : 0);
						appButtons[j].Tag = (int)j;
						appIcons[j] = appIcons[i];
					}

					/*// ドロップ先にドロップ元のボタンを移動する
					appButtons[shiftEnd] = appButton;
					appButtons[shiftEnd].TabIndex = destTabIndex;
					appButtons[shiftEnd].Tag = (int)shiftEnd;
					appIcons[shiftEnd] = appIcon;*/
				}

				// ドロップ先にドロップ元のボタンを移動する
				int destButtonTag = ccf.LOWORD(destButtonOrder);
				appButtons[destButtonTag] = appButton;
				appButtons[destButtonTag].TabIndex = ccf.HIWORD(destButtonOrder);
				appButtons[destButtonTag].Tag = destButtonTag;
				appIcons[destButtonTag] = appIcon;

				appButtons[destButtonTag].Invalidate();

				int sorcButtonTag = ccf.LOWORD(sorcButtonOrder);

				// インポート用 ccMushroom 構成ファイルの <application> ノードを移動する
				string fileCcConfigurationImport = Application.StartupPath + "\\" + CC_CONFIGURATION_IMPORT_FILE_NAME;
				XmlDocument xmlCcConfigurationImport = new XmlDocument();
				xmlCcConfigurationImport.Load(fileCcConfigurationImport);

				string xpath = ccMushroom.TAG_APPLICATION + "[" + TAG_BUTTON_TEXT + "='" + destButtonText + "']";
				XmlNode applicationImport = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);

				xpath = ccMushroom.TAG_APPLICATION + "[" + TAG_BUTTON_TEXT + "='" + sorcButtonText + "']";
				XmlNode sorcApplication = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);

				if ( move < 0 )
					xmlCcConfigurationImport.DocumentElement.InsertBefore(sorcApplication, applicationImport);
				else if ( 0 < move )
					xmlCcConfigurationImport.DocumentElement.InsertAfter(sorcApplication, applicationImport);
				xmlCcConfigurationImport.Save(fileCcConfigurationImport);	// 保存する

				// ccMshroom 構成ファイルの <application> ノードを移動する（一応保存しておく）
				sorcApplication = xmlCcConfiguration.DocumentElement.ChildNodes[sorcButtonTag];
				if ( move < 0 )
					xmlCcConfiguration.DocumentElement.InsertBefore(sorcApplication, application);
				else if ( 0 < move )
					xmlCcConfiguration.DocumentElement.InsertAfter(sorcApplication, application);
				xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);

				SetJumpList();
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally 
			{
				Debug.WriteLine("MouseUP dragButtonTag:" + dragButtonTag);
				if ( (0 <= dragButtonTag) && (dragButtonTag < dragButtonTagWait) )
				{
					appButtons[dragButtonTag].Invalidate();
					dragButtonTag = -1;
				}

				if ( !appButtonClicked )
				{
					RefreshSelectedTabPage(true);
				}
			}
		}
#endif
		#endregion

		#region timerAutoWindowClose 関連
		/// <summary>
		/// 自動ウィンドウ クローズ用のタイマを開始する
		/// </summary>
		private void StartTimerAutoWindowClose()
		{
			try
			{
				if ( !scanRemoteProgramsFolder || !Program.onlineEnable )	// スキャンしない、又はオフライン？
					return;
				if ( autoWindowCloseTime == 0 )
					return;

				System.Threading.TimerCallback timerDelegate = new System.Threading.TimerCallback(OnTimerAutoWindowClose);
				timerAutoWindowClose = new System.Threading.Timer(timerDelegate, null, System.Threading.Timeout.Infinite, 0);
				timerAutoWindowClose.Change(autoWindowCloseTime, System.Threading.Timeout.Infinite);	// SetTimer（周期的なシグナル通知は無効）
			}
			catch ( Exception exp )
			{
				MessageBox.Show("自動ウィンドウ クローズ用のタイマが設定できませんでした.\r\n原因：" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		/// <summary>
		/// OnTimerAutoWindowClose
		/// </summary>
		private void OnTimerAutoWindowClose(object obj)
		{
			try
			{
				//this.Close();
				this.Invoke(((MethodInvoker)delegate() { this.Close(); }));
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// 自動ウィンドウ クローズ用のタイマを終了する
		/// </summary>
		private void StopAutoWindowCloseTimer()
		{
			try
			{
				if ( timerAutoWindowClose == null )
					return;

				//timerAutoWindowClose.Change(Timeout.Infinite, 0);	// KillTimer
				timerAutoWindowClose.Dispose();
				timerAutoWindowClose = null;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region timerOpacity イベント
		/// <summary>
		/// timerOpacity_Tick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerOpacity_Tick(object sender, EventArgs e)
		{
			SetOpaqueForm(transparentWithClickThrough);
		}
		#endregion

		#region tabControl イベント
		/// <summary>
		/// tabControl_MouseMove
		/// </summary>
		private void tabControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( Program.debMode )
			{
				toolStripMousePosition.Text = e.X + "," + e.Y;
			}

#if MOVABLE_BUTTON_BY_DRAGDROP
			if ( movableButton )
			{
				try
				{
					if ( (0 <= dragButtonTag) && (dragButtonTag < dragButtonTagWait) )
					{
						if ( appButtons[dragButtonTag].Cursor != Cursors.Default )
						{
							appButtons[dragButtonTag].Cursor = Cursors.Default;
							appButtons[dragButtonTag].MouseUp -= this.appButton_MouseUp;
						}
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
				}
			}
#endif
		}

		/// <summary>
		/// tabControl_Selecting
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
		{
#if FOR_WINDOWS7
			if ( enabledTaskbarThumbnail )
			{
				// Before selecting,
				// If there is a selected tab, take it's screenshot
				// invalidate the tab's thumbnail
				// update the "preview" object with the new thumbnail
				if ( tabControl.TabPages.Count > 0 && tabControl.SelectedTab != null )
				{
					Debug.WriteLine(MethodBase.GetCurrentMethod().Name + ":" + (previousSelectedPage == null ? "null" : previousSelectedPage.Text));
					UpdatePreviewBitmap(previousSelectedPage);
				}
			}

			// update our selected tab
			previousSelectedPage = tabControl.SelectedTab;
#endif
		}

		/// <summary>
		/// タブページの選択が変更された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				if ( (tabControl.SelectedTab == null) || (appButtons == null) )
				{
					//SetSelectedTabPageText("none");
				}
				else
				{
					SetSelectedTabPageText(GetPlainTabPageText(tabControl.SelectedTab.Text));

#if ENABLE_TAB_BACKGROUND
					Size resumeFormSize = GetTabResumeFormSize(tabControl.SelectedTab.Name);
					this.Size = ((resumeFormSize != Size.Empty) && (tabControl.SelectedTab.BackgroundImage != null)) ? resumeFormSize : windowRectangle.Size;
#endif

					SetJumpList();

#if FOR_WINDOWS7
					if ( enabledTaskbarThumbnail )
					{
						// Make sure we let the Taskbar know about the active/selected tab
						// Tabbed thumbnails need to be updated to indicate which one is currently selected
						if ( tabControl.TabPages.Count > 0 && tabControl.SelectedTab != null )
						{
							TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(tabControl.SelectedTab);
						}
					}
#endif
				}

				StartTimerAutoOpacityForm();	// 半透明になるタイマを再スタートする
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}
		#endregion

		#region toolStripComboAppEnviron イベント
		/// <summary>
		/// アプリケーションの環境が変更された
		/// </summary>
		private void toolStripComboAppEnviron_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ( nowLoading )
				return;

			try
			{
				Cursor.Current = Cursors.WaitCursor;

				string newAppEnviron = toolStripComboAppEnviron.SelectedItem.ToString();
				if ( !ReadCcAppEnvironmentSetting(newAppEnviron) )
					return;

				AdjustComboAppEnviron(newAppEnviron);

				// ボタンの toolTip を変更後の <appEnviron> で書き換える
				XmlNodeList listApplication = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION);

				for ( int i = 0; i < listApplication.Count; i++ )
				{
					if ( appButtons[i] == null )
						continue;

					XmlNode application = listApplication[i];
#if true
					/*if ( application[TAG_APP_ENVIRON] != null )
					{
						XmlAttribute changed = application[TAG_APP_ENVIRON].Attributes[ATTRIB_CHANGED];
						if ( changed != null )
						{
							application[TAG_APP_ENVIRON].Attributes.Remove(changed);
						}
					}*/

					string appToolTip = MakeButtonToolTip(application, i);
					toolTip.SetToolTip(appButtons[i], appToolTip.ToString());
#else
					bool newApp;
					string[] appToolTips = GetButtonTextFromToolTip(toolTip.GetToolTip(appButtons[i]), out newApp, true).Split('\t');

					if ( application[TAG_APP_ENVIRON] == null )
					{
						appToolTips[(int)tt.appEnviron] = "";
					}
					else
					{
						XmlAttribute changed = application[TAG_APP_ENVIRON].Attributes[ATTRIB_CHANGED];
						if ( changed != null )
						{
							string ccAppEnviron = application[TAG_APP_ENVIRON].InnerText;
							int iSpace = ccAppEnviron.IndexOf(" ");
							appToolTips[(int)tt.appEnviron] = newAppEnviron + ccAppEnviron.Substring(iSpace);

							application[TAG_APP_ENVIRON].Attributes.Remove(changed);
						}
					}

					StringBuilder appToolTip = new StringBuilder(appToolTips[(int)tt.buttonText] + (newApp ? newAppIndicator : ""));
					for ( int j = 1; j < appToolTips.Length; j++ )
					{
						if ( appToolTips[j].Length == 0 )
							continue;
						appToolTip.Append("\r\n" + toolTipNames[j] + appToolTips[j]);
					}
					toolTip.SetToolTip(appButtons[i], appToolTip.ToString());
#endif
				}

				// ccMushroom 構成ファイルを保存する
				xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			finally
			{
				tabControl.Select();
				Cursor.Current = Cursors.Default;
			}
		}
		#endregion

		#region toolStripScanned イベント
		/// <summary>
		/// toolStripScanned_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripScanned_Click(object sender, EventArgs e)
		{
			//RefreshSelectedTabPage(true);
		}

		/// <summary>
		/// toolStripScanned_DoubleClick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripScanned_DoubleClick(object sender, EventArgs e)
		{
			//if ( !scanRemoteProgramsFolder || !Program.onlineEnable )
			//	return;

            toolStripScanned.Enabled = false;

			RefreshCcMushroomForm();

            toolStripScanned.Enabled = true;
		}
		#endregion

		#region toolTip イベント
		/// <summary>
		/// toolTip のイベントをセットする
		/// </summary>
		private void SetButtonToolTipEvent()
		{
			try
			{
				if ( (buttonToolTipWidth != null) && !toolTip.OwnerDraw )
				{
					this.toolTip.OwnerDraw = true;
					this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.toolTip_Popup);
					this.toolTip.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.toolTip_Draw);
				}
				else if ( (buttonToolTipWidth == null) && toolTip.OwnerDraw )
				{
					this.toolTip.OwnerDraw = false;
					this.toolTip.Popup -= this.toolTip_Popup;
					this.toolTip.Draw -= this.toolTip_Draw;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// toolTip_Popup
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolTip_Popup(object sender, PopupEventArgs e)
		{
			try
			{
				e.ToolTipSize = new Size(Math.Min(e.ToolTipSize.Width, int.Parse(buttonToolTipWidth)), e.ToolTipSize.Height);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// toolTip_Draw
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
		{
			try
			{
#if FOR_WINDOWS7
				System.Drawing.Drawing2D.LinearGradientBrush linearGradientBrush =
					new System.Drawing.Drawing2D.LinearGradientBrush(
						e.Graphics.VisibleClipBounds,
						Color.White,
						Color.FromArgb(228,229,240),
						System.Drawing.Drawing2D.LinearGradientMode.Vertical);
				e.Graphics.FillRectangle(linearGradientBrush, e.Bounds);
#else
				e.DrawBackground();
#endif

				e.DrawBorder();

				//e.DrawText();
				using ( StringFormat sf = new StringFormat() )
				{
					/*sf.Alignment = StringAlignment.Near;
					sf.LineAlignment = StringAlignment.Center;
					sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
					sf.FormatFlags = StringFormatFlags.NoWrap;*/
					sf.Trimming = StringTrimming.EllipsisPath/*EllipsisCharacter*/;

					string[] toolTipText = e.ToolTipText.Split('\n');
					int y = 2;
					Size size = new Size(e.Bounds.Width - 1 - 2, e.Font.Height);
					int heightOffset = (e.Font.Name == "Tahoma") ? -1 : 0;

					foreach ( string text in toolTipText )
					{
						Rectangle rect = new Rectangle(new Point(1, y), size);

						e.Graphics.DrawString(text.TrimEnd('\r'), e.Font, SystemBrushes.InfoText/*ActiveCaptionText*/, rect, sf);

						y += e.Font.Height + heightOffset;
					}
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region インポートするファイルのドラッグ＆ドロップ
		/// <summary>
		/// インポートするファイルのドラッグが開始された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl_DragEnter(object sender, DragEventArgs e)
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
		/// インポートするショートカットがドロップされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl_DragDrop(object sender, DragEventArgs e)
		{
			try
			{
				//string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
				List<string> fileNames = new List<string>((string[])e.Data.GetData(DataFormats.FileDrop));
				fileNames.Sort();

				foreach ( string fileName in fileNames )
				{
					// インポート用 ccMushroom 構成ファイル
					string fileCcConfigurationImport = Application.StartupPath + "\\" + CC_CONFIGURATION_IMPORT_FILE_NAME;
					XmlDocument xmlCcConfigurationImport = new XmlDocument();
					xmlCcConfigurationImport.Load(fileCcConfigurationImport);

					/*XmlNode nodeDefTabText = xmlCcConfigurationImport.DocumentElement[ccMushroom.TAG_DEFAULT_TAB_TEXT];
					string defTabText = (nodeDefTabText == null) ? "お気に入り" : nodeDefTabText.InnerText;*/
					//bool existDefTabPage = (tabControl.TabPages.IndexOfKey(defTabText) != -1);

					ShortcutMngDlg shortcutMngDlg = new ShortcutMngDlg(ShortcutMngDlg.usage.import, null);
					shortcutMngDlg.xmlCcConfigurationImport = xmlCcConfigurationImport;
					shortcutMngDlg.tabText = ((tabControl.TabCount == 0)/* || !existDefTabPage*/) ? defaultTabText/*defTabText*/ : selectedTabPageText;
					shortcutMngDlg.buttonText = Path.GetFileNameWithoutExtension(fileName);
					shortcutMngDlg.appName = fileName;

					if ( shortcutMngDlg.ShowDialog(this) != DialogResult.OK )
						continue;

					Cursor.Current = Cursors.WaitCursor;

					// ショートカットのインポート用ノードを作成する
					XmlNode applicationImport, application;
					if ( (applicationImport = CreateAppImportNode(ref xmlCcConfigurationImport, shortcutMngDlg.tabText, shortcutMngDlg.buttonText, shortcutMngDlg.buttonBackColor, shortcutMngDlg.appName, shortcutMngDlg.commandLine, shortcutMngDlg.workingDirectory, shortcutMngDlg.comment, shortcutMngDlg.iconFile, true, shortcutMngDlg.autoExec, shortcutMngDlg.newApp)) == null )
						continue;

					// タブページの処理
					int iTabPage;
					if ( (iTabPage = FindTabPage(applicationImport[TAG_TAB_TEXT].InnerText/*, ref appTabPages, true*/)) == -1 )
					{
						if ( (iTabPage = (appTabPages == null) ? 0 : appTabPages.Length) == 0 )
						{
							SetSelectedTabPageText(shortcutMngDlg.tabText);
						}
						AppendNewTabPage(iTabPage, applicationImport[TAG_TAB_TEXT].InnerText, true);	// タブページを作成して追加する
						this.tabControl.SelectedIndex = iTabPage;
						Application.DoEvents();
					}

					UpdateTabPageTextByNewApp(shortcutMngDlg.newApp);

					api.SendMessage(tabControl.SelectedTab.Handle, api.WM_HSCROLL, api.SB_LEFT, 0);

					// ボタンの処理
					int countButtonInTabPage = tabControl.SelectedTab.Controls.Count + 1;
					Point[] pointButtonInTabPage, pointButtonInTabPageHScroll = new Point[countButtonInTabPage];
					int hscrollButtonCount = ComputeButtonPoint(appTabPages[iTabPage].Name, new Rectangle(tabControl.SelectedTab.Top, tabControl.SelectedTab.Bottom, tabControl.SelectedTab.Width, tabControl.SelectedTab.Height), countButtonInTabPage, out pointButtonInTabPage, ref pointButtonInTabPageHScroll);
					int iButton = (appButtons == null) ? 0 : appButtons.Length;
					int tabIndex = countButtonInTabPage - 1;
					Point location = (countButtonInTabPage < hscrollButtonCount) ? pointButtonInTabPage[tabIndex] : pointButtonInTabPageHScroll[tabIndex];
					application = applicationImport;
					application[TAG_APP_NAME].InnerText = ccf.CheckEnvironmentVariable(application[TAG_APP_NAME].InnerText);
					Size _sizeButton = buttonSize;
					//Color _buttonTextColor = buttonTextColor;
					bool _transButton = false;
#if ENABLE_TAB_BACKGROUND
					if ( appTabPages[iTabPage].BackgroundImage != null )
					{
						_sizeButton = GetTabButtonSize(appTabPages[iTabPage].Tag/*Name*/);
						//_buttonTextColor = GetTabButtonTextColor(appTabPages[iTabPage].Name);
						_transButton = IsTabTransparentButton(appTabPages[iTabPage].Name);
					}
#endif
					AppendNewButton(iTabPage, iButton, location, tabIndex, application, _sizeButton, /*_buttonTextColor, */_transButton);	// ボタンを作成して追加する

					if ( hscrollButtonCount <= countButtonInTabPage )
					{
						api.PostMessage(tabControl.SelectedTab.Handle, api.WM_HSCROLL, api.SB_RIGHT, 0);
					}

					// <application> ノードを追加して保存する
					xmlCcConfigurationImport.DocumentElement.AppendChild(applicationImport);
					xmlCcConfigurationImport.Save(fileCcConfigurationImport);

					// ccMushroom 構成ファイルの最後尾（とりあえず）に、<application> ノードをインポートする（一応保存しておく）
					application = xmlCcConfiguration.ImportNode(application, true);
					SetApplicationAttribute(ref application, ATTRIB_IMPORTED, true.ToString().ToLower());
					xmlCcConfiguration.DocumentElement.AppendChild(application);
					xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);
#if (DEBUG)
					/*XmlNodeList appList = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION);
					for ( int i = 0; i < appList.Count; i++ )
					{
						Debug.WriteLine(i + " " + "xml:" + appList[i][TAG_BUTTON_TEXT].InnerText + "\t" + "btn:" + (appButtons[i] == null ? "NULL" : appButtons[i].Text));
					}*/
#endif

					SetJumpList();
				}
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
		/// インポート用のアプリケーション ノードを作成する
		/// </summary>
		/// <param name="xmlCcConfigurationImport"></param>
		/// <param name="tabText"></param>
		/// <param name="buttonText"></param>
		/// <param name="buttonTextBackColor"></param>
		/// <param name="appName"></param>
		/// <param name="commandLine"></param>
		/// <param name="workingDirectory"></param>
		/// <param name="comment"></param>
		/// <param name="iconFile"></param>
		/// <param name="enabled"></param>
		/// <param name="autoExec"></param>
		/// <param name="newApp"></param>
		/// <returns></returns>
		static public XmlNode CreateAppImportNode(ref XmlDocument xmlCcConfigurationImport, string tabText, string buttonText, string buttonTextBackColor, string appName, string commandLine, string workingDirectory, string comment, string iconFile, bool enabled, bool autoExec, bool newApp)
		{
			try
			{
				// <application> ノード作成する
				XmlNode application = xmlCcConfigurationImport.CreateElement(TAG_APPLICATION);

				XmlAttribute attrib = xmlCcConfigurationImport.CreateAttribute(ATTRIB_ENABLED);
				attrib.Value = enabled.ToString().ToLower();
				application.Attributes.Append(attrib);

				if ( autoExec )
				{
					attrib = xmlCcConfigurationImport.CreateAttribute(ATTRIB_AUTO_EXEC);
					attrib.Value = autoExec.ToString().ToLower();
					application.Attributes.Append(attrib);
				}

				if ( newApp )
				{
					attrib = xmlCcConfigurationImport.CreateAttribute(ATTRIB_NEW);
					attrib.Value = newApp.ToString().ToLower();
					application.Attributes.Append(attrib);
				}

				XmlElement elem = xmlCcConfigurationImport.CreateElement(TAG_TAB_TEXT);
				elem.InnerText = tabText;
				application.AppendChild(elem);

				elem = xmlCcConfigurationImport.CreateElement(TAG_BUTTON_TEXT);
				elem.InnerText = buttonText;
				application.AppendChild(elem);

				if ( !string.IsNullOrEmpty(buttonTextBackColor) )
				{
					XmlAttribute attr = xmlCcConfigurationImport.CreateAttribute(ATTRIB_BACK_COLOR);
					attr.Value = buttonTextBackColor;
					elem.Attributes.Append(attr);
				}

				elem = xmlCcConfigurationImport.CreateElement(TAG_APP_NAME);
				elem.InnerText = appName;
				application.AppendChild(elem);

				if ( !string.IsNullOrEmpty(commandLine) )
				{
					elem = xmlCcConfigurationImport.CreateElement(TAG_COMMAND_LINE);
					elem.InnerText = commandLine;
					application.AppendChild(elem);
				}

				if ( !string.IsNullOrEmpty(workingDirectory) )
				{
					elem = xmlCcConfigurationImport.CreateElement(TAG_WORKING_DIRECTORY);
					elem.InnerText = workingDirectory;
					application.AppendChild(elem);
				}

				if ( !string.IsNullOrEmpty(comment) )
				{
					elem = xmlCcConfigurationImport.CreateElement(TAG_COMMENT);
					elem.InnerText = comment;
					application.AppendChild(elem);
				}

				if ( iconFile != null )
				{
					elem = xmlCcConfigurationImport.CreateElement(TAG_ICON_FILE);
					elem.InnerText = iconFile;
					application.AppendChild(elem);
				}

				return application;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return null;
			}
		}
		#endregion

		#region contextMenuButton イベント
		/// <summary>
		/// contextMenuButton が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuButton_Opening(object sender, CancelEventArgs e)
		{
			try
			{
				Button appButton = contextMenuButton.SourceControl as Button;

				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)appButton.Tag];

				string appName = application[TAG_APP_NAME].InnerText;

				menuOpenAppFolder.Enabled = !appName.StartsWith("http");
				
				bool imported = (application.Attributes[ATTRIB_IMPORTED] != null);
				menuEditShortcut.Enabled = imported;

#if FOR_WINDOWS7
				if ( runAsButtons.IndexOf(application[TAG_BUTTON_TEXT].InnerText) == -1 )
				{
					menuRunAsAdministrator.Enabled = appName.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase) || appName.EndsWith(".com", StringComparison.CurrentCultureIgnoreCase) || appName.EndsWith(".bat", StringComparison.CurrentCultureIgnoreCase);
				}
				else
				{
					menuRunAsAdministrator.Enabled = false;
				}
#endif
				
				//ResetFormOpacity();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine("[" + MethodBase.GetCurrentMethod().Name + "] " + exp.Message);
			}
		}

		/// <summary>
		/// [ショートカットを編集する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuEditShortcut_Click(object sender, EventArgs e)
		{
			Button button = contextMenuButton.SourceControl as Button;
			EditShortcut(button);
		}

		/// <summary>
		/// [アプリケーション フォルダを開く] コンテキストメニュー
		/// </summary>
		private void menuOpenAppFolder_Click(object sender, System.EventArgs e)
		{
			try
			{
				Button button = contextMenuButton.SourceControl as Button;

				string directoryName;

				XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)button.Tag];

				string appName = application[TAG_APP_NAME].InnerText;
				directoryName = Path.GetDirectoryName(appName);

				if ( (application[TAG_COPY_MODE] != null) && (GetCopyMode(application[TAG_COPY_MODE].InnerText) == cm.shortCut) )
				{
					if ( application[TAG_WORKING_DIRECTORY] != null )
					{
						directoryName = application[TAG_WORKING_DIRECTORY].InnerText;
					}
				}
	
				Process.Start("explorer", directoryName);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// [管理者として実行] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuRunAsAdministrator_Click(object sender, EventArgs e)
		{
			try
			{
				Button button = contextMenuButton.SourceControl as Button;

				ProcessStart((int)button.Tag,  "RunAs");
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

		}
		#endregion

		#region ショートカットを編集する
		/// <summary>
		/// [ショートカットを編集する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditShortcut(Button button)
		{
			try
			{
				int iButton = (int)button.Tag;

				bool newApp;
				string buttonText = GetButtonTextFromToolTip(toolTip.GetToolTip(button), out newApp, false);

				// インポート用 ccMushroom 構成ファイル
				string fileCcConfigurationImport = Application.StartupPath + "\\" + CC_CONFIGURATION_IMPORT_FILE_NAME;
				XmlDocument xmlCcConfigurationImport = new XmlDocument();
				xmlCcConfigurationImport.Load(fileCcConfigurationImport);

				string xpath = ccMushroom.TAG_APPLICATION + "[" + TAG_BUTTON_TEXT + "='" + buttonText + "']";
				XmlNode applicationImport = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);
				if ( applicationImport == null )
				{
					MessageBox.Show("ボタン名が見つかりませんでした．\r\n" + fileCcConfigurationImport, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				StringBuilder tabNames = new StringBuilder(selectedTabPageText);
				for ( int i = 0; i < appTabPages.Length; i++ )
				{
					tabNames.Append("\t" + appTabPages[i].Name);
				}

				ShortcutMngDlg shortcutMngDlg = new ShortcutMngDlg(ShortcutMngDlg.usage.edit, buttonText);
				shortcutMngDlg.xmlCcConfigurationImport = xmlCcConfigurationImport;
				shortcutMngDlg.tabText = tabNames.ToString()/*selectedTabPageText*/;
				shortcutMngDlg.buttonText = buttonText;
				shortcutMngDlg.appName = applicationImport[TAG_APP_NAME].InnerText;
				shortcutMngDlg.commandLine = (applicationImport[TAG_COMMAND_LINE] != null) ? applicationImport[TAG_COMMAND_LINE].InnerText : "";
				shortcutMngDlg.workingDirectory = (applicationImport[TAG_WORKING_DIRECTORY] != null) ? applicationImport[TAG_WORKING_DIRECTORY].InnerText : "";
				shortcutMngDlg.comment = (applicationImport[TAG_COMMENT] != null) ? applicationImport[TAG_COMMENT].InnerText : "";
				shortcutMngDlg.iconFile = (applicationImport[TAG_ICON_FILE] != null) ? applicationImport[TAG_ICON_FILE].InnerText : null;
				shortcutMngDlg.autoExec = (applicationImport.Attributes[ATTRIB_AUTO_EXEC] != null) ? bool.Parse(applicationImport.Attributes[ATTRIB_AUTO_EXEC].Value) : false;
				shortcutMngDlg.buttonBackColor = (applicationImport[TAG_BUTTON_TEXT].Attributes[ATTRIB_BACK_COLOR] != null) ? applicationImport[TAG_BUTTON_TEXT].Attributes[ATTRIB_BACK_COLOR].Value : null;

				shortcutMngDlg.newApp = (applicationImport.Attributes[ATTRIB_NEW] != null) ? bool.Parse(applicationImport.Attributes[ATTRIB_NEW].Value) : false;

				if ( shortcutMngDlg.ShowDialog(this) != DialogResult.OK )
					return;

				Cursor.Current = Cursors.WaitCursor;

				if ( shortcutMngDlg.checkDelete.Checked )					// ボタンを削除する？
				{
					// <application> ノードを削除する
					xmlCcConfigurationImport.DocumentElement.RemoveChild(applicationImport);
					xmlCcConfigurationImport.Save(fileCcConfigurationImport);

					// ccMushroom 構成ファイルの <application> ノードを削除する（一応保存しておく）
					xmlCcConfiguration.DocumentElement.RemoveChild(xmlCcConfiguration.DocumentElement.ChildNodes[iButton]);
					xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);

					// タブページからボタンを削除する
					RemoveButton(iButton);

					// ボタンとアイコンの配列を削除する
					ArrayRemoveAt(ref appButtons, iButton);
					if ( appButtons != null )
					{
						for ( int i = iButton; i < appButtons.Length; i++ )
						{
							if ( appButtons[i] == null )
								continue;
							appButtons[i].Tag = (int)appButtons[i].Tag - 1;
						}
					}
					ArrayRemoveAt(ref appIcons, iButton);

					if ( tabControl.TabPages.Count == 0 )
					{
						SetSelectedTabPageText(null);
					}
				}
				else
				{
					XmlNode application;	// 編集後の <application> ノード
					string appName = ccf.CheckEnvironmentVariable(shortcutMngDlg.appName);
					if ( (application = CreateAppImportNode(ref xmlCcConfigurationImport, shortcutMngDlg.tabText, shortcutMngDlg.buttonText, shortcutMngDlg.buttonBackColor, appName, shortcutMngDlg.commandLine, shortcutMngDlg.workingDirectory, shortcutMngDlg.comment, shortcutMngDlg.iconFile, true, shortcutMngDlg.autoExec, shortcutMngDlg.newApp)) == null )
						return;

					// ボタンを編集する
					appButtons[iButton].Name = "button" + application[TAG_BUTTON_TEXT].InnerText;

					appButtons[iButton].BackColor = (shortcutMngDlg.buttonBackColor == null) ? buttonBackColor : Color.FromName(shortcutMngDlg.buttonBackColor);
#if ENABLE_TAB_BACKGROUND
					if ( IsTabTransparentButton(tabControl.SelectedTab.Name) )
					{
						appButtons[iButton].BackColor = Color.Transparent;
					}
#endif

					// アイコンを編集する
					if ( appIcons[iButton] != null )
					{
						api.DestroyIcon(appIcons[iButton].Handle);
						appIcons[iButton] = null;
					}
					string appIconFile = (application[TAG_ICON_FILE] == null) ? application[TAG_APP_NAME].InnerText : application[TAG_ICON_FILE].InnerText;
					appIcons[iButton] = ccf.GetIcon(appIconFile, smallApplicationIcon);

					// ToolTip を設定する
					string appToolTip = MakeButtonToolTip(application, iButton);
					toolTip.SetToolTip(appButtons[iButton], appToolTip);

					UpdateTabPageTextByNewApp(shortcutMngDlg.newApp);

					// <application> ノードを編集して保存する
					application[TAG_APP_NAME].InnerText = shortcutMngDlg.appName;
					if ( !shortcutMngDlg.checkMoveToLast.Checked )	// 順番は保持する？
					{
						xmlCcConfigurationImport.DocumentElement.ReplaceChild(application, applicationImport);
					}
					else
					{
						xmlCcConfigurationImport.DocumentElement.RemoveChild(applicationImport);
						xmlCcConfigurationImport.DocumentElement.AppendChild(application);
					}
					xmlCcConfigurationImport.Save(fileCcConfigurationImport);

					// ccMushroom 構成ファイルの <application> ノードを編集する（一応保存しておく）
					application[TAG_APP_NAME].InnerText = appName;
					application = xmlCcConfiguration.ImportNode(application, true);
					SetApplicationAttribute(ref application, ATTRIB_IMPORTED, true.ToString().ToLower());
					xmlCcConfiguration.DocumentElement.ReplaceChild(application, xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)button.Tag]);
					xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);

					if ( selectedTabPageText == shortcutMngDlg.tabText )	// タブ名は変更されてない？
					{
						appButtons[iButton].Invalidate();
						return;
					}

					// タブページからボタンを削除する
					RemoveButton(iButton);
					SetJumpList();

					// タブページの処理
					int iTabPage;
					if ( (iTabPage = FindTabPage(shortcutMngDlg.tabText/*, ref appTabPages, true*/)) == -1 )
					{
						iTabPage = (appTabPages == null) ? 0 : appTabPages.Length;
						AppendNewTabPage(iTabPage, shortcutMngDlg.tabText/*application*/, true);	// タブページを作成して追加する
					}
					this.tabControl.SelectedIndex = iTabPage;
					Application.DoEvents();

					/*// ボタンの処理
					int countButtonInTabPage = tabControl.SelectedTab.Controls.Count + 1;
					Point[] pointButtonInTabPage, pointButtonInTabPageHScroll = new Point[countButtonInTabPage];
					int hscrollButtonCount = ComputeButtonPoint(new Rectangle(tabControl.SelectedTab.Top, tabControl.SelectedTab.Bottom, tabControl.SelectedTab.Width, tabControl.SelectedTab.Height), countButtonInTabPage, out pointButtonInTabPage, ref pointButtonInTabPageHScroll);
					int tabIndex = countButtonInTabPage - 1;
					appButtons[iButton].TabIndex = tabIndex;
					appButtons[iButton].Location = (countButtonInTabPage < hscrollButtonCount) ? pointButtonInTabPage[tabIndex] : pointButtonInTabPageHScroll[tabIndex];*/

					// ボタンをタブページに追加する
					appTabPages[iTabPage].Controls.Add(appButtons[iButton]);

					UpdateTabPageTextByNewApp(shortcutMngDlg.newApp);

#if MOVABLE_BUTTON_BY_DRAGDROP
					if ( shortcutMngDlg.checkMoveToLast.Checked )
					{
						int destButtonTag = (importTabAppearFirst ? xmlCcConfigurationImport.DocumentElement.SelectNodes("/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[@" + ATTRIB_ENABLED + "='" + true.ToString().ToLower() + "']").Count : appButtons.Length) - 1;
						MoveAppButtonToLast(destButtonTag, iButton, true);
					}
#endif
				}

				RefreshSelectedTabPage(true);
				SetJumpList();
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

#if MOVABLE_BUTTON_BY_DRAGDROP
		/// <summary>
		/// ボタン配列の指定されたボタンを最後尾に移動する
		/// </summary>
		/// <param name="destButtonTag"></param>
		/// <param name="sorcButtonTag"></param>
		private void MoveAppButtonToLast(int destButtonTag, int sorcButtonTag, bool saveCcConfiguration)
		{
			Button destButton = appButtons[destButtonTag];

			Button appButton = appButtons[sorcButtonTag];
			Icon appIcon = appIcons[(int)appButton.Tag];

			//uint sorcButtonOrder = ccf.MAKELONG((ushort)(int)appButton.Tag, (ushort)appButton.TabIndex);
			uint destButtonOrder = ccf.MAKELONG((ushort)(int)destButton.Tag, (ushort)destButton.TabIndex);

			int move = (int)destButton.Tag - (int)appButton.Tag;

			// 後ろに移動
			int shiftStart = (int)appButton.Tag + 1;
			int shiftEnd = (int)destButton.Tag;

			// ドラッグ元の一つ後ろからドロップ先までのボタンを前にずらす
			for ( int i = shiftStart; i <= shiftEnd; i++ )
			{
				int j = i - 1;
				appButtons[j] = appButtons[i];
				//appButtons[j].TabIndex -= (selectedTab.Controls[appButtons[j].Name] != null ? 1 : 0);
				appButtons[j].Tag = (int)j;
				appIcons[j] = appIcons[i];
			}

			// ドロップ先にドロップ元のボタンを移動する
			//int destButtonTag = LOWORD(destButtonOrder);
			appButtons[destButtonTag] = appButton;
			appButtons[destButtonTag].TabIndex = ccf.HIWORD(destButtonOrder);
			appButtons[destButtonTag].Tag = destButtonTag;
			appIcons[destButtonTag] = appIcon;

			//int sorcButtonTag = ccf.LOWORD(sorcButtonOrder);

			// ccMshroom 構成ファイルの <application> ノードを移動する（一応保存しておく）
			XmlNode sorcApplication = xmlCcConfiguration.DocumentElement.ChildNodes[sorcButtonTag];
			XmlNode destApplication = xmlCcConfiguration.DocumentElement.ChildNodes[destButtonTag];
			xmlCcConfiguration.DocumentElement.InsertAfter(sorcApplication, destApplication);
			if ( saveCcConfiguration )
			{
				xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);
			}
		}
#endif

		/// <summary>
		/// 新しいアプリケーションの状態によりタブ名を更新する
		/// </summary>
		/// <param name="newApp"></param>
		private void UpdateTabPageTextByNewApp(bool newApp)
		{
			if ( !Program.debMode )
				return;

			if ( newApp )
			{
				if ( !tabControl.SelectedTab.Text.EndsWith(newAppIndicator) )
				{
					this.tabControl.SelectedTab.Text += newAppIndicator;
				}
			}
			else
			{
				if ( tabControl.SelectedTab.Text.EndsWith(newAppIndicator) )
				{
					if ( !HasTabPageNewApp() )
					{
						this.tabControl.SelectedTab.Text = selectedTabPageText;
					}
				}
			}
		}

		/// <summary>
		/// タブページが新しいアプリケーション ボタンを持っているか否か
		/// </summary>
		/// <returns></returns>
		private bool HasTabPageNewApp()
		{
			foreach ( Control control in tabControl.SelectedTab.Controls )
			{
				if ( !(control is Button) )
					continue;

				bool newApp;
				GetButtonTextFromToolTip(toolTip.GetToolTip(control), out newApp, false);
				if ( newApp )
					return true;
			}

			return false;
		}

		/// <summary>
		/// タブページからボタンを削除する
		/// </summary>
		/// <param name="iButton"></param>
		private void RemoveButton(int iButton)
		{
			foreach ( Control control in tabControl.SelectedTab.Controls )
			{
				if ( !(control is Button) )
					continue;
				if ( appButtons[iButton].TabIndex < control.TabIndex )
				{
					//int _iButton = (int)control.Tag;
					//appButtons[_iButton].TagIndex = appButtons[_iButton].TagIndex - 1;
					control.TabIndex--;		// タブページ内の順番を一つ前に詰める
				}
			}

			tabControl.SelectedTab.Controls.Remove(appButtons[iButton]);

			if ( tabControl.SelectedTab.Controls.Count == 0 )
			{
				int selectedIndex = tabControl.SelectedIndex;

				// タブページからアイコンを削除する
				DelTabPageIcon(selectedIndex);

#if ENABLE_TAB_BACKGROUND
				if ( tabControl.SelectedTab.BackgroundImage != null )
				{
					// タブの背景画像を解除する
					TabMngDlg.DeleteTabPageSection(tabControl.SelectedTab.Text);

					tabControl.SelectedTab.BackgroundImage.Dispose();
					tabControl.SelectedTab.BackgroundImage = null;
				}
#endif

				RemoveThumbnail(tabControl.SelectedTab);

				// ボタンが無くなったタブページは削除する
				tabControl.TabPages.Remove(tabControl.SelectedTab);

				// タブページの配列を削除する
				ArrayRemoveAt(ref appTabPages, selectedIndex);

				return;
			}

			UpdateTabPageTextByNewApp(false);
		}

		/// <summary>
		/// 配列の一部を削除する
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="remove"></param>
		private void ArrayRemoveAt<T>(ref T[] source, int remove)
		{
			T[] temp = new T[source.Length - 1];
			for ( int i = 0, j = 0; i < source.Length; i++ )
			{
				if ( i == remove )
					continue;
				temp[j] = source[i];
				j++;
			}

			source = temp;
			if ( source.Length == 0 )
			{
				source = null;
			}
		}
		#endregion

		#region タブを編集する
		/// <summary>
		/// contextMenuTab が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void contextMenuTab_Opening(object sender, CancelEventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				bool hasImportedApplication = false;

				foreach ( Control control in tabControl.SelectedTab.Controls )
				{
					if ( !(control is Button) )
						continue;

					XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)control.Tag];
					if ( (hasImportedApplication = (application.Attributes[ATTRIB_IMPORTED] != null)) )
						break;
				}

				//menuEditTab.Enabled = hasImportedApplication;
				toolStripMenuEditTab.Tag = hasImportedApplication;

#if ENABLE_TAB_BACKGROUND
				bool withBackgroundImage = (tabControl.SelectedTab.BackgroundImage != null);
				toolStripMenuImageLayout.Enabled = withBackgroundImage;

				toolStripMenuTransButton.Enabled = withBackgroundImage;
				toolStripMenuTransButton.Checked = IsTabTransparentButton(tabControl.SelectedTab.Name);

				toolStripMenuResumeFomeSize.Enabled = (withBackgroundImage && (this.WindowState == FormWindowState.Normal));
				toolStripMenuResumeFomeSize.Checked = (GetTabResumeFormSize(tabControl.SelectedTab.Name) != Size.Empty);
#if FOR_WINDOWS7
				if ( toolStripMenuResumeFomeSize.Enabled )
				{
					toolStripMenuResumeFomeSize.Enabled = !enabledTaskbarThumbnail;
				}
#endif
#else
				toolStripMenuImageLayout.Enabled = false;
				toolStripMenuTransButton.Enabled = false;
				toolStripMenuResumeFomeSize.Enabled = false;
#endif

#if FOR_WINDOWS7
				toolStripMenuForJumpListTabPage.Checked = (forJumpListTabPage == selectedTabPageText);
#endif
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
		/// [タブを編集する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuEditTab_Click(object sender, EventArgs e)
		{
			try
			{
				StringBuilder tabNames = new StringBuilder(selectedTabPageText);
				for ( int i = 0; i < appTabPages.Length; i++ )
				{
					tabNames.Append("\t" + appTabPages[i].Name);
				}

				TabMngDlg tabMngDlg = new TabMngDlg();
				tabMngDlg.hasImportedApplication = (bool)toolStripMenuEditTab.Tag;
				tabMngDlg.tabText = tabNames.ToString()/*selectedTabPageText*/;
				tabMngDlg.iconFileName = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + selectedTabPageText + ".ico";
				tabMngDlg.iconFileName = File.Exists(tabMngDlg.iconFileName) ? tabMngDlg.iconFileName : null;

				if ( tabMngDlg.ShowDialog(this) != DialogResult.OK )
					return;

				Cursor.Current = Cursors.WaitCursor;

				// タブページのアイコン処理
				if ( tabMngDlg.iconFileName != null )
				{
					try
					{
						if ( tabMngDlg.checkShowIcon.Checked )
						{
							string tabIconFileName = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + tabMngDlg.tabText + ".ico";
							if ( tabMngDlg.iconFileName != tabIconFileName )
							{
								File.Copy(tabMngDlg.iconFileName, tabIconFileName, true);	// とりあえず無条件にコピっとく
							}

							if ( tabMngDlg.tabText == selectedTabPageText )
							{
								SetTabPageIcon(tabControl.SelectedIndex);
							}
						}
						else
						{
							if ( tabMngDlg.tabText == selectedTabPageText )
							{
								DelTabPageIcon(tabControl.SelectedIndex);
							}
						}

						tabMngDlg.CleanupTempFile(tabMngDlg.tabText);
					}
					catch ( Exception exp )
					{
						//Debug.WriteLine(exp.Message);
						MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}

				if ( tabMngDlg.tabText == selectedTabPageText )
				{
#if ENABLE_TAB_BACKGROUND
					if ( tabControl.SelectedTab.BackgroundImage != null )
					{
						tabControl.SelectedTab.BackgroundImage.Dispose();
						tabControl.SelectedTab.BackgroundImage = null;
					}

					bool transButton = false;

					// タブの背景画像は有効？
					if ( tabMngDlg.checkEnabledBackground.Checked )
					{
						if ( tabMngDlg.tabBackground != null )
						{
							tabControl.SelectedTab.BackgroundImage = new Bitmap(tabMngDlg.tabBackground);
							tabControl.SelectedTab.BackgroundImageLayout = GetTabBackgroundImageLayout(tabControl.SelectedTab.Name);

							transButton = IsTabTransparentButton(tabControl.SelectedTab.Name);

							Size resumeFormSize = GetTabResumeFormSize(tabControl.SelectedTab.Name);
							if ( !resumeFormSize.IsEmpty )
							{
								this.Size = resumeFormSize;
							}

							tabMngDlg.tabBackground.Dispose();
							tabMngDlg.tabBackground = null;
						}
					}

					ResetButtonBackColor(tabControl.SelectedTab, transButton);

					tabControl.SelectedTab.Refresh();
#endif
					return;
				}

				// インポート用 ccMushroom 構成ファイル
				string fileCcConfigurationImport = Application.StartupPath + "\\" + CC_CONFIGURATION_IMPORT_FILE_NAME;
				XmlDocument xmlCcConfigurationImport = new XmlDocument();
				xmlCcConfigurationImport.Load(fileCcConfigurationImport);

				// 一旦、タブページ内のボタンの Tag を抜き出しておく
				List<int> iButtons = new List<int>();
				foreach ( Control control in tabControl.SelectedTab.Controls )
				{
					if ( !(control is Button) )
						continue;

					XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)control.Tag];
					if ( application.Attributes[ATTRIB_IMPORTED] == null )
						continue;

					iButtons.Add((int)control.Tag);
				}

				foreach ( int iButton in iButtons )
				{
					RemoveButton(iButton);	// タブページからボタンを削除する
				}

				// タブページの処理
				int iTabPage;
				if ( (iTabPage = FindTabPage(tabMngDlg.tabText/*, ref appTabPages, true*/)) == -1 )
				{
					iTabPage = (appTabPages == null) ? 0 : appTabPages.Length;
					AppendNewTabPage(iTabPage, tabMngDlg.tabText, true);	// タブページを作成して追加する
				}
				else
				{
					SetTabPageIcon(iTabPage);	// 既存の無アイコンタブに、同名タブがアイコン付きでマージされた時の対策
				}
				this.tabControl.SelectedIndex = iTabPage;
				SetSelectedTabPageText(this.tabControl.SelectedTab.Text);	// タブが１つの場合、SelectedIndexChanged のイベントは発生しない？
				Application.DoEvents();

				foreach ( int iButton in iButtons )
				{
					bool newApp;
					string buttonText = GetButtonTextFromToolTip(toolTip.GetToolTip(appButtons[iButton]), out newApp, false);

					string xpath = ccMushroom.TAG_APPLICATION + "[" + TAG_BUTTON_TEXT + "='" + buttonText + "']";
					XmlNode applicationImport = xmlCcConfigurationImport.DocumentElement.SelectSingleNode(xpath);
					if ( applicationImport == null )
						continue;

					XmlNode application;	// 編集後の <application> ノード
					if ( (application = CreateAppImportNode(ref xmlCcConfigurationImport, tabMngDlg.tabText, applicationImport[TAG_BUTTON_TEXT].InnerText, (applicationImport[TAG_BUTTON_TEXT].Attributes[ATTRIB_BACK_COLOR] != null) ? applicationImport[TAG_BUTTON_TEXT].Attributes[ATTRIB_BACK_COLOR].Value : null, applicationImport[TAG_APP_NAME].InnerText, (applicationImport[TAG_COMMAND_LINE] != null) ? applicationImport[TAG_COMMAND_LINE].InnerText : null, (applicationImport[TAG_WORKING_DIRECTORY] != null) ? applicationImport[TAG_WORKING_DIRECTORY].InnerText : null, (applicationImport[TAG_COMMENT] != null) ? applicationImport[TAG_COMMENT].InnerText : null, (applicationImport[TAG_ICON_FILE] != null) ? applicationImport[TAG_ICON_FILE].InnerText : null, true, (applicationImport.Attributes[ATTRIB_AUTO_EXEC] != null) ? bool.Parse(applicationImport.Attributes[ATTRIB_AUTO_EXEC].Value) : false, newApp)) == null )
						return;

					if ( !tabMngDlg.checkMoveToLast.Checked )	// 順番は保持する？
					{
						xmlCcConfigurationImport.DocumentElement.ReplaceChild(application, applicationImport);
					}
					else
					{
						xmlCcConfigurationImport.DocumentElement.RemoveChild(applicationImport);
						xmlCcConfigurationImport.DocumentElement.AppendChild(application);
					}

					// ccMushroom 構成ファイルの <application> ノードを編集する
					application[TAG_APP_NAME].InnerText = ccf.CheckEnvironmentVariable(application[TAG_APP_NAME].InnerText);
					application = xmlCcConfiguration.ImportNode(application, true);
					SetApplicationAttribute(ref application, ATTRIB_IMPORTED, true.ToString().ToLower());
					xmlCcConfiguration.DocumentElement.ReplaceChild(application, xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[(int)appButtons[iButton].Tag]);

					/*// ボタンの処理
					int countButtonInTabPage = tabControl.SelectedTab.Controls.Count + 1;
					Point[] pointButtonInTabPage, pointButtonInTabPageHScroll = new Point[countButtonInTabPage];
					int hscrollButtonCount = ComputeButtonPoint(new Rectangle(tabControl.SelectedTab.Top, tabControl.SelectedTab.Bottom, tabControl.SelectedTab.Width, tabControl.SelectedTab.Height), countButtonInTabPage, out pointButtonInTabPage, ref pointButtonInTabPageHScroll);
					int tabIndex = countButtonInTabPage - 1;
					appButtons[iButton].TabIndex = tabIndex;
					appButtons[iButton].Location = (countButtonInTabPage < hscrollButtonCount) ? pointButtonInTabPage[tabIndex] : pointButtonInTabPageHScroll[tabIndex];*/

					// ボタンをタブページに追加する
					appTabPages[iTabPage].Controls.Add(appButtons[iButton]);

					UpdateTabPageTextByNewApp(newApp);
				}

#if MOVABLE_BUTTON_BY_DRAGDROP
				if ( tabMngDlg.checkMoveToLast.Checked )
				{
					for ( int i = 0; i < iButtons.Count; i++ )
					{
						int destButtonTag = (importTabAppearFirst ? xmlCcConfigurationImport.DocumentElement.SelectNodes("/" + TAG_CONFIGURATION + "/" + TAG_APPLICATION + "[@" + ATTRIB_ENABLED + "='" + true.ToString().ToLower() + "']").Count : appButtons.Length) - 1;
						MoveAppButtonToLast(destButtonTag, iButtons[0], false);
					}
				}
#endif
				xmlCcConfigurationImport.Save(fileCcConfigurationImport);						// 保存する

				xmlCcConfiguration.Save(Application.StartupPath + "\\" + CC_CONFIGURATION_FILE_NAME);	//（一応保存しておく）

				RefreshSelectedTabPage(true);
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
		/// ボタンの背景色をリセットする
		/// </summary>
		/// <param name="tabPage"></param>
		/// <param name="transButton"></param>
		private void ResetButtonBackColor(TabPage tabPage, bool transButton)
		{
			foreach ( Control control in tabPage.Controls )
			{
				if ( !(control is Button) )
					continue;
				int iButton = (int)control.Tag;
				if ( transButton )
				{
					appButtons[iButton].BackColor = Color.Transparent;
				}
				else
				{
					XmlNode application = xmlCcConfiguration.DocumentElement.ChildNodes[iButton];
					SetEachButtonBackColor(iButton, application);
				}
			}
		}

		/// <summary>
		/// タブページにアイコンを登録する
		/// </summary>
		/// <param name="iTabPage"></param>
		private void SetTabPageIcon(int iTabPage)
		{
			string tabName = tabControl.TabPages[iTabPage].Name;
			string tabIconFileName = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + tabName + ".ico";

			if ( !File.Exists(tabIconFileName) )	// tabタブ名.ico ファイルは存在しない？
				return;

			try
			{
				int index = appTabIcons.Images.IndexOfKey(tabName);

				using ( FileStream fsIcon = new FileStream(tabIconFileName, FileMode.Open, FileAccess.Read) )
				{
					Bitmap bmp = (Bitmap)Bitmap.FromStream(fsIcon);

					if ( (index == -1)/* && (string.IsNullOrEmpty(tabName))*/ )
					{
						appTabIcons.Images.Add(bmp/*new Bitmap(tabIconFileName)*/);
						appTabIcons.Images.SetKeyName(appTabIcons.Images.Count - 1, tabName);
					}
					else
					{
						appTabIcons.Images[index].Dispose();
						appTabIcons.Images[index] = bmp/*new Bitmap(tabIconFileName)*/;
					}

					fsIcon.Close();
				}

				tabControl.TabPages[iTabPage].ImageKey = tabName;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// タブページからアイコンを削除する
		/// </summary>
		/// <param name="iTabPage"></param>
		private void DelTabPageIcon(int iTabPage)
		{
			try
			{
				string tabName = tabControl.TabPages[iTabPage].Name;
				string tabIconFileName = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + tabName + ".ico";

				if ( string.IsNullOrEmpty(tabControl.TabPages[iTabPage].ImageKey) || !File.Exists(tabIconFileName) )	// アイコンの無いタブページか、tabタブ名.ico ファイルは存在しない？
					return;

				tabControl.TabPages[iTabPage].ImageKey = string.Empty;
				//appTabIcons.Images.RemoveByKey(imageKey);
				if ( (tabName != defaultTabText) && (tabName != "お気に入り") )
				{
					File.Delete(tabIconFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

#if ENABLE_TAB_BACKGROUND
		/// <summary>
		/// toolStripMenuImageLayout が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuImageLayout_DropDownOpening(object sender, EventArgs e)
		{
			try
			{
				ImageLayout imageLayout = GetTabBackgroundImageLayout(tabControl.SelectedTab.Name);

				tileToolStripMenuItem.Checked = (imageLayout == ImageLayout.Tile);
				centerToolStripMenuItem.Checked = (imageLayout == ImageLayout.Center);
				stretchToolStripMenuItem.Checked = (imageLayout == ImageLayout.Stretch);
				zoomToolStripMenuItem.Checked = (imageLayout == ImageLayout.Zoom);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// toolStripMenuItem[Tile|Center|Stretch|Zoom] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuImageLayout_Click(object sender, EventArgs e)
		{
			try
			{
				tileToolStripMenuItem.Checked = false;
				centerToolStripMenuItem.Checked = false;
				stretchToolStripMenuItem.Checked = false;
				zoomToolStripMenuItem.Checked = false;

				((ToolStripMenuItem)sender).Checked = true;
				ImageLayout imageLayout = ConvertImageLayout((string)((ToolStripMenuItem)sender).Tag);

				tabControl.SelectedTab.BackgroundImageLayout = imageLayout;
				tabControl.Refresh();

				api.WritePrivateProfileString(tabControl.SelectedTab.Name, KEY_IMAGE_LAYOUT, imageLayout.ToString(), tabPageSettingsIniFileName);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [ボタンの背景を透過する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuTransButton_Click(object sender, EventArgs e)
		{
			try
			{
				bool transButton = !toolStripMenuTransButton.Checked;

				api.WritePrivateProfileString(tabControl.SelectedTab.Name, KEY_TRANSPARENT_BUTTON, transButton.ToString().ToLower(), tabPageSettingsIniFileName);

				ResetButtonBackColor(tabControl.SelectedTab, transButton);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [このフォーム サイズを保存する] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuResumeFormSize_Click(object sender, EventArgs e)
		{
			try
			{
				Size resumeFormSize = GetTabResumeFormSize(tabControl.SelectedTab.Name);

				string lpString = null;
				if ( resumeFormSize.Width != this.Size.Width || resumeFormSize.Height != this.Size.Height )
				{
					lpString = this.Size.Width + "," + this.Size.Height;
				}

				api.WritePrivateProfileString(tabControl.SelectedTab.Name, KEY_RESUME_FORM_SIZE, lpString, tabPageSettingsIniFileName);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#endif

#if FOR_WINDOWS7
		/// <summary>
		/// [ジャンプリスト用のタブページ] コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStripMenuForJumpListTabPage_Click(object sender, EventArgs e)
		{
			try
			{
				forJumpListTabPage = (toolStripMenuForJumpListTabPage.Checked) ? null : selectedTabPageText;
				api.WritePrivateProfileString(SETTINGS_SECTION, KEY_FOR_JUMPLIST_TABPAGE, forJumpListTabPage, ccMushroomIniFileName);

				SetJumpList();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#endif
		#endregion

		#region notifyIcon イベント
		/// <summary>
		/// notifyIcon がクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
			try
			{
				if ( e.Button == MouseButtons.Left )
				{
					if ( this.WindowState == FormWindowState.Minimized )
					{
						//this.Show();
						this.Visible = true;
						this.WindowState = FormWindowState.Normal;

						this.Activate();

						if ( !alwaysInTasktray )
						{
							notifyIcon.Visible = false;
						}
					}
					else
					{
						if ( alwaysInTasktray )
						{
							if ( this.Visible = !this.Visible )
							{
								this.Activate();
								DisTransparentWithClickThrough(true);
							}
						}
						else
						{
							this.WindowState = FormWindowState.Minimized;
						}
					}
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [元のサイズに戻す] 通知アイコンメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuRestoreWindow_Click(object sender, EventArgs e)
		{
			try
			{
				if ( menuRestoreWindow.Text.EndsWith("(&R)") )		// 元のサイズに戻す(&R)
				{
					if ( !this.Visible )
					{
						this.Visible = true;
						DisTransparentWithClickThrough(true);
					}
					if ( this.WindowState != FormWindowState.Normal )
					{
						this.WindowState = FormWindowState.Normal;
					}

					this.Activate();

					if ( !alwaysInTasktray )
					{
						notifyIcon.Visible = false;
					}
				}
				else if ( menuRestoreWindow.Text.EndsWith("(&D)") )	// Click-through を解除する(&D)
				{
					DisTransparentWithClickThrough(true);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// [プログラムを終了する] 通知アイコンメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// notifyIconMenu が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIconMenu_Opening(object sender, CancelEventArgs e)
		{
			try
			{
				Debug.WriteLine(MethodBase.GetCurrentMethod().Name);
				notifyIconMenu.Visible = true;
	
				// ここでのショートカットキーはクリックした時の判断で使う
				menuRestoreWindow.Text = "元のサイズに戻す(&R)";
				if ( !(menuRestoreWindow.Enabled = ((this.WindowState != FormWindowState.Normal) || !this.Visible)) )
				{
					if ( /*transparentWithClickThrough && */SetWsExTransparent(null) )
					{
						menuRestoreWindow.Text = "Click-through を解除する(&D)";
						menuRestoreWindow.Enabled = true;
					}
				}

				ClearNotifyIconMenu();

				// タブ名をサブメニューとして追加する
				notifyMenuTabPages = new ToolStripMenuItem[tabControl.TabCount];
				for ( int i = 0; i < tabControl.TabCount; i++ )
				{
					notifyMenuTabPages[i] = new ToolStripMenuItem();
					notifyMenuTabPages[i].Name = tabControl.TabPages[i].Text;
					notifyMenuTabPages[i].Text = tabControl.TabPages[i].Text;
					notifyMenuTabPages[i].Tag = i;
					int index = appTabIcons.Images.IndexOfKey(tabControl.TabPages[i].ImageKey/*tabControl.TabPages[i].Name*/);
					if ( index != -1 )
					{
						notifyMenuTabPages[i].Image = appTabIcons.Images[index];
					}
					notifyMenuTabPages[i].DropDownItems.Add(new ToolStripMenuItem());	// ドロップダウンの▲マークを出す為に、ダミー メニューを追加しておく
					notifyMenuTabPages[i].DropDownOpening += new EventHandler(notifyMenuTabPage_Opening);
					notifyIconMenu.Items.Insert(2, notifyMenuTabPages[i]);
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// notifyIconMenu が開かれた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIconMenu_Opened(object sender, EventArgs e)
		{
			try
			{
				if ( (int)notifyIconMenu.Tag != HOTKEY_SHOW_CONTEXT_MENU )	// ホットキーから開かれたのではない？
					return;

				if ( notifyIconMenu.Handle != IntPtr.Zero )
				{
					//api.SetWindowPos(notifyIconMenu.Handle, (IntPtr)api.HWND_TOPMOST, Cursor.Position.X, Cursor.Position.Y, 0, 0, api.SWP_NOSIZE);
					api.SetForegroundWindow(notifyIconMenu.Handle);
				}

				string latestNotifyMenuItem = string.Empty;
				if ( resumeLatestNotifyMenuItem )
				{
					StringBuilder returnedString = new StringBuilder(256);

					// 最後に選択された notifyIconMenu のアイテム名
					api.GetPrivateProfileString(RESUME_SECTION, KEY_LATEST_NOTIFY_MENU_ITEM, "", returnedString, (uint)returnedString.Capacity, ccMushroomIniFileName);
					latestNotifyMenuItem = returnedString.ToString();
				}

				//// 起動後の一番最初に表示された時、最後に挿入された Items[3].Bounds.Y が何故か 0 になっている為、高さを順番に足し込んでいく
				//int height = notifyIconMenu.Items[0].Bounds.Y;

				int i;
				for ( i = 0; i < notifyIconMenu.Items.Count; i++ )
				{
					Debug.WriteLine(i + " " + notifyIconMenu.Items[i].Bounds + " " + notifyIconMenu.Items[i].Text);

					if ( latestNotifyMenuItem == notifyIconMenu.Items[i].Text )
						break;

					//height += notifyIconMenu.Items[i].Height;
				}

				//if ( resumeLatestNotifyMenuItem )
				{
					i = (i == notifyIconMenu.Items.Count) ? 0 : i;
					int x = notifyIconMenu.Left + (int)(notifyIconMenu.Width * 0.8F);
					int y = notifyIconMenu.Top + notifyIconMenu.Items[i].Bounds.Y + notifyIconMenu.Items[i].Height / 2;
					//int y = notifyIconMenu.Top + height + (notifyIconMenu.Items[i].Height / 2);
					Cursor.Position = new Point(x, y);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
			finally
			{
				notifyIconMenu.Tag = 0;
			}
		}

		/// <summary>
		/// notifyIconMenu_Move
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIconMenu_Move(object sender, EventArgs e)
		{
			try
			{
				Debug.WriteLine("Move: " + notifyIconMenu.Left + "," + notifyIconMenu.Top);

				// ホットキーから開かれたのではない。またはフォームが非表示になっている？
				if ( ((int)notifyIconMenu.Tag != HOTKEY_SHOW_CONTEXT_MENU) || (notifyIconMenuLocation == Point.Empty) )
					return;

				int screenWidth, screenHeight;

				try
				{
					byte[] rect = new byte[sizeof(int) * 4];	// left, top, right, bottom
					api.SystemParametersInfo(api.SPI_GETWORKAREA, 0, rect, 0);
					screenWidth = BitConverter.ToInt32(rect, sizeof(int) * 2);
					screenHeight = BitConverter.ToInt32(rect, sizeof(int) * 3);
					/*int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(api.RECT));
					IntPtr pRect = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
					System.Runtime.InteropServices.Marshal.Copy(rect, 0, pRect, size);
					api.RECT _rect = (api.RECT)System.Runtime.InteropServices.Marshal.PtrToStructure(pRect, typeof(api.RECT));
					System.Runtime.InteropServices.Marshal.FreeHGlobal(pRect);
					screenWidth = _rect.Right;
					screenHeight = _rect.Bottom;*/
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
					screenWidth = Screen.PrimaryScreen.Bounds.Width;
					screenHeight = Screen.PrimaryScreen.Bounds.Height;
				}

				// フォームが表示されている時は、手動でコンテキストメニューの位置を決める
				notifyIconMenu.Left = (notifyIconMenuLocation.X + notifyIconMenu.Width <= screenWidth) ? notifyIconMenuLocation.X : screenWidth - notifyIconMenu.Width;
				notifyIconMenu.Top = (notifyIconMenuLocation.Y + notifyIconMenu.Height <= screenHeight) ? notifyIconMenuLocation.Y : screenHeight - notifyIconMenu.Height;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// notifyIconMenu が閉じられようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIconMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
		{
			/*try
			{
				if ( this.TopMost )
				{
					this.TopMost = false;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}*/
		}

		/// <summary>
		/// notifyIconMenu のサブメニューをクリアする
		/// </summary>
		private void ClearNotifyIconMenu()
		{
			if ( notifyMenuButtons != null )
			{
				for ( int i = 0; i < notifyMenuButtons.Length; i++ )
				{
					if ( notifyMenuButtons[i] != null )
					{
						notifyMenuButtons[i].Dispose();
					}
				}

				notifyMenuButtons = null;
			}

			if ( notifyMenuTabPages != null )
			{
				for ( int i = 0; i < notifyMenuTabPages.Length; i++ )
				{
					notifyMenuTabPages[i].DropDownItems.Clear();
					notifyMenuTabPages[i].Dispose();
					//notifyIconMenu.Items.RemoveAt(2 + i);
				}

				notifyMenuTabPages = null;
			}
		}

		/// <summary>
		/// notifyMenuTabPage が開かれようとしている
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyMenuTabPage_Opening(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				ToolStripMenuItem notifyMenuTabPage = (ToolStripMenuItem)sender;

				notifyMenuTabPage.DropDownItems.Clear();

				int iTabPage = (int)notifyMenuTabPage.Tag;
				notifyMenuButtons = new ToolStripMenuItem[tabControl.TabPages[iTabPage].Controls.Count];

				// タブ内のボタンをサブメニューとして追加する
#if MOVABLE_BUTTON_BY_DRAGDROP
				List<int> ctrlIndexes = new List<int>();
				TabPage tabPage = tabControl.TabPages[iTabPage];
				for ( int i = 0; i < tabPage.Controls.Count; i++ )
				{
					if ( !(tabPage.Controls[i] is Button) )
						continue;
					ctrlIndexes.Add(tabPage.Controls[i].TabIndex);
				}

				int n = 0;
				for ( int i = 0; i < ctrlIndexes.Count; i++ )
				{
					Control control = tabPage.Controls[ctrlIndexes.IndexOf(i)];
#else
				int n = 0;
				foreach ( Control control in tabControl.TabPages[iTabPage].Controls )
				{
					if ( !(control is Button) )
						continue;
#endif

					int iButton = (int)control.Tag;
					string appToolTip = toolTip.GetToolTip(control);
					bool newApp;
					string[] appToolTips = GetButtonTextFromToolTip(appToolTip, out newApp, true).Split('\t');

					notifyMenuButtons[n] = new ToolStripMenuItem();
					notifyMenuButtons[n].Name = appToolTips[(int)ccMushroom.tt.buttonText];
					notifyMenuButtons[n].Text = appToolTips[(int)ccMushroom.tt.buttonText];
					notifyMenuButtons[n].Tag = iButton;

					//contextMenuButtons[n].ToolTipText = appToolTip;
					StringBuilder toolTipText = new StringBuilder();
					if ( appToolTips[(int)tt.comment].Length != 0 )
					{
						toolTipText.Append(appToolTips[(int)tt.comment]);
					}
					if ( appToolTips[(int)tt.appEnviron].Length != 0 )
					{
						toolTipText.Append(toolTipText.Length == 0 ? "" : "\r\n");
						toolTipText.Append(appToolTips[(int)tt.appEnviron]);
					}
					notifyMenuButtons[n].ToolTipText = toolTipText.ToString();

					if ( appIcons[iButton] != null )
					{
						if ( smallApplicationIcon )
						{
							notifyMenuButtons[n].Image = appIcons[iButton].ToBitmap();
						}
						else
						{
							try
							{
								//notifyMenuButtons[n].Image = ccf.GetIcon(xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton][TAG_APP_NAME].InnerText, true).ToBitmap();
								XmlNode application = xmlCcConfiguration.DocumentElement.SelectNodes(TAG_APPLICATION)[iButton];
								string appIconFile = (application[TAG_ICON_FILE] == null) ? application[TAG_APP_NAME].InnerText : application[TAG_ICON_FILE].InnerText;
								Icon icon = ccf.GetIcon(appIconFile, true);
								if ( icon != null )
								{
									notifyMenuButtons[n].Image = icon.ToBitmap();
									api.DestroyIcon(icon.Handle);
								}
							}
							catch ( Exception exp )
							{
								Debug.WriteLine(exp.Message);
							}
						}
					}

					notifyMenuButtons[n].Click += new EventHandler(this.notifyMenuButton_Click);
					n++;
				}

				notifyMenuTabPage.DropDownItems.AddRange(notifyMenuButtons);
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
		/// [通知アイコンのアプリケーションメニュー] がクリックされた
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyMenuButton_Click(object sender, System.EventArgs e)
		{
			ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
			ProcessStart((int)toolStripMenuItem.Tag);

			try
			{
				if ( resumeLatestNotifyMenuItem )
				{
					// 最後に選択された notifyIconMenu のアイテム名
					api.WritePrivateProfileString(RESUME_SECTION, KEY_LATEST_NOTIFY_MENU_ITEM, toolStripMenuItem.OwnerItem.Text, ccMushroomIniFileName);
				}
				else
				{
					api.WritePrivateProfileString(RESUME_SECTION, KEY_LATEST_NOTIFY_MENU_ITEM, null, ccMushroomIniFileName);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
		#endregion

		#region タスクバーのサムネイル関連
		/// <summary>
		/// 全てのタブページをサムネイルに追加する
		/// </summary>
		void AddThumbnailAllTabPages(bool sequential)
		{
#if FOR_WINDOWS7
			if ( !enabledTaskbarThumbnail )
				return;

			int selectedIndex = tabControl.SelectedIndex;
			if ( selectedIndex == -1 )
				return;

			try
			{
				nowLoading = true;
				SplashForm splash = new SplashForm(this);
				splash.Show();
				Application.DoEvents();

				if ( sequential )
				{
					foreach ( TabPage tabPage in tabControl.TabPages )
					{
						ShowProgressMessage("サムネイル作成中...", tabPage.Text);

						AddThumbnailTabPage(tabPage);
					}

					tabControl.SelectedIndex = selectedIndex;
				}
				else
				{
					// 現在選択中のタブページの次から順番にサムネイルを追加する
					for ( int i = 0, j = selectedIndex; i < appTabPages.Length; i++ )
					{
						if ( appTabPages.Length <= ++j ) { j = 0; }
						ShowProgressMessage("サムネイル作成中...", appTabPages[j].Text);
						TabbedThumbnail preview = AddThumbnailTabPage(appTabPages[j]);

						if ( (selectedIndex == appTabPages.Length - 1) || (selectedIndex < j) )
							continue;

						TabbedThumbnail insertBeforePreview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(appTabPages[selectedIndex + 1]);
						if ( insertBeforePreview != null )
						{
							TaskbarManager.Instance.TabbedThumbnail.SetTabOrder(preview, insertBeforePreview);
						}
					}
				}
	
				tabControl.Select();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
			finally
			{
				SplashForm.CloseSplash();
				nowLoading = false;
			}
#endif
		}

#if FOR_WINDOWS7
		/// <summary>
		/// タブページをサムネイルに追加する
		/// </summary>
		/// <param name="tabPage"></param>
		TabbedThumbnail AddThumbnailTabPage(TabPage tabPage)
		{
			if ( !enabledTaskbarThumbnail )
				return null;

			TabbedThumbnail preview = null;

			try
			{
				// Add a new preview
				preview = new TabbedThumbnail(this.Handle, tabPage.Handle);

				/*string tabIconFileName = Application.StartupPath + "\\" + iconsFolder + "\\" + "tab" + tabPage.Text + ".ico";
				if ( File.Exists(tabIconFileName) )
				{
					Icon icon = new Icon(tabIconFileName);
					preview.SetWindowIcon(icon);
				}*/
				Image image = appTabIcons.Images[tabPage.Text];
				if ( image != null )
				{
					Bitmap bmp = new Bitmap(image);
					preview.SetWindowIcon(Icon.FromHandle(bmp.GetHicon()));
					bmp.Dispose();
				}

				// Event handlers for this preview
				preview.TabbedThumbnailActivated += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailActivated);
				preview.TabbedThumbnailClosed += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailClosed);
				preview.TabbedThumbnailMaximized += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMaximized);
				preview.TabbedThumbnailMinimized += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMinimized);
				//preview.TabbedThumbnailBitmapRequested += new EventHandler<TabbedThumbnailBitmapRequestedEventArgs>(preview_TabbedThumbnailBitmapRequested);
				preview.TabbedThumbnailRestored += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailRestored);
				preview.TabbedThumbnailMoved += new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMoved);

				TaskbarManager.Instance.TabbedThumbnail.AddThumbnailPreview(preview);

				// Select the tab in the application UI as well as taskbar tabbed thumbnail list
				tabControl.SelectedTab = tabPage;
				//TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(tabControl.SelectedTab);

#if _DEBUG
				IntPtr hWnd;
				hWnd = preview.TaskbarWindow.TabbedThumbnailProxyWindow.Handle;
				hWnd = preview.TaskbarWindow.TabbedThumbnail.WindowHandle;	// = tabPage.Handle
				hWnd = preview.TaskbarWindow.WindowToTellTaskbarAbout;		// = TabbedThumbnailProxyWindow.Handle
				if ( hWnd != IntPtr.Zero )
				{
					IntPtr sysMenuHandle = api.GetSystemMenu(hWnd, false);
					api.EnableMenuItem(sysMenuHandle, api.SC_CLOSE, api.MF_GRAYED);
				}
#endif
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}

			return preview;
		}
#endif

		/// <summary>
		/// 全てのサムネイルを削除する
		/// </summary>
		void RemoveAllThumbnail()
		{
#if FOR_WINDOWS7
			foreach ( TabPage tabPage in tabControl.TabPages )
			{
				RemoveThumbnail(tabPage);
			}
#endif
		}

		/// <summary>
		/// サムネイルを削除する
		/// </summary>
		/// <param name="tabPage"></param>
		void RemoveThumbnail(TabPage tabPage)
		{
#if FOR_WINDOWS7
			try
			{
				TaskbarManager.Instance.TabbedThumbnail.RemoveThumbnailPreview(tabPage);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif
		}

#if FOR_WINDOWS7
		/// <summary>
		/// preview_TabbedThumbnailActivated
		/// この関数は何故か２回（ほぼ同時に？）呼ばれる
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailActivated(object sender, TabbedThumbnailEventArgs e)
		{
			try
			{
				Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + MethodBase.GetCurrentMethod().Name + ":" + "SelectedTab:" + tabControl.SelectedTab.Text);

				// Also activate our parent form (incase we are minimized, this will restore it)
				if ( this.WindowState == FormWindowState.Minimized )
				{
					this.WindowState = FormWindowState.Normal;
				}

				// User selected a tab via the thumbnail preview
				// Select the corresponding control in our app
				foreach ( TabPage page in tabControl.TabPages )
				{
					if ( page.Handle != e.WindowHandle )
						continue;

					if ( e.TabbedThumbnail.Title != tabControl.SelectedTab.Text )
					{
						Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + MethodBase.GetCurrentMethod().Name + ":" + "TabbedThumbnail:" + e.TabbedThumbnail.Title);
						// Select the tab in the application UI as well as taskbar tabbed thumbnail list
						tabControl.SelectedTab = page;
						TaskbarManager.Instance.TabbedThumbnail.SetActiveTab(page);
						api.SetForegroundWindow(this.Handle);
						//SetForceForegroundWindow(this.Handle);
						tabControl.SelectedTab.Select();
					}
					break;
				}

				ResetOpaqueForm();

				Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + MethodBase.GetCurrentMethod().Name + ":" + "timerRecoverThumbnail is " + (timerRecoverThumbnail == null? "null":"not null"));
				if ( timerRecoverThumbnail == null )
				{
					// タイマで遅延させて最前面にする
					timerRecoverThumbnail = new System.Windows.Forms.Timer();
					timerRecoverThumbnail.Tag = new object[] { TIMERID_SET_FOREGROUND_WINDOW };
					timerRecoverThumbnail.Interval = setForegroundWindowDelayTime;
					timerRecoverThumbnail.Tick += new EventHandler(timerRecoverThumbnail_Tick);
					timerRecoverThumbnail.Start();
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// 指定されたハンドルを持つウィンドウを強制的にアクティブにします。
		/// </summary>
		/// <param name="targetHandle">対象となるウィンドウハンドルオブジェクト</param>
		protected void SetForceForegroundWindow(IntPtr targetHandle)
		{
#if false
			api.SetForegroundWindow(targetHandle);
#else
#if true
			uint nTargetID, nForegroundID;
			uint nullProcessId = 0;
			byte[] sp_time = new byte[sizeof(UInt32)];

			// フォアグラウンドウィンドウを作成したスレッドのIDを取得
			nForegroundID = GetWindowThreadProcessId(api.GetForegroundWindow(), out nullProcessId);
			// 目的のウィンドウを作成したスレッドのIDを取得
			nTargetID = GetWindowThreadProcessId(targetHandle, out nullProcessId);

			// スレッドのインプット状態を結び付ける
			AttachThreadInput(nTargetID, nForegroundID, true);  // TRUE で結び付け

			// 現在の設定を sp_time に保存
			api.SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, sp_time, 0);
			// ウィンドウの切り替え時間を 0ms にする
			SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, IntPtr.Zero/*(LPVOID)0*/, 0);

			//api.SetWindowPos(targetHandle, (IntPtr)api.HWND_TOPMOST, 0, 0, 0, 0, api.SWP_NOMOVE | api.SWP_NOSIZE | /*SWP_SHOWWINDOW*/0x40);
			//api.SetWindowPos(targetHandle, (IntPtr)api.HWND_NOTOPMOST, 0, 0, 0, 0, api.SWP_NOMOVE | api.SWP_NOSIZE | 0x40);

			// ウィンドウをフォアグラウンドに持ってくる
			api.SetForegroundWindow(targetHandle);

			// 設定を元に戻す
			api.SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, sp_time, 0);

			// スレッドのインプット状態を切り離す
			AttachThreadInput(nTargetID, nForegroundID, false);  // FALSE で切り離し
#else
			uint nullProcessId = 0;

			// ターゲットとなるハンドルのスレッドIDを取得.
			uint targetThreadId = GetWindowThreadProcessId(targetHandle, out nullProcessId);
			// 現在アクティブとなっているウィンドウのスレッドIDを取得.
			uint currentActiveThreadId = GetWindowThreadProcessId(GetForegroundWindow(), out nullProcessId);

			////////////////////////////////////////////////
			//
			// アクティブ処理
			//
			//
			SetForegroundWindow(targetHandle);
			if ( targetThreadId == currentActiveThreadId )
			{
				//
				// 現在アクティブなのが自分の場合は前面に持ってくる。
				//
				BringWindowToTop(targetHandle);
			}
			else
			{
				//
				// 別のプロセスがアクティブな場合は、そのプロセスにアタッチし、入力を奪う.
				//
				AttachThreadInput(targetThreadId, currentActiveThreadId, true);
				try
				{
					//
					// 自分を前面に持ってくる。
					//
					BringWindowToTop(targetHandle);
				}
				finally
				{
					//
					// アタッチを解除.
					//
					AttachThreadInput(targetThreadId, currentActiveThreadId, false);
				}
			}
#endif
#endif
		}

		/// <summary>
		/// preview_TabbedThumbnailClosed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailClosed(object sender, TabbedThumbnailEventArgs e)
		{
			try
			{
				TabPage pageClosed = null;

				// Find the tabpage that was "closed" by the user (via the taskbar tabbed thumbnail)
				foreach ( TabPage page in tabControl.TabPages )
				{
					if ( page.Handle == e.WindowHandle )
					{
						pageClosed = page;
						break;
					}
				}

				if ( pageClosed != null )
				{
					timerRecoverThumbnail = new System.Windows.Forms.Timer();
					timerRecoverThumbnail.Interval = 100;
					timerRecoverThumbnail.Tag = new object[] { TIMERID_TABPAGE_CLOSED, pageClosed, previousSelectedPage };
					timerRecoverThumbnail.Tick += new EventHandler(timerRecoverThumbnail_Tick);
					timerRecoverThumbnail.Start();
				}

#if false
			if ( pageClosed != null )
			{
				// Remove the event handlers
				WebBrowser wb = pageClosed.Controls[0] as WebBrowser;

				if ( wb != null )
				{
					wb.DocumentTitleChanged -= new EventHandler(wb_DocumentTitleChanged);
					//wb.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
					wb.Navigated -= new WebBrowserNavigatedEventHandler(wb_Navigated);
					wb.ProgressChanged -= new WebBrowserProgressChangedEventHandler(wb_ProgressChanged);
					wb.Document.Window.Scroll -= new HtmlElementEventHandler(Window_Scroll);

					wb.Dispose();
				}
				else
				{
					// It's most likely a RichTextBox.

					RichTextBox rtbText = pageClosed.Controls[0] as RichTextBox;

					if ( rtbText != null )
					{
						rtbText.KeyDown -= new KeyEventHandler(rtbText_KeyDown);
						rtbText.MouseMove -= new MouseEventHandler(rtbText_MouseMove);
						rtbText.KeyUp -= new KeyEventHandler(rtbText_KeyUp);
					}

					rtbText.Dispose();
				}

				// Finally, remove the tab from our UI
				if ( pageClosed != null )
					tabControl1.TabPages.Remove(pageClosed);

				// Dispose the tab
				pageClosed.Dispose();

				if ( tabControl1.TabPages.Count > 0 )
					button2.Enabled = true;
				else
					button2.Enabled = false;
			}
#endif

				// Remove the event handlers from the tab preview
				e.TabbedThumbnail.TabbedThumbnailActivated -= new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailActivated);
				e.TabbedThumbnail.TabbedThumbnailClosed -= new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailClosed);
				e.TabbedThumbnail.TabbedThumbnailMaximized -= new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMaximized);
				e.TabbedThumbnail.TabbedThumbnailMinimized -= new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMinimized);
				//e.TabbedThumbnail.TabbedThumbnailBitmapRequested -= new EventHandler<TabbedThumbnailBitmapRequestedEventArgs>(preview_TabbedThumbnailBitmapRequested);
				e.TabbedThumbnail.TabbedThumbnailRestored -= new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailRestored);
				e.TabbedThumbnail.TabbedThumbnailMoved -= new EventHandler<TabbedThumbnailEventArgs>(preview_TabbedThumbnailMoved);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// preview_TabbedThumbnailMinimized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailMinimized(object sender, TabbedThumbnailEventArgs e)
		{
			try
			{
				// User clicked on the minimize button on the thumbnail's context menu
				// Minimize the app
				this.WindowState = FormWindowState.Minimized;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// preview_TabbedThumbnailMaximized
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailMaximized(object sender, TabbedThumbnailEventArgs e)
		{
			try
			{
				// User clicked on the maximize button on the thumbnail's context menu
				// Maximize the app
				this.WindowState = FormWindowState.Maximized;
				Application.DoEvents();

				// If there is a selected tab, take it's screenshot
				// invalidate the tab's thumbnail
				// update the "preview" object with the new thumbnail
				if ( tabControl.Size != Size.Empty && tabControl.TabPages.Count > 0 && tabControl.SelectedTab != null )
				{
					Debug.WriteLine(MethodBase.GetCurrentMethod().Name + ":" + tabControl.SelectedTab.Text);
					UpdatePreviewBitmap(tabControl.SelectedTab);
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// preview_TabbedThumbnailBitmapRequested
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailBitmapRequested(object sender, TabbedThumbnailBitmapRequestedEventArgs e)
		{
			try
			{
				TabbedThumbnail preview = (TabbedThumbnail)sender;
				if ( preview != null )
				{
					foreach ( TabPage tabPage in appTabPages )
					{
						if ( tabPage.Text == preview.Title )
						{
							UpdatePreviewBitmap(tabPage);
							break;
						}
					}
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// preview_TabbedThumbnailRestored
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailRestored(object sender, TabbedThumbnailEventArgs e)
		{
			try
			{
				// User clicked on the restore button on the thumbnail's context menu
				// Restore the app
				this.WindowState = FormWindowState.Normal;
#if false
				if ( this.WindowState == FormWindowState.Minimized )
				{
					//this.Show();
					this.Visible = true;
					this.WindowState = FormWindowState.Normal;

					this.Activate();

					if ( !alwaysInTasktray )
					{
						notifyIcon.Visible = false;
					}
				}
				else
				{
					if ( alwaysInTasktray )
					{
						if ( this.Visible = !this.Visible )
						{
							this.Activate();
							DisTransparentWithClickThrough(true);
						}
					}
					else
					{
						this.WindowState = FormWindowState.Minimized;
					}
				}
#endif
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// preview_TabbedThumbnailMoved
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void preview_TabbedThumbnailMoved(object sender, TabbedThumbnailEventArgs e)
		{
			try
			{
				// User clicked on the move button on the thumbnail's context menu
				// Move the app
				RemoveAllThumbnail();
				AddThumbnailAllTabPages(false);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		///  timerRecoverThumbnail のイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerRecoverThumbnail_Tick(object sender, EventArgs e)
		{
			try
			{
				timerRecoverThumbnail.Stop();

				int timerID = (int)((object[])timerRecoverThumbnail.Tag)[0];

				// 閉じられたサムネイルを追加する
				if ( timerID == TIMERID_TABPAGE_CLOSED )
				{
					object[] objects = (object[])timerRecoverThumbnail.Tag;

					TabbedThumbnail preview = AddThumbnailTabPage((TabPage)objects[0 + 1]);

					timerRecoverThumbnail.Tag = new object[] { TIMERID_ARRANGEMENT_THUMBNAIL, preview, objects[1 + 1] };
					timerRecoverThumbnail.Interval = 100;
					timerRecoverThumbnail.Start();
				}
				// 追加されたサムネイルを整列する
				else if ( timerID == TIMERID_ARRANGEMENT_THUMBNAIL )
				{
					object[] objects = (object[])timerRecoverThumbnail.Tag;
					TabbedThumbnail preview = (TabbedThumbnail)objects[0 + 1];
					TabPage _previousSelectedPage = (TabPage)objects[1 + 1];

					int i;
					for ( i = 0; (i < appTabPages.Length) && (appTabPages[i].Text != preview.Title); i++ ) ;
					if ( i != appTabPages.Length - 1 )
					{
						TabbedThumbnail insertBeforePreview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(appTabPages[i + 1]);
						TaskbarManager.Instance.TabbedThumbnail.SetTabOrder(preview, insertBeforePreview);
					}

					/*if ( _previousSelectedPage != null )
					{
						tabControl.SelectedTab = _previousSelectedPage;	// サムネイルを閉じられた時の選択を復元する
						tabControl.SelectedTab.Select();
					}*/

					timerRecoverThumbnail.Dispose();
					timerRecoverThumbnail = null;
				}
				// ウィンドウを最前面にする
				else if ( timerID == TIMERID_SET_FOREGROUND_WINDOW )
				{
					Debug.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + MethodBase.GetCurrentMethod().Name);
					//api.SetForegroundWindow(this.Handle);
					SetForceForegroundWindow(this.Handle);

					int foregroundWindowIndicatorRemoveTime = (int)api.GetPrivateProfileInt(SETTINGS_SECTION, "ForegroundWindowIndicatorRemoveTime", 0, ccMushroomIniFileName);
					if ( foregroundWindowIndicatorRemoveTime != 0 )
					{
						toolStripVersion.Text += "*";

						timerRecoverThumbnail.Tag = new object[] { TIMERID_REMOVE_FOREGROUND_WINDOW_INDICATOR };
						timerRecoverThumbnail.Interval = foregroundWindowIndicatorRemoveTime;
						timerRecoverThumbnail.Start();
					}
					else
					{
						timerRecoverThumbnail.Dispose();
						timerRecoverThumbnail = null;
					}
				}
				// 最前面になった旨のインジケータを削除する
				else if ( timerID == TIMERID_REMOVE_FOREGROUND_WINDOW_INDICATOR )
				{
					if ( toolStripVersion.Text.EndsWith("*") )
					{
						toolStripVersion.Text = toolStripVersion.Text.Substring(0, toolStripVersion.Text.Length - 1);
					}

					timerRecoverThumbnail.Dispose();
					timerRecoverThumbnail = null;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// Helper method to update the thumbnail preview for a given tab page.
		/// </summary>
		/// <param name="tabPage"></param>
		private void UpdatePreviewBitmap(TabPage tabPage)
		{
			if ( tabPage != null )
			{
				TabbedThumbnail preview = TaskbarManager.Instance.TabbedThumbnail.GetThumbnailPreview(tabPage);

				if ( preview != null )
				{
					Bitmap bitmap = TabbedThumbnailScreenCapture.GrabWindowBitmap(tabPage.Handle, /*tabControl.SelectedTab.Size*/tabPage.Size);
					preview.SetImage(bitmap);

					if ( bitmap != null )
					{
						bitmap.Dispose();
						bitmap = null;
					}
				}
			}
		}
#endif
		#endregion
	}
}