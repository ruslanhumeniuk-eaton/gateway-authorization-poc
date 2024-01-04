using System.Text.RegularExpressions;
using System.Web;

namespace OcelotGateway.Hateoas;

/// <summary>
///     Represent a templated link.
/// </summary>
public class LinkTemplate
{
    private readonly string _template;

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinkTemplate" /> class.
    /// </summary>
    /// <param name="template">Link template.</param>
    public LinkTemplate(string template)
    {
        _template = template;
    }

    /// <summary>
    ///     Format the template with the given place holders values.
    /// </summary>
    /// <param name="placeHolderValues">Place holders values.</param>
    /// <returns>Template with replaced place holders.</returns>
    public string Format(IEnumerable<KeyValuePair<string, string?>>? placeHolderValues)
    {
        if (placeHolderValues is null)
        {
            return _template;
        }

        string replacedTemplate = _template;

        foreach (KeyValuePair<string, string?> placeHolderValue in placeHolderValues)
        {
            if (placeHolderValue.Value is null)
            {
                continue;
            }

            var regex = new Regex(Regex.Escape($"{{{placeHolderValue.Key}}}"));
            replacedTemplate = regex.Replace(replacedTemplate, HttpUtility.UrlEncode(placeHolderValue.Value), 1);
        }

        return replacedTemplate;
    }

    /// <inheritdoc />
    public override string ToString() => _template;
}