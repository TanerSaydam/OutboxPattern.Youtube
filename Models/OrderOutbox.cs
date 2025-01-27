namespace OutboxPattern.WebAPI.Models;

public sealed class OrderOutbox
{
    public OrderOutbox()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public DateTimeOffset CreateAt { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsFailed { get; set; }
    public string? FailMessage { get; set; }
    public DateTimeOffset? CompleteDate { get; set; }
    public int Attempt { get; set; }
}
