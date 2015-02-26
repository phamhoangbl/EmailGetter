using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace InquiriesWindowService
{
    public partial class Scheduler : ServiceBase
    {
        private const string appName = "InquiriesWindowService.exe";
        private Process _process;

        public Scheduler()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            string sourcePath = Environment.GetCommandLineArgs()[0].Replace(appName, "").Replace("InquiriesWindowService.vshost.exe", "");
            string fileName = sourcePath + "Source\\" + "EmailGetter.exe";
            //Library.WriteErrorLog(fileName);

            //Create process
            ProcessStartInfo startinfo = new ProcessStartInfo();

            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.FileName = fileName;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;

            //Get program output
            _process = Process.Start(startinfo);
        }

        public static SecureString GetSecureString(string str)
        {
            SecureString secureString = new SecureString();
            foreach (char ch in str)
            {
                secureString.AppendChar(ch);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        protected override void OnStop()
        {
            _process.Kill();
            //Library.WriteErrorLog("End call email getter bat");
            //TO DO Write log
            //Library.WriteErrorLog("Test window service stopped");
        }
    }
}
