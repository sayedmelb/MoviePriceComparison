namespace MoviePriceComparison.API.Models
{
    public class ProviderStatus
    {
        public string Provider { get; set; } = string.Empty;
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastChecked { get; set; }
        public TimeSpan? ResponseTime { get; set; }
    }
}
