using Mapster;
using Microsoft.EntityFrameworkCore;
using OutboxPattern.WebAPI.BackgroundServices;
using OutboxPattern.WebAPI.Context;
using OutboxPattern.WebAPI.Dtos;
using OutboxPattern.WebAPI.Models;
using Scalar.AspNetCore;
using TS.Result;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddFluentEmail("info@outbox.com").AddSmtpSender("localhost", 25);

builder.Services.AddHostedService<OrderBackgrodundService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.MapPost("orders", async (CreateOrderDto request, ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
{
    Order order = request.Adapt<Order>();
    order.CreateAt = DateTimeOffset.Now;
    dbContext.Add(order);

    OrderOutbox orderOutbox = new()
    {
        OrderId = order.Id,
        CreateAt = DateTimeOffset.Now,
    };
    dbContext.Add(orderOutbox);

    await dbContext.SaveChangesAsync(cancellationToken);

    return Results.Ok(Result<string>.Succeed("Sipariþ baþarýyla oluþturuldu"));
})
    .Produces<Result<string>>();

app.MapGet("orders", async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
{
    List<Order> orders = await dbContext.Orders.ToListAsync(cancellationToken);

    return Results.Ok(orders);
})
    .Produces<List<Order>>();

app.Run();
