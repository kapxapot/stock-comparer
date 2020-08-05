using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using StockComparer.Services;
using Xunit;

namespace StockComparer.Tests
{
    public class ExternalStockServiceTest
    {
        public IConfigurationRoot GetConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
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
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var config = GetConfigurationRoot();
                apiKey = config.GetValue<string>("AlphaVantageApiKey");
            }

            var service = new ExternalStockService(apiKey);

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
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                var config = GetConfigurationRoot();
                apiKey = config.GetValue<string>("AlphaVantageApiKey");
            }

            var service = new ExternalStockService(apiKey);

            await Assert.ThrowsAsync<Exception>(
                async () => await service.GetDailyStockData(symbol)
            );
        }
    }
}
