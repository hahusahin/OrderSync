# Roadmap — 46 work items

Progress is measured in work items, not hours.

## Phase D — domain (no code, conversation only)
- [x] D1 The problem: why this software exists, who pays, which pain it removes
- [x] D2 Actors and the life of an order
- [x] D3 Inventory reality: on-hand / reserved / available, what "single source of truth" means
- [x] D4 Product, SKU, variant and channel mapping
- [x] D5 Order lifecycle and state machine
- [x] D6 Physics of integration: polling vs webhook, rate limits, duplicate messages, ordering
- [x] D7 DDD map: bounded contexts, aggregate boundaries, where the invariant lives
- [x] D8 Reconciliation: why drift happens, how it is found, how it is fixed

## Phase 0 — skeleton
- [x] 01 Solution structure and modular monolith skeleton — vertical slice layout, `Shared.Kernel` /
      `Shared.Infrastructure` / `Shared.Contracts`, per-module `.Contracts` project, `Data/` per
      module (Claude)
- [x] 02 Docker Compose: SQL Server, Redis, RabbitMQ, MinIO, Seq (Claude)
- [x] 03 EF Core setup and first migration (Claude)
- [x] 04 Serilog + Seq, global exception handling (Claude)
- [x] 05 Swagger/Scalar + health checks (Claude)

## Phase 1 — core domain: inventory and orders
- [x] 06 Product and variant model
- [ ] 07 Inventory model: on-hand / reserved / available **(Huseyin)**
- [ ] 08 Stock movement ledger **(Huseyin)**
- [ ] 09 Reservation rules — domain logic **(Huseyin)**
- [ ] 10 Oversell protection: row-level concurrency on the stock row — pessimistic lock or rowversion **(Huseyin)**
- [ ] 11 Order model and state machine **(Huseyin)**
- [ ] 12 Create-order command (MediatR)
- [ ] 13 Unit tests: reservation + oversell **(Huseyin)**

## Phase 2 — identity and authorization
- [ ] 14 ASP.NET Core Identity setup
- [ ] 15 JWT + refresh token flow
- [ ] 16 Role model + policy authorization **(Huseyin)**
- [ ] 17 Protecting endpoints, authorization tests + **inbound rate limit** (built-in) **(Huseyin)**

## Phase 3 — integration (the heart of the project)
- [ ] 18 Channel model and configuration
- [ ] 19 Marketplace simulator — contract-faithful stub; injects `429` / timeout / duplicate webhook on demand (Claude)
- [ ] 20 Order pulling (polling) worker — Hangfire
- [ ] 21 Webhook receiver + idempotency **(Huseyin)**
- [ ] 22 Polly: retry, backoff, circuit breaker + **`429` / `Retry-After`** (outbound) **(Huseyin)**
- [ ] 23 MassTransit + RabbitMQ setup (Claude)
- [ ] 24 Publishing order events through the outbox
- [ ] 25 Stock write-back (push) consumer **(Huseyin)**
- [ ] 26 Integration log and error record model

## Phase 4 — reconciliation, reporting and bulk operations
- [ ] 27 Nightly reconciliation job **(Huseyin)**
- [ ] 28 Raw SQL reports: sales per channel, stock ageing **(Huseyin)**
- [ ] 29 Integration health panel data
- [ ] 30 Two integration tests with Testcontainers **(Huseyin)**
- [ ] 31 MinIO setup and file upload service — behind `IFileStorage` (Claude)
- [ ] 32 Bulk Excel/CSV product-stock upload: chunked processing, progress, partial error report **(Huseyin)**
- [ ] 33 Storing product images and pushing them to channels

## Phase 5 — frontend (all Claude)
- [ ] 34 Login and authorization flow
- [ ] 35 Order list and detail (includes the deep link to the channel's own order page)
- [ ] 36 Inventory management screen
- [ ] 37 Channel settings
- [ ] 38 Integration health panel

## Phase 6 — real world and demo (the core ends here)
- [ ] 39 TCMB exchange-rate service — a real external API integration
- [ ] 40 README, architecture diagram, 3-minute demo scenario **(Huseyin)**
- [ ] 41 Seed data — one command to a running system (Claude)

## Phase 7 — AI layer (not started before the core is done)
- [ ] 42 Python / FastAPI service skeleton
- [ ] 43 Product matching: embeddings + vector search **(Huseyin)**
- [ ] 44 Integration error triage agent (LangGraph) **(Huseyin)**
- [ ] 45 Querying operations in natural language — text-to-SQL **(Huseyin)**
- [ ] 46 .NET ↔ Python bridge and demo
