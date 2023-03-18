using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Promotions;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using System.Collections.Generic;
using PointCard = KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints.PointCard;

namespace KEDI.Core.Premise.Models.Sync.Customs.Server
{
    public class ServerSyncContainer
    {
        public IEnumerable<EntryMap<Company>> Companies { set; get; }
        public IEnumerable<EntryMap<Branch>> Branches { set; get; }
        public IEnumerable<EntryMap<Employee>> Employees { set; get; }
        public IEnumerable<EntryMap<Function>> Functions { set; get; }
        public IEnumerable<EntryMap<UserAccount>> UserAccounts { set; get; }
        public IEnumerable<EntryMap<UserPrivillege>> UserPrivilleges { set; get; }
        public IEnumerable<EntryMap<Promotion>> Promotions { set; get; }
        public IEnumerable<EntryMap<Currency>> Currencies { set; get; }
        public IEnumerable<EntryMap<ExchangeRate>> ExchangeRates { set; get; }
        public IEnumerable<EntryMap<PrinterName>> PrinterNames { set; get; }

        public IEnumerable<EntryMap<UnitofMeasure>> UoMs { set; get; }
        public IEnumerable<EntryMap<GroupUOM>> GroupUOMs { set; get; }
        public IEnumerable<EntryMap<GroupDUoM>> GroupDefinedUOMs { set; get; }
        public IEnumerable<EntryMap<PriceLists>> PriceLists { set; get; }
        public IEnumerable<EntryMap<PriceListDetail>> PriceListDetails { set; get; }
        public IEnumerable<EntryMap<GroupTable>> GroupTables { set; get; }
        public IEnumerable<EntryMap<Table>> Tables { set; get; }
        public IEnumerable<EntryMap<Warehouse>> Warehouses { set; get; }
        public IEnumerable<EntryMap<CardType>> CardTypes { set; get; }
        public IEnumerable<EntryMap<MemberCard>> MemberCards { set; get; }
        public IEnumerable<EntryMap<AuthorizationTemplate>> AuthTemplates { set; get; }

        public IEnumerable<EntryMap<PaymentMeans>> PaymentMeans { set; get; }
        public IEnumerable<EntryMap<ItemComment>> ItemComments { set; get; }
        public IEnumerable<EntryMap<BuyXGetX>> BuyXGetXs { set; get; }
        public IEnumerable<EntryMap<BuyXGetXDetail>> BuyXGetXDetails { set; get; }
        public IEnumerable<EntryMap<PBuyXAmountGetXDis>> PBuyXAmountGetXDiss { set; get; }
        public IEnumerable<EntryMap<BuyXQtyGetXDis>> BuyXQtyGetXDiss { set; get; }

        public IEnumerable<EntryMap<SaleCombo>> SaleCombos { set; get; }
        public IEnumerable<EntryMap<SaleComboDetail>> SaleComboDetails { set; get; }

        public IEnumerable<EntryMap<DocumentType>> DocumentTypes { set; get; }
        public IEnumerable<EntryMap<PeriodIndicator>> PeriodIndicators { set; get; }

        public IEnumerable<EntryMap<Series>> Serieses { set; get; }
        public IEnumerable<EntryMap<SeriesDetail>> SeriesDetails { set; get; }
        public IEnumerable<EntryMap<CanRing>> CanRings { set; get; }
        public IEnumerable<EntryMap<CanRingMaster>> CanRingMasters { set; get; }
        public IEnumerable<EntryMap<CanRingDetail>> CanRingDetails { set; get; }
        public IEnumerable<EntryMap<PointCard>> PointCards { set; get; }
        public IEnumerable<EntryMap<PointRedemption>> PointRedemptions { set; get; }
        public IEnumerable<EntryMap<PointItem>> PointItems { set; get; }
        public IEnumerable<EntryMap<Redeem>> Redeems { set; get; }
        public IEnumerable<EntryMap<RedeemRetail>> RedeemRetails { set; get; }
        public IEnumerable<EntryMap<BusinessPartner>> BusinessPartners { set; get; }
        public IEnumerable<EntryMap<TaxGroup>> TaxGroups { set; get; }
        public IEnumerable<EntryMap<TaxGroupDefinition>> TaxGroupDefinitions { set; get; }

        public IEnumerable<EntryMap<ItemGroup1>> ItemGroup1s { set; get; }
        public IEnumerable<EntryMap<ItemGroup2>> ItemGroup2s { set; get; }
        public IEnumerable<EntryMap<ItemGroup3>> ItemGroup3s { set; get; }

        public IEnumerable<EntryMap<ItemMasterData>> ItemMasterDatas { set; get; }
        public IEnumerable<EntryMap<ReceiptInformation>> ReceiptInfos { set; get; }
        public IEnumerable<EntryMap<PromoCodeDiscount>> PromoCodeDiscounts { set; get; }
        public IEnumerable<EntryMap<PromoCodeDetail>> PromoCodeDetails { set; get; }
        public IEnumerable<EntryMap<Property>> Properties { set; get; }
        public IEnumerable<EntryMap<PropertyDetails>> PropertyDetails { set; get; }
        public IEnumerable<EntryMap<ChildPreoperty>> ChildProperties { set; get; }
        public IEnumerable<EntryMap<Freight>> Freights { set; get; }
    }
}
