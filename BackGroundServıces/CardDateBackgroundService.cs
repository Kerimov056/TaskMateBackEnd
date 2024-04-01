using Microsoft.EntityFrameworkCore;
using TaskMate.Context;

namespace TaskMate.BackGroundServıces;

public class CardDateBackgroundService : IHostedService
{
    private IServiceProvider _serviceProvider;
    private Timer _timer;

    public CardDateBackgroundService(IServiceProvider serviceProvider)
   => _serviceProvider = serviceProvider;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"{nameof(CardDateBackgroundService)}Service started....");
        _timer = new Timer(CardDate, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }
    private async void CardDate(object state)
    {
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var today = DateTime.Now;

            var cards = await _dbContext.Cards.Where(x => x.Reminder != null
                                            && x.IsDateStatus !=null
                                            && x.IsDateStatus == false
                                            && x.Reminder.Value.Day == today.Day
                                            && x.Reminder.Value.Hour == today.Hour
                                            && x.Reminder.Value.Minute == today.Minute).ToListAsync();

            foreach (var item in cards)
            {
                var byCard = await _dbContext.Cards.FirstOrDefaultAsync(x => x.Id == item.Id);
                byCard.DateColor = "red";
                _dbContext.Cards.Update(byCard);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        Console.WriteLine($"{nameof(CardDateBackgroundService)}Service stopped....");
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer = null;
    }
}
