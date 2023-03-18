namespace CKBS.Models.Services.AlertManagement
{
    public class UserAlert
    {
        public int ID { get; set; }
        public int AlertDetailID { get; set; }
        public int UserAccountID { get; set; } 
        public string UserName { get; set; }
        public string TelegramUserID { get; set; }
        public bool IsAlert { get; set; }
        public int CompanyID { get; set; }
    }
}
