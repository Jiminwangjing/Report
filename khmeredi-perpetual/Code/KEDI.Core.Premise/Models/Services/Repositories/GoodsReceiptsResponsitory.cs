using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ClassCopy;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Purchase;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGoodsReceipts
    {
        IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs(int ID);
        IEnumerable<ServiceMapItemMasterDataPurchasAP> FindItemBarcode(int WarehouseID,string Barcode);
        IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs_Detail(int warehouseid, string invoice);
        IEnumerable<ExchangeRate> GetExchangeRates(int ID);
        void GoodReceiptStock(int purchaseID,string type);
        IEnumerable<ReportPurchaseAP> GetReportPurchaseAPs(int BranchID,int WarehouseID,string PostingDate,string DocumentDate ,string DeliverDate,string Check);
        IEnumerable<Company> GetCurrencies();
        IEnumerable<ReportPurchaseOrder> GetAllPruchaseOrder(int BranchID);
        IEnumerable<ReportPurchaseAP> GetALlGoodReceiptPO(int BranchID);
        IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseOrder(int ID, string Invoice);
        IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseGoodReceipt(int ID, string Invoice);
    }
    public class GoodsReceiptsResponsitory : IGoodsReceipts
    {
        private readonly DataContext _context;
        public GoodsReceiptsResponsitory(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseGoodReceipt(int ID, string Invoice) => _context.PurchaseAP_To_PurchaseMemos.FromSql("sp_GetPurchaseAP_From_PurchaseGoodRecepitPO @PurhcasePOID={0},@Invoice={1}",
           parameters: new[] {
                  ID.ToString(),
                Invoice.ToString()

           });
        public IEnumerable<ServiceMapItemMasterDataPurchasAP> FindItemBarcode(int WarehouseID, string Barcode) => _context.ServiceMapItemMasterDataPurchasAPs.FromSql("sp_FindBarcodePurchaseAP @WarehouseID={0},@Barcode={1}",
           parameters:new[] {
               WarehouseID.ToString(),
               Barcode.ToString()
           });
    
        public IEnumerable<ReportPurchaseOrder> GetAllPruchaseOrder(int BranchID) => _context.ReportPurchaseOrders.FromSql("sp_GetAllPurchaeOrder @BranchID={0}",
            parameters:new[] {
                BranchID.ToString()
            });

        public IEnumerable<ReportPurchaseAP> GetALlGoodReceiptPO(int BranchID) => _context.ReportPurchaseAPs.FromSql("sp_GetAllGoodReceiptPO @BranchID={0}",
            parameters:new[] {
                BranchID.ToString()
                });
        
        public IEnumerable<Company> GetCurrencies()
        {
            IEnumerable<Company> list = (
                from pd in _context.PriceLists.Where(x => x.Delete == false)
                join com in _context.Company.Where(x => x.Delete == false) on
                pd.ID equals com.PriceListID join cur in _context.Currency.Where(x => x.Delete == false)
                on pd.CurrencyID equals cur.ID
                select new Company {
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

        public IEnumerable<ExchangeRate> GetExchangeRates(int ID)
        {
            IEnumerable<ExchangeRate> list = from ex in _context.ExchangeRates.Where(x => x.Delete == false)
                                            join
                                           cur in _context.Currency.Where(x => x.Delete == false) on
                                           ex.CurrencyID equals cur.ID
                                            where cur.ID == ID
                                            select new ExchangeRate
                                            {
                                                Rate = ex.Rate,
                                                ID = ex.ID,
                                                CurrencyID = ex.CurrencyID,
                                                Currency = new Currency
                                                {
                                                    Description =cur.Description
                                                }
                                            };
            return list;
        }

        public IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseOrder(int ID, string Invoice) => _context.PurchaseAP_To_PurchaseMemos.FromSql("sp_GetPurchaseAP_form_PurchaseOrder @PurchaseOrderID={0},@Invoice={1}",
            parameters:new[] {
                ID.ToString(),
                Invoice.ToString()
            });
        
        public IEnumerable<ReportPurchaseAP> GetReportPurchaseAPs(int BranchID,int WarehouseID,string PostingDate,string DocumentDate,string DeliverDate,string Check) => _context.ReportPurchaseAPs.FromSql("sp_ReportPurchaseAP @BranchID={0},@warehouseID={1},@PostingDate={2},@DeliveryDate={3},@DocumentDate={4},@Check={5}",
            parameters: new[] {
                BranchID.ToString(),
                WarehouseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                DeliverDate.ToString(),
                Check.ToString()
            });

        public void GoodReceiptStock(int purchaseID,string type)
        {
            _context.Database.ExecuteSqlCommand("sp_GoodReceiptStockAP @purchaseID={0},@type={1}",
                parameters: new[] {
                   purchaseID.ToString(),
                   type.ToString()
                });
        }

        public IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs(int ID) => _context.ServiceMapItemMasterDataPurchasAPs.FromSql("sp_GetListItemMasterDataPurchaseAP @WarehouseID={0}",
            parameters:new[] {
                ID.ToString()
            });

        public IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs_Detail(int warehouseid,string invoice)=> _context.ServiceMapItemMasterDataPurchasAPs.FromSql("sp_GetListItemMasterDataPurchaseAP_Detail @WarehouseID={0},@InvoiceNo={1}",
            parameters: new[] {
                warehouseid.ToString(),
                invoice.ToString()
            });

    }
    
}
