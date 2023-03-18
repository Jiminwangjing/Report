using CKBS.AlertManagementsServices.Repositories;
using CKBS.Controllers.API.QRCodeV1.Security;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Hosting;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Utilities;
using KEDI.Core.Premise.Repositories;
using KEDI.Core.Premise.Repositories.Integrations;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Repository.Report;
using KEDI.Core.Premise.Services;
using KEDI.Core.Premise.Services.AlertService.Repositories;
using KEDI.Core.Repository;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using KEDI.Core.Premise.Models.Services.Repositories;
using KEDI.Core.Premise.AppContext;

namespace KEDI.Core.Premise.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKediCore(this IServiceCollection services, IConfiguration config)
        {
            // Custom extensions
            services.AddDataContext(config);
            services.AddHttpClientFactory(config);

            services.AddTransient<HostMessenger>();
            services.AddTransient<SystemManager>();
            services.AddTransient<UserManager>();
            services.AddTransient<IItemGroup, ItemGorup1Responsitory>();
            services.AddTransient<IItem2Group, ItemGroup2Responsitory>();
            services.AddTransient<IItemGroup3, ItemGroup3Responsitory>();
            services.AddTransient<IUOM, UnitofMeasureResponsitory>();
            services.AddTransient<IGUOM, GroupUoMResponsitory>();
            services.AddTransient<IBranch, BranchRepository>();
            services.AddTransient<IBusinessPartner, BusinessPartnerRepository>();
            services.AddTransient<ICompany, CompanyRepository>();
            services.AddTransient<ICurrency, CurrencyRepository>();
            services.AddTransient<IEmployee, EmployeeRepository>();
            services.AddTransient<IExchangeRate, ExcahageRateRepository>();
            services.AddTransient<IGroupTable, GroupTableRepository>();
            services.AddTransient<IItemMasterData, ItemMasterDataRepository>();
            services.AddTransient<IPaymentMean, PaymentMeanRepository>();
            services.AddTransient<IPriceList, PriceListRepository>();
            services.AddTransient<IPriceListDetail, PriceListDetailRepository>();
            services.AddTransient<IPrinterName, PrinterNameRepository>();
            services.AddTransient<IPromotion, PromotionRepository>();
            services.AddTransient<IReceiptInformation, ReceiptInformationRepository>();
            services.AddTransient<ITable, TableRepository>();
            services.AddTransient<ITax, TaxRepository>();
            services.AddTransient<IUserAccount, UserAccountRepositoy>();
            services.AddTransient<IUserPrivillege, UserPrivillegeRepository>();
            services.AddTransient<IWarehouse, WarehouseRepository>();
            services.AddTransient<IWarehouseDetail, WarehouseDetailRepository>();
            services.AddTransient<ICardType, CardTypeResponsitory>();
            services.AddTransient<IMemberCard, MemberResponsitory>();
            services.AddTransient<IPromitionPrice, PromotionPriceRepository>();
            services.AddTransient<IPurchaseQuotation, PurchaseQuotationResponsitory>();
            services.AddTransient<IPurchaseAP, PurchaseAPResponsitory>();
            services.AddTransient<IPurchaseCreditMemo, PurchaseCreditMemoResponsitory>();
            services.AddTransient<IPurchaseOrder, PurchaseOrderResponsitory>();
            services.AddTransient<IPOS, POSRepository>();
            services.AddTransient<IGoodIssuse, GoodIssuseResponsitory>();
            services.AddTransient<IGoodsReceipt, GoodsReceiptResponsitory>();
            services.AddTransient<IGoodsReceipts, GoodsReceiptsResponsitory>();
            services.AddTransient<ITransfer, TransferResponsitory>();
            services.AddTransient<IOutgoingPayment, OutgoingPaymentResponsitory>();
            services.AddTransient<IIncomingPayment, IncomingPaymentResponsitory>();
            services.AddTransient<IReport, ReportResponsitory>();
            services.AddTransient<IGoodRecepitPo, GoodReceiptPoResponsitory>();
            services.AddTransient<IGoodsReceiptPoReturn, GoodsReceiptPoReturnResponsitory>();
            services.AddTransient<IPurchaseRequest, PurchaseRquestResponsitory>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<ISale, SaleRepository>();
            services.AddTransient<IReturnOrCancelStockMaterial, ReturnOrCancelMaterialRepository>();
            services.AddTransient<IFinancialReports, FinancialReportsRepository>();
            services.AddTransient<ICostOfAccounting, CostOfAccountingRepository>();
            services.AddTransient<IPropertyRepository, PropertyRepository>();
            services.AddSingleton<DataProtectionPurposeStrings>();
            services.AddTransient<IPOSOrderQRCodeRepository, POSOrderQRCodeRepository>();
            services.AddTransient<ICheckFrequently, CheckFrequently>();
            services.AddTransient<IAlertStockRepo, AlertStockRepo>();
            services.AddTransient<IAlertDueDateRepo, AlertDueDateRepo>();
            services.AddTransient<IExpirationStockItemRepo, ExpirationStockItemRepo>();
            services.AddTransient<IControlAlertRepository, ControlAlertRepository>();
            services.AddTransient<IControlAlertAPIRepository, ControlAlertAPIRepository>();

            services.AddTransient<IGeneralSettingAdminRepository, GeneralSettingAdminRepository>();
            services.AddTransient<ITaxGroup, TaxGroupRepository>();
            services.AddTransient<IPurchaseRepository, PurchaseRepository>();
            services.AddTransient<ISaleSerialBatchRepository, SaleSerialBatchRepository>();
            services.AddTransient<IItemSerialNumberRepository, ItemSerialNumberRepository>();
            services.AddTransient<IItemBatchNoRepository, ItemBatchNoRepository>();
            services.AddTransient<IPOSSerialBatchRepository, POSSerialBatchRepository>();
            services.AddTransient<IEquipmentRepository, EquipmentRepository>();
            services.AddTransient<IKSServiceSetUpRepository, KSServiceSetUpRepository>();
            services.AddTransient<IKSMSRopository, KSMSRopository>();
            services.AddTransient<IPrintBarcodeRepository, PrintBarcodeRepository>();
            services.AddTransient<IPrintTemplateRepository, PrintTemplateRepository>();
            services.AddTransient<IRemarkDiscountRepository, RemarkDiscountRepository>();
            services.AddTransient<ICardMemberRepository, CardMemberRepository>();
            services.AddTransient<IProjectCostAnalysisRepository, ProjectCostAnalysisReponsitory>();
            services.AddTransient<IDataPropertyRepository, DataPropertyRepository>();
            services.AddTransient<ICanRingRepository, CanRingRepository>();
            services.AddTransient<ISerialBatchReportRepository, SerialBatchReportRepository>();
            services.AddTransient<ReportSaleRepo>();
            //Manage all pos retail process
            services.AddTransient<JwtManager>();
            services.AddTransient<PosRetailModule>();
            services.AddTransient<UtilityModule>();
            //Loyalty Program
            services.AddTransient<LoyaltyProgramPosModule>();
            services.AddTransient<BuyXGetXPosModule>();
            services.AddTransient<MemberPointModule>();

            services.AddTransient<IPrintTemplateRepo, PrintTemplateRepo>();
            services.AddTransient<ICopyFromDiliveryToAR, SaleARRepo>();
            services.AddTransient<IClientApiRepo, ClientApiRepo>();
            services.AddTransient<IWorkbookAdapter, WorkbookAdapter>();
            services.AddTransient<IDataSyncSettingRepo, DataSyncSettingRepo>();

            services.AddScoped<IQueryContext, QueryContext>();
            services.AddTransient<IPosSyncRepo, PosSyncRepo>();
            services.AddTransient<IServerEntityFilter, ServerEntityFilter>();
            services.AddTransient<IClientEntityFilter, ClientEntityFilter>();
            services.AddTransient<ISecuritySigner, SecuritySigner>();
            services.AddTransient<ISyncSender, SyncSender>();        
            services.AddTransient<ISyncAdapter, SyncAdapter>();
            services.AddTransient<ISyncClientWorker, SyncClientWorker>();
            services.AddTransient<ISyncServerWorker, SyncServerWorker>();        
            services.AddTransient<IPosClientSignal, PosClientSignal>();
            services.AddTransient<IAeonRepo, AeonRepo>();
            services.AddTransient<IChipMongRepo, ChipMongRepo>();
            services.AddTransient<ITenantSaleRepo, TenantSaleRepo>();
            services.AddTransient<IinventoryCounting, InventoryCountingRepository>();

            services.AddTransient<IPosExcelRepo, PosExcelRepo>();
             services.AddTransient<ILoanPartnerRepo, LoanPartnerRepo>();

            return services;
        }
    }
}
