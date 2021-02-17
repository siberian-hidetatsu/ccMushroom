using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ccMushroom
{
	public partial class ExitDlg : Form
	{
		private string appTitle;
		private int exitDialogCountDown;

		private int countDown;
		private bool mouseEntered = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExitDlg(string _appTitle, int _exitDialogCountDown)
		{
			InitializeComponent();

			this.appTitle = _appTitle;
			this.exitDialogCountDown = _exitDialogCountDown;
		}

		/// <summary>
		/// ExitDlg_Load
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitDlg_Load(object sender, EventArgs e)
		{
			try
			{
				this.buttonExit.Location = new Point(-1, -1);
				this.Size = new Size(buttonExit.Size.Width - 2, buttonExit.Size.Height - 2);
				this.Location = new Point(Cursor.Position.X - this.Size.Width, Cursor.Position.Y - this.Size.Height);

				countDown = exitDialogCountDown;
				SetButtonExitToolTip();

				timer.Interval = 1000;
				timer.Start();

				this.Activate();
			}
			catch ( Exception exp )
			{
				System.Diagnostics.Debug.WriteLine(exp.Message);
				this.Close();
			}
		}

		/// <summary>
		/// SetButtonExitToolTip
		/// </summary>
		private void SetButtonExitToolTip()
		{
			toolTip.SetToolTip(this.buttonExit, appTitle + " を終了する (..." + countDown + ")");
		}

		/// <summary>
		/// ExitDlg_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitDlg_Click(object sender, EventArgs e)
		{
			timer.Stop();
		}

		/// <summary>
		/// buttonExit_Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonExit_Click(object sender, EventArgs e)
		{
			this.Owner.Close();
		}

		/// <summary>
		/// timer_Tick
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_Tick(object sender, EventArgs e)
		{
			try
			{
				if ( mouseEntered )
					return;

				if ( --countDown == 0 )
				{
					timer.Stop();
					this.Close();
					return;
				}

				SetButtonExitToolTip();
				this.Opacity = (double)countDown / (double)exitDialogCountDown;
			}
			catch ( Exception exp )
			{
				System.Diagnostics.Debug.WriteLine(exp.Message);
				timer.Stop();
			}
		}

		/// <summary>
		/// buttonExit_MouseEnter
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonExit_MouseEnter(object sender, EventArgs e)
		{
			mouseEntered = true;
			this.Opacity = 100;
		}

		/// <summary>
		/// buttonExit_MouseLeave
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonExit_MouseLeave(object sender, EventArgs e)
		{
			try
			{
				mouseEntered = false;
				this.Opacity = (double)countDown / (double)exitDialogCountDown;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
	}
}
