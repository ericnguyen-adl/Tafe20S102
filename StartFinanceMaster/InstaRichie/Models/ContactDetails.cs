using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    class ContactDetails
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Unique]
        public string Name { get; set; }

        [NotNull]
        public string TelNum { get; set; }

        [NotNull]
        public string Address { get; set; }
    }
}
