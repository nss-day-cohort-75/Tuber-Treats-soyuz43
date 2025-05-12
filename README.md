# 🥔 TuberTreats API

TuberTreats is a simple .NET 8 Web API simulating a baked potato delivery service. It uses an in-memory data store (`FakeDataStore`) to manage Customers, Orders, Drivers, Toppings, and their relationships. This project serves as a practice ground for RESTful API design using **ASP.NET Core Minimal APIs**.

---

## 🛠 Tech Stack

- **.NET 8**
- **ASP.NET Core Minimal API**
- **Swagger / Swashbuckle** (for API exploration)
- **In-memory data via singleton service**

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Any IDE or editor (e.g. Visual Studio, Rider, VS Code)

### Running the App

```bash
dotnet run
````

Visit Swagger UI at:

```
https://localhost:<port>/swagger
```

---

## 📦 Project Structure

```
TuberTreats/
├── Models/
│   ├── Customer.cs
│   ├── TuberOrder.cs
│   ├── TuberDriver.cs
│   ├── Topping.cs
│   ├── TuberTopping.cs
│   └── FakeDataStore.cs
└── Program.cs
```

---

## 📖 API Overview

### `/tuberorders`

* `GET /tuberorders` – Get all orders
* `GET /tuberorders/{id}` – Get order by ID (includes customer, driver, and toppings)
* `POST /tuberorders` – Submit a new order (adds timestamp)
* `PUT /tuberorders/{id}` – Assign a driver to an order
* `POST /tuberorders/{id}/complete` – Complete and remove the order

### `/toppings`

* `GET /toppings` – Get all toppings
* `GET /toppings/{id}` – Get a single topping by ID

### `/tubertoppings`

* `GET /tubertoppings` – Get all TuberToppings
* `POST /tubertoppings?orderId=1&toppingId=2` – Add topping to order
* `DELETE /tubertoppings?orderId=1&toppingId=2` – Remove topping from order

### `/customers`

* `GET /customers` – Get all customers
* `GET /customers/{id}` – Get a customer and their orders
* `POST /customers` – Add a new customer
* `DELETE /customers/{id}` – Delete a customer

### `/tuberdrivers`

* `GET /tuberdrivers` – Get all drivers
* `GET /tuberdrivers/{id}` – Get a driver and their deliveries

---

## 🧪 Test Data

Seeded on startup:

* 2 customers: Alice, Bob
* 2 drivers: Dan, Dana
* 3 toppings: Chives, Cheddar, Sour Cream
* 1 order placed by Alice with 2 toppings

---

## 📌 Notes

* No database is used – everything runs in memory.
* This app is for educational/demo purposes and not meant for production use.
* Swagger provides a friendly UI to interact with all endpoints.

---
