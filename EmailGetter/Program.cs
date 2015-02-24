using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using EmailGetter.Core;
using EmailGetter.Core.Utils;
using EmailGetter.Core.Data.Repository;
using EmailGetter.Core.Data.Model;
using NLog;
using System.Threading;
using System.Net;
using CommandLine;
using CommandLine.Text;
using System.Diagnostics;
using OpenPop.Pop3;
using OpenPop.Mime;

namespace EmailGetter
{
    class Program
    {
        static Logger _logger = LogManager.GetCurrentClassLogger();

        private static string host = ConfigurationManager.AppSettings["EmailHost"];
        private static string emailTitle = ConfigurationManager.AppSettings["EmailTitle"];
        private static string emailUser = ConfigurationManager.AppSettings["EmailUser"];
        private static string emailPassword = ConfigurationManager.AppSettings["EmailPassword"];
        private static int numberEmailFetch = int.Parse(ConfigurationManager.AppSettings["numberOfEmailFetch"]);
        private const string directoryDTS = @"C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Binn\";

        private static string _commandDTS = "DTEXEC.exe";
        private static string _fileDTS = @"Package.dtsx";
        private static int _timeToRefesh;
        private static string _directoryDTS;

        static void Main(string[] args)
        {
            var options = new Options();
            var parser = new CommandLine.Parser(with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options, () => Environment.Exit(-2)))
            {
                Run(options);
            }
        }

        private static void Run(Options options)
        {
            _logger.Info("==========Start Program==============");
            Console.WriteLine("==========Start Program==============");
            _timeToRefesh = options.TimeToRefesh;

            //if do not configure Time and File Path, set them to default then
            if(options.TimeToRefesh == 0 || options.FilePathDTS == null)
            {
                _timeToRefesh = 900000;
                _directoryDTS = directoryDTS;
            }
            else if (options.FilePathDTS == "d")
            {
                _directoryDTS = directoryDTS;
            }
            else
            {
                _directoryDTS = options.FilePathDTS;
            }

            TimerCallback callback = new TimerCallback(Tick);
            Timer stateTimer = new Timer(callback, null, 0, _timeToRefesh);
            Console.ReadLine();
        }

        static public void Tick(Object stateInfo)
        {
            try
            {
                CheckAndStoreEmailContactForm();
                //Console.WriteLine("Tick: {0}-{1}-{2}", DateTime.Now.ToString("h:mm:ss"), _timeToRefesh, _directoryDTS);
            }
            catch(Exception ex)
            {
                Console.WriteLine(string.Format("There is an exception error: {0}", ex));
                _logger.Error(string.Format("There is an exception error {0}", ex));
            }
        }


