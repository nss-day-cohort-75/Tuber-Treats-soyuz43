// TuberTreats\Models\DTOs\CreateTuberOrderDto.cs
namespace TuberTreats.Models.DTOs
{
    public class CreateTuberOrderDto
    {
        public int CustomerId { get; set; }
        public List<int> ToppingIds { get; set; } = new();
    }
}
