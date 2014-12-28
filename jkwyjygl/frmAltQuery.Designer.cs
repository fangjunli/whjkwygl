namespace jkwyjygl
{
    partial class frmAltQuery
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
            this.xgcalt = new DevExpress.XtraGrid.GridControl();
            this.xgvalt = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.xgcalt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xgvalt)).BeginInit();
            this.SuspendLayout();
            // 
            // xgcalt
            // 
            this.xgcalt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xgcalt.Location = new System.Drawing.Point(0, 0);
            this.xgcalt.MainView = this.xgvalt;
            this.xgcalt.Name = "xgcalt";
            this.xgcalt.Size = new System.Drawing.Size(786, 402);
            this.xgcalt.TabIndex = 6;
            this.xgcalt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.xgvalt});
            this.xgcalt.DoubleClick += new System.EventHandler(this.xgcalt_DoubleClick);
            // 
            // xgvalt
            // 
            this.xgvalt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7});
            this.xgvalt.FixedLineWidth = 1;
            this.xgvalt.GridControl = this.xgcalt;
            this.xgvalt.Name = "xgvalt";
            this.xgvalt.OptionsBehavior.Editable = false;
            this.xgvalt.OptionsBehavior.ReadOnly = true;
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "变更人";
            this.gridColumn1.FieldName = "operuser";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "变更时间";
            this.gridColumn2.FieldName = "altdt";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "变更描述";
            this.gridColumn3.FieldName = "altmsg";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "房间编号";
            this.gridColumn4.FieldName = "unitno";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "合同编号";
            this.gridColumn5.FieldName = "contractno";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "客户编号";
            this.gridColumn6.FieldName = "cusno";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "用户名";
            this.gridColumn7.FieldName = "username";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            // 
            // frmAltQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 402);
            this.Controls.Add(this.xgcalt);
            this.Name = "frmAltQuery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "变更查询";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAltQuery_FormClosed);
            this.Load += new System.EventHandler(this.frmAltQuery_Load);
            ((System.ComponentModel.ISupportInitialize)(this.xgcalt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xgvalt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraGrid.GridControl xgcalt;
        public DevExpress.XtraGrid.Views.Grid.GridView xgvalt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
    }
}