using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class ShiftForm
    {
        public int ID { set; get; }
        public string Decription { get; set; }
        public double InputCash { get; set; }       
        public string Currency { get; set; }
        public double RateIn { get; set; }
        public double Amount { get; set; }
    }

    public class ShiftTemplate
    {
        public double GrandTotalSys { set; get; }
        public string CurrencySys { set; get; }
        public List<ShiftForm> ShiftForms { set; get; }
    }
}
