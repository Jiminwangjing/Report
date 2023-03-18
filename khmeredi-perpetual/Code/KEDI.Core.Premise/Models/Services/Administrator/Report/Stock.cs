using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Report
{
    public class Stock
    {
        public int ItemID { get; set; }
        public double CumlativeValue { get; set; }
        public string ItemNameStock { get; set; }
        public DateTime LastDate { get; set; }
        public int WarehouseID { get; set; }
    }
}
