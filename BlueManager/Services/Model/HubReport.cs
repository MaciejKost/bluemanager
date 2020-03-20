using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueManager.Services.Model
{
    public class HubReport
    {
        [JsonProperty("BLE_Devices")]
        public List<DeviceReport> Devices { get; set; }
    }
}