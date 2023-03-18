using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportInventory
{
    public class PrintStockinWarehouse
    {
        public string AddressEng { get; set; }
        public string WhCode { get; set; }
        public int ID { get; set; }
        public string Logo { get; set; }
        public string ItemCode { get; set; }
        public string ItemBarcode { get; set; }
        public string Uom { get; set; }
        public string itemName { get; set; }
        public string EngName { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public string Addresskh { get; set; }
        public double StockIn { get; set; }
        public double Orderedin { get; set; }
        public double CommittedName { get; set; }
        public string Brand { get; set; }
    }
}
