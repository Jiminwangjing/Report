using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.AppContext;
using KEDI.Core.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KEDI.Core.Premise.Models.Partners;

namespace KEDI.Core.Premise.Models.Services.Repositories
{
    public interface ILoanPartnerRepo
    {
        Task<List<LoanContactPerson>>BindRowsDefaultAsynce();
        Task<LoanContactPerson>AddRowsAsynce();
        Task<LoanPartner> FindLoanPartner(int id);
        
    
    }

    public class LoanPartnerRepo : ILoanPartnerRepo
    {
         private readonly DataContext _context;
        public LoanPartnerRepo(DataContext context)
        {
             _context = context;

        }
         public Dictionary<int, string> ContriesOfBirth => EnumHelper.ToDictionary(typeof(ContriesOfBirth));
        public Dictionary<int, string> Genders => EnumHelper.ToDictionary(typeof(Genders));

        public async Task<LoanContactPerson> AddRowsAsynce()
        {
            LoanContactPerson obj = new LoanContactPerson{
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
            return await Task.FromResult(obj) ;
        }

        public async Task<List<LoanContactPerson>> BindRowsDefaultAsynce()
        {
             List<LoanContactPerson> contactPeople = new();
           
            for (var i = 1; i <= 10; i++)
            {
                contactPeople.Add(await AddRowsAsynce());
            }
            return await Task.FromResult(contactPeople);
        }

        public async Task<LoanPartner> FindLoanPartner(int id)
        {
             
            var obj =await (from ob in  _context.LoanPartners.Where(s=> s.ID == id)
                        let g1 =  _context.GroupLoanPartners.FirstOrDefault(s=> s.ID== ob.Group1ID)?? new GroupLoanPartner()
                        let g2 = _context.GroupLoanPartners.FirstOrDefault(s=> s.ID==ob.Group2ID)?? new GroupLoanPartner()
                        select new LoanPartner
                        {
                            ID     = ob.ID,
                            Code    = ob.Code,
                            Name1   = ob.Name1,
                            Name2   = ob.Name2,
                            Group1ID = ob.Group1ID,
                            Group2ID    = ob.Group2ID,
                            Phone       = ob.Phone,
                            Address     = ob.Address,
                            Email       = ob.Email,
                            EmpID       = ob.EmpID,
                            EmloyeeName = ob.EmloyeeName,
                            VatNumber   = ob.VatNumber,
                            GPSLink     = ob.GPSLink,
                            LoanContactPeople =(from lcp in _context.LoanContactPersons.Where(s=> s.LoanPartnerID== ob.ID)
                                                select new LoanContactPerson
                                                {
                                                    ID= lcp.ID,
                                                    LineID = lcp.ID.ToString(),
                                                    LoanPartnerID = lcp.LoanPartnerID,
                                                    ContactID   = lcp.ContactID,
                                                    Gender      = lcp.Gender,
                                                    ContryOfBirth= lcp.ContryOfBirth,
                                                    FirstName   = lcp.FirstName,
                                                    LastName    = lcp.LastName,
                                                    MidelName   = lcp.MidelName,
                                                    Title       = lcp.Title,
                                                    Position    = lcp.Position,
                                                    Address     = lcp.Address,
                                                    Tel1        = lcp.Tel1,
                                                    Tel2        = lcp.Tel2,
                                                    MobilePhone = lcp.MobilePhone,
                                                    Fax         = lcp.Fax,
                                                    Email       = lcp.Pager,
                                                    Pager       = lcp.Pager,
                                                    Remark1     = lcp.Remark1,
                                                    Remark2     = lcp.Remark2,
                                                    Parssword   = lcp.Parssword,
                                                    CountriesOfBirth = ContriesOfBirth.Select(P => new SelectListItem
                                                    {
                                                        Value = P.Key.ToString(),
                                                        Text = P.Value,
                                                        Selected = P.Key == lcp.ContryOfBirth
                                                    }).ToList(),
                                                     Genders = Genders.Select(P => new SelectListItem
                                                    {
                                                        Value = P.Key.ToString(),
                                                        Text = P.Value,
                                                        Selected =P.Key == lcp.Gender
                                                    }).ToList(),
                                                    DateOfBirth = lcp.DateOfBirth,
                                                    Profession  = lcp.Profession,
                                                    SetAsDefualt = lcp.SetAsDefualt,
                                                }).ToList()


                        }).FirstOrDefaultAsync();
                        int count=obj.LoanContactPeople.Count;
                        if(count<10)
                        {
                             for (int i = 1; i <= 10-count; i++)
                            {
                                obj.LoanContactPeople.Add(await AddRowsAsynce());
                            }
                        }
                       
                   

           return obj;
        }
        
       
    }
}