# 🛒 EShop Clean Architecture

[![.NET CI](https://github.com/kiez2005/eshop-clean-architecture/actions/workflows/ci.yml/badge.svg)](https://github.com/kiez2005/eshop-clean-architecture/actions/workflows/ci.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20%7C%20DDD%20%7C%20CQRS-blue)
![License](https://img.shields.io/badge/License-MIT-green)

A production-style e-commerce backend API built with **.NET 8** following **Clean Architecture**, **Domain-Driven Design (DDD)**, and **CQRS** principles. Designed with strict layer boundaries, transactional outbox messaging, distributed caching, and automated architecture/unit testing.

## 📋 Overview

This project implements a modular e-commerce backend API covering product catalog management, shopping basket workflows, order checkout with domain events, and role/permission-based identity management. See [docs/DDD.md](docs/DDD.md) for detailed domain rules and invariants.

## ✨ Key Features

- **Clean Architecture & Tactical DDD** - Rich domain aggregates (`Product`, `Basket`, `Order`, `Category`), Value Objects (`Phone`, `Address`, `UserInformation`), Domain Services (`CheckoutService`), and Result/Error pattern
- **CQRS with MediatR Pipelines** - Segregated Commands and Queries with `ValidationPipelineBehavior`, `TransactionPipelineBehavior`, and `LoggingPipelineBehavior`
- **Transactional Outbox Pattern** - Domain events are serialized into `OutboxMessages` within the same EF Core transaction and processed reliably in the background via **Hangfire**
- **Authentication & Permission-Based Authorization** - ASP.NET Core Identity with JWT Bearer tokens, refresh token rotation, and dynamic permission claims caching
- **Specification Pattern** - Composable query specifications using `Ardalis.Specification` with pagination, filtering, and search
- **Distributed Caching & Real-Time Notifications** - Local memory / Redis caching abstraction and SignalR notification hub
- **Automated Testing Suite** - Unit tests (`xUnit`, `FluentAssertions`, `NSubstitute`) and layer boundary enforcement tests (`NetArchTest`)

## 🛠 Tech Stack

| Category | Technologies |
|----------|-------------|
| **Framework** | .NET 8, ASP.NET Core Web API |
| **Architecture** | Clean Architecture, DDD, CQRS, Outbox Pattern, Specification Pattern |
| **ORM & Data Access** | Entity Framework Core 8, Dapper, Ardalis.Specification |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Core Identity, JWT Bearer |
| **Validation & Mapping** | FluentValidation, Mapster |
| **Messaging & Jobs** | MediatR 12, Hangfire, SignalR |
| **Caching** | Redis (`StackExchange.Redis`), `IMemoryCache` |
| **Email & Templates** | MailKit, MimeKit, RazorEngineCore |
| **Logging & Docs** | Serilog (Console & File sinks), Swagger / OpenAPI (API Versioning V1) |
| **Testing & CI** | xUnit, FluentAssertions, NSubstitute, NetArchTest, GitHub Actions, Docker |

## 📁 Solution Structure

```text
CleanArchitecture.sln
├── src/
│   ├── Core/
│   │   ├── CleanArchitecture.Domain           # Aggregates, Entities, Value Objects, Domain Events, Domain Services
│   │   └── CleanArchitecture.Application      # CQRS Commands/Queries, Handlers, Validators, Pipeline Behaviors
│   │
│   ├── Infrastructure/
│   │   ├── CleanArchitecture.Persistence      # EF Core DbContext, Repositories, Outbox Pattern, Migrations
│   │   ├── CleanArchitecture.Identity         # ASP.NET Core Identity, JWT Auth, Permission Authorization
│   │   └── CleanArchitecture.Infrastructure   # Caching (Redis/Memory), SMTP Email, Hangfire, SignalR, File Storage
│   │
│   └── API/
│       └── CleanArchitecture.Api              # REST Controllers (V1), Exception Middleware, Swagger, Configurations
│
└── tests/
    ├── CleanArchitecture.Application.UnitTests   # Domain & Application Handler Unit Tests
    └── CleanArchitecture.ArchitectureTests       # NetArchTest Layer Dependency & Convention Tests
```

## 🏗 Architecture Diagram

```text
┌─────────────────────────────────────────────────────────────────┐
│                         API Layer                               │
│              (Controllers, Middlewares, Swagger)                │
├─────────────────────────────────────────────────────────────────┤
│                     Application Layer                           │
│     (Commands, Queries, Handlers, Validators, Behaviors)        │
├─────────────────────────────────────────────────────────────────┤
│                       Domain Layer                              │
│   (Aggregates, Entities, Value Objects, Domain Events/Services) │
├─────────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐   │
│  │  Persistence │  │   Identity   │  │    Infrastructure    │   │
│  │(EF Core/Outbox)│ │  (JWT/RBAC)  │  │ (Email, Cache, Jobs) │   │
│  └──────────────┘  └──────────────┘  └──────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## 🚀 Getting Started

### Option 1: Run with Docker Compose (Recommended)

Spin up SQL Server, Redis, and the API with a single command:

```bash
docker compose up -d --build
```

Once running, access:
- **Swagger UI**: `http://localhost:8080/swagger`
- **Hangfire Dashboard**: `http://localhost:8080/jobs`

### Option 2: Run Locally with .NET CLI

#### 1. Clone the repository

```bash
git clone https://github.com/kiez2005/eshop-clean-architecture.git
cd eshop-clean-architecture
```

#### 2. Configure Database Connection

Update `src/API/CleanArchitecture.Api/Configurations/database.json` with your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=localhost,1433;Initial Catalog=EShopDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=True"
  }
}
```

#### 3. Restore, Build, and Run

```bash
dotnet restore
dotnet build
dotnet run --project src/API/CleanArchitecture.Api
```

- **Swagger UI**: `https://localhost:7047/swagger` (or `http://localhost:5016/swagger`)
- **Hangfire Dashboard**: `https://localhost:7047/jobs`

> **Note:** Pending EF Core migrations and default admin/role seeding are applied automatically on startup.

## ⚙️ Configuration Files

Modular configuration files are located in `src/API/CleanArchitecture.Api/Configurations/`:

| File | Description |
|------|-------------|
| `database.json` | SQL Server connection string |
| `security.json` | JWT token settings & default Admin user seed |
| `cache.json` | In-memory / Redis distributed cache settings |
| `mail.json` | SMTP email settings |
| `hangfire.json` | Hangfire dashboard route & basic auth credentials |
| `outbox.json` | Transactional Outbox polling interval & batch size |
| `logger.json` | Serilog structured logging configuration |
| `signalr.json` | SignalR real-time hub & backplane configuration |

## 🧪 Running Tests

Run all Unit Tests and Architecture Tests:

```bash
dotnet test
```

Run individual test suites:

```bash
# Domain & Application Unit Tests
dotnet test tests/CleanArchitecture.Application.UnitTests

# Clean Architecture Layer & Naming Convention Tests
dotnet test tests/CleanArchitecture.ArchitectureTests
```

## 📄 License

This project is licensed under the [MIT License](LICENSE).
