using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlueManager.Services.Model
{
    public class HealthSystemReport
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("entries")]
        public Entries entries { get; set; }
    }

    public class Entries
    {
        [JsonProperty("sqlserver")]
        public SqlServer SqlServer { get; set; }
        [JsonProperty("Hubs")]
        public Hubs Hubs { get; set; }
    }

    public class SqlServer
    {
        //[JsonProperty("data")]
        //public string Data { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class Hubs
    {
        [JsonProperty("data")]
        public Dictionary<string, HubData> Data { get; set; }
        //public object Data { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class HubData
    {
        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }
        [JsonProperty("locationName")]
        public string LocationName { get; set; }
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("healthStatus")]
        public string HealthStatus { get; set; }
    }
}