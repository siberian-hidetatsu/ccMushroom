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
	public partial class SplashForm : Form
	{
		//Splashフォーム
		private static SplashForm _form = null;
		//メインフォーム
		private static Form _mainForm = null;
#if (USE_MULTI_THRED_SPLASH_FORM)
		//Splashを表示するスレッド
		private static System.Threading.Thread _thread = null;
#endif

		private static string _message1;
		private static string _message2;

		private static Font _messageFont = null;
		private static SolidBrush _messageBrush = null;
		private static bool _closing;			// Windows 98 はこのフラグが必要？

		/// <summary>
		/// Splashフォーム
		/// </summary>
		public static SplashForm Form
		{
			get { return _form; }
		}

#if (USE_MULTI_THRED_SPLASH_FORM)
		/// <summary>
		/// Splashフォームを表示する
		/// </summary>
		/// <param name="mainForm">メインフォーム</param>
		public static void ShowSplash(Form mainForm)
		{
			try
			{
				if ( _form != null || _thread != null )
					return;

				Cursor.Current = Cursors.WaitCursor;

				_mainForm = mainForm;
				////メインフォームのActivatedイベントでSplashフォームを消す
				//if ( _mainForm != null )
				//{
				//	_mainForm.Activated += new EventHandler(_mainForm_Activated);
				//}

				_messageFont = new Font ("MS UI Gothic", 9);
				_messageBrush = new SolidBrush (Color.Black);
				_closing = false;

				//スレッドの作成
				_thread = new System.Threading.Thread(new System.Threading.ThreadStart(StartThread));
				_thread.Name = "SplashForm";
				_thread.IsBackground = true;
				//_thread.ApartmentState = System.Threading.ApartmentState.STA;
				_thread.SetApartmentState(System.Threading.ApartmentState.STA);
				//スレッドの開始
				_thread.Start();

				while ( _form == null )
				{
					System.Threading.Thread.Sleep (1);
				}

				Cursor.Current = Cursors.Default ;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// スレッドで開始するメソッド
		/// </summary>
		private static void StartThread()
		{
			try
			{
				//Splashフォームを作成
				_form = new SplashForm();
				//Splashフォームをクリックして閉じられるようにする
				_form.Click += new EventHandler(_form_Click);
				//Splashフォームを表示する
				Application.Run(_form);
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// SplashForm コンストラクタ
		/// </summary>
		public SplashForm()
		{
			InitializeComponent();

			try
			{
				_form.progressBar.Style = ProgressBarStyle.Marquee;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#else
		/// <summary>
		/// SplashForm コンストラクタ
		/// </summary>
		public SplashForm(Form mainForm)
		{
			InitializeComponent();

			try
			{
				_mainForm = mainForm;
				_form = this;
				_form.progressBar.Style = ProgressBarStyle.Marquee;

				_messageFont = new Font("MS UI Gothic", 9);
				_messageBrush = new SolidBrush(Color.Black);
				_closing = false;
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}
#endif

		/// <summary>
		/// Splashフォームを消す
		/// </summary>
		public static void CloseSplash()
		{
			try
			{
				if ( _closing )
				{
					SaveCcConfigurationLog("Already closing");
					return;
				}

				_closing = true;

				SaveCcConfigurationLog("");
				SaveCcConfigurationLog("Start CloseSplash");

				if ( _mainForm != null )
				{
					//SaveCcConfigurationLog("Start _MainForm_Activated");
					//_mainForm.Activated -= new EventHandler(_mainForm_Activated);
					////メインフォームをアクティブにする
					_mainForm.Activate();
					//SaveCcConfigurationLog("End _MainForm_Activated");
				}

				if ( _form != null && _form.IsDisposed == false )
				{
					//Splashフォームを閉じる
					//Invokeが必要か調べる
					if ( _form.InvokeRequired )
					{
						SaveCcConfigurationLog("Start _form.Invoke(new MethodInvoker(_form.Close))");
						_form.Invoke(new MethodInvoker(_form.Close));
						SaveCcConfigurationLog("End _form.Invoke(new MethodInvoker(_form.Close))");
					}
					else
					{
						SaveCcConfigurationLog("Start _form.Close");
						_form.Close();
						SaveCcConfigurationLog("End _form.Close");
					}
				}

				//if ( _mainForm != null )
				//{
				//	SaveCcConfigurationLog("Start _MainForm_Activated");
				//	_mainForm.Activated -= new EventHandler(_mainForm_Activated);
				//	//メインフォームをアクティブにする
				//	_mainForm.Activate();
				//	SaveCcConfigurationLog("End _MainForm_Activated");
				//}

				_message1 = string.Empty;
				_message2 = string.Empty;

				if ( _messageFont != null )
				{
					_messageFont.Dispose();
					_messageFont = null;
				}

				if ( _messageBrush != null )
				{
					_messageBrush.Dispose();
					_messageBrush = null;
				}

				_form = null;
#if (USE_MULTI_THRED_SPLASH_FORM)
				_thread = null;
#endif
				_mainForm = null;
				SaveCcConfigurationLog("End CloseSplash");
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// Splashフォームがクリックされた時
		/// </summary>
		private static void _form_Click(object sender, EventArgs e)
		{
			// Splashフォームを閉じる
			CloseSplash();
		}

		/// <summary>
		/// メインフォームがアクティブになった時
		/// タブ ページが追加された時点でこのイベントが発生する？ので直接 CloseSplash() をメインから呼ぶようにした
		/// </summary>
		private static void _mainForm_Activated(object sender, EventArgs e)
		{
			// Splashフォームを閉じる
			SaveCcConfigurationLog("");
			SaveCcConfigurationLog("_mainForm_Activated");
			CloseSplash();
		}

		/// <summary>
		/// SplashForm_Load
		/// pictureBox.BackgroundImage を設定するとアニメーション GIF はアニメーションしない
		/// </summary>
		private void SplashForm_Load(object sender, System.EventArgs e)
		{
			try
			{
				_mainForm.AddOwnedForm(_form);

				// メイン フォームの位置とサイズ
#if true
				Rectangle mainFormRectangle = ccMushroom.windowRectangle;
				if ( mainFormRectangle != Rectangle.Empty )
#else
				string iniFileName = Application.StartupPath + ccMushroom.CCMUSHROOM_INI_FILE_NAME;
				Rectangle mainFormRectangle;
				if ( (mainFormRectangle = ccMushroom.GetWindowRectangle(iniFileName)) != Rectangle.Empty )
#endif
				{
					_form.Location = new Point(mainFormRectangle.X + (mainFormRectangle.Width - _form.Width) / 2, mainFormRectangle.Y + (mainFormRectangle.Height - _form.Height) / 2);
				}

				// パネルの枠線をフォームサイズに合わせる
				// OnPaint をオーバーライドして線を引こうとしたが出来なかった（マルチスレッドにしてるから？）のでパネルの外枠でごまかす
				panel1.Location = new Point(0, 0);
				panel1.Size = new Size(_form.Size.Width, _form.Size.Height);
				panel2.Location = new Point(1, 1);
				panel2.Size = new Size(panel1.Size.Width - 2, panel1.Size.Height - 2);

				labelMessage1.Text = string.Empty;
				labelMessage2.Text = string.Empty;

				//label.Click += new EventHandler(_form_Click);

#if (USE_MULTI_THRED_SPLASH_FORM)
				labelMultiThreadIndicator.Enabled = false;
				labelMultiThreadIndicator.Location = new Point (_form.Size.Width - labelMultiThreadIndicator.Width - 4, -1);
#else
				labelMultiThreadIndicator.Visible = false;
#endif
			}
			catch ( Exception exp )
			{
				MessageBox.Show(exp.Message, MethodBase.GetCurrentMethod().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// InitProgressBar
		/// </summary>
		public static void InitProgressBar(int min, int max, int step, int value)
		{
			try
			{
				if ( _form == null/* || _thread == null*/ )
					return;

				// ProgressBarStyle.Marquee にしたので不要
				/*_form.progressBar.Minimum = min;
				_form.progressBar.Maximum = max;
				_form.progressBar.Step = step;
				_form.progressBar.Value = value;*/
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// ProgressStepIt
		/// </summary>
		public static void ProgressStepIt(string message1, string message2)
		{
			try
			{
				if ( _form == null/* || _thread == null*/ )
					return;
#if false
				_form.labelMessage1.Text = (20 < message1.Length)? message1.Substring(0, 20) + "...": message1;
				_form.labelMessage2.Text = message2;
#else
				_message1 = message1;
				_message2 = message2;
				_form.labelMessage1.Invalidate();
				_form.labelMessage2.Invalidate();
#if (!USE_MULTI_THRED_SPLASH_FORM)
				Application.DoEvents();
#endif
#if (DEBUG)
				//System.Threading.Thread.Sleep (50);
#endif
#endif

				// ProgressBarStyle.Marquee にしたので不要
				/*_form.progressBar.PerformStep();
				if ( _form.progressBar.Value == 100 )
				{
					_form.progressBar.Value = 0;
				}*/
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// labelMessage_Paint
		/// </summary>
		private void labelMessage_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			try
			{
				if ( (_form == null/* || _thread == null*/) || (_messageFont == null || _messageBrush == null) )
					return;

				string message = string.Empty;
				StringFormat formatText = new StringFormat();
				if ( ((Label)sender).Name == "labelMessage1" )
				{
					message = _message1;
					formatText.Trimming = StringTrimming.EllipsisPath;
				}
				else if ( ((Label)sender).Name == "labelMessage2" )
				{
					message = _message2;
					formatText.Trimming = StringTrimming.EllipsisCharacter/*StringTrimming.EllipsisPath*/;
				}

				//Font messageFont = new Font ("MS UI Gothic", 9);
				SizeF sizeText = e.Graphics.MeasureString(message, _messageFont);
				//SolidBrush brush = new SolidBrush (Color.Black);
				RectangleF rectText = new RectangleF(0, 3, ((Label)sender).Size.Width, sizeText.Height + 1);
				e.Graphics.DrawString(message, _messageFont, _messageBrush, rectText, formatText);
				//brush.Dispose ();
				//messageFont.Dispose ();
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
		}

		/// <summary>
		/// SaveCcConfigurationLog
		/// </summary>
		private static void SaveCcConfigurationLog(string message)
		{
#if false
			try
			{
				string fileCcConfigurationLog = Application.StartupPath + "\\" + ccMushroom.CC_CONFIGURATION_LOG_FILE_NAME;

				using ( StreamWriter sw = new StreamWriter(fileCcConfigurationLog, true) )
				{
					sw.Write(string.Format ("{0,3:##0}", DateTime.Now.Millisecond) + " " + message + "\r\n");
					sw.Close ();
				}
			}
			catch ( Exception exp )
			{
				Debug.WriteLine(exp.Message);
			}
#endif
		}

	}
}