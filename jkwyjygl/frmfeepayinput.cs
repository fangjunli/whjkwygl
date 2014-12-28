using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views;

namespace jkwyjygl
{
    public partial class frmfeepayinput : DevExpress.XtraEditors.XtraForm
    {
        public string sfee = "";
        public string sct = "";
        public string sctno = "";
        public string sdt = "";
        public string edt = "";
        public string scus = "";

        public DevExpress.XtraGrid.Views.Grid.GridView gv = null;

        public frmfeepayinput()
        {
            InitializeComponent();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvfeepayedseq);
        }

        private void xsbok_Click(object sender, EventArgs e)
        {
            if (Convert.ToSingle(xtefeepayed.Text)<=0)
            {
                MessageBox.Show("应收应该大于0！");

                return;
            }

            if (Convert.ToSingle( xtefeepay.Text) != Convert.ToSingle( xtefeepayed.Text))
            {
                MessageBox.Show("应收应该等于实收！");
                return;
            }



            DialogResult dr1 = MessageBox.Show("确定要录入收费吗？", "确认录入", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            wheda.db.dboper mydb = new wheda.db.dboper();

            string seq = mydb.addfeepayedseqmgt(xtefeepayed.Text,sct,sctno);


            //DataTable dt=((DataView)gv.DataSource).Table;
            DataRow dr=gv.GetFocusedDataRow();

            //foreach (DataRow dr in dt.Rows)
            {
               mydb.updatefeepayedseqmgt(dr,seq);
            }


            getfeepayedseq();

            mydb.finalclose();
        }

        private void getfeepayedseq()
        {
            wheda.db.dboper mydb = new wheda.db.dboper();

            string str = "select feepayedctseq,contractno,contractid,payeddate,payedfee from t_fee_payed_seq_con_mgt where contractid=" +
                sct+" order by payeddate desc";

            xgcfeepayedseq.DataSource = mydb.gettablebystr(str);

            mydb.finalclose();
        }

        private void frmfeepayinput_Load(object sender, EventArgs e)
        {
            xtefeepay.Text = sfee;
            xdesdtfeepayedmgt.Text = sdt;
            xdeedtfeepayedmgt.Text = edt;
            xtecontractnofeepayedmgt.Text = sctno;
            xtecusname.Text = scus;

            getfeepayedseq();
        }

        private void xsbfile_Click(object sender, EventArgs e)
        {

        }

        private void xsbdelfeepay_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvfeepayedseq.GetFocusedDataRow();

            if (dr == null) return;

            string sseq = dr["feepayedctseq"].ToString();

            DialogResult dr1 = MessageBox.Show("确定要删除该收费记录吗？ ", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }

            wheda.db.dboper mydb = new wheda.db.dboper();

            mydb.delmgtpay(dr);

            mydb.finalclose();

            getfeepayedseq();
        }
    }
}