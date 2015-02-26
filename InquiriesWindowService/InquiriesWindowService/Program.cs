using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace InquiriesWindowService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
//#if DEBUG
//            Scheduler myScheduler = new Scheduler();
//            myScheduler.OnDebug();
//#else
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] 
            { 
                new Scheduler() 
            };
            ServiceBase.Run(ServicesToRun);
//#endif

        }
    }
}
