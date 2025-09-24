using System;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Service
{
  public interface IPaymentOptionsService
  {
        PaymentOptionsResponse CreatePayment(CreatePaymentRequest createPaymentRequest, string token);
        ListPaymentOptionsResponse GetPaymentOptions(Filter filter);
        PaymentOptionsResponse ActivePayment(UpdatePaymentRequest updatePaymentRequest, string token);
        ValueMinimun GetPagSeguroValueMin(Guid partner_id);
        AccessTokenResponse PostAccessTokenPagSeguro(string code, string state);
        
    }
}
