using System.Threading.Tasks;

namespace TelegramBotAPI.Models
{
    public interface ITelegramApiCleint
    {
        Task SendMessageAsync(TelegramSendMessageRequest request);
        Task SendPhotoAsync(SendPhotoRequest request);
        Task<dynamic> GetDetailAsync(TelegramSendMessageRequest request);
        //Task InputTextMessageContent(string token);
    }
}