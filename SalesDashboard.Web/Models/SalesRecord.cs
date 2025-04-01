using System;
using System.ComponentModel.DataAnnotations;

namespace SalesDashboard.Web.Models
{
    /// <summary>
    /// Represents a single sales record from the CSV data
    /// </summary>
    public class SalesRecord
    {
        /// <summary>
        /// Business segment (Government, Midmarket, etc.)
        /// </summary>
        public string Segment { get; set; } = "";

        /// <summary>
        /// Country where the sale occurred
        /// </summary>
        public string Country { get; set; } = "";

        /// <summary>
        /// Product that was sold
        /// </summary>
        public string Product { get; set; } = string.Empty;

        /// <summary>
        /// Discount band applied to the sale (None, Low, Medium, High)
        /// </summary>
        public string DiscountBand { get; set; } = string.Empty;

        /// <summary>
        /// Number of units sold in this transaction
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal UnitsSold { get; set; }

        /// <summary>
        /// Manufacturing price per unit
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ManufacturingPrice { get; set; }

        /// <summary>
        /// Sale price per unit
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// Date of the sale
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Total revenue for this sale (UnitsSold * SalePrice)
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Revenue => UnitsSold * SalePrice;

        /// <summary>
        /// Total cost for this sale (UnitsSold * ManufacturingPrice)
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Cost => UnitsSold * ManufacturingPrice;

        /// <summary>
        /// Profit for this sale (Revenue - Cost)
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Profit => Revenue - Cost;

        /// <summary>
        /// Profit margin as a percentage (Profit / Revenue * 100)
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal ProfitMargin => Revenue > 0 ? (Profit / Revenue) : 0;

        /// <summary>
        /// Year of the sale
        /// </summary>
        public int Year => Date.Year;

        /// <summary>
        /// Month of the sale
        /// </summary>
        public int Month => Date.Month;

        /// <summary>
        /// Quarter of the sale (1-4)
        /// </summary>
        public int Quarter => (Date.Month - 1) / 3 + 1;
    }
}