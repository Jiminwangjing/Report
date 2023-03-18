using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.PrintBarcode
{
    public class ItemPrintBarcodeView
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemBarcode { get; set; }
   
        public string ItemName { get; set; }
        public string UnitPrice { get; set; }
        public string ItemUnitprice { get; set; }
        public bool IsSelected { get; set; } = false;
        public double CostLocal { get; set; }
        public string ItemName2 { get; set; }
        public string Description { get; set; }
        public decimal CostSystem { get; set; }
        public string DescriptionChange { get; set; }
        public string Itemdesscription { get; set; }
        public string Setting { get; set; }
        public int Count { get; set; }
        public string ItemDes { get; set; }
        public string Salebydate { get; set; }
        public string NetWeight { get; set; }
    }

    public class PrinterNameModel
    {
        public List<ItemPrintBarcodeView> ItemPrintBarcodes { set; get; }
        public string Printername { get; set; }

    }
}
