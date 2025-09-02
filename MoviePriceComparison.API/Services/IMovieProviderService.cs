using MoviePriceComparison.API.Models;

namespace MoviePriceComparison.API.Services
{
    public interface IMovieProviderService
    {
        Task<ApiResponse<MovieSummary>> GetMoviesAsync(string provider);
        Task<ApiResponse<MovieDetail>> GetMovieDetailsAsync(string provider, string movieId);
        Task<List<ProviderStatus>> GetProviderHealthAsync();
    }
}
