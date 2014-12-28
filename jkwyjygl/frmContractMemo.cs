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
    public partial class frmContractMemo : DevExpress.XtraEditors.XtraForm
    {
        public DataRow dr = null;
        public frmContractMemo()
        {
            InitializeComponent();
        }

        private void frmContractMemo_Load(object sender, EventArgs e)
        {
            xmecontract.Text = dr["contracttext"].ToString();
        }

        private void xsbsavememo_Click(object sender, EventArgs e)
        {
            wheda.db.dboper mydb = new wheda.db.dboper();

            dr["contracttext"]=xmecontract.Text;

            mydb.updatecontracttext(dr);

            mydb.finalclose();
        }
    }
}