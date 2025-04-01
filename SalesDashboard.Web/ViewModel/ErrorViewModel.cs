namespace SalesDashboard.Web.ViewModel;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public string? Error { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public bool ShowError => !string.IsNullOrEmpty(Error);
}


