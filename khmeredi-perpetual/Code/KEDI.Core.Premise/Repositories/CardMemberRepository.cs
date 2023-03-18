using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Premise.Models.Services.Administrator.SetUp;
using KEDI.Core.Premise.Models.Services.CardMembers;
using KEDI.Core.Premise.Models.ServicesClass.CardMember;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Type = CKBS.Models.Services.Financials.Type;

namespace KEDI.Core.Premise.Repository
{
    public interface ICardMemberRepository
    {
        Task<List<BusinessPartner>> GetCustomerAsync();
        Task CreateUpdateAsync(TypeCard typeCard);
        Task<List<TypeCard>> GetCardTypesAsync(bool isDeleted);
        Task<List<CardMember>> GetRegisterMembersAsync();
        Task<CardMember> GetRegisterMemberDetialAsync(int id);
        Task<CardMember> GetRegisterMemberDetialAsync(string code);
        Task<TypeCard> GetTypeCardAsync(int id);
        Task<dynamic> DeleteCardTypeAsync(int id);
        Task<List<CustomerViewModel>> GetCustomerDipositAsync();
        Task RegisterMemberAsync(CardMember data);
        Task<List<CardMemberDeposit>> GetDepositCardMemberHistoryAsync(int id, string dateFrom, string dateTo);
        Task<List<CardMemberDepositTransaction>> GetCardTransactionsAsync(int id, string dateFrom, string dateTo);
        void DepositCardMember(CardMemberDeposit data, Company company, AccountMemberCard accountMember);
        Task<bool> RenewExpireDateCardAsync(RenewCardParamsObject data, ModelStateDictionary modelState);
        Task<List<RenewCardHistoryModel>> GetRenewHistoryAsync(int cardId);
    }

    public class CardMemberRepository : ICardMemberRepository
    {
        private readonly DataContext _context;

