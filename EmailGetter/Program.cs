using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpaqueMail.Net;
using System.Configuration;
using EmailGetter.Core;
using EmailGetter.Core.Utils;
using EmailGetter.Core.Data.Repository;
using EmailGetter.Core.Data.Model;
using NLog;
using System.Threading;

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

        static void Main(string[] args)
        {
            TimerCallback callback = new TimerCallback(Tick);
            Timer stateTimer = new Timer(callback, null, 0, 10000);
            Console.ReadLine();
        }

        static public void Tick(Object stateInfo)
        {
            try
            {
                //CheckAndStoreEmailContactForm();
                Console.WriteLine("Tick: {0}", DateTime.Now.ToString("h:mm:ss"));
            }
            catch(Exception ex)
            {
                _logger.Error("There is an exception error", ex);
            }
        }


        private static void CheckAndStoreEmailContactForm()
        {
            Pop3Client pop3Client = new Pop3Client(host, 110, emailUser, emailPassword, false);
            pop3Client.Connect();
            pop3Client.Authenticate();

            List<MailMessage> recentMessages = new List<MailMessage>();
            try
            {
                recentMessages = pop3Client.GetMessages(numberEmailFetch, 1, false, false);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw ex;
            }


            var titles = recentMessages.Select(x => x.Subject).ToList();

            var contactFormMessage = recentMessages.Where(x => x.Subject == emailTitle);

            foreach (var message in contactFormMessage)
            {
                var contact = EmailTextReader.ReadContactFrom(message.Body);
                var contactRepo = new ContactFormRepository();

                //Store contact from into database
                var contactForm = new ContactForm();
                contactForm.City = contact.City;
                contactForm.CommentOrQuestion = contact.CommentOrQuestion;
                contactForm.CompanyEmail = contact.CompanyEmail;
                contactForm.CompanyName = contact.CompanyName;
                contactForm.CompanyType = contact.CompanyType;
                contactForm.Country = contact.Country;
                contactForm.Email = contact.Email;
                contactForm.FirstName = contact.FirstName;
                contactForm.LastName = contact.LastName;
                contactForm.Phone = contact.Phone;
                contactForm.PositionTitle = contact.PositionTitle;
                contactForm.PrimaryJobFunction = contact.PrimaryJobFunction;
                contactForm.State = contact.State;
                contactForm.Zip = contact.Zip;
                contactForm.SentDate = message.Date;
                contactForm.MessageId = message.MessageId;

                if (!contactRepo.HasAlready(message.MessageId))
                {
                    contactRepo.Insert(contactForm);
                }
            }
        }

    }
}
