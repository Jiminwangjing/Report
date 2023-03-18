using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class DisplayCurrencyModel
    {
        public int ID { get; set; }
        public int BaseCurrencyID { get; set; }
        public int AltCurrencyID { get; set; }
        public string BaseCurrency { get; set; }
        public string AltCurrency { get; set; }
        public decimal Rate { get; set; }
        public decimal AltRate { get; set; }
        public List<DisplayCurrencyModel> DisplayAltCurrency { get; set; }
    }

    public class DisplayPayCurrencyModel
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public string LineID { get; set; }
        public string BaseCurrency { get; set; }
        public decimal Amount { get; set; }
        public string AltCurrency { get; set; }
        public decimal AltAmount { get; set; }
        public decimal Rate { get; set; }
        public decimal AltRate { get; set; }
        public decimal SCRate { get; set; }
        public decimal LCRate { get; set; }
        public int BaseCurrencyID { get; set; }
        public int AltCurrencyID { get; set; }
        public string AltSymbol { get; set; }
        public string BaseSymbol { get; set; }
        public bool IsLocalCurrency { get; set; }
        public bool IsShowCurrency { get; set; }
        public bool IsActive { get; set; }
        public bool IsShowOtherCurrency { get; set; }
        public double DecimalPlaces { get; set; }
    }
}
