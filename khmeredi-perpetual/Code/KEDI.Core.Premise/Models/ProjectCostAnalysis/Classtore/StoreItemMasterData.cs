using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore
{
    public class StoreItemMasterData
    {
    
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double InStock { get; set; }
        public double UnitPrice { get; set; }
        public int CurrencyID { get; set; }
        public string Currency { get; set; }
        public int UomID { get; set; }
        public string UoM { get; set; }
        public string Barcode { get; set; }
        public int PricListID { get; set; }
        public int GroupUomID { get; set; }
        
    }
}
