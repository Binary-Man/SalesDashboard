using SalesDashboard.Web.Models;

namespace SalesDashboard.Web.Services
{
    /// <summary>
    /// Service interface for sales data analysis
    /// </summary>
    public interface ISalesService
    {
        /// <summary>
        /// Gets all sales records
        /// </summary>
        Task<IEnumerable<SalesRecord>> GetAllSalesRecordsAsync();

        /// <summary>
        /// Gets sales summary statistics
        /// </summary>
        Task<SalesSummary> GetSalesSummaryAsync();

        /// <summary>
        /// Gets sales by segment
        /// </summary>
        Task<Dictionary<string, decimal>> GetSalesBySegmentAsync();

        /// <summary>
        /// Gets sales by country
        /// </summary>
        Task<Dictionary<string, decimal>> GetSalesByCountryAsync();

        /// <summary>
        /// Gets sales by product
        /// </summary>
        Task<Dictionary<string, decimal>> GetSalesByProductAsync();

        /// <summary>
        /// Gets sales by month
        /// </summary>
        Task<Dictionary<string, decimal>> GetSalesByMonthAsync();

        /// <summary>
        /// Gets sales by discount band
        /// </summary>
        Task<Dictionary<string, decimal>> GetSalesByDiscountBandAsync();
    }
}