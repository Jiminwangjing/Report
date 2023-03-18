using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KEDI.Core.Premise.Utilities
{
    public interface IServiceWorker : IHostedService, IDisposable {}
    public abstract class ServiceWorker : BackgroundService, IServiceWorker
    {
        private delegate Task WatchHandler(CancellationToken ct);
        public TimeSpan StartTime { set; get; }
        public TimeSpan EndTime { set; get; }
        public TimeSpan Every { set; get; }
        protected abstract Task WorkAsync(CancellationToken stoppingToken);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(Every.TotalSeconds <= 1){ Every = TimeSpan.FromSeconds(1); }     
            while(!stoppingToken.IsCancellationRequested){     
                DateTimeOffset startTime = DateTime.Today.Add(StartTime);
                DateTimeOffset endTime = DateTime.Today.Add(EndTime);
                DateTimeOffset now = DateTimeOffset.Now;
                if (startTime <= now && now <= endTime)
                {
                    await WorkAsync(stoppingToken);        
                }     
                
                await Task.Delay(Every, stoppingToken);
            }
        }
    }
}