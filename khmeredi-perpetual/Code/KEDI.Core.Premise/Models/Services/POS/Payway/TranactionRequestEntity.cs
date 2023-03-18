
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABA.Payway.Models
{
    public class TranactionRequestEntity
    {
        public string tran_id { get; set; }
        public string amount { get; set; }
        public string hash { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }
}
