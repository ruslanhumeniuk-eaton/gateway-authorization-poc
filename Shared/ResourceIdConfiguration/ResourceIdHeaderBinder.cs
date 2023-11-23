using Contracts.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Encryption;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="ResourceIdHeaderBinder{T}" /> model binder. Allows to use <see cref="ResourceId{T}" /> as controller parameter with <see cref="FromHeaderAttribute" />.
/// </summary>
/// <typeparam name="T">Type of resource ID.</typeparam>
public class ResourceIdHeaderBinder<T> : IModelBinder
{
    private readonly IEncryptor _encryptor;

    public ResourceIdHeaderBinder(IEncryptor encryptor)
    {
        _encryptor = encryptor;
    }

    /// <summary>
    ///     Allows to decrypt given string to the resource id parameter.
    /// </summary>
    /// <param name="bindingContext">Model binding context.</param>
    /// <returns>Async task.</returns>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var headerValue = bindingContext.HttpContext.Request.Headers[bindingContext.FieldName].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerValue))
        {
            bindingContext.Result = ModelBindingResult.Success(new ResourceId<T>(_encryptor.Decrypt<T>(headerValue)));
        }

        return Task.CompletedTask;
    }
}