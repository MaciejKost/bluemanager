using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BlueManager.Services.Model
{
    public class CheckReport
    {
        public string IpAddress { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }              
        public bool Status { get; set; }
    }
}