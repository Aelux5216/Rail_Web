using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Rail_Web.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the Rail_WebUser class
    public class Rail_WebUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string HouseName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Postcode { get; set; }
    }
}
