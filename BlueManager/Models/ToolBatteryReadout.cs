using System;

namespace BlueManager.Models
{
    public class ToolBatteryReadout
    {
        public int Id { get; set; }
        public int ToolId { get; set; }
        public DateTime Timestamp { get; set; }
        public int BatteryState { get; set; }

    }
}