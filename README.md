# OrderSync

Multi-channel order & inventory orchestration service.

Collects orders from multiple sales channels in one place, manages stock as a single source
of truth, prevents the same unit from being sold on two channels at once (oversell), and
pushes every stock change back to all channels.

> **Note:** The marketplace side is simulated by a stub that follows the contracts published
> in the official marketplace API documentation. Reason: error scenarios such as `429`,
> timeouts and duplicate webhooks can then be produced deterministically and tested.
> The system also connects to at least one real external service (TCMB exchange rates).

Status: **planning / domain phase.** See `docs/`.
