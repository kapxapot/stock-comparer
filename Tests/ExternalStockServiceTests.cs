using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockComparer.Config;
using StockComparer.Services;
using Xunit;

namespace StockComparer.Tests
{
    public class ExternalStockServiceTests : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ApiConfig _apiConfig;
        private readonly ILogger<ExternalStockService> _logger;

        public ExternalStockServiceTests()
        {
            _httpClient = new HttpClient();

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            _apiConfig = configRoot.Get<ApiConfig>();

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<ILoggerFactory>();

            _logger = factory.CreateLogger<ExternalStockService>();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        [Theory]
        [InlineData("demo", "IBM")]
        [InlineData("i am not an api key", "SPY")]
        [InlineData("", "IBM")]
        [InlineData("", "GOOGL")]
        [InlineData("", "SPY")]
        /// <summary>
        /// In case of empty apiKey parameter the API key is retrieved from the config.
        /// </summary>
        public async void SuccessfulLoadDailyTest(string apiKey, string symbol)
        {
            var apiConfig = new ApiConfig
            {
                AlphaVantageApiKey = string.IsNullOrWhiteSpace(apiKey)
                    ? _apiConfig.AlphaVantageApiKey
                    : apiKey,
                MaxRetries = _apiConfig.MaxRetries,
                InitialDelay = _apiConfig.InitialDelay
            };

            var service = new ExternalStockService(_httpClient, apiConfig, _logger);

            var result = await service.GetDailyStockData(symbol);

            Assert.NotEmpty(result);

            var first = result.First();

            Assert.Equal(symbol, first.Symbol);

            Assert.True(first.Open > 0);
            Assert.True(first.High > 0);
            Assert.True(first.Low > 0);
            Assert.True(first.Close > 0);
            Assert.True(first.Volume > 0);
        }

        [Theory]
        [InlineData("demo", "invalid symbol")]
        [InlineData("i am not an api key", "invalid symbol")]
        [InlineData("", "invalid symbol")]
        /// <summary>
        /// In case of empty apiKey parameter the API key is retrieved from the config.
        /// </summary>
        public async void FailedLoadDailyTest(string apiKey, string symbol)
        {
            var apiConfig = new ApiConfig
            {
                AlphaVantageApiKey = string.IsNullOrWhiteSpace(apiKey)
                    ? _apiConfig.AlphaVantageApiKey
                    : apiKey,
                MaxRetries = 1,
                InitialDelay = 0
            };

            var service = new ExternalStockService(_httpClient, apiConfig, _logger);

            await Assert.ThrowsAsync<Exception>(
                async () => await service.GetDailyStockData(symbol)
            );
        }
    }
}
