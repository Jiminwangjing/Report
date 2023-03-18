using System;

namespace KEDI.Core.Premise.Models.Services.AlertManagement
{
    public class ExpirationStockItem
    {
        public int ID { get; set; }
        public int ItemId { get; set; }
        public int WarehouseId { get; set; }
        public int WareDId { get; set; }
        public bool IsRead { get; set; } = false;
        public string Type { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemBarcode { get; set; }
        public string BatchNo { get; set; }
        public string BatchAttribute1 { get; set; }
        public string BatchAttribute2 { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? AddmissionDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public decimal Instock { get; set; }
        public string UomName { get; set; }
        public string WarehouseName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string ImageItem { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public int UomID { get; set; }
    }
}
