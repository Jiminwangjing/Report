using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.KAMSService
{
    public class OrderInfoOfQuote
    {
        public List<ServiceItemSales> BaseItemGroups { get; set; }
        public GeneralSetting Setting { get; set; }
        public List<OrderQAutoM> Orders { get; set; }
        public OrderQAutoM Order { get; set; }
        public Table OrderTable { get; set; }
        public DisplayCurrencyModel DisplayCurrency { get; set; }
        public List<ServiceItemSales> ServiceItems { get; set; }
    }
}
