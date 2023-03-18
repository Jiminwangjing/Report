using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Services.GeneralSettingAdmin
{
    public class DisplayViewModel
    {
        public string LineID { get; set; }
        public int ID { get; set; }
        public string Currency { get; set; }
        public int Amounts { get; set; }
        public int Prices { get; set; }
        public int Rates { get; set; }
        public int Quantities { get; set; }
        public int Percent { get; set; }
        public int Units { get; set; }
        public string DecimalSeparator { get; set; } = ".";
        public string ThousandsSep { get; set; } = ",";
        public int DisplayCurrencyID { get; set; }
    }
}
