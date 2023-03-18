using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass
{
    public class InstallmentDetailViewModel
    {
        public string UnitID { get; set; } = DateTime.Now.Ticks.ToString();
      
                    public int Day { get; set; }
                    public int InstallmentID { get; set; }
                    public int Months { get; set; }
                    public decimal Percent { get; set; } 
    }
}
