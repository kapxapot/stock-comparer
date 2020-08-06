using StockComparer.Extensions;
using Xunit;

namespace StockComparer.Tests
{
    public class MathTests
    {
        [Theory]
        [InlineData(100, 200, 1)]
        [InlineData(100, 120, 0.2)]
        [InlineData(100, 90, -0.1)]
        [InlineData(100, -100, -2)]
        [InlineData(0, 1, 0)]
        [InlineData(-100, 100, 2)]
        [InlineData(-100, -200, -1)]
        [InlineData(-100, -120, -0.2)]
        [InlineData(100, -200, -3)]
        [InlineData(100, 100, 0)]
        public void DeltaTest(decimal baseValue, decimal changedValue, decimal expected)
        {
            Assert.Equal(expected, baseValue.Delta(changedValue));
        }

        [Theory]
        [InlineData(1, 100)]
        [InlineData(0.1, 10)]
        [InlineData(0, 0)]
        [InlineData(-0.2, -20)]
        [InlineData(100.56, 10056)]
        public void PercentTest(decimal value, decimal expected)
        {
            Assert.Equal(expected, value.ToPercent());
        }
    }
}
