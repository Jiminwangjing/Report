using CKBS.Models.Services.AlertManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.AlertViewClass
{
    public class StockAlertViewModel
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public double InStock { get; set; }
        public bool IsRead { get; set; }
        public double MinInv { get; set; }
        public double MaxInv { get; set; }
        public string WarehouseName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
    }
}
