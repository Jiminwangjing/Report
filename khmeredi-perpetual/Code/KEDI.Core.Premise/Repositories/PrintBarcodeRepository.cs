using CKBS.AppContext;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.PrintBarcode;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrinterName = CKBS.Models.Services.Administrator.General.PrinterName;

namespace KEDI.Core.Premise.Repository
{
    public interface IPrintBarcodeRepository
    {
        Task<List<ItemPrintBarcodeView>> GetItemMasterBaseOnPriceListAsync(int plId, int count, string setting);
        Task<List<ItemMasterData>> GetItemMasterDataAsync();
        Task<List<PriceLists>> GetPriceListsAsync();
        Task<List<PrinterName>> GetPrinterNames();
        Task PrintBarcodeItemsAsync(List<ItemPrintBarcodeView> items, string printerName);
    }
    public class PrintBarcodeRepository : IPrintBarcodeRepository
    {
        private readonly DataContext _context;
        private readonly IPOS _pos;
        public PrintBarcodeRepository(DataContext context, IPOS pos)
        {
            _context = context;
            _pos = pos;
        }

        public async Task PrintBarcodeItemsAsync(List<ItemPrintBarcodeView> items, string printerName)
        {
            await _pos.PrintBarcodeItemsAsync(items, printerName);

        }

        public async Task<List<ItemMasterData>> GetItemMasterDataAsync()
        {
            var data = await _context.ItemMasterDatas.Where(i => !i.Delete).ToListAsync();
            return data;
        }

        public async Task<List<ItemPrintBarcodeView>> GetItemMasterBaseOnPriceListAsync(int plId, int count, string setting)
        {
            if (setting == "4")
            {
                var data = await (from pld in _context.PriceListDetails.Where(i => i.PriceListID == plId)
                                  join item in _context.ItemMasterDatas.Where(i => !i.Delete) on pld.ItemID equals item.ID
                                  join cur in _context.Currency on pld.CurrencyID equals cur.ID
                                  join ex in _context.DisplayCurrencies.Where(i => i.IsActive) on pld.PriceListID equals ex.PriceListID
                                  join curr in _context.Currency on ex.AltCurrencyID equals curr.ID

                                  join prd in _context.PropertyDetails on item.ID equals prd.ItemID
                                  join pr_COO in _context.Property.Where(w => w.Name == "Net Weight") on prd.ProID equals pr_COO.ID
                                  join crp_COO in _context.ChildPreoperties on prd.Value equals crp_COO.ID

                                  join prd2 in _context.PropertyDetails on item.ID equals prd2.ItemID
                                  join pr_SBD in _context.Property.Where(w => w.Name == "Sell By Date") on prd2.ProID equals pr_SBD.ID
                                  join crp_SBD in _context.ChildPreoperties on prd2.Value equals crp_SBD.ID
                                  select new ItemPrintBarcodeView
                                  {

                                      UnitPrice = cur.Symbol + " " + string.Format("{0:#,0.00}", pld.UnitPrice),
                                      ItemCode = item.Code,
                                      ItemID = item.ID,
                                      ItemName = item.KhmerName,
                                      ItemName2 = item.EnglishName,
                                      ItemBarcode = item.Barcode,
                                      Description = cur.Symbol,
                                      DescriptionChange = string.Format("{0:#,0.00}", curr.Symbol),
                                      IsSelected = false,
                                      ItemDes = item.Description ?? " ",
                                      Setting = setting,
                                      Count = count,
                                      Salebydate = DateTime.Today.AddDays(Convert.ToDouble(crp_SBD.Name)).ToShortDateString(),
                                      NetWeight = crp_COO.Name,
                                  }).ToListAsync();
                return data;
            }
            else
            {
                var data = await (from pld in _context.PriceListDetails.Where(i => i.PriceListID == plId)
                                  join item in _context.ItemMasterDatas.Where(i => !i.Delete) on pld.ItemID equals item.ID
                                  join cur in _context.Currency on pld.CurrencyID equals cur.ID
                                  join ex in _context.DisplayCurrencies.Where(i => i.IsActive) on pld.PriceListID equals ex.PriceListID
                                  join curr in _context.Currency on ex.AltCurrencyID equals curr.ID
                                  group new { pld, item, cur, ex, curr } by new { pld.ItemID } into r
                                  let datas = r.FirstOrDefault()
                                  select new ItemPrintBarcodeView
                                  {

                                      UnitPrice = datas.cur.Symbol + " " + string.Format("{0:#,0.00}", datas.pld.UnitPrice),
                                      ItemCode = datas.item.Code,
                                      ItemID = datas.item.ID,
                                      ItemName = datas.item.KhmerName,
                                      ItemName2 = datas.item.EnglishName,
                                      ItemBarcode = datas.item.Barcode,
                                      Description = datas.cur.Symbol,
                                      DescriptionChange = string.Format("{0:#,0.00}", datas.curr.Symbol),
                                      IsSelected = false,
                                      ItemDes = datas.item.Description ?? " ",
                                      Setting = setting,
                                      Count = count,

                                  }).ToListAsync();
                return data;
            }

        }

        public async Task<List<PriceLists>> GetPriceListsAsync()
        {
            var data = await _context.PriceLists.Where(i => !i.Delete).ToListAsync();
            return data;
        }

        public async Task<List<PrinterName>> GetPrinterNames()
        {
            var data = await _context.PrinterNames.Where(i => !i.Delete).ToListAsync();
            return data;
        }
    }
}
