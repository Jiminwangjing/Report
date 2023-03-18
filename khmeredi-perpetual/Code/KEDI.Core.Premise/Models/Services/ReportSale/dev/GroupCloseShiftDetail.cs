using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupCloseShiftDetail
    {
        public string GroupName {get;set;}
        public string SubTotal { get; set; }
        public HeaderCloseShift HeaderCloseShift { get; set; }
        public List<DetailItem> DetailItems { get; set; }
        public Footer Footer { get; set; }
    }
    public class HeaderCloseShift
    {
        public string Logo { get; set; }
        public string Branch { get; set; }
        public string EmpName { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string ExchangeRate { get; set; }
    }
    
}
