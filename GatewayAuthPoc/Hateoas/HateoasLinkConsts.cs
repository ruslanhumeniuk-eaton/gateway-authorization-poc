namespace OcelotGateway.Hateoas;

/// <summary>
///     Static class containing common HATEOAS link names.
/// </summary>
internal static class HateoasLinkConsts
{
    /// <summary>
    ///     Separator for link.
    /// </summary>
    public const string Separator = ":";

    /// <summary>
    ///     Self edit link name.
    /// </summary>
    public const string SelfEdit = Self + Separator + Edit;

    /// <summary>
    ///     Self delete link name.
    /// </summary>
    public const string SelfDelete = Self + Separator + Delete;

    /// <summary>
    ///     Collection link name.
    /// </summary>
    public const string Collection = "collection";

    /// <summary>
    ///     Self link name.
    /// </summary>
    public const string Self = "self";

    /// <summary>
    ///     Edit link name.
    /// </summary>
    public const string Edit = "edit";

    /// <summary>
    ///     Create link name.
    /// </summary>
    public const string Create = "create";

    /// <summary>
    ///     Delete link name.
    /// </summary>
    public const string Delete = "delete";
}