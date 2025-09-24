using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UpdatePaymentRequest
    {
        public string Payment_options_id { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        public bool Active { get; set; }
        [JsonIgnore]
        public  Guid Updated_by { get; set; }
    }
}
