using EmailGetter.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailGetter.Core.Data.Repository
{
    public interface IContactFormRepository
    {
        IEnumerable<ContactForm> SelectAll();
        void Insert(ContactForm obj, out int contactFromId);
        void Save();
    }
}
