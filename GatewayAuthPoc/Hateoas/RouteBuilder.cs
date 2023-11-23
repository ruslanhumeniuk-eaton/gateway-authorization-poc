namespace OcelotGateway.Hateoas;

/// <summary>
///     Route builder for the given DTO object.
/// </summary>
/// <typeparam name="T">DTO.</typeparam>
internal class RouteBuilder<T>
    where T : class
{
    private readonly Func<T, bool> _condition;
    private readonly IList<KeyValuePair<string, Func<T, object>>> _placeHolders;

    /// <summary>
    ///     Gets the route name.
    /// </summary>
    public string RouteName { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RouteBuilder{T}" /> class.
    /// </summary>
    /// <param name="routeName">Route name.</param>
    public RouteBuilder(string routeName)
    {
        RouteName = routeName;
        _placeHolders = new List<KeyValuePair<string, Func<T, object>>>();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RouteBuilder{T}" /> class.
    /// </summary>
    /// <param name="routeName">Route name.</param>
    /// <param name="condition">Condition.</param>
    public RouteBuilder(string routeName, Func<T, bool> condition)
        : this(routeName)
    {
        _condition = condition;
    }

    /// <summary>
    ///     Place holder to replace with the given function.
    /// </summary>
    /// <param name="placeHolder">Place holder.</param>
    /// <param name="placeHolderValueFunc">Function.</param>
    /// <returns>Return route builder for chaining.</returns>
    public RouteBuilder<T> Replace(string placeHolder, Func<T, object> placeHolderValueFunc)
    {
        _placeHolders.Add(new KeyValuePair<string, Func<T, object>>(placeHolder, placeHolderValueFunc));
        return this;
    }

    /// <summary>
    ///     Check if conditions is satisfied for creating the route.
    /// </summary>
    /// <param name="o">Object.</param>
    /// <returns>True if the condition is satisfied.</returns>
    public bool CanCreate(T o) => _condition is null || _condition(o);

    /// <summary>
    ///     Get place holders for the given object.
    /// </summary>
    /// <param name="dto">DTO.</param>
    /// <returns>List of place holders.</returns>
    public IEnumerable<KeyValuePair<string, object>> GetPlaceHolders(T dto)
    {
        var placeHolders = new List<KeyValuePair<string, object>>();

        foreach ((string key, Func<T, object> value) in _placeHolders)
        {
            placeHolders.Add(new KeyValuePair<string, object>(key, value(dto)));
        }

        return placeHolders;
    }
}