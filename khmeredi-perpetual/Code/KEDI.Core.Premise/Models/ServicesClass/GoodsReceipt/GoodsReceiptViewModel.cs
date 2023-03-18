using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.ServicesClass.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CKBS.Models.ServicesClass.GoodsReceipt
{
    public class GoodsReceiptViewModel
    {
        public int GoodReceitpDetailID { get; set; }
        public int GoodsReceiptID { get; set; }
        public int CurrencyID { get; set; }
        public int LineID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Quantity { get; set; }
        public double Cost { get; set; }
        public List<SelectListItem> Warehouse { get; set; }
        public string PaymentMeans { get; set; }
        public double AvgCost { get; set; }
        public double CostStore { get; set; }
        public string Currency { get; set; }
        public string UomName { get; set; }
        public string BarCode { get; set; }
        public string Check { get; set; }
        public string ManageExpire { get; set; }
        public List<SelectListItem> UoMs { get; set; }
        public string Type { get; set; }
        public DateTime ExpireDate { get; set; }
        public List<UOMSViewModel> UOMView { get; set; }
        public double MaxOrderQty { get; set; }
        public double MinOrderQty { get; set; }
        public bool IsLimitOrder { get; set; }
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
}
