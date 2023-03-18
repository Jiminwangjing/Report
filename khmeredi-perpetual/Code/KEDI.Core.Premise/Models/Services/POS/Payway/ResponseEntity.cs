using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondScreenPos.Model.Payway
{
    public class ResponseEntity
    {
        public string status { get; set; }
        public string description { get; set; }
        public string qrString { get; set; }
        public string qrImage { get; set; }
    }
}
