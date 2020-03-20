using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlueManager.Services.Model
{
    public class DeviceReport
    {
        [JsonProperty("MAC")]
        public string MacAddress { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Timestamp { get; set; }
    }
}