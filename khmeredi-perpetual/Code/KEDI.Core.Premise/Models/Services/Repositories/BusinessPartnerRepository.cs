using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.ServicesClass.ActivityViewModel;
using KEDI.Core.Premise.Models.Services.Activity;
using Microsoft.AspNetCore.Http;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.ChartOfAccounts;
using System.IO;
using NPOI.SS.UserModel;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Utilities;

namespace CKBS.Models.Services.Responsitory
{
    public interface IBusinessPartner
    {
        Task<int> SubmitFromExcelAsync(ModelStateDictionary modelState, IFormFile formFile, int sheetIndex);
        Task<Dictionary<string, List<BusinessPartner>>> GetFromExcelAsync(ModelStateDictionary modelState, IFormFile formFile);
        Task<BusinessPartner> SaveVendorAsync(BusinessPartner vendor, ModelStateDictionary modelState);
        Task<List<PriceLists>> GetPriceListsAsync();
        Task<List<GLAccount>> GetGLAccountsAsync();
        Task<BusinessPartner> SaveCustomerAsync(BusinessPartner customer, ModelStateDictionary modelState);
        Task<int> SaveVehiclesAsync(ModelStateDictionary modelState, int customerId, List<AutoMobile> vehicles);
        Task<List<BusinessPartner>> GetCustomersAsync(string keyword = "");
        Task<BusinessPartner> FindCustomerAsync(int customerId);
        List<SelectListItem> SelectPricelists(int pricelistId = 0, bool disabled = false);
        List<SelectListItem> SelectListTerritory(int Id = 0, string type = null, bool disabled = false);
        IQueryable<BusinessPartner> BusinessPartners { get; }
        IEnumerable<BusinessPartner> BusinessPartner { get; }
        Task Add(BusinessPartner business);
        Task Update(int? id, BusinessPartner business);
        Task Delete(int? id);
        List<SelectListItem> SelectGroup1(int Id = 0, string type = null, bool disabled = false);
        List<SelectListItem> SelectGroup2(int Id = 0, string type = null, bool disabled = false);

        Task AddOrEdit(GroupCustomer1 group1);
        Task AddGroup2OrEdit(GroupCustomer2 group2);
        Task<List<GroupCustomer1>> GetGroup1(string type);
        Task SaveContactPersonAsync(List<ContactPerson> contactPersons, ModelStateDictionary modelState);
        Task SaveBPBranchesAsync(List<BPBranch> bPBranches, ModelStateDictionary modelState);
        Task SaveContractBillingAsync(List<ContractBiling> contractBilings, ModelStateDictionary modelState);
        ActivityView FindActivity(int number);
        BusinessPartner GetCustomerData(int id);
        ContactPerson Contact(int bpid = 0);
        List<ContractBiling> GetServiceContract(int comId, int cusId);
        List<ContactPerson> GetDefultContactPerson();
    }

