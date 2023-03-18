
using Microsoft.Extensions.Configuration;

namespace TelegramBotAPI
{
    public static class AppSettingFileRegisteration
    {
        public static IConfigurationBuilder RegisterTelegramApiSettingsFile(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddJsonFile("telegramapisettings.json");
            return configurationBuilder;
        }
    }
}
