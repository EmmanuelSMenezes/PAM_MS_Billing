using System;
using System.Collections.Generic;
using Domain.Model;

namespace Infrastructure.Repository
{
  public interface IDeliveryOptionsRepository
    {
        DeliveryOptionsResponse CreateDelivery(CreateDeliveryRequest createDeliveryRequest);
        ListDeliveryOptionsResponse GetDeliveryOptions(Filter filter);
        DeliveryOptionsResponse ActiveDelivery(UpdateDeliveryRequest updateDeliveryRequest);
    }
}
