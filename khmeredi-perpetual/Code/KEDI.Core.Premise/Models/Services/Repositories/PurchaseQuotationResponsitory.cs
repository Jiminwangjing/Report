using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Purchase;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPurchaseQuotation
    {
        IEnumerable<ServiceMapItemMasterDataQuotation> ServiceMapItemMasterDatas(int ID);
        IEnumerable<ExchangeRate> GetExchangeRates(int ID);
        IEnumerable<ServiceQuotationDetail> ServiceQuotationDetails (int ID);
        IEnumerable<ReportPurchaseQuotation> GetPurchaseQuotations(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string RequierdDate, string Check);
        IEnumerable<Company> GetCurrencyDefualt();
        IEnumerable<ServiceMapItemMasterDataQuotation> GetQuotationDetail(int WarehoseID, string Invoice);
    }
    public class PurchaseQuotationResponsitory : IPurchaseQuotation
    {
        public readonly DataContext _context;
        public PurchaseQuotationResponsitory(DataContext dataContext)
        {
            _context = dataContext;
        }

        public IEnumerable<ExchangeRate> GetExchangeRates(int ID)
        {
            IQueryable<ExchangeRate> list =from ex in _context.ExchangeRates.Where(x=>x.Delete==false) join
                                           cur in _context.Currency.Where(x=>x.Delete==false) on
                                           ex.CurrencyID equals cur.ID
                                           where cur.ID==ID
                                           select new ExchangeRate
                                           {
                                              Rate=ex.Rate,
                                              ID=ex.ID,
                                              CurrencyID=ex.CurrencyID,
                                              Currency=new Currency
                                              {
                                                  Description=cur.Description + "(" +cur.Symbol+ ")"
                                              }
                                           };
            return list;
        }

      
        public IEnumerable<ServiceMapItemMasterDataQuotation> ServiceMapItemMasterDatas(int ID) => _context.ServiceMapItemMasterDatas.FromSql("sp_GetListItemMasterDataQuotation @WarehouseID={0}",
                parameters: new[] {
                    ID.ToString()
                });

        public IEnumerable<ServiceQuotationDetail> ServiceQuotationDetails(int ID) => _context.ServiceQuotationDetails.FromSql("sp_GetQuotaionDetail @QID={0}",
            parameters: new[] {
                ID.ToString()
            });

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

        public IEnumerable<ServiceMapItemMasterDataQuotation> GetQuotationDetail(int WarehoseID, string Invoice) => _context.ServiceMapItemMasterDatas.FromSql("sp_GetListItemMasterDataPurchaseQuotation_Detail @WarehouseID={0},@InvoiceNo={1}",
            parameters:new[] {
               WarehoseID.ToString(),
               Invoice.ToString()
            });

        public IEnumerable<ReportPurchaseQuotation> GetPurchaseQuotations(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string RequierdDate, string Check) => _context.ReportPurchaseQuotations.FromSql("sp_ReportPurchaseQuotation @BranchID={0},@warehouseID={1},@PostingDate={2},@DeliveryDate={3},@DocumentDate={4},@Check={5}",
            parameters:new[] {
                BranchID.ToString(),
                WarehouseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                RequierdDate.ToString(),
                Check.ToString()
            });


    }
}
