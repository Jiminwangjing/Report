using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Inventory.Transaction
{
    public class ViewItemMasterData
    {
        public int ID { get; set; }
        public int GuomID{get;set;}
        public int WarehouseID{get;set;}
        public string GuomName{get;set;}
        public string BarCode { get; set; }
        public string ItemNo { get; set; } 
        public string ItemName { get; set; }
        public double InStock { get; set; }
        public double CountedQty{get;set;}
        public double Varaince{get;set;}
        public string Batch { get; set; }
        public string Serial { get; set; }
    }
}