using IRS.DAL.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IRS.BLL.Managers.AccountManager.Auth
{
    public class ExpiredOtpCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // كل 5 دقايق

        public ExpiredOtpCleanupService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<IRS_Context>();

                var expiredOtps = context.otps
                    .Where(x => x.Expiry <= DateTime.UtcNow);

                if (expiredOtps.Any())
                {
                    context.otps.RemoveRange(expiredOtps);
                    await context.SaveChangesAsync();
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
