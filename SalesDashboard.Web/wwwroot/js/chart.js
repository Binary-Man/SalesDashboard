/**
 * Charts.js - Manages the creation and update of all dashboard charts
 */

// Chart colors with a consistent theme
const chartColors = {
    primary: 'rgba(59, 130, 246, 0.7)',
    secondary: 'rgba(147, 197, 253, 0.7)',
    success: 'rgba(16, 185, 129, 0.7)',
    info: 'rgba(6, 182, 212, 0.7)',
    warning: 'rgba(245, 158, 11, 0.7)',
    danger: 'rgba(239, 68, 68, 0.7)',
    light: 'rgba(249, 250, 251, 0.7)',
    dark: 'rgba(31, 41, 55, 0.7)',
    purple: 'rgba(139, 92, 246, 0.7)',
    orange: 'rgba(249, 115, 22, 0.7)',
    teal: 'rgba(20, 184, 166, 0.7)',
    cyan: 'rgba(6, 182, 212, 0.7)',
    pink: 'rgba(236, 72, 153, 0.7)',
    indigo: 'rgba(99, 102, 241, 0.7)'
};

// Chart palette for consistent coloring across charts
const chartPalette = [
    chartColors.primary,
    chartColors.success,
    chartColors.warning,
    chartColors.danger,
    chartColors.info,
    chartColors.purple,
    chartColors.orange,
    chartColors.teal,
    chartColors.cyan,
    chartColors.pink,
    chartColors.indigo
];

// Currency formatter
const currencyFormatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
});

// Percentage formatter
const percentFormatter = new Intl.NumberFormat('en-US', {
    style: 'percent',
    minimumFractionDigits: 1,
    maximumFractionDigits: 1
});

// Number formatter
const numberFormatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 2
});

