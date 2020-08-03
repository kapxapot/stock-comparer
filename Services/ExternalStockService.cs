using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using StockComparer.Models;
using StockComparer.Services.Interfaces;

namespace StockComparer.Services
{
    public class ExternalStockService : IExternalStockService
    {
        private string _apiKey;

        public ExternalStockService(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<IReadOnlyCollection<DailyStockData>> GetDailyStockData(string symbol)
        {
            var client = new HttpClient();

            var url = BuildUrl("TIME_SERIES_DAILY", symbol);
            var stringTask = client.GetStringAsync(url);
            var response = await stringTask;

            using (var document = JsonDocument.Parse(response))
            {
                var root = document.RootElement;

                if (!root.TryGetProperty("Time Series (Daily)", out var seriesElement))
                {
                    throw new Exception(ParseError(root));
                }

                var list = new List<DailyStockData>();

                foreach (var series in seriesElement.EnumerateObject())
                {
                    if (!DateTime.TryParseExact(
                        series.Name,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var date))
                    {
                        throw new Exception();
                    }

                    var values = series.Value;

                    values.TryGetProperty("1. open", out var open);
                    values.TryGetProperty("2. high", out var high);
                    values.TryGetProperty("3. low", out var low);
                    values.TryGetProperty("4. close", out var close);
                    values.TryGetProperty("5. volume", out var volume);


                    list.Add(
                        new DailyStockData
                        {
                            Symbol = symbol,
                            Open = open.GetDecimal(),
                            High = high.GetDecimal(),
                            Low = low.GetDecimal(),
                            Close = close.GetDecimal(),
                            Volume = volume.GetInt32()
                        }
                    );
                }

                return list.AsReadOnly();
            }
        }

        private string BuildUrl(string func, string symbol)
        {
            return $"https://www.alphavantage.co/query?function={func}&symbol={symbol}&apikey={_apiKey}";
        }

        private string ParseError(JsonElement root)
        {
            if (root.TryGetProperty("Error Message", out var error))
            {
                return error.GetString();
            }

            return "Critical API error.";
        }
    }
}
