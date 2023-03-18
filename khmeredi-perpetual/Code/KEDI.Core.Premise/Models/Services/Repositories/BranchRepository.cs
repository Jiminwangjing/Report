using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IBranch
    {
        IQueryable<Branch> GetBranch();
        Task AddOrEdit(Branch branch);
        Task<int> Delete(int? id);
    }
    public class BranchRepository : IBranch
    {
        private readonly DataContext _dataContext;
        public BranchRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task AddOrEdit(Branch branch)
        {
            if (branch.ID == 0)
            {
               await _dataContext.Branches.AddAsync(branch);
            }
            else
            {
                _dataContext.Branches.Update(branch);
            }
           await _dataContext.SaveChangesAsync();
        }

        public async Task<int> Delete(int? id)
        {
            var com = await _dataContext.Branches.FindAsync(id);
            if (com != null)
            {
                com.Delete = true;
                _dataContext.Branches.Update(com);
            }
            return await _dataContext.SaveChangesAsync();
        }

        public IQueryable<Branch> GetBranch()
        {
            IQueryable<Branch> list = (from b in _dataContext.Branches.Where(c => c.Delete == false)
                                       join com in _dataContext.Company.Where(o => o.Delete == false)
                                       on b.CompanyID equals com.ID
                                       select new Branch
                                       {
                                           ID = b.ID,
                                           Name = b.Name,
                                           CompanyID = b.CompanyID,
                                           Address = b.Address,
                                           Location = b.Location,
                                           Company = new Company
                                           {
                                               Name = com.Name
                                           }
                                       }
                                       );
            return list;
        }
    }
}
