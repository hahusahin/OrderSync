# Status

**Last updated:** 2026-09-03

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
- **Next: work item 01** — solution structure and modular monolith skeleton.

## Open debts

**Owed re-explanations** (asked once the code exists, out of that work item's own code):

4. What are the two different purposes of rate limiting? Why does retrying on a `429` from a
   marketplace make things worse? (D6 taught the outbound half.) — asked at work item 22.
5. Does this monolith scale horizontally? Which single piece does not? Why is the stock row lock
   not a global bottleneck? — asked at work item 10.
**Open question inside work item 01:** order creation writes `Order` and `StockItem` in one
transaction (rule 46), but they sit in two modules with a `DbContext` each. How the two contexts
share one transaction is decided when the skeleton is written, not before.

## Work items
8 / 46 (D1-D8)
