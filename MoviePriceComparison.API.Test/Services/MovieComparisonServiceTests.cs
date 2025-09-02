using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MoviePriceComparison.API.Models;
using MoviePriceComparison.API.Services;
using MoviePriceComparison.API.Test.Helpers;

namespace MoviePriceComparison.API.Test.Services
{
    public class MovieComparisonServiceTests
    {
        private Mock<IMovieProviderService> _providerServiceMock;
        private Mock<ILogger<MovieComparisonService>> _loggerMock;
        private IMemoryCache _memoryCache;
        private MovieComparisonService _service;

        [SetUp]
        public void Setup()
        {
            _providerServiceMock = new Mock<IMovieProviderService>();
            _loggerMock = new Mock<ILogger<MovieComparisonService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _service = new MovieComparisonService(_providerServiceMock.Object, _memoryCache, _loggerMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache?.Dispose();
        }

        [Test]
        public async Task GetAllMovieComparisonsAsync_WithValidData_ReturnsComparisons()
        {
            
            var cinemaMovies = TestData.GetSampleMovies("cinemaworld");
            var filmMovies = TestData.GetSampleMovies("filmworld");

            _providerServiceMock.Setup(x => x.GetMoviesAsync("cinemaworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = cinemaMovies });

            _providerServiceMock.Setup(x => x.GetMoviesAsync("filmworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = filmMovies });

            _providerServiceMock.Setup(x => x.GetMovieDetailsAsync("cinemaworld", It.IsAny<string>()))
                .ReturnsAsync((string provider, string movieId) => new ApiResponse<MovieDetail>
                {
                    Success = true,
                    Data = TestData.GetSampleMovieDetails("cinemaworld", movieId)
                });

            _providerServiceMock.Setup(x => x.GetMovieDetailsAsync("filmworld", It.IsAny<string>()))
                .ReturnsAsync((string provider, string movieId) => new ApiResponse<MovieDetail>
                {
                    Success = true,
                    Data = TestData.GetSampleMovieDetails("filmworld", movieId)
                });

            
            var result = await _service.GetAllMovieComparisonsAsync();

            Assert.IsTrue(result.Success);
            Assert.IsTrue((result.Data as List<Movie>).Count > 0);
         
            var starWarsComparison = result.Data.Where(m => m.Title == "Star Wars: Episode III - Revenge of the Sith");

            Assert.IsNotNull(starWarsComparison);
            Assert.IsTrue(starWarsComparison.Count() == 1);          
        }

        [Test]
        public async Task GetAllMovieComparisonsAsync_WithOneProviderDown_StillReturnsData()
        {
           
            var cinemaMovies = TestData.GetSampleMovies("cinemaworld");

            _providerServiceMock.Setup(x => x.GetMoviesAsync("cinemaworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = cinemaMovies });

            _providerServiceMock.Setup(x => x.GetMoviesAsync("filmworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = false, Message = "Service unavailable" });

            _providerServiceMock.Setup(x => x.GetMovieDetailsAsync("cinemaworld", It.IsAny<string>()))
                .ReturnsAsync((string provider, string movieId) => new ApiResponse<MovieDetail>
                {
                    Success = true,
                    Data = TestData.GetSampleMovieDetails("cinemaworld", movieId)
                });

            _providerServiceMock.Setup(x => x.GetMovieDetailsAsync("filmworld", It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<MovieDetail> { Success = false, Message = "Provider unavailable" });

          
            var result = await _service.GetAllMovieComparisonsAsync();

            Assert.IsTrue(result.Success);
            Assert.IsTrue((result.Data as List<Movie>).Count > 0);

            var starWarsComparison = result.Data.Where(m => m.Title == "Star Wars: Episode III - Revenge of the Sith");

            Assert.IsNotNull(starWarsComparison);
            Assert.IsTrue(starWarsComparison.Count() == 1);

        }

        [Test]
        public async Task GetAllMovieComparisonsAsync_WithAllProvidersDown_ReturnsFailure()
        {
            _providerServiceMock.Setup(x => x.GetMoviesAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = false, Message = "Service unavailable" });

            var result = await _service.GetAllMovieComparisonsAsync();

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.Success);
        }

        [Test]
        public async Task GetMovieComparisonAsync_WithValidMovie_ReturnsComparison()
        {
            var movieTitle = "Star Wars: Episode III - Revenge of the Sith";
            var movieYear = "2005";
            var cinemaMovies = TestData.GetSampleMovies("cinemaworld");
            var filmMovies = TestData.GetSampleMovies("filmworld");

            _providerServiceMock.Setup(x => x.GetMoviesAsync("cinemaworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = cinemaMovies });

            _providerServiceMock.Setup(x => x.GetMoviesAsync("filmworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = filmMovies });

            _providerServiceMock.Setup(x => x.GetMovieDetailsAsync("cinemaworld", "cw0121766"))
                .ReturnsAsync(new ApiResponse<MovieDetail>
                {
                    Success = true,
                    Data = TestData.GetSampleMovieDetails("cinemaworld", "cw0121766")
                });

            _providerServiceMock.Setup(x => x.GetMovieDetailsAsync("filmworld", "fw0121766"))
                .ReturnsAsync(new ApiResponse<MovieDetail>
                {
                    Success = true,
                    Data = TestData.GetSampleMovieDetails("filmworld", "fw0121766")
                });

            var result = await _service.GetMovieComparisonAsync(movieTitle, movieYear);

            Assert.NotNull(result);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Data.Provider, "cinemaworld");
           
        }

        [Test]
        public async Task GetMovieComparisonAsync_WithNonExistentMovie_ReturnsNotFound()
        {
            var movieTitle = "Non Existent Movie";
            var movieYear = "2024";
            var cinemaMovies = TestData.GetSampleMovies("cinemaworld");
            var filmMovies = TestData.GetSampleMovies("filmworld");

            _providerServiceMock.Setup(x => x.GetMoviesAsync("cinemaworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = cinemaMovies });

            _providerServiceMock.Setup(x => x.GetMoviesAsync("filmworld"))
                .ReturnsAsync(new ApiResponse<MovieSummary> { Success = true, Data = filmMovies });

            var result = await _service.GetMovieComparisonAsync(movieTitle, movieYear);

            Assert.NotNull(result);
            Assert.IsTrue(!result.Success);
        }

        [Test]
        public async Task GetProviderStatusAsync_ReturnsProviderHealth()
        {
            // Arrange
            var expectedStatus = TestData.GetSampleProviderStatus();
            _providerServiceMock.Setup(x => x.GetProviderHealthAsync())
                .ReturnsAsync(expectedStatus);

            // Act
            var result = await _service.GetProviderStatusAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.Count, 2);
            Assert.IsTrue(result[0].IsHealthy);
            Assert.IsTrue(result[1].IsHealthy);
            Assert.AreEqual(result[0].Status, "healthy");
            Assert.AreEqual(result[1].Status, "healthy");
        }

    }
}
