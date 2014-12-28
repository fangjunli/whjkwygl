namespace jkwyjygl
{
    partial class frmSingleSel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSingleSel));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.xgcsinglesel = new DevExpress.XtraGrid.GridControl();
            this.xgvsinglesel = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.xsbcontractaddok = new DevExpress.XtraEditors.SimpleButton();
            this.ribbonImageCollectionLarge = new DevExpress.Utils.ImageCollection();
            this.xsbcontractaddcancel = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xgcsinglesel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xgvsinglesel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollectionLarge)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.xgcsinglesel);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.xsbcontractaddok);
            this.splitContainerControl1.Panel2.Controls.Add(this.xsbcontractaddcancel);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(741, 469);
            this.splitContainerControl1.SplitterPosition = 396;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // xgcsinglesel
            // 
            this.xgcsinglesel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xgcsinglesel.Location = new System.Drawing.Point(0, 0);
            this.xgcsinglesel.MainView = this.xgvsinglesel;
            this.xgcsinglesel.Name = "xgcsinglesel";
            this.xgcsinglesel.Size = new System.Drawing.Size(741, 396);
            this.xgcsinglesel.TabIndex = 5;
            this.xgcsinglesel.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.xgvsinglesel});
            // 
            // xgvsinglesel
            // 
            this.xgvsinglesel.FixedLineWidth = 1;
            this.xgvsinglesel.GridControl = this.xgcsinglesel;
            this.xgvsinglesel.Name = "xgvsinglesel";
            this.xgvsinglesel.OptionsBehavior.Editable = false;
            this.xgvsinglesel.OptionsBehavior.ReadOnly = true;
            this.xgvsinglesel.OptionsSelection.MultiSelect = true;
            this.xgvsinglesel.DoubleClick += new System.EventHandler(this.xsbcontractaddok_Click);
            // 
            // xsbcontractaddok
            // 
            this.xsbcontractaddok.ImageIndex = 2;
            this.xsbcontractaddok.ImageList = this.ribbonImageCollectionLarge;
            this.xsbcontractaddok.Location = new System.Drawing.Point(655, 17);
            this.xsbcontractaddok.Name = "xsbcontractaddok";
            this.xsbcontractaddok.Size = new System.Drawing.Size(74, 33);
            this.xsbcontractaddok.TabIndex = 4;
            this.xsbcontractaddok.Text = "确定";
            this.xsbcontractaddok.ToolTip = "确定";
            this.xsbcontractaddok.Click += new System.EventHandler(this.xsbcontractaddok_Click);
            // 
            // ribbonImageCollectionLarge
            // 
            this.ribbonImageCollectionLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.ribbonImageCollectionLarge.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("ribbonImageCollectionLarge.ImageStream")));
            this.ribbonImageCollectionLarge.Images.SetKeyName(0, "OK.png");
            this.ribbonImageCollectionLarge.Images.SetKeyName(1, "cancel.png");
            this.ribbonImageCollectionLarge.Images.SetKeyName(2, "accept.png");
            // 
            // xsbcontractaddcancel
            // 
            this.xsbcontractaddcancel.ImageIndex = 1;
            this.xsbcontractaddcancel.ImageList = this.ribbonImageCollectionLarge;
            this.xsbcontractaddcancel.Location = new System.Drawing.Point(49, 17);
            this.xsbcontractaddcancel.Name = "xsbcontractaddcancel";
            this.xsbcontractaddcancel.Size = new System.Drawing.Size(71, 33);
            this.xsbcontractaddcancel.TabIndex = 3;
            this.xsbcontractaddcancel.Text = "取消";
            this.xsbcontractaddcancel.ToolTip = "取消";
            this.xsbcontractaddcancel.Click += new System.EventHandler(this.xsbcontractaddcancel_Click);
            // 
            // frmSingleSel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 469);
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "frmSingleSel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择";
            this.Load += new System.EventHandler(this.frmSingleSel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xgcsinglesel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xgvsinglesel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollectionLarge)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.Utils.ImageCollection ribbonImageCollectionLarge;
        private DevExpress.XtraEditors.SimpleButton xsbcontractaddok;
        private DevExpress.XtraEditors.SimpleButton xsbcontractaddcancel;
        public DevExpress.XtraGrid.GridControl xgcsinglesel;
        public DevExpress.XtraGrid.Views.Grid.GridView xgvsinglesel;
    }
}