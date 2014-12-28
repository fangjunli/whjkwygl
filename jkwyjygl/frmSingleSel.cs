using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Drawing;

namespace jkwyjygl
{
    public partial class frmSingleSel : DevExpress.XtraEditors.XtraForm
    {
        public DataTable dtsrc;
        public DataRow drrt;


        public frmSingleSel()
        {
            InitializeComponent();

            new CBLC.lifj.control.upgrade.gridviewupgrade(xgvsinglesel);
        }

        private void frmSingleSel_Load(object sender, EventArgs e)
        {
            xgcsinglesel.DataSource = dtsrc;
        }

        private void xsbcontractaddok_Click(object sender, EventArgs e)
        {

            drrt = xgvsinglesel.GetFocusedDataRow();

            if (drrt == null)
            {
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void xsbcontractaddcancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

  
    }
}