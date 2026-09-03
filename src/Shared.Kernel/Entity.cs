namespace Shared.Kernel;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid id) => Id = id;

    protected Entity()
    {
    }

    public Guid Id { get; protected init; }

    public bool Equals(Entity? other) =>
        other is not null && other.GetType() == GetType() && other.Id == Id && Id != Guid.Empty;

    public override bool Equals(object? obj) => obj is Entity entity && Equals(entity);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
