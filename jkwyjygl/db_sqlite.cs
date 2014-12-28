using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Configuration;
using MySql.Data;
using System.Data.SQLite;


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
                           "a.unitcarea as unitcarea,a.unituarea as unituarea,a.cusid,b.cusname as cusname,a.contractid,c.contractname as contractname," +
                           "a.contractsdt as contractsdt,a.contractedt as contractedt,a.unitrent as unitrent,a.unitarea ,a.unitbuilding,a.unitlevel, " +
                           "d.ppname as unitareaname,e.ppname as unitbuildingname,f.ppname as unitlevelname " +
                           "from t_ppunit a left outer join  t_cus b on a.cusid=b.cusid " +
                           " left outer join t_contract c on a.contractid=c.contractid  " +
                           " left outer join t_pp d on a.unitarea=d.id " +
                           " left outer join t_pp e on a.unitbuilding=e.id " +
                           " left outer join t_pp f on a.unitlevel=f.id ";

        public static string scusquery = "select cusid ,cusname as cusname,custype as custype,cusaddr as cusaddr,cuscerttype as cuscerttype,cuscertno as cuscertno," +
                      "cusmobnum as cusmobnum,cusmobnum2 as cusmobnum2,cusphonenum as cusphonenum,cusphonenum2 as cusphonenum2,cusothernum as cusothernum," +
                      "contractno as contractno," +
                      "cuspaytype as cuspaytype,cuspaybankname as cuspaybankname,cuspaybankno as cuspaybankno,cuspayname as cuspayname from t_cus ";

        public static string scontractquery = "select contractid,cusid,contractno as contractno,contractname as contractname,contractstatus as contractstatus,contractorg as contractorg," +
                                "signdt as signdt,signaddr as signaddr,contracttext as contracttext from t_contract ";

        //private MySql.Data.MySqlClient.MySqlConnection mysqldb;
        //private MySql.Data.MySqlClient.MySqlDataAdapter mysqlda;

        private SQLiteConnection mysqldb;
        private SQLiteDataAdapter mysqlda;

        //private System.Data.DataSet ds;
        private System.Data.DataTable dt;

        public dboper() { }

        public void doconnnect()
        {

            //string cnstr = ConfigurationManager.AppSettings["mysqlconn"];


            string cnstr ="Data Source="+
                          Application.StartupPath+@"\"+ConfigurationManager.AppSettings["sqlitedb"];


            //mysqldb = new MySql.Data.MySqlClient.MySqlConnection();
            mysqldb = new SQLiteConnection();
            mysqldb.ConnectionString = cnstr;
            mysqldb.Open();

            //accessdb = new System.Data.OleDb.OleDbConnection(cnstr);
            //accessdb.Open();
        }

        public void close()
        {
            mysqldb.Close();
        }

        public void executecmd(string str)
        {
            //MySql.Data.MySqlClient.MySqlCommand dbcmd = new MySql.Data.MySqlClient.MySqlCommand(str, mysqldb);
            SQLiteCommand dbcmd = new SQLiteCommand(str, mysqldb);
            dbcmd.ExecuteNonQuery();
        }

        

        public void getdatatable(string str)
        {

            //mysqlda = new MySql.Data.MySqlClient.MySqlDataAdapter(str, mysqldb);
            mysqlda = new SQLiteDataAdapter(str, mysqldb); 

            dt = new System.Data.DataTable();
            mysqlda.Fill(dt);
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

        //获取片区
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

            string squery =sppunitquery+" where " +
                        "unitarea=" + sarea + " and " +
                        "unitbuilding=" + sbuilding + " and " +
                        "unitlevel=" + slevel;


            getdatatable(squery);

            close();

            return dt;
        }

        //修改客户
        public void updatecusbyid(string scusid, string scmd)
        {
            doconnnect();

            string str1 = scmd +
                        " where cusid=" + scusid;

            executecmd(str1);

            close();
        }


        //修改ppunit
        public void updateppunitbyid(string sppid, string snewcarea, string snewuarea, string snewppname,string snewppno)
        {
            doconnnect();

            string str1 = "update t_ppunit set unitcarea=" + snewcarea + "," +
                        "unituarea=" + snewuarea + ", "+
                        "unitname='" + snewppname + "', " +
                        "unitno='"+snewppno+"' "+
                        "where ppid=" + sppid;

            executecmd(str1);

            close();
        }

        //拆分ppunit
        public void splitppunit(DataRow dr, string scarea, string suarea, string sppno, string sppname)
        {
            doconnnect();

            string str2 = "insert into t_ppunit(splitfromunit,unitno,unitname,unittype,unitorg,unitstatus,unitcarea,unituarea,unitarea,unitbuilding,unitlevel) values(" +
                        dr["ppid"].ToString()+",'"+
                        sppno + "', '" +
                        sppname + "', '" +
                        dr["unittype"].ToString() + "', " +
                        "'拆分'," +
                        "'空闲'," +
                        scarea + "," +
                        suarea + "," +
                        dr["unitarea"].ToString() + "," +
                        dr["unitbuilding"].ToString() + "," +
                        dr["unitlevel"].ToString() + ")"
                        ;
            executecmd(str2);

            str2 = "update t_ppunit set unitstatus='已拆' where ppid=" + dr["ppid"].ToString();

            executecmd(str2);

            close();
 
        }

        //合并ppunit
        public void combineppunit(DevExpress.XtraGrid.Views.Grid.GridView gv, string scarea, string suarea, string sppno, string sppname)
        {
            doconnnect();
            SQLiteTransaction mst = mysqldb.BeginTransaction();

            DataRow dr = gv.GetFocusedDataRow();

            string str2 = "insert into t_ppunit(unitno,unitname,unittype,unitorg,unitstatus,unitcarea,unituarea,unitarea,unitbuilding,unitlevel) values('" +
                        sppno + "', '" +
                        sppname + "', '" +
                        dr["unittype"].ToString() + "', " +
                        "'合并'," +
                        "'空闲'," +
                        scarea + "," +
                        suarea + "," +
                        dr["unitarea"].ToString() + "," +
                        dr["unitbuilding"].ToString() + "," +
                        dr["unitlevel"].ToString() + ")"
                        ;

            executecmd(str2);

            str2 = "select LAST_INSERT_ROWID() ";

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
            close();

        }

        //添加ppunit
        public Int32 addppunit(string pparea, string ppbuilding, string pplevel, string pptype, string ppno, string ppname, string ppcarea, string ppuarea)
        {
            doconnnect();

            SQLiteTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into  t_ppunit(unitno,unitname,unittype,unitorg,unitstatus,unitcarea,unituarea,unitarea,unitbuilding,unitlevel) values('" +
                          ppno + "', '" +
                          ppname + "', '" +
                          pptype + "', " +
                          "'原始'," +
                          "'空闲'," +
                          ppcarea + "," +
                          ppuarea + "," +
                          pparea + "," +
                          ppbuilding + "," +
                          pplevel + ")";


            executecmd(str1);

            str1 = "select last_insert_rowid()";

            getdatatable(str1);

            Int32 iid = Convert.ToInt32(dt.Rows[0][0].ToString());

            mst.Commit();

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

        //添加pparea
        public Int32 addpparea(string sareacode, string sareaname, string sareades)
        {
            doconnnect();

            SQLiteTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_pp(parentid,pptype,ppcode,ppname,ppdes) values(0,'0','" + sareacode + "','" +
                          sareaname + "','" +
                          sareades + "')";

            executecmd(str1);

            str1 = "select Last_insert_rowid()";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;
        }

        //添加ppbuilding
        public Int32 addppbuilding(string sparentid, string sbuildingcode, string sbuildingname, string sbuildingdes)
        {
            doconnnect();

            SQLiteTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_pp(parentid,pptype,ppcode,ppname,ppdes) values(" +
                          sparentid + "," +
                          "'1','" +
                          sbuildingcode + "','" +
                          sbuildingname + "','" +
                          sbuildingdes + "')";

            executecmd(str1);

            str1 = "select last_insert_rowid()";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;

        }

        //更新contract
        public void updatecontract(string supd, object o1,Image im)
        {

            doconnnect();

 

            //处理picture/BLOB

            if(o1 != null)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] BlobValue = ms.ToArray();

                
                //MySql.Data.MySqlClient.MySqlCommand dbcmd = new MySql.Data.MySqlClient.MySqlCommand(supd, mysqldb);

                //MySql.Data.MySqlClient.MySqlParameter BlobParam =
                //    new MySql.Data.MySqlClient.MySqlParameter("@spic", MySql.Data.MySqlClient.MySqlDbType.Binary);
                //dbcmd.Parameters.Add(BlobParam);
                //BlobParam.Value = BlobValue;

                SQLiteCommand dbcmd = new SQLiteCommand(supd, mysqldb);

                SQLiteParameter BlobParam =
                    new SQLiteParameter("@spic", DbType.Binary);
                dbcmd.Parameters.Add(BlobParam);

                BlobParam.Value = BlobValue;

                dbcmd.ExecuteNonQuery();

            }
            else
            {
                
                executecmd(supd);
            }


            close();

        }

        //添加contract
        public void addcontract(DataRow dr,Image im)
        {

            doconnnect();

            string s111="insert into t_contract(contractno,contractname,contractstatus,contractorg,signdt,signaddr,contracttext) values('" +
                             dr["contractno"].ToString() + "','" +
                             dr["contractname"].ToString() + "','" +
                             dr["contractstatus"].ToString() + "','" +
                             dr["contractorg"].ToString() + "','" +
                             dr["signdt"].ToString() + "','" +
                             dr["signaddr"].ToString() + "'," ;
 
            //处理picture/BLOB

            if (im != null)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                im.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] BlobValue = ms.ToArray();

                s111 += "@spic)";

                //MySql.Data.MySqlClient.MySqlCommand dbcmd = new MySql.Data.MySqlClient.MySqlCommand(s111, mysqldb);

                //MySql.Data.MySqlClient.MySqlParameter BlobParam =
                //    new MySql.Data.MySqlClient.MySqlParameter("@spic", MySql.Data.MySqlClient.MySqlDbType.Binary);

                SQLiteCommand dbcmd = new SQLiteCommand(s111, mysqldb);

                SQLiteParameter BlobParam =
                    new SQLiteParameter("@spic", DbType.Binary);
                
                dbcmd.Parameters.Add(BlobParam);
                BlobParam.Value = BlobValue;

                dbcmd.ExecuteNonQuery();

            }
            else
            {
                s111 += "null)";
                executecmd(s111);
            }


            close();

        }

        //添加cus
        public void addcus(DataRow dr)
        {
            doconnnect();

            string str1 = "insert into t_cus(cusname,cusaddr,custype,cuscerttype,cuscertno,cusmobnum,cusmobnum2," +
                "cusphonenum,cusphonenum2,cusothernum,cuspaytype,cuspaybankname,cuspaybankno,cuspayname) values('" +
                dr["cusname"].ToString() + "','" +
                dr["cusaddr"].ToString() + "','" +
                dr["custype"].ToString() + "','" +
                dr["cuscerttype"].ToString() + "','" +
                dr["cuscertno"].ToString() + "','" +
                dr["cusmobnum"].ToString() + "','" +
                dr["cusmobnum2"].ToString() + "','" +
                dr["cusphonenum"].ToString() + "','" +
                dr["cusphonenum2"].ToString() + "','" +
                dr["cusothernum"].ToString() + "','" +
                dr["cuspaytype"].ToString() + "','" +
                dr["cuspaybankname"].ToString() + "','" +
                dr["cuspaybankno"].ToString() + "','" +
                dr["cuspayname"].ToString() + "')";
                ;

            executecmd(str1);

            close();

        }


        //添加level
        public Int32 addpplevel(string sparentid, string slevelcode, string slevelname, string sleveldes)
        {
            doconnnect();

            SQLiteTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_pp(parentid,pptype,ppcode,ppname,ppdes) values(" +
                          sparentid + "," +
                          "'2','" +
                          slevelcode + "','" +
                          slevelname + "','" +
                          sleveldes + "')";

            executecmd(str1);

            str1 = "select last_insert_rowid()";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;
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

        //取合同名称列表
        public DataTable getcontractnameall()
        {
            doconnnect();

            string str = "select contractid,contractname from t_contract order by contractid ";
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

        public DataTable getppstatbyarea(string ppstatus)
        {
            doconnnect();

            if (ppstatus == null)
            {
                string str = "select b.ppname,count(*) as total from t_ppunit a,t_pp b where "+
                            " a.unitarea=b.id group by a.unitarea";

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


        //取系统参数
        public DataTable getpara(string paraname)
        {
            doconnnect();

            string str = "select * from t_para where paraname='" + paraname + "' order by paraseqno";
            getdatatable(str);

            close();

            return dt;

        }

        //去系统参数种类
        public DataTable getparatype()
        {
            doconnnect();

            string str = "select distinct paracatname,paraname  from t_para";
            getdatatable(str);

            close();

            return dt;

        }

        //登录处理
        public DataTable getsysusers()
        {
            doconnnect();

            string str = "select userid,username,userpassword,usergroup,userstatus,userdesc from t_user";
            getdatatable(str);

            close();

            return dt;
        }

        public bool verifyuser(string userid, string userpassword)
        {
            doconnnect();

            //string str = "insert into t_user(username,userpassword,userstatus) values('admin','admin','启用')";

            //executecmd(str);

            //close();

            //return true;

            string str = "select userstatus from t_user where userid=" + userid +
                         " and userpassword='" + userpassword + "'";

            getdatatable(str);

            close();

            if (dt.Rows.Count <= 0) return false;

            if (dt.Rows[0][0].ToString() != "启用") return false;

            return true;

        }

        public Int32 addparavalue(string scatname, string scat, string sparavalue, string seqno)
        {
            doconnnect();

            SQLiteTransaction mst = mysqldb.BeginTransaction();

            string str1 = "insert into t_para(paracatname,paraname,paravalue,paraseqno) values('" +
                          scatname + "','" +
                          scat + "','" +
                          sparavalue + "'," +
                          seqno + ")";

            executecmd(str1);

            str1 = "select last_insert_rowid()";

            getdatatable(str1);

            int idadd = Convert.ToInt32(dt.Rows[0][0].ToString());


            mst.Commit();

            close();

            return idadd;
        }

        //删除paravalue
        public void deleteparavalue(string sid)
        {
            doconnnect();

            string str1 = "delete from t_para " +
                        "where id=" + sid;


            executecmd(str1);

            close();
        }

        //
        public DataTable getparavaluebycat(string scat)
        {
            doconnnect();

            string str = "select * from t_para where paraname='" + scat + "' order by paraseqno";
            getdatatable(str);

            close();

            return dt;

        }

        //修改building
        public void updateparavalue(string sid, string snewvalue, string snewseqno)
        {
            doconnnect();

            string str1 = "update t_para set paravalue='" + snewvalue + "'," +
                        "paraseqno='" + snewseqno + "' " +
                        "where  id=" + sid;

            executecmd(str1);

            close();
        }
    }
}

