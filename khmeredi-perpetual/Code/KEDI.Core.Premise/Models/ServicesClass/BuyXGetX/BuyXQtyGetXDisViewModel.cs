using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.BuyXGetX
{
    public class BuyXQtyGetXDisViewModel
    {
        public string LineID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime DateF { get; set; }
        public DateTime DateT { get; set; }
        public int BuyItemID { get; set; }
        public string BuyItem { get; set; }
        public decimal Qty { get; set; }
        public int UomID { get; set; }
        public List<SelectListItem> UomSelect { get; set; }
        public int DisItemID { get; set; }
        public string DisItem { get; set; }
        public decimal DisRate { get; set; }
        public bool Active { get; set; }
    }
}
