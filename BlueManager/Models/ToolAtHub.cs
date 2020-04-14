using System;

namespace BlueManager.Models
{
    public class ToolAtHub
    {
        public int Id { get; set; }
        public int HubId { get; set; }
        public int ToolId { get; set; }
        public string BleName { get; set; }
        public DateTime Timestamp { get; set; }
        public int Strength { get; set; }
        public Tool Tool { get; set; }
        public Hub Hub { get; set; }

    }
}