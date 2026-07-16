# SnapCart Distributed — Microservices E-commerce Platform

![Architecture Diagram](./architecture.svg)

> **Diagram key:** Solid coloured nodes are **implemented today**. Faded/dashed nodes are **planned (roadmap)**.
> Regenerate the diagram source: `dot -Tsvg architecture.dot -o architecture.svg` or `rsvg-convert -w 1400 -h 840 architecture.svg -o architecture.png`

---

## Overview

**SnapCart Distributed** is a cloud-native e-commerce platform built with microservices architecture and orchestrated by **.NET Aspire**. It demonstrates enterprise-grade distributed system design patterns — database-per-service, Redis distributed caching, FastEndpoints REST APIs, EF Core with migrations, and OpenTelemetry observability — all wired together through the Aspire app model.

---

## Architecture

### Implemented today

| Layer | Component | Technology |
|-------|-----------|-----------|
| Orchestration | **AppHost** | .NET Aspire |
| Shared config | **ServiceDefaults** | OpenTelemetry · Health checks · Service discovery · Resilience |
| Microservice | **Catalog** | FastEndpoints · EF Core · Npgsql · .NET 10 |
| Microservice | **Cart** | FastEndpoints · StackExchange.Redis · .NET 10 |
| Database | **PostgreSQL** | `catalogdb` · persistent data volume · pgAdmin |
| Cache | **Redis** | `cache` connection · RedisInsight · persistent data volume |

### Planned (roadmap)

| Component | Purpose |
|-----------|---------|
| Blazor WebApp | Frontend — Blazor WebAssembly |
| Keycloak | OIDC / JWT authentication |
| RabbitMQ | Async event bus between services |
| Order Service | Order processing workflow |
| Payment Gateway | Payment integration |
| Notification Service | Email / push notifications |

---

## Technology Stack

| Component | Technology | Status |
|-----------|-----------|--------|
| Framework | .NET 10.0 | ✅ Implemented |
| Orchestration | .NET Aspire | ✅ Implemented |
| REST API | FastEndpoints v6 | ✅ Implemented |
| Database | PostgreSQL (EF Core / Npgsql) | ✅ Implemented |
| Cache | Redis (StackExchange.Redis) | ✅ Implemented |
| Observability | OpenTelemetry (traces · metrics · logs) | ✅ via ServiceDefaults |
| Frontend | Blazor WebAssembly | Planned |
| Message Broker | RabbitMQ | Planned |
| Authentication | Keycloak (OIDC) | Planned |
| Inter-service | HTTP / gRPC | Planned |

---

## Project Structure

```
Snapcart-Distributed/
├── AppHost/                        # .NET Aspire orchestration host
│   ├── AppHost.cs                  # Registers postgres, cache, catalog, cart
│   ├── AppHost.csproj
│   ├── appsettings.json
│   └── aspire.config.json
│
├── Catalog/                        # Catalog microservice ✅
│   ├── Program.cs
│   ├── Catalog.csproj
│   ├── Models/
│   │   └── Product.cs
│   ├── Endpoints/
│   │   ├── GetProductsEndpoint.cs      # GET  /products
│   │   ├── GetProductByIdEndpoint.cs   # GET  /products/{id}
│   │   ├── CreateProductEndpoint.cs    # POST /products
│   │   ├── UpdateProductEndpoint.cs    # PUT  /products/{id}
│   │   └── DeleteProductEndpoint.cs    # DELETE /products/{id}
│   ├── Services/
│   │   └── ProductService.cs
│   ├── Data/
│   │   └── ProductDbContext.cs
│   ├── Migrations/
│   ├── appsettings.json
│   └── Catalog.http
│
├── Cart/                           # Cart microservice ✅
│   ├── Program.cs
│   ├── Cart.csproj
│   ├── Models/
│   │   ├── ShoppingCart.cs
│   │   └── ShoppingCartItem.cs
│   ├── Endpoints/
│   │   ├── GetCartEndpoint.cs          # GET    /cart/{userName}
│   │   ├── UpdateCartEndpoint.cs       # POST   /cart
│   │   └── DeleteCartEndpoint.cs       # DELETE /cart/{userId}
│   ├── Services/
│   │   └── CartService.cs
│   ├── appsettings.json
│   └── Cart.http
│
├── ServiceDefaults/                # Shared cross-cutting concerns ✅
│   ├── Extensions.cs
│   └── ServiceDefaults.csproj
│
├── architecture.dot                # Graphviz diagram source
├── architecture.svg                # Architecture diagram (SVG — shown above)
├── architecture.png                # Architecture diagram (PNG)
├── Snapcart-Distributed.sln
└── README.md
```

---

## Getting Started

### Prerequisites

- **.NET 10.0 SDK**
- **Docker Desktop** (Aspire spins up PostgreSQL and Redis containers automatically)
- **Git**

> You do **not** need a separate `docker-compose.yml`. .NET Aspire orchestrates PostgreSQL and Redis containers directly through the AppHost.

### Clone and build

```bash
git clone https://github.com/GitItKrishna/SnapCart.git
cd SnapCart/Snapcart-Distributed
dotnet restore
dotnet build
```

### Run via .NET Aspire (recommended)

```bash
cd AppHost
dotnet run
```

The **Aspire Dashboard** opens automatically (port varies, typically `https://localhost:17XXX`). From there you can:
- See all running resources (catalog, cart, postgres, cache, pgAdmin, RedisInsight)
- Click each service to get its live URL
- View distributed traces, logs, and metrics across services

### Run individual services (requires backing services running)

