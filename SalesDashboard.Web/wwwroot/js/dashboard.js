/**
 * Sales Dashboard JavaScript
 * Handles data fetching, interactivity, and export functionality not fully implmented 
 */

// Namespace for the dashboard
const SalesDashboard = {
    // Cache for data
    data: {
        allSales: [],
        filteredSales: []
    },
    
    // Initialize the dashboard
    init: function() {
        this.setupEventListeners();
        this.initializeDataTable();
        this.setupExportButton();
    },
    
    // Set up event listeners
    setupEventListeners: function() {
        // Filter dropdown changes
        document.querySelectorAll('.filter-dropdown').forEach(dropdown => {
            dropdown.addEventListener('change', this.handleFilterChange.bind(this));
        });
        
        // Date range selector
        const dateRangeSelect = document.getElementById('dateRangeSelect');
        if (dateRangeSelect) {
            dateRangeSelect.addEventListener('change', this.handleDateRangeChange.bind(this));
        }
        
        // Refresh button
        const refreshButton = document.getElementById('refreshButton');
        if (refreshButton) {
            refreshButton.addEventListener('click', this.refreshData.bind(this));
        }
    },
    
    // Initialize DataTable
    initializeDataTable: function() {
        const salesTable = document.getElementById('salesTable');
        //if (salesTable) {
        //    salesTable.destroy();
        //}
        if (!salesTable) {
            this.dataTable = $(salesTable).DataTable({
                pageLength: 10,
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                responsive: true,
                dom: 'Bfrtip',
                buttons: [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ],
                language: {
                    search: "_INPUT_",
                    searchPlaceholder: "Search..."
                },
                order: [[0, 'desc']], // Sort by date descending
                columnDefs: [
                    { type: 'date', targets: 0 } // Date column
                ]
            });
        }
    },
    
    // Set up export button
    setupExportButton: function() {
        const exportButton = document.getElementById('btnExport');
        if (exportButton) {
            exportButton.addEventListener('click', function() {
                // Create a CSV from the current filtered data
                const csvContent = this.convertToCSV(this.data.filteredSales || this.data.allSales);
                
                // Create a downloadable link
                const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
                const url = URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.setAttribute('href', url);
                link.setAttribute('download', `sales_export_${new Date().toISOString().split('T')[0]}.csv`);
                link.style.visibility = 'hidden';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }.bind(this));
        }
    },
    
    // Handle filter changes
    handleFilterChange: function(event) {
        const filterId = event.target.id;
        const filterValue = event.target.value;
        
        // Apply the filter to the data
        if (filterValue === 'all') {
            this.data.filteredSales = [...this.data.allSales];
        } else {
            // Determine which property to filter on based on the filter ID
            let filterProperty;
            switch (filterId) {
                case 'segmentFilter':
                    filterProperty = 'Segment';
                    break;
                case 'countryFilter':
                    filterProperty = 'Country';
                    break;
                case 'productFilter':
                    filterProperty = 'Product';
                    break;
                case 'discountFilter':
                    filterProperty = 'DiscountBand';
                    break;
                default:
                    filterProperty = null;
            }
            
            if (filterProperty) {
                this.data.filteredSales = this.data.allSales.filter(item => 
                    item[filterProperty] === filterValue
                );
            }
        }
        
        // Update the UI with the filtered data
        this.updateCharts();
        this.updateDataTable();
    },
    
    // Handle date range changes
    handleDateRangeChange: function(event) {
        const range = event.target.value;
        const now = new Date();
        let startDate;
        
        // Calculate start date based on selected range
        switch (range) {
            case '7days':
                startDate = new Date(now.setDate(now.getDate() - 7));
                break;
            case '30days':
                startDate = new Date(now.setDate(now.getDate() - 30));
                break;
            case '90days':
                startDate = new Date(now.setDate(now.getDate() - 90));
                break;
            case '12months':
                startDate = new Date(now.setFullYear(now.getFullYear() - 1));
                break;
            case 'all':
            default:
                startDate = new Date(0); // Beginning of time
                break;
        }
        
        // Filter data by date range
        this.data.filteredSales = this.data.allSales.filter(item => 
            new Date(item.Date) >= startDate
        );
        
        // Update the UI
        this.updateCharts();
        this.updateDataTable();
    },
    
    // Refresh data from the server
    refreshData: function() {
        this.fetchSalesData()
            .then(() => {
                this.updateCharts();
                this.updateDataTable();
                this.showToast('Data refreshed successfully');
            })
            .catch(error => {
                console.error('Error refreshing data:', error);
                this.showToast('Failed to refresh data', 'error');
            });
    },
    
    // Fetch sales data from the API
    fetchSalesData: function() {
        return fetch('/Home/GetSalesData')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                this.data.allSales = data;
                this.data.filteredSales = [...data];
                return data;
            });
    },
    
    // Update charts with current data
    updateCharts: function() {
        // This would be implemented to update the charts with new data
        // For simplicity, we'll assume chart updates are handled elsewhere
        console.log('Charts should update with filtered data');
        
        // If using Chart.js, you would update each chart with new data like:
        // myChart.data.datasets[0].data = newData;
        // myChart.update();
    },
    
    // Update DataTable with current filtered data
    updateDataTable: function() {
        if (this.dataTable) {
            this.dataTable.clear();
            this.dataTable.rows.add(this.data.filteredSales);
            this.dataTable.draw();
        }
    },
    
    // Show a toast notification
    showToast: function(message, type = 'success') {
        // Find or create toast container
        let toastContainer = document.querySelector('.toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(toastContainer);
        }
        
        // Create toast element
        const toastEl = document.createElement('div');
        toastEl.className = `toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0`;
        //toastEl.setAttribute('role', 'alert');
        toastEl.setAttribute('aria-live', 'assertive');
        toastEl.setAttribute('aria-atomic', 'true');
        
        // Toast content
        toastEl.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;
        
        // Add to container
        toastContainer.appendChild(toastEl);
        
        // Initialize and show
        const toast = new bootstrap.Toast(toastEl, {
            autohide: true,
            delay: 3000
        });
        toast.show();
        
        // Remove after hiding
        toastEl.addEventListener('hidden.bs.toast', function() {
            toastEl.remove();
        });
    },
    
    // Convert data to CSV format
    convertToCSV: function(data) {
        if (!data || data.length === 0) {
            return '';
        }
        
        // Get headers
        const headers = Object.keys(data[0]);
        
        // Create CSV rows
        const csvRows = [
            headers.join(','), // Header row
            ...data.map(row => 
                headers.map(header => {
                    // Handle values that need to be quoted
                    const value = row[header];
                    const strValue = String(value);
                    return strValue.includes(',') ? `"${strValue}"` : strValue;
                }).join(',')
            )
        ];
        
        // Join rows with newlines
        return csvRows.join('\n');
    }
};

// Initialize dashboard when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    SalesDashboard.init();
});