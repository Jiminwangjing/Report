using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGoodsReceiptPoReturn
    {
        IEnumerable<Company> GetCurrencies();
        IEnumerable<GroupDUoM> GetAllgroupDUoMs();
        IEnumerable<ServiceMapItemMasterDataPurchaseCreditMemo> ServiceMapItemMasterDataGoodRetrun(int ID);
        void GoodReturnPO(int PurchaseMemoID, string InvoiceAP);
        IEnumerable<ServiceMapItemMasterDataPurchaseCreditMemo> ServiceMapItemMasterDataGoodReturnDetail(int warehouseID, string invoice);
        IEnumerable<ReportPurchasCreditMemo> ReportGoodReturn(int BranchID, int WarehoseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check);
    }
    public class GoodsReceiptPoReturnResponsitory : IGoodsReceiptPoReturn
    {
        private readonly DataContext _context;
        public GoodsReceiptPoReturnResponsitory(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<Company> GetCurrencies()
        {
            IEnumerable<Company> list = (
                from pd in _context.PriceLists.Where(x => x.Delete == false)
                join com in _context.Company.Where(x => x.Delete == false) on
                pd.ID equals com.PriceListID
                join cur in _context.Currency.Where(x => x.Delete == false)
                 on pd.CurrencyID equals cur.ID
                select new Company
                {
                    PriceList = new PriceLists
                    {
                        Currency = new Currency
                        {
                            Description = cur.Description
                        }
                    }
                }

                );
            return list;
        }
        public IEnumerable<GroupDUoM> GetAllgroupDUoMs()
        {
            var uom = _context.UnitofMeasures.Where(u => u.Delete == false);
            var guom = _context.GroupUOMs.Where(g => g.Delete == false);
            var duom = _context.GroupDUoMs.Where(o => o.Delete == false);
            var list = (from du in duom
                        join g_uom in guom on du.GroupUoMID equals g_uom.ID
                        join buo in uom on du.BaseUOM equals buo.ID
                        join auo in uom on du.AltUOM equals auo.ID
                        where g_uom.Delete == false
                        select new GroupDUoM
                        {
                            ID = du.ID,
                            GroupUoMID = du.GroupUoMID,
                            UoMID = du.UoMID,
                            AltQty = du.AltQty,
                            BaseUOM = du.BaseUOM,
                            AltUOM = du.AltUOM,
                            BaseQty = du.BaseQty,
                            Factor = du.Factor,

                            UnitofMeasure = new UnitofMeasure
                            {
                                ID = auo.ID,
                                Name = buo.Name,
                                AltUomName = auo.Name
                            },

                        }
                );
            return list;
        }
        public IEnumerable<ServiceMapItemMasterDataPurchaseCreditMemo> ServiceMapItemMasterDataGoodRetrun(int ID) => _context.ServiceMapItemMasterDataPurchaseCreditMemos.FromSql("sp_GetListItemMasterDataPurchaseCreditMemo @WarehouseID={0}",
           parameters: new[] {
                ID.ToString()
           });
        public void GoodReturnPO(int PurchaseMemoID, string InvoiceAP)
        {
            _context.Database.ExecuteSqlCommand("sp_GoodsReturnPO @PurchaseMemoID={0},@InvoiceAp={1}",
                parameters: new[] {
                    PurchaseMemoID.ToString(),
                    InvoiceAP.ToString()
                });
        }
        public IEnumerable<ServiceMapItemMasterDataPurchaseCreditMemo> ServiceMapItemMasterDataGoodReturnDetail(int warehouseID, string invoice) => _context.ServiceMapItemMasterDataPurchaseCreditMemos.FromSql("sp_GetListItemMasterDataGoodReturn_Detail @WarehouseID={0},@InvoiceNo={1}",
           parameters: new[] {
                warehouseID.ToString(),
                invoice
           });
        public IEnumerable<ReportPurchasCreditMemo> ReportGoodReturn(int BranchID, int WarehoseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check) => _context.ReportPurchasCreditMemos.FromSql("sp_ReportGoodReturn @BranchID={0},@WarehouseID={1},@PostingDate={2},@DocumentDate={3},@DeliveryDate={4},@Check={5}",
          parameters: new[] {
                BranchID.ToString(),
                WarehoseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                DeliveryDate.ToString(),
                Check.ToString()
          });
    }
}
