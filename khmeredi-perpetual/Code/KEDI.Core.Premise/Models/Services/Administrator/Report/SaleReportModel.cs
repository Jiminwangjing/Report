using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Report
{
    public class SaleReportModel
    {
        public IEnumerable<MonthlySale> MonthlySales { get; set; }
        public IEnumerable<SaleByGroup> SaleByGroups { get; set; }
        public IEnumerable<TopSale> TopSales { get; set; }
        public IEnumerable<Stock>Stocks { get; set; }
        public IEnumerable<AccountReceivable> AccountReceivables { get; set; }
        public GetCurrency BalanceTotal { get; set; }
        public GetCurrency Balance { get; set; }
        public GetCurrency AverageReceipts { get; set; }
        public GetCurrency AverageQty { get; set; }
    }
    public class GetCurrency
    {
        public decimal BalanceTotal { get; set; }
        public string Currency { get; set; }
    }
}
