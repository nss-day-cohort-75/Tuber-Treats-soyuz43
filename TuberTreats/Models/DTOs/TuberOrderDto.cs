public class TuberOrderDto
{
    public int Id { get; set; }
    public DateTime OrderPlacedOnDate { get; set; }
    public string? CustomerName { get; set; }
    public string? DriverName { get; set; }
    public List<string> Toppings { get; set; } = new();
}
