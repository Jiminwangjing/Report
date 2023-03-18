namespace Models.Services.ReportSale.dev
{
    public class StockCount
    {
        public int ReceiptDetailID { set; get; }
        public int ItemID { set; get; }
        public double Qty { set; get; }
        public int UomID { set; get; }
        public double Factor { set; get; }
    }
}