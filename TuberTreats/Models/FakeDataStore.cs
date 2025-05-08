namespace TuberTreats.Models
{
    public class FakeDataStore
    {
        public List<Customer> Customers { get; set; } = new();
        public List<TuberOrder> TuberOrders { get; set; } = new();
        public List<TuberDriver> TuberDrivers { get; set; } = new();
        public List<Topping> Toppings { get; set; } = new();
        public List<TuberTopping> TuberToppings { get; set; } = new();
    }
}