```bash
# Catalog
cd Catalog && dotnet run
# Available at: https://localhost:7249  |  http://localhost:5018

# Cart
cd Cart && dotnet run
# Available at: https://localhost:7119  |  http://localhost:5256
```

> When running individually, connection strings must be set manually in `appsettings.Development.json` since Aspire is not injecting them.

---

## API Endpoints

### Catalog Service — `https://localhost:7249`

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/products` | Get all products |
| `GET` | `/products/{id}` | Get product by ID |
| `POST` | `/products` | Create a product |
| `PUT` | `/products/{id}` | Update a product |
| `DELETE` | `/products/{id}` | Delete a product |

**Example — Create product (POST /products):**
```json
{
  "name": "Mechanical Keyboard",
  "description": "RGB, TKL layout",
  "price": 79.99,
  "imageUrl": "https://example.com/kb.png"
}
```

**Example — Update product (PUT /products/1):**
```json
{
  "name": "Mechanical Keyboard Pro",
  "description": "RGB, full layout",
  "price": 99.99,
  "imageUrl": "https://example.com/kb-pro.png"
}
```

> All endpoints use `[AllowAnonymous]` during development. JWT Bearer auth via Keycloak is on the roadmap.

### Cart Service — `https://localhost:7119`

| Method | Route | Description | Response |
|--------|-------|-------------|----------|
| `GET` | `/cart/{userName}` | Get cart for a user | `200` cart or `404` |
| `POST` | `/cart` | Create or update a cart (upsert) | `200` cart |
| `DELETE` | `/cart/{userId}` | Clear a user's cart | `204 No Content` |

**Example — Upsert cart (POST /cart):**
```json
{
  "userName": "alice",
  "items": [
    { "productId": 1, "productName": "Keyboard", "color": "Black", "quantity": 2, "price": 79.99 },
    { "productId": 2, "productName": "Mouse", "color": "White", "quantity": 1, "price": 29.99 }
  ]
}
```

> Cart is backed by Redis. The `userName` field is the Redis cache key — use the same value across GET / POST / DELETE.

---

## Data Flow (current implementation)

```
Postman / Browser
       │
       ▼
  FastEndpoints                 PostgreSQL
  (Catalog API) ─── EF Core ──▶ catalogdb
       │
  FastEndpoints                   Redis
  (Cart API)    ─── cache ──────▶ (connectionName: cache)
       │
  .NET Aspire Dashboard
  (OpenTelemetry traces · health checks · logs)
```

---

## AppHost Wiring

`AppHost/AppHost.cs` is the single source of truth for how services connect:

```csharp
var postgres    = builder.AddPostgres("postgres").WithPgAdmin().WithDataVolume();
var catalogDb   = postgres.AddDatabase("catalogdb");

var redisCache  = builder.AddRedis("cache").WithRedisInsight().WithDataVolume();

builder.AddProject<Projects.Catalog>("catalog")
       .WithReference(catalogDb)
       .WaitFor(catalogDb);

builder.AddProject<Projects.Cart>("cart")
       .WithReference(redisCache)
       .WithReference(catalog)      // cart can call catalog service by name
       .WaitFor(redisCache);
```

Aspire injects connection strings at runtime:
- Catalog receives `ConnectionStrings:catalogdb`
- Cart receives `ConnectionStrings:cache`

---

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| `No endpoints specified … 'ConnectionStrings:cache'` | Cart running standalone without Aspire | Run via AppHost so Aspire injects the connection string |
| `404` on any endpoint | Cookie auth challenge redirecting to `/Account/Login` | All endpoints have `[AllowAnonymous]` — if missing, add it |
| Cart `GET` returns `404` after a `POST` | `userName` in POST body doesn't match `{userName}` in GET path | Use the exact same value (e.g. `alice` in both) |
| Docker container won't start | Docker Desktop not running | Start Docker Desktop before running AppHost |
| `WaitFor` timeout | Postgres / Redis container slow to initialise | Wait; Aspire retries automatically with health checks |

---

## Design Patterns

| Pattern | Where applied |
|---------|--------------|
| **Database per Service** | Catalog owns PostgreSQL; Cart owns Redis |
| **Cache-Aside** | `CartService` reads/writes Redis via `IDistributedCache` |
| **Health Checks** | `MapDefaultEndpoints()` exposes `/health` and `/alive` |
| **Distributed Tracing** | OpenTelemetry via `AddServiceDefaults()` |
| **Saga / Event-Driven** | Planned — RabbitMQ on roadmap |
| **API Gateway** | Planned — YARP on roadmap |

---

## Roadmap

- [x] Catalog microservice (FastEndpoints + EF Core + PostgreSQL)
- [x] Cart microservice (FastEndpoints + Redis distributed cache)
- [x] .NET Aspire orchestration (AppHost + ServiceDefaults)
- [x] Architecture diagram with technology icons
- [ ] Blazor WebAssembly frontend
- [ ] Keycloak OIDC authentication + JWT Bearer on all endpoints
- [ ] RabbitMQ event bus (MassTransit)
- [ ] Order processing service
- [ ] Payment gateway integration
- [ ] Notification service
- [ ] API Gateway (YARP)
- [ ] Kubernetes deployment
- [ ] Comprehensive integration tests (Testcontainers)

---

## Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Commit your changes: `git commit -m 'Add your feature'`
3. Push to the branch: `git push origin feature/your-feature`
4. Submit a pull request

## License

This project is licensed under the MIT License.

## Author

**GitHub**: [@GitItKrishna](https://github.com/GitItKrishna)

---

*Last updated: July 2026*
