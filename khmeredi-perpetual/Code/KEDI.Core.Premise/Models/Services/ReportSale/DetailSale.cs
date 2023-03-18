﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale
{
    [Table("rp_DetailSale",Schema ="dbo")]
    public class DetailSale
    {
        [Key]
        public int OrderID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public double DisItem { get; set; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public double UnitPrice { get; set; }

        
    }
}
