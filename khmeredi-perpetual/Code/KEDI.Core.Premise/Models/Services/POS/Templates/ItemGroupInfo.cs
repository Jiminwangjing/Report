using CKBS.Models.Services.POS.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class GroupItemInfo
    {
        public List<ServiceItemSales> Items { get; set; }
        public int Level { get; set; }
    }

    public class GroupItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Group1 { get; set; }
        public int Group2 { get; set; }
        public int Group3 { get; set; }      
        public bool Visible { get; set; }
    }

}
