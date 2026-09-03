namespace Shared.Contracts;

/// <summary>
/// A fact that has already happened, published on the bus for other modules to react to.
/// Its type is shared because the publisher and the consumer both need it.
/// </summary>
public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAtUtc { get; }
}
