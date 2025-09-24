using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class BankPartner
    {
        public Guid Bank_details_id { get; set; }
        public Guid Partner_id { get; set; }
        public string Bank { get; set; }
        public string Agency { get; set; }
        public string Account_number { get; set; }
        public string Account_id { get; set; }
        public bool Active { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
