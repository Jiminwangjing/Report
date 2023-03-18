using CKBS.AppContext;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.SearchItemView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class SearchGlobleController : Controller
    {
        private readonly DataContext _context;
        public SearchGlobleController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Getsearchgbloble()
        {
            var search = await _context.Menu.ToListAsync();
            return Ok(search);
        }

        public IActionResult SearchAllItem(string keyword)
        {

            var itemmas = (from i in _context.ItemMasterDatas
                           select new ItemMasterDataView
                           {
                               Code = i.Code,
                               ID = i.ID,
                               EnglishName = i.EnglishName,
                               KhmerName = i.KhmerName
                           }).ToList();
            var receipt = (from i in _context.Receipt
                           select new RecieptView
                           {
                               ReceiptNo = i.ReceiptNo,
                               ReceiptID = i.ReceiptID,
                               OrderNO = i.OrderNo

                           }).ToList();
            var AccBalanc = (from i in _context.AccountBalances
                             select new AccountBalanceView
                             {
                                 ID = i.ID,
                                 OfsetAccount = i.OffsetAccount,
                                 Detail = i.Details

                             }).ToList();
            var ItemAcc = (from i in _context.ItemAccountings
                           select new ItemAccountView
                           {
                               ID = i.ID,
                               ExpenesAcc = i.ExpenseAccount,
                               RevenueAcc = i.RevenueAccount,
                               InventoryAcc = i.InventoryAccount

                           }).ToList();
            var Company = (from i in _context.Company
                           select new CompanyView
                           {
                               ID = i.ID,
                               Name = i.Name,
                               Location = i.Location
                           }).ToList();
            var Brand = (from i in _context.Branches
                         select new BrandsView
                         {
                             ID = i.ID,
                             BrandName = i.Name,
                             Location = i.Location
                         }).ToList();
            var UserAcc = (from i in _context.UserAccounts
                           select new UserAccountView
                           {
                               ID = i.ID,
                               UserName = i.Username

                           }).ToList();
            var PrinterName = (from i in _context.PrinterNames
                               select new PrinterNameView
                               {
                                   ID = i.ID,
                                   PrinterName = i.Name,
                                   OrderCount = i.OrderCount
                               }).ToList();
            var ItemgG1 = (from i in _context.ItemGroup1
                           select new ItemGroup1view
                           {
                               ID = i.ItemG1ID,
                               NameGroup = i.Name
                           }).ToList();
            var ItemgG2 = (from i in _context.ItemGroup2
                           select new ItemGroup2view
                           {
                               ID = i.ItemG2ID,
                               NameGroup = i.Name
                           }).ToList();
            var ItemgG3 = (from i in _context.ItemGroup3
                           select new ItemGroup3view
                           {
                               ID = i.ItemG2ID,
                               NameGroup = i.Name
                           }).ToList();
            var Pro = (from i in _context.Property
                       select new Propertiesview
                       {
                           ID = i.ID,
                           Name = i.Name,
                           Active = i.Active
                       }).ToList();
            var Table = (from i in _context.Tables
                         select new Tableview
                         {
                             ID = i.ID,
                             Name = i.Name,
                             //Status = i.Status
                         }).ToList();
            var GTable = (from i in _context.GroupTables
                          select new GroupTableview
                          {
                              ID = i.ID,
                              Name = i.Name,
                              Type = i.Types,

                          }).ToList();
            var Freights = (from i in _context.Freights
                            select new Freightview
                            {
                                ID = i.ID,
                                Name = i.Name
                            }).ToList();
            var Remarkdis = (from i in _context.RemarkDiscounts
                             select new Remarkdis
                             {
                                 ID = i.ID,
                                 Name = i.Remark
                             }).ToList();
            var per = (from i in _context.PeriodIndicators
                       select new PeriodIndicatorview
                       {
                           ID = i.ID,
                           Name = i.Name
                       }).ToList();
            var Posting = (from i in _context.PostingPeriods
                           select new Postingperiodview
                           {
                               ID = i.ID,
                               Name = i.PeriodName,
                               PeriodCode = i.PeriodCode,
                           }).ToList();
            var chartacc = (from i in _context.GLAccounts
                            select new ChartAccview
                            {
                                ID = i.ID,
                                Name = i.Name,
                                Code = i.Code
                            }).ToList();
            var Journal = (from i in _context.JournalEntries
                           select new JournalEntryview
                           {
                               ID = i.ID,
                               Number = i.Number,
                               Tranno = i.TransNo,
                               Remarks = i.Remarks
                           }).ToList();
            var Pricelist = (from i in _context.PriceLists
                             select new Pricelistview
                             {
                                 ID = i.ID,
                                 Name = i.Name
                             }).ToList();
            var Businespartner = (from i in _context.BusinessPartners
                                  select new BusinesPartnerview
                                  {
                                      ID = i.ID,
                                      Name = i.Name,
                                      Code = i.Code,
                                      Type = i.Type,
                                      Phone = i.Phone,
                                      Email = i.Email,
                                      Address = i.Address

                                  }).ToList();
            var Cardmember = (from i in _context.CardMembers
                              select new Cardsmemberview
                              {
                                  ID = i.ID,
                                  Name = i.Name,
                                  Code = i.Code,
                                  Discription = i.Description
                              }).ToList();
            var currency = (from i in _context.Currency
                            select new Currencyview
                            {
                                ID = i.ID,
                                Symbol = i.Symbol,
                                Description = i.Description
                            }).ToList();
            var Paymeant = (from i in _context.PaymentMeans
                            select new Paymeantview
                            {
                                ID = i.ID,
                                Type = i.Type,
                                AccountID = i.AccountID.ToString()
                            }).ToList();
            var Employee = (from i in _context.Employees
                            select new Employeeview
                            {
                                ID = i.ID,
                                Name = i.Name,
                                Code = i.Code,
                                Gender = i.Gender.ToString(),
                                Address = i.Address,
                                Phone = i.Phone,
                                Email = i.Email,
                                Positon = i.Position

                            }).ToList();
            var Setupservice = (from i in _context.ServiceSetups
                                select new Setupserviceview
                                {
                                    ID = i.ID,
                                    Setupdcode = i.SetupCode,
                                    Price = i.Price.ToString(),
                                    Remark = i.Remark
                                }).ToList();
            var Receiptinfo = (from i in _context.ReceiptInformation
                               select new Receiptinformationview
                               {
                                   ID = i.ID,
                                   Title = i.Title,
                                   Address = i.Address,
                                   Tel1 = i.Tel1,
                                   Tel2 = i.Tel2,
                                   Khmername = i.KhmerDescription,
                                   Englishname = i.EnglishDescription
                               }).ToList();
            var Function = (from i in _context.Functions
                            select new Functionview
                            {
                                ID = i.ID,
                                Name = i.Name,
                                Type = i.Type,
                                Code = i.Code
                            }).ToList();
            var Unitmeasure = (from i in _context.UnitofMeasures
                               select new Unitofmeasureview
                               { 
                                ID = i.ID,
                                Name = i.Name,
                                Code = i.Code
                               }).ToList();
            var guom = (from i in _context.GroupUOMs
                               select new GUOMview
                               {
                                   ID = i.ID,
                                   Name = i.Name,
                                   Code = i.Code
                               }).ToList();
            var warhouse = (from i in _context.Warehouses
                            select new Warhouseview
                            { 
                                ID = i.ID,
                                Code = i.Code,
                                Name =i.Name,
                                Location = i.Location,
                                Address = i.Address
                            }).ToList();



            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                receipt = FilterSearch(receipt,
                    c => RawWord(c.ReceiptNo).Contains(keyword, ignoreCase)
                        || RawWord(c.OrderNO).Contains(keyword, ignoreCase));
                //itemmas.Where().ToList();
                itemmas = FilterSearch(itemmas,
                    c => RawWord(c.KhmerName).Contains(keyword, ignoreCase)
                    || RawWord(c.EnglishName).Contains(keyword, ignoreCase)
                    || RawWord(c.Code).Contains(keyword, ignoreCase));

                AccBalanc = FilterSearch(AccBalanc,
                    c => RawWord(c.OfsetAccount).Contains(keyword, ignoreCase)
                    || RawWord(c.Detail).Contains(keyword, ignoreCase));

                ItemAcc = FilterSearch(ItemAcc,
                    c => RawWord(c.ExpenesAcc).Contains(keyword, ignoreCase)
                    || RawWord(c.RevenueAcc).Contains(keyword, ignoreCase)
                    || RawWord(c.InventoryAcc).Contains(keyword, ignoreCase));
                Company = FilterSearch(Company,
                    c => RawWord(c.Name).Contains(keyword, ignoreCase)
                    || RawWord(c.Location).Contains(keyword, ignoreCase));
                Brand = FilterSearch(Brand,
                    c => RawWord(c.BrandName).Contains(keyword, ignoreCase)
                    || RawWord(c.Location).Contains(keyword, ignoreCase));
                UserAcc = FilterSearch(UserAcc,
                    c => RawWord(c.UserName).Contains(keyword, ignoreCase));
                PrinterName = FilterSearch(PrinterName,
                    c => RawWord(c.PrinterName).Contains(keyword, ignoreCase));
                ItemgG1 = FilterSearch(ItemgG1,
                    c => RawWord(c.NameGroup).Contains(keyword, ignoreCase));
                ItemgG2 = FilterSearch(ItemgG2,
                    c => RawWord(c.NameGroup).Contains(keyword, ignoreCase));
                ItemgG3 = FilterSearch(ItemgG3,
                    c => RawWord(c.NameGroup).Contains(keyword, ignoreCase));
                Pro = FilterSearch(Pro,
                    c => RawWord(c.Name).Contains(keyword, ignoreCase)
                    || RawWord(c.Active.ToString()).Contains(keyword, ignoreCase));

                Table = FilterSearch(Table,
                     c => RawWord(c.Name).Contains(keyword, ignoreCase)
                     || RawWord(c.Status.ToString()).Contains(keyword, ignoreCase));

                GTable = FilterSearch(GTable,
                     c => RawWord(c.Name).Contains(keyword, ignoreCase)
                     || RawWord(c.Type).Contains(keyword, ignoreCase));
                Freights = FilterSearch(Freights,
                     c => RawWord(c.Name).Contains(keyword, ignoreCase));
                Remarkdis = FilterSearch(Remarkdis,
                     c => RawWord(c.Name).Contains(keyword, ignoreCase));
                per = FilterSearch(per,
                     c => RawWord(c.Name).Contains(keyword, ignoreCase));
                Posting = FilterSearch(Posting,
                      c => RawWord(c.Name).Contains(keyword, ignoreCase)
                      || RawWord(c.PeriodCode.ToString()).Contains(keyword, ignoreCase));
                chartacc = FilterSearch(chartacc,
                       c => RawWord(c.Name).Contains(keyword, ignoreCase)
                       || RawWord(c.Code.ToString()).Contains(keyword, ignoreCase));
                Journal = FilterSearch(Journal,
                      c => RawWord(c.Number).Contains(keyword, ignoreCase)
                      || RawWord(c.Tranno.ToString()).Contains(keyword, ignoreCase)
                      || RawWord(c.Remarks.ToString()).Contains(keyword, ignoreCase));
                Pricelist = FilterSearch(Pricelist,
                       c => RawWord(c.Name).Contains(keyword, ignoreCase));

                Businespartner = FilterSearch(Businespartner,
                        c => RawWord(c.Name).Contains(keyword, ignoreCase)
                        || RawWord(c.Code).Contains(keyword, ignoreCase)
                        || RawWord(c.Type).Contains(keyword, ignoreCase)
                        || RawWord(c.Phone).Contains(keyword, ignoreCase)
                        || RawWord(c.Email).Contains(keyword, ignoreCase)
                        || RawWord(c.Address).Contains(keyword, ignoreCase));


                Cardmember = FilterSearch(Cardmember,
                          c => RawWord(c.Name).Contains(keyword, ignoreCase)
                          || RawWord(c.Code).Contains(keyword, ignoreCase)
                          || RawWord(c.Discription).Contains(keyword, ignoreCase));

                currency = FilterSearch(currency,
                          c => RawWord(c.Symbol).Contains(keyword, ignoreCase)
                          || RawWord(c.Description).Contains(keyword, ignoreCase));
                Paymeant = FilterSearch(Paymeant,
                          c => RawWord(c.Type).Contains(keyword, ignoreCase)
                          || RawWord(c.AccountID).Contains(keyword, ignoreCase));
                Employee = FilterSearch(Employee,
                          c => RawWord(c.Name).Contains(keyword, ignoreCase)
                          || RawWord(c.Code).Contains(keyword, ignoreCase)
                          || RawWord(c.Address).Contains(keyword, ignoreCase)
                          || RawWord(c.Phone).Contains(keyword, ignoreCase)
                          || RawWord(c.Email).Contains(keyword, ignoreCase)
                          || RawWord(c.Positon).Contains(keyword, ignoreCase));
                Setupservice = FilterSearch(Setupservice,
                          c => RawWord(c.Setupdcode).Contains(keyword, ignoreCase)
                          || RawWord(c.Price).Contains(keyword, ignoreCase)
                          || RawWord(c.Remark).Contains(keyword, ignoreCase));
                Receiptinfo = FilterSearch(Receiptinfo,
                          c => RawWord(c.Title).Contains(keyword, ignoreCase)
                          || RawWord(c.Address).Contains(keyword, ignoreCase)
                          || RawWord(c.Tel1).Contains(keyword, ignoreCase)
                          || RawWord(c.Tel2).Contains(keyword, ignoreCase)
                          || RawWord(c.Khmername).Contains(keyword, ignoreCase)
                          || RawWord(c.Englishname).Contains(keyword, ignoreCase));
               Function = FilterSearch(Function,
                          c => RawWord(c.Name).Contains(keyword, ignoreCase)
                          || RawWord(c.Type).Contains(keyword, ignoreCase)
                          || RawWord(c.Code).Contains(keyword, ignoreCase));

                Unitmeasure = FilterSearch(Unitmeasure,
                           c => RawWord(c.Name).Contains(keyword, ignoreCase)
                           || RawWord(c.Code).Contains(keyword, ignoreCase));
                guom = FilterSearch(guom,
                           c => RawWord(c.Name).Contains(keyword, ignoreCase)
                           || RawWord(c.Code).Contains(keyword, ignoreCase));
                warhouse = FilterSearch(warhouse,
                           c => RawWord(c.Name).Contains(keyword, ignoreCase)
                           || RawWord(c.Code).Contains(keyword, ignoreCase)
                           || RawWord(c.Location).Contains(keyword, ignoreCase)
                           || RawWord(c.Address).Contains(keyword, ignoreCase));

            }
            int totalItems = itemmas.Count + receipt.Count+AccBalanc.Count+ItemAcc.Count+Company.Count+Brand.Count
                             +UserAcc.Count+PrinterName.Count+ItemgG1.Count+ItemgG2.Count+ItemgG3.Count+Pro.Count
                             +Table.Count+GTable.Count+Freights.Count+Remarkdis.Count+per.Count+Posting.Count+chartacc.Count
                             +Journal.Count+Pricelist.Count+Businespartner.Count+Cardmember.Count+currency.Count+
                             Paymeant.Count+Employee.Count+Setupservice.Count+Receiptinfo.Count+Function.Count+Unitmeasure.Count
                             +warhouse.Count;
            ItemMasterSearch itemMasters = new()
            {
                ItemMaster = itemmas,
                Receipts = receipt,
                AccountBalances = AccBalanc,
                ItemAccounts = ItemAcc,
                Companys = Company,
                BrandsViews = Brand,
                UserAccounts = UserAcc,
                PrinterNames = PrinterName,
                ItemGroup1s = ItemgG1,
                ItemGroup2s = ItemgG2,
                ItemGroup3s = ItemgG3,
                Property = Pro,
                Table = Table,
                Groups = GTable,
                Freights = Freights,
                Remarkdis = Remarkdis,
                Per = per,
                Posting = Posting,
                Chartsacc = chartacc,
                Journal = Journal,
                Pricelist = Pricelist,
                Busines = Businespartner,
                Cardsmember = Cardmember,
                Currency = currency,
                Payment = Paymeant,
                Emplyee = Employee,
                Setup = Setupservice,
                ReceiptIfo = Receiptinfo,
                Function = Function,
                Unitmeasure = Unitmeasure,
                Guom = guom,
                Warhous = warhouse,

                TotalItems = totalItems,
            };


            //var itemmaster =await _context.ItemMasterDatas.ToListAsync();

            return Ok(itemMasters);
        }

        private static List<T> FilterSearch<T>(List<T> data, Func<T, bool> predicate)
        {
            return data.Where(predicate).ToList();
        }
        static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }
    }
}

