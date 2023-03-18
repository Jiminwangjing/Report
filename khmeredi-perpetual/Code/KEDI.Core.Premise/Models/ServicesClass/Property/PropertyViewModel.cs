using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.Property
{
    public class PropertyViewModel
    {
        public int ID { get; set; }
        public string UnitID { get; set; } 
        public int ProID { get; set; }
        public string NameProp { get; set; }
        public string NamePropNoSpace { get; set; }
        public List<SelectListItem> Values { get; set; }
        public int ItemID { get; set; }
        public int Value { get; set; }
        public bool Active { get; set; }
    }
    public class PropertydetailsViewModel
    {
        public int ID { get; set; }
        public int ProID { get; set; }
        public int ItemID { get; set; }
        public int Value { get; set; }
        public string ValueName { get; set; }
    }
}
