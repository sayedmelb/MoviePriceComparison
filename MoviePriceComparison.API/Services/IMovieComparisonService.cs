using MoviePriceComparison.API.Models;

namespace MoviePriceComparison.API.Services
{
    public interface IMovieComparisonService
    {
        Task<ApiResponse<List<Movie>>> GetAllMovieComparisonsAsync();
        Task<ApiResponse<MovieComparison>> GetMovieComparisonAsync(string movieTitle, string year);
        Task<List<ProviderStatus>> GetProviderStatusAsync();
        Task<ApiResponse<MovieDetail>> GetMovieDetailAsync(string provider,string id);
    }
}
