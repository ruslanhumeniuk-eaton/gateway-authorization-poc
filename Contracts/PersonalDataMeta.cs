using System.Text.Json.Serialization;
using Contracts.Application;

namespace Contracts;

public class PersonalDataMeta
{
    /// <summary>
    ///     Gets organization id.
    /// </summary>
    [JsonIgnore]
    public ResourceId<Guid> ContractId { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PersonalDataMeta" /> class.
    /// </summary>
    /// <param name="contractId">Organization unique identifier.</param>
    public PersonalDataMeta(ResourceId<Guid> contractId)
    {
        ContractId = contractId;
    }
}