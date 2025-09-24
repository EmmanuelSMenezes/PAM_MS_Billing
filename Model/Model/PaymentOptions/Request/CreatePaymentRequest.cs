using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CreatePaymentRequest
    {
        public string Description { get; set; }
        [JsonIgnore]
        public  Guid Created_by { get; set; }
    }
}
