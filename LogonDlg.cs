using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Principal;
using CommonFunctions;

namespace ccMushroom
{
	public partial class LogonDlg : Form
	{
		private string dirPath = null;
		private string domain = null;
		private string userID = null;
		private string password = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="domain"></param>
		/// <param name="userID"></param>
		/// <param name="password"></param>
		public LogonDlg(string dirPath, string domain, string userID, string password)
		{
			try
			{
				InitializeComponent();

				this.dirPath = dirPath;
				this.domain = domain;
				this.userID = userID;
				this.password = password;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// LogonDlg_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogonDlg_Load(object sender, EventArgs e)
		{
			try
			{
				textUserName.Text = userID;
				textPassword.Text = password;
				if ( !string.IsNullOrEmpty(domain) )
				{
					string[] domains = domain.Split('.');
					textDomain.Text = domains[0];
				}

				buttonCancel.Location = buttonOK.Location;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
				DialogResult = DialogResult.Cancel;
				this.Close();
			}
		}

		/// <summary>
		/// [OK] ボタンが押された
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonOK_Click(object sender, EventArgs e)
		{
			if ( (textUserName.Text.Length == 0) || (textPassword.Text.Length == 0) )
				return;

			if ( !IsAuthenticated() )
			{
				textPassword.Focus();
				textPassword.SelectAll();
				return;
			}

			DialogResult = DialogResult.OK;
			this.Close();
		}

		/// <summary>
		/// ユーザー名が ActiveDirectory に存在するか確認する
		/// </summary>
		/// <returns></returns>
		private bool IsAuthenticated()
		{
			try
			{
				Cursor.Current = Cursors.WaitCursor;

				string domainAndUsername = domain + @"\" + textUserName.Text;
				using ( DirectoryEntry entry = new DirectoryEntry(dirPath, domainAndUsername, textPassword.Text) )
				{
					// Bind to the native AdsObject to force authentication. 
					Object obj = entry.NativeObject;

					DirectorySearcher searcher = new DirectorySearcher(entry);
					searcher.Filter = "(SAMAccountName=" + textUserName.Text + ")";
					searcher.PropertiesToLoad.Add("cn");
					SearchResult result = searcher.FindOne();
					if ( null == result )
					{
						MessageBox.Show("ユーザー名の検索が失敗しました", "LOGON ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}

					Debug.WriteLine((string)result.Properties["cn"][0]);
				}
			}
			catch ( Exception exp )
			{
				if ( !IsLockoutedAccount() )
				{
					MessageBox.Show(exp.Message, "LOGON ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				return false;
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}

			return true;
		}

		/// <summary>
		/// ロックアウトされたアカウントか否か確認する
		/// Windows 7 以降では、ドメインにログオンしていないと、Domain.GetDomain() で例外が発生する。
		/// ロックアウトの解除は、適切な権限が無いと拒否される。
		/// </summary>
		/// <returns></returns>
		private bool IsLockoutedAccount()
		{
			bool lockouted = false;
			DirectoryEntry entry = null;

			try
			{
				DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain, this.domain);
				Domain domain = Domain.GetDomain(context);
				entry = domain.GetDirectoryEntry();

				DirectorySearcher searcher = new DirectorySearcher(entry);
				//searcher.Filter = "(&(ObjectCategory=user)(LockOutTime>=1))";
				searcher.Filter = "(LockOutTime>=1)";
				SearchResultCollection src = searcher.FindAll();

				foreach ( SearchResult result in src )
				{
					Debug.WriteLine("■" + (string)result.Properties["cn"][0]);

					long ticks = (long)result.Properties["lockoutTime"][0];
					Debug.WriteLine(string.Format("{0} locked out at {1}", result.Properties["name"][0], DateTime.FromFileTime(ticks)));

#if (DEBUG)
					foreach ( string prop in result.Properties.PropertyNames )
					{
						Debug.WriteLine("  " + prop + ": " + result.Properties[prop][0]);
					}
#endif

					if ( result.Properties["samaccountname"][0].ToString() != textUserName.Text )
						continue;

					string caption = "ACCOUNT LOCKOUT";
					string message = result.Properties["displayname"][0] + " はアカウントロックされています\r\n" +
									 "ロックアウトされた日時: " + DateTime.FromFileTime(ticks);
					if ( Program.expertMode )
					{
						message += "\r\n解除しますか？";
						if ( MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes )
						{
							DirectoryEntry uEntry = new DirectoryEntry(result.Path);
							uEntry.Properties["LockOutTime"].Value = 0; //unlock account
							uEntry.CommitChanges(); //may not be needed but adding it anyways
							uEntry.Close();
						}
					}
					else
					{
						MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}

					lockouted = true;
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
#if (DEBUG)
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
			}
			finally
			{
				if ( entry != null )
				{
					entry.Dispose();
					entry = null;
				}
			}

			return lockouted;
		}
	}
}
