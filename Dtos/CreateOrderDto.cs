namespace OutboxPattern.WebAPI.Dtos;

public sealed record CreateOrderDto(
    string ProductName,
    int Quantity,
    decimal Price,
    string CustomerEmail);
