namespace Contracts;

public class AddressDto
{
    public string Line1 { get; set; } = "";

    public string Line2 { get; set; } = "";

    public IEnumerable<CityDto> AcceptedCities { get; set; } = new List<CityDto>();
}