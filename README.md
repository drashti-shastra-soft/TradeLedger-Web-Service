# 🚀 TradeLedger – Web Service API

**TradeLedger Web Service** is a backend API designed to power the TradeLedger trading journal and analytics platform.

It provides API services for managing trading activities, importing trade data, calculating profit and loss (P&L), analyzing trading performance, managing watchlists, and generating trading insights.

The API acts as the central backend layer between the TradeLedger Web Console and the application's data storage.

## ✨ Key Features

- **Authentication & Authorization:** Secure user authentication and access management.
- **User Management:** Manage user accounts and profile information.
- **Trade Management:** Create, update, retrieve, and manage trading records.
- **Trading Journal:** Maintain structured trading history.
- **CSV Import:** Process and import trading data from supported CSV files.
- **P&L Management:** Calculate and track trading profit and loss.
- **Analytics:** Provide trading performance statistics and insights.
- **Watchlist Management:** Manage and monitor trading watchlists.
- **Risk Management:** Support trading risk and reward analysis.
- **AI Trade Review:** Support AI-powered trading performance reviews.
- **Reports:** Provide historical trading data and performance reports.
- **Logging:** Maintain application logs for monitoring and troubleshooting.

## 🛠️ Technology Stack

- ASP.NET Core Web API / ASP.NET Web API (verify target framework)
- C#
- Entity Framework / Entity Framework Core (as configured)
- SQL Server (if configured as the database provider)
- RESTful API Architecture
- LINQ
- Dependency Injection
- JWT Authentication (if enabled)

## 🏗️ Project Architecture

The application follows a layered architecture to maintain separation of concerns, reusability, and maintainability.

```text
TradeLedger-Web-Service/
│
├── TradeLedgerWebService.Domain/
│
├── TradeLedgerWebService.Infrastructure/
│
├── TradeLedgerWebService.Persistence/
│
├── TradeLedgerWebService.Repository/
│
├── TradeLedgerWebService.Service/
│
├── TradeLedgerWebService/
│
└── TradeLedgerWebService.sln
```

### Layer Responsibilities

| Layer          | Responsibility                                       |
| -------------- | ---------------------------------------------------- |
| Domain         | Contains application entities and domain models.     |
| Infrastructure | Handles infrastructure-related functionality.        |
| Persistence    | Manages database context and persistence operations. |
| Repository     | Handles database access and data retrieval.          |
| Service        | Contains business logic and application services.    |
| Web Service    | Exposes RESTful endpoints and handles HTTP requests. |

## 🔐 Security

- Authentication and authorization.
- Protected API endpoints.
- Role-based access control (if configured).
- Secure API communication.
- Configuration-based secret management.

## ⚙️ Getting Started

### Prerequisites

- Visual Studio
- Compatible .NET SDK / Targeting Pack
- SQL Server (if required)
- NuGet Package Manager
- Entity Framework tools (if required)

### Installation

**1. Clone the repository**

```bash
git clone https://github.com/drashti-shastra-soft/TradeLedger-Web-Service.git
```

**2. Navigate to the project directory**

```bash
cd TradeLedger-Web-Service
```

**3. Open the solution**

Open the solution file in Visual Studio:

```text
TradeLedgerWebService.sln
```

**4. Restore NuGet Packages**

```bash
dotnet restore
```

**5. Configure Application Settings**

Configure the required database connection string, API settings, authentication settings, and other application configurations.

Never commit production passwords, API keys, or JWT secrets to GitHub.

**6. Build the Project**

```bash
dotnet build
```

**7. Run the API**

Select the Web Service project as the startup project in Visual Studio and run the application.

## 🔗 Related Project

### TradeLedger Web Console

The TradeLedger Web Console communicates with this Web Service to provide the user interface for managing and analyzing trading activities.

**GitHub Repository:**

[https://github.com/drashti-shastra-soft/TradeLedger-Web-Console](https://github.com/drashti-shastra-soft/TradeLedger-Web-Console)

## 🎯 Project Objective

The primary objective of TradeLedger Web Service is to provide a centralized, scalable, and maintainable backend solution for trading journal management.

It handles application business logic, trading records, data processing, performance calculations, and API communication to support the complete TradeLedger ecosystem.
