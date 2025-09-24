using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class AccessTokenResponse
    {
        public string Token_type { get; set; }
        public string Access_token { get; set; }
        public string Expires_in { get; set; }
        public string Refresh_token { get; set; }
        public string Scope { get; set; }
        public string Account_id { get; set; }
        public string Redirect_url_pam_pos_oauth_pagseguro { get; set;}
    }
}
