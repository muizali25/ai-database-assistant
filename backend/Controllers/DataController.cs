using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniPosAi.Data;

namespace MiniPosAi.Controllers;

[ApiController]
[Route("api/data")]
public class DataController(AppDbContext db) : ControllerBase
{
    private static readonly HashSet<string> AllowedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "Products", "Categories", "Customers", "Sales"
    };

    [HttpGet("{tableName}")]
    public async Task<IActionResult> GetTable(string tableName, CancellationToken cancellationToken)
    {
        if (!AllowedTables.Contains(tableName))
            return BadRequest(new { error = "Table not allowed. Use Products, Categories, Customers, or Sales." });

        object data = tableName.ToLowerInvariant() switch
        {
            "products" => await db.Products
                .AsNoTracking()
                .Select(p => new { p.ProductId, p.CategoryId, p.Name, p.Price, p.Stock })
                .ToListAsync(cancellationToken),
            "categories" => await db.Categories
                .AsNoTracking()
                .Select(c => new { c.CategoryId, c.Name, c.Description })
                .ToListAsync(cancellationToken),
            "customers" => await db.Customers
                .AsNoTracking()
                .Select(c => new { c.CustomerId, c.Name, c.Email, c.Phone })
                .ToListAsync(cancellationToken),
            "sales" => await db.Sales
                .AsNoTracking()
                .Select(s => new { s.SaleId, s.CustomerId, s.SaleDate, s.TotalAmount })
                .ToListAsync(cancellationToken),
            _ => Array.Empty<object>()
        };

        return Ok(data);
    }
}
