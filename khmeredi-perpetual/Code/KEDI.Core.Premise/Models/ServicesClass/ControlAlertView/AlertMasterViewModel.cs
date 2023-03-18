using CKBS.Models.Services.AlertManagement;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.ControlAlertView
{
    public class AlertMasterViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public TypeOfAlert TypeOfAlert { get; set; }
        public List<SelectListItem> TypeOfAlerts { get; set; }
        public bool Active { get; set; }
        public DeleteAlert DeleteAlert { get; set; }
        public long Frequently { get; set; }
        public TypeFrequently TypeFrequently { get; set; }
        public TypeFrequently TypeBeforeAppDate { get; set; }
        public long BeforeAppDate { get; set; }
        public int CompanyID { get; set; }
    }
}
