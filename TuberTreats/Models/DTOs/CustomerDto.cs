public class CustomerDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
}

public class CustomerWithOrdersDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public List<TuberOrderDto> Orders { get; set; } = new();
}
