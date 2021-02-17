using System;
using System.Text ;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.Configuration;
using System.Security;
using CommonFunctions;

namespace ProgUpdateClass
{
	/// <summary>
	/// module の概要の説明です。
	/// </summary>
	public class update
	{
		public const string CMDPARAM_SHOW_PROG_UPDATE_MESSAGE = "/ShowProgUpdateMessage";

		private const string APP_UPDATE_RUN_PROGRAM_NAME = "AppUpdateRun.exe";
		private const string CMDPARAM_APP_NAME = "/AppName:";
		private const string CMDPARAM_APP_FORM_TEXT = "/AppFormText:";
		private const string CMDPARAM_APP_CLASS_NAME = "/AppClassName:";
		private const string CMDPARAM_APP_WINDOW_STATE = "/AppWindowState:";
		private const string CMDPARAM_APP_COMMAND_LINE = "/AppCommandLine:";

		private const string ICPS_INI = "ICPS.INI";

		public enum options
		{
			none = 0x00000000, showProgUpdateMessage = 0x00000001, saveErrorMessage = 0x00000002, noConfirmProgUpdateMessage = 0x00000004, overWriteConfigKeys = 0x00000008, win32WindowsPlatform = 0x00000010
		};

		/// <summary>
		/// CheckProgramUpdate
		/// </summary>
		public static bool CheckProgramUpdate(string latestProgramFolder, string[] appProductNames, string[,] configKeys/*notOverwriteConfig*/, Rectangle mainFormRectangle, uint option)
		{
			try
			{
				if ( latestProgramFolder.Length == 0 )
					return false;

				PlatformID platformID = Environment.OSVersion.Platform;
				if ( (option & (uint)options.win32WindowsPlatform) != 0 )
				{
					platformID = PlatformID.Win32Windows;
				}

				const string tempExtension = ".temp";
				const string configExtension = ".config";

				string remoteProgramFileName = latestProgramFolder + appProductNames[0];
				string localProgramFileName = Application.StartupPath + "\\" + appProductNames[0];

				try
				{
					for ( int i = 0; i < appProductNames.Length; i++ )
					{
						File.Delete(Application.StartupPath + "\\" + appProductNames[i] + tempExtension);
					}
					if ( true/*platformID != PlatformID.Win32NT*/ )
					{
						File.Delete(APP_UPDATE_RUN_PROGRAM_NAME);
					}
				}
				catch ( IOException )
				{ // ignore some expected errors
				}
				catch ( UnauthorizedAccessException )
				{
				}
				catch ( SecurityException ) { }

				// アプリケーションの更新日時
#if true
				ProgUpdateRemoteCheck progUpdateRemoteCheck = new ProgUpdateRemoteCheck(remoteProgramFileName, mainFormRectangle);
				progUpdateRemoteCheck.ShowDialog();
				DateTime remoteDateTime = progUpdateRemoteCheck.remoteDateTime;
				if ( remoteDateTime == DateTime.MinValue )
				{
					throw new Exception(progUpdateRemoteCheck.errorMessage);
				}
#else
				DateTime remoteDateTime = File.GetLastWriteTime(remoteProgramFileName);
#endif
				DateTime currentDateTime = File.GetLastWriteTime(localProgramFileName);
				string messVersion = "新しいプログラムの更新日時：" + remoteDateTime.ToString("yyyy/MM/dd HH:mm") + "\r\n現在のプログラムの更新日時：" + currentDateTime.ToString("yyyy/MM/dd HH:mm");
				bool newVersion;

				if ( platformID == PlatformID.Win32NT )
				{
					// アップロードされているプログラムのバージョン情報を取得する
					FileVersionInfo fviRemote = FileVersionInfo.GetVersionInfo(remoteProgramFileName);

					// 現在のアプリケーションのバージョン情報を取得する
					FileVersionInfo fviCurrent = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

					messVersion += ",新しいプログラムのバージョン：" + fviRemote.FileVersion + "\r\n現在のプログラムのバージョン：" + fviCurrent.FileVersion;
					newVersion = (new Version(fviCurrent.FileVersion) < new Version(fviRemote.FileVersion));
				}
				else
				{
#if (xDEBUG)
					currentDateTime -= new TimeSpan (TimeSpan.TicksPerDay);
#endif
					newVersion = (currentDateTime < remoteDateTime);
				}
#if (xDEBUG)
				newVersion = true;
#endif

				if ( (option & (uint)options.showProgUpdateMessage) == 0 && !newVersion )
					return false;

				DialogResult progUpdate;

				if ( (option & (uint)options.noConfirmProgUpdateMessage) == 0 )
				{
					// 更新確認メッセージを表示する
					ProgUpdateMessage progUpdateMessage = new ProgUpdateMessage(mainFormRectangle, messVersion, 3);
					progUpdate = progUpdateMessage.ShowDialog();
					progUpdateMessage.Dispose();

					if ( progUpdate == DialogResult.Cancel )
						return false;
				}
				else
				{
					progUpdate = DialogResult.Yes;
				}

				// 新バージョンのプログラムをコピーする
				if ( platformID == PlatformID.Win32NT )
				{
					for ( int i = 0; i < appProductNames.Length; i++ )
					{
						string appProductName = Application.StartupPath + "\\" + appProductNames[i];
						try
						{
							if ( File.Exists(appProductName) )
							{
								File.Move(appProductName, appProductName + tempExtension);
							}
							File.Copy(latestProgramFolder + appProductNames[i], appProductName, true);
						}
						catch ( IOException ) { }
					}
				}
				else
				{
					for ( int i = 0; i < appProductNames.Length; i++ )
					{
						try
						{
							File.Copy(latestProgramFolder + appProductNames[i], Application.StartupPath + "\\" + appProductNames[i] + tempExtension);
						}
						catch ( IOException ) { }
					}
					File.Copy(latestProgramFolder + APP_UPDATE_RUN_PROGRAM_NAME, APP_UPDATE_RUN_PROGRAM_NAME);
				}

#if true
				// config ファイル
				if ( File.Exists(localProgramFileName + configExtension) )
				{
					File.Copy(localProgramFileName + configExtension, localProgramFileName + configExtension + ".bak", true);

					bool notOverwriteConfigKeys = ((option & (uint)options.overWriteConfigKeys) == 0);
					AppConfig remoteAppConfig = null;

					if ( (progUpdate != DialogResult.Yes) || notOverwriteConfigKeys )
					{
						// リモート側の config をローカルに上書きコピーする
						File.Copy(remoteProgramFileName + configExtension, localProgramFileName + configExtension, true);
					}
					else
					{
						remoteAppConfig = new AppConfig(remoteProgramFileName + configExtension);	// リモート側の config
					}

					// config ファイルの設定を保存する
					if ( progUpdate == DialogResult.Yes )
					{
						AppConfig appConfig = new AppConfig(appProductNames[0]);	// ローカル側の config

						if ( configKeys != null )
						{
							// notOverwriteConfigKeys: ローカル側に上書きコピーされた config に現在の設定を復元する
							// !notOverwriteConfigKeys: リモート側 config の指定された key をローカル側の config にコピーする
							for ( int i = 0; i < configKeys.Length / 2; i++ )
							{
								if ( configKeys[i, 1] != null )
									appConfig.SetValue(configKeys[i, 0], (notOverwriteConfigKeys) ? configKeys[i, 1] : remoteAppConfig.GetValue(configKeys[i, 0]));
								else
									appConfig.Remove(configKeys[i, 0]);	// key を削除する
							}
						}

						if ( !notOverwriteConfigKeys )
						{
							// リモート側にあってローカル側に無い key をコピーする
							StringBuilder xpath = new StringBuilder("/configuration/appSettings/add[");
							foreach ( XmlNode add in appConfig.xmlConfig.SelectNodes("/configuration/appSettings/add") )
							{
								xpath.Append("@key!='" + add.Attributes["key"].Value + "' and ");
							}
							xpath.Remove(xpath.Length - 5, 5);	// 5:" and "
							xpath.Append("]");

							foreach ( XmlNode add in remoteAppConfig.xmlConfig.SelectNodes(xpath.ToString()) )
							{
								appConfig.SetValue(add.Attributes["key"].Value, add.Attributes["value"].Value);
							}

#if true
							// appSettings ノード以外を更新する
							XmlNodeList remoteOtherNodes = remoteAppConfig.GetOtherNodes("appSettings");
							if ( remoteOtherNodes != null )
							{
								XmlNodeList otherNodes = appConfig.GetOtherNodes("appSettings");
								foreach ( XmlNode node in otherNodes )
								{
									appConfig.xmlConfig.DocumentElement.RemoveChild(node);
								}

								foreach ( XmlNode node in remoteOtherNodes )
								{
									XmlNode newNode = appConfig.xmlConfig.ImportNode(node, true);
									appConfig.xmlConfig.DocumentElement.AppendChild(newNode);
								}
								appConfig.Save();
							}
#endif
						}
					}
				}
#else
				// config ファイル
				if ( File.Exists(localProgramFileName + configExtension) )
				{
					File.Copy(localProgramFileName + configExtension, localProgramFileName + configExtension + ".bak", true);
					File.Copy(remoteProgramFileName + configExtension, localProgramFileName + configExtension, true);

					// config ファイルの設定を保存する
					if ( progUpdate == DialogResult.Yes && notOverwriteConfig != null )
					{
						AppConfig appConfig = new AppConfig(appProductNames[0]);

						for ( int i = 0; i < notOverwriteConfig.Length / 2; i++ )
						{
							if ( notOverwriteConfig[i, 1] != null )
								appConfig.SetValue(notOverwriteConfig[i, 0], notOverwriteConfig[i, 1]);
							else
								appConfig.Remove(notOverwriteConfig[i, 0]);
						}
					}
				}
#endif

				// ICPS.INI ファイル
				if ( File.Exists(latestProgramFolder + ICPS_INI) )
				{
					File.Copy(latestProgramFolder + ICPS_INI, Application.StartupPath + "\\" + ICPS_INI, true);
				}

				return true;
			}
			catch ( Exception exp )
			{
				if ( (option & (uint)options.saveErrorMessage) == 0 )
				{
					MyMessageBox.Show("プログラムの自動更新が失敗しました.\r\n" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error, mainFormRectangle, 10);
				}
				else
				{
					try
					{
						string fileName = @".\自動更新エラー.log";
						using ( StreamWriter sw = new StreamWriter(fileName, false) )
						{
							sw.WriteLine(MethodBase.GetCurrentMethod().Name);
							sw.WriteLine(exp.Message);
							sw.Close();
						}
					}
					catch ( Exception )
					{
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Re-run the application either in another AppDomain or in another process
		/// </summary>
		public static bool RelaunchExe(string[] appProductNames, string appFormText, string appClassName, string appWindowState, uint option)
		{
			try
			{
				//MessageBox.Show ("プログラムの更新が完了しました.\n[OK]ボタンを押した後、自動的に新しいバージョンで起動されます.", "プログラム更新", MessageBoxButtons.OK, MessageBoxIcon.Information);

				PlatformID platformID = Environment.OSVersion.Platform;
				if ( (option & (uint)options.win32WindowsPlatform) != 0 )
				{
					platformID = PlatformID.Win32Windows;
				}

				if ( platformID == PlatformID.Win32NT )
				{
					// Get assembly that started it all
					Assembly entry = Assembly.GetEntryAssembly();
					// "Intuit" its exe name
					String assemblyName = Application.StartupPath + "\\" + entry.GetName().Name + ".exe";

					// Create an AppDomain that shadow copies its files
					AppDomain current = AppDomain.CurrentDomain;
					AppDomainSetup info = current.SetupInformation;
					info.ShadowCopyFiles = true.ToString();
					AppDomain domain = AppDomain.CreateDomain(assemblyName, current.Evidence, info);

					// Relaunch the application in the new application domain
					// using the same command line arguments
					String[] argsOld = Environment.GetCommandLineArgs();
					String[] argsNew = new String[argsOld.Length - 1];
					Array.Copy(argsOld, 1, argsNew, 0, argsNew.Length);

#pragma warning disable 0618
					domain.ExecuteAssembly(assemblyName, entry.Evidence, argsNew);
#pragma warning restore 0618
				}
				else
				{
					StringBuilder arguments = new StringBuilder(CMDPARAM_APP_NAME);
					for ( int i = 0; i < appProductNames.Length; i++ )
					{
						arguments.Append(appProductNames[i]);
						arguments.Append((i != appProductNames.Length - 1) ? "," : " ");
					}

					if ( appFormText != null )
					{
						arguments.Append(CMDPARAM_APP_FORM_TEXT + appFormText);
					}

					if ( appClassName != null )
					{
						arguments.Append(" " + CMDPARAM_APP_CLASS_NAME + appClassName);
					}

					if ( appWindowState != null )
					{
						arguments.Append(" " + CMDPARAM_APP_WINDOW_STATE + appWindowState);
					}

					string[] args = Environment.GetCommandLineArgs();
					if ( 1 < args.Length )
					{
						// /param1:xxx,/param2:yyy の形式で渡し、AppUpdateRun.exe 側で /param1:xxx /param2:yyy に分割する
						arguments.Append(" " + CMDPARAM_APP_COMMAND_LINE + string.Join(",", args, 1, args.Length - 1));
					}

					Process.Start(APP_UPDATE_RUN_PROGRAM_NAME, arguments.ToString());
				}

				return true;
			}
			catch ( Exception exp )
			{
				if ( (option & (uint)options.saveErrorMessage) == 0 )
				{
					MessageBox.Show("プログラムの再起動が失敗しました.\r\n" + exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					string fileName = @".\再起動エラー.log";
					StreamWriter sw = new StreamWriter(fileName, false);
					sw.WriteLine(MethodBase.GetCurrentMethod().Name);
					sw.WriteLine(exp.Message);
					sw.Close();
				}
			}

			return false;
		}
	}
}
