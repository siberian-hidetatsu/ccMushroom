namespace ProgUpdateClass
{
    partial class ProgUpdateMessage
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgUpdateMessage));
			this.radioUpdateNotOverwrite = new System.Windows.Forms.RadioButton();
			this.labelTitle = new System.Windows.Forms.Label();
			this.labelVersionMessage = new System.Windows.Forms.Label();
			this.radioUpdateOverwrite = new System.Windows.Forms.RadioButton();
			this.radioNoUpdate = new System.Windows.Forms.RadioButton();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelStatus = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.buttonStretch = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// radioUpdateNotOverwrite
			// 
			this.radioUpdateNotOverwrite.Checked = true;
			this.radioUpdateNotOverwrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioUpdateNotOverwrite.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.radioUpdateNotOverwrite.Location = new System.Drawing.Point(8, 88);
			this.radioUpdateNotOverwrite.Name = "radioUpdateNotOverwrite";
			this.radioUpdateNotOverwrite.Size = new System.Drawing.Size(360, 24);
			this.radioUpdateNotOverwrite.TabIndex = 3;
			this.radioUpdateNotOverwrite.TabStop = true;
			this.radioUpdateNotOverwrite.Text = "現在の共通設定を保持したままプログラムを更新する";
			this.toolTip.SetToolTip(this.radioUpdateNotOverwrite, "変更した共通設定の内容は引き継がれます");
			// 
			// labelTitle
			// 
			this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.labelTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelTitle.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelTitle.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.labelTitle.Image = ((System.Drawing.Image)(resources.GetObject("labelTitle.Image")));
			this.labelTitle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelTitle.Location = new System.Drawing.Point(8, 8);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(352, 34);
			this.labelTitle.TabIndex = 6;
			this.labelTitle.Text = "　　　新しいバージョンのプログラムをダウンロードできます";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVersionMessage
			// 
			this.labelVersionMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelVersionMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.labelVersionMessage.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.labelVersionMessage.Location = new System.Drawing.Point(8, 48);
			this.labelVersionMessage.Name = "labelVersionMessage";
			this.labelVersionMessage.Size = new System.Drawing.Size(352, 35);
			this.labelVersionMessage.TabIndex = 7;
			this.labelVersionMessage.Text = "新しいプログラムのバージョン：1.0.0.0 FlatStyle=System 現在のプログラムのバージョン：1.0.0.0 だとW2Kで文字化け";
			// 
			// radioUpdateOverwrite
			// 
			this.radioUpdateOverwrite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioUpdateOverwrite.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.radioUpdateOverwrite.Location = new System.Drawing.Point(8, 112);
			this.radioUpdateOverwrite.Name = "radioUpdateOverwrite";
			this.radioUpdateOverwrite.Size = new System.Drawing.Size(360, 24);
			this.radioUpdateOverwrite.TabIndex = 4;
			this.radioUpdateOverwrite.Text = "現在の共通設定を初期値に戻してプログラムを更新する";
			this.toolTip.SetToolTip(this.radioUpdateOverwrite, "全ての共通設定の内容をデフォルトにします");
			// 
			// radioNoUpdate
			// 
			this.radioNoUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.radioNoUpdate.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.radioNoUpdate.Location = new System.Drawing.Point(8, 136);
			this.radioNoUpdate.Name = "radioNoUpdate";
			this.radioNoUpdate.Size = new System.Drawing.Size(208, 24);
			this.radioNoUpdate.TabIndex = 5;
			this.radioNoUpdate.Text = "今回はプログラムを更新しない";
			this.toolTip.SetToolTip(this.radioNoUpdate, "プログラムの更新処理をスキップします");
			// 
			// buttonOk
			// 
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOk.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonOk.Location = new System.Drawing.Point(224, 160);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(64, 32);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonCancel.Location = new System.Drawing.Point(296, 160);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(64, 32);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "スキップ";
			// 
			// labelStatus
			// 
			this.labelStatus.BackColor = System.Drawing.Color.Gray;
			this.labelStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStatus.ForeColor = System.Drawing.Color.White;
			this.labelStatus.Location = new System.Drawing.Point(8, 178);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(208, 14);
			this.labelStatus.TabIndex = 8;
			this.labelStatus.Text = "プログラムは自動的に再起動されます";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonStretch
			// 
			this.buttonStretch.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonStretch.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.buttonStretch.Location = new System.Drawing.Point(202, 160);
			this.buttonStretch.Name = "buttonStretch";
			this.buttonStretch.Size = new System.Drawing.Size(14, 14);
			this.buttonStretch.TabIndex = 2;
			this.toolTip.SetToolTip(this.buttonStretch, "詳細なオプションを表示します");
			this.buttonStretch.Click += new System.EventHandler(this.buttonStretch_Click);
			// 
			// ProgUpdateMessage
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 13);
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(370, 200);
			this.ControlBox = false;
			this.Controls.Add(this.buttonStretch);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.labelVersionMessage);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.radioUpdateNotOverwrite);
			this.Controls.Add(this.radioUpdateOverwrite);
			this.Controls.Add(this.radioNoUpdate);
			this.Controls.Add(this.buttonCancel);
			this.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "ProgUpdateMessage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "The program of the new version available.";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgUpdateMessage_FormClosing);
			this.Load += new System.EventHandler(this.ProgUpdateMessage_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioUpdateNotOverwrite;
        private System.Windows.Forms.Label labelVersionMessage;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.RadioButton radioNoUpdate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.RadioButton radioUpdateOverwrite;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button buttonStretch;
    }
}