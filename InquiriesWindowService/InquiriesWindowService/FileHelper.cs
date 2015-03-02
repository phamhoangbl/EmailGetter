using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InquiriesWindowService
{
    public static class FileHelper
    {
        public static void WriteFile(string fileName, string message, string filePath = null, bool isIncludeTime = true)
        {
            if (filePath != null)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(fileName, true);
                if (isIncludeTime)
                {
                    sw.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + ": " + message);
                }
                else
                {
                    sw.WriteLine(message);
                }
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }
    }
}
