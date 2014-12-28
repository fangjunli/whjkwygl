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
    public partial class frmAltQuery : DevExpress.XtraEditors.XtraForm
    {
        public string  sid;
        public string stype;
        wheda.db.dboper mydb;

        public frmAltQuery()
        {
            InitializeComponent();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvalt);

            mydb = new wheda.db.dboper();
        }

        private void frmAltQuery_Load(object sender, EventArgs e)
        {
            string s1 = wheda.db.dboper.saltquery;
            switch (stype)
            {
                case "1":
                    s1 += " where a.contractid=" + sid;
                    break;
                case "2":
                    s1 += " where a.ppid=" + sid;
                    break;
                case "3":
                    s1 += " where a.cusid=" + sid;
                    break;
                case "4":
                    s1 += " where a.userid=" + sid;
                    break;
                default:
                    break;
            }
            s1 += " order by altdt desc";

            
            xgcalt.DataSource = mydb.gettablebystr(s1);

//            xgvalt.BestFitColumns();
        }

        private void frmAltQuery_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { mydb.finalclose(); }
            finally { };
        }

        private void xgcalt_DoubleClick(object sender, EventArgs e)
        {
            DataRow dr = xgvalt.GetFocusedDataRow();

            if (dr == null) return;

            MessageBox.Show(dr["altmsg"].ToString(),"变更信息描述");
        }
    }
}