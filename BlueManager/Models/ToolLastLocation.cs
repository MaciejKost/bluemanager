using System;
using BlueManagerPlatform.Models;

namespace BlueManager.Models
{
    public class ToolLastLocation
    {
        public int ToolId { get; set; }
        public int HubId { get; set; }
        public string BleName { get; set; }
        public DateTime Timestamp { get; set; }

        public Tool Tool { get; set; }
        public Hub Hub { get; set; }
    }
}