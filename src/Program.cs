using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Schluesselzahlen
{
    static class Program
    {
        //public static Schluesselzahlen s;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        /// 
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //s = new Schluesselzahlen();
            Application.Run(new Schluesselzahlen());
        }
    }
}
