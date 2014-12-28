using System;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Drawing;
using System.Configuration;
using DevExpress.XtraScheduler;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System.Net;
using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace jkwyjygl
{


    public partial class Form1 : RibbonForm
    {
        public static string uid;
        private string sxlsfile;
        private wheda.db.dboper mydb;
        System.Threading.Thread thread;


        Dictionary<string, DataRow> dc_user_pl;
        Dictionary<string, DataRow> dc_user_pl_cat;

        string sAppVerDT = "";

        Int32 iqtype = -1;
        Int32 iqtypefnc = -1;

        public static System.OperatingSystem osInfo = null;

        private void AppThreadException(object source, System.Threading.ThreadExceptionEventArgs e)
        {
            string errorMsg = "";

            if (e.Exception.GetType().ToString().Contains("MySqlException"))
            {
            }

            if (e.Exception.ToString().Contains("Unable to connect to any of the specified MySQL hosts."))
            {
                errorMsg = string.Format("发生异常: \r\n{0}", "连接服务器失败，请正确配置服务器IP！");
            }
            else
            {
                errorMsg = string.Format("发生异常: \r\n{0}", e.Exception.ToString());

            }

            errorMsg += Environment.NewLine;

            if (XScmWF.IsSplashFormVisible)
            {
                XScmWF.CloseWaitForm();
            }

            //showopermsg("X", e.Exception.GetType().ToString());

            showopermsg("X", errorMsg);
            
        }


        private delegate void FlushScheduler();//代理
        private FlushScheduler fsd;

        public Form1()
        {

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(AppThreadException);

            osInfo = System.Environment.OSVersion;


            InitializeComponent();

            InitSkinGallery();

            InitMyLayout();

            InitGrid();



            xbsiVer.Caption = "版本号：[" + Assembly.GetEntryAssembly().GetName().Version.ToString() + "]";

            sAppVerDT = System.IO.File.GetLastWriteTime(
                                          System.Reflection.Assembly.GetExecutingAssembly().Location
                                          ).ToString("yyyyMMdd");

            xbsVerDT.Caption = "版本日期：[" + sAppVerDT + "]";

            //  new DevExpress.XtraGrid.Selection.GridCheckMarksSelection(xgvppunitgrid);
            CBLC.lifj.control.upgrade.gridviewupgrade lup = null;

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppunitgrid);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xlvppunit);

            lup=new CBLC.lifj.control.upgrade.gridviewupgrade(xgvcontract);
            //lup.AddFNCNoCol(xrpfinance.Visible);

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_ppunit);
            lup.GroupPanelCaption = "合同包含的房间";

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvcttobechecked);
            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvcttobechecked_ppunit);
            lup.GroupPanelCaption = "合同包含的房间";

 
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvcus);

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvuser);
            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvusergroup);
            lup.GroupPanelCaption = "用户所属的权限组";

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvusergroupprivilege);
            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvuserprivilege);
            lup.GroupPanelCaption = "权限组包含的用户";


            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpparea);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppbuilding);
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpplevel);

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_feepay);
            lup.GroupPanelCaption = "合同应收实收（按月）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpp_feepay);
            lup.GroupPanelCaption = "房间应收实收（按月）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppfeemgt);
            lup.GroupPanelCaption = "房间应收(时间段）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvppfeemgt_tobechecked);
            lup.GroupPanelCaption = "房间应收(时间段）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_feepayed);
            lup.GroupPanelCaption = "合同未收（按月）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpp_feepayed);
            lup.GroupPanelCaption = "房间未收（按月）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_payedfeeseq);
            lup.GroupPanelCaption = "合同收费（按录入顺序）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_payedfeeseqmon);
            lup.GroupPanelCaption = "合同收费（按月）";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvctpp_payedfeeseqmon);
            lup.GroupPanelCaption = "合同收费（按房间）";

            lup=new CBLC.lifj.control.upgrade.gridviewupgrade(xgvinfoqueryct);
            //lup.AddFNCNoCol(xrpfinance.Visible);

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvinfoqueryct_pp);
            lup.GroupPanelCaption = "合同包含的房间及租金";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvinfoquery_fee);
            lup.GroupPanelCaption = "合同应收";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvinfoquery_fee_pp);
            lup.GroupPanelCaption = "房间应收";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpayfeect);
            lup.GroupPanelCaption = "时间段应缴费的合同";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpayfeectfee);
            lup.GroupPanelCaption = "合同应收";

            lup = new CBLC.lifj.control.upgrade.gridviewupgrade(xgvpayfeeppfee);
            lup.GroupPanelCaption = "房间应收";

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvinfoquerypp);

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvrptmgtfee);

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvrptppnum);

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvrptfncfee);

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvmgtfeequery);

            //new CBLC.lifj.control.upgrade.gridviewupgrade(xgvxls);

            if (ConfigurationManager.AppSettings["storeribboncollapse"] == "1")
            {
                ribbonControl.Minimized = true;
            }
            else ribbonControl.Minimized = false;

        }

        void InitMyLayout()
        {

        }

        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }
        //BindingList<ppunit> gridDataList = new BindingList<ppunit>();
        void InitGrid()
        {

            //            LayoutView lView = new LayoutView(gridControl1);
            //            gridControl1.MainView = lView;

            //gridDataList.Add(new ppunit("万科0A", "01栋","01层","01室"));
            //gridDataList.Add(new ppunit("经开万达", "B19栋", "88层","C10室"));
            //gridDataList.Add(new ppunit("金地0B", "B座", "101层", "2578室"));
            //gridDataList.Add(new ppunit("三角湖市场", "B栋", "架空层", "A01摊位"));

            //            busto.db.dboper mysqldb = new busto.db.dboper();


            //            gridControl1.DataSource = mysqldb.getppunit();

            //layoutView2.Appearance.Card.BackColor = Color.Red;
        }

        private void iExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (MessageBox.Show("确定退出系统吗？", "提示",
                               MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                               MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
            {
                return;
            }

            mydb.finalclose();

            if (ribbonControl.Enabled)
            {
                //保存配置
                try
                {

                    if (ConfigurationManager.AppSettings["storedatadisplaystyle"].ToString() == "1")
                    {
                        xgvppunitgrid.SaveLayoutToXml("layoutppquery.xml");
                    }

                    Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                    if (ConfigurationManager.AppSettings["storeskin"].ToString() == "1")
                    {
                        oConfig.AppSettings.Settings["cusskin"].Value = UserLookAndFeel.Default.SkinName;
                    }

                    oConfig.AppSettings.Settings["username"].Value = xluesysusers.EditValue.ToString();
                    if (xcestoreusername.Checked)
                    {
                        oConfig.AppSettings.Settings["storeusername"].Value = "1";

                    }
                    else
                    {
                        oConfig.AppSettings.Settings["storeusername"].Value = "0";

                    }

                    if (xceautoconserver.Checked)
                    {
                        oConfig.AppSettings.Settings["autoconnectserver"].Value = "1";

                    }
                    else
                    {
                        oConfig.AppSettings.Settings["autoconnectserver"].Value = "0";

                    }

                    oConfig.Save(ConfigurationSaveMode.Modified);


                    ConfigurationManager.RefreshSection("appSettings");

                }
                finally
                {
                }
            }

            ;
            try
            {
                Environment.Exit(0);
            }
            finally
            { }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.mydb = new wheda.db.dboper();

            ribbonControl.Enabled = false;

            GridLocalizer.Active = new ChsGridLocalizer();
            Localizer.Active = new ChsXtraEditorsLocalizer();

            this.xtcsys.SelectedTabPage = xtphomepage;
            this.xtcsysparaconfig.SelectedTabPageIndex = 0;
            this.xtchomepage.SelectedTabPageIndex = 2;

            this.xtcsys.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtcsyscode.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtcppunitchange.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtcppunitchange.SelectedTabPageIndex = 1;

            this.xtccusinfochange.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtccusinfochange.SelectedTabPageIndex = 1;

            this.xtccontractchange.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;

            this.xtcsysparaconfig.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtchomepage.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtcuserchange.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtcinfoquery.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            this.xtcfeefnc.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;


            //
            try
            {
                if (ConfigurationManager.AppSettings["storedatadisplaystyle"].ToString() == "1")
                {
                    xgvppunitgrid.RestoreLayoutFromXml("layoutppquery.xml");
                }
            }
            catch
            {
            }

            //
            xsccppunitmodify.SplitterPosition = 0;
            xscccusinfochange.SplitterPosition = 0;
            xscccontractchange.SplitterPosition = 0;
            xsccuserchange.SplitterPosition = 0;

            initserveraddr();

            xtcsys.SelectedTabPage = xtplogin;

            if (ConfigurationManager.AppSettings["autoconnectserver"] == "1")
            {
                xceautoconserver.Checked = true;
                xluesysusers.Properties.DataSource = mydb.getloginusers();

            }
            else
            {
                xceautoconserver.Checked = false;
                xsbserverip_Click(null, null);
            }



            xlclogin.Select();


            if (ConfigurationManager.AppSettings["storeusername"] == "1")
            {
                xcestoreusername.Checked = true;
                xluesysusers.EditValue = Convert.ToInt32(ConfigurationManager.AppSettings["username"].ToString());
            }

            if (ConfigurationManager.AppSettings["treelistfont"] == "1")
            {
                xcestoreusername.Checked = true;
                xluesysusers.EditValue = Convert.ToInt32(ConfigurationManager.AppSettings["username"].ToString());
            }

            //navBarControl1.View = new DevExpress.XtraNavBar.ViewInfo.Office3ViewInfoRegistrator();

            xschec.Start = System.DateTime.Now.Date;

            //xschec.Views.TimelineView.Scales[1].Visible = false;
            TimeInterval internval=new TimeInterval();
           // xschec.ActiveView.SetSelection(internval, Resource.Empty);

          
    


            this.Focus();

        }

        private void ScheThread()
        {
            fsd = new FlushScheduler(createinform);

            while (true)
            {
                System.Threading.Thread.Sleep(1000 * 60 * 10);

                xschec.Invoke(fsd, null);

                break;
                // System.Threading.Thread.Sleep(1000 *  5);
            }

        }

        private void inituserinfoform()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取用户数据......");



            xgcuser.DataSource = mydb.getsysusers();


            this.xtcsys.SelectedTabPage = xtpuser;

            XScmWF.CloseWaitForm();

            xsccuserchange.SplitterPosition = 0;
        }

        private void initcusinfoform()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取客户基础数据......");

            if (xluecusarea.Properties.DataSource == null)
            {


                xluecusareaadd.Properties.DataSource = mydb.getpparea();
                xluecusarea.Properties.DataSource = xluecusareaadd.Properties.DataSource;
                xluecusareamodi.Properties.DataSource = xluecusareaadd.Properties.DataSource;


            }

            this.xtcsys.SelectedTabPage = xtpcusinfo;

            XScmWF.CloseWaitForm();


        }

        private void initcontractinfoform(int flag=0)
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取合同基础数据......");

            if (xccbctareaquery.Properties.DataSource == null||flag==1)
            {



                xccbcontractstatus.Properties.DataSource = mydb.getparacode("contractstatus");

                xluecontractorg.Properties.DataSource = mydb.getparacode("contractorg");

                
                xccbctareaquery.Properties.DataSource = mydb.getpparea();

                xluectareamodi.Properties.DataSource = xccbctareaquery.Properties.DataSource;
                xluecontractarea.Properties.DataSource = xccbctareaquery.Properties.DataSource;

                xluecontractpptype.Properties.DataSource = mydb.getparacode("contractpptype");
                xluectpptypemodi.Properties.DataSource = xluecontractpptype.Properties.DataSource;

                xluecontractrentpaystyle.Properties.DataSource = mydb.getparacode("rentpaystyle");
                xluectrentpaystylemodi.Properties.DataSource = xluecontractrentpaystyle.Properties.DataSource;




            }

            this.xtcsys.SelectedTabPage = xtpcontract;

            XScmWF.CloseWaitForm();


        }

        private void initsysmanform()
        {
            xtcsys.SelectedTabPage = xtpsyspara;
            xtcsysparaconfig.SelectedTabPage = xtpuserconfig;

        }

        private void enableallprivilege()
        {
            xrpcontract.Visible = true;
            xrppp.Visible = true;
            xrpcus.Visible = true;

            xrpfinance.Visible = true;
            xrpfee.Visible = true;

            xrpcontract.Visible = true;
            xtpcontract.PageVisible = true;


            bbicontractquery.Enabled = true;

            bbicontractadd.Enabled = true;

            bbicontractmodi.Enabled = true;
            xsbcontractmodiok.Enabled = true;
            xsbcontractchangeapply.Enabled = true;

            xsbct_ppunit.Visible = true;
            xsbimpconppfromxls.Visible = true;
            xsbimpconppfromxlshdfile.Visible = true;
            xsbsaveconpp.Visible = true;


            xsbfeeadjust.Enabled = true;

            bbicontractdel.Enabled = true;

            bbicontractadd.Enabled = true; ;
            xsbcontractaddok.Enabled = true;

            bbisendcttocheck.Enabled = true;

            bbicontractapproved.Enabled = true;

            bbicontractremodify.Enabled = true;

            bbicontractgoon.Enabled = true;
            bbicontractcancel.Enabled = true;
            bbictattquery.Enabled = true;
            bbictattupload.Enabled = true;



            bbiareablmgt.Enabled = true;
            bbippadd.Enabled = true;
            xsbppaddok.Enabled = true;
            bbippmadd.Enabled = true;

            bbippmodi.Enabled = true;
            xsbppmodiapply.Enabled = true;
            xsbppmodiok.Enabled = true;
            bbippdel.Enabled = true;
            bbippcombine.Enabled = true;
            xsbppcombineok.Enabled = true;

            bbippsplit.Enabled = true;
            xsbppsplitok.Enabled = true;
            xtpppunitgrid.PageVisible = true;
            bbippunitquery.Enabled = true;
            obbippunitquery.Enabled = true;

            xtpppunitlv.PageVisible = true;
            bbippunitlvview.Enabled = true;
            xrppp.Visible = true;
            xrpcus.Visible = true;
            xtpcusinfo.PageVisible = true;
            bbicusquery.Enabled = true;
            bbicusadd.Enabled = true;
            xsbcusaddok.Enabled = true;
            bbicusmodi.Enabled = true;
            xsbcusmodiapply.Enabled = true;
            xsbcusmodiok.Enabled = true;
            bbicusdel.Enabled = true;

            bbiqueryctbechecked.Enabled = true;
            bbicontractapproved.Enabled = true;
            bbicancelcheckct.Enabled = true;

            bbifeepayqueryfnc.Enabled = true;
            bbifeepayedinputfnc.Enabled = true;
            bbifeepayedqueryfnc.Enabled = true;
            bbirptmon.Enabled = true;
            bbifeepayeddeletefnc.Enabled = true;
            bbifeepayedrptfnc.Enabled = true;
            bbifeepayedconfirmfnccancel.Enabled = true;

            bbifeepayedconfirmfnc.Enabled = true;
            bbiattachctnofnc.Enabled = true;

            bbifeepayquerymgt.Enabled = true;
            bbifeepayedinputmgt.Enabled = true;
            bbifeepayedrptmgt.Enabled = true;

            bbiuserquery.Enabled = true;
            xtpuser.PageVisible = true;
            bbiuseradd.Enabled = true;
            xsbuseraddok.Enabled = true;
            bbiusermodify.Enabled = true;
            xsbusermodiapply.Enabled = true;
            xsbusermodiok.Enabled = true;
            bbiuserdelete.Enabled = true;
            bbiuserendis.Enabled = true;
            bbiuserprivilege.Enabled = true;
            bbisyscode.Enabled = true;
            xnbgsysconfig.Visible = true;
            xnbguserconfig.Visible = true;
            bbisyspara.Enabled = true;
        }

        private void inituserprivilege()
        {
            if (uid != "0")
            {
                //contract
                DataRow dr11;

                //合同权限
                if (!dc_user_pl.TryGetValue("contractquery", out dr11))
                {
                    xrpcontract.Visible = false;
                    xtpcontract.PageVisible = false;
                    bbicontractquery.Enabled = false;
                }

                if (!dc_user_pl.TryGetValue("contractadd", out dr11))
                {
                    bbicontractadd.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractmodify", out dr11))
                {
                    bbicontractmodi.Enabled = false;
                    xsbcontractmodiok.Enabled = false;
                    xsbcontractchangeapply.Enabled = false;

                    xsbct_ppunit.Visible = false;
                    xsbimpconppfromxls.Visible = false;
                    xsbimpconppfromxlshdfile.Visible = false;
                    xsbsaveconpp.Visible = false;


                    xsbfeeadjust.Enabled = false;
                }
                if (!dc_user_pl.TryGetValue("contractdelete", out dr11))
                {
                    bbicontractdel.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractadd", out dr11))
                {
                    bbicontractadd.Enabled = false; ;
                    xsbcontractaddok.Enabled = false;
                }
                if (!dc_user_pl.TryGetValue("contractcommit", out dr11))
                {
                    bbisendcttocheck.Enabled = false;

                }

                if (!dc_user_pl.TryGetValue("contractend", out dr11))
                {
                    bbicontractremodify.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractgoon", out dr11))
                {
                    bbicontractgoon.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractcancel", out dr11))
                {
                    bbicontractcancel.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractuploadatt", out dr11))
                {
                    bbictattupload.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractdownloadatt", out dr11))
                {
                    bbictattquery.Enabled = false;

                }



                //物业权限
                if (!dc_user_pl.TryGetValue("ppcode", out dr11))
                {
                    bbiareablmgt.Enabled = false;

                }

                if (!dc_user_pl.TryGetValue("ppaddsingle", out dr11))
                {
                    bbippadd.Enabled = false;
                    xsbppaddok.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("ppaddmulti", out dr11))
                {
                    bbippmadd.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("ppmodify", out dr11))
                {
                    bbippmodi.Enabled = false;
                    xsbppmodiapply.Enabled = false;
                    xsbppmodiok.Enabled = false;
                }
                if (!dc_user_pl.TryGetValue("ppdelete", out dr11))
                {
                    bbippdel.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("ppcombine", out dr11))
                {
                    bbippcombine.Enabled = false;
                    xsbppcombineok.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("ppsplit", out dr11))
                {
                    bbippsplit.Enabled = false;
                    xsbppsplitok.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("ppquery", out dr11))
                {
                    xtpppunitgrid.PageVisible = false;
                    bbippunitquery.Enabled = false;
                    obbippunitquery.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("ppcard", out dr11))
                {
                    xtpppunitlv.PageVisible = false;
                    bbippunitlvview.Enabled = false;

                }
                if ((!dc_user_pl.TryGetValue("ppquery", out dr11)) &&
                    (!dc_user_pl.TryGetValue("ppcard", out dr11))
                    )
                {
                    xrppp.Visible = false;
                }

                //客户资料权限
                if (!dc_user_pl.TryGetValue("cusquery", out dr11))
                {
                    xrpcus.Visible = false;
                    xtpcusinfo.PageVisible = false;
                    bbicusquery.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("cusadd", out dr11))
                {
                    bbicusadd.Enabled = false;
                    xsbcusaddok.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("cusmodi", out dr11))
                {
                    bbicusmodi.Enabled = false;
                    xsbcusmodiapply.Enabled = false;
                    xsbcusmodiok.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("cusdel", out dr11))
                {
                    bbicusdel.Enabled = false;

                }

                //财务权限
                if (!dc_user_pl.TryGetValue("contractcheckquery", out dr11))
                {
                    bbiqueryctbechecked.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractapprove", out dr11))
                {
                    bbicontractapproved.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("contractreturn", out dr11))
                {
                    bbicancelcheckct.Enabled = false;

                }

                //财务费用
                if (!dc_user_pl.TryGetValue("feepayqueryfnc", out dr11))
                {
                    bbifeepayqueryfnc.Enabled = false;

                } 
                if (!dc_user_pl.TryGetValue("feepayedinputfnc", out dr11))
                {
                    bbifeepayedinputfnc.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("feepayedqueryfnc", out dr11))
                {
                    bbifeepayedqueryfnc.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("feepayedrptmon", out dr11))
                {
                    bbirptmon.Enabled = false;

                }

                if (!dc_user_pl.TryGetValue("feepayeddeletefnc", out dr11))
                {
                    bbifeepayeddeletefnc.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("feepayedrptfnc", out dr11))
                {
                    bbifeepayedrptfnc.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("feepayedconfirm", out dr11))
                {
                    bbifeepayedconfirmfnc.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("feepayedconfirmcancel", out dr11))
                {
                    bbifeepayedconfirmfnccancel.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("attachctfncno", out dr11))
                {
                    bbiattachctnofnc.Enabled = false;

                }

                //经营费用

                if (!dc_user_pl.TryGetValue("feepayquerymgt", out dr11))
                {
                    bbifeepayquerymgt.Enabled = false;

                } if (!dc_user_pl.TryGetValue("feepayedinputmgt", out dr11))
                {
                    bbifeepayedinputmgt.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("feepayedrptmgt", out dr11))
                {
                    bbifeepayedrptmgt.Enabled = false;

                }

                //系统功能权限
                if (!dc_user_pl.TryGetValue("userquery", out dr11))
                {
                    bbiuserquery.Enabled = false;
                    xtpuser.PageVisible = false;

                }
                if (!dc_user_pl.TryGetValue("useradd", out dr11))
                {
                    bbiuseradd.Enabled = false;
                    xsbuseraddok.Enabled = false;
                }
                if (!dc_user_pl.TryGetValue("usermodify", out dr11))
                {
                    bbiusermodify.Enabled = false;
                    xsbusermodiapply.Enabled = false;
                    xsbusermodiok.Enabled = false;
                }
                if (!dc_user_pl.TryGetValue("userdelete", out dr11))
                {
                    bbiuserdelete.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("userstatuschange", out dr11))
                {
                    bbiuserendis.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("userprivilege", out dr11))
                {
                    bbiuserprivilege.Enabled = false;

                }
                if (!dc_user_pl.TryGetValue("syscode", out dr11))
                {
                    bbisyscode.Enabled = false;
                }

                if (!dc_user_pl.TryGetValue("sysconfig", out dr11))
                {
                    xnbgsysconfig.Visible = false;

                }
                if (!dc_user_pl.TryGetValue("userconfig", out dr11))
                {
                    xnbguserconfig.Visible = false;

                }

                if ((!dc_user_pl.TryGetValue("sysconfig", out dr11)) &&
                    (!dc_user_pl.TryGetValue("userconfig", out dr11))
                    )
                {
                    bbisyspara.Enabled = false;
                }
            }

        }

        private void initstartpage()
        {
            switch (ConfigurationManager.AppSettings["startpage"].ToString())
            {
                case "contract":
                    ribbonControl.SelectedPage = xrpcontract;
                    //initcontractinfoform();
                    initqueryform(1);
                    break;
                case "pp":
                    ribbonControl.SelectedPage = xrppp;
                    //initppunitlvview();
                    initqueryform(0);
                    break;
                case "cus":
                    ribbonControl.SelectedPage = xrpcus;
                    initcusinfoform();
                    break;
                case "feeO":
                    ribbonControl.SelectedPage = xrpfee;
                    initqueryform(2);
                    break;
                case "finance":
                    ribbonControl.SelectedPage = xrpfinance;
                    initfncfeeform(0);
                    break;
                case "system":
                    ribbonControl.SelectedPage = xrpsystem;
                    initsysmanform();
                    break;

                default:
                    break;
            }


        }

        private void inithomepage()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取统计数据......");

            //左边柱状图、饼图
            if (xccpptotal.Series[0].Tag == null)
            {


                DataTable dt = mydb.getppstatall();

                xccpptotal.Series[0].Points.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    xccpptotal.Series[0].Points.Add(new DevExpress.XtraCharts.SeriesPoint(dr["unitstatus"].ToString(),
                                                                                          dr["total"]));

                }

                DataTable dt1 = mydb.getppstatbyarea("出租");
                if (dt1.Rows.Count != 0)
                {
                    xccppbybuilding.Series[0].DataSource = dt1;
                    xccppbybuilding.Series[0].ArgumentDataMember = "ppname";
                    xccppbybuilding.Series[0].ValueDataMembers[0] = "total";
                }


                dt1 = mydb.getppstatbyarea("空闲");
                if (dt1.Rows.Count != 0)
                {
                    xccppbybuilding.Series[1].DataSource = dt1;
                    xccppbybuilding.Series[1].ArgumentDataMember = "ppname";
                    xccppbybuilding.Series[1].ValueDataMembers[0] = "total";
                }

                dt1 = mydb.getppstatbyarea(null);
                if (dt1.Rows.Count != 0)
                {
                    xccppbybuilding.Series[2].DataSource = dt1;
                    xccppbybuilding.Series[2].ArgumentDataMember = "ppname";
                    xccppbybuilding.Series[2].ValueDataMembers[0] = "total";
                }

                xccpptotal.Series[0].Tag = "OK";

            }


            //右边的曲线图
            if (xccpayfeechaincmp.Series[0].Tag == null)
            {
                //环比
                DataTable dt1 = mydb.getpayfeetotal(System.DateTime.Now.AddMonths(-3).ToString("yyyyMMdd"),
                                                    System.DateTime.Now.AddMonths(3).ToString("yyyyMMdd"));
                if (dt1.Rows.Count != 0)
                {
                    xccpayfeechaincmp.Series[0].DataSource = dt1;
                    xccpayfeechaincmp.Series[0].ArgumentDataMember = "feemonth";
                    xccpayfeechaincmp.Series[0].ValueDataMembers[0] = "fee";
                }

                dt1 = mydb.getpayedfeetotal(System.DateTime.Now.AddMonths(-3).ToString("yyyyMMdd"),
                                                    System.DateTime.Now.AddMonths(3).ToString("yyyyMMdd"));
                if (dt1.Rows.Count != 0)
                {
                    xccpayfeechaincmp.Series[1].DataSource = dt1;
                    xccpayfeechaincmp.Series[1].ArgumentDataMember = "feemonth";
                    xccpayfeechaincmp.Series[1].ValueDataMembers[0] = "fee";
                }


                //实收同比
                dt1 = mydb.getpayedfeeyeartotal(System.DateTime.Now.AddMonths(-3).ToString("yyyyMMdd"),
                                                System.DateTime.Now.AddMonths(3).ToString("yyyyMMdd"));


                if (dt1.Rows.Count != 0)
                {
                    xccpayfeeperiodcmp.Series[0].LegendText = System.DateTime.Now.Year.ToString();
                    xccpayfeeperiodcmp.Series[0].DataSource = dt1;
                    xccpayfeeperiodcmp.Series[0].ArgumentDataMember = "fm";
                    xccpayfeeperiodcmp.Series[0].ValueDataMembers[0] = "fee";
                }

                dt1 = mydb.getpayedfeeyeartotal(System.DateTime.Now.AddYears(-1).AddMonths(-3).ToString("yyyyMMdd"),
                                                System.DateTime.Now.AddYears(-1).AddMonths(3).ToString("yyyyMMdd"));


                if (dt1.Rows.Count != 0)
                {
                    xccpayfeeperiodcmp.Series[1].LegendText = System.DateTime.Now.AddYears(-1).Year.ToString();
                    xccpayfeeperiodcmp.Series[1].DataSource = dt1;
                    xccpayfeeperiodcmp.Series[1].ArgumentDataMember = "fm";
                    xccpayfeeperiodcmp.Series[1].ValueDataMembers[0] = "fee";
                }
            }



            this.xtcsys.SelectedTabPage = xtphomepage;
            this.xtchomepage.SelectedTabPage = xtphomepptotal;

            XScmWF.CloseWaitForm();
        }


        private void initppunitlvview()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取物业基础数据......");

            if (this.tlpp.DataSource == null)
            {
                this.tlpp.Nodes.Clear();

                this.tlpp.DataSource = mydb.getpp();
                this.tlpp.Columns["ppname"].Caption = "经开物业";



            }

            this.xtcsys.SelectedTabPage = xtpppunitlv;

            XScmWF.CloseWaitForm();

            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlpp.Nodes)
            {
                rn.Expanded = true;
            }

            //tlpp.ExpandAll();

        }

        private void initppunitgridview(int flag=0)
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取房间基础编码...");

            if (xccbarea.Properties.DataSource == null||flag==1)
            {


                xccbarea.Properties.DataSource = mydb.getpparea();

                xlueppareaadd.Properties.DataSource = xccbarea.Properties.DataSource;


                xccbpptype.Properties.DataSource = mydb.getparacode("pptype");
                xluepptypeadd.Properties.DataSource = xccbpptype.Properties.DataSource;
                xluepptypex.Properties.DataSource = xccbpptype.Properties.DataSource;
                xluepptypesplit.Properties.DataSource = xccbpptype.Properties.DataSource;
                xluepptypecombine.Properties.DataSource = xccbpptype.Properties.DataSource;

                xlueppsc.Properties.DataSource = mydb.getparacode("ppunitorg");

                xccbppstatus.Properties.DataSource = mydb.getparacode("ppunitstatus");
                



            }


            
            xtcsys.SelectedTabPage = xtpppunitgrid;

            XScmWF.CloseWaitForm();
        }

        private void initareacodeform()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取物业编码数据......");

            if (xgcpparea.DataSource == null)
            {


                xgcpparea.DataSource = mydb.getpparea();
            }

            //xgvpparea.BestFitColumns();
            //xgvppbuilding.BestFitColumns();
            //xgvpplevel.BestFitColumns();

            xtcsys.SelectedTabPage = xtpsyscode;
            xtcsyscode.SelectedTabPage = xtpppcode;

            xsccsyscode.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;

            XScmWF.CloseWaitForm();
        }



        private void initparacodeform()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取参数编码数据......");

            if (xgcparacodecatalog.DataSource == null)
            {


                xgcparacodecatalog.DataSource = mydb.getparatype();
            }



            xtcsys.SelectedTabPage = xtpsyscode;
            xtcsyscode.SelectedTabPage = xtpparacode;

            xsccsyscode.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;

            XScmWF.CloseWaitForm();
        }

        private void xgcpparea_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvpparea.GetFocusedDataRow();

            if (dr == null)
            {
                xteareacode.EditValue = null;
                xteareaname.EditValue = null;
                xmeareades.EditValue = null;
                return;
            }

            xteareacode.Text = dr["ppcode"].ToString();
            xteareaname.Text = dr["ppname"].ToString();
            xmeareades.Text = dr["ppdes"].ToString();


        }

        private void xgvpparea_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvpparea.GetFocusedDataRow();
            if (dr == null)
            {
                xgcppbuilding.DataSource = null;
                xgcpplevel.DataSource = null;
                return;
            }

            string sarea = dr["id"].ToString();




            xgcppbuilding.DataSource = mydb.getppbuildingbyareaid(sarea);

            xgvppbuilding_FocusedRowChanged(null, null);

            xgcppbuilding_Click(sender, null);
            xgcpplevel_Click(sender, null);
        }

        private void xgvppbuilding_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvppbuilding.GetFocusedDataRow();

            if (dr == null)
            {
                xgcpplevel.DataSource = null;
                return;
            }

            string sbuilding = dr["id"].ToString();



            xgcpplevel.DataSource = mydb.getpplevelbybuildingid(sbuilding);

            xgcpplevel_Click(sender, null);
        }



        public void showopermsg(string msgtype, string msg)
        {

            if (msgtype == "X")
            {

                xmedpmsg.Text = System.DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss.fff]") + msg + "\r\n" + xmedpmsg.Text;

                if (ConfigurationManager.AppSettings["showslideerrmsg"].ToString() == "1")
                {
                    xdpmsg.ShowSliding();
                }
            }
            if (msgtype == "Y")
            {
                xmedpmsg1.Text = System.DateTime.Now.ToString("[yyyyMMdd-HH:mm:ss.fff]") + msg + "\r\n" + xmedpmsg1.Text;

                if (ConfigurationManager.AppSettings["showslidesysmsg"].ToString() == "1")
                {
                    xdpmsg1.ShowSliding();
                }
            }
        }



        private void pictureEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void xdpmsg_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            if (e.Button == xdpmsg.CustomHeaderButtons[0])
            {
                xdpmsg.HideSliding();
            }

            if (e.Button == xdpmsg.CustomHeaderButtons[1])
            {
                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在发送bug邮件...");

                try
                {
                    //send use gmail

                    {
                        MailMessage myMail = new MailMessage();

                        myMail.From = new MailAddress("lfangjun@gmail.com");
                        myMail.To.Add(new MailAddress("416075422@qq.com"));

                        myMail.Subject = "软件bug报告--" + xluesysusers.Text;
                        myMail.SubjectEncoding = Encoding.UTF8;

                        myMail.Body = xmedpmsg.Text;
                        myMail.BodyEncoding = Encoding.UTF8;
                        myMail.IsBodyHtml = false;

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.Credentials = new NetworkCredential("lfangjun", "square_0617_hb");
                        smtp.EnableSsl = true;

                        smtp.Send(myMail);

                    }

                    //send use qqmail

                    {
                        MailMessage myMail = new MailMessage();

                        myMail.From = new MailAddress("416075422@qq.com");
                        myMail.To.Add(new MailAddress("416075422@qq.com"));

                        myMail.Subject = "软件bug报告--" + xluesysusers.Text;
                        myMail.SubjectEncoding = Encoding.UTF8;

                        myMail.Body = xmedpmsg.Text;
                        myMail.BodyEncoding = Encoding.UTF8;
                        myMail.IsBodyHtml = false;

                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.qq.com";
                        //smtp.Port = 587;
                        smtp.Credentials = new NetworkCredential("416075422", "hb_5358303_cb");
                        //smtp.EnableSsl = true;

                        smtp.Send(myMail);

                    }
                }
                finally
                {
                }

                XScmWF.CloseWaitForm();
            }
        }

        private void dockPanel1_Click(object sender, EventArgs e)
        {

        }

        private void xdpmsg1_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.ButtonEventArgs e)
        {
            xdpmsg1.HideSliding();
        }

        private void bbisyscode_ItemClick(object sender, ItemClickEventArgs e)
        {

            initparacodeform();
        }



        private void xsbmodilevel_Click(object sender, EventArgs e)
        {
            if (xgvpplevel.SelectedRowsCount > 1)
            {
                showopermsg("X", "[修改楼层] 您选中了多个楼层，请选中一个楼层！");
                return;
            }

            DataRow dr = xgvpplevel.GetFocusedDataRow();
            if (dr == null)
            {
                showopermsg("X", "[修改楼层] 你没有选中任何楼层!");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要修改选中的楼层吗？", "确认修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            string slevel = dr["id"].ToString();

            string snewname = xtelevelname.Text;
            string snewcode = xtelevelcode.Text;
            string snewdes = xmeleveldes.Text;


            mydb.updatepplevel(slevel, snewname, snewcode, snewdes);

            dr["ppname"] = snewname;
            dr["ppcode"] = snewcode;
            dr["ppdes"] = snewdes;

            showopermsg("Y", "<修改楼层> 成功修改楼层编码数据！");

        }

        private void xsbmodarea_Click(object sender, EventArgs e)
        {
            if (xgvpparea.SelectedRowsCount > 1)
            {
                showopermsg("X", "[修改项目] 您选中了多个项目，请选中一个项目！");
                return;
            }

            DataRow dr = xgvpparea.GetFocusedDataRow();
            if (dr == null)
            {
                showopermsg("X", "[修改项目] 你没有选中任何项目!");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要修改选中的项目吗？", "确认修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            string sarea = dr["id"].ToString();

            string snewname = xteareaname.Text;
            string snewcode = xteareacode.Text;
            string snewdes = xmeareades.Text;


            mydb.updatepparea(sarea, snewname, snewcode, snewdes);

            dr["ppcode"] = xteareacode.Text;
            dr["ppname"] = xteareaname.Text;
            dr["ppdes"] = xmeareades.Text;


            showopermsg("Y", "<修改项目> 成功修改项目编码数据！");

        }

        private void xsbmodibuilding_Click(object sender, EventArgs e)
        {
            if (xgvppbuilding.SelectedRowsCount > 1)
            {
                showopermsg("X", "[修改楼宇] 您选中了多个楼宇，请选中一个楼宇！");
                return;
            }

            DataRow dr = xgvppbuilding.GetFocusedDataRow();
            if (dr == null)
            {
                showopermsg("X", "[修改楼宇] 你没有选中任何一个楼宇!");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要修改选中的楼宇吗？", "确认修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            string sbuilding = dr["id"].ToString();

            string snewname = xtebuildingname.Text;
            string snewcode = xtebuildingcode.Text;
            string snewdes = xmebuildingdes.Text;


            mydb.updateppbuilding(sbuilding, snewname, snewcode, snewdes);

            dr["ppname"] = snewname;
            dr["ppcode"] = snewcode;
            dr["ppdes"] = snewdes;

            showopermsg("Y", "<修改楼宇> 成功修改楼宇编码数据！");



        }

        private void xgcppbuilding_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvppbuilding.GetFocusedDataRow();

            if (dr == null)
            {
                xtebuildingcode.EditValue = null;
                xtebuildingname.EditValue = null;
                xmebuildingdes.EditValue = null;

                return;
            }

            xtebuildingcode.Text = dr["ppcode"].ToString();
            xtebuildingname.Text = dr["ppname"].ToString();
            xmebuildingdes.Text = dr["ppdes"].ToString();


        }

        private void xgcpplevel_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvpplevel.GetFocusedDataRow();

            if (dr == null)
            {
                xtelevelcode.EditValue = null;
                xtelevelname.EditValue = null;
                xmeleveldes.EditValue = null;
                return;
            }

            xtelevelcode.Text = dr["ppcode"].ToString();
            xtelevelname.Text = dr["ppname"].ToString();
            xmeleveldes.Text = dr["ppdes"].ToString();


        }

        private void xsbdelarea_Click(object sender, EventArgs e)
        {
            if (xgvpparea.SelectedRowsCount < 1)
            {
                showopermsg("X", "[删除项目] 没有选中任何项目！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要删除选中的项目吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            foreach (Int32 i3 in xgvpparea.GetSelectedRows())
            {
                DataRow dr = xgvpparea.GetDataRow(i3);

                mydb.deletepp("0", dr["id"].ToString());
            }

            xgvpparea.DeleteSelectedRows();

            xgvpparea_FocusedRowChanged(sender, null);


            showopermsg("Y", "<删除项目> 删除项目编码数据成功！");
        }

        private void xgvpparea_MouseDown(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = xgvpparea.CalcHitInfo(new Point(e.X, e.Y));
            if (((e.Button & MouseButtons.Left) != 0) && (hitInfo.InRow))
            {
                //  showopermsg("Y", "mouse in rows");
            }
        }

        private void xsbaddarea_Click(object sender, EventArgs e)
        {
            if (xteareaname.Text.Length == 0 || xteareacode.Text.Length == 0)
            {
                showopermsg("X", "[增加项目] 项目名称|项目编码不能为空！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要增加项目[" + xteareaname.Text + "]吗？", "确认增加", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            Int32 searchid = mydb.addpparea(xteareacode.Text, xteareaname.Text, xmeareades.Text);

            xgcpparea.DataSource = null;
            initareacodeform();

            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcpparea.MainView;

            int rhFound = cv.LocateByValue("id", searchid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }

            showopermsg("Y", "<增加项目> 成功增加项目编码数据！");




        }

        private void xsbaddbuilding_Click(object sender, EventArgs e)
        {
            if (xtebuildingname.Text.Length == 0 || xtebuildingcode.Text.Length == 0)
            {
                showopermsg("X", "[增加楼宇] 楼宇名称|楼宇编码不能为空！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要增加楼宇[" + xtebuildingname.Text + "]吗？", "确认增加", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            DataRow dr2 = xgvpparea.GetFocusedDataRow();
            string sparentid = dr2["id"].ToString();


            Int32 searchid = mydb.addppbuilding(sparentid, xtebuildingcode.Text, xtebuildingname.Text, xmebuildingdes.Text);


            xgvpparea_FocusedRowChanged(null, null);

            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcppbuilding.MainView;

            int rhFound = cv.LocateByValue("id", searchid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }

            showopermsg("Y", "<添加楼宇> 成功增加楼宇编码数据！");
        }

        private void xsbdelbuilding_Click(object sender, EventArgs e)
        {
            if (xgvppbuilding.SelectedRowsCount < 1)
            {
                showopermsg("X", "[删除楼宇] 没有选中任何一个楼宇！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要删除选中的楼宇吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            foreach (Int32 i3 in xgvppbuilding.GetSelectedRows())
            {
                DataRow dr = xgvppbuilding.GetDataRow(i3);

                mydb.deletepp("1", dr["id"].ToString());
            }

            xgvppbuilding.DeleteSelectedRows();
            xgvppbuilding_FocusedRowChanged(sender, null);


            showopermsg("Y", "<删除楼宇> 删除楼宇编码数据成功！");

        }

        private void xsbaddlevel_Click(object sender, EventArgs e)
        {
            if (xtelevelname.Text.Length == 0 || xtelevelcode.Text.Length == 0)
            {
                showopermsg("X", "[增加楼层] 楼层名称|楼层编码不能为空！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要增加楼层[" + xtelevelname.Text + "]吗？", "确认增加", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            DataRow dr2 = xgvppbuilding.GetFocusedDataRow();
            string sparentid = dr2["id"].ToString();


            Int32 searchid = mydb.addpplevel(sparentid, xtelevelcode.Text, xtelevelname.Text, xmeleveldes.Text);


            xgvppbuilding_FocusedRowChanged(null, null);

            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcpplevel.MainView;

            int rhFound = cv.LocateByValue("id", searchid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }

            showopermsg("Y", "<增加楼层> 成功增加楼层编码数据！");



        }

        private void xsbdellevel_Click(object sender, EventArgs e)
        {
            if (xgvpplevel.SelectedRowsCount < 1)
            {
                showopermsg("X", "[删除楼层] 没有选中任何楼层!");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要删除选中的楼层吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            foreach (Int32 i3 in xgvpplevel.GetSelectedRows())
            {
                DataRow dr = xgvpplevel.GetDataRow(i3);

                mydb.deletepp("2", dr["id"].ToString());
            }

            xgvpplevel.DeleteSelectedRows();

            showopermsg("Y", "<删除楼层> 删除楼层编码数据成功！");


        }

        private void bbisyspara_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.xtcsys.SelectedTabPage = xtpsyspara;
            xtcsysparaconfig.SelectedTabPage = xtpuserconfig;

            initconfigform();

        }

        private void ribbonControl_MouseDown(object sender, MouseEventArgs e)
        {
            //return;

            DevExpress.XtraBars.Ribbon.ViewInfo.RibbonHitInfo rhi = ribbonControl.CalcHitInfo(e.Location);

            if (rhi.InPage)
            {

                if (rhi.Page == xrpcontract)
                {
                    initqueryform(1);
                 //   initcontractinfoform();
                }
                else if (rhi.Page == xrppp)
                {
                    //initppunitlvview();
                    //initppunitgridview();
                    initqueryform(0);
                }
                else if (rhi.Page == xrpcus)
                {
                    initcusinfoform();
                }
                else if (rhi.Page == xrpfee)
                {
                    initqueryform(2);
                }
                else if (rhi.Page == xrpfinance)
                {
                    initfncfeeform(0);
                }


            }

        }

        private void bbippunitquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            initppunitgridview();

        }

        private void tlpp_DoubleClick(object sender, EventArgs e)
        {

            Point point = tlpp.PointToClient(Cursor.Position);
            DevExpress.XtraTreeList.TreeListHitInfo hitInfo = tlpp.CalcHitInfo(point);

            if (hitInfo.HitInfoType != DevExpress.XtraTreeList.HitInfoType.Cell) return;

            DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlpp.FocusedNode;
            if (clickedNode.Level != 2) return;


            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取房间资料...");

            string sarea = clickedNode.ParentNode["parentid"].ToString();
            string sbuilding = clickedNode["parentid"].ToString();
            string slevel = clickedNode["id"].ToString();


            this.xgcppunit.DataSource = mydb.getppunitbyid(sarea, sbuilding, slevel);

            //string disPlayText = clickedNode.ParentNode["parentid"].ToString()+"--"+
            //                     clickedNode["parentid"].ToString() + "--" +
            //                     clickedNode["id"].ToString() + "--" +
            //                    clickedNode["ppname"].ToString();
            //                    ;
            //MessageBox.Show("You clicked level " + disPlayText);

            xlvppunit.ViewCaption = clickedNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                                   clickedNode.ParentNode["ppname"].ToString() + " | " +
                                   clickedNode["ppname"].ToString();

            XScmWF.CloseWaitForm();
        }

        private void xluearea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }
        }

        private void xsbppquery_Click(object sender, EventArgs e)
        {

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在查询房间数据...");

            string squery = wheda.db.dboper.sppunitquery + " where 1=1 ";

            if (xccbarea.Text != "")
            {
                //squery += " and unitarea=" + xluearea.EditValue.ToString();

                string sss = " and unitarea in ('0'";
                for (int ii = 0; ii < xccbarea.Properties.Items.Count; ii++)
                {
                    if (xccbarea.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbarea.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;
            }

            if (xluebuilding.EditValue != null)
            {
                squery += " and unitbuilding=" + xluebuilding.EditValue.ToString();
            }
            if (xluelevel.EditValue != null)
            {
                squery += " and unitlevel=" + xluelevel.EditValue.ToString();
            }


            if (xccbpptype.Text != "")
            {
               //squery += " and unittype='" + xccbpptype.EditValue.ToString() + "'";

                string sss = " and unittype in ('0'";
                for (int ii = 0; ii < xccbpptype.Properties.Items.Count; ii++)
                {
                    if (xccbpptype.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbpptype.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;
            }

            if (xlueppsc.EditValue != null)
            {
                squery += " and unitorg='" + xlueppsc.EditValue.ToString() + "'";
            }

            if (xccbppstatus.Text != "")
            {
                string sss = " and unitstatus in ('0'";
                for (int ii = 0; ii < xccbppstatus.Properties.Items.Count; ii++)
                {
                    if (xccbppstatus.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbppstatus.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;
            }
            else
            {
                squery += " and unitstatus not in ('已拆','已并') ";
            }

            if (xlueppcusname.EditValue != null)
            {
                squery += " and c.cusid=" + xlueppcusname.EditValue.ToString();
            }
            if (xlueppcontractname.EditValue != null)
            {
                squery += " and a.contractid=" + xlueppcontractname.EditValue.ToString();
            }


            if (xlueppuareafrom.EditValue != null)
            {
                squery += " and unituarea>=" + xlueppuareafrom.EditValue.ToString();
            }
            if (xlueppuareato.EditValue != null)
            {
                squery += " and unituarea<=" + xlueppuareato.EditValue.ToString();
            }

            if (xdeppcsdtfrom.EditValue != null)
            {
                squery += " and contractsdt>='" + xdeppcsdtfrom.Text.ToString() + "'";
            }
            if (xdeppcsdtto.EditValue != null)
            {
                squery += " and contractsdt<='" + xdeppcsdtto.Text.ToString() + "'";
            }

            if (xdeppcedtfrom.EditValue != null)
            {
                squery += " and contractedt>='" + xdeppcedtfrom.Text.ToString() + "'";
            }
            if (xdeppcedtto.EditValue != null)
            {
                squery += " and contractedt<='" + xdeppcedtto.Text.ToString() + "'";
            }



            xgcppunitgrid.DataSource = mydb.gettablebystr(squery);

            //xgvppunitgrid.BestFitColumns();

            xsccppunitmodify.SplitterPosition = 0;

            XScmWF.CloseWaitForm();

        }

        private void xluebuilding_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void xluelevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void changeuienable_ppquery()
        {
            if (xccbarea.Text!=""&&
                !xccbarea.Text.Contains(","))
            {
                xluebuilding.Enabled = true;
            }
            else
            {
                xluebuilding.EditValue = null;
                xluebuilding.Enabled = false;

                xluelevel.EditValue = null;
                xluelevel.Enabled = false;
            }

            if (xluebuilding.EditValue != null)
            {
                xluelevel.Enabled = true;
            }
            else
            {
                xluelevel.Enabled = false;
            }
        }

 

        private void xluebuilding_EditValueChanged(object sender, EventArgs e)
        {
            xlueppcontractname_EditValueChanged(sender, e);

            if (xluebuilding.EditValue != null)
            {
                string sbuilding = xluebuilding.EditValue.ToString();


                xluelevel.Properties.DataSource = mydb.getpplevelbybuildingid(sbuilding);
            }

            changeuienable_ppquery();
        }

        private void xlueppuareafrom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (xlueppuareafrom.Properties.DataSource == null)
            {

                xlueppuareafrom.Properties.DataSource = mydb.getppuarea();
                xlueppuareato.Properties.DataSource = xlueppuareafrom.Properties.DataSource;

                (sender as DevExpress.XtraEditors.LookUpEdit).ShowPopup();
            }

            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).EditValue = null;
            }
        }

        private void xlueppcusname_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (xlueppcusname.Properties.DataSource == null)
            {

                xlueppcusname.Properties.DataSource = mydb.getcusnameall();

                (sender as DevExpress.XtraEditors.LookUpEdit).ShowPopup();
            }

            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).EditValue = null;
            }
        }

        private void xlueppcusname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                xlueppcusname.EditValue = null;
            }
        }

        private void xlueppcontractname_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                xlueppcontractname.EditValue = null;
            }

        }

        private void xlueppcontractname_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (xlueppcontractname.Properties.DataSource == null)
            {

                xlueppcontractname.Properties.DataSource = mydb.getcontractnoall();

                (sender as DevExpress.XtraEditors.LookUpEdit).ShowPopup();
            }

            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).EditValue = null;
            }


        }

        private void obbippunitquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ribbonControl.SelectedPage != xrppp)
            {
                ribbonControl.SelectedPage = xrppp;
            }

            initppunitgridview();
        }

 

        private void xluepptype_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void xlueppsc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void xlueppstatus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void xlueppuareafrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void xlueppuareato_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }

        }

        private void xlueppcontractname_EditValueChanged(object sender, EventArgs e)
        {
            if ((sender as DevExpress.XtraEditors.BaseEdit).EditValue != null)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).ForeColor = Color.Red;
            }
            else
            {
                (sender as DevExpress.XtraEditors.BaseEdit).ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
            }


        }

        private void xdeppcsdtfrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.DateEdit).EditValue = null;
            }

        }



        private void showppunitmodifypanel(DataRow dr)
        {
            //
            xtcppunitchange.SelectedTabPage = xtpppunitmodi;


            //            xlueppareax.Properties.NullText = dr["unitareaname"].ToString();
            //            xlueppbuildingx.Properties.NullText = dr["unitbuildingname"].ToString();
            //            xluepplevelx.Properties.NullText = dr["unitlevelname"].ToString();

            xteppnox.Text = dr["unitno"].ToString();
            xluepptypex.EditValue = dr["unittype"].ToString();
            xteppuareax.Text = dr["unituarea"].ToString();
            xtepprentx.Text = dr["unitrent"].ToString();
            xteppbfeex.Text = dr["unitbfee"].ToString();


            xcepptypex.Checked = dr["unitstatus"].ToString() == "保留" ? true : false;


            xsccppunitmodify.SplitterPosition = 229;

        }

        private void showppunitcombinepanel(DataRow dr)
        {
            //
            xtcppunitchange.SelectedTabPage = xtpppunitcombine;



            xluepptypecombine.EditValue = dr["unittype"].ToString();


            float iuarea = 0, irent = 0, ibfee = 0;

            foreach (int i7 in xgvppunitgrid.GetSelectedRows())
            {
                DataRow dr2 = xgvppunitgrid.GetDataRow(i7);

                iuarea += Convert.ToSingle(dr2["unituarea"].ToString());
                irent += Convert.ToSingle(dr2["unitrent"].ToString());
                ibfee += Convert.ToSingle(dr2["unitbfee"].ToString());

            }

            xteppuareacombine.Text = iuarea.ToString();
            xtepprentcombine.Text = irent.ToString();
            xteppbfeecombine.Text = ibfee.ToString();


            xteppnocombine.Text = dr["unitno"].ToString() + "-H";



            xsccppunitmodify.SplitterPosition = 229;

        }


        private void showppunitsplitpanel(DataRow dr)
        {
            //
            xtcppunitchange.SelectedTabPage = xtpppunitsplit;




            xteppnobesplited.Text = dr["unitno"].ToString();

            xteppnosplit.Text = xteppnobesplited.Text + "-F";
            xluepptypesplit.EditValue = dr["unittype"].ToString();


            xsccppunitmodify.SplitterPosition = 229;

        }

        private void bbippmodi_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (xtcsys.SelectedTabPage == xtpppunitgrid)
            {
                DataRow dr = xgvppunitgrid.GetFocusedDataRow();

                if (dr == null)
                {
                    showopermsg("X", "[修改房间] 没有选中记录!");
                    return;
                }

                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_m_ppunit", dr))
                {
                    showopermsg("X", soper);
                    return;
                }

                showppunitmodifypanel(dr);
            }

            if (xtcsys.SelectedTabPage == xtpppunitlv)
            {
                DataRow dr = xlvppunit.GetFocusedDataRow();

                if (dr == null)
                {
                    showopermsg("X", "[修改房间] 没有选中记录!");
                    return;
                }

                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_m_ppunit", dr))
                {
                    showopermsg("X", soper);
                    return;
                }


                //初始化
                initppunitgridview();

                xtcsys.SelectedTabPage = xtpppunitgrid;
                xgcppunitgrid.DataSource = xlvppunit.DataSource;


                //定位到选中的记录上
                Int32 searchid = Convert.ToInt32(dr["ppid"].ToString());

                DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcppunitgrid.MainView;

                int rhFound = cv.LocateByValue("ppid", searchid);

                // focusing the cell
                if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    cv.FocusedRowHandle = rhFound;

                    cv.ClearSelection();

                    cv.SelectRow(rhFound);

                }


                showppunitmodifypanel(dr);

            }
        }

        private void splitContainerControl14_Resize(object sender, EventArgs e)
        {
            (sender as DevExpress.XtraEditors.SplitContainerControl).SplitterPosition =
                Convert.ToInt32((sender as DevExpress.XtraEditors.SplitContainerControl).Size.Height - 53);
        }

        private void xgvppunitgrid_Click(object sender, EventArgs e)
        {

            if (xsccppunitmodify.SplitterPosition == 0) return;

            Point point = xgcppunitgrid.PointToClient(Cursor.Position);

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = xgvppunitgrid.CalcHitInfo(point);

            if (!hit.InRowCell && !hit.InRow) return;

            DataRow dr = xgvppunitgrid.GetFocusedDataRow();

            showppunitmodifypanel(dr);

        }

        private void iAbout_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageBox.Show(this.Text+"\r\nCopyright by CBLC\r\n", "欢迎使用");
        }

        private void xsbppmodicancel_Click(object sender, EventArgs e)
        {
            xsccppunitmodify.SplitterPosition = 0;
        }

        private void updateppnuit(DataRow dr)
        {


            dr["unituarea"] = xteppuareax.Text;
            dr["unitrent"] = xtepprentx.Text;
            dr["unitbfee"] = xteppbfeex.Text;
            dr["unittype"] = xluepptypex.Text;

            dr["unitno"] = xteppnox.Text;

            mydb.updateppunitbyid(dr);

            showopermsg("Y", "<修改房间> 修改房间资料成功！");
        }

        private void xsbppmodiapply_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvppunitgrid.GetFocusedDataRow();

            if (dr == null) return;

            //状态判断
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_m_ppunit", dr))
            {
                showopermsg("X", soper);
                return;
            }

            bool bfg=dr["unitstatus"].ToString()=="保留"?true:false;
            

            if ((xteppuareax.Text != dr["unituarea"].ToString()) ||
               (xtepprentx.Text != dr["unitrent"].ToString()) ||
               (xteppbfeex.Text != dr["unitbfee"].ToString()) ||
                (xluepptypex.Text != dr["unittype"].ToString())||
                (xcepptypex.Checked!=bfg)
            )
            {
                if(xcepptypex.Checked == bfg)updateppnuit(dr);

                if (xcepptypex.Checked != bfg)
                {
                    if (xcepptypex.Checked) dr["unitstatus"] = "保留";
                    else dr["unitstatus"] = "空闲";

                    mydb.updateppunitbyid(dr,dr["unitstatus"]);
                }
            }
            else
            {
                showopermsg("X", "[修改房间] 房间数据并没有修改，无法保存!");
            }



        }

        private void xsbppmodiok_Click(object sender, EventArgs e)
        {
            xsbppmodiapply_Click(sender, e);

            xsccppunitmodify.SplitterPosition = 0;
        }

        private void xsbppaddok_Click(object sender, EventArgs e)
        {
            if (xlueppareaadd.EditValue == null || xlueppbuildingadd.EditValue == null || xlueppleveladd.EditValue == null)
            {
                showopermsg("X", "[增加房间] 项目|楼宇|楼层 不能为空!");
                return;
            }

            if (xluepptypeadd.EditValue == null)
            {
                showopermsg("X", "[增加房间] 房间类型不能为空！");
                return;
            }

            if (xteppnoadd.Text == "")
            {
                showopermsg("X", "[增加房间]  房间编号 不能为空!");
                return;
            }

            //添加数据库记录

            Int32 iaid = mydb.addppunit(xlueppareaadd.EditValue.ToString(),
                                        xlueppbuildingadd.EditValue.ToString(),
                                        xlueppleveladd.EditValue.ToString(),
                                        xluepptypeadd.EditValue.ToString(),
                                        xteppnoadd.Text,
                                        xteppuareaadd.Text,
                                        xtepprentadd.Text,
                                        xteppbfeeadd.Text);


            if (xgcppunitgrid.DataSource != null)
            {
                DataTable dt1 = ((DataView)xgvppunitgrid.DataSource).Table;

                string squery = wheda.db.dboper.sppunitquery + " where a.ppid= " + iaid.ToString();


                DataTable dt2 = mydb.gettablebystr(squery);

                dt1.ImportRow(dt2.Rows[0]);

                xgcppunitgrid.DataSource = dt1;
            }
            else
            {
                string squery = wheda.db.dboper.sppunitquery + " where a.ppid= " + iaid.ToString();


                xgcppunitgrid.DataSource = mydb.gettablebystr(squery);


            }

            //定位到选中的记录上
            Int32 searchid = iaid;

            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcppunitgrid.MainView;

            int rhFound = cv.LocateByValue("ppid", searchid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }


            showopermsg("Y", "[添加房间] 成功添加一个房间记录!");
        }

        private void xsbppaddcancel_Click(object sender, EventArgs e)
        {
            xsccppunitmodify.SplitterPosition = 0;
        }

        private void bbippadd_ItemClick(object sender, ItemClickEventArgs e)
        {
            bool bfrom = (xtcsys.SelectedTabPage == xtpppunitlv);

            initppunitgridview();

            //从哪个界面选的添加:树形界面
            DataRow dr;
            if (bfrom)
            {
                xgcppunitgrid.DataSource = xlvppunit.DataSource;

                dr = xlvppunit.GetFocusedDataRow();
            }
            else
            {
                dr = xgvppunitgrid.GetFocusedDataRow();
            }



            if (dr != null)
            {

                xlueppareaadd.EditValue = dr["unitarea"];
                xlueppbuildingadd.EditValue = dr["unitbuilding"];
                xlueppleveladd.EditValue = dr["unitlevel"];
            }
            else
            {
                if (xlvppunit.ViewCaption.Contains("|"))
                {

                    xlueppareaadd.EditValue = Convert.ToInt32(tlpp.FocusedNode.ParentNode["parentid"].ToString());
                    xlueppbuildingadd.EditValue = Convert.ToInt32(tlpp.FocusedNode["parentid"].ToString());
                    xlueppleveladd.EditValue = Convert.ToInt32(tlpp.FocusedNode["id"].ToString());
                }
            }



            xtcsys.SelectedTabPage = xtpppunitgrid;
            xtcppunitchange.SelectedTabPage = xtpppunitadd;





            xsccppunitmodify.SplitterPosition = 229;

        }

        private void changeuienable_add()
        {
            if (xlueppareaadd.EditValue != null)
            {
                xlueppbuildingadd.Enabled = true;
            }
            else
            {
                xlueppbuildingadd.EditValue = null;
                xlueppbuildingadd.Enabled = false;

                xlueppleveladd.EditValue = null;
                xlueppleveladd.Enabled = false;
            }

            if (xlueppbuildingadd.EditValue != null)
            {
                xlueppleveladd.Enabled = true;
            }
            else
            {
                xlueppleveladd.Enabled = false;
            }
        }

        private void xlueppareaadd_EditValueChanged(object sender, EventArgs e)
        {
            xlueppcontractname_EditValueChanged(sender, e);

            if (xlueppareaadd.EditValue != null)
            {
                string sarea = xlueppareaadd.EditValue.ToString();


                xlueppbuildingadd.Properties.DataSource = mydb.getppbuildingbyareaid(sarea);
            }

            changeuienable_add();
        }

        private void xlueppbuildingadd_EditValueChanged(object sender, EventArgs e)
        {
            xlueppcontractname_EditValueChanged(sender, e);

            if (xlueppbuildingadd.EditValue != null)
            {
                string sbuilding = xlueppbuildingadd.EditValue.ToString();


                xlueppleveladd.Properties.DataSource = mydb.getpplevelbybuildingid(sbuilding);
            }

            changeuienable_add();
        }

        private void bbippdel_ItemClick(object sender, ItemClickEventArgs e)
        {



            if (xtcsys.SelectedTabPage == xtpppunitgrid)
            {
                if (xgvppunitgrid.SelectedRowsCount < 1)
                {
                    showopermsg("X", "[删除房间] 没有选中记录!");
                    return;
                }

                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_d_ppunits", xgvppunitgrid))
                {
                    showopermsg("X", soper);
                    return;
                }


                DialogResult dr1 = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr1 == DialogResult.Cancel) return;



                foreach (Int32 i3 in xgvppunitgrid.GetSelectedRows())
                {
                    DataRow dr = xgvppunitgrid.GetDataRow(i3);

                    mydb.deleteppunit(dr["ppid"].ToString());


                }
                xgvppunitgrid.DeleteSelectedRows();


            }

            if (xtcsys.SelectedTabPage == xtpppunitlv)
            {

                if (xlvppunit.SelectedRowsCount < 1)
                {
                    showopermsg("X", "[删除房间] 没有选中记录!");
                    return;
                }



                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_d_ppunits", xlvppunit))
                {
                    showopermsg("X", soper);
                    return;
                }


                DialogResult dr1 = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr1 == DialogResult.Cancel) return;



                foreach (Int32 i3 in xlvppunit.GetSelectedRows())
                {
                    DataRow dr = xlvppunit.GetDataRow(i3);

                    mydb.deleteppunit(dr["ppid"].ToString());


                }
                xlvppunit.DeleteSelectedRows();


            }

            showopermsg("Y", "<删除房间> 删除房间记录成功！");

        }

        private void bbippcombine_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xtcsys.SelectedTabPage == xtpppunitgrid)
            {
                initppunitlvview();

                xtcsys.SelectedTabPage = xtpppunitlv;
                return;
            }


            if (xtcsys.SelectedTabPage == xtpppunitlv)
            {
                if (xlvppunit.SelectedRowsCount <= 1)
                {
                    showopermsg("X", "[合并房间] 请选择多条记录进行合并！");
                    return;
                }


                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_c_ppunits", xlvppunit))
                {
                    showopermsg("X", soper);
                    return;
                }

                initppunitgridview();


                xgcppunitgrid.DataSource = xlvppunit.DataSource;

                xgvppunitgrid.ClearSelection();

                foreach (Int32 i3 in xlvppunit.GetSelectedRows())
                {

                    xgvppunitgrid.SelectRow(i3);


                }

                DataRow dr = xlvppunit.GetFocusedDataRow();

                showppunitcombinepanel(dr);

                xtcsys.SelectedTabPage = xtpppunitgrid;

            }




        }

        private void bbippsplit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xtcsys.SelectedTabPage == xtpppunitgrid)
            {
                if (xgvppunitgrid.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[分拆房间] 选中了多条记录!请只选一条记录进行拆分");
                    return;
                }

                DataRow dr = xgvppunitgrid.GetFocusedDataRow();

                if (dr == null)
                {
                    showopermsg("X", "[分拆房间] 没有选中记录!");
                    return;
                }

                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_s_ppunit", dr))
                {
                    showopermsg("X", soper);
                    return;
                }


                showppunitsplitpanel(dr);
            }

            if (xtcsys.SelectedTabPage == xtpppunitlv)
            {
                if (xlvppunit.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[分拆房间] 选中了多条记录!请只选一条记录进行拆分");
                    return;
                }

                DataRow dr = xlvppunit.GetFocusedDataRow();

                if (dr == null)
                {
                    showopermsg("X", "[分拆房间] 没有选中记录!");
                    return;
                }

                //状态检查
                string soper = "";
                if (!mydb.checkoperconditions(ref soper, "op_s_ppunit", dr))
                {
                    showopermsg("X", soper);
                    return;
                }


                initppunitgridview();

                xtcsys.SelectedTabPage = xtpppunitgrid;
                xgcppunitgrid.DataSource = xlvppunit.DataSource;


                //定位到选中的记录上
                Int32 searchid = Convert.ToInt32(dr["ppid"].ToString());

                DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcppunitgrid.MainView;

                int rhFound = cv.LocateByValue("ppid", searchid);

                // focusing the cell
                if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
                {
                    cv.FocusedRowHandle = rhFound;

                    cv.ClearSelection();

                    cv.SelectRow(rhFound);

                }


                showppunitsplitpanel(dr);

            }

        }

        private void xsbppsplitcancel_Click(object sender, EventArgs e)
        {
            xsccppunitmodify.SplitterPosition = 0;
        }

        private void xsbppsplitok_Click(object sender, EventArgs e)
        {

            if (xteppnosplit.Text == "" ||
                xluepptypesplit.EditValue == null)
            {
                showopermsg("X", "[分拆房间] 房间编号|房间类型 不能为空!");
                return;
            }

            DataRow dr = xgvppunitgrid.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[分拆房间] 出现错误，请重新操作一次！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_s_ppunit", dr))
            {
                showopermsg("X", soper);
                return;
            }


            //添加数据库记录

            DataTable dts = mydb.splitppunit(dr,
                             xteppnosplit.Text,
                             xluepptypesplit.Text,
                             xteppuareasplit.Text,
                             xtepprentsplit.Text,
                             xteppbfeesplit.Text);



            DataTable dtd = ((DataView)xgvppunitgrid.DataSource).Table;

            dtd.ImportRow(dts.Rows[0]);

            dr["unitstatus"] = "已拆";

            showopermsg("Y", "[分拆房间] 成功分拆了一个房间!");

            xsccppunitmodify.SplitterPosition = 0;
        }

        private void xsbppcombinecancel_Click(object sender, EventArgs e)
        {
            xsccppunitmodify.SplitterPosition = 0;
        }

        private void xsbppcombineok_Click(object sender, EventArgs e)
        {

            if (xteppnocombine.Text == "")
            {
                showopermsg("X", "[合并房间] 房间名称|房间编号 不能为空!");
                return;
            }

            if (xgvppunitgrid.SelectedRowsCount <= 1)
            {
                showopermsg("X", "[合并房间] 出现错误，请重新操作一次！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_c_ppunits", xgvppunitgrid))
            {
                showopermsg("X", soper);
                return;
            }


            //添加数据库记录

            DataTable dtd = ((DataView)xgvppunitgrid.DataSource).Table;


            DataTable dts = mydb.combineppunit(xgvppunitgrid,
                               xteppnocombine.Text,
                               xluepptypecombine.Text,
                               xteppuareacombine.Text,
                               xtepprentcombine.Text,
                               xteppbfeecombine.Text);

            dtd.ImportRow(dts.Rows[0]);

            foreach (int i7 in xgvppunitgrid.GetSelectedRows())
            {
                DataRow dr2 = xgvppunitgrid.GetDataRow(i7);

                dr2["unitstatus"] = "已并";
            }

            showopermsg("Y", "[合并房间] 成功合并了房间!");
        }


        private void xsbcusquery_Click(object sender, EventArgs e)
        {
            string squery = wheda.db.dboper.scusquery + " where 1=1  ";

            if (xluecusarea.EditValue != null)
            {
                squery += " and cusarea='" + xluecusarea.EditValue.ToString() + "'";
            }

            if (xtecusname.Text != "")
            {
                squery += " and cusname like '%" + xtecusname.Text + "%'";
            }




            DataTable dt = mydb.gettablebystr(squery+" order by cusid desc");

            //DataColumn dc= dt.Columns.Add("gvsel", System.Type.GetType("System.Boolean"));

            //dc.Caption = "";


            xgccus.DataSource = dt;

            //xgvcus.BestFitColumns();

            xscccusinfochange.SplitterPosition = 0;

            xgvcus.Focus();
        }

        private void xluecustype_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                (sender as DevExpress.XtraEditors.LookUpEdit).EditValue = null;
            }
        }

        private void showusermodifypanel(DataRow dr)
        {
            //
            xtcuserchange.SelectedTabPage = xtpmodiuser;


            xteusernamemodi.Text = dr["username"].ToString();
            xteuserpassmodi.Text = dr["userpassword"].ToString();
            xteuserdescmodi.Text = dr["userdesc"].ToString();



            xsccuserchange.SplitterPosition = 229;

        }

        private void showcusmodifypanel(DataRow dr)
        {
            //
            xtccusinfochange.SelectedTabPage = xtpcusinfomodi;

            xluecusareamodi.EditValue = Convert.ToInt32(dr["cusarea"].ToString());

            xtecusnamemodi.EditValue = dr["cusname"].ToString();
            xtecusnomodi.EditValue = dr["cusno"].ToString();

            xtecusmobnummodi.EditValue = dr["cusmobnum"].ToString();
            xtecusaddrmodi.EditValue = dr["cusaddr"].ToString();



            xscccusinfochange.SplitterPosition = 229;

        }

        private void showcontractmodifypanel(DataRow dr)
        {
            //
            xtccontractchange.SelectedTabPage = xtpcontractmodi;

            xtectnomodi.Text = dr["contractno"].ToString();

            xdectsigndtmodi.Text = dr["signdt"].ToString();
            xluectareamodi.EditValue = Convert.ToInt32(dr["contractarea"].ToString());
            xluectpptypemodi.EditValue = dr["contractpptype"].ToString();


            xluectrentpaystylemodi.EditValue = dr["rentpaystyle"].ToString();
            xsectrentfreeperiodmodi.Text = dr["rentfreeperiod"].ToString();
            xtectdepositmodi.Text = dr["depositfee"].ToString();
            xtecttargetmodi.Text = dr["unittarget"].ToString();

            xdectsigndtmodi.Text = dr["signdt"].ToString();
            xdectsdtmodi.Text = dr["contractsdt"].ToString();
            xdectedtmodi.Text = dr["contractedt"].ToString();



            xscccontractchange.SplitterPosition = 229;

        }

        private void showcontractaddpanel(DataRow dr)
        {
            initcontractinfoform();

            xtcsys.SelectedTabPage = xtpcontract;
            
            //
            xtccontractchange.SelectedTabPage = xtpcontractadd;

            //xtecontractnoadd.Text = "";




            xscccontractchange.SplitterPosition = 229;

        }

        private void showcusaddpanel(DataRow dr)
        {
            //
            xtccusinfochange.SelectedTabPage = xtpcusinfoadd;

            xluecusareaadd.EditValue = null;

            xtecusnoadd.Text = "";
            xtecusnameadd.Text = "";
            xtecusmobnumadd.Text = "";
            xtecusaddradd.Text = "";


            if (xluecusarea.EditValue != null)
            {
                xluecusareaadd.EditValue = xluecusarea.EditValue;
            }

            xscccusinfochange.SplitterPosition = 229;

        }

        private void showuseraddpanel(DataRow dr)
        {
            //
            xtcuserchange.SelectedTabPage = xtpadduser;



            xteusernameadd.Text = "";
            xteuserpassadd.Text = "";
            xteuserdescadd.Text = "";




            xsccuserchange.SplitterPosition = 229;

        }

        private void bbicusmodi_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = xgvcus.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[修改客户] 没有选中记录!");
                return;
            }


            showcusmodifypanel(dr);
        }



        private void bbicusadd_ItemClick(object sender, ItemClickEventArgs e)
        {
            showcusaddpanel(null);
        }

        private void xsbcusmodicancel_Click(object sender, EventArgs e)
        {
            xscccusinfochange.SplitterPosition = 0;
        }

        private void xsbcusaddcancel_Click(object sender, EventArgs e)
        {
            xscccusinfochange.SplitterPosition = 0;
        }

        private void xgvcus_Click(object sender, EventArgs e)
        {
            if (xscccusinfochange.SplitterPosition == 0) return;

            Point point = xgccus.PointToClient(Cursor.Position);

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = xgvcus.CalcHitInfo(point);

            if (!hit.InRowCell && !hit.InRow) return;

            DataRow dr = xgvcus.GetFocusedDataRow();

            showcusmodifypanel(dr);
        }

        private void xsbcusmodiapply_Click(object sender, EventArgs e)
        {
            string supd = "update t_cus set cusid=cusid ";
            string stmp = supd;

            DataRow dr = xgvcus.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[修改客户] 出现一般性错误，请重试，若错误再次出现请联系联创科技！");
                return;
            }


            dr["cusname"] = xtecusnamemodi.Text;

            dr["cusmobnum"] = xtecusmobnummodi.Text;
            dr["cusaddr"] = xtecusaddrmodi.Text;


            mydb.updatecusbyid(dr);

            showopermsg("Y", "<修改客户> 成功修改客户资料！");

            //更新dr


        }

        private void bbicusdel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xgvcus.SelectedRowsCount < 1)
            {
                showopermsg("X", "[删除客户] 您没有选中客户记录！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            foreach (Int32 i3 in xgvcus.GetSelectedRows())
            {
                DataRow dr = xgvcus.GetDataRow(i3);

                mydb.deletecus(dr["cusid"].ToString());
            }


            xgvcus.DeleteSelectedRows();

            showopermsg("Y", "<删除客户> 删除客户资料成功！");
        }

        private void xsbcusmodiok_Click(object sender, EventArgs e)
        {
            xsbcusmodiapply_Click(sender, e);

            xscccusinfochange.SplitterPosition = 0;
        }

        private void xsbcusaddok_Click(object sender, EventArgs e)
        {

            if (xtecusnameadd.Text == "" ||
                xtecusnoadd.Text == "")
            {
                showopermsg("X", "[添加客户]  客户编号|客户姓名为必填项！");

                return;

            }


            DataTable dt;

            if (xgccus.DataSource != null)
            {
                dt = ((DataView)xgvcus.DataSource).Table;
            }
            else
            {
                string squery = wheda.db.dboper.scusquery + " where 1=2  ";


                xgccus.DataSource = mydb.gettablebystr(squery);

                dt = ((DataView)xgvcus.DataSource).Table;
            }

            DataRow dr = dt.NewRow();

            dr["cusno"] = xtecusnoadd.Text;
            dr["cusname"] = xtecusnameadd.Text;
            dr["cusarea"] = xluecusareaadd.EditValue;

            dr["cusmobnum"] = xtecusmobnumadd.Text;
            dr["cusaddr"] = xtecusaddradd.Text;


            Int32 inewid = mydb.addcus(dr);
            dr["cusid"] = inewid;


            dt.Rows.Add(dr);    //dt.ImportRow


            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgccus.MainView;

            int rhFound = cv.LocateByValue("cusid", inewid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }

            xluecusareaadd.EditValue = null;

            showopermsg("Y", "<添加客户> 成功添加客户资料!");
            xscccusinfochange.SplitterPosition = 0;

        }

        private void xsbcontractquery_Click(object sender, EventArgs e)
        {
            string squery = wheda.db.dboper.scontractquery + " where 1=1  ";

            if (xccbctareaquery.Text != "")
            {
                //squery += " and contractarea='" + xluectareaquery.EditValue.ToString() + "'";


                string sss = " and contractarea in ('0'";
                for (int ii = 0; ii < xccbctareaquery.Properties.Items.Count; ii++)
                {
                    if (xccbctareaquery.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbctareaquery.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;
            }

            if (xccbcontractstatus.Text != "")
            {
                //squery += " and contractstatus='" + xluecontractstatus.EditValue.ToString() + "'";

                string sss = " and contractstatus in ('0'";
                for (int ii = 0; ii < xccbcontractstatus.Properties.Items.Count; ii++)
                {
                    if (xccbcontractstatus.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbcontractstatus.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;

            }

            if (xluecontractorg.EditValue != null)
            {
                squery += " and contractorg='" + xluecontractorg.EditValue.ToString() + "'";
            }


            if (xdesdtquerys.EditValue != null)
            {
                squery += " and ContractSDT>='" + xdesdtquerys.Text.ToString() + "'";
            }

            if (xdesdtquerye.EditValue != null)
            {
                squery += " and ContractSDT<='" + xdesdtquerye.Text.ToString() + "'";
            }

            if (xdeedtquerys.EditValue != null)
            {
                squery += " and ContractEDT>='" + xdeedtquerys.Text.ToString() + "'";
            }

            if (xdeedtquerye.EditValue != null)
            {
                squery += " and ContractEDT<='" + xdeedtquerye.Text.ToString() + "'";
            }

            if (xdecontractsignsdt.EditValue != null)
            {
                squery += " and signdt>='" + xdecontractsignsdt.Text.ToString() + "'";
            }
            if (xdecontractsignedt.EditValue != null)
            {
                squery += " and signdt<='" + xdecontractsignedt.Text.ToString() + "'";
            }



            xgccontract.DataSource = mydb.gettablebystr(squery);

            //xgvcontract.BestFitColumns();

            xscccontractchange.SplitterPosition = 0;

            xgvcontract_FocusedRowChanged(null, null);

            xgccontract.Focus();
        }

 
        private void bbicontractadd_ItemClick(object sender, ItemClickEventArgs e)
        {
            //initcontractinfoform();
            showcontractaddpanel(null);
        }

        private void xsbcontractaddcancel_Click(object sender, EventArgs e)
        {
            xscccontractchange.SplitterPosition = 0;
        }

        private void xsbcontractaddok_Click(object sender, EventArgs e)
        {

            if (xluecontractarea.EditValue == null ||
               xluecontractpptype.EditValue == null ||
               xluecontractrentpaystyle.EditValue == null ||
                xdecontractsdt.EditValue == null ||
                xdecontractedt.EditValue == null ||
                xbectcus.Tag == null
               )
            {
                showopermsg("X", "[添加合同]  项目|类型|客户|收租方式|开始日期|结束日期 为必填项！");

                return;

            }


            DataTable dt;

            if (xgccontract.DataSource != null)
            {
                dt = ((DataView)xgvcontract.DataSource).Table;
            }
            else
            {
                string squery = wheda.db.dboper.scontractquery + " where 1=2  ";


                xgccontract.DataSource = mydb.gettablebystr(squery);

                dt = ((DataView)xgvcontract.DataSource).Table;
            }

            DataRow dr = dt.NewRow();

            dr["contractno"] = xtecontractno.Text;
            dr["contractarea"] = xluecontractarea.EditValue;
            dr["contractpptype"] = xluecontractpptype.Text;
            dr["rentpaystyle"] = xluecontractrentpaystyle.Text;
            dr["rentfreeperiod"] = xsecontractrentfreeperiod.Text;
            dr["depositfee"] = xtecontractdeposit.Text;
            dr["unittarget"] = xtecontractunittarget.Text;

            dr["contractstatus"] = "初登";
            dr["contractorg"] = "新签";

            dr["signdt"] = xdesigndtadd.Text;
            dr["contractsdt"] = xdecontractsdt.Text;
            dr["contractedt"] = xdecontractedt.Text;

            dr["cusname"] = xbectcus.Text;
            dr["cusid"] = xbectcus.Tag.ToString().Split(new char[]{'@'})[0];
            dr["cusno"] = xbectcus.Tag.ToString().Split(new char[] { '@' })[1];

  

            Int32 ictid = mydb.addcontract(dr);

            dr["contractid"] = ictid;

            dt.Rows.Add(dr);    //dt.ImportRow



            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgccontract.MainView;

            int rhFound = cv.LocateByValue("contractid", ictid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }


            xluecontractarea.EditValue = null;
            xluecontractpptype.EditValue = null;
            xtecontractdeposit.Text = "0";
            xtecontractno.EditValue = null;
            xtecontractunittarget.EditValue = null;
            xluecontractrentpaystyle.EditValue = null;
            xsecontractrentfreeperiod.EditValue = 0;
            xdesigndtadd.EditValue = null;
            xdecontractedt.EditValue = null;
            xdecontractsdt.EditValue = null;
            xbectcus.Text = "";
            xbectcus.Tag = null;


            showopermsg("Y", "<添加合同>  添加合同成功!");

            xscccontractchange.SplitterPosition = 0;

        }

        private void bbicontractmodi_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = xgvcontract.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[修改] 没有选中记录!");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_m_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }


            showcontractmodifypanel(dr);
        }

        private void xgvcontract_Click(object sender, EventArgs e)
        {
            if (xscccontractchange.SplitterPosition == 0) return;

            Point point = xgccontract.PointToClient(Cursor.Position);

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = xgvcontract.CalcHitInfo(point);

            if (!hit.InRowCell && !hit.InRow) return;

            DataRow dr = xgvcontract.GetFocusedDataRow();

            showcontractmodifypanel(dr);
        }


        private void xsbcontractmodicancel_Click(object sender, EventArgs e)
        {
            xscccontractchange.SplitterPosition = 0;
        }

        private void xsbcontractchangeapply_Click(object sender, EventArgs e)
        {
            string supd = "update t_contract set contractid=contractid ";
            string stmp = supd;

            DataRow dr = xgvcontract.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[修改合同] 出现一般性错误，请重试，若错误再次出现请联系联创科技！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_m_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }



            dr["rentpaystyle"] = xluectrentpaystylemodi.EditValue;
            dr["rentfreeperiod"] = xsectrentfreeperiodmodi.Text;
            dr["depositfee"] = xtectdepositmodi.Text;
            dr["unittarget"] = xtecttargetmodi.Text;
            dr["contractsdt"] = xdectsdtmodi.Text;
            dr["contractedt"] = xdectedtmodi.Text;
            dr["signdt"] = xdectsigndtmodi.Text;

            mydb.updatecontract(dr);

            showopermsg("Y", "<修改合同> 成功修改合同资料！");

        }

        private void xsbcontractmodiok_Click(object sender, EventArgs e)
        {
            xsbcontractchangeapply_Click(sender, e);

            xscccontractchange.SplitterPosition = 0;
        }

        private void bbicontractdel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xgvcontract.SelectedRowsCount < 1)
            {
                showopermsg("X", "[删除合同] 您没有选中合同记录！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_d_contracts", xgvcontract))
            {
                showopermsg("X", soper);
                return;
            }


            DialogResult dr1 = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            foreach (Int32 i3 in xgvcontract.GetSelectedRows())
            {
                DataRow dr = xgvcontract.GetDataRow(i3);

                mydb.deletecontract(dr["contractid"].ToString());
            }


            xgvcontract.DeleteSelectedRows();

            showopermsg("Y", "<删除合同> 删除合同资料成功！");
        }

        private void xtcsys_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.PageDown ||
                e.KeyCode == Keys.PageUp)
            {
                e.Handled = true;
            }

        }

        private void xluesysusers_EditValueChanged(object sender, EventArgs e)
        {
            xteuserpassword.EditValue = null;
            xteuserpassword.Focus();
        }

        private void xsbclearpass_Click(object sender, EventArgs e)
        {
            xteuserpassword.EditValue = null;
            xteuserpassword.Focus();

        }

        public void saveip()
        {
            if (xlcip.Visible == true)
            {

                Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);



                oConfig.AppSettings.Settings["intranetserverip"].Value = innerip.Text;
                oConfig.AppSettings.Settings["internetserverip"].Value = outerip.Text;
                oConfig.AppSettings.Settings["iptype"].Value = xrgip.SelectedIndex == 0 ? "0" : "1";


                oConfig.Save(ConfigurationSaveMode.Modified);


                ConfigurationManager.RefreshSection("appSettings");

            }
        }

        private void initgridcolbyuer()
        {

            if (xrpfinance.Visible)
            {
                //合同条件查询
                DevExpress.XtraGrid.Columns.GridColumn _gc = xgvcontract.Columns.Add();
                _gc.Caption = "合同编号(财)";
                _gc.FieldName = "contractnofnc";
                _gc.VisibleIndex = 0;

                _gc.Visible = true;

                //合同信息
                _gc = xgvinfoqueryct.Columns.Add();
                _gc.Caption = "合同编号(财)";
                _gc.FieldName = "contractnofnc";
                _gc.VisibleIndex = 0;

                _gc.Visible = true;
            }
            else
            {
                 
            }


        }


        private void xsblogin_Click(object sender, EventArgs e)
        {
            //保存IP地址
            saveip();


            if (xluesysusers.EditValue == null)
            {
                showopermsg("X", "[登录] 请选择用户!");
                xluesysusers.Focus();
                return;
            }

            if (xteuserpassword.EditValue == null)
            {
                showopermsg("X", "[登录] 请输入密码!");
                xteuserpassword.Focus();
                return;
            }

            if (xteuserpassword.Text.ToUpper().Contains(" OR "))
            {
                MessageBox.Show("非法用户！","检测到SQL注入攻击");
                return;
            }

            bool blogin = mydb.verifyuser(xluesysusers.EditValue.ToString(), xteuserpassword.Text);

            if (!blogin)
            {
                showopermsg("X", "[登录] 密码错误或用户被禁用，请重新输入或与管理员联系!");
                return;
            }

            ribbonControl.Enabled = true;

            this.Text = (mydb.getsyspara()).Rows[0]["sysname"].ToString();


            showopermsg("Y", "<登录> 成功登录物业经营管理系统!");

            xbsiuser.Caption = "用户:[" + xluesysusers.GetColumnValue("username").ToString() + "]";

            xbsip.Caption = xrgip.SelectedIndex == 0 ? innerip.Text : outerip.Text;

            //检测最新版本，不提示，用户需要的时候再更新，保持稳定
         //   CheckNewestVer();

            //分配权限--uid为0是超级用户
            enableallprivilege(); 

            uid = xluesysusers.EditValue.ToString();

            if (uid != "0")
            {
                dc_user_pl = new Dictionary<string, DataRow>();
                DataTable d333 = mydb.getuserprivilege(xluesysusers.EditValue.ToString());
                foreach (DataRow dr in d333.Rows)
                {
                    DataRow d111;
                    if (!dc_user_pl.TryGetValue(dr["opername"].ToString(), out d111))
                    {
                        dc_user_pl.Add(dr["opername"].ToString(), dr);
                    }
                }

                dc_user_pl_cat = new Dictionary<string, DataRow>();
                d333 = mydb.getuserprivilegecat(xluesysusers.EditValue.ToString());
                foreach (DataRow dr in d333.Rows)
                {
                    dc_user_pl_cat.Add(dr["opername"].ToString(), dr);
                }


                DataRow dr11;
                //if (!dc_user_pl_cat.TryGetValue("homepage", out dr11)) xrphome.Visible = false;
                if (!dc_user_pl_cat.TryGetValue("contract", out dr11)) xrpcontract.Visible = false;
                if (!dc_user_pl_cat.TryGetValue("pp", out dr11)) xrppp.Visible = false;
                if (!dc_user_pl_cat.TryGetValue("cus", out dr11)) xrpcus.Visible = false;

                if (!dc_user_pl_cat.TryGetValue("finance", out dr11)) xrpfinance.Visible = false;
                if (!dc_user_pl_cat.TryGetValue("feeO", out dr11)) xrpfee.Visible = false;
                if (!dc_user_pl_cat.TryGetValue("reportO", out dr11)) xrpanalyse.Visible = false;
                //if (!dc_user_pl_cat.TryGetValue("system", out dr11)) xrpsystem.Visible = false;

            }

            initstartpage();

            inituserprivilege();

            initgridcolbyuer();

            this.TopMost = false;

            WindowState = System.Windows.Forms.FormWindowState.Maximized;


            //提醒线程
            //thread = new System.Threading.Thread(ScheThread);

            //thread.Start();

            //用定时器操作，scheduler控件非线程安全
          //  tminform.Enabled = true;



            loaduserpara();

        }

        public void loaduserpara()
        {
            DataTable dt=mydb.getuserpara(uid);
            //树形字体
            xbetlfont.Text = dt.Rows[0]["tlfont"].ToString();

            string[] ss = xbetlfont.Text.Split(new char[] { ',' });

            if (ss.Length == 2)
            {

                System.Drawing.Font f = new Font(new FontFamily(ss[0]), Convert.ToSingle(ss[1]));

                tlquery.Appearance.Row.Font = f;

                tlfnc.Appearance.Row.Font = f;

            }   

            //树形宽度
            xsetlwidth.Text = dt.Rows[0]["tlwidth"].ToString();

            xscctlfnc.SplitterPosition = Convert.ToInt32(xsetlwidth.Value);
            xscctlmgt.SplitterPosition = Convert.ToInt32(xsetlwidth.Value);
            xsccrptfnc.SplitterPosition = Convert.ToInt32(xsetlwidth.Value);

           //提前通知天数
            xseinformdays.Value =Convert.ToDecimal( dt.Rows[0]["ifdays"]);
        }

        public static bool Connectfileserver(string remoteHost, string shareName, string userName, string passWord)
        {
            bool Flag = false;
            System.Diagnostics.Process proc = new System.Diagnostics.Process(); ;
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use \\" + remoteHost + @"\" + shareName + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }

                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (String.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }

        private void CheckNewestVer()
        {
            DataTable dt = mydb.getsyspara();
            string sNewestVer = dt.Rows[0]["sysversion"].ToString();



            if (sAppVerDT.CompareTo(sNewestVer) < 0)
            {
                DialogResult dr1 = MessageBox.Show("有新版本[" + sNewestVer + "],需要更新吗？", "更新版本", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr1 == DialogResult.Cancel) return;

               

                if (xrgip.SelectedIndex == 0)
                {

                    XScmWF.ShowWaitForm();
                    XScmWF.SetWaitFormCaption("正在读取更新包...");


                    Connectfileserver("192.168.0.111", "app", "aaaa", "ftp_1234567890");

                    System.Diagnostics.Process.Start(dt.Rows[0]["ufilepos"].ToString());
                }
                else
                {
                    XScmWF.ShowWaitForm();
                    XScmWF.SetWaitFormCaption("正在读取更新包...");
                    XScmWF.SetWaitFormDescription("外网下载速度较慢，请耐心等候");

                    
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dt.Rows[0]["ufileposinternet"].ToString());
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    request.UseBinary = true;
                    //request.UsePassive = false;



                    // This example assumes the FTP site uses anonymous logon.
                    request.Credentials = new NetworkCredential("aaaa", "ftp_1234567890");

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    
                    Stream responseStream = response.GetResponseStream();
                   

                    System.IO.File.Delete("a001.exe");

                    FileStream fs = System.IO.File.Open("a001.exe", FileMode.Create);
                    BinaryWriter destination = new BinaryWriter(fs) ;

                    
                    byte[] chunk = new byte[4096]; 
                    int bytesRead;
                    while ((bytesRead = responseStream.Read(chunk, 0, chunk.Length)) > 0) 
                    {
                        destination.Write(chunk, 0, bytesRead); 
                    }

                    destination.Flush();

                    destination.Close();

                    fs.Close();
                    response.Close();
                    

                    System.Threading.Thread.Sleep(1000*5);
                    System.Diagnostics.Process.Start("a001.exe");

                }


                XScmWF.CloseWaitForm();

                try
                {
                    //Environment.Exit(0);
                    this.Close();
                }
                finally
                { }

            }
        }

        private void xteuserpassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                xsblogin_Click(sender, e);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            iExit_ItemClick(sender, null);
        }

        private void bbilogout_ItemClick(object sender, ItemClickEventArgs e)
        {
            ribbonControl.Enabled = false;

            xtcsys.SelectedTabPage = xtplogin;
            xteuserpassword.EditValue = null;
            ribbonControl.SelectedPage = xrppp;

        }

        private void bbichangeuser_ItemClick(object sender, ItemClickEventArgs e)
        {
            ribbonControl.Enabled = false;

            xtcsys.SelectedTabPage = xtplogin;
            xteuserpassword.EditValue = null;
            xluesysusers.ShowPopup();

            ribbonControl.SelectedPage = xrppp;

        }

        private void xsbareacodeshrink_Click(object sender, EventArgs e)
        {
            if (xsccarea.SplitterPosition <= 22)
            {
                xsccarea.SplitterPosition = 198;
            }
            else
            {
                xsccarea.SplitterPosition = 22;
            }
        }

        private void xsbbuildingcodeshrink_Click(object sender, EventArgs e)
        {
            if (xsccbuilding.SplitterPosition <= 22)
            {
                xsccbuilding.SplitterPosition = 198;
            }
            else
            {
                xsccbuilding.SplitterPosition = 22;
            }
        }

        private void xsblevelcodeshrink_Click(object sender, EventArgs e)
        {
            if (xscclevel.SplitterPosition <= 22)
            {
                xscclevel.SplitterPosition = 198;
            }
            else
            {
                xscclevel.SplitterPosition = 22;
            }
        }


        private void nbiparacode_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            initparacodeform();
        }

        private void nbippcode_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            initareacodeform();
        }

        private void xgvparacodecatalog_Click(object sender, EventArgs e)
        {
        }

        private void xgvparacodecatalog_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvparacodecatalog.GetFocusedDataRow();
            if (dr == null)
            {
                return;
            }

            string scat = dr["paraname"].ToString();


            xgcparacodevalue.DataSource = mydb.getparavaluebycat(scat);


            xgcparacodevalue_Click(sender, null);

        }

        private void xsbaddparacodevalue_Click(object sender, EventArgs e)
        {
            if (xteparacodevalue.Text.Length == 0)
            {
                showopermsg("X", "[增加参数编码] 编码取值不能为空！");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要增加编码[" + xteparacodevalue.Text + "]吗？", "确认增加", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            DataRow dr2 = xgvparacodecatalog.GetFocusedDataRow();
            string scat = dr2["paraname"].ToString();
            string scatname = dr2["paracatname"].ToString();


            Int32 searchid = mydb.addparavalue(scatname, scat, xteparacodevalue.Text, xseparacodeseqno.Text);


            xgvparacodecatalog_FocusedRowChanged(null, null);

            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcparacodevalue.MainView;

            int rhFound = cv.LocateByValue("id", searchid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }

            showopermsg("Y", "<增加参数编码> 成功增加参数编码数据！");

        }

        private void xsbmodiparacodevalue_Click(object sender, EventArgs e)
        {

            if (xgvparacodevalue.SelectedRowsCount > 1)
            {
                showopermsg("X", "[修改参数编码] 您选中了多个参数值，请选中一个参数值！");
                return;
            }

            DataRow dr = xgvparacodevalue.GetFocusedDataRow();
            if (dr == null)
            {
                showopermsg("X", "[修改参数编码] 你没有选中任何参数值!");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要修改选中参数值？", "确认修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            string sid = dr["id"].ToString();

            string snewvalue = xteparacodevalue.Text;
            string snewseqno = xseparacodeseqno.Text;


            mydb.updateparavalue(sid, snewvalue, snewseqno);

            dr["paravalue"] = snewvalue;
            dr["paraseqno"] = snewseqno;

            showopermsg("Y", "<修改参数编码> 成功修改参数编码数据！");


        }

        private void xsbdelparacodevalue_Click(object sender, EventArgs e)
        {

            if (xgvparacodevalue.SelectedRowsCount < 1)
            {
                showopermsg("X", "[删除参数编码] 没有选中任何参数值!");
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要删除选中的参数值吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            foreach (Int32 i3 in xgvparacodevalue.GetSelectedRows())
            {
                DataRow dr = xgvparacodevalue.GetDataRow(i3);

                mydb.deleteparavalue(dr["id"].ToString());
            }

            xgvparacodevalue.DeleteSelectedRows();

            showopermsg("Y", "<删除参数编码> 删除参数编码数据成功！");
        }

        private void xgcparacodevalue_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvparacodevalue.GetFocusedDataRow();
            if (dr == null) return;

            xteparacodevalue.Text = dr["paravalue"].ToString();
            xseparacodeseqno.Text = dr["paraseqno"].ToString();
        }

        private void xgcparacodevalue_Leave(object sender, EventArgs e)
        {
            //xteparacodevalue.EditValue = null;
            //xseparacodeseqno.EditValue = null;
        }

        private void bbirefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xtcsys.SelectedTabPage == xtpinfoquery)
            {
                int ii = iqtype;
                iqtype = -1;
                initqueryform(ii);
            }

            if (xtcsys.SelectedTabPage == xtpfnc)
            {
                int ii = iqtypefnc;
                iqtypefnc = -1;
                initfncfeeform(ii);
            }


            if (xtcsys.SelectedTabPage == xtpppunitlv)
            {
                tlpp.DataSource = null;
                initppunitlvview();
            }

            if (xtcsys.SelectedTabPage == xtphomepage)
            {
                xccpptotal.Series[0].Tag = null;
                inithomepage();
            }

            if (xtcsys.SelectedTabPage == xtpppunitgrid)
            {
                initppunitgridview(1);
            }

            if (xtcsys.SelectedTabPage == xtpcusinfo)
            {
                xluecusarea.Properties.DataSource = null;
                initcusinfoform();
            }

            if (xtcsys.SelectedTabPage == xtpcontract)
            {
               
                initcontractinfoform(1);
            }

            if (xtcsys.SelectedTabPage == xtpsyscode)
            {
                if (xtcsyscode.SelectedTabPage == xtpppcode)
                {
                    xgcpparea.DataSource = null;
                    initareacodeform();
                }

                if (xtcsyscode.SelectedTabPage == xtpparacode)
                {
                    xgcparacodecatalog.DataSource = null;
                    initparacodeform();
                }
            }
        }

        private void initconfigform()
        {
            if (xluestartpage.Properties.DataSource == null)
            {
                xluestartpage.Properties.DataSource = mydb.getuserprivilegecat(uid);
            }

            xtesysname.Text = (mydb.getsyspara()).Rows[0]["sysname"].ToString();

            xluestartpage.EditValue = ConfigurationManager.AppSettings["startpage"].ToString();

            xcbsaveskin.Checked = ConfigurationManager.AppSettings["storeskin"].ToString() == "1" ? true : false;
            xcbribbonminimized.Checked = ConfigurationManager.AppSettings["storeribboncollapse"].ToString() == "1" ? true : false;

            xcbsavedatadisplaystyle.Checked = ConfigurationManager.AppSettings["storedatadisplaystyle"].ToString() == "1" ? true : false;


            xcbslidesysmsg.Checked = ConfigurationManager.AppSettings["showslidesysmsg"].ToString() == "1" ? true : false;
            xcbslideerrormsg.Checked = ConfigurationManager.AppSettings["showslideerrmsg"].ToString() == "1" ? true : false;

        }

        private void nbisysconfig_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            xtcsysparaconfig.SelectedTabPage = xtpsysconfig;
            initconfigform();

        }

        private void nbiuserconfig_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            xtcsysparaconfig.SelectedTabPage = xtpuserconfig;
            initconfigform();
        }

        private void xcbsaveskin_CheckedChanged(object sender, EventArgs e)
        {
            Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            oConfig.AppSettings.Settings["storeskin"].Value = xcbsaveskin.Checked ? "1" : "0";


            oConfig.Save(ConfigurationSaveMode.Modified);


            ConfigurationManager.RefreshSection("appSettings");
        }

        private void xcbslidesysmsg_CheckedChanged(object sender, EventArgs e)
        {
            Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            oConfig.AppSettings.Settings["showslidesysmsg"].Value = xcbslidesysmsg.Checked ? "1" : "0";


            oConfig.Save(ConfigurationSaveMode.Modified);


            ConfigurationManager.RefreshSection("appSettings");

        }

        private void xcbslideerrormsg_CheckedChanged(object sender, EventArgs e)
        {
            Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            oConfig.AppSettings.Settings["showslideerrmsg"].Value = xcbslideerrormsg.Checked ? "1" : "0";


            oConfig.Save(ConfigurationSaveMode.Modified);


            ConfigurationManager.RefreshSection("appSettings");

        }

        private void xcbsavedatadisplaystyle_CheckedChanged(object sender, EventArgs e)
        {
            Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            oConfig.AppSettings.Settings["storedatadisplaystyle"].Value = xcbsavedatadisplaystyle.Checked ? "1" : "0";


            oConfig.Save(ConfigurationSaveMode.Modified);


            ConfigurationManager.RefreshSection("appSettings");


        }

        private void xluesysusers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (xluesysusers.EditValue != null)
                {
                    xteuserpassword.Focus();
                }
            }
        }

        private void xluecontractarea_EditValueChanged(object sender, EventArgs e)
        {
            if (xluecontractpptype.EditValue != null &&
                xluecontractarea.EditValue != null)
            {



                xtecontractno.Text = mydb.getnewcontractno(xluecontractarea.EditValue.ToString(),
                                                           xluecontractpptype.EditValue.ToString()
                                                           );

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {

        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {

        }

        private void xsbct_ppunit_Click(object sender, EventArgs e)
        {

            DataRow dr = xgvcontract.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[关联房间] 没有选中合同记录!");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_m_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            frm_ct_ppunit fcp = new frm_ct_ppunit();
            fcp.msgshow = new frm_ct_ppunit.dsmsg(showopermsg);

            fcp.drcontract = dr;
            fcp.dtppunit = ((DataView)xgvct_ppunit.DataSource).Table;


            fcp.ShowDialog();

        }

        private void xgvcontract_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvcontract.GetFocusedDataRow();
            if (dr == null)
            {
                xgcct_ppunit.DataSource = null;
                xgcppfeemgt.DataSource = null;

                return;
            }

            string sctid = dr["contractid"].ToString();

            if (sctid != "")
            {
                xgcct_ppunit.DataSource = mydb.getppunitbycontract(sctid);
 //               xgvct_ppunit.BestFitColumns();
            }

            xgvct_ppunit_FocusedRowChanged(null, null);

            //button
            xsbsaveconpp.Enabled = false;
            xsbimpconppfromxls.Enabled = false;

        }

        private void xbectcus_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (xluecontractarea.EditValue != null)
            {
                frmSingleSel mysel = new frmSingleSel();

                mysel.Text = "选择客户";

                mysel.xgvsinglesel.Columns.Clear();
                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[0].Caption = "客户编号";
                mysel.xgvsinglesel.Columns[0].FieldName = "cusno";
                mysel.xgvsinglesel.Columns[0].Visible = true;

                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[1].Caption = "客户名称";
                mysel.xgvsinglesel.Columns[1].FieldName = "cusname";
                mysel.xgvsinglesel.Columns[1].Visible = true;

                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[2].Caption = "客户联系电话";
                mysel.xgvsinglesel.Columns[2].FieldName = "cusmobnum";
                mysel.xgvsinglesel.Columns[2].Visible = true;

                mysel.dtsrc = mydb.getallcusbyarea(xluecontractarea.EditValue.ToString());

                DialogResult dr = mysel.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    xbectcus.Text = mysel.drrt["cusname"].ToString();
                    xbectcus.Tag = mysel.drrt["cusid"].ToString() + "@" + mysel.drrt["cusno"].ToString();
                    //xbectcus.ErrorText = mysel.drrt["cusno"].ToString();
                }
            }
        }

        private void bbippmadd_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmppmadd myadd = new frmppmadd();

            myadd.msgshow = new frmppmadd.dsmsg(showopermsg);

            myadd.XScmWF = this.XScmWF;

            myadd.ShowDialog();


        }

        private void xluepptypex_EditValueChanged(object sender, EventArgs e)
        {
            //string sunitno = xteppnox.Text;
            string sunitno = (xgvppunitgrid.GetFocusedDataRow())["unitno"].ToString();

            string[] s1 = sunitno.Split(new char[] { '-' });

            string s2 = s1[3];

            switch (xluepptypex.Text)
            {
                case "商铺":
                    s2 = "S" + s2.Substring(1);
                    s2 = s1[0] + "-" + s1[1] + "-" + s1[2] + "-" + s2;
                    break;
                case "住宿":
                    s2 = "Z" + s2.Substring(1);
                    s2 = s1[0] + "-" + s1[1] + "-" + s1[2] + "-" + s2;
                    break;
                case "场地":
                    s2 = "C" + s2.Substring(1);
                    s2 = s1[0] + "-" + s1[1] + "-" + s1[2] + "-" + s2;
                    break;
                case "办公":
                    s2 = "G" + s2.Substring(1);
                    s2 = s1[0] + "-" + s1[1] + "-" + s1[2] + "-" + s2;
                    break;
                case "仓库":
                    s2 = "K" + s2.Substring(1);
                    s2 = s1[0] + "-" + s1[1] + "-" + s1[2] + "-" + s2;
                    break;
                default:
                    s2 = s1[3] ;
                    s2 = s1[0] + "-" + s1[1] + "-" + s1[2] + "-" + s2;
                    break;


            }

            xteppnox.Text = s2;

        }

        private void xlueppleveladd_EditValueChanged(object sender, EventArgs e)
        {
            if ((sender as DevExpress.XtraEditors.BaseEdit).EditValue != null)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).ForeColor = Color.Red;
            }
            else
            {
                (sender as DevExpress.XtraEditors.BaseEdit).ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
            }

            if (xlueppareaadd.EditValue != null &&
               xlueppbuildingadd.EditValue != null &&
               xlueppleveladd.EditValue != null &&
                xluepptypeadd.EditValue != null
             )
            {

                xteppnoadd.Text = xlueppareaadd.GetColumnValue("ppcode").ToString() + "-" +
                                  xlueppbuildingadd.GetColumnValue("ppcode").ToString() + "-" +
                                  xlueppleveladd.GetColumnValue("ppcode").ToString() + "-";

                switch (xluepptypeadd.Text)
                {
                    case "商铺":
                        xteppnoadd.Text += "S";
                        break;
                    case "住宿":
                        xteppnoadd.Text += "Z";
                        break;
                    case "场地":
                        xteppnoadd.Text += "C";
                        break;
                    case "办公":
                        xteppnoadd.Text += "G";
                        break;
                    case "仓库":
                        xteppnoadd.Text += "K";
                        break;
                    default:
                        xteppnoadd.Text += "";
                        break;


                }

            }
        }

        private void xluepptypeadd_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void xsbexptoxls_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvppunitgrid);
        }



        private void xsbexpcontracttoxls_Click_1(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvcontract);
        }

        private void xsbexpconpptoxls_Click(object sender, EventArgs e)
        {
            xgcxls.DataSource = xgcct_ppunit.DataSource;

            xgvxls.Columns[0].Width = 150;

            sxlsfile = mydb.exportgvtoxls(xgvxls);

            //sfile=System.IO.Path.GetTempPath()+System.DateTime.Now.ToString("HHmmssfff")+".xls"; 
            //xgvxls.ExportToXls(sfile);

            //System.Diagnostics.Process.Start(sfile);

            xsbimpconppfromxls.Enabled = true;
        }

        private void xsbimpconppfromxls_Click(object sender, EventArgs e)
        {
            xgcct_ppunit.DataSource = mydb.importxlstodatatable(sxlsfile);

            xsbsaveconpp.Enabled = true;

            showopermsg("Y", "[从xls导入合同房间数据]  成功导入!");
        }

        private void xsbsaveconpp_Click(object sender, EventArgs e)
        {


            DialogResult dr1 = MessageBox.Show("确定要保存合同所有房间吗？ ", "批量操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel)
            {
                return;
            }


            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在保存合同房间数据...");

            DataTable dt = ((DataView)xgvct_ppunit.DataSource).Table;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["cpid"].ToString() != "")
                {
                    mydb.updateconpp(dr);
                }
                else
                {
                    mydb.addcon_pp(dr);
                }
            }

            XScmWF.CloseWaitForm();

            xgvcontract_FocusedRowChanged(null, null);


            showopermsg("Y", "[保存合同房间数据]  成功保存!");
        }

        private void bbiinformrent_ItemClick(object sender, ItemClickEventArgs e)
        {
            //xtcsys.SelectedTabPage = xtpschedule;

            //DataTable dt= mydb.getrentinfoapt();

            //foreach (DataRow dr in dt.Rows)
            //{

            //    Appointment apt = xschec.Storage.CreateAppointment(AppointmentType.Normal);

            //    apt.Subject = dr["subject"].ToString();
            //    apt.AllDay = true;
            //    apt.LabelId =Convert.ToInt32(dr["label"].ToString());
            //    apt.Start =Convert.ToDateTime(dr["startdate"].ToString());
            //    apt.Description = dr["des"].ToString();


            //    xschec.Storage.Appointments.Add(apt);
            //}
        }

        public void createinform()
        {
            xschec.Start = System.DateTime.Now.Date;
            xschec.Storage.Appointments.Clear();
            xschec.Storage.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("contractid", "cid"));

            wheda.db.dboper myifdb = new wheda.db.dboper();

            int idays = Convert.ToInt32(mydb.getuserpara(uid).Rows[0]["ifdays"].ToString());
            DataTable dt = myifdb.getcontractinform(idays);

            foreach (DataRow dr in dt.Rows)
            {

                Appointment apt = xschec.Storage.CreateAppointment(AppointmentType.Normal);

                apt.Start = System.DateTime.Now.Date;

                System.DateTime datet = DateTime.ParseExact(dr["contractedt"].ToString(),
                                            "yyyyMMdd",
                                            new CultureInfo("zh-CN", true)
                                            );
                TimeSpan ts = datet - System.DateTime.Now.Date;

                Int32 ids = ts.Days;

                apt.Subject = "合同: [" +
                              dr["contractno"].ToString() + "]-[" +
                              dr["cusname"].ToString() + "]  " +
                              "将于[" + ids.ToString() + "]天后到期，到期日[" +
                              datet.ToString("yyyy-MM-dd") + "]";

                apt.AllDay = true;

                if(ids<3)
                {
                    apt.LabelId = 1;
                }
                else
                {
                    apt.LabelId = 3;//Convert.ToInt32(dr["label"].ToString());
                }

                apt.StatusId = 3;




                apt.Description = "";

                apt.CustomFields["contractid"] = dr["contractid"].ToString();

                apt.HasReminder = true;
                apt.Reminder.AlertTime = System.DateTime.Now.AddSeconds(10);


                xschec.Storage.Appointments.Add(apt);
            }

            dt = myifdb.getpayfeeinform(idays);
            foreach (DataRow dr in dt.Rows)
            {

                Appointment apt = xschec.Storage.CreateAppointment(AppointmentType.Normal);

                apt.Start = System.DateTime.Now.Date;

                System.DateTime datet = DateTime.ParseExact(dr["feepaysdt"].ToString(),
                                            "yyyyMMdd",
                                            new CultureInfo("zh-CN", true)
                                            );
                TimeSpan ts = datet - System.DateTime.Now.Date;

                Int32 ids = ts.Days;

                apt.Subject = "合同: [" +
                              dr["contractno"].ToString() + "]-[" +
                              dr["cusname"].ToString() + "]  " +
                              "将于[" + ids.ToString() + "]天后收租，收租日[" +
                              datet.ToString("yyyy-MM-dd") + "]";

                apt.AllDay = true;

                if (ids < 3)
                {
                    apt.LabelId = 1;
                }
                else
                {
                    apt.LabelId = 3;//Convert.ToInt32(dr["label"].ToString());
                }


                apt.StatusId = 3;




                apt.Description = "";

                apt.CustomFields["contractid"] = dr["contractid"].ToString();

                apt.HasReminder = true;
                apt.Reminder.AlertTime = System.DateTime.Now.AddSeconds(10);


                xschec.Storage.Appointments.Add(apt);
            }


 

            myifdb.finalclose();

           
        }

        private void bbiinformcontract_ItemClick(object sender, ItemClickEventArgs e)
        {
            xtcsys.SelectedTabPage = xtpschedule;

            xschec.ActiveViewType = SchedulerViewType.Timeline;

            createinform();

        }

 

        private void sches_AppointmentsDeleted(object sender, PersistentObjectsEventArgs e)
        {
        }

        private void sches_AppointmentDeleting(object sender, PersistentObjectCancelEventArgs e)
        {
            Appointment apt = e.Object as Appointment;
            string scid = apt.CustomFields["contractid"].ToString();

            string ssub = apt.Subject;

            mydb.addignoreinform(scid, ssub);
            //MessageBox.Show(  (e.Object as Appointment).CustomFields["contractid"].ToString());

        }

        private void xgvcus_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            //if (e.Column.FieldName == "gvsel" && e.IsGetData)
            //    e.Value = 0;

        }

        private void xluecusareaadd_EditValueChanged(object sender, EventArgs e)
        {
            if (xluecusareaadd.EditValue != null)
            {
                xtecusnoadd.Text = mydb.getnewcusno(xluecusareaadd.EditValue.ToString());
            }
            else
            {
                xtecusnoadd.Text = "";
            }
        }

        private void initserveraddr()
        {
            innerip.Text = ConfigurationManager.AppSettings["intranetserverip"].ToString();
            outerip.Text = ConfigurationManager.AppSettings["internetserverip"].ToString();

            string iptype = ConfigurationManager.AppSettings["iptype"].ToString();

            if (iptype == "0") xrgip.SelectedIndex = 0;
            else xrgip.SelectedIndex = 1;

        }

        private void xsbserverip_Click(object sender, EventArgs e)
        {

            initserveraddr();
            

            xlcip.Visible = true;
        }

        private void bbippunitlvview_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ribbonControl.SelectedPage != xrppp)
            {
                ribbonControl.SelectedPage = xrppp;
            }


            initppunitlvview();
        }

        private void bbisendcttocheck_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcsys.SelectedTabPage == xtpcontract)
            {
                if (xgvcontract.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[提交合同审核] 请一次只选一个合同提交审核！");
                    return;
                }

                dr = xgvcontract.GetFocusedDataRow();
                if (dr == null)
                {
                    showopermsg("X", "[提交合同审核] 请先选择一个合同！");
                    return;
                }
            }

            if (xtcsys.SelectedTabPage == xtpinfoquery &&
                xtcinfoquery.SelectedTabPage == xtpcontractinfo)
            {
                if (xgvinfoqueryct.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[提交合同审核] 请一次只选一个合同提交审核！");
                    return;
                }

                dr = xgvinfoqueryct.GetFocusedDataRow();
                if (dr == null)
                {
                    showopermsg("X", "[提交合同审核] 请先选择一个合同！");
                    return;
                }
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_check_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            //检查是否已经有费用数据
            if (!mydb.checkcthasfeepay(dr))
            {
                showopermsg("X", "[提交合同审核] 该合同还没有生成应收数据，请先生成！");
                return;
  
            }

            DialogResult dr1 = MessageBox.Show("确定要提交合同进行审核吗？", "确认提交", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            if (dr["contractstatus"].ToString() == "初登")
            {
                mydb.changecontractstatus(dr, 1);

                dr["contractstatus"] = "等待审核";
            }
            else if (dr["contractstatus"].ToString() == "修改")
            {
                mydb.changecontractstatus(dr, 6);

                dr["contractstatus"] = "等待修改审核";
            }

        }

        private void bbicancelcheckct_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcfeefnc.SelectedTabPage == xtpcttobechecked)
            {

                if (xgvcttobechecked.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[合同审核通过] 请一次只选一个合同！");
                    return;
                }

                dr = xgvcttobechecked.GetFocusedDataRow();
            }
            else
            {
                xtcfeefnc.SelectedTabPage = xtpcttobechecked;
                return;
            }

  

            if (dr == null)
            {
                showopermsg("X", "[取消合同审核] 请先选择一个合同！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_uncheck_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要取消审核该合同吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("输入取消审核原因",
                                                                                 dr["cusname"].ToString()+"["+
                                                                                 dr["contractnofnc"].ToString()+"  "+
                                                                                 dr["contractno"].ToString()+"]",
                                                                                 "", -1, -1);

            if (sss1 == "") return;

            sss1 = "[取消审核]\r\n" + sss1;

            if (dr["contractstatus"].ToString() == "等待审核")
            {
                mydb.changecontractstatus(dr, 0,sss1);

                dr["contractstatus"] = "初登";
            }
            else if (dr["contractstatus"].ToString() == "等待修改审核")
            {
                mydb.changecontractstatus(dr, 8,sss1);

                dr["contractstatus"] = "修改";
            }

            showopermsg("Y", "<取消审核合同> 取消成功！");


            //发送QQ消息
        }

        private void xluesysusers_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            //if (xluesysusers.Properties.DataSource == null)
            {
                if (xlcip.Visible)
                {
                    try { mydb.finalclose(); }
                    finally { };

                    xluesysusers.Properties.DataSource = null;

                    saveip();
                    xluesysusers.Properties.DataSource = mydb.getloginusers();

                    xluesysusers.ShowPopup();
                }
            }
        }

        private void sches_ReminderAlert(object sender, ReminderEventArgs e)
        {
            // MessageBox.Show(e.AlertNotifications[0].ActualAppointment.Subject);
        }





        private void xsbimpconppfromxlshdfile_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "xls 文件|*.xls;*.xlsx";
            DialogResult dr = op.ShowDialog();
            if (dr != DialogResult.OK) return;

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("从xls导入批量合同房间数据...");

            xgcct_ppunit.DataSource = null;
            xgcct_ppunit.DataSource = mydb.importxlstodatatable(op.FileName);


            XScmWF.CloseWaitForm();
            showopermsg("Y", "[从xls导入批量合同房间数据] 成功导入！");

            xsbsaveconpp.Enabled = true;
        }

        private void bbicusquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            initcusinfoform();
        }

        private void bbiuserquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            inituserinfoform();
        }

        private void xgvuser_Click(object sender, EventArgs e)
        {
            if (xsccuserchange.SplitterPosition == 0) return;

            Point point = xgcuser.PointToClient(Cursor.Position);

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit = xgvuser.CalcHitInfo(point);

            if (!hit.InRowCell && !hit.InRow) return;

            DataRow dr = xgvuser.GetFocusedDataRow();

            showusermodifypanel(dr);

        }


        private void bbiusermodify_ItemClick(object sender, ItemClickEventArgs e)
        {
            xtcsys.SelectedTabPage = xtpuser;

            DataRow dr = xgvuser.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[修改用户] 没有选中记录!");
                return;
            }


            showusermodifypanel(dr);

        }

        private void bbiuseradd_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xtcsys.SelectedTabPage == xtpprivilege)
            {
                DataRow dr1 = xgvusergroupprivilege.GetFocusedDataRow();

                if (dr1 == null) return;

                frmSingleSel mysel = new frmSingleSel();

                mysel.Text = "选择用户";

                mysel.xgvsinglesel.Columns.Clear();
                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[0].Caption = "用户名称";
                mysel.xgvsinglesel.Columns[0].FieldName = "username";
                mysel.xgvsinglesel.Columns[0].Visible = true;

                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[1].Caption = "用户状态";
                mysel.xgvsinglesel.Columns[1].FieldName = "userstatus";
                mysel.xgvsinglesel.Columns[1].Visible = true;

                mysel.xgvsinglesel.Columns.Add();
                mysel.xgvsinglesel.Columns[2].Caption = "用户描述";
                mysel.xgvsinglesel.Columns[2].FieldName = "userdesc";
                mysel.xgvsinglesel.Columns[2].Visible = true;

                mysel.dtsrc = mydb.getgroupadduser(dr1["id"].ToString());

                DialogResult dr = mysel.ShowDialog();

                if (dr == DialogResult.OK)
                {
                    DataTable dt = ((DataView)xgvuserprivilege.DataSource).Table;
                    dt.ImportRow(mysel.drrt);

                    mydb.addgroupuser(dr1["id"].ToString(), mysel.drrt);
                }

            }
            else
            {
                xtcsys.SelectedTabPage = xtpuser;

                showuseraddpanel(null);
            }
        }

        private void xsbusermodicancel_Click(object sender, EventArgs e)
        {
            xsccuserchange.SplitterPosition = 0;
        }

        private void xsbusermodiapply_Click(object sender, EventArgs e)
        {


            DataRow dr = xgvuser.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[修改用户] 出现一般性错误，请重试，若错误再次出现请联系联创科技！");
                return;
            }


            dr["username"] = xteusernamemodi.Text;

            dr["userpassword"] = xteuserpassmodi.Text;
            dr["userdesc"] = xteuserdescmodi.Text;


            mydb.updateuser(dr);

            showopermsg("Y", "<修改用户> 成功修改用户资料！");

            //更新dr
        }

        private void xgvuser_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvuser.GetFocusedDataRow();
            if (dr == null) return;

            string suserid = dr["userid"].ToString();

            if (suserid != "")
            {
                xgcusergroup.DataSource = mydb.getgroupbyuserid(suserid);
            }



        }



        private void xsbusermodiok_Click(object sender, EventArgs e)
        {
            xsbusermodiapply_Click(null, null);
            xsccuserchange.SplitterPosition = 0;
        }

        private void xsbuseraddok_Click(object sender, EventArgs e)
        {

            if (xteusernameadd.Text == "" ||
                xteuserpassadd.Text == "")
            {
                showopermsg("X", "[添加用户]  用户名称|用户密码为必填项！");

                return;

            }


            DataTable dt;

            if (xgcuser.DataSource != null)
            {
                dt = ((DataView)xgvuser.DataSource).Table;
            }
            else
            {
                string squery = wheda.db.dboper.suserquery + " and 1=2  ";


                xgcuser.DataSource = mydb.gettablebystr(squery);

                dt = ((DataView)xgvuser.DataSource).Table;
            }

            DataRow dr = dt.NewRow();

            dr["username"] = xteusernameadd.Text;
            dr["userpassword"] = xteuserpassadd.Text;
            dr["userdesc"] = xteuserdescadd.EditValue;
            dr["userstatus"] = "禁用";


            Int32 inewid = mydb.adduser(dr);
            dr["userid"] = inewid;


            dt.Rows.Add(dr);    //dt.ImportRow


            DevExpress.XtraGrid.Views.Base.ColumnView cv = (DevExpress.XtraGrid.Views.Base.ColumnView)xgcuser.MainView;

            int rhFound = cv.LocateByValue("userid", inewid);

            // focusing the cell
            if (rhFound != DevExpress.XtraGrid.GridControl.InvalidRowHandle)
            {
                cv.FocusedRowHandle = rhFound;

                cv.ClearSelection();

                cv.SelectRow(rhFound);

            }



            showopermsg("Y", "<添加用户> 成功添加用户资料!");
            xsccuserchange.SplitterPosition = 0;
        }

        private void xsbuseraddcancel_Click(object sender, EventArgs e)
        {
            xsccuserchange.SplitterPosition = 0;
        }


        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null)
            {
                bool b = false;
                CheckState state;
                for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                {
                    state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                    if (!check.Equals(state))
                    {
                        b = !b;
                        break;
                    }
                }
                node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        private void initsysprivilegeform()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取权限基础数据......");


            xgcusergroupprivilege.DataSource = mydb.getparacode("usergroup");




            this.xtcsys.SelectedTabPage = xtpprivilege;

            XScmWF.CloseWaitForm();
        }





        private void bbisysprivilege_ItemClick(object sender, ItemClickEventArgs e)
        {
            initsysprivilegeform();
        }

        private void tlprivilegecode_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        private void tlprivilegecode_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        private void xgvusergroupprivilege_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvusergroupprivilege.GetFocusedDataRow();
            if (dr == null) return;


            {
                this.tlprivilegecode.Nodes.Clear();

                this.tlprivilegecode.DataSource = mydb.getsysprivilegecode(dr["id"].ToString());
                this.tlprivilegecode.Columns["operdesc"].Caption = "未分配权限";
                this.tlprivilegecode.Columns["opername"].Visible = false;

                this.tlgroupprivilege.Nodes.Clear();

                this.tlgroupprivilege.DataSource = mydb.getgroupprivilege(dr["id"].ToString());
                this.tlgroupprivilege.Columns["operdesc"].Caption = "已分配权限";
                this.tlgroupprivilege.Columns["opername"].Visible = false;

            }

            tlprivilegecode.ExpandAll();
            tlgroupprivilege.ExpandAll();

            string sgroupid = dr["id"].ToString();

            if (sgroupid != "")
            {
                xgcuserprivilege.DataSource = mydb.getuserbygroupid(sgroupid);
            }

        }

        private void AddGroupPrivilege(TreeListNode tr)
        {
            tr.Selected = false;

            if (tr.HasChildren)
            {
                AddGroupPrivilege(tr.FirstNode);
            }

            if (tr.Level == 1)
            {
                //add to right
                if (tr.Checked)
                {

                    DataRow dr = ((DataRowView)tlprivilegecode.GetDataRecordByNode(tr)).Row;

                    DataTable dt = (DataTable)tlgroupprivilege.DataSource; //

                    dt.ImportRow(dr);

                    DataRow dr1 = xgvusergroupprivilege.GetFocusedDataRow();

                    mydb.addgroupprivilege(dr1["id"].ToString(), dr["id"].ToString());

                    tr.Selected = true;
                }

            }

            if (tr.NextNode != null)
            {
                AddGroupPrivilege(tr.NextNode);
            }


        }

        private void RemoveGroupPrivilege(TreeListNode tr)
        {
            tr.Selected = false;

            if (tr.HasChildren)
            {
                RemoveGroupPrivilege(tr.FirstNode);
            }

            if (tr.Level == 1)
            {
                //add to right
                if (tr.Checked)
                {

                    DataRow dr = ((DataRowView)tlgroupprivilege.GetDataRecordByNode(tr)).Row;

                    DataTable dt = (DataTable)tlprivilegecode.DataSource; //

                    dt.ImportRow(dr);

                    DataRow dr1 = xgvusergroupprivilege.GetFocusedDataRow();

                    mydb.removegroupprivilege(dr1["id"].ToString(), dr["id"].ToString());

                    tr.Selected = true;
                }

            }

            if (tr.NextNode != null)
            {
                RemoveGroupPrivilege(tr.NextNode);
            }


        }

        private void xsbaddprivilege_Click(object sender, EventArgs e)
        {

            TreeListNode tr = tlprivilegecode.Nodes.FirstNode;

            AddGroupPrivilege(tr);


            tlprivilegecode.DeleteSelectedNodes();

            showopermsg("Y", "[添加权限] 权限组成功添加权限!");
        }

        private void xsbremoveprivilege_Click(object sender, EventArgs e)
        {
            TreeListNode tr = tlgroupprivilege.Nodes.FirstNode;

            RemoveGroupPrivilege(tr);


            tlgroupprivilege.DeleteSelectedNodes();

            showopermsg("Y", "[移除权限] 权限组成功移除权限!");

        }

        private void tlgroupprivilege_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        private void tlgroupprivilege_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        private void bbiuserdelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xtcsys.SelectedTabPage == xtpuser)
            {

                if (xgvuser.SelectedRowsCount < 1)
                {
                    showopermsg("X", "[删除用户] 您没有选中用户记录！");
                    return;
                }

                DialogResult dr1 = MessageBox.Show("确定要删除选中的记录吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr1 == DialogResult.Cancel) return;


                foreach (Int32 i3 in xgvuser.GetSelectedRows())
                {
                    DataRow dr = xgvuser.GetDataRow(i3);


                    mydb.deleteuser(dr["userid"].ToString());
                }


                xgvuser.DeleteSelectedRows();

                showopermsg("Y", "<删除用户> 删除用户资料成功！");
            }

            if (xtcsys.SelectedTabPage == xtpprivilege)
            {
                if (xgvuserprivilege.SelectedRowsCount < 1)
                {
                    showopermsg("X", "[删除组用户] 您没有选中组用户记录！");
                    return;
                }

                DialogResult dr1 = MessageBox.Show("确定要删除选中组用户吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr1 == DialogResult.Cancel) return;

                DataRow d111 = xgvusergroupprivilege.GetFocusedDataRow();
                if (d111 == null) return;

                foreach (Int32 i3 in xgvuserprivilege.GetSelectedRows())
                {
                    DataRow dr = xgvuserprivilege.GetDataRow(i3);


                    mydb.deletegroupuser(d111,dr["userid"].ToString());
                }

                xgvuserprivilege.DeleteSelectedRows();

                showopermsg("Y", "<删除组用户> 删除成功！");
            }


        }

        private void bbiuserendis_ItemClick(object sender, ItemClickEventArgs e)
        {
            xtcsys.SelectedTabPage = xtpuser;

            if (xgvuser.SelectedRowsCount <= 0)
            {
                showopermsg("X", "[启用/禁止用户] 没有选择用户！");
                return;
            }

            if (xgvuser.SelectedRowsCount > 1)
            {
                showopermsg("X", "[启用/禁止用户] 请只选一个用户！");
                return;
            }

            DataRow dr = xgvuser.GetFocusedDataRow();


            if (dr["userstatus"].ToString() == "启用")
                dr["userstatus"] = "禁用";
            else dr["userstatus"] = "启用";

            mydb.updateuser(dr);
        }

        private void bbiignoreinform_ItemClick(object sender, ItemClickEventArgs e)
        {
           
            if (xschec.SelectedAppointments.Count < 1)
            {
                showopermsg("X", "[忽略提醒] 您没有选中任何需要忽略的提醒!");
                return;
            }

            xschec.DeleteSelectedAppointments();
        }

        private void bbicontracttobeapproved_ItemClick(object sender, ItemClickEventArgs e)
        {
            initfncfeeform(3);

            //ribbonControl.SelectedPage = xrpcontract;
            //initcontractinfoform();

            //xluecontractstatus.ItemIndex=
            //       xluecontractstatus.Properties.GetDataSourceRowIndex("paravalue", "等待审核");

            //xsbcontractquery_Click(null, null);

        }

        private void bbicontractapproved_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcfeefnc.SelectedTabPage == xtpcttobechecked)
            {

                if (xgvcttobechecked.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[合同审核通过] 请一次只选一个合同！");
                    return;
                }

                dr = xgvcttobechecked.GetFocusedDataRow();
            }
            else
            {
                xtcfeefnc.SelectedTabPage = xtpcttobechecked;
                return;
            }

  
            if (dr == null)
            {
                showopermsg("X", "[合同审核通过] 请先选择一个合同！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_approve_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要审核通过合同吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;



            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在生成房间状态，请稍等...");

            if (dr["contractstatus"].ToString() == "等待审核")
            {

                mydb.changecontractstatus(dr, 2);

                dr["contractstatus"] = "已审核";
            }
            else if (dr["contractstatus"].ToString() == "等待修改审核")
            {

                mydb.changecontractstatus(dr, 7);

                dr["contractstatus"] = "已审核";
            }

            XScmWF.CloseWaitForm();

            showopermsg("Y", "<合同审核通过> 操作成功！");
        }

        private void xdecontractsdt_EditValueChanged(object sender, EventArgs e)
        {
            if (xdecontractsdt.EditValue != null)
            {
                xdecontractedt.EditValue = ((DateTime)xdecontractsdt.EditValue).AddYears(1).AddDays(-1);
            }
        }

        private void xluestartpage_EditValueChanged(object sender, EventArgs e)
        {
            Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            oConfig.AppSettings.Settings["startpage"].Value = xluestartpage.EditValue.ToString();


            oConfig.Save(ConfigurationSaveMode.Modified);


            ConfigurationManager.RefreshSection("appSettings");
        }

        private void bbiupgrade_ItemClick(object sender, ItemClickEventArgs e)
        {
            CheckNewestVer();
        }

        private void bbichangepassword_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmChangePass cp = new frmChangePass();
            cp.ShowDialog();
        }

        private void bbialtquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmAltQuery faq = new frmAltQuery();


            DataRow dr = null;
            if (xtcsys.SelectedTabPage == xtpppunitgrid)
            {
                dr = xgvppunitgrid.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["ppid"].ToString();
                faq.stype = "2";
            }
            else if (xtcsys.SelectedTabPage == xtpinfoquery &&
                     xtcinfoquery.SelectedTabPage == xtpppinfo)
            {
                dr =xgvinfoquerypp.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["ppid"].ToString();
                faq.stype = "2";
            }
            else if (xtcsys.SelectedTabPage == xtpppunitlv)
            {
                dr = xlvppunit.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["ppid"].ToString();
                faq.stype = "2";
            }
            else if (xtcsys.SelectedTabPage == xtpcontract)
            {
                dr = xgvcontract.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["contractid"].ToString();
                faq.stype = "1";
            }
            else if(xtcsys.SelectedTabPage == xtpinfoquery&&xtcinfoquery.SelectedTabPage==xtpcontractinfo)
            {
                dr = xgvinfoqueryct.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["contractid"].ToString();
                faq.stype = "1";
            }
            else if (xtcsys.SelectedTabPage == xtpfnc && xtcfeefnc.SelectedTabPage == xtpcttobechecked)
            {
                dr = xgvcttobechecked.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["contractid"].ToString();
                faq.stype = "1";
            }
            else if (xtcsys.SelectedTabPage == xtpcusinfo)
            {
                dr = xgvcus.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["cusid"].ToString();
                faq.stype = "3";
            }
            else if (xtcsys.SelectedTabPage == xtpuser)
            {
                dr = xgvuser.GetFocusedDataRow();
                if (dr == null) return;

                faq.sid = dr["userid"].ToString();
                faq.stype = "4";
            }
            else return;

            faq.Show();

        }



        private void xgvcus_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvcus.GetFocusedDataRow();
            if (dr == null)
            {
                xgccuscontract.DataSource = null;
                return;
            }

            string sctid = dr["cusid"].ToString();

            if (sctid != "")
            {
                xgccuscontract.DataSource = mydb.getcontractbycus(sctid);
            }



        }

        private void bbipayfeemgt_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void xgvct_ppunit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            if (dr == null)
            {
                xgcppfeemgt.DataSource = null;
                return;
            }

           // xgcppfeemgt.DataSource = mydb.getppfeepayfnc(dr);
            xgcppfeemgt.DataSource = mydb.getppfeepaymgt(dr);
        }

        private void xgvppfeemgt_CustomDrawGroupPanel(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {

            //GridView gv = sender as GridView;
            //GridElementsPainter elementsPainter = (gv.GetViewInfo() as GridViewInfo).Painter.ElementsPainter;
            //StyleObjectInfoArgs groupArgs = new StyleObjectInfoArgs(e.Cache, e.Bounds, e.Appearance, ObjectState.Normal);
            //elementsPainter.GroupPanel.DrawObject(groupArgs);

            //Brush brush = e.Cache.GetGradientBrush(e.Bounds, Color.Blue, Color.Blue, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);

            //Point p = new Point(e.Bounds.X +  125, e.Bounds.Y + (e.Bounds.Height - 20) / 2);

            //string srecs = "RIRIRIIR";
            //e.Graphics.DrawString(srecs, e.Appearance.Font, brush, p);

            //e.Handled = true;

            //DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            //if (dr == null) return;



        }

        private void adjustpayfeemgtfnc(int isrc)
        {
            DataRow dr=null;

            if (isrc == 0) dr = xgvcontract.GetFocusedDataRow();
            if (isrc == 1) dr = xgvpayfeect.GetFocusedDataRow();

            if (dr == null) { showopermsg("X", "[调整合同费用] 请选择一个合同!"); return; }


            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_m_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            bool bb = mydb.cthaspayfee(dr);

            if (dr["contractstatus"].ToString() == "初登" ||
                !bb)
            {

                frmContractPayfeeMgt fcpm = new frmContractPayfeeMgt();
                fcpm.XScmWF = this.XScmWF;

                if (isrc == 0) fcpm.xgcct_ppunit.DataSource = xgcct_ppunit.DataSource;
                if (isrc == 1) fcpm.xgcct_ppunit.DataSource = mydb.getppunitbycontract(dr["contractid"].ToString());


                fcpm.drcontract = dr;

                fcpm.ShowDialog();

                xgvcontract_FocusedRowChanged(null, null);

                //xgvct_ppunit_FocusedRowChanged(null, null);
            }
            else if (uid == "0")
            {
                frmContractPayfeeMgt fcpm = new frmContractPayfeeMgt();
                fcpm.XScmWF = this.XScmWF;

                if (isrc == 0) fcpm.xgcct_ppunit.DataSource = xgcct_ppunit.DataSource;
                if (isrc == 1) fcpm.xgcct_ppunit.DataSource = mydb.getppunitbycontract(dr["contractid"].ToString());


                fcpm.drcontract = dr;

                fcpm.ShowDialog();

                xgvcontract_FocusedRowChanged(null, null);
                //xgvct_ppunit_FocusedRowChanged(null, null);
            }
            else if (dr["contractstatus"].ToString() == "修改")
            {
                frmpayfeeperiodadjust fpf = new frmpayfeeperiodadjust();
                fpf.XScmWF = this.XScmWF;
                fpf.drcontract = dr;

                if (isrc == 0) fpf.xgcct_ppunit.DataSource = xgcct_ppunit.DataSource;
                if (isrc == 1) fpf.xgcct_ppunit.DataSource = mydb.getppunitbycontract(dr["contractid"].ToString());
                

                fpf.ShowDialog();

                if (fpf.irt == 1)
                {
                    frmContractPayfeeMgt fcpm = new frmContractPayfeeMgt();
                    fcpm.XScmWF = this.XScmWF;

                    if (isrc == 0) fcpm.xgcct_ppunit.DataSource = xgcct_ppunit.DataSource;
                    if (isrc == 1) fcpm.xgcct_ppunit.DataSource = mydb.getppunitbycontract(dr["contractid"].ToString());


                    fcpm.drcontract = dr;

                    fcpm.ShowDialog();

                    xgvcontract_FocusedRowChanged(null, null);


                    return;
                }

                xgvcontract_FocusedRowChanged(null, null);
                //xgvct_ppunit_FocusedRowChanged(null, null);
            }


        }

        private void xsbfeeadjust_Click(object sender, EventArgs e)
        {
            int ii = xgvct_ppunit.RowCount;
            if (ii <= 0 || xgcct_ppunit.DataSource == null)
            {
                showopermsg("X", "[生成房间应收] 合同没有房间！");
                return;
            }


            adjustpayfeemgtfnc(0);

        }

        private void xcbribbonminimized_CheckedChanged(object sender, EventArgs e)
        {
            Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


            oConfig.AppSettings.Settings["storeribboncollapse"].Value = xcbribbonminimized.Checked ? "1" : "0";


            oConfig.Save(ConfigurationSaveMode.Modified);


            ConfigurationManager.RefreshSection("appSettings");
        }


        private void bbifeepayquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            //initpayfeequeryform();

            initfncfeeform(0);
        }

        private void bbicontractquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            ribbonControl.SelectedPage = xrpcontract;
            initcontractinfoform();
        }

        private void xgvct_feepay_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_feepay.GetFocusedDataRow();
            if (dr == null)
            {
                xgcpp_feepay.DataSource = null;
                return;
            }

            xgcpp_feepay.DataSource = mydb.getppfeepayfncbycontract(dr);
        }

        private void xbecontractno_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            initcontractinfoform();
        }

        private void bbifeepayedmgt_ItemClick(object sender, ItemClickEventArgs e)
        {
            //initfeepayedform();

            initfncfeeform(1);
        }

        private void initfeepayedform()
        {

            //if (xtcsys.SelectedTabPage == xtpcontract ||
            //    xtcsys.SelectedTabPage == xtppayfeemgt ||
            //    xtcsys.SelectedTabPage == xtppayedfeeseqmgt
            //)
            //{
            //    DataRow dr = xgvcontract.GetFocusedDataRow();
            //    if (dr != null)
            //    {
            //        xbecontractno.Text = dr["contractno"].ToString();
            //        xbecontractno.Tag = dr["contractid"];

            //        if (xluefeepaytillmon.Properties.DataSource == null)
            //        {
            //            xluefeepaytillmon.Properties.DataSource = mydb.getcontractfeepaymonth(dr);

            //            int i1 = 0;
            //            switch (dr["rentpaystyle"].ToString())
            //            {
            //                case "按月":
            //                    break;
            //                case "按季":
            //                    i1 = 3 - 1;
            //                    break;
            //                case "按年":
            //                    i1 = 12 - 1;
            //                    break;
            //                default:
            //                    break;

            //            }
            //            xluefeepaytillmon.EditValue = System.DateTime.Now.Date.AddMonths(i1).ToString("yyyyMM");
            //        }

            //        xgcct_feepayed.DataSource = mydb.getcontractfeepayed(dr, xluefeepaytillmon.EditValue);
            //    }
            //}
            //else return;

            //xlc001.Visible = true;
            //xluefeepaytillmon.Visible = true;

            //xsccfeepayed.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;

            //xtcsys.SelectedTabPage = xtppayfeemgt;
        }

        private void initpayfeequeryform()
        {
            //DataRow dr = xgvcontract.GetFocusedDataRow();
            //if (dr != null)
            //{
            //    xbecontractno.Text = dr["contractno"].ToString();
            //    xbecontractno.Tag = dr["contractid"];

            //    xgcct_feepay.DataSource = mydb.getcontractfeepay(dr);
            //}

            //xlc001.Visible = false;
            //xluefeepaytillmon.Visible = false;

            //xsccfeepayed.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;

            //xtcsys.SelectedTabPage = xtppayfeemgt;
        }

        private void xluefeepaytillmon_EditValueChanged(object sender, EventArgs e)
        {
            initfeepayedform();

           
        }

        private void xgvct_feepayed_DataSourceChanged(object sender, EventArgs e)
        {

            if (xgvct_feepayed.RowCount <= 0)
            {
                xgcpp_feepayed.DataSource = null;

                xtecontractpayfee.Text = "0";

                return;
            }

            double ff = 0;
            double ff1 = 0;
            for (int ii = 0; ii < xgvct_feepayed.RowCount; ii++)
            {
                DataRow dr = xgvct_feepayed.GetDataRow(ii);
               
                ff += Convert.ToDouble(dr["fee"].ToString())- Convert.ToDouble(dr["feepayed"].ToString());

                ff1 += Convert.ToDouble(dr["fee"].ToString());

            }

            ff1 = (double)System.Math.Round(ff1, 2);
            ff = (double)System.Math.Round(ff, 2);

            xtecontractpayfee.Text = ff.ToString();
            xtecontractpayfee.Tag = ff1;

            xgcpp_feepayed.DataSource = mydb.getppfeepayedbycontract(xgvct_feepayed);
        }

        private void xsbautosplit_Click(object sender, EventArgs e)
        {
            if (Convert.ToSingle(xtecontractfeepayed.Text) >
                Convert.ToSingle(xtecontractpayfee.Text)
            )
            {
                MessageBox.Show("实收大于应收，请多选择些未缴月份！", "提示");
                return;
            }

            double fpayed = Convert.ToSingle(xtecontractfeepayed.Text);

            DataTable dt1 = ((DataView)xgvpp_feepayed.DataSource).Table;

            foreach (DataRow dr in dt1.Rows) { dr["feepayednow"] = 0.0; };

            foreach (DataRow dr in dt1.Rows)
            {
                double f1 = Convert.ToDouble(dr["fee"].ToString()) - Convert.ToDouble(dr["feepayed"].ToString());

                if (fpayed > f1)
                {
                    dr["feepayednow"] = System.Math.Round(f1, 2);
                }
                else
                {
                    dr["feepayednow"] = System.Math.Round(fpayed, 2);
                    break;
                }

                fpayed -= f1;
            }

            xsbcontractfeepayedsave.Enabled = true;

        }

        private void xsbcontractfeepayedtoxls_Click(object sender, EventArgs e)
        {
            xgcxls.DataSource = xgcpp_feepayed.DataSource;

            xgvxls.Columns[0].Width = 50;

            sxlsfile = mydb.exportgvtoxls(xgvxls);

            xsbcontractfeepayedfromxls.Enabled = true;

        }

        private void importfncfeepayed(string sfilen)
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("从xls导入批量合同房间数据...");

            xgcpp_feepayed.DataSource = null;
            xgcpp_feepayed.DataSource = mydb.importxlstodatatable(sxlsfile);

            XScmWF.CloseWaitForm();
            showopermsg("Y", "[从xls导入批量合同房间数据] 成功导入！");

            DataTable dt1 = ((DataView)xgvpp_feepayed.DataSource).Table;

            double f1 = 0;
            double f2 = 0;
            foreach (DataRow dr in dt1.Rows)
            {
                f1 += Convert.ToDouble(dr["feepayednow"].ToString());
                f2 += Convert.ToDouble(dr["fee"].ToString());
            }

            f1 = System.Math.Round(f1, 2);
            f2 = System.Math.Round(f2, 2);
            //xtecontractfeepayed.Text = f1.ToString();

            double ftmp = Convert.ToDouble(xtecontractfeepayed.Text);

            if (f1 != ftmp)
            {
                showopermsg("X", "[导入xls] 导入的实收总和不等于输入的实收！");
                xsbcontractfeepayedsave.Enabled = false;

                return;

            }

            if (f2 != (Convert.ToDouble(xtecontractpayfee.Tag)))
            {
                showopermsg("X", "[导入xls] 导入的应收总和不等于原应收总和！");
                xsbcontractfeepaysave.Enabled = false;

                return;

            }

            xsbcontractfeepaysave.Enabled = true;
            xsbcontractfeepayedsave.Enabled = true;

            showopermsg("Y", "[从xls导入房间收费数据]  成功导入!");

        }

        private void xsbcontractfeepayedfromxls_Click(object sender, EventArgs e)
        {

            importfncfeepayed(sxlsfile);


        }

        private void xsbcontractfeepayedfromxlsfile_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "xls 文件|*.xls;*.xlsx";
            DialogResult dr = op.ShowDialog();
            if (dr != DialogResult.OK) return;

            importfncfeepayed(op.FileName);                    

        }

        private void xsbcontractfeepayedsave_Click(object sender, EventArgs e)
        {
            double ft1=Convert.ToDouble(xtecontractfeepayed.Text);
            ft1 = (double)System.Math.Round(ft1, 2);

            double ft2 = Convert.ToDouble(xtecontractpayfee.Text);
            ft2 = (double)System.Math.Round(ft2, 2);

            if (ft2 == 0)
            {
                showopermsg("X","[保存收费数据] 应收为0！");
                return;
            }

            if ( ft1>ft2)
            {
                showopermsg("X" ,"[保存收费数据] 实收大于应收，请多选择些未缴月份！");
                return;
            }

           
            DataTable dt1 = ((DataView)xgvpp_feepayed.DataSource).Table;

            double ftotal = 0;
            foreach (DataRow dr in dt1.Rows) 
            {
                double f1 = Convert.ToDouble(dr["fee"].ToString()) - Convert.ToDouble(dr["feepayed"].ToString());
                f1 = (double)System.Math.Round(f1, 2);

                double f2 = Convert.ToDouble(dr["feepayednow"].ToString());
                f2 = (double)System.Math.Round(f2, 2);

                if (f2 > f1)
                {
                    showopermsg("X" ,"[保存收费数据]+ [" + dr["feemonth"].ToString() + "]实收大于应收，请重新分配！");
                    return;
                }

                ftotal += f2;
            };

            ftotal = System.Math.Round(ftotal, 2);

            if (ftotal != ft1)
            {
                showopermsg("X" ,"[保存收费数据] 各房间本次收取之和不等于输入的实收! 请重新分配！");
                return;
            }



            DialogResult dr1 = MessageBox.Show("确定要保存收费记录吗？", "确认保存", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            //输入该笔收费对应的月份
            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("输入收费所属月份",
                                                          "输入月份（用于统计月度收费）",
                                                          DateTime.Now.ToString("yyyyMM"), -1, -1);


            try
            {
                System.DateTime datet = DateTime.ParseExact(sss1,"yyyyMM",new CultureInfo("zh-CN", true));
 
            }
            catch
            {
                showopermsg("X", "[保存收费数据] 输入的所属月份有误！");
                return;
            }

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在保存收费数据...");

            //生成应收录入数据

            mydb.addfeepayedseq(xgvct_feepayed.GetDataRow(0), xtecontractfeepayed.Text, xgvpp_feepayed,sss1);

            showopermsg("Y", "[录入合同收费数据] 保存数据成功！");

            xtecontractfeepayed.Text = "0";

            initfeepayedform();

            XScmWF.CloseWaitForm();
        }

        private void initfeepayedseqform()
        {
            DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlfnc.FocusedNode;
            if (clickedNode.Level == 0) return;


            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取已录入的合同收费数据...");


            string sct = clickedNode["id"].ToString();


            xgcct_payedfeeseq.DataSource = mydb.getcontractfeepayedseq(sct);


            XScmWF.CloseWaitForm();

            xgvct_payedfeeseq_FocusedRowChanged(null, null);

            
        }

        private void bbifeepayedquerymgt_ItemClick(object sender, ItemClickEventArgs e)
        {
            //initfeepayedseqform();

            initfncfeeform(2);
        }

        private void xgvct_payedfeeseq_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_payedfeeseq.GetFocusedDataRow();
            if (dr == null)
            {
                xgcct_payedfeeseqmon.DataSource = null;
                xgvct_payedfeeseqmon_FocusedRowChanged(null, null);
                return;
            }

            xgcct_payedfeeseqmon.DataSource = mydb.getcontractfeepayedseqmon(dr);

            xgvct_payedfeeseqmon_FocusedRowChanged(null, null);
        }

        private void xgvct_payedfeeseqmon_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_payedfeeseqmon.GetFocusedDataRow();
            if (dr == null)
            {
                xgcctpp_payedfeeseqmon.DataSource = null;
                return;
            }

            xgcctpp_payedfeeseqmon.DataSource = mydb.getcontractppfeepayedseqmon(dr);

            
        }

        private void bbipayedfeedelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = xgvct_payedfeeseq.GetFocusedDataRow();
            if (dr == null) return;

            if(dr!=(xgvct_payedfeeseq.GetDataRow(xgvct_payedfeeseq.RowCount-1)))
            {
                showopermsg("X", "[删除收费记录] 只能删除当前的最后一次收费记录");
                return;
            }

            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_delete_feepayed_fnc", dr))
            {
                showopermsg("X", soper);
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要删除收费记录吗？", "确认删除", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            mydb.deletepayfeeseqfnc(dr);

            initfeepayedseqform();
        }

        private void bbiareablmgt_ItemClick(object sender, ItemClickEventArgs e)
        {
            initareacodeform();

 
        }

        private void bbictattquery_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dtr = xgvcontract.GetFocusedDataRow();
            if (dtr == null)
            {
                showopermsg("X", "[合同附件下载] 请先选中一个合同!");
                return;
            }


            bool bhasatt = mydb.checkcthasatt(dtr);

            if (!bhasatt)
            {
                showopermsg("X", "[合同附件下载] 合同没有附件! 请先上传合同附件");
                return;
            }

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在下载合同附件...");
            if (xrgip.SelectedIndex == 1)
            {
                XScmWF.SetWaitFormDescription("互联网速度较慢，请耐心等候");
            }

            DataTable dt = mydb.getcontractatt(dtr);

            XScmWF.CloseWaitForm();

            SaveFileDialog sa = new SaveFileDialog();
            sa.Filter = "All files |*.*";
            sa.FileName = dt.Rows[0]["attname"].ToString();
            DialogResult dr = sa.ShowDialog();
            if (dr != DialogResult.OK) return;


            byte[] bfile = (byte[])dt.Rows[0]["attachment"];
            FileStream fs = new FileStream(sa.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

            fs.Write(bfile, 0, bfile.Length);

            fs.Close();

            showopermsg("Y", "<下载合同附件>  成功下载!");

        }

        private void bbictattupload_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dtr = xgvcontract.GetFocusedDataRow();
            if (dtr == null)
            {
                showopermsg("X", "[上传合同附件] 请先选中一个合同!");
                return;
            }

            bool bhasatt = mydb.checkcthasatt(dtr);

            if (bhasatt)
            {
                DialogResult r = MessageBox.Show("合同已存在附件，要替换吗？", "确认替换", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (r == DialogResult.Cancel) return;
            }


                OpenFileDialog op = new OpenFileDialog();
                op.Filter = "office文件|*.xls;*.xlsx;*.doc;*.docx|All files |*.*";
                DialogResult dr = op.ShowDialog();
                if (dr != DialogResult.OK) return;



                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在保存合同附件...");
                if (xrgip.SelectedIndex == 1)
                {
                    XScmWF.SetWaitFormDescription("互联网速度较慢，请耐心等候");
                }

                if (bhasatt)
                {
                    mydb.updatecontractatt(dtr, op.FileName,op.SafeFileName);

                    showopermsg("Y", "<替换合同附件> 替换成功!");
                }
                else
                {
                    mydb.addcontractatt(dtr, op.FileName,op.SafeFileName);

                    showopermsg("Y", "<添加合同附件>  成功添加!");
                }

                XScmWF.CloseWaitForm();
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const uint KEYEVENTF_KEYUP = 0x02;
        public const uint VK_CONTROL = 0x11;

        private void Form1_Leave(object sender, EventArgs e)
        {
           //
            //MessageBox.Show("RR");
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            //MessageBox.Show("XX");
           // keybd_event((byte)VK_CONTROL, 0, (byte)KEYEVENTF_KEYUP, 0);
        }

        private void xluearea_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).EditValue = null;
            }
        }

        private void xsbsysconfigsave_Click(object sender, EventArgs e)
        {
            mydb.updatesyspara("sysname", xtesysname.Text);

            showopermsg("Y", "<修改系统参数>  成功修改!");

            this.Text = xtesysname.Text;
        }

        private void initfncfeeform(int itype)
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取查询树......");

            if (itype == 0)//合同应收
            {
                if ((this.tlfnc.DataSource == null)||
                    (iqtypefnc != itype)
                )
                {
                    this.tlfnc.Nodes.Clear();

                    this.tlfnc.DataSource = mydb.getpp();
                    this.tlfnc.Columns["ppname"].Caption = "经开物业";


                    iqtypefnc = itype;

                    //xdefncpaymonquery.EditValue = System.DateTime.Now.ToString("yyyyMM");
                    

                }

                this.xtcsys.SelectedTabPage = xtpfnc;
                xtcfeefnc.SelectedTabPage = xtppayfnc;

                XScmWF.CloseWaitForm();

                foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlfnc.Nodes)
                {
                    rn.Expanded = true;
                }
            }
            else if (itype == 1||itype==2||itype==3)//费用录入、查询
            {
                if ((this.tlfnc.DataSource == null) ||
                    (iqtypefnc != itype)
                )
                {
                    this.tlfnc.Nodes.Clear();

                    this.tlfnc.DataSource = mydb.getppareatree();
                    this.tlfnc.Columns["ppname"].Caption = "经开物业";


                    iqtypefnc = itype;

                    if (itype == 1 || itype == 2)
                    {
                        foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlfnc.Nodes)
                        {
                            string s1 = rn["id"].ToString();
                            DataTable d123 = mydb.getcontractbyareafnc(s1);

                            for (int ii = 0; ii < d123.Rows.Count; ii++)
                            {
                                tlfnc.AppendNode(new Object[] { d123.Rows[ii]["contractid"].ToString(), 
                                                  d123.Rows[ii]["contractid"].ToString(), 
                                                  d123.Rows[ii]["ctno_cus"].ToString() }, rn);
                            }

                        }
                    }

                }



                if (itype == 1)
                {
                    this.xtcsys.SelectedTabPage = xtpfnc;
                    xtcfeefnc.SelectedTabPage = xtppayedfnc;

                    xdefncpaymon.EditValue = System.DateTime.Now.ToString("yyyyMM");

                }
                else if (itype == 2)
                {
                    this.xtcsys.SelectedTabPage = xtpfnc;
                    xtcfeefnc.SelectedTabPage = xtppayedseqfnc;
                }
                else if (itype == 3)
                {
                    this.xtcsys.SelectedTabPage = xtpfnc;
                    xtcfeefnc.SelectedTabPage = xtpcttobechecked;

                    string s119 = wheda.db.dboper.scontractquery + " where (contractstatus='等待审核' or contractstatus='等待修改审核' or contractstatus='申请修改')";
                    xgccttobechecked.DataSource = mydb.gettablebystr(s119);


                }



                XScmWF.CloseWaitForm();
            }


            foreach (DevExpress.XtraTreeList.Columns.TreeListColumn tlc in tlfnc.Columns)
            {
                tlc.OptionsColumn.AllowSort = false;
            }


            ribbonControl.SelectedPage = xrpfinance;
        }

        private void initqueryform(int itype)
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取查询树......");

            if (itype == 0)//房屋信息
            {
                if ((this.tlquery.DataSource == null) ||
                                   (iqtype != itype)
                               )
                {
                    this.tlquery.Nodes.Clear();

                    this.tlquery.DataSource = mydb.getpp();
                    this.tlquery.Columns["ppname"].Caption = "经开物业";


                    iqtype = itype;
                }

                ribbonControl.SelectedPage = xrppp;
                this.xtcsys.SelectedTabPage = xtpinfoquery;
                xtcinfoquery.SelectedTabPage = xtpppinfo;

                XScmWF.CloseWaitForm();

                foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlquery.Nodes)
                {
                    rn.Expanded = true;
                }
            }
            else if (itype == 1)//合同信息
            {
                if ((this.tlquery.DataSource == null)||
                    (iqtype!=itype)
                )
                {
                    this.tlquery.Nodes.Clear();

                    this.tlquery.DataSource = mydb.getpp();
                    this.tlquery.Columns["ppname"].Caption = "经开物业";


                    iqtype = itype;

                }

                ribbonControl.SelectedPage = xrpcontract;
                this.xtcsys.SelectedTabPage = xtpinfoquery;
                xtcinfoquery.SelectedTabPage = xtpcontractinfo;

                XScmWF.CloseWaitForm();

                foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlquery.Nodes)
                {
                    rn.Expanded = true;
                }
            }
            else if (itype == 2)//费用信息
            {
                if ((this.tlquery.DataSource == null) ||
                    (iqtype != itype)
                )
                {
                    this.tlquery.Nodes.Clear();

                    this.tlquery.DataSource = mydb.getppareatree();
                    this.tlquery.Columns["ppname"].Caption = "经开物业";


                    iqtype = itype;

                    foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlquery.Nodes)
                    {
                        string s1 = rn["id"].ToString();
                        DataTable d123 = mydb.getcontractbyarea(s1);

                        for (int ii = 0; ii < d123.Rows.Count; ii++)
                        {
                            tlquery.AppendNode(new Object[] { d123.Rows[ii]["contractid"].ToString(), 
                                                  d123.Rows[ii]["contractid"].ToString(), 
                                                  d123.Rows[ii]["ctno_cus"].ToString() }, rn);
                        }

                    }

                }

                ribbonControl.SelectedPage = xrpfee;
                this.xtcsys.SelectedTabPage = xtpinfoquery;
                xtcinfoquery.SelectedTabPage = xtpfeequery;

                



                XScmWF.CloseWaitForm();
            }
            else if (itype == 3)//收费
            {
                if ((this.tlquery.DataSource == null) ||
                    (iqtype != itype)
                )
                {
                    this.tlquery.Nodes.Clear();

                    this.tlquery.DataSource = mydb.getppareatree();
                    this.tlquery.Columns["ppname"].Caption = "经开物业";


                    iqtype = itype;
                }

                this.xtcsys.SelectedTabPage = xtpinfoquery;
                xtcinfoquery.SelectedTabPage = xtppayintput;

                if (xdepaysdt.EditValue == null)
                {
                    xdepaysdt.DateTime = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);

                    if (xdepayedt.EditValue == null)
                    {
                        xdepayedt.DateTime = xdepaysdt.DateTime.AddMonths(1).AddDays(-1);
                    }
                }

                
                XScmWF.CloseWaitForm();

            }
            else if (itype == 4)//台帐
            {
                if ((this.tlquery.DataSource == null) ||
                    (iqtype != itype)
                )
                {
                    this.tlquery.Nodes.Clear();

                    this.tlquery.DataSource = mydb.getppareatree();
                    this.tlquery.Columns["ppname"].Caption = "经开物业";


                    iqtype = itype;
                }

                this.xtcsys.SelectedTabPage = xtpinfoquery;
                xtcinfoquery.SelectedTabPage = xtprptmgtfee;

                if (xderptmgtfee.EditValue == null)
                {
                    xderptmgtfee.DateTime = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, 1);

                    
                }


                XScmWF.CloseWaitForm();
            }

            foreach (DevExpress.XtraTreeList.Columns.TreeListColumn tlc in tlquery.Columns)
            {
                tlc.OptionsColumn.AllowSort = false;
            }

        }

        private void bbictquerymgt_ItemClick(object sender, ItemClickEventArgs e)
        {
            initqueryform(1);
        }



        private void tlquery_Click(object sender, EventArgs e)
        {

            Point point = tlquery.PointToClient(Cursor.Position);
            DevExpress.XtraTreeList.TreeListHitInfo hitInfo = tlquery.CalcHitInfo(point);

            if (iqtype == 0)
            {
                if (hitInfo.HitInfoType != DevExpress.XtraTreeList.HitInfoType.Cell) return;

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlquery.FocusedNode;


                string sarea = null;
                string sbuilding = null;
                string slevel = null;
                if (clickedNode.Level == 0)
                {
                    sarea = clickedNode["id"].ToString();

                    xgvinfoquerypp.ViewCaption = clickedNode["ppname"].ToString();

                }
                else if (clickedNode.Level == 1)
                {
                    sarea=clickedNode.ParentNode["id"].ToString();
                    sbuilding = clickedNode["id"].ToString();

                    xgvinfoquerypp.ViewCaption = clickedNode.ParentNode["ppname"].ToString() + " | " +
                                                 clickedNode["ppname"].ToString();

                }
                else if (clickedNode.Level == 2)
                {
                    sarea =clickedNode.ParentNode.ParentNode["id"].ToString();
                    sbuilding = clickedNode.ParentNode["id"].ToString();
                    slevel = clickedNode["id"].ToString();

                    xgvinfoquerypp.ViewCaption = clickedNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                       clickedNode.ParentNode["ppname"].ToString() + " | " +
                       clickedNode["ppname"].ToString();

                }


                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取房间资料...");

                

                this.xgcinfoquerypp.DataSource = mydb.getppunitbyquery(sarea, sbuilding, slevel);

                
  
                XScmWF.CloseWaitForm();
            }
            else if (iqtype == 1)
            {
                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlquery.FocusedNode;
                //if (clickedNode.Level == 2) return;


                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取合同资料...");

                if (clickedNode.Level == 3)
                {
                    string spp = clickedNode["id"].ToString();

                    xgvinfoqueryct.ViewCaption = clickedNode.ParentNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                                                 clickedNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                                                 clickedNode.ParentNode["ppname"].ToString() + " | " +
                                                 clickedNode["ppname"].ToString();

                    string squery = wheda.db.dboper.scontractquery +
                                    " where contractid in (select a.contractid  from t_con_pp a ,t_ppunit b where a.ppid=b.ppid and a.ppid=" +
                                    spp + ")";

                    xgcinfoqueryct.DataSource = mydb.gettablebystr(squery);


                }
                else if (clickedNode.Level == 2)
                {
                    string sarea = clickedNode.ParentNode["parentid"].ToString();
                    string sbuilding = clickedNode["parentid"].ToString();
                    string slevel = clickedNode["id"].ToString();

                    xgvinfoqueryct.ViewCaption = clickedNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                           clickedNode.ParentNode["ppname"].ToString() + " | " +
                           clickedNode["ppname"].ToString();

                    string squery = wheda.db.dboper.scontractquery +
                    " where contractid in(select a.contractid  from t_con_pp a ,t_ppunit b where a.ppid=b.ppid and b.unitlevel=" +
                    slevel + ")";

                    xgcinfoqueryct.DataSource = mydb.gettablebystr(squery);

                }
                else if (clickedNode.Level == 1)
                {
                    string sarea = clickedNode["parentid"].ToString();
                    string sbuilding = clickedNode["id"].ToString();


                    xgvinfoqueryct.ViewCaption = clickedNode.ParentNode["ppname"].ToString() + " | " +
                           clickedNode["ppname"].ToString();

                    string squery = wheda.db.dboper.scontractquery +
                                    " where contractid in(select a.contractid  from t_con_pp a ,t_ppunit b where a.ppid=b.ppid and b.UnitBuilding=" +
                                    sbuilding + ")";

                    xgcinfoqueryct.DataSource = mydb.gettablebystr(squery);


                }
                else if (clickedNode.Level == 0)
                {

                    string sarea = clickedNode["id"].ToString();


                    xgvinfoqueryct.ViewCaption = clickedNode["ppname"].ToString();

                    string squery = wheda.db.dboper.scontractquery + " where contractarea=" + sarea;

                    xgcinfoqueryct.DataSource = mydb.gettablebystr(squery);

                }


                // this.xgcinfoqueryct.DataSource = mydb.getppunitbyid(sarea, sbuilding, slevel);

                XScmWF.CloseWaitForm();

                xgvinfoqueryct_FocusedRowChanged(null, null);
            }
            else if (iqtype == 2)
            {

                
                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlquery.FocusedNode;
                if (clickedNode.Level == 0)
                {
                    xgcinfoquery_fee.DataSource = null;
                    xgvinfoquery_fee_FocusedRowChanged(null, null);
                    return;
                }

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取合同应收...");


                string sctid = clickedNode["id"].ToString();

                xgcinfoquery_fee.DataSource = mydb.getcontractfeepaymgt(sctid);


                XScmWF.CloseWaitForm();
                xgvinfoquery_fee_FocusedRowChanged(null, null);
            }
            else if (iqtype == 3)
            {


                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlquery.FocusedNode;
                if (clickedNode.Level != 0) return;

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取合同待收费用...");


                string sarea = clickedNode["id"].ToString();
                string s1 = xdepaysdt.Text;
                string s2 = xdepayedt.Text;

                getareafeepaybyspan(sarea, s1, s2);

                

                XScmWF.CloseWaitForm();
                xgvpayfeect_FocusedRowChanged(null, null);
            }
            else if(iqtype == 4) //台帐
            {


                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlquery.FocusedNode;
                if (clickedNode.Level != 0) return;

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取项目应收台帐...");


                string sarea = clickedNode["id"].ToString();
                string smon = xderptmgtfee.Text;

                xlcrptmgtfee.Text = clickedNode["ppname"].ToString();

                xgcrptmgtfee.DataSource = mydb.getcontractrptfeemgt(sarea, smon);

                XScmWF.CloseWaitForm();
                xgvpayfeect_FocusedRowChanged(null, null);

                xgvpayfeect.BestFitColumns();
            }
           
        }

        private void getareafeepaybyspan(string sarea, string sdt, string edt)
        {
            string squery = wheda.db.dboper.scontractquery + " where contractarea=" + sarea +
                 " and contractid in (select distinct contractid from t_fee_pay_mgt_period " +
                 //" where feepaysdt>='" + sdt + "' and feepaysdt<='" + edt + "')";
                 " where feepaysdt<='" + edt +"'"+ 
                 " and ifnull(feepayed,0)=0 )";
            xgcpayfeect.DataSource = mydb.gettablebystr(squery);
        }

        private void tlquery_DoubleClick(object sender, EventArgs e)
        {
            if (iqtype == 1)
            {
                Point point = tlquery.PointToClient(Cursor.Position);
                DevExpress.XtraTreeList.TreeListHitInfo hitInfo = tlquery.CalcHitInfo(point);

                //
                if (hitInfo.HitInfoType != DevExpress.XtraTreeList.HitInfoType.Cell)
                {
                    return;
                }
                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlquery.FocusedNode;
                if (clickedNode.Level != 2)
                {
                    return;
                }
                if (clickedNode.HasChildren)
                {
                    return;
                }

                string sarea = clickedNode.ParentNode["parentid"].ToString();
                string sbuilding = clickedNode["parentid"].ToString();
                string slevel = clickedNode["id"].ToString();

                DataTable dt = mydb.getppunitbyid(sarea, sbuilding, slevel);

                for (int ii = 0; ii < dt.Rows.Count; ii++)
                {
                    tlquery.AppendNode(new Object[] { dt.Rows[ii]["ppid"].ToString(), 
                                                  dt.Rows[ii]["ppid"].ToString(), 
                                                  dt.Rows[ii]["unitno"].ToString() }, clickedNode);
                }
            }
        }

        private void xgvinfoqueryct_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvinfoqueryct.GetFocusedDataRow();
            if (dr == null)
            {
                xgcinfoqueryct_pp.DataSource = null;
                
                return;
            }

            string sctid = dr["contractid"].ToString();

            if (sctid != "")
            {
                xgcinfoqueryct_pp.DataSource = mydb.getppunitbycontract(sctid);
                
            }

            //xgvct_ppunit_FocusedRowChanged(null, null);


        }

        private void bbifeequerymgt_ItemClick(object sender, ItemClickEventArgs e)
        {
            initqueryform(2);
        }

        private void xgvinfoquery_fee_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvinfoquery_fee.GetFocusedDataRow();

            if (dr == null)
            {
                xgcinfoquery_fee_pp.DataSource = null;
                return;
            }


            xgcinfoquery_fee_pp.DataSource = mydb.getppfeepaymgtbyperiod(dr);
        }

        private void bbippquerymgt_ItemClick(object sender, ItemClickEventArgs e)
        {
            initqueryform(0);
        }

        private void bbifeepayinput_ItemClick(object sender, ItemClickEventArgs e)
        {
            initqueryform(3);
        }

        private void xgvpayfeect_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvpayfeect.GetFocusedDataRow();

            if (dr == null)
            {
                xgcpayfeectfee.DataSource = null;
                xgvpayfeectfee_FocusedRowChanged(null, null);

                return;
            }

            xgcpayfeectfee.DataSource = mydb.getcontractfeepaymgtbyspan(dr["contractid"].ToString(),
                                                                        xdepaysdt.Text,
                                                                        xdepayedt.Text);

            xgvpayfeectfee_FocusedRowChanged(null, null);
        }

        private void xgvpayfeectfee_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvpayfeectfee.GetFocusedDataRow();

            if (dr == null)
            {
                xgcpayfeeppfee.DataSource = null;
                return;
            }


            xgcpayfeeppfee.DataSource = mydb.getppfeepaymgtbyperiod(dr);
        }

        private void xsbfeepayperiodadjust_Click(object sender, EventArgs e)
        {
            adjustpayfeemgtfnc(1);
        }

        private void xsbfeepayquery_Click(object sender, EventArgs e)
        {
            string sarea = tlquery.FocusedNode["id"].ToString();
            string s1 = xdepaysdt.Text;
            string s2 = xdepayedt.Text;

            getareafeepaybyspan(sarea, s1, s2);


        }

        private void xsbfeepayinput_Click(object sender, EventArgs e)
        {
            DataRow ddr = xgvpayfeect.GetFocusedDataRow();

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_input_fee_manage", ddr))
            {
                showopermsg("X", soper);
                return;
            }

            DataRow dr = xgvpayfeectfee.GetFocusedDataRow();

            if (dr == null) return;

            if (xgvpayfeectfee.SelectedRowsCount > 1)
            {
                showopermsg("X", "[收费录入] 您选中了多个应收，请只选中一个应收！");
                return;
            }


            frmfeepayinput fcpm = new frmfeepayinput();
            //fcpm.XScmWF = this.XScmWF;

            //fcpm.xtefeepay.Text=xgvpayfeectfee.Columns["feepay"].SummaryText;
            fcpm.sfee = dr["feepay"].ToString();
            fcpm.sct = dr["contractid"].ToString();
            fcpm.sctno = dr["contractno"].ToString();
            fcpm.sdt = dr["feepaysdt"].ToString();
            fcpm.edt = dr["feepayedt"].ToString();
            fcpm.scus = (xgvpayfeect.GetFocusedDataRow())["cusname"].ToString();

            fcpm.gv = xgvpayfeectfee;


            fcpm.ShowDialog();

            xgvpayfeect_FocusedRowChanged(null, null);
        }

        private void xsbexpcustoxls_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvcus);
        }

        private void xbetlfont_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void xbetlfont_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            FontDialog fd = new FontDialog();

            string[] ss = xbetlfont.Text.Split(new char[] { ',' });
            if (ss.Length == 2)
            {

                fd.Font = new Font(new FontFamily(ss[0]), Convert.ToSingle(ss[1]));
            }

            if (fd.ShowDialog() == DialogResult.OK)
            {
                xbetlfont.Text = fd.Font.Name + "," + fd.Font.SizeInPoints.ToString();
                tlquery.Appearance.Row.Font = fd.Font;

                tlfnc.Appearance.Row.Font = fd.Font;

                mydb.updateuserpara("tlfont", xbetlfont.Text);
            }
        }

        private void bbippal_ItemClick(object sender, ItemClickEventArgs e)
        {
            inithomepage();
        }
        
        private void bbifeeal_ItemClick(object sender, ItemClickEventArgs e)
        {
            inithomepage();
        }

        private void xluefncpaymon_Popup(object sender, EventArgs e)
        {
  
        }

        private void xdefncpaymonquery_Popup(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.DateEdit edit = sender as DevExpress.XtraEditors.DateEdit;
            DevExpress.XtraEditors.Popup.PopupDateEditForm form =
                (edit as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupDateEditForm;
            form.Calendar.View = DevExpress.XtraEditors.Controls.DateEditCalendarViewType.YearInfo;
        }

        private void tlfnc_Click(object sender, EventArgs e)
        {

            Point point = tlfnc.PointToClient(Cursor.Position);
            DevExpress.XtraTreeList.TreeListHitInfo hitInfo = tlfnc.CalcHitInfo(point);

            if (iqtypefnc == 0)  //应收查询
            {
                if (hitInfo.HitInfoType != DevExpress.XtraTreeList.HitInfoType.Cell) return;

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlfnc.FocusedNode;


                string sarea = null;
                string sbuilding = null;
                string slevel = null;
                string spp = null;
                if (clickedNode.Level == 0)
                {
                    sarea = clickedNode["id"].ToString();

                    lxcfeefncct.Text= clickedNode["ppname"].ToString();

                }
                else if (clickedNode.Level == 1)
                {
                    sarea = clickedNode.ParentNode["id"].ToString();
                    sbuilding = clickedNode["id"].ToString();

                    lxcfeefncct.Text = clickedNode.ParentNode["ppname"].ToString() + " | " +
                                                 clickedNode["ppname"].ToString();

                }
                else if (clickedNode.Level == 2)
                {
                    sarea = clickedNode.ParentNode.ParentNode["id"].ToString();
                    sbuilding = clickedNode.ParentNode["id"].ToString();
                    slevel = clickedNode["id"].ToString();

                    lxcfeefncct.Text = clickedNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                       clickedNode.ParentNode["ppname"].ToString() + " | " +
                       clickedNode["ppname"].ToString();

                }
                else if (clickedNode.Level == 3)
                {
                    sarea = clickedNode.ParentNode.ParentNode.ParentNode["id"].ToString();
                    sbuilding = clickedNode.ParentNode.ParentNode["id"].ToString();
                    slevel = clickedNode.ParentNode["id"].ToString(); 
                    spp=clickedNode["id"].ToString();

                    lxcfeefncct.Text = clickedNode.ParentNode.ParentNode.ParentNode["ppname"].ToString() + " | " + 
                        clickedNode.ParentNode.ParentNode["ppname"].ToString() + " | " +
                      clickedNode.ParentNode["ppname"].ToString() + " | " +
                      clickedNode["ppname"].ToString();

                }


                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取合同应收...");

                xgcct_feepay.DataSource = mydb.getcontractfeepayfnc(xdefncpaymonquery.Text,sarea, sbuilding, slevel,spp);

                xgvct_feepay_FocusedRowChanged(null, null);

                XScmWF.CloseWaitForm();
            }
            else if (iqtypefnc == 2) //收费查询
            {
                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }


                initfeepayedseqform();


                // this.xgcinfoqueryct.DataSource = mydb.getppunitbyid(sarea, sbuilding, slevel);

               
            }
            else if (iqtypefnc == 1)
            {


                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlfnc.FocusedNode;
                if (clickedNode.Level == 0) return;

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取合同应收...");

                string sctid = clickedNode["id"].ToString();

                DataRow dr = mydb.getcontractbyid(sctid);

                xlcctfncinfo.Text = "[" + dr["contractnofnc"].ToString() + "] " +
                                  "[" + dr["contractno"].ToString() + "] " +
                                  "[" + dr["cusname"].ToString() + "] " +
                                  "[" + dr["contractstatus"].ToString() + "] " +
                                  "[" + dr["contractsdt"].ToString() + "]-" +
                                  "[" + dr["contractedt"].ToString() + "] ";

                xgcct_feepayed.DataSource = mydb.getcontractfeepayed(sctid, xdefncpaymon.Text, 
                                                                     xcefncpayfeeshowpayed.Checked);


                XScmWF.CloseWaitForm();

                xgvct_feepayed_DataSourceChanged(null, null);
            }
            else if (iqtypefnc == 3)
            {


                //
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
                {
                    return;
                }

                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlfnc.FocusedNode;
                if (clickedNode.Level != 0)
                {
                    return;
                }

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取待审合同...");


                string sarea = clickedNode["id"].ToString();

                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Column)
                {

                    xgccttobechecked.DataSource = mydb.gettablebystr(
                        wheda.db.dboper.scontractquery );
                }
                else
                {
                    xgccttobechecked.DataSource = mydb.gettablebystr(
                        wheda.db.dboper.scontractquery + " where (contractstatus='等待审核' or contractstatus='等待修改审核' or contractstatus='申请修改') " +
                        " and contractarea=" + sarea);
                }



                XScmWF.CloseWaitForm();
                xgvcttobechecked_FocusedRowChanged(null, null);
            }

           
        }

        private void xgvcttobechecked_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvcttobechecked.GetFocusedDataRow();
            if (dr == null)
            {
                xgccttobechecked_ppunit.DataSource = null;
                xgcppfeemgt_tobechecked.DataSource = null;

                return;
            }

            string sctid = dr["contractid"].ToString();

            if (sctid != "")
            {
                xgccttobechecked_ppunit.DataSource = mydb.getppunitbycontract(sctid);
                //               xgvct_ppunit.BestFitColumns();
            }

            xgvcttobechecked_ppunit_FocusedRowChanged(null, null);

   
        }

        private void xgvcttobechecked_ppunit_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvcttobechecked_ppunit.GetFocusedDataRow();

            if (dr == null)
            {
                xgcppfeemgt_tobechecked.DataSource = null;
                return;
            }

            
            xgcppfeemgt_tobechecked.DataSource = mydb.getppfeepaymgt(dr);
        }

        private void xgvct_ppunit_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            if (((DevExpress.XtraGrid.GridSummaryItem)e.Item).FieldName != "rent")
                return;

            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                e.TotalValue = 0;
            }

            if (e.SummaryProcess ==DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                Single val =Convert.ToSingle( e.FieldValue.ToString());
                DataRow dr = xgvct_ppunit.GetDataRow(e.RowHandle);
                
                e.TotalValue =Convert.ToSingle(e.TotalValue.ToString())+ val +
                              Convert.ToSingle( dr["bfee"].ToString());
            }

        }

        private void xdefncpaymonquery_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).EditValue = null;
            }
        }

        private void xdefncpaymon_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                (sender as DevExpress.XtraEditors.BaseEdit).EditValue = null;
            }
        }

        private void bbirptmgtfee_ItemClick(object sender, ItemClickEventArgs e)
        {
            initqueryform(4);
        }

        private void xderptmgtfee_Popup(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.DateEdit edit = sender as DevExpress.XtraEditors.DateEdit;
            DevExpress.XtraEditors.Popup.PopupDateEditForm form =
                (edit as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupDateEditForm;
            form.Calendar.View = DevExpress.XtraEditors.Controls.DateEditCalendarViewType.YearInfo;
        }

        private void xsbexportrptfeemgttoxls_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvrptmgtfee);
        }

        private void tlfnc_DoubleClick(object sender, EventArgs e)
        {
            if (iqtypefnc == 0)
            {
                Point point = tlfnc.PointToClient(Cursor.Position);
                DevExpress.XtraTreeList.TreeListHitInfo hitInfo = tlfnc.CalcHitInfo(point);

                //
                if (hitInfo.HitInfoType != DevExpress.XtraTreeList.HitInfoType.Cell)
                {
                    return;
                }
                DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlfnc.FocusedNode;
                if (clickedNode.Level != 2)
                {
                    return;
                }
                if (clickedNode.HasChildren)
                {
                    return;
                }

                string sarea = clickedNode.ParentNode["parentid"].ToString();
                string sbuilding = clickedNode["parentid"].ToString();
                string slevel = clickedNode["id"].ToString();

                DataTable dt = mydb.getppunitbyid(sarea, sbuilding, slevel);

                for (int ii = 0; ii < dt.Rows.Count; ii++)
                {
                    tlfnc.AppendNode(new Object[] { dt.Rows[ii]["ppid"].ToString(), 
                                                  dt.Rows[ii]["ppid"].ToString(), 
                                                  dt.Rows[ii]["unitno"].ToString() }, clickedNode);
                }
            }
        }

        private void xccbarea_EditValueChanged(object sender, EventArgs e)
        {
            xlueppcontractname_EditValueChanged(sender, e);

            if (xccbarea.Text != "")
            {
                string sarea = xccbarea.EditValue.ToString();


                xluebuilding.Properties.DataSource = mydb.getppbuildingbyareaid(sarea);
            }

            changeuienable_ppquery();
        }

        private void xccbarea_Popup(object sender, EventArgs e)
        {
        //    (sender as DevExpress.XtraEditors.CheckedComboBoxEdit).Properties.DropDownRows = 
        //        (sender as DevExpress.XtraEditors.CheckedComboBoxEdit).Properties.Items.Count+1;
        //
        }

        private void bbiattachctnofnc_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcfeefnc.SelectedTabPage == xtpcttobechecked)
            {

                if (xgvcttobechecked.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[附加财务合同编号] 请一次只选一个合同！");
                    return;
                }

                dr = xgvcttobechecked.GetFocusedDataRow();
            }
            else
            {
                xtcfeefnc.SelectedTabPage = xtpcttobechecked;
                return;
            }


            if (dr == null)
            {
                showopermsg("X", "[附加财务合同编号] 请先选择一个合同！");
                return;
            }


            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("输入财务合同编号", 
                                                                     dr["cusname"].ToString(),
                                                                     dr["contractnofnc"].ToString(), -1, -1);

            if (sss1 == "") return;

            mydb.attachcontractnofnc(dr,sss1);

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在附加财务合同编号，请稍等...");
            
            dr["contractnofnc"]=sss1;

            XScmWF.CloseWaitForm();

            showopermsg("Y", "<附加财务合同编号> 操作成功！");
        }

        private void bbifeepayedconfirmfnc_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = xgvct_payedfeeseq.GetFocusedDataRow();
            if (dr == null) return;

            if (dr == null)
            {
                showopermsg("X", "[收费复核] 请选择一条收费记录!");
                return;
            }


            DialogResult dr1 = MessageBox.Show("确定要复核该收费记录吗？", "确认复核", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            mydb.feepayedconfirmedfnc(dr);

            initfeepayedseqform();

            showopermsg("Y", "<复核收费记录> 操作成功！");
        }

        private void xsetlwidth_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == xsetlwidth.Properties.Buttons[1])
            {
                mydb.updateuserpara("tlwidth", xsetlwidth.Text);

                xscctlmgt.SplitterPosition = Convert.ToInt32(xsetlwidth.Value);
                xscctlfnc.SplitterPosition = xscctlmgt.SplitterPosition;
                xsccrptfnc.SplitterPosition = xscctlmgt.SplitterPosition;
            }
        }

        private void xseinformdays_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == xseinformdays.Properties.Buttons[1])
            {
                mydb.updateuserpara("ifdays", xseinformdays.Text);

             }
        }

        private void bbirestoreinform_ItemClick(object sender, ItemClickEventArgs e)
        {
            frmSingleSel mysel = new frmSingleSel();

            mysel.Text = "选择需要恢复的提醒";

            mysel.xgvsinglesel.Columns.Clear();
            mysel.xgvsinglesel.Columns.Add();
            mysel.xgvsinglesel.Columns[0].Caption = "提醒描述";
            mysel.xgvsinglesel.Columns[0].FieldName = "ignoredmsg";
            mysel.xgvsinglesel.Columns[0].Visible = true;

           
            mysel.dtsrc = mydb.getignoredinform(uid);

            DialogResult dr = mysel.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string idno = mysel.drrt["idno"].ToString();
                mydb.restoreinform(idno);
            }
        }

        public void dismonthinform()
        {
            xschec.ActiveViewType = SchedulerViewType.Month;
            {

                XScmWF.ShowWaitForm();
                XScmWF.SetWaitFormCaption("正在获取阶段提醒数据...");

                xschec.Storage.Appointments.Clear();
                xschec.Storage.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("contractid", "cid"));


                DataTable dt = mydb.getcontractinformmon(dninform.DateTime);

                foreach (DataRow dr in dt.Rows)
                {

                    Appointment apt = xschec.Storage.CreateAppointment(AppointmentType.Normal);


                    System.DateTime datet = DateTime.ParseExact(dr["contractedt"].ToString(),
                                                "yyyyMMdd",
                                                new CultureInfo("zh-CN", true)
                                                );
                    TimeSpan ts = datet - System.DateTime.Now.Date;

                    Int32 ids = ts.Days;

                    apt.Start = datet;

                    apt.Subject = "合同: [" +
                                  dr["contractno"].ToString() + "]-[" +
                                  dr["cusname"].ToString() + "]  " +
                                  "到期";

                    apt.AllDay = true;

                    {
                        apt.LabelId = 3;//Convert.ToInt32(dr["label"].ToString());
                    }

                    apt.StatusId = 2;




                    apt.Description = "";

                    apt.CustomFields["contractid"] = dr["contractid"].ToString();

                    apt.HasReminder = true;
                    apt.Reminder.AlertTime = System.DateTime.Now.AddSeconds(10);


                    xschec.Storage.Appointments.Add(apt);
                }


                dt = mydb.getpayfeeinformmon(dninform.DateTime);
                foreach (DataRow dr in dt.Rows)
                {

                    Appointment apt = xschec.Storage.CreateAppointment(AppointmentType.Normal);


                    System.DateTime datet = DateTime.ParseExact(dr["feepaysdt"].ToString(),
                                                "yyyyMMdd",
                                                new CultureInfo("zh-CN", true)
                                                );
                    TimeSpan ts = datet - System.DateTime.Now.Date;

                    Int32 ids = ts.Days;

                    apt.Start = datet;

                    apt.Subject = "合同: [" +
                                  dr["contractno"].ToString() + "]-[" +
                                  dr["cusname"].ToString() + "]  " +
                                  "收租";

                    apt.AllDay = true;

                    if (ids < 3)
                    {
                        apt.LabelId = 1;
                    }
                    else
                    {
                        apt.LabelId = 3;//Convert.ToInt32(dr["label"].ToString());
                    }


                    apt.StatusId = 3;




                    apt.Description = "";

                    apt.CustomFields["contractid"] = dr["contractid"].ToString();

                    apt.HasReminder = true;
                    apt.Reminder.AlertTime = System.DateTime.Now.AddSeconds(10);


                    xschec.Storage.Appointments.Add(apt);
                }

                XScmWF.CloseWaitForm();
            }

            xschec.Start = dninform.DateTime;
        }



       

      

        private void xschec_PopupMenuShowing(object sender, DevExpress.XtraScheduler.PopupMenuShowingEventArgs e)
        {
            e.Menu.Items.Clear();
        }

        private void bbiinformmon_ItemClick(object sender, ItemClickEventArgs e)
        {
            xtcsys.SelectedTabPage = xtpschedule;
            dismonthinform();
        }

        private void bbicontractremodify_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcsys.SelectedTabPage == xtpcontract)
            {
                if (xgvcontract.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[申请合同修改] 请一次只选一个合同！");
                    return;
                }

                dr = xgvcontract.GetFocusedDataRow();
                if (dr == null)
                {
                    showopermsg("X", "[申请合同修改] 请先选择一个合同！");
                    return;
                }
            }

            if (xtcsys.SelectedTabPage == xtpinfoquery &&
                xtcinfoquery.SelectedTabPage == xtpcontractinfo)
            {
                if (xgvinfoqueryct.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[申请合同修改] 请一次只选一个合同！");
                    return;
                }

                dr = xgvinfoqueryct.GetFocusedDataRow();
                if (dr == null)
                {
                    showopermsg("X", "[申请合同修改] 请先选择一个合同！");
                    return;
                }
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_remodify_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }


            DialogResult dr1 = MessageBox.Show("确定要申请合同修改吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;


            mydb.changecontractstatus(dr, 3);

            dr["contractstatus"] = "申请修改";

        }

        private void bbicancelremodify_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcfeefnc.SelectedTabPage == xtpcttobechecked)
            {

                if (xgvcttobechecked.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[合同取消修改] 请一次只选一个合同！");
                    return;
                }

                dr = xgvcttobechecked.GetFocusedDataRow();
            }
            else
            {
                xtcfeefnc.SelectedTabPage = xtpcttobechecked;
                return;
            }



            if (dr == null)
            {
                showopermsg("X", "[合同取消修改] 请先选择一个合同！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_cancelremodify_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要取消该合同的修改申请吗？", "确认取消修改申请", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("输入取消修改原因",
                                                                                 dr["cusname"].ToString() + "[" +
                                                                                 dr["contractnofnc"].ToString() + "  " +
                                                                                 dr["contractno"].ToString() + "]",
                                                                                 "", -1, -1);

            if (sss1 == "") return;

            sss1 = "[取消审核]\r\n" + sss1;


            mydb.changecontractstatus(dr, 4,sss1);

            dr["contractstatus"] = "已审核";

            showopermsg("Y", "<合同取消修改> 取消成功！");
        }

        private void bbiapproveremodify_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcfeefnc.SelectedTabPage == xtpcttobechecked)
            {

                if (xgvcttobechecked.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[同意合同修改] 请一次只选一个合同！");
                    return;
                }

                dr = xgvcttobechecked.GetFocusedDataRow();
            }
            else
            {
                xtcfeefnc.SelectedTabPage = xtpcttobechecked;
                return;
            }


            if (dr == null)
            {
                showopermsg("X", "[同意合同修改] 请先选择一个合同！");
                return;
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_approveremodify_contract", dr))
            {
                showopermsg("X", soper);
                return;
            }

            DialogResult dr1 = MessageBox.Show("确定要同意合同修改吗？", "确认同意修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;



            mydb.changecontractstatus(dr, 5);

            dr["contractstatus"] = "修改";


            showopermsg("Y", "<同意合同修改> 操作成功！");
        }

        private void bbicontractmemo_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcsys.SelectedTabPage == xtpcontract)
                dr = xgvcontract.GetFocusedDataRow();
            else if (xtcsys.SelectedTabPage == xtpinfoquery)
                dr = xgvinfoqueryct.GetFocusedDataRow();

            if (dr == null) return;

            frmContractMemo fcm = new frmContractMemo();
            fcm.dr = dr;

            fcm.ShowDialog();
        }

        private void tminform_Tick(object sender, EventArgs e)
        {
            createinform();
            tminform.Enabled = false;
        }

        private void initrptfncpp()
        {
            
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取查询树......");

            this.tlrptfnc.Nodes.Clear();

            this.tlrptfnc.DataSource = mydb.getppareatree();
            this.tlrptfnc.Columns["ppname"].Caption = "经开物业";

            XScmWF.CloseWaitForm();

            this.xtcsys.SelectedTabPage = xtprptfnc;
            

            foreach (DevExpress.XtraTreeList.Columns.TreeListColumn tlc in tlrptfnc.Columns)
            {
                tlc.OptionsColumn.AllowSort = false;
            }

            if (xderptfncfeemon.Text == "") xderptfncfeemon.DateTime = System.DateTime.Now;
        }

        private void bbifeepayedrptfnc_ItemClick(object sender, ItemClickEventArgs e)
        {
            // initrptfncpp();

            xtcrptfnc_SelectedPageChanged(null, null);
        }

        private void tlrptfnc_Click(object sender, EventArgs e)
        {
            Point point = tlrptfnc.PointToClient(Cursor.Position);
            DevExpress.XtraTreeList.TreeListHitInfo hitInfo = tlrptfnc.CalcHitInfo(point);


            //
            if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Button)
            {
                return;
            }

            DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = this.tlrptfnc.FocusedNode;
            if (clickedNode.Level < 0)
            {
                return;
            }

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取统计数据...");


            string sid = clickedNode["id"].ToString();

             if (xtcrptfnc.SelectedTabPage == xtprptfncpp)
            {

                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Column)
                {
                    xlbrptpparea.Text = "经开物业";
                    xgcrptppnum.DataSource = mydb.getrptfncpp();

                }
                else
                {
                    xlbrptpparea.Text = clickedNode["ppname"].ToString(); ;
                    xgcrptppnum.DataSource = mydb.getrptfncppbyarea(sid);
                }

            }
            else if (xtcrptfnc.SelectedTabPage == xtprptfncfee)
            {
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Column)
                {
                    
                   // xgcrptppnum.DataSource = mydb.getrptfncfee();

                }
                else if (clickedNode.Level == 1)
                {


                    //xgcrptfncfee.DataSource = mydb.getrptfncfeebyct(sid, xderptfncfeemon.Text);
                }
                else if (clickedNode.Level == 0)
                {
                    if (xderptfncfeemon.Text == "")
                    {
                        showopermsg("X", "[租金统计] 请先选统计月份!");

                        return;
                    }

                    xgcrptfncfee.DataSource = mydb.getrptfncfeebyarea(sid, xderptfncfeemon.Text);

                    xgvrptfncfee.Columns["unitno"].Width = 120;
                    xgvrptfncfee.Columns["cusname"].BestFit();
                }
            }
            else if (xtcrptfnc.SelectedTabPage == xtprptfncmonfee)
            {
                if (hitInfo.HitInfoType == DevExpress.XtraTreeList.HitInfoType.Column)
                {

                    // xgcrptppnum.DataSource = mydb.getrptfncfee();

                }
                else if (clickedNode.Level == 0)
                {
                    xgcrptfncmonfee.DataSource = mydb.getrptfncmonfeebyarea(sid, derptmon.Text);
                }
            }


            XScmWF.CloseWaitForm();
           
        }

        private void xsbexprptpptoxls_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvrptppnum);
        }

        private void xtprptfncfee_TabIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void initrptfncfee()
        {

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取查询树......");

            this.tlrptfnc.Nodes.Clear();

            this.tlrptfnc.DataSource = mydb.getppareatree();
            this.tlrptfnc.Columns["ppname"].Caption = "经开物业";

            foreach (DevExpress.XtraTreeList.Nodes.TreeListNode rn in this.tlrptfnc.Nodes)
            {
                string s1 = rn["id"].ToString();
                DataTable d123 = mydb.getcontractbyareafnc(s1);

                for (int ii = 0; ii < d123.Rows.Count; ii++)
                {
                    tlrptfnc.AppendNode(new Object[] { d123.Rows[ii]["contractid"].ToString(), 
                                                  d123.Rows[ii]["contractid"].ToString(), 
                                                  d123.Rows[ii]["ctno_cus"].ToString() }, rn);
                }

            }


            XScmWF.CloseWaitForm();

            this.xtcsys.SelectedTabPage = xtprptfnc;

            if (xderptfncfeemon.EditValue == null) xderptfncfeemon.DateTime = DateTime.Now;


            foreach (DevExpress.XtraTreeList.Columns.TreeListColumn tlc in tlrptfnc.Columns)
            {
                tlc.OptionsColumn.AllowSort = false;
            }
        }

        private void xtcrptfnc_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtcrptfnc.SelectedTabPage == xtprptfncpp)
            {
                initrptfncpp();
            }
            else if (xtcrptfnc.SelectedTabPage == xtprptfncfee)
            {
                //initrptfncfee();
                initrptfncpp();
            }
            else if (xtcrptfnc.SelectedTabPage == xtprptfncmonfee)
            {
                initrptfncpp();

            }
        }

        private void xderptfncfeemon_Popup(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.DateEdit edit = sender as DevExpress.XtraEditors.DateEdit;
            DevExpress.XtraEditors.Popup.PopupDateEditForm form =
                (edit as DevExpress.Utils.Win.IPopupControl).PopupWindow as DevExpress.XtraEditors.Popup.PopupDateEditForm;
            form.Calendar.View = DevExpress.XtraEditors.Controls.DateEditCalendarViewType.YearInfo;
        }

        private void xsbexprptfeetoxls_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvrptfncfee);
        }

        private void xschec_Click(object sender, EventArgs e)
        {
            
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            xschec.ActiveViewType = SchedulerViewType.Month;
        }

        private void xsbfeemgtquery_Click(object sender, EventArgs e)
        {
            string squery = "select b.*,(b.feepay-b.feepayed) as owedfee,c.cusname,d.units from (select a.contractid,a.contractno,feepaysdt,feepayedt,a.cusid, " +
                       " sum(feepay) as feepay, sum(ifnull(feepayed,0)) as feepayed" +
                       " from t_fee_pay_mgt_period a where 1=1 ";

            if (xdemgtfeequerysdts.EditValue != null)
            {

                squery += " and a.feepaysdt>='" + xdemgtfeequerysdts.Text.ToString() + "'";
            }
            if (xdemgtfeequerysdte.EditValue != null)
            {
                squery += " and a.feepaysdt<='" + xdemgtfeequerysdte.Text.ToString() + "'";
            }
            
            squery+=    " and  a.contractid in (select distinct contractid from t_contract where 1=1 ";

            if (xccbmgtfeequeryarea.Text != "")
            {
                //squery += " and contractarea='" + xluectareaquery.EditValue.ToString() + "'";


                string sss = " and contractarea in ('0'";
                for (int ii = 0; ii < xccbmgtfeequeryarea.Properties.Items.Count; ii++)
                {
                    if (xccbmgtfeequeryarea.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbmgtfeequeryarea.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;
            }

            if (xccbmgtfeequeryctstatus.Text != "")
            {
                //squery += " and contractstatus='" + xluecontractstatus.EditValue.ToString() + "'";

                string sss = " and contractstatus in ('0'";
                for (int ii = 0; ii < xccbmgtfeequeryctstatus.Properties.Items.Count; ii++)
                {
                    if (xccbmgtfeequeryctstatus.Properties.Items[ii].CheckState == CheckState.Checked)
                    {
                        sss += ",'" + xccbmgtfeequeryctstatus.Properties.Items[ii].Value.ToString() + "'";
                    }
                }
                sss += ") ";

                squery += sss;

            }

            if (xluemgtfeequeryctsrc.EditValue != null)
            {
                squery += " and contractorg='" + xluemgtfeequeryctsrc.EditValue.ToString() + "'";
            }


            if (xdemgtfeequeryctsdts.EditValue != null)
            {
                squery += " and ContractSDT>='" + xdemgtfeequeryctsdts.Text.ToString() + "'";
            }

            if (xdemgtfeequeryctsdte.EditValue != null)
            {
                squery += " and ContractSDT<='" + xdemgtfeequeryctsdte.Text.ToString() + "'";
            }

            if (xdemgtfeequeryctedts.EditValue != null)
            {
                squery += " and ContractEDT>='" + xdemgtfeequeryctedts.Text.ToString() + "'";
            }

            if (xdemgtfeequeryctedte.EditValue != null)
            {
                squery += " and ContractEDT<='" + xdemgtfeequeryctedte.Text.ToString() + "'";
            }

            //if (xdecontractsignsdt.EditValue != null)
            //{
            //    squery += " and signdt>='" + xdecontractsignsdt.Text.ToString() + "'";
            //}
            //if (xdecontractsignedt.EditValue != null)
            //{
            //    squery += " and signdt<='" + xdecontractsignedt.Text.ToString() + "'";
            //}

            squery += ") group by a.contractid,a.feepaysdt,a.feepayedt ) b join t_cus c on b.cusid=c.cusid " +
                      " join (select contractid,feepaysdt,group_concat(unitno SEPARATOR ',') as units from t_fee_pay_mgt_period" +
                      " group by contractid,feepaysdt) d on b.contractid=d.contractid and b.feepaysdt=d.feepaysdt ";

            if (xrgmgtfeequeryowe.SelectedIndex==0)
            {
                squery += " having owedfee>0 ";
            }

             squery+= " order by b.cusid,b.contractno,b.feepaysdt";

             xgcmgtfeequery.DataSource = mydb.gettablebystr(squery);

            //xgvcontract.BestFitColumns();

            

        }

        private void initmgtfeequeryform()
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在获取合同基础数据......");

            if (xccbmgtfeequeryarea.Properties.DataSource == null)
            {



                xccbmgtfeequeryctstatus.Properties.DataSource = mydb.getparacode("contractstatus");

                xluemgtfeequeryctsrc.Properties.DataSource = mydb.getparacode("contractorg");


                xccbmgtfeequeryarea.Properties.DataSource = mydb.getpparea();

                
            }

            this.xtcsys.SelectedTabPage = xtpmgtrpt;

            XScmWF.CloseWaitForm();
        }

        private void bbimgtfeequery_ItemClick(object sender, ItemClickEventArgs e)
        {
            initmgtfeequeryform();
        }

        private void xsbexpmgtfeequerytoxls_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvmgtfeequery);
        }

        private void xsbcontractfeepaysave_Click(object sender, EventArgs e)
        {
            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在保存修改后的应收数据......");

            mydb.savefncfeechange(xgvpp_feepayed);


            XScmWF.CloseWaitForm();
        }

        private void xsbcontractfeepayaddmon_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvpp_feepayed.GetFocusedDataRow();

            if (dr == null)
            {
                showopermsg("X", "[增加一个月应收] 请先选中该房间目前最后一个月记录!");

                return;
            }

            string smon = dr["feemonth"].ToString();

            System.DateTime dt_new = DateTime.ParseExact(smon, "yyyyMM", new CultureInfo("zh-CN", true));
            dt_new = dt_new.AddMonths(1);

            if (MessageBox.Show("确定增加房间["+dr["unitno"].ToString()+"] ["+dt_new.ToString("yyyyMM")+"] 应收吗？", "提示",
                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                                           MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
            {
                return;
            }

            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("输入应收金额",
                                                                                 "[" + dr["unitno"].ToString() +"] "+
                                                                                "[" +dt_new.ToString("yyyyMM") + "]",
                                                                                "", -1, -1);

            if (sss1 == "") return;

            try
            {
                Convert.ToDouble(sss1);
            }
            catch
            {
                showopermsg("X", "[增加一个月应收] 输入的应收有非法数字!");
                return;
            }

            int ii= mydb.addfeepaymgtbyfnc(dr, sss1, dt_new.ToString("yyyyMM"));

            if (ii == 0) { showopermsg("X", "[增加一个月应收] 已存在该月应收记录!"); }
            else
            {
                tlfnc_Click(null, null);
                //DevExpress.XtraTreeList.Nodes.TreeListNode clickedNode = tlfnc.FocusedNode;
                //if (clickedNode.Level == 0) return;

                //XScmWF.ShowWaitForm();
                //XScmWF.SetWaitFormCaption("正在获取合同应收...");


                //string sctid = clickedNode["id"].ToString();

                //xgcct_feepayed.DataSource = mydb.getcontractfeepayed(sctid, xdefncpaymon.Text,
                //                                                     xcefncpayfeeshowpayed.Checked);


                //XScmWF.CloseWaitForm();

                //xgvct_feepayed_DataSourceChanged(null, null);
            }


        }

        private void xdefncpaymon_EditValueChanged(object sender, EventArgs e)
        {
            tlfnc_Click(null, null);
        }

        private void xcefncpayfeeshowpayed_CheckedChanged(object sender, EventArgs e)
        {
            tlfnc_Click(null, null);
        }

        private void xsbfeepayautosplit_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show("确定将合同的所有房间应收费用进行首尾月拆分吗（针对半月合同)？", "提示",
                                           MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                                           MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
            {
                return;
            }

            XScmWF.ShowWaitForm();
            XScmWF.SetWaitFormCaption("正在拆分应收数据......");

            if (!mydb.splitfeepaymonfnc(xgvpp_feepayed.GetDataRow(0)))
            {
                showopermsg("X", "[应收拆分] 无需拆分!");
            }



            XScmWF.CloseWaitForm();

            tlfnc_Click(null, null);
            
        }

        private void xsbfncrptmonfee_Click(object sender, EventArgs e)
        {
            mydb.exportgvtoxls(xgvrptfncmonfee);
        }

        private void xderptfncfeemon_EditValueChanged(object sender, EventArgs e)
        {
            tlrptfnc_Click(null, null);
        }

        private void derptmon_EditValueChanged(object sender, EventArgs e)
        {
            tlrptfnc_Click(null, null);
        }

        private void bbifeepayedconfirmfnccancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = xgvct_payedfeeseq.GetFocusedDataRow();
            if (dr == null) return;

            if (dr == null)
            {
                showopermsg("X", "[收费复核] 请选择一条收费记录!");
                return;
            }


            DialogResult dr1 = MessageBox.Show("确定要取消复核该收费记录吗？", "确认取消复核", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (dr1 == DialogResult.Cancel) return;

            mydb.feepayedconfirmedfnccancel(dr);

            initfeepayedseqform();

            showopermsg("Y", "<取消复核收费记录> 操作成功！");
        }

        private void bbicontractcancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = null;

            if (xtcsys.SelectedTabPage == xtpcontract)
            {
                if (xgvcontract.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[提交合同审核] 请一次只选一个合同提交审核！");
                    return;
                }

                dr = xgvcontract.GetFocusedDataRow();
                if (dr == null)
                {
                    showopermsg("X", "[提交合同审核] 请先选择一个合同！");
                    return;
                }
            }

            if (xtcsys.SelectedTabPage == xtpinfoquery &&
                xtcinfoquery.SelectedTabPage == xtpcontractinfo)
            {
                if (xgvinfoqueryct.SelectedRowsCount > 1)
                {
                    showopermsg("X", "[提交合同审核] 请一次只选一个合同提交审核！");
                    return;
                }

                dr = xgvinfoqueryct.GetFocusedDataRow();
                if (dr == null)
                {
                    showopermsg("X", "[提交合同审核] 请先选择一个合同！");
                    return;
                }
            }

            //状态检查
            string soper = "";
            if (!mydb.checkoperconditions(ref soper, "op_check_contract_cancel", dr))
            {
                showopermsg("X", soper);
                return;
            }

            frmCTCancelPrev fctcp = new frmCTCancelPrev();
            fctcp.XScmWF = this.XScmWF;

            fctcp.xgcct_ppunit.DataSource = mydb.getppunitbycontract(dr["contractid"].ToString());


            fctcp.drcontract = dr;
            fctcp.sfncmon = mydb.getmaxfncpaymon(dr);

            fctcp.ShowDialog();

            xgvcontract_FocusedRowChanged(null, null);

        }

        private void bbirptmon_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = xgvct_payedfeeseq.GetFocusedDataRow();
            if (dr == null) return;

            if (dr == null)
            {
                showopermsg("X", "[更改归属月] 请选择一条收费记录!");
                return;
            }

            if (!Convert.IsDBNull(dr["rptmon"]))
            {

                DialogResult dr1 = MessageBox.Show("该收费记录已经存在归属月，确认要修改吗？", "确认更改归属月", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr1 == DialogResult.Cancel) return;
            }

            //输入该笔收费对应的月份
            string sss1 = Microsoft.VisualBasic.Interaction.InputBox("输入收费所属月份",
                                                          "输入月份（用于统计月度收费）",
                                                          DateTime.Now.ToString("yyyyMM"), -1, -1);


            try
            {
                System.DateTime datet = DateTime.ParseExact(sss1, "yyyyMM", new CultureInfo("zh-CN", true));

            }
            catch
            {
                showopermsg("X", "[保存收费数据] 输入的所属月份有误！");
                return;
            }

            mydb.changerptmon(dr,sss1);

            initfeepayedseqform();

            showopermsg("Y", "<更改归属月> 操作成功！");
        }



    }

    public class ChsGridLocalizer : GridLocalizer
    {

        public override string GetLocalizedString(GridStringId id)
        {
            switch (id)
            {
                case GridStringId.FilterBuilderCancelButton:
                    return "取消";

                case GridStringId.FilterBuilderCaption:
                    return "修改筛选条件";

                case GridStringId.FilterBuilderOkButton:
                    return "确定";

                case GridStringId.FilterPanelCustomizeButton:
                    return "筛选...";

                case GridStringId.FilterBuilderApplyButton:
                    return "应用";

                case GridStringId.FileIsNotFoundError:
                    return "文件{0}没有找到";

                case GridStringId.ColumnViewExceptionMessage:
                    return "是否确定修改？";

                case GridStringId.CustomizationCaption:
                    return "自定义显示字段";

                case GridStringId.CustomizationColumns:
                    return "列";

                case GridStringId.CustomizationBands:
                    return "分区";

                case GridStringId.PopupFilterAll:
                    return "(所有)";

                case GridStringId.PopupFilterCustom:
                    return "(自定义)";

                case GridStringId.PopupFilterBlanks:
                    return "(空值)";

                case GridStringId.PopupFilterNonBlanks:
                    return "(非空值)";

                case GridStringId.MenuGroupPanelHide:
                    return "隐藏分组面板";

                case GridStringId.MenuGroupPanelShow: 
                    return "显示分组面板";

                case GridStringId.CustomFilterDialogHint:
                    return "类似条件可以使用通配符：\r\n_  代表任意字符\r\n% 代表任何字符串";

                case GridStringId.CustomFilterDialogFormCaption:
                    return "自定义筛选条件";

                case GridStringId.CustomFilterDialogCaption:
                    return "显示符合下列条件的记录:";

                case GridStringId.CustomFilterDialogEmptyOperator:
                    return "(选择一个条件)";

                case GridStringId.CustomFilterDialogEmptyValue: 
                    return "(输入一个值)";

                case GridStringId.CustomFilterDialogRadioAnd:
                    return "并且";

                case GridStringId.CustomFilterDialogRadioOr:
                    return "或者";

                case GridStringId.CustomFilterDialogOkButton:
                    return "确定(&O)";

                case GridStringId.CustomFilterDialogClearFilter:
                    return "清除筛选条件(&L)";

                case GridStringId.CustomFilterDialog2FieldCheck:
                    return "字段";

                case GridStringId.CustomFilterDialogCancelButton:
                    return "取消(&C)";

                case GridStringId.MenuFooterSum:
                    return "合计";

                case GridStringId.MenuFooterMin:
                    return "最小";

                case GridStringId.MenuFooterMax:
                    return "最大";

                case GridStringId.MenuFooterCount:
                    return "计数";

                case GridStringId.MenuFooterAverage:
                    return "平均";

                case GridStringId.MenuFooterNone:
                    return "空";

                case GridStringId.MenuFooterSumFormat:
                    return "合计={0:#.##}";

                case GridStringId.MenuFooterMinFormat:
                    return "最小={0}";

                case GridStringId.MenuFooterMaxFormat:
                    return "最大={0}";

                case GridStringId.MenuFooterCountFormat:
                    return "{0}";

                case GridStringId.MenuFooterAverageFormat:
                    return "平均={0:#.##}";

                case GridStringId.MenuColumnSortAscending:
                    return "升序排序";

                case GridStringId.MenuColumnSortDescending:
                    return "降序排序";

                case GridStringId.MenuColumnGroup:
                    return "按此列分组";

                case GridStringId.MenuColumnUnGroup:
                    return "取消分组";

                case GridStringId.MenuColumnColumnCustomization:
                    return "自定义显示列...";

                case GridStringId.MenuColumnFilterEditor :
                    return "数据筛选条件...";

                case GridStringId.MenuColumnBestFit:
                    return "本字段宽度适应内容";

                case GridStringId.MenuColumnFindFilterHide:
                    return "隐藏查找面板" ;

                case GridStringId.MenuColumnFindFilterShow :
                    return "显示查找面板";

                case GridStringId.MenuColumnAutoFilterRowHide :
                    return "隐藏自动筛选器行";

                case GridStringId.MenuColumnAutoFilterRowShow: 
                    return "显示自动筛选器行";
                                        
                case GridStringId.FindControlClearButton : 
                    return "清除";

                case GridStringId.FindControlFindButton: 
                    return "查找";

                case GridStringId.MenuColumnFilter:
                    return "筛选";

                case GridStringId.MenuColumnRemoveColumn:
                    return "不显示本列";

                case GridStringId.MenuColumnClearFilter:
                    return "清除筛选条件";

                case GridStringId.MenuColumnBestFitAllColumns:
                    return "所有字段宽度适应内容";

                case GridStringId.MenuGroupPanelFullExpand:
                    return "展开全部分组";

                case GridStringId.MenuGroupPanelFullCollapse:
                    return "收缩全部分组";

                case GridStringId.MenuGroupPanelClearGrouping:
                    return "清除所有分组";

                case GridStringId.PrintDesignerGridView:
                    return "打印设置(表格模式)";

                case GridStringId.PrintDesignerCardView:
                    return "打印设置(卡片模式)";

                case GridStringId.PrintDesignerBandedView:
                    return "打印设置(区域模式)";

                case GridStringId.PrintDesignerBandHeader:
                    return "区标题";

                case GridStringId.MenuColumnGroupBox:
                    return "显示/隐藏分组区";

                case GridStringId.CardViewNewCard:
                    return "新卡片";

                case GridStringId.CardViewQuickCustomizationButton:
                    return "自定义格式";

                case GridStringId.CardViewQuickCustomizationButtonFilter:
                    return "筛选";

                case GridStringId.CardViewQuickCustomizationButtonSort:
                    return "排序:";

                case GridStringId.GridGroupPanelText:
                    return "";

                case GridStringId.GridNewRowText:
                    return "新增资料";

                case GridStringId.GridOutlookIntervals:
                    return "一个月以上;上个月;三周前;两周前;上周;;;;;;;;昨天;今天;明天; ;;;;;;;下周;两周后;三周后;下个月;一个月之后;";

                case GridStringId.PrintDesignerDescription:
                    return "为当前视图设置不同的打印选项.";

                case GridStringId.MenuFooterCustomFormat:
                    return "自定={0}";

                case GridStringId.MenuFooterCountGroupFormat:
                    return "计数={0}";

                case GridStringId.MenuColumnClearSorting:
                    return "清除排序";
            }
            
            return base.GetLocalizedString(id);
            
        }
    }

    //XtraEditors
    public class ChsXtraEditorsLocalizer : Localizer
    {
        public override string GetLocalizedString(StringId id)
        {
            switch (id)
            {
                case StringId.Apply: return "应用";
                case StringId.CalcButtonBack: return "后退";
                //case StringId.CalcButtonC: return "C";
                //case StringId.CalcButtonCE: return "CE";
                //case StringId.CalcButtonMC: return "MC";
                //case StringId.CalcButtonMR: return "MR";
                //case StringId.CalcButtonMS: return "MS";
                //case StringId.CalcButtonMx: return "M+";
                //case StringId.CalcButtonSqrt: return "平方根";
                //case StringId.CalcError: return "计算错误";
                case StringId.Cancel: return "取消";
                case StringId.CaptionError: return "错误";
                case StringId.CheckChecked: return "已经选取";
                case StringId.CheckIndeterminate: return "不确定";
                case StringId.CheckUnchecked: return "非选取";
                case StringId.ColorTabCustom: return "自定义";
                case StringId.ColorTabSystem: return "系统";
                case StringId.ColorTabWeb: return "网页";
                case StringId.ContainerAccessibleEditName: return "编辑控件";
                case StringId.DataEmpty: return "没有图像数据";
                case StringId.DateEditClear: return "清除";
                case StringId.DateEditToday: return "今天";
                case StringId.DefaultBooleanDefault: return "默认";
                case StringId.DefaultBooleanFalse: return "假";
                case StringId.DefaultBooleanTrue: return "真";
                case StringId.FieldListName: return "字段列表 ({0})";
                case StringId.FilterAggregateAvg: return "平均";
                case StringId.FilterAggregateCount: return "计数";
                case StringId.FilterAggregateExists: return "存在";
                case StringId.FilterAggregateMax: return "最大值";
                case StringId.FilterAggregateMin: return "最小值";
                case StringId.FilterAggregateSum: return "求和";
                case StringId.FilterClauseAnyOf: return "是下列任一项";
                case StringId.FilterClauseBeginsWith: return "开头是";
                case StringId.FilterClauseBetween: return "介于";
                case StringId.FilterClauseBetweenAnd: return "和";
                case StringId.FilterClauseContains: return "包含";
                case StringId.FilterClauseDoesNotContain: return "不包含";
                case StringId.FilterClauseDoesNotEqual: return "不等于";
                case StringId.FilterClauseEndsWith: return "结尾是";
                case StringId.FilterClauseEquals: return "等于";
                case StringId.FilterClauseGreater: return "大于";
                case StringId.FilterClauseGreaterOrEqual: return "大于或等于";
                case StringId.FilterClauseIsNotNull: return "不为空";
                case StringId.FilterClauseIsNotNullOrEmpty: return "不为空白";
                case StringId.FilterClauseIsNull: return "为空";
                case StringId.FilterClauseIsNullOrEmpty: return "为空白";
                case StringId.FilterClauseLess: return "小于";
                case StringId.FilterClauseLessOrEqual: return "小于或等于";
                case StringId.FilterClauseLike: return "类似于";
                case StringId.FilterClauseNoneOf: return "不是下列任一项";
                case StringId.FilterClauseNotBetween: return "不介于";
                case StringId.FilterClauseNotLike: return "不类似于";

                case StringId.FilterCriteriaInvalidExpression: return "指定的表达式包含无效的符号（行 {0}，字符 {1}）。";
                case StringId.FilterCriteriaInvalidExpressionEx: return "指定的表达式是无效的。";
                case StringId.FilterCriteriaToStringBetween: return "介于";
                    
                //case StringId.FilterCriteriaToStringBinaryOperatorBitwiseAnd: return "&";
                //case StringId.FilterCriteriaToStringBinaryOperatorBitwiseOr: return "'";
                //case StringId.FilterCriteriaToStringBinaryOperatorBitwiseXor: return "^";
                //case StringId.FilterCriteriaToStringBinaryOperatorDivide: return "/";
                case StringId.FilterCriteriaToStringBinaryOperatorEqual: return "等于";
                case StringId.FilterCriteriaToStringBinaryOperatorGreater: return "大于";
                case StringId.FilterCriteriaToStringBinaryOperatorGreaterOrEqual: return "大于等于";
                case StringId.FilterCriteriaToStringBinaryOperatorLess: return "小于";
                case StringId.FilterCriteriaToStringBinaryOperatorLessOrEqual: return "小于等于";
                case StringId.FilterCriteriaToStringBinaryOperatorLike: return "类似于";

                //case StringId.FilterCriteriaToStringBinaryOperatorMinus: return "-";
                //case StringId.FilterCriteriaToStringBinaryOperatorModulo: return "%";
                //case StringId.FilterCriteriaToStringBinaryOperatorMultiply: return "*";
                //case StringId.FilterCriteriaToStringBinaryOperatorNotEqual: return "<> ";
                //case StringId.FilterCriteriaToStringBinaryOperatorPlus: return "+";
                //case StringId.FilterCriteriaToStringFunctionAbs: return "Abs";
                //case StringId.FilterCriteriaToStringFunctionAcos: return "Acos";
                //case StringId.FilterCriteriaToStringFunctionAddDays: return "添加天";
                //case StringId.FilterCriteriaToStringFunctionAddHours: return "添加小时";
                //case StringId.FilterCriteriaToStringFunctionAddMilliSeconds: return "添加毫秒";
                //case StringId.FilterCriteriaToStringFunctionAddMinutes: return "添加分钟";
                //case StringId.FilterCriteriaToStringFunctionAddMonths: return "添加月";
                //case StringId.FilterCriteriaToStringFunctionAddSeconds: return "增加秒";
                //case StringId.FilterCriteriaToStringFunctionAddTicks: return "添加ticks";
                //case StringId.FilterCriteriaToStringFunctionAddTimeSpan: return "添加时间段";
                //case StringId.FilterCriteriaToStringFunctionAddYears: return "添加年";
                //case StringId.FilterCriteriaToStringFunctionAscii: return "Ascii";
                //case StringId.FilterCriteriaToStringFunctionAsin: return "Asin";
                //case StringId.FilterCriteriaToStringFunctionAtn: return "Atn";
                //case StringId.FilterCriteriaToStringFunctionAtn2: return "Atn2";
                //case StringId.FilterCriteriaToStringFunctionBigMul: return "大数积";
                //case StringId.FilterCriteriaToStringFunctionCeiling: return "ceiling";
                //case StringId.FilterCriteriaToStringFunctionChar: return "字符";
                //case StringId.FilterCriteriaToStringFunctionCharIndex: return "字符索引";
                //case StringId.FilterCriteriaToStringFunctionConcat: return "合并字符";
                case StringId.FilterCriteriaToStringFunctionContains: return "包含";
                //case StringId.FilterCriteriaToStringFunctionCos: return "Cos";
                //case StringId.FilterCriteriaToStringFunctionCosh: return "Cosh";
                //case StringId.FilterCriteriaToStringFunctionCustom: return "自定义";
                //case StringId.FilterCriteriaToStringFunctionCustomNonDeterministic: return "非确定性的自定义";
                //case StringId.FilterCriteriaToStringFunctionDateDiffDay: return "日期比较-天";
                //case StringId.FilterCriteriaToStringFunctionDateDiffHour: return "日期比较-小时";
                //case StringId.FilterCriteriaToStringFunctionDateDiffMilliSecond: return "日期比较-毫秒";
                //case StringId.FilterCriteriaToStringFunctionDateDiffMinute: return "日期比较-分钟";
                //case StringId.FilterCriteriaToStringFunctionDateDiffMonth: return "日期比较-月";
                //case StringId.FilterCriteriaToStringFunctionDateDiffSecond: return "日期比较-秒";
                //case StringId.FilterCriteriaToStringFunctionDateDiffTick: return "日期比较-ticks";
                //case StringId.FilterCriteriaToStringFunctionDateDiffYear: return "日期比较-年";
                case StringId.FilterCriteriaToStringFunctionEndsWith: return "结尾是";
                //case StringId.FilterCriteriaToStringFunctionExp: return "Exp";
                //case StringId.FilterCriteriaToStringFunctionFloor: return "floor";
                //case StringId.FilterCriteriaToStringFunctionGetDate: return "获取日期";
                //case StringId.FilterCriteriaToStringFunctionGetDay: return "获取天";
                //case StringId.FilterCriteriaToStringFunctionGetDayOfWeek: return "获取星期几";
                //case StringId.FilterCriteriaToStringFunctionGetDayOfYear: return "得到一年的天";
                //case StringId.FilterCriteriaToStringFunctionGetHour: return "获取小时";
                //case StringId.FilterCriteriaToStringFunctionGetMilliSecond: return "获取毫秒";
                //case StringId.FilterCriteriaToStringFunctionGetMinute: return "获取分钟";
                //case StringId.FilterCriteriaToStringFunctionGetMonth: return "获取月";
                //case StringId.FilterCriteriaToStringFunctionGetSecond: return "获取秒";
                //case StringId.FilterCriteriaToStringFunctionGetTimeOfDay: return "获取时间";
                //case StringId.FilterCriteriaToStringFunctionGetYear: return "获取年";
                //case StringId.FilterCriteriaToStringFunctionIif: return "如果";
                //case StringId.FilterCriteriaToStringFunctionInsert: return "插入";
                case StringId.FilterCriteriaToStringFunctionIsNull: return "为空";
                case StringId.FilterCriteriaToStringFunctionIsNullOrEmpty: return "为空白";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalBeyondThisYear: return "超出了本年度";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisMonth: return "本月早些时候";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisWeek: return "本周早些时候";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalEarlierThisYear: return "本年早些时候";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLastWeek: return "是否最后一个星期";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisMonth: return "本月晚些时候";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisWeek: return "本周晚些时候";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalLaterThisYear: return "本年晚些时候";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalNextWeek: return "下周";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalPriorThisYear: return "先于本年";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalToday: return "是否今天";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalTomorrow: return "是否明天";
                //case StringId.FilterCriteriaToStringFunctionIsOutlookIntervalYesterday: return "是否昨天";
                //case StringId.FilterCriteriaToStringFunctionLen: return "Len";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeDayAfterTomorrow: return "后天";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeLastWeek: return "最后一个星期";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeNextMonth: return "下个月";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeNextWeek: return "下个星期";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeNextYear: return "明年";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeNow: return "现在";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeThisMonth: return "本月";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeThisWeek: return "本周";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeThisYear: return "本年";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeToday: return "今天";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeTomorrow: return "明天";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeTwoWeeksAway: return "两周了";
                //case StringId.FilterCriteriaToStringFunctionLocalDateTimeYesterday: return "昨天";
                //case StringId.FilterCriteriaToStringFunctionLog: return "Log";
                //case StringId.FilterCriteriaToStringFunctionLog10: return "Log10";
                //case StringId.FilterCriteriaToStringFunctionLower: return "较低";
                //case StringId.FilterCriteriaToStringFunctionMax: return "最大值";
                //case StringId.FilterCriteriaToStringFunctionMin: return "最小值";
                //case StringId.FilterCriteriaToStringFunctionNone: return "无";
                //case StringId.FilterCriteriaToStringFunctionNow: return "现在";
                //case StringId.FilterCriteriaToStringFunctionPadLeft: return "左填充";
                //case StringId.FilterCriteriaToStringFunctionPadRight: return "右填充"; 
                case StringId.FilterCriteriaToStringFunctionPower: return "电源";
                case StringId.FilterCriteriaToStringFunctionRemove: return "删除";
                case StringId.FilterCriteriaToStringFunctionReplace: return "替换";
                case StringId.FilterCriteriaToStringFunctionReverse: return "反向";
                case StringId.FilterCriteriaToStringFunctionRnd: return "Rnd";
                case StringId.FilterCriteriaToStringFunctionRound: return "Round";
                case StringId.FilterCriteriaToStringFunctionSign: return "Sign";
                case StringId.FilterCriteriaToStringFunctionSin: return "Sin";
                case StringId.FilterCriteriaToStringFunctionSinh: return "Sinh";
                case StringId.FilterCriteriaToStringFunctionSqr: return "Sqr";
                case StringId.FilterCriteriaToStringFunctionStartsWith: return "开头是";
                case StringId.FilterCriteriaToStringFunctionSubstring: return "子字符串";
                case StringId.FilterCriteriaToStringFunctionTan: return "Tan";
                case StringId.FilterCriteriaToStringFunctionTanh: return "Tanh";
                case StringId.FilterCriteriaToStringFunctionToday: return "今天";
                case StringId.FilterCriteriaToStringFunctionToDecimal: return "To decimal";
                case StringId.FilterCriteriaToStringFunctionToDouble: return "To double";
                case StringId.FilterCriteriaToStringFunctionToFloat: return "To float";
                case StringId.FilterCriteriaToStringFunctionToInt: return "To int";
                case StringId.FilterCriteriaToStringFunctionToLong: return "To long";
                case StringId.FilterCriteriaToStringFunctionToStr: return "To str";
                case StringId.FilterCriteriaToStringFunctionTrim: return "Trim";
                case StringId.FilterCriteriaToStringFunctionUpper: return "Upper";
                case StringId.FilterCriteriaToStringFunctionUtcNow: return "Utc now";
                case StringId.FilterCriteriaToStringGroupOperatorAnd: return "且";
                case StringId.FilterCriteriaToStringGroupOperatorOr: return "或";
                case StringId.FilterCriteriaToStringIn: return "存在于";
                case StringId.FilterCriteriaToStringIsNotNull: return "不为空";
                case StringId.FilterCriteriaToStringNotLike: return "不类似于";
                case StringId.FilterCriteriaToStringUnaryOperatorBitwiseNot: return "~";
                case StringId.FilterCriteriaToStringUnaryOperatorIsNull: return "为空";
                case StringId.FilterCriteriaToStringUnaryOperatorMinus: return "-";
                case StringId.FilterCriteriaToStringUnaryOperatorNot: return "不";
                case StringId.FilterCriteriaToStringUnaryOperatorPlus: return "+";
                case StringId.FilterDateTimeConstantMenuCaption: return "日期时间常数";
                case StringId.FilterDateTimeOperatorMenuCaption: return "日期时间操作";
                case StringId.FilterEditorTabText: return "文本";
                case StringId.FilterEditorTabVisual: return "可见";
                case StringId.FilterEmptyEnter: return "< 输入值 >";
                case StringId.FilterEmptyParameter: return "< 输入参数 >";
                case StringId.FilterEmptyValue: return "<空值>";
                case StringId.FilterFunctionsMenuCaption: return "";
                case StringId.FilterGroupAnd: return "且";
                case StringId.FilterGroupNotAnd: return "不&且";
                case StringId.FilterGroupNotOr: return "不&或";
                case StringId.FilterGroupOr: return "或";
                case StringId.FilterMenuAddNewParameter: return "添加一个新的参数...";
                case StringId.FilterMenuClearAll: return "全部清除";
                case StringId.FilterMenuConditionAdd: return "添加条件";
                case StringId.FilterMenuGroupAdd: return "添加组";
                case StringId.FilterMenuRowRemove: return "删除行";
                case StringId.FilterShowAll: return "（选择所有）";
                case StringId.FilterToolTipElementAdd: return "将新项添加到列表中";
                case StringId.FilterToolTipKeysAdd: return "（使用插入(Ins)键）";
                case StringId.FilterToolTipKeysRemove: return "（使用删除(Del)键）";
                case StringId.FilterToolTipNodeAction: return "动作";
                case StringId.FilterToolTipNodeAdd: return "向该组添加一个新的条件";
                case StringId.FilterToolTipNodeRemove: return "删除此条件";
                case StringId.FilterToolTipValueType: return "比较值 / 另一个字段的值";
                case StringId.ImagePopupEmpty: return "(空)";
                case StringId.ImagePopupPicture: return "(图像)";
                case StringId.InvalidValueText: return "无效值";
                case StringId.LookUpColumnDefaultName: return "缺省名称";
                case StringId.LookUpEditValueIsNull: return "[编辑值为空]";
                case StringId.LookUpInvalidEditValueType: return "无效的 LookUpEdit 编辑值类型。";
                case StringId.MaskBoxValidateError: return "输入值不完整,是否修正? 是 - 返回编辑器,修正该值. 否 -保留该值. 取消 - 重设为原来的值. ";
                case StringId.NavigatorAppendButtonHint: return "追加";
                case StringId.NavigatorCancelEditButtonHint: return "取消编辑";
                case StringId.NavigatorEditButtonHint: return "编辑";
                case StringId.NavigatorEndEditButtonHint: return "结束编辑";
                case StringId.NavigatorFirstButtonHint: return "第一个";
                case StringId.NavigatorLastButtonHint: return "最后一个";
                case StringId.NavigatorNextButtonHint: return "下一个";
                case StringId.NavigatorNextPageButtonHint: return "下一页";
                case StringId.NavigatorPreviousButtonHint: return "前一个";
                case StringId.NavigatorPreviousPageButtonHint: return "前一页";
                case StringId.NavigatorRemoveButtonHint: return "删除";
                case StringId.NavigatorTextStringFormat: return "第{0}行({1})";
                case StringId.None: return "无";
                case StringId.NotValidArrayLength: return "无效的数组长度。";
                case StringId.OK: return "确定(&O)";
                case StringId.PictureEditCopyImageError: return "无法复制图像";
                case StringId.PictureEditMenuCopy: return "复制";
                case StringId.PictureEditMenuCut: return "剪切";
                case StringId.PictureEditMenuDelete: return "删除";
                case StringId.PictureEditMenuFitImage: return "适应图像尺寸";
                case StringId.PictureEditMenuFullSize: return "全尺寸";
                case StringId.PictureEditMenuLoad: return "装载";
                case StringId.PictureEditMenuPaste: return "粘贴";
                case StringId.PictureEditMenuSave: return "保存";
                case StringId.PictureEditMenuZoom: return "缩放";
                case StringId.PictureEditMenuZoomIn: return "放大";
                case StringId.PictureEditMenuZoomOut: return "缩小";
                case StringId.PictureEditMenuZoomTo: return "缩放到";
                case StringId.PictureEditMenuZoomToolTip: return "{0}%";
                case StringId.PictureEditOpenFileError: return "错误的图像格式";
                case StringId.PictureEditOpenFileErrorCaption: return "打开错误";
                case StringId.PictureEditOpenFileFilter: return "位图文件 (*.bmp)|*.bmp'" + "GIF文件 (*.gif)|*.gif'" + "JPG文件 (*.jpg;*.jpeg)|*.jpg;*.jpeg'" + "Icon 文件 (*.ico)|*.ico'" + "所有图像文件 '*.bmp;*.gif;*.jpg;*.jpeg;*.ico;*.png;*.tif'" + "所有文件 '*.*";
                case StringId.PictureEditOpenFileTitle: return "打开";
                case StringId.PictureEditSaveFileFilter: return "位图文件 (*.bmp)|*.bmp'" + "GIF文件 (*.gif)|*.gif'" + "JPG文件 (*.jpg)|*.jpg";
                case StringId.PictureEditSaveFileTitle: return "另存为";
                case StringId.PreviewPanelText: return "预览";
                case StringId.ProgressCancel: return "取消";
                case StringId.ProgressCancelPending: return "取消挂起";
                case StringId.ProgressCreateDocument: return "创建文档";
                case StringId.ProgressExport: return "导出";
                case StringId.ProgressLoadingData: return "装载数据";
                case StringId.ProgressPrinting: return "打印";
                case StringId.RestoreLayoutDialogFileFilter: return "XML 文件 (*.xml)|*.xml'所有文件'*.*";
                case StringId.RestoreLayoutDialogTitle: return "恢复布局";
                case StringId.SaveLayoutDialogFileFilter: return "XML 文件 (*.xml)|*.xml";
                case StringId.SaveLayoutDialogTitle: return "保存布局";
                case StringId.TabHeaderButtonClose: return "关闭";
                case StringId.TabHeaderButtonNext: return "下一个";
                case StringId.TabHeaderButtonPrev: return "前一个";
                case StringId.TabHeaderSelectorButton: return "选择按钮";
                case StringId.TextEditMenuCopy: return "复制(&C)";
                case StringId.TextEditMenuCut: return "剪切(&t)";
                case StringId.TextEditMenuDelete: return "删除(&D)";
                case StringId.TextEditMenuPaste: return "粘贴(&P)";
                case StringId.TextEditMenuSelectAll: return "全选(&A)";
                case StringId.TextEditMenuUndo: return "撤销(&U)";
                case StringId.TransparentBackColorNotSupported: return "此控件不支持透明背景色";
                case StringId.UnknownPictureFormat: return "未知的图形格式";
                case StringId.XtraMessageBoxAbortButtonText: return "中断(&A)";
                case StringId.XtraMessageBoxCancelButtonText: return "取消";
                case StringId.XtraMessageBoxIgnoreButtonText: return "忽略(&I)";
                case StringId.XtraMessageBoxNoButtonText: return "否(&N)";
                case StringId.XtraMessageBoxOkButtonText: return "确定(&O)";
                case StringId.XtraMessageBoxRetryButtonText: return "重试(&R)";
                case StringId.XtraMessageBoxYesButtonText: return "是(&Y)";
            }

            return base.GetLocalizedString(id);
        }
 
    }



}