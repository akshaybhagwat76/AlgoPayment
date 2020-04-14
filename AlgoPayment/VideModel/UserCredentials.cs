using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlgoPayment.VideModel
{
    public class UserCredentials
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string emailid { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string SocialId { get; set; }
        public string Password { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}