        public CardMemberRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessPartner>> GetCustomerAsync()
        {
            var data = await _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer").ToListAsync();
            return data;
        }
        public async Task<List<CustomerViewModel>> GetCustomerDipositAsync()
        {
            var data = await (from bp in _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer")
                              join card in _context.CardMembers on bp.CardMemberID equals card.ID
                              join pl in _context.PriceLists on bp.PriceListID equals pl.ID
                              select new CustomerViewModel
                              {
                                  ID = bp.ID,
                                  CardMemberID = card.ID,
                                  Address = bp.Address,
                                  CardName = card.Name,
                                  Name = bp.Name,
                                  Code = bp.Code,
                                  Phone = bp.Phone,
                                  PriceListID = pl.ID,
                                  PriceListName = pl.Name
                              }).ToListAsync();
            return data;
        }
        public async Task CreateUpdateAsync(TypeCard typeCard)
        {
            _context.TypeCards.Update(typeCard);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TypeCard>> GetCardTypesAsync(bool isDeleted)
        {
            var data = await _context.TypeCards.Where(i => i.IsDeleted == isDeleted).Select(i => new TypeCard
            {
                 Name = i.Name,
                Code = i.Code,
                Discount = i.Discount,
                ID = i.ID,
                IsDeleted = i.IsDeleted,
               
                TypeDiscount = i.TypeDiscount,
                TypeDiscountName = i.TypeDiscount == TypeCardDiscountType.Rate ? TypeCardDiscountType.Rate.ToString() :
                                    i.TypeDiscount == TypeCardDiscountType.Value ? TypeCardDiscountType.Value.ToString() : ""
            }).ToListAsync();
            return data;
        }
        public async Task<TypeCard> GetTypeCardAsync(int id)
        {
            var data = await _context.TypeCards.FirstOrDefaultAsync(i => !i.IsDeleted && i.ID == id);
            return data;
        }
        public async Task<dynamic> DeleteCardTypeAsync(int id)
        {
            var data = await _context.TypeCards.FindAsync(id);
            if (data == null) return new { Error = true, Message = "Could not delete" };
            var card = await _context.CardMembers.FirstOrDefaultAsync(i => i.TypeCardID == id && i.Active);
            if (card != null) return new { Error = true, Message = "This type card already linked to card. you cannot delete it" };
            data.IsDeleted = true;
            await _context.SaveChangesAsync();
            return new { Error = false };
        }

        public async Task RegisterMemberAsync(CardMember data)
        {
            _context.CardMembers.Update(data);
            await _context.SaveChangesAsync();

            data.Customer.CardMemberID = data.ID;
            _context.BusinessPartners.Update(data.Customer);
            _context.SaveChanges();
        }

        public async Task<List<CardMember>> GetRegisterMembersAsync()
        {
            var data = await _context.CardMembers.ToListAsync();
            return data;
        }

        public async Task<CardMember> GetRegisterMemberDetialAsync(int id)
        {
            var data = await (from i in _context.CardMembers.Where(i => i.ID == id)
                              select new CardMember
                              {
                                  Active = i.Active,
                                  ID = i.ID,
                                  Code = i.Code,
                                  Customer = (from bp in _context.BusinessPartners.Where(bp => bp.Type == "Customer" && bp.CardMemberID == i.ID)
                                              select new BusinessPartner
                                              {
                                                  Delete = bp.Delete,
                                                  Address = bp.Address,
                                                  AutoMobile = bp.AutoMobile,
                                                  Balance = bp.Balance,
                                                  BirthDate = bp.BirthDate,
                                                  CardMemberID = i.ID,
                                                  Code = bp.Code,
                                                  CumulativePoint = bp.CumulativePoint,
                                                  Email = bp.Email,
                                                  GLAccID = bp.GLAccID,
                                                  GroupID = bp.GroupID,
                                                  RedeemedPoint = bp.RedeemedPoint,
                                                  ID = bp.ID,
                                                  Type = bp.Type,
                                                  Name = bp.Name,
                                                  OutstandPoint = bp.OutstandPoint,
                                                  Phone = bp.Phone,
                                                  PriceList = bp.PriceList,
                                                  PriceListID = bp.PriceListID,
                                              }).FirstOrDefault() ?? new BusinessPartner(),
                                  Description = i.Description,
                                  Name = i.Name,
                                  TypeCardID = i.TypeCardID,
                                  LengthExpireCard = i.LengthExpireCard,
                                  ExpireDateFrom = i.ExpireDateFrom,
                                  ExpireDateTo = i.ExpireDateTo
                              }).FirstOrDefaultAsync() ?? new CardMember();
            return data;
        }
        public async Task<CardMember> GetRegisterMemberDetialAsync(string code)
        {
            var data = await (from i in _context.CardMembers.Where(i => i.Code == code)
                              select new CardMember
                              {
                                  Active = i.Active,
                                  ID = i.ID,
                                  Code = i.Code,
                                  Customer = (from bp in _context.BusinessPartners.Where(bp => bp.Type == "Customer" && bp.CardMemberID == i.ID)
                                              select new BusinessPartner
                                              {
                                                  Delete = bp.Delete,
                                                  Address = bp.Address,
                                                  AutoMobile = bp.AutoMobile,
                                                  Balance = bp.Balance,
                                                  BirthDate = bp.BirthDate,
                                                  CardMemberID = i.ID,
                                                  Code = bp.Code,
                                                  CumulativePoint = bp.CumulativePoint,
                                                  Email = bp.Email,
                                                  GLAccID = bp.GLAccID,
                                                  GroupID = bp.GroupID,
                                                  RedeemedPoint = bp.RedeemedPoint,
                                                  ID = bp.ID,
                                                  Type = bp.Type,
                                                  Name = bp.Name,
                                                  OutstandPoint = bp.OutstandPoint,
                                                  Phone = bp.Phone,
                                                  PriceList = bp.PriceList,
                                                  PriceListID = bp.PriceListID,
                                              }).FirstOrDefault() ?? new BusinessPartner(),
                                  Description = i.Description,
                                  Name = i.Name,
                                  TypeCardID = i.TypeCardID,
                                  ExpireDateFrom = i.ExpireDateFrom,
                                  ExpireDateTo = i.ExpireDateTo,
                                  LengthExpireCard = i.LengthExpireCard,
                                  Discount = i.Discount,
                                  TypeDiscount = i.TypeDiscount
                              }).FirstOrDefaultAsync() ?? new CardMember();
            return data;
        }
        public void DepositCardMember(CardMemberDeposit data, Company company, AccountMemberCard accountMember)
        {
            _context.CardMemberDeposits.Update(data);
            _context.SaveChanges();

            var cus = _context.BusinessPartners.Find(data.CusID) ?? new BusinessPartner();
            var pl = _context.PriceLists.Find(cus.PriceListID) ?? new PriceLists();
            var ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == pl.CurrencyID) ?? new ExchangeRate();
            JournalEntry journalEntry = new();
            SeriesDetail seriesDetail = new();
            List<JournalEntryDetail> journalEntryDetail = new();
            List<AccountBalance> accountBalance = new();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == company.LocalCurrencyID).SetRate;
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
                journalEntry.Creator = data.UserID;
                journalEntry.TransNo = Sno;
                journalEntry.PostingDate = data.PostingDate;
                journalEntry.DocumentDate = data.PostingDate;
                journalEntry.DueDate = data.PostingDate;
                journalEntry.SSCID = company.SystemCurrencyID;
                journalEntry.LLCID = company.LocalCurrencyID;
                journalEntry.CompanyID = company.ID;
                journalEntry.LocalSetRate = (decimal)localSetRate;
                journalEntry.SeriesDID = seriesDetail.ID;
                journalEntry.Remarks = defaultJE.Name + " " + Sno;
                _context.Update(journalEntry);
                _context.SaveChanges();
            }

