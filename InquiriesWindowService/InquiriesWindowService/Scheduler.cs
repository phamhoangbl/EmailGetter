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
        private const string configFile = "config.txt";

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
            Thread.Sleep(3000);
            string sourcePath = Environment.GetCommandLineArgs()[0].Replace(appName, "").Replace("InquiriesWindowService.vshost.exe", "");
            //string fileName = sourcePath + "Source\\" + "EmailGetter.exe";
            string fileName = @"D:\Virtium\ContactForm\Source\EmailGetter.bat";
            //string fileName = @"C:\Users\johnhoang\Desktop\TestProcess.exe";
            Library.WriteErrorLog("Get configuration file info");
            //Read config file to get DTS path and time
            string dtsPath;
            string time;

            ReadFileConfig(sourcePath + configFile, out dtsPath, out time);


            Library.WriteErrorLog("dtsPath: " + dtsPath);
            Library.WriteErrorLog("time: " + time);


            ProcessStartInfo startinfo = new ProcessStartInfo();

            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = false;
            //startinfo.Arguments = string.Format(" -t \"{0}\" -f \"{1}\"", time, dtsPath.Replace(@"\", @"\\"));
            startinfo.FileName = fileName;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;

            Library.WriteErrorLog("file + arguments: " + startinfo.FileName + " " + startinfo.Arguments);

            //Get program output
            _process = Process.Start(startinfo);

            
        }

        //private static void CallDTSApp()
        //{

        //    ProcessStartInfo startinfo = new ProcessStartInfo();
        //    startinfo.UseShellExecute = false;
        //    startinfo.RedirectStandardError = true;
        //    startinfo.CreateNoWindow = true;
        //    startinfo.WindowStyle = ProcessWindowStyle.Hidden;

        //    string dirSPImport = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "") + @"SPImport\";
        //    //string dirSPImport = AppDomain.CurrentDomain.BaseDirectory + @"SPImport\";

        //    startinfo.FileName = "C:\\Program Files (x86)\\Microsoft SQL Server\\110\\DTS\\Binn\\DTEXEC.exe";
        //    startinfo.Arguments = "/f D:\\Virtium\\Projects\\EmailGetter\\EmailGetter\\EmailGetter\\SPImport\\Package.dtsx";

        //    Process p = Process.Start(startinfo);

        //    //To make sure Import Sharepoint success.
        //    Thread.Sleep(30000);


        //    string errors = p.StandardError.ReadToEnd();
        //    Library.WriteErrorLog(errors);
        //}

        protected override void OnStop()
        {
            _process.Kill();
            Library.WriteErrorLog("End call email getter bat");
            //TO DO Write log
            //Library.WriteErrorLog("Test window service stopped");
        }

        private void ReadFileConfig(string fileName, out string dtsPath, out string time)
        {
            dtsPath = null;
            time = null;
            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = String.Empty;
                int line = 1;
                while ((s = sr.ReadLine()) != null)
                {
                    switch (line)
                    {
                        case 1:
                            dtsPath = s;
                            break;
                        case 2:
                            time = s;
                            break;
                    }
                    line++;
                }
            }
        }
    }
}
