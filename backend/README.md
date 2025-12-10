# GSC Tracking - Backend API

.NET 10 Web API for the GSC Small Engine Repair business management application.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Getting Started

### Running the API

```bash
cd backend/GscTracking.Api
dotnet run
```

The API will start at:
- HTTP: `http://localhost:5091`
- HTTPS: `https://localhost:7075`

By default, the HTTP profile is used. To use HTTPS:
```bash
dotnet run --launch-profile https
```

### Testing the API

Open your browser or use curl:

```bash
curl http://localhost:5091/api/hello
```

You should see:
```json
{
  "message": "Hello from GSC Tracking API!",
  "version": "1.0.0",
  "timestamp": "2025-12-08T04:10:00Z"
}
```

## Running Tests

### Run all tests
```bash
cd backend/GscTracking.Api.Tests
dotnet test
```

### Run tests with code coverage
```bash
cd backend/GscTracking.Api.Tests
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

### Generate coverage report
```bash
cd backend/GscTracking.Api.Tests
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:"Html;TextSummary"
# View the report
open coveragereport/index.html  # macOS
# or
xdg-open coveragereport/index.html  # Linux
```

### Current Coverage
- **Controllers**: 100% coverage (CustomersController)
- **Services**: 100% coverage (CustomerService)
- **Overall**: ~28% (excluding migrations, Program.cs, and validators)

### Development

The API is configured with:
- OpenAPI/Swagger documentation (available in development mode)
- CORS enabled for frontend at `http://localhost:5173`
- HTTPS redirection

### Project Structure

```
backend/
└── GscTracking.Api/
    ├── Program.cs           # Application entry point and configuration
    ├── appsettings.json     # Application configuration
    └── GscTracking.Api.csproj  # Project file
```

## Next Steps

1. Add Entity Framework Core for database access
2. Implement authentication with Auth0
3. Create customer, job, and financial endpoints
4. Add validation and error handling

See [business-management-app-analysis.md](../business-management-app-analysis.md) for full requirements.
