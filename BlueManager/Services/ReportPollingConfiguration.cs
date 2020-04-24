namespace BlueManager.Services
{
    public class ReportPollingConfiguration
    {
        /// <summary>
        /// Hubs polling interval (in milliseconds)
        /// </summary>
        public int PollingInterval { get; set; } = 10000;

        /// <summary>
        /// Timeout for a single request to Hub
        /// </summary>
        public double PollingRequestTimeout { get; set; } = 10000;
    }
}