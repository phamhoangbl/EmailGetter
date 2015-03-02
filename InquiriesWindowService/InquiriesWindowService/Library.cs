using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InquiriesWindowService
{
    public static class Library
    {
        private static string logFilePath = "C:\\logs";
        private static string logFileName = "C:\\logs\\InstallerEmailGetter.txt";
        //public static void WriteErrorLog(Exception ex)
        //{
        //    StreamWriter sw = null;
        //    try
        //    {

        //        sw = new StreamWriter(logFile, true);
        //        sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
        //        sw.Flush();
        //        sw.Close();
        //    }
        //    catch
        //    {
        //    }
        //}

        public static void WriteErrorLog(string message)
        {
            FileHelper.WriteFile(logFileName, message, logFilePath);
        }
    }
}
