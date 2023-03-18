using CKBS.AppContext;
using CKBS.Models.Services.ChartOfAccounts;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Responsitory
{
    public interface ITaxGroup
    {
        dynamic GetNewOneToCreate();
        dynamic GetNewOneToCreateTd();
        dynamic GetActiveGlacc(int comId);
        dynamic GetTD(int tid);
        IEnumerable<dynamic> GetTaxGraoups(int comId);
        void CreateTaxGroup(dynamic data);
        void UpdateTaxgroups(List<TaxGroup> taxes);
    }
    public class TaxGroupRepository : ITaxGroup
    {
        private readonly DataContext _context;

        public TaxGroupRepository(DataContext context)
        {
            _context = context;
        }
        Dictionary<string, string> category = new Dictionary<string, string>
            {
                { ((int)TypeTax.None).ToString(), TypeTax.None.ToString()},
                { ((int)TypeTax.OutPutTax).ToString(), TypeTax.OutPutTax.ToString()},
                { ((int)TypeTax.InputTax).ToString(), TypeTax.InputTax.ToString()},
            };
        public void CreateTaxGroup(dynamic data)
        {
            _context.TaxGroups.Update(data);
            _context.SaveChanges();
        }

        public dynamic GetActiveGlacc(int comId)
        {
            var glaccs = _context.GLAccounts.Where(i => i.CompanyID == comId && i.IsActive).ToList();
            return glaccs;
        }

        public dynamic GetNewOneToCreate()
        {

            var taxGroup = new TaxGroupViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                ID = 0,
                GLID = 0,
                Name = "",
                Code = "",
                Types = category.Select(c => new SelectListItem
                {
                    Value = c.Key,
                    Text = c.Value,
                }).ToList(),
                Effectivefrom = DateTime.MinValue,
                Rate = 0,
                Delete = false,
                Active = false,
                TaxGroupDefinitions = new List<TaxGroupDefinition>()
            };
            return taxGroup;
        }

        public dynamic GetNewOneToCreateTd()
        {
            var td = new TaxGroupDefinitionViewModel
            {
                LineID = DateTime.Now.Ticks.ToString(),
                EffectiveFrom = DateTime.MinValue,
                Rate = 0,
                ID = 0
            };
            return td;
        }
        public IEnumerable<dynamic> GetTaxGraoups(int comId)
        {
            var taxes = (from tax in _context.TaxGroups.Where(i => i.CompanyID == comId)
                         let gl = _context.GLAccounts.FirstOrDefault(i => i.ID == tax.GLID) ?? new GLAccount()
                         let td = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == tax.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                         let tds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == tax.ID).OrderBy(i => i.EffectiveFrom).ToList()
                         select new TaxGroupViewModel
                         {
                             LineID = DateTime.Now.Ticks.ToString() + tax.ID,
                             ID = tax.ID,
                             GLID = tax.GLID,
                             GlAcc = gl.Code,
                             Name = tax.Name,
                             Code = tax.Code,
                             CompanyID = tax.CompanyID,
                             Types = category.Select(c => new SelectListItem
                             {
                                 Value = c.Key,
                                 Text = c.Value,
                                 Selected = Convert.ToInt32(c.Key) == (int)tax.Type
                             }).ToList(),
                             Type = (int)tax.Type,
                             Effectivefrom = td.EffectiveFrom,
                             Rate = td.Rate,
                             Delete = false,
                             Active = tax.Active,
                             TaxGroupDefinitions = tds
                         }
                         );
            return taxes;
        }

        public dynamic GetTD(int tid)
        {
            var td = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == tid).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault();
            return td;
        }

        public void UpdateTaxgroups(List<TaxGroup> taxes)
        {
            _context.UpdateRange(taxes);
            _context.SaveChanges();
        }
    }
}
