using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class TokenCardRequest
    {
        [JsonProperty("encrypted")]
        public string Encrypted { get; set; }
       // [JsonProperty("holder")]
       // public HolderTokenCard Holder { get; set; }
    }

    public class HolderTokenCard
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
