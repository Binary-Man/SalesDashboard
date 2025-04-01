# Sales Dashboard

A modern, responsive web application for visualising sales data from CSV files.

## Overview

This web application displays sales data in an interactive dashboard that provides insights into sales performance across different segments, countries, products, and time periods. The dashboard includes summary statistics, various charts, and a detailed data table with filtering and export capabilities.

## Features

- **Dashboard Overview**: Key metrics with at-a-glance statistics
- **Interactive Charts**: Visualize sales data by segment, country, product, and more
- **Data Table**: Sortable and filterable table of sales records
- **Export Functionality**: Export data to CSV format (NOT FULLY IMPLEMENTED)
- **Responsive Design**: Works on desktop and mobile devices
- **Performance Optimized**: Data caching and efficient rendering

## Technology Stack

### Backend

- **ASP.NET Core 8.0**: Modern, cross-platform framework
- **C# 12**: Latest language features
- **MVC Architecture**: Clean separation of concerns
- **LINQ**: Efficient data processing and querying
- **Dependency Injection**: For loosely coupled components
- **CsvHelper**: Fast and flexible CSV parser

### Frontend

- **Bootstrap 5**: Responsive UI framework
- **Chart.js**: Interactive data visualization
- **jQuery**: DOM manipulation and AJAX
- **DataTables**: Enhanced table functionality
- **Font Awesome**: Icons

## Getting Started

### Prerequisites

- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (recommended) or **Visual Studio Code**

### Installation

1. Clone the repository:

   ```
   git clone https://github.com/Binary-Man/SalesDashboard.git
   ```

2. Navigate to the project directory:

   ```
   cd salesdashboard
   ```

3. Restore dependencies:

   ```
   dotnet restore
   ```

4. Build the project:

   ```
   dotnet build
   ```

5. Run the application:

   ```
   dotnet run --project SalesDashboard.Web
   ```

6. Open a web browser and navigate to:
   ```
   https://localhost:5064
   ```

### Data Setup

The application expects a CSV file named `Data.csv` in the following location:

```
SalesDashboard.Web/data/Data.csv
```

Make sure your CSV file has the following columns:

- Segment
- Country
- Product
- Discount Band
- Units Sold
- Manufacturing Price
- Sale Price
- Date

## Architecture

The solution follows a clean architecture approach:

### Core Components

1. **Data Models**:

   - `SalesRecord.cs`: Represents an individual sales record
   - `DashboardViewModel.cs`: View model for the dashboard

2. **Services**:

   - `CsvService.cs`: Responsible for loading and parsing CSV data
   - `SalesService.cs`: Handles business logic and data analysis

3. **Controllers**:

   - `HomeController.cs`: Main controller for the dashboard
   - `ApiController.cs`: API endpoints for AJAX requests (BONUS)

4. **Views**:

   - `Index.cshtml`: Main dashboard view
   - `_Layout.cshtml`: Shared layout

5. **JavaScript Libraries**:
   - `dashboard.js`: Core dashboard functionality
   - `charts.js`: Chart configuration and rendering

## Performance Considerations

- **Data Caching**: Sales data is cached in memory for improved performance
- **Lazy Loading**: Charts and data are loaded asynchronously
- **Client-Side Rendering**: Interactive charts are rendered on the client
- **Server-Side Processing**: Heavy data operations are performed on the server

## Extensibility

The application is designed for easy extension:

1. **Add New Chart Types**:

   - Create a new method in `DashboardCharts` in `charts.js`
   - Add a new container div in the view
   - Create the corresponding data aggregation method in `SalesService.cs`

2. **Add New Data Sources**:

   - Implement a new service that inherits from `ISalesService` interface
   - Register the new service in `Program.cs`
   - The dashboard will automatically use the new data source

3. **Add New Filters**:
   - Add a new dropdown in the view
   - Add the corresponding filter handlers in `dashboard.js`
   - Implement the filter logic in `ApiController.cs`

## Code Best Practices

This solution implements several industry best practices:

1. **SOLID Principles**:

   - Single Responsibility: Each class has a single responsibility
   - Open/Closed: Architecture is open for extension but closed for modification
   - Liskov Substitution: Services can be substituted with different implementations
   - Interface Segregation: Interfaces are focused and specific
   - Dependency Inversion: System depends on abstractions, not concrete implementations

2. **Clean Code**:

   - Meaningful names for classes, methods, and variables
   - Comments explaining complex logic
   - Consistent formatting and coding style
   - Error handling with meaningful exceptions
   - Proper null checking and validation

3. **Security**:

   - Input validation
   - HTTPS enforcement
   - Safe error handling that doesn't expose system details
   - CORS policy configuration

4. **Testing**:
   - Unit tests for services and controllers
   - Test coverage for critical business logic
   - Mock dependencies for isolated testing
