using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Drawing;

namespace jkwyjygl
{
    public partial class frmppmadd : DevExpress.XtraEditors.XtraForm
    {
        public delegate void dsmsg(string msgtype, string msg);
        public dsmsg msgshow;
        public DevExpress.XtraSplashScreen.SplashScreenManager XScmWF;

        string sfile;
        
        wheda.db.dboper mydb;

        public frmppmadd()
        {
            InitializeComponent();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppmaddlevel);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppmaddunits);

            mydb = new wheda.db.dboper();

        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {

        }

        private void frmppmadd_Load(object sender, EventArgs e)
        {
           // xtcadd.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

      

            xluearea.Properties.DataSource = mydb.getpparea();


            xgcppmaddlevel.DataSource = mydb.getnullpp();

            RepositoryItemComboBox riCombo = new RepositoryItemComboBox();

            DataTable dtt = mydb.getparacode("pptype");
            foreach (DataRow dr in dtt.Rows)
            {
                riCombo.Items.Add(dr["paravalue"].ToString());
            }
            //Add the item to the internal repository 
            xgcppmaddlevel.RepositoryItems.Add(riCombo);
            //Now you can define the repository item as an in-place column editor 
            xgvppmaddlevel.Columns["unittype"].ColumnEdit = riCombo;
            //xgvppmaddlevel.Columns["unittype"].OptionsColumn.ReadOnly = true;

            xluepptype.Properties.DataSource = dtt;
        }

        private void xluearea_EditValueChanged(object sender, EventArgs e)
        {
            xluebuilding.Properties.DataSource = mydb.getppbuildingbyareaid(xluearea.EditValue.ToString());
        }

