namespace MoviePriceComparison.API.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string ID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
        public decimal? Price { get; set; }
        public string Provider { get; set; }
    }
}
