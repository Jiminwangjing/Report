using KEDI.Core.Premise.Models.Services.SearchItemView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Inventory
{
    public class ItemMasterSearch
    {
        public List<ItemMasterDataView> ItemMaster { get; set; }
        public List<RecieptView> Receipts { get; set; }
        public List<AccountBalanceView> AccountBalances { get; set; }
        public List<ItemAccountView> ItemAccounts { get; set; }
        public List<CompanyView> Companys { get; set; }
        public List<BrandsView> BrandsViews { get; set; }
        public List<UserAccountView> UserAccounts { get; set; }
        public List<PrinterNameView> PrinterNames { get; set; }
        public List<ItemGroup1view> ItemGroup1s { get; set; }
        public List<ItemGroup2view> ItemGroup2s { get; set; }
        public List<ItemGroup3view> ItemGroup3s { get; set; }
        public List<Propertiesview> Property { get; set; }
        public List<Tableview> Table { get; set; }
        public List<GroupTableview> Groups { get; set; }
        public List<Freightview> Freights { get; set; }
        public List<Remarkdis> Remarkdis { get; set; }
        public List<PeriodIndicatorview> Per { get; set; }
        public List<Postingperiodview> Posting { get; set; }
        public List<ChartAccview> Chartsacc { get; set; }
        public List<JournalEntryview> Journal { get; set; }
        public List<Pricelistview> Pricelist { get; set; }
        public List<BusinesPartnerview> Busines { get; set; }
        public List<Cardsmemberview> Cardsmember { get; set; }
        public List<Currencyview> Currency { get; set; }
        public List<Paymeantview> Payment { get; set; }
        public List<Employeeview> Emplyee { get; set; }
        public List<Setupserviceview> Setup { get; set; }
        public List<Receiptinformationview> ReceiptIfo { get; set; }
        public List<Functionview> Function { get; set; }
        public List<Unitofmeasureview> Unitmeasure { get; set; }
        public List<GUOMview> Guom { get; set; }
        public List<Warhouseview> Warhous { get; set; }
        public int TotalItems { get; set; }
        


    }

}
