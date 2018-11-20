using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rail_Web.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() { }

        public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string HouseName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Postcode { get; set; }
    }
}
