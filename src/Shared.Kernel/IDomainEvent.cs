namespace Shared.Kernel;

public interface IDomainEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredAtUtc { get; }
}
