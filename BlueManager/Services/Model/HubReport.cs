using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueManager.Services.Model
{
    public class HubReport
    {
        [JsonProperty("dev_corrected")]
        public List<DeviceReport> Devices { get; set; }
    }
}