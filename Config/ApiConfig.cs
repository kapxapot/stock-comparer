namespace StockComparer.Config
{
    public class ApiConfig
    {
        public string AlphaVantageApiKey { get; set; }
        public int MaxRetries { get; set; }
        public int InitialDelay { get; set; }
    }
}
