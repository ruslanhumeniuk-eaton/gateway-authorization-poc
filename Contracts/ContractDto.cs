namespace Contracts
{
    public class ContractDto
    {
        public DateOnly Date { get; set; }

        public int RandomNumber { get; set; }

        public int RandomCalculation => 32 + (int)(RandomNumber / 0.5556);

        public string? Summary { get; set; }
    }
}