using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Transactions;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupTaxDeclaration
    {
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string User { get; set; }
        public string Receipt { get; set; }
        public string DateIn { get; set; }
        public string TimeIn { get; set; }
        public string DateOut { get; set; }
        public string TimeOut { get; set; }
        public double GrandTotal { get; set; }
        public double Tax { get; set; }
        public string LocalCurrency { get; set; }
        public Header Header { get; set; }
        public Footer Footer { get; set; }
    }
}
