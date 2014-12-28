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
    public partial class frmContractPayfeeMgt : DevExpress.XtraEditors.XtraForm
    {
        string sxlsfile;
        public DataRow drcontract=null;
        public DevExpress.XtraSplashScreen.SplashScreenManager XScmWF;
        wheda.db.dboper mydb;


        public frmContractPayfeeMgt()
        {
            InitializeComponent();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_ppunit);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppfeemgt);

            mydb = new wheda.db.dboper();
        }

        private void xsbcreatefee_Click(object sender, EventArgs e)
        {
            //检查是否已经有数据，提示重新生成

             
            if (xrgfeemtype.SelectedIndex == 0)
            {
                if (xgvct_ppunit.RowCount <= 0) return;

                if (mydb.checkhasfeepaymgt(drcontract, null))
                {
                    DialogResult dr1 = MessageBox.Show("该合同已经存在应收数据，需要重新生成吗？ ", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (dr1 == DialogResult.Cancel)
                    {
                        return;
                    }

                    mydb.deletepayfeemgt(drcontract, null);
                    mydb.deletepayfeemgt_period(drcontract, null);
                }



                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成应收数据...");


                DataTable dt1 = ((DataView)xgvct_ppunit.DataSource).Table;
                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    mydb.createfeepaymgt(dr, drcontract, Convert.ToInt16(xsenmons.Text),
                                                         Convert.ToSingle(xsedecincvalue.Text),
                                         xcbeincdectype.SelectedIndex,xceentiremonth.Checked);
                }

                foreach (DataRow dr in dt1.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted) continue;
                    mydb.createfeepaymgt_period(dr, drcontract,xceentiremonth.Checked);
                }
            }
            else
            {
                DataRow dr = xgvct_ppunit.GetFocusedDataRow();

                if (dr == null) return;

                if (mydb.checkhasfeepaymgt(drcontract, dr))
                {
                    DialogResult rt1 = MessageBox.Show("该房间已经存在应收数据，需要重新生成吗？ ", "重要提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                    if (rt1 == DialogResult.Cancel)
                    {
                        return;
                    }

                    mydb.deletepayfeemgt(drcontract, dr);
                    mydb.deletepayfeemgt_period(drcontract, dr);
                }



                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成应收数据...");

                mydb.createfeepaymgt(dr, drcontract, Convert.ToInt16(xsenmons.Text),
                                                     Convert.ToSingle(xsedecincvalue.Text), 
                                     xcbeincdectype.SelectedIndex,
                                     xceentiremonth.Checked);
                mydb.createfeepaymgt_period(dr, drcontract,xceentiremonth.Checked);
            }
          
           
            

            XScmWF.CloseWaitForm();

            xgvct_ppunit_FocusedRowChanged(null, null);
        }

        private void xgvct_ppunit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            if (dr == null) return;

       
            xgcppfeemgt.DataSource = mydb.getppfeepayfnc(dr);
        }

        private void xgvppfeemgt_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvppfeemgt.GetFocusedDataRow();

            if (dr == null) return;

            
            xteppbfeepay.Text = dr["bfee"].ToString();
            xtepprentfeepay.Text = dr["rentfee"].ToString();

            

        }

        private void xsbppfeepaymodify_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvppfeemgt.GetFocusedDataRow();

            if (dr == null) return;

            
            dr["bfee"] = xteppbfeepay.Text;
            dr["rentfee"] = xtepprentfeepay.Text;



         
            mydb.updateppfeepaymgt(dr);

            
        }

        private void frmContractPayfeeMgt_Load(object sender, EventArgs e)
        {
           
        }

        private void frmContractPayfeeMgt_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { mydb.finalclose(); }
            finally { }
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
                    mydb.createfeepaymgt_period(dr, drcontract,xceentiremonth.Checked);
                }
            }
            else
            {
                DataRow dr = xgvct_ppunit.GetFocusedDataRow();

                if (dr == null) return;

                    mydb.deletepayfeemgt_period(drcontract, dr);
 


                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在生成应收数据...");

                mydb.createfeepaymgt_period(dr, drcontract,xceentiremonth.Checked);
            }




            XScmWF.CloseWaitForm();

            xgvct_ppunit_FocusedRowChanged(null, null);
        }

        private void xsbzjinput_Click(object sender, EventArgs e)
        {
            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("如果合同是季付，就录入一季的折旧费用",
                                                                     "输入折旧费用",
                                                                     "0", -1, -1);

            if (sss1 == "") return;

            double dzj = 0;
            try
            {
               dzj= Convert.ToDouble(sss1);
            }
            catch
            {
                MessageBox.Show("请输入数字！不能含有字符","错误");
                return;
            }

            if (dzj == 0) return;

            //判断合同的房间起止时间，是否有多个

            string ssdt = "";
            string sedt = "";

            DataTable dt111 = mydb.getcon_pp_sdt_edt(drcontract);
            if (dt111.Rows.Count > 1)
            {
                //选择一个起止日期进行收费
                frmSingleSel mysel = new frmSingleSel();

                mysel.Text = "选择折旧收费起止日期";

                mysel.xgvsinglesel.Columns.Clear();
                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[0].Caption = "开始日期";
                mysel.xgvsinglesel.Columns[0].FieldName = "sdt";
                mysel.xgvsinglesel.Columns[0].Visible = true;

                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[1].Caption = "结束日期";
                mysel.xgvsinglesel.Columns[1].FieldName = "edt";
                mysel.xgvsinglesel.Columns[1].Visible = true;


                mysel.dtsrc = dt111;

                DialogResult dr = mysel.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    ssdt = mysel.drrt["sdt"].ToString();
                    sedt = mysel.drrt["edt"].ToString();

                }
                else
                {
                    return;
                }


            }
            else
            {
                ssdt = drcontract["contractsdt"].ToString();
                sedt = drcontract["contractedt"].ToString();
            }

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在生成合同折旧数据...");


            mydb.createfeepaymgt_zj(ssdt, sedt, dzj, drcontract);

            XScmWF.CloseWaitForm();

            xgvct_ppunit_FocusedRowChanged(null, null);


            this.Close();
            

        }

        private void xcbeincdectype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (xcbeincdectype.SelectedIndex > 3) xsenmons.Visible = true;
            else xsenmons.Visible = false;
        }

        private void xsbexpmonfeetoxls_Click(object sender, EventArgs e)
        {
            _gc.DataSource = xgvppfeemgt.DataSource;

            //_gc.Columns[0].Width = 150;

            sxlsfile = mydb.exportgvtoxls(_gv);

            //sfile=System.IO.Path.GetTempPath()+System.DateTime.Now.ToString("HHmmssfff")+".xls"; 
            //xgvxls.ExportToXls(sfile);

            //System.Diagnostics.Process.Start(sfile);

            xsbimpmonfeefromxls.Enabled = true;
        }

        private void xsbimpmonfeefromxls_Click(object sender, EventArgs e)
        {
            xgcppfeemgt.DataSource = mydb.importxlstodatatable(sxlsfile);

            xsbsavemonfee.Enabled = true;

            //showopermsg("Y", "[从xls导入合同房间数据]  成功导入!");
        }

        private void xsbimpconppfromxlshdfile_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "xls 文件|*.xls;*.xlsx";
            DialogResult dr = op.ShowDialog();
            if (dr != DialogResult.OK) return;

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("从xls导入批量房间月租数据...");

            xgcppfeemgt.DataSource = null;
            xgcppfeemgt.DataSource = mydb.importxlstodatatable(op.FileName);


            XScmWF.CloseWaitForm();

            xsbsavemonfee.Enabled = true;
        }

        private void xsbsavemonfee_Click(object sender, EventArgs e)
        {

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
                    mydb.updatefeepaymgt(dr);
                }
                
            }

            XScmWF.CloseWaitForm();


        }
    }
}