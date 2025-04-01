using SalesDashboard.Web.Models;

namespace SalesDashboard.Web.Services
{
    /// <summary>
    /// Service interface for CSV operations
    /// </summary>
    public interface ICsvService
    {
        /// <summary>
        /// Loads sales records from a CSV file
        /// </summary>
        /// <param name="filePath">Path to the CSV file</param>
        /// <returns>Collection of sales records</returns>
        Task<IEnumerable<SalesRecord>> LoadSalesDataAsync(string filePath);
    }
}