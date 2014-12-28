using System;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data.OleDb;
using System.Windows.Forms;
using DevExpress.XtraPrinting;

namespace wheda.db
{
    /// <summary>
    /// cndb 的摘要说明。
    /// </summary>
    /// 


    public class dboper
    {
       public static string sppunitquery = "select a.ppid,unitno as unitno,a.unitname as unitname,a.unittype as unittype,a.unitorg as unitorg," +
                          "a.unitstatus as unitstatus,a.splitfromunit,a.combinetounit," +
                          "ifnull(f.qf,0) as qf,"+
                         "a.unitcarea as unitcarea,a.unituarea as unituarea,c.cusid,d.cusname as cusname,d.cusno as cusno,a.contractid,c.contractno as contractno," +
                         "e.sdt as contractsdt,e.edt as contractedt,a.unitrent as unitrent,a.unitbfee as unitbfee,a.unitarea ,a.unitbuilding,a.unitlevel " +
                          "from t_ppunit a  left outer join t_contract c on a.contractid=c.contractid  " +
                         " left outer join t_cus d on c.cusid=d.cusid "+
                         " left outer join t_con_pp e on ( a.contractid=e.contractid and a.ppid=e.ppid) "+
                         " left outer join (select ppid,sum(feepay-ifnull(feepayed,0)) as qf from t_fee_pay_mgt_period "+
                         " where date_add(feepaysdt,interval 1 month)<=now() group  by ppid ) f on a.ppid=f.ppid "; 



        public static string scusquery = "select a.cusid ,a.cusarea,a.cusno as cusno ,a.cusname as cusname,a.cusmobnum as cusmobnum,a.cusaddr as cusaddr, "+
                                         "b.contractno from t_cus a left outer join t_contract b on a.cusid=b.cusid ";

        public static string scontractquery = "select contractid,contractnofnc,a.cusid,b.cusname as cusname, b.cusno as cusno,a.contractno as contractno,contractarea as contractarea,contractpptype as contractpptype,"+
                                "unittarget as unittarget,rentfreeperiod as rentfreeperiod,rentpaystyle as rentpaystyle,depositfee as depositfee,"+
                                "contractsdt as contractsdt,contractedt as contractedt, contractstatus as contractstatus,contractorg as contractorg," +
                                "signdt as signdt,contracttext as contracttext from t_contract a left outer join t_cus b on a.cusid=b.cusid ";

        public static string suserquery = "select userid,username,userpassword,userstatus,userdesc from t_user where userid<>0  ";

        public static string saltquery = "select b.unitno,c.contractno,d.cusno,e.username,f.username as operuser,altmsg,altdt from t_mgtalt a " +
                                       " left outer join t_ppunit b on a.ppid=b.ppid " +
                                       " left outer join t_contract c on a.contractid=c.contractid " +
                                       " left outer join t_cus d on a.cusid=d.cusid " +
                                       " left outer join t_user e on a.userid=e.userid " +
                                       " left outer join t_user f on a.operuser=f.userid ";


        private MySql.Data.MySqlClient.MySqlConnection mysqldb;
        private MySql.Data.MySqlClient.MySqlDataAdapter mysqlda;

        MySql.Data.MySqlClient.MySqlTransaction mst;

        //private System.Data.DataSet ds;
        private System.Data.DataTable dt;

        public dboper() { }

        public void doconnnect()
        {
            if (mysqldb == null || mysqldb.State != ConnectionState.Open)
            {

                string ipstr = "";
                string cnstr = ConfigurationManager.AppSettings["iptype"];
                if (cnstr == "0")
                {
                    ipstr = ConfigurationManager.AppSettings["intranetserverip"];
                }
                else
                {
                    ipstr = ConfigurationManager.AppSettings["internetserverip"];
                }

                cnstr = "Server=" + ipstr + ";Uid=whjkzcjyb;Pwd=cb15377155878;Database=whecodevarea;";


                mysqldb = new MySql.Data.MySqlClient.MySqlConnection();
                mysqldb.ConnectionString = cnstr;
                mysqldb.Open();
            }
            //accessdb = new System.Data.OleDb.OleDbConnection(cnstr);
            //accessdb.Open();
        }

        
 


        public void finalclose()
        {
            if (mysqldb != null)
            {
                mysqldb.Close();
            }
        }

        public void close()
        {
            //mysqldb.Close();
        }

        public void executecmd(string str)
        {
            MySql.Data.MySqlClient.MySqlCommand dbcmd = new MySql.Data.MySqlClient.MySqlCommand(str, mysqldb);
            dbcmd.ExecuteNonQuery();
        }

        

        public void getdatatable(string str)
        {

            mysqlda = new MySql.Data.MySqlClient.MySqlDataAdapter(str, mysqldb);

            dt = new System.Data.DataTable();
            mysqlda.Fill(dt);
        }

        public string exportgvtoxls(DevExpress.XtraGrid.Views.Grid.GridView xgvxls)
        {
            xgvxls.OptionsPrint.AutoWidth = false;

            for (int ii = 0; ii < xgvxls.Columns.Count; ii++)
            {
                xgvxls.Columns[ii].Width = 100;
            }

            string sfile = System.IO.Path.GetTempPath() + System.DateTime.Now.ToString("HHmmssfff") + ".xls";
            xgvxls.ExportToXls(sfile);

            System.Diagnostics.Process.Start(sfile);


            return sfile;
        }

        public DataTable importxlstodatatable(string sfile)
        {


            DataTable dt = null;
             
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + sfile + ";Extended Properties=Excel 8.0; ";//xls导入
            OleDbConnection objCon = new OleDbConnection(strCon);
            objCon.Open();

            DataTable yTable = objCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
            string strSel = "select * from [" + yTable.Rows[0]["Table_Name"].ToString() + "]";//xls导入

            OleDbDataAdapter objAdapter = new OleDbDataAdapter(strSel, objCon);
            
            DataSet ds = new DataSet();
            objAdapter.Fill(ds);
            dt = ds.Tables[0];
            objCon.Close();

            return dt;

        }

        public DataTable getgroupprivilege(string sgroupid)
        {
            doconnnect();
            string str = "select id,parentid,opername,operdesc from t_sysprivilegecode where parentid=0 or ( id in " +
                         " (select spid from t_groupprivilege where groupid=" + sgroupid + "))";
            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getsysprivilegecode(string sgroupid)
        {
            doconnnect();
            string str = "select id,parentid,opername,operdesc from t_sysprivilegecode where id not in "+
                         " (select spid from t_groupprivilege where groupid="+sgroupid+")";  
            getdatatable(str);
            close();



            return dt;
        }


        //获取室位级联数据
        public DataTable getpp()
        {
            doconnnect();
            string str = "select id,parentid,ppname from t_pp";
            getdatatable(str);
            close();



            return dt;
        }


        public DataTable getppfeepaymgtbyperiod(DataRow dr)
        {
            doconnnect();
            string str = "select feeid,contractid,contractno,ppid,unitno,cusid,cusno,feepaysdt,feepayedt,feepay,feepayed from t_fee_pay_mgt_period " +
                         "where contractid=" + dr["contractid"].ToString() + " and feepaysdt='" + dr["feepaysdt"].ToString()+"'";
            ;
            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getppfeepaymgt(DataRow dr)
        {
            doconnnect();
            string str = "select feeid,contractid,contractno,ppid,unitno,cusid,cusno,feepaysdt,feepayedt,feepay from t_fee_pay_mgt_period " +
                         "where contractid=" + dr["contractid"].ToString() + " and ppid=" + dr["ppid"].ToString();
            ;
            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getppfeepayfnc(DataRow dr)
        {
            doconnnect();
            string str = "select feeid,contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree from t_fee_pay_mgt " +
                         "where contractid=" + dr["contractid"].ToString() + " and ppid=" + dr["ppid"].ToString();
                          ;
            getdatatable(str);
            close();



            return dt;
        }

        public void feepayedconfirmedfnccancel(DataRow dr)
        {
            //更新财务复核状态

            doconnnect();

            string str = "update t_fee_payed_seq_con_fnc set  isconfirmed ='否' where feepayedctseq=" + dr["feepayedctseq"].ToString() +
                         " and isconfirmed='是'";
            executecmd(str);

            str = "insert into t_fnc_confirm_seq(feepayedctseq,canceldate,oper) values(" +
                     dr["feepayedctseq"].ToString() + ",'" + DateTime.Now.ToString("yyyyMMdd") + "',"
                     +jkwyjygl.Form1.uid+")";

            executecmd(str);

            close();
        }

        public void feepayedconfirmedfnc(DataRow dr)
        {
            //更新财务复核状态

            doconnnect();

            string str = "update t_fee_payed_seq_con_fnc set  isconfirmed ='是' where feepayedctseq=" + dr["feepayedctseq"].ToString();
            executecmd(str);

            str = "insert into t_fnc_confirm_seq(feepayedctseq,confirmdate,oper) values(" +
                     dr["feepayedctseq"].ToString() + ",'" + DateTime.Now.ToString("yyyyMMdd") + "',"
                     + jkwyjygl.Form1.uid + ")";

            executecmd(str);

            close();
        }

        public void attachcontractnofnc(DataRow dr,string sctnofnc)
        {

            //更新财务合同编号

            doconnnect();

            string str = "update t_contract set  contractnofnc ='"+sctnofnc+"' where contractid=" + dr["contractid"].ToString();
            executecmd(str);

            str = "update t_fee_pay_mgt set contractnofnc='" + sctnofnc + "' where contractid=" + dr["contractid"].ToString();
            executecmd(str);

            str = "update t_fee_payed_seq_con_fnc set contractnofnc='" + sctnofnc + "' where contractid=" + dr["contractid"].ToString();
            executecmd(str);

            

            close();

        }

        public void updateuserpara(string paraname, string paravalue)
        {
            doconnnect();

            string str = "update t_userpara set " + paraname + "='" + paravalue + "'"+
                         " where userid=" + jkwyjygl.Form1.uid;

            executecmd(str);

            close();



        }

        public void updatesyspara(string paraname, string paravalue)
        {
            doconnnect();

            string str = "update t_syspara set " + paraname + "='" + paravalue + "'";
            executecmd(str);

            close();

            

        }

        public void restoreinform(string idno)
        {
            doconnnect();

            string str = "delete from t_inform where idno=" + idno;
            executecmd(str);

            close();



        }

        public DataTable getignoredinform(string uid)
        {
            doconnnect();
            string str = "select * from t_inform where userid=" + uid;
            getdatatable(str);
            close();



            return dt;

        }
        public DataTable getuserpara(string uid)
        {
            doconnnect();
            string str = "select * from t_userpara where userid="+uid;
            getdatatable(str);
            close();



            return dt;
        }

        //
        public DataTable getsyspara()
        {
            doconnnect();
            string str = "select * from t_syspara";
            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getuserprivilegecat(string suserid)
        {
            doconnnect();
            string str = "select * from t_sysprivilegecode where parentid=0 and id in " +
                        "(select distinct parentid from t_groupprivilege a ,t_sysprivilegecode b where a.spid=b.id and a.groupid in " +
                        "(select groupid from t_user_group c where c.userid=" + suserid + "))"; 

            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getuserprivilege(string suserid)
        {
            doconnnect();
            string str = "select * from t_groupprivilege a ,t_sysprivilegecode b " +
                         " where a.spid=b.id and a.groupid in " +
                         " (select groupid from t_user_group c where c.userid=" + suserid + ")";

            getdatatable(str);
            close();



            return dt;
        }
        
        public DataTable getuserbygroupid(string sgroupid)
        {
            doconnnect();
            string str = "select a.userid,a.username,a.userstatus,a.userdesc from t_user a,t_user_group b where a.userid=b.userid and b.groupid="+sgroupid;
            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getgroupbyuserid(string suserid)
        {
            doconnnect();
            string str = "select b.paravalue as groupname from t_user_group a, t_syscode b where a.groupid=b.id and a.userid="+suserid;
            getdatatable(str);
            close();



            return dt;
        }

        public DataTable getppareatree()
        {
            doconnnect();
            string str = "select id,parentid,ppname from t_pp where pptype='0' order by id";
            getdatatable(str);
            close();


            return dt;
        }

        //获取项目
        public DataTable getpparea()
        {
            doconnnect();
            string str = "select id,ppcode,ppname,ppdes from t_pp where pptype='0' order by id";
            getdatatable(str);
            close();


            return dt;
        }

        //获取建筑
        public DataTable getppbuildingbyareaid(string sarea)
        {
            doconnnect();
            string str = "select id,ppcode,ppname,ppdes from t_pp where pptype='1' "+
                         "and parentid="+sarea+" order by id";
            getdatatable(str);
            close();


            return dt;

        }

        //获取楼层
        public DataTable getpplevelbybuildingid(string sbuilding)
        {
            doconnnect();
            string str = "select id,ppcode,ppname,ppdes from t_pp where pptype='2' " +
                         "and parentid=" + sbuilding + " order by id";
            getdatatable(str);
            close();


            return dt;

        }


        //根据ppid获取房间资料
        public DataTable getppunitbyppid(string sppid)
        {
            doconnnect();

            string squery =sppunitquery+" where a.ppid=" +sppid;

            getdatatable(squery);

            close();

            return dt;
        }

        //
        public DataTable getfreeandnotinppunitbyid(string sctid,string sarea, string sbuilding, string slevel)
        {
            doconnnect();


            string squery = "select ppid,unitno from t_ppunit a " + " where " +
                        "unitarea=" + sarea + " and " +
                        "unitbuilding=" + sbuilding + " and " +
                        "unitlevel=" + slevel+" and "+
                        "a.unitstatus='空闲' and "+
                        "a.ppid  not in (select ppid from t_con_pp where contractid="+sctid+")";


            getdatatable(squery);

            close();

            return dt;
        }

        public DataTable getppunitbyquery(string sarea, string sbuilding, string slevel)
        {
            doconnnect();


            string squery = sppunitquery + " where " +
                        "unitarea=" + sarea;
            if (sbuilding != null)
            {
                squery += " and " +
                        "unitbuilding=" + sbuilding;
            }
            if (slevel != null)
            {
                squery += " and " +
                        "unitlevel=" + slevel;
            }
            
            squery+=" and unitstatus not in('已拆','已并')";


            getdatatable(squery);

            close();

            return dt;
        }

        //根据id获取室位资料
        public DataTable getppunitbyid(string sarea,string sbuilding,string slevel)
        {
            doconnnect();

            //string str = "select ppid,cusname,unitno,unitcarea,unituarea"+
            //             ",unitstatus,contractsdt,contractedt "+
            //             "from t_ppunit a left outer join  t_cus b on a.cusid=b.cusid where "+
            //             "unitarea="+sarea+" and "+
            //             "unitbuilding="+sbuilding+" and "+
            //             "unitlevel="+slevel;

            string squery = sppunitquery + " where " +
                        "unitarea=" + sarea + " and " +
                        "unitbuilding=" + sbuilding + " and " +
                        "unitlevel=" + slevel +
                        " and unitstatus not in('已拆','已并')";


            getdatatable(squery);

            close();

            return dt;
        }

        public void deletepayfeemgt(DataRow drct, DataRow drpp)
        {
            doconnnect();

            string str1 = "delete from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString();
            if (drpp != null)
            {
                str1 += " and ppid=" + drpp["ppid"].ToString();
            }

            str1 += " and ppid<>9999";

            executecmd(str1);
 
            close();
        }

        public void deletepayfeemgt_period(DataRow drct, DataRow drpp)
        {
            doconnnect();

            string str1 = "delete from t_fee_pay_mgt_period where contractid=" + drct["contractid"].ToString();
            if (drpp != null)
            {
                str1 += " and ppid=" + drpp["ppid"].ToString();
            }
            executecmd(str1);

            close();

        }


        //检查是否已经有mgt应收数据
        public bool checkhasfeepaymgt(DataRow drct, DataRow drpp)
        {
            bool iret = true;
            doconnnect();

            string str1 = "select count(*) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString();
            if (drpp != null)
            {
                str1 += " and ppid=" + drpp["ppid"].ToString();
            }
            getdatatable(str1);

            if (Convert.ToInt32(dt.Rows[0][0].ToString()) > 0) iret = true;
            else iret = false;

            close();

            return iret;

        }

        //删除经营收费
        public void delmgtpay(DataRow dr)
        {
            doconnnect();

            string s1 = "update t_fee_pay_mgt_period set feepayed=0 where feepayedctseq=" + dr["feepayedctseq"].ToString();

            executecmd(s1);

            s1 = "update t_fee_payed_seq_con_mgt  set contractid=null where feepayedctseq=" + dr["feepayedctseq"].ToString();

            executecmd(s1);

            close();
        }


        //调整应收--合同提前终止
        public void createfeepaymgt_period_cancel(DataRow drct, DataRow drpp,string scancelmon, int bisNo1)
        {
            doconnnect();

            string sdt = drpp["sdt"].ToString();
            string edt = scancelmon;
            string speriod = drct["rentpaystyle"].ToString();
            int iperiod = 0;

            System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
            System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));


            //找出经营部门未录入收费的开始月份

            string ss11 = "select max(feepayedt) from t_fee_pay_mgt_period " +
                          " where contractid=" + drct["contractid"].ToString() +
                          " and  ppid=" + drpp["ppid"].ToString() +
                          " and feepayed>0";
            getdatatable(ss11);

            int ihaspay = 0;

            if (!Convert.IsDBNull(dt.Rows[0][0]))
            {
                string s1 = dt.Rows[0][0].ToString();

                //删除之后的所有时间段应收
                string str111 = "delete from t_fee_pay_mgt_period " +
                              " where contractid=" + drct["contractid"].ToString() +
                              " and  ppid=" + drpp["ppid"].ToString() +
                              " and  feepayedt>'" + s1 + "'";
                executecmd(str111);



                dt_s = DateTime.ParseExact(s1, "yyyyMMdd", new CultureInfo("zh-CN", true));

                dt_s = dt_s.AddDays(1);

                ihaspay = 1;
            }
            else
            {
                //未有收费,删除所有时间段应收
                string str111 = "delete from t_fee_pay_mgt_period " +
                              " where contractid=" + drct["contractid"].ToString() +
                              " and  ppid=" + drpp["ppid"].ToString();
                executecmd(str111);

            }


            switch (speriod)
            {
                case "按月":
                    iperiod = 1;
                    break;
                case "按季":
                    iperiod = 3;
                    break;
                case "半年":
                    iperiod = 6;
                    break;
                case "按年":
                    iperiod = 12;
                    break;
                default:
                    break;

            }

            try
            {
                //取得拆分后的最后月数据
                double d124 = 0.0;
                string smaxmon = "";

                if (bisNo1 == 0) //非1号合同，处理头尾月合并
                {
                    string s1 = "select max(feemonth) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                                    " and ppid=" + drpp["ppid"].ToString();

                    getdatatable(s1);


                    smaxmon = dt.Rows[0][0].ToString();

                    string sctmonend = scancelmon.Substring(0, 6);

                    //非1号合同，在财务拆分后，将有终止月月份的数据，未拆分则没有
                    if (string.Compare(smaxmon, sctmonend) == 0)
                    {
                        s1 = "select rentfee from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                                    " and ppid=" + drpp["ppid"].ToString() +
                                    " and feemonth='" + smaxmon + "'";
                        getdatatable(s1);

                        d124 = Convert.ToDouble(dt.Rows[0][0].ToString());
                    }
                }


                mst = mysqldb.BeginTransaction();

                int icount = 0;

                while (true)
                {


                    DateTime d1 = dt_s;
                    dt_s = dt_s.AddMonths(iperiod).AddDays(-1);

                    if (d1 >= dt_e) break;
                    //if (dt_s > dt_e) break;
                    if (dt_s > dt_e) dt_s = dt_e;



                    string smons = d1.ToString("yyyyMM");
                    string smone = d1.AddMonths(iperiod - 1).ToString("yyyyMM");


                    string str = "select sum(ifnull(rentfee,0)+ifnull(bfee,0)) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                               " and ppid=" + drpp["ppid"].ToString() +
                               " and feemonth>='" + smons + "'";

                    if(bisNo1==0 && string.Compare(smone,smaxmon)>=0)  //非1号合同，最后月不统计period中
                         str+=  " and feemonth<'" + smaxmon + "'";
                    else str += " and feemonth<='" + smone + "'";

                    getdatatable(str);

                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        double d112 = Convert.ToDouble(dt.Rows[0][0].ToString());
                        //d112 = Math.Round(d112);
                        string s111 = d1.ToString("yyyyMMdd");
                        string s112 = dt_s.ToString("yyyyMMdd");

                        if (icount == 0 && ihaspay == 0 && bisNo1==0)  //非1号合同，首个period，且无录入收费，加入最后月拆分数据
                        {
                            d112+=d124;
                        }

                        if (bisNo1==1)  //1--1号合同，0--非1号合同
                        {
                            s112 = smone;

                            int ndays = DateTime.DaysInMonth(Convert.ToInt32(smone.Substring(0, 4)),
                             Convert.ToInt32(smone.Substring(4, 2)));

                            s112 = s112 + ndays.ToString();

                            if (icount > 0)
                            {
                                s111 = s111.Substring(0, 6) + "01";
                            }

                        }

                        str = "insert into t_fee_pay_mgt_period(contractid,contractno,ppid,unitno,cusid,cusno,feepaysdt,feepayedt,feepay) values(" +
                            drct["contractid"].ToString() + ",'" +
                            drct["contractno"].ToString() + "'," +
                            drpp["ppid"].ToString() + ",'" +
                            drpp["unitno"].ToString() + "'," +
                            drct["cusid"].ToString() + ",'" +
                            drct["cusno"].ToString() + "','" +
                            s111 + "','" +
                            s112 + "'," +
                            d112.ToString() + ")";

                        executecmd(str);
                    }

                    dt_s = dt_s.AddDays(1);

                    icount++;
                }

            }
            catch
            {
                mst.Rollback();
                throw;
            }

            mst.Commit();

            close();

        }


        //调整应收--按合同付费方式(经营收款时间段)
        public void createfeepaymgt_period_mid(DataRow drpp, DataRow drct, bool bnmon)
        {
            doconnnect();

            string sdt = drpp["sdt"].ToString();
            string edt = drpp["edt"].ToString();
            string speriod = drct["rentpaystyle"].ToString();
            int iperiod = 0;

            System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
            System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));


            //找出经营部门未录入收费的开始月份

            string ss11 = "select max(feepayedt) from t_fee_pay_mgt_period " +
                          " where contractid=" + drct["contractid"].ToString() +
                          " and  ppid=" + drpp["ppid"].ToString() +
                          " and feepayed>0";
            getdatatable(ss11);


            if (!Convert.IsDBNull(dt.Rows[0][0]))
            {
                string s1 = dt.Rows[0][0].ToString();

                //删除之后的所有时间段应收
                string str111 = "delete from t_fee_pay_mgt_period " +
                              " where contractid=" + drct["contractid"].ToString() +
                              " and  ppid=" + drpp["ppid"].ToString() +
                              " and  feepayedt>'" + s1 + "'";
                executecmd(str111);



                dt_s = DateTime.ParseExact(s1, "yyyyMMdd", new CultureInfo("zh-CN", true));

                dt_s = dt_s.AddDays(1);
            }
            else
            {
                //未有收费,删除所有时间段应收
                string str111 = "delete from t_fee_pay_mgt_period " +
                              " where contractid=" + drct["contractid"].ToString() +
                              " and  ppid=" + drpp["ppid"].ToString();
                executecmd(str111);

            }


            switch (speriod)
            {
                case "按月":
                    iperiod = 1;
                    break;
                case "按季":
                    iperiod = 3;
                    break;
                case "半年":
                    iperiod = 6;
                    break;
                case "按年":
                    iperiod = 12;
                    break;
                default:
                    break;

            }

            try
            {
                string sdmaxmon = ""; double sdmax = 0;
                int isplitbyfnc = judgefncspilit(drpp, drct, ref sdmaxmon, ref sdmax);

                mst = mysqldb.BeginTransaction();

                int icount = 0;

                while (true)
                {
                    DateTime d1 = dt_s;
                    dt_s = dt_s.AddMonths(iperiod).AddDays(-1);

                    if (d1 >= dt_e) break;
                    //if (dt_s > dt_e) break;
                    if (dt_s > dt_e) dt_s = dt_e;

                    string smons = d1.ToString("yyyyMM");
                    string smone = d1.AddMonths(iperiod - 1).ToString("yyyyMM");


                    string str = "select sum(ifnull(rentfee,0)+ifnull(bfee,0)) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                               " and ppid=" + drpp["ppid"].ToString() +
                               " and feemonth>='" + smons + "'" +
                               " and feemonth<='" + smone + "'";

                    getdatatable(str);

                    if (!Convert.IsDBNull(dt.Rows[0][0]))
                    {
                        double d112 = Convert.ToDouble(dt.Rows[0][0].ToString());

                        if (isplitbyfnc == 1 && icount == 0) d112 += sdmax;

                        //d112 = Math.Round(d112);
                        string s111 = d1.ToString("yyyyMMdd");
                        string s112 = dt_s.ToString("yyyyMMdd");

                        if (bnmon)
                        {
                            s112 = smone;

                            int ndays = DateTime.DaysInMonth(Convert.ToInt32(smone.Substring(0, 4)),
                             Convert.ToInt32(smone.Substring(4, 2)));

                            s112 = s112 + ndays.ToString();

                            if (icount > 0)
                            {
                                s111 = s111.Substring(0, 6) + "01";
                            }

                        }

                        str = "insert into t_fee_pay_mgt_period(contractid,contractno,ppid,unitno,cusid,cusno,feepaysdt,feepayedt,feepay) values(" +
                            drct["contractid"].ToString() + ",'" +
                            drct["contractno"].ToString() + "'," +
                            drpp["ppid"].ToString() + ",'" +
                            drpp["unitno"].ToString() + "'," +
                            drct["cusid"].ToString() + ",'" +
                            drct["cusno"].ToString() + "','" +
                            s111 + "','" +
                            s112 + "'," +
                            d112.ToString() + ")";

                        executecmd(str);
                    }

                    dt_s = dt_s.AddDays(1);

                    icount++;
                }

            }
            catch
            {
                mst.Rollback();
                throw;
            }

            mst.Commit();

            close();

        }


        //生成应收--按合同付费方式(经营收款时间段)
        public void createfeepaymgt_period(DataRow drpp, DataRow drct,bool bnmon)
        {
            doconnnect();

            string sdt = drpp["sdt"].ToString();
            string edt = drpp["edt"].ToString();
            string speriod = drct["rentpaystyle"].ToString();
            int iperiod = 0;

            System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
            System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));

