using Microsoft.Extensions.Caching.Memory;
using MoviePriceComparison.API.Models;

namespace MoviePriceComparison.API.Services
{
    public class MovieComparisonService : IMovieComparisonService
    {
        private readonly IMovieProviderService _providerService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MovieComparisonService> _logger;
        private readonly string[] _providers = { "cinemaworld", "filmworld" };

        public MovieComparisonService(IMovieProviderService providerService, IMemoryCache cache, ILogger<MovieComparisonService> logger)
        {
            _providerService = providerService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<List<Movie>>> GetAllMovieComparisonsAsync()
        {
            var cacheKey = "all_movie_comparisons";

            if (_cache.TryGetValue(cacheKey, out ApiResponse<List<Movie>>? cachedResult))
            {
                _logger.LogInformation("Returning cached movie comparisons");
                return cachedResult!;
            }

            var result = new ApiResponse<List<Movie>>();

            try
            {
                // Fetch movies from all providers in parallel
                var moviesTasks = _providers.Select(provider => _providerService.GetMoviesAsync(provider));
                var moviesResults = await Task.WhenAll(moviesTasks);

                var movieMap = new Dictionary<string, MovieComparison>();
                var availableProviders = new List<string>();

                for (int i = 0; i < _providers.Length; i++)
                {
                    var provider = _providers[i];
                    var movieResult = moviesResults[i];

                    if (movieResult.Success && movieResult.Data.Movies != null)
                    {
                        availableProviders.Add(provider);
                        foreach (var movie in movieResult.Data.Movies)
                        {
                            var key = $"{movie.Title}_{movie.Year}";

                            if (!movieMap.ContainsKey(key))
                            {
                                movieMap[key] = new MovieComparison
                                {
                                    Title = movie.Title,
                                    Year = movie.Year,
                                    Type = movie.Type,
                                    Poster = movie.Poster,
                                    Provider = provider,
                                    LastUpdated = DateTime.UtcNow,
                                    ProviderMovieIds = new Dictionary<string, string>() // new for mapping
                                };
                            }

                            movieMap[key].ProviderMovieIds[provider] = movie.ID;
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to get movies from {provider}: {movieResult.Message}");
                    }
                }

                if (movieMap.Count == 0)
                {
                    result.Success = false;
                    result.Message = "No movies available from any provider";
                    return result;
                }

                // Fetch detailed prices for each movie in parallel with concurrency control
                var semaphore = new SemaphoreSlim(5);
                var comparisonTasks = movieMap.Values.Select(async movie =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        await PopulateMoviePricesAsync(movie, availableProviders, movie.ProviderMovieIds);
                        return movie;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var completedComparisons = await Task.WhenAll(comparisonTasks);

                result.Data = completedComparisons.Select(s => new Movie
                {
                    ID = s.CheapestProvider != null && s.ProviderMovieIds.ContainsKey(s.CheapestProvider)
                        ? s.ProviderMovieIds[s.CheapestProvider]
                        : s.ProviderMovieIds.Values.FirstOrDefault(),
                    Title = s.Title,
                    Year = s.Year,
                    Type = s.Type,
                    Provider = s.CheapestProvider,
                    Poster = s.Poster,
                    Price = s.CheapestPrice
                }).ToList();

                result.Success = true;
                result.Message = $"Successfully compared {result.Data.Count} movies across {availableProviders.Count} providers";

                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(3));

                _logger.LogInformation($"Successfully generated {result.Data.Count} movie comparisons");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get movie comparisons");
                result.Success = false;
                result.Message = "Failed to retrieve movie comparisons";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<ApiResponse<MovieComparison>> GetMovieComparisonAsync(string movieTitle, string year)
        {
            var cacheKey = $"movie_comparison_{movieTitle}_{year}";

            if (_cache.TryGetValue(cacheKey, out ApiResponse<MovieComparison>? cachedResult))
            {
                return cachedResult!;
            }

            var result = new ApiResponse<MovieComparison>();

            try
            {
                // Find movie IDs from both providers
                var moviesTasks = _providers.Select(provider => _providerService.GetMoviesAsync(provider));
                var moviesResults = await Task.WhenAll(moviesTasks);

                var comparison = new MovieComparison
                {
                    Title = movieTitle,
                    Year = year,
                    Providers = new List<ProviderPrice>(),
                    LastUpdated = DateTime.UtcNow
                };

                var foundMovieIds = new Dictionary<string, string>();

                // Find matching movies
                for (int i = 0; i < _providers.Length; i++)
                {
                    var provider = _providers[i];
                    var moviesResult = moviesResults[i];

                    if (moviesResult.Success && moviesResult.Data != null)
                    {
                        var matchingMovie = moviesResult.Data.Movies.FirstOrDefault(m =>
                            m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase) && m.Year == year);

                        if (matchingMovie != null)
                        {
                            foundMovieIds[provider] = matchingMovie.ID;
                            comparison.MovieId = matchingMovie.ID;
                        }
                    }
                }

                if (foundMovieIds.Count == 0)
                {
                    result.Success = false;
                    result.Message = $"Movie '{movieTitle} ({year})' not found in any provider";
                    return result;
                }

                // Fetch detailed prices in parallel
                await PopulateMoviePricesAsync(comparison, foundMovieIds.Keys.ToList(), foundMovieIds);

                result.Data = comparison;
                result.Success = true;
                result.Message = "Successfully retrieved movie comparison";

                // Cache for 5 minutes
                _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get comparison for movie {movieTitle} ({year})");
                result.Success = false;
                result.Message = "Failed to retrieve movie comparison";
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public async Task<ApiResponse<MovieDetail>> GetMovieDetailAsync(string provider, string id)
        {
            return  await _providerService.GetMovieDetailsAsync(provider, id);
        }

        public async Task<List<ProviderStatus>> GetProviderStatusAsync()
        {
            return await _providerService.GetProviderHealthAsync();
        }

        private async Task PopulateMoviePricesAsync(MovieComparison movie, List<string> availableProviders, Dictionary<string, string> movieIds)
        {
            var priceTasks = _providers.Select(async provider =>
            {
                var providerPrice = new ProviderPrice
                {
                    Provider = provider,
                    LastChecked = DateTime.UtcNow
                };

                try
                {
                    if (!availableProviders.Contains(provider))
                    {
                        providerPrice.Available = false;
                        providerPrice.Error = "Provider unavailable";
                        return providerPrice;
                    }

                    if (!movieIds.TryGetValue(provider, out var movieId))
                    {
                        providerPrice.Available = false;
                        providerPrice.Error = "Movie not available on this provider";
                        return providerPrice;
                    }

                    var detailsResult = await _providerService.GetMovieDetailsAsync(provider, movieId);

                    if (detailsResult.Success && detailsResult.Data != null)
                    {
                        providerPrice.Available = true;
                        providerPrice.Price = Convert.ToDecimal(detailsResult.Data.Price);
                        providerPrice.Type = detailsResult.Data.Type;
                        providerPrice.Poster = detailsResult.Data.Poster;
                        providerPrice.MovieId = detailsResult.Data.ID;
                        providerPrice.Provider = provider;
                    }
                    else
                    {
                        providerPrice.Available = false;
                        providerPrice.Error = detailsResult.Message;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error getting price from {provider} for movie {movie.Title}: {ex.Message}");
                    providerPrice.Available = false;
                    providerPrice.Error = ex.Message;
                }

                return providerPrice;
            });

            movie.Providers = (await Task.WhenAll(priceTasks)).ToList();

            // Determine cheapest price
            var availablePrices = movie.Providers.Where(p => p.Available && p.Price.HasValue).ToList();
            if (availablePrices.Any())
            {
                var cheapest = availablePrices.MinBy(p => p.Price!.Value);
                movie.CheapestPrice = cheapest!.Price;
                movie.CheapestProvider = cheapest.Provider;
                movie.Type = cheapest.Type;
                movie.Poster = cheapest.Poster;
                movie.Provider = cheapest.Provider;
                foreach (var provider in movie.Providers.Where(p => p.Price == cheapest.Price))
                {
                    provider.IsCheapest = true;
                }
            }
        }
    }
}
