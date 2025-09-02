using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MoviePriceComparison.API.Services;
using MoviePriceComparison.API.Test.Helpers;
using System.Net;

namespace MoviePriceComparison.API.Test.Services
{
    public class MovieProviderServiceTests
    {
        private Mock<ILogger<MovieProviderService>> _loggerMock;
        private IMemoryCache _memoryCache;
        private MovieProviderService _service;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<MovieProviderService>>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        [TearDown]
        public void TearDown()
        {
            _memoryCache?.Dispose();
        }
        [Test]
        public async Task GetMoviesAsync_WithValidProvider_ReturnsSuccess()
        {
            var provider = "cinemaworld";
            var expectedMovies = TestData.GetSampleMovies(provider);

            var responses = new Dictionary<string, object>
            {
                [$"{provider}/movies"] = expectedMovies
            };

            var httpClient = MockHttpClientHelper.CreateMockHttpClient(responses);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var result = await _service.GetMoviesAsync(provider);

            Assert.NotNull(result.Data);
            Assert.IsTrue(result.Data.Movies.Count > 0);
        }

        [Test]
        public async Task GetMoviesAsync_WithNetworkFailure_ReturnsFailure()
        {
            var provider = "cinemaworld";
            var httpClient = MockHttpClientHelper.CreateFailingHttpClient(HttpStatusCode.ServiceUnavailable);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var result = await _service.GetMoviesAsync(provider);

            Assert.IsFalse(result.Success);
            Assert.Null(result.Data);
            Assert.IsTrue(result.Errors.Count > 0);
            Assert.AreEqual(result.Message, "Failed to retrieve movies from cinemaworld");
        }

        [Test]
        public async Task GetMoviesAsync_WithCachedData_ReturnsCachedResult()
        {
            var provider = "cinemaworld";
            var expectedMovies = TestData.GetSampleMovies(provider);

            var responses = new Dictionary<string, object>
            {
                [$"{provider}/movies"] = expectedMovies
            };

            var httpClient = MockHttpClientHelper.CreateMockHttpClient(responses);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var firstResult = await _service.GetMoviesAsync(provider);

            Assert.IsTrue(firstResult.Success);

           var secondResult = await _service.GetMoviesAsync(provider);

            Assert.IsTrue(secondResult.Success);
            Assert.AreSame(firstResult.Data, secondResult.Data);

            // Verify logger was called for cache hit
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("cached movies")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetMovieDetailsAsync_WithValidMovieId_ReturnsSuccess()
        {
            var provider = "cinemaworld";
            var movieId = "cw0121766";
            var expectedMovie = TestData.GetSampleMovieDetails(provider, movieId);

            var responses = new Dictionary<string, object>
            {
                [$"{provider}/movie/{movieId}"] = expectedMovie
            };

            var httpClient = MockHttpClientHelper.CreateMockHttpClient(responses);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var result = await _service.GetMovieDetailsAsync(provider, movieId);

            Assert.IsTrue(result.Success);
            Assert.NotNull(result.Data);
            Assert.AreEqual(result.Data.Title, "Star Wars: Episode III - Revenge of the Sith");
        }

        [Test]
        public async Task GetMovieDetailsAsync_WithInvalidMovieId_ReturnsFailure()
        {
            var provider = "cinemaworld";
            var movieId = "invalid_id";
            var httpClient = MockHttpClientHelper.CreateFailingHttpClient(HttpStatusCode.NotFound);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var result = await _service.GetMovieDetailsAsync(provider, movieId);

            Assert.IsTrue(!result.Success);
            Assert.Null(result.Data);
        }

        [Test]
        public async Task GetProviderHealthAsync_WithHealthyProviders_ReturnsHealthyStatus()
        {
            var responses = new Dictionary<string, object>
            {
                ["cinemaworld/movies"] = TestData.GetSampleMovies("cinemaworld"),
                ["filmworld/movies"] = TestData.GetSampleMovies("filmworld")
            };

            var httpClient = MockHttpClientHelper.CreateMockHttpClient(responses);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var result = await _service.GetProviderHealthAsync();

            Assert.NotNull(result);
            Assert.AreEqual(result.Count, 2);
            Assert.IsTrue(result[0].IsHealthy);
            Assert.IsTrue(result[1].IsHealthy);
            Assert.AreEqual(result[0].Status, "healthy");
            Assert.AreEqual(result[1].Status, "healthy");
        }

        [Test]
        public async Task GetProviderHealthAsync_WithUnhealthyProvider_ReturnsPartialHealth()
        {
            var httpClient = MockHttpClientHelper.CreateFailingHttpClient(HttpStatusCode.ServiceUnavailable);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

            var result = await _service.GetProviderHealthAsync();

            Assert.NotNull(result);
            Assert.AreEqual(result.Count, 2);
            Assert.IsTrue(!result[0].IsHealthy);
            Assert.IsTrue(!result[1].IsHealthy);
            Assert.AreEqual(result[0].Status, "error (ServiceUnavailable)");
            Assert.AreEqual(result[1].Status, "error (ServiceUnavailable)");
        }

        [Test]
        public async Task RetryLogic_WithTransientFailures_EventuallySucceeds()
        {
            var provider = "cinemaworld";
            var expectedMovies = TestData.GetSampleMovies(provider);
            var callCount = 0;

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(() =>
                {
                    callCount++;
                    if (callCount < 3) // Fail first 2 attempts
                    {
                        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
                    }

                    // Succeed on 3rd attempt
                    var content = System.Text.Json.JsonSerializer.Serialize(expectedMovies);
                    return Task.FromResult(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                    });
                });

            var httpClient = new HttpClient(handlerMock.Object);
            _service = new MovieProviderService(httpClient, _memoryCache, _loggerMock.Object);

           
            var result = await _service.GetMoviesAsync(provider);

            Assert.NotNull(result);
            Assert.IsTrue(result.Success);
        }       

    }
}
