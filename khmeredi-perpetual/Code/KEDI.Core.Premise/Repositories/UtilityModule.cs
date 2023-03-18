using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.POS;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using Microsoft.Extensions.Configuration;

namespace KEDI.Core.Premise.Repository
{
    public class UtilityModule
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configure;
        public UtilityModule(DataContext context, IConfiguration iConfig)
        {
            _context = context;
            _configure = iConfig;
        }

        public string PrintTemplateUrl()
        {
            var url = _configure.GetSection("PrintTempateUrl").Value;
            return url;
        }
        public DisplayCurrency GetBaseCurrency(int plid, int plCurId)
        {
            var baseCurrency = _context.DisplayCurrencies.FirstOrDefault(i => i.PriceListID == plid && i.AltCurrencyID == plCurId) ?? new DisplayCurrency();
            return baseCurrency ?? new DisplayCurrency();
        }

        public DisplayCurrency GetAltCurrency(int plid)
        {
            var altCurrency = _context.DisplayCurrencies.FirstOrDefault(i => i.PriceListID == plid && i.IsActive) ?? new DisplayCurrency();
            return altCurrency ?? new DisplayCurrency();
        }

        public string ToCurrencySplit(double amount, double decimalPlaces = 3)
        {
            return ToCurrencySplit((decimal)amount, decimalPlaces);
        }

        public string ToCurrencySplit(decimal amount, double decimalPlaces = 3)
        {
            var zeroes = "";
            decimalPlaces = Math.Abs(decimalPlaces);
            zeroes = zeroes.PadLeft(Convert.ToInt32(decimalPlaces), '0');
            string value = amount.ToString().Contains(".") ? $"{amount}{zeroes}" : $"{amount}.{zeroes}";
            if (decimalPlaces <= 0)
            {
                value = value.Split(".")[0];
            }
            else
            {
                string[] splitValues = value.Split(".");
                string dotValue = $"{splitValues[1]}".Substring(0, Convert.ToInt32(decimalPlaces));
                value = $"{splitValues[0]}.{dotValue}";
            }
            return value;
        }

        public string ToCurrency(decimal amount, double decimalPlaces = 3)
        {
            var zeroes = "";
            zeroes = zeroes.PadLeft(Convert.ToInt32(decimalPlaces), '0');
            string value = string.Format("{0:#,0." + zeroes + "}", amount);

            if (decimalPlaces <= 0)
            {
                value = value.Split(".")[0];
            }
            else
            {
                string[] splitValues = value.Split(".");
                string dotValue = splitValues[1].Substring(0, Convert.ToInt32(decimalPlaces));
                value = $"{splitValues[0]}.{dotValue}";
            }
            return value;
        }
        public string ToCurrency(double amount, double decimalPlaces = 3)
        {
            var zeroes = "";
            zeroes = zeroes.PadLeft(Convert.ToInt32(decimalPlaces), '0');
            string value = string.Format("{0:#,0." + zeroes + "}", amount);

            if (decimalPlaces <= 0)
            {
                value = value.Split(".")[0];
            }
            else
            {
                string[] splitValues = value.Split(".");
                string dotValue = splitValues[1].Substring(0, Convert.ToInt32(decimalPlaces));
                value = $"{splitValues[0]}.{dotValue}";
            }
            return value;
        }

        public decimal ToDecimal(string value)
        {
            string numberStr = value;
            if (value.Contains(","))
            {
                string[] values = value.Split(",");
                numberStr = string.Join("", values);
            }
            return Convert.ToDecimal(numberStr);
        }

        public double ToDouble(string value)
        {
            string numberStr = value;
            if (value.Contains(","))
            {
                string[] values = value.Split(",");
                numberStr = string.Join("", values);
            }
            return Convert.ToDouble(numberStr);
        }

