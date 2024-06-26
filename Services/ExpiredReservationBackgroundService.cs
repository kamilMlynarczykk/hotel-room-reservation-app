using HotelReservationApp.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ExpiredReservationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Check every 24 hours

    public ExpiredReservationBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var reservationHistoriesController = scope.ServiceProvider.GetRequiredService<ReservationHistoriesController>();
                await reservationHistoriesController.MoveExpiredReservationsToHistory();
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
