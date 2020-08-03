using System.Linq;
using StockComparer.Services;
using Xunit;

namespace StockComparer.Tests
{
    public class ExternalStockServiceTest
    {
        [Theory]
        [InlineData("GOOGL")]
        [InlineData("IBM")]
        public async void SuccessfulLoadDailyTest(string symbol)
        {
            var apiKey = "NGCJ7PPUPB8W7FII";

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
