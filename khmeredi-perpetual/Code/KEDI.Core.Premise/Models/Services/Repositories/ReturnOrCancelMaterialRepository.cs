using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.Sale;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using KEDI.Core.Premise.Models.Services.Administrator.SetUp;
using Type = CKBS.Models.Services.Financials.Type;
using CKBS.Models.Services.Administrator.Setup;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;

namespace CKBS.Models.Services.Responsitory
{
    public interface IReturnOrCancelStockMaterial

    {
        //void OrderDetailReturnStock(int receiptid_new);
        void CreditmemoReturnStock(int orderid, string type, List<SaleARDPINCN> ards,
            SaleGLAccountDetermination saleGlDeter, FreightSale freight,
            List<SerialNumber> serials, List<BatchNo> batches, TransTypeWD transType, int BaseOn);

        void CreditmemoReturnStockBasic(int orderid, string type, List<SaleARDPINCN> ards,
               SaleGLAccountDetermination saleGlDeter, FreightSale freight,
               List<SerialNumber> serials, List<BatchNo> batches, TransTypeWD transType, int BaseOn);

        void CreateIncomingPayment(SaleCreditMemo saleCreditMemo, IncomingPaymentCustomer incomingPaymentCustomer);
        void CreditmemoReturnARReserveInvoiceStock(int orderid, string type, List<SaleARDPINCN> ards,
          SaleGLAccountDetermination saleGlDeter, FreightSale freight,
          List<SerialNumber> serials, List<BatchNo> batches, TransTypeWD transType);
        void CreditmemoReturnARReserveInvoiceAfterDeliveryStock(int orderid, string type, int BapsedOnID, List<SaleARDPINCN> ards,
         SaleGLAccountDetermination saleGlDeter, FreightSale freight,
         List<SerialNumber> serials, List<BatchNo> batches, TransTypeWD transType);
        void IssuseInStockARReserveInvoiceEDT(int orderid, string type, List<SaleARDPINCN> ards, SaleGLAccountDetermination saleGlDeter, FreightSale freight,
       List<SerialNumber> serials, List<BatchNo> batches, TransTypeWD transType, int BaseOn, bool transaction_Delivery);
    }
    public class ReturnOrCancelMaterialRepository : IReturnOrCancelStockMaterial
    {
        private readonly DataContext _context;
        public ReturnOrCancelMaterialRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public void CreateIncomingPayment(SaleCreditMemo saleCreditMemo, IncomingPaymentCustomer incomingPaymentCustomer)
        {
            var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
            var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
            var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            SeriesDetail seriesDetailIn = new();
            IncomingPayment incomingPayment = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            List<IncomingPaymentDetail> incomingPaymentDetails = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            //Update Series Incoming
            string incomingNextNo = seriesIn.NextNo;
            long incomingNo = long.Parse(incomingNextNo);
            seriesIn.NextNo = Convert.ToString(incomingNo + 1);
            // Update Series Incoming Detail
            seriesDetailIn.SeriesID = seriesIn.ID;
            seriesDetailIn.Number = incomingNextNo;
            _context.Update(seriesIn);
            _context.Update(seriesDetailIn);
            // update series JE
            string Sno = defaultJE.NextNo;
            long No = long.Parse(Sno);
            defaultJE.NextNo = Convert.ToString(No + 1);
            // update series JE details
            seriesDetail.SeriesID = defaultJE.ID;
            seriesDetail.Number = Sno;
            _context.Update(defaultJE);
            _context.Update(seriesDetail);
            _context.SaveChanges();
            // Insert Journal Entry
            journalEntry.SeriesID = defaultJE.ID;
            journalEntry.Number = Sno;
            journalEntry.DouTypeID = defaultJE.DocuTypeID;
            journalEntry.Creator = saleCreditMemo.UserID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = saleCreditMemo.PostingDate;
            journalEntry.DocumentDate = saleCreditMemo.DocumentDate;
            journalEntry.DueDate = saleCreditMemo.DueDate;
            journalEntry.SSCID = saleCreditMemo.SaleCurrencyID;
            journalEntry.LLCID = saleCreditMemo.LocalCurID;
            journalEntry.CompanyID = saleCreditMemo.CompanyID;
            journalEntry.LocalSetRate = (decimal)saleCreditMemo.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            var docNo = incomingPaymentCustomer.ItemInvoice.Split("-")[0];
            incomingPaymentDetails.Add(new IncomingPaymentDetail
            {
                InvoiceNumber = incomingPaymentCustomer.InvoiceNumber,
                ItemInvoice = incomingPaymentCustomer.ItemInvoice,
                DocNo = docNo,
                CheckPay = false,
                DocTypeID = incomingPaymentCustomer.DocTypeID,
                DocumentNo = incomingPaymentCustomer.DocumentNo,
                DocumentType = incomingPaymentCustomer.DocumentType,
                Date = incomingPaymentCustomer.Date,
                OverdueDays = incomingPaymentCustomer.OverdueDays,
                CurrencyName = incomingPaymentCustomer.CurrencyName,
                Total = incomingPaymentCustomer.Total,
                BalanceDue = incomingPaymentCustomer.BalanceDue,
                CashDiscount = incomingPaymentCustomer.CashDiscount,
                TotalDiscount = incomingPaymentCustomer.TotalDiscount,
                Totalpayment = incomingPaymentCustomer.Applied_Amount,
                Applied_Amount = incomingPaymentCustomer.Applied_Amount,
                CurrencyID = incomingPaymentCustomer.CurrencyID,
                ExchangeRate = incomingPaymentCustomer.ExchangeRate,
                Delete = false,
                LocalCurID = incomingPaymentCustomer.LocalCurID,
                LocalSetRate = incomingPaymentCustomer.LocalSetRate,
            });
            incomingPayment = new IncomingPayment
            {
                UserID = saleCreditMemo.UserID,
                SeriesID = seriesIn.ID,
                SeriesDID = seriesDetailIn.ID,
                DocTypeID = docTypeIn.ID,
                CompanyID = saleCreditMemo.CompanyID,
                PaymentMeanID = paymentMean.ID,
                InvoiceNumber = incomingNextNo,
                CustomerID = saleCreditMemo.CusID,
                BranchID = saleCreditMemo.BranchID,
                Ref_No = saleCreditMemo.RefNo,
                PostingDate = saleCreditMemo.PostingDate,
                DocumentDate = saleCreditMemo.DocumentDate,
                TotalAmountDue = incomingPaymentDetails.Sum(i => i.Total) - incomingPaymentDetails.Sum(i => i.Applied_Amount),
                Remark = saleCreditMemo.Remarks,
                Status = "close",
                IncomingPaymentDetails = incomingPaymentDetails,
                LocalCurID = saleCreditMemo.LocalCurID,
                LocalSetRate = saleCreditMemo.LocalSetRate,
            };
            _context.Update(incomingPayment);
            _context.SaveChanges();
            var TotalAmountDue = incomingPaymentDetails.Sum(i => i.Totalpayment);
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == saleCreditMemo.CusID);
            var glAccD = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.BPCode,
                ItemID = accountReceive.GLAccID,
                Credit = (decimal)TotalAmountDue,
                BPAcctID = saleCreditMemo.CusID,
            });
            //Insert            
            glAccD.Balance -= (decimal)TotalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = saleCreditMemo.PostingDate,
                Origin = docTypeIn.ID,
                OriginNo = incomingNextNo,
                OffsetAccount = glAccD.Code,
                Details = docTypeIn.Name + " - " + glAccD.Code,
                CumulativeBalance = glAccD.Balance,
                Credit = (decimal)TotalAmountDue,
                LocalSetRate = (decimal)saleCreditMemo.LocalSetRate,
                GLAID = accountReceive.GLAccID,
                Creator = saleCreditMemo.UserID,
                BPAcctID = saleCreditMemo.CusID,
                Effective = EffectiveBlance.Credit
            });
            var glAccC = _context.GLAccounts.FirstOrDefault(w => w.ID == paymentMean.AccountID);
            journalEntryDetail.Add(new JournalEntryDetail
            {
                JEID = journalEntry.ID,
                Type = Type.BPCode,
                ItemID = paymentMean.AccountID,
                Debit = (decimal)TotalAmountDue,
                BPAcctID = saleCreditMemo.CusID,
            });
            //Insert 
            glAccC.Balance += (decimal)TotalAmountDue;
            accountBalance.Add(new AccountBalance
            {
                JEID = journalEntry.ID,

                PostingDate = saleCreditMemo.PostingDate,
                Origin = docTypeIn.ID,
                OriginNo = incomingNextNo,
                OffsetAccount = glAccC.Code,
                Details = docTypeIn.Name + " - " + glAccC.Code,
                CumulativeBalance = glAccC.Balance,
                Debit = (decimal)TotalAmountDue,
                LocalSetRate = (decimal)saleCreditMemo.LocalSetRate,
                GLAID = paymentMean.AccountID,
                Creator = saleCreditMemo.UserID,
                BPAcctID = saleCreditMemo.CusID,
                Effective = EffectiveBlance.Debit

            });
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.Update(glAccC);
            _context.Update(glAccD);
            _context.UpdateRange(accountBalance);
            _context.UpdateRange(journalEntryDetail);
            _context.SaveChanges();
        }

        public void CreditmemoReturnARReserveInvoiceStock(
           int orderid,
           string type,
           List<SaleARDPINCN> ards,
           SaleGLAccountDetermination saleGlDeter,
           FreightSale freight,
           List<SerialNumber> serials,
           List<BatchNo> batches,
           TransTypeWD transType)
        {
            var SysCurID = _context.Company.FirstOrDefault(w => !w.Delete).SystemCurrencyID;
            var cancelreceipt = _context.SaleCreditMemos.First(r => r.SCMOID == orderid);
            var receiptdetail = _context.SaleCreditMemoDetails.Where(d => d.SCMOID == orderid).ToList();
            var docType = _context.DocumentTypes.Find(cancelreceipt.DocTypeID);
            var series = _context.SeriesDetails.Find(cancelreceipt.SeriesDID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();

            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
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
            journalEntry.Creator = cancelreceipt.UserID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = cancelreceipt.PostingDate;
            journalEntry.DocumentDate = cancelreceipt.DocumentDate;
            journalEntry.DueDate = cancelreceipt.DueDate;
            journalEntry.SSCID = cancelreceipt.SaleCurrencyID;
            journalEntry.LLCID = cancelreceipt.LocalCurID;
            journalEntry.CompanyID = cancelreceipt.CompanyID;
            journalEntry.LocalSetRate = (decimal)cancelreceipt.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // BP ARDown Payment //
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == cancelreceipt.CusID) ?? new HumanResources.BusinessPartner();
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            var dpmAcc = _context.GLAccounts.FirstOrDefault(i => i.ID == saleGlDeter.GLID) ?? new GLAccount();
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
                            var _framount = fr.Amount * (decimal)cancelreceipt.ExchangeRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance += _framount * (-1);
                                //journalEntryDetail
                                frgljur.Debit += _framount * (-1);
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _framount * (-1);
                            }
                            else
                            {
                                frgl.Balance += _framount * (-1);
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount * (-1),
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount * (-1),
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)cancelreceipt.ExchangeRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance += _frtaxamount * (-1);
                                //journalEntryDetail
                                frtaxgljur.Debit += _frtaxamount * (-1);
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _frtaxamount * (-1);
                            }
                            else
                            {
                                taxgacc.Balance += _frtaxamount * (-1);
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount * (-1),
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount * (-1),
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            // AccountReceice
            // BP ARDown Payment //
            if (dpmAcc.ID > 0)
            {
                decimal dp = cancelreceipt.SubTotalAfterDisSys * -1;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = dp,
                    BPAcctID = cancelreceipt.CusID,
                });
                //Insert 
                dpmAcc.Balance -= dp;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = cancelreceipt.PostingDate,
                    Origin = docType.ID,
                    OriginNo = cancelreceipt.InvoiceNumber,
                    OffsetAccount = dpmAcc.Code,
                    Details = douTypeID.Name + " - " + dpmAcc.Code,
                    CumulativeBalance = dpmAcc.Balance,
                    Debit = dp,
                    LocalSetRate = cancelreceipt.LocalCurID,
                    GLAID = dpmAcc.ID,
                    Creator = cancelreceipt.UserID,
                    BPAcctID = cancelreceipt.CusID,
                    Effective = EffectiveBlance.Debit

                });
                _context.Update(dpmAcc);
            }
            //return stock memo
            foreach (var item in receiptdetail)
            {
                //update_warehouse_summary && itemmasterdata
                int revenueAccID = 0;
                decimal revenueAccAmount = 0;
                List<ItemAccounting> itemAccs = new();
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);
                if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                    var revenueAcc = (from ia in itemAccs
                                      join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    if (cancelreceipt.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                        revenueAccAmount = ((decimal)item.TotalSys - disvalue) * -1;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys * -1;
                    }
                }
                else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                    var revenueAcc = (from ia in itemAccs
                                      join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    if (cancelreceipt.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                        revenueAccAmount = ((decimal)item.TotalSys - disvalue) * -1;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys * -1;
                    }
                }
                // Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = (item.TaxOfFinDisValue * (decimal)cancelreceipt.ExchangeRate) * -1;
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
                            PostingDate = cancelreceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.InvoiceNumber,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,
                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
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
                        glAccRevenfifo.Balance += revenueAccAmount;
                        //journalEntryDetail
                        listRevenfifo.Credit += revenueAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                        accBalance.Credit += revenueAccAmount;
                    }
                    else
                    {
                        glAccRevenfifo.Balance += revenueAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = revenueAccID,
                            Credit = revenueAccAmount,
                            BPAcctID = cancelreceipt.CusID
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = cancelreceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.InvoiceNumber,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                            CumulativeBalance = glAccRevenfifo.Balance,
                            Credit = revenueAccAmount,
                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = revenueAccID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                }
                _context.Update(glAccRevenfifo);
                _context.SaveChanges();
            }
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        public void CreditmemoReturnARReserveInvoiceAfterDeliveryStock(
            int orderid,
            string type,
            int BapsedOnID,
            List<SaleARDPINCN> ards,
            SaleGLAccountDetermination saleGlDeter,
            FreightSale freight,
            List<SerialNumber> serials,
            List<BatchNo> batches,
            TransTypeWD transType)
        {
            var SysCurID = _context.Company.FirstOrDefault(w => !w.Delete).SystemCurrencyID;
            var cancelreceipt = _context.SaleCreditMemos.First(r => r.SCMOID == orderid);
            var receiptdetail = _context.SaleCreditMemoDetails.Where(d => d.SCMOID == orderid).ToList();
            var docType = _context.DocumentTypes.Find(cancelreceipt.DocTypeID);
            var series = _context.SeriesDetails.Find(cancelreceipt.SeriesDID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
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
            journalEntry.Creator = cancelreceipt.UserID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = cancelreceipt.PostingDate;
            journalEntry.DocumentDate = cancelreceipt.DocumentDate;
            journalEntry.DueDate = cancelreceipt.DueDate;
            journalEntry.SSCID = cancelreceipt.SaleCurrencyID;
            journalEntry.LLCID = cancelreceipt.LocalCurID;
            journalEntry.CompanyID = cancelreceipt.CompanyID;
            journalEntry.LocalSetRate = (decimal)cancelreceipt.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // BP ARDown Payment //
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == cancelreceipt.CusID) ?? new HumanResources.BusinessPartner();
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            var dpmAcc = _context.GLAccounts.FirstOrDefault(i => i.ID == saleGlDeter.GLID) ?? new GLAccount();
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
                            var _framount = fr.Amount * (decimal)cancelreceipt.ExchangeRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance += _framount;
                                //journalEntryDetail
                                frgljur.Debit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _framount;
                            }
                            else
                            {
                                frgl.Balance += _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)cancelreceipt.ExchangeRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance += _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Debit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance += _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            // AccountReceice
            // BP ARDown Payment //
            if (dpmAcc.ID > 0)
            {
                //decimal dp = cancelreceipt.DownPayment * (decimal)cancelreceipt.ExchangeRate;
                decimal dp = cancelreceipt.SubTotalAfterDisSys * -1;
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = dp,
                    BPAcctID = cancelreceipt.CusID,
                });
                //Insert 
                dpmAcc.Balance -= dp;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = cancelreceipt.PostingDate,
                    Origin = docType.ID,
                    OriginNo = cancelreceipt.InvoiceNumber,
                    OffsetAccount = dpmAcc.Code,
                    Details = douTypeID.Name + " - " + dpmAcc.Code,
                    CumulativeBalance = dpmAcc.Balance,
                    Debit = dp,
                    LocalSetRate = cancelreceipt.LocalCurID,
                    GLAID = dpmAcc.ID,
                    Creator = cancelreceipt.UserID,
                    BPAcctID = cancelreceipt.CusID,
                    Effective = EffectiveBlance.Debit
                });
                _context.Update(dpmAcc);
            }
            //return stock memo
            foreach (var item in receiptdetail)
            {
                //update_warehouse_summary && itemmasterdata
                int revenueAccID = 0, inventoryAccID = 0, COGSAccID = 0;
                decimal revenueAccAmount = 0, inventoryAccAmount = 0, COGSAccAmount = 0;
                List<ItemAccounting> itemAccs = new();
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);
                if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                    var revenueAcc = (from ia in itemAccs
                                      join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                            ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in itemAccs
                                   join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;
                    if (cancelreceipt.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                        revenueAccAmount = ((decimal)item.TotalSys - disvalue) * -1;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys * -1;
                    }
                }
                else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                    var revenueAcc = (from ia in itemAccs
                                      join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                            ).FirstOrDefault() ?? new GLAccount();
                    var COGSAcc = (from ia in itemAccs
                                   join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                   select gl
                                         ).FirstOrDefault() ?? new GLAccount();
                    revenueAccID = revenueAcc.ID;
                    inventoryAccID = inventoryAcc.ID;
                    COGSAccID = COGSAcc.ID;
                    if (cancelreceipt.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                        revenueAccAmount = ((decimal)item.TotalSys - disvalue) * -1;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys * -1;
                    }
                }
                if (itemMaster.Process != "Standard")
                {
                    var warehouseSummary = _context.WarehouseSummary
                        .FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                    double @Qty = item.Qty * item.Factor;
                    double @Cost = 0;
                    warehouseSummary.InStock += @Qty;
                    itemMaster.StockIn += @Qty;
                    UpdateItemAccounting(_itemAcc, warehouseSummary);
                    if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                    {
                        if (serials.Count > 0)
                        {
                            foreach (var s in serials)
                            {
                                if (s.SerialNumberSelected != null)
                                {
                                    foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                    {
                                        decimal _inventoryAccAmount = 0M;
                                        decimal _COGSAccAmount = 0M;
                                        StockOut waredetial = _context.StockOuts
                                        .FirstOrDefault(i =>
                                        i.ItemID == item.ItemID
                                        && ss.SerialNumber == i.SerialNumber
                                        && i.InStock > 0 && i.TransType == transType);
                                        if (waredetial != null)
                                        {
                                            waredetial.InStock -= 1;
                                            @Cost = (double)waredetial.Cost;
                                            // insert to warehouse detail
                                            var ware = new WarehouseDetail
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (double)waredetial.Cost,
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
                                                UserID = cancelreceipt.UserID,
                                                ExpireDate = item.ExpireDate,
                                                TransType = TransTypeWD.CreditMemo,
                                                InStockFrom = cancelreceipt.SCMOID,
                                                IsDeleted = true,
                                                BPID = cancelreceipt.CusID
                                            };
                                            _inventoryAccAmount = waredetial.Cost;
                                            _COGSAccAmount = waredetial.Cost;
                                            _context.WarehouseDetails.Add(ware);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancialCreditMemo(
                                            revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail,
                                            accountBalance, revenueAccAmount, _inventoryAccAmount, _COGSAccAmount,
                                            journalEntry, cancelreceipt, docType, douTypeID, glAcc
                                        );
                                    }
                                }
                            }
                            // Insert to Inventory Audit
                            var inventory_audit = _context.InventoryAudits
                                .Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            var inventory = new InventoryAudit
                            {
                                ID = 0,
                                WarehouseID = cancelreceipt.WarehouseID,
                                BranchID = cancelreceipt.BranchID,
                                UserID = cancelreceipt.UserID,
                                ItemID = item.ItemID,
                                CurrencyID = cancelreceipt.SaleCurrencyID,
                                UomID = baseUOM.BaseUOM,
                                InvoiceNo = cancelreceipt.InvoiceNumber,
                                Trans_Type = docType.Code,
                                Process = itemMaster.Process,
                                SystemDate = DateTime.Now,
                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                Qty = @Qty,
                                Cost = @Cost,
                                Price = 0,
                                CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty,
                                CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + @Qty * @Cost,
                                Trans_Valuse = @Cost * @Qty,
                                ExpireDate = item.ExpireDate,
                                LocalCurID = cancelreceipt.LocalCurID,
                                LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                CompanyID = cancelreceipt.CompanyID,
                                DocumentTypeID = docType.ID,
                                SeriesID = cancelreceipt.SeriesID,
                                SeriesDetailID = cancelreceipt.SeriesDID,
                            };
                            CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                            _context.InventoryAudits.Add(inventory);
                            _context.SaveChanges();
                        }
                    }
                    else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                    {
                        if (batches.Count > 0)
                        {
                            foreach (var b in batches)
                            {
                                if (b.BatchNoSelected != null)
                                {
                                    foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                    {
                                        var waredetial = _context.StockOuts
                                            .FirstOrDefault(i =>
                                            i.ItemID == item.ItemID
                                            && sb.BatchNo == i.BatchNo
                                            && i.TransType == transType && i.InStock > 0);
                                        decimal _inventoryAccAmount = 0M;
                                        decimal _COGSAccAmount = 0M;
                                        if (waredetial != null)
                                        {
                                            decimal selectedQty = sb.SelectedQty * (decimal)item.Factor;
                                            waredetial.InStock -= selectedQty;
                                            @Cost = (double)waredetial.Cost;
                                            _context.SaveChanges();

                                            // insert to waredetial
                                            var ware = new WarehouseDetail
                                            {
                                                AdmissionDate = waredetial.AdmissionDate,
                                                Cost = (double)waredetial.Cost,
                                                CurrencyID = waredetial.CurrencyID,
                                                Details = waredetial.Details,
                                                ID = 0,
                                                InStock = (double)selectedQty,
                                                ItemID = item.ItemID,
                                                Location = waredetial.Location,
                                                MfrDate = waredetial.MfrDate,
                                                ProcessItem = ProcessItem.SEBA,
                                                SyetemDate = DateTime.Now,
                                                SysNum = 0,
                                                TimeIn = DateTime.Now,
                                                WarehouseID = waredetial.WarehouseID,
                                                UomID = item.UomID,
                                                UserID = cancelreceipt.UserID,
                                                ExpireDate = (DateTime)waredetial.ExpireDate,
                                                BatchAttr1 = waredetial.BatchAttr1,
                                                BatchAttr2 = waredetial.BatchAttr2,
                                                BatchNo = waredetial.BatchNo,
                                                TransType = TransTypeWD.CreditMemo,
                                                InStockFrom = cancelreceipt.SCMOID,
                                                IsDeleted = true,
                                                BPID = cancelreceipt.CusID
                                            };

                                            _inventoryAccAmount = waredetial.Cost * selectedQty;
                                            _COGSAccAmount = waredetial.Cost * selectedQty;
                                            _context.WarehouseDetails.Add(ware);
                                            _context.SaveChanges();
                                        }
                                        InsertFinancialCreditMemo(
                                            revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail,
                                            accountBalance, revenueAccAmount, _inventoryAccAmount, _COGSAccAmount,
                                            journalEntry, cancelreceipt, docType, douTypeID, glAcc
                                        );
                                    }
                                }
                            }
                            // insert to inventory audit
                            var inventory_audit = _context.InventoryAudits
                                .Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            var inventory = new InventoryAudit
                            {
                                ID = 0,
                                WarehouseID = cancelreceipt.WarehouseID,
                                BranchID = cancelreceipt.BranchID,
                                UserID = cancelreceipt.UserID,
                                ItemID = item.ItemID,
                                CurrencyID = cancelreceipt.SaleCurrencyID,
                                UomID = baseUOM.BaseUOM,
                                InvoiceNo = cancelreceipt.InvoiceNumber,
                                Trans_Type = docType.Code,
                                Process = itemMaster.Process,
                                SystemDate = DateTime.Now,
                                TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                Qty = @Qty,
                                Cost = @Cost,
                                Price = 0,
                                CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty,
                                CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost),
                                Trans_Valuse = @Qty * @Cost,
                                ExpireDate = item.ExpireDate,
                                LocalCurID = cancelreceipt.LocalCurID,
                                LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                CompanyID = cancelreceipt.CompanyID,
                                DocumentTypeID = docType.ID,
                                SeriesID = cancelreceipt.SeriesID,
                                SeriesDetailID = cancelreceipt.SeriesDID,
                            };
                            CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                            _context.InventoryAudits.Add(inventory);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {

                        //insert_warehousedetail
                        var inventoryAudit = new InventoryAudit();
                        var warehouseDetail = new WarehouseDetail();
                        warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                        warehouseDetail.UomID = item.UomID;
                        warehouseDetail.UserID = cancelreceipt.UserID;
                        warehouseDetail.SyetemDate = cancelreceipt.PostingDate;
                        warehouseDetail.TimeIn = DateTime.Now;
                        warehouseDetail.InStock = @Qty;
                        warehouseDetail.CurrencyID = SysCurID;
                        warehouseDetail.ItemID = item.ItemID;
                        warehouseDetail.Cost = @Cost;
                        warehouseDetail.ExpireDate = item.ExpireDate;
                        warehouseDetail.IsDeleted = true;
                        warehouseDetail.TransType = TransTypeWD.CreditMemo;
                        warehouseDetail.InStockFrom = cancelreceipt.SCMOID;
                        warehouseDetail.BPID = cancelreceipt.CusID;
                        if (itemMaster.Process == "FIFO")
                        {
                            //var ware = _context.WarehouseDetails.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID) ?? new WarehouseDetail();
                            var data = (from sd in _context.SaleDeliveries.Where(x => x.BaseOnID == BapsedOnID)
                                        join ind in _context.InventoryAudits on sd.SeriesDID equals ind.SeriesDetailID
                                        select new
                                        {
                                            ind.Cost
                                        }
                              ).FirstOrDefault();

                            @Cost = data.Cost;
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            inventoryAudit.ID = 0;
                            inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                            inventoryAudit.BranchID = cancelreceipt.BranchID;
                            inventoryAudit.UserID = cancelreceipt.UserID;
                            inventoryAudit.ItemID = item.ItemID;
                            inventoryAudit.CurrencyID = SysCurID;
                            inventoryAudit.UomID = item.UomID;
                            inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                            inventoryAudit.Trans_Type = docType.Code;
                            inventoryAudit.Process = itemMaster.Process;
                            inventoryAudit.SystemDate = DateTime.Now;
                            inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                            inventoryAudit.Qty = @Qty;
                            inventoryAudit.Cost = @Cost;
                            inventoryAudit.Price = 0;
                            inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                            inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                            inventoryAudit.Trans_Valuse = @Qty * @Cost;
                            inventoryAudit.ExpireDate = item.ExpireDate;
                            inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                            inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                            inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                            inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                            inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                            inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                            inventoryAccAmount = (decimal)inventoryAudit.Cost * (decimal)@Qty;
                            COGSAccAmount += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                            CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                        }
                        else
                        {
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);

                            if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost)) AvgCost = 0;

                            inventoryAudit.ID = 0;
                            inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                            inventoryAudit.BranchID = cancelreceipt.BranchID;
                            inventoryAudit.UserID = cancelreceipt.UserID;
                            inventoryAudit.ItemID = item.ItemID;
                            inventoryAudit.CurrencyID = SysCurID;
                            inventoryAudit.UomID = item.UomID;
                            inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                            inventoryAudit.Trans_Type = docType.Code;
                            inventoryAudit.Process = itemMaster.Process;
                            inventoryAudit.SystemDate = DateTime.Now;
                            inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                            inventoryAudit.Qty = @Qty;
                            inventoryAudit.Cost = @AvgCost;
                            inventoryAudit.Price = 0;
                            inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                            inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                            inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                            inventoryAudit.ExpireDate = item.ExpireDate;
                            inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                            inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                            inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                            inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                            inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                            inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                            inventoryAccAmount = (decimal)inventoryAudit.Cost;
                            //
                            var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                            double InvCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                            if (double.IsNaN(InvCost) || double.IsInfinity(InvCost))
                            {
                                InvCost = 0;
                                inventoryAudit.Cost = 0;
                            }
                            inventoryAccAmount = (decimal)InvCost * (decimal)@Qty;
                            COGSAccAmount += (decimal)InvCost * (decimal)@Qty;
                            UpdateAvgCost(item.ItemID, cancelreceipt.WarehouseID, item.GUomID, @Qty, @AvgCost);
                            UpdateBomCost(item.ItemID, @Qty, @AvgCost);
                            CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                        }
                        _context.InventoryAudits.Update(inventoryAudit);
                        _context.WarehouseDetails.Update(warehouseDetail);
                    }
                    _context.WarehouseSummary.Update(warehouseSummary);
                    _context.ItemMasterDatas.Update(itemMaster);
                }
                else
                {
                    var priceListDetail = _context.PriceListDetails.FirstOrDefault(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.PriceListID == cancelreceipt.PriceListID) ?? new Inventory.PriceList.PriceListDetail();
                    inventoryAccAmount = (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)cancelreceipt.ExchangeRate;
                    COGSAccAmount += (decimal)priceListDetail.Cost * (decimal)item.Qty * (decimal)cancelreceipt.ExchangeRate;
                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                    InventoryAudit item_inventory_audit = new()
                    {
                        ID = 0,
                        WarehouseID = cancelreceipt.WarehouseID,
                        BranchID = cancelreceipt.BranchID,
                        UserID = cancelreceipt.UserID,
                        ItemID = item.ItemID,
                        CurrencyID = SysCurID,
                        UomID = baseUOM.BaseUOM,
                        InvoiceNo = cancelreceipt.InvoiceNumber,
                        Trans_Type = docType.Code,
                        Process = itemMaster.Process,
                        SystemDate = DateTime.Now,
                        TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                        Qty = item.Qty,
                        Cost = priceListDetail.Cost,
                        Price = 0,
                        CumulativeQty = inventory_audit.Sum(q => q.Qty) + item.Qty,
                        CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (item.Qty * priceListDetail.Cost),
                        Trans_Valuse = item.Qty * priceListDetail.Cost,
                        LocalCurID = cancelreceipt.LocalCurID,
                        LocalSetRate = cancelreceipt.LocalSetRate,
                        SeriesDetailID = cancelreceipt.SeriesDID,
                        SeriesID = cancelreceipt.SeriesID,
                        DocumentTypeID = cancelreceipt.DocTypeID,
                        CompanyID = cancelreceipt.CompanyID,
                    };
                    _context.InventoryAudits.Update(item_inventory_audit);
                    _context.SaveChanges();
                }

                // Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = item.TaxOfFinDisValue * (decimal)cancelreceipt.ExchangeRate * -1;
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

                            PostingDate = cancelreceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.InvoiceNumber,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,
                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(taxAcc);
                }

                if (itemMaster.ManItemBy == ManageItemBy.None)
                {
                    InsertFinancialCreditMemoARR(
                        revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail, accountBalance,
                        revenueAccAmount, inventoryAccAmount, COGSAccAmount, journalEntry, cancelreceipt,
                        docType, douTypeID, glAcc
                    );
                }
            }

            //returm_stock_memo_bom
            foreach (var item in receiptdetail)
            {
                var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new BOMaterial();
                var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                      join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                      join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                      join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                      select new
                                      {
                                          bomd.ItemID,
                                          gd.GroupUoMID,
                                          GUoMID = i.GroupUomID,
                                          Qty = ((double)item.Qty * (double)orft.Factor) * ((double)bomd.Qty * (double)gd.Factor),
                                          bomd.NegativeStock,
                                          i.Process,
                                          UomID = uom.ID,
                                          gd.Factor
                                      }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                if (items_material != null)
                {
                    foreach (var item_cancel in items_material.ToList())
                    {
                        //update_warehouse_summary && itemmasterdata
                        var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                        List<ItemAccounting> itemAccs = new();
                        ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                        double @Qty = item_cancel.Qty;
                        double @Cost = 0;
                        warehouseSummary.InStock += @Qty;
                        itemMaster.StockIn += @Qty;
                        UpdateItemAccounting(_itemAcc, warehouseSummary);
                        int revenueAccIDAvg = 0, inventoryAccIDAvg = 0, COGSAccIDAvg = 0;
                        decimal revenueAccAmountAvg = 0, inventoryAccAmountAvg = 0, COGSAccAmountAvg = 0;
                        var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_cancel.GUoMID);
                        if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                        {
                            itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                            var revenueAcc = (from ia in itemAccs
                                              join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                              select gl
                                                ).FirstOrDefault() ?? new GLAccount();
                            var inventoryAcc = (from ia in itemAccs
                                                join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                                select gl
                                                    ).FirstOrDefault() ?? new GLAccount();
                            var COGSAcc = (from ia in itemAccs
                                           join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                           select gl
                                                ).FirstOrDefault() ?? new GLAccount();
                            COGSAccIDAvg = COGSAcc.ID;
                            revenueAccIDAvg = revenueAcc.ID;
                            inventoryAccIDAvg = inventoryAcc.ID;
                        }
                        else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                        {
                            itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                            var revenueAcc = (from ia in itemAccs
                                              join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                              select gl
                                                ).FirstOrDefault() ?? new GLAccount();
                            var inventoryAcc = (from ia in itemAccs
                                                join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                                select gl
                                                    ).FirstOrDefault() ?? new GLAccount();
                            var COGSAcc = (from ia in itemAccs
                                           join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                           select gl
                                                ).FirstOrDefault() ?? new GLAccount();
                            COGSAccIDAvg = COGSAcc.ID;
                            revenueAccIDAvg = revenueAcc.ID;
                            inventoryAccIDAvg = inventoryAcc.ID;
                        }
                        //insert_warehousedetail
                        var inventoryAudit = new InventoryAudit();
                        var warehouseDetail = new WarehouseDetail();
                        warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                        warehouseDetail.UomID = item_cancel.UomID;
                        warehouseDetail.UserID = cancelreceipt.UserID;
                        warehouseDetail.SyetemDate = cancelreceipt.PostingDate;
                        warehouseDetail.TimeIn = DateTime.Now;
                        warehouseDetail.InStock = @Qty;
                        warehouseDetail.CurrencyID = SysCurID;
                        warehouseDetail.ItemID = item_cancel.ItemID;
                        warehouseDetail.Cost = @Cost;
                        warehouseDetail.ExpireDate = item.ExpireDate;
                        warehouseDetail.InStockFrom = cancelreceipt.SCMOID;
                        warehouseDetail.IsDeleted = true;
                        warehouseDetail.TransType = TransTypeWD.CreditMemo;
                        if (itemMaster.Process == "FIFO")
                        {
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            inventoryAudit.ID = 0;
                            inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                            inventoryAudit.BranchID = cancelreceipt.BranchID;
                            inventoryAudit.UserID = cancelreceipt.UserID;
                            inventoryAudit.ItemID = item_cancel.ItemID;
                            inventoryAudit.CurrencyID = SysCurID;
                            inventoryAudit.UomID = item_cancel.UomID;
                            inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                            inventoryAudit.Trans_Type = docType.Code;
                            inventoryAudit.Process = item_cancel.Process;
                            inventoryAudit.SystemDate = DateTime.Now;
                            inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                            inventoryAudit.Qty = @Qty;
                            inventoryAudit.Cost = @Cost;
                            inventoryAudit.Price = 0;
                            inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                            inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                            inventoryAudit.Trans_Valuse = @Qty * @Cost;
                            inventoryAudit.ExpireDate = item.ExpireDate;
                            inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                            inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                            inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                            inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                            inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                            inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                            inventoryAccAmountAvg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                            COGSAccAmountAvg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                            CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                        }
                        else
                        {
                            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                            inventoryAudit.ID = 0;
                            inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                            inventoryAudit.BranchID = cancelreceipt.BranchID;
                            inventoryAudit.UserID = cancelreceipt.UserID;
                            inventoryAudit.ItemID = item_cancel.ItemID;
                            inventoryAudit.CurrencyID = SysCurID;
                            inventoryAudit.UomID = item_cancel.UomID;
                            inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                            inventoryAudit.Trans_Type = docType.Code;
                            inventoryAudit.Process = item_cancel.Process;
                            inventoryAudit.SystemDate = DateTime.Now;
                            inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                            inventoryAudit.Qty = @Qty;
                            inventoryAudit.Cost = @AvgCost;
                            inventoryAudit.Price = 0;
                            inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                            inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                            inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                            inventoryAudit.ExpireDate = item.ExpireDate;
                            inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                            inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                            inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                            inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                            inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                            inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                            inventoryAccAmountAvg += (decimal)inventoryAudit.Cost;
                            //
                            var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                            double InvCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                            if (double.IsNaN(InvCost))
                            {
                                InvCost = 0;
                                inventoryAudit.Cost = 0;
                            }
                            COGSAccAmountAvg += (decimal)InvCost * (decimal)@Qty;
                            UpdateAvgCost(item_cancel.ItemID, cancelreceipt.WarehouseID, item.GUomID, @Qty, @AvgCost);
                            UpdateBomCost(item_cancel.ItemID, @Qty, @AvgCost);
                            CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                        }
                        _context.InventoryAudits.Update(inventoryAudit);
                        _context.WarehouseSummary.Update(warehouseSummary);
                        _context.ItemMasterDatas.Update(itemMaster);
                        _context.WarehouseDetails.Update(warehouseDetail);

                        // Account Revenue
                        var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccIDAvg) ?? new GLAccount();
                        if (glAccRevenfifo.ID > 0)
                        {
                            var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                            if (listRevenfifo.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccIDAvg);
                                glAccRevenfifo.Balance += revenueAccAmountAvg;
                                //journalEntryDetail
                                listRevenfifo.Debit += revenueAccAmountAvg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                                accBalance.Debit += revenueAccAmountAvg;
                            }
                            else
                            {
                                glAccRevenfifo.Balance += revenueAccAmountAvg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.BPCode,
                                    ItemID = revenueAccIDAvg,
                                    Credit = revenueAccAmountAvg,
                                    BPAcctID = cancelreceipt.CusID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                                    CumulativeBalance = glAccRevenfifo.Balance,
                                    Credit = revenueAccAmountAvg,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = revenueAccIDAvg,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                        }
                        //inventoryAccID
                        var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDAvg) ?? new GLAccount();
                        if (glAccInvenfifo.ID > 0)
                        {
                            var listInvenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                            if (listInvenfifo.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDAvg);
                                glAccInvenfifo.Balance += inventoryAccAmountAvg;
                                //journalEntryDetail
                                listInvenfifo.Debit += inventoryAccAmountAvg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                                accBalance.Debit += inventoryAccAmountAvg;
                            }
                            else
                            {
                                glAccInvenfifo.Balance += inventoryAccAmountAvg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.BPCode,
                                    ItemID = inventoryAccIDAvg,
                                    Debit = inventoryAccAmountAvg,
                                    BPAcctID = cancelreceipt.CusID
                                });

                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                                    CumulativeBalance = glAccInvenfifo.Balance,
                                    Debit = inventoryAccAmountAvg,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = inventoryAccIDAvg,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                        }
                        // COGS
                        var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccIDAvg) ?? new GLAccount();
                        if (glAccCOGSfifo.ID > 0)
                        {
                            var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                            if (list.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccIDAvg);
                                glAccCOGSfifo.Balance -= COGSAccAmountAvg;
                                //journalEntryDetail
                                list.Credit += COGSAccAmountAvg;
                                //accountBalance
                                accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                                accBalance.Credit += COGSAccAmountAvg;
                            }
                            else
                            {
                                glAccCOGSfifo.Balance -= COGSAccAmountAvg;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.BPCode,
                                    ItemID = COGSAccIDAvg,
                                    Credit = COGSAccAmountAvg,
                                    BPAcctID = cancelreceipt.CusID
                                });
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + glAccCOGSfifo.Code,
                                    CumulativeBalance = glAccCOGSfifo.Balance,
                                    Credit = COGSAccAmountAvg,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = COGSAccIDAvg,
                                    Effective = EffectiveBlance.Credit
                                });
                            }
                        }
                        _context.Update(glAccRevenfifo);
                        _context.Update(glAccInvenfifo);
                        _context.Update(glAccCOGSfifo);
                        _context.SaveChanges();
                    }
                }

            }
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        #region  CreditmemoReturnStockBasic
        public void CreditmemoReturnStockBasic(
                    int orderid,
                    string type,
                    List<SaleARDPINCN> ards,
                    SaleGLAccountDetermination saleGlDeter,
                    FreightSale freight,
                    List<SerialNumber> serials,
                    List<BatchNo> batches,
                    TransTypeWD transType, int BaseOn)
        {
            var SysCurID = _context.Company.FirstOrDefault(w => !w.Delete).SystemCurrencyID;
            var cancelreceipt = _context.SaleCreditMemos.First(r => r.SCMOID == orderid);
            var receiptdetail = _context.SaleCreditMemoDetails.Where(d => d.SCMOID == orderid).ToList();
            var docType = _context.DocumentTypes.Find(cancelreceipt.DocTypeID);
            var series = _context.SeriesDetails.Find(cancelreceipt.SeriesDID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            var baseonid = _context.SaleARs.Find(BaseOn);
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
            string Sno = defaultJE.NextNo;
            long No = long.Parse(Sno);
            defaultJE.NextNo = Convert.ToString(No + 1);
            // update series details
            seriesDetail.SeriesID = defaultJE.ID;
            seriesDetail.Number = Sno;
            _context.Update(defaultJE);
            _context.Update(seriesDetail);
            _context.SaveChanges();
            //IssuseInstock       
            // AccountReceice
            if (type == "IN" || type == "CN")
            {
                //return stock memo
                foreach (var item in receiptdetail)
                {
                    //update_warehouse_summary && itemmasterdate              
                    List<ItemAccounting> itemAccs = new();
                    ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                    var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);

                    double @Qty = item.Qty * item.Factor;
                    double @Cost = 0;
                    if (itemMaster.Process != "Standard")
                    {
                        var warehouseSummary = _context.WarehouseSummary
                            .FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        warehouseSummary.InStock += @Qty;
                        itemMaster.StockIn += @Qty;
                        UpdateItemAccounting(_itemAcc, warehouseSummary);
                        if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                        {
                            if (serials.Count > 0)
                            {
                                foreach (var s in serials)
                                {
                                    if (s.SerialNumberSelected != null)
                                    {
                                        foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                        {
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            StockOut waredetial = _context.StockOuts
                                            .FirstOrDefault(i =>
                                            i.ItemID == item.ItemID
                                            && ss.SerialNumber == i.SerialNumber
                                            && i.InStock > 0 && i.TransType == transType);
                                            if (waredetial != null)
                                            {
                                                waredetial.InStock -= 1;
                                                @Cost = (double)waredetial.Cost;
                                                // insert to warehouse detail
                                                var ware = new WarehouseDetail
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (double)waredetial.Cost,
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
                                                    UserID = cancelreceipt.UserID,
                                                    ExpireDate = item.ExpireDate,
                                                    TransType = TransTypeWD.CreditMemo,
                                                    InStockFrom = cancelreceipt.SCMOID,
                                                    IsDeleted = true,
                                                    BPID = cancelreceipt.CusID
                                                };
                                                _inventoryAccAmount = waredetial.Cost;
                                                _COGSAccAmount = waredetial.Cost;
                                                _context.WarehouseDetails.Add(ware);
                                                _context.SaveChanges();
                                            }

                                        }
                                    }
                                }
                                // Insert to Inventory Audit
                                var inventory_audit = _context.InventoryAudits
                                    .Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                var inventory = new InventoryAudit
                                {
                                    ID = 0,
                                    WarehouseID = cancelreceipt.WarehouseID,
                                    BranchID = cancelreceipt.BranchID,
                                    UserID = cancelreceipt.UserID,
                                    ItemID = item.ItemID,
                                    CurrencyID = cancelreceipt.SaleCurrencyID,
                                    UomID = baseUOM.BaseUOM,
                                    InvoiceNo = cancelreceipt.InvoiceNumber,
                                    Trans_Type = docType.Code,
                                    Process = itemMaster.Process,
                                    SystemDate = DateTime.Now,
                                    TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                    Qty = @Qty,
                                    Cost = @Cost,
                                    Price = 0,
                                    CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty,
                                    CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + @Qty * @Cost,
                                    Trans_Valuse = @Cost * @Qty,
                                    ExpireDate = item.ExpireDate,
                                    LocalCurID = cancelreceipt.LocalCurID,
                                    LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                    CompanyID = cancelreceipt.CompanyID,
                                    DocumentTypeID = docType.ID,
                                    SeriesID = cancelreceipt.SeriesID,
                                    SeriesDetailID = cancelreceipt.SeriesDID,
                                };
                                CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                _context.InventoryAudits.Add(inventory);
                                _context.SaveChanges();
                            }
                        }
                        else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                        {
                            if (batches.Count > 0)
                            {
                                foreach (var b in batches)
                                {
                                    if (b.BatchNoSelected != null)
                                    {
                                        foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                        {
                                            var waredetial = _context.StockOuts
                                                .FirstOrDefault(i =>
                                                i.ItemID == item.ItemID
                                                && sb.BatchNo == i.BatchNo
                                                && i.TransType == transType && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                decimal selectedQty = sb.SelectedQty * (decimal)item.Factor;
                                                waredetial.InStock -= selectedQty;
                                                @Cost = (double)waredetial.Cost;
                                                _context.SaveChanges();

                                                // insert to waredetial
                                                var ware = new WarehouseDetail
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (double)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = (double)selectedQty,
                                                    ItemID = item.ItemID,
                                                    Location = waredetial.Location,
                                                    MfrDate = waredetial.MfrDate,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = cancelreceipt.UserID,
                                                    ExpireDate = (DateTime)waredetial.ExpireDate,
                                                    BatchAttr1 = waredetial.BatchAttr1,
                                                    BatchAttr2 = waredetial.BatchAttr2,
                                                    BatchNo = waredetial.BatchNo,
                                                    TransType = TransTypeWD.CreditMemo,
                                                    InStockFrom = cancelreceipt.SCMOID,
                                                    IsDeleted = true,
                                                    BPID = cancelreceipt.CusID
                                                };

                                                _inventoryAccAmount = waredetial.Cost * selectedQty;
                                                _COGSAccAmount = waredetial.Cost * selectedQty;
                                                _context.WarehouseDetails.Add(ware);
                                                _context.SaveChanges();
                                            }

                                        }
                                    }
                                }
                                // insert to inventory audit
                                var inventory_audit = _context.InventoryAudits
                                    .Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                var inventory = new InventoryAudit
                                {
                                    ID = 0,
                                    WarehouseID = cancelreceipt.WarehouseID,
                                    BranchID = cancelreceipt.BranchID,
                                    UserID = cancelreceipt.UserID,
                                    ItemID = item.ItemID,
                                    CurrencyID = cancelreceipt.SaleCurrencyID,
                                    UomID = baseUOM.BaseUOM,
                                    InvoiceNo = cancelreceipt.InvoiceNumber,
                                    Trans_Type = docType.Code,
                                    Process = itemMaster.Process,
                                    SystemDate = DateTime.Now,
                                    TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                    Qty = @Qty,
                                    Cost = @Cost,
                                    Price = 0,
                                    CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty,
                                    CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost),
                                    Trans_Valuse = @Qty * @Cost,
                                    ExpireDate = item.ExpireDate,
                                    LocalCurID = cancelreceipt.LocalCurID,
                                    LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                    CompanyID = cancelreceipt.CompanyID,
                                    DocumentTypeID = docType.ID,
                                    SeriesID = cancelreceipt.SeriesID,
                                    SeriesDetailID = cancelreceipt.SeriesDID,
                                };
                                CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                _context.InventoryAudits.Add(inventory);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            @Cost = item.Cost * cancelreceipt.ExchangeRate;
                            //insert_warehousedetail
                            var inventoryAudit = new InventoryAudit();
                            var warehouseDetail = new WarehouseDetail();
                            warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                            warehouseDetail.UomID = item.UomID;
                            warehouseDetail.UserID = cancelreceipt.UserID;
                            warehouseDetail.SyetemDate = cancelreceipt.PostingDate;
                            warehouseDetail.TimeIn = DateTime.Now;
                            warehouseDetail.InStock = @Qty;
                            warehouseDetail.CurrencyID = SysCurID;
                            warehouseDetail.ItemID = item.ItemID;
                            warehouseDetail.Cost = @Cost;
                            warehouseDetail.ExpireDate = item.ExpireDate;
                            warehouseDetail.IsDeleted = true;
                            warehouseDetail.TransType = TransTypeWD.CreditMemo;
                            warehouseDetail.InStockFrom = cancelreceipt.SCMOID;
                            warehouseDetail.BPID = cancelreceipt.CusID;
                            if (itemMaster.Process == "FIFO")
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = baseUOM.BaseUOM;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = itemMaster.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @Cost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                                inventoryAudit.Trans_Valuse = @Qty * @Cost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;

                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            else
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                                if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost)) AvgCost = 0;
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = item.UomID;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = itemMaster.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @AvgCost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                                inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                                inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;

                                //
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double InvCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                                if (double.IsNaN(InvCost) || double.IsInfinity(InvCost))
                                {
                                    InvCost = 0;
                                    inventoryAudit.Cost = 0;
                                }

                                UpdateAvgCost(item.ItemID, cancelreceipt.WarehouseID, item.GUomID, @Qty, @AvgCost);
                                UpdateBomCost(item.ItemID, @Qty, @AvgCost);
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            _context.InventoryAudits.Update(inventoryAudit);
                            _context.WarehouseDetails.Update(warehouseDetail);
                        }
                        _context.WarehouseSummary.Update(warehouseSummary);
                        _context.ItemMasterDatas.Update(itemMaster);
                    }
                    else
                    {
                        @Cost = item.Cost * cancelreceipt.ExchangeRate;

                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        InventoryAudit item_inventory_audit = new()
                        {
                            ID = 0,
                            WarehouseID = cancelreceipt.WarehouseID,
                            BranchID = cancelreceipt.BranchID,
                            UserID = cancelreceipt.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = SysCurID,
                            UomID = baseUOM.BaseUOM,
                            InvoiceNo = cancelreceipt.InvoiceNumber,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = item.Qty,
                            Cost = @Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) + item.Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (item.Qty * @Cost),
                            Trans_Valuse = item.Qty * @Cost,
                            LocalCurID = cancelreceipt.LocalCurID,
                            LocalSetRate = cancelreceipt.LocalSetRate,
                            SeriesDetailID = cancelreceipt.SeriesDID,
                            SeriesID = cancelreceipt.SeriesID,
                            DocumentTypeID = cancelreceipt.DocTypeID,
                            CompanyID = cancelreceipt.CompanyID,
                        };
                        _context.InventoryAudits.Update(item_inventory_audit);
                        _context.SaveChanges();
                    }

                    // Tax Account ///
                    var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = item.TaxOfFinDisValue * (decimal)cancelreceipt.ExchangeRate;
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

                                PostingDate = cancelreceipt.PostingDate,
                                Origin = docType.ID,
                                OriginNo = cancelreceipt.InvoiceNumber,
                                OffsetAccount = taxAcc.Code,
                                Details = douTypeID.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Debit = taxValue,
                                LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                GLAID = taxAcc.ID,
                                Effective = EffectiveBlance.Debit
                            });
                        }
                        _context.Update(taxAcc);
                    }

                }
                //returm_stock_memo_bom
                foreach (var item in receiptdetail)
                {
                    var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                    var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new BOMaterial();
                    var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                          join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                          join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                          join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                          select new
                                          {
                                              bomd.ItemID,
                                              gd.GroupUoMID,
                                              GUoMID = i.GroupUomID,
                                              Qty = ((double)item.Qty * (double)orft.Factor) * ((double)bomd.Qty * (double)gd.Factor),
                                              bomd.NegativeStock,
                                              i.Process,
                                              UomID = uom.ID,
                                              gd.Factor
                                          }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                    if (items_material != null)
                    {
                        foreach (var item_cancel in items_material.ToList())
                        {
                            //update_warehouse_summary && itemmasterdata
                            var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                            List<ItemAccounting> itemAccs = new();
                            ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                            double @Qty = item_cancel.Qty;
                            double @Cost = 0;
                            warehouseSummary.InStock += @Qty;
                            itemMaster.StockIn += @Qty;
                            UpdateItemAccounting(_itemAcc, warehouseSummary);
                            var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_cancel.GUoMID);

                            //insert_warehousedetail
                            var inventoryAudit = new InventoryAudit();
                            var warehouseDetail = new WarehouseDetail();
                            warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                            warehouseDetail.UomID = item_cancel.UomID;
                            warehouseDetail.UserID = cancelreceipt.UserID;
                            warehouseDetail.SyetemDate = cancelreceipt.PostingDate;
                            warehouseDetail.TimeIn = DateTime.Now;
                            warehouseDetail.InStock = @Qty;
                            warehouseDetail.CurrencyID = SysCurID;
                            warehouseDetail.ItemID = item_cancel.ItemID;
                            warehouseDetail.Cost = @Cost;
                            warehouseDetail.ExpireDate = item.ExpireDate;
                            warehouseDetail.InStockFrom = cancelreceipt.SCMOID;
                            warehouseDetail.IsDeleted = true;
                            warehouseDetail.TransType = TransTypeWD.CreditMemo;
                            if (itemMaster.Process == "FIFO")
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item_cancel.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = item_cancel.UomID;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = item_cancel.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @Cost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                                inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                                inventoryAudit.Trans_Valuse = @Qty * @Cost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;

                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            else
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item_cancel.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = item_cancel.UomID;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = item_cancel.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @AvgCost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                                inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                                inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;

                                //
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double InvCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                                if (Double.IsNaN(InvCost))
                                {
                                    InvCost = 0;
                                    inventoryAudit.Cost = 0;
                                }

                                UpdateAvgCost(item_cancel.ItemID, cancelreceipt.WarehouseID, item.GUomID, @Qty, @AvgCost);
                                UpdateBomCost(item_cancel.ItemID, @Qty, @AvgCost);
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            _context.InventoryAudits.Update(inventoryAudit);
                            _context.WarehouseSummary.Update(warehouseSummary);
                            _context.ItemMasterDatas.Update(itemMaster);
                            _context.WarehouseDetails.Update(warehouseDetail);

                            _context.SaveChanges();
                        }
                    }

                }
            }

        }




        #endregion CreatitmemeoReturnSotckBasic

        #region  CreditmemoReturnStock
        //CreditmemoReturnStock
        public void CreditmemoReturnStock(
             int orderid,
             string type,
             List<SaleARDPINCN> ards,
             SaleGLAccountDetermination saleGlDeter,
             FreightSale freight,
             List<SerialNumber> serials,
             List<BatchNo> batches,
             TransTypeWD transType, int BaseOn)
        {
            var SysCurID = _context.Company.FirstOrDefault(w => !w.Delete).SystemCurrencyID;
            var cancelreceipt = _context.SaleCreditMemos.First(r => r.SCMOID == orderid);
            var receiptdetail = _context.SaleCreditMemoDetails.Where(d => d.SCMOID == orderid).ToList();
            var docType = _context.DocumentTypes.Find(cancelreceipt.DocTypeID);
            var series = _context.SeriesDetails.Find(cancelreceipt.SeriesDID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            var baseonid = _context.SaleARs.Find(BaseOn);
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
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
            journalEntry.Creator = cancelreceipt.UserID;
             journalEntry.BranchID = cancelreceipt.BranchID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = cancelreceipt.PostingDate;
            journalEntry.DocumentDate = cancelreceipt.DocumentDate;
            journalEntry.DueDate = cancelreceipt.DueDate;
            journalEntry.SSCID = cancelreceipt.SaleCurrencyID;
            journalEntry.LLCID = cancelreceipt.LocalCurID;
            journalEntry.CompanyID = cancelreceipt.CompanyID;
            journalEntry.LocalSetRate = (decimal)cancelreceipt.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // BP ARDown Payment //
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == cancelreceipt.CusID) ?? new HumanResources.BusinessPartner();
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            var dpmAcc = _context.GLAccounts.FirstOrDefault(i => i.ID == saleGlDeter.GLID) ?? new GLAccount();

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
                            var _framount = fr.Amount * (decimal)cancelreceipt.ExchangeRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance += _framount;
                                //journalEntryDetail
                                frgljur.Debit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _framount;
                            }
                            else
                            {
                                frgl.Balance += _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)cancelreceipt.ExchangeRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance += _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Debit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance += _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            // AccountReceice
            if (type == "IN" || type == "CN")
            {
                // BP ARDown Payment //
                if (cancelreceipt.DownPaymentSys > 0)
                {
                    if (dpmAcc.ID > 0)
                    {
                        decimal dp = cancelreceipt.DownPayment * (decimal)cancelreceipt.ExchangeRate;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Type.BPCode,
                            ItemID = accountReceive.GLAccID,
                            Credit = dp,
                            BPAcctID = cancelreceipt.CusID,
                        });
                        //Insert 
                        dpmAcc.Balance -= dp;
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = cancelreceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.InvoiceNumber,
                            OffsetAccount = dpmAcc.Code,
                            Details = douTypeID.Name + " - " + dpmAcc.Code,
                            CumulativeBalance = dpmAcc.Balance,
                            Credit = dp,
                            LocalSetRate = cancelreceipt.LocalCurID,
                            GLAID = dpmAcc.ID,
                            Creator = cancelreceipt.UserID,
                            BPAcctID = cancelreceipt.CusID,
                            Effective = EffectiveBlance.Credit


                        });
                        _context.Update(dpmAcc);
                    }
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
                                decimal taxValue = i.TaxDownPaymentValue * (decimal)cancelreceipt.ExchangeRate;
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
                                        taxAcc.Balance += taxValue;
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

                                            PostingDate = cancelreceipt.PostingDate,
                                            Origin = docType.ID,
                                            OriginNo = cancelreceipt.InvoiceNumber,
                                            OffsetAccount = taxAcc.Code,
                                            Details = douTypeID.Name + " - " + taxAcc.Code,
                                            CumulativeBalance = taxAcc.Balance,
                                            Credit = taxValue,
                                            LocalSetRate = ard.LocalSetRate,
                                            GLAID = taxAcc.ID,
                                            Effective = EffectiveBlance.Credit
                                        });
                                    }
                                    _context.Update(taxAcc);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        var __ard = _context.ARDownPayments.Find(ard.ARDID) ?? new ARDownPayment();
                        __ard.Status = "used";
                        _context.ARDownPayments.Update(__ard);
                        _context.SaveChanges();
                    }
                }
                if (glAcc.ID > 0)
                {
                    journalEntryDetail.Add(
                    new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = accountReceive.GLAccID,
                        Credit = (decimal)cancelreceipt.TotalAmountSys,
                        BPAcctID = cancelreceipt.CusID,
                    }
                );
                    //Insert 
                    glAcc.Balance -= (decimal)cancelreceipt.TotalAmountSys;
                    accountBalance.Add(
                        new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = cancelreceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.InvoiceNumber,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAcc.Code,
                            CumulativeBalance = glAcc.Balance,
                            Credit = (decimal)cancelreceipt.TotalAmountSys,
                            LocalSetRate = cancelreceipt.LocalCurID,
                            GLAID = accountReceive.GLAccID,
                            Creator = cancelreceipt.UserID,
                            BPAcctID = cancelreceipt.CusID,
                            Effective = EffectiveBlance.Credit
                        }
                    );
                    //      
                    _context.Update(glAcc);
                }
                //return stock memo
                foreach (var item in receiptdetail)
                {
                    //update_warehouse_summary && itemmasterdata
                    int revenueAccID = 0, inventoryAccID = 0, COGSAccID = 0;
                    decimal revenueAccAmount = 0, inventoryAccAmount = 0, COGSAccAmount = 0;
                    List<ItemAccounting> itemAccs = new();
                    ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                    var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);
                    if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                    {
                        itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                        var revenueAcc = (from ia in itemAccs
                                          join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                          select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var inventoryAcc = (from ia in itemAccs
                                            join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                                ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in itemAccs
                                       join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        revenueAccID = revenueAcc.ID;
                        inventoryAccID = inventoryAcc.ID;
                        COGSAccID = COGSAcc.ID;
                        if (cancelreceipt.DisRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                            revenueAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            revenueAccAmount = (decimal)item.TotalSys;
                        }
                    }
                    else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                    {
                        itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                        var revenueAcc = (from ia in itemAccs
                                          join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                          select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        var inventoryAcc = (from ia in itemAccs
                                            join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                            select gl
                                                ).FirstOrDefault() ?? new GLAccount();
                        var COGSAcc = (from ia in itemAccs
                                       join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                       select gl
                                             ).FirstOrDefault() ?? new GLAccount();
                        revenueAccID = revenueAcc.ID;
                        inventoryAccID = inventoryAcc.ID;
                        COGSAccID = COGSAcc.ID;
                        if (cancelreceipt.DisRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                            revenueAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            revenueAccAmount = (decimal)item.TotalSys;
                        }
                    }
                    double @Qty = item.Qty * item.Factor;
                    double @Cost = 0;
                    if (itemMaster.Process != "Standard")
                    {
                        var warehouseSummary = _context.WarehouseSummary
                            .FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        warehouseSummary.InStock += @Qty;
                        itemMaster.StockIn += @Qty;
                        UpdateItemAccounting(_itemAcc, warehouseSummary);
                        if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers)
                        {
                            if (serials.Count > 0)
                            {
                                foreach (var s in serials)
                                {
                                    if (s.SerialNumberSelected != null)
                                    {
                                        foreach (var ss in s.SerialNumberSelected.SerialNumberSelectedDetails)
                                        {
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            StockOut waredetial = _context.StockOuts
                                            .FirstOrDefault(i =>
                                            i.ItemID == item.ItemID
                                            && ss.SerialNumber == i.SerialNumber
                                            && i.InStock > 0 && i.TransType == transType);
                                            if (waredetial != null)
                                            {
                                                waredetial.InStock -= 1;
                                                @Cost = (double)waredetial.Cost;
                                                // insert to warehouse detail
                                                var ware = new WarehouseDetail
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (double)waredetial.Cost,
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
                                                    UserID = cancelreceipt.UserID,
                                                    ExpireDate = item.ExpireDate,
                                                    TransType = TransTypeWD.CreditMemo,
                                                    InStockFrom = cancelreceipt.SCMOID,
                                                    IsDeleted = true,
                                                    BPID = cancelreceipt.CusID
                                                };
                                                _inventoryAccAmount = waredetial.Cost;
                                                _COGSAccAmount = waredetial.Cost;
                                                _context.WarehouseDetails.Add(ware);
                                                _context.SaveChanges();
                                            }
                                            InsertFinancialCreditMemo(
                                                revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail,
                                                accountBalance, revenueAccAmount, _inventoryAccAmount, _COGSAccAmount,
                                                journalEntry, cancelreceipt, docType, douTypeID, glAcc
                                            );
                                        }
                                    }
                                }
                                // Insert to Inventory Audit
                                var inventory_audit = _context.InventoryAudits
                                    .Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                var inventory = new InventoryAudit
                                {
                                    ID = 0,
                                    LineID = item.SCMODID.ToString(),
                                    TypeItem = SaleCopyType.CreditMemo.ToString(),
                                    WarehouseID = cancelreceipt.WarehouseID,
                                    BranchID = cancelreceipt.BranchID,
                                    UserID = cancelreceipt.UserID,
                                    ItemID = item.ItemID,
                                    CurrencyID = cancelreceipt.SaleCurrencyID,
                                    UomID = baseUOM.BaseUOM,
                                    InvoiceNo = cancelreceipt.InvoiceNumber,
                                    Trans_Type = docType.Code,
                                    Process = itemMaster.Process,
                                    SystemDate = DateTime.Now,
                                    PostingDate = cancelreceipt.PostingDate,
                                    TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                    Qty = @Qty,
                                    Cost = @Cost,
                                    Price = 0,
                                    CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty,
                                    CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + @Qty * @Cost,
                                    Trans_Valuse = @Cost * @Qty,
                                    ExpireDate = item.ExpireDate,
                                    LocalCurID = cancelreceipt.LocalCurID,
                                    LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                    CompanyID = cancelreceipt.CompanyID,
                                    DocumentTypeID = docType.ID,
                                    SeriesID = cancelreceipt.SeriesID,
                                    SeriesDetailID = cancelreceipt.SeriesDID,
                                };
                                CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                _context.InventoryAudits.Add(inventory);
                                _context.SaveChanges();
                            }
                        }
                        else if (itemMaster.ManItemBy == ManageItemBy.Batches)
                        {
                            if (batches.Count > 0)
                            {
                                foreach (var b in batches)
                                {
                                    if (b.BatchNoSelected != null)
                                    {
                                        foreach (var sb in b.BatchNoSelected.BatchNoSelectedDetails)
                                        {
                                            var waredetial = _context.StockOuts
                                                .FirstOrDefault(i =>
                                                i.ItemID == item.ItemID
                                                && sb.BatchNo == i.BatchNo
                                                && i.TransType == transType && i.InStock > 0);
                                            decimal _inventoryAccAmount = 0M;
                                            decimal _COGSAccAmount = 0M;
                                            if (waredetial != null)
                                            {
                                                decimal selectedQty = sb.SelectedQty * (decimal)item.Factor;
                                                waredetial.InStock -= selectedQty;
                                                @Cost = (double)waredetial.Cost;
                                                _context.SaveChanges();

                                                // insert to waredetial
                                                var ware = new WarehouseDetail
                                                {
                                                    AdmissionDate = waredetial.AdmissionDate,
                                                    Cost = (double)waredetial.Cost,
                                                    CurrencyID = waredetial.CurrencyID,
                                                    Details = waredetial.Details,
                                                    ID = 0,
                                                    InStock = (double)selectedQty,
                                                    ItemID = item.ItemID,
                                                    Location = waredetial.Location,
                                                    MfrDate = waredetial.MfrDate,
                                                    ProcessItem = ProcessItem.SEBA,
                                                    SyetemDate = DateTime.Now,
                                                    SysNum = 0,
                                                    TimeIn = DateTime.Now,
                                                    WarehouseID = waredetial.WarehouseID,
                                                    UomID = item.UomID,
                                                    UserID = cancelreceipt.UserID,
                                                    ExpireDate = (DateTime)waredetial.ExpireDate,
                                                    BatchAttr1 = waredetial.BatchAttr1,
                                                    BatchAttr2 = waredetial.BatchAttr2,
                                                    BatchNo = waredetial.BatchNo,
                                                    TransType = TransTypeWD.CreditMemo,
                                                    InStockFrom = cancelreceipt.SCMOID,
                                                    IsDeleted = true,
                                                    BPID = cancelreceipt.CusID
                                                };

                                                _inventoryAccAmount = waredetial.Cost * selectedQty;
                                                _COGSAccAmount = waredetial.Cost * selectedQty;
                                                _context.WarehouseDetails.Add(ware);
                                                _context.SaveChanges();
                                            }
                                            InsertFinancialCreditMemo(
                                                revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail,
                                                accountBalance, revenueAccAmount, _inventoryAccAmount, _COGSAccAmount,
                                                journalEntry, cancelreceipt, docType, douTypeID, glAcc
                                            );
                                        }
                                    }
                                }
                                // insert to inventory audit
                                var inventory_audit = _context.InventoryAudits
                                    .Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                var inventory = new InventoryAudit
                                {
                                    ID = 0,
                                    LineID = item.SCMODID.ToString(),
                                    TypeItem = SaleCopyType.CreditMemo.ToString(),
                                    WarehouseID = cancelreceipt.WarehouseID,
                                    BranchID = cancelreceipt.BranchID,
                                    UserID = cancelreceipt.UserID,
                                    ItemID = item.ItemID,
                                    CurrencyID = cancelreceipt.SaleCurrencyID,
                                    UomID = baseUOM.BaseUOM,
                                    InvoiceNo = cancelreceipt.InvoiceNumber,
                                    Trans_Type = docType.Code,
                                    Process = itemMaster.Process,
                                    SystemDate = DateTime.Now,
                                    PostingDate = cancelreceipt.PostingDate,
                                    TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                                    Qty = @Qty,
                                    Cost = @Cost,
                                    Price = 0,
                                    CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty,
                                    CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost),
                                    Trans_Valuse = @Qty * @Cost,
                                    ExpireDate = item.ExpireDate,
                                    LocalCurID = cancelreceipt.LocalCurID,
                                    LocalSetRate = (double)cancelreceipt.LocalSetRate,
                                    CompanyID = cancelreceipt.CompanyID,
                                    DocumentTypeID = docType.ID,
                                    SeriesID = cancelreceipt.SeriesID,
                                    SeriesDetailID = cancelreceipt.SeriesDID,
                                };
                                CumulativeValue(inventory.WarehouseID, inventory.ItemID, inventory.CumulativeValue, _itemAcc);
                                _context.InventoryAudits.Add(inventory);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            @Cost = item.Cost * cancelreceipt.ExchangeRate;
                            //insert_warehousedetail
                            var inventoryAudit = new InventoryAudit();
                            var warehouseDetail = new WarehouseDetail();
                            warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                            warehouseDetail.UomID = item.UomID;
                            warehouseDetail.UserID = cancelreceipt.UserID;
                            warehouseDetail.SyetemDate = cancelreceipt.PostingDate;
                            warehouseDetail.TimeIn = DateTime.Now;
                            warehouseDetail.InStock = @Qty;
                            warehouseDetail.CurrencyID = SysCurID;
                            warehouseDetail.ItemID = item.ItemID;
                            warehouseDetail.Cost = @Cost;
                            warehouseDetail.ExpireDate = item.ExpireDate;
                            warehouseDetail.IsDeleted = true;
                            warehouseDetail.TransType = TransTypeWD.CreditMemo;
                            warehouseDetail.InStockFrom = cancelreceipt.SCMOID;
                            warehouseDetail.BPID = cancelreceipt.CusID;
                            if (itemMaster.Process == "FIFO")
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                inventoryAudit.ID = 0;
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item.ItemID;
                                inventoryAudit.LineID = item.SCMODID.ToString();
                                inventoryAudit.TypeItem = SaleCopyType.CreditMemo.ToString();
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = baseUOM.BaseUOM;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = itemMaster.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.PostingDate = cancelreceipt.PostingDate;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @Cost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                                inventoryAudit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (@Qty * @Cost);
                                inventoryAudit.Trans_Valuse = @Qty * @Cost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                                inventoryAccAmount = (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                COGSAccAmount += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            else
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                                if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost)) AvgCost = 0;
                                inventoryAudit.ID = 0;
                                inventoryAudit.LineID = item.SCMODID.ToString();
                                inventoryAudit.TypeItem = SaleCopyType.CreditMemo.ToString();
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = item.UomID;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = itemMaster.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.PostingDate = cancelreceipt.PostingDate;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @AvgCost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                                inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                                inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                                inventoryAccAmount = (decimal)inventoryAudit.Cost;
                                //
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double InvCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                                if (double.IsNaN(InvCost) || double.IsInfinity(InvCost))
                                {
                                    InvCost = 0;
                                    inventoryAudit.Cost = 0;
                                }
                                inventoryAccAmount = (decimal)InvCost * (decimal)@Qty;
                                COGSAccAmount += (decimal)InvCost * (decimal)@Qty;
                                UpdateAvgCost(item.ItemID, cancelreceipt.WarehouseID, item.GUomID, @Qty, @AvgCost);
                                UpdateBomCost(item.ItemID, @Qty, @AvgCost);
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            _context.InventoryAudits.Update(inventoryAudit);
                            _context.WarehouseDetails.Update(warehouseDetail);
                        }
                        _context.WarehouseSummary.Update(warehouseSummary);
                        _context.ItemMasterDatas.Update(itemMaster);
                    }
                    else
                    {
                        @Cost = item.Cost * cancelreceipt.ExchangeRate;
                        inventoryAccAmount = (decimal)(@Cost * @Qty);
                        COGSAccAmount += (decimal)(@Cost * @Qty);
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                        InventoryAudit item_inventory_audit = new()
                        {
                            ID = 0,
                            LineID = item.SCMODID.ToString(),
                            TypeItem = SaleCopyType.CreditMemo.ToString(),
                            WarehouseID = cancelreceipt.WarehouseID,
                            BranchID = cancelreceipt.BranchID,
                            UserID = cancelreceipt.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = SysCurID,
                            UomID = baseUOM.BaseUOM,
                            InvoiceNo = cancelreceipt.InvoiceNumber,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            PostingDate = cancelreceipt.PostingDate,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = item.Qty,
                            Cost = @Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) + item.Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) + (item.Qty * @Cost),
                            Trans_Valuse = item.Qty * @Cost,
                            LocalCurID = cancelreceipt.LocalCurID,
                            LocalSetRate = cancelreceipt.LocalSetRate,
                            SeriesDetailID = cancelreceipt.SeriesDID,
                            SeriesID = cancelreceipt.SeriesID,
                            DocumentTypeID = cancelreceipt.DocTypeID,
                            CompanyID = cancelreceipt.CompanyID,
                        };
                        _context.InventoryAudits.Update(item_inventory_audit);
                        _context.SaveChanges();
                    }

                    // Tax Account ///
                    var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = item.TaxOfFinDisValue * (decimal)cancelreceipt.ExchangeRate;
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

                                PostingDate = cancelreceipt.PostingDate,
                                Origin = docType.ID,
                                OriginNo = cancelreceipt.InvoiceNumber,
                                OffsetAccount = taxAcc.Code,
                                Details = douTypeID.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Debit = taxValue,
                                LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                GLAID = taxAcc.ID,
                                Effective = EffectiveBlance.Debit
                            });
                        }
                        _context.Update(taxAcc);
                    }

                    if (itemMaster.ManItemBy == ManageItemBy.None)
                    {
                        if (baseonid != null)
                        {
                            InsertFinancialCreditMemoarcoopy(
                            revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail, accountBalance,
                            revenueAccAmount, inventoryAccAmount, COGSAccAmount, journalEntry, cancelreceipt,
                            docType, douTypeID, glAcc
                        );
                        }
                        else
                        {
                            InsertFinancialCreditMemo(
                            revenueAccID, inventoryAccID, COGSAccID, journalEntryDetail, accountBalance,
                            revenueAccAmount, inventoryAccAmount, COGSAccAmount, journalEntry, cancelreceipt,
                            docType, douTypeID, glAcc
                            );
                        }
                    }
                }
                //returm_stock_memo_bom
                foreach (var item in receiptdetail)
                {
                    var itemM = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                    var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemM.GroupUomID && w.AltUOM == item.UomID);
                    var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID) ?? new BOMaterial();
                    var items_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.Detele == false)
                                          join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
                                          join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                          join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                          select new
                                          {
                                              bomd.ItemID,
                                              gd.GroupUoMID,
                                              GUoMID = i.GroupUomID,
                                              Qty = ((double)item.Qty * (double)orft.Factor) * ((double)bomd.Qty * (double)gd.Factor),
                                              bomd.NegativeStock,
                                              i.Process,
                                              UomID = uom.ID,
                                              gd.Factor
                                          }).Where(w => w.GroupUoMID == w.GUoMID).ToList();
                    if (items_material != null)
                    {
                        foreach (var item_cancel in items_material.ToList())
                        {
                            //update_warehouse_summary && itemmasterdata
                            var warehouseSummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                            var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item_cancel.ItemID);
                            List<ItemAccounting> itemAccs = new();
                            ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                            double @Qty = item_cancel.Qty;
                            double @Cost = 0;
                            warehouseSummary.InStock += @Qty;
                            itemMaster.StockIn += @Qty;
                            UpdateItemAccounting(_itemAcc, warehouseSummary);
                            int revenueAccIDAvg = 0, inventoryAccIDAvg = 0, COGSAccIDAvg = 0;
                            decimal revenueAccAmountAvg = 0, inventoryAccAmountAvg = 0, COGSAccAmountAvg = 0;
                            var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_cancel.GUoMID);
                            if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                            {
                                itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                                var revenueAcc = (from ia in itemAccs
                                                  join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                                  select gl
                                                    ).FirstOrDefault() ?? new GLAccount();
                                var inventoryAcc = (from ia in itemAccs
                                                    join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                                    select gl
                                                        ).FirstOrDefault() ?? new GLAccount();
                                var COGSAcc = (from ia in itemAccs
                                               join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                               select gl
                                                    ).FirstOrDefault() ?? new GLAccount();
                                COGSAccIDAvg = COGSAcc.ID;
                                revenueAccIDAvg = revenueAcc.ID;
                                inventoryAccIDAvg = inventoryAcc.ID;
                            }
                            else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                            {
                                itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                                var revenueAcc = (from ia in itemAccs
                                                  join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                                  select gl
                                                    ).FirstOrDefault() ?? new GLAccount();
                                var inventoryAcc = (from ia in itemAccs
                                                    join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                                    select gl
                                                        ).FirstOrDefault() ?? new GLAccount();
                                var COGSAcc = (from ia in itemAccs
                                               join gl in gLAccounts on ia.CostofGoodsSoldAccount equals gl.Code
                                               select gl
                                                    ).FirstOrDefault() ?? new GLAccount();
                                COGSAccIDAvg = COGSAcc.ID;
                                revenueAccIDAvg = revenueAcc.ID;
                                inventoryAccIDAvg = inventoryAcc.ID;
                            }
                            //insert_warehousedetail
                            var inventoryAudit = new InventoryAudit();
                            var warehouseDetail = new WarehouseDetail();
                            warehouseDetail.WarehouseID = cancelreceipt.WarehouseID;
                            warehouseDetail.UomID = item_cancel.UomID;
                            warehouseDetail.UserID = cancelreceipt.UserID;
                            warehouseDetail.SyetemDate = cancelreceipt.PostingDate;
                            warehouseDetail.TimeIn = DateTime.Now;
                            warehouseDetail.InStock = @Qty;
                            warehouseDetail.CurrencyID = SysCurID;
                            warehouseDetail.ItemID = item_cancel.ItemID;
                            warehouseDetail.Cost = @Cost;
                            warehouseDetail.ExpireDate = item.ExpireDate;
                            warehouseDetail.InStockFrom = cancelreceipt.SCMOID;
                            warehouseDetail.IsDeleted = true;
                            warehouseDetail.TransType = TransTypeWD.CreditMemo;
                            if (itemMaster.Process == "FIFO")
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                inventoryAudit.ID = 0;
                                inventoryAudit.LineID = item.SCMODID.ToString();
                                inventoryAudit.TypeItem = SaleCopyType.CreditMemo.ToString();
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item_cancel.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = item_cancel.UomID;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = item_cancel.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.PostingDate = cancelreceipt.PostingDate;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @Cost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                                inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @Cost);
                                inventoryAudit.Trans_Valuse = @Qty * @Cost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                                inventoryAccAmountAvg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                COGSAccAmountAvg += (decimal)inventoryAudit.Cost * (decimal)@Qty;
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            else
                            {
                                var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item_cancel.ItemID && w.WarehouseID == cancelreceipt.WarehouseID);
                                double @AvgCost = (inventory_audit.Sum(s => s.Trans_Valuse) + @Cost) / (inventory_audit.Sum(s => s.Qty) + Qty);
                                inventoryAudit.ID = 0;
                                inventoryAudit.LineID = item.SCMODID.ToString();
                                inventoryAudit.TypeItem = SaleCopyType.CreditMemo.ToString();
                                inventoryAudit.WarehouseID = cancelreceipt.WarehouseID;
                                inventoryAudit.BranchID = cancelreceipt.BranchID;
                                inventoryAudit.UserID = cancelreceipt.UserID;
                                inventoryAudit.ItemID = item_cancel.ItemID;
                                inventoryAudit.CurrencyID = SysCurID;
                                inventoryAudit.UomID = item_cancel.UomID;
                                inventoryAudit.InvoiceNo = cancelreceipt.InvoiceNumber;
                                inventoryAudit.Trans_Type = docType.Code;
                                inventoryAudit.Process = item_cancel.Process;
                                inventoryAudit.SystemDate = DateTime.Now;
                                inventoryAudit.PostingDate = cancelreceipt.PostingDate;
                                inventoryAudit.TimeIn = DateTime.Now.ToShortTimeString();
                                inventoryAudit.Qty = @Qty;
                                inventoryAudit.Cost = @AvgCost;
                                inventoryAudit.Price = 0;
                                inventoryAudit.CumulativeQty = (inventory_audit.Sum(q => q.Qty)) + (@Qty);
                                inventoryAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * @AvgCost);
                                inventoryAudit.Trans_Valuse = @Qty * @AvgCost;
                                inventoryAudit.ExpireDate = item.ExpireDate;
                                inventoryAudit.LocalCurID = cancelreceipt.LocalCurID;
                                inventoryAudit.LocalSetRate = cancelreceipt.LocalSetRate;
                                inventoryAudit.SeriesDetailID = cancelreceipt.SeriesDID;
                                inventoryAudit.SeriesID = cancelreceipt.SeriesID;
                                inventoryAudit.DocumentTypeID = cancelreceipt.DocTypeID;
                                inventoryAudit.CompanyID = cancelreceipt.CompanyID;
                                inventoryAccAmountAvg += (decimal)inventoryAudit.Cost;
                                //
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double InvCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + inventoryAudit.Cost) / (inventoryAcct.Sum(q => q.Qty) + inventoryAudit.Qty));
                                if (Double.IsNaN(InvCost))
                                {
                                    InvCost = 0;
                                    inventoryAudit.Cost = 0;
                                }
                                COGSAccAmountAvg += (decimal)InvCost * (decimal)@Qty;
                                UpdateAvgCost(item_cancel.ItemID, cancelreceipt.WarehouseID, item.GUomID, @Qty, @AvgCost);
                                UpdateBomCost(item_cancel.ItemID, @Qty, @AvgCost);
                                CumulativeValue(inventoryAudit.WarehouseID, inventoryAudit.ItemID, inventoryAudit.CumulativeValue, _itemAcc);
                            }
                            _context.InventoryAudits.Update(inventoryAudit);
                            _context.WarehouseSummary.Update(warehouseSummary);
                            _context.ItemMasterDatas.Update(itemMaster);
                            _context.WarehouseDetails.Update(warehouseDetail);

                            // Account Revenue
                            var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccIDAvg) ?? new GLAccount();
                            if (glAccRevenfifo.ID > 0)
                            {
                                var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                                if (listRevenfifo.ItemID > 0)
                                {
                                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccIDAvg);
                                    glAccRevenfifo.Balance += revenueAccAmountAvg;
                                    //journalEntryDetail
                                    listRevenfifo.Debit += revenueAccAmountAvg;
                                    //accountBalance
                                    accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                                    accBalance.Debit += revenueAccAmountAvg;
                                }
                                else
                                {
                                    glAccRevenfifo.Balance += revenueAccAmountAvg;
                                    journalEntryDetail.Add(new JournalEntryDetail
                                    {
                                        JEID = journalEntry.ID,
                                        Type = Type.BPCode,
                                        ItemID = revenueAccIDAvg,
                                        Debit = revenueAccAmountAvg,
                                        BPAcctID = cancelreceipt.CusID
                                    });
                                    //
                                    accountBalance.Add(new AccountBalance
                                    {
                                        JEID = journalEntry.ID,

                                        PostingDate = cancelreceipt.PostingDate,
                                        Origin = docType.ID,
                                        OriginNo = cancelreceipt.InvoiceNumber,
                                        OffsetAccount = glAcc.Code,
                                        Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                                        CumulativeBalance = glAccRevenfifo.Balance,
                                        Debit = revenueAccAmountAvg,
                                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                        GLAID = revenueAccIDAvg,
                                        Effective = EffectiveBlance.Debit
                                    });
                                }
                            }
                            //inventoryAccID
                            var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccIDAvg) ?? new GLAccount();
                            if (glAccInvenfifo.ID > 0)
                            {
                                var listInvenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                                if (listInvenfifo.ItemID > 0)
                                {
                                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccIDAvg);
                                    glAccInvenfifo.Balance += inventoryAccAmountAvg;
                                    //journalEntryDetail
                                    listInvenfifo.Debit += inventoryAccAmountAvg;
                                    //accountBalance
                                    accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                                    accBalance.Debit += inventoryAccAmountAvg;
                                }
                                else
                                {
                                    glAccInvenfifo.Balance += inventoryAccAmountAvg;
                                    journalEntryDetail.Add(new JournalEntryDetail
                                    {
                                        JEID = journalEntry.ID,
                                        Type = Type.BPCode,
                                        ItemID = inventoryAccIDAvg,
                                        Debit = inventoryAccAmountAvg,
                                        BPAcctID = cancelreceipt.CusID
                                    });

                                    //
                                    accountBalance.Add(new AccountBalance
                                    {
                                        JEID = journalEntry.ID,

                                        PostingDate = cancelreceipt.PostingDate,
                                        Origin = docType.ID,
                                        OriginNo = cancelreceipt.InvoiceNumber,
                                        OffsetAccount = glAcc.Code,
                                        Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                                        CumulativeBalance = glAccInvenfifo.Balance,
                                        Debit = inventoryAccAmountAvg,
                                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                        GLAID = inventoryAccIDAvg,
                                        Effective = EffectiveBlance.Debit
                                    });
                                }
                            }
                            // COGS
                            var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccIDAvg) ?? new GLAccount();
                            if (glAccCOGSfifo.ID > 0)
                            {
                                var list = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                                if (list.ItemID > 0)
                                {
                                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccIDAvg);
                                    glAccCOGSfifo.Balance -= COGSAccAmountAvg;
                                    //journalEntryDetail
                                    list.Credit += COGSAccAmountAvg;
                                    //accountBalance
                                    accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                                    accBalance.Credit += COGSAccAmountAvg;
                                }
                                else
                                {
                                    glAccCOGSfifo.Balance -= COGSAccAmountAvg;
                                    journalEntryDetail.Add(new JournalEntryDetail
                                    {
                                        JEID = journalEntry.ID,
                                        Type = Type.BPCode,
                                        ItemID = COGSAccIDAvg,
                                        Credit = COGSAccAmountAvg,
                                        BPAcctID = cancelreceipt.CusID
                                    });
                                    accountBalance.Add(new AccountBalance
                                    {
                                        JEID = journalEntry.ID,

                                        PostingDate = cancelreceipt.PostingDate,
                                        Origin = docType.ID,
                                        OriginNo = cancelreceipt.InvoiceNumber,
                                        OffsetAccount = glAcc.Code,
                                        Details = douTypeID.Name + " - " + glAccCOGSfifo.Code,
                                        CumulativeBalance = glAccCOGSfifo.Balance,
                                        Credit = COGSAccAmountAvg,
                                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                        GLAID = COGSAccIDAvg,
                                        Effective = EffectiveBlance.Credit
                                    });
                                }
                            }
                            _context.Update(glAccRevenfifo);
                            _context.Update(glAccInvenfifo);
                            _context.Update(glAccCOGSfifo);
                            _context.SaveChanges();
                        }
                    }

                }
            }
            else if (type == "CD")
            {
                foreach (var i in receiptdetail)
                {
                    // Tax Account ///
                    var taxg = _context.TaxGroups.Find(i.TaxGroupID) ?? new TaxGroup();
                    var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                    decimal taxValue = i.TaxOfFinDisValue * (decimal)cancelreceipt.ExchangeRate;
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

                                PostingDate = cancelreceipt.PostingDate,
                                Origin = docType.ID,
                                OriginNo = cancelreceipt.InvoiceNumber,
                                OffsetAccount = taxAcc.Code,
                                Details = douTypeID.Name + " - " + taxAcc.Code,
                                CumulativeBalance = taxAcc.Balance,
                                Debit = taxValue,
                                LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                GLAID = taxAcc.ID,
                                Effective = EffectiveBlance.Debit
                            });
                        }
                        _context.Update(taxAcc);
                    }
                }
                if (glAcc.ID > 0)
                {
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = accountReceive.GLAccID,
                        Credit = (decimal)cancelreceipt.TotalAmountSys,
                        BPAcctID = cancelreceipt.CusID,
                    });
                    //Insert 
                    glAcc.Balance -= (decimal)cancelreceipt.TotalAmountSys;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAcc.Code,
                        CumulativeBalance = glAcc.Balance,
                        Credit = (decimal)cancelreceipt.TotalAmountSys,
                        LocalSetRate = cancelreceipt.LocalCurID,
                        GLAID = accountReceive.GLAccID,
                        Effective = EffectiveBlance.Credit
                    });
                    _context.Update(glAcc);
                }
                if (dpmAcc.ID > 0)
                {
                    decimal dpmValue = cancelreceipt.DPMValue * (decimal)cancelreceipt.ExchangeRate;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = accountReceive.GLAccID,
                        Debit = dpmValue,
                        BPAcctID = cancelreceipt.CusID,
                    });
                    //Insert 
                    dpmAcc.Balance += dpmValue;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = dpmAcc.Code,
                        Details = douTypeID.Name + " - " + dpmAcc.Code,
                        CumulativeBalance = dpmAcc.Balance,
                        Debit = dpmValue,
                        LocalSetRate = cancelreceipt.LocalCurID,
                        GLAID = dpmAcc.ID,
                        Effective = EffectiveBlance.Debit
                    });
                    _context.Update(dpmAcc);
                }
            }
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }

        #endregion CreditmemoReturnStock
        private void InsertFinancialCreditMemoarcoopy(
          int revenueAccID,
          int inventoryAccID,
          int COGSAccID,
          List<JournalEntryDetail> journalEntryDetail,
          List<AccountBalance> accountBalance,
          decimal revenueAccAmount,
          decimal inventoryAccAmount,
          decimal COGSAccAmount,
          JournalEntry journalEntry,
          SaleCreditMemo cancelreceipt,
          DocumentType docType,
          DocumentType douTypeID,
          GLAccount glAcc
          )
        {
            // Account Revenue
            var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
            if (glAccRevenfifo.ID > 0)
            {
                var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                if (listRevenfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                    glAccRevenfifo.Balance += revenueAccAmount;
                    //journalEntryDetail
                    listRevenfifo.Debit = revenueAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                    accBalance.Debit = revenueAccAmount;
                }
                else
                {
                    glAccRevenfifo.Balance += revenueAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = revenueAccID,
                        Debit = revenueAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                        CumulativeBalance = glAccRevenfifo.Balance,
                        Debit = revenueAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = revenueAccID,
                        Effective = EffectiveBlance.Debit
                    });
                }
            }

            //inventoryAccID
            var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
            if (glAccInvenfifo.ID > 0)
            {
                var listInvenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                if (listInvenfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                    glAccInvenfifo.Balance += inventoryAccAmount;
                    //journalEntryDetail
                    listInvenfifo.Debit += inventoryAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                    accBalance.Debit += inventoryAccAmount;
                }
                else
                {
                    glAccInvenfifo.Balance += inventoryAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = inventoryAccID,
                        Debit = inventoryAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                        CumulativeBalance = glAccInvenfifo.Balance,
                        Debit = inventoryAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = inventoryAccID,
                        Effective = EffectiveBlance.Debit
                    });
                }
            }
            // COGS
            var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccID) ?? new GLAccount();
            if (glAccCOGSfifo.ID > 0)
            {
                var listCOGSfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                if (listCOGSfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccID);
                    glAccCOGSfifo.Balance -= COGSAccAmount;
                    //journalEntryDetail
                    listCOGSfifo.Credit += COGSAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                    accBalance.Credit += COGSAccAmount;
                }
                else
                {
                    glAccCOGSfifo.Balance -= COGSAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = COGSAccID,
                        Credit = COGSAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccCOGSfifo.Code,
                        CumulativeBalance = glAccCOGSfifo.Balance,
                        Credit = COGSAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = COGSAccID,
                        Effective = EffectiveBlance.Credit
                    });
                }
            }
            _context.Update(glAccRevenfifo);
            _context.Update(glAccInvenfifo);
            _context.Update(glAccCOGSfifo);
            _context.SaveChanges();
        }


        private void InsertFinancialCreditMemo(
            int revenueAccID,
            int inventoryAccID,
            int COGSAccID,
            List<JournalEntryDetail> journalEntryDetail,
            List<AccountBalance> accountBalance,
            decimal revenueAccAmount,
            decimal inventoryAccAmount,
            decimal COGSAccAmount,
            JournalEntry journalEntry,
            SaleCreditMemo cancelreceipt,
            DocumentType docType,
            DocumentType douTypeID,
            GLAccount glAcc
            )
        {
            // Account Revenue
            var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
            if (glAccRevenfifo.ID > 0)
            {
                if (glAccRevenfifo.Code == null)
                {
                    glAccRevenfifo.Code = "";
                }
                var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                if (listRevenfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                    glAccRevenfifo.Balance += revenueAccAmount;
                    //journalEntryDetail
                    listRevenfifo.Debit += revenueAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                    accBalance.Debit += revenueAccAmount;
                }
                else
                {
                    glAccRevenfifo.Balance += revenueAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = revenueAccID,
                        Debit = revenueAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                        CumulativeBalance = glAccRevenfifo.Balance,
                        Debit = revenueAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = revenueAccID,
                        Effective = EffectiveBlance.Debit
                    });
                }
            }

            //inventoryAccID
            var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
            if (glAccInvenfifo.ID > 0)
            {
                if (glAccInvenfifo.Code == null)
                {
                    glAccInvenfifo.Code = "";
                }
                var listInvenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                if (listInvenfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                    glAccInvenfifo.Balance += inventoryAccAmount;
                    //journalEntryDetail
                    listInvenfifo.Debit += inventoryAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                    accBalance.Debit += inventoryAccAmount;
                }
                else
                {
                    glAccInvenfifo.Balance += inventoryAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = inventoryAccID,
                        Debit = inventoryAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                        CumulativeBalance = glAccInvenfifo.Balance,
                        Debit = inventoryAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = inventoryAccID,
                        Effective = EffectiveBlance.Debit
                    });
                }
            }
            // COGS
            var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccID) ?? new GLAccount();
            if (glAccCOGSfifo.ID > 0)
            {
                if (glAccCOGSfifo.Code == null)
                {
                    glAccCOGSfifo.Code = "";
                }
                var listCOGSfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                if (listCOGSfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccID);
                    glAccCOGSfifo.Balance -= COGSAccAmount;
                    //journalEntryDetail
                    listCOGSfifo.Credit += COGSAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                    accBalance.Credit += COGSAccAmount;
                }
                else
                {
                    glAccCOGSfifo.Balance -= COGSAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = COGSAccID,
                        Credit = COGSAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccCOGSfifo.Code,
                        CumulativeBalance = glAccCOGSfifo.Balance,
                        Credit = COGSAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = COGSAccID,
                        Effective = EffectiveBlance.Credit
                    });
                }
            }
            _context.Update(glAccRevenfifo);
            _context.Update(glAccInvenfifo);
            _context.Update(glAccCOGSfifo);
            _context.SaveChanges();
        }
        private void InsertFinancialCreditMemoARR(
          int revenueAccID,
          int inventoryAccID,
          int COGSAccID,
          List<JournalEntryDetail> journalEntryDetail,
          List<AccountBalance> accountBalance,
          decimal revenueAccAmount,
          decimal inventoryAccAmount,
          decimal COGSAccAmount,
          JournalEntry journalEntry,
          SaleCreditMemo cancelreceipt,
          DocumentType docType,
          DocumentType douTypeID,
          GLAccount glAcc
          )
        {
            // Account Revenue
            var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
            if (glAccRevenfifo.ID > 0)
            {
                var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                if (listRevenfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID);
                    glAccRevenfifo.Balance += revenueAccAmount;
                    //journalEntryDetail
                    listRevenfifo.Debit += revenueAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                    accBalance.Debit += revenueAccAmount;
                }
                else
                {
                    glAccRevenfifo.Balance += revenueAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = revenueAccID,
                        Credit = revenueAccAmount,
                        BPAcctID = cancelreceipt.CusID
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                        CumulativeBalance = glAccRevenfifo.Balance,
                        Credit = revenueAccAmount,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = revenueAccID,
                        Effective = EffectiveBlance.Credit
                    });
                }
            }
            //inventoryAccID
            var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
            if (glAccInvenfifo.ID > 0)
            {
                var listInvenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                if (listInvenfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                    glAccInvenfifo.Balance += inventoryAccAmount;
                    //journalEntryDetail
                    listInvenfifo.Debit += inventoryAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                    accBalance.Debit += inventoryAccAmount;
                }
                else
                {
                    glAccInvenfifo.Balance += inventoryAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = inventoryAccID,
                        Credit = inventoryAccAmount * -1,
                        BPAcctID = cancelreceipt.CusID
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccInvenfifo.Code,
                        CumulativeBalance = glAccInvenfifo.Balance,
                        Credit = inventoryAccAmount * -1,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = inventoryAccID,
                        Effective = EffectiveBlance.Credit
                    });
                }
            }
            // COGS
            var glAccCOGSfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == COGSAccID) ?? new GLAccount();
            if (glAccCOGSfifo.ID > 0)
            {
                var listCOGSfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccCOGSfifo.ID) ?? new JournalEntryDetail();
                if (listCOGSfifo.ItemID > 0)
                {
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == COGSAccID);
                    glAccCOGSfifo.Balance -= COGSAccAmount;
                    //journalEntryDetail
                    listCOGSfifo.Credit += COGSAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccCOGSfifo.Balance;
                    accBalance.Credit += COGSAccAmount;
                }
                else
                {
                    glAccCOGSfifo.Balance -= COGSAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = COGSAccID,
                        Debit = COGSAccAmount * -1,
                        BPAcctID = cancelreceipt.CusID
                    });
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccCOGSfifo.Code,
                        CumulativeBalance = glAccCOGSfifo.Balance,
                        Debit = COGSAccAmount * -1,
                        LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                        GLAID = COGSAccID,
                        Effective = EffectiveBlance.Debit
                    });
                }
            }
            _context.Update(glAccRevenfifo);
            _context.Update(glAccInvenfifo);
            _context.Update(glAccCOGSfifo);
            _context.SaveChanges();
        }
        //update_AvgCost
        public void UpdateAvgCost(int itemid, int whid, int guomid, double qty, double avgcost)
        {
            // update pricelistdetial
            var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemid);
            var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemid);
            double @AvgCost = ((inventory_audit.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_audit.Sum(q => q.Qty) + qty));
            if (double.IsNaN(@AvgCost) || double.IsInfinity(@AvgCost)) @AvgCost = 0;
            foreach (var pri in pri_detial)
            {
                var guom = _context.GroupDUoMs.Where(g => g.GroupUoMID == guomid);
                var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                foreach (var g in guom)
                {
                    pri.Cost = @AvgCost * exp.SetRate * g.Factor;
                }
                _context.PriceListDetails.Update(pri);
            }
            //update_waresummary
            var inventory_warehouse = _context.InventoryAudits.Where(w => w.ItemID == itemid && w.WarehouseID == whid);
            var waresummary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemid && w.WarehouseID == whid);
            double @AvgCostWare = (inventory_warehouse.Sum(s => s.Trans_Valuse) + avgcost) / (inventory_warehouse.Sum(s => s.Qty) + qty);
            if (double.IsNaN(@AvgCostWare) || double.IsInfinity(@AvgCostWare)) @AvgCostWare = 0;
            waresummary.Cost = @AvgCostWare;
            _context.WarehouseSummary.Update(waresummary);
            _context.SaveChanges();
        }
        //update_bomCost
        public void UpdateBomCost(int itemid, double qty, double avgcost)
        {
            var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == itemid);
            foreach (var itembom in ItemBOMDetail)
            {
                var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                double @AvgCost = (Inven.Sum(s => s.Trans_Valuse) + avgcost) / (Inven.Sum(q => q.Qty) + qty);
                var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                itembom.Cost = @AvgCost;
                itembom.Amount = itembom.Qty * @AvgCost;
                if (double.IsNaN(@AvgCost))
                {
                    itembom.Cost = 0;
                    itembom.Amount = 0;
                }
                _context.BOMDetail.UpdateRange(itembom);
                _context.SaveChanges();
                // sum 
                var BOM = _context.BOMaterial.FirstOrDefault(w => w.BID == itembom.BID);
                var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && !w.Detele);
                BOM.TotalCost = DBOM.Sum(s => s.Amount);
                _context.BOMaterial.Update(BOM);
                _context.SaveChanges();
            }
        }
        private void CumulativeValue(int whid, int itemid, double value, ItemAccounting itemAcc)
        {
            if (double.IsNaN(value))
            {
                value = 0;
            }
            var wherehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == whid && w.ItemID == itemid) ?? new WarehouseSummary();
            wherehouse.CumulativeValue = (decimal)value;
            _context.WarehouseSummary.Update(wherehouse);
            if (itemAcc != null) itemAcc.CumulativeValue = wherehouse.CumulativeValue;
            _context.SaveChanges();
        }
        private void UpdateItemAccounting(ItemAccounting itemAcc, WarehouseSummary ws)
        {
            // item Accounting 
            if (itemAcc != null)
            {
                itemAcc.InStock = ws.InStock;
                itemAcc.Committed = ws.Committed;
                itemAcc.Available = ws.Available;
                itemAcc.Ordered = ws.Ordered;
                _context.ItemAccountings.Update(itemAcc);
                _context.SaveChanges();
            }
        }
        public void IssuseInStockARReserveInvoiceEDT(int orderid, string type, List<SaleARDPINCN> ards, SaleGLAccountDetermination saleGlDeter, FreightSale freight, List<SerialNumber> serials, List<BatchNo> batches, TransTypeWD transType, int BaseOn, bool transaction_Delivery)
        {
            var SysCurID = _context.Company.FirstOrDefault(w => !w.Delete).SystemCurrencyID;
            var cancelreceipt = _context.SaleCreditMemos.First(r => r.SCMOID == orderid);
            var receiptdetail = _context.SaleCreditMemoDetails.Where(d => d.SCMOID == orderid).ToList();
            var docType = _context.DocumentTypes.Find(cancelreceipt.DocTypeID);
            var series = _context.SeriesDetails.Find(cancelreceipt.SeriesDID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            int baseonid = BaseOn;
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            // update series
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
            journalEntry.Creator = cancelreceipt.UserID;
             journalEntry.BranchID = cancelreceipt.BranchID;
            journalEntry.TransNo = Sno;
            journalEntry.PostingDate = cancelreceipt.PostingDate;
            journalEntry.DocumentDate = cancelreceipt.DocumentDate;
            journalEntry.DueDate = cancelreceipt.DueDate;
            journalEntry.SSCID = cancelreceipt.SaleCurrencyID;
            journalEntry.LLCID = cancelreceipt.LocalCurID;
            journalEntry.CompanyID = cancelreceipt.CompanyID;
            journalEntry.LocalSetRate = (decimal)cancelreceipt.LocalSetRate;
            journalEntry.SeriesDID = seriesDetail.ID;
            journalEntry.Remarks = defaultJE.Name + " " + Sno;
            _context.Update(journalEntry);
            _context.SaveChanges();
            //IssuseInstock
            // BP ARDown Payment //
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == cancelreceipt.CusID) ?? new HumanResources.BusinessPartner();
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID) ?? new GLAccount();
            var dpmAcc = _context.GLAccounts.FirstOrDefault(i => i.ID == saleGlDeter.GLID) ?? new GLAccount();

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
                            var _framount = fr.Amount * (decimal)cancelreceipt.ExchangeRate;
                            if (frgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == frgl.ID);
                                frgl.Balance += _framount;
                                //journalEntryDetail
                                frgljur.Debit += _framount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _framount;
                            }
                            else
                            {
                                frgl.Balance += _framount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = frgl.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(frgl);
                            _context.SaveChanges();
                        }
                        if (taxgacc.ID > 0)
                        {
                            var frtaxgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == taxgacc.ID) ?? new JournalEntryDetail();
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)cancelreceipt.ExchangeRate;
                            if (frtaxgljur.ItemID > 0)
                            {
                                var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == taxgacc.ID) ?? new AccountBalance();
                                taxgacc.Balance += _frtaxamount;
                                //journalEntryDetail
                                frtaxgljur.Debit += _frtaxamount;
                                //accountBalance
                                accBalance.CumulativeBalance = frgl.Balance;
                                accBalance.Debit += _frtaxamount;
                            }
                            else
                            {
                                taxgacc.Balance += _frtaxamount;
                                journalEntryDetail.Add(new JournalEntryDetail
                                {
                                    JEID = journalEntry.ID,
                                    Type = Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = cancelreceipt.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = cancelreceipt.InvoiceNumber,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                    GLAID = taxgacc.ID,
                                    Effective = EffectiveBlance.Debit
                                });
                            }
                            _context.Update(taxgacc);
                            _context.SaveChanges();
                        }
                    }
                }
            }
            // AccountReceice

            #region 

            #endregion
            // else
            // {

            // BP ARDown Payment //
            if (cancelreceipt.DownPaymentSys > 0)
            {
                if (dpmAcc.ID > 0)
                {
                    decimal dp = cancelreceipt.DownPayment * (decimal)cancelreceipt.ExchangeRate;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = accountReceive.GLAccID,
                        Credit = dp,

                        BPAcctID = cancelreceipt.CusID,
                    });
                    //Insert 
                    dpmAcc.Balance -= dp;
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = dpmAcc.Code,
                        Details = douTypeID.Name + " - " + dpmAcc.Code,
                        CumulativeBalance = dpmAcc.Balance,
                        Credit = dp,

                        LocalSetRate = cancelreceipt.LocalCurID,
                        GLAID = dpmAcc.ID,
                        Creator = cancelreceipt.UserID,
                        BPAcctID = cancelreceipt.CusID,
                        Effective = EffectiveBlance.Credit


                    });
                    _context.Update(dpmAcc);
                }
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
                            decimal taxValue = i.TaxDownPaymentValue * (decimal)cancelreceipt.ExchangeRate;
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
                                    taxAcc.Balance += taxValue;
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

                                        PostingDate = cancelreceipt.PostingDate,
                                        Origin = docType.ID,
                                        OriginNo = cancelreceipt.InvoiceNumber,
                                        OffsetAccount = taxAcc.Code,
                                        Details = douTypeID.Name + " - " + taxAcc.Code,
                                        CumulativeBalance = taxAcc.Balance,
                                        Credit = taxValue,
                                        LocalSetRate = ard.LocalSetRate,
                                        GLAID = taxAcc.ID,
                                        Effective = EffectiveBlance.Credit
                                    });
                                }
                                _context.Update(taxAcc);
                                _context.SaveChanges();
                            }
                        }
                    }
                    var __ard = _context.ARDownPayments.Find(ard.ARDID) ?? new ARDownPayment();
                    __ard.Status = "used";
                    _context.ARDownPayments.Update(__ard);
                    _context.SaveChanges();
                }
            }
            if (glAcc.ID > 0)
            {
                journalEntryDetail.Add(
                new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Credit = (decimal)cancelreceipt.TotalAmountSys,
                    BPAcctID = cancelreceipt.CusID,
                }
            );
                //Insert 
                glAcc.Balance -= (decimal)cancelreceipt.TotalAmountSys;
                accountBalance.Add(
                    new AccountBalance
                    {
                        JEID = journalEntry.ID,

                        PostingDate = cancelreceipt.PostingDate,
                        Origin = docType.ID,
                        OriginNo = cancelreceipt.InvoiceNumber,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAcc.Code,
                        CumulativeBalance = glAcc.Balance,
                        Credit = (decimal)cancelreceipt.TotalAmountSys,

                        LocalSetRate = cancelreceipt.LocalCurID,
                        GLAID = accountReceive.GLAccID,
                        Creator = cancelreceipt.UserID,
                        BPAcctID = cancelreceipt.CusID,
                        Effective = EffectiveBlance.Credit
                    }
                );
                //      
                _context.Update(glAcc);
            }
            // return stock memo
            foreach (var item in receiptdetail)
            {
                // update_warehouse_summary && itemmasterdata
                int revenueAccID = 0;
                decimal revenueAccAmount = 0;
                List<ItemAccounting> itemAccs = new();
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && i.WarehouseID == cancelreceipt.WarehouseID);
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var baseUOM = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GUomID);
                if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == cancelreceipt.WarehouseID).ToList();
                    var revenueAcc = (from ia in itemAccs
                                      join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                        ).FirstOrDefault() ?? new GLAccount();

                    revenueAccID = revenueAcc.ID;
                    // inventoryAccID = inventoryAcc.ID;
                    // COGSAccID = COGSAcc.ID;
                    if (cancelreceipt.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                        revenueAccAmount = (decimal)item.TotalSys - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys;
                    }
                }
                else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID).ToList();
                    var revenueAcc = (from ia in itemAccs
                                      join gl in gLAccounts on ia.RevenueAccount equals gl.Code
                                      select gl
                                        ).FirstOrDefault() ?? new GLAccount();

                    revenueAccID = revenueAcc.ID;
                    // inventoryAccID = inventoryAcc.ID;
                    // COGSAccID = COGSAcc.ID;
                    if (cancelreceipt.DisRate > 0)
                    {
                        decimal disvalue = (decimal)item.TotalSys * (decimal)cancelreceipt.DisRate / 100;
                        revenueAccAmount = (decimal)item.TotalSys - disvalue;
                    }
                    else
                    {
                        revenueAccAmount = (decimal)item.TotalSys;
                    }
                }


                // Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = item.TaxOfFinDisValue * (decimal)cancelreceipt.ExchangeRate;
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

                            PostingDate = cancelreceipt.PostingDate,
                            Origin = docType.ID,
                            OriginNo = cancelreceipt.InvoiceNumber,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,

                            LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(taxAcc);
                }

                if (itemMaster.ManItemBy == ManageItemBy.None)
                {


                    // Account Revenue
                    var glAccRevenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == revenueAccID) ?? new GLAccount();
                    if (glAccRevenfifo.ID > 0)
                    {
                        var listRevenfifo = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccRevenfifo.ID) ?? new JournalEntryDetail();
                        if (listRevenfifo.ItemID > 0)
                        {
                            var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == revenueAccID) ?? new AccountBalance();
                            glAccRevenfifo.Balance += revenueAccAmount;
                            //journalEntryDetail
                            listRevenfifo.Debit = revenueAccAmount;
                            //accountBalance
                            accBalance.CumulativeBalance = glAccRevenfifo.Balance;
                            //accBalance.Debit = revenueAccAmount;
                            accBalance.Credit = revenueAccAmount;
                        }
                        else
                        {
                            glAccRevenfifo.Balance += revenueAccAmount;
                            journalEntryDetail.Add(new JournalEntryDetail
                            {
                                JEID = journalEntry.ID,
                                Type = Type.BPCode,
                                ItemID = revenueAccID,
                                Debit = revenueAccAmount,

                                BPAcctID = cancelreceipt.CusID
                            });
                            //
                            accountBalance.Add(new AccountBalance
                            {
                                JEID = journalEntry.ID,

                                PostingDate = cancelreceipt.PostingDate,
                                Origin = docType.ID,
                                OriginNo = cancelreceipt.InvoiceNumber,
                                OffsetAccount = glAcc.Code,
                                Details = douTypeID.Name + " - " + glAccRevenfifo.Code,
                                CumulativeBalance = glAccRevenfifo.Balance,
                                Debit = revenueAccAmount,

                                LocalSetRate = (decimal)cancelreceipt.LocalSetRate,
                                GLAID = revenueAccID,
                                Effective = EffectiveBlance.Debit
                            });
                        }
                    }

                }
            }



            // }


            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
    }
}