            switch (speriod)
            {
                case "按月":
                    iperiod = 1;
                    break;
                case "按季":
                    iperiod = 3;
                    break;
                case "半年":
                    iperiod = 6;
                    break;
                case "按年":
                    iperiod = 12;
                    break;
                default:
                    break;

            }

            try
            {

                string dssmon = ""; double dsmax = 0;

                int isplitbyfnc = judgefncspilit(drpp, drct, ref dssmon, ref dsmax);    //判断数据是否被财务拆分

                mst = mysqldb.BeginTransaction();

                int icount = 0;

                while (true)
                {
                    DateTime d1 = dt_s;
                    dt_s = dt_s.AddMonths(iperiod).AddDays(-1);

                    if (d1 >= dt_e) break;
                    //if (dt_s > dt_e) break;
                    if (dt_s > dt_e) dt_s = dt_e;

                    string smons = d1.ToString("yyyyMM");
                    string smone = d1.AddMonths(iperiod - 1).ToString("yyyyMM");


                    string str = "select sum(ifnull(rentfee,0)+ifnull(bfee,0)) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                               " and ppid=" + drpp["ppid"].ToString() +
                               " and feemonth>='" + smons +"'"+
                               " and feemonth<='" + smone + "'";

                    getdatatable(str);

                    if (!Convert.IsDBNull( dt.Rows[0][0]))
                    {
                        double d112 = Convert.ToDouble(dt.Rows[0][0].ToString());


                        //已拆分，加上最后月数据
                        if (isplitbyfnc == 1 && icount == 0)
                        {
                            d112 += dsmax;
                        }

                        //d112 = Math.Round(d112);
                        string s111 = d1.ToString("yyyyMMdd");
                        string s112 = dt_s.ToString("yyyyMMdd");

                        if (bnmon)
                        {
                            s112 = smone;

                            int ndays=DateTime.DaysInMonth(Convert.ToInt32(smone.Substring(0, 4)),
                             Convert.ToInt32(smone.Substring(4, 2)));

                            s112 = s112 + ndays.ToString();

                            if (icount > 0)
                            {
                                s111 = s111.Substring(0, 6) + "01";
                            }

                        }

                        str = "insert into t_fee_pay_mgt_period(contractid,contractno,ppid,unitno,cusid,cusno,feepaysdt,feepayedt,feepay) values(" +
                            drct["contractid"].ToString() + ",'" +
                            drct["contractno"].ToString() + "'," +
                            drpp["ppid"].ToString() + ",'" +
                            drpp["unitno"].ToString() + "'," +
                            drct["cusid"].ToString() + ",'" +
                            drct["cusno"].ToString() + "','" +
                            s111 + "','" +
                            s112 + "'," +
                            d112.ToString() + ")";

                        executecmd(str);
                    }

                    dt_s=dt_s.AddDays(1);

                    icount++;
                }

            }
            catch
            {
                mst.Rollback();
                throw;
            }

            mst.Commit();

            close();

        }

        public void updatefncsplitfeepay(DataRow drct,DataRow drpp)
        {
            doconnnect();
            string s1 = "select max(feemonth) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                      " and ppid=" + drpp["ppid"].ToString();

            getdatatable(s1);

            string smaxmon = dt.Rows[0][0].ToString();

            //更新最大月的应收到财务收费记录中（如果存在的话），该应收是冗余复制品，提前退租会导致该应收不正确
            string s11= "update t_fee_payed_seq_fnc a,t_fee_pay_mgt b set a.payfee=b.rentfee+b.bfee where " +
                      "a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth "+
                      " and a.contractid=" +drct["contractid"].ToString()+
                      " and a.ppid=" + drpp["ppid"].ToString()+
                      " and a.feemonth='" + smaxmon + "'";

            executecmd(s11);

            close();
        }

        //提前终止合同,1号合同
        public int docancelctprev1(DataRow drct, DataRow drpp, string scancelmon)
        {
            doconnnect();

            string s11 = "delete from  t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                      " and ppid=" + drpp["ppid"].ToString() +
                      " and feemonth>'" + scancelmon.Substring(0, 6) + "' ";

            executecmd(s11);

            //更新房间终止日期
            s11 = "update t_con_pp set edt='" + scancelmon + "' " +
                " where contractid=" + drct["contractid"].ToString() +
                " and ppid=" + drpp["ppid"].ToString();

            executecmd(s11);

            close();
            return 0;
        }

        //提前终止合同,非1号合同
        public int docancelctprevNo1(DataRow drct,DataRow drpp, string scancelmon)
        {
            doconnnect();

            string s1 = "select max(feemonth) from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                      " and ppid=" + drpp["ppid"].ToString();

            getdatatable(s1);

            
            string smaxmon = dt.Rows[0][0].ToString();

            string sctmonend = drpp["edt"].ToString().Substring(0,6);

            //非1号合同，在财务拆分后，将有终止月月份的数据，未拆分则没有
            if (string.Compare(smaxmon, sctmonend) == 0)
            {
                //删除终止月---合同最大月，保留最大月（已拆分）
                string s11 = "delete from  t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                          " and ppid=" + drpp["ppid"].ToString() +
                          " and feemonth>='" + scancelmon.Substring(0, 6) + "' " +
                          " and feemonth<'" + smaxmon + "'";

                executecmd(s11);

                //最后月更新为终止月（已拆分)
                s11 = "update  t_fee_pay_mgt set feemonth='" + scancelmon.Substring(0, 6) +
                          "' where contractid=" + drct["contractid"].ToString() +
                          " and ppid=" + drpp["ppid"].ToString() +
                          " and feemonth='" + smaxmon + "'";

                executecmd(s11);

                //更新房间终止日期
                s11 = "update t_con_pp set edt='" + scancelmon + "' " +
                    " where contractid=" + drct["contractid"].ToString() +
                    " and ppid=" + drpp["ppid"].ToString();

                executecmd(s11);

 
            }
            else  //未拆分
            {
                string s11 = "delete from  t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                          " and ppid=" + drpp["ppid"].ToString() +
                          " and feemonth>='" + scancelmon.Substring(0, 6) + "' ";

                executecmd(s11);
            }

            //更新已录入的财务收费应收

            close();
            return 0;
        }

        public int judgefncspilit(DataRow drpp, DataRow drct, ref string ssmon, ref double ssrent)      //判断月度数据是否被财务分拆过
        {
            string ss11 = "select max(feemonth),min(feemonth) from t_fee_pay_mgt" +
                          " where contractid=" + drct["contractid"].ToString() +
                          " and  ppid=" + drpp["ppid"].ToString() +
                          " and rentfee>0";

            getdatatable(ss11);

            if (Convert.IsDBNull(dt.Rows[0][0])) return 0;
            if (string.Compare(dt.Rows[0][0].ToString(), dt.Rows[0][1].ToString()) == 0) return 0;

            string smaxmon = dt.Rows[0][0].ToString();
            string sminmon = dt.Rows[0][1].ToString();

            ss11 = "select rentfee+bfee from t_fee_pay_mgt " +
                  " where contractid=" + drct["contractid"].ToString() +
                          " and  ppid=" + drpp["ppid"].ToString() +
                          " and rentfee>0 " +
                          " order by feemonth  limit 0,2 ";

            getdatatable(ss11);

            double dmin = Convert.ToDouble(dt.Rows[0][0].ToString());
            double d2 = Convert.ToDouble(dt.Rows[1][0].ToString());

            ss11 = "select rentfee+bfee from t_fee_pay_mgt " +
                  " where contractid=" + drct["contractid"].ToString() +
                          " and  ppid=" + drpp["ppid"].ToString() +
                          " and rentfee>0 " +
                          " order by feemonth desc limit 0,1 ";
            getdatatable(ss11);

            double dmax = Convert.ToDouble(dt.Rows[0][0].ToString());

            if ((dmax + dmin) == d2)   //被拆分了
            {

                ssmon = smaxmon;
                ssrent = dmax;

                return 1;
            }

            return 0;
        }


