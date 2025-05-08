namespace TuberTreats.Models
{
    public class TuberOrder
    {
        public int Id { get; set; }
        public DateTime OrderPlacedOnDate { get; set; }
        public DateTime? DeliveredOnDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int? TuberDriverId { get; set; }
        public TuberDriver TuberDriver { get; set; }

        public List<TuberTopping> TuberToppings { get; set; } = new();

        // ✅ Writable and directly usable in tests
        public List<Topping> Toppings { get; set; } = new();
    }
}
