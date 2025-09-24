using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class UpdateDeliveryRequest
    {
        public string Delivery_options_id { get; set; }
        [JsonIgnore]
        public string Name { get; set; }
        public bool Active { get; set; }
        [JsonIgnore]
        public  Guid Updated_by { get; set; }
    }
}
