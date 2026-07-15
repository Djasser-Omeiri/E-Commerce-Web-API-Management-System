# E-Commerce Web API Management System

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?logo=nuget&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-black?logo=jsonwebtokens)
![License](https://img.shields.io/badge/License-MIT-green)

A RESTful e-commerce backend built with **ASP.NET Core 8** and **Entity Framework Core**. It provides everything a storefront needs on the server side: product & category catalogs with stock tracking, order creation and lifecycle management, customer reviews, and JWT-based authentication with role-based access control (Admin / User).

The project follows a clean, layered architecture — **Controllers → Services → Repositories (Unit of Work) → EF Core** — with DTOs used at every API boundary to keep the domain models decoupled from what's exposed over HTTP.

## Features

- **Authentication & Authorization** — Register/login with ASP.NET Core Identity, JWT access tokens, refresh token rotation, and logout (token revocation). Role-based access (`Admin`, `User`) via `[Authorize(Roles = "Admin")]`.
- **Product catalog** — Public browsing (list/get by ID); Admin-only create, update, and delete. Each product tracks its own stock quantity.
- **Categories** — Public browsing; Admin-only create, update, and delete, with a product count per category.
- **Orders** — Customers can place orders (with line items), view their own order history, and update their shipping address. Admins can view all orders and update order status (`Pending → Processing → Shipped → Delivered → Cancelled`).
- **Reviews** — Customers can leave, edit, and delete reviews on products (rating + comment); reviews can be listed per product or per user, with ownership checks enforced.
- **Cross-cutting concerns** — Structured logging (`ILogger`) on every controller action, rate limiting on auth endpoints (5 requests/minute per IP), and Swagger/OpenAPI docs with built-in JWT bearer support.

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 (Web API) |
| ORM | Entity Framework Core 8 (SQL Server provider) |
| Auth | ASP.NET Core Identity + JWT Bearer tokens |
| API Docs | Swashbuckle (Swagger / OpenAPI) |
| Database | Microsoft SQL Server |

## Project Structure

```
E-Commerce-Web-API/
├── Controllers/        # API endpoints (Account, Category, Product, Order, Review)
├── DTOs/                # Request/response contracts, grouped by resource
├── Data/
│   ├── AppDbContext.cs  # EF Core DbContext
│   ├── Configurations/  # Fluent API entity configurations
│   └── DbSeeder.cs      # Seeds the Admin/User roles on startup
├── Enums/               # eOrderStatus
├── Interfaces/
│   ├── Repositories/    # Repository contracts
│   └── Services/        # Service contracts
├── Migrations/          # EF Core migrations
├── Models/               # Domain entities (Product, Category, Order, OrderItem, Review, Stock, User)
├── Repositories/        # Data-access implementations + UnitOfWork
├── Services/            # Business logic implementations
└── Program.cs           # App startup & DI configuration
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, a full instance, or a Docker container)

### 1. Clone the repository

```bash
git clone https://github.com/Djasser-Omeiri/E-Commerce-Web-API-Management-System.git
cd E-Commerce-Web-API-Management-System/E-Commerce-Web-API
```

### 2. Configure the database connection

Update the connection string in `E-Commerce-Web-API/appsettings.json`:

```json
"ConnectionStrings": {
  "cs": "Data Source=.;Initial Catalog=ITIDB_API;Integrated Security=True;TrustServerCertificate=True"
}
```

### 3. Configure JWT settings

Also in `appsettings.json`, set your own secret key (32+ characters) before deploying anywhere beyond your local machine:

```json
"JWT": {
  "SecretKey": "YourSuperSecretAndVeryLongKeyHere123!",
  "Issuer": "E_Commerce_Web_API",
  "Audience": "E_Commerce_Clients"
}
```

> **Note:** For any real deployment, move the `SecretKey` and connection string out of `appsettings.json` and into [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets), environment variables, or a secrets manager rather than committing them to source control.

### 4. Apply EF Core migrations

```bash
cd E-Commerce-Web-API
dotnet tool install --global dotnet-ef   # if you don't already have it
dotnet ef database update
```

This creates the database schema and, on first run, `DbSeeder` will seed the `Admin` and `User` roles automatically.

### 5. Run the API

```bash
dotnet run
```

By default the API is available at `http://localhost:5084` (HTTP) and `https://localhost:7091` (HTTPS), and will launch Swagger UI at `/swagger` automatically in Development.

## Authentication

1. **Register** — `POST /api/account/register`
2. **Login** — `POST /api/account/login` to receive a JWT access token and a refresh token
3. **Authorize requests** — pass the access token as `Authorization: Bearer <token>` on protected endpoints
4. **Refresh** — `POST /api/account/refresh` with the username and refresh token to get a new token pair once the access token expires
5. **Logout** — `POST /api/account/logout` to revoke the stored refresh token

New accounts are assigned the `User` role by default. Admin-only actions (creating/editing products, categories, and updating order status) require a user with the `Admin` role.

## API Endpoints

### Account (`/api/account`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/register` | Public | Create a new account |
| POST | `/login` | Public (rate-limited) | Get access + refresh tokens |
| POST | `/refresh` | Public (rate-limited) | Exchange a refresh token for a new token pair |
| POST | `/logout` | Authenticated | Revoke the current refresh token |
| GET | `/profile` | Authenticated | Get the current user's username and ID |

### Products (`/api/product`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | Public | List all products |
| GET | `/{id}` | Public | Get a product by ID |
| POST | `/` | Admin | Create a product (with initial stock) |
| PUT | `/{id}` | Admin | Update a product |
| DELETE | `/{id}` | Admin | Delete a product |

### Categories (`/api/category`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | Public | List all categories |
| GET | `/{id}` | Public | Get a category by ID |
| POST | `/` | Admin | Create a category |
| PUT | `/{id}` | Admin | Update a category |
| DELETE | `/{id}` | Admin | Delete a category |

### Orders (`/api/order`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | Authenticated | List orders (own orders, or all orders for Admins) |
| GET | `/{id}` | Authenticated (owner or Admin) | Get an order by ID |
| POST | `/` | Authenticated | Place a new order |
| PUT | `/{id}/address` | Authenticated (owner or Admin) | Update the shipping address |
| PUT | `/{id}/status` | Admin | Update the order status |
| DELETE | `/{id}` | Authenticated (owner or Admin) | Delete an order |

### Reviews (`/api/review`)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/` | Authenticated | List reviews (own reviews, or all for Admins) |
| GET | `/{id}` | Authenticated (owner or Admin) | Get a review by ID |
| GET | `/product/{productId}` | Authenticated | List reviews for a specific product |
| POST | `/` | Authenticated | Create a review |
| PUT | `/{id}` | Authenticated (owner or Admin) | Update a review |
| DELETE | `/{id}` | Authenticated (owner or Admin) | Delete a review |

Full request/response schemas are available in the interactive Swagger UI once the API is running, at `/swagger`.

## Data Model

- **Category** `1 ── *` **Product** `1 ── 1` **Stock**
- **Product** `1 ── *` **Review** `* ── 1` **User**
- **User** `1 ── *` **Order** `1 ── *` **OrderItem** `* ── 1` **Product**

Order status is tracked via the `eOrderStatus` enum: `Pending`, `Processing`, `Shipped`, `Delivered`, `Cancelled`.

## Architecture & Design Decisions

This project isn't just wired to work — several patterns were chosen deliberately for query performance, data consistency, and security. This section documents the *why*, not just the *what*, for anyone reviewing or extending the codebase.

### Summary

| Concern | Technique | Applied in |
|---|---|---|
| Query performance | Deferred `IQueryable<T>` composition | `ProductRepository`, `CategoryRepository`, `OrderRepository`, `ReviewRepository` |
| Query performance | `AsNoTracking()` on all reads | Every repository read method |
| Query performance | Server-side projection (`Select` → DTO) | All list-returning service methods, `OrderItemRepository`, `StockRepository` |
| Query performance | Eager loading (`Include` / `ThenInclude`) | `GetOrderByIdAsync`, `GetProductByIdAsync`, `GetReviewByIdAsync` |
| Data consistency | Unit of Work / single `SaveChangesAsync()` | `UnitOfWork.CompleteAsync()` |
| Security | Ownership-based authorization | `ClaimsPrincipal.CanAccess()` extension |
| Security | Refresh token rotation | `AccountController` + `UserManager` token store |
| Security | Fixed-window rate limiting | `/api/account/login`, `/api/account/refresh` |

---

### 1. Query Performance

**Deferred execution with `IQueryable<T>`.** List-style repository methods don't return a materialized list — they return an unexecuted `IQueryable<T>`. The service layer then appends its own `.Where()` filters and a `.Select()` projection *before* the query ever runs:

```csharp
// Repository layer — builds the query, does NOT execute it
public async Task<IQueryable<Product>> GetProductsAsync()
    => _context.Products.AsNoTracking();

// Service layer — filtering + projection are appended to the same
// expression tree, and EF Core compiles everything into ONE SQL
// statement the moment ToListAsync() is called
var products = await _unitOfWork.Products.GetProductsAsync();
return await products
    .Select(p => new ProductDTO
    {
        Name = p.Name,
        Price = p.Price,
        CategoryName = p.Category.Name,
        IsAvailable = p.Stock.Quantity > 0
    })
    .ToListAsync();
```

Because the repository never calls `.ToList()`/`.ToListAsync()` itself, the database only executes a single, fully-composed query with just the required columns and `WHERE` clauses — instead of the repository fetching every row into memory and the service filtering/mapping it in C#.

**`AsNoTracking()` on every read.** GET endpoints never mutate the entities they fetch, so all read queries opt out of EF Core's change tracker, cutting memory allocation and CPU overhead on the request path that runs most often.

**Server-side projection straight to DTOs.** `.Select()` is used to shape the response *inside* the query (not after materialization), so SQL Server returns only the columns each DTO actually needs rather than full entity rows.

**Eager loading to prevent N+1 queries.** Single-item lookups that need related data use `.Include()` / `.ThenInclude()` up front — e.g. `OrderRepository.GetOrderByIdAsync` includes `User` and `OrderItems.Product` in one round trip — instead of triggering lazy-load queries per related entity.

### 2. Data Consistency

**Unit of Work pattern.** Every repository shares a single `AppDbContext` instance through `IUnitOfWork`. Multi-step writes — like placing an order, which decrements `Stock` *and* inserts `OrderItem` rows — are committed with one `SaveChangesAsync()` call (`CompleteAsync()`), so the whole operation succeeds or fails atomically rather than leaving the database in a partially-updated state.

### 3. Security

**Ownership-based authorization.** A `ClaimsPrincipal.CanAccess(resourceOwnerId)` extension method centralizes the "Admins can access anything; regular users can only access their own resources" rule in one place, instead of re-implementing the same role/ID check in the `Order`, `Review`, and `Account` controllers.

**Refresh token rotation.** Access tokens are short-lived (30 minutes). A cryptographically random refresh token is issued alongside each access token and stored server-side via ASP.NET Identity's token store; `/refresh` validates it before issuing a new pair, and `/logout` revokes it — so a leaked access token has a limited blast radius.

**Fixed-window rate limiting.** `/login` and `/refresh` are capped at 5 requests per minute per IP (`AddRateLimiter` in `Program.cs`), which slows down brute-force credential-stuffing attempts against the auth endpoints specifically, without throttling the rest of the API.

## License

This project is licensed under the [MIT License](LICENSE).
