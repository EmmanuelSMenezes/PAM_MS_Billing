using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model.Orders
{
    public class OrderPix
    {
        public string Id { get; set; } 
        public Guid Order_id { get; set; }
        public Guid Created_by { get; set; }
    }
}
