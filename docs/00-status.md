# Status

**Last updated:** 2026-09-07

## Where we are

**Phase D is finished.** Code starts at work item 01.

- **D1 done.** Authority/replica, one-way flow, buffer, on-hand vs reserved, invariant.
- **D2 done.** Actors, authority held per field, commercial and fulfilment loops separated,
  stock changes on physical events, order status derived from its lines.
- **D3 done.** Where the numbers live: counter + ledger together, available is computed,
  single-warehouse assumption, the two forms of the invariant. `01-domain.md` rules 15-20.
- **D4 done.** Stock lives on the variant; SKU vs barcode vs channel code; the listing row as
  the bridge, per-listing push, unmatched order lines. `01-domain.md` rules 21-26.
- **D5 done.** State machine defined (finite states + allowed transitions), `ChannelStatus` vs
  `FulfillmentStatus` split, machine lives on the order line and the order status is derived,
  reservation on arrival, partial shipment out of scope. `01-domain.md` rules 27-33.
- **D6 done.** Webhook carries and polling guarantees; the webhook body is not trusted, only
  its identity; push carries a listing id and reads the number at send time; duplicates are
  stopped by a unique index, ordering by the state machine. `01-domain.md` rules 34-43.
- **D7 done.** Two aggregates (`StockItem` per variant, `Order`); the invariant lives in
  `StockItem` and the aggregate is the unit of locking; order creation is one transaction across
  both, deliberately breaking "one transaction per aggregate"; module = bounded context, tactical
  DDD only where an invariant exists; `Catalog` renamed **`Inventory`** and it knows nothing about
  channels. `01-domain.md` rules 44-51.
- **D8 done.** Two axes of drift (communication vs physical) and channel reconciliation only sees
  one; not every mismatch is drift — an in-flight order is always "channel < us"; the quiet window;
  diagnosis from the three-number signature and a repeated signature is a fault, not drift; order
  reconciliation as the third net under polling; the internal check is a bug alarm; auto-correction
  only where we hold authority over a replica; the net cannot share the mechanism it audits.
  `01-domain.md` rules 52-60.
- **Architecture debt cleared (2026-09-03).** Questions 1-3 re-explained in his own words:
  the vertical-slice/Clean trade-off and the price paid (no compiler-enforced boundary), why
  DDD is selective, why `Domain/` is not sliced. One correction was needed - the invariant had
  been stated backwards ("reserved cannot be *lower* than on-hand"); it is `reserved <= on-hand`.
- **01 done (2026-09-03).** Solution skeleton: 10 projects, 4 modules each with its own
  `.Contracts`, `Shared.Kernel` / `Shared.Infrastructure` / `Shared.Contracts`, and
  `OrderSync.Api` as the only composition root. Modules are registered through `IModule` and
  endpoints discovered through `IEndpoint` per module assembly. `dotnet build` clean, all four
  `/api/<module>/ping` placeholders answer. The open transaction question is decided (see
  `decisions.md`) and wired at 03. Review passed with one correction: the `.Contracts` boundary
  had been justified by "it makes a later microservice split easy", a reason this project does
  not have (the split is out of scope). It exists so that the aggregate's invariant cannot be
  reached from outside its own module - with a direct reference `stockItem.Reserved += n` would
  compile and skip `Reserve()`.
- **02 done (2026-09-04).** `docker-compose.yml` at the repo root: SQL Server 2022 Developer,
  Redis, RabbitMQ (+ management UI), MinIO, Seq. Passwords are inline defaults overridable by a
  `.env` (`.env.example` committed), every service has a real `healthcheck` so 03 can wait on
  `service_healthy`, and Redis deliberately has no volume. Verified from the Windows host, not
  only inside the containers: `sa` login returns `@@VERSION`, Redis answers `+PONG`, the RabbitMQ
  management API accepts the credentials, MinIO `/minio/health/live` is `200`, a CLEF event posted
  to Seq returns `201`. Seq needed `SEQ_FIRSTRUN_NOAUTHENTICATION` - without it the container
  crash-loops. The API is not containerised yet; it runs from Rider against these ports
  (Dockerfile deferred to work item 41). `docs/rider-notlari.md` renamed to `docs/dev-notes.md`
  and given a Docker section.
