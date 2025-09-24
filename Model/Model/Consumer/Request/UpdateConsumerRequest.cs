using System;

namespace Domain.Model
{
  public class UpdateConsumerRequest
  {
    public Guid Consumer_id { get; set; }
    public Guid? Default_address { get; set; }
    public Guid User_id { get; set; }
    public string Legal_name { get; set; }
    public string Fantasy_name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone_number { get; set; }
    public bool Active { get; set; }
  }
}
