using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace EmailGetter
{
    class Options
    {
        [Option('t', "timeToRefesh", Required = true, HelpText = "Set time to refesh retrieving list email (miliseconds)")]
        public int TimeToRefesh { get; set; }

        [Option('f', "filePathDTS", Required = true,
        HelpText = @"Input directory path DTS DTExec.exe to be processed or input 'd' (Default: 'C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Binn\)'")]
        public string FilePathDTS { get; set; }



        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}