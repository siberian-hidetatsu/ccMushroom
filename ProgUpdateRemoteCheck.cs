using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ProgUpdateClass
{
	public partial class ProgUpdateRemoteCheck : Form
	{
		private string remoteProgramFileName;
		private Rectangle mainFormRectangle;
		private Size defaultFormSize;

		public DateTime remoteDateTime = DateTime.MinValue;
		public string errorMessage;

		public ProgUpdateRemoteCheck(string remoteProgramFileName, Rectangle mainFormRectangle)
		{
			InitializeComponent();

			this.remoteProgramFileName = remoteProgramFileName;
			this.mainFormRectangle = mainFormRectangle;
		}

		private void ProgUpdateRemoteCheck_Load(object sender, EventArgs e)
		{
			defaultFormSize = this.Size;
			Size zeroSize = new Size(0, 0);
			this.Size = zeroSize;

			if ( !mainFormRectangle.IsEmpty )
			{
				this.Location = new Point(mainFormRectangle.X + (mainFormRectangle.Width - defaultFormSize.Width) / 2, mainFormRectangle.Y + (mainFormRectangle.Height - defaultFormSize.Height) / 2);
			}

			progressBar.Dock = DockStyle.Fill;

			backgroundWorker.RunWorkerAsync();
			timer.Start();
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				remoteDateTime = File.GetLastWriteTime(remoteProgramFileName);
			}
			catch ( Exception exp )
			{
				errorMessage = exp.Message;
			}
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			timer.Stop();
			this.Close();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			this.Size = defaultFormSize;
		}
	}
}