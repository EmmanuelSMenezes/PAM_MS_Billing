using System;

namespace Domain.Model
{
  public class CreateConsumerRequest
  {
    public string Legal_name { get; set; }
    public string Fantasy_name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone_number { get; set; }
    public Guid User_id { get; set; }
  }
}
