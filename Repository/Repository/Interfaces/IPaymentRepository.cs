using System;
using System.Collections.Generic;
using Domain.Model;
using Domain.Model.Orders;

namespace Infrastructure.Repository
{
  public interface IPaymentRepository
  {
        PaymentOptionsResponse CreatePayment(CreatePaymentRequest createPaymentRequest);
        ListPaymentOptionsResponse GetPaymentOptions(Filter filter);
        PaymentOptionsResponse ActivePayment(UpdatePaymentRequest updatePaymentRequest);
        List<OrderPix> GetOrdersPix();
        bool CreatePaymentHistory(Guid order_id, string jsonpayment, int satus);
        ValueMinimun GetPagSeguroValueMin(Guid partner_id);
        bool CreateAccessTokenHistory(Partner partner, string jsonaccesstoken);
        Partner GetPartnerByUserId(Guid user_id);
        BankPartner CreateBank(string account_id, Partner partner);
    }
}
