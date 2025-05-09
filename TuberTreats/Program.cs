// TuberTreats\Program.cs
using TuberTreats.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ! ✅ this registers the in-memory data store
builder.Services.AddSingleton<FakeDataStore>();
var app = builder.Build();


// 🧪 Seed FakeDataStore with test data
var dataStore = app.Services.GetRequiredService<FakeDataStore>();

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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//add endpoints here

// ! Customers
// Get all customers
app.MapGet("/customers", () => dataStore.Customers);

// Get customer by id with their orders
app.MapGet("/customers/{id:int}", (int id) =>
{
    var customer = dataStore.Customers.FirstOrDefault(c => c.Id == id);
    if (customer is null) return Results.NotFound();

    var orders = dataStore.TuberOrders.Where(o => o.CustomerId == id).ToList();

    return Results.Ok(new
    {
        Customer = customer,
        Orders = orders
    });
});

// Add customer
app.MapPost("/customers", (Customer newCustomer) =>
{
    newCustomer.Id = dataStore.Customers.Max(c => c.Id) + 1;
    dataStore.Customers.Add(newCustomer);
    return Results.Created($"/customers/{newCustomer.Id}", newCustomer);
});

// Delete customer
app.MapDelete("/customers/{id:int}", (int id) =>
{
    var customer = dataStore.Customers.FirstOrDefault(c => c.Id == id);
    if (customer is null) return Results.NotFound();

    dataStore.Customers.Remove(customer);
    return Results.NoContent();
});
// ! End

// ! Toppings

// Get all toppings
app.MapGet("/toppings", () => dataStore.Toppings);

// Get topping by id
app.MapGet("/toppings/{id:int}", (int id) =>
{
    var topping = dataStore.Toppings.FirstOrDefault(t => t.Id == id);
    return topping is not null ? Results.Ok(topping) : Results.NotFound();

});
// ! End

// ! Drivers
// Get all drivers
app.MapGet("/tuberdrivers", () => dataStore.TuberDrivers);

// Get driver by id with their deliveries
app.MapGet("/tuberdrivers/{id:int}", (int id) =>
{
    var driver = dataStore.TuberDrivers.FirstOrDefault(d => d.Id == id);
    if (driver is null) return Results.NotFound();

    var deliveries = dataStore.TuberOrders.Where(o => o.TuberDriverId == id).ToList();

    return Results.Ok(new
    {
        Driver = driver,
        Deliveries = deliveries
    });
});
// ! End

// ! Orders
// Get all orders
app.MapGet("/tuberorders", () =>
{
    return dataStore.TuberOrders;
});

// Get an order by id, including customer, driver, toppings
app.MapGet("/tuberorders/{id:int}", (int id) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == id);
    if (order is null) return Results.NotFound();

    var customer = dataStore.Customers.FirstOrDefault(c => c.Id == order.CustomerId);
    var driver = order.TuberDriverId.HasValue ? dataStore.TuberDrivers.FirstOrDefault(d => d.Id == order.TuberDriverId.Value) : null;

    var result = new
    {
        order.Id,
        order.OrderPlacedOnDate,
        Customer = customer,
        Driver = driver,
        Toppings = order.Toppings
    };

    return Results.Ok(result);
});

// Submit a new order
app.MapPost("/tuberorders", (TuberOrder newOrder) =>
{
    newOrder.Id = dataStore.TuberOrders.Max(o => o.Id) + 1;
    newOrder.OrderPlacedOnDate = DateTime.Now;
    newOrder.Toppings = newOrder.Toppings ?? new List<Topping>();
    dataStore.TuberOrders.Add(newOrder);
    return Results.Created($"/tuberorders/{newOrder.Id}", newOrder);
});

// Assign a driver to an order
app.MapPut("/tuberorders/{id:int}", (int id, int driverId) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == id);
    if (order is null) return Results.NotFound();

    var driver = dataStore.TuberDrivers.FirstOrDefault(d => d.Id == driverId);
    if (driver is null) return Results.BadRequest("Driver not found");

    order.TuberDriverId = driverId;
    return Results.Ok(order);
});

// Complete an order
app.MapPost("/tuberorders/{id:int}/complete", (int id) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == id);
    if (order is null) return Results.NotFound();

    // Define "complete" as removing it or setting a flag (extend model if needed)
    dataStore.TuberOrders.Remove(order);
    return Results.Ok($"Order {id} completed and removed.");
});
// ! End

// ! TuberToppings
// Get all TuberToppings
app.MapGet("/tubertoppings", () => dataStore.TuberToppings);

// Add topping to a TuberOrder
app.MapPost("/tubertoppings", (int orderId, int toppingId) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == orderId);
    var topping = dataStore.Toppings.FirstOrDefault(t => t.Id == toppingId);
    if (order is null || topping is null)
        return Results.BadRequest("Invalid order or topping ID.");

    order.Toppings.Add(topping);

    var newTuberTopping = new TuberTopping { Id = dataStore.TuberToppings.Count + 1, TuberOrderId = orderId, ToppingId = toppingId };
    dataStore.TuberToppings.Add(newTuberTopping);

    return Results.Ok(newTuberTopping);
});

// Remove topping from a TuberOrder
app.MapDelete("/tubertoppings", (int orderId, int toppingId) =>
{
    var order = dataStore.TuberOrders.FirstOrDefault(o => o.Id == orderId);
    if (order is null) return Results.NotFound("Order not found.");

    var toppingToRemove = order.Toppings.FirstOrDefault(t => t.Id == toppingId);
    if (toppingToRemove is null) return Results.NotFound("Topping not found on order.");

    order.Toppings.Remove(toppingToRemove);

    var tt = dataStore.TuberToppings.FirstOrDefault(x => x.TuberOrderId == orderId && x.ToppingId == toppingId);
    if (tt is not null)
        dataStore.TuberToppings.Remove(tt);

    return Results.Ok("Topping removed from order.");
});
// ! End
app.Run();


//don't touch or move this!
public partial class Program { }