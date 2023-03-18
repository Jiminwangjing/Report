using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Integrations.Aeon
{
    public class TenantSale
    {
        public string transaction_oid { get; set; }
        public string receipt_id { get; set; }
        public string invoice_id { get; set; }
        public string document_type { get; set; }
        public string date_time { get; set; }
        public string currency_name { get; set; }
        public string discount_type { get; set; }
        public string discount_amount { get; set; }
        public string return_qty { get; set; }
        public string return_amount { get; set; }
        public string refund_qty { get; set; }
        public string refund_amount { get; set; }
        public string payment_method_1 { get; set; }
        public string payment_amount_1 { get; set; }
        public string payment_method_2 { get; set; }
        public string payment_amount_2 { get; set; }
        public string payment_method_3 { get; set; }
        public string payment_amount_3 { get; set; }
        public string delivery_service { get; set; }
        public string exchange_rate_value { get; set; }
        public string change_amount_dollar { get; set; }
        public string change_amount_base { get; set; }
        public string vat { get; set; }
        public string cashier_id { get; set; }
        public string amount_before_vat_discount { get; set; }
    }

    public enum TenantSaleType {All = 0, Ageing = 1, Return = 2 }
}