using System;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

public delegate int EnumWindowCallBack(IntPtr hwnd, uint lParam);

namespace CommonFunctions
{
	/// <summary>
	/// common の概要の説明です。
	/// </summary>
	public class common
	{
		public enum platform
		{
			unknown = 0x00000000,
			win32s = 0x00000001, win95 = 0x00000002, win98 = 0x00000004, winme = 0x00000008,
			winnt3 = 0x00000010, winnt4 = 0x00000020, win2000 = 0x00000040, winxp = 0x00000080,
			winvista = 0x00000100, win7 = 0x00000200
		};

		/// <summary>
		/// プラットフォームの取得
		/// </summary>
		public static platform GetOsPlatform()
		{
			System.OperatingSystem os = System.Environment.OSVersion;

			switch ( os.Platform )
			{
				case PlatformID.Win32Windows:
					Debug.WriteLine("OSは Windows 95 以降です.");
					if ( os.Version.Major >= 4 )
					{
						switch ( os.Version.Minor )
						{
							case 0:
								Debug.WriteLine("OSは Windows 95 です.");
								return platform.win95;
							case 10:
								Debug.WriteLine("OSは Windows 98 です.");
								return platform.win98;
							case 90:
								Debug.WriteLine("OSは Windows Me です.");
								return platform.winme;
						}
					}
					break;

				case PlatformID.Win32NT:
					Debug.WriteLine("OSは Windows NT 以降です.");
					switch ( os.Version.Major )
					{
						case 3:
							switch ( os.Version.Minor )
							{
								case 0:
									Debug.WriteLine("OSは Windows NT 3 です.");
									return platform.winnt3;
								case 1:
									Debug.WriteLine("OSは Windows NT 3.1 です.");
									return platform.winnt3;
								case 5:
									Debug.WriteLine("OSは Windows NT 3.5 です.");
									return platform.winnt3;
								case 51:
									Debug.WriteLine("OSは Windows NT 3.51 です.");
									return platform.winnt3;
							}
							break;
						case 4:
							if ( os.Version.Minor == 0 )
							{
								Debug.WriteLine("OSは Windows NT 4.0 です.");
								return platform.winnt4;
							}
							break;
						case 5:
							if ( os.Version.Minor == 0 )
							{
								Debug.WriteLine("OSは Windows 2000 です.");
								return platform.win2000;
							}
							else if ( os.Version.Minor == 1 )
							{
								Debug.WriteLine("OSは Windows XP です.");
								return platform.winxp;
							}
							break;
						case 6:
							if ( os.Version.Minor == 0 )
							{
								Debug.WriteLine("OSは Windows Vista です.");
								return platform.winvista;
							}
							else if ( os.Version.Minor == 1 )
							{
								Debug.WriteLine("OSは Windows 7 です.");
								return platform.win7;
							}
							break;
					}
					break;

				case PlatformID.Win32S:
					Debug.WriteLine("OSは Win32s です.");
					return platform.win32s;

				default:
					Debug.WriteLine("（不明）");
					break;
			}

			return platform.unknown;
		}

		/// <summary>
		/// 文字列中に環境変数があれば変換する
		/// </summary>
		public static string CheckEnvironmentVariable(string source)
		{
			int startEnvironment, endEnvironment;

			if ( ((startEnvironment = source.IndexOf("%")) != -1) &&
				((endEnvironment = source.IndexOf("%", startEnvironment + 1)) != -1) )
			{
				string environment = source.Substring(startEnvironment + 1, endEnvironment - startEnvironment - 1);
				string variable = System.Environment.GetEnvironmentVariable(environment);

				if ( (source.IndexOf("%", endEnvironment + 1) == -1) && (source.Length != endEnvironment + 1) )
				{
					if ( variable.EndsWith("\\") )
					{
						if ( source[endEnvironment + 1] == '\\' )
							variable = variable.Substring(0, variable.Length - 1);
					}
					else
					{
						if ( source[endEnvironment + 1] != '\\' )
							variable += "\\";
					}
				}

				source = source.Replace("%" + environment + "%", variable);

				source = CheckEnvironmentVariable(source);
			}

			return source;
		}

