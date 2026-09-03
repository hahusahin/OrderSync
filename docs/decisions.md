# Decisions

Short decision log. Not ADRs - one file, newest first. Current decisions only: when one
changes, the line is edited, not appended to.

Scope: decisions that do **not** become domain rules - stack choices, working setup, what was
left out. Anything that becomes a rule lives only in `01-domain.md`; do not restate it here.

## 2026-09-03 - Two DbContexts, one transaction: a shared connection, not a distributed transaction
Order creation writes `Order` and `StockItem` in one transaction (rule 46), but each module owns
its own `DbContext`. The two contexts are built on the **same open `DbConnection`** for the
lifetime of the request; the transaction is begun once on that connection and each context is
enlisted with `Database.UseTransaction(...)`. One connection means one SQL Server transaction -
no `TransactionScope`, no MSDTC, no second commit that can fail on its own. The price: the
connection is a scoped, shared object, so a handler that opens work on a background thread would
be sharing a connection that cannot be used concurrently. Wired up in work item 03.

## 2026-09-02 - Orders come in two ways: webhook carries, polling guarantees (D6)
A lost webhook has no signal - it looks like "no errors" - so polling is never switched off; it
becomes a long-interval safety net whose window is deliberately wider than needed, sweeping the
gap after any outage. The webhook body is not trusted, only its identity: we take the order
number and re-fetch the order from the channel (authority is the channel's, the endpoint is
public and forgeable, and a fresh fetch is immune to reordering). Receiving is separated from
processing: the endpoint stores the raw message, queues it and returns `200` - including for
duplicates, because for a channel anything but `2xx` means "retry", and endpoints that keep
failing get disabled. Detail in `01-domain.md` rules 34-35, 39.

## 2026-09-02 - Stock push carries a listing id, not a number (D6)
Absolute values ("set stock to 7") make a push idempotent but not order-safe: a delayed "7"
landing after a successful "5" leaves the channel overstated with no message having failed. So
the message carries only the listing; the number is read from the database at send time - retries
then send the current value for free and coalescing falls out of it. Queues are per channel,
processed with concurrency 1, under a client-side throttle that keeps us below the channel's
limit; `Retry-After` is honoured and retries use backoff + jitter. Push stays best-effort:
"last reported quantity" is written only on success and reconciliation is the actual guarantee.
Detail in `01-domain.md` rules 36-38, 42.

## 2026-09-02 - Duplicates are stopped by a unique index, ordering by the state machine (D6)
Delivery is at-least-once and the same order reaches us by four routes, so duplicates are normal.
Creation cannot be guarded by a lock - the row does not exist yet and "check, then insert" looks
empty to both racers; the guard is a unique index on (Channel, ChannelOrderNumber), keyed on the
identity of the work, not of the message. A repeated message is applied, not discarded: it is
usually an update, and the state machine refuses backwards transitions, which is also what makes
stale fetches harmless. Fields outside the machine (address, carrier) are guarded by the
channel's own updated-at instead. Detail in `01-domain.md` rules 40-41, 43.

## 2026-09-01 - SKU is a business code, not the primary key (D4)
Stock lives on the variant, not the product; every product has at least one variant so there is
only one code path. The variant's key is a surrogate `Id`; the SKU is a uniquely indexed
business code. Making SKU the key would mean rewriting the ledger whenever a code is corrected.
SKUs are never reused: a closed variant is deactivated, not deleted, so the unique index keeps
rejecting the code. Detail in `01-domain.md` rules 21-22.

## 2026-09-01 - Channel mapping is per listing, not per channel (D4)
The bridge row is (Channel, Variant, ChannelCode) and it carries rule 17's "last pushed
quantity" column. One variant may have several listings on the same channel; all of them get
the same available number, never a split. A listing resolves to exactly one variant.
Detail in `01-domain.md` rules 23-24.

## 2026-09-01 - Purpose: skill building, not portfolio
The muscle: designing and standing up a system with concurrency, asynchronous integration and
reconciliation, and diagnosing it when it breaks. Not scale design. Tested by three questions:
one command brings it up, injected faults (`429`, duplicate webhook, two concurrent orders for
the last unit) produce correct behaviour, Huseyin can explain the mechanism in his own words.
Frontend is out of the muscle and stays with Claude. Decisions are never justified by
"defensible in an interview". The README still explains the system honestly, including the
simulator stub and the single-warehouse assumption, because a system that cannot be explained
is not finished.

