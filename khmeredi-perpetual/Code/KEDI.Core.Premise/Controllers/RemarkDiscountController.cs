using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.Services.RemarkDiscount;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class RemarkDiscountController : Controller
    {
        private readonly IRemarkDiscountRepository _remark;

        public RemarkDiscountController(IRemarkDiscountRepository remark)
        {
            _remark = remark;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.RemarkDiscount = "highlight";
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.RemarkDiscount = "highlight";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.RemarkDiscount = "highlight";
            var data = await _remark.GetRemarkDiscountAsync(id);
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RemarkDiscountItem data)
        {
            ViewBag.RemarkDiscount = "highlight";
            if (string.IsNullOrEmpty(data.Remark))
            {
                ModelState.AddModelError("Remark", "Remark cannot be empty!");
            }
            if (ModelState.IsValid)
            {
                await _remark.UpdateAsync(data);
                return RedirectToAction("Index");
            }
            return View(data);
        }
        public async Task<IActionResult> GetRemarkDiscounts()
        {
            var data = await _remark.GetRemarkDiscountsAsync();
            return Ok(data);
        }
        public async Task<IActionResult> SaveAll(string data)
        {
            ModelMessage msg = new();
            List<RemarkDiscountItem> _data = JsonConvert.DeserializeObject<List<RemarkDiscountItem>>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            int i = 0;
            foreach(var item in _data)
            {
                i++;
                if (string.IsNullOrEmpty(item.Remark))
                {
                    ModelState.AddModelError($"{i}Remark", $"Remark at line {i} cannot be empty!");
                }
            }
            if (ModelState.IsValid)
            {
                await _remark.SaveAllAsync(_data);
                msg.Approve();
                ModelState.AddModelError("Success", "Save successfully!");
            }
            
            return Ok(msg.Bind(ModelState));
        }
    }
}
