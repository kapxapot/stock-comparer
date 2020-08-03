using System.Text.Json;
using StockComparer.Extensions;
using Xunit;

namespace StockComparer.Tests
{
    public class JsonTest
    {
        [Fact]
        public void TryGetPropertyTest()
        {
            var json = "{ \"one\": 1 }";

            using (var document = JsonDocument.Parse(json))
            {
                var root = document.RootElement;

                Assert.True(root.IsObject());

                root.TryGetProperty("one", out var one);

                Assert.Equal(JsonValueKind.Number, one.ValueKind);
                Assert.Equal(1, one.GetInt32());

                root.TryGetProperty("two", out var two);

                Assert.True(two.IsUndefined());
            }
        }
    }
}