		/// <summary>
		/// パスワードを暗号化する。
		/// 文字列を一字ずつ１６進数にして、その上位と下位を入れ替える。
		/// 偶数番目の上位と下位の間にダミー キャラクタを挿入する。
		/// ABCDEFG -> 1x4243x4445x4647x4
		/// </summary>
		public static string EncodePassword(string decodePassword)
		{
			byte[] bytePass = Encoding.Default.GetBytes(decodePassword);
			char[] chPass = new char[decodePassword.Length * 3];
			Random random = new Random();
			int dest = 0;

			for ( int i = 0; i < decodePassword.Length; i++ )
			{
				string hex = bytePass[i].ToString("X");
				chPass[dest++] = hex[1];
				if ( i % 2 == 0 )
					chPass[dest++] = random.Next(0, 9).ToString()[0];	// ダミー キャラクタ
				chPass[dest++] = hex[0];
			}
			//return new string(chPass);
			return new string(chPass, 0, dest);		// 2007/09/04 末尾に余分な '\0' が入るのを防ぐ為
		}

		/// <summary>
		/// 暗号化されたパスワードを復元する。
		/// 偶数番目の上位と下位の間のダミー キャラクタは読み飛ばす。
		/// 1x4243x4445x4647x4 -> ABCDEFG
		/// </summary>
		public static string DecodePassword(string encodePassword)
		{
			byte[] bytePass = new byte[encodePassword.Length];
			char[] chPass = new char[2];
			int i, src = 0;

			for ( i = 0; src < encodePassword.Length; i++ )
			{
				chPass[1] = encodePassword[src++];
				if ( i % 2 == 0 )
					src++;
				chPass[0] = encodePassword[src++];
				bytePass[i] = Byte.Parse(new string(chPass), System.Globalization.NumberStyles.HexNumber);
			}
			return new string(Encoding.Default.GetChars(bytePass, 0, i));
		}

	}

	/// <summary>
	/// アプリ名.exe.config 読み込み/書き込みクラス
	/// </summary>
	public class AppConfig
	{
		private string fileName;
		public XmlDocument xmlConfig = new XmlDocument();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="appName">アプリ名称</param>
		public AppConfig(string appName)
		{
			if ( appName.IndexOf(".config") == -1 )
			{
				fileName = Application.StartupPath + "\\" + appName + ((appName.IndexOf(".exe") == -1) ? ".exe" : "") + ".config";
			}
			else
			{
				fileName = appName;
			}

			xmlConfig.Load(fileName);
		}

		/// <summary>
		/// config ファイルを読み込みます
		/// </summary>
		/// <param name="cfgKey">キー名称</param>
		/// <returns>読み込んだ値</returns>
		public string GetValue(string cfgKey)
		{
#if (DEBUG)
			//string filename = Path.GetFileName(fileName);
#endif
			string cfgValue = null;

			//if ( Path.GetFileName(fileName) == Application.ProductName + ".exe.config" )
			//{
			//	//AppSettingsReader configurationAppSettings = new AppSettingsReader();
			//	//cfgValue = (string)(configurationAppSettings.GetValue(cfgKey, typeof(string)));
			//	cfgValue = ConfigurationManager.AppSettings[cfgKey];
			//}
			//else
			//{
				XmlNode cfgNode = xmlConfig.SelectSingleNode("/configuration/appSettings/add[@key='" + cfgKey + "']");
				if ( cfgNode != null )
				{
					cfgValue = cfgNode.Attributes["value"].InnerText;
				}
			//}

			return cfgValue;
		}

		/// <summary>
		/// config ファイルに書き込みます
		/// </summary>
		/// <param name="cfgKey">キー名称</param>
		/// <param name="cfgValue">セットする値</param>
		public void SetValue(string cfgKey, string cfgValue)
		{
			XmlNode cfgNode = xmlConfig.SelectSingleNode("/configuration/appSettings/add[@key='" + cfgKey + "']");
			if ( cfgNode != null )
			{
				cfgNode.Attributes.GetNamedItem("value").Value = cfgValue;
			}
			else
			{
				cfgNode = xmlConfig.CreateElement("add");
				XmlAttribute attrKey = xmlConfig.CreateAttribute("key");
				attrKey.Value = cfgKey;
				XmlAttribute attrValue = xmlConfig.CreateAttribute("value");
				attrValue.Value = cfgValue;
				cfgNode.Attributes.Append(attrKey);
				cfgNode.Attributes.Append(attrValue);
				xmlConfig.SelectSingleNode("/configuration/appSettings").AppendChild(cfgNode);
			}

			Save();
		}

