public class TuberDriverDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class TuberDriverWithDeliveriesDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<TuberOrderDto> Deliveries { get; set; } = new();
}