- **03 done (2026-09-04).** EF Core wired: a `DbContext` per module, each in its own schema
  (`inventory`, `ordering`, `integration`) with its own `__EFMigrationsHistory`; Identity's
  context waits for work item 14. `Shared.Infrastructure/Data/` holds the pieces that make the
  cross-module transaction possible: `DbConnectionAccessor` (one `SqlConnection` per request,
  every context built on it) and `IUnitOfWork` (begins the transaction once, enlists every context with
  `UseTransaction`, saves and commits together) - written not to wrap EF but to supply the one
  thing EF has no answer for, since `SaveChanges` commits a single context. The three `Initial`
  migrations are deliberately empty: they create the schema and the history table, and the
  tables arrive with work items 06-11. In Development a hosted `DatabaseMigrator` creates the
  database and applies pending migrations at startup. Verified from an empty server: database
  and three schemas created, three history rows written, all four `ping` endpoints answering.
  Two things bit on the way - `InvariantGlobalization` (inherited from 01) makes
  `Microsoft.Data.SqlClient` refuse every connection, and EF cannot create a missing database
  when it is handed a connection *object*, so the migrator creates it through master first.
  `appsettings.Development.json` was in `.gitignore`; it is now committed, since without it a
  fresh clone has no connection string. Review removed a `DatabaseSettings` class that wrapped a
  single string - the connection string is now read once in `AddSharedPersistence`. The review
  also produced a standing instruction, see Open debts.
- **04 done (2026-09-07).** Serilog + Seq and global exception handling. One new file
  (`OrderSync.Api/GlobalExceptionHandler.cs`, an `IExceptionHandler`); everything else is
  configuration. Serilog is configured from `appsettings.json` (`ReadFrom.Configuration`), so a
  sink or a silenced namespace is an edit to JSON, not to code; the built-in `Logging` section was
  removed so one job has one tool. The handler logs the exception with its stack trace and returns
  `ProblemDetails` with **no `Detail`** - the caller's only clue is `traceId`, which equals Seq's
  `@TraceId`, so a support ticket carrying the response body finds the full story in one search.
  Middleware order matters and is commented in `Program.cs`: `UseSerilogRequestLogging()` before
  `UseExceptionHandler()`, otherwise the exception escapes to the request logger and is written in
  full twice. Verified end to end against a temporary `/dev/boom` endpoint (since removed):
  `500` + `application/problem+json`, and both events in Seq under the same trace id. Deliberately
  not written: bootstrap logger, `Result`->HTTP mapping (task 12), PII masking.
- **Next: work item 05** — Swagger/Scalar + health checks.

## Open debts

**Less scaffolding (2026-09-05).** His feedback at 03: the volume of infrastructure code makes it
harder, not safer, to learn from - every hole closed with another file is attention taken away from
the thing being taught. From 04 on: the fewest files that make the item work, holes named in prose
instead of closed with code, and review questions about the mechanism rather than a rule to
memorise. He also said this stretch is costing him more effort than the eShop course did.

**Owed re-explanations** (asked once the code exists, out of that work item's own code):

4. What are the two different purposes of rate limiting? Why does retrying on a `429` from a
   marketplace make things worse? (D6 taught the outbound half.) — asked at work item 22.
5. Does this monolith scale horizontally? Which single piece does not? Why is the stock row lock
   not a global bottleneck? — asked at work item 10.
6. Why does `Ordering` referencing `Inventory.Contracts` — and never `Inventory` — matter, when
   both end up in the same process anyway? What breaks the day someone references the module
   project directly? — asked at work item 12.

## Work items
12 / 46 (D1-D8, 01-04)
