using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Drawing;

namespace jkwyjygl
{
    public partial class frm_ct_ppunit : DevExpress.XtraEditors.XtraForm
    {
        public delegate void dsmsg(string msgtype, string msg);
        public dsmsg msgshow;

        public DataRow drcontract;

        public DataTable dtppunit;

        private wheda.db.dboper mydb;


        public frm_ct_ppunit()
        {
           
            InitializeComponent();
            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvct_ppunit);

            mydb = new wheda.db.dboper();
        }

        private void frm_ct_ppunit_Load(object sender, EventArgs e)
        {
           

            string sarea = drcontract["contractarea"].ToString();

            //获取本项目
            DataTable dt = mydb.getppnamebyid(sarea);

            tlpp.BeginUpdate();
            tlpp.Columns.Add();
            tlpp.Columns[0].Name = "tcode";
            tlpp.Columns[0].Caption = dt.Rows[0]["ppname"].ToString();
            tlpp.Columns[0].VisibleIndex = 0;
            tlpp.Columns.Add();
            tlpp.Columns[1].Name = "tid";
            tlpp.Columns[1].VisibleIndex = 1;
            tlpp.Columns[1].Caption ="ID";
            tlpp.Columns[1].Visible=false;

            tlpp.EndUpdate();

            //获取项目下的楼宇
            DataTable dt1 = mydb.getppbuildingbyareaid(sarea);
            foreach(DataRow drr in dt1.Rows)
            {
               TreeListNode trr=tlpp.AppendNode(new Object[] {  dt.Rows[0]["ppcode"].ToString()+"-"+drr["ppcode"].ToString(),
                                                               drr["id"].ToString() }, null);

               DataTable dtt = mydb.getpplevelbybuildingid(drr["id"].ToString());

               foreach (DataRow drr1 in dtt.Rows)
               {
                   
                  TreeListNode trr1= tlpp.AppendNode(new Object[] {trr[0].ToString()+"-"+ drr1["ppcode"].ToString(),
                                   drr1["id"].ToString() }, trr);

                  DataTable dtt1 = mydb.getfreeandnotinppunitbyid(drcontract["contractid"].ToString(),
                                                                  sarea, drr["id"].ToString(), drr1["id"].ToString());

                  foreach (DataRow dr123 in dtt1.Rows)
                  {
                      tlpp.AppendNode(new Object[] {dr123["unitno"].ToString(),dr123["ppid"].ToString() }, trr1);
                  }

               }

               trr.Expanded = true; ;

            }


            xgcct_ppunit.DataSource = dtppunit;
            
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


        private void AddCTPPunit(TreeListNode tr)
        {
            tr.Selected = false;

            if (tr.HasChildren)
            {
                AddCTPPunit(tr.FirstNode);
            }

            if (tr.Level == 2)
            {
                //add to right
                if (tr.Checked)
                {
                    string sppid = tr[1].ToString();

                    DataTable dt1 = mydb.getppunitbyppid(sppid);  //ppunit主表
                    DataRow dr1 = dt1.Rows[0];

                    DataTable dt = ((DataView)xgvct_ppunit.DataSource).Table; //con_pp明细表

                    DataRow dr = dt.NewRow();

                    dr["ppid"] = dr1["ppid"].ToString();
                    dr["unitno"] = dr1["unitno"].ToString();

                    //dr["unittype"] = dr1["unittype"].ToString();
                    //dr["unitorg"] = dr1["unitorg"].ToString();

                    dr["contractid"] = drcontract["contractid"].ToString();
                    dr["sdt"] = drcontract["contractsdt"].ToString();
                    dr["edt"] = drcontract["contractedt"].ToString();

                    dr["uarea"]=dr1["unituarea"].ToString();
                    dr["rent"] = dr1["unitrent"].ToString();
                    dr["bfee"] = dr1["unitbfee"].ToString();


                    Int32 inewid = mydb.addcon_pp(dr);
                    dr["cpid"] = inewid.ToString();

                    dt.Rows.Add(dr);





                    tr.Selected = true;
                }

            }

            if (tr.NextNode != null)
            {
                AddCTPPunit(tr.NextNode);
            }
 
           
        }

        private void xsbctaddppunit_Click(object sender, EventArgs e)
        {
        
 
            TreeListNode tr = tlpp.Nodes.FirstNode;

            AddCTPPunit(tr);

            tlpp.DeleteSelectedNodes();

            xgvct_ppunit.BestFitColumns();

            msgshow("Y", "[添加合同房间] 成功添加房间!");
        }

        private void tlpp_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        private void tlpp_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
        }

        private void xsbctdelppunit_Click(object sender, EventArgs e)
        {
            if (xgvct_ppunit.SelectedRowsCount < 1)
            {
                return;
            }

            foreach (int ii in xgvct_ppunit.GetSelectedRows())
            {
                DataRow dr = xgvct_ppunit.GetDataRow(ii);

                if (!mydb.delcon_pp(dr))
                {
                    msgshow("X", "要移除的房间存在收费数据，并没有被移除!");
                }
            }

            xgvct_ppunit.DeleteSelectedRows();
            

            msgshow("Y", "[删除合同房间] 成功删除!");
        }

        private void xgvct_ppunit_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            if (dr == null) return;

            xteconppuarea.Text = dr["uarea"].ToString();
            xteconppbfee.Text = dr["bfee"].ToString();
            xteconpprent.Text = dr["rent"].ToString();

            xdeconppsdt.Text = dr["sdt"].ToString();
            xdeconppedt.Text = dr["edt"].ToString();

        }

        private void xsbconppmodify_Click(object sender, EventArgs e)
        {
            DataRow dr = xgvct_ppunit.GetFocusedDataRow();

            if (dr == null) return;

            dr["uarea"] = xteconppuarea.Text;
            dr["bfee"] = xteconppbfee.Text;
            dr["rent"] = xteconpprent.Text;

            dr["sdt"] = xdeconppsdt.Text;
            dr["edt"] = xdeconppedt.Text;


            mydb.updateconpp(dr);

            msgshow("Y", "[修改合同房间] 成功修改合同房间!");

        }

        private void frm_ct_ppunit_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { mydb.finalclose(); }
            finally { }
        }

 
    }
}