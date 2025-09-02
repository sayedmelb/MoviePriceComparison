using Microsoft.Extensions.Caching.Memory;
using MoviePriceComparison.API.Models;
using System.Diagnostics;
using System.Text.Json;

namespace MoviePriceComparison.API.Services
{
    public class MovieProviderService : IMovieProviderService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MovieProviderService> _logger;
        private readonly string _baseUrl = "https://webjetapitest.azurewebsites.net/api";
        private readonly string[] _providers = { "cinemaworld", "filmworld" };

        public MovieProviderService(HttpClient httpClient, IMemoryCache cache, ILogger<MovieProviderService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<MovieSummary>> GetMoviesAsync(string provider)
        {
            var cacheKey = $"movies_{provider}";

            // Check cache first
            if (_cache.TryGetValue(cacheKey, out ApiResponse<MovieSummary>? cachedResult))
            {
                _logger.LogInformation($"Returning cached movies for {provider}");
                return cachedResult!;
            }

            var result = new ApiResponse<MovieSummary>();

            try
            {
                var response = await ExecuteWithRetryAsync(async () =>
                {
                    var httpResponse = await _httpClient.GetAsync($"{_baseUrl}/{provider}/movies");
                    httpResponse.EnsureSuccessStatusCode();
                    return await httpResponse.Content.ReadAsStringAsync();
                });

                var movies = JsonSerializer.Deserialize<MovieSummary>(response);

                result.Data = movies as MovieSummary;
                result.Success = true;
                result.Message = $"Successfully retrieved {result.Data.Movies.Count} movies from {provider}";

              
                // Cache for 5 minutes
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

                _logger.LogInformation($"Successfully fetched {result.Data.Movies.Count} movies from {provider}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch movies from {provider}");
                result.Success = false;
                result.Message = $"Failed to retrieve movies from {provider}";
                result.Errors.Add(ex.Message);

                // Cache failed result for shorter time to allow quick retry
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
            }

            return result;
        }

        public async Task<ApiResponse<MovieDetail>> GetMovieDetailsAsync(string provider, string movieId)
        {
            var cacheKey = $"movie_{provider}_{movieId}";

            // Check cache first
            if (_cache.TryGetValue(cacheKey, out ApiResponse<MovieDetail>? cachedResult))
            {
                _logger.LogInformation($"Returning cached movie details for {movieId} from {provider}");
                return cachedResult!;
            }

            var result = new ApiResponse<MovieDetail>();

            try
            {
                var response = await ExecuteWithRetryAsync(async () =>
                {
                    var httpResponse = await _httpClient.GetAsync($"{_baseUrl}/{provider}/movie/{movieId}");
                    httpResponse.EnsureSuccessStatusCode();
                    return await httpResponse.Content.ReadAsStringAsync();
                });

                var movie = JsonSerializer.Deserialize<MovieDetail>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                result.Data = movie;
                result.Success = true;
                result.Message = $"Successfully retrieved movie details from {provider}";

                // Cache for 10 minutes
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));

                _logger.LogInformation($"Successfully fetched movie {movieId} from {provider}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to fetch movie {movieId} from {provider}");
                result.Success = false;
                result.Message = $"Failed to retrieve movie details from {provider}";
                result.Errors.Add(ex.Message);

                // Cache failed result for shorter time
                _cache.Set(cacheKey, result, TimeSpan.FromSeconds(30));
            }

            return result;
        }

        public async Task<List<ProviderStatus>> GetProviderHealthAsync()
        {
            var healthTasks = _providers.Select(CheckProviderHealthAsync);
            var healthResults = await Task.WhenAll(healthTasks);
            return healthResults.ToList();
        }

        private async Task<ProviderStatus> CheckProviderHealthAsync(string provider)
        {
            var stopwatch = Stopwatch.StartNew();
            var status = new ProviderStatus
            {
                Provider = provider,
                LastChecked = DateTime.UtcNow
            };

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await _httpClient.GetAsync($"{_baseUrl}/{provider}/movies", cts.Token);

                stopwatch.Stop();
                status.ResponseTime = stopwatch.Elapsed;
                status.IsHealthy = response.IsSuccessStatusCode;
                status.Status = response.IsSuccessStatusCode ? "healthy" : $"error ({response.StatusCode})";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                status.IsHealthy = false;
                status.Status = "error";
                status.ResponseTime = stopwatch.Elapsed;
                _logger.LogWarning($"Health check failed for {provider}: {ex.Message}");
            }

            return status;
        }

        private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
        {
            var delay = TimeSpan.FromMilliseconds(500);

            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (i < maxRetries)
                {
                    _logger.LogWarning($"Attempt {i + 1} failed: {ex.Message}. Retrying in {delay.TotalMilliseconds}ms");
                    await Task.Delay(delay);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2); // Exponential backoff
                }
            }

            throw new InvalidOperationException($"Operation failed after {maxRetries + 1} attempts");
        }
    }
}
