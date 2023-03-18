using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.KSMS
{
    public class ServiceSetUpViewModel
    {
        public int ID { get; set; }
        public int PriceListID { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SetupCode { get; set; }
        public decimal Price { get; set; }
        public string ItemName { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string UomName { get; set; }
        public string UserName { get; set; }
        public string Remark { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
    }
    public class ServiceSetUpDetailModelView 
    {
        public int ID { get; set; }
        public int ServiceSetupID { get; set; }
        public int CurrencyID { get; set; }
        public int ItemID { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public List<SelectListItem> UomList{ get; set; }
        public decimal Cost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Factor { get; set; }
        public List<UomPriceList> UomPriceLists { get; set; }
        public List<UOMSViewModel> UoMsList { get; set; }
    }
    public class ServiceMasterDataShow
    {
        public ServiceSetUpViewModel ServiceSetUpView { get; set; }
        public List<ServiceSetUpDetailModelView> ServiceSetUpDetailModels { get; set; }
    }
    public class SeriveItemMasterData
    {
        public int ID { get; set; }
        public int UomID { get; set; }
        public string Code { get; set; }
        public string ItemName1 { get; set; }
        public string Barcode { get; set; }
        public string Uom { get; set; }
        public string ItemName2 { get; set; }
    }

    public class ServiceSetUpHistory
    {
        public int ID { get; set; }
        public string SetupCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UserName { get; set; }
        public string PriceList { get; set; }
        public string Currency { get; set; }
        public string Price { get; set; }
        public string Uom { get; set; }
        public string CreationDate { get; set; }
        public bool Active { get; set; }
    }
}
