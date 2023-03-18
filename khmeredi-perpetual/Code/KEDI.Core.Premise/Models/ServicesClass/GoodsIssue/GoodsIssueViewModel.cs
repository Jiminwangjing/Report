using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.ServicesClass.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CKBS.Models.ServicesClass.GoodsIssue
{
    public class GoodsIssueViewModel
    {
        public int GoodIssuesDetailID { get; set; }
        public int GoodIssuesID { get; set; }
        public int CurrencyID { get; set; }
        public int LineID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Quantity { get; set; }
        public double QuantitySum { get; set; }
        public double InStock { get; set; }
        public double Cost { get; set; }
        public double CostStore { get; set; }
        public string Currency { get; set; }
        public string UomName { get; set; }
        public string BarCode { get; set; }
        public List<SelectListItem> Warehouse { get; set; }
        public string PaymentMeans { get; set; }
        public string Check { get; set; }
        public string ManageExpire { get; set; }
        public List<SelectListItem> UoMs { get; set; }
        public string Type { get; set; }
        public decimal Factor { get; set; }
        public double AvgCost { get; set; }
        public List<UOMSViewModel> UoMsList { get; set; }
        public DateTime ExpireDate { get; set; }
        public ItemMasterData ItemMasterData { get; set; }
        public UnitofMeasure UnitofMeasure { get; set; }
        public Currency Currencys { get; set; }
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
    public class UOMSViewModel
    {
        public double Factor { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int BaseUoMID { get; set; }
    }
    public class WHViewModel
    {
        public int ID { get; set; }
        public int BranchID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
