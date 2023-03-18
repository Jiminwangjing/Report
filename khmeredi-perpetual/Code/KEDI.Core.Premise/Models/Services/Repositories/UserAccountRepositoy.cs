
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.ServicesClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CKBS.Models.Services.Responsitory
{
    public interface IUserAccount
    {
        IQueryable<UserAccount> UserAccounts();
        Task Delete(int? id);
        Task Register(UserAccount user);
        UserAccount GetUser(int id);
        IEnumerable<UserPrivillege> GetUserPrivilleges(int id);
        void UpdateUserPrivilleges(List<UserPrivillege> userPrivilleges);
        void UpdateAllselect(bool all,int UserID);
    }

    public class UserAccountRepositoy : IUserAccount
    {
        private readonly DataContext _context;      
        public UserAccountRepositoy(DataContext dataContext)
        {
            _context = dataContext;
        }
       
        public IQueryable<UserAccount> UserAccounts()
        {
            IQueryable<UserAccount> list = (from acc in _context.UserAccounts.Where(x => x.Delete == false)
                                            join emp in _context.Employees.Where(x => x.Delete == false) on
                                            acc.EmployeeID equals emp.ID
                                            join ban in _context.Branches.Where(x => x.Delete == false) on
                                            acc.BranchID equals ban.ID
                                            join com in _context.Company.Where(x => x.Delete == false ) on
                                            acc.CompanyID equals com.ID
                                            where acc.Delete == false
                                            select new UserAccount
                                            {
                                                ID = acc.ID,
                                                Username = acc.Username,
                                                Password = acc.Password,
                                                BranchID = acc.BranchID,
                                                CompanyID = acc.CompanyID,
                                                Language = acc.Language,
                                                Employee = new Employee
                                                {
                                                    Name = emp.Name,
                                                    Address = emp.Address,
                                                    Birthdate = emp.Birthdate,
                                                    Code = emp.Code,
                                                    Email = emp.Email,
                                                    Gender = emp.Gender,
                                                    Hiredate = emp.Hiredate,
                                                    Phone = emp.Phone,
                                                    Photo = emp.Photo,
                                                    Position = emp.Position
                                                  
                                                },
                                                Branch = new Branch
                                                {
                                                    Name = ban.Name
                                                },
                                                Company = new Company
                                                {
                                                    Name = com.Name,
                                                    Logo=com.Logo
                                                   
                                                }
                                            }

                );
            return list;
        }
              
        public async Task Delete(int? id)
        {
            var user = await _context.UserAccounts.FindAsync(id);
            if (user != null)
            {
                user.Delete = true;
                _context.UserAccounts.Update(user);
            }
            await _context.SaveChangesAsync();
        }

        public UserAccount GetUser(int id) => _context.UserAccounts.Find(id);

        public async Task Register(UserAccount user)
        {
            if (user.ID == 0)
            {
                await _context.UserAccounts.AddAsync(user);
                await _context.SaveChangesAsync();
                int id = user.ID;
                AddUserPrivillege(id);
            }
            else
            {
                _context.UserAccounts.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddUserPrivillege(int id)
        {            
            List<UserPrivillege> userPrivilleges = new ();
            var privileges = _context.Functions.Select(f => new UserPrivillege {
                UserID = id,
                FunctionID = f.ID,
                Code = f.Code,
                Used = false,
                Delete = false
            });
            _context.UserPrivilleges.UpdateRange(privileges);
            _context.SaveChanges();
           
            //_context.Database.ExecuteSqlCommand("sp_InsertUserpivillege @UserID={0}",
            //parameters: new[] {
            //    id.ToString()
            //});
        }

        public IEnumerable<UserPrivillege> GetUserPrivilleges(int id)
        {
            var user= _context.UserAccounts.Where(x => x.Delete == false);
            IEnumerable<UserPrivillege> list = (
                from Userpri in _context.UserPrivilleges.Where(x => x.Delete == false)
                join    
                fun in _context.Functions on
                Userpri.FunctionID equals fun.ID
                where Userpri.UserID == id
                select new UserPrivillege
                {
                    ID=Userpri.ID,
                    UserID = Userpri.UserID,
                    FunctionID = Userpri.FunctionID,
                    Used = Userpri.Used,
                    Function = new Function
                    {
                        ID=fun.ID,
                        Name = fun.Name,
                        Type = fun.Type
                    }
                }
            );
            return list;
        }

        public void UpdateUserPrivilleges(List<UserPrivillege> userPrivilleges)
        {
            foreach (var user in userPrivilleges)
            {
                var user_update = _context.UserPrivilleges.FirstOrDefault(w => w.ID == user.ID);
                user_update.Used = user.Used;
                user_update.Code = Regex.Replace(user_update.Code, "\\s+", string.Empty);
                _context.UserPrivilleges.Update(user_update);
                _context.SaveChanges();
            }
        }

        public void UpdateAllselect(bool all,int userID)
        {
            List<UserPrivillege> list = _context.UserPrivilleges.Where(w => w.UserID == userID).ToList();
            foreach (var item in list)
            {
                item.Used = all;
                _context.Update(item);
                _context.SaveChanges();
            }
        }
    }
}
