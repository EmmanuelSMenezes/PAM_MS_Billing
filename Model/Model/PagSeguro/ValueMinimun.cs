using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ValueMinimun
    {
        public Guid Pagseguro_value_minimum_id { get; set; }
        public Guid Partner_id { get; set; }
        public decimal Value_minimum_standard { get; set; }
        public decimal Value_minimum { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
