using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Inventory.PriceList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface ICompany
    {
        IQueryable<Company> GetCompany();
        Task<int> AddOrEdit(Company company);
        Task<int> DeleteCompany(int? id);
        void UpdateCurrencyWarehouseDetail(int PricelistID);
        Company FindCompany();
    }
    public class CompanyRepository : ICompany
    {
        private readonly DataContext _context;
        public CompanyRepository(DataContext dataContext)
        {
            _context = dataContext;
        }
        public Task<int> AddOrEdit(Company company)
        {
            throw new NotImplementedException();
        }

        public async Task<int> DeleteCompany(int? id)
        {
            var com = await _context.Company.FindAsync(id);
            if (com != null)
            {
                com.Delete = true;
                _context.Company.Update(com);
            }
            return await _context.SaveChangesAsync();
        }



        public IQueryable<Company> GetCompany()
        {
            IQueryable<Company> list = (from com in _context.Company.Where(c => c.Delete == false)
                                        join pri in _context.PriceLists.Where(cr => cr.Delete == false)
                                        on com.PriceListID equals pri.ID
                                       
                                        select new Company
                                        {
                                            ID = com.ID,
                                            Name = com.Name,                                            
                                            Location = com.Location,
                                            Address = com.Address,
                                            Process = com.Process,
                                            Delete = com.Delete,
                                         
                                            PriceListID = com.PriceListID,
                                            Logo=com.Logo,
                                          
                                            PriceList = new PriceLists
                                            {
                                                Name = pri.Name
                                            }

                                        });
            return list;
        }
        public Company FindCompany()
        {
            var company = _context.Company.Include(c => c.PriceList).FirstOrDefault();          
            return company;
        }

        public void UpdateCurrencyWarehouseDetail(int PricelistID)
        {
            _context.Database.ExecuteSqlCommand("sp_UpdateCurrencyWarehouseDetail @PricelistID={0}",
                parameters: new[] {
                    PricelistID.ToString()
                });
        }
      
    }
}
