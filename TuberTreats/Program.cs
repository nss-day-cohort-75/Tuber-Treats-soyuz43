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

app.Run();
//don't touch or move this!
public partial class Program { }