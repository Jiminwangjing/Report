namespace Models.Services.ReportSale.dev
{
    public class WarehouseStockItemView
    {
        public int ID { set; get; }
        public int BranchID { set; get; }
        public int WarehouseID { set; get; }
        public string Barcode { set; get; }
        public string Code { set; get; }
        public string Image { set; get; }
        public string KhmerName { set; get; }
        public string EnglishName { set; get; }
        public double StockIn { set; get; }
        //StockPending = receiptDetails.Sum(rd => rd.Qty * rd.Factor),
        public double StockPending { set; get; }
        public double StockCommit { set; get; }
        public double Ordered { set; get; }
        public string UomName { set; get; }
        public string CumulativeValue { set; get; }
        public string WhCode { set; get; }
        public decimal TotalCost { set; get; }
        public string CumulativeCost { set; get; }
    }
}