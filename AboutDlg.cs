using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace ccMushroom
{
	public partial class AboutDlg : Form
	{
		/// <summary>
		/// AboutDlg
		/// </summary>
		public AboutDlg()
		{
			InitializeComponent();
		}

		/// <summary>
		/// AboutDlg_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AboutDlg_Load(object sender, EventArgs e)
		{
			// 自分自身の Assembly を取得
			Assembly myAssembly = Assembly.GetExecutingAssembly();
			// バージョンの取得
			Version myVer = myAssembly.GetName().Version;

			DateTime buildDateTime = new DateTime(2000, 1, 1);
			TimeSpan verSpan = new TimeSpan(myVer.Build * TimeSpan.TicksPerDay + myVer.Revision * 2 * TimeSpan.TicksPerSecond);
			buildDateTime += verSpan;

			labelVersion.Text = ccMushroom.appTitle + " version " + myVer.Major + "." + myVer.Minor.ToString("D2") + "." + myVer.Build;
			toolTip.SetToolTip(labelVersion, "Builded: " + buildDateTime.ToString("yyyy/MM/dd HH:mm"));
		}

		/// <summary>
		/// linkLabel1_LinkClicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			string url = null;

			try
			{
				buttonOK.Select();

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
				MessageBox.Show(exp.Message + "\r\n" + url, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		/// <summary>
		/// buttonOK_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}