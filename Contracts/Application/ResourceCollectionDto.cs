namespace Contracts.Application;

/// <summary>
///     Basic class for DTOs returning collections.
/// </summary>
public abstract class ResourceCollectionDto
{
    /// <summary>
    ///     Gets or sets total amount of items.
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    ///     Gets or sets current page.
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    ///     Gets or sets a number of items per page.
    /// </summary>
    public int? PageSize { get; set; }
}