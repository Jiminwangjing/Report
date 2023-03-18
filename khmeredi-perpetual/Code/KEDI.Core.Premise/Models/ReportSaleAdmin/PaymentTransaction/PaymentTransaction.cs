using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ReportSaleAdmin.PaymentTransaction
{
    public class PaymentTransaction
    {
      public int SeriesID { get; set; }
      public double Appiled { get; set; }
      public DateTime PostingDate { get; set; }
    }
}