        private static void CheckAndStoreEmailContactForm()
        {
            List<Message> recentMessages = new List<Message>();
            var contactRepo = new ContactFormRepository();

            try
            {
                _logger.Info(string.Format("{0}: Start retrieving email", DateTime.Now.ToString("h:mm:ss")));
                Console.WriteLine(string.Format("{0}: Start retrieving email", DateTime.Now.ToString("h:mm:ss")));

                Pop3Client pop3Client = new Pop3Client();
                pop3Client.Connect(host, 110, false);
                pop3Client.Authenticate(emailUser, emailPassword);

                for (int i = 1; i <= pop3Client.GetMessageCount(); i++)
                {
                    var message = pop3Client.GetMessage(i);
                    recentMessages.Add(message);
                }

                pop3Client.Disconnect();
                pop3Client.Dispose();

                _logger.Info(string.Format("{0}: End retrieving email", DateTime.Now.ToString("h:mm:ss")));
                Console.WriteLine(string.Format("{0}: End retrieving email", DateTime.Now.ToString("h:mm:ss")));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            var contactFormMessages = recentMessages.Where(x => x.Headers.Subject.Contains(emailTitle));

            foreach (var contactFormMessage in contactFormMessages)
            {
                var message = contactFormMessage.ToMailMessage();

                var contact = EmailTextReader.ReadContactFrom(message.Body);

                //Store contact from into database
                var contactForm = new ContactForm();
                contactForm.City = WebUtility.HtmlDecode(contact.City);
                contactForm.CommentOrQuestion = WebUtility.HtmlDecode(contact.CommentOrQuestion);
                contactForm.CompanyEmail = WebUtility.HtmlDecode(contact.CompanyEmail);
                contactForm.CompanyName = WebUtility.HtmlDecode(contact.CompanyName);
                contactForm.CompanyType = WebUtility.HtmlDecode(contact.CompanyType);
                contactForm.Country = WebUtility.HtmlDecode(contact.Country);
                contactForm.Email = WebUtility.HtmlDecode(contact.Email);
                contactForm.FirstName = WebUtility.HtmlDecode(contact.FirstName);
                contactForm.LastName = WebUtility.HtmlDecode(contact.LastName);
                contactForm.Phone = WebUtility.HtmlDecode(contact.Phone);
                contactForm.PositionTitle = WebUtility.HtmlDecode(contact.PositionTitle);
                contactForm.PrimaryJobFunction = WebUtility.HtmlDecode(contact.PrimaryJobFunction);
                contactForm.State = WebUtility.HtmlDecode(contact.State);
                contactForm.Zip = WebUtility.HtmlDecode(contact.Zip);
                contactForm.SentDate = contactFormMessage.Headers.DateSent;
                contactForm.MessageId = contactFormMessage.Headers.MessageId;
                contactForm.Status = "Open";
                contactForm.IsProcessed = false;

                if (!contactRepo.HasAlready(contactForm.MessageId))
                {
                    try
                    {
                        contactRepo.Insert(contactForm);
                        contactRepo.Save();

                        Console.WriteLine("Store Contact into database successfully");
                        _logger.Info("Store Contact into database successfully");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            //get contact forms is not processed yet
            var unProcessedEmail = contactRepo.Select(false);
            if (unProcessedEmail.Any())
            {
                Console.WriteLine("Begin call Console App exe");
                _logger.Info("Begin call Console App exe");

                Console.WriteLine(string.Format("{0}: There is {1} entry(ies) to process", DateTime.Now.ToString("h:mm:ss"), unProcessedEmail.Count()));
                _logger.Info(string.Format("{0}: There is {1} entry(ies) to process", DateTime.Now.ToString("h:mm:ss"), unProcessedEmail.Count()));

                //Execute DTS SSIS
                try
                {
                    CallDTSApp();

                    //Update IsProcessed
                    foreach (var email in unProcessedEmail)
                    {
                        var contactForm = contactRepo.Select(email.MessageId);
                        contactForm.IsProcessed = true;
                    }
                    contactRepo.Save();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                Console.WriteLine("Done call Console App exe");
                _logger.Info("Done call Console App exe");
            }
            else
            {
                Console.WriteLine(string.Format("{0}: There is no entry to process", DateTime.Now.ToString("h:mm:ss")));
                _logger.Info(string.Format("{0}: There is no entry to process", DateTime.Now.ToString("h:mm:ss")));
            }
        }

        private static void CallDTSApp()
        {
            ProcessStartInfo startinfo = new ProcessStartInfo();
            startinfo.UseShellExecute = false;
            startinfo.CreateNoWindow = true;
            startinfo.WindowStyle = ProcessWindowStyle.Hidden;

            //string dirSPImport = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "") + @"SPImport\";
            string dirSPImport = AppDomain.CurrentDomain.BaseDirectory + @"SPImport\";
            startinfo.FileName = string.Format("{0}", _directoryDTS + _commandDTS);
            startinfo.Arguments =  " /f " + dirSPImport + _fileDTS;
            startinfo.RedirectStandardOutput = true;

            Process p = Process.Start(startinfo);

            //To make sure Import Sharepoint success.
            Thread.Sleep(30000);
        }
    }
}