		/// <summary>
		/// config ファイルのキーを削除する
		/// </summary>
		/// <param name="cfgKey">キー名称</param>
		public void Remove(string cfgKey)
		{
			XmlNode cfgNode = xmlConfig.SelectSingleNode("/configuration/appSettings/add[@key='" + cfgKey + "']");
			if ( cfgNode != null )
			{
				xmlConfig.SelectSingleNode("/configuration/appSettings").RemoveChild(cfgNode);
				Save();
			}
		}

		/// <summary>
		/// 指定されたノード名以外のノードを取得する
		/// </summary>
		/// <param name="nodeName"></param>
		/// <returns></returns>
		public XmlNodeList GetOtherNodes(string nodeName)
		{
			string xpath = "/configuration/*[local-name()!='" + nodeName + "']";
			XmlNodeList list = xmlConfig.SelectNodes(xpath);
			return list;
		}

		/// <summary>
		/// config ファイルの保存
		/// </summary>
		public void Save()
		{
			xmlConfig.Save(fileName);
		}
	}

	/// <summary>
	/// api の概要の説明です。
	/// </summary>
	public class api
	{
		[DllImport("KERNEL32.DLL")]
		public static extern uint GetPrivateProfileSectionNames(byte[] lpszReturnBuffer, uint nSize, string lpFileName);

		[DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileSectionA")]
		public static extern uint GetPrivateProfileSection(string lpAppName, byte[] lpRetunedString, uint nSize, string lpFileName);

		[DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileSectionA")]
		public static extern uint WritePrivateProfileSection(string lpAppName, byte[] lpString, string lpFileName);

		[DllImport("KERNEL32.DLL")]
		public static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, uint nSize, string lpFileName);

		[DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringA")]
		public static extern uint GetPrivateProfileString/*ByByteArray*/(string lpAppName, string lpKeyName, string lpDefault, byte[] lpReturnedString, uint nSize, string lpFileName);

		[DllImport("KERNEL32.DLL")]
		public static extern uint GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

		[DllImport("KERNEL32.DLL")]
		public static extern uint WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string strClassName, string strWindowTitle);

		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hwnd);

		[DllImport("user32.Dll")]
		public static extern IntPtr GetActiveWindow();

		[DllImport("user32.Dll")]
		public static extern IntPtr SetActiveWindow(IntPtr hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("KERNEL32.DLL")]
		public static extern uint GetTempFileName(string lpPathName, string lpPrefixString, uint uUnique, StringBuilder lpTempFileName);

		[DllImport("KERNEL32.DLL")]
		public static extern uint lstrlenA(byte[] lPString);

		[DllImport("KERNEL32.DLL")]
		public static extern uint lstrlenA(string lPString);

		public const int SW_HIDE = 0;
		public const int SW_SHOWNORMAL = 1;
		public const int SW_NORMAL = 1;
		public const int SW_SHOWMINIMIZED = 2;
		public const int SW_SHOWMAXIMIZED = 3;
		public const int SW_MAXIMIZE = 3;
		public const int SW_SHOWNOACTIVATE = 4;
		public const int SW_SHOW = 5;
		public const int SW_MINIMIZE = 6;
		public const int SW_SHOWMINNOACTIVE = 7;
		public const int SW_SHOWNA = 8;
		public const int SW_RESTORE = 9;
		public const int SW_SHOWDEFAULT = 10;
		public const int SW_FORCEMINIMIZE = 11;
		public const int SW_MAX = 11;

		public const uint SWP_NOSIZE = 0x0001;
		public const uint SWP_NOMOVE = 0x0002;
		public const uint SWP_NOZORDER = 0x0004;
		public const uint SWP_NOACTIVATE = 0x0010;
		public const uint SWP_SHOWWINDOW = 0x0040;

		public const uint HWND_TOP = 0;
		public const uint HWND_BOTTOM = 1;
		public const int HWND_TOPMOST = -1;
		public const int HWND_NOTOPMOST = -2;

