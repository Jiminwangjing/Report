using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
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
    public interface IPurchaseRequest
    {
        IEnumerable<Company> GetCurrencyDefualt();
        IEnumerable<GroupDUoM> GetAllgroupDUoMs();
        IEnumerable<ServiceMapItemPurchaseRequest> ServiceMapItemPurchaseRequests(int WarehouseID);
        IEnumerable<ServiceMapItemPurchaseRequest> FindBarcode(int WarehouseID, string Barcode);
        IEnumerable<ServiceMapItemPurchaseRequest> GetDetailPurchaseRequest(int WarehouseID, string Invoice);
    }
    public class PurchaseRquestResponsitory:IPurchaseRequest
    {
        private readonly DataContext _context;

       

        public PurchaseRquestResponsitory(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<Company> GetCurrencyDefualt()
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

        public IEnumerable<ServiceMapItemPurchaseRequest> ServiceMapItemPurchaseRequests(int WarehouseID) => _context.ServiceMapItemPurchaseRequests.FromSql("sp_GetListItemMasterDataPurchaseRequest @WarehouseID={0}",
         parameters:new[] {
             WarehouseID.ToString()
         });

        public IEnumerable<ServiceMapItemPurchaseRequest> FindBarcode(int WarehouseID, string Barcode) => _context.ServiceMapItemPurchaseRequests.FromSql("sp_FindBarcodePurchaseRequset @WarehouseID={0},@Barcode={1}",
            parameters:new[] {
                WarehouseID.ToString(),
                Barcode.ToString()
            });

        public IEnumerable<ServiceMapItemPurchaseRequest> GetDetailPurchaseRequest(int WarehouseID, string Invoice) => _context.ServiceMapItemPurchaseRequests.FromSql("sp_GetDetailPurchaseRequest @WarehouseID={0},@InvoiceNo={1}",
            parameters:new[] {
                WarehouseID.ToString(),
                Invoice.ToString()
            });
       
    }
}
