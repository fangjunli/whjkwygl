using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using System.Configuration;

namespace jkwyjygl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DevExpress.Skins.SkinManager.EnableFormSkins();
            DevExpress.UserSkins.BonusSkins.Register();

            string us = ConfigurationManager.AppSettings["cusskin"];
            UserLookAndFeel.Default.SetSkinStyle(us);

            
            Application.Run(new Form1());
        }
    }
}