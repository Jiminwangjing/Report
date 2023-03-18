using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.POS.Templates
{
    public class CurrentOrderInfo
    {
        public Table OrderTable { set; get; }
        public List<Order> Orders { set; get; }
        public Order Order { set; get; } 
        public List<DisplayPayCurrencyModel> DisplayPayOtherCurrency { get; set; }
        public List<DisplayPayCurrencyModel> DisplayTotalAndChangeCurrency { get; set; }
        public List<DisplayPayCurrencyModel> DisplayTotalAndReceiveCurrency { get; set; }
        public List<DisplayPayCurrencyModel> DisplayGrandTotalOtherCurrency { get; set; }
        public List<ServiceItemSales> SaleItems { set; get; }
    }
}
