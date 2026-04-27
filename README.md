# StockPulse API

StockPulse is a small backend-focused Financial Data Tracker built with ASP.NET Core Web API.  
The project fetches real stock quote data from an external financial API, stores the relevant data in a local SQLite database, exposes RESTful endpoints, and provides an analytical view for tracked stocks.

This project was developed for the Rasyonet Software Engineering Internship technical assessment.

---

## Project Purpose

The goal of StockPulse is to simulate a simple internal tool that could be used by a finance-oriented company.

The application allows users to:

- Track stock symbols such as `AAPL`, `MSFT`, `TSLA`, and `NVDA`
- Fetch real quote data from Finnhub
- Store stock and price snapshot data locally
- Retrieve tracked stocks through RESTful API endpoints
- Refresh quote data for already tracked stocks
- View an analytical "top movers" list based on latest percentage change

The scope is intentionally small and backend-oriented. The priority was to deliver a clean, working, maintainable Web API rather than a large or over-engineered solution.

---

## Assessment Requirements Coverage

| Requirement                        | Implementation                                                                              |
| ---------------------------------- | ------------------------------------------------------------------------------------------- |
| .NET 6 or later Web API            | Implemented with .NET 8 ASP.NET Core Web API                                                |
| External financial API integration | Finnhub quote API integration                                                               |
| Local database persistence         | SQLite with Entity Framework Core                                                           |
| At least one core entity           | `Stock` and `PriceSnapshot` entities                                                        |
| RESTful API endpoints              | `StocksController` endpoints for tracking, retrieving, refreshing, and deleting stocks      |
| Analytical / aggregation use case  | `GET /api/Analytics/top-movers`                                                             |
| OOP principles                     | Models, DTOs, interfaces, services, repositories, dependency injection                      |
| Design pattern                     | Repository Pattern and Strategy Pattern                                                     |
| Inline design pattern comment      | Added near the pattern implementation in code                                               |
| Swagger / OpenAPI                  | Enabled and functional through Swagger UI                                                   |
| README documentation               | Project purpose, setup, API/database choice, endpoints, patterns, and trade-offs documented |
| No sensitive data committed        | API key is handled through User Secrets / environment configuration                         |
| Clean layer separation             | Controllers, Services, Repositories, Models, DTOs, External, Data                           |


---

## Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **C#**
- **Entity Framework Core**
- **SQLite**
- **Finnhub API**
- **Swagger / OpenAPI**
- **User Secrets for local API key management**

---

## External API Choice

The project uses **Finnhub** as the external financial data provider.

Finnhub was selected because:

- It provides stock quote data suitable for this project.
- It has a free tier that is enough for a small backend assessment project.
- It supports symbol-based quote requests.
- Its quote response contains useful financial fields such as current price, open price, high price, low price, previous close, and percentage change.

The application currently uses Finnhub quote data for stock symbols such as:

```text
AAPL
MSFT
TSLA
NVDA
```

---

## Database Choice

The project uses **SQLite** as the local database.

SQLite was selected because:

- It is lightweight and easy to run locally.
- It does not require a separate database server.
- It is suitable for a small internal-tool-style assessment project.
- It integrates cleanly with Entity Framework Core.
- It keeps the setup simple for reviewers.

The local database file is intentionally ignored by Git through `.gitignore`.  
The database schema can be recreated through EF Core migrations.

---

## Database Design

The project has two main database entities:

### Stock

Represents a tracked stock symbol.

Main fields:

- `Id`
- `Symbol`
- `CompanyName`
- `Exchange`
- `CreatedAtUtc`
- `PriceSnapshots`

A unique index is configured for `Symbol` to avoid duplicate tracked stock records.

### PriceSnapshot

Represents quote data captured at a specific point in time.

Main fields:

- `Id`
- `StockId`
- `CapturedAtUtc`
- `CurrentPrice`
- `OpenPrice`
- `HighPrice`
- `LowPrice`
- `PreviousClose`
- `ChangePercent`

A stock can have multiple price snapshots.  
This allows the application to store refreshed quote data over time.

---

## Entity Framework Core and Migrations

This project uses Entity Framework Core with a Code First approach.

The database schema is generated from C# entity models instead of handwritten SQL scripts.

The flow is:

```text
C# Entity Models
→ AppDbContext
→ EF Core Migration
→ SQLite Database
```

The initial migration creates:

- `Stocks`
- `PriceSnapshots`
- `__EFMigrationsHistory`

The relationship between `Stock` and `PriceSnapshot` is configured with a foreign key.  
Cascade delete is enabled, so when a stock is deleted, its related price snapshots are deleted as well.

---

## Project Structure

