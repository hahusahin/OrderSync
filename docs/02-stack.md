# Technology Choices

Every tool once. Solving the same problem with two tools is deliberately avoided.

| Area | Choice | Reason |
|---|---|---|
| Runtime | .NET 10, ASP.NET Core Web API | LTS |
| Architecture | Modular monolith - 4 modules | Inventory, Ordering, Integration, Identity |
| Domain modelling | **Selective tactical DDD** - Inventory + Ordering only | Aggregates only where an invariant exists; other modules are plain CRUD |
| Code layout | **Vertical slice** - `Features/<UseCase>/` inside a module | Changing one use case means opening one folder. `Domain/` is shared, never sliced |
| CQRS | MediatR | Most common name in Turkish job posts |
| Data access | EF Core (writes) + Dapper (reports) | SQL practice comes from the reporting side |
| Database | SQL Server (Docker) | Standard in the Turkish .NET market; Developer Edition is free |
| Cache | Redis - StackExchange.Redis | Stock read cache: list and detail screens |
| Messaging | MassTransit + RabbitMQ | Outbox, retry and dead-letter included |
| Scheduled jobs | Hangfire | Dashboard is worth the demo value |
| Resilience | Polly | Retry, exponential backoff, circuit breaker |
| Rate limiting | ASP.NET Core built-in (inbound) + Polly `429`/`Retry-After` (outbound) | No new package; two different purposes |
| Identity | ASP.NET Core Identity + JWT | Role + policy authorization |
| File storage | MinIO (S3-compatible), **behind `IFileStorage`** | Bulk upload + product images; what is learned is the S3 API. The interface keeps the Azure Blob door open |
| Observability | Serilog + Seq | Cheap, visible in a demo |
| Packaging | Docker Compose | - |
| Frontend | Next.js + TypeScript | Written entirely by Claude |
| AI phase | Python + FastAPI + LangGraph | Separate service, after the core is done |

## Repository layout (the template for work item 01)

Single repository. Backend and frontend are separate build artifacts, not separate repos:
one `docker compose up` has to bring the whole thing up, and an API contract change touches
both sides in one commit.

```
/
  docker-compose.yml
  web/                            <- Next.js frontend
  src/
    OrderSync.Api/                <- host: composition root, middleware, module registration
    Shared.Kernel/                <- no business meaning: Entity, AggregateRoot, IDomainEvent, Result
    Shared.Infrastructure/        <- cross-cutting: Serilog, MediatR behaviours, auth, IFileStorage
    Shared.Contracts/             <- integration events (publisher and consumer both need the type)
    Modules/
      Ordering/
        Ordering.Contracts/       <- the module's public synchronous surface; the only thing others reference
        Ordering/
          Domain/                 <- aggregate: guardian of the rules, shared, not owned by a feature
            Order.cs
            OrderStatus.cs
          Features/
            CreateOrder/          <- command + handler + validator + endpoint, one folder
            CancelOrder/
            GetOrderDetail/
          Data/
            OrderingDbContext.cs
            Configurations/
            Migrations/
      Inventory/  Integration/  Identity/
```

Rules:
- **The application layer is sliced vertically; `Domain/` and `Data/` are not.**
- **A module references another module only through its `.Contracts` project** - the one module
  boundary the compiler enforces.
- **A business concept never enters `Shared.*`.** Shared holds things with no business meaning;
  the moment a domain term moves there it becomes a fifth module and the boundaries dissolve.
- One SQL Server database, **one schema and one `DbContext` per module** (`ordering.Orders`,
  `inventory.StockItems`). A single database because order creation writes `Order` and
  `StockItem` in one transaction across two modules (rule 46).

## Explicitly out

multi-tenancy - Kubernetes - OpenTelemetry + Grafana - ADR discipline - microservice split -
gRPC - OAuth provider - permission-management screens - CI/CD and lint setup - PR/branch
ritual - a broad test suite - solving one problem with two tools -
**Clean/Onion layer projects** (separate `Application` / `Domain` / `Infrastructure` csproj,
a competing solution to vertical slice, we do not run both). What is dropped is the layer
*projects*, not the dependency rule: `Domain/` still references no infrastructure, and nothing
inside it knows about EF Core, HTTP or the message bus. Vertical slice has no quarrel with that
rule - only with laying the folders out by technical type. Here the rule is kept by discipline,
not by the compiler.
