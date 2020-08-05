using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StockComparer.Config;
using StockComparer.Extensions;
using StockComparer.Models;
using StockComparer.Services.Interfaces;

namespace StockComparer.Services
{
    public class ExternalStockService : IExternalStockService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiConfig _apiConfig;
        private readonly ILogger<ExternalStockService> _logger;

        public ExternalStockService(HttpClient httpClient, ApiConfig apiConfig, ILogger<ExternalStockService> logger)
        {
            _httpClient = httpClient;
            _apiConfig = apiConfig;
            _logger = logger;
        }

        public async Task<IEnumerable<DailyStockData>> GetDailyStockData(string symbol)
        {
            return await Retry(
                async () =>
                {
                    var url = BuildUrl("TIME_SERIES_DAILY", symbol);
                    var stringTask = _httpClient.GetStringAsync(url);
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
                },
                _apiConfig.MaxRetries,
                _apiConfig.InitialDelay
            );
        }

        private async Task<T> Retry<T>(Func<Task<T>> func, int retries = 6, int initialDelay = 2000)
        {
            var delay = initialDelay;
            var retriesLeft = retries;
            var attempt = 1;

            while (retriesLeft > 0)
            {
                _logger.LogInformation($"Executing func, attempt: {attempt++}");

                try
                {
                    return await func();
                }
                catch
                {
                    retriesLeft--;

                    if (retriesLeft == 0)
                    {
                        throw;
                    }

                    Thread.Sleep(delay);

                    delay *= 2;
                }
            }

            throw new Exception("No retries left");
        }

        private string BuildUrl(string func, string symbol)
        {
            return $"https://www.alphavantage.co/query?function={func}&symbol={symbol}&apikey={_apiConfig.AlphaVantageApiKey}";
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
