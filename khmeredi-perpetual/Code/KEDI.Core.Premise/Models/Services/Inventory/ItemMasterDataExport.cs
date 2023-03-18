using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Inventory
{
    public class ItemMasterDataExport
    {
        public int No {get; set;} 
        public string Code { get; set; } // Code
        public string ItemName1 { get; set; } //KhmerName
        public string ItemName2 { get; set; } //EnglishName
        public string UoM { get; set; } //InventoryUoMID
        public string ItemGroup1 { get; set; } //ItemGroup1ID
        public string Barcode { get; set; }
        public string Type { get; set; }//FIFO,Average,Standard/SEBA(SerialBatch)/FEFO //Process
        public double Stock { get; set; } //StockIn
    }
    
}