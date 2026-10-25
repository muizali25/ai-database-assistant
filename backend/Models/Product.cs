namespace MiniPosAi.Models;

public class Product
{
    public int ProductId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Category? Category { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
