using Contracts.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Shared.ResourceIdConfiguration;

public static class GenericExtensions
{
    /// <summary>
    ///     Return true if the given type is <see cref="ResourceId{T}" />.
    /// </summary>
    /// <returns>True if resource id.</returns>
    public static bool IsResourceIdType(this Type typeToCheck)
    {
        if (!typeToCheck.IsGenericType)
        {
            return false;
        }

        Type type = typeToCheck;

        if (!type.IsGenericTypeDefinition)
        {
            type = type.GetGenericTypeDefinition();
        }

        return type == typeof(ResourceId<>);
    }

    /// <summary>
    ///     Checks if <see cref="ModelMetadata"/> contains <see cref="FromHeaderAttribute"/>.
    /// </summary>
    /// <param name="metadata">Metadata to check.</param>
    /// <returns>True if contains <see cref="FromHeaderAttribute"/>.</returns>
    public static bool ContainsHeaderAttribute(this ModelMetadata metadata)
    {
        var defaultMetadata = metadata as DefaultModelMetadata;

        return defaultMetadata.Attributes.Attributes.Any(a => a.GetType() == typeof(FromHeaderAttribute));
    }
}