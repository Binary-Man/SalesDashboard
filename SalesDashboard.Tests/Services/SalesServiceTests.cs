using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SalesDashboard.Web.Models;
using SalesDashboard.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SalesDashboard.Tests.Services
{
    public class SalesServiceTests
    {
        private readonly Mock<ICsvService> _mockCsvService;
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<SalesService>> _mockLogger;
        private readonly SalesService _salesService;
        private readonly List<SalesRecord> _testData;

        public SalesServiceTests()
        {
            // Setup test data
            _testData = new List<SalesRecord>
            {
                new SalesRecord
                {
                    Segment = "Government",
                    Country = "United States of America",
                    Product = "Carretera",
                    DiscountBand = "None",
                    UnitsSold = 1000,
                    ManufacturingPrice = 3,
                    SalePrice = 20,
                    Date = new DateTime(2014, 1, 1)
                },
                new SalesRecord
                {
                    Segment = "Government",
                    Country = "Canada",
                    Product = "Carretera",
                    DiscountBand = "None",
                    UnitsSold = 1500,
                    ManufacturingPrice = 3,
                    SalePrice = 20,
                    Date = new DateTime(2014, 1, 1)
                },
                new SalesRecord
                {
                    Segment = "Midmarket",
                    Country = "France",
                    Product = "Velo",
                    DiscountBand = "Medium",
                    UnitsSold = 2000,
                    ManufacturingPrice = 5,
                    SalePrice = 15,
                    Date = new DateTime(2014, 2, 1)
                },
                new SalesRecord
                {
                    Segment = "Enterprise",
                    Country = "Germany",
                    Product = "Paseo",
                    DiscountBand = "High",
                    UnitsSold = 3000,
                    ManufacturingPrice = 10,
                    SalePrice = 30,
                    Date = new DateTime(2014, 3, 1)
                },
                new SalesRecord
                {
                    Segment = "Small Business",
                    Country = "Mexico",
                    Product = "Montana",
                    DiscountBand = "Low",
                    UnitsSold = 2500,
                    ManufacturingPrice = 8,
                    SalePrice = 25,
                    Date = new DateTime(2014, 4, 1)
                }
            };

            // Setup mocks
            _mockCsvService = new Mock<ICsvService>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<SalesService>>();
            
            // Setup cache mock
            var mockCacheEntry = new Mock<ICacheEntry>();
            _mockCache
                .Setup(m => m.CreateEntry(It.IsAny<object>()))
                .Returns(mockCacheEntry.Object);
            
            // Configure the mock service to return test data
            _mockCsvService
                .Setup(m => m.LoadSalesDataAsync(It.IsAny<string>()))
                .ReturnsAsync(_testData);
            
            // Create service instance with mocked dependencies
            _salesService = new SalesService(_mockCsvService.Object, _mockCache.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllSalesRecordsAsync_ShouldReturnAllRecords()
        {
            // Arrange
            var cacheKey = "SalesData";
            object testDataOut = null;
            
            _mockCache
                .Setup(m => m.TryGetValue(cacheKey, out testDataOut))
                .Returns(false);
            
            // Act
            var result = await _salesService.GetAllSalesRecordsAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(_testData.Count, result.Count());
            _mockCsvService.Verify(m => m.LoadSalesDataAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetSalesSummaryAsync_ShouldCalculateCorrectTotals()
        {
            // Arrange
            var cacheKey = "SalesData";
            object testDataOut = null;
            
            _mockCache
                .Setup(m => m.TryGetValue(cacheKey, out testDataOut))
                .Returns(false);
            
            // Expected values from test data
            decimal expectedTotalRevenue = 
                (1000 * 20) + // 1st record
                (1500 * 20) + // 2nd record
                (2000 * 15) + // 3rd record
                (3000 * 30) + // 4th record
                (2500 * 25);  // 5th record
            
            decimal expectedTotalProfit = 
                (1000 * (20 - 3)) + // 1st record
                (1500 * (20 - 3)) + // 2nd record
                (2000 * (15 - 5)) + // 3rd record
                (3000 * (30 - 10)) + // 4th record
                (2500 * (25 - 8));  // 5th record
            
            decimal expectedTotalUnitsSold = 1000 + 1500 + 2000 + 3000 + 2500;
            decimal expectedAvgOrderValue = expectedTotalRevenue / 5; // 5 records
            
            // Act
            var result = await _salesService.GetSalesSummaryAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedTotalRevenue, result.TotalRevenue);
            Assert.Equal(expectedTotalProfit, result.TotalProfit);
            Assert.Equal(expectedTotalUnitsSold, result.TotalUnitsSold);
            Assert.Equal(expectedAvgOrderValue, result.AverageOrderValue);
            Assert.Equal(5, result.OrderCount);
            Assert.Equal(expectedTotalProfit / expectedTotalRevenue, result.ProfitMargin);
        }

        [Fact]
        public async Task GetSalesBySegmentAsync_ShouldGroupCorrectly()
        {
            // Arrange
            var cacheKey = "SalesData";
            object testDataOut = null;
            
            _mockCache
                .Setup(m => m.TryGetValue(cacheKey, out testDataOut))
                .Returns(false);
            
            // Expected values from test data
            var expectedSegments = new Dictionary<string, decimal>
            {
                ["Government"] = (1000 * 20) + (1500 * 20), // 50,000
                ["Midmarket"] = (2000 * 15),               // 30,000
                ["Enterprise"] = (3000 * 30),              // 90,000
                ["Small Business"] = (2500 * 25)           // 62,500
            };
            
            // Act
            var result = await _salesService.GetSalesBySegmentAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count); // 4 different segments
            
            foreach (var segment in expectedSegments.Keys)
            {
                Assert.True(result.ContainsKey(segment));
                Assert.Equal(expectedSegments[segment], result[segment]);
            }
        }

        [Fact]
        public async Task GetSalesByMonthAsync_ShouldGroupByYearAndMonth()
        {
            // Arrange
            var cacheKey = "SalesData";
            object testDataOut = null;
            
            _mockCache
                .Setup(m => m.TryGetValue(cacheKey, out testDataOut))
                .Returns(false);
            
            // Expected values from test data (grouped by year-month)
            var expectedMonths = new Dictionary<string, decimal>
            {
                ["2014-01"] = (1000 * 20) + (1500 * 20),  // Jan 2014: 50,000
                ["2014-02"] = (2000 * 15),                // Feb 2014: 30,000
                ["2014-03"] = (3000 * 30),                // Mar 2014: 90,000
                ["2014-04"] = (2500 * 25)                 // Apr 2014: 62,500
            };
            
            // Act
            var result = await _salesService.GetSalesByMonthAsync();
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(4, result.Count); // 4 different months
            
            foreach (var month in expectedMonths.Keys)
            {
                Assert.True(result.ContainsKey(month));
                Assert.Equal(expectedMonths[month], result[month]);
            }
        }
    }
}