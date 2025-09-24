using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Service
{
  public interface IDeliveryOptionsService
    {
        DeliveryOptionsResponse CreateDelivery(CreateDeliveryRequest createDeliveryRequest, string token);
        ListDeliveryOptionsResponse GetDeliveryOptions(Filter filter);

        DeliveryOptionsResponse ActiveDelivery(UpdateDeliveryRequest updateDeliveryRequest, string token);
    }
}
