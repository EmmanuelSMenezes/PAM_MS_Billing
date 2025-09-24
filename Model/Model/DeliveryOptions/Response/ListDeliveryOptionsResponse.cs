using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ListDeliveryOptionsResponse
    {
        public List<DeliveryOptions> Delivery_options { get; set; }
        public Pagination Pagination { get; set; }
    }
}
