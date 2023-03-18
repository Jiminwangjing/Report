using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.LoyaltyProgram.ComboSale;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Production;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram.ComboSale;
using KEDI.Core.Premise.Models.ServicesClass.BuyXGetX;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SetGlAccount = CKBS.Models.Services.Inventory.SetGlAccount;
using Type = CKBS.Models.Services.Financials.Type;

namespace KEDI.Core.Premise.Repository
{
    public class LoyaltyProgramPosModule
    {
        readonly ILogger<LoyaltyProgramPosModule> _logger;
        readonly BuyXGetXPosModule _buyXgetX;
        readonly DataContext _dataContext;
        readonly MemberPointModule _memberPoint;
        readonly IBusinessPartner _partner;
        readonly ICompany _company;
        readonly UserManager _userModule;
        public LoyaltyProgramPosModule(ILogger<LoyaltyProgramPosModule> logger, IBusinessPartner partner,
            DataContext dataContext, BuyXGetXPosModule buyXgetX, MemberPointModule memberPoint, ICompany company, UserManager userModule)
        {
            _logger = logger;
            _buyXgetX = buyXgetX;
            _dataContext = dataContext;
            _memberPoint = memberPoint;
            _partner = partner;
            _company = company;
            _userModule = userModule;
        }

        public async Task<List<MemberCardModel>> GetMemberCardsAsync(string keyword = "")
        {
            try
            {
                var today = DateTime.Today;
                var memberCards = await _dataContext.MemberCards.Include(c => c.CardType).Where(w => !w.Delete && w.ExpireDate >= today)
                                .Select(m => new MemberCardModel
                                {
                                    ID = m.ID,
                                    RefNo = m.Ref_No,
                                    Name = m.Name,
                                    CardType = m.CardType.Name,
                                    Description = m.Description,
                                    Discount = m.CardType.Discount.ToString("N3"),
                                    ExpireDate = m.ExpireDate.ToShortDateString(),
                                    DiscountType = m.CardType.TypeDis
                                }).Where(m => string.Compare(m.DiscountType, "percent", true) == 0).ToListAsync();
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    memberCards = memberCards.Where(m =>
                    RawWord(m.RefNo).Contains(keyword, ignoreCase)
                    || RawWord(m.CardType).Contains(keyword, ignoreCase)
                    || RawWord(m.Name).Contains(keyword, ignoreCase)
                    ).ToList();
                }
                return memberCards;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return await Task.FromResult(new List<MemberCardModel>());
            }
        }

        public async Task<List<BuyXGetXDetailModel>> GetBuyXGetXDetailsAsync(int priceListId)
        {
            return await _buyXgetX.GetBuyXGetXDetailsAsync(priceListId);
        }

        public async Task<List<BusinessPartner>> GetAvailablePointMembersAsync()
        {
            var pointRedempts = (await _memberPoint.GetPointRedemptsAsync())
                .Where(p => IsBetweenDate(p.DateFrom, p.DateTo));
            var customers = (await _memberPoint.GetCustomersAsync()).Where(c => pointRedempts.Any(p => p.Active && (decimal)p.PointQty <= c.OutstandPoint));

            return customers.ToList();
        }


        //public async Task<int> CountointsReceiptAsync(int customerId, PointTemplate point)
        //{
        //    try
        //    {
        //        var member = await _partner.FindCustomerAsync(customerId);
        //        member.OutstandPoint += point.Point;
        //        member.CumulativePoint += point.Point;
        //        _dataContext.BusinessPartners.Update(member);
        //        return await _dataContext.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //    }
        //    return -1;

        //}

