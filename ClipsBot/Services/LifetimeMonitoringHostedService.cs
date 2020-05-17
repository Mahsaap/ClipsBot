using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ClipsBot.Services
{
    public class LifetimeMonitoringHostedService : IHostedService
    {
        private readonly ILogger<LifetimeMonitoringHostedService> _logger;

        public LifetimeMonitoringHostedService(ILogger<LifetimeMonitoringHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Application started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Application shutting down");

            return Task.CompletedTask;
        }
    }
}
