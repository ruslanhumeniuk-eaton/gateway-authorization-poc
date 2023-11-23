using Contracts.Application;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shared.Encryption;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     <see cref="ResourceId{T}" /> model binder. Allows to use <see cref="ResourceId{T}" /> as controller parameter.
/// </summary>
/// <typeparam name="T">Type of resource ID.</typeparam>
public class ResourceIdBinder<T> : IModelBinder
{
    private readonly IEncryptor _encryptor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResourceIdBinder{T}" /> class.
    /// </summary>
    /// <param name="encryptor">Encryptor.</param>
    public ResourceIdBinder(IEncryptor encryptor)
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

        string modelName = bindingContext.ModelName;

        // Try to fetch the value of the argument by name
        ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        string value = valueProviderResult.FirstValue;

        // Check if the argument value is null or empty
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        // Convert the value
        bindingContext.Result = ModelBindingResult.Success(new ResourceId<T>(_encryptor.Decrypt<T>(value)));

        return Task.CompletedTask;
    }
}