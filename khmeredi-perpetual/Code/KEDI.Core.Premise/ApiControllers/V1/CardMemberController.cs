using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.ServicesClass;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.CardMembers;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Services.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.ApiControllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [JwtAuthorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CardMemberController : Controller
    {
        private readonly ICardMemberRepository _cardMember;
        private readonly DataContext _context;
        private readonly UserManager _userModule;
        public CardMemberController(ICardMemberRepository cardMember, DataContext context, UserManager userModule)
        {
            _cardMember = cardMember;
            _context = context;
            _userModule = userModule;
        }

        public Dictionary<int, string> TypeCardDiscountTypes => EnumHelper.ToDictionary(typeof(TypeCardDiscountType));
        [HttpGet("getSeries")]
        public IActionResult GetSeries()
        {
            var seriesCM = (from dt in _context.DocumentTypes.Where(i => i.Code == "CM")
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
            return Ok(seriesCM);
        }

        // [HttpGet("depositCardMemberHistory")]
        // public async Task<IActionResult> DepositCardMemberHistory()
        // {
        //     var cus = await _cardMember.GetCustomerDipositAsync();
        //     return Ok(cus);
        // }

        // [HttpGet("cardTransaction")]
        // public async Task<IActionResult> CardTransaction()
        // {
        //     var cus = await _cardMember.GetCustomerDipositAsync();
        //     return Ok(cus);
        // }

        [HttpGet("getDepositCardMemberHistory")]
        public async Task<IActionResult> GetDepositCardMemberHistory(int id, string dateFrom, string dateTo)
        {
            var data = await _cardMember.GetDepositCardMemberHistoryAsync(id, dateFrom, dateTo);
            return Ok(data);
        }

        [HttpGet("getCardTransactions")]
        public async Task<IActionResult> GetCardTransactions(int id, string dateFrom, string dateTo)
        {
            var data = await _cardMember.GetCardTransactionsAsync(id, dateFrom, dateTo);
            return Ok(data);
        }

        // [HttpPost("depositCardMember")]
        // public async Task<IActionResult> DepositCardMember([FromBody] CardMemberDeposit data)
        // {
        //     SeriesDetail seriesDetail = new SeriesDetail();
        //     ModelMessage msg = new();
        //     ValidateDepositCardMember(data);

        //     var series = _context.Series.Find(data.SeriesID);
        //     var cardMemberAcc = _context.AccountMemberCards.FirstOrDefault(i => i.CashAccID > 0 && i.UnearnedRevenueID > 0);
        //     var card = await _cardMember.GetRegisterMemberDetialAsync(data.CardMemberID);
        //     if (card == null) ModelState.AddModelError("card", "Could not find card!");
        //     else
        //     {
        //         if (card.ExpireDateTo < DateTime.Today) ModelState.AddModelError("card", "Card is expired!");
        //     }

        //     if (cardMemberAcc == null)
        //     {
        //         ModelState.AddModelError("mappingAcc", "You need to map Member Card Account first, in order to do this transaction!");
        //     }

        //     if (series == null)
        //     {
        //         ModelState.AddModelError("Series", "Series is missing!");
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         using var t = _context.Database.BeginTransaction();
        //         data.SeriesID = series.ID;
        //         data.Number = series.NextNo;
        //         data.UserID = GetUserID();
        //         // insert Series Detail
        //         seriesDetail.Number = series.NextNo;
        //         seriesDetail.SeriesID = series.ID;
        //         _context.Entry(data).State = EntityState.Detached;
        //         _context.SeriesDetails.Update(seriesDetail);
        //         _context.SaveChanges();
        //         data.SeriesDID = seriesDetail.ID;
        //         string Sno = series.NextNo;
        //         long No = long.Parse(Sno);
        //         series.NextNo = Convert.ToString(No + 1);
        //         if (No > long.Parse(series.LastNo))
        //         {
        //             ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
        //             return Ok(msg.Bind(ModelState));
        //         }
        //         _cardMember.DepositCardMember(data, GetCompany(), cardMemberAcc);
        //         msg.Approve();
        //         msg.AddItem(series, "series");
        //         t.Commit();
        //         ModelState.AddModelError("success", "Transaction completed successfully!");
        //     }

        //     return Ok(msg.Bind(ModelState));
        // }

        [HttpGet("typeCardCreate/{id}")]
        public async Task<IActionResult> TypeCardCreate(int id)
        {
            ViewBag.TypeCardDiscountTypes = TypeCardDiscountTypes.Select(i => new SelectListItem
            {
                Text = i.Value,
                Value = i.Key.ToString()
            });
            var data = await _cardMember.GetTypeCardAsync(id);
            return Ok(data);
        }

        // [HttpGet("registerMember/{id}")]
        // public IActionResult RegisterMember(int id)
        // {
        //     var typeCards = _context.TypeCards.Where(i => !i.IsDeleted).ToList();
        //     return Ok(typeCards);
        // }

        [HttpGet("renewExpireDateCard/{id}")]
        public async Task<IActionResult> RenewExpireDateCard(int id)
        {
            var data = await _cardMember.GetRegisterMemberDetialAsync(id);
            return Ok(data);
        }

        [HttpPost("renewExpireDateCard")]
        public async Task<IActionResult> RenewExpireDateCard([FromBody] RenewCardParamsObject data)
        {
            ModelMessage msg = new();
            bool success = await _cardMember.RenewExpireDateCardAsync(data, ModelState);
            if (success)
            {
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            else
            {
                msg.Reject();
                return Ok(msg.Bind(ModelState));
            }
        }

        // [HttpPost("registerMember")]
        // public async Task<IActionResult> RegisterMember([FromBody] CardMember data)
        // {
        //     ModelMessage msg = new();
        //     await ValidateCardMember(data);
        //     using var t = _context.Database.BeginTransaction();
        //     if (ModelState.IsValid)
        //     {
        //         await _cardMember.RegisterMemberAsync(data);
        //         msg.Approve();
        //         ModelState.AddModelError("Success", "Card saved successfully");
        //         t.Commit();
        //     }
        //     return Ok(msg.Bind(ModelState));
        // }

        [HttpGet("getRegisteredMembers")]
        public async Task<IActionResult> GetRegisterMembers()
        {
            var data = await _cardMember.GetRegisterMembersAsync();
            return Ok(data);
        }

        [HttpGet("getRegisteredMemberDetail")]
        public async Task<IActionResult> GetRegisterMemberDetial(int id)
        {
            var data = await _cardMember.GetRegisterMemberDetialAsync(id);
            return Ok(data);
        }

        [HttpGet("getRegisteredMemberDetailByCode/{code}/{active?}")]
        public async Task<IActionResult> GetRegisterMemberDetialByCode(string code, bool active = false)
        {
            if (active)
            {
                var data = await _cardMember.GetRegisterMemberDetialAsync(code);
                if (data != null)
                {
                    if (!data.Active)
                    {
                        return Ok("Card is inactive!");
                    }
                }
                else
                {
                    return Ok("Card not found!");
                }
                return Ok(data);
            }
            else
            {
                var data = await _cardMember.GetRegisterMemberDetialAsync(code);
                return Ok(data);
            }
        }

        [HttpGet("getCustomer")]
        public async Task<IActionResult> GetCustomer()
        {
            var data = await _cardMember.GetCustomerAsync();
            return Ok(data);
        }

        [HttpPost("checkMemberInCard")]
        public IActionResult CheckMemberInCard([FromBody] BusinessPartner bp)
        {
            var card = _context.CardMembers.Find(bp.CardMemberID);
            return Ok($"Custmer name \"{bp.Name}\" already has card name \"{card.Name}\"");
        }

        [HttpGet("getCustomerDiposit")]
        public async Task<IActionResult> GetCustomerDiposit()
        {
            var data = await _cardMember.GetCustomerDipositAsync();
            return Ok(data);
        }

        [HttpPost("typeCardCreateOrUpdate")]
        public async Task<IActionResult> TypeCardCreateOrUpdate([FromBody] TypeCard typeCard)
        {
            ModelMessage msg = new();
            await ValidateTypeCard(typeCard);
            if (ModelState.IsValid)
            {
                await _cardMember.CreateUpdateAsync(typeCard);
                msg.Approve();
                ModelState.AddModelError("success", "item saved successfully!");
            }
            return Ok(msg.Bind(ModelState));
        }

        private async Task ValidateTypeCard(TypeCard typeCard)
        {
            var typeCards = await _context.TypeCards.Where(i => !i.IsDeleted).AsNoTracking().ToListAsync();
            var _typeCard = await _context.TypeCards.AsNoTracking().FirstOrDefaultAsync(i => i.ID == typeCard.ID);
            if (string.IsNullOrEmpty(typeCard.Code))
            {
                ModelState.AddModelError("Code", "Code cannot be empty!");
            }
            if (string.IsNullOrEmpty(typeCard.Name))
            {
                ModelState.AddModelError("Name", "Name cannot be empty!");
            }
            if (typeCard.ID == 0)
            {
                if (typeCards.Any(i => i.Code == typeCard.Code))
                {
                    ModelState.AddModelError("Code", "Code is already existed!");
                }
            }
            if (typeCard.ID > 0)
            {
                if (_typeCard != null)
                {
                    if (typeCard.Code != _typeCard.Code)
                    {
                        if (typeCards.Any(i => i.Code == typeCard.Code))
                        {
                            ModelState.AddModelError("Code", "Code is already existed!");
                        }
                    }
                }
            }

        }
        private async Task ValidateCardMember(CardMember data)
        {
            var cards = await _context.CardMembers.AsNoTracking().ToListAsync();
            var card = await _context.CardMembers.AsNoTracking().FirstOrDefaultAsync(i => i.ID == data.ID);
            if (string.IsNullOrEmpty(data.Code))
            {
                ModelState.AddModelError("Code", "Code cannot be empty!");
            }
            if (string.IsNullOrEmpty(data.Name))
            {
                ModelState.AddModelError("Name", "Name cannot be empty!");
            }
            if (data.ID == 0)
            {
                if (cards.Any(i => i.Code == data.Code))
                {
                    ModelState.AddModelError("Code", "Code is already existed!");
                }
            }
            if (data.ID > 0)
            {
                if (card != null)
                {
                    if (data.Code != card.Code)
                    {
                        if (cards.Any(i => i.Code == data.Code))
                        {
                            ModelState.AddModelError("Code", "Code is already existed!");
                        }
                    }
                }
            }
            if (data.TypeCardID <= 0)
            {
                ModelState.AddModelError("TypeCardID", "Please select any type card!");
            }
            if (data.Customer == null)
            {
                ModelState.AddModelError("Customer", "Please choose any customer!");
            }
            if (data.LengthExpireCard != LengthExpireCard.None)
                await SetCardExpiryPeriod(data);
        }
        private async Task SetCardExpiryPeriod(CardMember data)
        {
            if (data.ID == 0)
            {
                DateTime dateNow = DateTime.Today;
                data.ExpireDateFrom = dateNow;
                if (data.LengthExpireCard == LengthExpireCard.ThreeMonths)
                {
                    data.ExpireDateTo = dateNow.AddMonths(3);
                }
                if (data.LengthExpireCard == LengthExpireCard.SixMonths)
                {
                    data.ExpireDateTo = dateNow.AddMonths(6);
                }
                if (data.LengthExpireCard == LengthExpireCard.OneYear)
                {
                    data.ExpireDateTo = dateNow.AddMonths(12);
                }
            }
            else
            {
                var _data = await _cardMember.GetRegisterMemberDetialAsync(data.ID);
                if (_data.ExpireDateFrom.Year != 0001) data.ExpireDateFrom = _data.ExpireDateFrom;
                if (data.LengthExpireCard == LengthExpireCard.ThreeMonths)
                {
                    data.ExpireDateTo = _data.ExpireDateFrom.AddMonths(3);
                }
                if (data.LengthExpireCard == LengthExpireCard.SixMonths)
                {
                    data.ExpireDateTo = _data.ExpireDateFrom.AddMonths(6);
                }
                if (data.LengthExpireCard == LengthExpireCard.OneYear)
                {
                    data.ExpireDateTo = _data.ExpireDateFrom.AddMonths(12);
                }
            }
        }

        [HttpGet("getRenewExpirationHistory/{cardId}")]
        public async Task<IActionResult> GetRenewExpirationHistory(int cardId)
        {
            var data = await _cardMember.GetRenewHistoryAsync(cardId);
            return Ok(data);
        }

        private void ValidateDepositCardMember(CardMemberDeposit data)
        {
            if (data.PostingDate < DateTime.Today)
            {
                ModelState.AddModelError("PostingDate", "Invalid Date!");
            }
            if (data.CusID <= 0)
            {
                ModelState.AddModelError("CusID", "Please choose any customer!");
            }
            if (data.TotalDeposit <= 0)
            {
                ModelState.AddModelError("Amount", "Amount has to be greater than 0!");
            }
        }

        [HttpGet("getCardTypes/{isDeleted?}")]
        public async Task<IActionResult> GetCardTypes(bool isDeleted = false)
        {
            var data = await _cardMember.GetCardTypesAsync(isDeleted);
            return Ok(data);
        }

        [HttpGet("deleteCardType/{id}")]
        public async Task<IActionResult> DeleteCardType(int id)
        {
            var message = await _cardMember.DeleteCardTypeAsync(id);
            return Ok(message);
        }

        [HttpGet("getCodeCardRamdom")]
        public IActionResult GetCodeCardRamdom()
        {
            var strignCode = GetNumberString();
            return Ok(strignCode);
        }

        private static string Get13CharacterRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            path = $"CM{path[..9]}";  // Return 8 character string
            return path;
        }

        private static string GetNumberString()
        {
            string path = $"{DateTime.Now.Ticks.ToString()[5..]}";
            return path;
        }
        private int GetUserID()
        {
            int userId = _userModule.GetUserId();
            return userId;
        }
        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

    }
}
