using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using CKBS.AppContext;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using KEDI.Core.Models.Validation;
using Microsoft.Extensions.Configuration;
using KEDI.Core.Premise.Repository;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.Models.SignalR;
using System;
using Newtonsoft.Json;
using System.Diagnostics;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Repository;
using CKBS.Models.Services.Responsitory;
using System.Timers;
using KEDI.Core.Premise.Models.Integrations;

namespace KEDI.Core.Premise
{
    public interface IScheduler {
        Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval);
        Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval, CancellationToken ct);
        Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval, TimeSpan startTime, CancellationToken ct);
        Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval, TimeSpan startTime, TimeSpan endTime,  CancellationToken ct);
    }

    public class Scheduler: BackgroundService, IScheduler
    {
        private readonly ILogger<Scheduler> _logger;
        private TimeSpan Interval = TimeSpan.FromSeconds(1);
        private TimeSpan StartTime = TimeSpan.Zero;
        private TimeSpan EndTime = TimeSpan.FromHours(24);
        private Func<CancellationToken, Task> _process;
        public Scheduler(ILogger<Scheduler> logger)
        {
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000);
            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime startTime = DateTime.Today.Add(StartTime);
                DateTime endTime = DateTime.Today.Add(EndTime);
                DateTime now = DateTime.Now;
                if (startTime <= now && now <= endTime)
                {
                    await _process?.Invoke(stoppingToken);
                }

                await Task.Delay(Interval);
            }
        }

        public Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval)
        {
            return TickAsync(process, interval, CancellationToken.None);
        }

        public Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval, CancellationToken ct)
        {
            Interval = interval;
            _process = process;
            return base.StartAsync(ct);
        }

        public Task TickAsync(Func<CancellationToken, Task> process,  TimeSpan interval, 
            TimeSpan startTime, CancellationToken ct)
        {
            StartTime = startTime;
            return TickAsync(process, interval, ct);
        }

        public Task TickAsync(Func<CancellationToken, Task> process, TimeSpan interval, 
            TimeSpan startTime, TimeSpan endTime,  CancellationToken ct)
        {
            EndTime = endTime;
            return TickAsync(process, interval, startTime, ct);
        }
    }
}
