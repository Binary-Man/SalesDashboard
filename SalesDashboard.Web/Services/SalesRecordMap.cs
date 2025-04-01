using CsvHelper.Configuration;
using SalesDashboard.Web.Models;
using System.Globalization;

namespace SalesDashboard.Web.Services
{
    /// <summary>
    /// CSV mapping class for SalesRecord
    /// </summary>
    public sealed class SalesRecordMap : ClassMap<SalesRecord>
    {
        public SalesRecordMap()
        {
            Map(m => m.Segment).Name("Segment");
            Map(m => m.Country).Name("Country");
            Map(m => m.Product).Name("Product").Convert(row => row.Row.GetField("Product")?.Trim());
            Map(m => m.DiscountBand).Name("Discount Band").Convert(row => row.Row.GetField("Discount Band")?.Trim());
            Map(m => m.UnitsSold).Name("Units Sold").Convert(row =>
            {
                if (decimal.TryParse(row.Row.GetField("Units Sold"), out var result))
                    return result;
                return 0m;
            });
            Map(m => m.ManufacturingPrice).Name("Manufacturing Price").Convert(row =>
            {
                var value = row.Row.GetField("Manufacturing Price")?.Replace("£", "").Trim();
                if (decimal.TryParse(value, out var result))
                    return result;
                return 0m;
            });
            Map(m => m.SalePrice).Name("Sale Price").Convert(row =>
            {
                var value = row.Row.GetField("Sale Price")?.Replace("£", "").Trim();
                if (decimal.TryParse(value, out var result))
                    return result;
                return 0m;
            });
            Map(m => m.Date).Name("Date").Convert(row =>
            {
                if (DateTime.TryParseExact(row.Row.GetField("Date"), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                    return result;

                // Try alternate format if the first one fails
                if (DateTime.TryParse(row.Row.GetField("Date"), CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    return result;

                return DateTime.MinValue;
            });
        }
    }
}