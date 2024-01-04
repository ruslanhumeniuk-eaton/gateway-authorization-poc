namespace Contracts;
public class ContractDto
{
    public Guid Id { get; set; }

    public DateOnly Date { get; set; }

    public AddressDto Address { get; set; } = new();
}
