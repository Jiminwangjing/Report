using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class ItemMaterial
    {
        public int ItemID { get; set; }
        public int GroupUoMID { get; set; }
        public int GUoMID { get; set; }
        public double Qty { get; set; }
        public bool NegativeStock { get; set; }
        public string Process { get; set; }
        public int UomID { get; set; }
        public float Factor { get; set; }
        public bool IsKsms { get; set; }
        public bool IsKsmsMaster { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
