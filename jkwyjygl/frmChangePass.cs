using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace jkwyjygl
{
    public partial class frmChangePass : DevExpress.XtraEditors.XtraForm
    {
        wheda.db.dboper mydb;

        public frmChangePass()
        {
            InitializeComponent();

            mydb = new wheda.db.dboper();
            mydb.doconnnect();
        }

        private void xsbok_Click(object sender, EventArgs e)
        {
            if (xtenewpass.Text != xtenewpass2.Text)
            {
                xlcmsg.Text="两次输入的新密码不一致！";
                return;
            }

           
            if (!mydb.verifyuser(Form1.uid, xteoldpass.Text))
            {
                xlcmsg.Text = "原密码错误！";
                return;

            }

            mydb.changepassword(Form1.uid, xtenewpass2.Text);

            MessageBox.Show( "修改密码成功！","提示");
            this.Close();
        }

        private void frmChangePass_Load(object sender, EventArgs e)
        {
           
        }

        private void frmChangePass_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { mydb.finalclose(); }
            finally { }
        }
    }
}