        public async Task<int> CountPointsReceiptAsync(int customerId, PointTemplate point = null)
        {
            try
            {
                var member = await _partner.FindCustomerAsync(customerId);
                if (member.ID != 0)
                {
                    if (point != null)
                    {
                        member.OutstandPoint += point.Point;
                        member.CumulativePoint += point.Point;
                    }

                    _dataContext.BusinessPartners.Update(member);
                    return await _dataContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return -1;
        }

        public async Task<PointTemplate> CountPointOrder(Order order)
        {
            decimal netAmount = (decimal)order.GrandTotal;
            var pointcard = await _memberPoint.FindActivePointCardAsync(order.PriceListID);
            var member = await _partner.FindCustomerAsync(order.CustomerID);
            var factorAmount = (pointcard.PointQty > 0) ? (pointcard.Amount / pointcard.PointQty) : 0;
            decimal points = (factorAmount > 0) ? (netAmount / factorAmount) : 0;
            return await Task.FromResult(new PointTemplate
            {
                OutStandingPoint = member.OutstandPoint + points,
                Point = points
            });
        }

        public async Task<List<PointRedemption>> GetPointRedemptionWarehouseAsync(int customerId, int warehouseId)
        {
            var member = await _partner.FindCustomerAsync(customerId);
            var pointRedempts = await _memberPoint.GetPointRedemptsWarehouseAsync(warehouseId);
            pointRedempts = pointRedempts.Where(pr => (decimal)pr.PointQty <= member.OutstandPoint
                && pr.Active && IsBetweenDate(pr.DateFrom, pr.DateTo)).ToList();
            foreach (var pr in pointRedempts)
            {
                foreach (var pi in pr.PointItems)
                {
                    var itemMaster = _dataContext.ItemMasterDatas.FirstOrDefault(i => i.ID == pi.ItemID) ?? new ItemMasterData();
                    var uom = _dataContext.UnitofMeasures.FirstOrDefault(u => u.ID == pi.UomID && !u.Delete);
                    if (itemMaster.Process != "Standard")
                    {
                        var item = (from i in _dataContext.ItemMasterDatas
                                    join w in _dataContext.WarehouseSummary on i.ID equals w.ItemID
                                    select new
                                    {
                                        w.WarehouseID,
                                        LineID = i.ID,
                                        i.Code,
                                        i.KhmerName,
                                        Instock = w.InStock,
                                    }).FirstOrDefault(s => s.LineID == pi.ItemID && s.WarehouseID == warehouseId);

                        pi.ItemCode = item.Code;
                        pi.ItemName = item.KhmerName;
                        pi.UomName = uom?.Name;
                        pi.Instock = item.Instock;
                        pi.Process = itemMaster.Process;
                        pi.BaseItemQty = pi.Qty;
                    }
                    else
                    {
                        pi.ItemCode = itemMaster.Code;
                        pi.ItemName = itemMaster.KhmerName;
                        pi.UomName = uom?.Name;
                        pi.Instock = 0;
                        pi.Process = itemMaster.Process;
                        pi.BaseItemQty = pi.Qty;
                    }
                    pi.ID = 0;
                }
            }
            return pointRedempts ?? new List<PointRedemption>();
        }

        public async Task<PointRedemptionMaster> PostPointRedemptionsAsync(int customerId, PointRedemptionMaster point, DocumentType douType, List<SerialNumber> serials, List<BatchNo> batches)
        {
            var openShift = _dataContext.OpenShift.AsEnumerable().LastOrDefault(x => x.UserID == _userModule.CurrentUser.ID && x.Open) ?? new OpenShift();
            var member = await _partner.FindCustomerAsync(customerId);
            var com = _company.FindCompany();
            var lc = _dataContext.ExchangeRates.FirstOrDefault(e => e.CurrencyID == com.SystemCurrencyID);
            int inventoryAccID = 0;
            decimal inventoryAccAmount = 0;
            int expenAccID = 0;
            decimal expenAccAmount = 0;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _dataContext.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _dataContext.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
                // update series
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = defaultJE.ID;
                seriesDetail.Number = Sno;
                _dataContext.Update(defaultJE);
                _dataContext.Update(seriesDetail);
                _dataContext.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.TransNo = Sno;
                journalEntry.PostingDate = DateTime.Today;
                journalEntry.DocumentDate = DateTime.Today;
                journalEntry.DueDate = DateTime.Today;
                journalEntry.SSCID = com.SystemCurrencyID;
                journalEntry.LLCID = com.LocalCurrencyID;
                journalEntry.CompanyID = com.ID;
                journalEntry.LocalSetRate = (decimal)lc.SetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = defaultJE.Name + " " + Sno;
                _dataContext.Update(journalEntry);
                _dataContext.SaveChanges();
            }
            Redeem redeem = new();
            {
                redeem.CustomerID = customerId;
                redeem.SeriesID = point.SeriesID;
                redeem.UserID = point.UserID;
                redeem.BranchID = point.BranchID;
                redeem.DateIn = DateTime.Today;
                redeem.DateOut = DateTime.Today;
                redeem.Number = point.Number;
                redeem.RedeemPoint += (decimal)point.PointRedemptions.Sum(s => s.PointQty * s.Factor);
                redeem.OpenShiftID = openShift.ID;
            };
            _dataContext.Redeems.Add(redeem);
            double sumCost = 0;
            var priceList = _dataContext.PriceListDetails.Where(s => s.ItemID == point.PointRedemptions.FirstOrDefault().PointItems.FirstOrDefault().ItemID);
            sumCost = point.PointRedemptions.FirstOrDefault().PointItems.Sum(s => s.Qty * priceList.FirstOrDefault().Cost);
            // expense account //
            var items = _dataContext.GLAccounts.Where(s => s.ID == member.GLAccID);
            var glAcc = _dataContext.GLAccounts.FirstOrDefault(w => w.ID == items.FirstOrDefault().ID);
            //_dataContext.Update(glAcc);
            _dataContext.SaveChanges();
            foreach (var po in point.PointRedemptions)
            {
                //var pointsItem = po.PointItems.Where(pi => _dataContext.WarehouseSummary.Any(wd => pi.ItemID == wd.ItemID && wd.InStock > 0)).ToList();
                foreach (var item in po.PointItems)
                {
                    var itemMaster = _dataContext.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var itemPrice = _dataContext.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemID);
                    if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                    {
                        var inventoryAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == item.WarehouseID)
                                            join gl in _dataContext.GLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault();
                        var expenAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == item.WarehouseID)
                                        join gl in _dataContext.GLAccounts on ia.ExpenseAccount equals gl.Code
                                        select gl
                                             ).FirstOrDefault();
                        expenAccID = expenAcc.ID;
                        inventoryAccID = inventoryAcc.ID;
                    }
                    else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                    {
                        var inventoryAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                            join gl in _dataContext.GLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                             ).FirstOrDefault();
                        var expenAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                        join gl in _dataContext.GLAccounts on ia.ExpenseAccount equals gl.Code
                                        select gl
                                             ).FirstOrDefault();
                        expenAccID = expenAcc.ID;
                        expenAccAmount = (decimal)(item.Qty * itemPrice.Cost);
                        inventoryAccID = inventoryAcc.ID;
                    }

                    WarehouseDetail warehousedetail = new();
                    InventoryAudit item_inventory_audit = new();
                    if (itemMaster.Process != "Standard")
                    {
                        double @Check_Stock;
                        double @Remain;
                        double @IssusQty;
                        double @FIFOQty;
                        var baseUOM = _dataContext.GroupDUoMs.FirstOrDefault(w => w.UoMID == item.UomID);
                        var warehouse = _dataContext.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == item.WarehouseID);
                        var warehouseDetail = _dataContext.WarehouseDetails.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new WarehouseDetail();
                        var wareDetails = _dataContext.WarehouseDetails.Where(w => w.WarehouseID == point.WarehouseID && w.ItemID == item.ItemID).ToList();
                        var gd = _dataContext.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemMaster.GroupUomID && W.AltUOM == item.UomID);
                        var _itemAcc = _dataContext.ItemAccountings.FirstOrDefault(i => i.WarehouseID == point.WarehouseID && i.ItemID == item.ItemID);
                        double @Qty = item.Qty * gd.Factor;
                        double Cost = 0;
                        // update itmemasterdata
                        itemMaster.StockIn -= @Qty;
                        //update warehouse                    
                        warehouse.InStock -= @Qty;
                        warehouseDetail.InStock -= @Qty;
                        _dataContext.WarehouseDetails.Update(warehouseDetail);
                        _dataContext.ItemMasterDatas.Update(itemMaster);
                        _dataContext.WarehouseSummary.Update(warehouse);
                        _dataContext.SaveChanges();
                        UpdateItemAccounting(_itemAcc, warehouse);
                        //Checking Serial Batch //
                        if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                        {
                            if (serials.Count > 0)
                            {
                                List<WareForAudiView> wareForAudis = new();

                                foreach (var s in serials)
                                {
                                    if (s.SerialNumberSelected != null)
                                    {
                                        foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                        {
                                            var waredetial = wareDetails.FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _expAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                Cost = waredetial.Cost;
                                                wareForAudis.Add(new WareForAudiView
                                                {
                                                    Cost = waredetial.Cost,
                                                    Qty = waredetial.InStock,
                                                    ExpireDate = waredetial.ExpireDate,
                                                });
                                                waredetial.InStock -= 1;
                                                // insert to warehouse detail
                                                var stockOut = new StockOut
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (decimal)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = 1,
                                                    ItemID = waredetial.ItemID,
                                                    Location = waredetial.Location,
                                                    LotNumber = waredetial.LotNumber,
                                                    MfrDate = waredetial.MfrDate,
                                                    MfrSerialNumber = waredetial.MfrSerialNumber,
                                                    MfrWarDateEnd = waredetial.MfrWarDateEnd,
                                                    MfrWarDateStart = waredetial.MfrWarDateStart,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SerialNumber = waredetial.SerialNumber,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = point.UserID,
                                                    ExpireDate = waredetial.ExpireDate,
                                                    TransType = TransTypeWD.PointRedempt,
                                                    TransID = point.ID,
                                                    Contract = itemMaster.ContractID,
                                                    OutStockFrom = point.ID,
                                                    FromWareDetialID = waredetial.ID,
                                                    BPID = point.CustomerID
                                                };
                                                _inventoryAccAmount = (decimal)waredetial.Cost;
                                                _expAmount = (decimal)waredetial.Cost;
                                                _dataContext.StockOuts.Add(stockOut);
                                                _dataContext.SaveChanges();
                                            }
                                            FinancailRedemptionPoint(
                                                expenAccID, inventoryAccID, _expAmount, _inventoryAccAmount, journalEntryDetail,
                                                accountBalance, journalEntry, seriesDetail, douType, douTypeID, glAcc
                                            );
                                        }
                                    }
                                }
                                wareForAudis = (from wa in wareForAudis
                                                group wa by wa.Cost into g
                                                let wha = g.FirstOrDefault()
                                                select new WareForAudiView
                                                {
                                                    Qty = g.Sum(i => i.Qty),
                                                    Cost = wha.Cost,
                                                    ExpireDate = wha.ExpireDate,
                                                }).ToList();
                                if (wareForAudis.Any())
                                {
                                    foreach (var i in wareForAudis)
                                    {
                                        // Insert to Inventory Audit
                                        var inventory_audit = _dataContext.InventoryAudits
                                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID && w.Cost == i.Cost).ToList();
                                        //var item_IssusStock = wareDetails.FirstOrDefault(w => w.InStock > 0);
                                        var inventory = new InventoryAudit
                                        {
                                            ID = 0,
                                            WarehouseID = point.WarehouseID,
                                            BranchID = point.BranchID,
                                            UserID = point.UserID,
                                            ItemID = item.ItemID,
                                            CurrencyID = point.SysCurrencyID,
                                            UomID = gd.BaseUOM,
                                            InvoiceNo = point.Number,
                                            Trans_Type = douType.Code,
                                            Process = itemMaster.Process,
                                            SystemDate = DateTime.Now,
                                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                            Qty = i.Qty * -1,
                                            Cost = i.Cost,
                                            Price = 0,
                                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                            Trans_Valuse = i.Qty * i.Cost * -1,
                                            ExpireDate = i.ExpireDate,
                                            LocalCurID = point.LocalCurrencyID,
                                            LocalSetRate = point.LocalSetRate,
                                            CompanyID = point.CompanyID,
                                            DocumentTypeID = douType.ID,
                                            SeriesID = point.SeriesID,
                                            SeriesDetailID = point.SeriesDID,
                                        };
                                        CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                        _dataContext.InventoryAudits.Add(inventory);
                                        _dataContext.SaveChanges();
                                    }
                                }
                            }
                        }
                        else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                        {
                            if (batches.Count > 0)
                            {
                                List<WareForAudiView> wareForAudis = new();
                                foreach (var b in batches)
                                {
                                    if (b.BatchNoSelected != null)
                                    {
                                        foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                        {
                                            decimal selectedQty = sb.SelectedQty * (decimal)gd.Factor;
                                            var waredetial = wareDetails.FirstOrDefault(i => sb.BatchNo == i.BatchNo && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _expAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                wareForAudis.Add(new WareForAudiView
                                                {
                                                    Cost = waredetial.Cost,
                                                    Qty = (double)selectedQty,
                                                    ExpireDate = waredetial.ExpireDate,
                                                });
                                                waredetial.InStock -= (double)selectedQty;
                                                Cost = waredetial.Cost;
                                                // insert to waredetial
                                                var stockOut = new StockOut
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (decimal)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = selectedQty,
                                                    ItemID = item.ItemID,
                                                    Location = waredetial.Location,
                                                    MfrDate = waredetial.MfrDate,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = point.UserID,
                                                    ExpireDate = waredetial.ExpireDate,
                                                    BatchAttr1 = waredetial.BatchAttr1,
                                                    BatchAttr2 = waredetial.BatchAttr2,
                                                    BatchNo = waredetial.BatchNo,
                                                    TransType = TransTypeWD.PointRedempt,
                                                    TransID = point.ID,
                                                    OutStockFrom = point.ID,
                                                    FromWareDetialID = waredetial.ID,
                                                    BPID = point.CustomerID
                                                };
                                                _inventoryAccAmount = (decimal)waredetial.Cost * selectedQty;
                                                _expAmount = (decimal)waredetial.Cost * selectedQty;
                                                _dataContext.StockOuts.Add(stockOut);
                                                _dataContext.SaveChanges();
                                            }
                                            FinancailRedemptionPoint(
                                                expenAccID, inventoryAccID, _expAmount, _inventoryAccAmount, journalEntryDetail,
                                                accountBalance, journalEntry, seriesDetail, douType, douTypeID, glAcc
                                            );
                                        }
                                    }
                                }
                                wareForAudis = (from wa in wareForAudis
                                                group wa by wa.Cost into g
                                                let wha = g.FirstOrDefault()
                                                select new WareForAudiView
                                                {
                                                    Qty = g.Sum(i => i.Qty),
                                                    Cost = wha.Cost,
                                                    ExpireDate = wha.ExpireDate
                                                }).ToList();

                                if (wareForAudis.Any())
                                {
                                    foreach (var i in wareForAudis)
                                    {
                                        // insert to inventory audit
                                        var inventory_audit = _dataContext.InventoryAudits
                                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID && w.Cost == i.Cost).ToList();
                                        var inventory = new InventoryAudit
                                        {
                                            ID = 0,
                                            WarehouseID = point.WarehouseID,
                                            BranchID = point.BranchID,
                                            UserID = point.UserID,
                                            ItemID = item.ItemID,
                                            CurrencyID = point.SysCurrencyID,
                                            UomID = gd.BaseUOM,
                                            InvoiceNo = point.Number,
                                            Trans_Type = douType.Code,
                                            Process = itemMaster.Process,
                                            SystemDate = DateTime.Now,
                                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                            Qty = i.Qty * -1,
                                            Cost = i.Cost,
                                            Price = 0,
                                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - i.Qty,
                                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (i.Qty * i.Cost),
                                            Trans_Valuse = i.Qty * i.Cost * -1,
                                            ExpireDate = i.ExpireDate,
                                            LocalCurID = point.LocalCurrencyID,
                                            LocalSetRate = point.LocalSetRate,
                                            CompanyID = point.CompanyID,
                                            DocumentTypeID = douType.ID,
                                            SeriesID = point.SeriesID,
                                            SeriesDetailID = point.SeriesDID,
                                        };
                                        CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                        _dataContext.InventoryAudits.Add(inventory);
                                        _dataContext.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            var whs = wareDetails.Where(w => w.InStock > 0).ToList();
                            foreach (var item_warehouse in whs)
                            {
                                var item_IssusStock = whs.FirstOrDefault();
                                @Check_Stock = item_warehouse.InStock - @Qty;
                                if (@Check_Stock < 0)
                                {
                                    @Remain = (item_warehouse.InStock - @Qty) * (-1);
                                    @IssusQty = @Qty - @Remain;
                                    if (@Remain <= 0)
                                    {
                                        @Qty = 0;
                                    }
                                    else
                                    {
                                        @Qty = @Remain;
                                    }
                                    if (itemMaster.Process == "FIFO")
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_warehouse.Cost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = point.UserID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.PointRedempt,
                                                TransID = point.ID,
                                                OutStockFrom = point.ID,
                                                BPID = point.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _dataContext.StockOuts.Add(stockOuts);
                                            var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = point.WarehouseID;
                                            item_inventory_audit.BranchID = point.BranchID;
                                            item_inventory_audit.UserID = point.UserID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = point.SysCurrencyID;
                                            item_inventory_audit.UomID = gd.BaseUOM;
                                            item_inventory_audit.InvoiceNo = point.Number;
                                            item_inventory_audit.Trans_Type = douType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = point.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = point.LocalSetRate;
                                            item_inventory_audit.CompanyID = point.CompanyID;
                                            item_inventory_audit.DocumentTypeID = douType.ID;
                                            item_inventory_audit.SeriesID = point.SeriesID;
                                            item_inventory_audit.SeriesDetailID = point.SeriesDID;
                                        }
                                        inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        expenAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _dataContext.WarehouseDetails.Update(item_IssusStock);
                                        _dataContext.InventoryAudits.Add(item_inventory_audit);
                                        _dataContext.SaveChanges();
                                    }
                                    else if (itemMaster.Process == "Average")
                                    {
                                        item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _dataContext.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID);
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)@sysAvCost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.Average,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = point.UserID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.PointRedempt,
                                                TransID = point.ID,
                                                OutStockFrom = point.ID,
                                                BPID = point.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _dataContext.StockOuts.Add(stockOuts);

                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = point.WarehouseID;
                                            item_inventory_audit.BranchID = point.BranchID;
                                            item_inventory_audit.UserID = point.UserID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = point.SysCurrencyID;
                                            item_inventory_audit.UomID = gd.BaseUOM;
                                            item_inventory_audit.InvoiceNo = point.Number;
                                            item_inventory_audit.Trans_Type = douType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = point.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = point.LocalSetRate;
                                            item_inventory_audit.CompanyID = point.CompanyID;
                                            item_inventory_audit.DocumentTypeID = douType.ID;
                                            item_inventory_audit.SeriesID = point.SeriesID;
                                            item_inventory_audit.SeriesDetailID = point.SeriesDID;
                                        }
                                        var inventoryAcct = _dataContext.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                        double @AvgCost = (inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty);
                                        @AvgCost = CheckNaNOrInfinity(@AvgCost);
                                        inventoryAccAmount += (decimal)@AvgCost * (decimal)@IssusQty;
                                        expenAccAmount += (decimal)@AvgCost * (decimal)@IssusQty;
                                        UpdateAvgCost(item_warehouse.ItemID, point.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _dataContext.WarehouseDetails.Update(item_IssusStock);
                                        _dataContext.InventoryAudits.Add(item_inventory_audit);
                                        _dataContext.SaveChanges();
                                    }
                                }
                                else
                                {
                                    @FIFOQty = item_IssusStock.InStock - @Qty;
                                    @IssusQty = item_IssusStock.InStock - @FIFOQty;
                                    if (itemMaster.Process == "FIFO")
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)item_warehouse.Cost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.FIFO,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = point.UserID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.POS,
                                                TransID = point.ID,
                                                OutStockFrom = point.ID,
                                                BPID = point.CustomerID,
                                                FromWareDetialID = item_IssusStock.ID,
                                            };
                                            _dataContext.StockOuts.Add(stockOuts);
                                            var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = point.WarehouseID;
                                            item_inventory_audit.BranchID = point.BranchID;
                                            item_inventory_audit.UserID = point.UserID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = point.SysCurrencyID;
                                            item_inventory_audit.UomID = gd.BaseUOM;
                                            item_inventory_audit.InvoiceNo = point.Number;
                                            item_inventory_audit.Trans_Type = douType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = item_IssusStock.Cost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = point.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = point.LocalSetRate;
                                            item_inventory_audit.CompanyID = point.CompanyID;
                                            item_inventory_audit.DocumentTypeID = douType.ID;
                                            item_inventory_audit.SeriesID = point.SeriesID;
                                            item_inventory_audit.SeriesDetailID = point.SeriesDID;
                                        }
                                        inventoryAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        expenAccAmount += (decimal)item_inventory_audit.Cost * (decimal)@IssusQty;
                                        CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _dataContext.WarehouseDetails.Update(item_IssusStock);
                                        _dataContext.InventoryAudits.Add(item_inventory_audit);
                                        _dataContext.SaveChanges();
                                    }
                                    else if (itemMaster.Process == "Average")
                                    {
                                        item_IssusStock.InStock = @FIFOQty;
                                        if (@IssusQty > 0)
                                        {
                                            var warehouse_summary = _dataContext.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID);
                                            double @sysAvCost = warehouse_summary.Cost;
                                            var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID);

                                            var stockOuts = new StockOut
                                            {
                                                Cost = (decimal)@sysAvCost,
                                                CurrencyID = item_warehouse.CurrencyID,
                                                ID = 0,
                                                InStock = (decimal)@IssusQty,
                                                ItemID = item.ItemID,
                                                ProcessItem = ProcessItem.Average,
                                                SyetemDate = DateTime.Now,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = item_warehouse.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = point.UserID,
                                                ExpireDate = item_IssusStock.ExpireDate,
                                                TransType = TransTypeWD.PointRedempt,
                                                TransID = point.ID,
                                                OutStockFrom = point.ID,
                                                FromWareDetialID = item_IssusStock.ID,
                                                BPID = point.CustomerID
                                            };
                                            _dataContext.StockOuts.Add(stockOuts);
                                            item_inventory_audit.ID = 0;
                                            item_inventory_audit.WarehouseID = point.WarehouseID;
                                            item_inventory_audit.BranchID = point.BranchID;
                                            item_inventory_audit.UserID = point.UserID;
                                            item_inventory_audit.ItemID = item.ItemID;
                                            item_inventory_audit.CurrencyID = point.SysCurrencyID;
                                            item_inventory_audit.UomID = gd.BaseUOM;
                                            item_inventory_audit.InvoiceNo = point.Number;
                                            item_inventory_audit.Trans_Type = douType.Code;
                                            item_inventory_audit.Process = itemMaster.Process;
                                            item_inventory_audit.SystemDate = DateTime.Now;
                                            item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                            item_inventory_audit.Qty = @IssusQty * -1;
                                            item_inventory_audit.Cost = @sysAvCost;
                                            item_inventory_audit.Price = 0;
                                            item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                            item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                            item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                            item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                            item_inventory_audit.LocalCurID = point.LocalCurrencyID;
                                            item_inventory_audit.LocalSetRate = point.LocalSetRate;
                                            item_inventory_audit.CompanyID = point.CompanyID;
                                            item_inventory_audit.DocumentTypeID = douType.ID;
                                            item_inventory_audit.SeriesID = point.SeriesID;
                                            item_inventory_audit.SeriesDetailID = point.SeriesDID;
                                        }
                                        var inventoryAcct = _dataContext.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                        double @AvgCost = (inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty);
                                        @AvgCost = CheckNaNOrInfinity(@AvgCost);
                                        inventoryAccAmount += (decimal)@AvgCost * (decimal)@IssusQty;
                                        expenAccAmount += (decimal)@AvgCost * (decimal)@IssusQty;
                                        UpdateAvgCost(item_warehouse.ItemID, point.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                        CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                        _dataContext.WarehouseDetails.Update(item_IssusStock);
                                        _dataContext.InventoryAudits.Add(item_inventory_audit);
                                        _dataContext.SaveChanges();
                                    }
                                    wareDetails = new List<WarehouseDetail>();
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        var priceListDetail = _dataContext.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.PriceListID == point.PriceListID) ?? new PriceListDetail();
                        inventoryAccAmount = (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)point.PLRate;
                        expenAccAmount = (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)point.PLRate;
                    }
                    if (itemMaster.ManItemBy == ManageItemBy.None)
                    {
                        FinancailRedemptionPoint(
                            expenAccID, inventoryAccID, expenAccAmount, inventoryAccAmount, journalEntryDetail,
                            accountBalance, journalEntry, seriesDetail, douType, douTypeID, glAcc
                            );
                    }
                    RedeemRetail redeemRetail = new();
                    {
                        redeemRetail.RID = redeem.ID;
                        redeemRetail.ItemID = item.ItemID;
                        redeemRetail.ItemName = item.ItemName;
                        redeemRetail.Qty = (decimal)item.Qty;
                        redeemRetail.Uom = item.UomName;
                        redeemRetail.UomID = item.UomID;
                        redeemRetail.WarehouseID = item.WarehouseID;
                    }
                    _dataContext.RedeemDetails.Add(redeemRetail);
                    var journal = _dataContext.JournalEntries.Find(journalEntry.ID);
                    journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                    journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                    _dataContext.JournalEntries.Update(journal);
                    _dataContext.JournalEntryDetails.UpdateRange(journalEntryDetail);
                    _dataContext.AccountBalances.UpdateRange(accountBalance);
                    _dataContext.SaveChanges();
                }
                foreach (var item in po.PointItems)
                {
                    var itemM = _dataContext.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var orft = _dataContext.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                    var bom = _dataContext.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new BOMaterial();
                    var items_material = (from bomd in _dataContext.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                          join i in _dataContext.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                          join gd in _dataContext.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                          join uom in _dataContext.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                          select new
                                          {
                                              bomd.ItemID,
                                              gd.GroupUoMID,
                                              GUoMID = i.GroupUomID,
                                              Qty = (double)item.Qty * orft.Factor * ((double)bomd.Qty * gd.Factor),
                                              bomd.NegativeStock,
                                              i.Process,
                                              UomID = uom.ID,
                                              gd.Factor,
                                          }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                    if (items_material.Count > 0)
                    {
                        foreach (var itemBom in items_material.ToList())
                        {
                            int inventoryAccIDavg = 0, expAccIDavg = 0;
                            decimal inventoryAccAmountavg = 0, expAccAmountavg = 0;
                            var item_warehouse_material = _dataContext.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == point.WarehouseID && w.ItemID == itemBom.ItemID);
                            var all_item_warehouse_detail = _dataContext.WarehouseDetails.Where(w => w.WarehouseID == point.WarehouseID && w.ItemID == itemBom.ItemID);
                            var item_cost = _dataContext.PriceListDetails.FirstOrDefault(w => w.ItemID == itemBom.ItemID);

                            //update_warehouse_summary && itemmasterdata
                            var warehouseSummary = _dataContext.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemBom.ItemID && w.WarehouseID == point.WarehouseID);
                            var itemMaster = _dataContext.ItemMasterDatas.FirstOrDefault(w => w.ID == itemBom.ItemID);
                            var _itemAcc = _dataContext.ItemAccountings.FirstOrDefault(i => i.WarehouseID == point.WarehouseID && i.ItemID == itemBom.ItemID);
                            if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                            {
                                var inventoryAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID)
                                                    join gl in _dataContext.GLAccounts on ia.InventoryAccount equals gl.Code
                                                    select gl
                                                     ).FirstOrDefault() ?? new GLAccount();
                                var COGSAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == point.WarehouseID)
                                               join gl in _dataContext.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                               select gl
                                                     ).FirstOrDefault();
                                inventoryAccIDavg = inventoryAcc.ID;
                                expAccIDavg = COGSAcc.ID;
                            }
                            else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                            {
                                var inventoryAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                                    join gl in _dataContext.GLAccounts on ia.InventoryAccount equals gl.Code
                                                    select gl
                                                     ).FirstOrDefault() ?? new GLAccount();
                                var COGSAcc = (from ia in _dataContext.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                               join gl in _dataContext.GLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                               select gl
                                                     ).FirstOrDefault() ?? new GLAccount();
                                inventoryAccIDavg = inventoryAcc.ID;
                                expAccIDavg = COGSAcc.ID;
                            }
                            var inventoryAudit = new InventoryAudit();
                            var warehouseDetail = new WarehouseDetail();
                            double @Qty = itemBom.Qty;
                            double @Cost = item_cost.Cost;
                            warehouseSummary.InStock += @Qty;
                            itemMaster.StockIn += @Qty;
                            UpdateItemAccounting(_itemAcc, warehouseSummary);
                            //insert_warehousedetail
                            warehouseDetail.WarehouseID = point.WarehouseID;
                            warehouseDetail.UomID = itemBom.UomID;
                            warehouseDetail.UserID = point.UserID;
                            warehouseDetail.SyetemDate = point.PostingDate;
                            warehouseDetail.TimeIn = DateTime.Now;
                            warehouseDetail.InStock = @Qty;
                            warehouseDetail.CurrencyID = point.SysCurrencyID;
                            warehouseDetail.ItemID = itemBom.ItemID;
                            warehouseDetail.Cost = @Cost;
                            warehouseDetail.InStockFrom = point.ID;
                            warehouseDetail.IsDeleted = true;
                            warehouseDetail.BPID = point.CustomerID;
                            if (itemMaster.Process == "FIFO")
                            {
                                var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == itemBom.ItemID && w.WarehouseID == point.WarehouseID);
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = point.WarehouseID;
                                inventoryAudit.BranchID = point.BranchID;
                                inventoryAudit.UserID = point.UserID;
                                inventoryAudit.ItemID = itemBom.ItemID;
                                inventoryAudit.CurrencyID = point.SysCurrencyID;
                                inventoryAudit.UomID = itemBom.UomID;
                                inventoryAudit.InvoiceNo = point.Number;
                                inventoryAudit.Trans_Type = douType.Code;
                                inventoryAudit.Process = itemMaster.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @Cost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty;
                                inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * @Cost);
                                inventoryAudit.Trans_Valuse = @Qty * @Cost * -1;
                                inventoryAudit.LocalCurID = point.LocalCurrencyID;
                                inventoryAudit.LocalSetRate = point.LocalSetRate;
                                inventoryAudit.DocumentTypeID = douType.ID;
                                inventoryAudit.SeriesID = point.SeriesID;
                                inventoryAudit.SeriesDetailID = point.SeriesDID;
                                //
                                inventoryAccAmountavg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                expAccAmountavg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            else
                            {
                                var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == itemBom.ItemID && w.WarehouseID == point.WarehouseID);
                                double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                                @AvgCost = CheckNaNOrInfinity(@AvgCost);
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = point.WarehouseID;
                                inventoryAudit.BranchID = point.BranchID;
                                inventoryAudit.UserID = point.UserID;
                                inventoryAudit.ItemID = itemBom.ItemID;
                                inventoryAudit.CurrencyID = point.SysCurrencyID;
                                inventoryAudit.UomID = itemBom.UomID;
                                inventoryAudit.InvoiceNo = point.Number;
                                inventoryAudit.Trans_Type = douType.Code;
                                inventoryAudit.Process = itemMaster.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @AvgCost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @AvgCost);
                                inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                inventoryAudit.LocalCurID = point.LocalCurrencyID;
                                inventoryAudit.LocalSetRate = point.LocalSetRate;
                                inventoryAudit.DocumentTypeID = douType.ID;
                                inventoryAudit.SeriesID = point.SeriesID;
                                inventoryAudit.SeriesDetailID = point.SeriesDID;
                                //
                                var inventoryAcct = _dataContext.InventoryAudits.Where(w => w.ItemID == itemBom.ItemID);
                                double @ACost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                                @ACost = CheckNaNOrInfinity(@ACost);
                                inventoryAccAmountavg = (decimal)@ACost * (decimal)@Qty;
                                expAccAmountavg += (decimal)@ACost * (decimal)@Qty;
                                //
                                UpdateAvgCost(itemBom.ItemID, point.WarehouseID, itemBom.GUoMID, @Qty, @AvgCost);
                                UpdateBomCost(itemBom.ItemID, @Qty, @AvgCost);
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }

                            //inventoryAccID
                            var glAccInvenfifo = _dataContext.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDavg) ?? new GLAccount();
                            if (glAccInvenfifo.ID > 0)
                            {
                                var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                                if (journalDetail.ItemID > 0)
                                {
                                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDavg);
                                    glAccInvenfifo.Balance += inventoryAccAmountavg;
                                    //journalEntryDetail
                                    journalDetail.Debit += inventoryAccAmountavg;
                                    //accountBalance
                                    accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                                    accBalance.Debit += inventoryAccAmountavg;
                                }
                                else
                                {
                                    glAccInvenfifo.Balance += inventoryAccAmountavg;
                                    journalEntryDetail.Add(new JournalEntryDetail
                                    {
                                        JEID = journalEntry.ID,
                                        Type = Type.GLAcct,
                                        ItemID = inventoryAccIDavg,
                                        Debit = inventoryAccAmountavg,
                                    });
                                    //
                                    accountBalance.Add(new AccountBalance
                                    {
                                        PostingDate = point.PostingDate,
                                        Origin = douType.ID,
                                        OriginNo = point.Number,
                                        OffsetAccount = "",
                                        Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                                        CumulativeBalance = glAccInvenfifo.Balance,
                                        Debit = inventoryAccAmountavg,
                                        LocalSetRate = (decimal)point.LocalSetRate,
                                        GLAID = inventoryAccIDavg,
                                    });
                                }
                                _dataContext.GLAccounts.Update(glAccInvenfifo);
                            }
                            // COGS
                            var glAccCOGSfifo = _dataContext.GLAccounts.FirstOrDefault(w => w.ID == expAccIDavg) ?? new GLAccount();
                            if (glAccCOGSfifo.ID > 0)
                            {
                                var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                                if (journalDetail.ItemID > 0)
                                {
                                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == expAccIDavg);
                                    glAccCOGSfifo.Balance -= expAccAmountavg;
                                    //journalEntryDetail
                                    journalDetail.Credit += expAccAmountavg;
                                    //accountBalance
                                    accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                                    accBalance.Credit += expAccAmountavg;
                                }
                                else
                                {
                                    glAccCOGSfifo.Balance -= expAccAmountavg;
                                    journalEntryDetail.Add(new JournalEntryDetail
                                    {
                                        JEID = journalEntry.ID,
                                        Type = Type.GLAcct,
                                        ItemID = expAccIDavg,
                                        Credit = expAccAmountavg,
                                    });
                                    //
                                    accountBalance.Add(new AccountBalance
                                    {
                                        PostingDate = point.PostingDate,
                                        Origin = douType.ID,
                                        OriginNo = point.Number,
                                        OffsetAccount = "",
                                        Details = douTypeID.Name + "-" + glAccCOGSfifo.Code,
                                        CumulativeBalance = glAccCOGSfifo.Balance,
                                        Credit = expAccAmountavg,
                                        LocalSetRate = (decimal)point.LocalSetRate,
                                        GLAID = expAccIDavg,
                                    });
                                }
                                _dataContext.GLAccounts.Update(glAccCOGSfifo);
                            }
                            _dataContext.InventoryAudits.Update(inventoryAudit);
                            _dataContext.WarehouseSummary.Update(warehouseSummary);
                            _dataContext.ItemMasterDatas.Update(itemMaster);
                            _dataContext.WarehouseDetails.Update(warehouseDetail);
                            _dataContext.SaveChanges();
                        }
                    }
                }
            }
            member.RedeemedPoint += (decimal)point.PointRedemptions.Sum(s => s.PointQty * s.Factor);
            member.OutstandPoint -= (decimal)point.PointRedemptions.Sum(s => s.PointQty * s.Factor);
            _dataContext.BusinessPartners.Update(member);
            await _dataContext.SaveChangesAsync();
            return point ?? new PointRedemptionMaster();
        }

        private void FinancailRedemptionPoint(
            int expenAccID, int inventoryAccID, decimal expenAccAmount, decimal inventoryAccAmount,
            List<JournalEntryDetail> journalEntryDetail, List<AccountBalance> accountBalance,
            JournalEntry journalEntry, SeriesDetail seriesDetail, DocumentType douType, DocumentType douTypeID,
            GLAccount glAcc)
        {
            //expense Acc
            var glAccExpen = _dataContext.GLAccounts.FirstOrDefault(w => w.ID == expenAccID);
            var journalDetailExpen = journalEntryDetail.FirstOrDefault(w => w.ItemID == expenAccID) ?? new JournalEntryDetail();
            if (journalDetailExpen.ItemID > 0)
            {
                glAccExpen.Balance += expenAccAmount;
                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == expenAccID);
                //journalEntryDetail
                journalDetailExpen.Credit += expenAccAmount;
                //accountBalance
                accBalance.CumulativeBalance = glAccExpen.Balance;
                accBalance.Debit += expenAccAmount;
            }
            else
            {
                glAccExpen.Balance -= expenAccAmount;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = CKBS.Models.Services.Financials.Type.BPCode,
                    ItemID = expenAccID,
                    Debit = expenAccAmount,
                });
                accountBalance.Add(new AccountBalance
                {
                    PostingDate = DateTime.Today,
                    Origin = douType.ID,
                    OriginNo = seriesDetail.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + " - " + glAccExpen.Code,
                    CumulativeBalance = glAccExpen.Balance,
                    Debit = expenAccAmount,
                    //LocalSetRate = (decimal)lc.SetRate,
                    GLAID = expenAccID,
                });
            }
            //inventoryAccID
            var glAccInven = _dataContext.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID);
            var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == inventoryAccID) ?? new JournalEntryDetail();
            if (journalDetail.ItemID > 0)
            {
                glAccInven.Balance -= inventoryAccAmount;
                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                //journalEntryDetail
                journalDetail.Credit += inventoryAccAmount;
                //accountBalance
                accBalance.CumulativeBalance = glAccInven.Balance;
                accBalance.Credit += inventoryAccAmount;
            }
            else
            {
                glAccInven.Balance -= inventoryAccAmount;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = CKBS.Models.Services.Financials.Type.BPCode,
                    ItemID = inventoryAccID,
                    Credit = inventoryAccAmount,
                });
                accountBalance.Add(new AccountBalance
                {
                    PostingDate = DateTime.Today,
                    Origin = douType.ID,
                    OriginNo = seriesDetail.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + " - " + glAccInven.Code,
                    CumulativeBalance = glAccInven.Balance,
                    Credit = inventoryAccAmount,
                    //LocalSetRate = (decimal)lc.SetRate,
                    GLAID = inventoryAccID,
                });
            }
        }
        private void CumulativeValue(int whid, int itemid, double value, ItemAccounting itemAcc)
        {
            value = CheckNaNOrInfinity(value);
            var wherehouse = _dataContext.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == whid && w.ItemID == itemid) ?? new WarehouseSummary();
            wherehouse.CumulativeValue = (decimal)value;
            _dataContext.WarehouseSummary.Update(wherehouse);
            if (itemAcc != null) itemAcc.CumulativeValue = wherehouse.CumulativeValue;
            _dataContext.SaveChanges();
        }
        //update_bomCost
        public void UpdateBomCost(int itemid, double qty, double avgcost)
        {
            avgcost = CheckNaNOrInfinity(avgcost);
            var ItemBOMDetail = _dataContext.BOMDetail.Where(w => w.ItemID == itemid);
            foreach (var itembom in ItemBOMDetail)
            {
                var Inven = _dataContext.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                double @AvgCost = (Inven.Sum(s => s.Trans_Valuse) + avgcost) / (Inven.Sum(q => q.Qty) + qty);
                @AvgCost = CheckNaNOrInfinity(@AvgCost);
                var Gdoum = _dataContext.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                itembom.Cost = @AvgCost;
                itembom.Amount = itembom.Qty * @AvgCost;
                _dataContext.BOMDetail.UpdateRange(itembom);
                _dataContext.SaveChanges();
                // sum 
                var BOM = _dataContext.BOMaterial.FirstOrDefault(w => w.BID == itembom.BID);
                var DBOM = _dataContext.BOMDetail.Where(w => w.BID == BOM.BID && !w.Detele);
                BOM.TotalCost = DBOM.Sum(s => s.Amount);
                _dataContext.BOMaterial.Update(BOM);
                _dataContext.SaveChanges();
            }
        }
        public void UpdateAvgCost(int itemid, int whid, int guomid, double qty, double avgcost)
        {
            // update pricelistdetial
            var inventory_audit = _dataContext.InventoryAudits.Where(w => w.ItemID == itemid);
            var pri_detial = _dataContext.PriceListDetails.Where(w => w.ItemID == itemid);
            double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_audit.Sum(q => q.Qty) + qty);
            @AvgCost = CheckNaNOrInfinity(@AvgCost);
            foreach (var pri in pri_detial)
            {
                var guom = _dataContext.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == guomid && g.AltUOM == pri.UomID);
                var exp = _dataContext.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                pri.Cost = @AvgCost * exp.SetRate * guom.Factor;
                _dataContext.PriceListDetails.Update(pri);
            }
            //update_waresummary
            var inventory_warehouse = _dataContext.InventoryAudits.Where(w => w.ItemID == itemid && w.WarehouseID == whid);
            var waresummary = _dataContext.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemid && w.WarehouseID == whid);
            double @AvgCostWare = (inventory_warehouse.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_warehouse.Sum(s => s.Qty) + qty);
            waresummary.Cost = @AvgCostWare;

            _dataContext.WarehouseSummary.Update(waresummary);
            _dataContext.SaveChanges();
        }
        //add new Get Invoice

        private static bool IsBetweenDate(DateTime dateFrom, DateTime dateTo)
        {
            DateTime today = DateTime.Today;
            var _dateFrom = DateTime.Compare(today, dateFrom);
            var _dateTo = DateTime.Compare(today, dateTo);

            bool isValid = _dateFrom >= 0 && _dateTo <= 0;
            return isValid;
        }

        private static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }
        /// <summary>
        /// Combo Sale Block
        /// </summary>
        public async Task<List<ComboSaleViewModel>> GetComboSaleAsync(int plId)
        {
            var data = await (from cs in _dataContext.SaleCombos.Where(i => i.Active && i.PriListID == plId)
                              join item in _dataContext.ItemMasterDatas on cs.ItemID equals item.ID
                              join uom in _dataContext.UnitofMeasures on cs.UomID equals uom.ID
                              let pld = _dataContext.PriceListDetails.FirstOrDefault(i => i.ItemID == item.ID && i.UomID == uom.ID) ?? new PriceListDetail()
                              let cur = _dataContext.Currency.FirstOrDefault(i => i.ID == pld.CurrencyID) ?? new Currency()
                              select new ComboSaleViewModel
                              {
                                  ItemCode = item.Code,
                                  ItemName = item.KhmerName,
                                  LineID = $"{DateTime.Now.Ticks}{cs.ID}".Substring(11),
                                  Name = cs.Barcode,
                                  Type = cs.Type == SaleType.SaleChild ? "Sale Child" : "Sale Parent",
                                  UoM = uom.Name,
                                  ID = cs.ID,
                                  ItemID = item.ID,
                                  TaxGroupID = item.TaxGroupSaleID,
                                  UomID = uom.ID,
                                  Qty = 1,
                                  QtyF = $"{string.Format("{0:#,0.00}", 1)} {uom.Name}",
                                  UnitPrice = (decimal)pld.UnitPrice,
                                  UnitPriceF = $"{string.Format("{0:#,0.00}", pld.UnitPrice)} {cur.Description}",
                                  TypeEnum = cs.Type,
                                  ComboSaleDetials = (from csd in _dataContext.SaleComboDetails.Where(i => i.SaleComboID == cs.ID)
                                                      join itemd in _dataContext.ItemMasterDatas on csd.ItemID equals itemd.ID
                                                      join uomd in _dataContext.UnitofMeasures on csd.UomID equals uomd.ID
                                                      let pldd = _dataContext.PriceListDetails.FirstOrDefault(i => i.ItemID == item.ID && i.UomID == uom.ID) ?? new PriceListDetail()
                                                      select new ComboSaleViewModel
                                                      {
                                                          ID = csd.ID,
                                                          ItemCode = itemd.Code,
                                                          ItemID = itemd.ID,
                                                          ItemName = itemd.KhmerName,
                                                          LineID = $"{DateTime.Now.Ticks}{csd.ID}".Substring(10),
                                                          TaxGroupID = item.TaxGroupSaleID,
                                                          UoM = uomd.Name,
                                                          UomID = uomd.ID,
                                                          Qty = csd.Qty,
                                                          UnitPrice = (decimal)pldd.UnitPrice,
                                                      }).ToList(),
                              }).ToListAsync();
            return data;
        }

        /// <summary>
        /// End Combo Sale Block
        /// </summary>

        /// <summary>
        /// buy x amount get x discount Block
        /// </summary>
        public async Task<List<BuyXAmountGetXDiscountViewModel>> GetbuyxamountgetxdiscountAsync(int plId)
        {
            var now = DateTime.Today;
            var data = await _dataContext.PBuyXAmountGetXDis.Where(i => i.PriListID == plId && i.DateF <= now && i.DateT >= now && i.Active)
                              .Select(i => new BuyXAmountGetXDiscountViewModel
                              {
                                  Active = i.Active,
                                  DateT = i.DateT,
                                  DateF = i.DateF,
                                  PriListID = i.PriListID,
                                  Amount = i.Amount,
                                  DisRateValue = i.DisRateValue,
                                  DisType = i.DisType,
                                  ID = i.ID,
                              }).ToListAsync();
            return data;
        }

        /// <summary>
        /// End buy x amount get x discount Block
        /// </summary>


        /// /// <summary>
        /// buy x qty get x discount Block
        /// </summary>
        public async Task<List<BuyXQtyGetXDisViewModel>> GetbuyxqtygetxdiscountAsync()
        {
            var now = DateTime.Today;
            var data = await _dataContext.BuyXQtyGetXDis.Where(i => i.DateF <= now && i.DateT >= now && i.Active)
                              .Select(i => new BuyXQtyGetXDisViewModel
                              {
                                  ID = i.ID,
                                  Active = i.Active,
                                  BuyItem = "",
                                  BuyItemID = i.BuyItemID,
                                  Code = i.Code,
                                  DateF = i.DateF,
                                  DateT = i.DateT,
                                  DisItem = "",
                                  DisItemID = i.DisItemID,
                                  DisRate = i.DisRate,
                                  LineID = $"{DateTime.Now.Ticks}{i.ID}",
                                  Name = i.Name,
                                  Qty = i.Qty,
                                  UomID = i.UomID,
                              }).ToListAsync();
            return data;
        }

        /// <summary>
        /// End buy x qty get x discount Block
        /// </summary>
        private static double CheckNaNOrInfinity(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                value = 0;
            }
            return value;
        }
        private void UpdateItemAccounting(ItemAccounting itemAcc, WarehouseSummary ws)
        {
            if (itemAcc != null)
            {
                itemAcc.Committed = ws.Committed;
                itemAcc.InStock = ws.InStock;
                itemAcc.Ordered = ws.Ordered;
                itemAcc.Available = ws.Available;
                _dataContext.ItemAccountings.Update(itemAcc);
                _dataContext.SaveChanges();
            }
        }
    }
}


