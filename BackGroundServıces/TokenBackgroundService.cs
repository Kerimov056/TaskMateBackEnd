
using Microsoft.EntityFrameworkCore;
using TaskMate.Context;

namespace TaskMate.BackGroundServıces;

public class TokenBackgroundService : IHostedService
{
    private IServiceProvider _serviceProvider;
    private Timer _timer;

    public TokenBackgroundService(IServiceProvider serviceProvider)
   => _serviceProvider = serviceProvider;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(TokenBackgroundService)}Service started....");
        _timer = new Timer(tokenCheck, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }
    private async void tokenCheck(object state)
    {
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var today = DateTime.Now;

            var tokens = await _dbContext.Tokens
                .Where(x => x.CreateToken.Hour == today.Hour && x.CreateToken.Minute == today.Minute)
                .ToListAsync();

            foreach (var item in tokens)
            {
                var byToken = await _dbContext.Tokens.FirstOrDefaultAsync(x=>x.TokenId==item.TokenId);
                _dbContext.Tokens.Remove(byToken);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        Console.WriteLine($"{nameof(TokenBackgroundService)}Service stopped....");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer = null;
    }
}
