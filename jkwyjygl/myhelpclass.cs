using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using   System.Runtime.InteropServices;


namespace CBLC.lifj.control.upgrade
{


    public class gridviewupgrade 
    {

        protected bool bctrl;
        protected GridView _view;
        protected LayoutView _lview;

        protected string _grouppanelcaption;

        

        public string GroupPanelCaption
        {
            get { return _grouppanelcaption; }
            set { _grouppanelcaption = value; }
        }


        public void AddFNCNoCol(bool bfnc)
        {
            if (bfnc)
            {
                GridColumn _gc = _view.Columns.Add();
                _gc.Caption = "合同编号(财)";
                _gc.FieldName = "contractnofnc";
                _gc.VisibleIndex = 0;

                _gc.Visible = true;
            }

        }

        public gridviewupgrade(GridView view)            
        {
            View = view;
        }

        public gridviewupgrade(LayoutView view)
        {
            LView = view;
        }

        public LayoutView LView
        {
            get { return _lview; }
            set
            {
                if (_lview != value)
                {
                    Detach();
                    LAttach(value);
                }
            }
        }


        public GridView View
        {
            get { return _view; }
            set 
            {
                if (_view != value) 
                {
                    Detach();
                    Attach(value);
                }
            }
        }

        protected virtual void LAttach(LayoutView view)
        {
            if (view == null) return;
            this._lview = view;
            view.BeginUpdate();
            try
            {

              // view.RowStyle += new RowStyleEventHandler(view_RowStyle);
            }
            finally
            {
                view.EndUpdate();
            }
        }

        protected virtual void Attach(GridView view)
        {
            if (view == null) return;
            this._view = view;
            view.BeginUpdate();
            try 
            {
                bctrl = false;

                view.RowStyle += new RowStyleEventHandler(view_RowStyle);
                view.CustomDrawGroupPanel += new CustomDrawEventHandler(view_CustomDrawGroupPanel);
                view.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(view_SelectionChanged);
                view.MouseMove+=new MouseEventHandler(view_MouseMove);
                view.MouseLeave += new EventHandler(view_MouseLeave);
                view.DataSourceChanged += new EventHandler(view_DataSourceChanged);
                
            } 
            finally 
            {
                view.EndUpdate();
            }
        }

        protected virtual void Detach() 
        {
            if (_view == null) return;

            _view = null;
        }
        
        void view_RowStyle(object sender, RowStyleEventArgs e) 
        {
            
            if (_view.IsRowSelected(e.RowHandle))
            {
                if((jkwyjygl.Form1.uid!="16")&&
                   (jkwyjygl.Form1.uid != "19")
                )
                {
                    e.Appearance.BackColor = SystemColors.Highlight;

                    e.HighPriority = true;
                }

            }
        }

        void view_CustomDrawGroupPanel(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            GridView gv = sender as GridView;
            GridElementsPainter elementsPainter = (gv.GetViewInfo() as GridViewInfo).Painter.ElementsPainter;
            StyleObjectInfoArgs groupArgs = new StyleObjectInfoArgs(e.Cache, e.Bounds, e.Appearance, ObjectState.Normal);
            elementsPainter.GroupPanel.DrawObject(groupArgs);

            Brush brush = e.Cache.GetGradientBrush(e.Bounds, Color.Blue, Color.Blue, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);

            Point p = new Point(e.Bounds.X + e.Bounds.Width - 125, e.Bounds.Y + (e.Bounds.Height - 20) / 2);

            string srecs = "总：" + gv.RowCount.ToString()+"  选中："+gv.SelectedRowsCount.ToString();
            e.Graphics.DrawString(srecs, e.Appearance.Font, brush, p);

            p = new Point(e.Bounds.X + 0, e.Bounds.Y + (e.Bounds.Height - 20) / 2);

            srecs = _grouppanelcaption;
            e.Graphics.DrawString(srecs, e.Appearance.Font, brush, p);

            e.Handled = true;
 
        }

        void view_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            (sender as GridView).RefreshData();
        }

        [DllImport("user32.dll", SetLastError = true)]           
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);  
        public const uint KEYEVENTF_KEYUP = 0x02;
        public const uint VK_CONTROL = 0x11;

        void view_MouseLeave(object sender, EventArgs e)
        {
            if (bctrl)
            {
                keybd_event((byte)VK_CONTROL, 0, (byte)KEYEVENTF_KEYUP, 0);
                bctrl = false;
            }
        }

        void view_DataSourceChanged(object sender, EventArgs e)
        {
            if(((sender as GridView).Name!="xgvrptmgtfee")&&
               ((sender as GridView).Name != "xgvmgtfeequery")
            )(sender as GridView).BestFitColumns();
        }

        void view_MouseMove(object sender, MouseEventArgs e)
        {

           
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hit =
                    (sender as DevExpress.XtraGrid.Views.Grid.GridView).CalcHitInfo(e.Location);



            if (hit.HitTest == GridHitTest.RowIndicator)
            {
                if (!bctrl)
                {
                    keybd_event((byte)VK_CONTROL, 0, 0, 0);
                    bctrl = true;
                }
            }
            else
            {
                if (bctrl)
                {
                    keybd_event((byte)VK_CONTROL, 0, (byte)KEYEVENTF_KEYUP, 0);
                    bctrl = false;
                }
            }

        }


    }
}