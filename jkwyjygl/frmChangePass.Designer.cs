namespace jkwyjygl
{
    partial class frmChangePass
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChangePass));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.xtenewpass2 = new DevExpress.XtraEditors.TextEdit();
            this.xtenewpass = new DevExpress.XtraEditors.TextEdit();
            this.xteoldpass = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.xlcmsg = new DevExpress.XtraEditors.LabelControl();
            this.xsbok = new DevExpress.XtraEditors.SimpleButton();
            this.ribbonImageCollectionLarge = new DevExpress.Utils.ImageCollection(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.xtenewpass2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtenewpass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xteoldpass.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollectionLarge)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.xlcmsg);
            this.splitContainerControl1.Panel2.Controls.Add(this.xsbok);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(225, 193);
            this.splitContainerControl1.SplitterPosition = 146;
            this.splitContainerControl1.TabIndex = 0;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.xtenewpass2);
            this.layoutControl1.Controls.Add(this.xtenewpass);
            this.layoutControl1.Controls.Add(this.xteoldpass);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(535, 143, 250, 350);
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(225, 146);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // xtenewpass2
            // 
            this.xtenewpass2.Location = new System.Drawing.Point(76, 114);
            this.xtenewpass2.Name = "xtenewpass2";
            this.xtenewpass2.Properties.PasswordChar = '*';
            this.xtenewpass2.Size = new System.Drawing.Size(137, 20);
            this.xtenewpass2.StyleController = this.layoutControl1;
            this.xtenewpass2.TabIndex = 2;
            // 
            // xtenewpass
            // 
            this.xtenewpass.Location = new System.Drawing.Point(76, 90);
            this.xtenewpass.Name = "xtenewpass";
            this.xtenewpass.Properties.PasswordChar = '*';
            this.xtenewpass.Size = new System.Drawing.Size(137, 20);
            this.xtenewpass.StyleController = this.layoutControl1;
            this.xtenewpass.TabIndex = 1;
            // 
            // xteoldpass
            // 
            this.xteoldpass.Location = new System.Drawing.Point(76, 12);
            this.xteoldpass.Name = "xteoldpass";
            this.xteoldpass.Properties.PasswordChar = '*';
            this.xteoldpass.Size = new System.Drawing.Size(137, 20);
            this.xteoldpass.StyleController = this.layoutControl1;
            this.xteoldpass.TabIndex = 0;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "Root";
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Size = new System.Drawing.Size(225, 146);
            this.layoutControlGroup1.Text = "Root";
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.xteoldpass;
            this.layoutControlItem1.CustomizationFormText = "原密码";
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(205, 24);
            this.layoutControlItem1.Text = "原密码";
            this.layoutControlItem1.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.xtenewpass;
            this.layoutControlItem2.CustomizationFormText = "新密码";
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 78);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(205, 24);
            this.layoutControlItem2.Text = "新密码";
            this.layoutControlItem2.TextSize = new System.Drawing.Size(60, 14);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.xtenewpass2;
            this.layoutControlItem3.CustomizationFormText = "确认新密码";
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 102);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(205, 24);
            this.layoutControlItem3.Text = "确认新密码";
            this.layoutControlItem3.TextSize = new System.Drawing.Size(60, 14);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 24);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(205, 54);
            this.emptySpaceItem1.Text = "emptySpaceItem1";
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // xlcmsg
            // 
            this.xlcmsg.Location = new System.Drawing.Point(12, 16);
            this.xlcmsg.Name = "xlcmsg";
            this.xlcmsg.Size = new System.Drawing.Size(0, 14);
            this.xlcmsg.TabIndex = 1;
            // 
            // xsbok
            // 
            this.xsbok.ImageIndex = 2;
            this.xsbok.ImageList = this.ribbonImageCollectionLarge;
            this.xsbok.Location = new System.Drawing.Point(172, 5);
            this.xsbok.Name = "xsbok";
            this.xsbok.Size = new System.Drawing.Size(41, 33);
            this.xsbok.TabIndex = 0;
            this.xsbok.ToolTip = "确定";
            this.xsbok.Click += new System.EventHandler(this.xsbok_Click);
            // 
            // ribbonImageCollectionLarge
            // 
            this.ribbonImageCollectionLarge.ImageSize = new System.Drawing.Size(32, 32);
            this.ribbonImageCollectionLarge.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("ribbonImageCollectionLarge.ImageStream")));
            this.ribbonImageCollectionLarge.Images.SetKeyName(0, "OK.png");
            this.ribbonImageCollectionLarge.Images.SetKeyName(1, "cancel.png");
            this.ribbonImageCollectionLarge.Images.SetKeyName(2, "accept.png");
            // 
            // frmChangePass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 193);
            this.Controls.Add(this.splitContainerControl1);
            this.Name = "frmChangePass";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修改密码";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmChangePass_FormClosed);
            this.Load += new System.EventHandler(this.frmChangePass_Load);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.xtenewpass2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtenewpass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xteoldpass.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonImageCollectionLarge)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.TextEdit xtenewpass2;
        private DevExpress.XtraEditors.TextEdit xtenewpass;
        private DevExpress.XtraEditors.TextEdit xteoldpass;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.SimpleButton xsbok;
        private DevExpress.Utils.ImageCollection ribbonImageCollectionLarge;
        private DevExpress.XtraEditors.LabelControl xlcmsg;
    }
}