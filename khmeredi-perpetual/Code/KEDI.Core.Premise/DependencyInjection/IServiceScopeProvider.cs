using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace KEDI.Core.Premise.DependencyInjection
{
    public interface IServiceScopeProvider
    {
        TService GetService<TService>();
        TService GetRequiredService<TService>();
        IEnumerable<TService> GetServices<TService>();
    }

    public class ServiceScopeProvider : IServiceScopeProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public ServiceScopeProvider(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public TService GetRequiredService<TService>()
        {
            var scope = _scopeFactory.CreateScope();        
            return scope.ServiceProvider.GetRequiredService<TService>();
        }

        public TService GetService<TService>()
        {
            var scope = _scopeFactory.CreateScope();   
            return scope.ServiceProvider.GetService<TService>();
        }

        public IEnumerable<TService> GetServices<TService>()
        {
            var scope = _scopeFactory.CreateScope(); 
            return scope.ServiceProvider.GetServices<TService>();
        }
    }
}