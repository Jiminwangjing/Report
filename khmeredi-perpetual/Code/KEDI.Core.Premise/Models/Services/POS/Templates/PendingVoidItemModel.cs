namespace KEDI.Core.Premise.Models.Services.POS.Templates
{
    public class PendingVoidItemModel
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public string QueueNo { get; set; }
        public string Cashier { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Table { get; set; }
        public string Amount { get; set; }
        public bool IsVoided { get; set; }
    }
}
