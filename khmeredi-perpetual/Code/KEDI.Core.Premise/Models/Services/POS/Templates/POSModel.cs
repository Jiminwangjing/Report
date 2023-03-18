using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.POS.service;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.Template
{
    public class POSModel
    {
        public ServiceTable ServiceTable { get; set; }
        public List<ItemGroup1> ItemGroup1s { get;set; }
        public SettingModel Setting { get; set; }
        public BusinessPartner Customer { get; set; }
        public List<AutoMobile> AutoMobiles { get; set; }
        public List<SelectListItem> Warehouses { set; get; }
        public string Templateurl { get; set; }
    }
}
