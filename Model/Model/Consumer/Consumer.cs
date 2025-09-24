using System;
using System.Collections.Generic;

namespace Domain.Model
{
  public class Consumer
  {
    public Guid Consumer_id { get; set; }
    public string Legal_name { get; set; }
    public string Fantasy_name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone_number { get; set; }
    public bool Active { get; set; }
    public DateTime? Created_at { get; set; }
    public DateTime? Deleted_at { get; set; }
    public DateTime? Updated_at { get; set; }
    public Guid User_id { get; set; }
    //public Guid? Default_address { get; set; }
    //public List<Address> Addresses { get; set; }
  }
}
