namespace jkwyjygl
{
    partial class frmContractMemo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.xmecontract = new DevExpress.XtraEditors.MemoEdit();
            this.xsbsavememo = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xmecontract.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.xmecontract);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.xsbsavememo);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(474, 353);
            this.splitContainerControl1.SplitterPosition = 80;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // xmecontract
            // 
            this.xmecontract.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmecontract.Location = new System.Drawing.Point(0, 0);
            this.xmecontract.Name = "xmecontract";
            this.xmecontract.Size = new System.Drawing.Size(389, 353);
            this.xmecontract.TabIndex = 0;
            // 
            // xsbsavememo
            // 
            this.xsbsavememo.Location = new System.Drawing.Point(1, 318);
            this.xsbsavememo.Name = "xsbsavememo";
            this.xsbsavememo.Size = new System.Drawing.Size(76, 23);
            this.xsbsavememo.TabIndex = 0;
            this.xsbsavememo.Text = "保存备注";
            this.xsbsavememo.Click += new System.EventHandler(this.xsbsavememo_Click);
            // 
            // frmContractMemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 353);
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "frmContractMemo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "合同备注";
            this.Load += new System.EventHandler(this.frmContractMemo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xmecontract.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.MemoEdit xmecontract;
        private DevExpress.XtraEditors.SimpleButton xsbsavememo;
    }
}