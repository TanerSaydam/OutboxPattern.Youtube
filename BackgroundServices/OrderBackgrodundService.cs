
using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using OutboxPattern.WebAPI.Context;

namespace OutboxPattern.WebAPI.BackgroundServices;

public sealed class OrderBackgrodundService(
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scoped = serviceProvider.CreateScope())
        {
            var dbConext = scoped.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var fluentEmail = scoped.ServiceProvider.GetRequiredService<IFluentEmail>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var outboxes = await dbConext.OrderOutboxes
                .Where(p => !p.IsCompleted)
                .OrderBy(p => p.CreateAt)
                .ToListAsync(stoppingToken);

                foreach (var item in outboxes)
                {
                    try
                    {
                        if (item.Attempt >= 3)
                        {
                            item.IsCompleted = true;
                            item.CompleteDate = DateTimeOffset.Now;
                            item.IsFailed = true;
                            item.FailMessage = "Mail gönderme başarısız";
                            continue;
                        }

                        var order = await dbConext.Orders.FirstAsync(p => p.Id == item.OrderId, stoppingToken);

                        string body = @"
                            <h1>Sipariş Durumu: <b>Başarılı</b></h1>
                            <p>Ürün Adı: {productName}</p>
                            <p>Siparişiniz başarıyla oluşturuldu.</p>
                            <p>Süreç hakkında bilgilendirme maili göndereceğiz</p>
                            ";

                        body = body.Replace("{productName}", order.ProductName);

                        var response = await fluentEmail
                              .To(order.CustomerEmail)
                              .Subject("Oluşturulan Sipariş")
                              .Body(body)
                              .SendAsync(stoppingToken);

                        if (!response.Successful)
                        {
                            item.Attempt++;
                        }
                        else
                        {
                            item.IsCompleted = true;
                            item.CompleteDate = DateTimeOffset.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        item.Attempt++;
                        if (item.Attempt >= 3)
                        {
                            item.IsFailed = true;
                            item.IsCompleted = true;
                            item.CompleteDate = DateTimeOffset.Now;
                            item.FailMessage = ex.Message;
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }

                await dbConext.SaveChangesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
