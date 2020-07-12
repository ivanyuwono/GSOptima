using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace GSOptima.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string MembershipType { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Address { get; set; }


        public List<StockWatchList> StockWatchList { get; set; }
    }
}
