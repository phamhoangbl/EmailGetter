using EmailGetter.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailGetter.Core.Data.Repository
{
    public class ContactFormRepository : IContactFormRepository
    {
        private EmailGetterContext db = null;

        public ContactFormRepository()
        {
            this.db = new EmailGetterContext();
        }

        public IEnumerable<ContactForm> SelectAll()
        {
            return db.ContactForm.ToList();
        }

        public ContactForm Select(string messageId)
        {
            return db.ContactForm.Where(x => x.MessageId == messageId).FirstOrDefault();
        }

        public IEnumerable<ContactForm> Select(bool isProcessed)
        {
            return db.ContactForm.Where(x => x.IsProcessed == isProcessed).ToList();
        }

        public void Insert(ContactForm obj, out int contactFormId)
        {
            var a = db.ContactForm.Add(obj);
            db.SaveChanges();
            contactFormId = a.ContactFormId;
        }

        public void Insert(ContactForm obj)
        {
            db.ContactForm.Add(obj);
            db.SaveChanges();
        }

        public bool HasAlready(string messageId)
        {
            return db.ContactForm.Any(x => x.MessageId == messageId);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
