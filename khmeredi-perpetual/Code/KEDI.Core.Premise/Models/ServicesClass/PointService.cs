using CKBS.Models.Services.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class PointService
    {
        public int PointID { get; set; }
        public int Promotion { get; set; }
        public double Amount { get; set; }
        public int Point { get; set; }
        public int SetPoint { get; set; }
        public string Action { get; set; }
        public List<ServicePointDetail> ServicePointDetails { get; set; }
    } 
}
