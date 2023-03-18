using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass
{
    public class SeriesInPurchasePOViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Default { get; set; }
        public string NextNo { get; set; }
        public int DocumentTypeID { get; set; }
        public int SeriesDetailID { get; set; }
    }
}
