// TuberTreats\Program.cs
using TuberTreats.Models;
using TuberTreats.Models.DTOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FakeDataStore>();

var app = builder.Build();
var dataStore = app.Services.GetRequiredService<FakeDataStore>();

// Seed test data
dataStore.Customers.AddRange(new List<Customer>
{
    new() { Id = 1, Name = "Alice", Address = "123 Spud Ln" },
    new() { Id = 2, Name = "Bob", Address = "456 Tater Ave" }
});

dataStore.TuberDrivers.AddRange(new List<TuberDriver>
{
    new() { Id = 1, Name = "Driver Dan" },
    new() { Id = 2, Name = "Driver Dana" }
});

dataStore.Toppings.AddRange(new List<Topping>
{
    new() { Id = 1, Name = "Chives" },
    new() { Id = 2, Name = "Cheddar" },
    new() { Id = 3, Name = "Sour Cream" }
});

dataStore.TuberOrders.AddRange(new List<TuberOrder>
{
    new()
    {
        Id = 1,
        OrderPlacedOnDate = DateTime.Now.AddHours(-2),
        CustomerId = 1,
        TuberDriverId = null,
        Toppings = new List<Topping> { dataStore.Toppings[0], dataStore.Toppings[1] }
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Customers
app.MapGet("/customers", () =>
{
    var dtos = dataStore.Customers.Select(c => new CustomerDto
    {
        Id = c.Id,
        Name = c.Name,
        Address = c.Address
    });
    return Results.Ok(dtos);
});

app.MapGet("/customers/{id:int}", (int id) =>
{
    var customer = dataStore.Customers.FirstOrDefault(c => c.Id == id);
    if (customer is null) return Results.NotFound();

    var orders = dataStore.TuberOrders
        .Where(o => o.CustomerId == id)
        .Select(order => new TuberOrderDto
        {
            Id = order.Id,
            OrderPlacedOnDate = order.OrderPlacedOnDate,
            CustomerName = customer.Name ?? "",
            DriverName = order.TuberDriverId is int driverId
                ? dataStore.TuberDrivers.FirstOrDefault(d => d.Id == driverId)?.Name
                : null,
            Toppings = order.Toppings.Select(t => t.Name).ToList()
        }).ToList();

    var dto = new CustomerWithOrdersDto
    {
        Id = customer.Id,
        Name = customer.Name,
        Address = customer.Address,
        Orders = orders
    };

    return Results.Ok(dto);
});

app.MapPost("/customers", (CreateCustomerDto dto) =>
{
    var newCustomer = new Customer
    {
        Id = dataStore.Customers.Max(c => c.Id) + 1,
        Name = dto.Name,
        Address = dto.Address
    };

    dataStore.Customers.Add(newCustomer);

    var result = new CustomerDto
    {
        Id = newCustomer.Id,
        Name = newCustomer.Name,
        Address = newCustomer.Address
    };

    return Results.Created($"/customers/{newCustomer.Id}", result);
});

app.MapDelete("/customers/{id:int}", (int id) =>
{
    var customer = dataStore.Customers.FirstOrDefault(c => c.Id == id);
    if (customer is null) return Results.NotFound();

    dataStore.Customers.Remove(customer);
    return Results.NoContent();
});

// Toppings
app.MapGet("/toppings", () =>
{
    var dtos = dataStore.Toppings.Select(t => new ToppingDto
    {
        Id = t.Id,
        Name = t.Name
    });
    return Results.Ok(dtos);
});

app.MapGet("/toppings/{id:int}", (int id) =>
{
    var topping = dataStore.Toppings.FirstOrDefault(t => t.Id == id);
    if (topping is null) return Results.NotFound();

    var dto = new ToppingDto
    {
        Id = topping.Id,
        Name = topping.Name
    };

    return Results.Ok(dto);
});

// Drivers
app.MapGet("/tuberdrivers", () =>
{
    var dtos = dataStore.TuberDrivers.Select(d => new TuberDriverDto
    {
        Id = d.Id,
        Name = d.Name
    });
    return Results.Ok(dtos);
});

app.MapGet("/tuberdrivers/{id:int}", (int id) =>
{
    var driver = dataStore.TuberDrivers.FirstOrDefault(d => d.Id == id);
    if (driver is null) return Results.NotFound();

    var deliveries = dataStore.TuberOrders
        .Where(o => o.TuberDriverId == id)
        .Select(order => new TuberOrderDto
        {
            Id = order.Id,
            OrderPlacedOnDate = order.OrderPlacedOnDate,
            CustomerName = dataStore.Customers.First(c => c.Id == order.CustomerId).Name ?? "",
            DriverName = driver.Name,
            Toppings = order.Toppings.Select(t => t.Name).ToList()
        }).ToList();

    var dto = new TuberDriverWithDeliveriesDto
    {
        Id = driver.Id,
        Name = driver.Name,
        Deliveries = deliveries
    };

    return Results.Ok(dto);
});

// TuberOrders
app.MapGet("/tuberorders", () =>
{
    var dtos = dataStore.TuberOrders.Select(order => new TuberOrderDto
    {
        Id = order.Id,
        OrderPlacedOnDate = order.OrderPlacedOnDate,
        CustomerName = dataStore.Customers.First(c => c.Id == order.CustomerId).Name ?? "",
        DriverName = order.TuberDriverId is int driverId
            ? dataStore.TuberDrivers.FirstOrDefault(d => d.Id == driverId)?.Name
            : null,
        Toppings = order.Toppings.Select(t => t.Name).ToList()
    });

    return Results.Ok(dtos);
});

app.MapGet("/tuberorders/{id:int}", (int id) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == id);
    if (order is null) return Results.NotFound();

    var dto = new TuberOrderDto
    {
        Id = order.Id,
        OrderPlacedOnDate = order.OrderPlacedOnDate,
        CustomerName = dataStore.Customers.FirstOrDefault(c => c.Id == order.CustomerId)?.Name ?? "",
        DriverName = order.TuberDriverId is int driverId
            ? dataStore.TuberDrivers.FirstOrDefault(d => d.Id == driverId)?.Name
            : null,
        Toppings = order.Toppings.Select(t => t.Name).ToList()
    };

    return Results.Ok(dto);
});

app.MapPost("/tuberorders", (CreateTuberOrderDto dto) =>
{
    var toppings = dto.ToppingIds
        .Select(id => dataStore.Toppings.FirstOrDefault(t => t.Id == id))
        .Where(t => t is not null)
        .ToList();

    var newOrder = new TuberOrder
    {
        Id = dataStore.TuberOrders.Max(o => o.Id) + 1,
        OrderPlacedOnDate = DateTime.Now,
        CustomerId = dto.CustomerId,
        Toppings = toppings!
    };

    dataStore.TuberOrders.Add(newOrder);

    var result = new TuberOrderDto
    {
        Id = newOrder.Id,
        OrderPlacedOnDate = newOrder.OrderPlacedOnDate,
        CustomerName = dataStore.Customers.First(c => c.Id == dto.CustomerId).Name ?? "",
        DriverName = null,
        Toppings = toppings.Select(t => t!.Name).ToList()
    };

    return Results.Created($"/tuberorders/{newOrder.Id}", result);
});

// TuberToppings
app.MapGet("/tubertoppings", () =>
{
    var dtos = dataStore.TuberToppings.Select(tt => new TuberToppingDto
    {
        Id = tt.Id,
        TuberOrderId = tt.TuberOrderId,
        ToppingName = dataStore.Toppings.FirstOrDefault(t => t.Id == tt.ToppingId)?.Name ?? ""
    });

    return Results.Ok(dtos);
});

app.MapPost("/tubertoppings", (int orderId, int toppingId) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == orderId);
    var topping = dataStore.Toppings.FirstOrDefault(t => t.Id == toppingId);
    if (order is null || topping is null)
        return Results.BadRequest("Invalid order or topping ID.");

    order.Toppings.Add(topping);

    var newTuberTopping = new TuberTopping
    {
        Id = dataStore.TuberToppings.Count + 1,
        TuberOrderId = orderId,
        ToppingId = toppingId
    };

    dataStore.TuberToppings.Add(newTuberTopping);

    var dto = new TuberToppingDto
    {
        Id = newTuberTopping.Id,
        TuberOrderId = newTuberTopping.TuberOrderId,
        ToppingName = topping.Name ?? ""
    };

    return Results.Ok(dto);
});

app.Run();

public partial class Program { }