    public class BusinessPartnerRepository : IBusinessPartner
    {
        private readonly DataContext _context;
        private readonly ILogger<BusinessPartnerRepository> _logger;
        private readonly WorkbookContext _workbook;
        private readonly UtilityModule _fncModule;
        public BusinessPartnerRepository(ILogger<BusinessPartnerRepository> logger, DataContext ctx, UtilityModule format)
        {
            _logger = logger;
            _context = ctx;
            _workbook = new WorkbookContext();
            _fncModule = format;
        }
        public Dictionary<int, string> ContriesOfBirth => EnumHelper.ToDictionary(typeof(ContriesOfBirth));
        public Dictionary<int, string> Genders => EnumHelper.ToDictionary(typeof(Genders));
        //=============enum of contract billing========
        public Dictionary<int, string> Status => new()
        {
            { 0, "-- Select --" },
            { 1, "Full Active" },
            { 2, "Not Full Active" },
        };
        public Dictionary<int, string> ConfirmRenew => new()
        {
            { 0, "-- Select --" },
            { 1, "Yes" },
            { 2, "No" },
        };
        public Dictionary<int, string> Payment => new()
        {
            { 0, "-- Select --" },
            { 1, "Paid" },
            { 2, "Not Yet Paid" },
        };
        public Dictionary<int, string> ContractType => new()
        {
            { 0, "-- Select --" },
            { 1, "New" },
            { 2, "Renewal" },
        };
        // index of customer
        public async Task<int> SubmitFromExcelAsync(ModelStateDictionary modelState, IFormFile formFile, int sheetIndex)
        {
            try
            {
                if (formFile == null) { return -1; }
                if (modelState.IsValid && formFile.Length > 0)
                {
                    using Stream fs = new MemoryStream();
                    await formFile.CopyToAsync(fs);
                    fs.Position = 0;
                    IWorkbook wb = _workbook.ReadWorkbook(fs);
                    var bps = _workbook.ToList<BusinessPartner>(wb.GetSheetAt(sheetIndex));
                    var dbBps = _context.BusinessPartners.AsNoTracking();
                    foreach (var bp in bps)
                    {
                        if (dbBps.Where(_bp => _bp.Code.ToLower() == bp.Code.ToLower()).Any())
                        {
                            modelState.AddModelError(bp.Code, $"Business Partner with code [{bp.Code}] is existing.");
                        }
                    }

                    if (modelState.IsValid)
                    {
                        _context.BusinessPartners.AddRange(bps);
                        return _context.SaveChanges();
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return -1;
            }
        }

        public async Task<Dictionary<string, List<BusinessPartner>>> GetFromExcelAsync(ModelStateDictionary modelState, IFormFile formFile)
        {
            if (modelState.IsValid && formFile.Length > 0)
            {
                using Stream fs = new MemoryStream();
                await formFile.CopyToAsync(fs);
                fs.Position = 0;
                IWorkbook wb = _workbook.ReadWorkbook(fs);
                return _workbook.ToDictionary<BusinessPartner>(wb);
            }
            return new Dictionary<string, List<BusinessPartner>>();
        }

        public async Task<List<PriceLists>> GetPriceListsAsync()
        {
            var pricelists = await _context.PriceLists.Where(pl => !pl.Delete).ToListAsync();
            return pricelists;
        }

        public async Task<List<Dictionary<string, string>>> GetExcelAsync(ModelStateDictionary modelState, IFormFile formFile)
        {
            var keyValues = new List<Dictionary<string, string>>();
            if (modelState.IsValid && formFile.Length > 0)
            {
                using Stream fs = new MemoryStream();
                await formFile.CopyToAsync(fs);
                fs.Position = 0;
                IWorkbook wb = _workbook.ReadWorkbook(fs);
                return _workbook.ParseKeyValue(wb.GetSheetAt(0));
            }
            return keyValues;
        }

        public async Task<List<GLAccount>> GetGLAccountsAsync()
        {
            var glaccts = await _context.GLAccounts.Where(g => g.IsActive && g.IsControlAccount).ToListAsync();
            return glaccts;
        }
        public IQueryable<BusinessPartner> BusinessPartners => _context.BusinessPartners.Where(x => x.Delete == false).Include(x => x.PriceList);

        public IEnumerable<BusinessPartner> BusinessPartner => _context.BusinessPartners.Where(x => x.Delete == false).Include(x => x.PriceList);

        public async Task Add(BusinessPartner business)
        {
            if (business != null && business.ID == 0)
            {
                await _context.AddAsync(business);
            }
            else
            {
                _context.Update(business);
            }
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int? id)
        {
            if (id != null)
            {
                var bp = await _context.BusinessPartners.FindAsync(id);
                if (bp != null)
                {
                    bp.Delete = true;
                    _context.Update(bp);
                    await _context.SaveChangesAsync();
                }

            }
        }

        public async Task Update(int? id, BusinessPartner business)
        {
            if (id != null)
            {
                var bp = await _context.BusinessPartners.FindAsync(id);
                if (bp != null)
                {
                    bp.Code = business.Code;
                    bp.Name = business.Name;
                    bp.Phone = business.Phone;
                    bp.Address = business.Address;
                    bp.Type = business.Type;
                    bp.Email = business.Email;
                    bp.PriceListID = business.PriceListID;
                    _context.Update(bp);
                    await _context.SaveChangesAsync();
                }
            }
        }


        //Mak Sokmanh
        private static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        private bool EqualNotEmpty(string a, string b, bool ignoreCase = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b))
                {
                    return false;
                }
                a = Regex.Replace(a, "\\s+", "");
                b = Regex.Replace(b, "\\s+", "");
                return string.Compare(a, b, ignoreCase) == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        //public async Task<BusinessPartner> SaveCustomerAsync(BusinessPartner customer, ModelStateDictionary modelState)
        //{
        //    try
        //    {
        //        if (customer.ID <= 0)
        //        {
        //            if (_context.BusinessPartners.Any(c => EqualNotEmpty(c.Code, customer.Code, true)))
        //            {
        //                modelState.AddModelError("Code", "Code is already existing.");
        //            }
        //        }

        //        if (modelState.IsValid)
        //        {
        //            customer.Type = "Customer";
        //            _context.BusinessPartners.Update(customer);
        //            await _context.SaveChangesAsync();
        //        }

        //        return customer;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        modelState.AddModelError("Exception", ex.Message);
        //        return await Task.FromResult(new BusinessPartner());
        //    }
        //}


        public async Task<BusinessPartner> SaveCustomerAsync(BusinessPartner customer, ModelStateDictionary modelState)
        {
            try
            {
                if (customer.ID <= 0)
                {
                    if (_context.BusinessPartners.Any(c => EqualNotEmpty(c.Code, customer.Code, true)))
                    {
                        modelState.AddModelError("Code", "Code is already existing.");
                    }
                }
                if (modelState.IsValid)
                {
                    customer.Type = "Customer";
                    customer.GroupID = Convert.ToInt32(customer.Point);
                    _context.BusinessPartners.Update(customer);
                    await _context.SaveChangesAsync();
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
                return await Task.FromResult(new BusinessPartner());
            }
        }

        public List<ContractBiling> GetServiceContract(int comId, int cusId)
        {
            List<ServiceContract> serviceContracts = _context.ServiceContracts.Where(w => w.CompanyID == comId && w.CusID == cusId).ToList();
            var service = (from s in serviceContracts
                           join bp in _context.BusinessPartners on s.CusID equals bp.ID
                           let ac = _context.Activites.Where(x => x.BPID == bp.ID).ToList().Count
                           join doc in _context.DocumentTypes on s.DocTypeID equals doc.ID
                           join curr_pl in _context.Currency on s.SaleCurrencyID equals curr_pl.ID
                           let sd = _context.ServiceContractDetails.Where(i => i.ServiceContractID == s.ID).ToList()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_pl.ID) ?? new Display()
                           let att = _context.AttchmentFiles.Where(x => x.ServiceContractID == s.ID).ToList().Count
                           let con = _context.Contracts.Where(x => x.ID == s.ContractTemplateID).FirstOrDefault() ?? new ContractTemplate()
                           let contype = _context.SetupContractTypes.FirstOrDefault(x => x.ID == con.ContracType) ?? new SetupContractType()
                           select new ContractBiling
                           {
                               SaleID = s.ID,
                               SeriesDID = s.SeriesDID,
                               DocumentNo = s.InvoiceNumber,
                               Amount = $"{curr_pl.Description} {_fncModule.ToCurrency(s.TotalAmount, plCur.Amounts)}",
                               NumExpiresOfDay = s.ContractENDate.Subtract(DateTime.Now).Days.ToString() + " Days",
                               Statuss = Status.Select(P => new SelectListItem
                               {
                                   Value = P.Key.ToString(),
                                   Text = P.Value
                               }).ToList(),
                               Status = 0,
                               ConfrimRenews = ConfirmRenew.Select(P => new SelectListItem
                               {
                                   Value = P.Key.ToString(),
                                   Text = P.Value
                               }).ToList(),
                               Payments = Payment.Select(P => new SelectListItem
                               {
                                   Value = P.Key.ToString(),
                                   Text = P.Value
                               }).ToList(),
                               NewContractStartDate = s.ContractStartDate,
                               NewContractEndDate = s.ContractENDate,

                               NextOpenRenewalDate = DateTime.Now,
                               Renewalstartdate = DateTime.Now,
                               Renewalenddate = DateTime.Now,
                               TerminateDate = DateTime.Now,
                               ContractType = s.ContractType,
                               //ContractTypes = ContractType.Select(P => new SelectListItem
                               //{
                               //    Value = P.Key.ToString(),
                               //    Text = P.Value
                               //}).ToList(),
                               //ContractName = 0,
                               //SelectContractname = Nameselect().Select(i => new SelectListItem
                               //{
                               //    Text = i.ContractName, //+ i.Name,
                               //    Value = i.ID.ToString(),
                               //}).ToList(),
                               //ContractID = 0,
                               //SetupContractName = "",
                               ContractNameTemplate = con.Name,
                               SubContractTypeTemplate = contype.ContractType,
                               Activities = ac,
                               EstimateSupportCost = 0,
                               Remark = s.Remark,
                               Attachement = att,

                           }).ToList();
            return service;
        }
        public List<ContactPerson> GetDefultContactPerson()
        {
            List<ContactPerson> contactPeople = new();
            //var data = _context.ContactPersons.ToList();
            //contactPeople.AddRange(data);
            for (var i = 1; i <= 5; i++)
            {
                var contactPeoples = new ContactPerson
                {
                    LineID = DateTime.Now.Ticks.ToString(),
                    ID = 0,
                    ContactID = "",
                    FirstName = "",
                    MidelName = "",
                    LastName = "",
                    Title = "",
                    Position = "",
                    Address = "",
                    Tel1 = "",
                    Tel2 = "",
                    MobilePhone = "",
                    Fax = "",
                    Email = "",
                    Pager = "",
                    Remark1 = "",
                    Remark2 = "",
                    Parssword = "",
                    CountriesOfBirth = ContriesOfBirth.Select(P => new SelectListItem
                    {
                        Value = P.Key.ToString(),
                        Text = P.Value
                    }).ToList(),
                    DateOfBirth = DateTime.Now,
                    Genders = Genders.Select(P => new SelectListItem
                    {
                        Value = P.Key.ToString(),
                        Text = P.Value
                    }).ToList(),
                    Profession = "",
                    SetAsDefualt = false,
                };
                contactPeople.Add(contactPeoples);
            }
            return contactPeople;

        }
        public BusinessPartner GetCustomerData(int id)
        {
            var data = (from bp in _context.BusinessPartners.Include(c => c.AutoMobile)
                        join pr in _context.PriceLists on bp.PriceListID equals pr.ID
                        // join gl in _context.GLAccounts on bp.GLAccID equals gl.ID
                        let gl = _context.GLAccounts.FirstOrDefault(w => w.ID == bp.GLAccID) ?? new GLAccount()
                        let slemp = _context.Employees.FirstOrDefault(x => x.ID == bp.SaleEMID) ?? new Employee()
                        let gr1 = _context.GroupCustomer1s.Where(x => x.ID == bp.Group1ID).FirstOrDefault() ?? new GroupCustomer1()
                        let gr2 = _context.GroupCustomer2s.Where(x => x.ID == bp.Group2ID).FirstOrDefault() ?? new GroupCustomer2()
                        let pay = _context.PaymentTerms.Where(x => x.ID == bp.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        let aut = _context.AutoMobiles.Where(x => x.BusinessPartnerID == bp.ID).ToList()
                        let terr = _context.Territories.FirstOrDefault(x => x.ID == bp.TerritoryID) ?? new KEDI.Core.Premise.Models.Services.Territory.Territory()
                        let cussource = _context.SetupCustomerSources.FirstOrDefault(x => x.ID == bp.CustomerSourceID) ?? new SetupCustomerSource()
                        select new BusinessPartner
                        {
                            ID = bp.ID,
                            Code = bp.Code,
                            Name = bp.Name,
                            Name2 = bp.Name2,
                            PaymentTermsID = pay.ID,
                            PaymentCode = pay.Code,
                            TerritoryID = terr.ID,
                            TerrName = terr.Name,
                            CustomerSourceID = cussource.ID,
                            CustomerSourceName = cussource.Name,
                            Type = bp.Type,
                            Address = bp.Address,
                            Phone = bp.Phone,
                            Email = bp.Email,
                            PriceListID = pr.ID,
                            PriName = pr.Name,
                            GPSink = bp.GPSink,
                            CreditLimit = bp.CreditLimit,
                            GLAccID = gl.ID,
                            GLCode = gl.Code,
                            GLName = gl.Name,
                            Group1ID = bp.Group1ID,
                            Group2ID = bp.Group2ID,
                            SaleEMID = slemp.ID,
                            SaleEmpName = slemp.Name,
                            Group1Name = gr1.Name,
                            Group2Name = gr2.Name,
                            VatNumber = bp.VatNumber,
                            AutoMobile = aut,
                            Point = bp.Point,
                            ContactPeople = (from con in _context.ContactPersons.Where(i => i.BusinessPartnerID == bp.ID)
                                             select new ContactPerson
                                             {
                                                 //LineID = DateTime.Now.Ticks.ToString(),
                                                 ID = con.ID,
                                                 BusinessPartnerID = bp.ID,
                                                 ContactID = con.ContactID,
                                                 FirstName = con.FirstName,
                                                 MidelName = con.MidelName,
                                                 LastName = con.LastName,
                                                 Title = con.Title,
                                                 Position = con.Position,
                                                 Address = con.Address,
                                                 Tel1 = con.Tel1,
                                                 Tel2 = con.Tel2,
                                                 MobilePhone = con.MobilePhone,
                                                 Fax = con.Fax,
                                                 Email = con.Email,
                                                 Pager = con.Pager,
                                                 Remark1 = con.Remark1,
                                                 Remark2 = con.Remark2,
                                                 Parssword = con.Parssword,
                                                 ContryOfBirth = con.ContryOfBirth,
                                                 CountriesOfBirth = ContriesOfBirth.Select(P => new SelectListItem
                                                 {
                                                     Value = P.Key.ToString(),
                                                     Text = P.Value,
                                                     Selected = P.Key == con.ContryOfBirth
                                                 }).ToList(),
                                                 DateOfBirth = con.DateOfBirth,
                                                 Genders = Genders.Select(P => new SelectListItem
                                                 {
                                                     Value = P.Key.ToString(),
                                                     Text = P.Value
                                                 }).ToList(),
                                                 Profession = con.Profession,
                                                 SetAsDefualt = con.SetAsDefualt,
                                             }).ToList() ?? new List<ContactPerson>(),
                            BPBranches = (from br in _context.BPBranches.Where(x => x.BusinessPartnerID == bp.ID)
                                          select new BPBranch
                                          {
                                              ID = br.ID,
                                              BusinessPartnerID = bp.ID,
                                              Name = br.Name,
                                              Tel = br.Tel,
                                              Address = br.Address,
                                              Email = br.Email,
                                              BranchCotactPerson = br.BranchCotactPerson,
                                              ContactTel = br.ContactTel,
                                              ContactEmail = br.ContactEmail,
                                              GPSLink = br.GPSLink,
                                              SetDefualt = br.SetDefualt
                                          }
                                            ).ToList() ?? new List<BPBranch>(),

                            ContractBilings = (from con in _context.ContractBilings.Where(x => x.BusinessPartnerID == bp.ID)

                                               select new ContractBiling
                                               {
                                                   ID = con.ID,
                                                   DocumentNo = con.DocumentNo,
                                                   Amount = con.Amount,
                                                   NumExpiresOfDay = con.NumExpiresOfDay,
                                                   Statuss = Status.Select(P => new SelectListItem
                                                   {
                                                       Value = P.Key.ToString(),
                                                       Text = P.Value
                                                   }).ToList(),
                                                   Status = con.Status,
                                                   ConfrimRenews = ConfirmRenew.Select(P => new SelectListItem
                                                   {
                                                       Value = P.Key.ToString(),
                                                       Text = P.Value
                                                   }).ToList(),
                                                   Payments = Payment.Select(P => new SelectListItem
                                                   {
                                                       Value = P.Key.ToString(),
                                                       Text = P.Value
                                                   }).ToList(),
                                                   NewContractStartDate = con.NewContractStartDate,
                                                   NewContractEndDate = con.NewContractEndDate,
                                                   NextOpenRenewalDate = con.NextOpenRenewalDate,
                                                   Renewalstartdate = con.Renewalstartdate,
                                                   Renewalenddate = con.Renewalenddate,
                                                   TerminateDate = con.TerminateDate,
                                                   ContractType = con.ContractType,
                                                   ContractNameTemplate = con.ContractNameTemplate,
                                                   SubContractTypeTemplate = con.SubContractTypeTemplate,
                                                   Activities = con.Activities,
                                                   EstimateSupportCost = con.EstimateSupportCost,
                                                   Remark = con.Remark,
                                                   Attachement = con.Attachement,
                                                   FileName = con.FileName,
                                                   Path = con.Path,
                                                   SaleID = con.SaleID,
                                                   SeriesDID = con.SeriesDID
                                               }
                                           ).ToList() ?? new List<ContractBiling>()
                        }).FirstOrDefault(c => c.ID == id);
            return data;
        }
        public ContactPerson Contact(int bpid = 0)
        {
            ContactPerson contacts = new()
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                BusinessPartnerID = bpid,
                ContactID = "",
                FirstName = "",
                MidelName = "",
                LastName = "",
                Title = "",
                Position = "",
                Address = "",
                Tel1 = "",
                Tel2 = "",
                MobilePhone = "",
                Fax = "",
                Email = "",
                Pager = "",
                Remark1 = "",
                Remark2 = "",
                Parssword = "",
                CountriesOfBirth = ContriesOfBirth.Select(P => new SelectListItem
                {
                    Value = P.Key.ToString(),
                    Text = P.Value
                }).ToList(),
                DateOfBirth = DateTime.Now,
                Genders = Genders.Select(P => new SelectListItem
                {
                    Value = P.Key.ToString(),
                    Text = P.Value
                }).ToList(),
                Profession = "",
                SetAsDefualt = false
            };
            return contacts;
        }
        //===========ContactPerson===============
        public async Task SaveContactPersonAsync(List<ContactPerson> contactPersons, ModelStateDictionary modelState)
        {

            try
            {
                if (modelState.IsValid)
                {
                    contactPersons = contactPersons
                        .Where(i => !string.IsNullOrEmpty(i.ContactID)).ToList();
                    _context.ContactPersons.UpdateRange(contactPersons);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }
        }
        //======================BPbranches=============================
        public async Task SaveBPBranchesAsync(List<BPBranch> bpbranches, ModelStateDictionary modelState)
        {

            try
            {
                if (modelState.IsValid)
                {
                    bpbranches = bpbranches
                        .Where(i => !string.IsNullOrEmpty(i.Name)).ToList();
                    _context.BPBranches.UpdateRange(bpbranches);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }
        }
        public async Task<List<BusinessPartner>> GetCustomersAsync(string keyword = "")
        {
            try
            {
                var customers = _context.BusinessPartners.Where(c => string.Compare(c.Type, "customer", true) == 0 && !c.Delete).ToList();
                customers = await SearchPartnersAsync(customers, keyword);
                return customers;
            }
            catch
            {
                return await Task.FromResult(new List<BusinessPartner>());
            }
        }

        private static async Task<List<BusinessPartner>> SearchPartnersAsync(List<BusinessPartner> partners, string keyword = "")
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                partners = partners.Where(c =>
                            RawWord(c.Code).Contains(keyword, ignoreCase)
                            || RawWord(c.Name).Contains(keyword, ignoreCase)
                            || RawWord(c.Phone).Contains(keyword, ignoreCase)
                            || RawWord(c.Email).Contains(keyword, ignoreCase)
                            || RawWord(c.Type).Contains(keyword, ignoreCase)
                            || RawWord(c.Address).Contains(keyword, ignoreCase)).ToList();
            }
            return await Task.FromResult(partners);
        }

        public async Task<BusinessPartner> FindCustomerAsync(int customerId)
        {
            try
            {
                var customer = _context.BusinessPartners.FirstOrDefault(c => c.ID == customerId
                    && !c.Delete && string.Compare(c.Type, "customer", true) == 0) ?? new BusinessPartner();
                return await Task.FromResult(customer);
            }
            catch
            {
                return await Task.FromResult(new BusinessPartner());
            }
        }

        public List<SelectListItem> SelectPricelists(int pricelistId = 0, bool disabled = false)
        {
            return _context.PriceLists.Where(p => !p.Delete).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.Name,
                Selected = p.ID == pricelistId,
                Disabled = disabled
            }).ToList();
        }
        // block Mr Bunthorn