```text
StockPulse
│
├── StockPulse.sln
├── README.md
├── .gitignore
│
└── StockPulse.Api
    │
    ├── Controllers
    │   ├── StocksController.cs
    │   └── AnalyticsController.cs
    │
    ├── Data
    │   └── AppDbContext.cs
    │
    ├── DTOs
    │   ├── StockResponseDto.cs
    │   ├── PriceSnapshotResponseDto.cs
    │   └── TopMoverDto.cs
    │
    ├── External
    │   ├── IFinancialDataProvider.cs
    │   ├── FinnhubFinancialDataProvider.cs
    │   ├── FinnhubOptions.cs
    │   ├── FinnhubQuoteResponse.cs
    │   └── FinancialQuoteDto.cs
    │
    ├── Migrations
    │
    ├── Models
    │   ├── Stock.cs
    │   └── PriceSnapshot.cs
    │
    ├── Repositories
    │   ├── IStockRepository.cs
    │   └── StockRepository.cs
    │
    ├── Services
    │   ├── IStockService.cs
    │   ├── StockService.cs
    │   ├── IAnalyticsService.cs
    │   └── AnalyticsService.cs
    │
    ├── appsettings.json
    └── Program.cs
```

---

## Architecture and Separation of Concerns

The project follows a simple layered architecture:

### Controllers

Controllers are responsible for HTTP request/response handling only.

They do not contain business logic.

Controllers call service interfaces and return meaningful HTTP responses such as:

- `200 OK`
- `204 No Content`
- `400 Bad Request`
- `404 Not Found`

### Services

Services contain the main business logic.

Examples:

- Normalizing stock symbols
- Tracking a new stock
- Refreshing quote data
- Mapping entities to DTOs
- Preparing analytical results

### Repositories

Repositories isolate database access from the service layer.

This prevents business logic from depending directly on Entity Framework Core query details.

### External API Layer

External API integration is isolated under the `External` folder.

The current implementation uses Finnhub, but the rest of the application depends on an abstraction instead of directly depending on Finnhub-specific code.

---

## Design Patterns Used

### Repository Pattern

The Repository Pattern is used to separate database access logic from business logic.

Implemented in:

```text
Repositories/IStockRepository.cs
Repositories/StockRepository.cs
```

Why it was used:

- Keeps EF Core-specific data access code out of services and controllers
- Improves separation of concerns
- Makes the service layer cleaner
- Makes future testing or database changes easier

### Strategy Pattern

The Strategy Pattern is used for external financial data provider access.

Implemented in:

```text
External/IFinancialDataProvider.cs
External/FinnhubFinancialDataProvider.cs
```

Why it was used:

- The application currently uses Finnhub.
- The service layer depends on `IFinancialDataProvider`, not directly on Finnhub.
- Another provider such as Alpha Vantage could be added later without rewriting the business logic.
- This keeps external API-specific code isolated.

---

## API Endpoints

### Stocks

#### Get all tracked stocks

```http
GET /api/Stocks
```

Returns all tracked stocks with their latest price snapshot.

---

#### Get a stock by symbol

```http
GET /api/Stocks/{symbol}
```

Example:

```http
GET /api/Stocks/AAPL
```

Returns a tracked stock by symbol.

If the stock does not exist, the API returns:

```http
404 Not Found
```

---

#### Track a stock

```http
POST /api/Stocks/track/{symbol}
```

Example:

```http
POST /api/Stocks/track/AAPL
```

This endpoint:

1. Normalizes the symbol
2. Calls Finnhub quote API
3. Creates a `Stock` record if it does not exist
4. Creates a `PriceSnapshot`
5. Saves the data to SQLite
6. Returns the tracked stock with the latest snapshot

If the stock already exists, the endpoint refreshes it by adding a new price snapshot.

---

#### Refresh a tracked stock

```http
POST /api/Stocks/{symbol}/refresh
```

Example:

```http
POST /api/Stocks/AAPL/refresh
```

This endpoint fetches the latest quote data for an already tracked stock and adds a new price snapshot.

If the stock does not exist, the API returns:

```http
404 Not Found
```

---

#### Delete a tracked stock

```http
DELETE /api/Stocks/{symbol}
```

Example:

```http
DELETE /api/Stocks/AAPL
```

Deletes the stock and its related price snapshots.

If deletion succeeds, the API returns:

```http
204 No Content
```

---

### Analytics

#### Get top movers

```http
GET /api/Analytics/top-movers?limit=5
```

Returns tracked stocks ordered by their latest `ChangePercent` value in descending order.

This endpoint satisfies the analytical / aggregation requirement.

Example response:

