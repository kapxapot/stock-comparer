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
        [InlineData("GOOGL")]
        [InlineData("IBM")]
        [InlineData("SPY")]
        public async void SuccessfulLoadDailyTest(string symbol)
        {
            var config = GetConfigurationRoot();
            var apiKey = config.GetValue<string>("AlphaVantageApiKey");

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
    }
}
