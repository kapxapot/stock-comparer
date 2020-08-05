using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using StockComparer.Extensions;
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

        public async Task<IEnumerable<DailyStockData>> GetDailyStockData(string symbol)
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
                        throw new Exception($"Incorrect date: {series.Name}");
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
                            Date = date,
                            Open = open.ToDecimal(),
                            High = high.ToDecimal(),
                            Low = low.ToDecimal(),
                            Close = close.ToDecimal(),
                            Volume = volume.ToInt()
                        }
                    );
                }

                return list;
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

            if (root.TryGetProperty("Note", out var note))
            {
                return note.GetString();
            }

            return $"Critical API error: {root.ToString()}";
        }
    }
}
