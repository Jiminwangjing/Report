using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.Promotions;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.RemarkDiscount;
using KEDI.Core.Premise.Models.Sync;
using KEDI.Core.Premise.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Function = CKBS.Models.Services.Account.Function;
using PointCard = KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints.PointCard;

namespace KEDI.Core.Premise.AppContext.Sync.Customs
{
    //Client part
    public partial class CustomEntityContext : EntityContext, ICustomEntityContext
    {       
        public CustomEntityContext(ILogger<CustomEntityContext> logger, IQueryContext query): base(logger, query) {}

        public async Task<EntryMap<Receipt>> MapReferencesAsync(Receipt entity)
        {
            var entityBinder = await MapForeignKeysAsync(entity,
                MapWith<Series>(nameof(entity.SeriesID), entity.SeriesID, true),
                MapWith<SeriesDetail>(nameof(entity.SeriesDID), entity.SeriesDID, true),
                MapWith<PromoCodeDiscount>(nameof(entity.PromoCodeID), entity.PromoCodeID, true),
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),
                MapWith<UserAccount>(nameof(entity.UserDiscountID), entity.UserDiscountID, true),
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
                MapWith<BusinessPartner>(nameof(entity.CustomerID), entity.CustomerID),
                MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),  
                MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID),
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),
                MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
                MapWith<Currency>(nameof(entity.PLCurrencyID), entity.PLCurrencyID),
                MapWith<UserAccount>(nameof(entity.UserOrderID), entity.UserOrderID)             
            );
            return entityBinder;
        }

        public async Task<EntryMap<ReceiptDetail>> MapReferencesAsync(ReceiptDetail entity)
        {
            var entityBinder = await MapForeignKeysAsync(entity,
                MapWith<Receipt>(nameof(entity.ReceiptID), entity.ReceiptID, true),
                MapWith<RemarkDiscountItem>(nameof(entity.RemarkDiscountID), entity.RemarkDiscountID, true),
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),
                MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),           
                MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID)
            );
            return entityBinder;
        }

        public async Task<EntryMap<MultiPaymentMeans>> MapReferencesAsync(MultiPaymentMeans entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Receipt>(nameof(entity.ReceiptID), entity.ReceiptID, true), // Master ID
                MapWith<Currency>(nameof(entity.AltCurrencyID), entity.AltCurrencyID),
                MapWith<Currency>(nameof(entity.PLCurrencyID), entity.PLCurrencyID),
                MapWith<PaymentMeans>(nameof(entity.PaymentMeanID), entity.PaymentMeanID)
            );
        }

        public async Task<EntryMap<FreightReceipt>> MapReferencesAsync(FreightReceipt entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Receipt>(nameof(entity.ReceiptID), entity.ReceiptID, true), // Master ID
                MapWith<Freight>(nameof(entity.FreightID), entity.FreightID)
            );
        }

        public async Task<EntryMap<ReceiptMemo>> MapReferencesAsync(ReceiptMemo entity)
        {
            var entityBinder = await MapForeignKeysAsync(entity,
                MapWith<Series>(nameof(entity.SeriesID), entity.SeriesID, true), // Generate at server
                MapWith<SeriesDetail>(nameof(entity.SeriesDID), entity.SeriesDID, true), // Generate at server
                MapWith<PromoCodeDiscount>(nameof(entity.PromoCodeID), entity.PromoCodeID, true),
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),
                MapWith<UserAccount>(nameof(entity.UserDiscountID), entity.UserDiscountID, true),
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
                MapWith<BusinessPartner>(nameof(entity.CustomerID), entity.CustomerID),
                MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),          
                MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID),
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),
                MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
                MapWith<Currency>(nameof(entity.PLCurrencyID), entity.PLCurrencyID),
                MapWith<UserAccount>(nameof(entity.UserOrderID), entity.UserOrderID)            
            );
            return entityBinder;
        }

        public async Task<EntryMap<ReceiptDetailMemo>> MapReferencesAsync(ReceiptDetailMemo entity)
        {
            var entityBinder = await MapForeignKeysAsync(entity,
                MapWith<ReceiptMemo>(nameof(entity.ReceiptMemoID), entity.ReceiptMemoID, true), //Master ID
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),
                MapWith<RemarkDiscountItem>(nameof(entity.RemarkDiscountID), entity.RemarkDiscountID, true),  
                MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),                           
                MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID)
            );
            return entityBinder;
        }

        public async Task<EntryMap<VoidOrder>> MapReferencesAsync(VoidOrder entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<RemarkDiscountItem>(nameof(entity.RemarkDiscountID), entity.RemarkDiscountID, true),
                MapWith<UserAccount>(nameof(entity.UserDiscountID), entity.UserDiscountID, true),
                MapWith<PBuyXAmountGetXDis>(nameof(entity.BuyXAmountGetXDisID), entity.BuyXAmountGetXDisID, true),
                MapWith<PaymentMeans>(nameof(entity.PaymentMeansID), entity.PaymentMeansID, true),
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),
                MapWith<PromoCodeDiscount>(nameof(entity.PromoCodeID), entity.PromoCodeID, true), 
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),            
                MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),             
                MapWith<Currency>(nameof(entity.PLCurrencyID), entity.PLCurrencyID),                       
                MapWith<UserAccount>(nameof(entity.UserOrderID), entity.UserOrderID),
                MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID),
                MapWith<BusinessPartner>(nameof(entity.CustomerID), entity.CustomerID)                          
            );
        }

        public async Task<EntryMap<VoidOrderDetail>> MapReferencesAsync(VoidOrderDetail entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<RemarkDiscountItem>(nameof(entity.OrderID), entity.OrderID, true), //Master ID
                MapWith<RemarkDiscountItem>(nameof(entity.RemarkDiscountID), entity.RemarkDiscountID, true),
                MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),              
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID),
                MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID)
           );
        }

        public async Task<EntryMap<VoidItem>> MapReferencesAsync(VoidItem entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),
                MapWith<PBuyXAmountGetXDis>(nameof(entity.BuyXAmountGetXDisID), entity.BuyXAmountGetXDisID, true),
                MapWith<PaymentMeans>(nameof(entity.PaymentMeansID), entity.PaymentMeansID, true),
                MapWith<UserAccount>(nameof(entity.UserDiscountID), entity.UserDiscountID, true),
                MapWith<PromoCodeDiscount>(nameof(entity.PromoCodeID), entity.PromoCodeID, true),
                MapWith<RemarkDiscountItem>(nameof(entity.RemarkDiscountID), entity.RemarkDiscountID, true),
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
                MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),              
                MapWith<Currency>(nameof(entity.PLCurrencyID), entity.PLCurrencyID),
                MapWith<UserAccount>(nameof(entity.UserOrderID), entity.UserOrderID),
                MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID),
                MapWith<BusinessPartner>(nameof(entity.CustomerID), entity.CustomerID)    
           );
        }

        public async Task<EntryMap<VoidItemDetail>> MapReferencesAsync(VoidItemDetail entity)
        {
            return await MapForeignKeysAsync(entity,   
                MapWith<VoidItem>(nameof(entity.VoidItemID), entity.VoidItemID, true), //Master ID
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID, true),                
                MapWith<RemarkDiscountItem>(nameof(entity.RemarkDiscountID), entity.RemarkDiscountID, true),        
                MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),
                MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID)                 
            );
        }

        public async Task<EntryMap<OpenShift>> MapReferencesAsync(OpenShift entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),
                MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
                MapWith<UserAccount>(nameof(entity.UserID), entity.UserID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
                MapWith<Receipt>(nameof(entity.Trans_From), entity.Trans_From)
            );
        }

        public async Task<EntryMap<CloseShift>> MapReferencesAsync(CloseShift entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),
                MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
                MapWith<UserAccount>(nameof(entity.UserID), entity.UserID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
                MapWith<Receipt>(nameof(entity.Trans_From), entity.Trans_From),
                MapWith<Receipt>(nameof(entity.Trans_To), entity.Trans_To)
            );
        }
    }

    //Server part
    public partial class CustomEntityContext : EntityContext, ICustomEntityContext
    {
        public async Task<EntryMap<Company>> MapReferencesAsync(Company entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),
                MapWith<Currency>(nameof(entity.SystemCurrencyID), entity.SystemCurrencyID)
            );
        }

        public async Task<EntryMap<Branch>> MapReferencesAsync(Branch entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<Currency>(nameof(entity.CompanyID), entity.CompanyID)
           );
        }

        public async Task<EntryMap<PriceLists>> MapReferencesAsync(PriceLists entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<Currency>(nameof(entity.CurrencyID), entity.CurrencyID)
            );
        }

        public async Task<EntryMap<PriceListDetail>> MapReferencesAsync(PriceListDetail entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),
              MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),
              MapWith<Promotion>(nameof(entity.PromotionID), entity.PromotionID),
              MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
              MapWith<UserAccount>(nameof(entity.UserID), entity.UserID),
              MapWith<Currency>(nameof(entity.CurrencyID), entity.CurrencyID)
            );
        }

        public async Task<EntryMap<Employee>> MapReferencesAsync(Employee entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID)
            );
        }

        public async Task<EntryMap<UserAccount>> MapReferencesAsync(UserAccount entity)
        {
            entity.RefreshTokens = null;
            return await MapForeignKeysAsync(entity,
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
                MapWith<Employee>(nameof(entity.EmployeeID), entity.EmployeeID)
            );
        }

        public async Task<EntryMap<UserPrivillege>> MapReferencesAsync(UserPrivillege entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<UserAccount>(nameof(entity.UserID), entity.UserID),
                MapWith<Function>(nameof(entity.FunctionID), entity.FunctionID)
            );
        }

        public async Task<EntryMap<ExchangeRate>> MapReferencesAsync(ExchangeRate entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Currency>(nameof(entity.CurrencyID), entity.CurrencyID)
            );
        }

        public async Task<EntryMap<GroupTable>> MapReferencesAsync(GroupTable entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Branch>(nameof(entity.BranchID), entity.BranchID)
            );
        }

        public async Task<EntryMap<Table>> MapReferencesAsync(Table entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<GroupTable>(nameof(entity.GroupTableID), entity.GroupTableID),
                MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID)
            );
        }

        public async Task<EntryMap<ItemGroup2>> MapReferencesAsync(ItemGroup2 entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<ItemGroup1>(nameof(entity.ItemG1ID), entity.ItemG1ID)
            );
        }

        public async Task<EntryMap<ItemGroup3>> MapReferencesAsync(ItemGroup3 entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<ItemGroup1>(nameof(entity.ItemG1ID), entity.ItemG1ID),
               MapWith<ItemGroup2>(nameof(entity.ItemG2ID), entity.ItemG2ID)
           );
        }

        public async Task<EntryMap<ItemMasterData>> MapReferencesAsync(ItemMasterData entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<UnitofMeasure>(nameof(entity.InventoryUoMID), entity.InventoryUoMID),
                MapWith<UnitofMeasure>(nameof(entity.BaseUomID), entity.BaseUomID),
                MapWith<UnitofMeasure>(nameof(entity.SaleUomID), entity.SaleUomID),
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<GroupUOM>(nameof(entity.GroupUomID), entity.GroupUomID),
                MapWith<ItemGroup1>(nameof(entity.ItemGroup1ID), entity.ItemGroup1ID),
                MapWith<ItemGroup2>(nameof(entity.ItemGroup2ID), entity.ItemGroup2ID),
                MapWith<ItemGroup3>(nameof(entity.ItemGroup3ID), entity.ItemGroup3ID),
                MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),
                MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID),
                MapWith<TaxGroup>(nameof(entity.TaxGroupPurID), entity.TaxGroupPurID),
                MapWith<TaxGroup>(nameof(entity.TaxGroupSaleID), entity.TaxGroupSaleID)
            );
        }

        public async Task<EntryMap<BuyXGetX>> MapReferencesAsync(BuyXGetX entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<PriceLists>(nameof(entity.PriListID), entity.PriListID)
           );
        }

        public async Task<EntryMap<BuyXGetXDetail>> MapReferencesAsync(BuyXGetXDetail entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<BuyXGetX>(nameof(entity.BuyXGetXID), entity.BuyXGetXID),
                MapWith<ItemMasterData>(nameof(entity.BuyItemID), entity.BuyItemID),
                MapWith<ItemMasterData>(nameof(entity.GetItemID), entity.GetItemID),
                MapWith<UnitofMeasure>(nameof(entity.GetUomID), entity.GetUomID),
                MapWith<UnitofMeasure>(nameof(entity.ItemUomID), entity.ItemUomID)
            );
        }

        public async Task<EntryMap<BuyXQtyGetXDis>> MapReferencesAsync(BuyXQtyGetXDis entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<ItemMasterData>(nameof(entity.BuyItemID), entity.BuyItemID),
                MapWith<ItemMasterData>(nameof(entity.DisItemID), entity.DisItemID),
                MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID)
            );
        }

        public async Task<EntryMap<PBuyXAmountGetXDis>> MapReferencesAsync(PBuyXAmountGetXDis entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<PriceLists>(nameof(entity.PriListID), entity.PriListID)
           );
        }

        public async Task<EntryMap<Warehouse>> MapReferencesAsync(Warehouse entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<Branch>(nameof(entity.BranchID), entity.BranchID)
            );
        }

        public async Task<EntryMap<BusinessPartner>> MapReferencesAsync(BusinessPartner entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),
              MapWith<GLAccount>(nameof(entity.GLAccID), entity.GLAccID),
              MapWith<GLAccount>(nameof(entity.GLAccDepositID), entity.GLAccDepositID)
            );
        }

        public async Task<EntryMap<TaxGroup>> MapReferencesAsync(TaxGroup entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
                MapWith<GLAccount>(nameof(entity.GLID), entity.GLID)
            );
        }

        public async Task<EntryMap<TaxGroupDefinition>> MapReferencesAsync(TaxGroupDefinition entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<TaxGroup>(nameof(entity.TaxGroupID), entity.TaxGroupID)
            );
        }

        public async Task<EntryMap<CanRingMaster>> MapReferencesAsync(CanRingMaster entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
               MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
               MapWith<Currency>(nameof(entity.LocalCurrencyID), entity.LocalCurrencyID),
               MapWith<Currency>(nameof(entity.SysCurrencyID), entity.SysCurrencyID),
               MapWith<UserAccount>(nameof(entity.UserID), entity.UserID),
               MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID),
               MapWith<DocumentType>(nameof(entity.DocTypeID), entity.DocTypeID),
               MapWith<PaymentMeans>(nameof(entity.PaymentMeanID), entity.PaymentMeanID),
               MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),
               MapWith<Series>(nameof(entity.SeriesID), entity.SeriesID),
               MapWith<SeriesDetail>(nameof(entity.SeriesDID), entity.SeriesDID)
           );
        }

        public async Task<EntryMap<CanRing>> MapReferencesAsync(CanRing entity)
        {
            return await MapForeignKeysAsync(entity,
                MapWith<CanRing>(nameof(entity.CanRingID), entity.CanRingID),
                MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
                MapWith<UnitofMeasure>(nameof(entity.UomChangeID), entity.UomChangeID), 
                MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),
                MapWith<ItemMasterData>(nameof(entity.ItemChangeID), entity.ItemChangeID),
                MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),             
                MapWith<UserAccount>(nameof(entity.UserID), entity.UserID)                      
            );
        }

        public async Task<EntryMap<CanRingDetail>> MapReferencesAsync(CanRingDetail entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<CanRing>(nameof(entity.CanRingID), entity.CanRingID),
               MapWith<CanRingMaster>(nameof(entity.CanRingMasterID), entity.CanRingMasterID),
               MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
               MapWith<UnitofMeasure>(nameof(entity.UomChangeID), entity.UomChangeID),
               MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),
               MapWith<ItemMasterData>(nameof(entity.ItemChangeID), entity.ItemChangeID),
               MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID),
               MapWith<UserAccount>(nameof(entity.UserID), entity.UserID)             
           );
        }

        public async Task<EntryMap<Series>> MapReferencesAsync(Series entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
               MapWith<PeriodIndicator>(nameof(entity.PeriodIndID), entity.PeriodIndID),
               MapWith<DocumentType>(nameof(entity.DocuTypeID), entity.DocuTypeID)
           );
        }

        public async Task<EntryMap<SeriesDetail>> MapReferencesAsync(SeriesDetail entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<Series>(nameof(entity.SeriesID), entity.SeriesID)
            );
        }

        public async Task<EntryMap<Promotion>> MapReferencesAsync(Promotion entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID)
            );
        }

        public async Task<EntryMap<SaleCombo>> MapReferencesAsync(SaleCombo entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
               MapWith<PriceLists>(nameof(entity.PriListID), entity.PriListID),
               MapWith<UserAccount>(nameof(entity.CreatorID), entity.CreatorID),
               MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID)
           );
        }

        public async Task<EntryMap<SaleComboDetail>> MapReferencesAsync(SaleComboDetail entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
               MapWith<SaleCombo>(nameof(entity.SaleComboID), entity.SaleComboID)
           );
        }

        public async Task<EntryMap<Redeem>> MapReferencesAsync(Redeem entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<Branch>(nameof(entity.BranchID), entity.BranchID),
               MapWith<OpenShift>(nameof(entity.OpenShiftID), entity.OpenShiftID),
               MapWith<BusinessPartner>(nameof(entity.CustomerID), entity.CustomerID),
               MapWith<Series>(nameof(entity.SeriesID), entity.SeriesID),
               MapWith<UserAccount>(nameof(entity.UserID), entity.UserID)
           );
        }

        public async Task<EntryMap<RedeemRetail>> MapReferencesAsync(RedeemRetail entity)
        {
            return await MapForeignKeysAsync(entity,
               MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),
               MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
               MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID)
           );
        }

        public async Task<EntryMap<PromoCodeDiscount>> MapReferencesAsync(PromoCodeDiscount entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID)
            );
        }

        public async Task<EntryMap<PromoCodeDetail>> MapReferencesAsync(PromoCodeDetail entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<PromoCodeDiscount>(nameof(entity.PromoCodeID), entity.PromoCodeID)
            );
        }

        public async Task<EntryMap<Property>> MapReferencesAsync(Property entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID)
            );
        }

        public async Task<EntryMap<PropertyDetails>> MapReferencesAsync(PropertyDetails entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID)
            );
        }

        public async Task<EntryMap<ChildPreoperty>> MapReferencesAsync(ChildPreoperty entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<Property>(nameof(entity.ProID), entity.ProID)
            );
        }

        public async Task<EntryMap<PaymentMeans>> MapReferencesAsync(PaymentMeans entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<GLAccount>(nameof(entity.AccountID), entity.AccountID),
              MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID),
              MapWith<Currency>(nameof(entity.AltCurrencyID), entity.AltCurrencyID)
            );
        }

        public async Task<EntryMap<Freight>> MapReferencesAsync(Freight entity)
        {
           return await MapForeignKeysAsync(entity,
             MapWith<GLAccount>(nameof(entity.ExpenAcctID), entity.ExpenAcctID),
             MapWith<GLAccount>(nameof(entity.RevenAcctID), entity.RevenAcctID)
           );
        }

        public async Task<EntryMap<MemberCard>> MapReferencesAsync(MemberCard entity)
        {
            return await MapForeignKeysAsync(entity,
             MapWith<CardType>(nameof(entity.CardTypeID), entity.CardTypeID)
           );
        }

        public async Task<EntryMap<PointCard>> MapReferencesAsync(PointCard entity)
        {
            return await MapForeignKeysAsync(entity,
             MapWith<PriceLists>(nameof(entity.PriceListID), entity.PriceListID)
           );
        }

        public async Task<EntryMap<PointRedemption>> MapReferencesAsync(PointRedemption entity)
        {
            return await MapForeignKeysAsync(entity,
             MapWith<BusinessPartner>(nameof(entity.CustomerID), entity.CustomerID),
             MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID)
           );
        }

        public async Task<EntryMap<PointItem>> MapReferencesAsync(PointItem entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<PointRedemption>(nameof(entity.PointRedemptID), entity.PointRedemptID),
              MapWith<ItemMasterData>(nameof(entity.ItemID), entity.ItemID),
              MapWith<UnitofMeasure>(nameof(entity.UomID), entity.UomID),
              MapWith<Warehouse>(nameof(entity.WarehouseID), entity.WarehouseID)
            );
        }

        public async Task<EntryMap<PeriodIndicator>> MapReferencesAsync(PeriodIndicator entity)
        {
            return await MapForeignKeysAsync(entity,
             MapWith<Company>(nameof(entity.CompanyID), entity.CompanyID)
           );
        }

        public async Task<EntryMap<ReceiptInformation>> MapReferencesAsync(ReceiptInformation entity)
        {
            return await MapForeignKeysAsync(entity,
              MapWith<Branch>(nameof(entity.BranchID), entity.BranchID)
            );
        }
    }
}
