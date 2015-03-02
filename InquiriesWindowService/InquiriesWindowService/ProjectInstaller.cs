using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace InquiriesWindowService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private const string configFile = "config.txt";
        private const string dtsPath = ":\\Program Files (x86)\\Microsoft SQL Server\\110\\DTS\\Binn\\";

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {

            string dtsDriveName = Context.Parameters["DTSValue"];
            string time = Context.Parameters["TimeValue"];

            if (string.IsNullOrEmpty(dtsDriveName) || string.IsNullOrEmpty(time))
            {
                //Library.WriteErrorLog("invalid installation");
                new ServiceController(serviceInstaller1.ServiceName).Dispose();
                //serviceInstaller1.Uninstall(null);
                throw new InstallException("DTS path or Time interval is not specified");
            }

            base.Install(stateSaver);

            string path = Context.Parameters["Targetdir"];
            string fileName = path + configFile;

            string message = dtsDriveName  + dtsPath + "\r\n" + time;


            Library.WriteErrorLog("path DTS: " + dtsDriveName);
            Library.WriteErrorLog("path of file config : " + fileName);

            //Create file configuration for storing DTS and Time

            FileHelper.WriteFile(fileName, message, null, false);
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller1.ServiceName).Start();
        }

        private void serviceInstaller1_AfterUninstall(object sender, InstallEventArgs e)
        {
            string path = Context.Parameters["Targetdir"];
            string fileName = path + configFile;

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

    }
}
