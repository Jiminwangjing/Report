using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.AlertManagement
{
    public class AlertWarehouses
    {
        public int ID { get; set; }
        public int AlertDetailID { get; set; }
        public int WarehouseID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsAlert { get; set; }
        public int CompanyID { get; set; }
    }
}
