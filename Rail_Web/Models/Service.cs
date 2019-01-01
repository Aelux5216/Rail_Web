using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rail_Web.Models
{
    public class Service
    {
        [JsonProperty("service_id")]
        public string service_id { get; set; }
        [JsonProperty("operator")]
        public string service_operator { get; set; }
        [JsonProperty("dep_name")]
        public string dep_name { get; set; }
        [JsonProperty("dep_code")]
        public string dep_code { get; set; }
        [JsonProperty("dep_time")]
        public DateTime dep_time { get; set; }
        [JsonProperty("dep_platform")]
        public string dep_platform { get; set; }
        [JsonProperty("arr_name")]
        public string arr_name { get; set; }
        [JsonProperty("arr_code")]
        public string arr_code { get; set; }
        [JsonProperty("arr_time")]
        public DateTime arr_time { get; set; }
        [JsonProperty("arr_platform")]
        public string arr_platform { get; set;}
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("disrupt_reason")]
        public string disrupt_reason { get; set; }
        [JsonProperty("calls_at")]
        public string[] Calls_at_Temp { get; set; }
        public List<CallingPoints> Calls_at { get; set; }
        [JsonProperty("stops")]
        public int stops { get; set; }
    }
}
