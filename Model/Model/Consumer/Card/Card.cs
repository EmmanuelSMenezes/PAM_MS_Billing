using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Card
    {
        public Guid Card_id { get; set; }
        public Guid Consumer_id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Validity { get; set; }
        public bool Active { get; set; }
        public string Id { get; set; }
        public string Brand { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get;set; }
    }
}
