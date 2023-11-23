using System.ComponentModel;

namespace Shared.ResourceIdConfiguration;

/// <summary>
///     Parameter type converter for OpenApi documentation.
/// </summary>
public class StringParameterTypeConverter : TypeConverter
{
    /// <summary>
    ///     Specify that the given type must be converted from a string.
    /// </summary>
    /// <param name="context">Type descriptor context.</param>
    /// <param name="sourceType">Source type.</param>
    /// <returns>Return true for string.</returns>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);
}