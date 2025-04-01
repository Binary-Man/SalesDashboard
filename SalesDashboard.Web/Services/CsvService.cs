using CsvHelper;
using CsvHelper.Configuration;
using SalesDashboard.Web.Models;
using System.Globalization;
using System.Text;

namespace SalesDashboard.Web.Services
{

    /// <summary>
    /// Implementation of CSV service using CsvHelper
    /// </summary>
    public class CsvService : ICsvService
    {
        private readonly ILogger<CsvService> _logger;

        public CsvService(ILogger<CsvService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Loads and parses sales data from CSV file
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <returns>Collection of SalesRecord objects</returns>
        public async Task<IEnumerable<SalesRecord>> LoadSalesDataAsync(string filePath)
        {
            try
            {
                _logger.LogInformation("Loading sales data from {FilePath}", filePath);

                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var reader = new StreamReader(stream, Encoding.GetEncoding(1252)); // Use CP1252 encoding
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null
                });

                // Define custom mappings
                csv.Context.RegisterClassMap<SalesRecordMap>();

                var records = await Task.Run(() => csv.GetRecords<SalesRecord>().ToList());
                _logger.LogInformation("Successfully loaded {Count} sales records", records.Count);

                return records;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sales data from {FilePath}", filePath);
                throw;
            }
        }
    }
}