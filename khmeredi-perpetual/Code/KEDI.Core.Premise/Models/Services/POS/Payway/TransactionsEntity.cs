namespace PayWayIntegration.Controllers
{
    public class TransactionsEntity
    {
        public string tran_id { get; set; }
        public decimal amount { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string hash { get; set; }
    }
}