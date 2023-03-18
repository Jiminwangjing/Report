using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Integrations.Aeon
{
    public class TenantSaleViewModel
    {
        public string receipt_id { get; set; }
        public string document_type { get; set; }
        public string date_time { get; set; }
        public string currency_name { get; set; }
        public string discount_type { get; set; }
        public string discount_amount { get; set; }
        public string delivery_service { get; set; }
        public string exchange_rate_value { get; set; }
        public string change_amount_dollar { get; set; }
        public string change_amount_base { get; set; }
        public string vat { get; set; }
        public string cashier_id { get; set; }
        public string amount_before_vat_discount { get; set; }
        public bool Synced { get; set; }
        public DateTime SyncedDate { get; set; }
    }
}