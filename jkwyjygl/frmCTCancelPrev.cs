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
    public partial class frmCTCancelPrev : DevExpress.XtraEditors.XtraForm
    {

        string sxlsfile;
        public DataRow drcontract = null;
        public DevExpress.XtraSplashScreen.SplashScreenManager XScmWF;
        wheda.db.dboper mydb;
        public string sfncmon = "";

        public frmCTCancelPrev()
        {
            InitializeComponent();

            mydb = new wheda.db.dboper();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_ppunit);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppfeemgt);

        }

        private void xdeconppsdt_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void xgvct_ppunit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            if (dr == null) return;


            xgcppfeemgt.DataSource = mydb.getppfeepayfnc(dr);
        }

        private void frmCTCancelPrev_FormClosed(object sender, FormClosedEventArgs e)
        {
            mydb.finalclose();
        }

        private void xsbexpconpptoxls_Click(object sender, EventArgs e)
        {
            _gc.DataSource = xgvppfeemgt.DataSource;

            //_gc.Columns[0].Width = 150;

            sxlsfile = mydb.exportgvtoxls(_gv);

            //sfile=System.IO.Path.GetTempPath()+System.DateTime.Now.ToString("HHmmssfff")+".xls"; 
            //xgvxls.ExportToXls(sfile);

            //System.Diagnostics.Process.Start(sfile);

            xsbimpconppfromxls.Enabled = true;
        }

        private void xsbppfeepaymodify_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvppfeemgt.GetFocusedDataRow();


            if (dr == null) return;

            string a1 = dr["bfee"].ToString();
            string a2 = dr["rentfee"].ToString();

            dr["bfee"] = xteppbfeepay.Text;
            dr["rentfee"] = xtepprentfeepay.Text;

            int aa = mydb.updateppfeepaymgt_adjust(dr);

            if (aa == -1)
            {
                MessageBox.Show("该月财务已经录入收费，无法修改！", "错误提示");

                dr["bfee"] = a1;

                dr["rentfee"] = a2;

                return;
            }
        }

        private void xgvppfeemgt_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvppfeemgt.GetFocusedDataRow();

            if (dr == null) return;


            xteppbfeepay.Text = dr["bfee"].ToString();
            xtepprentfeepay.Text = dr["rentfee"].ToString();
        }

        private void xsbimpconppfromxls_Click(object sender, EventArgs e)
        {
            xgcppfeemgt.DataSource = mydb.importxlstodatatable(sxlsfile);

            xsbsaveconpp.Enabled = true;
        }

        private void xsbsaveconpp_Click(object sender, EventArgs e)
        {
            int aa = 0;

            DialogResult dr1 = MessageBox.Show("确定要保存该房间月租吗？ ", "批量操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }


            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在保存房间月租...");

            DataTable dt = ((DataView)xgvppfeemgt.DataSource).Table;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["feeid"].ToString() != "")
                {
                    aa = mydb.updatefeepaymgt_adjust(dr);
                }

            }

            XScmWF.CloseWaitForm();

            if (aa == -1)
            {
                MessageBox.Show("有部分月份财务已经录入收费，并未保存！", "重要提示");

            }
        }

        private void frmCTCancelPrev_Load(object sender, EventArgs e)
        {
            lblfncmon.Text = "财务已录入收费至：" + sfncmon;
        }

        private void xsbcancelct_Click(object sender, EventArgs e)
        {
            if (xdectcanceldt.EditValue == null)
            {
                MessageBox.Show("请选择提前终止日期", "错误提示");

                return;
            }

            string sctmonstart = drcontract["ContractSDT"].ToString();
            string sctmonend = drcontract["ContractEDt"].ToString();

            string scancelmon = xdectcanceldt.DateTime.ToString("yyyyMMdd");

            if (string.Compare(scancelmon, sctmonend) > 0)
            {
                MessageBox.Show("终止日期大于合同结束日期!", "错误提示");

                return;
            }

            int imon = Convert.ToInt32(scancelmon.Substring(0, 6));
            int ifmon = Convert.ToInt32(sfncmon);

            if (imon < ifmon)
            {
                MessageBox.Show("终止月份小于财务录入收费的最大月份，请联系财务删除录入的收费先!", "错误提示");

                return;
            }

            //非1号合同
            if (sctmonstart.Substring(6, 2) != "01")
            {
                int itmp = Convert.ToInt16(sctmonstart.Substring(6, 2));
                int itmp1 = Convert.ToInt16(scancelmon.Substring(6, 2));

                if (itmp != (itmp1 + 1))
                {
                    MessageBox.Show("提前终止日期与合同开始日期不构成一个整月!", "重要提示");

                    {
                        return;
                    }
                }
                else
                {
                    //删除t_fee_pay_mgt 大于终止月份的数据，并将最后月的数据调整月份（财务拆分的情况）

                    DialogResult dr1 = MessageBox.Show("将执行提前终止合同操作，确定吗？", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (dr1 == DialogResult.Cancel)
                    {
                        return;
                    }

                    DataTable dt1 = ((DataView)xgvct_ppunit.DataSource).Table;
                    foreach (DataRow dr in dt1.Rows)
                    {
                        if (dr.RowState == DataRowState.Deleted) continue;

                        mydb.docancelctprevNo1(drcontract, dr, scancelmon);
                        mydb.createfeepaymgt_period_cancel(drcontract, dr, scancelmon, 0);

                        //更新财务拆分月应收
                        mydb.updatefncsplitfeepay(drcontract, dr);

                    }



                }

                MessageBox.Show("提前终止合同--操作成功!", "提示");

            }
            else //1号合同
            {
               
                DialogResult dr1 = MessageBox.Show("将执行提前终止合同操作，确定吗？", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (dr1 == DialogResult.Cancel)
                {
                    return;
                }

                DataTable dt1 = ((DataView)xgvct_ppunit.DataSource).Table;
                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;

                    mydb.docancelctprev1(drcontract, dr, scancelmon);
                    mydb.createfeepaymgt_period_cancel(drcontract, dr, scancelmon, 1);

                   
                }

                MessageBox.Show("提前终止合同--操作成功!", "提示");

            }

            mydb.addalt(drcontract, "提前终止");

            Close();
        }

        private void bbicreatefeeperiod_Click(object sender, EventArgs e)
        {

        }
    }
}