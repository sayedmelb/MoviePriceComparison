namespace MoviePriceComparison.API.Models
{
    public class ProviderPrice
    {
        public string Provider { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public bool Available { get; set; }
        public string? Error { get; set; }
        public bool IsCheapest { get; set; }
        public DateTime LastChecked { get; set; }
        public string MovieId { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

}
