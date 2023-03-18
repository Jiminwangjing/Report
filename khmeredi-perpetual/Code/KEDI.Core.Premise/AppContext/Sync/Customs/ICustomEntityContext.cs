using KEDI.Core.Premise.Models.Sync;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using CKBS.AppContext;
using KEDI.Core.Premise.Repositories.Sync;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using CKBS.Models.Services.Inventory.Property;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using PointCard = KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints.PointCard;
using CKBS.Models.Services.POS.KVMS;

namespace KEDI.Core.Premise.AppContext.Sync.Customs
{
    public partial interface ICustomEntityContext //Clients' entities
    {
        Task<EntryMap<Receipt>> MapReferencesAsync(Receipt entity);
        Task<EntryMap<ReceiptDetail>> MapReferencesAsync(ReceiptDetail entity);
        Task<EntryMap<ReceiptMemo>> MapReferencesAsync(ReceiptMemo entity);
        Task<EntryMap<ReceiptDetailMemo>> MapReferencesAsync(ReceiptDetailMemo entity);
        Task<EntryMap<MultiPaymentMeans>> MapReferencesAsync(MultiPaymentMeans entity);
        Task<EntryMap<FreightReceipt>> MapReferencesAsync(FreightReceipt entity);
        Task<EntryMap<VoidOrder>> MapReferencesAsync(VoidOrder entity);
        Task<EntryMap<VoidOrderDetail>> MapReferencesAsync(VoidOrderDetail entity);
        Task<EntryMap<VoidItem>> MapReferencesAsync(VoidItem entity);
        Task<EntryMap<VoidItemDetail>> MapReferencesAsync(VoidItemDetail entity);
        Task<EntryMap<OpenShift>> MapReferencesAsync(OpenShift entity);
        Task<EntryMap<CloseShift>> MapReferencesAsync(CloseShift entity);
    }

    public partial interface ICustomEntityContext //Server's entities
    {
        Task<EntryMap<Company>> MapReferencesAsync(Company entity);
        Task<EntryMap<Branch>> MapReferencesAsync(Branch entity);
        Task<EntryMap<PriceLists>> MapReferencesAsync(PriceLists entity);
        Task<EntryMap<PriceListDetail>> MapReferencesAsync(PriceListDetail entity);
        Task<EntryMap<Employee>> MapReferencesAsync(Employee entity);
        Task<EntryMap<UserAccount>> MapReferencesAsync(UserAccount entity);
        Task<EntryMap<UserPrivillege>> MapReferencesAsync(UserPrivillege entity);
        Task<EntryMap<ExchangeRate>> MapReferencesAsync(ExchangeRate entity);
        Task<EntryMap<GroupTable>> MapReferencesAsync(GroupTable entity);
        Task<EntryMap<Table>> MapReferencesAsync(Table entity);
        Task<EntryMap<ItemGroup2>> MapReferencesAsync(ItemGroup2 entity);
        Task<EntryMap<ItemGroup3>> MapReferencesAsync(ItemGroup3 entity);
        Task<EntryMap<ItemMasterData>> MapReferencesAsync(ItemMasterData entity);
        Task<EntryMap<BuyXGetX>> MapReferencesAsync(BuyXGetX entity);
        Task<EntryMap<BuyXGetXDetail>> MapReferencesAsync(BuyXGetXDetail entity);
        Task<EntryMap<BuyXQtyGetXDis>> MapReferencesAsync(BuyXQtyGetXDis entity);
        Task<EntryMap<PBuyXAmountGetXDis>> MapReferencesAsync(PBuyXAmountGetXDis entity);
        Task<EntryMap<Warehouse>> MapReferencesAsync(Warehouse entity);
        Task<EntryMap<BusinessPartner>> MapReferencesAsync(BusinessPartner entity);
        Task<EntryMap<TaxGroup>> MapReferencesAsync(TaxGroup entity);
        Task<EntryMap<TaxGroupDefinition>> MapReferencesAsync(TaxGroupDefinition entity);
        Task<EntryMap<CanRingMaster>> MapReferencesAsync(CanRingMaster entity);
        Task<EntryMap<CanRing>> MapReferencesAsync(CanRing entity);
        Task<EntryMap<CanRingDetail>> MapReferencesAsync(CanRingDetail entity);
        Task<EntryMap<Series>> MapReferencesAsync(Series entity);
        Task<EntryMap<SeriesDetail>> MapReferencesAsync(SeriesDetail entity);
        Task<EntryMap<Promotion>> MapReferencesAsync(Promotion entity);
        Task<EntryMap<SaleCombo>> MapReferencesAsync(SaleCombo entity);
        Task<EntryMap<SaleComboDetail>> MapReferencesAsync(SaleComboDetail entity);     
        Task<EntryMap<Redeem>> MapReferencesAsync(Redeem entity);
        Task<EntryMap<RedeemRetail>> MapReferencesAsync(RedeemRetail entity);
        Task<EntryMap<PromoCodeDiscount>> MapReferencesAsync(PromoCodeDiscount entity);
        Task<EntryMap<PromoCodeDetail>> MapReferencesAsync(PromoCodeDetail entity);
        Task<EntryMap<Property>> MapReferencesAsync(Property entity);
        Task<EntryMap<PropertyDetails>> MapReferencesAsync(PropertyDetails entity);
        Task<EntryMap<ChildPreoperty>> MapReferencesAsync(ChildPreoperty entity);
        Task<EntryMap<PaymentMeans>> MapReferencesAsync(PaymentMeans entity);
        Task<EntryMap<Freight>> MapReferencesAsync(Freight entity);
        Task<EntryMap<MemberCard>> MapReferencesAsync(MemberCard entity);
        Task<EntryMap<PointCard>> MapReferencesAsync(PointCard entity);
        Task<EntryMap<PointRedemption>> MapReferencesAsync(PointRedemption entity);
        Task<EntryMap<PointItem>> MapReferencesAsync(PointItem entity);
        Task<EntryMap<PeriodIndicator>> MapReferencesAsync(PeriodIndicator entity);
        Task<EntryMap<ReceiptInformation>> MapReferencesAsync(ReceiptInformation entity);
    }
}
