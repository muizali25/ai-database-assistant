using MiniPosAi.Models;

namespace MiniPosAi.Data;

public static class SeedData
{
    public static Category[] Categories { get; } =
    [
        new() { CategoryId = 1, Name = "Beverages", Description = "Drinks and refreshments" },
        new() { CategoryId = 2, Name = "Snacks", Description = "Packaged snacks" },
        new() { CategoryId = 3, Name = "Dairy", Description = "Milk and cheese products" },
        new() { CategoryId = 4, Name = "Bakery", Description = "Bread and baked goods" },
        new() { CategoryId = 5, Name = "Grocery", Description = "Everyday grocery items" }
    ];

    public static Customer[] Customers { get; } = CreateCustomers();
    public static Product[] Products { get; } = CreateProducts();
    public static Sale[] Sales { get; } = CreateSales();
    public static SaleItem[] SaleItems { get; } = CreateSaleItems();

    private static Customer[] CreateCustomers()
    {
        var firstNames = new[]
        {
            "Alice", "Bob", "Carol", "David", "Emma", "Frank", "Grace", "Henry", "Ivy", "Jack",
            "Karen", "Leo", "Mia", "Noah", "Olivia", "Paul", "Quinn", "Rita", "Sam", "Tina",
            "Uma", "Victor", "Wendy", "Xavier", "Yara", "Zane"
        };
        var lastNames = new[]
        {
            "Johnson", "Smith", "Lee", "Brown", "Garcia", "Martinez", "Davis", "Wilson", "Moore", "Taylor"
        };

        var customers = new Customer[50];
        for (var i = 1; i <= 50; i++)
        {
            var first = firstNames[(i - 1) % firstNames.Length];
            var last = lastNames[(i - 1) % lastNames.Length];
            customers[i - 1] = new Customer
            {
                CustomerId = i,
                Name = $"{first} {last}",
                Email = $"{first.ToLowerInvariant()}.{last.ToLowerInvariant()}{i}@example.com",
                Phone = $"555-{(1000 + i):D4}"
            };
        }

        return customers;
    }

    private static Product[] CreateProducts()
    {
        var names = new[]
        {
            "Cola 500ml", "Orange Juice 1L", "Mineral Water", "Green Tea", "Coffee Pack",
            "Potato Chips", "Chocolate Bar", "Cookies Pack", "Nuts Mix", "Candy Pack",
            "Whole Milk 1L", "Cheddar Cheese", "Yogurt Cup", "Butter 250g", "Cream Cheese",
            "White Bread", "Croissant", "Bagel", "Muffin", "Donut Pack",
            "Rice 1kg", "Pasta 500g", "Olive Oil", "Tomato Sauce", "Cereal Box"
        };

        var products = new Product[100];
        for (var i = 1; i <= 100; i++)
        {
            var categoryId = ((i - 1) % Categories.Length) + 1;
            var baseName = names[(i - 1) % names.Length];
            var price = Math.Round(1.25m + ((i * 0.37m) % 18.5m), 2);
            var stock = 20 + ((i * 7) % 180);

            products[i - 1] = new Product
            {
                ProductId = i,
                CategoryId = categoryId,
                Name = $"{baseName} #{i}",
                Price = price,
                Stock = stock
            };
        }

        return products;
    }

    private static Sale[] CreateSales()
    {
        var sales = new Sale[100];
        var start = new DateTime(2026, 1, 1, 9, 0, 0);

        for (var i = 1; i <= 100; i++)
        {
            var customerId = ((i - 1) % 50) + 1;
            var productId = ((i - 1) % 100) + 1;
            var secondProductId = (i % 100) + 1;
            var qty1 = (i % 3) + 1;
            var qty2 = (i % 2) + 1;

            var price1 = Products[productId - 1].Price;
            var price2 = Products[secondProductId - 1].Price;
            var total = Math.Round((price1 * qty1) + (price2 * qty2), 2);

            sales[i - 1] = new Sale
            {
                SaleId = i,
                CustomerId = customerId,
                SaleDate = start.AddDays(i - 1).AddHours(i % 8).AddMinutes((i * 7) % 60),
                TotalAmount = total
            };
        }

        return sales;
    }

    private static SaleItem[] CreateSaleItems()
    {
        // 2 line items per sale => 200 sale items
        var items = new SaleItem[200];
        var saleItemId = 1;

        for (var saleId = 1; saleId <= 100; saleId++)
        {
            var productId1 = ((saleId - 1) % 100) + 1;
            var productId2 = (saleId % 100) + 1;
            var qty1 = (saleId % 3) + 1;
            var qty2 = (saleId % 2) + 1;

            items[saleItemId - 1] = new SaleItem
            {
                SaleItemId = saleItemId,
                SaleId = saleId,
                ProductId = productId1,
                Quantity = qty1,
                UnitPrice = Products[productId1 - 1].Price
            };
            saleItemId++;

            items[saleItemId - 1] = new SaleItem
            {
                SaleItemId = saleItemId,
                SaleId = saleId,
                ProductId = productId2,
                Quantity = qty2,
                UnitPrice = Products[productId2 - 1].Price
            };
            saleItemId++;
        }

        return items;
    }
}