## 2026-08-31 - Single-warehouse assumption (D3)
One inventory row per SKU; no warehouse/location breakdown. The real cost is not the table but
the decision that follows it: with several warehouses the key becomes `(SKU, Warehouse)` and
then "which warehouse fulfils this order" appears (nearest / most stocked / split the order).
A separate, large problem that adds nothing to the definition of done. Stated in the README
(work item 40) as a known trade-off.

## 2026-08-31 - Stock numbers: counter and ledger together (D3)
On-hand and reserved are kept both as a counter and as ledger records, a deliberate
denormalisation. A ledger alone leaves no row to lock, which makes oversell protection
(work item 10) structurally impossible. The reservation decision is read and written in one
transaction on that single locked row; that row is the whole of the oversell protection.
Available is never stored, it is computed; what is
stored per channel instead is the "last reported number", to locate drift.
Details in `01-domain.md` rules 15-20.

## 2026-08-31 - Deep link to the channel order: folded into work item 35
The channel panel is not abandoned (customer messages, return approval, payouts, pricing and
campaigns stay there). The order detail keeps a link to the channel own order page - not new
code, a single URL field.

## 2026-08-31 - Turkish term collision: a work item is called **task**
Three things shared one word ("kalem"): work item, order line, order field. The word is retired.
A roadmap work item is **"task 27"** in Turkish docs and chat - an English term used as-is, like
interface or idempotency. Order line is **"siparis satiri"**, order field is **"alan"**.

## 2026-08-31 - `01-domain.md` holds only rules that become code
Shared language, authority, direction of flow, invariants, buffer policy. Lesson questions and
answers stay in the conversation; verification happens there and archiving adds nothing.

## 2026-08-31 - Architecture: selective DDD + vertical slice, no Clean Architecture layers
Inside a module, files are grouped by the job being done, not by technical type:
`Features/CancelOrder/` holds command + handler + validator + endpoint together. `Domain/` is
not sliced; the aggregate is the shared thing.

Tactical DDD (aggregate, value object, domain event) **only where an invariant exists**: Inventory
and Ordering. The rule being protected: *reserved stock can never
exceed on-hand stock.* Identity and Integration configuration are plain CRUD.

Separate `Application` / `Domain` / `Infrastructure` projects will **not** be created - a
competing solution to vertical slice, and running both violates rule 3.

## 2026-08-31 - Rate limiting: two purposes, no new work item
**Inbound** (our own API, protecting ourselves): the ASP.NET Core built-in rate limiter, in the
framework since .NET 7. Added to work item 17. Known limit: it is in-memory, so every instance
keeps its own counter - 3 replicas means 3x the effective limit. Distributed limiting needs
Redis and is not done here; knowing the limit is enough.

**Outbound** (requests to marketplaces): the marketplace limits us and returns `429`. Honour
`Retry-After` and throttle on the client side; retrying aggressively makes it worse. Folded
into work item 22 (Polly).

## 2026-08-31 - Deployment: decision deferred, code kept ready
The Azure/AWS/GCP choice is made after the core is done (pricing research is on Huseyin).
Deferring costs nothing because three habits are adopted from day one, all of them correct
practice regardless:
1. Configuration comes from environment variables; no connection string in code
2. The application never writes to local disk (files live in MinIO)
3. File storage sits behind `IFileStorage` - MinIO speaks the S3 SDK, Azure Blob has a
   different API. With the interface, switching is one class

RabbitMQ is already absorbed by MassTransit (changing transport is configuration).

## 2026-08-29 - Working setup: Rider + Claude Code CLI in the terminal
JetBrains Rider (free for non-commercial use, supports .NET 10 / C# 14). The agent runs as the
Claude Code CLI in Rider integrated terminal, not as the IDE plugin (plugin is beta, version
mismatches reported). The review surface is **the Git diff window in Rider**, not a chat panel.

## 2026-08-29 - Database: SQL Server
De facto standard in Turkish .NET job posts; T-SQL practice comes from here. Developer Edition
is free and runs in Docker.

## 2026-08-29 - Messaging: MassTransit + RabbitMQ
Outbox, retry and dead-letter come built in. Writing an outbox by hand would teach more but
would slow things down noticeably - what happens inside is explained once, verbally.

## 2026-08-29 - Testing: minimal core
~10 unit tests (reservation/oversell) + 2 Testcontainers integration tests. Nothing more.
Never having written a test is a real gap; this much closes it.

## 2026-08-29 - MinIO is in scope
Bulk Excel/CSV product-stock upload and product images are real use cases. Being S3-compatible,
what is actually learned is the S3 API. **The one deliberate scope addition; that door is closed.**

## 2026-08-29 - CI/CD, lint and PR ritual are out of scope
Setup ate too much time in the previous project. Single branch, plain commits. GitHub Actions
last and optional.
