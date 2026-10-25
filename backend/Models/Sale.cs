namespace MiniPosAi.Models;

public class Sale
{
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