```json
[
  {
    "symbol": "NVDA",
    "companyName": "NVDA",
    "currentPrice": 215.6,
    "previousClose": 208.27,
    "changePercent": 3.52,
    "capturedAtUtc": "2026-04-27T19:26:50Z"
  },
  {
    "symbol": "TSLA",
    "companyName": "TSLA",
    "currentPrice": 379.24,
    "previousClose": 376.3,
    "changePercent": 0.78,
    "capturedAtUtc": "2026-04-27T19:26:26Z"
  }
]
```

---

## Error Handling

The project includes basic error handling with meaningful HTTP status codes.

Examples:

- Empty or invalid stock symbol → `400 Bad Request`
- Requested stock does not exist → `404 Not Found`
- Delete request for a missing stock → `404 Not Found`
- Successful deletion → `204 No Content`
- Successful read/create/update requests → `200 OK`

The API aims to return meaningful HTTP responses for expected error cases instead of exposing implementation details.

---

## API Key Management

The Finnhub API key should not be committed to source control.

The project keeps the key out of `appsettings.json`.

`appsettings.json` contains only a placeholder section:

```json
{
  "Finnhub": {
    "ApiKey": ""
  }
}
```

For local development, use User Secrets.

### Option 1: Visual Studio

Right-click the `StockPulse.Api` project:

```text
Manage User Secrets
```

Add:

```json
{
  "Finnhub": {
    "ApiKey": "YOUR_FINNHUB_API_KEY"
  }
}
```

### Option 2: Command Line

From the solution root:

```bash
dotnet user-secrets set "Finnhub:ApiKey" "YOUR_FINNHUB_API_KEY" --project StockPulse.Api
```

---

## Setup and Run Instructions

### 1. Clone the repository

```bash
git clone <repository-url>
cd StockPulse
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Configure Finnhub API key

Set the Finnhub API key using User Secrets:

```bash
dotnet user-secrets set "Finnhub:ApiKey" "YOUR_FINNHUB_API_KEY" --project StockPulse.Api
```

Alternatively, use Visual Studio:

```text
Right-click StockPulse.Api
→ Manage User Secrets
→ Add Finnhub API key
```

### 4. Apply database migrations

Using .NET CLI:

```bash
dotnet ef database update --project StockPulse.Api
```

Or using Visual Studio Package Manager Console:

```powershell
Update-Database
```

The SQLite database will be created locally.

### 5. Run the API

Using .NET CLI:

```bash
dotnet run --project StockPulse.Api
```

Or run the `https` profile from Visual Studio.

### 6. Open Swagger UI

After running the project, open:

```text
https://localhost:<port>/swagger
```

Swagger UI can be used to test all endpoints.

---

## Suggested Test Flow

After starting the application, test the API in Swagger in this order:

```http
POST /api/Stocks/track/AAPL
POST /api/Stocks/track/MSFT
POST /api/Stocks/track/TSLA
POST /api/Stocks/track/NVDA
GET  /api/Stocks
GET  /api/Stocks/AAPL
POST /api/Stocks/AAPL/refresh
GET  /api/Analytics/top-movers?limit=5
```

Expected behavior:

- The `track` endpoints fetch real data from Finnhub and persist it.
- `GET /api/Stocks` returns all tracked stocks.
- `GET /api/Stocks/{symbol}` returns a specific tracked stock.
- `refresh` adds a new price snapshot for an existing stock.
- `top-movers` returns the latest tracked stocks ordered by percentage change.

---

## Example Tracked Symbols

The application was tested with the following symbols:

```text
AAPL
MSFT
TSLA
NVDA
```

---

## Git Ignore Notes

The repository excludes local and sensitive files such as:

```text
bin/
obj/
.vs/
*.db
*.db-shm
*.db-wal
secrets.json
.env
```

The local SQLite database file is intentionally not committed.

Migrations are committed so the database schema can be recreated.

---

## Trade-offs and Scope Decisions

This project focuses on the required backend functionality.

The following items were intentionally not included because they were listed as bonus items rather than must-have requirements:

- Frontend application
- Docker support
- Unit tests

The goal was to keep the project small, clean, and working while fully covering the must-have backend requirements.

A frontend could be added later using React, Angular, Blazor, or MVC, but it was not necessary for the core backend assessment.

---

## Current Status

Implemented:

- ASP.NET Core Web API
- Finnhub external API integration
- SQLite persistence
- EF Core migrations
- RESTful stock endpoints
- Analytical top movers endpoint
- Repository Pattern
- Strategy Pattern
- Swagger / OpenAPI
- User Secrets for API key management
- Layered project structure

---

## Summary

StockPulse is a backend Financial Data Tracker that fetches real stock quote data from Finnhub, stores it in a local SQLite database, exposes RESTful API endpoints, and provides an analytical top movers view.

The project is intentionally small, clean, and structured around maintainable backend development practices.