// Namespace for charts management
const DashboardCharts = {
    // Chart instances
    charts: {},
    
    // Initialize all charts
    init: function(data) {
        this.createSegmentChart(data.salesBySegment);
        this.createCountryChart(data.salesByCountry);
        this.createMonthlyChart(data.salesByMonth);
        this.createDiscountChart(data.salesByDiscountBand);
        this.createProductChart(data.salesByProduct);
        
        // Make charts responsive
        window.addEventListener('resize', this.handleResize.bind(this));
    },
    
    // Handle window resize
    handleResize: function() {
        Object.values(this.charts).forEach(chart => {
            if (chart && typeof chart.resize === 'function') {
                chart.resize();
            }
        });
    },
    
    // Create segment pie chart
    createSegmentChart: function(data) {
        const ctx = document.getElementById('segmentChart');
        if (!ctx) return;
        
        const labels = Object.keys(data);
        const values = Object.values(data);
        
        this.charts.segment = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: chartPalette.slice(0, labels.length),
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'right',
                        labels: {
                            boxWidth: 12,
                            padding: 15
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const value = context.raw;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = (value / total) * 100;
                                return `${context.label}: ${currencyFormatter.format(value)} (${percentage.toFixed(1)}%)`;
                            }
                        }
                    },
                    title: {
                        display: false
                    }
                },
                cutout: '60%',
                animation: {
                    animateScale: true,
                    animateRotate: true
                }
            }
        });
    },
    
    // Create country bar chart
    createCountryChart: function(data) {
        const ctx = document.getElementById('countryChart');
        if (!ctx) return;
        
        const labels = Object.keys(data);
        const values = Object.values(data);
        
        this.charts.country = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Revenue',
                    data: values,
                    backgroundColor: chartColors.primary,
                    borderColor: chartColors.primary,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `Revenue: ${currencyFormatter.format(context.raw)}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return currencyFormatter.format(value);
                            }
                        }
                    },
                    x: {
                        ticks: {
                            autoSkip: false,
                            maxRotation: 45,
                            minRotation: 45
                        }
                    }
                }
            }
        });
    },
    
    // Create monthly line chart
    createMonthlyChart: function(data) {
        const ctx = document.getElementById('monthlyChart');
        if (!ctx) return;
        
        // Sort data by date
        const sortedData = Object.entries(data)
            .sort(([a], [b]) => a.localeCompare(b))
            .reduce((acc, [key, value]) => {
                acc[key] = value;
                return acc;
            }, {});
        
        const labels = Object.keys(sortedData);
        const values = Object.values(sortedData);
        
        this.charts.monthly = new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Monthly Revenue',
                    data: values,
                    fill: true,
                    backgroundColor: 'rgba(59, 130, 246, 0.1)',
                    borderColor: chartColors.primary,
                    borderWidth: 2,
                    tension: 0.4,
                    pointBackgroundColor: chartColors.primary,
                    pointBorderColor: '#fff',
                    pointRadius: 4,
                    pointHoverRadius: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `Revenue: ${currencyFormatter.format(context.raw)}`;
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return currencyFormatter.format(value);
                            }
                        }
                    },
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            autoSkip: true,
                            maxTicksLimit: 12
                        }
                    }
                },
                interaction: {
                    intersect: false,
                    mode: 'index'
                }
            }
        });
    },
    
    // Create discount band pie chart
    createDiscountChart: function(data) {
        const ctx = document.getElementById('discountChart');
        if (!ctx) return;
        
        const labels = Object.keys(data);
        const values = Object.values(data);
        
        this.charts.discount = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: [
                        chartColors.success,
                        chartColors.info,
                        chartColors.warning,
                        chartColors.danger
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            boxWidth: 12,
                            padding: 15
                        }
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const value = context.raw;
                                const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                const percentage = (value / total) * 100;
                                return `${context.label}: ${currencyFormatter.format(value)} (${percentage.toFixed(1)}%)`;
                            }
                        }
                    }
                },
                animation: {
                    animateScale: true,
                    animateRotate: true
                }
            }
        });
    },
    
    // Create product horizontal bar chart
    createProductChart: function(data) {
        const ctx = document.getElementById('productChart');
        if (!ctx) return;
        
        // Sort products by revenue
        const sortedData = Object.entries(data)
            .sort(([, a], [, b]) => b - a)
            .slice(0, 10)
            .reduce((acc, [key, value]) => {
                acc[key] = value;
                return acc;
            }, {});
        
        const labels = Object.keys(sortedData);
        const values = Object.values(sortedData);
        
        this.charts.product = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Revenue',
                    data: values,
                    backgroundColor: labels.map((_, i) => chartPalette[i % chartPalette.length]),
                    borderWidth: 1
                }]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `Revenue: ${currencyFormatter.format(context.raw)}`;
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return currencyFormatter.format(value);
                            }
                        }
                    },
                    y: {
                        ticks: {
                            autoSkip: false,
                        }
                    }
                }
            }
        });
    },
    
    // Update chart data
    updateChartData: function(chartName, newData) {
        const chart = this.charts[chartName];
        if (!chart) return;
        
        const labels = Object.keys(newData);
        const values = Object.values(newData);
        
        chart.data.labels = labels;
        chart.data.datasets[0].data = values;
        
        // Update colors if needed
        if (chartName === 'segment' || chartName === 'product') {
            chart.data.datasets[0].backgroundColor = labels.map((_, i) => chartPalette[i % chartPalette.length]);
        }
        
        chart.update();
    },
    
    // Update all charts with new data
    updateAllCharts: function(data) {
        if (data.salesBySegment) {
            this.updateChartData('segment', data.salesBySegment);
        }
        
        if (data.salesByCountry) {
            this.updateChartData('country', data.salesByCountry);
        }
        
        if (data.salesByMonth) {
            this.updateChartData('monthly', data.salesByMonth);
        }
        
        if (data.salesByDiscountBand) {
            this.updateChartData('discount', data.salesByDiscountBand);
        }
        
        if (data.salesByProduct) {
            this.updateChartData('product', data.salesByProduct);
        }
    },
    
    // Destroy all charts
    destroyAllCharts: function() {
        Object.values(this.charts).forEach(chart => {
            if (chart && typeof chart.destroy === 'function') {
                chart.destroy();
            }
        });
        
        this.charts = {};
    }
};

// Initialize charts when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Get chart data from the window object (populated by server)
    const dashboardData = window.dashboardData || {
        salesBySegment: {},
        salesByCountry: {},
        salesByMonth: {},
        salesByDiscountBand: {},
        salesByProduct: {}
    };
    
    // Initialize charts
    DashboardCharts.init(dashboardData);
});