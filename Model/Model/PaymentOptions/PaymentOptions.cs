using System;
using System.Collections.Generic;

namespace Domain.Model
{
    public class PaymentOptions
    {
        public Guid Payment_options_id { get; set; }
        public int Identifier { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public Guid Payment_local_id { get; set; }
        public string Payment_local_name { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
