using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MoviePriceComparison.API.Models;
using MoviePriceComparison.API.Services;
using System.Collections;

namespace MoviePriceComparison.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieComparisonService _comparisonService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(IMovieComparisonService comparisonService, ILogger<MoviesController> logger)
        {
            _comparisonService = comparisonService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<MovieComparison>>>> GetMovies()
        {
            try
            {
                _logger.LogInformation("Getting all movie comparisons");
                var result = await _comparisonService.GetAllMovieComparisonsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie comparisons");
                return StatusCode(500, new ApiResponse<List<MovieComparison>>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpGet("{title}/{year}")]
        public async Task<ActionResult<ApiResponse<MovieComparison>>> GetMovieComparison(string title, string year)
        {
            try
            {
                _logger.LogInformation($"Getting comparison for movie {title} ({year})");
                var result = await _comparisonService.GetMovieComparisonAsync(title, year);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting comparison for movie {title} ({year})");
                return StatusCode(500, new ApiResponse<MovieComparison>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpGet("detail/{provider}/{id}")]
        public async Task<ActionResult<ApiResponse<MovieComparison>>> GetMovieDetail(string provider,string id)
        {
            try
            {
                _logger.LogInformation($"Getting detaiul for movie {provider} {id})");
                var result = await _comparisonService.GetMovieDetailAsync(provider,id);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting comparison for movie {provider} ({id})");
                return StatusCode(500, new ApiResponse<MovieComparison>
                {
                    Success = false,
                    Message = "Internal server error",
                    Errors = { ex.Message }
                });
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<List<ProviderStatus>>> GetProviderStatus()
        {
            try
            {
                var status = await _comparisonService.GetProviderStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provider status");
                return StatusCode(500, new { message = "Error checking provider status" });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<List<MovieComparison>>>> RefreshMovies()
        {
            try
            {
                _logger.LogInformation("Refreshing movie data (clearing cache)");

                // Clear relevant cache entries
                var cache = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                if (cache is MemoryCache memCache)
                {
                    // Note: In production, implement proper cache invalidation
                    var field = typeof(MemoryCache).GetField("_coherentState",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null)
                    {
                        var coherentState = field.GetValue(memCache);
                        var entriesCollection = coherentState?.GetType()
                            .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (entriesCollection?.GetValue(coherentState) is IDictionary entries)
                        {
                            entries.Clear();
                        }
                    }
                }

                var result = await _comparisonService.GetAllMovieComparisonsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing movie data");
                return StatusCode(500, new ApiResponse<List<MovieComparison>>
                {
                    Success = false,
                    Message = "Error refreshing movie data",
                    Errors = { ex.Message }
                });
            }
        }
    }
}