        private void xsblevelok_Click(object sender, EventArgs e)
        {
            if (xluearea.EditValue == null ||
                xluebuilding.EditValue == null||
                xluepptype.EditValue==null)
            {
                msgshow("X", "[批量生成房间] 项目|楼宇|房间类型 必须选择!");
                return;

            }

            if (!mydb.checkhasunits(xluearea.EditValue.ToString(),
                                 xluebuilding.EditValue.ToString()))
            {
                //throw new Exception("该项目|楼宇下已经有房间!");
                msgshow("X", "[批量生成房间] 该项目|楼宇下已经有房间!");
                return;
            }


            DataTable dt = ((DataView)xgvppmaddlevel.DataSource).Table;
            dt.Rows.Clear();

            Int32 ii =Convert.ToInt32(xselevels.Text);
            Int32 i2=Convert.ToInt32(xsefirstlevel.Text);
            ii+=i2;

            for(int k=i2;k<ii;k++)
            {


               DataRow dr=dt.NewRow();

               dr["levelno"] = k.ToString();
               dr["units"] = xseunits.Text;
               dr["unittype"] = xluepptype.EditValue;
               dr["unituarea"] =0;
               dr["unitrent"] = xterent.Text;
               dr["unitbfee"] = 0;

               dt.Rows.Add(dr);
                
            }

            xsbunitsok.Enabled = true;
           
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {

        }

        private void xsbexptoxls_Click(object sender, EventArgs e)
        {
            if (xgvppmaddunits.RowCount <= 0) return;

            xgcxls.DataSource = xgcppmaddunits.DataSource;

            xgvxls.Columns[0].Width = 150;

            sfile = mydb.exportgvtoxls(xgvxls);

            //sfile=System.IO.Path.GetTempPath()+System.DateTime.Now.ToString("HHmmssfff")+".xls"; 
            //xgvxls.ExportToXls(sfile);

            //System.Diagnostics.Process.Start(sfile);

            xsbimpfromxls.Enabled = true;
        }

        private void xsbimpfromxls_Click(object sender, EventArgs e)
        {
 
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("从xls导入批量房间数据...");


            xgcppmaddunits.DataSource = null;
            xgcppmaddunits.DataSource = mydb.importxlstodatatable(sfile);

            XScmWF.CloseWaitForm();

            msgshow("Y", "[从xls导入批量房间数据] 成功导入！");
        }

        private void xsbsaveunits_Click(object sender, EventArgs e)
        {
            DialogResult dr1 = MessageBox.Show("确定要增加所有房间吗？", "批量操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在保存房间资料到系统...");

            BindingList<levellist> bl = new BindingList<levellist>();

            DataTable dt=((DataView)xgvppmaddunits.DataSource).Table;
            
            foreach(DataRow dr in dt.Rows)
            {
                //获取层id，如果没有就向数据库添加
                string s1 = dr["unitno"].ToString();
                string[] s2 = s1.Split(new char[] { '-' });
                s1 = s2[2];

                string ss = levellist.findIDbyNo(s1,bl);


                if (ss == "")
                {
                    ss = mydb.addpplevel(xluebuilding.EditValue.ToString(), s1, s1+"层", "").ToString();

                    bl.Add(new levellist(ss, s1));
                }


                dr["unitlevel"] = Convert.ToInt32(ss);

                mydb.addppunit(dr);


            }

            XScmWF.CloseWaitForm();

            this.Close();

            msgshow("Y", "[批量增加房间]  成功增加!");
        }

        private void xsbunitsok_Click(object sender, EventArgs e)
        {

            string sunitno = xluearea.GetColumnValue("ppcode").ToString() + "-" +
                             xluebuilding.GetColumnValue("ppcode").ToString() + "-";

            DataTable dtd = mydb.getnullpp2();
            xgcppmaddunits.DataSource = dtd;

            DataTable dt = ((DataView)xgvppmaddlevel.DataSource).Table;
            foreach (DataRow dr in dt.Rows)
            {
                string slevelno = dr["levelno"].ToString();
                string sppno = sunitno + slevelno + "-";

                int ii = Convert.ToInt32(dr["units"]);
                for (int k = 0; k < ii; k++)
                {
                    DataRow d1 = dtd.NewRow();

                    string sppno1 = "";
                    switch (dr["unittype"].ToString())
                    {
                        case "商铺":
                            sppno1 = sppno + "S" + (k + 1).ToString("D2");
                            break;
                        case "住宿":
                            sppno1 = sppno + "Z" + (k + 1).ToString("D2");
                            break;
                        case "场地":
                            sppno1 = sppno + "C" + (k + 1).ToString("D2");
                            break;
                        case "办公":
                            sppno1 = sppno + "G" + (k + 1).ToString("D2");
                            break;
                        case "仓库":
                            sppno1 = sppno + "K" + (k + 1).ToString("D2");
                            break;
                        default:
                            sppno1 = sppno + (k + 1).ToString("D2");
                            break;


                    }

                    d1["unitno"] = sppno1;

                    d1["unittype"] = dr["unittype"].ToString();
                    d1["unitorg"] = "原始";
                    d1["unitstatus"] = "空闲";
                    d1["unituarea"] = dr["unituarea"].ToString();
                    d1["unitrent"] = dr["unitrent"].ToString();
                    d1["unitbfee"] = dr["unitbfee"].ToString();
                    d1["unitarea"] = xluearea.EditValue;
                    d1["unitbuilding"] = xluebuilding.EditValue;


                    dtd.Rows.Add(d1);


                }

                xtcadd.SelectedTabPage = xtpppmaddunit;
            }

        }

        private void xsbimpfromxlshdfile_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "xls 文件|*.xls;*.xlsx";
            DialogResult dr = op.ShowDialog();
            if (dr != DialogResult.OK) return;

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("从xls导入批量房间数据...");

            xgcppmaddunits.DataSource = null;
            xgcppmaddunits.DataSource = mydb.importxlstodatatable(op.FileName);


            XScmWF.CloseWaitForm();
            msgshow("Y", "[从xls导入批量房间数据] 成功导入！");

        }

        private void frmppmadd_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { mydb.finalclose(); }
            finally { }
        }

        private void labelControl1_Click(object sender, EventArgs e)
        {

        }

        private void xselevels_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void labelControl2_Click(object sender, EventArgs e)
        {

        }

        private void xseunits_EditValueChanged(object sender, EventArgs e)
        {

        }


    }
}