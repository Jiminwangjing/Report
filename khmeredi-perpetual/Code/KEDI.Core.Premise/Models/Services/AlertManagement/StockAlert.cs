using System;

namespace CKBS.Models.Services.AlertManagement
{
    public class StockAlert
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public double InStock { get; set; }
        public bool IsRead { get; set; } = false;
        public double MinInv { get; set; }
        public double MaxInv { get; set; }
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public StockType StockType { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
    }
    public enum StockType
    {
        Null,
        MinStock,
        MaxStock
    }
}
