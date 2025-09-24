using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class PagSeguro
    {
        public string Token { get; set; }
        public string Url { get; set; }
        public string Method_Split { get; set; }
        public string Account_Id { get; set; }
        public string Redirect_uri { get; set; }
        public string Client_id { get; set; }
        public string Client_secret { get; set; }
        public string Redirect_url_pam_pos_oauth_pagseguro { get; set; }

    }
    public enum Payment_PagSeguro
    {

        CREDIT_CARD,
        DEBIT_CARD,
        PIX
    }

    public enum Payment_PagSeguro_Status
    {
        CANCELED = 0,
        PAID = 1,
        AUTHORIZED = 2,
        IN_ANALYSIS = 3,
        DECLINED = 4,
        WAITING_PIX = 5
    }
}