        //生成调整应收--按财务账期
        public void createfeepaymgt_mid(DataRow drpp, DataRow drct, float idiscount, int idistype, bool bnmon)
        {
            doconnnect();
            //生成调整应收--很复杂
            
            //找出经营部门未录入收费的开始月份

            string ss11 = "select max(feepayedt) from t_fee_pay_mgt_period " +
                          " where contractid=" + drct["contractid"].ToString() +
                          " and  ppid=" + drpp["ppid"].ToString()+
                          " and feepayed>0";
            getdatatable(ss11);

            string speroidmon = "";
            if (Convert.IsDBNull(dt.Rows[0][0])) speroidmon = "197507";
            else
            {
                string s1 = dt.Rows[0][0].ToString();
                System.DateTime dt_1_1 = DateTime.ParseExact(s1, "yyyyMMdd", new CultureInfo("zh-CN", true));

                System.DateTime dt_1_2 = new DateTime(dt_1_1.Year, dt_1_1.Month, 1);

                if (dt_1_2.AddMonths(1).AddDays(-1) == dt_1_1)//是当月最后一天
                {
                    speroidmon = dt_1_1.ToString("yyyyMM");
                }
                else
                {
                    speroidmon = dt_1_1.AddMonths(-1).ToString("yyyyMM");
                }
            }


            //判断是否被财务拆分了月度数据
            double dmax=0; string sdmonmax="";

            int isplitbyfnc = judgefncspilit(drpp, drct, ref sdmonmax, ref dmax);

            if (!bnmon)
            {

                string sdt = drpp["sdt"].ToString();
                string edt = drpp["edt"].ToString();

                System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
                System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));

                System.DateTime dt_s_s = dt_s;


                //免费期数
                int nfree = Convert.ToInt32(drct["rentfreeperiod"].ToString());
                int imons = 0;
                double fdisfee = 0;

                for (int ii = 0; ii <= 240; ii++)
                {
                    double rentfee = 0, bfee = 0;
                    string sfree = "否";

                    rentfee = Convert.ToDouble(drpp["rent"].ToString());
                    bfee = Convert.ToDouble(drpp["bfee"].ToString());


                    if (dt_s.AddMonths(1).AddDays(-1) > dt_e)
                    {
                        ii = 119;

                        int ikdays = (dt_e - dt_s).Days + 1;

                        rentfee = (ikdays / 30.0) * rentfee;
                        bfee = (ikdays / 30.0) * bfee;
                    }



                    if (nfree > 0)
                    {
                        rentfee = 0;
                        nfree--;
                        sfree = "是";
                    }

                    if (idistype != -1)
                    {
                        switch (idistype)
                        {
                            case 0: //按月递增
                                if (imons == 0)
                                {
                                    fdisfee = rentfee;
                                    break;
                                }

                                //fdisfee = fdisfee * (1 + idiscount / 100.0);
                                rentfee = rentfee * (1 + imons * idiscount / 100.0);

                                break;
                            case 1://按月递减
                                rentfee = rentfee * (1 - imons * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 2://按年递增
                                rentfee = rentfee * (1 + (imons / 12) * idiscount / 100.0);
                                break;
                            case 3://按年递减
                                rentfee = rentfee * (1 - (imons / 12) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;

                            default:
                                break;
                        }
                    }

                    //只调整未录入收费的应收
                    if (dt_s.ToString("yyyyMM").CompareTo(speroidmon) > 0)
                    {
                        //没有财务收费记录
                        string ss1 = "select 1 from t_fee_payed_fnc " +
                                   " where contractid=" + drct["contractid"].ToString() +
                                   " and ppid=" + drpp["ppid"].ToString() +
                                   " and feemonth='" + dt_s.ToString("yyyyMM") + "'" +
                                   " and payedfee>0";
                        getdatatable(ss1);


                        if (dt.Rows.Count == 0)
                        {
                            //update ,if not exist,insert
                            string sxx = "update t_fee_pay_mgt set rentfee=" + rentfee.ToString() + "," +
                                       " bfee=" + bfee.ToString() +
                                       " where contractid=" + drct["contractid"].ToString() +
                                       " and ppid=" + drpp["ppid"].ToString() +
                                       " and feemonth='" + dt_s.ToString("yyyyMM") + "'";
                            executecmd(sxx);

                            sxx = "select row_count()";
                            getdatatable(sxx);

                            if (Convert.ToInt32(dt.Rows[0][0]) == 0)
                            {
                                //生成fee_pay
                                sxx = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                                          drct["contractid"].ToString() + ",'" +
                                          drct["contractno"].ToString() + "'," +
                                          drpp["ppid"].ToString() + ",'" +
                                          drpp["unitno"].ToString() + "'," +
                                          drct["cusid"].ToString() + ",'" +
                                          drct["cusno"].ToString() + "','" +
                                          dt_s.ToString("yyyyMM") + "'," +
                                          rentfee.ToString() + "," +
                                          bfee.ToString() + ",'" +
                                          sfree + "')";

                                executecmd(sxx);
                            }
                        }
                    }


                    imons++;



                    //增加一个月，并设置为第一天
                    //dt_s = dt_s.AddMonths(1);
                    dt_s = dt_s_s.AddMonths(imons);

                    if (dt_s > dt_e)
                    {
                        //租期可能缩短（提前终止），删除后续费用
                        string xxs = "delete from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                                   " and ppid=" + drpp["ppid"].ToString() +
                                   " and feemonth>='" + dt_s.ToString("yyyyMM") + "'";
                        executecmd(xxs);

                        if (isplitbyfnc == 1)
                        {
                            //生成被删除的dmaxmonth的fee_pay
                            string saa = "";
                            saa = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                                      drct["contractid"].ToString() + ",'" +
                                      drct["contractno"].ToString() + "'," +
                                      drpp["ppid"].ToString() + ",'" +
                                      drpp["unitno"].ToString() + "'," +
                                      drct["cusid"].ToString() + ",'" +
                                      drct["cusno"].ToString() + "','" +
                                      dt_s.ToString("yyyyMM") + "'," +
                                      dmax.ToString() + "," +
                                      0.ToString() + ",'" +
                                      sfree + "')";

                            executecmd(saa);

                        }


                        break;
                    }

                    //dt_s = new DateTime(dt_s.Year, dt_s.Month, 1);

                }
            }
            else
            {
                string sdt = drpp["sdt"].ToString();
                string edt = drpp["edt"].ToString();

                System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
                System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));


                //免费期数
                int nfree = Convert.ToInt32(drct["rentfreeperiod"].ToString());
                int imons = 0;
                double fdisfee = 0;

                for (int ii = 0; ii < 240; ii++)
                {
                    double rentfee = 0, bfee = 0;
                    string sfree = "否";

                    rentfee = Convert.ToDouble(drpp["rent"].ToString());
                    bfee = Convert.ToDouble(drpp["bfee"].ToString());

                    //按自然月算月租，计算首月
                    if (dt_s.Day != 1)
                    {
                        int ndays = DateTime.DaysInMonth(dt_s.Year, dt_s.Month);

                        rentfee = rentfee * ((ndays - dt_s.Day) * 1.00 / ndays);
                        //rentfee = rentfee * ((20) * 1.00 / ndays);
                        rentfee = Math.Round(rentfee, 2);
                    }

                    //尾月不足一月
                    if (dt_s.AddMonths(1).AddDays(-1) > dt_e)
                    {
                        ii = 119;

                        int ikdays = (dt_e - dt_s).Days + 1;

                        rentfee = (ikdays / 30.0) * rentfee;
                        bfee = (ikdays / 30.0) * bfee;
                    }


                    if (nfree > 0)
                    {
                        rentfee = 0;
                        nfree--;
                        sfree = "是";
                    }

                    if (idistype != -1)
                    {
                        switch (idistype)
                        {
                            case 0: //按月递增
                                if (imons == 0)
                                {
                                    fdisfee = rentfee;
                                    break;
                                }

                                //fdisfee = fdisfee * (1 + idiscount / 100.0);
                                rentfee = rentfee * (1 + imons * idiscount / 100.0);

                                break;
                            case 1://按月递减
                                rentfee = rentfee * (1 - imons * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 2://按年递增
                                rentfee = rentfee * (1 + (imons / 12) * idiscount / 100.0);
                                break;
                            case 3://按年递减
                                rentfee = rentfee * (1 - (imons / 12) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;

                            default:
                                break;
                        }
                    }

                                        //只调整未录入收费的应收
                    if (dt_s.ToString("yyyyMM").CompareTo(speroidmon) > 0)
                    {
                        //没有财务收费记录
                        string ss1 = "select 1 from t_fee_payed_fnc " +
                                   " where contractid=" + drct["contractid"].ToString() +
                                   " and ppid=" + drpp["ppid"].ToString() +
                                   " and feemonth='" + dt_s.ToString("yyyyMM") + "'" +
                                   " and payedfee>0";
                        getdatatable(ss1);


                        if (dt.Rows.Count == 0)
                        {
                            //update ,if not exist,insert
                            string sxx = "update t_fee_pay_mgt set rentfee=" + rentfee.ToString() + "," +
                                       " bfee=" + bfee.ToString() +
                                       " where contractid=" + drct["contractid"].ToString() +
                                       " and ppid=" + drpp["ppid"].ToString() +
                                       " and feemonth='" + dt_s.ToString("yyyyMM") + "'";
                            executecmd(sxx);

                            sxx = "select row_count()";
                            getdatatable(sxx);

                            if (Convert.ToInt32(dt.Rows[0][0]) == 0)
                            {

                                //生成fee_pay
                                sxx = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                                          drct["contractid"].ToString() + ",'" +
                                          drct["contractno"].ToString() + "'," +
                                          drpp["ppid"].ToString() + ",'" +
                                          drpp["unitno"].ToString() + "'," +
                                          drct["cusid"].ToString() + ",'" +
                                          drct["cusno"].ToString() + "','" +
                                          dt_s.ToString("yyyyMM") + "'," +
                                          rentfee.ToString() + "," +
                                          bfee.ToString() + ",'" +
                                          sfree + "')";

                                executecmd(sxx);
                            }
                        }
                    }


                    imons++;



                    //增加一个月，并设置为第一天
                    dt_s = dt_s.AddMonths(1);
                    dt_s = new DateTime(dt_s.Year, dt_s.Month, 1);

                    if (dt_s > dt_e)
                    {
                        //租期可能缩短（提前终止），删除后续费用
                        string xxs = "delete from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                                   " and ppid=" + drpp["ppid"].ToString() +
                                   " and feemonth>='" + dt_s.ToString("yyyyMM") + "'";
                        executecmd(xxs);

                        if (isplitbyfnc == 1)
                        {
                            //生成被删除的dmaxmonth的fee_pay
                            string saa = "";
                            saa = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                                      drct["contractid"].ToString() + ",'" +
                                      drct["contractno"].ToString() + "'," +
                                      drpp["ppid"].ToString() + ",'" +
                                      drpp["unitno"].ToString() + "'," +
                                      drct["cusid"].ToString() + ",'" +
                                      drct["cusno"].ToString() + "','" +
                                      dt_s.ToString("yyyyMM") + "'," +
                                      dmax.ToString() + "," +
                                      0.ToString() + ",'" +
                                      sfree + "')";

                            executecmd(saa);

                        }

                        break;
                    }

                }

            }





            close();
        }

        //生成应收--按财务账期
        public void createfeepaymgt_zj(string ssdt,string sedt,double dzj,DataRow drct)
        {
            doconnnect();

            //
            string s123 = "delete from t_fee_pay_mgt where contractid=" + drct["contractid"].ToString() +
                        " and ppid=9999";
            executecmd(s123);

            s123 = "delete from t_con_pp where contractid=" + drct["contractid"].ToString() +
                        " and ppid=9999";
            executecmd(s123);

            s123 = "insert into t_con_pp(contractid,ppid,sdt,edt,rent,bfee,uarea) values(" +
                  drct["contractid"].ToString() + ",9999,'" +
                  ssdt + "','" +
                  sedt + "',0,0,0)";
            executecmd(s123);
                  



            //生成折旧按月分摊

            {

                string sdt = ssdt;
                string edt = sedt;

                System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
                System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));

                System.DateTime dt_s_s = dt_s;

                //免费期数
                int imons = 0;
                
                int ikk = 0;
                int iperiod=0;
                double dd123 = 0;

                switch(drct["rentpaystyle"].ToString())
                {
                    case "按年":
                        iperiod = 12;
                        break;
                    case "按季":
                        iperiod = 3;
                        break;
                    case "按月":
                        iperiod = 1;
                        break;
                    case "半年":
                        iperiod = 6;
                        break;
                    default:
                        iperiod = 3;
                        break;


                }



                double rentfeeb =System.Math.Floor( (dzj / iperiod)*100)/100.0;
                


