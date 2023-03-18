using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.InventoryAuditReport;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPriceList
    {
        IQueryable<PriceLists> GetPriceLists(); 
        PriceLists GetId(int id);
        Task AddOrEdit(PriceLists pricelist);
        Task<int> Deletepricelist(int id);
        IEnumerable<ItemMasterDataService> ItemMasters(string Action,int ID);
        void UpdatePriceListDetail(ServiceDetail dataService);
        IEnumerable<ServiceInventoryAudit> GetInventoryAuditsByItem(int ItemID,int BranchID,int UomID);
        IEnumerable<ServiceInventoryAudit> GetInventoryAuditsFilterSystemDate(string DateFrom, string DateTo, int ItemID, int BranchID,int UomID);
         Task<List<ServicePriceListCopyItem>> GetItemMasterToCopy(int pricelistbase,int pricelistid, int group1, int group2, int group3);
        void InsertIntoPricelist(ItemCopyToPriceList itemCopyToPriceList); 
    }
    public class PriceListRepository : IPriceList
    {
        private readonly DataContext _context;

       

        public PriceListRepository(DataContext context)
        {
            _context = context;
        }
        public async Task AddOrEdit(PriceLists pricelist)
        {
            if (pricelist.ID == 0)
            {
                await _context.PriceLists.AddAsync(pricelist);
                await _context.SaveChangesAsync();
            }
            else
            {
               
                _context.PriceLists.Update(pricelist);
                await _context.SaveChangesAsync();
            }
            
        }
        public async Task<int> Deletepricelist(int id)
        {
            var pric = await _context.PriceLists.FirstAsync(p => p.ID ==id);
            pric.Delete = true;
            return await _context.SaveChangesAsync();
        }

        public PriceLists GetId(int id) => _context.PriceLists.Find(id);
       
        public IQueryable<PriceLists> GetPriceLists()
        {
            IQueryable<PriceLists> list = (from price in _context.PriceLists.Where(p => p.Delete == false)
                                          join
                                           cur in _context.Currency.Where(c => c.Delete == false) on
                                           price.CurrencyID equals cur.ID
                                          select new PriceLists
                                          {
                                              ID = price.ID,
                                              Name = price.Name,
                                              CurrencyID = price.CurrencyID,
                                              Currency = new Currency
                                              {
                                                  Symbol = cur.Symbol
                                              },
                                              CurrencyName = cur.Symbol
                                          }
                );
            return list;
        }

        public void UpdatePriceListDetail(ServiceDetail dataService)
        {
           foreach(var item in dataService.SetpriceDatail)
            {
                if(item.UnitPrice!=null)
                {
                    if (item.Cost == null)
                    {
                        item.Cost = "0";
                    }
                    var pricelistdetail = _context.PriceListDetails.FirstOrDefault(x => x.ID == item.ID);
                    pricelistdetail.Cost = Convert.ToDouble(item.Cost);
                    pricelistdetail.UnitPrice = Convert.ToDouble(item.UnitPrice);
                    _context.Update(pricelistdetail);
                    _context.SaveChanges();
                }
                
            }
           
        }
        public IEnumerable<ItemMasterDataService> ItemMasters(string Action,int Id)=>
        
            _context.ItemMasterDataServices.FromSql("sp_GetItemPriceListDetail @Action={0},@PriceListId={1}",
               parameters: new[] {
                    Action.ToString(),
                    Id.ToString()
               });

        public IEnumerable<ServiceInventoryAudit> GetInventoryAuditsByItem(int ItemID,int BranchID,int UomID) => _context.ServiceInventoryAudits.FromSql("sp_GetInventoryAudit @ItemID={0},@BrachID={1},@uomID={2}",
            parameters:new[] {
               ItemID.ToString(),
               BranchID.ToString(),
               UomID.ToString()
            });

        public IEnumerable<ServiceInventoryAudit> GetInventoryAuditsFilterSystemDate(string DateFrom, string DateTo, int ItemID, int BranchID,int UomID) => _context.ServiceInventoryAudits.FromSql("sp_GetInventoryAuditFilterSystemDate @DateFrom={0},@DateTo={1},@ItemID={2},@BranchID={3},@UomID={4}",
            parameters: new[] {
                DateFrom,
                DateTo,
                ItemID.ToString(),
                BranchID.ToString(),
                UomID.ToString()
            });
       
        public async Task<List<ServicePriceListCopyItem>> GetItemMasterToCopy(int pricelistbase, int pricelistid,int group1, int group2,int group3){

         var list= await(from i in   _context.ServicePriceListCopyItem.FromSql("sp_GetItemMasterToCopy @PriceListBaseID={0},@PriceListID={1},@Group1={2},@Group2={3},@Group3={4}",
              parameters: new[] {
                    pricelistbase.ToString(),
                    pricelistid.ToString(),
                    group1.ToString(),
                    group2.ToString(),
                    group3.ToString()
              })
              select new ServicePriceListCopyItem
              {
                 Code= i.Code,
                 Barcode= i.Barcode,
                ItemID= i.ItemID,
                KhmerName = i.KhmerName,
                EnglishName = i.EnglishName,
                UoM = i.UoM,
                Process = i.Process,
                Active = false,
 
              }).OrderBy(x=> x.KhmerName).ToListAsync();
            
              return list;
    }
        public void InsertIntoPricelist(ItemCopyToPriceList itemCopyToPriceList)
        {
            if (itemCopyToPriceList.ItemCopyToPriceListDetail.Count == 0)
            {
                _context.Database.ExecuteSqlCommand("sp_InsertPriceListDatail @Process={0},@FromPriceListID={1},@ToPriceListID={2}",
                parameters: new[] {
                    "A", //insert all item
                    itemCopyToPriceList.FromPriceListID.ToString(),
                    itemCopyToPriceList.ToPriceListID.ToString()
                });
                _context.SaveChanges();
            }
            else
            {
                _context.ItemCopyToPriceList.Add(itemCopyToPriceList);
                _context.SaveChanges();

                _context.Database.ExecuteSqlCommand("sp_InsertPriceListDatail @Process={0},@FromPriceListID={1},@ToPriceListID={2}",
               parameters: new[] {
                    "S", //insert spcific item
                    itemCopyToPriceList.FromPriceListID.ToString(),
                    itemCopyToPriceList.ToPriceListID.ToString()
               });
               
            }
        }
    }
}
