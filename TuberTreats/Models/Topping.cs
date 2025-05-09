namespace TuberTreats.Models
{
    public class Topping
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Optional, but strongly recommended: reverse navigation
        public List<TuberTopping> TuberToppings { get; set; } = new();

        // Optional if you're modeling .Toppings directly on TuberOrder
        public List<TuberOrder> TuberOrders { get; set; } = new();
    }
}
