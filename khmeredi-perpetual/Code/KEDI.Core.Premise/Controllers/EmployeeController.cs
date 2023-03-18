using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Models.Validation;

namespace CKBS.Controllers
{
    [Privilege]
    public class EmployeeController : Controller
    {
        private readonly IEmployee _emp;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmployeeController(IEmployee emp, DataContext context, IWebHostEnvironment environment)
        {
            _emp = emp;
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        [Privilege("A011")]
        public IActionResult Index()
        {
            ViewBag.style = "fa-user";
            ViewBag.Main = "Human Resources";
            ViewBag.Page = "Employee Master Data";
            ViewBag.HR = "show";
            ViewBag.EmpMasterData = "highlight";
            return View();
        }
        public IActionResult ListEmployees()
        {
            var prod = _emp.Employees();
            return Ok(prod);
        }

        [Privilege("A011")]
        public IActionResult Create()
        {
            ViewBag.style = "fa-user";
            ViewBag.Main = "Human Resources";
            ViewBag.Page = "Employee Master Data";
            ViewBag.button = "fa-plus-circle";
            ViewBag.type = "Create";
            ViewBag.HR = "show";
            ViewBag.EmpMasterData = "highlight";
            return View();
        }

        [HttpPost]
        //[AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            ViewBag.style = "fa-user";
            ViewBag.Main = "Human Resources";
            ViewBag.Page = "Employee Master Data";
            ViewBag.button = "fa-plus-circle";
            ViewBag.type = "Create";
            ViewBag.HR = "show";
            ViewBag.EmpMasterData = "highlight";
            employee.Birthdate.ToString("yyyy/MM/dd");
            employee.Hiredate.ToString("yyyy/MM/dd");
            employee.EMType ??= "";
            if (ModelState.IsValid)
            {
                try
                {
                    await _emp.Add(employee);
                    UploadImg(employee);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    ViewBag.Error = $"This code already exist ! //{e.Message}";

                    return View(employee);
                }

            }
            else
            {
                var error = ModelState.Values.SelectMany(v => v.Errors);


            }
            return View(employee);

        }

        public void UploadImg(Employee employee)
        {
            try
            {
                var Image = HttpContext.Request.Form.Files[0];
                if (Image != null && Image.Length > 0)
                {
                    var file = Image;
                    var uploads = Path.Combine(_environment.WebRootPath, "images/employee");
                    if (file.Length > 0)
                    {
                        using var fileStream = new FileStream(Path.Combine(uploads, employee.Photo), FileMode.Create);
                        file.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        [Privilege("A011")]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.style = "fa-user";
            ViewBag.Main = "Human Resources";
            ViewBag.Page = "Employee Master Data";
            ViewBag.button = "fa-edit";
            ViewBag.type = "Edit";
            ViewBag.HR = "show";
            ViewBag.EmpMasterData = "highlight";
            var employee = await _emp.EmployeeAsync(id);
            if (employee.Delete == true)
            {
                return RedirectToAction(nameof(Index));
            }
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id, Employee employee)
        {
            if (id != employee.ID)
            {
                return NotFound();
            }
            ViewBag.style = "fa-user";
            ViewBag.Main = "Human Resources";
            ViewBag.Page = "Employee Master Data";
            ViewBag.button = "fa-edit";
            ViewBag.type = "Edit";
            ViewBag.HR = "show";
            ViewBag.EmpMasterData = "highlight";
            if (ModelState.IsValid)
            {
                try
                {
                    await _emp.Update(employee);
                    UploadImg(employee);
                }
                catch (Exception)
                {
                    ViewBag.Error = "This code already exist !";
                    return View(employee);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        public IActionResult CreateDefualtType()
        {
            List<EmployeeType> emtype = new();

            var list = (from em in _context.EmployeeTypes.Where(s => s.Delete == false)
                        select new EmployeeType
                        {
                            ID = em.ID,
                            LineID = DateTime.Now.Ticks.ToString() + em.ID,
                            Type = em.Type,
                            Status = false,
                        }).OrderBy(s => s.Type).ToList();


            return Ok(list);
        }
        [HttpPost]// save data to database
        public IActionResult SaveEMType(EmployeeType data)
        {

            ModelMessage msg = new();

            //if (string.IsNullOrWhiteSpace(data.Type))
            //{
            //    ModelState.AddModelError("", "please input Type ...!");
            //}

            if (ModelState.IsValid)
            {
                _context.Update(data);
                _context.SaveChanges();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            var list = (from em in _context.EmployeeTypes.Where(s => s.Delete == false)
                        select new EmployeeType
                        {
                            ID = em.ID,
                            LineID = DateTime.Now.Ticks.ToString() + em.ID,
                            Type = em.Type,
                            Status = false,
                        }).OrderBy(s => s.Type).ToList();
            return Ok(new { Model = msg.Bind(ModelState), Data = list });

        }
        public async Task<IActionResult> Delete(int id)
        {
            await _emp.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.ID == id);
        }
    }
}