		public const uint FILE_ATTRIBUTE_READONLY = 0x00000001;
		public const uint FILE_ATTRIBUTE_HIDDEN = 0x00000002;
		public const uint FILE_ATTRIBUTE_SYSTEM = 0x00000004;
		public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
		public const uint FILE_ATTRIBUTE_ARCHIVE = 0x00000020;
		public const uint FILE_ATTRIBUTE_DEVICE = 0x00000040;
		public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
		public const uint FILE_ATTRIBUTE_TEMPORARY = 0x00000100;
		public const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200;
		public const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
		public const uint FILE_ATTRIBUTE_COMPRESSED = 0x00000800;
		public const uint FILE_ATTRIBUTE_OFFLINE = 0x00001000;
		public const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000;
		public const uint FILE_ATTRIBUTE_ENCRYPTED = 0x00004000;

		public const UInt32 SC_SIZE = 0xF000;
		public const UInt32 SC_MAXIMIZE = 0xF030;
		public const UInt32 SC_CLOSE = 0xF060;
		public const UInt32 SC_RESTORE = 0xF120;

		public const UInt32 MF_BYCOMMAND = 0x00000000;
		public const UInt32 MF_BYPOSITION = 0x00000400;

		public const UInt32 MF_SEPARATOR = 0x00000800;

		public const UInt32 MF_ENABLED = 0x00000000;
		public const UInt32 MF_GRAYED = 0x00000001;

		public const UInt32 MF_UNCHECKED = 0x00000000;
		public const UInt32 MF_CHECKED = 0x00000008;

		public const UInt32 MF_STRING = 0x00000000;

		public const UInt32 MIIM_FTYPE = 0x00000100;
		public const UInt32 MIIM_ID = 0x00000002;
		public const UInt32 MIIM_STATE = 0x00000001;
		public const UInt32 MIIM_STRING = 0x00000040;

		public const UInt32 MFS_DEFAULT = 0x00001000;

		public const UInt32 WM_SYSCOMMAND = 0x112;

		public const uint WM_KEYDOWN = 0x0100;
		public const uint WM_KEYUP = 0x0101;

		public const uint VK_ESCAPE = 0x1B;

		public const uint WM_CLOSE = 0x0010;
		public const uint WM_HSCROLL = 0x114;

		public const uint LVM_SCROLL = 0x1014;

		public const uint SB_LINERIGHT = 1;
		public const uint SB_PAGERIGHT = 3;
		public const uint SB_LEFT = 6;
		public const uint SB_RIGHT = 7;

		public const uint WS_VISIBLE = 0x10000000;

		public const uint WM_GETTEXT = 0x000D;

		public const uint WM_USER = 0x0400;
		public const uint TTM_POP = (WM_USER + 28);

		[StructLayout(LayoutKind.Sequential)]
		public struct MENUITEMINFO
		{
			public uint cbSize;
			public uint fMask;
			public uint fType;
			public uint fState;
			public uint wID;
			public IntPtr hSubMenu;
			public IntPtr hbmpChecked;
			public IntPtr hbmpUnchecked;
			public IntPtr dwItemData;
			public string dwTypeData;
			public uint cch;
			public IntPtr hbmpItem;

			// return the size of the structure
			internal static uint sizeOf
			{
				get { return (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); }
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint FormatMessage(uint dwFlags,
			IntPtr lpSource,
			uint dwMessageId,
			uint dwLanguageId,
			StringBuilder lpBuffer,
			uint nsize,
			IntPtr Arguments);

		[DllImport("user32.dll")]
		public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32.dll")]
		public static extern bool InsertMenu(IntPtr hMenu, UInt32 wPosition, UInt32 wFlags, UInt32 wIDNewItem, string lpNewItem);

		[DllImport("user32.dll")]
		public static extern bool InsertMenuItem(IntPtr hMenu, uint uItem, bool fByPosition, [In] ref MENUITEMINFO lpmii);

		[DllImport("USER32.DLL", EntryPoint = "RemoveMenu")]
		public static extern UInt32 RemoveMenu(IntPtr hMenu, UInt32 nPosition, UInt32 wFlags);

		[DllImport("User32.dll", EntryPoint = "EnableMenuItem")]
		public static extern UInt32 EnableMenuItem(IntPtr hMenu, UInt32 uIDEnableItem, UInt32 uEnable);

		[DllImport("user32.dll")]
		public static extern UInt32 CheckMenuItem(IntPtr hMenu, UInt32 uIDCheckItem, UInt32 uCheck);

		[DllImport("user32.dll")]
		public static extern int GetMenuItemCount(IntPtr hMenu);

		[DllImport("user32.dll")]
		public static extern bool SetMenuItemBitmaps(IntPtr hMenu, UInt32 wPosition, UInt32 wFlags, IntPtr hBitmapUnchecked, IntPtr hBitmapChecked);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		public enum SHGFI : int
		{
			SmallIcon = 0x00000001,
			LargeIcon = 0x00000000,
			Icon = 0x00000100,
			DisplayName = 0x00000200,
			Typename = 0x00000400,
			SysIconIndex = 0x00004000,
			UseFileAttributes = 0x00000010
		}

		[DllImport("Shell32", CharSet = CharSet.Auto)]
		public static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, int nIcons);

