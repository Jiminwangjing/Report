namespace PayWayIntegration.Controllers
{
    public class Transaction
    {
        public string order_id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string datetime { get; set; }
        public decimal total_amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
        public string apv { get; set; }
        public string payment_type { get; set; }
    }
}