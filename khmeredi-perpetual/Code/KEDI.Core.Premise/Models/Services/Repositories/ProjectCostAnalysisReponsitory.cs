using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.ProjectCostAnalysis;
using KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IProjectCostAnalysisRepository
    {
        // Task<ModelStateDictionary> CreateOrUpdate(ProjectCostAnalysis objpm, ModelStateDictionary ModelState);
        FreightSaleView GetFreights();
        // Task<ProjectCostAnalysis> SearchAsync(string seriel);
        Task<List<Customer>> GetCustomerAsync();
        IEnumerable<DetailItemMasterData> GetItemDetails(int PLID, int comId, int itemId, string barCode, int uomId);
        Task<List<Employee>> GetSaleEmployeeAsync();
        IEnumerable<GroupDUoM> GetAllGroupDefind();
        Task<List<ProjeccostStory>> GetStoryProjcostAsyce(string seriel);
        Task<List<ProjeccostStory>> GetStorySolutionDataAsyce(int cusid);
        Task<List<ProjeccostStory>> GetHistStorySolutionDataAsyce();
        dynamic FindProjectCostAnalysis(string number, int seriesId, int comId);
        dynamic CopySolutionDataMGAsynce(string number, int seriesId, int comId);
        dynamic FindSolutionDatas(string number, int seriesId, int comId);
        IEnumerable<ProjCostAnDetail> GetItemMaster(int PLID, int comId);
        Task<List<ProjeccostStory>> GetStoryProjcosQuotetAsyce(int id);// add on 22/4/2022
    }
    public class ProjectCostAnalysisReponsitory : IProjectCostAnalysisRepository
    {
        private readonly DataContext _context;

        public ProjectCostAnalysisReponsitory(DataContext context)
        {
            _context = context;

        }
        public IEnumerable<GroupDUoM> GetAllGroupDefind()
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
        private List<TaxGroupViewModel> GetTaxGroups()
        {
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           //GLID = tg.GLID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            return tgs;
        }
        public async Task<List<Customer>> GetCustomerAsync()
        {
            var list = (from cus in _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").OrderBy(s => s.Name)
                        select new Customer
                        {
                            ID = cus.ID,
                            Code = cus.Code,
                            Name = cus.Name,
                            Type = cus.Type,
                            Phone = cus.Phone,
                        }).ToList();

            return await Task.FromResult(list);
        }
        public IEnumerable<DetailItemMasterData> GetItemDetails(int PLID, int comId, int itemId, string barCode, int uomId)
        {
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var _item = new ItemMasterData();
            var _priceListDetails = new List<PriceListDetail>();
            if (uomId == 0)
                _priceListDetails = _context.PriceListDetails.Where(i => i.PriceListID == PLID).ToList();
            else
                _priceListDetails = _context.PriceListDetails.Where(i => i.PriceListID == PLID && i.UomID == uomId).ToList();
            if (itemId != 0)
            {
                _item = _context.ItemMasterDatas.Find(itemId);
            }
            if (barCode != null)
            {
                _item = _context.ItemMasterDatas.FirstOrDefault(i => i.Barcode == barCode) ?? new ItemMasterData();
            }
            var tgs = GetTaxGroups();
            tgs.Insert(0, _tg);
            var uoms = from guom in _context.ItemMasterDatas.Where(i => i.ID == _item.ID)
                       join GDU in _context.GroupDUoMs on guom.GroupUomID equals GDU.GroupUoMID
                       join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                       select new UOMSViewModel
                       {
                           BaseUoMID = GDU.BaseUOM,
                           Factor = GDU.Factor,
                           ID = UNM.ID,
                           Name = UNM.Name
                       };
            var uomPriceLists = (from pld in _context.PriceListDetails.Where(i => i.ItemID == _item.ID && i.PriceListID == PLID)
                                 select new UomPriceList
                                 {
                                     UoMID = (int)pld.UomID,
                                     UnitPrice = (decimal)pld.UnitPrice
                                 }).ToList();

            var data = (from pld in _priceListDetails
                        join prot in _context.Promotions on pld.PromotionID equals prot.ID
                        into g
                        from prot in g.DefaultIfEmpty()
                        join item in _context.ItemMasterDatas.Where(i => i.Sale && !i.Delete && i.CompanyID == comId && i.ID == _item.ID) on pld.ItemID equals item.ID
                        join GDU in _context.GroupDUoMs on item.GroupUomID equals GDU.GroupUoMID
                        join uom in _context.UnitofMeasures on pld.UomID equals uom.ID
                        join cur in _context.Currency on pld.CurrencyID equals cur.ID
                        join ptn in _context.PrinterNames on item.PrintToID equals ptn.ID
                        where uom.ID == item.SaleUomID
                        let tg = _context.TaxGroups.FirstOrDefault(i => !i.Delete && i.Active && item.TaxGroupSaleID == i.ID) ?? new TaxGroup()
                        let ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == pld.CurrencyID)
                        let tgd = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == tg.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                        let taxValue = (tgd.Rate * (decimal)pld.UnitPrice) == 0 ? 0 : ((tgd.Rate * (decimal)pld.UnitPrice)) / 100
                        select new DetailItemMasterData
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            ItemCode = item.Code,
                            BarCode = item.Barcode,
                            Currency = cur.Description,
                            TotalSys = (double)(pld.UnitPrice * ex.Rate),
                            Description = item.KhmerName,
                            ID = 0,
                            ProjCostID = 0,
                            Cost = (decimal)pld.Cost,
                            CurrencyID = cur.ID,
                            DisRate = 0,
                            DisValue = 0,

                            ItemNameEN = item.EnglishName,
                            ItemNameKH = item.KhmerName,
                            GUomID = item.GroupUomID,
                            ItemID = item.ID,
                            ItemType = item.Type,
                            Process = item.Process,
                            Qty = 1,
                            OpenQty = 1,
                            TypeDis = pld.TypeDis,
                            UnitPrice = (decimal)pld.UnitPrice,
                            UnitPriceAfterDis = (decimal)pld.UnitPrice,
                            LineTotalBeforeDis = (decimal)pld.UnitPrice * 1,
                            LineTotalCost = (decimal)pld.Cost,
                            UomName = uom.Name,
                            UomID = uom.ID,
                            TaxGroupID = tg.ID,
                            TaxRate = tgd.Rate,
                            InStock = item.StockIn,
                            Remarks = "",
                            TaxDownPaymentValue = 0,
                            FinDisRate = 0,
                            FinDisValue = 0,
                            TaxOfFinDisValue = 0,
                            FinTotalValue = (decimal)pld.UnitPrice + taxValue,
                            TaxGroupList = tgs.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = $"{c.Code}-{c.Name}",
                                Selected = c.ID == tg.ID
                            }).ToList(),
                            Total = (decimal)pld.UnitPrice,
                            UnitMargin = (decimal)pld.UnitPrice - (decimal)pld.Cost,
                            LineTotalMargin = ((decimal)pld.UnitPrice - (decimal)pld.Cost),
                            TaxValue = taxValue,
                            TotalWTax = (decimal)pld.UnitPrice + taxValue,
                            UoMs = uoms.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = c.Name,
                                Selected = c.ID == pld.UomID
                            }).ToList(),
                            UoMsList = uoms.ToList(),
                            TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                                         let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                         select new TaxGroupViewModel
                                         {
                                             ID = t.ID,
                                             //GLID = tg.GLID,
                                             Name = t.Name,
                                             Code = t.Code,
                                             Effectivefrom = tgds.EffectiveFrom,
                                             Rate = tgds.Rate,
                                             Type = (int)t.Type,
                                         }
                                         ).ToList(),
                            UomPriceLists = uomPriceLists,


                        }).GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            return data;
        }
        public dynamic FindProjectCostAnalysis(string number, int seriesId, int comId)
        {
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var proc = (from pa in _context.ProjectCostAnalyses.Where(x => x.InvoiceNumber == number && x.SeriesID == seriesId && x.CompanyID == comId)
                        join cus in _context.BusinessPartners on pa.CusID equals cus.ID
                        join cont in _context.BusinessPartners on pa.ConTactID equals cont.ID
                        join warehouse in _context.Warehouses on pa.WarehouseID equals warehouse.ID
                        join pricelist in _context.PriceLists on pa.PriceListID equals pricelist.ID
                        join saleem in _context.Employees on pa.SaleEMID equals saleem.ID
                        join owner in _context.Employees on pa.OwnerID equals owner.ID
                        join docType in _context.DocumentTypes on pa.DocTypeID equals docType.ID
                        let fs = _context.FreightProjectCosts.Where(i => i.ProjCAID == pa.ID && i.SaleType == SaleCopyType.ProjectCostAnalysisDetail).FirstOrDefault() ?? new FreightProjectCost()
                        select new ProjCostAnalysisVeiw
                        {
                            ID = pa.ID,
                            Name = pa.Name,
                            CusID = pa.CusID,
                            CusName = cus.Name,
                            CusCode = cus.Code,
                            ConTactID = pa.ConTactID,
                            ContName = cont.Name,
                            Phone = cont.Phone,
                            SaleEMID = pa.SaleEMID,
                            EmName = saleem.Name,
                            OwnerID = pa.OwnerID,
                            OwnerName = owner.Name,
                            BranchID = pa.BranchID,
                            ChangeLog = pa.ChangeLog,
                            CompanyID = pa.CompanyID,
                            DeliveryDate = pa.ValidUntilDate,
                            DisRate = pa.DisRate,
                            DisValue = pa.DisValue.ToString(),
                            DocTypeID = pa.DocTypeID,
                            DocumentDate = pa.DocumentDate,
                            ExchangeRate = pa.ExchangeRate,
                            FreightAmount = pa.FreightAmount,
                            FreightAmountSys = pa.FreightAmountSys,
                            FreightProjectCost = new FreightProjectCost
                            {
                                AmountReven = fs.AmountReven,
                                ProjCAID = pa.ID,
                                ID = fs.ID,
                                SaleType = fs.SaleType,
                                TaxSumValue = fs.TaxSumValue,
                                FreightProjCostDetails = (from fsd in _context.FreightProjCostDetails.Where(i => i.FreightProjectCostID == fs.ID)
                                                          select new FreightProjCostDetail
                                                          {
                                                              ID = fsd.ID,
                                                              LineID = DateTime.Now.Ticks.ToString() + "" + fsd.ID,
                                                              FreightID = fsd.FreightID,
                                                              Amount = fsd.Amount,
                                                              AmountWithTax = fsd.AmountWithTax,
                                                              FreightProjectCostID = fsd.FreightProjectCostID,
                                                              Name = fsd.Name,
                                                              TaxGroup = fsd.TaxGroup,
                                                              TaxGroupID = fsd.TaxGroupID,
                                                              TaxGroups = GetTaxGroups(),
                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                              {
                                                                  Value = i.ID.ToString(),
                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                  Text = $"{i.Code}-{i.Name}"
                                                              }).ToList(),
                                                              TaxRate = fsd.TaxRate,
                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                          }).ToList(),
                            },
                            IncludeVat = pa.IncludeVat,
                            InvoiceNo = $"{docType.Code}-{pa.InvoiceNumber}",
                            InvoiceNumber = pa.InvoiceNumber,
                            LocalCurID = pa.LocalCurID,
                            LocalSetRate = pa.LocalSetRate,
                            PostingDate = pa.PostingDate,
                            PriceListID = pa.PriceListID,
                            RefNo = pa.RefNo,
                            Remarks = pa.Remarks,
                            TotalCommission = pa.TotalCommission,
                            OtherCost = pa.OtherCost,
                            SaleCurrencyID = pa.SaleCurrencyID,
                            SeriesDID = pa.SeriesDID,
                            SeriesID = pa.SeriesID,

                            Status = (KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore.Status)pa.Status,
                            SubTotal = pa.SubTotal,
                            SubTotalAfterDis = pa.SubTotalAfterDis,
                            SubTotalAfterDisSys = pa.SubTotalAfterDisSys,
                            SubTotalBefDis = pa.SubTotalBefDis,
                            SubTotalBefDisSys = pa.SubTotalBefDisSys,
                            SubTotalSys = pa.SubTotalSys,
                            TotalAmount = pa.TotalAmount,
                            TotalAmountSys = pa.TotalAmountSys,
                            TotalMargin = pa.TotalMargin,
                            TypeDis = pa.TypeDis,
                            UserID = pa.UserID,
                            ValidUntilDate = pa.ValidUntilDate,
                            VatRate = pa.VatRate,
                            VatValue = pa.VatValue,
                            WarehouseID = pa.WarehouseID
                        }).ToList();
            var doctype = new DocumentType();
            Currency _cur = new();
            if (proc.Count > 0)
            {
                _cur = _context.Currency.Find(proc.FirstOrDefault().SaleCurrencyID) ?? new Currency();
            }
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            var _pad = (from pa in proc
                        join pad in _context.ProjCostAnalysisDetails on pa.ID equals pad.ProjectCostAnalysisID
                        join item in _context.ItemMasterDatas on pad.ItemID equals item.ID
                        join cur in _context.Currency on pa.SaleCurrencyID equals cur.ID
                        select new DetailItemMasterData
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            ID = pad.ID,
                            ProjCostID = pad.ProjectCostAnalysisID,
                            ItemID = pad.ItemID,
                            ItemCode = pad.ItemCode,
                            ItemType = pad.ItemType,
                            TaxGroupID = pad.TaxGroupID,
                            TaxRate = pad.TaxRate,
                            TaxValue = pad.TaxValue,
                            TaxOfFinDisValue = pad.TaxOfFinDisValue,
                            FinTotalValue = pad.FinTotalValue,
                            ItemNameEN = item.EnglishName,
                            ItemNameKH = item.KhmerName,
                            Description = item.KhmerName,
                            Qty = (decimal)pad.Qty,
                            OpenQty = (decimal)pad.OpenQty,
                            GUomID = pad.GUomID,
                            UomID = pad.UomID,
                            UomName = pad.UomName,
                            Factor = (decimal)pad.Factor,
                            Cost = (decimal)pad.Cost,
                            UnitPrice = (decimal)pad.UnitPrice,
                            DisRate = (decimal)pad.DisRate,
                            DisValue = (decimal)pad.DisValue,
                            UnitPriceAfterDis = pad.UnitPriceAfterDis,
                            Total = (decimal)pad.Total,
                            LineTotalBeforeDis = pad.LineTotalBeforeDis,
                            LineTotalCost = pad.LineTotalCost,
                            FinDisRate = pad.FinDisRate,
                            FinDisValue = pad.FinDisValue,
                            UnitMargin = pad.UnitMargin,
                            TotalWTax = (decimal)pad.TotalWTax,
                            LineTotalMargin = pad.LineTotalMargin,
                            InStock = pad.InStock,
                            Remarks = pad.Remarks,
                            BarCode = item.Barcode,
                            Currency = cur.Description,
                            TotalSys = pad.TotalSys,
                            //TotalSys = pad.UnitPrice *(double)pa.ExchangeRate,

                            CurrencyID = cur.ID,
                            // ItemType = item.Type,
                            Process = pad.Process,
                            BaseUoMID = 0,
                            InvenUoMID = 0,
                            VatValue = pad.VatValue,
                            VatRate = pad.VatRate,

                            TypeDis = pad.TypeDis,
                            ExpireDate = pad.ExpireDate,
                            TaxDownPaymentValue = 0M,
                            TaxGroupList = tgs.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = $"{c.Code}-{c.Name}",
                                Selected = c.ID == pad.TaxGroupID
                            }).ToList(),


                            /// select List UoM ///
                            UoMs = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                    select new UOMSViewModel
                                    {
                                        BaseUoMID = GDU.BaseUOM,
                                        Factor = GDU.Factor,
                                        ID = UNM.ID,
                                        Name = UNM.Name
                                    }).Select(c => new SelectListItem
                                    {
                                        Value = c.ID.ToString(),
                                        Text = c.Name,
                                        Selected = c.ID == pad.UomID
                                    }).ToList(),
                            /// List UoM ///
                            UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                        select new UOMSViewModel
                                        {
                                            BaseUoMID = GDU.BaseUOM,
                                            Factor = GDU.Factor,
                                            ID = UNM.ID,
                                            Name = UNM.Name
                                        }).ToList(),
                            TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                                         let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                         select new TaxGroupViewModel
                                         {
                                             ID = t.ID,
                                             Name = t.Name,
                                             Code = t.Code,
                                             Effectivefrom = tgds.EffectiveFrom,
                                             Rate = tgds.Rate,
                                             Type = (int)t.Type,
                                         }
                                         ).ToList(),
                            UomPriceLists = (from pld in _context.PriceListDetails.Where(i => i.ItemID == item.ID && i.PriceListID == pa.PriceListID)
                                             select new UomPriceList
                                             {
                                                 UoMID = (int)pld.UomID,
                                                 UnitPrice = (decimal)pld.UnitPrice
                                             }).ToList(),

                        }).ToList();

            var data = new ProjCostAnalysisupdate
            {
                ProjectCostAnalysis = proc.FirstOrDefault(),
                DetailItemMasterDatas = _pad,
                Currency = _cur
            };
            return data;
        }



        //public IEnumerable<DetailItemMasterData> GetItemDetails(int PLID, int comId, int itemId, string barCode, int uomId)
        //{
        //    var _tg = new TaxGroupViewModel
        //    {
        //        ID = 0,
        //        GLID = tg.GLID,
        //        Name = "- Select --",
        //        Code = "",
        //        Rate = 0,
        //        Type = 0,
        //    };
        //    var _item = new ItemMasterData();
        //    var _priceListDetails = new List<PriceListDetail>();
        //    if (uomId == 0)
        //        _priceListDetails = _context.PriceListDetails.Where(i => i.PriceListID == PLID).ToList();
        //    else
        //        _priceListDetails = _context.PriceListDetails.Where(i => i.PriceListID == PLID && i.UomID == uomId).ToList();
        //    if (itemId != 0)
        //    {
        //        _item = _context.ItemMasterDatas.Find(itemId);
        //    }
        //    if (barCode != null)
        //    {
        //        _item = _context.ItemMasterDatas.FirstOrDefault(i => i.Barcode == barCode) ?? new ItemMasterData();
        //    }
        //    var tgs = GetTaxGroups();
        //    tgs.Insert(0, _tg);
        //    var uoms = from guom in _context.ItemMasterDatas.Where(i => i.ID == _item.ID)
        //               join GDU in _context.GroupDUoMs on guom.GroupUomID equals GDU.GroupUoMID
        //               join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
        //               select new UOMSViewModel
        //               {
        //                   BaseUoMID = GDU.BaseUOM,
        //                   Factor = GDU.Factor,
        //                   ID = UNM.ID,
        //                   Name = UNM.Name
        //               };
        //    var uomPriceLists = (from pld in _context.PriceListDetails.Where(i => i.ItemID == _item.ID && i.PriceListID == PLID)
        //                         select new UomPriceList
        //                         {
        //                             UoMID = (int)pld.UomID,
        //                             UnitPrice = (decimal)pld.UnitPrice
        //                         }).ToList();

        //    var data = (from pld in _priceListDetails
        //                join prot in _context.Promotions on pld.PromotionID equals prot.ID
        //                into g
        //                from prot in g.DefaultIfEmpty()
        //                join item in _context.ItemMasterDatas.Where(i => i.Sale && !i.Delete && i.CompanyID == comId && i.ID == _item.ID) on pld.ItemID equals item.ID
        //                join GDU in _context.GroupDUoMs on item.GroupUomID equals GDU.GroupUoMID
        //                join uom in _context.UnitofMeasures on pld.UomID equals uom.ID
        //                join cur in _context.Currency on pld.CurrencyID equals cur.ID
        //                join ptn in _context.PrinterNames on item.PrintToID equals ptn.ID
        //                where GDU.BaseUOM == item.SaleUomID
        //                let tg = _context.TaxGroups.FirstOrDefault(i => !i.Delete && i.Active && item.TaxGroupSaleID == i.ID) ?? new TaxGroup()
        //                let ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == pld.CurrencyID)
        //                let tgd = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == tg.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
        //                let taxValue = (tgd.Rate * (decimal)pld.UnitPrice) == 0 ? 0 : (tgd.Rate * (decimal)pld.UnitPrice) / 100
        //                select new DetailItemMasterData
        //                {
        //                    ID = 0,
        //                    LineID = DateTime.Now.Ticks.ToString(),
        //                    ItemMaterDataID = item.ID,
        //                    ItemCode = item.Code,
        //                    BarCode = item.Barcode,
        //                    Description = item.KhmerName,
        //                    Qty = 1,
        //                    Currency = cur.Description,
        //                    TotalSys = (decimal)(pld.UnitPrice * ex.Rate),
        //                    paDID = 0,
        //                    paID = 0,

        //                    CurrencyID = cur.ID,
        //                    DisRate = 0,
        //                    DisValue = 0,
        //                    ItemNameEN = item.EnglishName,
        //                    GUomID = item.GroupUomID,
        //                    ItemID = item.ID,
        //                    ItemType = item.Type,
        //                    Process = item.Process,

        //                    OpenQty = 1,
        //                    TypeDis = pld.TypeDis,
        //                    Cost = (decimal)pld.Cost,
        //                    UnitPrice = (decimal)pld.UnitPrice,
        //                    UnitPriceAfterDis = (decimal)pld.UnitPrice,
        //                    LineTotalBeforeDis = (decimal)pld.UnitPrice * 1,
        //                    LineTotalCost = (decimal)pld.Cost,
        //                    UomName = uom.Name,
        //                    UomID = uom.ID,
        //                    TaxGroupID = tg.ID,
        //                    TaxRate = tgd.Rate,
        //                    Remarks = "",
        //                    TaxDownPaymentValue = 0,
        //                    TaxGroupList = tgs.Select(c => new SelectListItem
        //                    {
        //                        Value = c.ID.ToString(),
        //                        Text = $"{c.Code}-{c.Name}",
        //                        Selected = c.ID == tg.ID
        //                    }).ToList(),
        //                    Total = (decimal)pld.UnitPrice,
        //                    UnitMargin = (decimal)pld.UnitPrice - (decimal)pld.Cost,
        //                    TaxValue = taxValue,
        //                    TotalWTax = (decimal)pld.UnitPrice + taxValue,
        //                    LineTotalMargin = 1 * ((decimal)pld.UnitPrice - (decimal)pld.Cost),
        //                    UoMs = uoms.Select(c => new SelectListItem
        //                    {
        //                        Value = c.ID.ToString(),
        //                        Text = c.Name,
        //                        Selected = c.ID == pld.UomID
        //                    }).ToList(),
        //                    UoMsList = uoms.ToList(),
        //                    TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
        //                                 let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
        //                                 select new TaxGroupViewModel
        //                                 {
        //                                     ID = t.ID,
        //                                     GLID = tg.GLID,
        //                                     Name = t.Name,
        //                                     Code = t.Code,
        //                                     Effectivefrom = tgds.EffectiveFrom,
        //                                     Rate = tgds.Rate,
        //                                     Type = (int)t.Type,
        //                                 }).ToList(),
        //                    UomPriceLists = uomPriceLists,
        //                    FinDisRate = 0,
        //                    FinDisValue = 0,
        //                    TaxOfFinDisValue = 0,
        //                    TaxSumFinRate = 0,
        //                    TaxSumFinRateValue = 0,
        //                    FinTotalValue = (decimal)pld.UnitPrice + taxValue,
        //                }).GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
        //    return data;
        //}

        public async Task<List<Employee>> GetSaleEmployeeAsync()
        {
            var list = (from em in _context.Employees
                        select new Employee
                        {
                            ID = em.ID,
                            Code = em.Code,
                            Name = em.Name,
                            Type = em.EMType,
                            Phone = em.Phone,
                        }).OrderBy(s => s.Name).ToList();
            return await Task.FromResult(list);
        }

        public IEnumerable<ProjCostAnDetail> GetItemMaster(int PLID, int comId)
        {
            var data = (from pld in _context.PriceListDetails.Where(i => i.PriceListID == PLID)
                        join item in _context.ItemMasterDatas.Where(i => i.Sale && !i.Delete && i.CompanyID == comId) on pld.ItemID equals item.ID
                        join uom in _context.UnitofMeasures on pld.UomID equals uom.ID
                        join cur in _context.Currency on pld.CurrencyID equals cur.ID
                        where uom.ID == item.SaleUomID
                        let ws = _context.WarehouseSummary.Where(i => i.ItemID == pld.ItemID).ToList()
                        select new ProjCostAnDetail
                        {
                            ID = pld.ID,
                            Barcode = item.Barcode,
                            Code = item.Code,
                            Cost = pld.Cost,
                            Currency = cur.Description,
                            CurrencyID = cur.ID,
                            Description = item.Description,
                            EnglishName = item.EnglishName,
                            Image = item.Image,
                            ItemID = item.ID,
                            ItemType = item.Type,
                            KhmerName = item.KhmerName,
                            InStock = ws.Sum(s => s.InStock),
                            PricListID = pld.PriceListID,
                            UnitPrice = pld.UnitPrice,
                            UoM = uom.Name,
                            UomID = uom.ID,
                            Process = item.Process,

                        }).GroupBy(i => i.ItemID).Select(i => i.FirstOrDefault()).ToList();
            return data;
        }


        //public async Task<ModelStateDictionary> CreateOrUpdate(ProjectCostAnalysis objpm, ModelStateDictionary ModelState)
        //{

        //        if(string.IsNullOrWhiteSpace(objpm.Name))
        //            ModelState.AddModelError("Name", "Please Input Name...!");
        //        if(objpm.CusID==0)
        //            ModelState.AddModelError("CusID", "Please Input Customer Name...!");
        //        if (objpm.CusContactID == 0)
        //            ModelState.AddModelError("CusContactID", "Please Input Contact Person ...!");
        //        if (objpm.WarehouseID == 0)
        //            ModelState.AddModelError("WarehouseID", "Please Select Warehouse...!");

        //        if (objpm.PriceListID == 0)
        //            ModelState.AddModelError("PriceListID", "Please Select PriceList...!");
        //        if (string.IsNullOrWhiteSpace(objpm.UserName))
        //            ModelState.AddModelError("UserName", "Please Input User Name...!");
        //        if (objpm.PostingDate.Year==1)
        //            ModelState.AddModelError("PostingDate", "Please Input PostingDate...!");
        //        if (objpm.PostingDate.Year< objpm.ValidUntilDate.Year)
        //            ModelState.AddModelError("PostingDate", "PostingDate must bigger than ValidUntilDate or equal ValidUntilDate...!");
        //        if (objpm.ValidUntilDate.Year==1)
        //            ModelState.AddModelError("ValidUntilDate", "please Input ValidUntilDate...!");
        //        if (objpm.ValidUntilDate.Year <objpm.Documentdate.Year)
        //            ModelState.AddModelError("ValidUntilDate", "ValidUntilDate must bigger than Documentdate or equal ValidUntilDate...!");
        //        if (objpm.Documentdate.Year == 1)
        //            ModelState.AddModelError("Documentate", "Please Input Documentdate...!");
        //        if (objpm.SaleEmID == 0)
        //            ModelState.AddModelError("SaleEmID", "Please Input Sale Emaployee...!");
        //        if (objpm.OwnerID == 0)
        //            ModelState.AddModelError("OwnerID", "Please Input Owner...!");
        //        if(objpm.ProjCostAnalysisDetails !=null)
        //        {
        //            objpm.ProjCostAnalysisDetails = objpm.ProjCostAnalysisDetails.Where(s => !string.IsNullOrWhiteSpace(s.Barcode) && !string.IsNullOrWhiteSpace(s.Description)).ToList();
        //            if (objpm.ProjCostAnalysisDetails.Count() > 0)
        //            {
        //                foreach (var list in objpm.ProjCostAnalysisDetails)
        //                {
        //                    if (list.Qty <= 0)
        //                        ModelState.AddModelError("Quantity", "Please Input Quantity...!");
        //                    if (list.Cost <= 0)
        //                        ModelState.AddModelError("Cost", "Please Input Cost...!");
        //                    if (list.TaxGroupID <= 0)
        //                        ModelState.AddModelError("TaxID", "Please select Tax...!");
        //                }
        //            }
        //        }
        //      if(objpm.FreightProjCostDetails!=null|| objpm.FreightProjCostDetails.Count()>0)
        //    {

        //        if(objpm.FreightProjCostDetails[0].Amount == 0)
        //        {
        //            objpm.FreightProjCostDetails = null;
        //        }

        //    }



        //    Series series = _context.Series.Find(objpm.SeriesID) ?? new Series();
        //    SeriesDetail seriesDetail = new SeriesDetail();
        //    if (ModelState.IsValid)
        //    {
        //        seriesDetail.SeriesID = objpm.SeriesID;
        //        seriesDetail.Number = series.NextNo;
        //        _context.SeriesDetails.Update(seriesDetail);

        //        await _context.SaveChangesAsync();
        //        string Sno = seriesDetail.Number;
        //        long No = long.Parse(Sno);
        //        series.NextNo = Convert.ToString(No + 1);
        //        objpm.SeriesDID = seriesDetail.ID;

        //        _context.UpdateRange(objpm);
        //        await _context.SaveChangesAsync();


        //    }
        //    return await Task.FromResult(ModelState);
        //}


        //public async Task<ProjectCostAnalysis> SearchAsync(string seriel)
        //{

        //    var _tg = new TaxGroupViewModel
        //    {
        //        ID = 0,
        //        //GLID = tg.GLID,
        //        Name = "- Select --",
        //        Code = "",
        //        Rate = 0,
        //        Type = 0,
        //    };

        //    var tgs = GetTaxGroups();
        //    tgs.Insert(0, _tg);
        //    var taxGroup = GetTaxGroups();
        //    taxGroup.Insert(0, new TaxGroupViewModel
        //    {
        //        ID = 0,
        //        Name = "- Select --",
        //        Code = "",
        //        Rate = 0,
        //        Type = 0,
        //    });

        //    var projcosts = (from procost in _context.ProjectCostAnalyses.Where(s => s.SeriesNo == seriel)
        //                    join cus in _context.BusinessPartners on procost.CusID equals cus.ID
        //                    join cont in _context.BusinessPartners on procost.CusContactID equals cont.ID
        //                    join warehouse in _context.Warehouses on procost.WarehouseID equals warehouse.ID
        //                    join pricelist in _context.PriceLists on procost.PriceListID equals pricelist.ID
        //                    join saleem in _context.Employees on procost.SaleEmID equals saleem.ID
        //                    join owner in _context.Employees on procost.OwnerID equals owner.ID

        //                    select new ProjectCostAnalysis
        //                    {
        //                        #region
        //                        ID = procost.ID,
        //                        Name = procost.Name,
        //                        CusID = procost.CusID,
        //                        CusName = cus.Name,
        //                        CusCode=cus.Code,
        //                        Status = procost.Status,
        //                        CusContactID = procost.CusContactID,
        //                        ContName = cont.Name,
        //                        Phone = cont.Phone,
        //                        WarehouseID = procost.WarehouseID,
        //                        Warehouse = warehouse.Name,
        //                        CustomerRef = procost.CustomerRef,
        //                        PriceListID = procost.PriceListID,
        //                        PriceList = pricelist.Name,
        //                        UserName = procost.UserName,
        //                        SeriesID = procost.SeriesID,
        //                        SeriesNo = procost.SeriesNo,
        //                        SeriesDID = procost.SeriesDID,
        //                        PostingDate = procost.PostingDate,
        //                        ValidUntilDate = procost.ValidUntilDate,
        //                        Documentdate = procost.Documentdate,
        //                        Barcodereadign = procost.Barcodereadign,
        //                        SaleEmID = procost.SaleEmID,
        //                        SaleEmName = saleem.Name,
        //                        OwnerID = procost.OwnerID,
        //                        OwnerName = owner.Name,
        //                        ExchangeRate = procost.ExchangeRate,
        //                        Remarks = procost.Remarks,
        //                        SubTotalBeforeDis = procost.SubTotalBeforeDis,
        //                        Discount = procost.Discount,
        //                        SubTotalAfterDis = procost.SubTotalAfterDis,

        //                        FreightAmount = procost.FreightAmount,
        //                        Tax = procost.Tax,
        //                        TotalAmount = procost.TotalAmount,
        //                        TotalMargin = procost.TotalMargin,
        //                        TotalCommission = procost.TotalCommission,
        //                        OtherCost = procost.OtherCost,
        //                        ExpectedTotalProfit = procost.ExpectedTotalProfit,
        //                        #endregion
        //                        ProjCostAnalysisDetails = (from projde in _context.ProjCostAnalysisDetails.Where(s => s.ProjectCostAnalysisID == procost.ID)
        //                                                join item in _context.ItemMasterDatas on projde.ItemMaterDataID equals item.ID

        //                                               select new ProjCostAnalysisDetail
        //                                               {
        //                                                   ID=projde.ID,
        //                                                   ProjectCostAnalysisID=projde.ProjectCostAnalysisID,
        //                                                   ItemMaterDataID = projde.ItemMaterDataID,
        //                                                   ItemCode = projde.ItemCode,
        //                                                   Barcode = projde.Barcode,
        //                                                   Description = projde.Description,
        //                                                   Qty = projde.Qty,
        //                                                   UomID = projde.UomID,
        //                                                   UoMs = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
        //                                                           join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
        //                                                           select new UOMSViewModel
        //                                                           {
        //                                                               BaseUoMID = GDU.BaseUOM,
        //                                                               Factor = GDU.Factor,
        //                                                               ID = UNM.ID,
        //                                                               Name = UNM.Name
        //                                                           }).Select(c => new SelectListItem
        //                                                           {
        //                                                               Value = c.ID.ToString(),
        //                                                               Text = c.Name,
        //                                                               Selected = c.ID == projde.UomID
        //                                                           }).ToList(),

        //                                                   Currency = projde.Currency,
        //                                                   Cost = projde.Cost,
        //                                                   UnitPrice = projde.UnitPrice,
        //                                                   UnitPriceAfterDis= projde.UnitPriceAfterDis,
        //                                                   LineTotalBeforeDis = projde.LineTotalBeforeDis,
        //                                                   LineTotalCost=projde.LineTotalCost,
        //                                                   TaxGroupID = projde.TaxGroupID,
        //                                                   TaxGroupList = tgs.Select(c => new SelectListItem
        //                                                   {
        //                                                       Value = c.ID.ToString(),
        //                                                       Text = $"{c.Code}-{c.Name}",
        //                                                       Selected = c.ID == projde.TaxGroupID
        //                                                   }).ToList(),

        //                                                   TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
        //                                                                let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
        //                                                                select new TaxGroupViewModel
        //                                                                {
        //                                                                    ID = t.ID,
        //                                                                    //GLID = tg.GLID,
        //                                                                    Name = t.Name,
        //                                                                    Code = t.Code,
        //                                                                    Effectivefrom = tgds.EffectiveFrom,
        //                                                                    Rate = tgds.Rate,
        //                                                                    Type = (int)t.Type,
        //                                                                }).ToList(),
        //                                                   TaxRate = projde.TaxRate,
        //                                                   TaxValue = projde.TaxValue,
        //                                                   TaxOfFinDisValue = projde.TaxOfFinDisValue,
        //                                                   DisRate = projde.DisRate,
        //                                                   DisValue = projde.DisValue,
        //                                                   FinDisRate = projde.FinDisRate,
        //                                                   FinDisValue = projde.FinDisValue,
        //                                                   Total = projde.Total,
        //                                                   UnitMargin = projde.UnitMargin,
        //                                                   TotalWTax = projde.TotalWTax,
        //                                                   LineTotalMargin = projde.LineTotalMargin,
        //                                                   InStock = projde.InStock,
        //                                                   FinTotalValue = projde.FinTotalValue,
        //                                                   Remarks = projde.Remarks,
        //                                                   LineID = DateTime.Now.Ticks.ToString() + "" + projde.ID

        //                                               }).ToList(),
        //                        #region
        //                        FreightProjCostDetails = (from fsd in _context.FreightProjCostDetails.Where(s => s.ProjectCostAnalysisID == procost.ID)
        //                                                  select new FreightProjCostDetail
        //                                                  {
        //                                                      ID = fsd.ID,
        //                                                      FreightID= fsd.ID,
        //                                                      ProjectCostAnalysisID = fsd.ProjectCostAnalysisID,
        //                                                      Amount = fsd.Amount,
        //                                                      AmountWithTax = fsd.AmountWithTax,
        //                                                      Name = fsd.Name,
        //                                                      TaxGroup = fsd.TaxGroup,
        //                                                      TaxGroupID = fsd.TaxGroupID,
        //                                                      TaxGroups = GetTaxGroups(),
        //                                                      TaxGroupSelect = taxGroup.Select(i => new SelectListItem
        //                                                      {
        //                                                          Value = i.ID.ToString(),
        //                                                          Selected = i.ID == fsd.TaxGroupID,
        //                                                          Text = $"{i.Code}-{i.Name}"
        //                                                      }).ToList(),
        //                                                      TaxRate = fsd.TaxRate,
        //                                                      TotalTaxAmount = fsd.TotalTaxAmount
        //                                                  }).ToList(),

        //                    }).FirstOrDefault();
        //    #endregion

        //    return await Task.FromResult(projcosts); 

        //}

        public async Task<List<ProjeccostStory>> GetStoryProjcostAsyce(string seriel)
        {
            var list = (from projc in _context.ProjectCostAnalyses.OrderBy(s => s.Name)
                        select new ProjeccostStory
                        {
                            ID = projc.ID,
                            Name = projc.Name,
                            SeriesID = projc.SeriesID,
                            InvoiceNumber = projc.InvoiceNumber,
                            CompanyID = projc.CompanyID,
                            PostingDate = projc.PostingDate.ToString("dd/MM/yyyy"),
                            ValidUntilDate = projc.ValidUntilDate.ToString("dd/MM/yyyy"),
                            DocumentDate = projc.DocumentDate.ToString("dd/MM/yyyy"),
                        }).ToList();
            return await Task.FromResult(list);
        }
        public async Task<List<ProjeccostStory>> GetStorySolutionDataAsyce(int cusid)
        {
            
            var list = (from projc in _context.SolutionDataManagements.Where(s=>(int)s.Status != 3 && s.CusID==cusid).OrderByDescending(s => s.ID)
                        select new ProjeccostStory
                        {
                            ID = projc.ID,
                            Name = projc.Name,
                            SeriesID = projc.SeriesID,
                            InvoiceNumber = projc.InvoiceNumber,
                            CompanyID = projc.CompanyID,
                            PostingDate = projc.PostingDate.ToString("dd/MM/yyyy"),
                            ValidUntilDate = projc.ValidUntilDate.ToString("dd/MM/yyyy"),
                           
                        }).ToList();
            return await Task.FromResult(list);
        }
        public async Task<List<ProjeccostStory>> GetHistStorySolutionDataAsyce()
        {
            
            var list = (from projc in _context.SolutionDataManagements.OrderByDescending(s => s.ID)
                        select new ProjeccostStory
                        {
                            ID = projc.ID,
                            Name = projc.Name,
                            SeriesID = projc.SeriesID,
                            InvoiceNumber = projc.InvoiceNumber,
                            CompanyID = projc.CompanyID,
                            PostingDate = projc.PostingDate.ToString("dd/MM/yyyy"),
                            ValidUntilDate = projc.ValidUntilDate.ToString("dd/MM/yyyy"),
                           
                        }).ToList();
            return await Task.FromResult(list);
        }

        public FreightSaleView GetFreights()
        {
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                //GLID = tg.GLID,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });

            var freightDetails = (from fre in _context.Freights
                                  select new FreightSaleDetailViewModel
                                  {
                                      Amount = 0,
                                      FreightID = fre.ID,
                                      ID = 0,
                                      LineID = DateTime.Now.Ticks.ToString() + "" + fre.ID,
                                      FreightSaleID = 0,
                                      Name = fre.Name,
                                      TaxGroupSelect = taxGroup.Select(c => new SelectListItem
                                      {
                                          Value = c.ID.ToString(),
                                          Text = $"{c.Code}-{c.Name}",
                                      }).ToList(),
                                      TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                                                   let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                                   select new TaxGroupViewModel
                                                   {
                                                       ID = t.ID,
                                                       //GLID = tg.GLID,
                                                       Name = t.Name,
                                                       Code = t.Code,
                                                       Effectivefrom = tgds.EffectiveFrom,
                                                       Rate = tgds.Rate,
                                                       Type = (int)t.Type,
                                                   }
                                         ).ToList(),
                                      TaxGroup = "",
                                      TaxGroupID = 0,
                                      TaxRate = 0,
                                      TotalTaxAmount = 0,
                                  }).ToList();
            var freights = new FreightSaleView
            {
                AmountReven = 0,
                FreightSaleDetailViewModels = freightDetails,
                ID = 0,
                SaleID = 0,
                SaleType = SaleCopyType.None,
                TaxSumValue = 0,
            };
            return freights;
        }
        public async Task<List<ProjeccostStory>> GetStoryProjcosQuotetAsyce(int id)
        {
            var list = (from sq in _context.SaleQuotes.Where(i => i.CusID == id && i.BaseonProjCostANID != 0)
                       join projc in _context.ProjectCostAnalyses on sq.BaseonProjCostANID equals projc.ID
                        select new ProjeccostStory
                        {
                            ID = projc.ID,
                            Name = projc.Name,
                            SeriesID = projc.SeriesID,
                            InvoiceNumber = projc.InvoiceNumber,
                            CompanyID = projc.CompanyID,
                            PostingDate = projc.PostingDate.ToString("dd/MM/yyyy"),
                            ValidUntilDate = projc.ValidUntilDate.ToString("dd/MM/yyyy"),
                            DocumentDate = projc.DocumentDate.ToString("dd/MM/yyyy"),
                            BaseOnID=sq.SQID,
                            CopyTypeSQ=CopyTypeSQ.SQ,
                            KeyCopy="SQ-"+sq.InvoiceNumber,
                            
                        }).OrderByDescending(s=>s.ID).ToList();
            return await Task.FromResult(list);
        }

        public dynamic FindSolutionDatas(string number, int seriesId, int comId)
        {
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var proc = (from pa in _context.SolutionDataManagements.Where(x => x.InvoiceNumber == number && x.SeriesID == seriesId && x.CompanyID == comId)
                        join cus in _context.BusinessPartners on pa.CusID equals cus.ID
                        join cont in _context.BusinessPartners on pa.ConTactID equals cont.ID
                        join warehouse in _context.Warehouses on pa.WarehouseID equals warehouse.ID
                        join pricelist in _context.PriceLists on pa.PriceListID equals pricelist.ID
                        join saleem in _context.Employees on pa.SaleEMID equals saleem.ID
                        join owner in _context.Employees on pa.OwnerID equals owner.ID
                        join docType in _context.DocumentTypes on pa.DocTypeID equals docType.ID
                     
                        select new ProjCostAnalysisVeiw
                        {
                            ID = pa.ID,
                            Name = pa.Name,
                            CusID = pa.CusID,
                            CusName = cus.Name,
                            CusCode = cus.Code,
                            ConTactID = pa.ConTactID,
                            ContName = cont.Name,
                            Phone = cont.Phone,
                            SaleEMID = pa.SaleEMID,
                            EmName = saleem.Name,
                            OwnerID = pa.OwnerID,
                            OwnerName = owner.Name,
                            BranchID = pa.BranchID,
                            
                            CompanyID = pa.CompanyID,
                           
                            DocTypeID = pa.DocTypeID,
                          
                           
                            
                            InvoiceNo = $"{docType.Code}-{pa.InvoiceNumber}",
                            InvoiceNumber = pa.InvoiceNumber,
                            
                            
                            PostingDate = pa.PostingDate,
                            PriceListID = pa.PriceListID,
                            RefNo = pa.RefNo,
                            Remarks = pa.Remarks,
                            
                            SaleCurrencyID = pa.SaleCurrencyID,
                            SeriesDID = pa.SeriesDID,
                            SeriesID = pa.SeriesID,

                            Status = (KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore.Status)pa.Status,
                            
                            UserID = pa.UserID,
                            ValidUntilDate = pa.ValidUntilDate,
                            
                            WarehouseID = pa.WarehouseID
                        }).ToList();
            var doctype = new DocumentType();
            Currency _cur = new();
            if (proc.Count > 0)
            {
                _cur = _context.Currency.Find(proc.FirstOrDefault().SaleCurrencyID) ?? new Currency();
            }
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            var _pad = (from pa in proc
                        join pad in _context.SolutionDataManagementDetails on pa.ID equals pad.SolutionDataManagementID
                        join item in _context.ItemMasterDatas on pad.ItemID equals item.ID
                        join cur in _context.Currency on pa.SaleCurrencyID equals cur.ID
                        select new DetailItemMasterData
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            ID = pad.ID,
                            ProjCostID = pad.SolutionDataManagementID,
                            ItemID = pad.ItemID,
                            ItemCode = pad.ItemCode,
                           
                            
                            ItemNameEN = item.EnglishName,
                            ItemNameKH = item.KhmerName,
                            Description = item.KhmerName,
                            Qty = (decimal)pad.Qty,
                           
                            GUomID = pad.GUomID,
                            UomID = pad.UomID,
                            UomName = pad.UomName,
                            
                           
                            InStock = pad.InStock,
                            Remarks = pad.Remarks,
                            BarCode = item.Barcode,
                            Currency = cur.Description,
                          
                            //TotalSys = pad.UnitPrice *(double)pa.ExchangeRate,

                            CurrencyID = cur.ID,
                            // ItemType = item.Type,
                           
                            BaseUoMID = 0,
                            InvenUoMID = 0,
                           
                            

                            /// select List UoM ///
                            UoMs = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                    select new UOMSViewModel
                                    {
                                        BaseUoMID = GDU.BaseUOM,
                                        Factor = GDU.Factor,
                                        ID = UNM.ID,
                                        Name = UNM.Name
                                    }).Select(c => new SelectListItem
                                    {
                                        Value = c.ID.ToString(),
                                        Text = c.Name,
                                        Selected = c.ID == pad.UomID
                                    }).ToList(),
                            /// List UoM ///
                            UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                        select new UOMSViewModel
                                        {
                                            BaseUoMID = GDU.BaseUOM,
                                            Factor = GDU.Factor,
                                            ID = UNM.ID,
                                            Name = UNM.Name
                                        }).ToList(),
                            
                            UomPriceLists = (from pld in _context.PriceListDetails.Where(i => i.ItemID == item.ID && i.PriceListID == pa.PriceListID)
                                             select new UomPriceList
                                             {
                                                 UoMID = (int)pld.UomID,
                                                 UnitPrice = (decimal)pld.UnitPrice
                                             }).ToList(),

                        }).ToList();

            var data = new ProjCostAnalysisupdate
            {
                ProjectCostAnalysis = proc.FirstOrDefault(),
                DetailItemMasterDatas = _pad,
                Currency = _cur
            };
            return data;
        }

        public dynamic CopySolutionDataMGAsynce(string number, int seriesId, int comId)
        {
            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var proc = (from pa in _context.SolutionDataManagements.Where(x => x.InvoiceNumber == number && x.SeriesID == seriesId && x.CompanyID == comId)
                        join cus in _context.BusinessPartners on pa.CusID equals cus.ID
                        join cont in _context.BusinessPartners on pa.ConTactID equals cont.ID
                        join warehouse in _context.Warehouses on pa.WarehouseID equals warehouse.ID
                        join pricelist in _context.PriceLists on pa.PriceListID equals pricelist.ID
                        join saleem in _context.Employees on pa.SaleEMID equals saleem.ID
                        join owner in _context.Employees on pa.OwnerID equals owner.ID
                        join docType in _context.DocumentTypes on pa.DocTypeID equals docType.ID
                        let fs = _context.FreightProjectCosts.Where(i => i.ProjCAID == pa.ID && i.SaleType == SaleCopyType.ProjectCostAnalysisDetail).FirstOrDefault() ?? new FreightProjectCost()
                        select new ProjCostAnalysisVeiw
                        {
                            ID = pa.ID,
                            BaseOnID=pa.ID,
                            CopyType= KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore.CopyType.SDM,
                            KeyCopy="SD-"+pa.InvoiceNumber,
                            Name = pa.Name,
                            CusID = pa.CusID,
                            CusName = cus.Name,
                            CusCode = cus.Code,
                            ConTactID = pa.ConTactID,
                            ContName = cont.Name,
                            Phone = cont.Phone,
                            SaleEMID = pa.SaleEMID,
                            EmName = saleem.Name,
                            OwnerID = pa.OwnerID,
                            OwnerName = owner.Name,
                            BranchID = pa.BranchID,
                            ChangeLog = DateTime.Now,
                            CompanyID = pa.CompanyID,
                            DeliveryDate = pa.ValidUntilDate,
                            DisRate = 0,
                            DisValue = "",
                            DocTypeID = pa.DocTypeID,
                            DocumentDate = DateTime.Now,
                            ExchangeRate = 1,//pa.ExchangeRate,
                            FreightAmount = 0,//pa.FreightAmount,
                            FreightAmountSys = 0,//pa.FreightAmountSys,
                            //FreightProjectCost = new FreightProjectCost
                            //{
                            //    AmountReven = fs.AmountReven,
                            //    ProjCAID = pa.ID,
                            //    ID = fs.ID,
                            //    SaleType = fs.SaleType,
                            //    TaxSumValue = fs.TaxSumValue,
                            //    FreightProjCostDetails = (from fsd in _context.FreightProjCostDetails.Where(i => i.FreightProjectCostID == fs.ID)
                            //                              select new FreightProjCostDetail
                            //                              {
                            //                                  ID = fsd.ID,
                            //                                  LineID = DateTime.Now.Ticks.ToString() + "" + fsd.ID,
                            //                                  FreightID = fsd.FreightID,
                            //                                  Amount = fsd.Amount,
                            //                                  AmountWithTax = fsd.AmountWithTax,
                            //                                  FreightProjectCostID = fsd.FreightProjectCostID,
                            //                                  Name = fsd.Name,
                            //                                  TaxGroup = fsd.TaxGroup,
                            //                                  TaxGroupID = fsd.TaxGroupID,
                            //                                  TaxGroups = GetTaxGroups(),
                            //                                  TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                            //                                  {
                            //                                      Value = i.ID.ToString(),
                            //                                      Selected = fsd.TaxGroupID == i.ID,
                            //                                      Text = $"{i.Code}-{i.Name}"
                            //                                  }).ToList(),
                            //                                  TaxRate = fsd.TaxRate,
                            //                                  TotalTaxAmount = fsd.TotalTaxAmount
                            //                              }).ToList(),
                            //},
                            IncludeVat = false,//pa.IncludeVat,
                            InvoiceNo = $"{docType.Code}-{pa.InvoiceNumber}",
                            InvoiceNumber = pa.InvoiceNumber,
                            LocalCurID = 0,//pa.LocalCurID,
                            LocalSetRate =0,// pa.LocalSetRate,
                            PostingDate = pa.PostingDate,
                            PriceListID = pa.PriceListID,
                            RefNo = pa.RefNo,
                            Remarks = pa.Remarks,
                            TotalCommission = 0,//pa.TotalCommission,
                            OtherCost =0,// pa.OtherCost,
                            SaleCurrencyID = pa.SaleCurrencyID,
                            SeriesDID = pa.SeriesDID,
                            SeriesID = pa.SeriesID,

                            Status = (KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore.Status)pa.Status,
                            SubTotal = 0,//pa.SubTotal,
                            SubTotalAfterDis = 0,//pa.SubTotalAfterDis,
                            SubTotalAfterDisSys =0,// pa.SubTotalAfterDisSys,
                            SubTotalBefDis =0,// pa.SubTotalBefDis,
                            SubTotalBefDisSys =0,// pa.SubTotalBefDisSys,
                            SubTotalSys =0,// pa.SubTotalSys,
                            TotalAmount =0,// pa.TotalAmount,
                            TotalAmountSys =0,// pa.TotalAmountSys,
                            TotalMargin =0,// pa.TotalMargin,
                            TypeDis = "",//pa.TypeDis,
                            UserID = pa.UserID,
                            ValidUntilDate = pa.ValidUntilDate,
                            VatRate =0,// pa.VatRate,
                            VatValue =0,// pa.VatValue,
                            WarehouseID = pa.WarehouseID
                        }).ToList();
            var doctype = new DocumentType();
            Currency _cur = new();
            if (proc.Count > 0)
            {
                _cur = _context.Currency.Find(proc.FirstOrDefault().SaleCurrencyID) ?? new Currency();
            }
            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            var _pad = (from pa in proc
                        join pad in _context.SolutionDataManagementDetails on pa.ID equals pad.SolutionDataManagementID
                        join item in _context.ItemMasterDatas on pad.ItemID equals item.ID
                        join cur in _context.Currency on pa.SaleCurrencyID equals cur.ID
                        select new DetailItemMasterData
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            ID = pad.ID,
                            ProjCostID = pad.SolutionDataManagementID,
                            ItemID = pad.ItemID,
                            ItemCode = pad.ItemCode,
                            ItemType = "",//pad.ItemType,
                            TaxGroupID =0,// pad.TaxGroupID,
                            TaxRate = 0,//pad.TaxRate,
                            TaxValue =0,// pad.TaxValue,
                            TaxOfFinDisValue =0,// pad.TaxOfFinDisValue,
                            FinTotalValue =0,// pad.FinTotalValue,
                            ItemNameEN = item.EnglishName,
                            ItemNameKH = item.KhmerName,
                            Description = item.KhmerName,
                            Qty = (decimal)pad.Qty,
                            OpenQty = 0,//(decimal)pad.OpenQty,
                            GUomID = pad.GUomID,
                            UomID = pad.UomID,
                            UomName = pad.UomName,
                            Factor = 1,//(decimal)pad.Factor,
                            Cost =0,// (decimal)pad.Cost,
                            UnitPrice = 0,//(decimal)pad.UnitPrice,
                            DisRate = 0,//(decimal)pad.DisRate,
                            DisValue =0,// (decimal)pad.DisValue,
                            UnitPriceAfterDis = 0,//pad.UnitPriceAfterDis,
                            Total =0,// (decimal)pad.Total,
                            LineTotalBeforeDis =0,// pad.LineTotalBeforeDis,
                            LineTotalCost = 0,//pad.LineTotalCost,
                            FinDisRate = 0,//pad.FinDisRate,
                            FinDisValue =0,// pad.FinDisValue,
                            UnitMargin = 0,//pad.UnitMargin,
                            TotalWTax = 0,//(decimal)pad.TotalWTax,
                            LineTotalMargin =0,// pad.LineTotalMargin,
                            InStock = pad.InStock,
                            Remarks = pad.Remarks,
                            BarCode = item.Barcode,
                            Currency = cur.Description,
                            TotalSys = 0,//pad.TotalSys,
                            //TotalSys = 0.0,//pad.UnitPrice *(double)pa.ExchangeRate,

                            CurrencyID = cur.ID,
                            // ItemType ="",// item.Type,
                            Process = "",//pad.Process,
                            BaseUoMID = 0,
                            InvenUoMID = 0,
                            VatValue =0,// pad.VatValue,
                            VatRate = 0,//pad.VatRate,

                            TypeDis ="",// pad.TypeDis,
                            ExpireDate =DateTime.Now ,//pad.ExpireDate,
                            TaxDownPaymentValue = 0M,
                            TaxGroupList = tgs.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = $"{c.Code}-{c.Name}",
                                Selected = c.ID == 0//pad.TaxGroupID
                            }).ToList(),


                            /// select List UoM ///
                            UoMs = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                    select new UOMSViewModel
                                    {
                                        BaseUoMID = GDU.BaseUOM,
                                        Factor = GDU.Factor,
                                        ID = UNM.ID,
                                        Name = UNM.Name
                                    }).Select(c => new SelectListItem
                                    {
                                        Value = c.ID.ToString(),
                                        Text = c.Name,
                                        Selected = c.ID == pad.UomID
                                    }).ToList(),
                            /// List UoM ///
                            UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                        select new UOMSViewModel
                                        {
                                            BaseUoMID = GDU.BaseUOM,
                                            Factor = GDU.Factor,
                                            ID = UNM.ID,
                                            Name = UNM.Name
                                        }).ToList(),
                            TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                                         let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                         select new TaxGroupViewModel
                                         {
                                             ID = t.ID,
                                             Name = t.Name,
                                             Code = t.Code,
                                             Effectivefrom = tgds.EffectiveFrom,
                                             Rate = tgds.Rate,
                                             Type = (int)t.Type,
                                         }
                                         ).ToList(),
                            UomPriceLists = (from pld in _context.PriceListDetails.Where(i => i.ItemID == item.ID && i.PriceListID == pa.PriceListID)
                                             select new UomPriceList
                                             {
                                                 UoMID = (int)pld.UomID,
                                                 UnitPrice = (decimal)pld.UnitPrice
                                             }).ToList(),

                        }).ToList();

            var data = new ProjCostAnalysisupdate
            {
                ProjectCostAnalysis = proc.FirstOrDefault(),
                DetailItemMasterDatas = _pad,
                Currency = _cur
            };
            return data;
        }
    }
}
