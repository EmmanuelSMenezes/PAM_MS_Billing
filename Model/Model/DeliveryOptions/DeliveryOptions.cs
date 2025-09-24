using System;

namespace Domain.Model
{
    public class DeliveryOptions
    {
        public Guid Delivery_option_id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public Guid Created_by { get; set; }
        public DateTime Created_at { get; set; }
        public Guid? Updated_by { get; set; }
        public DateTime? Updated_at { get; set; }
    }
}
