using SalesDashboard.Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace SalesDashboard.Web.Services
{

    /// <summary>
    /// Implementation of sales service
    /// </summary>
    public class SalesService : ISalesService
    {
        private readonly ICsvService _csvService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SalesService> _logger;
        private readonly string _dataFilePath;
        private const string CacheKey = "SalesData";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

        public SalesService(
            ICsvService csvService,
            IMemoryCache cache,
            ILogger<SalesService> logger)
        {
            _csvService = csvService;
            _cache = cache;
            _logger = logger;
            _dataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "Data.csv");
        }

        /// <summary>
        /// Gets all sales records, with caching
        /// </summary>
        public async Task<IEnumerable<SalesRecord>> GetAllSalesRecordsAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out IEnumerable<SalesRecord>? salesRecords))
            {
                _logger.LogInformation("Sales data not found in cache, loading from file");
                salesRecords = await _csvService.LoadSalesDataAsync(_dataFilePath);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(CacheDuration)
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _cache.Set(CacheKey, salesRecords, cacheEntryOptions);
            }

            return salesRecords;
        }

        /// <summary>
        /// Gets sales summary statistics
        /// </summary>
        public async Task<SalesSummary> GetSalesSummaryAsync()
        {
            var salesRecords = await GetAllSalesRecordsAsync();

            return new SalesSummary
            {
                TotalRevenue = salesRecords.Sum(r => r.Revenue),
                TotalProfit = salesRecords.Sum(r => r.Profit),
                TotalUnitsSold = salesRecords.Sum(r => r.UnitsSold),
                AverageOrderValue = salesRecords.Average(r => r.Revenue),
                OrderCount = salesRecords.Count(),
                ProfitMargin = salesRecords.Sum(r => r.Revenue) > 0
                    ? salesRecords.Sum(r => r.Profit) / salesRecords.Sum(r => r.Revenue)
                    : 0
            };
        }

        /// <summary>
        /// Gets sales aggregated by segment
        /// </summary>
        public async Task<Dictionary<string, decimal>> GetSalesBySegmentAsync()
        {
            var salesRecords = await GetAllSalesRecordsAsync();

            return salesRecords
                .GroupBy(r => r.Segment)
                .ToDictionary(
                    g => g.Key ?? "Unknown",
                    g => g.Sum(r => r.Revenue)
                );
        }

        /// <summary>
        /// Gets sales aggregated by country
        /// </summary>
        public async Task<Dictionary<string, decimal>> GetSalesByCountryAsync()
        {
            var salesRecords = await GetAllSalesRecordsAsync();

            return salesRecords
                .GroupBy(r => r.Country)
                .OrderByDescending(g => g.Sum(r => r.Revenue))
                .Take(10) // Top 10 countries
                .ToDictionary(
                    g => g.Key ?? "Unknown",
                    g => g.Sum(r => r.Revenue)
                );
        }

        /// <summary>
        /// Gets sales aggregated by product
        /// </summary>
        public async Task<Dictionary<string, decimal>> GetSalesByProductAsync()
        {
            var salesRecords = await GetAllSalesRecordsAsync();

            return salesRecords
                .GroupBy(r => r.Product)
                .OrderByDescending(g => g.Sum(r => r.Revenue))
                .Take(10) // Top 10 products
                .ToDictionary(
                    g => g.Key ?? "Unknown",
                    g => g.Sum(r => r.Revenue)
                );
                       
        }

        /// <summary>
        /// Gets sales aggregated by month
        /// </summary>
        public async Task<Dictionary<string, decimal>> GetSalesByMonthAsync()
        {
            var salesRecords = await GetAllSalesRecordsAsync();

            return salesRecords
                .GroupBy(r => new { r.Year, r.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => $"{g.Key.Year}-{g.Key.Month:D2}",
                    g => g.Sum(r => r.Revenue)
                );
        }

        /// <summary>
        /// Gets sales aggregated by discount band
        /// </summary>
        public async Task<Dictionary<string, decimal>> GetSalesByDiscountBandAsync()
        {
            var salesRecords = await GetAllSalesRecordsAsync();

            return salesRecords
                .GroupBy(r => string.IsNullOrWhiteSpace(r.DiscountBand) ? "None" : r.DiscountBand)
                .ToDictionary(
                    g => g.Key ?? "Unknown",
                    g => g.Sum(r => r.Revenue)
                );
        }
    }
}