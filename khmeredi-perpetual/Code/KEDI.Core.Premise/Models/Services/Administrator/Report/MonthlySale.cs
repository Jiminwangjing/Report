using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Report
{
    public class MonthlySale
    {
        public int ReceiptID { get; set; }
        public int ItemID { get; set; }
        public int InVentoryAditID{ get; set; }
        public string ItemName { get; set; }
        public string Group1 { get; set; }
        public double SubTotal { get; set; }
        public double GrandTotal { get; set; }
        public string Month { get; set; }
        public int MonthIndex { get; set; }
        public double CumlativeValue { get; set; }
        //public string ItemNameStock { get; set; }
        public DateTime DateOut { get; set; }
        public int Group1ID { get; set; }
        public double TotalItem { get; set; }
        public int Sarid { get; set; }
    }
}
