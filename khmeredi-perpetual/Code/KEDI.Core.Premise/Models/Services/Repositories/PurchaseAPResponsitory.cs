using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.ClassCopy;
using CKBS.Models.ReportClass;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.Services.Purchase;
using CKBS.Models.Services.Administrator.Setup;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Repository;
using CKBS.Models.Services.Purchase;
using KEDI.Core.Premise.Models.Services.Administrator.Inventory;
using KEDI.Core.Helpers.Enumerations;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPurchaseAP
    {
        IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs(int ID);
        IEnumerable<ServiceMapItemMasterDataPurchasAP> FindItemBarcode(int WarehouseID, string Barcode);
        IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs_Detail(int warehouseid, string numebr);
        void GoodReceiptStock(int purchaseID, string Type, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases, FreightPurchase freight);
        void GoodReceiptStockBasic(int purchaseID, string Type, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases, FreightPurchase freight);

        void GoodReceiptStockAPReserve(int purchaseID, string Type, FreightPurchase freight);
        List<PurchaseReport> GetPurchaseAPReserves(int BranchID, int WarehouseID, int VendorID, string PostingDate, string DocumentDate, bool check);
        void CopyGRPOtoAP(int purchaseID, FreightPurchase freight);
        void CopyGRPOtoAPReserves(int purchaseID, FreightPurchase freight);
        IEnumerable<ReportPurchaseAP> GetReportPurchaseAPs(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliverDate, string Check);
        IEnumerable<Company> GetCurrencies();
        IEnumerable<ReportPurchaseOrder> GetAllPruchaseOrder(int BranchID);
        IEnumerable<ReportPurchaseAP> GetALlGoodReceiptPO(int BranchID);
        IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseOrder(int ID, string Number);
        IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseGoodReceipt(int ID, string Number);
        void IssuseCancelPurchaseAP(Purchase_AP purchase, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight);
    }
    public class PurchaseAPResponsitory : IPurchaseAP
    {
        private readonly DataContext _context;
        private readonly UtilityModule _utility;
        public PurchaseAPResponsitory(DataContext context, UtilityModule utility)
        {
            _context = context;
            _utility = utility;
        }
        public IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseGoodReceipt(int ID, string Number) => _context.PurchaseAP_To_PurchaseMemos.FromSql("sp_GetPurchaseAP_From_PurchaseGoodRecepitPO @PurhcasePOID={0},@Number={1}",
           parameters: new[] {
                  ID.ToString(),
                Number.ToString()

           });
        public IEnumerable<ServiceMapItemMasterDataPurchasAP> FindItemBarcode(int WarehouseID, string Barcode) => _context.ServiceMapItemMasterDataPurchasAPs.FromSql("sp_FindBarcodePurchaseAP @WarehouseID={0},@Barcode={1}",
           parameters: new[] {
               WarehouseID.ToString(),
               Barcode.ToString()
           });

        public IEnumerable<ReportPurchaseOrder> GetAllPruchaseOrder(int BranchID) => _context.ReportPurchaseOrders.FromSql("sp_GetAllPurchaeOrder @BranchID={0}",
            parameters: new[] {
                BranchID.ToString()
            });

        public IEnumerable<ReportPurchaseAP> GetALlGoodReceiptPO(int BranchID) => _context.ReportPurchaseAPs.FromSql("sp_GetAllGoodReceiptPO @BranchID={0}",
            parameters: new[] {
                BranchID.ToString()
                });

        public IEnumerable<Company> GetCurrencies()
        {
            IEnumerable<Company> list = (
                from pd in _context.PriceLists.Where(x => x.Delete == false)
                join com in _context.Company.Where(x => x.Delete == false) on
                pd.ID equals com.PriceListID
                join cur in _context.Currency.Where(x => x.Delete == false) on pd.CurrencyID equals cur.ID
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

        public IEnumerable<PurchaseAP_To_PurchaseMemo> GetPurchaseAP_From_PurchaseOrder(int ID, string Number) => _context.PurchaseAP_To_PurchaseMemos.FromSql("sp_GetPurchaseAP_form_PurchaseOrder @PurchaseOrderID={0},@Number={1}",
            parameters: new[] {
                ID.ToString(),
                Number.ToString()
            });

        public IEnumerable<ReportPurchaseAP> GetReportPurchaseAPs(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliverDate, string Check) => _context.ReportPurchaseAPs.FromSql("sp_ReportPurchaseAP @BranchID={0},@warehouseID={1},@PostingDate={2},@DeliveryDate={3},@DocumentDate={4},@Check={5}",
            parameters: new[] {
                BranchID.ToString(),
                WarehouseID.ToString(),
                PostingDate.ToString(),
                DocumentDate.ToString(),
                DeliverDate.ToString(),
                Check.ToString()
            });
        public void CopyGRPOtoAP(int purchaseID, FreightPurchase freight)
        {
            var ap = _context.Purchase_APs.Find(purchaseID) ?? new Purchase_AP();
            var ItemPO = _context.PurchaseAPDetail.Where(w => w.PurchaseAPID == purchaseID);
            var docType = _context.DocumentTypes.Find(ap.DocumentTypeID) ?? new DocumentType();
            var series = _context.SeriesDetails.Find(ap.SeriesDetailID);
            int allocateAccID = 0;
            decimal allocateAccAmount = 0;
            OutgoingPaymentVendor outgoingPaymentVendor = new();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE") ?? new DocumentType();
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
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
                journalEntry.Creator = ap.UserID;
                journalEntry.TransNo = Sno;
                journalEntry.PostingDate = ap.PostingDate;
                journalEntry.DocumentDate = ap.DocumentDate;
                journalEntry.DueDate = ap.DueDate;
                journalEntry.SSCID = ap.SysCurrencyID;
                journalEntry.LLCID = ap.LocalCurID;
                journalEntry.CompanyID = ap.CompanyID;
                journalEntry.LocalSetRate = (decimal)ap.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = defaultJE.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            // AccountReceice
            var accountPayable = _context.BusinessPartners.FirstOrDefault(w => w.ID == ap.VendorID) ?? new HumanResources.BusinessPartner();
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountPayable.GLAccID) ?? new GLAccount();

            List<JournalEntryDetail> journalEntryDetail = new()
            {
                new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountPayable.GLAccID,
                    Credit = (decimal)ap.BalanceDueSys,
                    BPAcctID = ap.VendorID,
                }
            };
            //Insert 
            glAcc.Balance -= (decimal)ap.BalanceDueSys;
            List<AccountBalance> accountBalance = new()
            {
                new AccountBalance
                {
                      JEID = journalEntry.ID,
                    PostingDate = ap.PostingDate,
                    Origin = docType.ID,
                    OriginNo = ap.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = (decimal)ap.BalanceDueSys,
                    LocalSetRate = (decimal)ap.LocalSetRate,
                    GLAID = accountPayable.GLAccID,
                    Creator = ap.UserID,
                    BPAcctID = ap.VendorID,
                    Effective=EffectiveBlance.Credit
                }
            };
            //  
            _context.Update(glAcc);
            _context.SaveChanges();

            #region
            // Freight //
            if (freight != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                    BPAcctID = ap.VendorID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.InvoiceNo,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount,
                                    BPAcctID = ap.VendorID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
            #endregion
            foreach (var itemdt in ItemPO)
            {
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    var allocateAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID)
                                       join gl in _context.GLAccounts on ia.AllocationAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();

                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }
                else if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    var allocateAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID)
                                       join gl in _context.GLAccounts on ia.AllocationAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();
                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }

                #region
                //// Tax Account ///
                var taxg = _context.TaxGroups.Find(itemdt.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = itemdt.TaxOfFinDisValue * (decimal)ap.PurRate;
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
                            Type = Financials.Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Debit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(taxAcc);
                }
                #endregion

                //inventoryAccID
                var glAccInven = _context.GLAccounts.FirstOrDefault(w => w.ID == allocateAccID) ?? new GLAccount();
                if (glAccInven.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == allocateAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccInven.Balance += allocateAccAmount;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == allocateAccID);
                        //journalEntryDetail
                        journalDetail.Debit += allocateAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInven.Balance;
                        accBalance.Debit += allocateAccAmount;
                    }
                    else
                    {
                        glAccInven.Balance += allocateAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = allocateAccID,
                            Debit = allocateAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccInven.Code,
                            CumulativeBalance = glAccInven.Balance,
                            Debit = allocateAccAmount,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = allocateAccID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(glAccInven);
                    _context.SaveChanges();
                }

            }
            //insert Outgoing payment
            outgoingPaymentVendor.TypePurchase = TypePurchase.AP;
            outgoingPaymentVendor.BalanceDue = ap.BalanceDue;
            outgoingPaymentVendor.PostingDate = ap.PostingDate;
            outgoingPaymentVendor.Date = ap.DueDate;
            outgoingPaymentVendor.OverdueDays = 0;
            outgoingPaymentVendor.Total = ap.BalanceDue;// ap.SubTotal;
            outgoingPaymentVendor.TotalPayment = ap.BalanceDue;// ap.SubTotal - ap.DiscountValue;
            outgoingPaymentVendor.BranchID = ap.BranchID;
            outgoingPaymentVendor.CurrencyID = ap.PurCurrencyID;
            outgoingPaymentVendor.VendorID = ap.VendorID;
            outgoingPaymentVendor.WarehouseID = ap.WarehouseID;
            outgoingPaymentVendor.Status = ap.Status;
            outgoingPaymentVendor.CashDiscount = ap.DiscountRate;
            outgoingPaymentVendor.TotalDiscount = ap.DiscountValue;
            outgoingPaymentVendor.Applied_Amount = ap.AppliedAmount;
            outgoingPaymentVendor.ExchangeRate = ap.PurRate;
            outgoingPaymentVendor.SysCurrency = ap.SysCurrencyID;
            outgoingPaymentVendor.LocalCurID = ap.LocalCurID;
            outgoingPaymentVendor.LocalSetRate = ap.LocalSetRate;
            outgoingPaymentVendor.CompanyID = ap.CompanyID;
            outgoingPaymentVendor.SeriesDetailID = ap.SeriesDetailID;
            outgoingPaymentVendor.DocumentID = ap.DocumentTypeID;
            outgoingPaymentVendor.Number = series.Number;
            outgoingPaymentVendor.ItemInvoice = $"{docType.Code}-{ap.Number}";
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.Update(outgoingPaymentVendor);
            _context.SaveChanges();
        }
        public void CopyGRPOtoAPReserves(int purchaseID, FreightPurchase freight)
        {
            var ap = _context.PurchaseAPReserves.Find(purchaseID) ?? new PurchaseAPReserve();
            var ItemPO = _context.PurchaseAPReserveDetails.Where(w => w.PurchaseAPReserveID == purchaseID);
            var docType = _context.DocumentTypes.Find(ap.DocumentTypeID) ?? new DocumentType();
            var series = _context.SeriesDetails.Find(ap.SeriesDetailID);
            int allocateAccID = 0;
            decimal allocateAccAmount = 0;
            OutgoingPaymentVendor outgoingPaymentVendor = new();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
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
                journalEntry.Creator = ap.UserID;
                journalEntry.TransNo = Sno;
                journalEntry.PostingDate = ap.PostingDate;
                journalEntry.DocumentDate = ap.DocumentDate;
                journalEntry.DueDate = ap.DueDate;
                journalEntry.SSCID = ap.SysCurrencyID;
                journalEntry.LLCID = ap.LocalCurID;
                journalEntry.CompanyID = ap.CompanyID;
                journalEntry.LocalSetRate = (decimal)ap.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = defaultJE.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            // AccountReceice
            var accountPayable = _context.BusinessPartners.FirstOrDefault(w => w.ID == ap.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountPayable.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new()
            {
                new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountPayable.GLAccID,
                    Credit = (decimal)ap.BalanceDueSys,
                    BPAcctID = ap.VendorID,
                }
            };
            //Insert 
            glAcc.Balance -= (decimal)ap.BalanceDueSys;
            List<AccountBalance> accountBalance = new()
            {
                new AccountBalance
                {
                      JEID = journalEntry.ID,
                    PostingDate = ap.PostingDate,
                    Origin = docType.ID,
                    OriginNo = ap.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = (decimal)ap.BalanceDueSys,
                    LocalSetRate = (decimal)ap.LocalSetRate,
                    GLAID = accountPayable.GLAccID,
                    Creator = ap.UserID,
                    BPAcctID = ap.VendorID,
                    Effective=EffectiveBlance.Credit
                }
            };
            //  
            _context.Update(glAcc);
            _context.SaveChanges();

            #region
            // Freight //
            if (freight != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                    BPAcctID = ap.VendorID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.InvoiceNo,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount,
                                    BPAcctID = ap.VendorID
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
            #endregion
            foreach (var itemdt in ItemPO)
            {
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    var allocateAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID)
                                       join gl in _context.GLAccounts on ia.AllocationAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();

                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }
                else if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    var allocateAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID)
                                       join gl in _context.GLAccounts on ia.AllocationAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();
                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }

                #region
                //// Tax Account ///
                var taxg = _context.TaxGroups.Find(itemdt.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = itemdt.TaxOfFinDisValue * (decimal)ap.PurRate;
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
                            Type = Financials.Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Debit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(taxAcc);
                }
                #endregion

                //inventoryAccID
                var glAccInven = _context.GLAccounts.FirstOrDefault(w => w.ID == allocateAccID) ?? new GLAccount();
                if (glAccInven.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == allocateAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccInven.Balance += allocateAccAmount;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == allocateAccID);
                        //journalEntryDetail
                        journalDetail.Debit += allocateAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInven.Balance;
                        accBalance.Debit += allocateAccAmount;
                    }
                    else
                    {
                        glAccInven.Balance += allocateAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = allocateAccID,
                            Debit = allocateAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccInven.Code,
                            CumulativeBalance = glAccInven.Balance,
                            Debit = allocateAccAmount,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = allocateAccID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(glAccInven);
                    _context.SaveChanges();
                }

            }
            //insert Outgoing payment
            outgoingPaymentVendor.TypePurchase = TypePurchase.APReserve;
            outgoingPaymentVendor.BalanceDue = ap.BalanceDue;
            outgoingPaymentVendor.PostingDate = ap.PostingDate;
            outgoingPaymentVendor.Date = ap.DueDate;
            outgoingPaymentVendor.OverdueDays = 0;
            outgoingPaymentVendor.Total = (double)ap.SubTotalAfterDisSys;// ap.SubTotal;
            outgoingPaymentVendor.TotalPayment = ap.BalanceDue;// ap.SubTotal - ap.DiscountValue;
            outgoingPaymentVendor.BranchID = ap.BranchID;
            outgoingPaymentVendor.CurrencyID = ap.PurCurrencyID;
            outgoingPaymentVendor.VendorID = ap.VendorID;
            outgoingPaymentVendor.WarehouseID = ap.WarehouseID;
            outgoingPaymentVendor.Status = ap.Status;
            outgoingPaymentVendor.CashDiscount = ap.DiscountRate;
            outgoingPaymentVendor.TotalDiscount = ap.DiscountValue;
            outgoingPaymentVendor.Applied_Amount = ap.AppliedAmount;
            outgoingPaymentVendor.ExchangeRate = ap.PurRate;
            outgoingPaymentVendor.SysCurrency = ap.SysCurrencyID;
            outgoingPaymentVendor.LocalCurID = ap.LocalCurID;
            outgoingPaymentVendor.LocalSetRate = ap.LocalSetRate;
            outgoingPaymentVendor.CompanyID = ap.CompanyID;
            outgoingPaymentVendor.SeriesDetailID = ap.SeriesDetailID;
            outgoingPaymentVendor.DocumentID = ap.DocumentTypeID;
            outgoingPaymentVendor.Number = series.Number;
            outgoingPaymentVendor.ItemInvoice = $"{docType.Code}-{ap.Number}";
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.Update(outgoingPaymentVendor);
            _context.SaveChanges();
        }
        #region GoodReceiptStockBasic
        public void GoodReceiptStockBasic(int purchaseID, string Type, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases, FreightPurchase freight)
        {
            var ap = _context.Purchase_APs.Find(purchaseID);
            var ItemPO = _context.PurchaseAPDetail.Where(w => w.PurchaseAPID == purchaseID).ToList();
            OutgoingPaymentVendor outgoingPaymentVendor = new();
            var docType = _context.DocumentTypes.Find(ap.DocumentTypeID);
            var series = _context.Series.Find(ap.SeriesID);
            //insert inventoryaudit
            foreach (var itemdt in ItemPO)
            {
                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                //var ap = _context.Purchase_APs.FirstOrDefault(w => w.PurchaseAPID == itemdt.PurchaseAPID);
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemmaster.GroupUomID && W.AltUOM == itemdt.UomID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemmaster.GroupUomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == ap.WarehouseID);
                List<ItemAccounting> itemAccs = new();
                double @Qty = itemdt.Qty * gd.Factor;
                double _cost = (itemdt.PurchasPrice / gd.Factor) * ap.PurRate;
                InventoryAudit item_inventory_audit = new();
                WarehouseDetail warehousedetail = new();
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                if (Type == "PO")
                {
                    //update itmemasterdata
                    itemmaster.StockOnHand -= @Qty;
                    itemmaster.StockIn += @Qty;
                    //update warehouse
                    warehouse.Ordered -= @Qty;
                    warehouse.InStock += @Qty;

                    // update itmemasterdata                    
                    itemmaster.StockIn += @Qty;
                    //update warehouse                    
                    warehouse.InStock += @Qty;
                    _utility.UpdateItemAccounting(_itemAcc, warehouse);

                    _context.ItemMasterDatas.Update(itemmaster);
                    _context.WarehouseSummary.Update(warehouse);
                }
                else if (Type == "AP")
                {
                    // update itmemasterdata                    
                    itemmaster.StockIn += @Qty;
                    //update warehouse                    
                    warehouse.InStock += @Qty;
                    _utility.UpdateItemAccounting(_itemAcc, warehouse);
                    _context.ItemMasterDatas.Update(itemmaster);
                    _context.WarehouseSummary.Update(warehouse);
                }
                //insert warehousedetail
                if (itemmaster.ManItemBy == ManageItemBy.SerialNumbers && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var svmp = serialViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID);
                    List<WarehouseDetail> whsDetials = new();
                    List<InventoryAudit> inventoryAudit = new();
                    if (svmp != null)
                    {
                        foreach (var sv in svmp.SerialDetialViewModelPurchase.Where(i => !string.IsNullOrEmpty(i.SerialNumber)).ToList())
                        {
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = sv.AdmissionDate,
                                Cost = _cost,
                                CurrencyID = ap.PurCurrencyID,
                                Details = sv.Detials,
                                ID = 0,
                                InStock = 1,
                                ItemID = itemdt.ItemID,
                                Location = sv.Location,
                                LotNumber = sv.LotNumber,
                                MfrDate = sv.MfrDate,
                                MfrSerialNumber = sv.MfrSerialNo,
                                MfrWarDateEnd = sv.MfrWarrantyEnd,
                                MfrWarDateStart = sv.MfrWarrantyStart,
                                ProcessItem = ProcessItem.SEBA,
                                SerialNumber = sv.SerialNumber,
                                PlateNumber = sv.PlateNumber,
                                Color = sv.Color,
                                Brand = sv.Brand,
                                Condition = sv.Condition,
                                Type = sv.Type,
                                Power = sv.Power,
                                Year = sv.Year,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = ap.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = ap.UserID,
                                ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                TransType = TransTypeWD.PurAP,
                                BPID = ap.VendorID,
                                IsDeleted = true,
                                InStockFrom = ap.PurchaseAPID,
                                BaseOnID = itemdt.PurchaseDetailAPID,
                                PurCopyType = PurCopyType.PurAP,
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == ap.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = ap.WarehouseID;
                        invAudit.BranchID = ap.BranchID;
                        invAudit.UserID = ap.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = ap.SysCurrencyID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = ap.InvoiceNo;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = ap.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = _cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        invAudit.Trans_Valuse = (@Qty * _cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = ap.LocalCurID;
                        invAudit.LocalSetRate = ap.LocalSetRate;
                        invAudit.DocumentTypeID = ap.DocumentTypeID;
                        invAudit.CompanyID = ap.CompanyID;
                        invAudit.SeriesID = ap.SeriesID;
                        invAudit.SeriesDetailID = ap.SeriesDetailID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else if (itemmaster.ManItemBy == ManageItemBy.Batches && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID);
                    List<WarehouseDetail> whsDetials = new();

                    if (bvmp != null)
                    {
                        var bvs = bvmp.BatchDetialViewModelPurchases.Where(i => !string.IsNullOrEmpty(i.Batch) && i.Qty > 0).ToList();
                        foreach (var bv in bvs)
                        {
                            var _qty = (double)bv.Qty;
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = bv.AdmissionDate,
                                Cost = _cost,
                                CurrencyID = ap.PurCurrencyID,
                                Details = bv.Detials,
                                ID = 0,
                                InStock = _qty,
                                ItemID = itemdt.ItemID,
                                Location = bv.Location,
                                MfrDate = bv.MfrDate,
                                ProcessItem = ProcessItem.SEBA,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = ap.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = ap.UserID,
                                ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                BatchAttr1 = bv.BatchAttribute1,
                                BatchAttr2 = bv.BatchAttribute2,
                                BatchNo = bv.Batch,
                                TransType = TransTypeWD.PurAP,
                                BPID = ap.VendorID,
                                InStockFrom = ap.PurchaseAPID,
                                IsDeleted = true,
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == ap.WarehouseID)
                            .ToList();
                        // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = ap.WarehouseID;
                        invAudit.BranchID = ap.BranchID;
                        invAudit.UserID = ap.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = ap.SysCurrencyID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = ap.InvoiceNo;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = ap.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = _cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        invAudit.Trans_Valuse = (@Qty * _cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = ap.LocalCurID;
                        invAudit.LocalSetRate = ap.LocalSetRate;
                        invAudit.DocumentTypeID = ap.DocumentTypeID;
                        invAudit.CompanyID = ap.CompanyID;
                        invAudit.SeriesID = ap.SeriesID;
                        invAudit.SeriesDetailID = ap.SeriesDetailID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    warehousedetail.ID = 0;
                    warehousedetail.WarehouseID = ap.WarehouseID;
                    warehousedetail.UserID = ap.UserID;
                    warehousedetail.UomID = itemdt.UomID;
                    warehousedetail.SyetemDate = DateTime.Now;
                    warehousedetail.TimeIn = DateTime.Now;
                    warehousedetail.InStock = @Qty;
                    warehousedetail.CurrencyID = ap.SysCurrencyID;
                    warehousedetail.ExpireDate = itemdt.ExpireDate;
                    warehousedetail.ItemID = itemdt.ItemID;
                    warehousedetail.Cost = _cost;
                    warehousedetail.IsDeleted = true;
                    warehousedetail.BPID = ap.VendorID;
                    warehousedetail.TransType = TransTypeWD.PurAP;
                    warehousedetail.InStockFrom = ap.PurchaseAPID;
                    _context.WarehouseDetails.Add(warehousedetail);
                    _context.SaveChanges();
                    if (itemmaster.Process == "FIFO")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = ap.WarehouseID;
                        item_inventory_audit.BranchID = ap.BranchID;
                        item_inventory_audit.UserID = ap.UserID;
                        item_inventory_audit.ItemID = itemdt.ItemID;
                        item_inventory_audit.CurrencyID = ap.SysCurrencyID;
                        item_inventory_audit.UomID = orft.BaseUOM;
                        item_inventory_audit.InvoiceNo = ap.InvoiceNo;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemmaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = ap.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty;
                        item_inventory_audit.Cost = _cost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        item_inventory_audit.Trans_Valuse = (@Qty * _cost);
                        item_inventory_audit.ExpireDate = itemdt.ExpireDate;
                        item_inventory_audit.LocalCurID = ap.LocalCurID;
                        item_inventory_audit.LocalSetRate = ap.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = ap.SeriesDetailID;
                        item_inventory_audit.SeriesID = ap.SeriesID;
                        item_inventory_audit.DocumentTypeID = ap.DocumentTypeID;
                        item_inventory_audit.CompanyID = ap.CompanyID;
                        //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                        // update pricelistdetial
                        foreach (var pri in pri_detial)
                        {
                            var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == itemmaster.GroupUomID && g.AltUOM == pri.UomID);
                            var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                            pri.Cost = _cost * exp.SetRate * guom.Factor;
                            _context.PriceListDetails.Update(pri);
                        }
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }
                    else if (itemmaster.Process == "Average")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        InventoryAudit avgInAudit = new() { Qty = @Qty, Cost = _cost };
                        double @AvgCost = _utility.CalAVGCost(itemdt.ItemID, ap.WarehouseID, avgInAudit);
                        @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = ap.WarehouseID;
                        item_inventory_audit.BranchID = ap.BranchID;
                        item_inventory_audit.UserID = ap.UserID;
                        item_inventory_audit.ItemID = itemdt.ItemID;
                        item_inventory_audit.CurrencyID = ap.SysCurrencyID;
                        item_inventory_audit.UomID = orft.BaseUOM;
                        item_inventory_audit.InvoiceNo = ap.InvoiceNo;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemmaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = ap.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty;
                        item_inventory_audit.Cost = @AvgCost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (Qty * _cost);
                        item_inventory_audit.Trans_Valuse = (@Qty * _cost);
                        item_inventory_audit.ExpireDate = itemdt.ExpireDate;
                        item_inventory_audit.LocalCurID = ap.LocalCurID;
                        item_inventory_audit.LocalSetRate = ap.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = ap.SeriesDetailID;
                        item_inventory_audit.SeriesID = ap.SeriesID;
                        item_inventory_audit.DocumentTypeID = ap.DocumentTypeID;
                        item_inventory_audit.CompanyID = ap.CompanyID;
                        //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                        // update_warehouse_summary
                        warehouse_sammary.Cost = @AvgCost;
                        _context.WarehouseSummary.Update(warehouse_sammary);
                        // update_pricelistdetial
                        var inventory_pricelist = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID).ToList();
                        double @AvgCostPL = inventory_pricelist.Sum(s => s.Trans_Valuse) / inventory_pricelist.Sum(q => q.Qty);
                        @AvgCostPL = _utility.CheckNaNOrInfinity(@AvgCostPL);
                        foreach (var pri in pri_detial)
                        {
                            var guom = _context.GroupDUoMs.Where(g => g.GroupUoMID == itemmaster.GroupUomID && g.AltUOM == pri.UomID);
                            var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                            foreach (var g in guom)
                            {
                                pri.Cost = @AvgCostPL * exp.SetRate * g.Factor;
                            }
                            _context.PriceListDetails.Update(pri);
                        }
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }
                }

            }

            //update bom
            foreach (var item in ItemPO)
            {
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = Inven.Sum(s => s.Trans_Valuse) / Inven.Sum(q => q.Qty);
                    @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                    var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                    var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                    itembom.Cost = @AvgCost * Factor;
                    itembom.Amount = itembom.Qty * (@AvgCost * Factor);
                    _context.BOMDetail.UpdateRange(ItemBOMDetail);
                    _context.SaveChanges();
                    BOM.TotalCost = DBOM.Sum(w => w.Amount);
                    _context.BOMaterial.Update(BOM);
                    _context.SaveChanges();
                }
            }
            //insert Outgoing payment
            outgoingPaymentVendor.TypePurchase = TypePurchase.AP;
            outgoingPaymentVendor.BalanceDue = ap.BalanceDue;
            outgoingPaymentVendor.PostingDate = ap.PostingDate;
            outgoingPaymentVendor.Date = ap.DueDate;
            outgoingPaymentVendor.OverdueDays = 0;
            outgoingPaymentVendor.Total = (double)ap.SubTotalAfterDisSys;//ap.BalanceDue;//ap.SubTotal;
            outgoingPaymentVendor.TotalPayment = ap.BalanceDue;// ap.SubTotal - ap.DiscountValue;
            outgoingPaymentVendor.BranchID = ap.BranchID;
            outgoingPaymentVendor.CurrencyID = ap.PurCurrencyID;
            outgoingPaymentVendor.VendorID = ap.VendorID;
            outgoingPaymentVendor.WarehouseID = ap.WarehouseID;
            outgoingPaymentVendor.Status = ap.Status;
            outgoingPaymentVendor.CashDiscount = ap.DiscountRate;
            outgoingPaymentVendor.TotalDiscount = ap.DiscountValue;
            outgoingPaymentVendor.Applied_Amount = ap.AppliedAmount;
            outgoingPaymentVendor.ExchangeRate = ap.PurRate;
            outgoingPaymentVendor.SysCurrency = ap.SysCurrencyID;
            outgoingPaymentVendor.LocalCurID = ap.LocalCurID;
            outgoingPaymentVendor.LocalSetRate = ap.LocalSetRate;
            outgoingPaymentVendor.CompanyID = ap.CompanyID;
            outgoingPaymentVendor.SeriesDetailID = ap.SeriesDetailID;
            outgoingPaymentVendor.DocumentID = ap.DocumentTypeID;
            outgoingPaymentVendor.Number = ap.Number;
            outgoingPaymentVendor.ItemInvoice = $"{docType.Code}-{ap.Number}";
            _context.Update(outgoingPaymentVendor);
            _context.SaveChanges();
        }

        #endregion GoodReceiptStockBasic
        #region  GoodReceiptStock
        public void GoodReceiptStock(int purchaseID, string Type, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases, FreightPurchase freight)
        {
            var ap = _context.Purchase_APs.Find(purchaseID);
            var ItemPO = _context.PurchaseAPDetail.Where(w => w.PurchaseAPID == purchaseID).ToList();
            OutgoingPaymentVendor outgoingPaymentVendor = new();
            var docType = _context.DocumentTypes.Find(ap.DocumentTypeID);
            var series = _context.Series.Find(ap.SeriesID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            int inventoryAccID = 0, allocateAccID = 0;
            decimal inventoryAccAmount = 0, allocateAccAmount = 0;

            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
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
                journalEntry.Creator = ap.UserID;
                journalEntry.BranchID = ap.BranchID;
                journalEntry.TransNo = ap.Number;
                journalEntry.PostingDate = ap.PostingDate;
                journalEntry.DocumentDate = ap.DocumentDate;
                journalEntry.DueDate = ap.DueDate;
                journalEntry.SSCID = ap.SysCurrencyID;
                journalEntry.LLCID = ap.LocalCurID;
                journalEntry.CompanyID = ap.CompanyID;
                journalEntry.LocalSetRate = (decimal)ap.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            // AccountReceice
            var accountPayable = _context.BusinessPartners.FirstOrDefault(w => w.ID == ap.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountPayable.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new()
            {
                new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountPayable.GLAccID,
                    Credit = (decimal)ap.BalanceDueSys,
                    BPAcctID = ap.VendorID,
                }
            };
            //Insert 
            glAcc.Balance -= (decimal)ap.BalanceDueSys;
            List<AccountBalance> accountBalance = new()
            {
                new AccountBalance
                {
                    JEID = journalEntry.ID,

                    PostingDate = ap.PostingDate,
                    Origin = docType.ID,
                    OriginNo = ap.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = (decimal)ap.BalanceDueSys,
                    LocalSetRate = (decimal)ap.LocalSetRate,
                    GLAID = accountPayable.GLAccID,
                    BPAcctID = ap.VendorID,
                    Creator = ap.UserID,
                    Effective=EffectiveBlance.Credit
                }
            };
            #region
            // Freight //
            if (freight != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.InvoiceNo,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,

                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
            #endregion
            _context.Update(glAcc);
            _context.SaveChanges();
            //insert inventoryaudit
            foreach (var itemdt in ItemPO)
            {
                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                //var ap = _context.Purchase_APs.FirstOrDefault(w => w.PurchaseAPID == itemdt.PurchaseAPID);
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemmaster.GroupUomID && W.AltUOM == itemdt.UomID);
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemmaster.GroupUomID);
                var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == ap.WarehouseID);
                List<ItemAccounting> itemAccs = new();
                double @Qty = itemdt.Qty * gd.Factor;
                double _cost = (itemdt.PurchasPrice / gd.Factor) * ap.PurRate;
                InventoryAudit item_inventory_audit = new();
                WarehouseDetail warehousedetail = new();
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID).ToList();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    var allocateAcc = (from ia in itemAccs
                                       join gl in gLAccounts on ia.ExpenseAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();
                    if (inventoryAcc != null)
                    {
                        inventoryAccID = inventoryAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            inventoryAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            inventoryAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }
                else if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID).ToList();
                    var inventoryAcc = (from ia in itemAccs
                                        join gl in gLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    var allocateAcc = (from ia in itemAccs
                                       join gl in gLAccounts on ia.ExpenseAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();
                    if (inventoryAcc != null)
                    {
                        inventoryAccID = inventoryAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            inventoryAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            inventoryAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }
                if (Type == "PO")
                {
                    //update itmemasterdata
                    itemmaster.StockOnHand -= @Qty;
                    itemmaster.StockIn += @Qty;
                    //update warehouse
                    warehouse.Ordered -= @Qty;
                    warehouse.InStock += @Qty;

                    // update itmemasterdata                    
                    itemmaster.StockIn += @Qty;
                    _utility.UpdateItemAccounting(_itemAcc, warehouse);

                    //inventoryAccID
                    var glAccInven = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == inventoryAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccInven.Balance += inventoryAccAmount;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInven.Balance;
                        accBalance.Debit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInven.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = inventoryAccAmount,

                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + " - " + glAccInven.Code,
                            CumulativeBalance = glAccInven.Balance,
                            Debit = inventoryAccAmount,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Debit
                        });
                        _context.Update(glAccInven);
                        _context.SaveChanges();
                    }
                    _context.ItemMasterDatas.Update(itemmaster);
                    _context.WarehouseSummary.Update(warehouse);
                }
                else if (Type == "AP")
                {
                    // update itmemasterdata                    
                    itemmaster.StockIn += @Qty;
                    //update warehouse                    
                    warehouse.InStock += @Qty;
                    _utility.UpdateItemAccounting(_itemAcc, warehouse);
                    //inventoryAccID
                    var glAccInven = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                    if (glAccInven.Code == null && glAccInven.Name == null)
                    {
                        glAccInven.Code = "";
                        glAccInven.Name = "";
                    }
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == inventoryAccID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        glAccInven.Balance += inventoryAccAmount;
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        //journalEntryDetail
                        journalDetail.Debit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInven.Balance;
                        accBalance.Debit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInven.Balance += inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Debit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + "-" + glAccInven.Code,
                            CumulativeBalance = glAccInven.Balance,
                            Debit = inventoryAccAmount,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Debit
                        });

                        _context.Update(glAccInven);
                        _context.SaveChanges();
                    }
                    _context.ItemMasterDatas.Update(itemmaster);
                    _context.WarehouseSummary.Update(warehouse);
                }
                //insert warehousedetail
                if (itemmaster.ManItemBy == ManageItemBy.SerialNumbers && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var svmp = serialViewModelPurchases.FirstOrDefault(s => s.LineID == itemdt.LineIDUN);
                    List<WarehouseDetail> whsDetials = new();
                    List<InventoryAudit> inventoryAudit = new();
                    if (svmp != null)
                    {
                        foreach (var sv in svmp.SerialDetialViewModelPurchase.Where(i => i.LineMID == svmp.LineID && !string.IsNullOrEmpty(i.SerialNumber)).ToList())
                        {
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = sv.AdmissionDate,
                                Cost = _cost,
                                CurrencyID = ap.PurCurrencyID,
                                Details = sv.Detials,
                                ID = 0,
                                InStock = 1,
                                ItemID = itemdt.ItemID,
                                Location = sv.Location,
                                LotNumber = sv.LotNumber,
                                MfrDate = sv.MfrDate,
                                MfrSerialNumber = sv.MfrSerialNo,
                                MfrWarDateEnd = sv.MfrWarrantyEnd,
                                MfrWarDateStart = sv.MfrWarrantyStart,
                                ProcessItem = ProcessItem.SEBA,
                                SerialNumber = sv.SerialNumber,
                                PlateNumber = sv.PlateNumber,
                                Color = sv.Color,
                                Brand = sv.Brand,
                                Condition = sv.Condition,
                                Type = sv.Type,
                                Power = sv.Power,
                                Year = sv.Year,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = ap.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = ap.UserID,
                                ExpireDate = sv.ExpirationDate == null ? default : (DateTime)sv.ExpirationDate,
                                TransType = TransTypeWD.PurAP,
                                BPID = ap.VendorID,
                                IsDeleted = true,
                                InStockFrom = ap.PurchaseAPID,
                                BaseOnID = itemdt.PurchaseDetailAPID,
                                PurCopyType = PurCopyType.PurAP,
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == ap.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = ap.WarehouseID;
                        invAudit.BranchID = ap.BranchID;
                        invAudit.UserID = ap.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = ap.SysCurrencyID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = ap.InvoiceNo;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = ap.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = _cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        invAudit.Trans_Valuse = (@Qty * _cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = ap.LocalCurID;
                        invAudit.LocalSetRate = ap.LocalSetRate;
                        invAudit.DocumentTypeID = ap.DocumentTypeID;
                        invAudit.CompanyID = ap.CompanyID;
                        invAudit.SeriesID = ap.SeriesID;
                        invAudit.SeriesDetailID = ap.SeriesDetailID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else if (itemmaster.ManItemBy == ManageItemBy.Batches && itemmaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    var bvmp = batchViewModelPurchases.FirstOrDefault(s => s.ItemID == itemdt.ItemID);
                    List<WarehouseDetail> whsDetials = new();

                    if (bvmp != null)
                    {
                        var bvs = bvmp.BatchDetialViewModelPurchases.Where(i => !string.IsNullOrEmpty(i.Batch) && i.Qty > 0).ToList();
                        foreach (var bv in bvs)
                        {
                            var _qty = (double)bv.Qty;
                            whsDetials.Add(new WarehouseDetail
                            {
                                AdmissionDate = bv.AdmissionDate,
                                Cost = _cost,
                                CurrencyID = ap.PurCurrencyID,
                                Details = bv.Detials,
                                ID = 0,
                                InStock = _qty,
                                ItemID = itemdt.ItemID,
                                Location = bv.Location,
                                MfrDate = bv.MfrDate,
                                ProcessItem = ProcessItem.SEBA,
                                SyetemDate = DateTime.Now,
                                SysNum = 0,
                                TimeIn = DateTime.Now,
                                WarehouseID = ap.WarehouseID,
                                UomID = itemdt.UomID,
                                UserID = ap.UserID,
                                ExpireDate = bv.ExpirationDate == null ? default : (DateTime)bv.ExpirationDate,
                                BatchAttr1 = bv.BatchAttribute1,
                                BatchAttr2 = bv.BatchAttribute2,
                                BatchNo = bv.Batch,
                                TransType = TransTypeWD.PurAP,
                                BPID = ap.VendorID,
                                InStockFrom = ap.PurchaseAPID,
                                IsDeleted = true,
                            });
                        }
                        //insert inventoryaudit
                        InventoryAudit invAudit = new();
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID && w.WarehouseID == ap.WarehouseID)
                            .ToList();
                        // var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID && w.UomID == itemdt.UomID);
                        invAudit.ID = 0;
                        invAudit.WarehouseID = ap.WarehouseID;
                        invAudit.BranchID = ap.BranchID;
                        invAudit.UserID = ap.UserID;
                        invAudit.ItemID = itemdt.ItemID;
                        invAudit.CurrencyID = ap.SysCurrencyID;
                        invAudit.UomID = itemdt.UomID;
                        invAudit.InvoiceNo = ap.InvoiceNo;
                        invAudit.Trans_Type = docType.Code;
                        invAudit.Process = itemmaster.Process;
                        invAudit.SystemDate = DateTime.Now;
                        invAudit.PostingDate = ap.PostingDate;
                        invAudit.TimeIn = DateTime.Now.ToString("h:mm:ss tt");
                        invAudit.Qty = @Qty;
                        invAudit.Cost = _cost;
                        invAudit.Price = 0;
                        invAudit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        invAudit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        invAudit.Trans_Valuse = (@Qty * _cost);
                        invAudit.ExpireDate = itemdt.ExpireDate;
                        invAudit.LocalCurID = ap.LocalCurID;
                        invAudit.LocalSetRate = ap.LocalSetRate;
                        invAudit.DocumentTypeID = ap.DocumentTypeID;
                        invAudit.CompanyID = ap.CompanyID;
                        invAudit.SeriesID = ap.SeriesID;
                        invAudit.SeriesDetailID = ap.SeriesDetailID;
                        // update pricelistdetial
                        _utility.CumulativeValue(invAudit.WarehouseID, invAudit.ItemID, invAudit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(invAudit);
                        _context.WarehouseDetails.AddRange(whsDetials);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    warehousedetail.ID = 0;
                    warehousedetail.WarehouseID = ap.WarehouseID;
                    warehousedetail.UserID = ap.UserID;
                    warehousedetail.UomID = itemdt.UomID;
                    warehousedetail.SyetemDate = DateTime.Now;
                    warehousedetail.TimeIn = DateTime.Now;
                    warehousedetail.InStock = @Qty;
                    warehousedetail.CurrencyID = ap.SysCurrencyID;
                    warehousedetail.ExpireDate = itemdt.ExpireDate;
                    warehousedetail.ItemID = itemdt.ItemID;
                    warehousedetail.Cost = _cost;
                    warehousedetail.IsDeleted = true;
                    warehousedetail.BPID = ap.VendorID;
                    warehousedetail.TransType = TransTypeWD.PurAP;
                    warehousedetail.InStockFrom = ap.PurchaseAPID;
                    _context.WarehouseDetails.Add(warehousedetail);
                    _context.SaveChanges();
                    if (itemmaster.Process == "FIFO")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = ap.WarehouseID;
                        item_inventory_audit.BranchID = ap.BranchID;
                        item_inventory_audit.UserID = ap.UserID;
                        item_inventory_audit.ItemID = itemdt.ItemID;
                        item_inventory_audit.CurrencyID = ap.SysCurrencyID;
                        item_inventory_audit.UomID = orft.BaseUOM;
                        item_inventory_audit.InvoiceNo = ap.InvoiceNo;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemmaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = ap.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty;
                        item_inventory_audit.Cost = _cost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (@Qty * _cost);
                        item_inventory_audit.Trans_Valuse = (@Qty * _cost);
                        item_inventory_audit.ExpireDate = itemdt.ExpireDate;
                        item_inventory_audit.LocalCurID = ap.LocalCurID;
                        item_inventory_audit.LocalSetRate = ap.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = ap.SeriesDetailID;
                        item_inventory_audit.SeriesID = ap.SeriesID;
                        item_inventory_audit.DocumentTypeID = ap.DocumentTypeID;
                        item_inventory_audit.CompanyID = ap.CompanyID;
                        //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                        // update pricelistdetial
                        foreach (var pri in pri_detial)
                        {
                            var guom = _context.GroupDUoMs.FirstOrDefault(g => g.GroupUoMID == itemmaster.GroupUomID && g.AltUOM == pri.UomID);
                            var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                            pri.Cost = _cost * exp.SetRate * guom.Factor;
                            _context.PriceListDetails.Update(pri);
                        }
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }
                    else if (itemmaster.Process == "Average")
                    {
                        //insert inventoryaudit
                        var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                        var warehouse_sammary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID);
                        var pri_detial = _context.PriceListDetails.Where(w => w.ItemID == itemdt.ItemID);
                        InventoryAudit avgInAudit = new() { Qty = @Qty, Cost = _cost };
                        double @AvgCost = _utility.CalAVGCost(itemdt.ItemID, ap.WarehouseID, avgInAudit);
                        @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                        item_inventory_audit.ID = 0;
                        item_inventory_audit.WarehouseID = ap.WarehouseID;
                        item_inventory_audit.BranchID = ap.BranchID;
                        item_inventory_audit.UserID = ap.UserID;
                        item_inventory_audit.ItemID = itemdt.ItemID;
                        item_inventory_audit.CurrencyID = ap.SysCurrencyID;
                        item_inventory_audit.UomID = orft.BaseUOM;
                        item_inventory_audit.InvoiceNo = ap.InvoiceNo;
                        item_inventory_audit.Trans_Type = docType.Code;
                        item_inventory_audit.Process = itemmaster.Process;
                        item_inventory_audit.SystemDate = DateTime.Now;
                        item_inventory_audit.PostingDate = ap.PostingDate;
                        item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString();
                        item_inventory_audit.Qty = @Qty;
                        item_inventory_audit.Cost = @AvgCost;
                        item_inventory_audit.Price = 0;
                        item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) + @Qty;
                        item_inventory_audit.CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) + (Qty * _cost);
                        item_inventory_audit.Trans_Valuse = (@Qty * _cost);
                        item_inventory_audit.ExpireDate = itemdt.ExpireDate;
                        item_inventory_audit.LocalCurID = ap.LocalCurID;
                        item_inventory_audit.LocalSetRate = ap.LocalSetRate;
                        item_inventory_audit.SeriesDetailID = ap.SeriesDetailID;
                        item_inventory_audit.SeriesID = ap.SeriesID;
                        item_inventory_audit.DocumentTypeID = ap.DocumentTypeID;
                        item_inventory_audit.CompanyID = ap.CompanyID;
                        //inventoryAccAmount = (decimal)item_inventory_audit.Cost;
                        // update_warehouse_summary
                        warehouse_sammary.Cost = @AvgCost;
                        _context.WarehouseSummary.Update(warehouse_sammary);
                        // update_pricelistdetial
                        var inventory_pricelist = _context.InventoryAudits.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID).ToList();
                        double @AvgCostPL = inventory_pricelist.Sum(s => s.Trans_Valuse) / inventory_pricelist.Sum(q => q.Qty);
                        @AvgCostPL = _utility.CheckNaNOrInfinity(@AvgCostPL);
                        foreach (var pri in pri_detial)
                        {
                            var guom = _context.GroupDUoMs.Where(g => g.GroupUoMID == itemmaster.GroupUomID && g.AltUOM == pri.UomID);
                            var exp = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == pri.CurrencyID);
                            foreach (var g in guom)
                            {
                                pri.Cost = @AvgCostPL * exp.SetRate * g.Factor;
                            }
                            _context.PriceListDetails.Update(pri);
                        }
                        _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                        _context.InventoryAudits.Add(item_inventory_audit);
                        _context.SaveChanges();
                    }
                }


                #region
                //// Tax Account ///
                var taxg = _context.TaxGroups.Find(itemdt.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = itemdt.TaxOfFinDisValue * (decimal)ap.PurRate;
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
                            Type = Financials.Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Debit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,

                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(taxAcc);
                }
                #endregion

            }

            //update bom
            foreach (var item in ItemPO)
            {
                var ItemBOMDetail = _context.BOMDetail.Where(w => w.ItemID == item.ItemID);
                foreach (var itembom in ItemBOMDetail)
                {
                    var BOM = _context.BOMaterial.First(w => w.BID == itembom.BID);
                    var DBOM = _context.BOMDetail.Where(w => w.BID == BOM.BID && w.Detele == false);
                    var Inven = _context.InventoryAudits.Where(w => w.ItemID == itembom.ItemID);
                    double @AvgCost = Inven.Sum(s => s.Trans_Valuse) / Inven.Sum(q => q.Qty);
                    @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                    var Gdoum = _context.GroupDUoMs.Where(w => w.GroupUoMID == itembom.GUomID);
                    var Factor = Gdoum.FirstOrDefault(w => w.AltUOM == itembom.UomID).Factor;
                    itembom.Cost = @AvgCost * Factor;
                    itembom.Amount = itembom.Qty * (@AvgCost * Factor);
                    _context.BOMDetail.UpdateRange(ItemBOMDetail);
                    _context.SaveChanges();
                    BOM.TotalCost = DBOM.Sum(w => w.Amount);
                    _context.BOMaterial.Update(BOM);
                    _context.SaveChanges();
                }
            }
            //insert Outgoing payment
            outgoingPaymentVendor.TypePurchase = TypePurchase.AP;
            outgoingPaymentVendor.BalanceDue = ap.BalanceDue;
            outgoingPaymentVendor.PostingDate = ap.PostingDate;
            outgoingPaymentVendor.Date = ap.DueDate;
            outgoingPaymentVendor.OverdueDays = 0;
            outgoingPaymentVendor.Total = (double)ap.SubTotalAfterDisSys;// ap.BalanceDue;//ap.SubTotal;
            outgoingPaymentVendor.TotalPayment = ap.BalanceDue;// ap.SubTotal - ap.DiscountValue;
            outgoingPaymentVendor.BranchID = ap.BranchID;
            outgoingPaymentVendor.CurrencyID = ap.PurCurrencyID;
            outgoingPaymentVendor.VendorID = ap.VendorID;
            outgoingPaymentVendor.WarehouseID = ap.WarehouseID;
            outgoingPaymentVendor.Status = ap.Status;
            outgoingPaymentVendor.CashDiscount = ap.DiscountRate;
            outgoingPaymentVendor.TotalDiscount = ap.DiscountValue;
            outgoingPaymentVendor.Applied_Amount = ap.AppliedAmount;
            outgoingPaymentVendor.ExchangeRate = ap.PurRate;
            outgoingPaymentVendor.SysCurrency = ap.SysCurrencyID;
            outgoingPaymentVendor.LocalCurID = ap.LocalCurID;
            outgoingPaymentVendor.LocalSetRate = ap.LocalSetRate;
            outgoingPaymentVendor.CompanyID = ap.CompanyID;
            outgoingPaymentVendor.SeriesDetailID = ap.SeriesDetailID;
            outgoingPaymentVendor.DocumentID = ap.DocumentTypeID;
            outgoingPaymentVendor.Number = ap.Number;
            outgoingPaymentVendor.ItemInvoice = $"{docType.Code}-{ap.Number}";

            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.Update(outgoingPaymentVendor);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }

        #endregion GoodReceiptStock
        public void GoodReceiptStockAPReserve(int purchaseID, string Type, FreightPurchase freight)
        {
            var ap = _context.PurchaseAPReserves.Find(purchaseID);
            var ItemPO = _context.PurchaseAPReserveDetails.Where(w => w.PurchaseAPReserveID == purchaseID).ToList();

            OutgoingPaymentVendor outgoingPaymentVendor = new();
            var docType = _context.DocumentTypes.Find(ap.DocumentTypeID);
            var series = _context.SeriesDetails.Find(ap.SeriesDetailID);
            List<GLAccount> gLAccounts = _context.GLAccounts.Where(i => i.IsActive).ToList();
            int allocateAccID = 0;
            decimal allocateAccAmount = 0;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo) ?? new Series();
            if (defaultJE.ID > 0)
            {
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
                journalEntry.Creator = ap.UserID;
                 journalEntry.BranchID = ap.BranchID;
                journalEntry.TransNo = Sno;
                journalEntry.PostingDate = ap.PostingDate;
                journalEntry.DocumentDate = ap.DocumentDate;
                journalEntry.DueDate = ap.DueDate;
                journalEntry.SSCID = ap.SysCurrencyID;
                journalEntry.LLCID = ap.LocalCurID;
                journalEntry.CompanyID = ap.CompanyID;
                journalEntry.LocalSetRate = (decimal)ap.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = defaultJE.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            // AccountReceice
            var accountPayable = _context.BusinessPartners.FirstOrDefault(w => w.ID == ap.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountPayable.GLAccID);
            List<JournalEntryDetail> journalEntryDetail = new()
            {
                new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountPayable.GLAccID,
                    Credit = (decimal)ap.BalanceDueSys,
                    BPAcctID = ap.VendorID,
                }
            };
            //Insert 
            glAcc.Balance -= (decimal)ap.BalanceDueSys;
            List<AccountBalance> accountBalance = new()
            {
                new AccountBalance
                {
                      JEID = journalEntry.ID,
                    PostingDate = ap.PostingDate,
                    Origin = docType.ID,
                    OriginNo = ap.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Credit = (decimal)ap.BalanceDueSys,
                    LocalSetRate = (decimal)ap.LocalSetRate,
                    GLAID = accountPayable.GLAccID,
                    BPAcctID = ap.VendorID,
                    Creator = ap.UserID,
                    Effective=EffectiveBlance.Credit
                }
            };
            #region
            // Freight //
            if (freight != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Debit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.InvoiceNo,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Debit = _framount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)ap.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Debit = _frtaxamount
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = ap.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = ap.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Debit = _frtaxamount,
                                    LocalSetRate = (decimal)ap.LocalSetRate,
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
            #endregion
            _context.Update(glAcc);
            _context.SaveChanges();
            //insert inventoryaudit
            foreach (var itemdt in ItemPO)
            {
                var itemmaster = _context.ItemMasterDatas.FirstOrDefault(w => !w.Delete && w.ID == itemdt.ItemID);
                //var ap = _context.Purchase_APs.FirstOrDefault(w => w.PurchaseAPID == itemdt.PurchaseAPID);                
                var _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == itemdt.ItemID && i.WarehouseID == ap.WarehouseID);
                List<ItemAccounting> itemAccs = new();
                var item_master_data = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == itemdt.ItemID);
                if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemID == itemdt.ItemID && w.WarehouseID == ap.WarehouseID).ToList();
                    var allocateAcc = (from ia in itemAccs
                                       join gl in gLAccounts on ia.AllocationAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();
                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }
                else if (item_master_data.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    itemAccs = _context.ItemAccountings.Where(w => w.ItemGroupID == item_master_data.ItemGroup1ID).ToList();
                    var allocateAcc = (from ia in itemAccs
                                       join gl in gLAccounts on ia.AllocationAccount equals gl.Code
                                       select gl
                                         ).FirstOrDefault();
                    if (allocateAcc != null)
                    {
                        allocateAccID = allocateAcc.ID;
                        if (ap.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)itemdt.TotalSys * (decimal)ap.DiscountRate / 100;
                            allocateAccAmount = (decimal)itemdt.TotalSys - disvalue;
                        }
                        else
                        {
                            allocateAccAmount = (decimal)itemdt.TotalSys;
                        }
                    }
                }
                //inventoryAccID
                var glAccAllocate = _context.GLAccounts.FirstOrDefault(w => w.ID == allocateAccID);
                var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == allocateAccID) ?? new JournalEntryDetail();
                if (journalDetail.ItemID > 0)
                {
                    glAccAllocate.Balance += allocateAccAmount;
                    var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == allocateAccID);
                    //journalEntryDetail
                    journalDetail.Debit += allocateAccAmount;
                    //accountBalance
                    accBalance.CumulativeBalance = glAccAllocate.Balance;
                    accBalance.Debit += allocateAccAmount;
                }
                else
                {
                    glAccAllocate.Balance += allocateAccAmount;
                    journalEntryDetail.Add(new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Financials.Type.GLAcct,
                        ItemID = allocateAccID,
                        Debit = allocateAccAmount,
                    });
                    //
                    accountBalance.Add(new AccountBalance
                    {
                        JEID = journalEntry.ID,
                        PostingDate = ap.PostingDate,
                        Origin = docType.ID,
                        OriginNo = ap.Number,
                        OffsetAccount = glAcc.Code,
                        Details = douTypeID.Name + " - " + glAccAllocate.Code,
                        CumulativeBalance = glAccAllocate.Balance,
                        Debit = allocateAccAmount,
                        LocalSetRate = (decimal)ap.LocalSetRate,
                        GLAID = allocateAccID,
                        Effective = EffectiveBlance.Debit
                    });
                    _context.Update(glAccAllocate);
                    _context.SaveChanges();
                }

                #region
                //// Tax Account ///
                var taxg = _context.TaxGroups.Find(itemdt.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = itemdt.TaxOfFinDisValue * (decimal)ap.PurRate;
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
                            Type = Financials.Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Debit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = ap.PostingDate,
                            Origin = docType.ID,
                            OriginNo = ap.Number,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Debit = taxValue,
                            LocalSetRate = (decimal)ap.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Debit
                        });
                    }
                    _context.Update(taxAcc);
                }
                #endregion
            }
            //insert Outgoing payment
            outgoingPaymentVendor.TypePurchase = TypePurchase.APReserve;
            outgoingPaymentVendor.BalanceDue = ap.BalanceDue;
            outgoingPaymentVendor.PostingDate = ap.PostingDate;
            outgoingPaymentVendor.Date = ap.DueDate;
            outgoingPaymentVendor.OverdueDays = 0;
            outgoingPaymentVendor.Total = (double)ap.SubTotalAfterDisSys;//ap.SubTotal;
            outgoingPaymentVendor.TotalPayment = ap.BalanceDue;//ap.SubTotal - ap.DiscountValue;
            outgoingPaymentVendor.BranchID = ap.BranchID;
            outgoingPaymentVendor.CurrencyID = ap.PurCurrencyID;
            outgoingPaymentVendor.VendorID = ap.VendorID;
            outgoingPaymentVendor.WarehouseID = ap.WarehouseID;
            outgoingPaymentVendor.Status = ap.Status;
            outgoingPaymentVendor.CashDiscount = ap.DiscountRate;
            outgoingPaymentVendor.TotalDiscount = ap.DiscountValue;
            outgoingPaymentVendor.Applied_Amount = ap.AppliedAmount;
            outgoingPaymentVendor.ExchangeRate = ap.PurRate;
            outgoingPaymentVendor.SysCurrency = ap.SysCurrencyID;
            outgoingPaymentVendor.LocalCurID = ap.LocalCurID;
            outgoingPaymentVendor.LocalSetRate = ap.LocalSetRate;
            outgoingPaymentVendor.CompanyID = ap.CompanyID;
            outgoingPaymentVendor.SeriesDetailID = ap.SeriesDetailID;
            outgoingPaymentVendor.DocumentID = ap.DocumentTypeID;
            outgoingPaymentVendor.Number = series.Number;
            outgoingPaymentVendor.ItemInvoice = $"{docType.Code}-{ap.Number}";
            outgoingPaymentVendor.TypePurchase = TypePurchase.APReserve;


            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.Update(outgoingPaymentVendor);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
        public IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs(int ID) => _context.ServiceMapItemMasterDataPurchasAPs.FromSql("sp_GetListItemMasterDataPurchaseAP @WarehouseID={0}",
            parameters: new[] {
                ID.ToString()
            });

        public IEnumerable<ServiceMapItemMasterDataPurchasAP> ServiceMapItemMasterDataPurchasAPs_Detail(int warehouseid, string number) => _context.ServiceMapItemMasterDataPurchasAPs.FromSql("sp_GetListItemMasterDataPurchaseAP_Detail @WarehouseID={0},@Number={1}",
            parameters: new[] {
                warehouseid.ToString(),
                number.ToString()
            });
        public Dictionary<int, string> Typevat => EnumHelper.ToDictionary(typeof(PVatType));
        public List<PurchaseReport> GetPurchaseAPReserves(int BranchID, int WarehouseID, int VendorID, string PostingDate, string DocumentDate, bool check)
        {
            List<PurchaseAPReserve> ServiceCalls = new();

            //WareHouse
            if (WarehouseID != 0 && VendorID == 0 && DocumentDate == null && PostingDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.WarehouseID == WarehouseID).ToList();
            }

            // filterVendor
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.VendorID == VendorID).ToList();
            }

            // warehouse and Vendor

            else if (WarehouseID != 0 && VendorID != 0 && DocumentDate == null && PostingDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && VendorID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (WarehouseID != 0 & VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.VendorID == VendorID && w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //FilterDateFrom and Date To
            else if (WarehouseID == 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.VendorID == VendorID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (WarehouseID != 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseAPReserves.Where(w => w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                ServiceCalls = _context.PurchaseAPReserves.ToList();
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.VendorID equals cus.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new PurchaseReport
                        {
                            ID = s.ID,
                            Invoice = s.InvoiceNo,
                            VendorName = cus.Name,
                            UserName = item.Username,
                            Balance = s.BalanceDue,
                            ExchangeRate = s.PurRate,
                            Cancele = "<i class= 'fa fa-ban'style='color:red;' ></i>",
                            Status = s.Status,
                            VatType = Typevat.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),

                        }).ToList();
            return list;
        }

        public void IssuseCancelPurchaseAP(Purchase_AP purchase, List<APCSerialNumber> serials, List<APCBatchNo> batches, FreightPurchase freight)
        {
            var docType = _context.DocumentTypes.Find(purchase.DocumentTypeID);
            var series = _context.Series.Find(purchase.SeriesID);
            int inventoryAccID = 0;
            decimal inventoryAccAmount = 0;
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID && w.NextNo != w.LastNo);
            if (defaultJE.ID > 0)
            {
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
                journalEntry.Creator = purchase.UserID;
                journalEntry.BranchID = purchase.BranchID;
                journalEntry.TransNo = purchase.Number;
                journalEntry.PostingDate = purchase.PostingDate;
                journalEntry.DocumentDate = purchase.DocumentDate;
                journalEntry.DueDate = purchase.DueDate;
                journalEntry.SSCID = purchase.SysCurrencyID;
                journalEntry.LLCID = purchase.LocalCurID;
                journalEntry.CompanyID = purchase.CompanyID;
                journalEntry.LocalSetRate = (decimal)purchase.LocalSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = series.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }
            //IssuseInstock
            // AccountReceice
            var accountReceive = _context.BusinessPartners.FirstOrDefault(w => w.ID == purchase.VendorID);
            var glAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountReceive.GLAccID);
            if (glAcc.ID > 0)
            {
                journalEntryDetail.Add(new JournalEntryDetail
                {
                    JEID = journalEntry.ID,
                    Type = Financials.Type.BPCode,
                    ItemID = accountReceive.GLAccID,
                    Debit = purchase.SubTotalAfterDisSys,
                    BPAcctID = purchase.VendorID,
                });
                //Insert 
                glAcc.Balance += purchase.SubTotalAfterDisSys;
                accountBalance.Add(new AccountBalance
                {
                    JEID = journalEntry.ID,
                    PostingDate = purchase.PostingDate,
                    Origin = docType.ID,
                    OriginNo = purchase.Number,
                    OffsetAccount = glAcc.Code,
                    Details = douTypeID.Name + "-" + glAcc.Code,
                    CumulativeBalance = glAcc.Balance,
                    Debit = purchase.SubTotalAfterDisSys,
                    LocalSetRate = (decimal)purchase.LocalSetRate,
                    GLAID = accountReceive.GLAccID,
                    BPAcctID = purchase.VendorID,
                    Creator = purchase.UserID,
                    Effective = EffectiveBlance.Debit
                });
                _context.Update(glAcc);
            }
            #region
            // Freight //
            if (freight != null && freight.FreightPurchaseDetials != null)
            {
                if (freight.FreightPurchaseDetials.Any())
                {
                    foreach (var fr in freight.FreightPurchaseDetials.Where(i => i.Amount > 0).ToList())
                    {
                        var freightOg = _context.Freights.Find(fr.FreightID) ?? new Freight();
                        var frgl = _context.GLAccounts.Find(freightOg.ExpenAcctID) ?? new GLAccount();
                        var taxfr = _context.TaxGroups.Find(fr.TaxGroupID) ?? new TaxGroup();
                        var taxgacc = _context.GLAccounts.Find(taxfr.GLID) ?? new GLAccount();
                        if (frgl.ID > 0)
                        {
                            var frgljur = journalEntryDetail.FirstOrDefault(w => w.ItemID == frgl.ID) ?? new JournalEntryDetail();
                            var _framount = fr.Amount * (decimal)purchase.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = frgl.ID,
                                    Credit = _framount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = purchase.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = purchase.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + frgl.Code,
                                    CumulativeBalance = frgl.Balance,
                                    Credit = _framount,
                                    LocalSetRate = (decimal)purchase.LocalSetRate,
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
                            var _frtaxamount = fr.TotalTaxAmount * (decimal)purchase.PurRate;
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
                                    Type = Financials.Type.GLAcct,
                                    ItemID = taxgacc.ID,
                                    Credit = _frtaxamount,
                                });
                                //
                                accountBalance.Add(new AccountBalance
                                {
                                    JEID = journalEntry.ID,
                                    PostingDate = purchase.PostingDate,
                                    Origin = docType.ID,
                                    OriginNo = purchase.Number,
                                    OffsetAccount = glAcc.Code,
                                    Details = douTypeID.Name + " - " + taxgacc.Code,
                                    CumulativeBalance = taxgacc.Balance,
                                    Credit = _frtaxamount,
                                    LocalSetRate = (decimal)purchase.LocalSetRate,
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
            #endregion
            foreach (var item in purchase.PurchaseAPDetails.ToList())
            {
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                ItemAccounting _itemAcc = _context.ItemAccountings.FirstOrDefault(i => i.ItemID == item.ItemID && purchase.WarehouseID == i.WarehouseID);
                if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemLevel)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchase.WarehouseID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    if (inventoryAcc != null)
                    {
                        inventoryAccID = inventoryAcc.ID;
                        if (purchase.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)purchase.DiscountRate / 100;
                            inventoryAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            inventoryAccAmount = (decimal)item.TotalSys;
                        }
                    }
                }
                else if (itemMaster.SetGlAccount == Inventory.SetGlAccount.ItemGroup)
                {
                    var inventoryAcc = (from ia in _context.ItemAccountings.Where(w => w.ItemGroupID == itemMaster.ItemGroup1ID)
                                        join gl in _context.GLAccounts on ia.InventoryAccount equals gl.Code
                                        select gl
                                         ).FirstOrDefault();
                    if (inventoryAcc != null)
                    {
                        inventoryAccID = inventoryAcc.ID;
                        if (purchase.DiscountRate > 0)
                        {
                            decimal disvalue = (decimal)item.TotalSys * (decimal)purchase.DiscountRate / 100;
                            inventoryAccAmount = (decimal)item.TotalSys - disvalue;
                        }
                        else
                        {
                            inventoryAccAmount = (decimal)item.TotalSys;
                        }
                    }
                }
                #region
                //// Tax Account ///
                var taxg = _context.TaxGroups.Find(item.TaxGroupID) ?? new TaxGroup();
                var taxAcc = _context.GLAccounts.Find(taxg.GLID) ?? new GLAccount();
                decimal taxValue = item.TaxOfFinDisValue * (decimal)purchase.PurRate;
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
                            Type = Financials.Type.GLAcct,
                            ItemID = taxAcc.ID,
                            Credit = taxValue,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = purchase.PostingDate,
                            Origin = docType.ID,
                            OriginNo = purchase.Number,
                            OffsetAccount = taxAcc.Code,
                            Details = douTypeID.Name + " - " + taxAcc.Code,
                            CumulativeBalance = taxAcc.Balance,
                            Credit = taxValue,
                            LocalSetRate = (decimal)purchase.LocalSetRate,
                            GLAID = taxAcc.ID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.Update(taxAcc);
                }
                #endregion
                var glAccInvenfifo = _context.GLAccounts.FirstOrDefault(w => w.ID == inventoryAccID) ?? new GLAccount();
                if (glAccInvenfifo.ID > 0)
                {
                    var journalDetail = journalEntryDetail.FirstOrDefault(w => w.ItemID == glAccInvenfifo.ID) ?? new JournalEntryDetail();
                    if (journalDetail.ItemID > 0)
                    {
                        var accBalance = accountBalance.FirstOrDefault(w => w.GLAID == inventoryAccID);
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        //journalEntryDetail
                        journalDetail.Credit += inventoryAccAmount;
                        //accountBalance
                        accBalance.CumulativeBalance = glAccInvenfifo.Balance;
                        accBalance.Credit += inventoryAccAmount;
                    }
                    else
                    {
                        glAccInvenfifo.Balance -= inventoryAccAmount;
                        journalEntryDetail.Add(new JournalEntryDetail
                        {
                            JEID = journalEntry.ID,
                            Type = Financials.Type.GLAcct,
                            ItemID = inventoryAccID,
                            Credit = inventoryAccAmount,
                        });
                        //
                        accountBalance.Add(new AccountBalance
                        {
                            JEID = journalEntry.ID,
                            PostingDate = purchase.PostingDate,
                            Origin = docType.ID,
                            OriginNo = purchase.Number,
                            OffsetAccount = glAcc.Code,
                            Details = douTypeID.Name + "-" + glAccInvenfifo.Code,
                            CumulativeBalance = glAccInvenfifo.Balance,
                            Credit = inventoryAccAmount,
                            LocalSetRate = (decimal)purchase.LocalSetRate,
                            GLAID = inventoryAccID,
                            Effective = EffectiveBlance.Credit
                        });
                    }
                    _context.GLAccounts.Update(glAccInvenfifo);
                }
                var orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == itemMaster.GroupUomID && w.AltUOM == item.UomID);
                var itemWareSum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID && w.ItemID == item.ItemID);
                var Cost = item.PurchasPrice * purchase.PurRate;
                var waredetials = _context.WarehouseDetails.Where(w => w.WarehouseID == purchase.WarehouseID && w.ItemID == item.ItemID && w.Cost == Cost).ToList();
                if (itemWareSum != null)
                {
                    //WerehouseSummary
                    itemWareSum.InStock -= (double)item.Qty;
                    //Itemmasterdata
                    itemMaster.StockIn = itemWareSum.InStock - (double)item.Qty;
                    _context.WarehouseSummary.Update(itemWareSum);
                    _context.ItemMasterDatas.Update(itemMaster);
                    _utility.UpdateItemAccounting(_itemAcc, itemWareSum);
                }
                double @Check_Stock;
                double @Remain;
                double @IssusQty;
                double @FIFOQty;
                double @Qty = item.Qty * orft.Factor;
                if (itemMaster.ManItemBy == ManageItemBy.SerialNumbers
                    && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (serials.Count > 0)
                    {
                        List<InventoryAudit> invens = new();
                        List<StockOut> stockOuts = new();
                        foreach (var s in serials)
                        {
                            if (s.APCSNSelected != null)
                            {
                                foreach (var ss in s.APCSNSelected.APCSNDSelectedDetails)
                                {
                                    var waredetial = waredetials
                                        .FirstOrDefault(i => ss.SerialNumber == i.SerialNumber && i.InStock > 0 && i.Cost == item.PurchasPrice * purchase.PurRate);
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= 1;
                                        // Insert to warehouse detial //
                                        stockOuts.Add(new StockOut
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
                                            PlateNumber = waredetial.PlateNumber,
                                            Color = waredetial.Color,
                                            Brand = waredetial.Brand,
                                            Condition = waredetial.Condition,
                                            Type = waredetial.Type,
                                            Power = waredetial.Power,
                                            Year = waredetial.Year,
                                            SyetemDate = DateTime.Now,
                                            SysNum = 0,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = waredetial.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = purchase.UserID,
                                            ExpireDate = item.ExpireDate,
                                            TransType = TransTypeWD.PurAP,
                                            TransID = purchase.PurchaseAPID,
                                            OutStockFrom = purchase.PurchaseAPID,
                                            FromWareDetialID = waredetial.ID,
                                        });
                                    }
                                }
                            }
                        }
                        // Insert to InventoryAudit //
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchase.WarehouseID);
                        var inven = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = purchase.WarehouseID,
                            BranchID = purchase.BranchID,
                            UserID = purchase.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = purchase.CompanyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = purchase.Number,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = Cost,
                            Price = 0,
                            CumulativeQty = (inventory_audit.Sum(q => q.Qty) - @Qty),
                            CumulativeValue = (inventory_audit.Sum(s => s.Trans_Valuse)) - (@Qty * Cost),
                            Trans_Valuse = @Qty * Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = purchase.LocalCurID,
                            LocalSetRate = purchase.LocalSetRate,
                            CompanyID = purchase.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = purchase.SeriesID,
                            SeriesDetailID = purchase.SeriesDetailID,
                        };
                        _context.InventoryAudits.Add(inven);
                        _context.StockOuts.AddRange(stockOuts);
                        _context.SaveChanges();
                    }
                }
                else if (itemMaster.ManItemBy == ManageItemBy.Batches
                    && itemMaster.ManMethod == ManagementMethod.OnEveryTransation)
                {
                    if (batches.Count > 0)
                    {
                        foreach (var b in batches)
                        {
                            if (b.APCBatchNoSelected != null)
                            {
                                foreach (var sb in b.APCBatchNoSelected.APCBatchNoSelectedDetails)
                                {
                                    var waredetial = waredetials
                                        .FirstOrDefault(i =>
                                            sb.BatchNo == i.BatchNo
                                            && i.InStock > 0
                                            && i.Cost == item.PurchasPrice * purchase.PurRate);
                                    if (waredetial != null)
                                    {
                                        waredetial.InStock -= (double)sb.SelectedQty;
                                        _context.SaveChanges();
                                        // insert to waredetial
                                        var stockOuts = new StockOut
                                        {
                                            AdmissionDate = waredetial.AdmissionDate,
                                            Cost = (decimal)waredetial.Cost,
                                            CurrencyID = waredetial.CurrencyID,
                                            Details = waredetial.Details,
                                            ID = 0,
                                            InStock = sb.SelectedQty,
                                            ItemID = item.ItemID,
                                            Location = waredetial.Location,
                                            MfrDate = waredetial.MfrDate,
                                            ProcessItem = ProcessItem.SEBA,
                                            SyetemDate = DateTime.Now,
                                            SysNum = 0,
                                            TimeIn = DateTime.Now,
                                            WarehouseID = waredetial.WarehouseID,
                                            UomID = item.UomID,
                                            UserID = purchase.UserID,
                                            ExpireDate = item.ExpireDate,
                                            BatchAttr1 = waredetial.BatchAttr1,
                                            BatchAttr2 = waredetial.BatchAttr2,
                                            BatchNo = waredetial.BatchNo,
                                            TransType = TransTypeWD.PurAP,
                                            TransID = purchase.PurchaseAPID,
                                            FromWareDetialID = waredetial.ID,
                                            OutStockFrom = purchase.PurCurrencyID,
                                        };
                                        _context.StockOuts.Add(stockOuts);
                                        _context.SaveChanges();
                                    }
                                }
                            }
                        }
                        // insert to inventory audit
                        var inventory_audit = _context.InventoryAudits
                            .Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchase.WarehouseID);
                        var invens = new InventoryAudit
                        {
                            ID = 0,
                            WarehouseID = purchase.WarehouseID,
                            BranchID = purchase.BranchID,
                            UserID = purchase.UserID,
                            ItemID = item.ItemID,
                            CurrencyID = purchase.CompanyID,
                            UomID = orft.BaseUOM,
                            InvoiceNo = purchase.Number,
                            Trans_Type = docType.Code,
                            Process = itemMaster.Process,
                            SystemDate = DateTime.Now,
                            TimeIn = DateTime.Now.ToShortTimeString().ToString(),
                            Qty = @Qty * -1,
                            Cost = Cost,
                            Price = 0,
                            CumulativeQty = inventory_audit.Sum(q => q.Qty) - @Qty,
                            CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@Qty * Cost),
                            Trans_Valuse = @Qty * Cost * -1,
                            ExpireDate = item.ExpireDate,
                            LocalCurID = purchase.LocalCurID,
                            LocalSetRate = purchase.LocalSetRate,
                            CompanyID = purchase.CompanyID,
                            DocumentTypeID = docType.ID,
                            SeriesID = purchase.SeriesID,
                            SeriesDetailID = purchase.SeriesDetailID,
                        };
                        _context.InventoryAudits.Add(invens);
                        _context.SaveChanges();
                    }
                }
                else
                {
                    foreach (var item_warehouse in waredetials.Where(w => w.InStock > 0))
                    {
                        var item_inventory_audit = new InventoryAudit();
                        var item_IssusStock = waredetials.FirstOrDefault(w => w.InStock > 0);
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
                                        UserID = purchase.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurAP,
                                    };
                                    _context.StockOuts.Add(stockOuts);
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.WarehouseID == purchase.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchase.WarehouseID;
                                    item_inventory_audit.BranchID = purchase.BranchID;
                                    item_inventory_audit.UserID = purchase.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchase.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchase.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
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
                                    item_inventory_audit.LocalCurID = purchase.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchase.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchase.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchase.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchase.SeriesDetailID;
                                }
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else if (itemMaster.Process == "Average")
                            {
                                item_IssusStock.InStock = item_IssusStock.InStock -= @IssusQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == purchase.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == purchase.WarehouseID);
                                    double cost = (inventory_audit.Sum(s => s.Trans_Valuse) - @IssusQty * @sysAvCost / (inventory_audit.Sum(q => q.Qty) - @IssusQty));
                                    cost = _utility.CheckNaNOrInfinity(cost);
                                    var stockOuts = new StockOut
                                    {
                                        Cost = (decimal)cost,
                                        CurrencyID = item_warehouse.CurrencyID,
                                        ID = 0,
                                        InStock = (decimal)@IssusQty,
                                        ItemID = item.ItemID,
                                        ProcessItem = ProcessItem.Average,
                                        SyetemDate = DateTime.Now,
                                        TimeIn = DateTime.Now,
                                        WarehouseID = item_warehouse.WarehouseID,
                                        UomID = item.UomID,
                                        UserID = purchase.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurAP,
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchase.WarehouseID;
                                    item_inventory_audit.BranchID = purchase.BranchID;
                                    item_inventory_audit.UserID = purchase.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchase.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchase.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchase.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchase.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchase.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchase.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchase.SeriesDetailID;
                                }
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                double @AvgCost = ((inventoryAcct.Sum(s => s.Trans_Valuse) + item_inventory_audit.Cost) / (inventoryAcct.Sum(q => q.Qty) + item_inventory_audit.Qty));
                                @AvgCost = _utility.CheckNaNOrInfinity(@AvgCost);
                                _utility.UpdateAvgCost(item_warehouse.ItemID, purchase.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
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
                                        UserID = purchase.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurAP,
                                    };
                                    _context.StockOuts.Add(stockOuts);

                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == purchase.WarehouseID);
                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchase.WarehouseID;
                                    item_inventory_audit.BranchID = purchase.BranchID;
                                    item_inventory_audit.UserID = purchase.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchase.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchase.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = item_IssusStock.Cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * item_IssusStock.Cost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * item_IssusStock.Cost * (-1);
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchase.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchase.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchase.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchase.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchase.SeriesDetailID;
                                }
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            else if (itemMaster.Process == "Average")
                            {
                                item_IssusStock.InStock = @FIFOQty;
                                if (@IssusQty > 0)
                                {
                                    var warehouse_summary = _context.WarehouseSummary.FirstOrDefault(w => w.ItemID == item.ItemID && w.WarehouseID == purchase.WarehouseID);
                                    double @sysAvCost = warehouse_summary.Cost;
                                    var inventory_audit = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID && w.UomID == item.UomID && w.WarehouseID == purchase.WarehouseID);
                                    double cost = (inventory_audit.Sum(s => s.Trans_Valuse) + @IssusQty * @sysAvCost * (-1)) / (inventory_audit.Sum(q => q.Qty) + (@IssusQty * (-1)));
                                    cost = _utility.CheckNaNOrInfinity(cost);
                                    var stockOuts = new StockOut
                                    {
                                        Cost = (decimal)cost,
                                        CurrencyID = item_warehouse.CurrencyID,
                                        ID = 0,
                                        InStock = (decimal)@IssusQty,
                                        ItemID = item.ItemID,
                                        ProcessItem = ProcessItem.Average,
                                        SyetemDate = DateTime.Now,
                                        TimeIn = DateTime.Now,
                                        WarehouseID = item_warehouse.WarehouseID,
                                        UomID = item.UomID,
                                        UserID = purchase.UserID,
                                        ExpireDate = item.ExpireDate,
                                        TransType = TransTypeWD.PurAP,
                                    };

                                    _context.StockOuts.Add(stockOuts);

                                    item_inventory_audit.ID = 0;
                                    item_inventory_audit.WarehouseID = purchase.WarehouseID;
                                    item_inventory_audit.BranchID = purchase.BranchID;
                                    item_inventory_audit.UserID = purchase.UserID;
                                    item_inventory_audit.ItemID = item.ItemID;
                                    item_inventory_audit.CurrencyID = purchase.CompanyID;
                                    item_inventory_audit.UomID = orft.BaseUOM;
                                    item_inventory_audit.InvoiceNo = purchase.Number;
                                    item_inventory_audit.Trans_Type = docType.Code;
                                    item_inventory_audit.Process = itemMaster.Process;
                                    item_inventory_audit.SystemDate = DateTime.Now;
                                    item_inventory_audit.TimeIn = DateTime.Now.ToShortTimeString().ToString();
                                    item_inventory_audit.Qty = @IssusQty * -1;
                                    item_inventory_audit.Cost = cost;
                                    item_inventory_audit.Price = 0;
                                    item_inventory_audit.CumulativeQty = inventory_audit.Sum(q => q.Qty) - @IssusQty;
                                    item_inventory_audit.CumulativeValue = inventory_audit.Sum(s => s.Trans_Valuse) - (@IssusQty * @sysAvCost);
                                    item_inventory_audit.Trans_Valuse = @IssusQty * @sysAvCost * -1;
                                    item_inventory_audit.ExpireDate = item_IssusStock.ExpireDate;
                                    item_inventory_audit.LocalCurID = purchase.LocalCurID;
                                    item_inventory_audit.LocalSetRate = purchase.LocalSetRate;
                                    item_inventory_audit.CompanyID = purchase.CompanyID;
                                    item_inventory_audit.DocumentTypeID = docType.ID;
                                    item_inventory_audit.SeriesID = purchase.SeriesID;
                                    item_inventory_audit.SeriesDetailID = purchase.SeriesDetailID;
                                }
                                var inventoryAcct = _context.InventoryAudits.Where(w => w.ItemID == item.ItemID);
                                _utility.UpdateAvgCost(item_warehouse.ItemID, purchase.WarehouseID, itemMaster.GroupUomID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.UpdateBomCost(item_warehouse.ItemID, item_inventory_audit.Qty, item_inventory_audit.Cost);
                                _utility.CumulativeValue(item_inventory_audit.WarehouseID, item_inventory_audit.ItemID, item_inventory_audit.CumulativeValue, _itemAcc);
                                _context.WarehouseDetails.Update(item_IssusStock);
                                _context.InventoryAudits.Add(item_inventory_audit);
                                _context.SaveChanges();
                            }
                            waredetials = new List<WarehouseDetail>();
                            break;
                        }
                    }
                }
            }
            var journal = _context.JournalEntries.Find(journalEntry.ID);
            journal.TotalDebit = journalEntryDetail.Sum(s => s.Debit);
            journal.TotalCredit = journalEntryDetail.Sum(s => s.Credit);
            _context.JournalEntries.Update(journal);
            _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            _context.AccountBalances.UpdateRange(accountBalance);
            _context.SaveChanges();
        }
    }

}
