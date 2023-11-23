namespace Contracts.Application;

/// <summary>
///     Represent a resource ID.
/// </summary>
/// <typeparam name="T">Internal type of the ID.</typeparam>
public sealed class ResourceId<T> : IResourceId
{
    /// <summary>
    ///     Gets the value of the resource ID.
    /// </summary>
    public T Value { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResourceId{T}" /> class.
    /// </summary>
    /// <param name="id">ID of the resource.</param>
    public ResourceId(T id)
    {
        Value = id;
    }

    /// <summary>
    ///     Explicit cast return value.
    /// </summary>
    /// <param name="id">Resource id to be cast.</param>
    public static explicit operator T(ResourceId<T> id) => id.Value;

    public static implicit operator ResourceId<T>(T id) => new(id);

    /// <summary>
    ///     Return string form of the value.
    /// </summary>
    /// <returns>String form of the value.</returns>
    public override string ToString() => Value.ToString();
}