namespace BlueManager.Services.Model
{
    internal class ReportDownload
    {
        public ReportDownload(int hubId, bool isSuccessful, HubReport report)
        {
            HubId = hubId;
            IsSuccessful = isSuccessful;
            Report = report;
        }

        public int HubId { get; set; }
        public bool IsSuccessful { get; set; }
        public HubReport Report { get; set; }
    }
}