namespace OutboxPattern.WebAPI.Models;

public sealed class Order
{
    public Order()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string CustomerEmail { get; set; } = default!;
    public DateTimeOffset CreateAt { get; set; }
}
