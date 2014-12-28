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
    public partial class frmpayfeeperiodadjust : DevExpress.XtraEditors.XtraForm
    {
        public int irt = 0;
        string sxlsfile;
        public DataRow drcontract = null;
        public DevExpress.XtraSplashScreen.SplashScreenManager XScmWF;
        wheda.db.dboper mydb;


        public frmpayfeeperiodadjust()
        {
            InitializeComponent();

            mydb = new wheda.db.dboper();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_ppunit);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppfeemgt);

            
        }

        private void frmpayfeeperiodadjust_Load(object sender, EventArgs e)
        {
 
        }

        private void xgvpayfeectfee_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
           
        }

        private void xgvct_ppunit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            if (dr == null) return;


            xgcppfeemgt.DataSource = mydb.getppfeepayfnc(dr);
        }

        private void groupControl3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmpayfeeperiodadjust_FormClosed(object sender, FormClosedEventArgs e)
        {
            mydb.finalclose();
        }

        private void xsbcreatefee_Click(object sender, EventArgs e)
        {
            //检查是否已经有数据，提示重新生成

            DialogResult dr1 = MessageBox.Show("将按调整后的合同/房间参数重新生成未录入收费的月租/应收，确定吗？", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }

            if (xrgfeemtype.SelectedIndex == 0)
            {
                if (xgvct_ppunit.RowCount <= 0) return;

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成调整后数据...");


                DataTable dt1 = ((DataView)xgvct_ppunit.DataSource).Table;
                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    mydb.createfeepaymgt_mid(dr, drcontract, Convert.ToSingle(xsedecincvalue.Text),
                                         xcbeincdectype.SelectedIndex, xceentiremonth.Checked);
                }

                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    mydb.createfeepaymgt_period_mid(dr, drcontract, xceentiremonth.Checked);
                }
            }
            else
            {
                DataRow dr = xgvct_ppunit.GetFocusedDataRow();

                if (dr == null) return;

 
                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成应收数据...");

                mydb.createfeepaymgt_mid(dr, drcontract, Convert.ToSingle(xsedecincvalue.Text),
                                     xcbeincdectype.SelectedIndex,
                                     xceentiremonth.Checked);
                mydb.createfeepaymgt_period_mid(dr, drcontract, xceentiremonth.Checked);
            }


            XScmWF.CloseWaitForm();

            xgvct_ppunit_FocusedRowChanged(null, null);
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
                MessageBox.Show("该月财务已经录入收费，无法修改！","错误提示");

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
            int aa=0;

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
                    aa=mydb.updatefeepaymgt_adjust(dr);
                }

            }

            XScmWF.CloseWaitForm();

            if (aa == -1)
            {
                MessageBox.Show("有部分月份财务已经录入收费，并未保存！", "重要提示");

            }

        }

        private void bbicreatefeeperiod_Click(object sender, EventArgs e)
        {
            //检查是否已经有数据，提示重新生成

            DialogResult dr1 = MessageBox.Show("确定重新生成时间段应收？ ", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }

            if (xrgfeemtype.SelectedIndex == 0)
            {
                if (xgvct_ppunit.RowCount <= 0) return;


                mydb.deletepayfeemgt_period(drcontract, null);


                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成应收数据...");


                DataTable dt1 = ((DataView)xgvct_ppunit.DataSource).Table;

                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    mydb.createfeepaymgt_period_mid(dr, drcontract, xceentiremonth.Checked);
                   
                }
            }
            else
            {
                DataRow dr = xgvct_ppunit.GetFocusedDataRow();

                if (dr == null) return;

                mydb.deletepayfeemgt_period(drcontract, dr);



                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成应收数据...");

                mydb.createfeepaymgt_period_mid(dr, drcontract, xceentiremonth.Checked);
               
            }




            XScmWF.CloseWaitForm();

            xgvct_ppunit_FocusedRowChanged(null, null);
        }

        private void xsbimpconppfromxlshdfile_Click(object sender, EventArgs e)
        {

        }

        private void xsbresetfee_Click(object sender, EventArgs e)
        {
            DialogResult dr1 = MessageBox.Show("当使用修改功能生成的费用无法满足实际要求时，可以重置费用，一般情况下请不要这么做。\r\n费用重置不影响财务月度实收数据，但影响财务月度应收数据。r\n当重置费用后，请协调财务重新对月度费用进行拆分，以保证月度应收的正确性。", "确定要重置该合同费用？", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }

            irt = 1;

            Close();

        }



          
    }
}