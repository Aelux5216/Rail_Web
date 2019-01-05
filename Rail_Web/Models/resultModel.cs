using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rail_Web.Models
{
    public class resultModel
    {
        public List<Service> resultValue {get;set;}
        public Ticket ticketInstance { get; set; }
        public string error { get; set; }

        public static resultModel modelInstance { get; set; }
    }
}
