using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class AccessToken
    {
        public string grant_type {  get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }
        public string sms_code { get; set; }
    }
}
