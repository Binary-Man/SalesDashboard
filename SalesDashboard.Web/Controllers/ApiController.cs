using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Web.Services;
using System.Text;

namespace SalesDashboard.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<ApiController> _logger;

        public ApiController(ISalesService salesService, ILogger<ApiController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// Get all sales data
        /// </summary>
        [HttpGet("sales")]
        public async Task<IActionResult> GetSales()
        {
            try
            {
                var sales = await _salesService.GetAllSalesRecordsAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales data");
                return StatusCode(500, "An error occurred while retrieving sales data");
            }
        }

        /// <summary>
        /// Get filtered sales data
        /// </summary>
        [HttpGet("sales/filter")]
        public async Task<IActionResult> GetFilteredSales(
            [FromQuery] string segment = "",
            [FromQuery] string country = "",
            [FromQuery] string product = "",
            [FromQuery] string discountBand = "",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var sales = await _salesService.GetAllSalesRecordsAsync();
                
                // Apply filters
                if (!string.IsNullOrEmpty(segment))
                {
                    sales = sales.Where(s => s.Segment == segment);
                }
                
                if (!string.IsNullOrEmpty(country))
                {
                    sales = sales.Where(s => s.Country == country);
                }
                
                if (!string.IsNullOrEmpty(product))
                {
                    sales = sales.Where(s => s.Product == product);
                }
                
                if (!string.IsNullOrEmpty(discountBand))
                {
                    sales = sales.Where(s => s.DiscountBand == discountBand);
                }
                
                if (startDate.HasValue)
                {
                    sales = sales.Where(s => s.Date >= startDate.Value);
                }
                
                if (endDate.HasValue)
                {
                    sales = sales.Where(s => s.Date <= endDate.Value);
                }
                
                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered sales data");
                return StatusCode(500, "An error occurred while retrieving filtered sales data");
            }
        }

        /// <summary>
        /// Get available filter options
        /// </summary>
        [HttpGet("filter-options")]
        public async Task<IActionResult> GetFilterOptions()
        {
            try
            {
                var sales = await _salesService.GetAllSalesRecordsAsync();
                
                var filterOptions = new 
                {
                    Segments = sales.Select(s => s.Segment).Distinct().OrderBy(s => s).ToList(),
                    Countries = sales.Select(s => s.Country).Distinct().OrderBy(c => c).ToList(),
                    Products = sales.Select(s => s.Product).Distinct().OrderBy(p => p).ToList(),
                    DiscountBands = sales.Select(s => s.DiscountBand).Distinct()
                        .Where(d => !string.IsNullOrEmpty(d))
                        .OrderBy(d => d)
                        .ToList()
                };
                
                return Ok(filterOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filter options");
                return StatusCode(500, "An error occurred while retrieving filter options");
            }
        }

        /// <summary>
        /// Export sales data as CSV
        /// </summary>
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                var sales = await _salesService.GetAllSalesRecordsAsync();
                var builder = new StringBuilder();
                
                // Add header
                builder.AppendLine("Date,Segment,Country,Product,DiscountBand,UnitsSold,ManufacturingPrice,SalePrice,Revenue,Profit");
                
                // Add data rows
                foreach (var sale in sales)
                {
                    builder.AppendLine(string.Join(",",
                        sale.Date.ToString("yyyy-MM-dd"),
                        EscapeCsvField(sale.Segment),
                        EscapeCsvField(sale.Country),
                        EscapeCsvField(sale.Product),
                        EscapeCsvField(sale.DiscountBand),
                        sale.UnitsSold,
                        sale.ManufacturingPrice,
                        sale.SalePrice,
                        sale.Revenue,
                        sale.Profit
                    ));
                }
                
                var bytes = Encoding.UTF8.GetBytes(builder.ToString());
                return File(bytes, "text/csv", $"sales_export_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting sales data");
                return StatusCode(500, "An error occurred while exporting sales data");
            }
        }

        /// <summary>
        /// Get sales summary statistics
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var summary = await _salesService.GetSalesSummaryAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales summary");
                return StatusCode(500, "An error occurred while retrieving sales summary");
            }
        }

        /// <summary>
        /// Escape CSV field if it contains commas, quotes, or newlines
        /// </summary>
        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return string.Empty;
            }
            
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                // Escape quotes by doubling them and wrap in quotes
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            
            return field;
        }
    }
}