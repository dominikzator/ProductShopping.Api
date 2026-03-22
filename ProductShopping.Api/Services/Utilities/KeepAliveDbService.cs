using Microsoft.EntityFrameworkCore;
using ProductShopping.Identity.DbContext;

namespace ProductShopping.Api.Services.Utilities;

public class KeepAliveDbService : IHostedService, IAsyncDisposable
{
    private readonly ILogger<KeepAliveDbService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Timer? _timer;
    private readonly Task _completedTask = Task.CompletedTask;

    private const int _longIntervalInMinutes = 27;
    private const int _shortIntervalInSeconds = 35;

    public KeepAliveDbService(ILogger<KeepAliveDbService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _logger.LogInformation("KeepAliveDbService Constructor!!!");
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(_longIntervalInMinutes));
        return _completedTask;
    }

    private async void DoWork(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductShoppingIdentityDbContext>();

        try
        {
            for (int i = 0; i < 3; i++)
            {
                await dbContext.Database.ExecuteSqlRawAsync("SELECT * FROM ProductCategories");
                _logger.LogInformation("Ping {PingNumber} at {Time}", i + 1, DateTimeOffset.Now);
                if (i < 2) await Task.Delay(_shortIntervalInSeconds * 1000, CancellationToken.None);
            }
            _logger.LogInformation("Burst of 3 pings completed at {Time}", DateTimeOffset.Now);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Burst ping failed at {Time}", DateTimeOffset.Now);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return _completedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_timer is IAsyncDisposable timer)
        {
            await timer.DisposeAsync();
        }
        _timer?.Dispose();
    }
}
