namespace OcelotGateway.Hateoas;

/// <summary>
///     Represent a permission for a given resource.
/// </summary>
internal class Permission
{
    /// <summary>
    ///     Gets the permission name.
    /// </summary>
    public string PermissionKey { get; }

    /// <summary>
    ///     Gets the organization id key.
    /// </summary>
    public string OrganizationIdKey { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Permission" /> class.
    /// </summary>
    /// <param name="permissionKey">Permission key.</param>
    /// <param name="organizationIdKey">Organization id key.</param>
    public Permission(string permissionKey, string organizationIdKey)
    {
        PermissionKey = permissionKey;
        OrganizationIdKey = organizationIdKey;
    }
}