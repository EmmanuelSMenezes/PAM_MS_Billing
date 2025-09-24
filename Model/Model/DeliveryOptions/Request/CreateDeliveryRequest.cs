using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CreateDeliveryRequest
    {
        public string Name { get; set; }
        [JsonIgnore]
        public  Guid Created_by { get; set; }
    }
}
