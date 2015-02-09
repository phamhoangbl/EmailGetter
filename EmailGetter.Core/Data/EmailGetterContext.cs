using EmailGetter.Core.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailGetter.Core.Data
{
    public class EmailGetterContext : DbContext
    {
        public DbSet<ContactForm> ContactForm { get; set; }
    }
}
