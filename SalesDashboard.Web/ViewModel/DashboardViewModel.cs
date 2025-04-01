using System.Collections.Generic;
using SalesDashboard.Web.Models;
using SalesDashboard.Web.Services;

namespace SalesDashboard.Web.ViewModel
{
    /// <summary>
    /// View model for the dashboard page
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// Summary of sales statistics
        /// </summary>
        public SalesSummary Summary { get; set; } = new();

        /// <summary>
        /// Sales data by segment
        /// </summary>
        public Dictionary<string, decimal> SalesBySegment { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Sales data by country
        /// </summary>
        public Dictionary<string, decimal> SalesByCountry { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Sales data by product
        /// </summary>
        public Dictionary<string, decimal> SalesByProduct { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Sales data by month
        /// </summary>
        public Dictionary<string, decimal> SalesByMonth { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Sales data by discount band
        /// </summary>
        public Dictionary<string, decimal> SalesByDiscountBand { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Recent sales records for the data table
        /// </summary>
        public IEnumerable<SalesRecord> RecentSales { get; set; } = new List<SalesRecord>();
    }
}