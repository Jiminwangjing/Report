using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.HumanResources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Responsitory
{
    public interface IEmployee
    {
        List<Employee> Employees();
        Task<Employee> EmployeeAsync(int id);
        Task Add(Employee employee);
        Task Update(Employee employee);
        Task Delete(int id);

    }
    public class EmployeeRepository : IEmployee
    {
        private readonly DataContext _context;
        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }
        //public IQueryable<Employee> Employees => _context.Employees.Where(x => x.Delete == false);

        public List<Employee> Employees()
        {
            var data = (from e in _context.Employees.Where(e => !e.Delete)
                        let empType = _context.EmployeeTypes.FirstOrDefault(i => i.ID == e.EMTypeID) ?? new EmployeeType()
                        select new Employee
                        {
                            ID = e.ID,
                            Code = e.Code,
                            Name = e.Name,
                            Gender = e.Gender,
                            GenderDisplay = e.Gender.ToString(),// == Gender.Female ? "Female" : "Male",
                            Birthdate = e.Birthdate,
                            Hiredate = e.Hiredate,
                            Phone = e.Phone,
                            Position = e.Position,
                            HireDateDisplay = e.Hiredate.ToShortDateString(),
                            BirthdateDisplay = e.Birthdate.ToShortDateString(),
                            EMTypeID = e.EMTypeID,
                            EMType = empType.Type,
                        }).ToList();
            return data;
        }
        public async Task<Employee> EmployeeAsync(int id)
        {
            var data = await (from e in _context.Employees.Where(e => !e.Delete && e.ID == id)
                              let empType = _context.EmployeeTypes.FirstOrDefault(i => i.ID == e.EMTypeID) ?? new EmployeeType()
                              select new Employee
                              {
                                  ID = e.ID,
                                  Code = e.Code,
                                  Name = e.Name,
                                  Gender = e.Gender,
                                  GenderDisplay = e.Gender.ToString(),// == Gender.Female ? "Female" : "Male",
                                  Birthdate = e.Birthdate,
                                  Hiredate = e.Hiredate,
                                  Phone = e.Phone,
                                  Position = e.Position,
                                  HireDateDisplay = e.Hiredate.ToShortDateString(),
                                  BirthdateDisplay = e.Birthdate.ToShortDateString(),
                                  EMTypeID = e.EMTypeID,
                                  EMType = empType.Type,
                                  Action = e.Action,
                                  Address = e.Address,
                                  CompanyID = e.CompanyID,
                                  Delete = e.Delete,
                                  Email = e.Email,
                                  Image = e.Image,
                                  IsUser = e.IsUser,
                                  Photo = e.Photo,
                                  Stopwork = e.Stopwork,
                              }).FirstOrDefaultAsync();
            return data;
        }
        public async Task Add(Employee employee)
        {

            try
            {
                if (employee != null)
                {
                    await _context.AddAsync(employee);
                    await _context.SaveChangesAsync();
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        public async Task Delete(int id)
        {
            if (id > 0)
            {
                var emp = await _context.Employees.FindAsync(id);
                if (emp != null)
                {
                    emp.Delete = true;
                    _context.Update(emp);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task Update(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
    }
}
