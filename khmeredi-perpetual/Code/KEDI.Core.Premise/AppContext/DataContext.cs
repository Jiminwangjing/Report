using Microsoft.EntityFrameworkCore;
using CKBS.Models.ClassCopy;
using CKBS.Models.InventoryAuditReport;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.tmptable;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.ReportSale;
using CKBS.Models.Services.ReportPurchase;
using CKBS.Models.ServicesClass;
using CKBS.Models.Temporary_Table;
using Receipt = CKBS.Models.Services.POS.Receipt;
using CKBS.Models.Services.ReportDashboard;
using CKBS.Models.Services.ReportInventory;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.Services.ReportPurchase.dev;
using CKBS.Models.Services.ReportSaleAdmin;
using CKBS.Models.Services.Production;
using Microsoft.Extensions.Configuration;
using System.IO;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.Appointment;
using CKBS.Models.Services.Administrator.AlertManagement;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.CostOfAccounting;
using CKBS.Models.Services.Administrator.SettingDashboard;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.AlertManagement;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Models.Services.Administrator.SetUp;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Models.Services.AlertManagement;
using KEDI.Core.System.Models;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using PointCard = KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints.PointCard;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.KSMS;
using KEDI.Core.Premise.Models.Services.POS.KSMS;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using KEDI.Core.Premise.Models.Services.RemarkDiscount;
using KEDI.Core.Premise.Models.Services.CardMembers;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.Opportunity;
using KEDI.Core.Premise.Models.ProjectCostAnalysis;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.Activity;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.CanRingExchangeAdmin;
using KEDI.Core.Premise.Models.Services.Territory;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Models.SolutionDataManagement;
using KEDI.Core.Premise.Models.Services.ChartOfAccounts;
using KEDI.Core.Models.ControlCenter.ApiManagement;
using KEDI.Core.Premise.Models.Sync;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using KEDI.Core.Premise.Models.Services.Inventory.Transaction;
using System;
using KEDI.Core.Premise.Models.Integrations;
using KEDI.Core.Premise.Models.Integrations.Aeon;
using KEDI.Core.Premise.Models.Integrations.ChipMong;
using Models.Services.LoyaltyProgram.PromotionDiscount;
using KEDI.Core.Premise.Models.ClientApi;
using KEDI.Core.Premise.Models.Partners;
using KEDI.Core.Premise.Models.Services.CustomerConsignments;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CKBS.AppContext
{
    public partial class DataContext : DbContext
    {
        //Opporturnity
        public DbSet<ProjectCostAnalysis> ProjectCostAnalyses { get; set; }
        public DbSet<ProjCostAnalysisDetail> ProjCostAnalysisDetails { get; set; }
        // Mr Bunthorn
        public DbSet<SolutionDataManagement> SolutionDataManagements { set; get; }
        public DbSet<SolutionDataManagementDetail> SolutionDataManagementDetails { set; get; }
        public DbSet<ServiceData> ServiceDatas { set; get; }
        public DbSet<ServiceItem> ServiceItems { set; get; }
        public DbSet<Opex> Opexs { set; get; }
        public DbSet<FreightProjCostDetail> FreightProjCostDetails { get; set; }
        public DbSet<FreightProjectCost> FreightProjectCosts { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<InterestLevel> InterestLevels { get; set; }
        public DbSet<SaleEmployee> SaleEmployees { get; set; }
        public DbSet<StageDetail> StageDetails { get; set; }
        public DbSet<SetUpStage> SetUpStages { get; set; }
        public DbSet<PartnerDetail> PartnerDetails { get; set; }
        public DbSet<SetupPartner> SetupPartneres { get; set; }
        public DbSet<SetupRelationship> SetupRelationships { get; set; }
        public DbSet<CompetitorDetail> CompetitorDetail { get; set; }
        public DbSet<SetupCompetitor> SetupCompetitors { get; set; }
        public DbSet<SetupReasons> SetupReasons { get; set; }
        public DbSet<DescriptionPotentialDetail> DescriptionPotentials { get; set; }
        public DbSet<DescriptionSummaryDetail> DescriptionSummaryDetails { get; set; }
        public DbSet<PotentialDetail> PotentialDetails { get; set; }
        public DbSet<SetupInterestange> SetupInterestRange { get; set; }
        public DbSet<SummaryDetail> SummaryDetail { get; set; }
        public DbSet<OpportunityMasterData> OpportunityMasterDatas { get; set; }

        // end  Opporturnity

        public DbSet<SystemLicense> SystemLicenses { get; set; }
        //============================combo sale============================
        public DbSet<SaleCombo> SaleCombos { get; set; }
        public DbSet<SaleComboDetail> SaleComboDetails { get; set; }
        //================================end comsale==========================
        public DbSet<PBuyXAmountGetXDis> PBuyXAmountGetXDis { get; set; }
        public DbSet<BuyXQtyGetXDis> BuyXQtyGetXDis { get; set; }
        public DbSet<ItemGroup1> ItemGroup1 { get; set; }
        public DbSet<ItemGroup2> ItemGroup2 { get; set; }
        public DbSet<ItemGroup3> ItemGroup3 { get; set; }
        public DbSet<Colors> Colors { get; set; }
        public DbSet<Background> Backgrounds { get; set; }
        public DbSet<UnitofMeasure> UnitofMeasures { get; set; }
        public DbSet<GroupUOM> GroupUOMs { get; set; }
        public DbSet<GroupDUoM> GroupDUoMs { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserPrivillege> UserPrivilleges { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<BusinessPartner> BusinessPartners { get; set; }
        public DbSet<SetupCustomerSource> SetupCustomerSources { get; set; }
        public DbSet<ContractBiling> ContractBilings { get; set; }
        public DbSet<SetupContractName> SetupContractNames { get; set; }
        public DbSet<BPBranch> BPBranches { get; set; }
        public DbSet<GroupCustomer1> GroupCustomer1s { get; set; }
        public DbSet<GroupCustomer2> GroupCustomer2s { get; set; }
        public DbSet<ContactPerson> ContactPersons { get; set; }
        public DbSet<PaymentTerms> PaymentTerms { get; set; }
        public DbSet<Instaillment> Instaillments { get; set; }
        public DbSet<InstaillmentDetail> InstaillmentDetails { get; set; }
        public DbSet<CashDiscount> CashDiscounts { get; set; }
        public DbSet<TypeCard> TypeCards { get; set; }
        public DbSet<CardMember> CardMembers { get; set; }
        public DbSet<RenewCardHistory> RenewCardHistories { get; set; }
        public DbSet<AccountMemberCard> AccountMemberCards { get; set; }
        public DbSet<CardMemberDeposit> CardMemberDeposits { get; set; }
        public DbSet<CardMemberDepositTransaction> CardMemberDepositTransactions { get; set; }

        //========Activity=============
        public DbSet<Activity> Activites { get; set; }
        public DbSet<SetupType> SetupTypes { get; set; }
        public DbSet<SetupSubject> SetupSubjects { get; set; }
        public DbSet<SetupActivity> SetupActivities { get; set; }
        public DbSet<SetupStatus> SetupStatuses { get; set; }
        public DbSet<SetupLocation> SetupLocations { get; set; }
        public DbSet<General> Generals { get; set; }
        //==========Activity==========
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<PriceLists> PriceLists { get; set; }
        public DbSet<PriceListDetail> PriceListDetails { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ReceiptInformation> ReceiptInformation { get; set; }
        public DbSet<Tax> Tax { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseDetail> WarehouseDetails { get; set; }
        public DbSet<StockOut> StockOuts { get; set; }
        public DbSet<GroupTable> GroupTables { get; set; }
        public DbSet<ItemMasterData> ItemMasterDatas { get; set; }
        public DbSet<ContractTemplate> Contracts { get; set; }
        public DbSet<ServiceCall> ServiceCalls { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<PrinterName> PrinterNames { get; set; }
        public DbSet<PaymentMeans> PaymentMeans { get; set; }
        public DbSet<MemberCard> MemberCards { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PointDetail> PointDetails { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<Purchase_AP> Purchase_APs { get; set; }
        public DbSet<DraftAP> DraftAPs { get; set; }
        public DbSet<DraftAPDetail> DraftAPDetails { get; set; }
        public DbSet<DraftReserve> DraftReserves { get; set; }

        public DbSet<PurchaseAPReserve> PurchaseAPReserves { get; set; }
        public DbSet<PurchaseAPReserveDetail> PurchaseAPReserveDetails { get; set; }
        public DbSet<Purchase_APDetail> PurchaseDetails { get; set; }
        public DbSet<PurchaseCreditMemo> PurchaseCreditMemos { get; set; }
        public DbSet<PurchaseCreditMemoDetail> PurchaseCreditMemoDetails { get; set; }
        public DbSet<PurchaseQuotation> PurchaseQuotations { get; set; }
        public DbSet<PurchaseQuotationDetail> PurchaseQuotationDetails { get; set; }
        public DbSet<FreightPurchase> FreightPurchases { get; set; }
        public DbSet<FreightPurchaseDetial> FreightPurchaseDetails { get; set; }
        public DbSet<PromotionPrice> PromotionPrice { get; set; }
        public DbSet<GoodReciptPODetail> GoodReciptPODetails { get; set; }
        public DbSet<GoodsReciptPO> GoodsReciptPOs { get; set; }
        public DbSet<GoodIssues> GoodIssues { get; set; }
        public DbSet<GoodIssuesDetail> GoodIssuesDetails { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodReceiptDetail> GoodReceiptDetails { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferDetail> TransferDetails { get; set; }
        public DbSet<OutgoingPayment> OutgoingPayments { get; set; }
        public DbSet<OutgoingPaymentDetail> OutgoingPaymentDetails { get; set; }
        public DbSet<OutgoingPaymentVendor> OutgoingPaymentVendors { get; set; }
        public DbSet<ItemCopyToPriceListDetail> ItemCopyToPriceListDetail { get; set; }
        public DbSet<ItemCopyToPriceList> ItemCopyToPriceList { get; set; }
        public DbSet<ServicePriceListCopyItem> ServicePriceListCopyItem { get; set; }
        public DbSet<ItemCopyToWHDetail> ItemCopyToWHDetail { get; set; }
        public DbSet<ItemCopyToWH> ItemCopyToWH { get; set; }
        public DbSet<DiscountItemDetail> DiscountItemDetail { get; set; }
        public DbSet<PricelistSetPrice> PricelistSetPrice { get; set; }
        public DbSet<InventoryAudit> InventoryAudits { get; set; }
        public DbSet<GoodsReceiptPoReturn> GoodsReceiptPoReturns { get; set; }
        public DbSet<GoodsReceiptPoReturnDetail> GoodsReceiptPoReturnDetails { get; set; }
        public DbSet<WarehouseSummary> WarehouseSummary { get; set; }
        //Service map data

        public DbSet<ItemMasterDataService> ItemMasterDataServices { get; set; }
        public DbSet<ServicePointDetail> ServicePointDetails { get; set; }
        public DbSet<ServiceMapItemMasterDataQuotation> ServiceMapItemMasterDatas { get; set; }
        public DbSet<ServiceQuotationDetail> ServiceQuotationDetails { get; set; }
        public DbSet<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs { get; set; }
        public DbSet<ServiceMapItemMasterDataPurchaseCreditMemo> ServiceMapItemMasterDataPurchaseCreditMemos { get; set; }
        public DbSet<ServiceMapItemMasterDataPurchaseOrder> ServiceMapItemMasterDataPurchaseOrders { get; set; }
        //public DbSet<PricelistSetUpdatePrice> PricelistSetUpdatePrice { get; set; }
        //02.02.2019
        public DbSet<ServiceMapItemPurchaseRequest> ServiceMapItemPurchaseRequests { get; set; }
        public DbSet<ReportPurchaseRequset> ReportPurchaseRequsets { get; set; }
        public DbSet<PurchaesOrder_from_Quotation> PurchaesOrder_From_Quotations { get; set; }

        //Report serive
        public DbSet<ReportPurchaseQuotation> ReportPurchaseQuotations { get; set; }
        public DbSet<ReportPurchaseAP> ReportPurchaseAPs { get; set; }
        public DbSet<ReportPurchasCreditMemo> ReportPurchasCreditMemos { get; set; }
        public DbSet<ReportPurchaseOrder> ReportPurchaseOrders { get; set; }

        // stock
        public DbSet<GoodReceiptStock> GoodReceiptStocks { get; set; }
        // invetory Audit
        public DbSet<ServiceInventoryAudit> ServiceInventoryAudits { get; set; }
        //temporary table
        public DbSet<TpPriceList> TpPriceLists { get; set; }
        //POS
        public DbSet<RemarkDiscountItem> RemarkDiscounts { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<MultipayMeansSetting> MultipayMeansSetting { get; set; }
        public DbSet<MultiPaymentMeans> MultiPaymentMeans { get; set; }
        public DbSet<SettingPayment> SettingPayments { get; set; }

        public DbSet<OrderDetail_Addon> OrderDetail_Addon { get; set; }
        public DbSet<Order_Receipt> Order_Receipt { get; set; }
        public DbSet<Order_Queue> Order_Queue { get; set; }
        public DbSet<Receipt> Receipt { get; set; }
        public DbSet<ReceiptDetail> ReceiptDetail { get; set; }
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<OpenShift> OpenShift { get; set; }
        public DbSet<CloseShift> CloseShift { get; set; }
        public DbSet<DisplayCurrency> DisplayCurrencies { get; set; }
        public DbSet<TmpOrderDetail> TmpOrderDetail { get; set; }
        public DbSet<SystemType> SystemType { get; set; }
        public DbSet<Counter> Counters { get; set; }

        //Service map data
        public DbSet<ServiceItemSales> ServiceItemSales { get; set; }
        public DbSet<CheckPayment> CheckPayments { get; set; }

        public DbSet<StockMoving> StockMoving { get; set; }
        // service copy
        public DbSet<PurchaseAP_To_PurchaseMemo> PurchaseAP_To_PurchaseMemos { get; set; }
        //Report
        public DbSet<SummarySale> SummarySales { get; set; }
        public DbSet<DetailSale> DetailSales { get; set; }
        public DbSet<SummaryPurchaseAP> SummaryPurchaseAPs { get; set; }
        public DbSet<DetailPurchaseAp> DetailPurchaseAps { get; set; }
        public DbSet<ReportCloseShft> ReportCloseShfts { get; set; }
        public DbSet<TopSaleQuantity> TopSaleQuantities { get; set; }
        public DbSet<DetailTopSaleQty> DetailTopSaleQties { get; set; }
        public DbSet<DashboardSaleSummary> DashboardSaleSummary { get; set; }
        public DbSet<StockExpired> StockExpireds { get; set; }
        public DbSet<StockInWarehouse> StockInWarehouses { get; set; }
        public DbSet<StockInWarehouse_Detail> StockInWarehouse_Details { get; set; }
        public DbSet<SummaryOutgoingPayment> SummaryOutgoingPayments { get; set; }
        public DbSet<SummaryDetailOutgoingPayment> SummaryDetailOutgoingPayments { get; set; }
        public DbSet<SummaryTransferStock> SummaryTransferStocks { get; set; }
        public DbSet<SummaryDetaitTransferStock> SummaryDetaitTransferStocks { get; set; }
        public DbSet<SummaryRevenuesItem> SummaryRevenuesItems { get; set; }
        public DbSet<DashboardTopSale> DashboardTopsale { get; set; }
        public DbSet<RevenueItem> RevenueItems { get; set; }
        public DbSet<CashoutReport> CashoutReport { get; set; }
        // Sale Quotation
        public DbSet<SaleQuote> SaleQuotes { get; set; }
        public DbSet<SaleQuoteDetail> SaleQuoteDetails { get; set; }
        //Report Sale Admin
        public DbSet<SummarySaleAdmin> SummarySaleAdmin { get; set; }
        //Sale Order
        public DbSet<SaleOrder> SaleOrders { get; set; }
        public DbSet<SaleOrderDetail> SaleOrderDetails { get; set; }

        //Sale Delivery
        public DbSet<SaleDelivery> SaleDeliveries { get; set; }
        public DbSet<SaleDeliveryDetail> SaleDeliveryDetails { get; set; }
        //==========sale draft delivery========
        public DbSet<DraftDelivery> DraftDeliveries { get; set; }
        public DbSet<DraftDeliveryDetail> DraftDeliveryDetails { get; set; }
        // Return Delivery
        public DbSet<ReturnDeliveryDetail> ReturnDeliveryDetails { get; set; }
        public DbSet<ReturnDelivery> ReturnDeliverys { get; set; }
        public DbSet<FreightSale> FreightSales { get; set; }
        public DbSet<FreightSaleDetail> FreightSaleDetails { get; set; }
        // Sale ARDown Payment
        public DbSet<ARDownPayment> ARDownPayments { get; set; }
        public DbSet<ARDownPaymentDetail> ARDownPaymentDetails { get; set; }
        //Sale AR
        public DbSet<SaleAR> SaleARs { get; set; }
        public DbSet<SaleARDetail> SaleARDetails { get; set; }
        public DbSet<SaleAREdite> SaleAREdites { get; set; }
        public DbSet<SaleAREditeDetail> SaleAREditeDetails { get; set; }
        public DbSet<DraftAR> DraftARs { get; set; }
        public DbSet<DraftARDetail> DraftARDetails { get; set; }
        //===============Service Contract=========
        public DbSet<ServiceContract> ServiceContracts { get; set; }
        public DbSet<ServiceContractDetail> ServiceContractDetails { get; set; }
        public DbSet<AttchmentFile> AttchmentFiles { get; set; }

        public DbSet<DraftServiceContract> DraftServiceContracts { get; set; }
        public DbSet<DraftServiceContractDetail> DraftServiceContractDetails { get; set; }
        public DbSet<DraftAttchmentFile> DraftAttchmentFiles { get; set; }

        //=============Contract Template============
        //===============ARReserveInvoice============
        public DbSet<ARReserveInvoice> ARReserveInvoices { get; set; }
        public DbSet<ARReserveInvoiceDetail> ARReserveInvoiceDetails { get; set; }
        public DbSet<ARReserveInvoiceEditable> ARReserveInvoiceEditables { get; set; }
        public DbSet<ARReserveInvoiceEditableDetail> ARReserveInvoiceEditableDetails { get; set; }
        public DbSet<DraftReserveInvoiceEditable> DraftReserveInvoiceEditables { get; set; }
        public DbSet<DraftReserveInvoiceEditableDetail> DraftReserveInvoiceEditableDetails { get; set; }
        public DbSet<DraftReserveInvoice> DraftReserveInvoices { get; set; }
        public DbSet<DraftReserveInvoiceDetail> DraftReserveInvoiceDetails { get; set; }
        public DbSet<AttachmentFileOfContractTemplate> AttachmentFileOfContractTemplates { get; set; }
        public DbSet<Remark> Remarks { get; set; }
        public DbSet<Converage> Converages { get; set; }
        public DbSet<SetupContractType> SetupContractTypes { get; set; }

        //===========Territory=============
        public DbSet<Territory> Territories { get; set; }

        //Sale Credit Memo
        public DbSet<SaleCreditMemo> SaleCreditMemos { get; set; }
        public DbSet<SaleCreditMemoDetail> SaleCreditMemoDetails { get; set; }

        //Service Banking Incoming Payment
        public DbSet<IncomingPaymentCustomer> IncomingPaymentCustomers { get; set; }
        public DbSet<IncomingPayment> IncomingPayments { get; set; }
        public DbSet<IncomingPaymentDetail> IncomingPaymentDetails { get; set; }
        public DbSet<MultiIncomming> MultiIncommings { get; set; }

        //Dev
        public DbSet<SummaryTotalSale> SummaryTotalSale { get; set; }
        //Pisey
        public DbSet<SummaryPurchaseTotal> SummaryPurchaseTotal { get; set; }
        public DbSet<Purchase_APDetail> PurchaseAPDetail { get; set; }
        //Production
        public DbSet<BOMaterial> BOMaterial { get; set; }
        public DbSet<BOMDetail> BOMDetail { get; set; }
        //Void Order
        public DbSet<VoidOrder> VoidOrders { get; set; }
        public DbSet<VoidOrderDetail> VoidOrderDetails { get; set; }
        //PaymentMeans
        public DbSet<CashoutPaymentMeans> CashoutPaymentMeans { get; set; }
        public DbSet<ItemComment> ItemComment { get; set; }

        //Financials 
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<ItemAccounting> ItemAccountings { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryDetail> JournalEntryDetails { get; set; }
        public DbSet<AccountBalance> AccountBalances { get; set; }
        //System Initialization
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<PeriodIndicator> PeriodIndicators { get; set; }
        public DbSet<PostingPeriod> PostingPeriods { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<SeriesDetail> SeriesDetails { get; set; }

        /// <summary>
        /// KSMS => Kernel Service Managemnet System
        /// </summary>
        public DbSet<ServiceSetupDetial> ServiceSetupDetials { get; set; }
        public DbSet<ServiceSetup> ServiceSetups { get; set; }
        public DbSet<KSService> KSServices { get; set; }
        public DbSet<KSServiceMaster> KSServiceMaster { get; set; }
        public DbSet<KSServiceHistory> KSServiceHistories { get; set; }

        //KVMS
        public DbSet<AutoMobile> AutoMobiles { get; set; }
        public DbSet<AutoType> AutoTypes { get; set; }
        public DbSet<AutoBrand> AutoBrands { get; set; }
        public DbSet<AutoModel> AutoModels { get; set; }
        public DbSet<AutoColor> AutoColors { get; set; }

        //Quote KVMS
        public DbSet<QuoteAutoM> QuoteAutoMs { get; set; }
        public DbSet<OrderQAutoM> OrderQAutoMs { get; set; }
        public DbSet<OrderDetailQAutoMs> OrderDetailQAutoMs { get; set; }

        //Receipt KVMS
        public DbSet<KvmsInfo> KvmsInfo { get; set; }
        public DbSet<ReceiptKvms> ReceiptKvms { get; set; }
        public DbSet<ReceiptDetailKvms> ReceiptDetailKvms { get; set; }

        //Aging Payment
        public DbSet<AgingPaymentCustomer> AgingPaymentCustomer { get; set; }
        public DbSet<AgingPayment> AgingPayment { get; set; }
        public DbSet<AgingPaymentDetail> AgingPaymentDetails { get; set; }

        //Receipt Memo KVMS
        public DbSet<ReceiptMemo> ReceiptMemo { get; set; }
        public DbSet<ReceiptDetailMemo> ReceiptDetailMemoKvms { get; set; }
        //Appointment
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<AppointmentService> AppointmentService { get; set; }
        //Property
        public DbSet<Property> Property { get; set; }
        public DbSet<PropertyDetails> PropertyDetails { get; set; }
        public DbSet<ChildPreoperty> ChildPreoperties { get; set; }
        //Cost of Accounting
        public DbSet<CostOfAccountingType> CostOfAccountingTypes { get; set; }
        public DbSet<CostOfCenter> CostOfCenter { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<DashboardSetting> DashboardSettings { get; set; }
        public DbSet<Freight> Freights { get; set; }
        public DbSet<FreightReceipt> FreightReceipts { get; set; }
        public DbSet<SaleGLDeterminationMaster> SaleGLDeterminationMasters { get; set; }
        public DbSet<ControlAccountsReceivable> ControlAccountsReceivables { get; set; }
        public DbSet<SaleGLAccountDeterminationResources> SaleGLAccountDeterminationResources { get; set; }
        public DbSet<SaleGLAccountDetermination> SaleGLAccountDeterminations { get; set; }
        public DbSet<TaxGroup> TaxGroups { get; set; }
        public DbSet<TaxGroupDefinition> TaxGroupDefinitions { get; set; }
        /// Alert Management
        /// </summary>
        public DbSet<StockAlert> StockAlerts { get; set; }
        public DbSet<ExpirationStockItem> ExpirationStockItems { get; set; }
        public DbSet<AlertWarehouses> AlertWarehouse { get; set; }
        public DbSet<DueDateAlert> DueDateAlerts { get; set; }
        public DbSet<ServiceContractAlert> ServiceContractAlerts { get; set; }
        public DbSet<AlertManagement> AlertManagement { get; set; }
        public DbSet<SetttingAlert> SetttingAlert { get; set; }
        public DbSet<SetttingAlertUser> SetttingAlertUser { get; set; }
        public DbSet<TypeOfAlertM> TypeOfAlertM { get; set; }

        public DbSet<AlertMaster> AlertMasters { get; set; }
        public DbSet<AlertDetail> AlertDetails { get; set; }
        public DbSet<UserAlert> UserAlerts { get; set; }
        public DbSet<TelegramToken> TelegramTokens { get; set; }
        public DbSet<CashOutAlert> CashOutAlerts { get; set; }
        public DbSet<Display> Displays { get; set; }
        public DbSet<CardMemberTemplate> CardMemberTemplates { get; set; }
        public DbSet<GeneralServiceSetup> GeneralServiceSetups { get; set; }
        // Void Item
        public DbSet<VoidItem> VoidItems { get; set; }
        public DbSet<VoidReason> VoidReasons { get; set; }
        public DbSet<VoidItemDetail> VoidItemDetails { get; set; }
        public DbSet<PendingVoidItem> PendingVoidItems { get; set; }
        public DbSet<PendingVoidItemDetail> PendingVoidItemDetails { get; set; }
        //Loyalty Program
        public DbSet<BuyXGetX> BuyXGetXes { get; set; }
        public DbSet<BuyXGetXDetail> BuyXGetXDetails { get; set; }

        //Member points
        public DbSet<PointCard> PointCards { get; set; }
        public DbSet<PointRedemption> PointRedemptions { get; set; }
        public DbSet<PointItem> PointItems { get; set; }
        public DbSet<Redeem> Redeems { get; set; }
        public DbSet<RedeemRetail> RedeemDetails { get; set; }
        public DbSet<PointRedemptionMaster> PointRedemptionMasters { get; set; }
        public DbSet<PointRedemptionHistory> PointRedemptionHistories { get; set; }
        public DbSet<PointItemHistory> PointItemHistories { get; set; }

        public DbSet<PromoCodeDiscount> PromoCodeDiscounts { get; set; }
        public DbSet<PromoCodeDetail> PromoCodeDetails { get; set; }

        //Authorization Template (POS)
        public DbSet<AuthorizationTemplate> AuthorizationTemplates { set; get; }
        //Can Ring
        public DbSet<CanRing> CanRings { get; set; }
        public DbSet<CanRingMaster> CanRingMasters { get; set; }
        public DbSet<CanRingDetail> CanRingDetails { get; set; }
        public DbSet<ExchangeCanRingMaster> ExchangeCanRingMasters { get; set; }
        public DbSet<ExchangeCanRingDetail> ExchangeCanRingDetails { get; set; }
        public DbSet<Menu> Menu { get; set; }

        //temporary table storing data *** Serials && Batches *** //
        // *** Serials ***
        public DbSet<SerialNumber> SerialNumbers { get; set; }
        public DbSet<SerialNumberSelected> SerialNumberSelecteds { get; set; }
        public DbSet<SerialNumberSelectedDetail> SerialNumberSelectedDetails { get; set; }
        // *** Batches ***
        public DbSet<BatchNo> BatchNos { get; set; }
        public DbSet<BatchNoSelected> BatchNoSelecteds { get; set; }
        public DbSet<BatchNoSelectedDetail> BatchNoSelectedDetails { get; set; }

        public DbSet<SaleAREditeHistory> SaleAREditeHistory { get; set; }
        public DbSet<SaleAREditeDetailHistory> SaleAREditeDetailHistory { get; set; }
        public DbSet<SubTypeAcount> SubTypeAcounts { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
        public DbSet<TransferRequestDetail> TransferRequestDetails { get; set; }
        public DbSet<ARReserveEditableHistory> ARReserveEditableHistories { get; set; }
        public DbSet<ARReserveEditableDetailHistory> ARReserveEditableDetailHistories { get; set; }
        public DbSet<ColorSetting> ColorSettings { get; set; }
        public DbSet<FontSetting> FontSettings { get; set; }
        public DbSet<SkinItem> SkinItem { get; set; }
        public DbSet<SkinUser> SkinUser { get; set; }
        public DbSet<TransactionAeon> TransactionAeons { get; set; }
        public DbSet<TransactionChipMong> TransactionChipMongs { get; set; }
        public DbSet<CustomerTips> CustomerTips { get; set; }
        public DbSet<DataSyncSetting> DataSyncSettings { set; get; }
        public DbSet<PromotionDetail> PromotionDetails { get; set; }
         public DbSet<LoanPartner> LoanPartners { get; set; }
        public DbSet<IncomingPaymentOrder> IncomingPaymentOrders { get; set; }
        public DbSet<IncomingPaymentOrderDetail> IncomingPaymentOrderDetails { get; set; }
        public DbSet<MultiIncomingPaymentOrder> MultiIncomingPaymentOrders { get; set; }
        public DbSet<GroupLoanPartner> GroupLoanPartners { get; set; }
        public DbSet<LoanContactPerson> LoanContactPersons { get; set; }
        public DbSet<MultiBrand> MultiBrands { get; set; }
        public DbSet<CustomerConsignment> CustomerConsignments { get; set; }
        public DbSet<CustomerConsignmentDetail> CustomerConsignmentDetails { get; set; }
         public DbSet<OutgoingPaymentOrder> OutgoingPaymentOrders { set; get; }
        public DbSet<OutgoingPaymentOrderDetail> OutgoingPaymentOrderDetails { set; get; }
        public DbSet<InventoryCounting> InventoryCountings { set; get; }
        public DbSet<InventoryCountingDetail> InventoryCountingDetails { set; get; }

        public DbSet<ClientSyncHistory> ClientSyncHistories { set; get; }
        public DbSet<ServerSyncHistory> ServerSyncHistories { set; get; }
        public DbSet<ClientApp> ClientApps { get; set; }
    }

    public partial class DataContext : DbContext
    {
        public DataContext() : base() { }
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        private void DefineSyncEntityIdentity()
        {
            var entries = ChangeTracker.Entries<ISyncEntity>()
                        .Where(ent => ent.State != EntityState.Unchanged);
            string nameOfRowId = nameof(ISyncEntity.RowId);
            foreach (var entry in entries)
            {
                PropertyValues dbValues = entry.GetDatabaseValues();
                Guid dbRowId = dbValues == null ? Guid.Empty : dbValues.GetValue<Guid>(nameOfRowId);
                switch (entry.State)
                {
                    case EntityState.Added:
                        Guid currentRowId = entry.CurrentValues.GetValue<Guid>(nameOfRowId);
                        if (currentRowId == Guid.Empty || currentRowId == dbRowId)
                        {
                            SequentialGuidValueGenerator sgvg = new();
                            entry.Entity.RowId = sgvg.Next(entry);
                        }
                        break;
                    case EntityState.Modified:
                        entry.Entity.RowId = dbRowId;
                        break;
                }
                entry.Entity.ChangeLog = DateTimeOffset.UtcNow;
            }
        }

        public override int SaveChanges()
        {
            DefineSyncEntityIdentity();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DefineSyncEntityIdentity();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            DefineSyncEntityIdentity();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DefineSyncEntityIdentity();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UnitofMeasure>().HasIndex(x => x.Code).IsUnique();
            builder.Entity<Employee>().HasIndex(x => x.Code).IsUnique();
            builder.Entity<BusinessPartner>().HasIndex(x => x.Code).IsUnique();
            builder.Entity<GroupUOM>().HasIndex(x => x.Code).IsUnique();
            builder.Entity<Warehouse>().HasIndex(x => x.Code).IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration["UsersConnection:ConnectionString"];
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
