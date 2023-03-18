using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.ServicesClass.GoodsIssue;
using CKBS.Models.Services.Inventory.PriceList;
using SetGlAccount = CKBS.Models.Services.Inventory.SetGlAccount;
using Type = CKBS.Models.Services.Financials.Type;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Sale;
using SaleOrderViewModel = KEDI.Core.Premise.Models.ServicesClass.Sale.SaleOrderViewModel;
using HumanResourcesEmployee = CKBS.Models.Services.HumanResources.Employee;
using KEDI.Core.Premise.Models.Services.Administrator.SetUp;
using CKBS.Models.Services.Administrator.Setup;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Premise.Repository;
using CKBS.Models.Services.HumanResources;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.ProjectCostAnalysis.Classtore;
using KEDI.Core.Premise.Models.ProjectCostAnalysis;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using Microsoft.EntityFrameworkCore;
using CKBS.Models.ReportClass;
using KEDI.Core.Helpers.Enumerations;

namespace KEDI.Core.Premise.Repository
{
    public interface ICopyFromDiliveryToAR
    {

        void IssuseInStockSaleAR(SaleAR saleAR, List<SaleARDPINCN> ards, SaleGLAccountDetermination saleGlDeter, FreightSale freight);
        void IssuseInstockSaleAREDT(SaleAREdite saleAR, List<SaleARDPINCN> ards, SaleGLAccountDetermination saleGlDeter, FreightSale freight);


    }
    public class SaleARRepo : ICopyFromDiliveryToAR
    {
        private readonly DataContext _context;
        private readonly IDataPropertyRepository _dataProp;
        private readonly UtilityModule _utility;
        public SaleARRepo(DataContext context, IDataPropertyRepository dataProperty, UtilityModule utility)
        {
            _context = context;
            _dataProp = dataProperty;
            _utility = utility;
        }
        public void IssuseInStockSaleAR(SaleAR saleAR, List<SaleARDPINCN> ards, SaleGLAccountDetermination saleGlDeter, FreightSale freight)
        {
            var Com = _context.Company.FirstOrDefault(c => !c.Delete && c.ID == saleAR.CompanyID);
            var docType = _context.DocumentTypes.Find(saleAR.DocTypeID);
            var series = _context.Series.Find(saleAR.SeriesID);
            var warehouse = _context.Warehouses.Find(saleAR.WarehouseID) ?? new Warehouse();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID) ?? new Series();
            // update series
            if (defaultJE.ID > 0)
            {
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = defaultJE.ID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.BranchID = saleAR.BranchID;
                journalEntry.Number = Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = saleAR.UserID;
                journalEntry.TransNo = saleAR.InvoiceNumber;
                journalEntry.PostingDate = saleAR.PostingDate;
                journalEntry.DocumentDate = saleAR.DocumentDate;
                journalEntry.DueDate = saleAR.DueDate;
                journalEntry.SSCID = saleAR.SaleCurrencyID;
                journalEntry.LLCID = saleAR.LocalCurID;
                journalEntry.CompanyID = saleAR.CompanyID;
                journalEntry.LocalSetRate = (decimal)saleAR.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + saleAR.InvoiceNumber;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == saleAR.CusID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            if (glAcc.ID > 0)
            {
                decimal accreAmount = (decimal)saleAR.TotalAmountSys;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = accreAmount,
                    BPAcctID = saleAR.CusID,
                });
                //Insert 
                glAcc.Balance += accreAmount;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = saleAR.PostingDate,
                    Origin = docType.ID,
                    OriginNo = saleAR.InvoiceNumber,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + " - " + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = accreAmount,
                    LocalSetRate = saleAR.LocalCurID,
                    GLAID = accountReceive.GLAccID,
                    Creator = saleAR.UserID,
                    BPAcctID = saleAR.CusID,
                    Effective = EffectiveBlance.Debit
                });
                _context.Update(glAcc);
            }
            // BP ARDown Payment //

            if (saleAR.DownPaymentSys > 0)
            {
                var dpmAcc = _context.GLAccounts.FirstOrDefault(i => i.ID == saleGlDeter.GLID) ?? new GLAccount();
                if (dpmAcc.ID > 0)
                {
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = accountReceive.GLAccID,
                        Debit = saleAR.DownPaymentSys,
                        BPAcctID = saleAR.CusID,
                    });
                    //Insert 
                    dpmAcc.Balance += saleAR.DownPaymentSys;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = saleAR.PostingDate,
                        Origin = docType.ID,
                        OriginNo = saleAR.InvoiceNumber,
                        OffsetAccount = dpmAcc.Code,
                        Details = douTypeID.Name + " - " + dpmAcc.Code,
                        CumulativeBalance = dpmAcc.Balance,
                        Debit = saleAR.DownPaymentSys,
                        LocalSetRate = saleAR.LocalCurID,
                        GLAID = dpmAcc.ID,
                        BPAcctID = saleAR.CusID,
                        Creator = saleAR.UserID,
                        Effective = EffectiveBlance.Debit
                    });
                    _context.Update(dpmAcc);
                }
                // Tax AR Down Payment //
                var _ards = ards.Where(i => i.Selected).ToList();
                if (_ards.Count > 0)
                {
                    foreach (var ard in _ards)
                    {
                        if (ard.SaleARDPINCNDetails.Any())
                        {
                            foreach (var i in ard.SaleARDPINCNDetails)
                            {
                                // Tax Account ///
                                var taxg = _context.TaxGroups.Find(i.TaxGroupID) ?? new TaxGroup();
                                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                                decimal taxValue = i.TaxDownPaymentValue * (decimal)saleAR.ExchangeRate;
                                if (taxAcc.ID > 0)
                                {
                                    var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                                    if (taxjur.ItemID > 0)
                                    {
                                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                                        taxAcc.Balance += taxValue;
                                        //journalEntryDetail
                                        taxjur.Debit += taxValue;
                                        //accountBalance
                                        accBalance.CumulativeBalance = taxAcc.Balance;
                                        accBalance.Debit += taxValue;
                                    }
                                    else
                                    {
                                        taxAcc.Balance += taxValue;
                                        journalEntryDetail.Add(new JournalEntryDetail
                                        {
                                            JEID = journalEntry.ID,
                                            Type = Type.GLAcct,
                                            ItemID = taxAcc.ID,
                                            Debit = taxValue,
                                        });
                                        //
                                        accountBalance.Add(new AccountBalance
                                        {
                                            JEID = journalEntry.ID,
                                            PostingDate = saleAR.PostingDate,
                                            Origin = docType.ID,
                                            OriginNo = saleAR.InvoiceNumber,
                                            OffsetAccount = taxAcc.Code,
                                            Details = douTypeID.Name + " - " + taxAcc.Code,
                                            CumulativeBalance = taxAcc.Balance,
                                            Debit = taxValue,
                                            LocalSetRate = ard.LocalSetRate,
                                            GLAID = taxAcc.ID,
                                            Effective = EffectiveBlance.Debit
                                        });
                                    }
                                    _context.Update(taxAcc);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        var __ard = _context.ARDownPayments.Find(ard.ARDID) ?? new ARDownPayment();
                        __ard.Status = "used";
                        // __ard.ARID = orderid;
                        _context.ARDownPayments.Update(__ard);
                        _context.SaveChanges();
                    }
                }

            }
            // Freight //
            if (freight != null)
            {
                if (freight.FreightSaleDetails.Any())
                {
                    foreach (var fr in freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.RevenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)saleAR.ExchangeRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance -= _framount;
                                //journalEntryDetail
                                frgljur.Credit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _framount;
                            }
                            else
                            {
                                frgl.Balance -= _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Credit = _framount,
                                    BPAcctID = saleAR.CusID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = saleAR.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = saleAR.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Credit = _framount,
                                    LocalSetRate = (decimal)saleAR.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)saleAR.ExchangeRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance -= _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Credit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance -= _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Credit = _frtaxamount,
                                    BPAcctID = saleAR.CusID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = saleAR.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = saleAR.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Credit = _frtaxamount,
                                    LocalSetRate = (decimal)saleAR.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            //IssuseInStockSaleAR
            foreach (var item in saleAR.SaleARDetails)
            {
                int revenueAccID = 0;
                decimal revenueAccAmount = 0;
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var revenueAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == saleAR.WarehouseID)
                                      join gl in _context.GLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();

                    revenueAccID = revenueAcc.ID;
                    if (saleAR.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)saleAR.DisRate / 100;
                        revenueAccAmount = (decimal)item.TotalSys - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys;
                    }
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var revenueAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                      join gl in _context.GLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    if (revenueAcc != null)
                    {
                        revenueAccID = revenueAcc.ID;
                        if (saleAR.DisRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)saleAR.DisRate / 100;
                            revenueAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            revenueAccAmount = (decimal)item.TotalSys;
                        }
                    }
                }
                // Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = item.TaxOfFinDisValue * (decimal)saleAR.ExchangeRate;
                if (taxAcc.ID > 0)
                {
                    var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                    if (taxjur.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                        taxAcc.Balance -= taxValue;
                        //journalEntryDetail
                        taxjur.Credit += taxValue;
                        //accountBalance
                        accBalance.CumulativeBalance = taxAcc.Balance;
                        accBalance.Credit += taxValue;
                    }
                    else
                    {
                        taxAcc.Balance -= taxValue;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Credit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = saleAR.PostingDate,
                            Origin = docType.ID,
                            OriginNo = saleAR.InvoiceNumber,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Credit = taxValue,
                            LocalSetRate = (decimal)saleAR.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(taxAcc);
                }
                // Account Revenue
                var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                if (glAccRevenfifo.ID > 0)
                {
                    var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                    if (listRevenfifo.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                        glAccRevenfifo.Balance -= revenueAccAmount;
                        //journalEntryDetail
                        listRevenfifo.Credit += revenueAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                        accBalance.Credit += revenueAccAmount;
                    }
                    else
                    {
                        glAccRevenfifo.Balance -= revenueAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = revenueAccID,
                            Credit = revenueAccAmount,
                            BPAcctID = saleAR.CusID
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = saleAR.PostingDate,
                            Origin = docType.ID,
                            OriginNo = saleAR.InvoiceNumber,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                            CumulativeBalance = glAccRevenfifo.Balance,
                            Credit = revenueAccAmount,
                            LocalSetRate = (decimal)saleAR.LocalSetRate,
                            GLAID = revenueAccID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(glAccRevenfifo);
                    _context.SaveChanges();
                }
            }

            var journal = _context.JournalEntries.Find(journalEntry.ID) ?? new JournalEntry();
            if (journal.ID > 0)
            {
                journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
                _context.AccountBalances.UpdateRange(accountBalance);
                _context.SaveChanges();
            }
        }

        public void IssuseInstockSaleAREDT(SaleAREdite saleAR, List<SaleARDPINCN> ards, SaleGLAccountDetermination saleGlDeter, FreightSale freight)
        {
            var Com = _context.Company.FirstOrDefault(c => !c.Delete && c.ID == saleAR.CompanyID);
            var docType = _context.DocumentTypes.Find(saleAR.DocTypeID);
            var series = _context.Series.Find(saleAR.SeriesID);
            var warehouse = _context.Warehouses.Find(saleAR.WarehouseID) ?? new Warehouse();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID) ?? new Series();
            // update series
            if (defaultJE.ID > 0)
            {
                string Sno = defaultJE.NextNo;
                long No = long.Parse(Sno);
                defaultJE.NextNo = Convert.ToString(No + 1);
                // update series details
                seriesDetail.SeriesID = defaultJE.ID;
                seriesDetail.Number = Sno;
                _context.Update(defaultJE);
                _context.Update(seriesDetail);
                _context.SaveChanges();
                // Insert Journal Entry
                journalEntry.SeriesID = defaultJE.ID;
                journalEntry.Number = Sno;
                journalEntry.DouTypeID = defaultJE.DocuTypeID;
                journalEntry.Creator = saleAR.UserID;
                 journalEntry.BranchID = saleAR.BranchID;
                journalEntry.TransNo = saleAR.InvoiceNumber;
                journalEntry.PostingDate = saleAR.PostingDate;
                journalEntry.DocumentDate = saleAR.DocumentDate;
                journalEntry.DueDate = saleAR.DueDate;
                journalEntry.SSCID = saleAR.SaleCurrencyID;
                journalEntry.LLCID = saleAR.LocalCurID;
                journalEntry.CompanyID = saleAR.CompanyID;
                journalEntry.LocalSetRate = (decimal)saleAR.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + saleAR.InvoiceNumber;
                _context.Update(journalEntry);
            }
            _context.SaveChanges();
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == saleAR.CusID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            if (glAcc.ID > 0)
            {
                decimal accreAmount = (decimal)saleAR.TotalAmountSys;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = accreAmount,
                    BPAcctID = saleAR.CusID,
                });
                //Insert 
                glAcc.Balance += accreAmount;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = saleAR.PostingDate,
                    Origin = docType.ID,
                    OriginNo = saleAR.InvoiceNumber,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + " - " + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = accreAmount,
                    LocalSetRate = saleAR.LocalCurID,
                    GLAID = accountReceive.GLAccID,
                    Creator = saleAR.UserID,
                    BPAcctID = saleAR.CusID,
                    Effective = EffectiveBlance.Debit
                });
                _context.Update(glAcc);
            }
            // BP ARDown Payment //

            if (saleAR.DownPaymentSys > 0)
            {
                var dpmAcc = _context.GLAccounts.FirstOrDefault(i => i.ID == saleGlDeter.GLID) ?? new GLAccount();
                if (dpmAcc.ID > 0)
                {
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = accountReceive.GLAccID,
                        Debit = saleAR.DownPaymentSys,
                        BPAcctID = saleAR.CusID,
                    });
                    //Insert 
                    dpmAcc.Balance += saleAR.DownPaymentSys;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = saleAR.PostingDate,
                        Origin = docType.ID,
                        OriginNo = saleAR.InvoiceNumber,
                        OffsetAccount = dpmAcc.Code,
                        Details = douTypeID.Name + " - " + dpmAcc.Code,
                        CumulativeBalance = dpmAcc.Balance,
                        Debit = saleAR.DownPaymentSys,
                        LocalSetRate = saleAR.LocalCurID,
                        GLAID = dpmAcc.ID,
                        BPAcctID = saleAR.CusID,
                        Creator = saleAR.UserID,
                        Effective = EffectiveBlance.Debit
                    });
                    _context.Update(dpmAcc);
                }
                // Tax AR Down Payment //
                var _ards = ards.Where(i => i.Selected).ToList();
                if (_ards.Count > 0)
                {
                    foreach (var ard in _ards)
                    {
                        if (ard.SaleARDPINCNDetails.Any())
                        {
                            foreach (var i in ard.SaleARDPINCNDetails)
                            {
                                // Tax Account ///
                                var taxg = _context.TaxGroups.Find(i.TaxGroupID) ?? new TaxGroup();
                                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                                decimal taxValue = i.TaxDownPaymentValue * (decimal)saleAR.ExchangeRate;
                                if (taxAcc.ID > 0)
                                {
                                    var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                                    if (taxjur.ItemID > 0)
                                    {
                                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                                        taxAcc.Balance += taxValue;
                                        //journalEntryDetail
                                        taxjur.Debit += taxValue;
                                        //accountBalance
                                        accBalance.CumulativeBalance = taxAcc.Balance;
                                        accBalance.Debit += taxValue;
                                    }
                                    else
                                    {
                                        taxAcc.Balance += taxValue;
                                        journalEntryDetail.Add(new JournalEntryDetail
                                        {
                                            JEID = journalEntry.ID,
                                            Type = Type.GLAcct,
                                            ItemID = taxAcc.ID,
                                            Debit = taxValue,
                                        });
                                        //
                                        accountBalance.Add(new AccountBalance
                                        {
                                            JEID = journalEntry.ID,
                                            PostingDate = saleAR.PostingDate,
                                            Origin = docType.ID,
                                            OriginNo = saleAR.InvoiceNumber,
                                            OffsetAccount = taxAcc.Code,
                                            Details = douTypeID.Name + " - " + taxAcc.Code,
                                            CumulativeBalance = taxAcc.Balance,
                                            Debit = taxValue,
                                            LocalSetRate = ard.LocalSetRate,
                                            GLAID = taxAcc.ID,
                                            Effective = EffectiveBlance.Debit
                                        });
                                    }
                                    _context.Update(taxAcc);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        var __ard = _context.ARDownPayments.Find(ard.ARDID) ?? new ARDownPayment();
                        __ard.Status = "used";
                        // __ard.ARID = orderid;
                        _context.ARDownPayments.Update(__ard);
                        _context.SaveChanges();
                    }
                }

            }
            // Freight //
            if (freight != null)
            {
                if (freight.FreightSaleDetails.Any())
                {
                    foreach (var fr in freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.RevenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)saleAR.ExchangeRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance -= _framount;
                                //journalEntryDetail
                                frgljur.Credit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _framount;
                            }
                            else
                            {
                                frgl.Balance -= _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Credit = _framount,
                                    BPAcctID = saleAR.CusID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = saleAR.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = saleAR.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Credit = _framount,
                                    LocalSetRate = (decimal)saleAR.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)saleAR.ExchangeRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance -= _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Credit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Credit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance -= _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Credit = _frtaxamount,
                                    BPAcctID = saleAR.CusID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = saleAR.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = saleAR.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Credit = _frtaxamount,
                                    LocalSetRate = (decimal)saleAR.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            //IssuseInStockSaleAR EDT
            foreach (var item in saleAR.SaleAREditeDetails)
            {
                int revenueAccID = 0;
                decimal revenueAccAmount = 0;
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);
                if (itemMaster.SetGlAccount == SetGlAccount.ItemLevel)
                {
                    var revenueAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == saleAR.WarehouseID)
                                      join gl in _context.GLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();

                    revenueAccID = revenueAcc.ID;
                    if (saleAR.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)saleAR.DisRate / 100;
                        revenueAccAmount = (decimal)item.TotalSys - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys;
                    }
                }
                else if (itemMaster.SetGlAccount == SetGlAccount.ItemGroup)
                {
                    var revenueAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                      join gl in _context.GLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    if (revenueAcc != null)
                    {
                        revenueAccID = revenueAcc.ID;
                        if (saleAR.DisRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)saleAR.DisRate / 100;
                            revenueAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            revenueAccAmount = (decimal)item.TotalSys;
                        }
                    }
                }
                // Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = item.TaxOfFinDisValue * (decimal)saleAR.ExchangeRate;
                if (taxAcc.ID > 0)
                {
                    var taxjur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxAcc.ID) ?? new JournalEntryDetail();
                    if (taxjur.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxAcc.ID);
                        taxAcc.Balance -= taxValue;
                        //journalEntryDetail
                        taxjur.Credit += taxValue;
                        //accountBalance
                        accBalance.CumulativeBalance = taxAcc.Balance;
                        accBalance.Credit += taxValue;
                    }
                    else
                    {
                        taxAcc.Balance -= taxValue;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Credit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = saleAR.PostingDate,
                            Origin = docType.ID,
                            OriginNo = saleAR.InvoiceNumber,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Credit = taxValue,
                            LocalSetRate = (decimal)saleAR.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(taxAcc);
                }
                // Account Revenue
                var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                if (glAccRevenfifo.ID > 0)
                {
                    var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                    if (listRevenfifo.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                        glAccRevenfifo.Balance -= revenueAccAmount;
                        //journalEntryDetail
                        listRevenfifo.Credit += revenueAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                        accBalance.Credit += revenueAccAmount;
                    }
                    else
                    {
                        glAccRevenfifo.Balance -= revenueAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.GLAcct,
                            ItemID = revenueAccID,
                            Credit = revenueAccAmount,
                            BPAcctID = saleAR.CusID
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = saleAR.PostingDate,
                            Origin = docType.ID,
                            OriginNo = saleAR.InvoiceNumber,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                            CumulativeBalance = glAccRevenfifo.Balance,
                            Credit = revenueAccAmount,
                            LocalSetRate = (decimal)saleAR.LocalSetRate,
                            GLAID = revenueAccID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(glAccRevenfifo);
                    _context.SaveChanges();
                }
            }

            var journal = _context.JournalEntries.Find(journalEntry.ID) ?? new JournalEntry();
            if (journal.ID > 0)
            {
                journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
                journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
                _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
                _context.AccountBalances.UpdateRange(accountBalance);
                _context.SaveChanges();
            }
        }
    }
}
