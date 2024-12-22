using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RestSharp;
using StockpriceDashboard.Models.DTO;
using System.Globalization;
using System.Text.Json;

namespace StockpriceDashboard.Services
{
    public class StockService
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly IMemoryCache _cache;
        private readonly ILogger<StockService> _logger;

        public StockService(IConfiguration config, IMemoryCache cache, ILogger<StockService> logger)
        {
            _baseUrl = config["AlphaVantage:BaseUrl"];
            _apiKey = config["AlphaVantage:ApiKey"];
            _cache = cache;
            _logger = logger;
        }

        public async Task<StockResponseDto> GetStockDataAsync(string symbol)
        {
            string cacheKey = $"StockData_{symbol}";

            // Check if data is in cache
            if (_cache.TryGetValue(cacheKey, out StockResponseDto cachedData))
            {
                _logger.LogInformation($"Cache hit for symbol: {symbol}");
                return cachedData;
            }

            _logger.LogInformation($"Cache miss for symbol: {symbol}. Fetching data from API.");
            var client = new RestClient(_baseUrl);
            var request = new RestRequest()
                .AddParameter("function", "TIME_SERIES_DAILY")
                .AddParameter("symbol", symbol)
                .AddParameter("outputsize", "compact")
                .AddParameter("datatype", "json")
                .AddHeader("X-RapidAPI-Key", _apiKey);

            // Make API request
            var response = await client.GetAsync(request);

            if (response == null || !response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            {
                _logger.LogError($"Failed to fetch data for {symbol}: {response?.StatusDescription}");
                throw new Exception("Error retrieving data from Alpha Vantage API.");
            }

            // Parse and map the response to DTO
            try
            {
                var jsonDocument = JsonDocument.Parse(response.Content);
                var stockResponse = BuildStockResponse(jsonDocument, symbol);

                // Cache the response
                _cache.Set(cacheKey, stockResponse, TimeSpan.FromMinutes(5));
                return stockResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error parsing stock data for {symbol}");
                throw;
            }
        }

        private StockResponseDto BuildStockResponse(JsonDocument jsonDocument, string symbol)
        {
            var stockResponse = new StockResponseDto
            {
                MetaData = new MetaDataDto
                {
                    Information = jsonDocument.RootElement
                        .GetProperty("Meta Data")
                        .GetProperty("1. Information")
                        .GetString(),
                    Symbol = jsonDocument.RootElement
                        .GetProperty("Meta Data")
                        .GetProperty("2. Symbol")
                        .GetString(),
                    LastRefreshed = jsonDocument.RootElement
                        .GetProperty("Meta Data")
                        .GetProperty("3. Last Refreshed")
                        .GetString(),
                    OutputSize = jsonDocument.RootElement
                        .GetProperty("Meta Data")
                        .GetProperty("4. Output Size")
                        .GetString(),
                    TimeZone = jsonDocument.RootElement
                        .GetProperty("Meta Data")
                        .GetProperty("5. Time Zone")
                        .GetString()
                },
                TimeSeriesDaily = new Dictionary<string, DailyDataDto>()
            };

            // Parse time series data
            if (jsonDocument.RootElement.TryGetProperty("Time Series (Daily)", out var timeSeries))
            {
                foreach (var property in timeSeries.EnumerateObject())
                {
                    var date = property.Name;
                    var details = property.Value;

                    stockResponse.TimeSeriesDaily[date] = new DailyDataDto
                    {
                        Open = details.GetProperty("1. open").GetString(),
                        High = details.GetProperty("2. high").GetString(),
                        Low = details.GetProperty("3. low").GetString(),
                        Close = details.GetProperty("4. close").GetString(),
                        Volume = details.GetProperty("5. volume").GetString()
                    };
                }
            }
            else
            {
                _logger.LogError("Response JSON does not contain 'Time Series (Daily)'");
                throw new Exception("Invalid JSON structure from API.");
            }

            return stockResponse;
        }
    }
}
