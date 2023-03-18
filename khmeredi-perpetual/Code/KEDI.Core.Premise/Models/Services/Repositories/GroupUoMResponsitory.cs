using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CKBS.Models.Services.Responsitory
{
    public interface IGUOM
    {
        IQueryable<GroupUOM> GetGroupUOMs();
        Task<int> AddorEditGUOM(GroupUOM groupUOM);
        GroupUOM getid(int id);
        Task<int> DleteGUoM(int ID);
        IQueryable<GroupDUoM> GetGroupDUoMs(int ID);
        Task<int> InsertDGroupUOM(GroupDUoM groupDUoM);
        Task<int> DeleteDefinedGroup(int ID);
        IEnumerable<GroupDUoM> GetAllgroupDUoMs();
        Task<List<SelectListItem>> SelectGroupDefinedUoMsAsync(int groupUomID = 0);
        Task<List<SelectListItem>> SelectDefinedUoMsAsync(int groupUomID = 0);
    }

    public class GroupUoMResponsitory:IGUOM
    {
        private readonly DataContext _context;
        public GroupUoMResponsitory(DataContext dataCotext)
        {
            _context = dataCotext;
        }

        public async Task<List<SelectListItem>> SelectGroupDefinedUoMsAsync(int groupUomID = 0)
        {
            var groupUoMs = from guom in _context.GroupUOMs.Where(g => !g.Delete)
                            join gduom in _context.GroupDUoMs.Where(g => !g.Delete) on guom.ID equals gduom.GroupUoMID
                            group new {guom,gduom} by new {guom.ID} into g
                            let data=g.FirstOrDefault()
                            select new SelectListItem
                            {
                                Value = data.guom.ID.ToString(),
                                Text = data.guom.Name,
                                Selected = data.guom.ID == groupUomID
                            };
            return await groupUoMs.ToListAsync();
        }

        public async Task<List<SelectListItem>> SelectDefinedUoMsAsync(int groupUomID = 0)
        {
            var definedUoMs = GetGroupDUoMs(groupUomID);
            return await definedUoMs.Select(g => 
            new SelectListItem { 
                Value = g.AltUOM.ToString(),
                Text = g.UnitofMeasure.AltUomName,
                Selected = g.AltUOM == groupUomID               
            }).ToListAsync();
        }

        public async Task<int> AddorEditGUOM(GroupUOM groupUOM)
        {
            if (groupUOM.ID == 0)
            {
                await _context.GroupUOMs.AddAsync(groupUOM);
            }
            else
            {
              
                _context.GroupUOMs.Update(groupUOM);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteDefinedGroup(int ID)
        {
            var dg = await _context.GroupDUoMs.FirstAsync(d => d.ID ==ID);
            dg.Delete = true;
            _context.GroupDUoMs.Update(dg);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DleteGUoM(int ID)
        {
            var Guom = await _context.GroupUOMs.FirstAsync(g => g.ID ==ID);
            Guom.Delete = true;
            _context.GroupUOMs.Update(Guom);
            return await _context.SaveChangesAsync();
        }

        public IEnumerable<GroupDUoM> GetAllgroupDUoMs()
        {
            var uom = _context.UnitofMeasures.Where(u => u.Delete == false);
            var guom = _context.GroupUOMs.Where(g => g.Delete == false);
            var duom = _context.GroupDUoMs.Where(o => o.Delete == false);
            var list = (from du in duom
                        join g_uom in guom on du.GroupUoMID equals g_uom.ID
                        join  buo in uom on du.BaseUOM equals buo.ID
                        join auo in uom on du.AltUOM equals auo.ID
                        where g_uom.Delete==false
                        select new GroupDUoM
                        {
                            ID = du.ID,
                            GroupUoMID = du.GroupUoMID,
                            UoMID = du.UoMID,
                            AltQty = du.AltQty,
                            BaseUOM = du.BaseUOM,
                            AltUOM = du.AltUOM,
                            BaseQty = du.BaseQty,
                            Factor = du.Factor,

                            UnitofMeasure = new UnitofMeasure
                            {
                                ID=auo.ID,
                                Name = buo.Name,
                                AltUomName = auo.Name
                            },

                        }
                );
            return list;
        }

        public IQueryable<GroupDUoM> GetGroupDUoMs(int ID)
        {
            var uom = _context.UnitofMeasures.Where(u => u.Delete == false);
            var guom = _context.GroupUOMs.Where(g => g.Delete == false);
            var duom =  _context.GroupDUoMs.Where(o => o.Delete == false);
            var groupDUoMs = (from du in duom
                        join buo  in uom on du.BaseUOM equals buo.ID
                        join auo in uom on du.AltUOM equals auo.ID                      
                        where du.GroupUoMID == ID && du.Delete == false
                        select new GroupDUoM
                        {
                            ID = du.ID,
                            GroupUoMID = du.GroupUoMID,
                            UoMID = du.UoMID,
                            AltQty = du.AltQty,
                            BaseUOM = du.BaseUOM,
                            AltUOM = du.AltUOM,
                            BaseQty = du.BaseQty,
                            Factor = du.Factor,
                      
                            UnitofMeasure = new UnitofMeasure
                            {                              
                                Name = buo.Name,
                                AltUomName = auo.Name
                            },
                            
                        }
                );
            return groupDUoMs;
        }

        public IQueryable<GroupUOM> GetGroupUOMs()
        {
            IQueryable<GroupUOM> list = _context.GroupUOMs.Where(g => g.Delete == false);
            return list;
        }

        public GroupUOM getid(int id) => _context.GroupUOMs.Find(id);

        public async Task<int> InsertDGroupUOM(GroupDUoM groupDUoM)
        {
            if (groupDUoM.ID == 0)
           {
                var items = _context.ItemMasterDatas.Where(i => i.GroupUomID == groupDUoM.GroupUoMID).ToList();
                if (items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        var pld = _context.PriceListDetails.FirstOrDefault(i => i.ItemID == item.ID);
                        if (pld != null)
                        {
                            var newPld = pld.DeepCopy();
                            newPld.ID = 0;
                            newPld.UomID = groupDUoM.AltUOM;
                            newPld.UnitPrice = 0;
                            newPld.Cost = 0;
                            await _context.PriceListDetails.AddAsync(newPld);
                        }
                    }
                }
                await _context.GroupDUoMs.AddAsync(groupDUoM);
            }
            else
            {
                             
                var dg = await _context.GroupDUoMs.FirstAsync(d => d.ID == groupDUoM.ID);
                dg.AltQty = groupDUoM.AltQty;
                dg.AltUOM = groupDUoM.AltUOM;
                dg.BaseQty = groupDUoM.BaseQty;
                dg.UoMID = groupDUoM.UoMID;
                dg.GroupUoMID = groupDUoM.GroupUoMID;
                dg.Factor = groupDUoM.Factor;
                _context.GroupDUoMs.Update(dg);

            }
            return await _context.SaveChangesAsync();
        }
    }
    public static class ExtensionMethods
    {
        public static T DeepCopy<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
