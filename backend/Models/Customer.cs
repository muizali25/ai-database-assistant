namespace MiniPosAi.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
