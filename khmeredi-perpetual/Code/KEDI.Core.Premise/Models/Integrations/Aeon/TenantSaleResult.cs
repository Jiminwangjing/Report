using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Integrations.Aeon
{
    public class TenantSaleResult
    {
        public string transaction_oid { get; set; }
        public string invoice_id { get; set; }
        public string date_time { get; set; }
        public string message { get; set; }
    }

}