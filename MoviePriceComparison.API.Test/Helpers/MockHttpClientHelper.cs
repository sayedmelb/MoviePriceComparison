using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace MoviePriceComparison.API.Test.Helpers
{
    public static class MockHttpClientHelper
    {
        public static HttpClient CreateMockHttpClient(
            Dictionary<string, object> responses,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            TimeSpan? delay = null)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            foreach (var kvp in responses)
            {
                var uri = kvp.Key;
                var responseObject = kvp.Value;

                var responseContent = responseObject is string str ? str : JsonSerializer.Serialize(responseObject);

                handlerMock.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(uri)),
                        ItExpr.IsAny<CancellationToken>())
                    .Returns(async () =>
                    {
                        if (delay.HasValue)
                            await Task.Delay(delay.Value);

                        return new HttpResponseMessage
                        {
                            StatusCode = statusCode,
                            Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                        };
                    });
            }

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://test.com")
            };
        }

        public static HttpClient CreateFailingHttpClient(HttpStatusCode statusCode = HttpStatusCode.ServiceUnavailable)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent("Service unavailable", Encoding.UTF8, "text/plain")
                });

            return new HttpClient(handlerMock.Object);
        }

        public static HttpClient CreateTimeoutHttpClient()
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(30)); // Simulate timeout
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            return new HttpClient(handlerMock.Object) { Timeout = TimeSpan.FromMilliseconds(100) };
        }
    }
}
