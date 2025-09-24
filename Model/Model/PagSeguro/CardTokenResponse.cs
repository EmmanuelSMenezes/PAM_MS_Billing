using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class CardTokenResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("brand")]
        public string Brand { get; set; }
        [JsonProperty("first_digits")]
        public string First_digits { get; set; }
        [JsonProperty("last_digits")]
        public string Last_digits { get; set; }
        [JsonProperty("exp_month")]
        public string Exp_month { get; set; }
        [JsonProperty("exp_year")]
        public string Exp_year { get; set; }
        [JsonProperty("holder")]
        public Holder Holder { get; set; }
        public Guid? Created_by { get; set; }
        public Guid? Consumer_id { get; set; }
        public Guid? Card_id { get; set; }
    }
    public class Holder
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
