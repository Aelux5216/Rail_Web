using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rail_Web.Models
{
    public class Ticket
    {
        public string reference { get; set; }
        public string depCode { get; set; }
        public string arrCode { get; set; }
        public string classType { get; set; }
        public string totalCost { get; set; }
        public DateTime date { get; set; }
        public string time { get; set; }
    }
}
