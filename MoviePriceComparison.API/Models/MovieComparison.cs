namespace MoviePriceComparison.API.Models
{
    public class MovieComparison
    {
        public string MovieId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public List<ProviderPrice> Providers { get; set; } = new();
        public string Provider { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal? CheapestPrice { get; set; }
        public string? CheapestProvider { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CheapestMovieId { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public Dictionary<string, string> ProviderMovieIds;
    }
}
