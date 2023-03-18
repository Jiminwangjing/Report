using CKBS.Models.Services.Administrator.Inventory;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Report
{
    public class ItemBatchView
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string StockIn { get; set; }
        public string StockCommit { get; set; }
        public string Cost { get; set; }
        public string UniPrice { get; set; }
        public string Process { get; set; }
        public string BatchNo { get; set; }
        public string StockOut { get; set; }
        public string Systemdate { get; set; }
        public string Expiredate { get; set; }
        public string BatchAttr1 { get; set; }
        public string BatchAttr2 { get; set; }
        public string Admisiondate { get; set; }
        public string TotalStockOut { get; set; }
        public string TotalStockIn { get; set; }
        public string Transaction { get; set; }
        public string InvoiceNo { get; set; }
        public TransTypeWD TransType { get; set; }
        public int TransID { get; set; }
        public string InvoceTrand { get; set; }
        public string ItemNameOut { get; set; }
        public string SerNumberout { get; set; }
        public int BranchID { get; set; }
        public string SystemdateOut { get; set; }
        public string ExpiredateOut { get; set; }
        public string AdmisiondateOut { get; set; }
        public string TransactionIn { get; set; }
        public string TransactionOut { get; set; }
        public string ProseccIn { get; set; }
        public string Time { get; set; }
        //////
        public string MfrSerialNo { get; set; }
        public string SerialNumber { get; set; }
        public string PlateNumber { get; set; }
        public string LotNumber { get; set; }
        public string Status { get; set; }
        public string ValuationMethod { get; set; }
        public string MfrDate { get; set; }
        public string InType { get; set; }
        public string InDoc { get; set; }
        public string InDate { get; set; }
        public string InQuantity { get; set; }
        public string InWarehouse { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string OutType { get; set; }
        public string OutDoc { get; set; }
        public string OutDate { get; set; }
        public string OutQuantity { get; set; }
        public string OutWarehouse { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string MfrWarrantyStart { get; set; }
        public string MfrWarrantyEnd { get; set; }
        public int StockOnHand { get; set; }
        public int CountStockIn { get; set; }
        public int CountStockOut { get; set; }
        public string TotalGroup { get; set; }
        public string GrandTotal { get; set; }
    }
}
