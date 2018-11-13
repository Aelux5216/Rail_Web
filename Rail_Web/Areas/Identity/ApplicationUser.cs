using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // Add profile data for application users by adding properties to the ApplicationUser class

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string HouseName { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Postcode { get; set; }
    }
}