        public async Task AddOrEdit(GroupCustomer1 group1)
        {
            if (group1.ID == 0)
            {
                await _context.GroupCustomer1s.AddAsync(group1);
                await _context.SaveChangesAsync();
                //int id=group1.ID;
                //AddPrintListDetail(id);
            }
            else
            {

                _context.GroupCustomer1s.Update(group1);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<GroupCustomer1>> GetGroup1(string type)
        {
            var list = (from g in _context.GroupCustomer1s.Where(s => s.Type == type && s.Delete == false)
                        select new GroupCustomer1
                        {
                            ID = g.ID,
                            Name = g.Name,
                            Type = g.Type,

                        }).ToList();
            return await Task.FromResult(list);
        }
        // Group 1
        public List<SelectListItem> SelectGroup1(int Id = 0, string type = null, bool disabled = false)
        {
            return _context.GroupCustomer1s.Where(p => !p.Delete && p.Type == type).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.Name,
                Selected = p.ID == Id,
                Disabled = disabled
            }).ToList();
        }
        public async Task AddGroup2OrEdit(GroupCustomer2 group1)
        {
            if (group1.ID == 0)
            {

                await _context.GroupCustomer2s.AddAsync(group1);
                await _context.SaveChangesAsync();
                //int id=group1.ID;
                //AddPrintListDetail(id);
            }
            else
            {

                _context.GroupCustomer2s.Update(group1);
                await _context.SaveChangesAsync();
            }
        }
        public List<SelectListItem> SelectGroup2(int Id = 0, string type = null, bool disabled = false)
        {
            return _context.GroupCustomer2s.Where(p => !p.Delete && p.Type == type).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.Name,
                Selected = p.ID == Id,
                Disabled = disabled
            }).ToList();
        }
        public ActivityView FindActivity(int number)
        {
            var activity = (from ac in _context.Activites.Where(x => x.Number == number)
                            join g in _context.Generals on ac.ID equals g.ActivityID
                            let bp = _context.BusinessPartners.FirstOrDefault(x => x.ID == ac.BPID) ?? new BusinessPartner()
                            join us in _context.UserAccounts on ac.UserID equals us.ID
                            let emp = _context.Employees.FirstOrDefault(x => x.ID == us.EmployeeID) ?? new Employee()
                            let actype = _context.SetupTypes.FirstOrDefault(x => x.ID == ac.TypeID) ?? new SetupType()
                            let st = _context.SetupStatuses.FirstOrDefault(x => x.ID == g.StatusID) ?? new SetupStatus()
                            select new ActivityView
                            {
                                ID = ac.ID,
                                GID = g.ID,
                                BPID = bp.ID,
                                UserID = ac.UserID,
                                EmpID = ac.EmpID,
                                Activities = (int)ac.Activities,
                                TypeID = ac.TypeID,
                                TypeName = actype.Name,
                                AssignedByID = ac.AssignByID,
                                BpCode = bp.Code,
                                BpName = bp.Name,
                                BpType = bp.Type,
                                SubNameID = ac.SubNameID,
                                Personal = ac.Personal,
                                EmpNameID = ac.EmpNameID,
                                EmpName = emp.Name,
                                Color = st.Color,
                                //=====ContactPerson======
                                Contact = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == bp.ID)
                                           select new ContactPersonViewModel
                                           {
                                               ID = con.ID,
                                               ContactID = con.ContactID,
                                               Tel1 = con.Tel1,
                                               SetAsDefualt = con.SetAsDefualt
                                           }).ToList() ?? new List<ContactPersonViewModel>(),
                                //===general==
                                Remark = g.Remark,
                                StartTime = g.StartTime,
                                EndTime = g.EndTime,
                                Durration = g.Durration,
                                StatusID = g.StatusID,
                                Status = st.Status,
                                Location = g.Location,
                                Priority = g.Priority,
                                Recurrences = g.Recurrence,
                                After = g.After,
                                By = g.By,
                                NoEndDate = g.NoEndDate,
                                NumAfter = g.NumAfter,
                                RepeatDate = g.RepeatDate,
                                Start = g.Start,
                                RepeatEveryRecurr = g.RepeatEveryRecurr,
                                RepeatEveryWeek = g.RepeatEveryWeek,
                                ByDate = g.ByDate,
                                Mon = g.Mon,
                                Tue = g.Tue,
                                Wed = g.Wed,
                                Thu = g.Thu,
                                Fri = g.Fri,
                                Sat = g.Sat,
                                Sun = g.Sun,
                                //==monthly====
                                Days = g.Days,
                                numDay = g.numDay,
                                repeatOn = g.repeatOn,
                                numOfRepeat = g.numOfRepeat,
                                DaysInMonthly = g.DaysInMonthly,

                                //======yearly=====
                                RepeatOncheckYearly = g.RepeatOncheckYearly,
                                MonthsInAnnualy = g.MonthsInAnnualy,
                                NumOfMonths = g.NumOfMonths,
                                checkNumAnnualy = g.checkNumAnnualy,
                                NumofAnnualy = g.NumofAnnualy,
                                DaysOfAnnualy = g.DaysOfAnnualy,
                                MonthsOfAnnulay = g.MonthsInAnnualy,
                                RepeatofNumAnnualy = g.RepeatofNumAnnualy,
                                RepeatNumOfmonths = g.RepeatNumOfmonths,
                                Number = ac.Number,
                            }).FirstOrDefault();
            return activity;
        }

        public async Task<BusinessPartner> SaveVendorAsync(BusinessPartner vendor, ModelStateDictionary modelState)
        {
            try
            {
                if (vendor.ID <= 0)
                {
                    if (_context.BusinessPartners.Any(c => EqualNotEmpty(c.Code, vendor.Code, true)))
                    {
                        modelState.AddModelError("Code", "Code is already existing.");
                    }
                }

                if (modelState.IsValid)
                {
                    vendor.Type = "Vendor";
                    _context.BusinessPartners.Update(vendor);
                    await _context.SaveChangesAsync();
                }

                return vendor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
                return await Task.FromResult(new BusinessPartner());
            }
        }

        public List<SelectListItem> SelectListTerritory(int Id = 0, string type = null, bool disabled = false)
        {
            return _context.SetupCustomerSources.Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.Name,
                Selected = p.ID == Id,
                Disabled = disabled
            }).ToList();
        }

        public async Task SaveContractBillingAsync(List<ContractBiling> contractBilings, ModelStateDictionary modelState)
        {

            try
            {
                if (modelState.IsValid)
                {
                    contractBilings = contractBilings
                        .Where(i => !string.IsNullOrEmpty(i.DocumentNo)).ToList();
                    _context.ContractBilings.UpdateRange(contractBilings);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }
        }
        public async Task<int> SaveVehiclesAsync(ModelStateDictionary modelState, int customerId, List<AutoMobile> vehicles)
        {

            if (modelState.IsValid)
            {
                if (vehicles != null)
                {
                    foreach (var vh in vehicles)
                    {
                        vh.BusinessPartnerID = customerId;
                    }
                    _context.AutoMobiles.UpdateRange(vehicles);
                    return await _context.SaveChangesAsync();
                }

            }
            return 0;
        }
    }
}
