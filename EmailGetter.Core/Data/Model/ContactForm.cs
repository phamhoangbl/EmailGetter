using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailGetter.Core.Data.Model
{
    public class ContactForm
    {
        public int ContactFormId { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public string CompanyEmail { set; get; }
        public string City { set; get; }
        public string State { set; get; }
        public string Zip { set; get; }
        public string Country { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string CompanyName { set; get; }
        public string CompanyType { set; get; }
        public string PositionTitle { set; get; }
        public string PrimaryJobFunction { set; get; }
        public string CommentOrQuestion { set; get; }
        public DateTime SentDate { set; get; }
        public string MessageId { set; get; }
    }
}
