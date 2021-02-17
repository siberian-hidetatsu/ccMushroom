using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using CommonFunctions;

namespace ProgUpdateClass
{
    public partial class ProgUpdateMessage : Form
    {
        private Rectangle mainFormRectangle;
        private bool simpleForm = true;
        private int stretchHeight;
        private System.Threading.Timer timerAutoWindowClose = null;

        /// <summary>
        /// ProgUpdateMessage のコンストラクタ
        /// </summary>
        public ProgUpdateMessage(Rectangle mainFormRectangle, string messVersion, int autoWindowCloseTime)
        {
			try
			{
				InitializeComponent();

				this.mainFormRectangle = mainFormRectangle;

				string[] messVersions = messVersion.Split(',');
				labelVersionMessage.Text = messVersions[0];
				if ( 2 <= messVersions.Length )
				{
					toolTip.SetToolTip(labelVersionMessage, messVersions[1]);
				}

				// 自動ウィンドウ クローズ用のタイマ
				if ( autoWindowCloseTime != 0 )
				{
					System.Threading.TimerCallback timerDelegate = new System.Threading.TimerCallback(OnTimerAutoWindowClose);
					timerAutoWindowClose = new System.Threading.Timer(timerDelegate, null, System.Threading.Timeout.Infinite, 0);
					timerAutoWindowClose.Change(autoWindowCloseTime * 1000, System.Threading.Timeout.Infinite);	// SetTimer（周期的なシグナル通知は無効）
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
        }

        /// <summary>
        /// ProgUpdateMessage_Load
        /// </summary>
        private void ProgUpdateMessage_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }

                if ((common.GetOsPlatform() & common.platform.winnt4) != 0)
                {
                    int expand = 20;
                    this.Width += expand;
                    labelTitle.Width += expand;
                    labelVersionMessage.Width += expand;
                    radioUpdateNotOverwrite.Width += expand;
                    radioUpdateOverwrite.Width += expand;
                    radioNoUpdate.Width += expand;
                    labelStatus.Width += expand;
                    buttonStretch.Left += expand;
                    buttonOk.Left += expand;
                    buttonCancel.Left += expand;
                }

                if (!mainFormRectangle.IsEmpty)
                {
                    this.Location = new Point(mainFormRectangle.X + (mainFormRectangle.Width - this.Width) / 2, mainFormRectangle.Y + (mainFormRectangle.Height - this.Height) / 2);
                }

                stretchHeight = buttonOk.Top - radioUpdateOverwrite.Top;
                ChangeFormSize();

                //this.Text = "The " + Assembly.GetEntryAssembly().GetName().Name + " of the new version arrived.";
                this.Text = "The " + Assembly.GetEntryAssembly().GetName().Name + " of the new version available.";

                toolTip.SetToolTip(labelTitle, Application.StartupPath + "\\" + Assembly.GetEntryAssembly().GetName().Name + ".exe");

#if (USE_MULTI_THRED_SPLASH_FORM)
				if ( (common.GetOsPlatform() & common.platform.winxp) == 0 )
				{
					radioNoUpdate.Enabled = false;
					buttonCancel.Enabled = false;
				}
#endif
            }
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

        /// <summary>
        /// ChangeFormSize
        /// </summary>
        private void ChangeFormSize()
        {
            try
            {
                if (simpleForm)
                {
                    radioUpdateNotOverwrite.Checked = true;
                    radioUpdateOverwrite.Visible = false;
                    radioNoUpdate.Visible = false;
                    labelStatus.Top -= stretchHeight;
                    buttonStretch.Top -= stretchHeight;
                    buttonOk.Top -= stretchHeight;
                    buttonCancel.Top -= stretchHeight;
                    this.Height -= stretchHeight;
                    toolTip.SetToolTip(buttonStretch, "詳細なオプションを表示します");
                }
                else
                {
                    radioUpdateOverwrite.Visible = true;
                    radioNoUpdate.Visible = true;
                    labelStatus.Top += stretchHeight;
                    buttonStretch.Top += stretchHeight;
                    buttonOk.Top += stretchHeight;
                    buttonCancel.Top += stretchHeight;
                    this.Height += stretchHeight;
                    toolTip.SetToolTip(buttonStretch, "詳細なオプションを隠します");
                }

                buttonOk.Select();
            }
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

        /// <summary>
        /// radioUpdate_CheckedChanged
        /// </summary>
        private void radioUpdate_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                RadioButton radioUpdate = (RadioButton)sender;

                if (!radioUpdate.Checked)
                    return;

                if (radioUpdate.Name == radioUpdateNotOverwrite.Name ||
                    radioUpdate.Name == radioUpdateOverwrite.Name)
                    labelStatus.Text = "プログラムは自動的に再起動されます";
                else if (radioUpdate.Name == radioNoUpdate.Name)
                    labelStatus.Text = "プログラムを更新せずに続行します";
            }
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

        /// <summary>
        /// buttonStretch_Click
        /// </summary>
        private void buttonStretch_Click(object sender, System.EventArgs e)
        {
			try
			{
				if ( timerAutoWindowClose != null )
				{
					timerAutoWindowClose.Change(System.Threading.Timeout.Infinite, 0);	// KillTimer
					timerAutoWindowClose = null;
				}

				simpleForm = !simpleForm;
				ChangeFormSize();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

        /// <summary>
        /// buttonOk_Click
        /// </summary>
        private void buttonOk_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (radioUpdateNotOverwrite.Checked)
                    this.DialogResult = DialogResult.Yes;
                else if (radioUpdateOverwrite.Checked)
                    this.DialogResult = DialogResult.No;
                else
                    this.DialogResult = DialogResult.Cancel;

	            this.Close();
            }
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
        }

		delegate void AutoWindowClodeDelegate(DialogResult dialogResult);

		/// <summary>
		/// AutoWindowClode
		/// </summary>
		private void AutoWindowClode(DialogResult dialogResult)
		{
            try
            {
                this.DialogResult = dialogResult;

                this.Close();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
		}

		/// <summary>
        /// OnTimerAutoWindowClose
        /// </summary>
        private void OnTimerAutoWindowClose(object obj)
        {
            try
            {
				this.Invoke(new AutoWindowClodeDelegate(AutoWindowClode), new Object[] { DialogResult.Yes });
            }
            catch (Exception exp)
            {
#if (DEBUG)
                MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
				Debug.WriteLine(exp.Message);
            }
        }

		/// <summary>
		/// ProgUpdateMessage_FormClosing
		/// </summary>
		private void ProgUpdateMessage_FormClosing(object sender, FormClosingEventArgs e)
		{
            if (timerAutoWindowClose != null)
            {
                timerAutoWindowClose.Change(System.Threading.Timeout.Infinite, 0);	// KillTimer
                timerAutoWindowClose = null;
            }
		}

        /// <summary>
        /// ProgUpdateMessage_KeyUp
        /// </summary>
        private void ProgUpdateMessage_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
#if (USE_MULTI_THRED_SPLASH_FORM)
			if ( e.Alt && e.KeyCode == Keys.F4 )
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close ();
			}
#endif
        }
    }
}