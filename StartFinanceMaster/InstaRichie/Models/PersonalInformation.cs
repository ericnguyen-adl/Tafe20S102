using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class PersonalInformation
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [Unique]
        public string Name { get; set; }

        [NotNull]
        public double Age { get; set; }

        [NotNull]
        public string Gender { get; set; }
    }
}
