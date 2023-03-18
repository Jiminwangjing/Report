using CKBS.AppContext;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KEDI.Core.Premise.Repository
{
    public interface IDataPropertyRepository
    {
        void DataProperty<T>(List<T> data, int comId, string itemIdProp, string propItem);
    }

    public class DataPropertyRepository : IDataPropertyRepository
    {
        private readonly DataContext _context;
        private readonly IPropertyRepository _prop;
        public DataPropertyRepository(DataContext context, IPropertyRepository propertyRepository)
        {
            _context = context;
            _prop = propertyRepository;
        }
        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data;
        }
        public void DataProperty<T>(List<T> data, int comId, string itemIdProp, string propItem)
        {
            var props = _prop.GetActivePropertiesOrdering(comId).Where(i => i.Active).ToList();
            Type type = typeof(T);
            PropertyInfo _propItem = type.GetProperty(propItem);
            if (props.Count > 0)
            {
                foreach (var item in data)
                {
                    Dictionary<string, PropertydetailsViewModel> AddictionProps = new();
                    int itemId = Convert.ToInt32(GetValue(item, itemIdProp));
                    var pds = _context.PropertyDetails.Where(i => i.ItemID == itemId).ToList();
                    foreach (var prop in props)
                    {
                        var notInProps = pds.FirstOrDefault(i => i.ProID == prop.ProID);
                        if (notInProps == null)
                        {
                            pds.Add(new PropertyDetails
                            {
                                ProID = prop.ProID,
                                ID = 0,
                                ItemID = 0,
                                Value = 0,
                            });
                        }
                    }
                    pds = pds.GroupBy(i => i.ProID).Select(i => i.FirstOrDefault()).ToList();
                    foreach (var pd in pds)
                    {
                        var chpd = _context.ChildPreoperties.FirstOrDefault(_chpd => _chpd.ID == pd.Value) ?? new ChildPreoperty();
                        var prop = props.FirstOrDefault(pr => pr.ProID == pd.ProID);
                        if (prop != null)
                        {
                            PropertydetailsViewModel _propDeView = new()
                            {
                                ID = pd.ID,
                                ValueName = chpd.Name ?? "",
                                ItemID = pd.ItemID,
                                ProID = pd.ProID,
                                Value = pd.Value
                            };
                            AddictionProps.Add(prop.NamePropNoSpace, _propDeView);
                        }
                    }
                    if (_propItem != null) _propItem.SetValue(item, Convert.ChangeType(AddictionProps, _propItem.PropertyType), null);
                }
            }
        }
    }
}
