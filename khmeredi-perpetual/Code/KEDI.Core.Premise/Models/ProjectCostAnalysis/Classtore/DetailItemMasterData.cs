using CKBS.Models.Services.Banking;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.ProjectCostAnalysis
{
    public class DetailItemMasterData
    {
        public int ID { get; set; }
        public int ProjCostID { get; set; }
        public int GUomID { get; set; }
        public int UomID { get; set; }
        public int ItemID { get; set; }
        public int TaxGroupID { get; set; }
        public int CurrencyID { get; set; }
        public string LineID { get; set; }
        public string ItemCode { get; set; }
        public string BarCode { get; set; }
        public string Description { get; set; }
        public string ItemNameKH { get; set; }
        public string ItemNameEN { get; set; }
        public decimal Qty { get; set; }
        public decimal OpenQty { get; set; }
        public List<SelectListItem> UoMs { get; set; }
        public string Currency { get; set; }
        public decimal Cost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DisRate { get; set; }
        public decimal DisValue { get; set; }
        public decimal UnitPriceAfterDis { get; set; }
        public decimal LineTotalBeforeDis { get; set; }
        public decimal Total { get; set; } // Column Name Total After Discount
        public decimal LineTotalCost { get; set; }
        public List<SelectListItem> TaxGroupList { get; set; }
        public string UomName { get; set; }
        public decimal Factor { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxValue { get; set; }
        public decimal TaxOfFinDisValue { get; set; }
        public decimal TaxDownPaymentValue { get; set; }
        public decimal FinDisRate { get; set; } // Final Discount Rate
        public decimal FinDisValue { get; set; } // Final Discount Value
        public decimal UnitMargin { get; set; } // Final Discount Value
        public decimal TotalWTax { get; set; }
        public decimal LineTotalMargin { get; set; }
        public double InStock { get; set; }
        public decimal FinTotalValue { get; set; }

        public string Remarks { get; set; }
        public List<TaxGroupViewModel> TaxGroups { get; set; }
        public List<UOMSViewModel> UoMsList { get; set; }
        public string TypeDis { get; set; }
        public double VatRate { get; set; }
        public double VatValue { get; set; }
        public double TotalSys { get; set; }
        public string Process { get; set; }
        public DateTime ExpireDate { get; set; }
        public string ItemType { get; set; }
        public bool Delete { get; set; }
        public int BaseUoMID { get; set; }
        public int InvenUoMID { get; set; }
        public List<UomPriceList> UomPriceLists { get; set; }
    }
    public class ProjCostAnalysisupdate
    {
        public ProjCostAnalysisVeiw ProjectCostAnalysis { get; set; }
        public List<DetailItemMasterData> DetailItemMasterDatas { get; set; }
        public Currency Currency { get; set; }
    }
}
