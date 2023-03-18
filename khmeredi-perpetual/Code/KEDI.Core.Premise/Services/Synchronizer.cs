using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Premise.Repositories.Integrations;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Services;

namespace KEDI.Core.Premise
{
    public class Synchronizer : BackgroundService
    {  
        private readonly ILogger<Synchronizer> _logger;
        private readonly ISyncClientWorker _syncClient;
        private readonly ISyncServerWorker _syncServer;
        private readonly IAeonRepo _aeonRepo;
        private readonly IChipMongRepo _chipMong;
        private readonly IPosClientSignal _clientsignal;
        public Synchronizer(ILogger<Synchronizer> logger,
            IServiceScopeProvider provider)
        {
            _logger = logger;
            _syncServer = provider.GetRequiredService<ISyncServerWorker>();
            _syncClient = provider.GetRequiredService<ISyncClientWorker>();
            _aeonRepo = provider.GetRequiredService<IAeonRepo>();
            _chipMong = provider.GetRequiredService<IChipMongRepo>();
            _clientsignal = provider.GetRequiredService<IPosClientSignal>();           
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _clientsignal.StartAsync(stoppingToken);
            await _syncServer.StartAsync(stoppingToken);
            await _syncClient.StartAsync(stoppingToken);           
            await _aeonRepo.StartAsync(stoppingToken);
            await _chipMong.StartAsync(stoppingToken);         
        }

    }
}
