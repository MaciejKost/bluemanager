using System;

namespace BlueManagerPlatform.Models
{
    public class ToolBatteryReadout
    {
        public int ToolId { get; set; }
        public DateTime Timestamp { get; set; }
        public int BatteryState { get; set; }

    }
}