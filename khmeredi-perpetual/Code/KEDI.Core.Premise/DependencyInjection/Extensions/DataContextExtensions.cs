using System;
using System.Linq;
using CKBS.AppContext;
using KEDI.Core.Premise.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KEDI.Core.Premise.DependencyInjection.Extensions
{
    public static class DataContextExtensions
    {
        public static IServiceCollection AddDataContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(option =>
                option.UseSqlServer(config["UsersConnection:ConnectionString"])
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()   
            );
            
            services.AddSingleton<IServiceScopeProvider, ServiceScopeProvider>();
            return services;
        }

        public static IQueryable<object> Set(this DbContext dbContext, Type entityType)
        {
            return (IQueryable<object>) dbContext.GetType().GetMethod("Set").MakeGenericMethod(entityType).Invoke(dbContext, null);
        }
    }
}
