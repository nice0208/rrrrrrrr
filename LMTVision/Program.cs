using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PylonC.NET;
using System.Threading;
using System.Reflection;

namespace LMTVision
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;
            using (Mutex mutex = new Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {

#if DEBUG
                    /* This is a special debug setting needed only for GigE cameras.
                            See 'Building Applications with pylon' in the Programmer's Guide. */
                    Environment.SetEnvironmentVariable("PYLON_GIGE_HEARTBEAT", "10000" /*ms*/);
#endif
                    try
                    {
                        Pylon.Initialize();
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Assembly.LoadFrom(Application.StartupPath + "\\LMTVision.XmlSerializers.dll");
                        Thread.Sleep(1000);
                        Application.Run(new FrmMain());
                    }
                    catch (Exception es)
                    {
                        Pylon.Terminate();
                        //MessageBox.Show(es.Message);
                        //throw;
                    }
                    //Pylon.Terminate();
                }
                else
                {
                    MessageBox.Show("应用程序已经在运行中···");
                    System.Threading.Thread.Sleep(1000);
                    System.Environment.Exit(1);
                }
            }
        }
    }

}
