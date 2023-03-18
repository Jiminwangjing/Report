using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBotAPI.Models;

namespace TelegramBotAPI
{
    public static class DependencyRegitration
    {
        public static void RegisterTelegramApiDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITelegramApiCleint, TelegramApiCleint>();
            services.Configure<TelegramApiSettings>(configuration);
            services.AddOptions<TelegramApiSettings>();
            services.AddHttpClient();
        }
    }
}
