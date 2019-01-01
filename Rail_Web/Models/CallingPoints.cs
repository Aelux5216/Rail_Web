using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rail_Web.Models
{
    public class CallingPoints
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("code")]
        public string code { get; set; }
        [JsonProperty("time")]
        public string time { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
    }
}
