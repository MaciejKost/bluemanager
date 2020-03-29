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

        [JsonProperty("battery_time")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime BatteryTimestamp { get; set; }
      
        [JsonProperty("battery_level")]
        public int BatteryLevel { get; set; }
    }
}