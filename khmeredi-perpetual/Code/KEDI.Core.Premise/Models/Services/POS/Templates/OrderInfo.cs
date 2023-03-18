using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.POS.service;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CKBS.Models.Services.POS.Template
{
    public class OrderInfo
    {
        public Company Company { get; set; }
        public Branch Branch { get; set; }
        public GeneralSetting Setting { get; set; }
        public AuthorizationOptions AuthOption { set; get; }
        public CardMemberOptions CardMemberOption { get; set; }
        public List<Order> Orders { get; set; }
        public Order Order { get; set; }
        public Table OrderTable { get; set; }
        public List<GroupTable> GroupTables { set; get; }
        public List<ServiceItemSales> BaseItemGroups { get; set; }
        public List<ServiceItemSales> ItemGroups { get; set; }
        public List<ServiceItemSales> SaleItems { get; set; }
        public List<FreightReceipt> Freights { set; get; }
        public List<ItemUoM> ItemUoMs { set; get; }
        public List<TaxGroupModel> TaxGroups { set; get; }
        public Dictionary<int, string> PromoTypes { set; get; }
        public LoyaltyProgModel LoyaltyProgram { set; get; }
        public List<SeriesInPurchasePoViewModel> SeriesPS { get; set; }
        public List<SelectListItem> RemarkDiscountItem { get; set; }
        public List<DisplayPayCurrencyModel> DisplayPayOtherCurrency { get; set; }
        public List<SelectListItem> DisplayPayOtherCurrencyDefualt { get; set; }
        public List<DisplayPayCurrencyModel> DisplayTotalAndChangeCurrency { get; set; }
        public List<DisplayPayCurrencyModel> DisplayTotalAndReceiveCurrency { get; set; }
        public List<DisplayPayCurrencyModel> DisplayGrandTotalOtherCurrency { get; set; }
        public List<SeriesInPurchasePoViewModel> SeriesCR { get; set; }
        public List<PaymentMeans> PaymentMeans { get; set; }
        public string[] SlideImageNames { get; set; }
    }
}