                for (int ii = 0; ii <= 240; ii++)
                {
                    double bfee = 0,rentfee=0;
                    string sfree = "否";

                    ikk++;
                    if (ikk == iperiod)
                    {
                        rentfee = dzj - dd123;
                        dd123 = 0;
                        ikk = 0;
                    }
                    else
                    {
                        rentfee = rentfeeb;
                    }

 
                    if(ikk!=0)dd123+=rentfee;

                    //生成fee_pay
                    string sxx = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                               drct["contractid"].ToString() + ",'" +
                               drct["contractno"].ToString() + "'," +
                               "9999,'" +
                               "JKZJFT" + "'," +
                               drct["cusid"].ToString() + ",'" +
                               drct["cusno"].ToString() + "','" +
                               dt_s.ToString("yyyyMM") + "'," +
                               rentfee.ToString() + "," +
                               bfee.ToString() + ",'" +
                               sfree + "')";

                    executecmd(sxx);

                    imons++;



                    //增加一个月，并设置为第一天
                    //dt_s = dt_s.AddMonths(1);
                    dt_s = dt_s_s.AddMonths(imons);

                    if (dt_s > dt_e) break;

                    //dt_s = new DateTime(dt_s.Year, dt_s.Month, 1);

                }
            }
            


            close();
        }

        //生成应收--按财务账期
        public void createfeepaymgt(DataRow drpp, DataRow drct,Int16 idismons, float idiscount, int idistype, bool bnmon)
        {

            if (drpp["unitno"].ToString() == "JKZJFT") return;

            doconnnect();
            //生成应收--很复杂
            //按房间的总月数
            //财务应收

            if (!bnmon)
            {

                string sdt = drpp["sdt"].ToString();
                string edt = drpp["edt"].ToString();

                System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
                System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));

                System.DateTime dt_s_s = dt_s;

                //免费期数
                int nfree = Convert.ToInt32(drct["rentfreeperiod"].ToString());
                int imons = 0;
                double fdisfee = 0;

                for (int ii = 0; ii <= 240; ii++)
                {
                    double rentfee = 0, bfee = 0;
                    string sfree = "否";

                    rentfee = Convert.ToDouble(drpp["rent"].ToString());
                    bfee = Convert.ToDouble(drpp["bfee"].ToString());


                    if (dt_s.AddMonths(1).AddDays(-1) > dt_e)
                    {
                        ii = 119;

                        int ikdays = (dt_e - dt_s).Days + 1;

                        rentfee = (ikdays / 30.0) * rentfee;
                        bfee = (ikdays / 30.0) * bfee;
                    }



                    if (nfree > 0)
                    {
                        rentfee = 0;
                        nfree--;
                        sfree = "是";
                    }

                    if (idistype != -1)
                    {
                        switch (idistype)
                        {
                            case 0: //按月递增
                                if (imons == 0)
                                {
                                    fdisfee = rentfee;
                                    break;
                                }

                                //fdisfee = fdisfee * (1 + idiscount / 100.0);
                                rentfee = rentfee * (1 + imons * idiscount / 100.0);

                                break;
                            case 1://按月递减
                                rentfee = rentfee * (1 - imons * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 2://按年递增
                                rentfee = rentfee * (1 + (imons / 12) * idiscount / 100.0);
                                break;
                            case 3://按年递减
                                rentfee = rentfee * (1 - (imons / 12) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 4:
                                rentfee = rentfee * (1 + (imons / idismons) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 5:
                                rentfee = rentfee * (1 - (imons / idismons) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;

                            default:
                                break;
                        }
                    }

                    //生成fee_pay
                    string sxx = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                               drct["contractid"].ToString() + ",'" +
                               drct["contractno"].ToString() + "'," +
                               drpp["ppid"].ToString() + ",'" +
                               drpp["unitno"].ToString() + "'," +
                               drct["cusid"].ToString() + ",'" +
                               drct["cusno"].ToString() + "','" +
                               dt_s.ToString("yyyyMM") + "'," +
                               rentfee.ToString() + "," +
                               bfee.ToString() + ",'" +
                               sfree + "')";

                    executecmd(sxx);

                    imons++;



                    //增加一个月，并设置为第一天
                    //dt_s = dt_s.AddMonths(1);
                    dt_s = dt_s_s.AddMonths(imons);

                    if (dt_s > dt_e) break;

                    //dt_s = new DateTime(dt_s.Year, dt_s.Month, 1);

                }
            }
            else
            {
                string sdt = drpp["sdt"].ToString();
                string edt = drpp["edt"].ToString();

                System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
                System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));

               
                //免费期数
                int nfree = Convert.ToInt32(drct["rentfreeperiod"].ToString());
                int imons = 0;
                double fdisfee = 0;

                for (int ii = 0; ii < 240; ii++)
                {
                    double rentfee = 0, bfee = 0;
                    string sfree = "否";

                    rentfee = Convert.ToDouble(drpp["rent"].ToString());
                    bfee = Convert.ToDouble(drpp["bfee"].ToString());

                    if (dt_s.Day != 1)
                    {
                        int ndays = DateTime.DaysInMonth(dt_s.Year, dt_s.Month);
                        
                        rentfee = rentfee * ((ndays - dt_s.Day)*1.00 / ndays);
                        //rentfee = rentfee * ((20) * 1.00 / ndays);
                        rentfee=Math.Round(rentfee,2);
                    }

                    

                    if (nfree > 0)
                    {
                        rentfee = 0;
                        nfree--;
                        sfree = "是";
                    }

                    if (idistype != -1)
                    {
                        switch (idistype)
                        {
                            case 0: //按月递增
                                if (imons == 0)
                                {
                                    fdisfee = rentfee;
                                    break;
                                }

                                //fdisfee = fdisfee * (1 + idiscount / 100.0);
                                rentfee = rentfee * (1 + imons * idiscount / 100.0);

                                break;
                            case 1://按月递减
                                rentfee = rentfee * (1 - imons * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 2://按年递增
                                rentfee = rentfee * (1 + (imons / 12) * idiscount / 100.0);
                                break;
                            case 3://按年递减
                                rentfee = rentfee * (1 - (imons / 12) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;

                            case 4:
                                rentfee = rentfee * (1 + (imons / idismons) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;
                            case 5:
                                rentfee = rentfee * (1 - (imons / idismons) * idiscount / 100.0);
                                rentfee = System.Math.Round(rentfee, 2);
                                break;


                            default:
                                break;
                        }
                    }

                    //生成fee_pay
                    string sxx = "insert into t_fee_pay_mgt(contractid,contractno,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) values(" +
                               drct["contractid"].ToString() + ",'" +
                               drct["contractno"].ToString() + "'," +
                               drpp["ppid"].ToString() + ",'" +
                               drpp["unitno"].ToString() + "'," +
                               drct["cusid"].ToString() + ",'" +
                               drct["cusno"].ToString() + "','" +
                               dt_s.ToString("yyyyMM") + "'," +
                               rentfee.ToString() + "," +
                               bfee.ToString() + ",'" +
                               sfree + "')";

                    executecmd(sxx);

                    imons++;



                    //增加一个月，并设置为第一天
                    dt_s = dt_s.AddMonths(1);
                    dt_s = new DateTime(dt_s.Year, dt_s.Month, 1);

                    if (dt_s > dt_e) break;


                }

            }





            close();
        }


        public void addalt(DataRow dr, string msg)
        {
            //记载变更

            string str1 = "";

            str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["contractid"].ToString() + ",'contract','"+msg+"',now())";

            executecmd(str1);

        }

        public void changecontractstatus(DataRow dr,Int32 iseq,string msg="")
        {
            doconnnect();

            //提交审核
            if (iseq == 1)
            {
                string str1 = "update t_contract set contractstatus='等待审核' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid+","+
                     dr["contractid"].ToString() + ",'contract','提交审核',now())";
                executecmd(str1);
                     
            }


            //取消审核
            if (iseq == 0)
            {
                string str1 = "update t_contract set contractstatus='初登' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid + "," +
                     dr["contractid"].ToString() + ",'contract','"+msg+"',now())";
                executecmd(str1);
                     

            }

            //取消修改审核
            if (iseq == 8)
            {
                string str1 = "update t_contract set contractstatus='修改' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid + "," +
                     dr["contractid"].ToString() + ",'contract','"+msg+"',now())";
                executecmd(str1);
            
            }


            //审核通过,有一系列操作
            if (iseq == 2)
            {
                MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

               


                try
                {
                    string str2 = "update t_contract set contractstatus='已审核' where contractid=" + dr["contractid"].ToString();

                    executecmd(str2);

                    //修改ppunit
                    //string sss1 = "update t_ppunit set unitstatus='出租',contractid=" +
                    //            dr["contractid"].ToString() +
                    //            " where ppid in (select ppid from t_con_pp where contractid=" +
                    //            dr["contractid"].ToString() +")";

                    string sss1 = "update t_ppunit a, t_con_pp b  set  a.unitstatus='出租',a.unitrent=b.rent,a.unitbfee=b.bfee,a.contractid=b.contractid " +
                                  " where a.ppid=b.ppid and b.contractid=" +
                                dr["contractid"].ToString() + "";


                    executecmd(sss1);



                    mst.Commit();

                    //记载变更
                    str2 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                         jkwyjygl.Form1.uid + "," +
                         dr["contractid"].ToString() + ",'contract','审核通过',now())";
                    executecmd(str2);
                     
                }
                catch 
                {
                    mst.Rollback();
                    throw ;
                }
                finally
                {
                }
                

            }

            //申请修改
            if (iseq == 3)
            {
                string str1 = "update t_contract set contractstatus='申请修改' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid + "," +
                     dr["contractid"].ToString() + ",'contract','申请修改',now())";
                executecmd(str1);


            }

            //取消申请修改
            if (iseq == 4)
            {
                string str1 = "update t_contract set contractstatus='已审核' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid + "," +
                     dr["contractid"].ToString() + ",'contract','"+msg+"',now())";
                executecmd(str1);


            }

            //同意申请修改
            if (iseq == 5)
            {
                string str1 = "update t_contract set contractstatus='修改' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid + "," +
                     dr["contractid"].ToString() + ",'contract','同意申请修改',now())";
                executecmd(str1);


            }

            //提交修改审核
            if (iseq == 6)
            {
                string str1 = "update t_contract set contractstatus='等待修改审核' where contractid=" + dr["contractid"].ToString();

                executecmd(str1);

                //记载变更
                str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                     jkwyjygl.Form1.uid + "," +
                     dr["contractid"].ToString() + ",'contract','提交修改审核',now())";
                executecmd(str1);


            }

            //修改审核通过
            if (iseq == 7)
            {
                MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();




                try
                {
                    string str1 = "update t_contract set contractstatus='已审核' where contractid=" + dr["contractid"].ToString();

                    executecmd(str1);

                    
                    string sss1 = "update t_ppunit a, t_con_pp b  set  a.unitstatus='出租',a.unitrent=b.rent,a.unitbfee=b.bfee,a.contractid=b.contractid " +
                                  " where a.ppid=b.ppid and b.contractid=" + dr["contractid"].ToString() + 
                                  " and b.edt>=now()";


                    executecmd(sss1);



                    mst.Commit();

                    //记载变更
                    str1 = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                         jkwyjygl.Form1.uid + "," +
                         dr["contractid"].ToString() + ",'contract','修改审核通过',now())";
                    executecmd(str1);

                }
                catch
                {
                    mst.Rollback();
                    throw;
                }
                finally
                {
                }


            }

            close();
        }

        public void updateuser(DataRow dr)
        {
            doconnnect();

            string str1 = "update t_user set username='" + dr["username"].ToString() + "'," +
                          "userpassword='" + dr["userpassword"].ToString() + "'," +
                          "userstatus='" + dr["userstatus"].ToString() + "'," +
                          "userdesc='" + dr["userdesc"].ToString() + "' " +
                          " where userid=" + dr["userid"].ToString();

            executecmd(str1);

            //记载变更
            str1 = "insert into t_mgtalt(operuser,userid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["userid"].ToString() + ",'user','修改用户',now())";
            executecmd(str1);
                     

            close();


        }

        public void changepassword(string uid,string snewpass)
        {
            doconnnect();

            string str1 = "update t_user set userpassword='" +snewpass + "' "+
                          " where userid=" + uid;

            executecmd(str1);

            //记载变更
            str1 = "insert into t_mgtalt(operuser,userid,alttype,altmsg,altdt) values(" +
                 uid + "," +
                 uid + ",'user','修改密码',now())";
            executecmd(str1);
                     

            close();


        }

        //修改客户
        public void updatecusbyid(DataRow dr)
        {
            doconnnect();

            string str1 = "update t_cus set cusname='"+dr["cusname"].ToString()+"',"+
                          "cusmobnum='"+dr["cusmobnum"].ToString()+"',"+
                          "cusaddr='"+dr["cusaddr"].ToString()+"' "+
                          " where cusid=" + dr["cusid"].ToString();

            executecmd(str1);

            //记载变更
            str1 = "insert into t_mgtalt(operuser,cusid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["cusid"].ToString() + ",'cus','修改客户',now())";
            executecmd(str1);

            close();
        }

        public int updateppfeepaymgt_adjust(DataRow dr)
        {
            doconnnect();

            string str2 = "select 1 from t_fee_payed_fnc where contractid=" + dr["contractid"].ToString() +
                        " and ppid=" + dr["ppid"].ToString() +
                        " and feemonth='" + dr["feemonth"].ToString() + "'";
            getdatatable(str2);

            if (dt.Rows.Count > 0) return -1;

            string str1 = "update t_fee_pay_mgt set rentfee=" + dr["rentfee"].ToString() + "," +
                        "bfee=" + dr["bfee"].ToString() + " " +
                        "where feeid=" + dr["feeid"].ToString();
            executecmd(str1);

            close();

            return 1;
        }

        public void updateppfeepaymgt(DataRow dr)
        {
            doconnnect();

            string str1 = "update t_fee_pay_mgt set rentfee=" + dr["rentfee"].ToString() + "," +
                        "bfee=" + dr["bfee"].ToString() + " " +
                        "where feeid=" + dr["feeid"].ToString();
            executecmd(str1);

            close();
        }

        public string getmaxfncpaymon(DataRow dr)
        {
            string smon = "194910";

            string str1 = "select max(feemonth) from t_fee_payed_fnc where contractid=" + dr["contractid"].ToString()+
                          " and payedfee>0";

            getdatatable(str1);

            if(!Convert.IsDBNull(dt.Rows[0][0])) smon = dt.Rows[0][0].ToString();

            return smon;

        }

        public int updatefeepaymgt_adjust(DataRow dr)
        {
            doconnnect();

            string str2 = "select 1 from t_fee_payed_fnc where contractid=" + dr["contractid"].ToString() +
                        " and ppid=" + dr["ppid"].ToString() +
                        " and feemonth='" + dr["feemonth"].ToString() + "'";
            getdatatable(str2);

            if (dt.Rows.Count > 0) return -1;

            string str1 = "update t_fee_pay_mgt set rentfee=" + dr["rentfee"].ToString() + "," +
                        "bfee=" + dr["bfee"].ToString() + " " +
                        "where feeid=" + dr["feeid"].ToString();
            executecmd(str1);

            close();

            return 1;
        }

        public void updatefeepaymgt(DataRow dr)
        {
            doconnnect();

            string str1 = "update t_fee_pay_mgt  set rentfee=" + dr["rentfee"].ToString() + "," +
                        "bfee=" + dr["bfee"].ToString() +
                        " where feeid=" + dr["feeid"].ToString();

            executecmd(str1);


            close();
        }

        //修改conpp
        public void updateconpp(DataRow dr)
        {
            doconnnect();

            string str1 = "update t_con_pp set uarea=" + dr["uarea"].ToString() + "," +
                        "rent=" + dr["rent"].ToString() + ", " +
                        "bfee=" + dr["bfee"].ToString() + ", " +
                        "sdt='" + dr["sdt"].ToString() + "', " +
                        "edt='" + dr["edt"].ToString() + "' " +
                        "where cpid=" + dr["cpid"].ToString();

            executecmd(str1);


            close();
        }


        //修改ppunit
        public void updateppunitbyid(DataRow dr,object O=null)
        {
            doconnnect();

            string str1 = "update t_ppunit set unituarea=" + dr["unituarea"].ToString() + "," +
                        "unitrent=" + dr["unitrent"].ToString() + ", " +
                        "unitbfee=" + dr["unitbfee"].ToString() + ", " +
                        "unittype='" + dr["unittype"].ToString() + "', " +
                        "unitno='" + dr["unitno"].ToString() + "' ";
            if (O != null)
            {
                str1 += ", unitstatus='" + O.ToString() + "' ";
            }
            
            str1+= "where ppid=" + dr["ppid"].ToString();

            executecmd(str1);

            //记载变更
            str1 = "insert into t_mgtalt(operuser,ppid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["ppid"].ToString() + ",'pp','修改房间',now())";
            executecmd(str1);

            close();
        }

        //拆分ppunit
        public DataTable splitppunit(DataRow dr, string sppno, string spptype,string suarea, string spprent, string sppbfee)
        {
            doconnnect();

            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

            string str2 = "insert into t_ppunit(splitfromunit,unitno,unittype,unitorg,unitstatus,"+
                          "unituarea,unitarea,unitbuilding,unitlevel,"+
                          "unitrent,unitbfee) values(" +
                        dr["ppid"].ToString()+",'"+
                        sppno + "', '" +
                        spptype+ "', " +
                        "'拆分'," +
                        "'空闲'," +
                        suarea + "," +
                        dr["unitarea"].ToString() + "," +
                        dr["unitbuilding"].ToString() + "," +
                        dr["unitlevel"].ToString() + "," +
                        spprent+ "," +
                        sppbfee+")"
                        ;

            executecmd(str2);

            str2 = "select @@identity";

            getdatatable(str2);

            string snewppid = dt.Rows[0][0].ToString();

            str2 = "update t_ppunit set unitstatus='已拆' where ppid=" + dr["ppid"].ToString();

            executecmd(str2);

            mst.Commit();

            //记载变更
            str2 = "insert into t_mgtalt(operuser,ppid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["ppid"].ToString() + ",'pp','拆分房间',now())";
            executecmd(str2);

            close();

            return getppunitbyppid(snewppid);

 
        }

        //合并ppunit
        public DataTable combineppunit(DevExpress.XtraGrid.Views.Grid.GridView gv, 
                                  string sppno, string spptype, string suarea, string spprent, string sppbfee)
        {
            doconnnect();
            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

            DataRow dr = gv.GetFocusedDataRow();

            string str2 = "insert into t_ppunit(unitno,unittype,unitorg,unitstatus,unituarea,unitarea,unitbuilding,unitlevel,"+
                          "unitrent,unitbfee) values('" +
                        sppno + "', '" +
                        spptype+ "', " +
                        "'合并'," +
                        "'空闲'," +
                        suarea + "," +
                        dr["unitarea"].ToString() + "," +
                        dr["unitbuilding"].ToString() + "," +
                        dr["unitlevel"].ToString() + "," +
                        spprent + "," +
                        sppbfee + ")"
                        ;

            executecmd(str2);

            str2 = "select @@identity";

            getdatatable(str2);

            string sppid = dt.Rows[0][0].ToString();

            foreach (int i7 in gv.GetSelectedRows())
            {
                DataRow dr2 = gv.GetDataRow(i7);

                str2 = "update t_ppunit set  unitstatus='已并',combinetounit="+
                       sppid+
                       " where ppid=" + dr2["ppid"].ToString();

                executecmd(str2);
            }


            mst.Commit();

            //记载变更
            str2 = "insert into t_mgtalt(operuser,ppid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 sppid + ",'pp','合并房间',now())";
            executecmd(str2);

            close();

            return getppunitbyppid(sppid);

        }

        //使用dr方式增加房间
        public Int32 addppunit(DataRow dr)
        {
            Int32 iid;

            doconnnect();

            try
            {

                MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

                string str1 = "insert into  t_ppunit(unitno,unittype,unitorg,unitstatus,unituarea," +
                              "unitarea,unitbuilding,unitlevel,unitrent,unitbfee) values('" +
                              dr["unitno"].ToString() + "', '" +
                              dr["unittype"].ToString() + "', '" +
                              dr["unitorg"].ToString() + "','" +
                              dr["unitstatus"].ToString() + "'," +
                              dr["unituarea"].ToString() + "," +
                              dr["unitarea"].ToString() + "," +
                              dr["unitbuilding"].ToString() + "," +
                              dr["unitlevel"].ToString() + "," +
                              dr["unitrent"].ToString() + "," +
                              dr["unitbfee"].ToString() + ")";


                executecmd(str1);

                str1 = "select @@identity";

                getdatatable(str1);

                iid = Convert.ToInt32(dt.Rows[0][0].ToString());

                mst.Commit();
            }
            catch
            {
                mst.Rollback();
                throw;
            }


            close();

            return iid;
        }


        //添加ppunit
        public Int32  addppunit(string pparea, string ppbuilding, string pplevel, string pptype, string ppno,  string ppuarea,string pprent, string ppbfee)
        {
            doconnnect();

            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into  t_ppunit(unitno,unittype,unitorg,unitstatus,unituarea,unitarea,unitbuilding,unitlevel,unitrent,unitbfee) values('" +
                          ppno + "', '" +
                          pptype + "', " +
                          "'原始'," +
                          "'空闲'," +
                          ppuarea + "," +
                          pparea + "," +
                          ppbuilding + "," +
                          pplevel + "," +
                          pprent + "," +
                          ppbfee+")";

 
            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            Int32 iid =Convert.ToInt32( dt.Rows[0][0].ToString());

            mst.Commit();

            //记载变更
            str1 = "insert into t_mgtalt(operuser,ppid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 iid + ",'pp','新增单个房间',now())";
            executecmd(str1);


            close();

            return iid;
        }

        //修改area
        public void updatepparea(string sarea, string snewname, string snewcode, string snewdes)
        {
            doconnnect();

            string str1 = "update t_pp set ppname='" + snewname + "'," +
                        "ppcode='" + snewcode + "'," +
                        "ppdes='" + snewdes + "' " +
                        "where pptype='0' and id=" + sarea;

            executecmd(str1);

            close();
        }

        
    
        //删除paravalue
        public void deleteparavalue(string sid)
        {
            doconnnect();

            string str1 = "delete from t_syscode " +
                        "where id=" + sid;


            executecmd(str1);

            close();
        }

        public void deletegroupuser(DataRow dr, string suserid)
        {
            doconnnect();

            string str1 = "delete from t_user_group   " +
                        "where userid=" + suserid +
                        " and groupid=" + dr["id"].ToString();


            executecmd(str1);

            close();

        }

        public void deleteuser(string suserid)
        {
            doconnnect();

            string str1 = "delete from t_user  " +
                        "where userid=" + suserid;


            executecmd(str1);

            close();
        }

        public void deletecontract(string sctid)
        {
            doconnnect();

            string str1 = "delete from t_contract " +
                        "where contractid=" + sctid;


            executecmd(str1);

            close();
        }

        //删除cus
        public void deletecus(string scusid)
        {
            doconnnect();

            string str1 = "delete from t_cus " +
                        "where cusid=" + scusid;


            executecmd(str1);

            close();
        }

        //删除ppunit
        public void deleteppunit(string sppunitid)
        {
            doconnnect();

            string str1 = "delete from t_ppunit " +
                        "where ppid=" + sppunitid;


            executecmd(str1);

            close();
        }

        //删除pp
        public void deletepp(string stype,string sppid)
        {
            doconnnect();

            string str1 = "delete from t_pp "+
                        "where pptype='"+stype+"' "+
                        "and id=" + sppid;

            executecmd(str1);

            close();
        }


        public bool cthaspayfee(DataRow dr)
        {
            bool rt = true;

            doconnnect();
            string str2 = "select 1 from t_fee_payed_fnc where contractid=" + dr["contractid"].ToString() +
                        " and payedfee>0";
            getdatatable(str2);

            if (dt.Rows.Count > 0) return rt;

            str2 = "select 1 from t_fee_pay_mgt_period where contractid=" + dr["contractid"].ToString() +
                        " and feepayed>0";
            getdatatable(str2);

            if (dt.Rows.Count > 0) return rt;

            close();

            return false;
        }
        //

        public bool delcon_pp(DataRow dr)
        {
            bool rt = false;

            doconnnect();

            //判断是否有财务收费
            string str2 = "select 1 from t_fee_payed_fnc where contractid=" + dr["contractid"].ToString() +
                        " and ppid=" + dr["ppid"].ToString() +
                        " and payedfee>0";
            getdatatable(str2);

            if (dt.Rows.Count > 0) return rt;

            //判断是否有经营收费
            str2 = "select 1 from t_fee_pay_mgt_period where contractid=" + dr["contractid"].ToString() +
                        " and ppid=" + dr["ppid"].ToString() +
                        " and feepayed>0";
            getdatatable(str2);

            if (dt.Rows.Count > 0) return rt;

            string str1 = "delete from t_con_pp where cpid=" + dr["cpid"].ToString();
            executecmd(str1);


            //删除收费
            str1 = "delete from t_fee_pay_mgt where  contractid=" + dr["contractid"].ToString() +
                        " and ppid=" + dr["ppid"].ToString();
            
            executecmd(str1);

            str1 = "delete from t_fee_pay_mgt_period where  contractid=" + dr["contractid"].ToString() +
                        " and ppid=" + dr["ppid"].ToString();

            executecmd(str1);

            //置空房间
            str1 = "update t_ppunit set unitstatus='空闲',contractid=null  "+
                   " where  contractid=" + dr["contractid"].ToString() +
                   " and ppid=" + dr["ppid"].ToString();

            executecmd(str1);


            close();


            rt = true;
            return rt;
        }

        public void removegroupprivilege(string sgroupid, string sprivilegeid)
        {
            doconnnect();

            string str1 = "delete from t_groupprivilege where groupid=" +
                          sgroupid + " and spid=" +
                          sprivilegeid;

            executecmd(str1);


            close();


        }


        public Int32 addgroupprivilege(string sgroupid,string sprivilegeid)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_groupprivilege(groupid,spid) values(" +
                          sgroupid + "," +
                          sprivilegeid + ")"
                          ;

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            Int32 idadd = Convert.ToInt32(dt.Rows[0][0].ToString());

            mst.Commit();

            close();

            return idadd;
        }

        public Int32 addcon_pp(DataRow dr)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_con_pp(ppid,contractid,sdt,edt,uarea,rent,bfee) values('" +
                          dr["ppid"].ToString() + "','" +
                          dr["contractid"].ToString() + "','" +
                          dr["sdt"].ToString() + "','" +
                          dr["edt"].ToString() + "'," +
                          dr["uarea"].ToString() + "," +
                          dr["rent"].ToString() + "," +
                          dr["bfee"].ToString() + ")"
                          ;

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            Int32 idadd = Convert.ToInt32(dt.Rows[0][0].ToString());

            mst.Commit();

            //记载变更
            str1 = "insert into t_mgtalt(operuser,contractid,ppid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["contractid"].ToString() + ","+
                 dr["ppid"].ToString() +",'pp','加入合同',now())";
            executecmd(str1);

            close();

            return idadd;
        }

        public Int32 addparavalue(string scatname, string scat, string sparavalue,string seqno)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_syscode(paracatname,paraname,paravalue,paraseqno) values('"+
                          scatname+"','"+
                          scat+"','"+
                          sparavalue+"',"+
                          seqno+")";

            executecmd(str1);

            str1="select @@identity";

           getdatatable(str1);

            int idadd =Convert.ToInt32( dt.Rows[0][0].ToString());

 
            mst.Commit();

            close();

            return idadd;
        }

        //添加pparea
        public Int32 addpparea(string sareacode, string sareaname,string sareades )
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_pp(parentid,pptype,ppcode,ppname,ppdes) values(0,'0','" + sareacode + "','" +
                          sareaname + "','" +
                          sareades + "')";

            executecmd(str1);

            str1="select @@identity";

           getdatatable(str1);

            int idadd =Convert.ToInt32( dt.Rows[0][0].ToString());

 
            mst.Commit();


            str1 = "insert into t_ppgrp(ppaid,ppcode,ppgrp) values(" + idadd.ToString() +
                ",'" + sareacode + "','" + idadd.ToString() + "')";

            executecmd(str1);

            close();

            return idadd;
        }

        //添加ppbuilding
        public Int32 addppbuilding(string sparentid, string sbuildingcode, string sbuildingname, string sbuildingdes)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_pp(parentid,pptype,ppcode,ppname,ppdes) values("+
                          sparentid+","+
                          "'1','" +
                          sbuildingcode + "','" +
                          sbuildingname + "','" +
                          sbuildingdes + "')";

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;

        }


        public void updatecontracttext(DataRow dr)
        {
            doconnnect();

            string supd = "update t_contract set contracttext='"+dr["contracttext"].ToString()+"' "+
                          "where contractid=" + dr["contractid"].ToString();

            executecmd(supd);

            //记载变更
            supd = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["contractid"].ToString() + ",'contract','修改合同备注',now())";
            executecmd(supd);


            close();
        }

        public void updatecontract(DataRow dr)
        {
            doconnnect();

            string supd = "update t_contract set rentpaystyle='" +
                          dr["rentpaystyle"].ToString() + "'," +
                          "rentfreeperiod=" + dr["rentfreeperiod"].ToString() + "," +
                          "depositfee=" + dr["depositfee"].ToString() + "," +
                          "unittarget='" + dr["unittarget"].ToString() + "'," +
                          "contractsdt='" + dr["contractsdt"].ToString() + "'," +
                          "contractedt='" + dr["contractedt"].ToString() + "'," +
                          "signdt='" + dr["signdt"].ToString() + "' "+
                          "where contractid=" + dr["contractid"].ToString();

            executecmd(supd);

            //记载变更
            supd = "insert into t_mgtalt(operuser,contractid,alttype,altmsg,altdt) values(" +
                 jkwyjygl.Form1.uid + "," +
                 dr["contractid"].ToString() + ",'contract','修改合同',now())";
            executecmd(supd);


            close();
        }


        public DataTable getcontractatt(DataRow dr)
        {
            doconnnect();

            string str = "select * from t_contract_att where contractid=" + dr["contractid"].ToString();
            getdatatable(str);


            close();

            return dt;
        }

        //添加contract-att
        public void addcontractatt(DataRow dr, string satt,string sname)
        {

            doconnnect();

            string supd = "insert into t_contract_att(contractid,attname,attachment) values("+
                          dr["contractid"].ToString()+",'"+
                          sname+"',@att)";

            //处理picture/BLOB

            if (satt != "")
            {


                System.IO.FileStream fs = new System.IO.FileStream(satt, System.IO.FileMode.Open, System.IO.FileAccess.Read);


                byte[] BlobValue = new byte[fs.Length];

                fs.Read(BlobValue, 0, BlobValue.Length);

                fs.Close();


                MySql.Data.MySqlClient.MySqlCommand dbcmd = new MySql.Data.MySqlClient.MySqlCommand(supd, mysqldb);

                MySql.Data.MySqlClient.MySqlParameter BlobParam =
                    new MySql.Data.MySqlClient.MySqlParameter("@att", MySql.Data.MySqlClient.MySqlDbType.Binary);
                dbcmd.Parameters.Add(BlobParam);
                BlobParam.Value = BlobValue;

                dbcmd.ExecuteNonQuery();
            }


            close();

        }

        //更新contract-att
        public void updatecontractatt(DataRow dr, string satt,string sname)
        {

            doconnnect();

            string supd = "update t_contract_att set attachment=@att,attname='"+
                          sname+"' where contractid=" + dr["contractid"].ToString();

            //处理picture/BLOB

            if (satt != "")
            {


                System.IO.FileStream fs = new System.IO.FileStream(satt, System.IO.FileMode.Open, System.IO.FileAccess.Read);


                byte[] BlobValue = new byte[fs.Length];

                fs.Read(BlobValue, 0, BlobValue.Length);

                fs.Close();


                MySql.Data.MySqlClient.MySqlCommand dbcmd = new MySql.Data.MySqlClient.MySqlCommand(supd, mysqldb);

                MySql.Data.MySqlClient.MySqlParameter BlobParam =
                    new MySql.Data.MySqlClient.MySqlParameter("@att", MySql.Data.MySqlClient.MySqlDbType.Binary);
                dbcmd.Parameters.Add(BlobParam);
                BlobParam.Value = BlobValue;

                dbcmd.CommandTimeout = 60 * 5;
                dbcmd.ExecuteNonQuery();
            }


            close();

        }

        public bool checkcthasatt(DataRow dr)
        {
            bool b = false;

            doconnnect();

            string str = "select 1 from t_contract_att where contractid=" + dr["contractid"].ToString();
            getdatatable(str);

            if (dt.Rows.Count > 0) b = true;

            close();

            return b;
        }

        public Int32 addcontract(DataRow dr)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string s111 = "insert into t_contract(contractno,contractstatus,contractorg,signdt," +
                             "contractarea,contractpptype,unittarget,rentfreeperiod,rentpaystyle,depositfee," +
                             "contractsdt,contractedt,cusid,contracttext) values('" +
                             dr["contractno"].ToString() + "','" +
                             dr["contractstatus"].ToString() + "','" +
                             dr["contractorg"].ToString() + "','" +
                             dr["signdt"].ToString() + "','" +
                             dr["contractarea"].ToString() + "','" +
                             dr["contractpptype"].ToString() + "','" +
                             dr["unittarget"].ToString() + "'," +
                             dr["rentfreeperiod"].ToString() + ",'" +
                             dr["rentpaystyle"].ToString() + "'," +
                              dr["depositfee"].ToString() + ",'" +
                                   dr["contractsdt"].ToString() + "','" +
                                   dr["contractedt"].ToString() + "'," +
                                   dr["cusid"].ToString() + ",null)";
            executecmd(s111);
            

            string str1 = "select @@identity";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();


            close();

            return idadd;
        }


        public Int32 addgroupuser(string sgroupid,DataRow dr)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();


            string str1 = "insert into t_user_group(groupid,userid) values(" +
                sgroupid + "," +
                dr["userid"].ToString() + ")";
            ;

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;

        }

        public Int32 addignoreinform(string scontractid,string ssubject)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();


            string str1 = "insert into t_inform(userid,ignorecontractid,ignoredmsg) values(" +
                jkwyjygl.Form1.uid+","+
                scontractid + ",'" +
                ssubject + "')";
            ;

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;

        }

        public Int32 adduser(DataRow dr)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();


            string str1 = "insert into t_user(username,userpassword,userstatus,userdesc) values('" +
                dr["username"].ToString() + "','" +
                dr["userpassword"].ToString() + "','" +
                "禁用" + "','" +
                dr["userdesc"].ToString() + "')";
            ;

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());

            str1 = "insert into t_userpara(userid,bupd,isif,ifdays,tlwidth) values(" +
                idadd.ToString() + ",0,1,7,200)";

            executecmd(str1);

            mst.Commit();

            close();

            return idadd;

        }


        //添加cus
        public Int32 addcus(DataRow dr)
        {
            doconnnect();

            int idadd = -1;
            try
            {

                mst = mysqldb.BeginTransaction();


                string str1 = "insert into t_cus(cusno,cusarea,cusname,cusmobnum,cusaddr) values('" +
                    dr["cusno"].ToString() + "'," +
                    dr["cusarea"].ToString() + ",'" +
                    dr["cusname"].ToString() + "','" +
                    dr["cusmobnum"].ToString() + "','" +
                    dr["cusaddr"].ToString() + "')";
                ;

                executecmd(str1);

                str1 = "select @@identity";

                getdatatable(str1);

                idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


                mst.Commit();

            }
            catch
            {
                mst.Rollback();
                throw;
            }


            close();

            return idadd;

        }


        //添加level
        public Int32 addpplevel(string sparentid, string slevelcode, string slevelname, string sleveldes)
        {
            doconnnect();

            MySqlTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_pp(parentid,pptype,ppcode,ppname,ppdes) values(" +
                          sparentid + "," +
                          "'2','" +
                          slevelcode + "','" +
                          slevelname + "','" +
                          sleveldes + "')";

            executecmd(str1);

            str1 = "select @@identity";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;
        }

        //修改building
        public void updateparavalue(string sid, string snewvalue, string snewseqno)
        {
            doconnnect();

            string str1 = "update t_syscode set paravalue='" + snewvalue + "'," +
                        "paraseqno='" + snewseqno + "' "+
                        "where  id=" + sid;

            executecmd(str1);

            close();
        }

        //修改building
        public void updateppbuilding(string sbuilding, string snewname, string snewcode, string snewdes)
        {
            doconnnect();

            string str1 = "update t_pp set ppname='" + snewname + "'," +
                        "ppcode='" + snewcode + "'," +
                        "ppdes='" + snewdes + "' " +
                        "where pptype='1' and id=" + sbuilding;

            executecmd(str1);

            close();
        }

        //修改level
        public void updatepplevel(string slevel, string snewname, string snewcode, string snewdes)
        {
            doconnnect();

            string str1 = "update t_pp set ppname='" + snewname + "'," +
                        "ppcode='" + snewcode + "'," +
                        "ppdes='" + snewdes + "' " +
                        "where pptype='2' and id=" + slevel;

            executecmd(str1);

            close();
        }

        public DataTable getallcusbyarea(string sarea)
        {
            doconnnect();

            string str = "select cusid,cusno,cusname,cusmobnum,cusaddr from t_cus where cusarea="+sarea+" order by cusid desc";
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getnullpp()
        {
            doconnnect();

            string str = "select unitno as levelno,ppid as units,unittype,unituarea,unitrent,unitbfee " +
                         "from t_ppunit where 1=2";
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getnullpp2()
        {
            doconnnect();

            string str = "select unitno,unittype,unitorg,unitstatus,unituarea,"+
                         "unitrent,unitbfee,unitarea,unitbuilding,unitlevel " +
                         "from t_ppunit where 1=2";


            getdatatable(str);

            close();

            return dt;
        }

        public void updatefeepayedseqmgt(DataRow  dr,string seq)
        {
            doconnnect();

            string str1 = "update t_fee_pay_mgt_period set feepayed=feepay"+
                        ",feepayedctseq="+seq+
                        " where contractid=" +
                        dr["contractid"].ToString() +
                        " and feepaysdt='" + dr["feepaysdt"].ToString() + "'";

            executecmd(str1);

            close();
        }

        public int addfeepaymgtbyfnc(DataRow dr,string sfee, string snewmon)
        {
            doconnnect();

            string str11 = "select 1 from t_fee_pay_mgt where contractid=" + dr["contractid"].ToString() +
                         " and ppid=" + dr["ppid"].ToString() +
                         " and feemonth='" + snewmon + "'";
            getdatatable(str11);

            if (dt.Rows.Count > 0)
            {
                close();

                return 0;
            }

            string str2 = "select cusid from t_contract where contractid=" + dr["contractid"].ToString();
            getdatatable(str2);
            string scusid = dt.Rows[0][0].ToString();

            str2 = "select cusno from t_cus where cusid=" + scusid;
            getdatatable(str2);
            string scusno = dt.Rows[0][0].ToString();

            
            string str1 = "insert into t_fee_pay_mgt(contractid,contractno,contractnofnc,ppid,unitno,cusid,cusno,feemonth,rentfee,bfee,feefree) " +
                        " values(" + dr["contractid"].ToString() + ",'" +
                        dr["contractno"].ToString() + "','" +
                        dr["contractnofnc"].ToString() + "'," +
                        dr["ppid"].ToString() + ",'" +
                        dr["unitno"].ToString() + "'," +
                        scusid + ",'" +
                        scusno + "','" +
                        snewmon + "',"+
                        Convert.ToDouble(sfee)+" ,0,'否')";

            executecmd(str1);



            close();

            return 1;

        }

        public string addfeepayedseqmgt(string sfeepayed,string sct,string sctno)
        {
             doconnnect();

            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

            string str = "insert into t_fee_payed_seq_con_mgt(contractid,contractno,payeddate,payedfee) values(" +
                       sct + ",'" + sctno + "',now()," + sfeepayed + ")";

            executecmd(str);

            string str2 = "select @@identity";

            getdatatable(str2);

            string snewseq = dt.Rows[0][0].ToString();

            mst.Commit();
            close();

            return snewseq;


        }

        public void savefncfeechange(DevExpress.XtraGrid.Views.Grid.GridView gvpp)
        {
            doconnnect();

            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

            //更新应收：财务可能会更改非1号开始合同（半月合同）的应收：改首月和尾月
            for (int ii = 0; ii < gvpp.RowCount; ii++)
            {

                DataRow d123 = gvpp.GetDataRow(ii);
                string s118 = "update t_fee_pay_mgt set rentfee=" + d123["fee"].ToString() +
                            " where contractid=" + d123["contractid"].ToString() +
                            " and ppid=" + d123["ppid"].ToString() +
                            " and feemonth='" + d123["feemonth"].ToString() + "' " +
                            " and rentfee!=" + d123["fee"].ToString();

                executecmd(s118);
            }

            mst.Commit();
            close();

            return;
        }


        public void addfeepayedseq(DataRow dr, string stotal, DevExpress.XtraGrid.Views.Grid.GridView gvpp,string srptmon)
        {
            doconnnect();

            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();


            string snewseq=""; 
            //添加收费主记录
//            if (dr["feepayedctseq"].ToString() == "-1")
            {
                string str = "insert into t_fee_payed_seq_con_fnc(contractid,contractno,contractnofnc,payeddate,rptmon,payedfee,isconfirmed) values(" +
                           dr["contractid"].ToString() + ",'" +
                           dr["contractno"].ToString() + "','" +
                           dr["contractnofnc"].ToString() + "','" +
                           System.DateTime.Now.ToString("yyyyMMdd") + "','" +
                           srptmon+"',"+
                           stotal+ ",'否')";  //财务部门应收，需要复核

                executecmd(str);

                string str2 = "select @@identity";

                getdatatable(str2);

                snewseq = dt.Rows[0][0].ToString();
            }
            //else
            //{
            //    string str="update t_fee_payed_seq_con_fnc set payeddate='"+System.DateTime.Now.ToString("yyyyMMdd")+
            //               "',
            //}

            //添加收费记录
            for (int ii = 0; ii < gvpp.RowCount; ii++)
            {
                DataRow d123 = gvpp.GetDataRow(ii);
//                if (d123["feepayedctseq"].ToString() == "-1")
                {
                    if (Convert.ToDouble(d123["feepayednow"].ToString()) <= 0) continue;


                    {
                        string s1 = "insert into t_fee_payed_seq_fnc(feepayedctseq,contractid,ppid,feemonth,payfee,feepayedprev,feepayednow) values(" +
                                  snewseq + "," +
                                  dr["contractid"].ToString() + "," +
                                  d123["ppid"].ToString() + ",'" +
                                  d123["feemonth"].ToString() + "'," +
                                  d123["fee"].ToString() + "," +
                                  d123["feepayed"].ToString() + "," +
                                  d123["feepayednow"].ToString() + ")";
                        executecmd(s1);
                    }
                }

                string s19 = "update t_fee_payed_fnc set payedfee=payedfee+" + d123["feepayednow"].ToString() +
                             " where feepayedid=" + d123["feepayedid"].ToString();

                executecmd(s19);

                s19 = "select row_count()";
                getdatatable(s19);

                if (Convert.ToInt32(dt.Rows[0][0]) == 0)
                {
                    //直接生成收费细表，经营部门
                    //          if (d123["feepayedid"].ToString() == "-1")
                    {
                        string s1 = "insert into t_fee_payed_fnc(contractid,ppid,feemonth,payedfee) values(" +
                                   dr["contractid"].ToString() + "," +
                                   d123["ppid"].ToString() + ",'" +
                                   d123["feemonth"].ToString() + "'," +
                                   d123["feepayednow"].ToString() + ")";

                        executecmd(s1);
                    }
                }


 

            }

            mst.Commit();
            close();

            return;
        }

        public DataTable getcontractfeepayed(string sct, string O,bool ball)
        {
            doconnnect();

            string strs="select a.contractid,a.contractno,a.contractnofnc,a.feemonth,"+
                        "sum(a.rentfee+a.bfee) as fee,sum(ifnull(c.payedfee,0)) as feepayed from t_fee_pay_mgt a "+
                        "left outer join t_fee_payed_fnc c on "+
                        "a.contractid=c.contractid and a.ppid=c.ppid and a.feemonth=c.feemonth "+
                        //"left outer join t_fee_payed_seq_fnc b "+
                        //"on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth "+
                        "where a.contractid="+sct;

            if (O != "")
            {
                strs += " and a.feemonth<='" + O.ToString() + "' ";
            }

            strs += " group by a.contractid,a.feemonth ";

            if (!ball)
            {
                strs += " having fee>feepayed ";
            }

            strs+=    " order by a.feemonth ";

            getdatatable(strs);

            //string str1 = "select feepayedctseq from t_fee_payed_seq_con_fnc where contractid=" + sct; 
            //              //+  " and isconfirmed='否'";

            //getdatatable(str1);

            //if (dt.Rows.Count > 0)
            //{
            //    string str = "select a.feepayedctseq,b.contractid,b.contractno,b.contractnofnc,b.feemonth, sum(b.rentfee+b.bfee) as fee," +
            //                " sum(ifnull(c.payedfee,0)) as feepayed " +
            //                " from t_fee_payed_seq_fnc a join t_fee_pay_mgt b " +
            //                " on a.contractid=b.contractid and  a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //                " left outer join t_fee_payed_fnc c " +
            //                " on a.contractid=c.contractid and  a.ppid=c.ppid and a.feemonth=c.feemonth " +
            //                " where a.feepayedctseq=" + dt.Rows[0]["feepayedctseq"].ToString() +
            //                " group by a.contractid,a.feemonth "; 

            //    getdatatable(str);
            //}
            //else
            //{


            //    string str = "select -1 as feepayedctseq,  a.contractid,a.contractno,a.contractnofnc,a.feemonth, sum(a.rentfee+a.bfee) as fee," +
            //                 " sum(ifnull(b.payedfee,0)) as feepayed " +
            //                 " from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
            //                 " on a.contractid=b.contractid and  a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //                 " where a.contractid=" + sct;
            //    if (O != "")
            //    {
            //        str += " and a.feemonth<='" + O.ToString() + "' ";
            //    }
            //    str += " group by a.contractid,a.feemonth " +
            //                 " having fee>feepayed " +
            //                 " order by a.feemonth ";

            //    getdatatable(str);
            //}

            close();

            return dt;
        }

        public DataTable getppfeepayedbycontract(DevExpress.XtraGrid.Views.Grid.GridView gv)
        {
            doconnnect();

            string strs = "select ifnull(c.feepayedid,-1) as feepayedid,a.ppid,a.unitno, a.contractid,a.contractno,a.contractnofnc,a.feemonth," +
                        "(a.rentfee+a.bfee) as fee,ifnull(c.payedfee,0) as feepayed,0.0 as feepayednow "+
                        "  from t_fee_pay_mgt a " +
                        " left outer join t_fee_payed_fnc c on "+
                        "  a.contractid=c.contractid and a.ppid=c.ppid and a.feemonth=c.feemonth "+
                        " where a.contractid="+gv.GetDataRow(0)["contractid"].ToString();

            string ss = " in ('0'";
            for (int kk = 0; kk < gv.RowCount; kk++)
            {
                DataRow ddr = gv.GetDataRow(kk);
                ss += ",'" + ddr["feemonth"].ToString() + "'";
            }
            ss += ")";

            strs = strs + " and a.feemonth " + ss;
            strs += " order by a.unitno,a.feemonth";

            getdatatable(strs);
            
            //if (gv.GetDataRow(0)["feepayedctseq"].ToString() != "-1")
            //{
            //    string str= "select a.feepayedctseq,ifnull(c.feepayedid,-1) as feepayedid, b.ppid,b.unitno,b.feemonth, (b.rentfee+b.bfee) as fee," +
            //                " (ifnull(c.payedfee,0)) as feepayed, a.feepayednow " +
            //                " from t_fee_payed_seq_fnc a join t_fee_pay_mgt b " +
            //                " on a.contractid=b.contractid and  a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //                " left outer join t_fee_payed_fnc c "+
            //                " on a.contractid=c.contractid and  a.ppid=c.ppid and a.feemonth=c.feemonth " +
            //                " where a.feepayedctseq=" + gv.GetDataRow(0)["feepayedctseq"].ToString();

            //    getdatatable(str);

            //}
            //else
            //{

            //    string ss = " in ('0'";
            //    for (int kk = 0; kk < gv.RowCount; kk++)
            //    {
            //        DataRow ddr = gv.GetDataRow(kk);
            //        ss += ",'" + ddr["feemonth"].ToString() + "'";
            //    }
            //    ss += ")";

            //    string str = "select -1 as feepayedctseq, ifnull(b.feepayedid,-1) as feepayedid,a.ppid,a.unitno,a.feemonth, (a.rentfee+a.bfee) as fee," +
            //                 " (ifnull(b.payedfee,0)) as feepayed,0.0 as feepayednow " +
            //                 " from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
            //                 " on a.contractid=b.contractid and  a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //                 " where a.contractid=" + gv.GetDataRow(0)["contractid"].ToString() +
            //                 " and a.feemonth " + ss;


            //    getdatatable(str);
            //}

            close();

            return dt;
        }

        public DataTable getppfeepayfncbycontract(DataRow dr)
        {
            doconnnect();

            string str = "select a.ppid,a.unitno,'" + dr["feemonth"].ToString() + "' as feemonth," +
                  " sum(a.rentfee+a.bfee) as fee," +
                  " sum(ifnull(b.payedfee,0)) as feepayed " +
                  " from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
                  " on a.contractid=b.contractid and  a.ppid=b.ppid and a.feemonth=b.feemonth " +
                  " where a.contractid="+dr["contractid"].ToString()+
                  " and a.feemonth<='" + dr["feemonth"].ToString() + "'" +
                  " group by a.ppid ";


            getdatatable(str);


            close();

            return dt;
        }

        public DataTable getcontractfeepaymgtbyspan(string sid,string sdt,string edt)
        {
            doconnnect();

            string str = "select a.contractid,a.contractno,feepaysdt,feepayedt, sum(feepay) as feepay, sum(ifnull(feepayed,0)) as feepayed  " +
                            " from t_fee_pay_mgt_period a" +
                             " where a.contractid=" + sid +
                             //" and feepaysdt>='"+sdt+"'"+
                             " and feepaysdt<='"+edt+"'"+
                             //" and ifnull(feepayed,0)=0"+
                             " group by a.contractid,a.feepaysdt,a.feepayedt "+
                             " having feepayed<feepay order by a.feepaysdt";

            getdatatable(str);


            close();

            return dt;
        }

        public DataTable getcontractrptfeemgt(string sarea, string smon)
        {
            doconnnect();
            string s1=smon+"01";
            string s2=smon+"31";

            //string str="select b.*,c.cusname,d.units from (select a.contractid,a.contractno,feepaysdt,feepayedt,a.cusid, "+
            //           " sum(feepay) as feepay, sum(ifnull(feepayed,0)) as feepayed"+ 
            //           " from t_fee_pay_mgt_period a where "+
            //           " a.feepaysdt>='"+s1+"' and a.feepaysdt<='"+s2+"'"+
            //           " and  a.contractid in (select distinct contractid from t_contract where contractarea="+sarea+")"+
            //           " group by a.contractid,a.feepaysdt,a.feepayedt ) b join t_cus c on b.cusid=c.cusid "+
            //           " join (select contractid,feepaysdt,group_concat(unitno SEPARATOR ',') as units from t_fee_pay_mgt_period"+
            //           " group by contractid,feepaysdt) d on b.contractid=d.contractid and b.feepaysdt=d.feepaysdt "+
            //           " order by b.feepaysdt,c.cusname";

            string str = "select substr(b.payeddate,1,4) as tY," +
                       "substr(b.payeddate,6,2) as tM," +
                       "substr(b.payeddate,9,2) as tD," +
                       "e.ppname," +
                       "f.ppname," +
                       "g.ppname," +
                       "d.UnitType," +
                       "d.UnitNO," +
                       "h.cusname," +
                       "'租金' as feetype," +
                       "concat(substr(a.feepaysdt,1,4),'.',substr(a.feepaysdt,5,2),'.',substr(a.feepaysdt,7,2),'-'," +
                       "substr(a.feepayedt,1,4),'.',substr(a.feepayedt,5,2),'.',substr(a.feepayedt,7,2)) as sedt," +
                       "a.feepayed " +
                       "from t_fee_pay_mgt_period a," +
                       "t_fee_payed_seq_con_mgt b," +
                       "t_cus h,"+
                       "t_ppunit d," +
                       "t_pp e," +
                       "t_pp f," +
                       "t_pp g " +
                       "where a.feepayedctseq=b.feepayedctseq and " +
                       "a.ppid=d.ppid  and " +
                       "a.cusid=h.cusid and "+
                       "d.unitarea=e.id and " +
                       "d.unitbuilding=f.id and " +
                       "d.unitlevel=g.id and " +
                       "d.unitarea=" + sarea + " and " +
                       "substr(b.payeddate,1,4)='" + smon.Substring(0, 4) + "' " +
                       " order by ty,tm,td,e.ppname,f.ppname,g.ppname"
                       
                       ;


            getdatatable(str);

            close();

            return dt;

        }

        public DataTable getcontractfeepaymgt(string sid)
        {
            doconnnect();

            string str = "select a.contractid,a.contractno,feepaysdt,feepayedt, sum(feepay) as feepay, sum(ifnull(feepayed,0)) as feepayed  " +
                            " from t_fee_pay_mgt_period a"+
                             " where a.contractid=" +sid +
                             " group by a.contractid,a.feepaysdt,a.feepayedt order by a.feepaysdt";

            getdatatable(str);


            close();

            return dt;
        }


  
        public DataTable getcontractfeepayfnc(string O,string sarea,string sbuilding ,string slevel,string spp)
        {
            doconnnect();

            string str = "select c.cusname,a.contractid,a.contractno,a.contractnofnc,a.feemonth," +
                             " sum(a.rentfee+a.bfee) as fee," +
                             " sum(ifnull(b.payedfee,0)) as feepayed " +
                             " from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
                             " on a.contractid=b.contractid and  a.ppid=b.ppid and a.feemonth=b.feemonth " +
                             " left outer join t_cus c on a.cusid=c.cusid"+
                             " where 1=1 ";

            if(O!="")str+=" and a.feemonth<='" + O + "'";

            if (spp!=null)
            {
                str += " and a.ppid=" + spp;
            }
            else if (slevel != null)
            {
                string str1="select distinct x.contractid  from t_con_pp x ,t_ppunit y where x.ppid=y.ppid and y.unitlevel=" + slevel ;
                getdatatable(str1);

                if (dt.Rows.Count > 0)
                {
                    for (int ikk = 0; ikk < dt.Rows.Count; ikk++)
                    {
                        if (ikk == 0) str += " and a.contractid in (" + dt.Rows[ikk][0].ToString();
                        else str += "," + dt.Rows[ikk][0].ToString();

                        if (ikk == (dt.Rows.Count - 1)) str += ")";
                    }
                }
                else
                {
                    str += "and a.contractid=null";
                }


            }
            else if(sbuilding!=null)
            {
                string str1 = "select distinct x.contractid  from t_con_pp x ,t_ppunit y where x.ppid=y.ppid and y.unitbuilding=" + sbuilding;
                getdatatable(str1);

                if (dt.Rows.Count > 0)
                {
                    for (int ikk = 0; ikk < dt.Rows.Count; ikk++)
                    {
                        if (ikk == 0) str += " and a.contractid in (" + dt.Rows[ikk][0].ToString();
                        else str += "," + dt.Rows[ikk][0].ToString();

                        if (ikk == (dt.Rows.Count - 1)) str += ")";
                    }
                }
                else
                {
                    str += "and a.contractid =null";
                }
            }
            else if (sarea != null)
            {
                str += " and a.contractid in " +
                     "(select  contractid  from t_contract where contractarea=" + sarea + ")";
            }

            str+=" group by a.contractid,a.feemonth ";

            getdatatable(str);


            close();

            return dt;
        }

        public bool splitfeepaymonfnc(DataRow dr)
        {
            doconnnect();

            string sctid = dr["contractid"].ToString();

            string str = "select * from t_con_pp where contractid="+sctid+ " and substr(sdt,7,2)!='01'";

            DataTable dt1= gettablebystr(str);

            if (dt1.Rows.Count <= 0) return false;

            for (int kk = 0; kk < dt1.Rows.Count; kk++)
            {
                string sdt = dt1.Rows[kk]["sdt"].ToString();
                string edt = dt1.Rows[kk]["edt"].ToString();

                System.DateTime dt_s = DateTime.ParseExact(sdt, "yyyyMMdd", new CultureInfo("zh-CN", true));
                System.DateTime dt_e = DateTime.ParseExact(edt, "yyyyMMdd", new CultureInfo("zh-CN", true));

                int nmondays = DateTime.DaysInMonth(dt_s.Year, dt_s.Month);  //该月天数

                int ndays = (nmondays - dt_s.Day) +1; //该月纳入计算天数

                string sq1 = "select rentfee+bfee as fee,feeid from t_fee_pay_mgt where contractid=" + dr["contractid"].ToString() +
                           " and ppid=" + dt1.Rows[kk]["ppid"].ToString() +
                           " and feemonth='" + dt_s.ToString("yyyyMM")+"'";

                getdatatable(sq1);

                double fee =Convert.ToDouble( dt.Rows[0][0].ToString());

                double feesplit =(System.Math.Round( fee * 1.0 / nmondays,2))*ndays;


                //保存到数据库

                sq1 = "update t_fee_pay_mgt set rentfee=" + feesplit.ToString() +
                    " where feeid=" + dt.Rows[0][1].ToString();
                executecmd(sq1);

                double f1=System.Math.Round((fee-feesplit),2);

                sq1=" update t_fee_pay_mgt set rentfee=rentfee+"+f1+
                    " where contractid=" + dr["contractid"].ToString() +
                           " and ppid=" + dt1.Rows[kk]["ppid"].ToString() +
                           " and feemonth='" + dt_e.ToString("yyyyMM")+"'";
                executecmd(sq1);

                string sxx = "select row_count()";

                getdatatable(sxx);

                if (Convert.ToInt32(dt.Rows[0][0]) == 0)
                {
                    sq1 = "select unitno,contractno,contractnofnc,a.ppid,a.contractid " +
                        "from t_con_pp a,t_contract b,t_ppunit c " +
                        "where  a.contractid=b.contractid and a.ppid=c.ppid " +
                        "and a.ppid=" + dt1.Rows[kk]["ppid"].ToString() +
                        " and a.contractid="+dt1.Rows[kk]["contractid"].ToString();
                    getdatatable(sq1);

                    addfeepaymgtbyfnc(dt.Rows[0],f1.ToString(),dt_e.ToString("yyyyMM"));
                }

            }



            close();

            return true;
        }

        public DataRow getcontractbyid(string sid)
        {
            doconnnect();

            string s1 = "select a.contractnofnc,a.contractno,b.cusname,a.contractstatus,a.contractsdt,a.contractedt " +
                      " from t_contract a,t_cus b " +
                      " where a.cusid=b.cusid " +
                      " and a.contractid=" + sid;

            getdatatable(s1);

            close();

            return dt.Rows[0];
        }

        public void changerptmon(DataRow dr,string rptmon)
        {
            doconnnect();

            string str = "update t_fee_payed_seq_con_fnc set rptmon='"+rptmon +"'"+
                         "where feepayedctseq=" + dr["feepayedctseq"].ToString()  ;

            executecmd(str);

            close();

            return;

        }

        public DataTable getcontractfeepayedseq(string  sct)
        {
            doconnnect();

            string str = "select * from t_fee_payed_seq_con_fnc where contractid=" + sct+
                         " order by feepayedctseq";

            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getcontractfeepayedseqmon(DataRow dr)
        {
            doconnnect();

            string str = "select a.contractid,a.feemonth, a.feepayedctseq,a.ppid, sum(ifnull(payfee,0)) as fee," +
            " sum(ifnull(a.feepayedprev,0)) as feepayedprev, sum(ifnull(a.feepayednow,0)) as feepayednow " +
            " from t_fee_payed_seq_fnc a" +
            " where a.feepayedctseq=" + dr["feepayedctseq"].ToString() +
            " group by a.contractid,a.feemonth";

            getdatatable(str);

            close();

            return dt;

        }


        public DataTable getcontractppfeepayedseqmon(DataRow dr)
        {
            doconnnect();

            string str = "select a.ppid,b.unitno,a.feemonth, a.feepayedctseq,a.ppid, payfee as fee," +
            " feepayedprev as feepayed, a.feepayednow " +
            " from t_fee_payed_seq_fnc a join t_fee_pay_mgt b "+
            " on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth" +
            " where a.feepayedctseq=" + dr["feepayedctseq"].ToString() +
            " and a.feemonth='"+dr["feemonth"].ToString()+"'";

            getdatatable(str);

            close();

            return dt;

        }

        public void deletepayfeeseqfnc(DataRow dr)
        {
            doconnnect();
            MySql.Data.MySqlClient.MySqlTransaction mst = mysqldb.BeginTransaction();

            string str = "delete from t_fee_payed_seq_con_fnc where feepayedctseq=" + dr["feepayedctseq"].ToString();
            executecmd(str);

            str="update t_fee_payed_fnc a,t_fee_payed_seq_fnc b set a.payedfee=a.payedfee-b.feepayednow "+
                " where a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth "+
                " and b.feepayedctseq=" + dr["feepayedctseq"].ToString();
            executecmd(str);

            str = "delete from t_fee_payed_seq_fnc where feepayedctseq=" + dr["feepayedctseq"].ToString();
            executecmd(str);
    

            mst.Commit();
            

            close();

            return;
        }

        public DataTable getppunitbycontract(string sctid)
        {
            doconnnect();

            string str = "select a.cpid,a.contractid,a.ppid,c.unitno,a.sdt,a.edt,a.uarea,a.rent,a.bfee "+
                         "from t_con_pp a,t_ppunit c "+
                         "where a.ppid=c.ppid and a.contractid=" + sctid;
            getdatatable(str);

            close();

            return dt;
        }

        //取套内面积列表
        public DataTable getppuarea()
        {
            doconnnect();

            string str = "select distinct(unituarea)  as uua from t_ppunit  order by uua ";
            getdatatable(str);

            close();

            return dt;
        }

        //根据string读取表
        public DataTable gettablebystr(string squery)
        {
            doconnnect();


            getdatatable(squery);

            close();

            return dt;
        }

        //取客户名称列表
        public DataTable getcusnameall()
        {
            doconnnect();

            string str = "select cusid,cusname from t_cus order by cusid ";
            getdatatable(str);

            close();

            return dt;
        }


        public DataTable getcontractbyareafnc(string sarea)
        {
            doconnnect();


            string str = "select contractid,contractid,concat(ifnull(contractnofnc,''),'|',cusname,'|',contractno) as ctno_cus " +
                         "from t_contract a, t_cus b where a.cusid=b.cusid and a.contractarea=" + sarea +
                         " order by contractnofnc";

            getdatatable(str);

            close();

            return dt;
        }



        public DataTable getcontractbyarea(string sarea)
        {
            doconnnect();


            string str = "select contractid,contractid,concat(contractno,'|',cusname) as ctno_cus " +
                         "from t_contract a, t_cus b where a.cusid=b.cusid and a.contractarea=" + sarea;
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getcontractbycus(string scusid)
        {
            doconnnect();

                               
            string str = "select contractid,contractno,contractarea,contractpptype,contractstatus,"+
                         "contractorg,signdt,unittarget,contracttext,rentfreeperiod,rentpaystyle,"+
                         "depositfee,contractsdt,contractedt,cusname "+
                         "from t_contract a, t_cus b where a.cusid=b.cusid and a.cusid=" + scusid;
            getdatatable(str);

            close();

            return dt;
        }

        //取合同名称列表
        public DataTable getcontractnoall()
        {
            doconnnect();

            string str = "select contractid,contractno from t_contract order by contractid ";
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getppstatall()
        {
            doconnnect();

            string str = "select unitstatus,count(*) as total from t_ppunit where "+
                         "unitstatus in ('出租','空闲','保留') "+
                         " group by unitstatus";
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getpayedfeeyeartotal(string smon, string emon)
        {
            doconnnect();

            string str = "select substr(feemonth,5,2) as fm, sum(ifnull(payedfee,0)) as fee from t_fee_payed_fnc where " +
                       " feemonth>='" + smon + "' and " +
                       " feemonth<='" + emon + "' " +
                       " group by fm order by fm";

            gettablebystr(str);

            close();


            return dt;
        }

        public DataTable getpayedfeetotal(string smon, string emon)
        {
            doconnnect();

            string str = "select feemonth, sum(ifnull(payedfee,0)) as fee from t_fee_payed_fnc where " +
                       " feemonth>='" + smon + "' and " +
                       " feemonth<='" + emon + "' " +
                       " group by feemonth order by feemonth";

            gettablebystr(str);

            close();


            return dt;
        }

        public DataTable getpayfeetotal(string smon, string emon)
        {
            doconnnect();

            string str = "select feemonth, sum(ifnull(rentfee,0)+ifnull(bfee,0)) as fee from t_fee_pay_mgt where " +
                       " feemonth>='" + smon +"' and "+
                       " feemonth<='" + emon +"' "+
                       " group by feemonth order by feemonth";

            gettablebystr(str);

            close();


            return dt;
        }

        public DataTable getppstatbyarea(string ppstatus)
        {
            doconnnect();

            if (ppstatus == null)
            {
                string str = "select b.ppname,count(*) as total from t_ppunit a,t_pp b where "+
                            " a.unitarea=b.id and  a.unitstatus not in ('已拆','已并') group by a.unitarea";

                getdatatable(str);
            }
            else
            {
                string str = "select b.ppname,count(*) as total from t_ppunit a,t_pp b where a.unitstatus='" +
                             ppstatus + "' and " +
                             " a.unitarea=b.id group by a.unitarea";

                getdatatable(str);
            }
            close();

            return dt;

        }

        //
        public DataTable getparavaluebycat(string scat)
        {
            doconnnect();

            string str = "select * from t_syscode where paraname='" + scat + "' order by paraseqno";
            getdatatable(str);

            close();

            return dt;

        }

        public bool checkhasunits(string sarea, string sbuilding)
        {
            doconnnect();

            string str = "select 1 from t_ppunit where unitarea=" + sarea + " and unitbuilding=" + sbuilding;
            getdatatable(str);

            close();

            if (dt.Rows.Count == 0) return true;
            else  return false;


        }


        //取参数编码
        public DataTable getparacode(string paraname)
        {
            doconnnect();

            string str = "select * from t_syscode where paraname='" + paraname + "' order by paraseqno";
            getdatatable(str);

            close();

            return dt;

        }

        //去系统参数种类
        public DataTable getparatype()
        {
            doconnnect();

            string str = "select distinct paracatname,paraname from t_syscode";
            getdatatable(str);

            close();

            return dt;

        }

        public DataTable getppnamebyid(string sarea)
        {
            doconnnect();

            string str = "select ppcode, ppname from t_pp where id="+sarea;
            getdatatable(str);

            close();

            return dt;

        }

        public DataTable getgroupadduser(string sgroupid)
        {
            doconnnect();

            string str = suserquery + " and userid not in (select userid from t_user_group where groupid="+sgroupid+")";
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getsysusers()
        {
            doconnnect();

            string str = suserquery;
            getdatatable(str);

            close();

            return dt;
        }

        //登录处理
        public DataTable getloginusers()
        {
            doconnnect();

            string str = "select * from t_user";
            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getrentinfoapt()
        {
            return dt;
        }

        public DataTable getpayfeeinformmon(DateTime d11)
        {
            doconnnect();



            string stoday = d11.ToString("yyyyMMdd");
            string sifday = d11.AddDays(31).ToString("yyyyMMdd");

            string s1 = "select distinct a.contractid,a.contractno,b.cusname,a.feepaysdt from t_fee_pay_mgt_period a ,t_cus b " +
                      "where a.cusid=b.cusid and  feepaysdt>='" + stoday + "' and " +
                      "feepaysdt<='" + sifday +
                      "' and feepay>ifnull(feepayed,0) " ;

            getdatatable(s1);


            close();


            return dt;
        }

        public DataTable getcontractinformmon(DateTime d11)
        {
            doconnnect();



            string stoday = d11.ToString("yyyyMMdd");
            string sifday = d11.AddDays(31).ToString("yyyyMMdd");

            string s1 = "select a.contractid,a.contractno,b.cusname,a.contractedt from t_contract a ,t_cus b " +
                      "where a.cusid=b.cusid and contractedt>='" + stoday + "' and " +
                      "contractedt<='" + sifday+"'";

            getdatatable(s1);


            close();


            return dt;
        }

        public DataTable getpayfeeinform(int idays)
        {
            doconnnect();



            string stoday = System.DateTime.Now.ToString("yyyyMMdd");
            string sifday = System.DateTime.Now.AddDays(idays).ToString("yyyyMMdd");

            string s1 = "select distinct a.contractid,a.contractno,b.cusname,a.feepaysdt from t_fee_pay_mgt_period a ,t_cus b " +
                      "where a.cusid=b.cusid and  feepaysdt>='" + stoday + "' and " +
                      "feepaysdt<='" + sifday + 
                      "' and feepay>ifnull(feepayed,0) "+
                      " and feeid not in (select ignorefeeid from t_inform where userid=" + jkwyjygl.Form1.uid + ")";

            getdatatable(s1);


            close();


            return dt;
        }

        public DataTable getcontractinform(int idays)
        {
            doconnnect();



            string stoday = System.DateTime.Now.ToString("yyyyMMdd");
            string sifday = System.DateTime.Now.AddDays(idays).ToString("yyyyMMdd");

            string s1 = "select a.contractid,a.contractno,b.cusname,a.contractedt from t_contract a ,t_cus b " +
                      "where a.cusid=b.cusid and contractedt>='"+stoday +"' and "+
                      "contractedt<='"+sifday+"' and contractid not in (select ignorecontractid from t_inform)";

            getdatatable(s1);

            
            close();

                
            return dt;
        }

        public bool verifyuser(string userid, string userpassword)
        {
            doconnnect();

            string str = "select userstatus from t_user where userid=" + userid +
                         " and userpassword='" + userpassword + "'";

            getdatatable(str);

            close();

            if (dt.Rows.Count <= 0) return false;

            if (dt.Rows[0][0].ToString() != "启用") return false;

            return true;

        }

        //自动生成客户号
        public string getnewcusno(string sarea)
        {
            doconnnect();

            string s1 = "select ppcode from t_pp where id=" + sarea;
            getdatatable(s1);


            string s0 = dt.Rows[0][0].ToString() + "-";
            s0 += "KH-";


            s1 = "select max(cusid) from t_cus where cusno like '" + s0 + "%'";

            getdatatable(s1);

            if (Convert.IsDBNull(dt.Rows[0][0]))
            {
                s0 += "001";
                return s0;
            }

            s1 = "select cusno from t_cus where cusid=" + dt.Rows[0][0].ToString();

            getdatatable(s1);

            close();


            s1 = dt.Rows[0][0].ToString();

            string[] sxx = s1.Split(new char[] { '-' });

            Int32 ii = Convert.ToInt32(sxx[2]) + 1;

            string syy = "";

            if (ii < 1000) syy = ii.ToString("D3");
            else syy = ii.ToString();

            s0 += syy;



            return s0;
        }

        //自动生成合同号
        public string getnewcontractno(string sarea,string stype)
        {
            doconnnect();

            string s1 = "select ppcode,id from t_pp where id=" + sarea;
            getdatatable(s1);


            string s0=dt.Rows[0][0].ToString()+"-";
            if (stype == "住宿") s0 += "GY-";
            else if (stype == "商铺") s0 += "SP-";
            else if (stype == "场地") s0 += "CD-";
            else if (stype == "办公") s0 += "BG-";

            s1 = "select max(contractid) from t_contract where contractpptype='" + stype+"' "+
                 " and contractarea in " +
               "(select ppaid from t_ppgrp where ppgrp in (select ppgrp from t_ppgrp where ppaid=" + dt.Rows[0][1].ToString() +
               "))";

          //  s1 = "select max(contractid) from t_contract where contractno like '" + s0 + "%'";

            getdatatable(s1);

            if (Convert.IsDBNull( dt.Rows[0][0]))
            {
                s0 += "001";
                return s0;
            }

            s1 = "select contractno from t_contract where contractid=" + dt.Rows[0][0].ToString();

            getdatatable(s1);

            close();

            
            s1 = dt.Rows[0][0].ToString();

            string[] sxx = s1.Split(new char[] { '-' });

            Int32 ii = Convert.ToInt32(sxx[2]) + 1;

            string syy = "";

            if (ii < 1000) syy = ii.ToString("D3");
            else syy = ii.ToString();

            s0 += syy;



            return s0;
        }

        public DataTable getrptfncmonfeebyarea(string sarea, string smon)
        {
            doconnnect();

            string str = "select a.contractnofnc,payeddate,rptmon,payedfee,isconfirmed,cusname " +
                        "from t_fee_payed_seq_con_fnc a ,t_contract b,t_cus c where a.rptmon='" + smon + "' " +
                        "and  a.contractid=b.contractid and b.cusid=c.cusid and b.contractarea=" + sarea; 

            getdatatable(str);

            close();

            return dt;
        }



        public DataTable getrptfncfeebyarea(string sarea, string smon)
        {
            doconnnect();

            //string str = "select xx.*,y1.unitno as unitno,y3.cusname as cusname from (" +
            //             "select y.contractnofnc,y.contractid,y.ppid,y.feemonth,z.owedfee,y.feepay,y.payedfee,x.prepay from " +
            //             "(select a.contractnofnc,a.contractid,a.ppid,a.feemonth,(a.rentfee+a.bfee) as feepay ,ifnull(b.payedfee,0) as payedfee " +
            //             "from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
            //             "on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //             "where a.contractid in ( select contractid from t_contract where contractarea=" + sarea + ") and a.feemonth='" + smon + "' ) y," +
            //             "(select a.contractid,a.ppid ,sum(a.rentfee+a.bfee)-sum(ifnull(b.payedfee,0)) as owedfee " +
            //             "from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
            //             "on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //             "where a.contractid in ( select contractid from t_contract where contractarea=" + sarea + ") and  a.feemonth<='" + smon + "' group by a.contractid,a.ppid) z ," +
            //             "(select a.contractid,a.ppid ,sum(ifnull(b.payedfee,0)) as prepay " +
            //             "from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
            //             "on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth " +
            //             "where a.contractid in ( select contractid from t_contract where contractarea=" + sarea + ") and  a.feemonth>'" + smon + "' group by a.contractid,a.ppid ) x " +
            //             "where y.contractid=z.contractid and y.ppid=z.ppid and " +
            //             "y.contractid=x.contractid and y.ppid=x.ppid ) xx ,t_ppunit y1,t_contract y2,t_cus y3 " +
            //             "where xx.ppid=y1.ppid and xx.contractid=y2.ContractID and y2.CusID=y3.cusid";



            System.DateTime dt_1_1 = DateTime.ParseExact(smon, "yyyyMM", new CultureInfo("zh-CN", true));

 
            string p_mon_p = dt_1_1.AddMonths(-1).ToString("yyyyMM");
   
            string s1="call rpt_rentpay('"+smon+"','"+p_mon_p+"')";

            executecmd(s1);


            s1 = "select  a.* from t_rpt_rentpay a, t_contract b where a.contractid=b.contractid and "+
                 " b.contractarea="+sarea;

            getdatatable(s1);

            close();

            return dt;
        }

        public DataTable getrptfncfeebyct(string sid,string smon)
        {
            doconnnect();

            string str = "select xx.*,y1.unitno as unitno,y3.cusname as cusname from ("+
                         "select y.contractnofnc,y.contractid,y.ppid,y.feemonth,z.owedfee,y.feepay,y.payedfee,x.prepay from " +
                         "(select a.contractnofnc,a.contractid,a.ppid,a.feemonth,(a.rentfee+a.bfee) as feepay ,ifnull(b.payedfee,0) as payedfee " +
                         "from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
                         "on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth " +
                         "where a.contractid="+sid+" and a.feemonth='" + smon + "' ) y," +
                         "(select a.contractid,a.ppid ,sum(a.rentfee+a.bfee)-sum(ifnull(b.payedfee,0)) as owedfee " +
                         "from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
                         "on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth " +
                         "where a.contractid=" + sid + " and  a.feemonth<='" + smon + "' group by a.contractid,a.ppid) z ," +
                         "(select a.contractid,a.ppid ,sum(ifnull(b.payedfee,0)) as prepay " +
                         "from t_fee_pay_mgt a left outer join t_fee_payed_fnc b " +
                         "on a.contractid=b.contractid and a.ppid=b.ppid and a.feemonth=b.feemonth " +
                         "where a.contractid=" + sid + " and  a.feemonth>'" + smon + "' group by a.contractid,a.ppid ) x " +
                         "where y.contractid=z.contractid and y.ppid=z.ppid and " +
                         "y.contractid=x.contractid and y.ppid=x.ppid ) xx ,t_ppunit y1,t_contract y2,t_cus y3 "+
                         "where xx.ppid=y1.ppid and xx.contractid=y2.ContractID and y2.CusID=y3.cusid";

            getdatatable(str);

            close();

            return dt;
        }


        public DataTable getrptfncppbyarea(string sarea)
        {
            doconnnect();

            string str = "select ppname,"+
                         "max(case unitstatus when '空闲' then nums else 0 end) idlenums,"+
                         "max(case unitstatus when '出租' then nums else 0 end) rentnums,"+
                         "max(case unitstatus when '保留' then nums else 0 end) resnums,"+
                         "sum(nums) ppnums from "+
                         "(select unitarea,unitbuilding,unitstatus,count(*) as nums from t_ppunit "+
                         " where unitarea="+sarea+
                         " and unitstatus not in ('已拆','已并') "+
                         " group by unitarea,unitbuilding,unitstatus) f,t_pp n "+
                         " where f.unitbuilding=n.id "+
                         " group by unitarea,unitbuilding";

            getdatatable(str);

            close();

            return dt;
        }

        public DataTable getcon_pp_sdt_edt(DataRow drcon)
        {
            doconnnect();

            string str = "select distinct sdt,edt from t_con_pp where contractid="+drcon["contractid"].ToString();

            getdatatable(str);

            close();

            return dt;
        }


        public DataTable getrptfncpp()
        {
            doconnnect();

            string str = "select ppname," +
                       "max(case unitstatus when '空闲' then nums else 0 end) idlenums," +
                       "max(case unitstatus when '出租' then nums else 0 end) rentnums," +
                       "max(case unitstatus when '保留' then nums else 0 end) resnums," +
                       "sum(nums) ppnums from " +
                       "(select unitarea,unitstatus,count(*) as nums from t_ppunit " +
                       "where unitstatus not in ('已拆','已并') " +
                       "group by unitarea,unitstatus) f ,t_pp n where f.unitarea=n.id " +
                       "group by unitarea";
            
            getdatatable(str);

            close();

            return dt;
        }

        public bool checkcthasfeepay(DataRow dr)
        {
            bool rt = false;

            getdatatable("select 1 from t_fee_pay_mgt_period where contractid=" + dr["contractid"].ToString());

            if (dt.Rows.Count > 0) rt = true;

            return rt;

        }

        //检查操作状态
        public bool checkoperconditions(ref string smsg,string opertype, object e)
        {
            bool rt = false;
            smsg = "状态检查错误！";

            if (e== null) return rt;

            if (jkwyjygl.Form1.uid == "0") return true;


            DataRow dr = null;
            DevExpress.XtraGrid.Views.Layout.LayoutView lv = null;
            DevExpress.XtraGrid.Views.Grid.GridView gv = null;


            switch (opertype)
            {
                case "op_delete_feepayed_fnc":
                    dr = (DataRow)e;

                    getdatatable("select isconfirmed from t_fee_payed_seq_con_fnc where contractid=" + dr["contractid"].ToString()+
                                 " and feepayedctseq="+dr["feepayedctseq"].ToString()
                        );

                    if (dr["isconfirmed"].ToString() == "否" &&
                        dt.Rows[0]["isconfirmed"].ToString() == "否"
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[删除收费] 只能删除『未复核』状态的收费！";
                    }
                    break;



                case "op_approve_contract":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());
                    if ((dr["contractstatus"].ToString()=="等待审核"&&
                         dt.Rows[0]["contractstatus"].ToString() == "等待审核"
                         )||
                        (dr["contractstatus"].ToString()=="等待修改审核"&&
                         dt.Rows[0]["contractstatus"].ToString() == "等待修改审核"
                        )
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[合同审核通过] 只能对『等待审核/等待修改审核』状态的合同审核通过！";
                    }
                    break;


                case "op_uncheck_contract":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());
                    if ((dr["contractstatus"].ToString() == "等待审核" &&
                         dt.Rows[0]["contractstatus"].ToString() == "等待审核"
                        )||
                        (dr["contractstatus"].ToString() == "等待修改审核" &&
                         dt.Rows[0]["contractstatus"].ToString() == "等待修改审核"
                        )
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[取消合同审核] 只能对『等待审核/等待修改审核』状态的合同取消审核！";
                    }
                    break;

                case "op_approveremodify_contract":
                     dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if (dr["contractstatus"].ToString() == "申请修改"&&
                        dt.Rows[0]["contractstatus"].ToString() == "申请修改")
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[同意申请合同修改] 只能同意『申请修改』状态的合同！";
                    }
                    break;

                case "op_cancelremodify_contract":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if (dr["contractstatus"].ToString() == "申请修改"&&
                        dt.Rows[0]["contractstatus"].ToString() == "申请修改")
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[取消申请合同修改] 只能取消『申请修改』状态的合同！";
                    }
                    break;

                case "op_input_fee_manage":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if (dr["contractstatus"].ToString() == "已审核"&&
                        dt.Rows[0]["contractstatus"].ToString() == "已审核")
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[录入收费] 只能对『已审核』状态的合同进行录入收费！";
                    }
                    break;

                case "op_remodify_contract":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if (dr["contractstatus"].ToString() == "已审核"&&
                        dt.Rows[0]["contractstatus"].ToString() == "已审核")
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[申请修改合同] 只能申请『已审核』状态的合同进行修改！";
                    }
                    break;



                case "op_check_contract_cancel":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if ((dr["contractstatus"].ToString() == "初登" &&
                         dt.Rows[0]["contractstatus"].ToString() == "初登"
                         ) ||
                        (dr["contractstatus"].ToString() == "修改" &&
                         dt.Rows[0]["contractstatus"].ToString() == "修改"
                        )
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[合同提前终止] 只能对『初登/修改』状态的合同进行提前终止操作！请先向财务申请修改.";
                    }
                    break;


                case "op_check_contract":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if ((dr["contractstatus"].ToString() == "初登"&&
                         dt.Rows[0]["contractstatus"].ToString() == "初登"
                         )||
                        (dr["contractstatus"].ToString() == "修改"&&
                         dt.Rows[0]["contractstatus"].ToString() == "修改"
                        )
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[提交合同审核] 只能提交『初登/修改』状态的合同进行审核！";
                    }
                    break;


               case "op_d_contracts":
                    
                    gv = (DevExpress.XtraGrid.Views.Grid.GridView)e;
                    foreach (Int32 i3 in gv.GetSelectedRows())
                    {
                        DataRow dr1 = gv.GetDataRow(i3);

                        getdatatable("select contractstatus from t_contract where contractid=" + dr1["contractid"].ToString());

                        if (dr1["contractstatus"].ToString() != "初登"||
                            dt.Rows[0]["contractstatus"].ToString() != "初登")
                        {
                            smsg = "[删除合同]  要删除的合同状态『不全为初登』！";
                            return rt;
                        }
                    }
                    rt = true;
                    
                    break;
                case "op_m_contract":
                    dr = (DataRow)e;

                    getdatatable("select contractstatus from t_contract where contractid=" + dr["contractid"].ToString());

                    if ((dr["contractstatus"].ToString() =="初登"&&
                         dt.Rows[0]["contractstatus"].ToString() == "初登"
                         )||
                        (dr["contractstatus"].ToString() =="修改"&&
                         dt.Rows[0]["contractstatus"].ToString() == "修改"
                        )
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[修改合同] 只能修改『初登/修改』状态的合同！";
                    }
                    break;


                 case "op_m_ppunit":
                    dr = (DataRow)e;

                    getdatatable("select unitstatus from t_ppunit where ppid=" + dr["ppid"].ToString());

                    if (dr["unitstatus"].ToString() == "空闲"&&
                        dt.Rows[0]["unitstatus"].ToString() == "空闲")
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[修改房间] 只能修改『空闲』状态的房间！";
                    }
                    break;
                 case "op_s_ppunit":
                    dr = (DataRow)e;

                    getdatatable("select unitstatus from t_ppunit where ppid=" + dr["ppid"].ToString());

                    if ((dr["unitstatus"].ToString() == "空闲" &&dt.Rows[0]["unitstatus"].ToString() == "空闲")||
                        (dr["unitstatus"].ToString() == "已拆"&&dt.Rows[0]["unitstatus"].ToString() == "已拆")
                    )
                    {
                        rt = true;
                    }
                    else
                    {
                        smsg = "[拆分房间] 只能拆分『空闲|已拆』状态的房间！";
                    }
                    break;
                case "op_c_ppunits":
                    try
                    {
                        gv = (DevExpress.XtraGrid.Views.Grid.GridView)e;
                        foreach (Int32 i3 in gv.GetSelectedRows())
                        {
                            DataRow dr1 = gv.GetDataRow(i3);

                            getdatatable("select unitstatus from t_ppunit where ppid=" + dr1["ppid"].ToString());

                            if (dr1["unitstatus"].ToString() != "空闲" ||
                                dt.Rows[0]["unitstatus"].ToString() != "空闲"
                            )
                            {
                                smsg = "[合并房间]  要合并的房间状态『不全为空闲』！";
                                return rt;
                            }
                        }
                        rt = true;
                    }
                    catch
                    {
                        lv = (DevExpress.XtraGrid.Views.Layout.LayoutView)e;
                        foreach (Int32 i3 in lv.GetSelectedRows())
                        {
                            DataRow dr1 = lv.GetDataRow(i3);

                            getdatatable("select unitstatus from t_ppunit where ppid=" + dr1["ppid"].ToString());


                            if (dr1["unitstatus"].ToString() != "空闲"||
                                dt.Rows[0]["unitstatus"].ToString() != "空闲"
                            )
                            {
                                smsg = "[合并房间]  要合并的房间状态『不全为空闲』！";
                                return rt;
                            }
                        }
                        rt = true;
                    }

                    break;
                case "op_d_ppunits":
                    try 
                    {
                        gv= (DevExpress.XtraGrid.Views.Grid.GridView)e;
                        foreach (Int32 i3 in gv.GetSelectedRows())
                        {
                            DataRow dr1 = gv.GetDataRow(i3);

                            getdatatable("select unitstatus from t_ppunit where ppid=" + dr1["ppid"].ToString());

                            if (dr1["unitstatus"].ToString() != "空闲"||
                                dt.Rows[0]["unitstatus"].ToString() != "空闲"
                            )
                            {
                                smsg = "[删除房间]  要删除的房间状态『不全为空闲』！";
                                return rt;
                            }
                        }
                        rt = true;
                    }
                    catch
                    {
                        lv= (DevExpress.XtraGrid.Views.Layout.LayoutView)e;
                        foreach (Int32 i3 in lv.GetSelectedRows())
                        {
                            DataRow dr1 = lv.GetDataRow(i3);

                            getdatatable("select unitstatus from t_ppunit where ppid=" + dr1["ppid"].ToString());

                            if (dr1["unitstatus"].ToString() != "空闲"||
                                dt.Rows[0]["unitstatus"].ToString() != "空闲"
                            )
                            {
                                smsg = "[删除房间]  要删除的房间状态『不全为空闲』！";
                                return rt;
                            }
                        }
                        rt = true;
                    }
 
                    break;
                default:
                    break;
                    
            }

            return rt;
        }
    }
}