        public GeneralSettingAdminViewModel GetGeneralSettingAdmin(int curId = 0)
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            if (curId > 0)
            {
                display = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curId) ?? new Display();
            }
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;
        }

        public double CheckNaNOrInfinity(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                value = 0;
            }
            return value;
        }
        public void UpdateAvgCost(int itemid, int whid, int guomid, double qty, double avgcost)
        {
            avgcost = CheckNaNOrInfinity(avgcost);
            // update pricelistdetial
            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemid).ToList();
            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemid).ToList();
            double totalTransVal = inventory_audit.Sum(s => s.Trans_Valuse);
            double toalQty = inventory_audit.Sum(q => q.Qty);
            double @AvgCost = (totalTransVal + (avgcost * qty)) / (toalQty + qty);
            @AvgCost = CheckNaNOrInfinity(@AvgCost);
            @AvgCost = @AvgCost < 0 ? 0 : @AvgCost;
            foreach (var pri in pri_detial)
            {
                var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == guomid && g.AltUOM == pri.UomID);
                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                pri.Cost = @AvgCost * exp.SetRate * guom.Factor;
                _context.PriceListDetails.Update(pri);
                _context.SaveChanges();
            }
            //update_waresummary
            var inventory_warehouse = _context.InventoryAudits.Where(w => w.ItemID == itemid && w.WarehouseID == whid).ToList();
            var waresummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemid && w.WarehouseID == whid);
            double @AvgCostWare = (inventory_warehouse.Sum(s => s.Trans_Valuse) + (avgcost * qty)) / (inventory_warehouse.Sum(s => s.Qty) + qty);
            @AvgCostWare = CheckNaNOrInfinity(@AvgCostWare);
            @AvgCostWare = @AvgCostWare < 0 ? 0 : @AvgCostWare;
            waresummary.Cost = @AvgCostWare;
            _context.WarehouseSummary.Update(waresummary);
            _context.SaveChanges();
        }
        public void CumulativeValue(int whid, int itemid, double value, ItemAccounting itemAcc)
        {
            value = CheckNaNOrInfinity(value);
            var wherehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == whid && w.ItemID == itemid) ?? new WarehouseSummary();
            wherehouse.CumulativeValue = (decimal)value;
            _context.WarehouseSummary.Update(wherehouse);
            if (itemAcc != null) itemAcc.CumulativeValue = wherehouse.CumulativeValue;
            _context.SaveChanges();
        }
        public void UpdateBomCost(int itemid, double qty, double avgcost)
        {
            avgcost = CheckNaNOrInfinity(avgcost);
            var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == itemid).ToList();
            foreach (var itembom in ItemBOMDetail)
            {
                var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID).ToList();
                double @AvgCost = (Inven.Sum(s => s.Trans_Valuse) + avgcost) / (Inven.Sum(q => q.Qty) + qty);
                @AvgCost = CheckNaNOrInfinity(@AvgCost);
                var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID).ToList();
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                itembom.Cost = @AvgCost;
                itembom.Amount = itembom.Qty * @AvgCost;
                _context.BOMDetail.UpdateRange(itembom);
                _context.SaveChanges();
                // sum 
                var BOM = _context.BOMaterial.FirstOrDefault(w => w.BID == itembom.BID);
                var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && !w.Detele).ToList();
                BOM.TotalCost = DBOM.Sum(s => s.Amount);
                _context.BOMaterial.Update(BOM);
                _context.SaveChanges();
            }
        }
        public void UpdateItemAccounting(ItemAccounting itemAcc, WarehouseSummary ws)
        {
            if (itemAcc != null)
            {
                itemAcc.Committed = ws.Committed;
                itemAcc.InStock = ws.InStock;
                itemAcc.Ordered = ws.Ordered;
                itemAcc.Available = ws.Available;
                _context.ItemAccountings.Update(itemAcc);
                _context.SaveChanges();
            }
        }
        public double CalAVGCost(int itemId, int warehouseId, InventoryAudit inventoryAudit, bool isIn = true)
        {
            var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == itemId && w.WarehouseID == warehouseId).ToList();
            double tranVal = inventoryAudit.Cost * inventoryAudit.Qty;
            double totalTranVal = inventoryAcct.Sum(s => s.Trans_Valuse);
            double totalQty = inventoryAcct.Sum(q => q.Qty);
            double avgCost = 0;
            if (isIn) avgCost = CheckNaNOrInfinity((totalTranVal + tranVal) / (totalQty + inventoryAudit.Qty));
            else avgCost = CheckNaNOrInfinity((totalTranVal - tranVal) / (totalQty - inventoryAudit.Qty));
            return avgCost < 0 ? 0 : avgCost;
        }

        public ProcessItem CheckProcessItem(string process)
        {
            return process switch
            {
                "SEBA" => ProcessItem.SEBA,
                "FIFO" => ProcessItem.FIFO,
                "Average" => ProcessItem.Average,
                "FEFO" => ProcessItem.FEFO,
                _ => ProcessItem.Standard,
            };
        }
        public List<SeriesInPurchasePoViewModel> GetSeries(string code)
        {
            var series = (from dt in _context.DocumentTypes.Where(i => i.Code == code)
                          join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                          select new SeriesInPurchasePoViewModel
                          {
                              ID = sr.ID,
                              Name = sr.Name,
                              Default = sr.Default,
                              NextNo = sr.NextNo,
                              DocumentTypeID = sr.DocuTypeID,
                              SeriesDetailID = _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault() == null ? 0 :
                              _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault().ID
                          }).ToList();
            return series;
        }

        public bool IsBetweenDate(DateTime dateFrom, DateTime dateTo, DateTime? input = null)
        {
            DateTime inputDate = input ?? DateTime.Now;
            bool isBetween = dateFrom <= inputDate && inputDate <= dateTo;
            return isBetween;
        }
         public bool IsBetweenDateEmtyTime(DateTime dateFrom, DateTime dateTo, DateTime? input = null)
        {
            DateTime inputDate = input ?? DateTime.Now;
            bool isBetween = dateFrom <= DateTime.Today && DateTime.Today <= dateTo;
            return isBetween;
        }
       
        public DateTime ConcatDateTime(string dateString, string timeString)
        {
            DateTime.TryParse(dateString, out DateTime _date);
            DateTime.TryParse(timeString, out DateTime _dateTime);
            TimeSpan _time = _dateTime.TimeOfDay;
            _date = _date.Add(_time);
            return _date;
        }
        public DateTime ConcatDateTime(DateTime dateObj, TimeSpan timeObj)
        {
            return dateObj.Add(timeObj);
        }

        public string NoSpace(string value)
        {
            return Regex.Replace(value, "\\s+", string.Empty);
        }

        public string ToAlphaNumeric(string input){
            return Regex.Replace(input, "[^0-9a-zA-Z]+", string.Empty);
        }
    }
}
