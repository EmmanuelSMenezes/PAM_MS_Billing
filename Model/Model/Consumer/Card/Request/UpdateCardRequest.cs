using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model 
{ 
    public class UpdateCardRequest
    {
        public Guid Card_id { get; set; }
        public Guid Consumer_id { get; set; }
        public string Encrypted { get; set; }
       
        [JsonIgnore]
        public Guid Updated_by { get; set; }
    }
}
