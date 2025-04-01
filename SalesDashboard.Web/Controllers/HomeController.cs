using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Web.Services;
using SalesDashboard.Web.ViewModel;
using System.Diagnostics;

namespace SalesDashboard.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISalesService _salesService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ISalesService salesService, ILogger<HomeController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        /// <summary>
        /// Main dashboard page
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                //throw new Exception("Simulated error for testing");
                var viewModel = new DashboardViewModel
                {
                    Summary = await _salesService.GetSalesSummaryAsync(),
                    SalesBySegment = await _salesService.GetSalesBySegmentAsync(),
                    SalesByCountry = await _salesService.GetSalesByCountryAsync(),
                    SalesByProduct = await _salesService.GetSalesByProductAsync(),
                    SalesByMonth = await _salesService.GetSalesByMonthAsync(),
                    SalesByDiscountBand = await _salesService.GetSalesByDiscountBandAsync()
                };

                // Get recent sales for data table (limit to 50 for performance)
                var allSales = await _salesService.GetAllSalesRecordsAsync();
                viewModel.RecentSales = allSales.OrderByDescending(s => s.Date).Take(50);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Error = ex.Message });
            }
        }

        /// <summary>
        /// API endpoint to get full sales data for client-side processing
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSalesData()
        {
            try
            {
                var salesData = await _salesService.GetAllSalesRecordsAsync();
                return Json(salesData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales data");
                return StatusCode(500, "An error occurred while retrieving sales data");
            }
        }

        /// <summary>
        /// Error page
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    //public class ErrorViewModel
    //{
    //    public string? RequestId { get; set; }
    //    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    //}
}