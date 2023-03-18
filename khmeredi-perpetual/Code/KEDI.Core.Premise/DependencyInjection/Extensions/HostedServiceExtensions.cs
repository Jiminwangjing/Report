using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AlertManagementsServices.AlertManagementServices;
using KEDI.Core.Premise.Repositories.Integrations;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Premise.Services;
using KEDI.Core.Premise.Services.AlertService.AlertServices;
using Microsoft.Extensions.DependencyInjection;

namespace KEDI.Core.Premise.DependencyInjection.Extensions
{
    public static class HostedServiceExtensions
    {
        public static IServiceCollection AddKediHostedServices(this IServiceCollection services){
            services.AddHostedService<Synchronizer>();
            services.AddHostedService<AlertDueDateService>();
            services.AddHostedService<AlertStockService>();
            services.AddHostedService<ExpirationStockItemService>();
            return services;
        }
    }
}