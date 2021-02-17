using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.Configuration;
using System.Security;
using System.Collections.Generic;
using TAFactory.IconPack;
using CommonFunctions;

namespace ccMushroom
{
	/// <summary>
	/// ccFunction の概要の説明です。
	/// </summary>
	public class ccf
	{
		[System.Runtime.InteropServices.DllImport("shell32.dll", EntryPoint = "#62", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
		private static extern bool SHPickIconDialog(IntPtr hWnd, StringBuilder pszFilename, int cchFilenameMax, out int pnIconIndex);

		//[System.Runtime.InteropServices.DllImport("shell32.dll")]
		//private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

		private const string CC_MUSHROOM_LOCAL_FOLDER = "$CCLOCAL$";

		public struct hotKeys
		{
			public bool alt;
			public bool ctrl;
			public bool shift;
			public bool win;
			public int keyCode;

			public void Init()
			{
				alt = false;
				ctrl = false;
				shift = false;
				win = false;
				keyCode = -1;
			}

			public int CharToKeyCode(char _keyChar)
			{
				return (int)Keys.A + (_keyChar - 'a');
			}

			public bool IsValid()
			{
				return ((alt || ctrl || shift || win) && (keyCode != -1));
			}

			public int GetModifiers()
			{
				return (alt ? api.MOD_ALT : 0) | (ctrl ? api.MOD_CONTROL : 0) | (shift ? api.MOD_SHIFT : 0) | (win ? api.MOD_WIN : 0);
			}

			public string GetKeyChar()
			{
				return (keyCode != -1) ? ((char)(keyCode)).ToString() : "";
			}
		};

		/// <summary>
		/// AdjustFolderFormat
		/// </summary>
		public static bool AdjustFolderFormat(ref string folder)
		{
			try
			{
				if ( folder == null || folder.Length == 0 )
					return false;

				if ( folder.EndsWith(@"\") )
					folder = folder.Substring(0, folder.Length - 1);

				if ( !folder.StartsWith(@"\\") )
				{
					if ( (1 < folder.Length) && (folder[1] != ':') )
						folder = Directory.GetCurrentDirectory().Substring(0, 2) + folder;
				}
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			return true;
		}

		/// <summary>
		/// IsSystemFolder
		/// </summary>
		public static bool IsSystemFolder(string folder)
		{
			try
			{
				if ( folder == null || folder.Length == 0 )
					return false;

				foreach ( Environment.SpecialFolder sf in Enum.GetValues(typeof(Environment.SpecialFolder)) )
				{
					string folderPath = Environment.GetFolderPath(sf);
#if (DEBUG)
					//Console.WriteLine("{0}: {1}", sf, folderPath);
#endif
					if ( String.Compare(folder, folderPath, true) == 0 )
						return true;
				}

				string windir = Environment.GetEnvironmentVariable("windir");
				string systemDriveRoot = (3 < windir.Length) ? windir.Substring(0, 3) : string.Empty;
				if ( String.Compare(folder, windir, true) == 0 ||
					String.Compare(folder, systemDriveRoot, true) == 0 )
					return true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			return false;
		}

		/// <summary>
		/// ローカル側のプログラムフォルダ定数を実フォルダ名（またはその逆）に変換する
		/// </summary>
		/// <param name="localProgramsFolder"></param>
		/// <param name="getActualFolder"></param>
		/// <returns></returns>
		public static string ReplaceCcMushroomLocalFolder(string localProgramsFolder, bool getActualFolder)
		{
			if ( ccMushroom.ccMushroomLocalFolder != null )
			{
				if ( getActualFolder )
				{
					localProgramsFolder = localProgramsFolder.Replace(CC_MUSHROOM_LOCAL_FOLDER, ccMushroom.ccMushroomLocalFolder);
				}
				else
				{
					localProgramsFolder = localProgramsFolder.Replace(ccMushroom.ccMushroomLocalFolder, CC_MUSHROOM_LOCAL_FOLDER);

					if ( localProgramsFolder.IndexOf(CC_MUSHROOM_LOCAL_FOLDER) != -1 )
					{
						int lenCC_MUSHROOM_LOCAL_FOLDER = CC_MUSHROOM_LOCAL_FOLDER.Length;
						if ( (lenCC_MUSHROOM_LOCAL_FOLDER < localProgramsFolder.Length) && (localProgramsFolder[lenCC_MUSHROOM_LOCAL_FOLDER] != '\\') )
						{
							localProgramsFolder = localProgramsFolder.Insert(lenCC_MUSHROOM_LOCAL_FOLDER, "\\");
						}
					}
				}
			}

			return localProgramsFolder;
		}

		/// <summary>
		/// ShowApplicationConfig
		/// </summary>
		public static void ShowApplicationConfig(string fileCcConfiguration)
		{
			try
			{
				string fileAppConfig = Application.StartupPath + "\\" + Application.ProductName + ".exe.config";
#if true
				// バイナリ(FileStream) で読み込むと、XmlDocument.Save() で保存された後の
				// UTF-8 BOM (Byte Order Mark) も表示されてしまうのでこうした。
				StreamReader srAppConfig = File.OpenText(fileAppConfig);
				string appConfig = srAppConfig.ReadToEnd();
				srAppConfig.Close();
#else
				FileStream fsAppConfig = File.OpenRead (fileAppConfig);
				byte [] byAppConfig = new byte [fsAppConfig.Length];
				fsAppConfig.Read (byAppConfig, 0, byAppConfig.Length);
				string appConfig = new string (Encoding.Default.GetChars(byAppConfig));
				fsAppConfig.Close ();
#endif
				if ( !appConfig.EndsWith("\r\n") )
					appConfig += "\r\n";

				string fileAppIni = Application.StartupPath + "\\" + Application.ProductName.ToUpper() + ".INI";
				FileStream fsAppIni = File.OpenRead(fileAppIni);
				byte[] byAppIni = new byte[fsAppIni.Length];
				fsAppIni.Read(byAppIni, 0, byAppIni.Length);
				fsAppIni.Close();
				string appIni = new string(Encoding.Default.GetChars(byAppIni));

				MessageBox.Show(fileAppConfig + "\r\n" + appConfig + "\r\n" + fileAppIni + "\r\n" + appIni, "環境設定", MessageBoxButtons.OK);

				Process.Start("IExplore.exe", fileCcConfiguration);
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// string array の再配置
		/// </summary>
		public static void reallocStringArray(ref string[] strings, int count)
		{
			if ( strings == null )
			{
				strings = new string[count];
			}
			else
			{
				string[] stringsTemp;
				stringsTemp = new string[strings.Length];
				Array.Copy(strings, stringsTemp, strings.Length);

				strings = new string[count];
				Array.Copy(stringsTemp, strings, (stringsTemp.Length < count) ? stringsTemp.Length : count);
			}
		}

		/// <summary>
		/// int array の再配置
		/// </summary>
		public static void reallocIntArray(ref int[] ints, int count)
		{
			if ( ints == null )
			{
				ints = new int[count];
			}
			else
			{
				int[] intsTemp;
				intsTemp = new int[ints.Length];
				Array.Copy(ints, intsTemp, ints.Length);

				ints = new int[count];
				Array.Copy(intsTemp, ints, (intsTemp.Length < count) ? intsTemp.Length : count);
			}
		}

		/// <summary>
		/// bool array の再配置
		/// </summary>
		public static void reallocBoolArray(ref bool[] bools, int count)
		{
			if ( bools == null )
			{
				bools = new bool[count];
			}
			else
			{
				bool[] boolsTemp;
				boolsTemp = new bool[bools.Length];
				Array.Copy(bools, boolsTemp, bools.Length);

				bools = new bool[count];
				Array.Copy(boolsTemp, bools, (boolsTemp.Length < count) ? boolsTemp.Length : count);
			}
		}

#if false
		/// <summary>
		/// TabPage array の再配置
		/// </summary>
		public static void reallocTabPageArray(ref System.Windows.Forms.TabPage[] tabPages, int count)
		{
			if ( tabPages == null )
			{
				tabPages = new System.Windows.Forms.TabPage[count];
			}
			else
			{
				System.Windows.Forms.TabPage[] tabPagesTemp;
				tabPagesTemp = new System.Windows.Forms.TabPage[tabPages.Length];
				Array.Copy(tabPages, tabPagesTemp, tabPages.Length);

				tabPages = new System.Windows.Forms.TabPage[count];
				Array.Copy(tabPagesTemp, tabPages, (tabPagesTemp.Length < count) ? tabPagesTemp.Length : count);
			}
		}

		/// <summary>
		/// Point array の再配置
		/// </summary>
		public static void reallocPointArray(ref Point[] points, int count)
		{
			if ( points == null )
			{
				points = new Point[count];
			}
			else
			{
				Point[] pointsTemp;
				pointsTemp = new Point[points.Length];
				Array.Copy(points, pointsTemp, points.Length);

				points = new Point[count];
				Array.Copy(pointsTemp, points, (pointsTemp.Length < count) ? pointsTemp.Length : count);
			}
		}

		/// <summary>
		/// Button array の再配置
		/// </summary>
		public static void reallocButtonArray(ref System.Windows.Forms.Button[] buttons, int count)
		{
			if ( buttons == null )
			{
				buttons = new System.Windows.Forms.Button[count];
			}
			else
			{
				System.Windows.Forms.Button[] buttonsTemp;
				buttonsTemp = new System.Windows.Forms.Button[buttons.Length];
				Array.Copy(buttons, buttonsTemp, buttons.Length);

				buttons = new System.Windows.Forms.Button[count];
				Array.Copy(buttonsTemp, buttons, (buttonsTemp.Length < count) ? buttonsTemp.Length : count);
			}
		}

		/// <summary>
		/// Icon array の再配置
		/// </summary>
		public static void reallocIconArray(ref Icon[] icons, int count)
		{
			if ( icons == null )
			{
				icons = new Icon[count];
			}
			else
			{
				Icon[] iconsTemp;
				iconsTemp = new Icon[icons.Length];
				Array.Copy(icons, iconsTemp, icons.Length);

				icons = new Icon[count];
				Array.Copy(iconsTemp, icons, (iconsTemp.Length < count) ? iconsTemp.Length : count);
			}
		}
#endif

		/// <summary>
		/// PaintButton
		/// </summary>
		public static void PaintButton(Control control, Graphics g, bool smallIcon, Icon appIcon, int iconOffset, bool drawIconRect, Icon newAppIcon, string buttonText, float textOffset, Font buttonFont, Color buttonTextColor, Color buttonBackColor, bool dragButton)
		{
			try
			{
				/*SolidBrush backColor = new SolidBrush(buttonBackColor);
				//g.FillRectangle(backColor, control.Location.X, control.Location.Y, control.Size.Width, control.Size.Height);
				g.FillRectangle(backColor, control.ClientRectangle);
				backColor.Dispose();*/

				int iconSize = (smallIcon) ? 16 : 32;
				int iconX = (smallIcon) ? 6 : 4;

				// アプリケーション アイコン
				if ( appIcon != null )
				{
					int iconY = (control.Size.Height - iconSize) / 2 + iconOffset;
					g.DrawIcon(appIcon, iconX, iconY);
					if ( drawIconRect )
					{
						Pen pen = new Pen(Color.LightGray);
						g.DrawRectangle(pen, new Rectangle(iconX, iconY, iconSize - 1, iconSize - 1));
						pen.Dispose();
					}
				}

				// 更新されたアプリケーション
				if ( newAppIcon != null )
				{
					//int newAppIconX = control.Size.Width - newAppIcon.Size.Width - 1 + 3;	// 右上
					//int newAppIconX = 1;	// 左上
					int newAppIconX = iconX + iconSize - ((smallIcon)? 7: 9);	// アイコンの右上
					int newAppIconY = (smallIcon) ? 2 : 1;
					g.DrawIcon(newAppIcon, newAppIconX, newAppIconY);
				}

				// ボタン テキスト
				if ( buttonText != null && !dragButton )
				{
					int textMargin = 4;
					int textX = iconX + iconSize + textMargin;
					SizeF sizeText = g.MeasureString(buttonText, buttonFont);

#if true
					StringFormat formatText = new StringFormat();
					formatText.Trimming = StringTrimming.EllipsisCharacter;
					SolidBrush brush = new SolidBrush(buttonTextColor);
					RectangleF rectText = new RectangleF(textX, (control.Size.Height - buttonFont.Height) / 2 + textOffset, control.Size.Width - textX - textMargin, sizeText.Height + 1);
					g.DrawString(buttonText, buttonFont, brush, rectText, formatText);
					brush.Dispose();
#else
					// テキストがボタンの幅より長い？
					if ( control.Size.Width - textX - textMargin <= sizeText.Width )
					{
						for ( int i = buttonText.Length; 0 < i; i-- )
						{
							sizeText = g.MeasureString(buttonText + "...", buttonFont);
							if ( sizeText.Width < control.Size.Width - textX - textMargin )
								break;
							buttonText = buttonText.Substring (0, buttonText.Length - 1);
						}
						buttonText += "...";
					}

					SolidBrush brush = new SolidBrush (buttonTextColor);
					g.DrawString (buttonText, buttonFont, brush, textX, (control.Size.Height - buttonFont.Height) / 2 + textOffset);
					brush.Dispose ();
#endif
				}

				if ( dragButton )
				{
					int y = 2, width = 2, step = width + 2;
					for ( int i = 3; 0 < i; i-- )
					{
						int x = 2;
						for ( int j = 0; j < i; j++ )
						{
							g.FillRegion(ccMushroom.rectMarkBackColor/*Brushes.GhostWhite*/, new Region(new Rectangle(x + 1, y + 1, width, width)));
							g.FillRegion(ccMushroom.rectMarkForeColor/*Brushes.DarkGray*/, new Region(new Rectangle(x, y, width, width)));
							x += step;
						}

						y += step;
					}
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
		/// Get the associated Icon for a file or application, this method always returns
		/// an icon.  If the strPath is invalid or there is no idonc the default icon is returned
		/// </summary>
		/// <param name="strPath">full path to the file</param>
		/// <param name="bSmall">if true, the 16x16 icon is returned otherwise the 32x32</param>
		/// <returns></returns>
		public static Icon GetIcon(string strPath, bool bSmall)
		{
			if ( strPath.StartsWith(@"\\") && (!Program.onlineEnable || ccMushroom.ignoreRemoteInfo) )
				return null;

			Icon icon = null;

			try
			{
				if ( File.Exists(strPath) )
				{
					api.SHFILEINFO info = new api.SHFILEINFO();
					int cbFileInfo = Marshal.SizeOf(info);
					api.SHGFI flags;
					flags = api.SHGFI.Icon | ((bSmall) ? api.SHGFI.SmallIcon : api.SHGFI.LargeIcon)/* | api.SHGFI.UseFileAttributes*/;
					api.SHGetFileInfo(strPath, 0/*api.FILE_ATTRIBUTE_TEMPORARY*/, ref info, (uint)cbFileInfo, flags);
					if ( info.hIcon != IntPtr.Zero )
					{
						icon = Icon.FromHandle(info.hIcon);
					}
				}
				else if ( Directory.Exists(strPath) )
				{
					string shell32dll;
					int iconIndex;
					if ( Environment.OSVersion.Platform == PlatformID.Win32NT )
					{
						shell32dll = common.CheckEnvironmentVariable(@"%SystemRoot%\system32\shell32.dll");
						if ( strPath.StartsWith(@"\\") )
							iconIndex = ((common.GetOsPlatform() & common.platform.winnt4) != 0) ? 9 : -172/*85*/;
						else
							iconIndex = -4/*3*/;
					}
					else
					{
						shell32dll = common.CheckEnvironmentVariable(@"%windir%\system\shell32.dll");
						if ( strPath.StartsWith(@"\\") )
							iconIndex = 9;
						else
							iconIndex = 3;
					}
					icon = ExtractIconEx(shell32dll, iconIndex, bSmall);
				}
			}
			catch ( Exception exp )
			{
				if ( Program.debMode )
				{
					MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}

			if ( strPath.StartsWith(@"\\") && (icon == null) )
			{
				if ( ccMushroom.enableIgnoreRemoteInfo )
				{
					ccMushroom.ignoreRemoteInfo = true;
				}
			}

			return icon;
		}

		/// <summary>
		/// ExtractIconEx
		/// </summary>
		/// <param name="strPath"></param>
		/// <param name="iconIndex">マイナス(-)でアイコンのIDを直接指定できる</param>
		/// <param name="bSmall"></param>
		/// <returns></returns>
		public static Icon ExtractIconEx(string strPath, int iconIndex, bool bSmall)
		{
			int readIconCount = 0;
			IntPtr[] hDummy = new IntPtr[1] { IntPtr.Zero };
			IntPtr[] hIconEx = new IntPtr[1] { IntPtr.Zero };

			try
			{
				if ( bSmall )
					readIconCount = api.ExtractIconEx(strPath, iconIndex, hDummy, hIconEx, 1);
				else
					readIconCount = api.ExtractIconEx(strPath, iconIndex, hIconEx, hDummy, 1);

				if ( readIconCount > 0 && hIconEx[0] != IntPtr.Zero )
				{
					// GET FIRST EXTRACTED ICON
					Icon extractedIcon = (Icon)Icon.FromHandle(hIconEx[0]).Clone();

					return extractedIcon;
				}
				else
				{
					// NO ICONS READ
					return null;
				}
			}
			catch ( Exception ex )
			{
				/* EXTRACT ICON ERROR */
				// BUBBLE UP
				throw new ApplicationException("Could not extract icon", ex);
			}
			finally
			{
				// RELEASE RESOURCES
				foreach ( IntPtr ptr in hIconEx )
				{
					if ( ptr != IntPtr.Zero )
						api.DestroyIcon(ptr);
				}

				foreach ( IntPtr ptr in hDummy )
				{
					if ( ptr != IntPtr.Zero )
						api.DestroyIcon(ptr);
				}
			}
		}

		/// <summary>
		/// ExtractIconEx
		/// </summary>
		public static Icon ExtractIconEx(string strPath, int iconIndex, int getIndex, bool bSmall)
		{
			int icons = 10;
			IntPtr[] hDummy = new IntPtr[icons];
			IntPtr[] hIconEx = new IntPtr[icons];
			int readIconCount = 0;

			for ( int i = 0; i < icons; i++ )
			{
				hDummy[i] = IntPtr.Zero;
				hIconEx[i] = IntPtr.Zero;
			}

			try
			{
				if ( bSmall )
					readIconCount = api.ExtractIconEx(strPath, iconIndex, hDummy, hIconEx, icons);
				else
					readIconCount = api.ExtractIconEx(strPath, iconIndex, hIconEx, hDummy, icons);

				if ( (readIconCount > 0) && (hIconEx[getIndex] != IntPtr.Zero) )
				{
					// GET FIRST EXTRACTED ICON
					Icon extractedIcon = (Icon)Icon.FromHandle(hIconEx[getIndex]).Clone();

					return extractedIcon;
				}
				else
				{
					// NO ICONS READ
					return null;
				}
			}
			catch ( Exception ex )
			{
				/* EXTRACT ICON ERROR */
				// BUBBLE UP
				throw new ApplicationException("Could not extract icon", ex);
			}
			finally
			{
				// RELEASE RESOURCES
				foreach ( IntPtr ptr in hIconEx )
				{
					if ( ptr != IntPtr.Zero )
						api.DestroyIcon(ptr);
				}

				foreach ( IntPtr ptr in hDummy )
				{
					if ( ptr != IntPtr.Zero )
						api.DestroyIcon(ptr);
				}
			}
		}

		/// <summary>
		/// 文字列中に環境変数があれば変換する（http 以外）
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string CheckEnvironmentVariable(string source)
		{
			if ( source.StartsWith("http") )
				return source;

			return common.CheckEnvironmentVariable(source);
		}

		/// <summary>
		/// ホットキーの設定を解析する
		/// </summary>
		/// <param name="hotKey"></param>
		/// <param name="hotKeys"></param>
		/// <returns></returns>
		public static bool ParseHotKey(string hotKey, out hotKeys hotKeys)
		{
			hotKeys = new hotKeys();
			hotKeys.Init();

			try
			{
				string[] values = hotKey.Split('+');

				for ( int i = 0; i < values.Length; i++ )
				{
					string value = values[i].ToLower();
					if ( value == "alt" )
						hotKeys.alt = true;
					else if ( value == "ctrl" )
						hotKeys.ctrl = true;
					else if ( value == "shift" )
						hotKeys.shift = true;
					else if ( value == "win" )
						hotKeys.win = true;
					else
						hotKeys.keyCode = (value.Length != 0) ? hotKeys.CharToKeyCode(value[0]) : -1;
				}

				return hotKeys.IsValid();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
				return false;
			}
		}

		/// <summary>
		/// LOWORD
		/// </summary>
		/// <param name="dwValue"></param>
		/// <returns></returns>
		public static ushort LOWORD(uint dwValue)
		{
			return (ushort)(dwValue & 0xffff);
		}
		
		/// <summary>
		/// HIWORD
		/// </summary>
		/// <param name="dwValue"></param>
		/// <returns></returns>
		public static ushort HIWORD(uint dwValue)
		{
			return (ushort)(dwValue >> 16);
		}
		
		/// <summary>
		/// MAKELONG
		/// </summary>
		/// <param name="wLow"></param>
		/// <param name="wHigh"></param>
		/// <returns></returns>
		public static uint MAKELONG(ushort wLow, ushort wHigh)
		{
			return (uint)(wLow & 0xffff) | (uint)(wHigh << 16);
		}

		/// <summary>
		/// ファイルを更新/削除する
		/// </summary>
		/// <param name="sorcFileName"></param>
		/// <param name="destFileName"></param>
		/// <param name="tempFileName"></param>
		public static void UpdateFile(string sorcFileName, string destFileName, string tempFileName, ref StringBuilder log)
		{
			log.Append("UpdateFile(" + sorcFileName + "," + destFileName + "," + tempFileName + "\r\n");

			// テンポラリ ファイルを削除する
			if ( tempFileName != null )
			{
				string _tempFileName = Path.GetFileName(tempFileName);
				log.Append(" (" + _tempFileName + " != null)\r\n");

				if ( File.Exists(tempFileName) )
				{
					log.Append(" File.Exists(" + _tempFileName + ")\r\n");

					try
					{
						File.Delete(tempFileName);
						log.Append(" File.Delete(" + _tempFileName + ")\r\n");
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
						log.Append(" " + exp.Message + "\r\n");
					}
				}
			}

			// ファイルを更新する
			if ( (sorcFileName != null) && (destFileName != null) )
			{
				string _sorcFileName = Path.GetFileName(sorcFileName);
				string _destFileName = Path.GetFileName(destFileName);
				log.Append(" (" + _sorcFileName + " != null) && (" + _destFileName + " != null)\r\n");

				try
				{
					File.Copy(sorcFileName, destFileName, true);
					log.Append(" File.Copy(" + _sorcFileName + ", " + _destFileName + ", true)\r\n");
				}
				catch ( IOException ioexp )	// 別のプロセスで使用されているため？
				{
					Debug.WriteLine(ioexp.Message);
					log.Append(" " + ioexp.Message + "\r\n");
					string _tempFileName = Path.GetFileName(tempFileName);

					try
					{
						// とりあえず、curFileName を一時ファイルにリネームして、newFileName をコピーしておく
						File.Move(destFileName, tempFileName);
						log.Append(" File.Move(" + _destFileName + ", " + _tempFileName + ")\r\n");
						File.Copy(sorcFileName, destFileName, true);
						log.Append(" File.Copy(" + _sorcFileName + ", " + _destFileName + ", true)\r\n");
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
						log.Append(" " + exp.Message + "\r\n");

#if true
						try
						{
							// MoveFileEx を使ってコピーする
							string subKeyName = @"SYSTEM\CurrentControlSet\Control\Session Manager";
							Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKeyName);
							string[] pendingFileRenameOperations = (string[])regKey.GetValue("PendingFileRenameOperations");
							regKey.Close();
							log.Append(" LocalMachine.OpenSubKey(" + subKeyName + ")\r\n");
							log.Append(" PendingFileRenameOperations\r\n");

							string sorcMoveFileName = sorcFileName + ".move";
							string _sorcMoveFileName = Path.GetFileName(sorcMoveFileName);

							bool alreadyPending = false;
							if ( pendingFileRenameOperations != null )
							{
								for ( int i = 0; i < pendingFileRenameOperations.Length / 2; i++ )
								{
									log.Append("  " + pendingFileRenameOperations[(i * 2) + 0] + "\r\n");
									log.Append("  " + pendingFileRenameOperations[(i * 2) + 1] + "\r\n");

									if ( pendingFileRenameOperations[(i * 2) + 0].IndexOf(sorcMoveFileName) != -1 )
									{
										alreadyPending = true;
										//break;
									}
								}
							}

							if ( !alreadyPending )
							{
								File.Copy(sorcFileName, sorcMoveFileName, true);
								log.Append(" File.Copy(" + _sorcFileName + ", " + _sorcMoveFileName + ", true)\r\n");
								bool res = api.MoveFileEx(sorcMoveFileName, destFileName, api.MOVEFILE_REPLACE_EXISTING | api.MOVEFILE_DELAY_UNTIL_REBOOT);
								Debug.WriteLine("MoveFileEx:" + res);
								log.Append(" " + res + " = api.MoveFileEx(" + _sorcMoveFileName + ", " + _destFileName + ", api.MOVEFILE_REPLACE_EXISTING | api.MOVEFILE_DELAY_UNTIL_REBOOT)\r\n");
							}
						}
						catch ( Exception _exp )
						{
							Debug.WriteLine(_exp.Message);
							log.Append(" " + _exp.Message + "\r\n");
						}
#endif
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
					log.Append(" " + exp.Message + "\r\n");
				}
			}
			// ファイルを削除する
			else if ( (sorcFileName == null) && (destFileName != null) )
			{
				string _destFileName = Path.GetFileName(destFileName);
				log.Append(" (sorcFileName == null) && (" + _destFileName + " != null)\r\n");

				try
				{
					File.Delete(destFileName);
					log.Append(" File.Delete(" + _destFileName + ")\r\n");
				}
				catch ( UnauthorizedAccessException uaexp )	// アクセスが拒否されました？
				{
					Debug.WriteLine(uaexp.Message);
					log.Append(" " + uaexp.Message + "\r\n");
					string _tempFileName = Path.GetFileName(tempFileName);

					try
					{
						File.Move(destFileName, tempFileName);
						log.Append(" File.Move(" + _destFileName + ", " + _tempFileName + ")\r\n");
					}
					catch ( Exception exp )
					{
						Debug.WriteLine(exp.Message);
						log.Append(" " + exp.Message + "\r\n");
					}
				}
				catch ( Exception exp )
				{
					Debug.WriteLine(exp.Message);
					log.Append(" " + exp.Message + "\r\n");
				}
			}
		}

		/// <summary>
		/// ファイルからアイコンリソースを抽出して保存する
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="moduleFileName"></param>
		/// <param name="saveIconFileName"></param>
		/// <returns></returns>
		public static bool ExtractResourceIcon(IntPtr hWnd, string moduleFileName, string saveIconFileName)
		{
			try
			{
				StringBuilder pszFilename = new StringBuilder(moduleFileName, Microsoft.API.Win32.MAX_PATH);
				int iconIndex;

				if ( !SHPickIconDialog(hWnd, pszFilename, Microsoft.API.Win32.MAX_PATH, out iconIndex) )
					return false;
				moduleFileName = Environment.ExpandEnvironmentVariables(pszFilename.ToString());

#if false
				IntPtr hIcon = ExtractIcon(Process.GetCurrentProcess().Handle, fileName, iconIndex);
				if ( hIcon != null )
				{
					//IconTest.GetIconImage(hIcon);

					Icon icon = Icon.FromHandle(hIcon);
					using ( FileStream fs = new FileStream(saveIconFileName, FileMode.Create) )
					{
						icon.Save(fs);
						fs.Close();
					}
					api.DestroyIcon(hIcon);
				}
#else
				List<Icon> extractedIcons = null;

				try
				{
					Cursor.Current = Cursors.WaitCursor;
					extractedIcons = IconHelper.ExtractAllIcons(moduleFileName);
				}
				catch ( Exception exp )
				{
					MessageBox.Show(exp.Message, "Icon Extractor", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return false;
				}
				finally
				{
					Cursor.Current = Cursors.Default;
				}

				using ( FileStream fs = new FileStream(saveIconFileName, FileMode.Create) )
				{
					extractedIcons[iconIndex].Save(fs);
					fs.Close();
				}
#endif

				return true;
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
	}
}
