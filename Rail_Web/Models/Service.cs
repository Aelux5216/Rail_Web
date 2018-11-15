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
        string service_id { get; set; }
        [JsonProperty("operator")]
        string service_operator { get; set; }
        [JsonProperty("dep_name")]
        string dep_name { get; set; }
        [JsonProperty("dep_code")]
        string dep_code { get; set; }
        [JsonProperty("dep_time")]
        string dep_time { get; set; }
        [JsonProperty("dep_platform")]
        string dep_platform { get; set; }
        [JsonProperty("arr_name")]
        string arr_name { get; set; }
        [JsonProperty("arr_code")]
        string arr_code { get; set; }
        [JsonProperty("arr_time")]
        string arr_time { get; set; }
        [JsonProperty("arr_platform")]
        string arr_platform { get; set;}
        [JsonProperty("status")]
        string status { get; set; }
        [JsonProperty("disrupt_reason")]
        string disrupt_reason { get; set; }
        [JsonProperty("calls_at")]
        string[] Calls_at { get; set; }
    }
}