		[DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
		public static extern int DestroyIcon(IntPtr hIcon);

		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleTitle(string lpConsoleTitle);

		[DllImport("kernel32.dll")]
		public static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		private const uint FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
		private const uint FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
		private const uint FORMAT_MESSAGE_FROM_STRING = 0x00000400;
		private const uint FORMAT_MESSAGE_FROM_HMODULE = 0x00000800;
		private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
		private const uint FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000;
		private const uint FORMAT_MESSAGE_MAX_WIDTH_MASK = 0x000000FF;

		public static void ShowFormatMessage(int dwMessageId, string message, string title)
		{
			StringBuilder sbFormatMessage = new StringBuilder(1024);
			uint cb;

			cb = FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM,
							   IntPtr.Zero,
							   (uint)dwMessageId,
							   0,
							   sbFormatMessage,
							   (uint)sbFormatMessage.Capacity,
							   IntPtr.Zero);

			string errorMessage = "[ERROR CODE:" + dwMessageId + "] ";

			if ( cb == 0 )
			{
				sbFormatMessage = new StringBuilder("\r\n");
			}

			errorMessage += sbFormatMessage;
			MessageBox.Show(message + "\r\n" + errorMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		//        HWND GetParent(
		//            HWND hWnd   // 子ウィンドウのハンドル
		//            );
		[DllImport("User32.Dll")]
		static extern int GetParent(
			IntPtr hWnd
			);

		//        int GetClassName(
		//            HWND hWnd,           // ウィンドウのハンドル
		//            LPTSTR lpClassName,  // クラス名
		//            int nMaxCount        // クラス名バッファのサイズ
		//            );
		//        Unicode：Windows NT/2000 は Unicode 版と ANSI 版を実装
		[DllImport("User32.Dll", CharSet = CharSet.Unicode)]
		public static extern int GetClassName(
			IntPtr hWnd,
			StringBuilder lpClassName,
			int nMaxCount
			);

		//        HWND GetDesktopWindow(VOID);        
		[DllImport("User32.Dll")]
		public static extern IntPtr GetDesktopWindow();

		//        int GetWindowText(
		//            HWND hWnd,        // ウィンドウまたはコントロールのハンドル
		//            LPTSTR lpString,  // テキストバッファ
		//            int nMaxCount     // コピーする最大文字数
		//            );
		//        Unicode：Windows NT/2000 は Unicode 版と ANSI 版を実装
		[DllImport("User32.Dll", CharSet = CharSet.Unicode)]
		public static extern int GetWindowText(
			IntPtr hWnd,
			StringBuilder s,
			int nMaxCount
			);

		//		HWND GetWindow(
		public const int GW_HWNDFIRST = 0;
		public const int GW_HWNDLAST = 1;
		public const int GW_HWNDNEXT = 2;
		public const int GW_HWNDPREV = 3;
		public const int GW_OWNER = 4;
		public const int GW_CHILD = 5;
		[DllImport("User32.Dll")]
		public static extern int GetWindow(
			IntPtr hWnd,
			uint uCmd
			);

		[DllImport("User32.Dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.Dll")]
		public static extern int EnumWindows(EnumWindowCallBack lpEnumFunc, uint lParam);

		public const int GWL_EXSTYLE = (-20);
		public const int GWL_STYLE = (-16);

		public const int WS_EX_TRANSPARENT = 0x00000020;
		public const int WS_EX_TOOLWINDOW = 0x00000080;
		public const int WS_EX_APPWINDOW = 0x00040000;

		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
		public static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern UInt32 SetWindowLong(IntPtr hWnd, int index, UInt32 unValue);

		//        BOOL IsWindowVisible(
		//            HWND hWnd   // ウィンドウのハンドル
		//            );
		[DllImport("User32.Dll")]
		public static extern bool/*int*/ IsWindowVisible(
			IntPtr hWnd
			);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
		public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		// This static method is required because legacy OSes do not support
		// GetWindowLongPtr 
		public static long GetWindowLongPtr(IntPtr hWnd, int nIndex)
		{
			if ( IntPtr.Size == 8 )
				return (long)GetWindowLongPtr64(hWnd, nIndex);
			else
				return (long)new IntPtr(GetWindowLong32(hWnd, nIndex));
		}

		public static bool IsTaskbarWindow(IntPtr hwnd)
		{
			long lExStyle;
			IntPtr hParent;
			bool taskbarWindow;

			lExStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
			hParent = (IntPtr)GetParent(hwnd);

			// if these 3 conditions are true, it probably has a button...
			taskbarWindow = (IsWindowVisible(hwnd)/*((IntPtr)IsWindowVisible(hwnd) != IntPtr.Zero)*/ &&
				((IntPtr)GetWindow(hwnd, GW_OWNER) == IntPtr.Zero) &&
				(hParent == IntPtr.Zero || hParent == GetDesktopWindow()));

			// ... but if it has the WS_EX_TOOLWINDOW style is set, no button will be displayed ...
			if ( (lExStyle & WS_EX_TOOLWINDOW) != 0 )
				taskbarWindow = false;

			// ... unless it also has the WS_EX_APPWINDOW style, which forces a taskbar button
			if ( (lExStyle & WS_EX_APPWINDOW) != 0 )
				taskbarWindow = true;

			return taskbarWindow;
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool LockWindowUpdate(IntPtr hwnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

		[DllImport("USER32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder lParam);

		[DllImport("user32.dll")]
		public static extern bool EnableScrollBar(IntPtr hWnd, uint wSBflags, uint wArrows);

		public enum SBFlags { SB_HORZ = 0, SB_VERT = 1, SB_CTL = 2, SB_BOTH = 3 };
		public enum SBArrows { ESB_ENABLE_BOTH = 0, ESB_DISABLE_BOTH = 3, ESB_DISABLE_LEFT = 1, ESB_DISABLE_RIGHT = 2, ESB_DISABLE_UP = 1, ESB_DISABLE_DOWN = 2, ESB_DISABLE_LTUP = 1, ESB_DISABLE_RTDN = 2 };

		public const uint WM_APP = 0x8000;

		public const uint WM_COPYDATA = 0x4a;
		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public int cbData;
			public IntPtr lpData;
		}

		[DllImport("user32", EntryPoint = "SendMessage")]
		public static extern int SendMessageCds(IntPtr hWndControl, uint msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

		public const int MOD_ALT = 0x0001;
		public const int MOD_CONTROL = 0x0002;
		public const int MOD_SHIFT = 0x0004;
		public const int MOD_WIN = 0x0008;
		public const int WM_HOTKEY = 0x0312;

		[DllImport("user32.dll")]		// 返り値:  成功 = 0以外,  失敗 = 0(既に他が登録済み)
		public extern static int RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, int KEY);

		[DllImport("user32.dll")]		// 返り値:  成功 = 0以外,  失敗 = 0
		public extern static int UnregisterHotKey(IntPtr HWnd, int ID);

		public const int MOVEFILE_REPLACE_EXISTING = 0x00000001;
		public const int MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004;

		[DllImport("kernel32.dll", SetLastError = true)]
		public extern static bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

		public enum INSTALLSTATE
		{
			INSTALLSTATE_NOTUSED = -7,  // component disabled
			INSTALLSTATE_BADCONFIG = -6,  // configuration data corrupt
			INSTALLSTATE_INCOMPLETE = -5,  // installation suspended or in progress
			INSTALLSTATE_SOURCEABSENT = -4,  // run from source, source is unavailable
			INSTALLSTATE_MOREDATA = -3,  // return buffer overflow
			INSTALLSTATE_INVALIDARG = -2,  // invalid function argument
			INSTALLSTATE_UNKNOWN = -1,  // unrecognized product or feature
			INSTALLSTATE_BROKEN = 0,  // broken
			INSTALLSTATE_ADVERTISED = 1,  // advertised feature
			INSTALLSTATE_REMOVED = 1,  // component being removed (action state, not settable)
			INSTALLSTATE_ABSENT = 2,  // uninstalled (or action state absent but clients remain)
			INSTALLSTATE_LOCAL = 3,  // installed on local drive
			INSTALLSTATE_SOURCE = 4,  // run from source, CD or net
			INSTALLSTATE_DEFAULT = 5,  // use default, local or source
		}

		[DllImport("msi.dll", SetLastError = true)]
		public extern static INSTALLSTATE MsiQueryProductState(string product);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		public static extern UInt32 MsiGetShortcutTargetW(string szShortcutTarget, [Out] StringBuilder szProductCode, [Out] StringBuilder szFeatureId, [Out] StringBuilder szComponentCode);

		[DllImport("msi.dll", CharSet = CharSet.Unicode)]
		public static extern INSTALLSTATE MsiGetComponentPath(string szProduct, string szComponent, [Out] StringBuilder lpPathBuf, ref UInt32 pcchBuf);

		public static uint SPI_GETWORKAREA = 48;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, byte[] pvParam, uint fWinIni);
	}

	#region MyMessageBox クラス
	/// <summary>
	/// MyMessageBox
	/// メインフォームが作成される前では、LockWindowUpdate が効かない？（true が返ってきてても）
	/// </summary>
	public class MyMessageBox
	{
		static private Rectangle _mainFormRectangle;
		static private string _caption;

		static private Timer timer = null;

		/// <summary>
		/// MessageBox を親ウィンドウの中央に表示する
		/// </summary>
		/// <param name="text"></param>
		/// <param name="caption"></param>
		/// <param name="buttons"></param>
		/// <param name="icon"></param>
		/// <returns></returns>
		static public DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, Rectangle mainFormRectangle, int mesBoxCheckInterval)
		{
			if ( !mainFormRectangle.IsEmpty )
			{
				_mainFormRectangle = mainFormRectangle;
				_caption = caption;

				IntPtr hwndDesktop = api.GetDesktopWindow();
				bool b = api.LockWindowUpdate(hwndDesktop);
				Debug.WriteLine("Show " + hwndDesktop + " " + b);

				timer = new Timer();
				timer.Interval = mesBoxCheckInterval;	// LockWindowUpdate が効かない時は、ここを短くしてみる
				timer.Tick -= Timer_Tick;
				timer.Tick += new EventHandler(Timer_Tick);
				timer.Start();
			}

			return MessageBox.Show(text, caption, buttons, icon);
		}

		/// <summary>
		/// Timer_Tick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		static private void Timer_Tick(object sender, EventArgs e)
		{
			try
			{
				timer.Stop();

				IntPtr hMessageBox = api.FindWindow(null, _caption);
				if ( hMessageBox != null )
				{
					api.RECT rectMessageBox;
					api.GetWindowRect(hMessageBox, out rectMessageBox);

					int x = _mainFormRectangle.Left + ((_mainFormRectangle.Right - _mainFormRectangle.Left) - (rectMessageBox.Right - rectMessageBox.Left)) / 2;
					int y = _mainFormRectangle.Top + ((_mainFormRectangle.Bottom - _mainFormRectangle.Top) - (rectMessageBox.Bottom - rectMessageBox.Top)) / 2;

					api.SetWindowPos(hMessageBox, (int)api.HWND_TOP, x, y, 0, 0, api.SWP_NOZORDER | api.SWP_NOSIZE);
				}
			}
			catch ( Exception )
			{
			}
			finally
			{
				if ( !api.LockWindowUpdate(IntPtr.Zero) )
				{
					int errorNo = Marshal.GetLastWin32Error();
					Debug.WriteLine("errorNo " + errorNo);
				}
				timer.Dispose();
			}
		}
	}
	#endregion
}
