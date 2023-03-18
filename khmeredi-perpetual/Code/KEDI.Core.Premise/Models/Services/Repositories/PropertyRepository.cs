using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.ServicesClass.Property;
using KEDI.Core.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CKBS.Models.Services.Responsitory
{
    public interface IPropertyRepository
    {
        List<Property> GetProperties(int comId);
        List<PropertyViewModel> GetActiveProperties(int comId);
        List<ChildPreoperty> GetChildrenOfProperty(int propID);
        List<PropertyViewModel> GetActivePropertiesOrdering(int comId);
        Property GetProperty(int id, int comId);
        ChildPreoperty GetLastestCCP(int PropID);
        void CreateOrUpdate(Property data);
        void CreateCPD(ChildPreoperty data);
        void UpdateCPD(string data);
        void DeletedCPD(int id);
    }
    public class PropertyRepository : IPropertyRepository
    {
        private readonly DataContext _context;

        public PropertyRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public void CreateOrUpdate(Property data)
        {
            data.Name = data.Name.Capitalize();
            _context.Property.Update(data);
            _context.SaveChanges();
        }

        public List<Property> GetProperties(int comId)
        {
            var properties = _context.Property.Where(i => i.CompanyID == comId).ToList();
            return properties;
        }

        public List<PropertyViewModel> GetActiveProperties(int comId)
        {
            var list = (from p in _context.Property.Where(i => i.CompanyID == comId && i.Active)
                            //join cp in _context.ChildPreoperties on p.ID equals 
                        let cp = _context.ChildPreoperties.Where(i => i.ProID == p.ID).OrderBy(i => i.Name).ToList()

                        select new PropertyViewModel
                        {
                            UnitID = DateTime.Now.Millisecond.ToString() + "" + p.ID,
                            ProID = p.ID,
                            NameProp = p.Name,
                            Values = cp.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = c.Name,
                                Selected = c.ProID == p.ID
                            }).ToList(),
                            Value = cp.FirstOrDefault().ID
                        }
                        ).ToList();
            return list;
        }
        public List<PropertyViewModel> GetActivePropertiesOrdering(int comId)
        {
            var list = (from p in _context.Property.Where(i => i.CompanyID == comId && i.Active)
                        select new PropertyViewModel
                        {
                            UnitID = DateTime.Now.Millisecond.ToString() + "" + p.ID,
                            ProID = p.ID,
                            NameProp = p.Name,
                            Active = p.Active
                        }).OrderBy(i => i.ProID).ToList();
            foreach (var i in list)
            {
                i.NamePropNoSpace = StringHelper.RemoveSpace(i.NameProp);
            }
            return list;
        }

        public Property GetProperty(int id, int comId)
        {
            var property = _context.Property.FirstOrDefault(i => i.CompanyID == comId && i.ID == id) ?? new Property();
            return property;
        }

        public void CreateCPD(ChildPreoperty data)
        {
            _context.ChildPreoperties.Update(data);
            _context.SaveChanges();
        }

        public List<ChildPreoperty> GetChildrenOfProperty(int propID)
        {
            var cprops = _context.ChildPreoperties.Where(i => i.ProID == propID).ToList();
            return cprops;
        }

        public void UpdateCPD(string data)
        {

            List<ChildPreoperty> childPreoperty = JsonConvert.DeserializeObject<List<ChildPreoperty>>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            _context.ChildPreoperties.UpdateRange(childPreoperty);
            _context.SaveChanges();
        }

        public ChildPreoperty GetLastestCCP(int PropID)
        {
            var _ccp = _context.ChildPreoperties.Where(i => i.ProID == PropID).OrderByDescending(i => i.ID).FirstOrDefault();
            return _ccp;
        }
        public void DeletedCPD(int id)
        {
            var delCPD = _context.ChildPreoperties.Find(id);
            _context.ChildPreoperties.Remove(delCPD);
            _context.SaveChanges();
        }
    }
}