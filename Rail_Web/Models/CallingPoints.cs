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
        string name { get; set; }
        [JsonProperty("code")]
        string code { get; set; }
        [JsonProperty("time")]
        string time { get; set; }
        [JsonProperty("status")]
        string status { get; set; }
    }
}
