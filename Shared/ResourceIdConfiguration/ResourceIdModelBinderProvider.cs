using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="ResourceIdBinder{T}" /> provider.
/// </summary>
public class ResourceIdModelBinderProvider : IModelBinderProvider
{
    /// <summary>
    ///     In function of the context instantiate a <see cref="ResourceIdBinder{T}" />.
    /// </summary>
    /// <param name="context">Model binder provider context.</param>
    /// <returns>Model binder, null if not supported.</returns>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.Metadata.ModelType.IsResourceIdType())
        {
            return null;
        }

        Type keyType = context.Metadata.ModelType.GenericTypeArguments[0];
        Type converterType = context.Metadata.ContainsHeaderAttribute() ?
            typeof(ResourceIdHeaderBinder<>).MakeGenericType(keyType) :
            typeof(ResourceIdBinder<>).MakeGenericType(keyType);

        return new BinderTypeModelBinder(converterType);
    }
}