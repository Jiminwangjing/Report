using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.ServicesClass.Property;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.Transfer
{
    public class TransferViewModel
    {
         public int ID { get; set; }
         public int TransferRequestID { get; set; }
        public int TarnsferDetailID { get; set; }
        public int TransferID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int LineID { get; set; }
        public int CurrencyID { get; set; }
         public int FWarehouseID { get; set; }
        public int TWarehouseID { get; set; }
        public List<SelectListItem> FWarehouse { get; set; }
        public List<SelectListItem> TWarehouse { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        
        public double QuantitySum { get; set; }
        public double InStock { get; set; }
        public double Cost { get; set; }
        public double CostStore { get; set; }
        public string Currency { get; set; }
        public string UomName { get; set; }
        public string Barcode { get; set; }
        public string Check { get; set; }
        public string ManageExpire { get; set; }
        public DateTime ExpireDate { get; set; }
        public double AvgCost { get; set; }
        public List<SelectListItem> UoMs { get; set; }
        public string Type { get; set; }
        public ItemMasterData ItemMasterData { get; set; }
        public UnitofMeasure UnitofMeasure { get; set; }
        public Currency Currencys { get; set; }
        public List<UOMSViewModel> UoMsList { get; set; }
        public int BaseOnID { get; set; }
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
}
