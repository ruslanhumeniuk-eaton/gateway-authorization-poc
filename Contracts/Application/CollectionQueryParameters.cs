namespace Contracts.Application;

public class CollectionQueryParameters
{
    /// <summary>
    ///     Gets or sets page.
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    ///     Gets or sets a number of items per page.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    ///     Gets or sets column name to sort by.
    /// </summary>
    public string SortBy { get; set; }

    /// <summary>
    ///     Gets or sets sort order.
    /// </summary>
    public string SortOrder { get; set; }

    /// <summary>
    ///     Gets or sets column name to filter by.
    /// </summary>
    public string FilterBy { get; set; }

    /// <summary>
    ///     Gets or sets filter criteria.
    /// </summary>
    public string FilterCriteria { get; set; }
}