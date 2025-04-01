namespace SalesDashboard.Web.Services
{
    /// <summary>
    /// Summary statistics for sales data
    /// </summary>
    public class SalesSummary
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalUnitsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int OrderCount { get; set; }
        public decimal ProfitMargin { get; set; }
    }
}