            // financail
            var cashAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountMember.CashAccID);
            decimal totalDepositSys = data.TotalDeposit * (decimal)ex.Rate;
            if (cashAcc != null)
            {
                journalEntryDetail.Add(
                    new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.BPCode,
                        ItemID = cashAcc.ID,
                        Debit = totalDepositSys,
                        BPAcctID = data.CusID,
                    });
                //Insert 
                cashAcc.Balance += totalDepositSys;
                accountBalance.Add(
                    new AccountBalance
                    {
                        PostingDate = data.PostingDate,
                        Origin = data.DocTypeID,
                        OriginNo = data.Number,
                        OffsetAccount = cashAcc.Code,
                        Details = douTypeID.Name + "-" + cashAcc.Code,
                        CumulativeBalance = cashAcc.Balance,
                        Debit = totalDepositSys,
                        LocalSetRate = (decimal)localSetRate,
                        GLAID = cashAcc.ID,
                        BPAcctID = data.CusID,
                        Creator = data.UserID,
                    });
            }
            // financail
            var unearnedAcc = _context.GLAccounts.FirstOrDefault(w => w.ID == accountMember.UnearnedRevenueID);
            if (unearnedAcc != null)
            {
                journalEntryDetail.Add(
                    new JournalEntryDetail
                    {
                        JEID = journalEntry.ID,
                        Type = Type.GLAcct,
                        ItemID = unearnedAcc.ID,
                        Credit = totalDepositSys,
                        BPAcctID = data.CusID,
                    });
                //Insert 
                unearnedAcc.Balance -= totalDepositSys;
                accountBalance.Add(
                    new AccountBalance
                    {
                        PostingDate = data.PostingDate,
                        Origin = data.DocTypeID,
                        OriginNo = data.Number,
                        OffsetAccount = unearnedAcc.Code,
                        Details = douTypeID.Name + "-" + unearnedAcc.Code,
                        CumulativeBalance = unearnedAcc.Balance,
                        Credit = totalDepositSys,
                        LocalSetRate = (decimal)localSetRate,
                        GLAID = unearnedAcc.ID,
                        //BPAcctID = data.CusID,
                        //Creator = data.UserID,
                    });
            }
            if (accountBalance.Count > 0)
            {
                _context.AccountBalances.UpdateRange(accountBalance);
            }
            if (journalEntryDetail.Count > 0)
            {
                _context.JournalEntryDetails.UpdateRange(journalEntryDetail);
            }
            journalEntry.TotalDebit = journalEntryDetail.Sum(i => i.Debit);
            journalEntry.TotalCredit = journalEntryDetail.Sum(i => i.Credit);
            cus.Balance += data.TotalDeposit;
            // insert to card member transaction
            CardMemberDepositTransaction cmt = new()
            {
                CardMemberDepositID = data.ID,
                Amount = data.TotalDeposit,
                CardMemberID = data.CardMemberID,
                CumulativeBalance = cus.Balance,
                CusID = cus.ID,
                ID = 0,
                DocTypeID = data.DocTypeID,
                Number = data.Number,
                PostingDate = data.PostingDate,
                SeriesDID = data.SeriesDID,
                SeriesID = data.SeriesID,
                UserID = data.UserID
            };
            _context.CardMemberDepositTransactions.Update(cmt);
            _context.SaveChanges();
        }

        public async Task<List<CardMemberDeposit>> GetDepositCardMemberHistoryAsync(int id, string dateFrom, string dateTo)
        {
            List<BusinessPartner> businessPartners = new();
            List<CardMemberDeposit> cardmems = new();
            _ = DateTime.TryParse(dateTo, out DateTime _toDate);
            _ = DateTime.TryParse(dateFrom, out DateTime _fromDate);
            if (id == 0)
            {
                businessPartners = _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer").ToList();
            }
            if (id > 0)
            {
                businessPartners = _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer" && i.ID == id).ToList();
            }
            if (dateFrom == null || dateTo == null)
            {
                cardmems = _context.CardMemberDeposits.ToList();
            }
            else
            {
                cardmems = _context.CardMemberDeposits.Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate).ToList();
            }

            var data = (from cm in cardmems
                        join cus in businessPartners on cm.CusID equals cus.ID
                        join series in _context.Series on cm.SeriesID equals series.ID
                        join card in _context.CardMembers on cm.CardMemberID equals card.ID
                        join pl in _context.PriceLists on cus.PriceListID equals pl.ID
                        join doc in _context.DocumentTypes on cm.DocTypeID equals doc.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        select new CardMemberDeposit
                        {
                            ID = cm.ID,
                            CusID = cm.CusID,
                            CardMemberID = cm.CardMemberID,
                            CustomerName = cus.Name,
                            DocTypeID = cm.DocTypeID,
                            SeriesID = series.ID,
                            Number = $"{series.Name}-{cm.Number}",
                            PostingDate = cm.PostingDate,
                            CardName = card.Name,
                            PostingDateFormat = cm.PostingDate.ToShortDateString(),
                            PriceListName = pl.Name,
                            SeriesDID = cm.SeriesDID,
                            TotalDeposit = cm.TotalDeposit,
                            UserID = cm.UserID,
                            DocCode = doc.Code,
                            TotalDepositF = $"{cur.Description} {string.Format("{0:#,0.00}", cm.TotalDeposit)}",
                        }).ToList();
            return await Task.FromResult(data);
        }

        public async Task<List<CardMemberDepositTransaction>> GetCardTransactionsAsync(int id, string dateFrom, string dateTo)
        {
            List<BusinessPartner> businessPartners = new();
            List<CardMemberDepositTransaction> cardmems = new();
            _ = DateTime.TryParse(dateTo, out DateTime _toDate);
            _ = DateTime.TryParse(dateFrom, out DateTime _fromDate);
            if (id == 0)
            {
                businessPartners = _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer").ToList();
            }
            if (id > 0)
            {
                businessPartners = _context.BusinessPartners.Where(i => !i.Delete && i.Type == "Customer" && i.ID == id).ToList();
            }
            if (dateFrom == null || dateTo == null)
            {
                cardmems = _context.CardMemberDepositTransactions.ToList();
            }
            else
            {
                cardmems = _context.CardMemberDepositTransactions.Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate).ToList();
            }

            var data = (from cm in cardmems
                        join cus in businessPartners on cm.CusID equals cus.ID
                        join series in _context.Series on cm.SeriesID equals series.ID
                        join card in _context.CardMembers on cm.CardMemberID equals card.ID
                        join pl in _context.PriceLists on cus.PriceListID equals pl.ID
                        join doc in _context.DocumentTypes on cm.DocTypeID equals doc.ID
                        join cur in _context.Currency on pl.CurrencyID equals cur.ID
                        select new CardMemberDepositTransaction
                        {
                            ID = cm.ID,
                            CusID = cm.CusID,
                            CardMemberID = cm.CardMemberID,
                            CustomerName = cus.Name,
                            DocTypeID = cm.DocTypeID,
                            SeriesID = series.ID,
                            Number = $"{series.Name}-{cm.Number}",
                            PostingDate = cm.PostingDate,
                            CardName = card.Name,
                            PostingDateFormat = cm.PostingDate.ToShortDateString(),
                            PriceListName = pl.Name,
                            SeriesDID = cm.SeriesDID,
                            UserID = cm.UserID,
                            DocCode = doc.Code,
                            InAmount = cm.Amount > 0 ? $"{cur.Description} {string.Format("{0:#,0.00}", cm.Amount)}" : "",
                            OutAmount = cm.Amount < 0 ? $"{cur.Description} {string.Format("{0:#,0.00}", cm.Amount * -1)}" : "",
                            CumulativeBalanceDisplay = $"{cur.Description} {string.Format("{0:#,0.00}", cm.CumulativeBalance)}",
                        }).ToList() ?? new List<CardMemberDepositTransaction>();
            return await Task.FromResult(data);
        }
        public async Task<bool> RenewExpireDateCardAsync(RenewCardParamsObject data, ModelStateDictionary modelState)
        {
            try
            {
                var card = await _context.CardMembers.FindAsync(data.CardID);
                BusinessPartner customer = await _context.BusinessPartners.FindAsync(data.CusID);
                if (card == null) modelState.AddModelError("Card", "Card not found!");
                if (customer == null) modelState.AddModelError("Customer", "Customer not found!");
                if (data.DateFrom == null) modelState.AddModelError("DateFrom", "Extend date from is require!");
                if (data.DateTo == null) modelState.AddModelError("DateTo", "Extend date to is require!");
                if (modelState.IsValid)
                {
                    using var t = _context.Database.BeginTransaction();
                    RenewCardHistory renewCardHistory = new()
                    {
                        LengthExpireCard = data.LengthExpireCard,
                        CusID = data.CusID,
                        CardID = data.CardID,
                        ID = 0,
                        LastDateExpirationFrom = card.ExpireDateFrom,
                        LastDateExpirationTo = card.ExpireDateTo,
                        RenewDateFrom = DateTime.Parse(data.DateFrom),
                        RenewDateTo = DateTime.Parse(data.DateTo),
                    };
                    card.ExpireDateTo = DateTime.Parse(data.DateTo);
                    card.ExpireDateFrom = DateTime.Parse(data.DateFrom);
                    card.LengthExpireCard = data.LengthExpireCard;

                    await _context.RenewCardHistories.AddAsync(renewCardHistory);
                    _context.CardMembers.Update(card);
                    await _context.SaveChangesAsync();
                    t.Commit();
                    return true;
                }
                else return false;
            }
            catch (Exception err)
            {
                modelState.AddModelError("Error", err.Message);
                return false;
            }
        }
        public async Task<List<RenewCardHistoryModel>> GetRenewHistoryAsync(int cardId)
        {
            var data = await (from card in _context.CardMembers.Where(i => i.ID == cardId)
                              join renew in _context.RenewCardHistories on card.ID equals renew.CardID
                              join cus in _context.BusinessPartners on renew.CusID equals cus.ID
                              select new RenewCardHistoryModel
                              {
                                  ID = renew.ID,
                                  CardHolderName = cus.Name,
                                  CardName = card.Name,
                                  CardNumber = card.Code,
                                  LastDateExpirationFrom = renew.LastDateExpirationFrom.ToShortDateString(),
                                  LastDateExpirationTo = renew.LastDateExpirationTo.ToShortDateString(),
                                  LengthExpireCard = renew.LengthExpireCard.GetDisplayName(),
                                  RenewDateFrom = renew.RenewDateFrom.ToShortDateString(),
                                  RenewDateTo = renew.RenewDateTo.ToShortDateString(),
                              }).ToListAsync();
            return data;
        }
    }
}
